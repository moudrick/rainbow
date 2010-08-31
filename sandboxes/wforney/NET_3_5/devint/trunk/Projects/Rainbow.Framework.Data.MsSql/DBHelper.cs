namespace Rainbow.Framework.Data
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data.MsSql;
    using Rainbow.Framework.Data.MsSql.Debugger;
    using Rainbow.Framework.Exceptions;

    /// <summary>
    /// Summary description for DBHelper
    /// </summary>
    public class DBHelper
    {
        #region Public Methods

        /// <summary>
        /// Exes the SQL.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <returns>
        /// A int value...
        /// </returns>
        public static int ExeSQL(string sql)
        {
            int returnValue;

            using (var db = new DataClassesDataContext(Config.ConnectionString))
            {
                db.Log = new DebuggerWriter();

                try
                {
                    returnValue = db.ExecuteCommand(sql);
                }
                catch (Exception e)
                {
                    ErrorHandler.Publish(LogLevels.Error, string.Format("Error in DBHelper - ExeSQL - SQL: '{0}'", sql), e);
                    throw new DatabaseUnreachableException("Error in DBHelper - ExeSQL", e);

                    // throw new Exception("Error in DBHelper:ExeSQL()-> " + e.ToString());
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the SQL scalar.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <returns>
        /// A object value...
        /// </returns>
        public static object ExecuteSQLScalar(string sql)
        {
            object returnValue;
            using (var db = new DataClassesDataContext(Config.ConnectionString))
            {
                db.Log = new DebuggerWriter();

                using (var sqlCommand = new SqlCommand(sql, (SqlConnection)db.Connection))
                {
                    try
                    {
                        sqlCommand.Connection.Open();
                        returnValue = sqlCommand.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseUnreachableException("Error in DBHelper - ExecuteSQLScalar", e);
                    }
                    finally
                    {
                        sqlCommand.Dispose();
                        db.Connection.Close();
                        db.Connection.Dispose();
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Execute script using transaction
        /// </summary>
        /// <param name="scriptPath">
        /// The script path.
        /// </param>
        /// <param name="useTransaction">
        /// if set to <c>true</c> [use transaction].
        /// </param>
        /// <returns>
        /// </returns>
        public static ArrayList ExecuteScript(string scriptPath, bool useTransaction)
        {
            var db = new DataClassesDataContext(Config.ConnectionString) { Log = new DebuggerWriter() };

            return ExecuteScript(scriptPath, db.Connection, useTransaction);
        }

        /// <summary>
        /// Execute script (no transaction)
        /// </summary>
        /// <param name="scriptPath">
        /// The script path.
        /// </param>
        /// <returns>
        /// </returns>
        public static ArrayList ExecuteScript(string scriptPath)
        {
            var db = new DataClassesDataContext(Config.ConnectionString) { Log = new DebuggerWriter() };

            return ExecuteScript(scriptPath, db.Connection);
        }

        /// <summary>
        /// Execute script using transaction
        /// </summary>
        /// <param name="scriptPath">
        /// The script path.
        /// </param>
        /// <param name="myConnection">
        /// My connection.
        /// </param>
        /// <param name="useTransaction">
        /// if set to <c>true</c> [use transaction].
        /// </param>
        /// <returns>
        /// </returns>
        public static ArrayList ExecuteScript(string scriptPath, DbConnection myConnection, bool useTransaction)
        {
            if (!useTransaction)
            {
                return ExecuteScript(scriptPath, myConnection); // FIX: Must pass connection as well
            }

            var strScript = GetScript(scriptPath);
            ErrorHandler.Publish(LogLevels.Info, string.Format("Executing Script '{0}'", scriptPath));
            var errors = new ArrayList();

            // Subdivide script based on GO keyword
            var sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);

            // Open connection
            myConnection.Open();

            // Wraps execution on a transaction 
            // so we know that the script runs or fails
            using (var transaction = myConnection.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                ErrorHandler.Publish(LogLevels.Debug, "Start Script Transaction ");

                try
                {
                    // Cycles command and execute them
                    for (var s = 0; s <= sqlCommands.GetUpperBound(0); s++)
                    {
                        var mySqlText = sqlCommands[s].Trim();

                        try
                        {
                            if (mySqlText.Length > 0)
                            {
                                // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Configuration.LogLevel.Debug, "Executing: " + mySqlText.Replace("\n", " "));
                                // Must assign both transaction object and connection
                                // to Command object for a pending local transaction
                                using (var sqldbCommand = new SqlCommand())
                                {
                                    sqldbCommand.Connection = (SqlConnection)myConnection;
                                    sqldbCommand.CommandType = CommandType.Text;
                                    sqldbCommand.Transaction = (SqlTransaction)transaction;
                                    sqldbCommand.CommandText = mySqlText;
                                    sqldbCommand.CommandTimeout = 150;
                                    sqldbCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            errors.Add(string.Format("<P>{0}<br>{1}</P>", ex.Message, mySqlText));
                            ErrorHandler.Publish(LogLevels.Warn, string.Format("ExecuteScript Failed: {0}", mySqlText), ex);
                            throw new DatabaseUnreachableException(string.Format("ExecuteScript Failed: {0}", mySqlText), ex);
                        }
                    }

                    // Succesfully applied this script
                    transaction.Commit();
                    ErrorHandler.Publish(LogLevels.Debug, "Commit Script Transaction.");
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                    var count = 0;

                    while (ex.InnerException != null && count < 100)
                    {
                        ex = ex.InnerException;
                        errors.Add(ex.Message);
                        count++;
                    }
                }
                finally
                {
                    if (myConnection.State == ConnectionState.Open)
                    {
                        myConnection.Close();
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Execute script (no transaction)
        /// </summary>
        /// <param name="scriptPath">
        /// The script path.
        /// </param>
        /// <param name="myConnection">
        /// My connection.
        /// </param>
        /// <returns>
        /// </returns>
        public static ArrayList ExecuteScript(string scriptPath, DbConnection myConnection)
        {
            var strScript = GetScript(scriptPath);
            ErrorHandler.Publish(LogLevels.Info, string.Format("Executing Script '{0}'", scriptPath));
            var errors = new ArrayList();

            // Subdivide script based on GO keyword
            var sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);

            try
            {
                // Cycles command and execute them
                for (var s = 0; s <= sqlCommands.GetUpperBound(0); s++)
                {
                    var mySqlText = sqlCommands[s].Trim();

                    try
                    {
                        if (mySqlText.Length > 0)
                        {
                            // Open connection
                            myConnection.Open();

                            ErrorHandler.Publish(LogLevels.Debug, "Executing: " + mySqlText.Replace("\n", " "));
                            using (var sqldbCommand = new SqlCommand())
                            {
                                sqldbCommand.Connection = (SqlConnection)myConnection;
                                sqldbCommand.CommandType = CommandType.Text;
                                sqldbCommand.CommandText = mySqlText;
                                sqldbCommand.CommandTimeout = 150;
                                sqldbCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(string.Format("<P>{0}<br>{1}</P>", ex.Message, mySqlText));
                        ErrorHandler.Publish(LogLevels.Warn, string.Format("ExecuteScript Failed: {0}", mySqlText), ex);

                        // Rethrow exception
                        throw new RainbowException(
                            LogLevels.Fatal,
                            HttpStatusCode.ServiceUnavailable,
                            string.Format("Script failed, please correct the error and retry: {0}", mySqlText),
                            ex);

                        // throw new Exception("Script failed, please correct the error and retry", ex);
                    }
                    finally
                    {
                        if (myConnection.State == ConnectionState.Open)
                        {
                            myConnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                var count = 0;

                while (ex.InnerException != null && count < 100)
                {
                    ex = ex.InnerException;
                    errors.Add(ex.Message);
                    count++;
                }
            }

            return errors;
        }

        /// <summary>
        /// Gets the data reader.
        /// </summary>
        /// <param name="selectCmd">
        /// The select CMD.
        /// </param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        public static SqlDataReader GetDataReader(string selectCmd)
        {
            var db = new DataClassesDataContext(Config.ConnectionString) { Log = new DebuggerWriter() };
            var myConnection = db.Connection;

            using (var sqlCommand = new SqlCommand(selectCmd, (SqlConnection)myConnection))
            {
                try
                {
                    sqlCommand.Connection.Open();
                }
                catch (Exception e)
                {
                    throw new DatabaseUnreachableException("Error in DBHelper - GetDataReader", e);

                    // throw new Exception("Error in DBHelper::GetDataReader()-> " + e.ToString());
                }

                return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="selectCmd">
        /// The select CMD.
        /// </param>
        /// <returns>
        /// A System.Data.DataSet value...
        /// </returns>
        public static DataSet GetDataSet(string selectCmd)
        {
            DataSet ds;
            var db = new DataClassesDataContext(Config.ConnectionString) { Log = new DebuggerWriter() };

            using (var connection = db.Connection)
            using (var sqlDataAdapter = new SqlDataAdapter(selectCmd, (SqlConnection)connection))
            {
                try
                {
                    ds = new DataSet();
                    sqlDataAdapter.Fill(ds, "Table0");
                }
                catch (Exception e)
                {
                    throw new DatabaseUnreachableException("Error in DBHelper - GetDataSet", e);

                    // throw new Exception("Error in ItemBase:GetDataSet()-> " + e.ToString());
                }
                finally
                {
                    sqlDataAdapter.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
            }

            return ds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the script from a file
        /// </summary>
        /// <param name="scriptPath">
        /// The script path.
        /// </param>
        /// <returns>
        /// The get script.
        /// </returns>
        private static string GetScript(string scriptPath)
        {
            string strScript;

            // Load script file 
            // using (System.IO.StreamReader objStreamReader = System.IO.File.OpenText(scriptPath)) 
            // http://support.rainbowportal.net/jira/browse/RBP-693
            // to make it possible to have german umlauts or other special characters in the install_scripts
            using (var objStreamReader = new StreamReader(scriptPath, Encoding.Default))
            {
                strScript = objStreamReader.ReadToEnd();
                objStreamReader.Close();
            }

            return strScript + Environment.NewLine; // Append carriage for execute last command 
        }

        #endregion
    }
}