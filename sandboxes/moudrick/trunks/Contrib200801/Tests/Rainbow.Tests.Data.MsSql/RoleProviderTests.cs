using System;
using System.Collections.Generic;
using System.Web.Security;
using NUnit.Framework;
using Rainbow.Framework.Providers.RainbowMembershipProvider;
using Rainbow.Framework.Providers.RainbowRoleProvider;

namespace Rainbow.Tests.Data.MsSql
{
    [TestFixture]
    public class RoleProviderTests
    {
        Guid userId;

        static RainbowRoleProvider Provider
        {
            get
            {
                //return Roles.Provider as RainbowRoleProvider;
                return RainbowRoleProvider.Instance;
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            MembershipCreateStatus status;
            MembershipUser user = Membership.CreateUser("user@user.com", "user", "user@user.com",
                "question", "answer", true, out status);

            Assert.AreEqual(MembershipCreateStatus.Success, status); 
            Assert.IsNotNull(user);
            Assert.AreEqual(user.UserName, "user@user.com");
            Assert.AreEqual(user.Email, "user@user.com");

            //RainbowUser rainbowUser = (RainbowUser) user.ProviderUserKey;
            //userId = new Guid("34ADB714-92B0-47ff-B5AF-5DB2E0D124A9"); //"user@user.com"
            userId = (Guid) user.ProviderUserKey;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            bool success = Membership.DeleteUser("user@user.com", true);
            Assert.IsTrue(success);
        }

        [Test]
        public void ApplicationName()
        {
            Assert.AreEqual("Rainbow", Membership.ApplicationName,
                "Error retrieving ApplicationName property");
        }

        [Test]
        public void GetAllRolesTest_Roles()
        {
            string[] roles = Roles.GetAllRoles();
            Assert.AreEqual(4, roles.Length);
            Assert.AreEqual("All Users", roles[0]);
            Assert.AreEqual("Authenticated Users", roles[1]);
            Assert.AreEqual("Unauthenticated Users", roles[2]);
            Assert.AreEqual("Admins", roles[3]);
        }

        [Test]
        public void Instance()
        {
            RainbowRoleProvider provider = Roles.Provider as RainbowRoleProvider;
            Assert.IsNotNull(provider, "Roles.Provider should be of RainbowRoleProvider type");
            Assert.AreSame(Provider, provider, "RoleProvider is not configured correctly");
        }

        [Test]
        public void GetAllRolesTest_RolesProvider()
        {
            IList<RainbowRole> roles =
                Provider.GetAllRoles(Roles.ApplicationName);
            Assert.AreEqual(4, roles.Count);
            Assert.AreEqual("All Users", roles[0].Name);
            Assert.AreEqual("Authenticated Users", roles[1].Name);
            Assert.AreEqual("Unauthenticated Users", roles[2].Name);
            Assert.AreEqual("Admins", roles[3].Name);
        }

        [Test]
        public void GetRolesForUserTest1()
        {
            Guid adminId = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); //"admin@rainbowportal.net" 

            IList<RainbowRole> roles =
                Provider.GetRolesForUser(Roles.ApplicationName, adminId);
            Assert.AreEqual(1, roles.Count, "Error in GetRolesForUserTest1");
            Assert.AreEqual("Admins", roles[0].Name, "Error in GetRolesForUserTest1");
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void GetRolesForUserTest2()
        {
            Provider.GetRolesForUser(Roles.ApplicationName, new Guid());
        }

        [Test]
        //[ExpectedException(typeof (RainbowRoleProviderException))]
        public void GetRolesForUserTest3()
        {
//            Guid userId = new Guid("34ADB714-92B0-47ff-B5AF-5DB2E0D124A9"); // user@user.com;
            IList<RainbowRole> roles =
                Provider.GetRolesForUser(Roles.ApplicationName, userId);
            Assert.AreEqual(0, roles.Count, "Error in GetRolesForUserTest3");
        }

        [Test]
        public void CreateRoleTest1()
        {
            Provider.CreateRole(Roles.ApplicationName, "editors");
            Provider.CreateRole(Roles.ApplicationName, "clerks");
            Provider.CreateRole(Roles.ApplicationName, "salesman");
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void CreateRoleTest2()
        {
            Provider.CreateRole(Roles.ApplicationName, "Admins");
        }

        [Test]
        [ExpectedException(typeof(RainbowRoleProviderException), "Role name can't contain commas")]
        public void CreateRoleTest3()
        {
            Provider.CreateRole(Roles.ApplicationName, "Admins,editors");
        }

        [Test]
        public void IsUserInRoleTest1()
        {
            Assert.IsTrue(Provider.IsUserInRole("admin@rainbowportal.net", "Admins"));
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void IsUserInRoleTest2()
        {
            Provider.IsUserInRole("invalid@user.com", "Admins");
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void IsUserInRoleTest3()
        {
            Provider.IsUserInRole("admin@rainbowportal.net", "invalidRole");
        }

        [Test]
        public void IsUserInRoleTest4()
        {
            Assert.IsFalse(Provider.IsUserInRole("admin@rainbowportal.net", "editors"));
        }

        [Test]
        public void RoleExistsTest1()
        {
            Assert.IsTrue(Provider.RoleExists("editors"));
        }

        [Test]
        public void RoleExistsTestInvalidRoleName()
        {
            Assert.IsFalse(Provider.RoleExists("invalidRole"));
        }

        [Test]
        public void GetUsersInRoleTest1()
        {
            string[] names = Provider.GetUsersInRole("Admins");
            Assert.AreEqual(1, names.Length);
        }

        [Test]
        public void GetUsersInRoleTest2()
        {
            string[] names = Provider.GetUsersInRole("editors");
            Assert.AreEqual(0, names.Length);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void GetUsersInRoleTest3()
        {
            Provider.GetUsersInRole("invalidRole");
        }

        [Test]
        [ExpectedException(typeof (RainbowMembershipProviderException))]
        public void AddUsersToRolesTest1()
        {
            string[] users = new string[] {"invalid@user.com"};
            string[] roles = new string[] {"Admins"};
            Provider.AddUsersToRoles(users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void AddUsersToRolesTest2()
        {
            string[] users = new string[] {"admin@rainbowportal.net"};
            string[] roles = new string[] {"invalidRole"};
            Provider.AddUsersToRoles(users, roles);
        }

        [Test]
        public void AddUsersToRolesTest3()
        {
            string[] users = new string[] {"admin@rainbowportal.net"};
            string[] roles = new string[] {"editors"};
            Provider.AddUsersToRoles(users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowMembershipProviderException))]
        public void AddUsersToRolesTest4()
        {
            Guid[] users = new Guid[1];
            users[0] = Guid.NewGuid();

            Guid[] roles = new Guid[1];
            roles[0] = new Guid("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D"); // Admins

            Provider.AddUsersToRoles("Rainbow", users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void AddUsersToRolesTest5()
        {
            Guid[] users = new Guid[1];
            users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); //"admin@rainbowportal.net";

            Guid[] roles = new Guid[1];
            roles[0] = Guid.NewGuid();

            Provider.AddUsersToRoles("Rainbow", users, roles);
        }

        [Test]
        public void AddUsersToRolesTest6()
        {
            RainbowUser user = (RainbowUser) Membership.GetUser("admin@rainbowportal.net");
            Guid[] users = new Guid[1];
            users[0] = user.ProviderUserKey;

            RainbowRole role = Provider.GetRoleByName("Rainbow", "clerks");
            Guid[] roles = new Guid[1];
            roles[0] = role.Id;

            Provider.AddUsersToRoles("Rainbow", users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowMembershipProviderException))]
        public void RemoveUsersFromRolesTest1()
        {
            string[] users = new string[1];
            users[0] = "invalid@user.com";

            string[] roles = new string[1];
            roles[0] = "Admins";

            Provider.RemoveUsersFromRoles(users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void RemoveUsersFromRolesTest2()
        {
            string[] users = new string[1];
            users[0] = "admin@rainbowportal.net";

            string[] roles = new string[1];
            roles[0] = "invalidRole";

            Provider.RemoveUsersFromRoles(users, roles);
        }

        [Test]
        public void RemoveUsersFromRolesTest3()
        {
            string[] users = new string[1];
            users[0] = "admin@rainbowportal.net";

            string[] roles = new string[1];
            roles[0] = "editors";
            Provider.RemoveUsersFromRoles(users, roles); // admin is in editors role
        }

        [Test]
        [Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest4()
        {
            Guid[] users = new Guid[1];
            users[0] = Guid.NewGuid();

            Guid[] roles = new Guid[1];
            roles[0] = new Guid("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D"); // Admins

            Provider.RemoveUsersFromRoles("Rainbow", users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void RemoveUsersFromRolesTest5()
        {
            Guid[] users = new Guid[1];
            users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); //"admin@rainbowportal.net";

            Guid[] roles = new Guid[1];
            roles[0] = Guid.NewGuid();

            Provider.RemoveUsersFromRoles("Rainbow", users, roles);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        //[Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest6()
        {
            Guid[] users = new Guid[1];
            users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); //"admin@rainbowportal.net";

            RainbowRole editors = Provider.GetRoleByName("Rainbow", "salesman");
            Guid[] roles = new Guid[1];
            roles[0] = editors.Id;

            Provider.RemoveUsersFromRoles("Rainbow", users, roles);
        }

        [Test]
        //[Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest7()
        {
//            Guid userId = new Guid("34ADB714-92B0-47ff-B5AF-5DB2E0D124A9"); // user@user.com;

            Guid[] users = new Guid[] {userId};

            RainbowRole editors = Provider.GetRoleByName("Rainbow", "editors");
            Guid[] roles = new Guid[1];
            roles[0] = editors.Id;

            Provider.AddUsersToRoles("Rainbow", users, roles);
            Assert.IsTrue(Provider.IsUserInRole("Rainbow", userId, editors.Id));

            Provider.RemoveUsersFromRoles("Rainbow", users, roles);
            Assert.IsFalse(Provider.IsUserInRole("Rainbow", userId, editors.Id));
        }

        [Test]
        public void RemoveUsersFromRolesTest8()
        {
            string[] users = new string[] {"user@user.com"};

            string[] roles = new string[] {"editors"};

            Provider.AddUsersToRoles(users, roles);
            Assert.IsTrue(Provider.IsUserInRole("user@user.com", "editors"));

            Provider.RemoveUsersFromRoles(users, roles);
            Assert.IsFalse(Provider.IsUserInRole("user@user.com", "editors"));
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void DeleteRoleTest1()
        {
            Provider.DeleteRole("Admins", true);
        }

        [Test]
        public void DeleteRole_NotPopulated()
        {
            string roleName = "tempRole1";
            Provider.CreateRole(roleName);
            Provider.DeleteRole(roleName, true);
        }

        [Test]
        public void DeleteRole_PopulatedNoThrow()
        {
            string roleName = "tempRole2";
            Provider.CreateRole(roleName);
            Provider.AddUsersToRoles(new string[] {"user@user.com"}, new string[] {roleName});
            Provider.DeleteRole(roleName, false);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void DeleteRoleTest4()
        {
            RainbowRole editors = Provider.GetRoleByName("Rainbow", "editors");
            Provider.DeleteRole("invalidApp", editors.Id, true);
        }

        [Test]
        [ExpectedException(typeof (RainbowRoleProviderException))]
        public void DeleteRoleTest5()
        {
            Provider.DeleteRole("invalidRole", true);
        }

//        public abstract string[] FindUsersInRole( string portalAlias, string roleName, string usernameToMatch );
//        rename role
    }
}