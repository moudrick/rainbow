using System;
using NUnit.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers.Exceptions;
using System.Web.Security;
using RainbowSqlMembershipProvider=Rainbow.Framework.Providers.MsSql.RainbowSqlMembershipProvider;

namespace Rainbow.Tests.Data.MsSql
{
    [TestFixture]
    public class MembershipProviderTests : BaseProviderTestFixture
    {
        #region Config properties

        [Test]
        public void ApplicationNameTest()
        {
            string appName = Membership.ApplicationName;
            Assert.AreEqual(appName, "Rainbow", "Error retrieving ApplicationName property");
        }

        [Test]
        public void EnablePasswordResetTest()
        {
            bool enablePwdReset = Membership.EnablePasswordReset;
            Assert.AreEqual(enablePwdReset, true, "Error retrieving EnablePasswordReset property");
        }

        [Test]
        public void EnablePasswordRetrievalTest()
        {
            bool enablePwdRetrieval = Membership.EnablePasswordRetrieval;
            Assert.AreEqual(enablePwdRetrieval, false, "Error retrieving EnablePasswordRetrieval property");
        }

        [Test]
        public void HashAlgorithmTypeTest()
        {
            string hashAlgType = Membership.HashAlgorithmType;
            Assert.AreEqual(hashAlgType, "SHA1", "Error retrieving HashAlgorithmType property");
        }

        [Test]
        public void MaxInvalidPasswordAttemptsTest()
        {
            int maxInvalidPwdAttempts = Membership.MaxInvalidPasswordAttempts;
            Assert.AreEqual(maxInvalidPwdAttempts, 5, "Error retrieving MaxInvalidPasswordAttempts property");
        }

        [Test]
        public void MinRequiredNonAlphanumericCharactersTest()
        {
            int minReqNonAlpha = Membership.MinRequiredNonAlphanumericCharacters;
            Assert.AreEqual(minReqNonAlpha, 1, "Error retrieving MinRequiredNonAlphanumericCharacters property");
        }

        [Test]
        public void MinRequiredPasswordLengthTest()
        {
            int minReqPwdLength = Membership.MinRequiredPasswordLength;
            Assert.AreEqual(minReqPwdLength, 5, "Error retrieving MinRequiredPasswordLength property");
        }

        [Test]
        public void PasswordAttemptWindowTest()
        {
            int pwdAttemptWindow = Membership.PasswordAttemptWindow;
            Assert.AreEqual(pwdAttemptWindow, 15, "Error retrieving PasswordAttemptWindows property");
        }

        [Test]
        public void PasswordStrengthRegularExpressionTest()
        {
            string pwdStrengthRegex = Membership.PasswordStrengthRegularExpression;
            Assert.AreEqual(pwdStrengthRegex, string.Empty,
                            "Error retrieving PasswordStrengthRegularExpression property");
        }

        [Test]
        public void ProviderTest()
        {
            MembershipProvider provider = Membership.Provider;
            Assert.AreEqual(provider.GetType(),
                typeof (RainbowSqlMembershipProvider),
                "Error retrieving Provider property");
        }

        [Test]
        public void ProvidersTest()
        {
            MembershipProviderCollection providers = Membership.Providers;
            Assert.AreEqual(providers.Count, 1, "Error retrieving Providers property");
        }

        [Test]
        public void RequiresQuestionAndAnswerTest()
        {
            bool reqQuestionAndAnswer = Membership.RequiresQuestionAndAnswer;
            Assert.AreEqual(reqQuestionAndAnswer, false, "Error retrieving RequiresQuestionAndAnswer property");
        }

        #endregion

        [Test]
        public void GetAllUsers()
        {
            int totalRecords;
            Membership.GetAllUsers();
            Membership.GetAllUsers(0, 1, out totalRecords);
        }

        [Test]
        public void GetNumberOfUsersOnline()
        {
            Membership.GetNumberOfUsersOnline();
        }

        [Test]
        public void GetPassword()
        {
            if (Membership.EnablePasswordRetrieval)
            {
                Membership.Provider.GetPassword("admin@rainbowportal.net", "answer");
            }
        }

        [Test]
        public void GetUser()
        {
            Membership.GetUser("admin@rainbowportal.net");
        }

        [Test]
        public void GetUserNameByEmailValidUser()
        {
            string userName = Membership.GetUserNameByEmail("admin@rainbowportal.net");
            Assert.AreEqual(userName, "admin@rainbowportal.net", "Error in GetUserNameByEmailValidUserTest");
        }

        [Test]
        public void GetUserNameByEmailInvalidUser()
        {
            string userName = Membership.GetUserNameByEmail("invaliduser@doesnotexist.com");
            Assert.AreEqual(userName, string.Empty, "Error in GetUserNameByEmailInvalidUserTest");
        }

        [Test]
        public void FindUsersByName1()
        {
            MembershipUserCollection users = Membership.FindUsersByName("admin@rainbowportal.net");
            Assert.AreEqual(users.Count, 1, "Error in FindUsersByNameTest1");
        }

        [Test]
        public void FindUsersByName2()
        {
            MembershipUserCollection users = Membership.FindUsersByName("invaliduser@doesnotexist.com");
            Assert.IsEmpty(users, "Error in FindUsersByNameTest2");
        }

        [Test]
        public void FindUsersByName3()
        {
            int totalRecords;
            MembershipUserCollection users = Membership.FindUsersByName("admin@rainbowportal.net", 0, 10,
                                                                        out totalRecords);
            Assert.AreEqual(users.Count, 1, "Error in FindUsersByNameTest3");
            Assert.Greater(totalRecords, 0, "Error in FindUsersByNameTest3");
        }

        [Test]
        public void FindUsersByName4()
        {
            int totalRecords;
            MembershipUserCollection users = Membership.FindUsersByName("invaliduser@doesnotexist.com", 0, 10,
                                                                        out totalRecords);
            Assert.IsEmpty(users, "Error in FindUsersByNameTest3");
            Assert.AreEqual(totalRecords, 0, "Error in FindUsersByNameTest3");
        }

        [Test]
        public void FindUsersByEmail1()
        {
            MembershipUserCollection users = Membership.FindUsersByEmail("admin@rainbowportal.net");
            Assert.AreEqual(users.Count, 1, "Error in FindUsersByEmailTest1");
        }

        [Test]
        public void FindUsersByEmail2()
        {
            MembershipUserCollection users = Membership.FindUsersByName("invaliduser@doesnotexist.com");
            Assert.IsEmpty(users, "Error in FindUsersByEmailTest2");
        }

        [Test]
        public void FindUsersByEmail3()
        {
            int totalRecords;
            MembershipUserCollection users = Membership.FindUsersByEmail("admin@rainbowportal.net", 0, 10,
                                                                         out totalRecords);
            Assert.AreEqual(users.Count, 1, "Error in FindUsersByEmailTest3");
            Assert.Greater(totalRecords, 0, "Error in FindUsersByEmailTest3");
        }

        [Test]
        public void FindUsersByEmail4()
        {
            int totalRecords;
            MembershipUserCollection users =
                Membership.FindUsersByEmail("invaliduser@doesnotexist.com", 0, 10, out totalRecords);
            Assert.IsEmpty(users, "Error in FindUsersByEmailTes4");
            Assert.AreEqual(totalRecords, 0, "Error in FindUsersByEmailTes4");
        }

        [Test]
        public void ValidateUser1()
        {
            bool isValid = Membership.ValidateUser("admin@rainbowportal.net", "notavalidpwd");
            Assert.IsFalse(isValid, "Error in ValidateUserTest1");
        }

        [Test]
        public void ValidateUser2()
        {
            bool isValid = Membership.ValidateUser("admin@rainbowportal.net", "admin");
            Assert.IsTrue(isValid, "Error in ValidateUserTest2");
        }

        [Test]
        public void ValidateUserTest3()
        {
            bool isValid = Membership.ValidateUser("invaliduser@doesnotexist.com", "notavalidpwd");
            Assert.IsFalse(isValid, "Error in ValidateUserTest3");
        }

        [Test]
        public void ValidateUser4()
        {
            bool isValid = Membership.ValidateUser("invaliduser@doesnotexist.com", "admin");
            Assert.IsFalse(isValid, "Error in ValidateUserTest4");
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer1()
        {
            bool pwdChanged =
                Membership.Provider.ChangePasswordQuestionAndAnswer("admin@rainbowportal.net", "admin",
                                                                    "newPasswordQuestion", "newPasswordAnswer");
            Assert.IsTrue(pwdChanged, "Error in ChangePasswordQuestionAndAnswer1");
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer2()
        {
            bool pwdChanged =
                Membership.Provider.ChangePasswordQuestionAndAnswer("admin@rainbowportal.net", "invalidPwd",
                                                                    "newPasswordQuestion", "newPasswordAnswer");
            Assert.IsFalse(pwdChanged, "Error in ChangePasswordQuestionAndAnswer2");
        }

        [Test]
        public void ChangePasswordQuestionAndAnswer3()
        {
            bool pwdChanged =
                Membership.Provider.ChangePasswordQuestionAndAnswer("invaliduser@doesnotexist.com", "InvalidPwd",
                                                                    "newPasswordQuestion", "newPasswordAnswer");
            Assert.IsFalse(pwdChanged, "Error in ChangePasswordQuestionAndAnswer3");
        }

        [Test]
        public void UnlockUser1()
        {
            bool unlocked = Membership.Provider.UnlockUser("admin@rainbowportal.net");
            Assert.IsTrue(unlocked, "Error in UnlockUserTest1");
        }

        [Test]
        public void UnlockUser2()
        {
            bool unlocked = Membership.Provider.UnlockUser("invaliduser@doesnotexist.com");
            Assert.IsFalse(unlocked, "Error in UnlockUserTest2");
        }

        [Test]
        public void CreateUser_ExistingFail()
        {
            MembershipCreateStatus status;
            MembershipUser user =
                Membership.CreateUser("Admin@rainbowportal.net", "admin", "Admin@rainbowportal.net", "question",
                                      "answer", true, out status);
            Assert.IsNull(user, "Error in CreateUserTest1");
            Assert.AreEqual(status, MembershipCreateStatus.DuplicateUserName, "Error in CreateUserTest1");
        }

        [Test]
        public void CreateUser_NewSuccess()
        {
            MembershipCreateStatus status;
            MembershipUser user = Membership.CreateUser("Tito", "tito", "tito@tito.com", 
                "question", "answer", true, out status);

            Assert.IsNotNull(user);
            Assert.AreEqual(status, MembershipCreateStatus.Success);
            Assert.AreEqual(user.UserName, "Tito", "Error in CreateUserTest2");
            Assert.AreEqual(user.Email, "tito@tito.com", "Error in CreateUserTest2");
            Assert.AreEqual(user.PasswordQuestion, "question", "Error in CreateUserTest2");
            Assert.IsTrue(user.IsApproved);
        }

        [Test]
        public void ChangePassword1()
        {
            bool sucess = Membership.Provider.ChangePassword("Tito", "tito", "newPassword");

            Assert.IsTrue(sucess, "Error in ChangePasswordTest1");
        }

        [Test]
        public void ChangePassword2()
        {
            bool sucess = Membership.Provider.ChangePassword("invaliduser@doesnotexist.com", "pwd", "newPassword");

            Assert.IsFalse(sucess, "Error in ChangePasswordTest2");
        }

        [Test]
        public void ChangePassword3()
        {
            bool sucess = Membership.Provider.ChangePassword("Admin@rainbowportal.net", "invalidPwd", "newPassword");
            Assert.IsFalse(sucess, "Error in ChangePasswordTest3");
        }

        [Test]
        public void UpdateUser1()
        {
            RainbowUser user = (RainbowUser) Membership.GetUser("Tito");

            Assert.AreEqual(user.Email, "tito@tito.com");
            Assert.IsTrue(user.IsApproved);

            user.Email = "newEmail@tito.com";
            user.IsApproved = false;
            user.LastLoginDate = new DateTime(1982, 2, 6);

            Membership.UpdateUser(user);

            user = (RainbowUser) Membership.GetUser("Tito");
            Assert.AreEqual(user.Email, "newEmail@tito.com", "Error in UpdateUserTest1");
            Assert.IsFalse(user.IsApproved, "Error in UpdateUserTest1");
            Assert.AreEqual(new DateTime(1982, 2, 6), user.LastLoginDate, "Error in UpdateUserTest1");
        }

        [Test]
        [ExpectedException(typeof (RainbowMembershipProviderException))]
        public void UpdateUser2()
        {
            RainbowUser user =
                new RainbowUser(Membership.Provider.Name, "invalidUserName", Guid.NewGuid(), "tito@tito.com", "question",
                                "answer", true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now,
                                DateTime.MinValue);
            Membership.UpdateUser(user);
            //"Error in UpdateUserTest1"
            //"UpdateUser didn't throw an exception even though userName was invalid"
        }

        [Test]
        [ExpectedException(typeof (RainbowMembershipProviderException))]
        public void ResetPassword1()
        {
            Membership.Provider.ResetPassword("invalidUser", "answer");
            //"ResetPassword went ok with invalid user name" );
            //"Error in ResetPasswordTest1"
        }

        [Test]
        [ExpectedException(typeof (MembershipPasswordException))]
        public void ResetPassword2()
        {
            Membership.Provider.ResetPassword("Tito", "invalidAnswer");
            //"ResetPassword went ok with invalid password answer"
            //"Error in ResetPasswordTest2"
        }

        [Test]
        public void ResetPassword3()
        {
            Membership.Provider.ResetPassword("Tito", "answer");
            //"Error in ResetPasswordTest3"
        }

        [Test]
        public void DeleteUser_InvalidFail()
        {
            bool success = Membership.DeleteUser("invalidUser");
            Assert.IsFalse(success, "Error in DeleteUserTest1");
        }

        [Test]
        public void DeleteUsert_ValidSuccess()
        {
            bool success = Membership.DeleteUser("Tito");
            Assert.IsTrue(success, "Error in DeleteUserTest2");
        }
    }
}
