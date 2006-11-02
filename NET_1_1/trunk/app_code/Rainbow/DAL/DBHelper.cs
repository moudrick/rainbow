using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Rainbow.Configuration;
using Rainbow.Settings;

namespace Rainbow.Helpers
{

	/// <summary>
	/// Summary description for DBHelper
	/// </summary>
	public class DBHelper
	{
		/// <summary>
		/// Execute script using transaction
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <param name="useTransaction"></param>
		/// <returns></returns>
		static public ArrayList ExecuteScript(string scriptPath, bool useTransaction)
		{
			return ExecuteScript(scriptPath, Config.SqlConnectionString, useTransaction);
		}
		/// <summary>
		/// Execute script (no transaction)
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <returns></returns>
		static public ArrayList ExecuteScript(string scriptPath)
		{
			return ExecuteScript(scriptPath, Config.SqlConnectionString);
		}
		/// <summary>
		/// Execute script using transaction
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <param name="myConnection"></param>
		/// <param name="useTransaction"></param>
		/// <returns></returns>
		static public ArrayList ExecuteScript(string scriptPath, SqlConnection  myConnection, bool useTransaction)
		{

			if (!useTransaction)
				return ExecuteScript(scriptPath, myConnection); //FIX: Must pass connection as well
			string strScript = GetScript(scriptPath);
			LogHelper.Logger.Log(LogLevel.Info, "Executing Script '" + scriptPath + "'");
			ArrayList errors = new ArrayList();
			// Subdivide script based on GO keyword
			string[] sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);
			//Open connection
			myConnection.Open();
			//Wraps execution on a transaction 
			//so we know that the script runs or fails
			SqlTransaction myTrans;
			string transactionName = "Rainbow";
			myTrans = myConnection.BeginTransaction(IsolationLevel.RepeatableRead, transactionName);
			LogHelper.Logger.Log(LogLevel.Debug, "Start Script Transaction ");

			try
			{

				//Cycles command and execute them
				for (int s = 0; s <= sqlCommands.GetUpperBound(0); s++)
				{
					string mySqlText = sqlCommands[s].Trim();

					try 
					{

						if (mySqlText.Length > 0)
						{

							//Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Debug, "Executing: " + mySqlText.Replace("\n", " "));
							// Must assign both transaction object and connection
							// to Command object for a pending local transaction
							using (SqlCommand  sqldbCommand = new SqlCommand()) 
							{
								sqldbCommand.Connection = myConnection;
								sqldbCommand.CommandType = CommandType.Text;
								sqldbCommand.Transaction = myTrans;
								sqldbCommand.CommandText = mySqlText;
								sqldbCommand.CommandTimeout = 150;
								sqldbCommand.ExecuteNonQuery();
							}
						}
					} 

					catch(Exception ex) 
					{
						myTrans.Rollback();
						errors.Add("<P>" 
							+ ex.Message + "<br>"
							+ mySqlText 
							+ "</P>");
						//LogHelper.Logger.Log(LogLevel.Warn, "ExecuteScript Failed: " + mySqlText, ex);
						throw new DatabaseUnreachableException("ExecuteScript Failed: " + mySqlText,ex);
					}
				}
				// Succesfully applied this script
				myTrans.Commit();
				LogHelper.Logger.Log(LogLevel.Debug, "Commit Script Transaction.");
			}

			catch(Exception ex) 
			{
				errors.Add(ex.Message);
				int count = 0;

				while(ex.InnerException != null && count < 100)
				{
					ex = ex.InnerException;
					errors.Add(ex.Message);
					count++;
				}
			}

			finally
			{

				if (myConnection.State == ConnectionState.Open)
					myConnection.Close();
			}
			return errors;
		}
		/// <summary>
		/// Execute script (no transaction)
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <param name="myConnection"></param>
		/// <returns></returns>
		static public ArrayList ExecuteScript(string scriptPath, SqlConnection  myConnection)
		{
			string strScript = GetScript(scriptPath);
			LogHelper.Logger.Log(LogLevel.Info, "Executing Script '" + scriptPath + "'");
			ArrayList errors = new ArrayList();
			// Subdivide script based on GO keyword
			string[] sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);

			try
			{

				//Cycles command and execute them
				for (int s = 0; s <= sqlCommands.GetUpperBound(0); s++)
				{
					string mySqlText = sqlCommands[s].Trim();

					try 
					{

						if (mySqlText.Length > 0)
						{
							//Open connection
							myConnection.Open();

							//Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Debug, "Executing: " + mySqlText.Replace("\n", " "));
							using (SqlCommand  sqldbCommand = new SqlCommand()) 
							{
								sqldbCommand.Connection = myConnection;
								sqldbCommand.CommandType = CommandType.Text;
								sqldbCommand.CommandText = mySqlText;
								sqldbCommand.CommandTimeout = 150;
								sqldbCommand.ExecuteNonQuery();
							}
						}
					} 

					catch(Exception ex) 
					{
						errors.Add("<P>" 
							+ ex.Message + "<br>"
							+ mySqlText 
							+ "</P>");
						//LogHelper.Logger.Log(LogLevel.Warn, "ExecuteScript Failed: " + mySqlText, ex);
						// Rethrow exception
						throw new RainbowException(LogLevel.Fatal,HttpStatusCode.ServiceUnavailable,"Script failed, please correct the error and retry: "  + mySqlText,ex);
						//throw new Exception("Script failed, please correct the error and retry", ex);
					}

					finally
					{

						if (myConnection.State == ConnectionState.Open)
							myConnection.Close();
					}
				}
			}

			catch(Exception ex) 
			{
				errors.Add(ex.Message);
				int count = 0;

				while(ex.InnerException != null && count < 100)
				{
					ex = ex.InnerException;
					errors.Add(ex.Message);
					count++;
				}
			}
			return errors;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="sql" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A object value...
		/// </returns>
		static public object ExecuteSQLScalar(string sql)
		{
			object returnValue;

			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{

				using (SqlCommand sqlCommand = new SqlCommand(sql, myConnection))
				{

					try
					{ 
						sqlCommand.Connection.Open();
						returnValue = sqlCommand.ExecuteScalar();
					}

					catch (Exception e)
					{
						throw new DatabaseUnreachableException("Error in DBHelper - ExecuteSQLScalar",e);
					}

					finally
					{ 
						sqlCommand.Dispose();
						myConnection.Close();
						myConnection.Dispose();
					}
				}
			}
			return returnValue;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="sql" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		static public Int32 ExeSQL(string sql)
		{
			int returnValue = -1;

			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{

				using (SqlCommand sqlCommand = new SqlCommand(sql, myConnection)) 
				{

					try
					{ 
						sqlCommand.Connection.Open();
						returnValue = sqlCommand.ExecuteNonQuery();
					}

					catch (Exception e)
					{
						throw new DatabaseUnreachableException("Error in DBHelper - ExeSQL",e);
						//throw new Exception("Error in DBHelper:ExeSQL()-> " + e.ToString());
					}

					finally
					{ 
						sqlCommand.Dispose();
						myConnection.Close();
						myConnection.Dispose();
					}
				}
			}
			return returnValue;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="selectCmd" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		static public SqlDataReader GetDataReader(string selectCmd)
		{
			SqlConnection myConnection = Config.SqlConnectionString;

			using (SqlCommand sqlCommand = new SqlCommand(selectCmd, myConnection)) 
			{

				try 
				{
					sqlCommand.Connection.Open();
				}

				catch (Exception e)
				{
					throw new DatabaseUnreachableException("Error in DBHelper - GetDataReader",e);
					//throw new Exception("Error in DBHelper::GetDataReader()-> " + e.ToString());
				}
				return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection); 
			}
		} 
		/// <summary>
		///     
		/// </summary>
		/// <param name="selectCmd" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.DataSet value...
		/// </returns>
		static public DataSet GetDataSet(string selectCmd) 
		{
			DataSet ds = null;

			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{

				using (SqlDataAdapter m_SqlDataAdapter = new SqlDataAdapter(selectCmd, myConnection)) 
				{

					try 
					{
						ds = new DataSet();
						m_SqlDataAdapter.Fill(ds,"Table0");
					}

					catch (Exception e)
					{
						throw new DatabaseUnreachableException("Error in DBHelper - GetDataSet",e);
						//throw new Exception("Error in ItemBase:GetDataSet()-> " + e.ToString());
					}

					finally
					{ 
						m_SqlDataAdapter.Dispose();
						myConnection.Close();
						myConnection.Dispose();
					}
				}
			}
			return ds; 
		}

		/// <summary>
		/// Get the script from a file
		/// </summary>
		/// <returns></returns>
		private static string GetScript(string scriptPath) 
		{ 
			string strScript = string.Empty; 

			// Load script file 
			//using (System.IO.StreamReader objStreamReader = System.IO.File.OpenText(scriptPath)) 
			//http://support.rainbowportal.net/jira/browse/RBP-693
			//to make it possible to have german umlauts or other special characters in the install_scripts
			using (System.IO.StreamReader objStreamReader = new System.IO.StreamReader(scriptPath, System.Text.Encoding.Default)) 
			{ 
				strScript = objStreamReader.ReadToEnd(); 
				objStreamReader.Close(); 
			} 
			return strScript + Environment.NewLine; //Append carriage for execute last command 
		} 

	}
}
