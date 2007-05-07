using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rainbow
{
	namespace Scheduler
	{
		//Author: Federico Dal Maso
		//e-mail: ifof@libero.it
		//date: 2003-06-17
		/// <summary>
		/// Summary description for SchedulerDB.
		/// </summary>
		internal class SchedulerDB
		{
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			private string _appMapPath;

			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			private IDbConnection _cn;

			/// <summary>
			///     
			/// </summary>
			/// <param name="cn" type="System.Data.IDbConnection">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="applicationMapPath" type="string">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			public SchedulerDB(IDbConnection cn, string applicationMapPath)
			{
				_cn = cn;
				_appMapPath = applicationMapPath; //should be: HttpContext.Current.Server.MapPath(PortalSettings.ApplicationPath)
			}

			/// <summary>
			///     
			/// </summary>
			/// 
			/// <returns>
			///     A Rainbow.Scheduler.SchedulerTask[] value...
			/// </returns>
			public SchedulerTask[] GetExpiredTask()
			{
				ArrayList ary = new ArrayList();

				_cn.Open();
				using (IDbCommand cmd = _cn.CreateCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "rb_SchedulerGetExpiredTasks";

					using (IDataReader dr = cmd.ExecuteReader())
					{
						while (dr.Read())
							ary.Add(new SchedulerTask(dr));
						dr.Close();
					}
				}
				_cn.Close();
				return (SchedulerTask[]) ary.ToArray(typeof (SchedulerTask));
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="idModule" type="int">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A Rainbow.Scheduler.ISchedulable value...
			/// </returns>
			public ISchedulable GetModuleInstance(int idModule)
			{
				_cn.Open();
				IDbCommand cmd = _cn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "rb_SchedulerGetModuleClassName";
				IDataParameter par;
				par = cmd.CreateParameter();
				par.DbType = DbType.Int32;
				par.ParameterName = "@IDModule";
				par.Value = idModule;
				cmd.Parameters.Add(par);
				IDataReader dr;

				try
				{
					dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}

				catch (Exception ex)
				{
					_cn.Close();
					throw new SchedulerException("Unable to load assembly name from database", ex);
				}

				catch
				{
					_cn.Close();
					throw;
				}
				string assemblyName;
				string typeName;

				using (dr)
				{
					if (dr.Read())
					{
						assemblyName = string.Concat(_appMapPath, @"\bin\",
						                             (string) dr["AssemblyName"]);
						typeName = (string) dr["ClassName"];
					}

					else
						throw new SchedulerException("Not assembly in database");
					dr.Close(); //_cn is closed by behavior.
				}
				Assembly a;

				try
				{
					a = Assembly.LoadFrom(assemblyName);
				}

				catch (Exception ex)
				{
					throw new SchedulerException("Cannot load assembly", ex);
				}
				object o;

				try
				{
					o = a.CreateInstance(typeName);
				}

				catch (Exception ex)
				{
					throw new SchedulerException(
						string.Format("Unable to create instance of assembly: {0} , typeName: {1}",
						              assemblyName,
						              typeName),
						ex);
				}

				catch
				{
					throw;
				}
				ISchedulable module = o as ISchedulable;

				if (module == null)
					throw new SchedulerException("Module don't implement ISchedulable interface");

				else
					return module;
			}

			/// <summary>
			///     
			/// </summary>
			/// 
			/// <returns>
			///     A System.Data.IDataReader value...
			/// </returns>
			public IDataReader GetOrderedTask()
			{
				_cn.Open();
				IDbCommand cmd = _cn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "rb_SchedulerGetOrderedTasks";
				return cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="idOwner" type="int">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A System.Data.IDataReader value...
			/// </returns>
			public IDataReader GetTasksByOwner(int idOwner)
			{
				_cn.Open();
				IDbCommand cmd = _cn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "rb_SchedulerGetTasksByOwner";
				IDbDataParameter par;
				par = cmd.CreateParameter();
				par.ParameterName = "@IDOwner";
				par.DbType = DbType.Int32;
				par.Direction = ParameterDirection.Input;
				par.Value = idOwner;
				cmd.Parameters.Add(par);
				IDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				return dr;
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="idTarget" type="int">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A System.Data.IDataReader value...
			/// </returns>
			public IDataReader GetTasksByTarget(int idTarget)
			{
				_cn.Open();
				IDbCommand cmd = _cn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "rb_SchedulerGetTasksByTarget";
				IDbDataParameter par;
				par = cmd.CreateParameter();
				par.ParameterName = "@IDTarget";
				par.DbType = DbType.Int32;
				par.Direction = ParameterDirection.Input;
				par.Value = idTarget;
				cmd.Parameters.Add(par);
				IDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				return dr;
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="task" type="Rainbow.Scheduler.SchedulerTask">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A int value...
			/// </returns>
			public int InsertTask(SchedulerTask task)
			{
				if (task.DueTime < DateTime.Now)
					throw new SchedulerException("Cannot schedule an expired task");
				byte[] arg;

				using (MemoryStream ss = new MemoryStream())
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(ss, task.Argument);
					arg = ss.ToArray();
					ss.Close();
				}
				int idTask;
				_cn.Open();

				using (IDbCommand cmd = _cn.CreateCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "rb_SchedulerAddTask";
					IDbDataParameter par;
					par = cmd.CreateParameter();
					par.ParameterName = "@IDOwner";
					par.DbType = DbType.Int32;
					par.Direction = ParameterDirection.Input;
					par.Value = task.IDModuleOwner;
					cmd.Parameters.Add(par);
					par = cmd.CreateParameter();
					par.ParameterName = "@IDTarget";
					par.DbType = DbType.Int32;
					par.Direction = ParameterDirection.Input;
					par.Value = task.IDModuleTarget;
					cmd.Parameters.Add(par);
					par = cmd.CreateParameter();
					par.ParameterName = "@DueTime";
					par.DbType = DbType.DateTime;
					par.Direction = ParameterDirection.Input;
					par.Value = task.DueTime;
					cmd.Parameters.Add(par);
					par = cmd.CreateParameter();
					par.ParameterName = "@Description";
					par.DbType = DbType.String;
					par.Size = 150;
					par.Direction = ParameterDirection.Input;
					par.Value = task.Description;
					cmd.Parameters.Add(par);
					par = cmd.CreateParameter();
					par.ParameterName = "@Argument";
					//par.DbType = DbType;
					par.Direction = ParameterDirection.Input;
					par.Value = arg;
					cmd.Parameters.Add(par);
					par = cmd.CreateParameter();
					par.ParameterName = "@IDTask";
					par.DbType = DbType.Int32;
					par.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(par);
					cmd.ExecuteNonQuery();
					idTask = (int) ((IDataParameter) cmd.Parameters["@IDTask"]).Value;
				}
				_cn.Close();

				if (idTask == -1)
					throw new SchedulerException("Task add fail in DB");
				return idTask;
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="IDTask" type="int">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			public void RemoveTask(int iDTask)
			{
				_cn.Open();

				using (IDbCommand cmd = _cn.CreateCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "rb_SchedulerRemoveTask";
					IDbDataParameter par;
					par = cmd.CreateParameter();
					par.ParameterName = "@IDTask";
					par.DbType = DbType.Int32;
					par.Direction = ParameterDirection.Input;
					par.Value = iDTask;
					cmd.Parameters.Add(par);
					cmd.ExecuteNonQuery();
				}
				_cn.Close();
			}
		}
	}
}