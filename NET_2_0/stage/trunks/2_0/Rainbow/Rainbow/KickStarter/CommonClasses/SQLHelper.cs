
// ===============================================================================
// 
// This code has been generated by KickStarter :- http://kickstarter.net
// 
// Version :- 2.0.24.14705
// Date    :- 16/09/2003
// Time    :- 14.32
// 
// ===============================================================================
// 
// Copyright (C) 2002 - 2003 KickStarter
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// 
// ===============================================================================
using System.Data;
using System.Data.SqlClient;

namespace Rainbow.KickStarter.CommonClasses 
{

	/// <summary>
	///     
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	public class SqlHelper 
	{

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private SqlHelper() 
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="connectionString" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sqlCommand" type="System.Data.SqlClient.SqlCommand">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.DataSet value...
		/// </returns>
		public static DataSet ExecuteDataSet(string connectionString, SqlCommand sqlCommand) 
		{

			using (SqlConnection sqlConnection = new SqlConnection(connectionString)) 
			{

				try 
				{
					sqlConnection.Open();
					sqlCommand.Connection = sqlConnection;

					using (DataSet dataSet = new DataSet())
					{

						using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand)) 
						{
							sqlDataAdapter.Fill(dataSet);
							sqlCommand.Dispose(); //by Manu fix close bug #2
							sqlConnection.Close(); //by Manu fix close bug #2
							sqlConnection.Dispose(); //by Manu fix close bug #2
						}
						return dataSet;
					}
				}

				catch 
				{
					throw;
				}

				finally 
				{
					sqlConnection.Close();
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="connectionString" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sqlCommand" type="System.Data.SqlClient.SqlCommand">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		public static int ExecuteNonQuery(string connectionString, SqlCommand sqlCommand) 
		{
			return ExecuteNonQuery(null, connectionString, sqlCommand);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="sqlTransaction" type="System.Data.SqlClient.SqlTransaction">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="connectionString" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sqlCommand" type="System.Data.SqlClient.SqlCommand">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		public static int ExecuteNonQuery(SqlTransaction sqlTransaction, string connectionString, SqlCommand sqlCommand) 
		{

			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{

				try 
				{

					if (sqlTransaction != null) 
					{
						sqlCommand.Transaction = sqlTransaction;
						sqlCommand.Connection = sqlTransaction.Connection;
					}

					else 
					{
						sqlConnection.Open();
						sqlCommand.Connection = sqlConnection;
					}
					return sqlCommand.ExecuteNonQuery();
				}

				catch 
				{
					throw;
				}

				finally 
				{

					if (sqlConnection.State == ConnectionState.Open) 
					{
						sqlConnection.Close();
					}
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="connectionString" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sqlCommand" type="System.Data.SqlClient.SqlCommand">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		public static SqlDataReader ExecuteReader(string connectionString, SqlCommand sqlCommand) 
		{

			using (SqlConnection sqlConnection = new SqlConnection(connectionString)) 
			{

				try 
				{
					sqlConnection.Open();
					sqlCommand.Connection = sqlConnection;
					return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				}

				catch 
				{
					throw;
				}
			}
		}
	}
}
