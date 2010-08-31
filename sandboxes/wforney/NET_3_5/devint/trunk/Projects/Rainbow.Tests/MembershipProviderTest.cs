namespace Rainbow.Tests
{
    using System;
    using System.Web.Security;

    using NUnit.Framework;

    using Rainbow.Framework.Providers.RainbowMembershipProvider;

    /// <summary>
    /// The membership provider test.
    /// </summary>
    [TestFixture]
    public class MembershipProviderTest
    {
        #region Public Methods

        /// <summary>
        /// The application name test.
        /// </summary>
        [Test]
        public void ApplicationNameTest()
        {
            try
            {
                var appName = Membership.ApplicationName;
                Assert.AreEqual(appName, "Rainbow");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving ApplicationName property", ex);
            }
        }

        /// <summary>
        /// The change password question and answer test 1.
        /// </summary>
        [Test]
        public void ChangePasswordQuestionAndAnswerTest1()
        {
            try
            {
                var pwdChanged = Membership.Provider.ChangePasswordQuestionAndAnswer(
                    "admin@rainbowportal.net", "admin", "newPasswordQuestion", "newPasswordAnswer");
                Assert.IsTrue(pwdChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordQuestionAndAnswer1", ex);
            }
        }

        /// <summary>
        /// The change password question and answer test 2.
        /// </summary>
        [Test]
        public void ChangePasswordQuestionAndAnswerTest2()
        {
            try
            {
                var pwdChanged = Membership.Provider.ChangePasswordQuestionAndAnswer(
                    "admin@rainbowportal.net", "invalidPwd", "newPasswordQuestion", "newPasswordAnswer");
                Assert.IsFalse(pwdChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordQuestionAndAnswer2", ex);
            }
        }

        /// <summary>
        /// The change password question and answer test 3.
        /// </summary>
        [Test]
        public void ChangePasswordQuestionAndAnswerTest3()
        {
            try
            {
                var pwdChanged = Membership.Provider.ChangePasswordQuestionAndAnswer(
                    "invaliduser@doesnotexist.com", "InvalidPwd", "newPasswordQuestion", "newPasswordAnswer");
                Assert.IsFalse(pwdChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordQuestionAndAnswer3", ex);
            }
        }

        /// <summary>
        /// The change password test 1.
        /// </summary>
        [Test]
        public void ChangePasswordTest1()
        {
            try
            {
                var sucess = Membership.Provider.ChangePassword("Tito", "tito", "newPassword");

                Assert.IsTrue(sucess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordTest1", ex);
            }
        }

        /// <summary>
        /// The change password test 2.
        /// </summary>
        [Test]
        public void ChangePasswordTest2()
        {
            try
            {
                var sucess = Membership.Provider.ChangePassword("invaliduser@doesnotexist.com", "pwd", "newPassword");

                Assert.IsFalse(sucess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordTest2", ex);
            }
        }

        /// <summary>
        /// The change password test 3.
        /// </summary>
        [Test]
        public void ChangePasswordTest3()
        {
            try
            {
                var sucess = Membership.Provider.ChangePassword("Admin@rainbowportal.net", "invalidPwd", "newPassword");

                Assert.IsFalse(sucess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ChangePasswordTest3", ex);
            }
        }

        /// <summary>
        /// The create user test 1.
        /// </summary>
        [Test]
        public void CreateUserTest1()
        {
            try
            {
                MembershipCreateStatus status;
                var user = Membership.CreateUser(
                    "Admin@rainbowportal.net", 
                    "admin", 
                    "Admin@rainbowportal.net", 
                    "question", 
                    "answer", 
                    true, 
                    out status);
                Assert.IsNull(user);
                Assert.AreEqual(status, MembershipCreateStatus.DuplicateUserName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in CreateUserTest1", ex);
            }
        }

        /// <summary>
        /// The create user test 2.
        /// </summary>
        [Test]
        public void CreateUserTest2()
        {
            try
            {
                MembershipCreateStatus status;
                var user = Membership.CreateUser(
                    "Tito", "tito", "tito@tito.com", "question", "answer", true, out status);

                Assert.IsNotNull(user);
                Assert.AreEqual(status, MembershipCreateStatus.Success);
                Assert.AreEqual(user.UserName, "Tito");
                Assert.AreEqual(user.Email, "tito@tito.com");
                Assert.AreEqual(user.PasswordQuestion, "question");
                Assert.IsTrue(user.IsApproved);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in CreateUserTest2", ex);
            }
        }

        /// <summary>
        /// The delete user test 1.
        /// </summary>
        [Test]
        public void DeleteUserTest1()
        {
            try
            {
                var success = Membership.DeleteUser("invalidUser");
                Assert.IsFalse(success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in DeleteUserTest1", ex);
            }
        }

        /// <summary>
        /// The delete user test 2.
        /// </summary>
        [Test]
        public void DeleteUserTest2()
        {
            try
            {
                var success = Membership.DeleteUser("Tito");
                Assert.IsTrue(success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in DeleteUserTest2", ex);
            }
        }

        /// <summary>
        /// The enable password reset test.
        /// </summary>
        [Test]
        public void EnablePasswordResetTest()
        {
            try
            {
                var enablePwdReset = Membership.EnablePasswordReset;
                Assert.AreEqual(enablePwdReset, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving EnablePasswordReset property", ex);
            }
        }

        /// <summary>
        /// The enable password retrieval test.
        /// </summary>
        [Test]
        public void EnablePasswordRetrievalTest()
        {
            try
            {
                var enablePwdRetrieval = Membership.EnablePasswordRetrieval;
                Assert.AreEqual(enablePwdRetrieval, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving EnablePasswordRetrieval property", ex);
            }
        }

        /// <summary>
        /// The find users by email test 1.
        /// </summary>
        [Test]
        public void FindUsersByEmailTest1()
        {
            try
            {
                var users = Membership.FindUsersByEmail("admin@rainbowportal.net");
                Assert.AreEqual(users.Count, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByEmailTest1", ex);
            }
        }

        /// <summary>
        /// The find users by email test 2.
        /// </summary>
        [Test]
        public void FindUsersByEmailTest2()
        {
            try
            {
                var users = Membership.FindUsersByName("invaliduser@doesnotexist.com");
                Assert.IsEmpty(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByEmailTest2", ex);
            }
        }

        /// <summary>
        /// The find users by email test 3.
        /// </summary>
        [Test]
        public void FindUsersByEmailTest3()
        {
            try
            {
                var totalRecords = -1;
                var users = Membership.FindUsersByEmail("admin@rainbowportal.net", 0, 10, out totalRecords);
                Assert.AreEqual(users.Count, 1);
                Assert.Greater(totalRecords, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByEmailTest3", ex);
            }
        }

        /// <summary>
        /// The find users by email test 4.
        /// </summary>
        [Test]
        public void FindUsersByEmailTest4()
        {
            try
            {
                var totalRecords = -1;
                var users = Membership.FindUsersByEmail("invaliduser@doesnotexist.com", 0, 10, out totalRecords);
                Assert.IsEmpty(users);
                Assert.AreEqual(totalRecords, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByEmailTes4", ex);
            }
        }

        /// <summary>
        /// The find users by name test 1.
        /// </summary>
        [Test]
        public void FindUsersByNameTest1()
        {
            try
            {
                var users = Membership.FindUsersByName("admin@rainbowportal.net");
                Assert.AreEqual(users.Count, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByNameTest1", ex);
            }
        }

        /// <summary>
        /// The find users by name test 2.
        /// </summary>
        [Test]
        public void FindUsersByNameTest2()
        {
            try
            {
                var users = Membership.FindUsersByName("invaliduser@doesnotexist.com");
                Assert.IsEmpty(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByNameTest2", ex);
            }
        }

        /// <summary>
        /// The find users by name test 3.
        /// </summary>
        [Test]
        public void FindUsersByNameTest3()
        {
            try
            {
                var totalRecords = -1;
                var users = Membership.FindUsersByName("admin@rainbowportal.net", 0, 10, out totalRecords);
                Assert.AreEqual(users.Count, 1);
                Assert.Greater(totalRecords, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByNameTest3", ex);
            }
        }

        /// <summary>
        /// The find users by name test 4.
        /// </summary>
        [Test]
        public void FindUsersByNameTest4()
        {
            try
            {
                var totalRecords = -1;
                var users = Membership.FindUsersByName("invaliduser@doesnotexist.com", 0, 10, out totalRecords);
                Assert.IsEmpty(users);
                Assert.AreEqual(totalRecords, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in FindUsersByNameTest4", ex);
            }
        }

        /// <summary>
        /// The fixture set up.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            // Set up initial database environment for testing purposes
            TestHelper.TearDownDb();
            TestHelper.RecreateDbSchema();
        }

        /// <summary>
        /// The foo.
        /// </summary>
        [Test]
        public void Foo()
        {
            Console.WriteLine("This should pass. It only writes to the Console.");
        }

        /// <summary>
        /// The get all users test.
        /// </summary>
        [Test]
        public void GetAllUsersTest()
        {
            try
            {
                int totalRecords;
                Membership.GetAllUsers();
                Membership.GetAllUsers(0, 1, out totalRecords);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetAllUsersTest", ex);
            }
        }

        /// <summary>
        /// The get number of users online test.
        /// </summary>
        [Test]
        public void GetNumberOfUsersOnlineTest()
        {
            try
            {
                Membership.GetNumberOfUsersOnline();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetNumberOfUsersOnlineTest", ex);
            }
        }

        /// <summary>
        /// The get password test.
        /// </summary>
        [Test]
        public void GetPasswordTest()
        {
            try
            {
                if (Membership.EnablePasswordRetrieval)
                {
                    var pwd = Membership.Provider.GetPassword("admin@rainbowportal.net", "answer");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetPasswordTest", ex);
            }
        }

        /// <summary>
        /// The get user name by email invalid user test.
        /// </summary>
        [Test]
        public void GetUserNameByEmailInvalidUserTest()
        {
            try
            {
                var userName = Membership.GetUserNameByEmail("invaliduser@doesnotexist.com");
                Assert.AreEqual(userName, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetUserNameByEmailInvalidUserTest", ex);
            }
        }

        /// <summary>
        /// The get user name by email valid user test.
        /// </summary>
        [Test]
        public void GetUserNameByEmailValidUserTest()
        {
            try
            {
                var userName = Membership.GetUserNameByEmail("admin@rainbowportal.net");
                Assert.AreEqual(userName, "admin@rainbowportal.net");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetUserNameByEmailValidUserTest", ex);
            }
        }

        /// <summary>
        /// The get user test.
        /// </summary>
        [Test]
        public void GetUserTest()
        {
            try
            {
                var user = Membership.GetUser("admin@rainbowportal.net");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in GetUserTest", ex);
            }
        }

        /// <summary>
        /// The hash algorithm type test.
        /// </summary>
        [Test]
        public void HashAlgorithmTypeTest()
        {
            try
            {
                var hashAlgType = Membership.HashAlgorithmType;
                Assert.AreEqual(hashAlgType, "SHA1");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving HashAlgorithmType property", ex);
            }
        }

        /// <summary>
        /// The max invalid password attempts test.
        /// </summary>
        [Test]
        public void MaxInvalidPasswordAttemptsTest()
        {
            try
            {
                var maxInvalidPwdAttempts = Membership.MaxInvalidPasswordAttempts;
                Assert.AreEqual(maxInvalidPwdAttempts, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving MaxInvalidPasswordAttempts property", ex);
            }
        }

        /// <summary>
        /// The min required non alphanumeric characters test.
        /// </summary>
        [Test]
        public void MinRequiredNonAlphanumericCharactersTest()
        {
            try
            {
                var minReqNonAlpha = Membership.MinRequiredNonAlphanumericCharacters;
                Assert.AreEqual(minReqNonAlpha, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving MinRequiredNonAlphanumericCharacters property", ex);
            }
        }

        /// <summary>
        /// The min required password length test.
        /// </summary>
        [Test]
        public void MinRequiredPasswordLengthTest()
        {
            try
            {
                var minReqPwdLength = Membership.MinRequiredPasswordLength;
                Assert.AreEqual(minReqPwdLength, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving MinRequiredPasswordLength property", ex);
            }
        }

        /// <summary>
        /// The password attempt window test.
        /// </summary>
        [Test]
        public void PasswordAttemptWindowTest()
        {
            try
            {
                var pwdAttemptWindow = Membership.PasswordAttemptWindow;
                Assert.AreEqual(pwdAttemptWindow, 15);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving PasswordAttemptWindows property", ex);
            }
        }

        /// <summary>
        /// The password strength regular expression test.
        /// </summary>
        [Test]
        public void PasswordStrengthRegularExpressionTest()
        {
            try
            {
                var pwdStrengthRegex = Membership.PasswordStrengthRegularExpression;
                Assert.AreEqual(pwdStrengthRegex, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving PasswordStrengthRegularExpression property", ex);
            }
        }

        /// <summary>
        /// The provider test.
        /// </summary>
        [Test]
        public void ProviderTest()
        {
            try
            {
                var provider = Membership.Provider;
                Assert.AreEqual(provider.GetType(), typeof(RainbowSqlMembershipProvider));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving Provider property", ex);
            }
        }

        /// <summary>
        /// The providers test.
        /// </summary>
        [Test]
        public void ProvidersTest()
        {
            try
            {
                var providers = Membership.Providers;
                Assert.AreEqual(providers.Count, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving Providers property", ex);
            }
        }

        /// <summary>
        /// The requires question and answer test.
        /// </summary>
        [Test]
        public void RequiresQuestionAndAnswerTest()
        {
            try
            {
                var reqQuestionAndAnswer = Membership.RequiresQuestionAndAnswer;
                Assert.AreEqual(reqQuestionAndAnswer, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error retrieving RequiresQuestionAndAnswer property", ex);
            }
        }

        /// <summary>
        /// The reset password test 1.
        /// </summary>
        [Test]
        public void ResetPasswordTest1()
        {
            try
            {
                var newPwd = Membership.Provider.ResetPassword("invalidUser", "answer");

                Assert.Fail("ResetPassword went ok with invalid user name");
            }
            catch (RainbowMembershipProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ResetPasswordTest1", ex);
            }
        }

        /// <summary>
        /// The reset password test 2.
        /// </summary>
        [Test]
        public void ResetPasswordTest2()
        {
            try
            {
                var newPwd = Membership.Provider.ResetPassword("Tito", "invalidAnswer");
                Assert.Fail("ResetPassword went ok with invalid password answer");
            }
            catch (MembershipPasswordException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ResetPasswordTest2", ex);
            }
        }

        /// <summary>
        /// The reset password test 3.
        /// </summary>
        [Test]
        public void ResetPasswordTest3()
        {
            try
            {
                var newPwd = Membership.Provider.ResetPassword("Tito", "answer");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ResetPasswordTest3", ex);
            }
        }

        /// <summary>
        /// The unlock user test 1.
        /// </summary>
        [Test]
        public void UnlockUserTest1()
        {
            try
            {
                var unlocked = Membership.Provider.UnlockUser("admin@rainbowportal.net");
                Assert.IsTrue(unlocked);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in UnlockUserTest1", ex);
            }
        }

        /// <summary>
        /// The unlock user test 2.
        /// </summary>
        [Test]
        public void UnlockUserTest2()
        {
            try
            {
                var unlocked = Membership.Provider.UnlockUser("invaliduser@doesnotexist.com");
                Assert.IsFalse(unlocked);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in UnlockUserTest2", ex);
            }
        }

        /// <summary>
        /// The update user test 1.
        /// </summary>
        [Test]
        public void UpdateUserTest1()
        {
            try
            {
                var user = (RainbowUser)Membership.GetUser("Tito");

                if (user != null)
                {
                    Assert.AreEqual(user.Email, "tito@tito.com");
                    Assert.IsTrue(user.IsApproved);

                    user.Email = "newEmail@tito.com";
                    user.IsApproved = false;
                    user.LastLoginDate = new DateTime(1982, 2, 6);

                    Membership.UpdateUser(user);
                }

                user = (RainbowUser)Membership.GetUser("Tito");
                if (user != null)
                {
                    Assert.AreEqual(user.Email, "newEmail@tito.com");
                    Assert.IsFalse(user.IsApproved);
                    Assert.AreEqual(new DateTime(1982, 2, 6), user.LastLoginDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in UpdateUserTest1", ex);
            }
        }

        /// <summary>
        /// The update user test 2.
        /// </summary>
        [Test]
        public void UpdateUserTest2()
        {
            try
            {
                var user = new RainbowUser(
                    Membership.Provider.Name, 
                    "invalidUserName", 
                    Guid.NewGuid(), 
                    "tito@tito.com", 
                    "question", 
                    "answer", 
                    true, 
                    false, 
                    DateTime.Now, 
                    DateTime.Now, 
                    DateTime.Now, 
                    DateTime.Now, 
                    DateTime.MinValue);

                Membership.UpdateUser(user);

                Assert.Fail("UpdateUser didn't throw an exception even though userName was invalid");
            }
            catch (RainbowMembershipProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in UpdateUserTest1", ex);
            }
        }

        /// <summary>
        /// The validate user test 1.
        /// </summary>
        [Test]
        public void ValidateUserTest1()
        {
            try
            {
                var isValid = Membership.ValidateUser("admin@rainbowportal.net", "notavalidpwd");
                Assert.IsFalse(isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ValidateUserTest1", ex);
            }
        }

        /// <summary>
        /// The validate user test 2.
        /// </summary>
        [Test]
        public void ValidateUserTest2()
        {
            try
            {
                var isValid = Membership.ValidateUser("admin@rainbowportal.net", "admin");
                Assert.IsTrue(isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ValidateUserTest2", ex);
            }
        }

        /// <summary>
        /// The validate user test 3.
        /// </summary>
        [Test]
        public void ValidateUserTest3()
        {
            try
            {
                var isValid = Membership.ValidateUser("invaliduser@doesnotexist.com", "notavalidpwd");
                Assert.IsFalse(isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ValidateUserTest3", ex);
            }
        }

        /// <summary>
        /// The validate user test 4.
        /// </summary>
        [Test]
        public void ValidateUserTest4()
        {
            try
            {
                var isValid = Membership.ValidateUser("invaliduser@doesnotexist.com", "admin");
                Assert.IsFalse(isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Assert.Fail("Error in ValidateUserTest4", ex);
            }
        }

        #endregion
    }
}