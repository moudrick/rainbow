using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Security;
using Rainbow.Configuration;
using Rainbow.Settings;

namespace Rainbow.Helpers
{
	/// <summary>
	/// Summary description for CryptoHelper.
	/// </summary>
	public class CryptoHelper
	{
		public CryptoHelper()
		{
		}
		public string CreateSalt(int size) 
		{
			// generate cryptographic rand numb.
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];
			rng.GetBytes(buff);
			// return base64 string representation of random number
			return Convert.ToBase64String(buff);
		}

		public string CreatePasswordHash(string pwd, string salt) 
		{
			string saltAndPwd = String.Concat(pwd, salt);
			string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "SHA1");
			return hashedPwd;
		}
		public void HashPasswords() 
		{
			string salt;
			//string password;
			SqlParameter parameterPassword;
			SqlParameter parameterSalt;
			SqlParameter parameterEmail;
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				
				SqlDataAdapter adapter = new SqlDataAdapter("SELECT Email, Password FROM rb_Users", myConnection);
				SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
				DataSet ds = new DataSet();
				myConnection.Open();
				adapter.Fill(ds, "rb_Users");
				foreach(DataRow dr in ds.Tables["rb_Users"].Rows)
				{
					using (SqlCommand insertCommand = new SqlCommand("rb_HashPasswords", myConnection))
					{
						insertCommand.CommandType = CommandType.StoredProcedure;
					
						salt = CreateSalt(5);	
						parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
						parameterEmail.Value = dr["Email"];
						insertCommand.Parameters.Add(parameterEmail);
						parameterPassword = new SqlParameter("@Password", SqlDbType.VarChar, 40);
						parameterPassword.Value = CreatePasswordHash(dr["Password"].ToString(), salt);
						insertCommand.Parameters.Add(parameterPassword);
						parameterSalt = new SqlParameter("@Salt", SqlDbType.VarChar, 10);
						parameterSalt.Value = salt;
						insertCommand.Parameters.Add(parameterSalt);

						insertCommand.ExecuteNonQuery();
					}
				}
			
			}
		}
		// update db with random generated password
		public void ResetPassword(string email, string randomPassword)
		{
			string salt;
			SqlParameter parameterPassword;
			SqlParameter parameterSalt;
			SqlParameter parameterEmail;
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{	
				using (SqlCommand insertCommand= new SqlCommand("rb_HashPasswords", myConnection))
				{
					// Mark the Command as a SPROC
					insertCommand.CommandType = CommandType.StoredProcedure;
					// Open the database connection and execute the command
					myConnection.Open();

					salt = CreateSalt(5);
					parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					insertCommand.Parameters.Add(parameterEmail);
					parameterPassword = new SqlParameter("@Password", SqlDbType.VarChar, 40);
					parameterPassword.Value = CreatePasswordHash(randomPassword, salt);
					insertCommand.Parameters.Add(parameterPassword);
					parameterSalt = new SqlParameter("@Salt", SqlDbType.VarChar, 10);
					parameterSalt.Value = salt;
					insertCommand.Parameters.Add(parameterSalt);

					insertCommand.ExecuteNonQuery();
				}
			}
		}
		
		// get current password
		public string GetPassword(int userID)
		{
			string password = string.Empty;
			string salt = string.Empty;
			using(SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand selectCommand = new SqlCommand("select password, salt from rb_users where userid = '" + userID + "'"))
				{
					selectCommand.Connection = myConnection;
					selectCommand.CommandType = CommandType.Text;
					myConnection.Open();
					SqlDataReader reader = selectCommand.ExecuteReader();
					while(reader.Read())
					{
						password = reader[0].ToString();
						salt = reader[1].ToString();
					}
				}
			}
			
			return password + "," + salt;
		}
	}
}






















