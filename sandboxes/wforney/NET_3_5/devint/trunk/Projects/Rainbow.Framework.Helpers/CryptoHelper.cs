namespace Rainbow.Framework.Helpers
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Web.Security;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data;

    /// <summary>
    /// Summary description for CryptoHelper.
    /// </summary>
    public class CryptoHelper
    {
        #region Public Methods

        /// <summary>
        /// Creates the password hash.
        /// </summary>
        /// <param name="pwd">
        /// The PWD.
        /// </param>
        /// <param name="salt">
        /// The salt.
        /// </param>
        /// <returns>
        /// The create password hash.
        /// </returns>
        public string CreatePasswordHash(string pwd, string salt)
        {
            var saltAndPwd = String.Concat(pwd, salt);
            var hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "SHA1");
            return hashedPwd;
        }

        /// <summary>
        /// Creates the salt.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The create salt.
        /// </returns>
        public string CreateSalt(int size)
        {
            // generate cryptographic rand numb.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // return base64 string representation of random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Hashes the passwords.
        /// </summary>
        public void HashPasswords()
        {
            // string password;
            SqlParameter parameterPassword;
            SqlParameter parameterSalt;
            SqlParameter parameterEmail;
            using (var connectionString = Config.SqlConnectionString)
            {
                var adapter = new SqlDataAdapter("SELECT Email, Password FROM rb_Users", connectionString);
                var builder = new SqlCommandBuilder(adapter);
                var ds = new DataSet();
                connectionString.Open();
                adapter.Fill(ds, "rb_Users");
                foreach (DataRow dr in ds.Tables["rb_Users"].Rows)
                {
                    using (var insertCommand = new SqlCommand("rb_HashPasswords", connectionString))
                    {
                        insertCommand.CommandType = CommandType.StoredProcedure;

                        string salt = this.CreateSalt(5);
                        parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
                        parameterEmail.Value = dr["Email"];
                        insertCommand.Parameters.Add(parameterEmail);
                        parameterPassword = new SqlParameter("@Password", SqlDbType.VarChar, 40);
                        parameterPassword.Value = this.CreatePasswordHash(dr["Password"].ToString(), salt);
                        insertCommand.Parameters.Add(parameterPassword);
                        parameterSalt = new SqlParameter("@Salt", SqlDbType.VarChar, 10);
                        parameterSalt.Value = salt;
                        insertCommand.Parameters.Add(parameterSalt);

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="randomPassword">
        /// The random password.
        /// </param>
        /// <remarks>
        /// update db with random generated password
        /// </remarks>
        public void ResetPassword(string email, string randomPassword)
        {
            string salt;
            SqlParameter parameterPassword;
            SqlParameter parameterSalt;
            SqlParameter parameterEmail;
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (var insertCommand = new SqlCommand("rb_HashPasswords", myConnection))
                {
                    // Mark the Command as a SPROC
                    insertCommand.CommandType = CommandType.StoredProcedure;

                    // Open the database connection and execute the command
                    myConnection.Open();

                    salt = this.CreateSalt(5);
                    parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
                    parameterEmail.Value = email;
                    insertCommand.Parameters.Add(parameterEmail);
                    parameterPassword = new SqlParameter("@Password", SqlDbType.VarChar, 40);
                    parameterPassword.Value = this.CreatePasswordHash(randomPassword, salt);
                    insertCommand.Parameters.Add(parameterPassword);
                    parameterSalt = new SqlParameter("@Salt", SqlDbType.VarChar, 10);
                    parameterSalt.Value = salt;
                    insertCommand.Parameters.Add(parameterSalt);

                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}