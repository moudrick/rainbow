namespace Rainbow.Tests
{
    using System;
    using System.Web.Security;

    using NUnit.Framework;

    using Rainbow.Framework.Providers.RainbowMembershipProvider;
    using Rainbow.Framework.Providers.RainbowRoleProvider;

    /// <summary>
    /// The role provider test.
    /// </summary>
    [TestFixture]
    public class RoleProviderTest
    {
        #region Public Methods

        /// <summary>
        /// The add users to roles test 1.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "invalid@user.com";

                var roles = new string[1];
                roles[0] = "Admins";

                provider.AddUsersToRoles(users, roles);
                Assert.Fail();
            }
            catch (RainbowMembershipProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The add users to roles test 2.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "admin@rainbowportal.net";

                var roles = new string[1];
                roles[0] = "invalidRole";

                provider.AddUsersToRoles(users, roles);
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The add users to roles test 3.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "admin@rainbowportal.net";

                var roles = new string[1];
                roles[0] = "editors";
                provider.AddUsersToRoles(users, roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The add users to roles test 4.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest4()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new Guid[1];
                users[0] = Guid.NewGuid();

                var roles = new Guid[1];
                roles[0] = new Guid("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D"); // Admins

                provider.AddUsersToRoles("Rainbow", users, roles);
                Assert.Fail();
            }
            catch (RainbowMembershipProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest4" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The add users to roles test 5.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest5()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new Guid[1];
                users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); // "admin@rainbowportal.net";

                var roles = new Guid[1];
                roles[0] = Guid.NewGuid();

                provider.AddUsersToRoles("Rainbow", users, roles);
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest5" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The add users to roles test 6.
        /// </summary>
        [Test]
        public void AddUsersToRolesTest6()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var user = (RainbowUser)Membership.GetUser("admin@rainbowportal.net");
                var users = new Guid[1];
                users[0] = user.ProviderUserKey;

                var role = provider.GetRoleByName("Rainbow", "clerks");
                var roles = new Guid[1];
                roles[0] = role.Id;

                provider.AddUsersToRoles("Rainbow", users, roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in AddUsersToRolesTest6" + ex.Message, ex);
            }
        }

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
                Console.WriteLine(ex.Message);
                Assert.Fail("Error retrieving ApplicationName property" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The create role test 1.
        /// </summary>
        [Test]
        public void CreateRoleTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.CreateRole(Roles.ApplicationName, "editors");
                provider.CreateRole(Roles.ApplicationName, "clerks");
                provider.CreateRole(Roles.ApplicationName, "salesman");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in CreateRoleTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The create role test 2.
        /// </summary>
        [Test]
        public void CreateRoleTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.CreateRole(Roles.ApplicationName, "Admins");
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in CreateRoleTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The create role test 3.
        /// </summary>
        [Test]
        public void CreateRoleTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.CreateRole(Roles.ApplicationName, "Admins,editors");
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in CreateRoleTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The delete role test 1.
        /// </summary>
        [Test]
        public void DeleteRoleTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.DeleteRole("Admins", true);
                Assert.Fail(); // Admins has users
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in DeleteRoleTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The delete role test 2.
        /// </summary>
        [Test]
        public void DeleteRoleTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.CreateRole("tempRole1");
                provider.DeleteRole("tempRole1", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in DeleteRoleTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The delete role test 3.
        /// </summary>
        [Test]
        public void DeleteRoleTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.CreateRole("tempRole2");

                provider.AddUsersToRoles(new[] { "user@user.com" }, new[] { "tempRole2" });

                provider.DeleteRole("tempRole2", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in DeleteRoleTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The delete role test 4.
        /// </summary>
        [Test]
        public void DeleteRoleTest4()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var editors = provider.GetRoleByName("Rainbow", "editors");
                provider.DeleteRole("invalidApp", editors.Id, true);
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in DeleteRoleTest4" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The delete role test 5.
        /// </summary>
        [Test]
        public void DeleteRoleTest5()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                provider.DeleteRole("invalidRole", true);
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in DeleteRoleTest5" + ex.Message, ex);
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
        /// The get all roles test 1.
        /// </summary>
        [Test]
        public void GetAllRolesTest1()
        {
            try
            {
                var roles = Roles.GetAllRoles();
                Assert.AreEqual(4, roles.Length);
                Assert.AreEqual("All Users", roles[0]);
                Assert.AreEqual("Authenticated Users", roles[1]);
                Assert.AreEqual("Unauthenticated Users", roles[2]);
                Assert.AreEqual("Admins", roles[3]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetAllRolesTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get all roles test 2.
        /// </summary>
        [Test]
        public void GetAllRolesTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var roles = provider.GetAllRoles(Roles.ApplicationName);
                Assert.AreEqual(4, roles.Count);
                Assert.AreEqual("All Users", roles[0].Name);
                Assert.AreEqual("Authenticated Users", roles[1].Name);
                Assert.AreEqual("Unauthenticated Users", roles[2].Name);
                Assert.AreEqual("Admins", roles[3].Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetAllRolesTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get roles for user test 1.
        /// </summary>
        [Test]
        public void GetRolesForUserTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var userId = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); // "admin@rainbowportal.net" 

                var roles = provider.GetRolesForUser(Roles.ApplicationName, userId);
                Assert.AreEqual(1, roles.Count);
                Assert.AreEqual("Admins", roles[0].Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetRolesForUserTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get roles for user test 2.
        /// </summary>
        [Test]
        public void GetRolesForUserTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var roles = provider.GetRolesForUser(Roles.ApplicationName, new Guid());
                Assert.Fail();
            }
            catch (RainbowRoleProviderException ex)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetRolesForUserTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get roles for user test 3.
        /// </summary>
        [Test]
        public void GetRolesForUserTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var userId = new Guid("34ADB714-92B0-47ff-B5AF-5DB2E0D124A9"); // "user@user.com"
                if (provider != null)
                {
                    var roles = provider.GetRolesForUser(Roles.ApplicationName, userId);
                    Assert.AreEqual(roles.Count, 0);
                }
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetRolesForUserTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get users in role test 1.
        /// </summary>
        [Test]
        public void GetUsersInRoleTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var names = provider.GetUsersInRole("Admins");
                    Assert.AreEqual(1, names.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetUsersInRoleTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get users in role test 2.
        /// </summary>
        [Test]
        public void GetUsersInRoleTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var names = provider.GetUsersInRole("editors");
                    Assert.AreEqual(0, names.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetUsersInRoleTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The get users in role test 3.
        /// </summary>
        [Test]
        public void GetUsersInRoleTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var names = provider.GetUsersInRole("invalidRole");
                }
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in GetUsersInRoleTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The is user in role test 1.
        /// </summary>
        [Test]
        public void IsUserInRoleTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.IsUserInRole("admin@rainbowportal.net", "Admins");
                    Assert.IsTrue(isInRole);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in IsUserInRoleTest1" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The is user in role test 2.
        /// </summary>
        [Test]
        public void IsUserInRoleTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.IsUserInRole("invalid@user.com", "Admins");
                }
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in IsUserInRoleTest2" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The is user in role test 3.
        /// </summary>
        [Test]
        public void IsUserInRoleTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.IsUserInRole("admin@rainbowportal.net", "invalidRole");
                }
                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in IsUserInRoleTest3" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The is user in role test 4.
        /// </summary>
        [Test]
        public void IsUserInRoleTest4()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.IsUserInRole("admin@rainbowportal.net", "editors");
                    Assert.IsFalse(isInRole);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in IsUserInRoleTest4" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 1.
        /// </summary>
        [Test]
        public void RemoveUsersFromRolesTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "invalid@user.com";

                var roles = new string[1];
                roles[0] = "Admins";

                if (provider != null)
                {
                    provider.RemoveUsersFromRoles(users, roles);
                }

                Assert.Fail();
            }
            catch (RainbowMembershipProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest1{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 2.
        /// </summary>
        [Test]
        public void RemoveUsersFromRolesTest2()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "admin@rainbowportal.net";

                var roles = new string[1];
                roles[0] = "invalidRole";

                if (provider != null)
                {
                    provider.RemoveUsersFromRoles(users, roles);
                }

                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest2{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 3.
        /// </summary>
        [Test]
        public void RemoveUsersFromRolesTest3()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new string[1];
                users[0] = "admin@rainbowportal.net";

                var roles = new string[1];
                roles[0] = "editors";
                if (provider != null)
                {
                    provider.RemoveUsersFromRoles(users, roles); // admin is in editors role
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest3{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 4.
        /// </summary>
        [Test]
        [Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest4()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new Guid[1];
                users[0] = Guid.NewGuid();

                var roles = new Guid[1];
                roles[0] = new Guid("F6A4ADDA-8450-4F9A-BE86-D0719B239A8D"); // Admins

                if (provider != null)
                {
                    provider.RemoveUsersFromRoles("Rainbow", users, roles);
                }

                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest4{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 5.
        /// </summary>
        [Test]
        public void RemoveUsersFromRolesTest5()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new Guid[1];
                users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); // "admin@rainbowportal.net";

                var roles = new Guid[1];
                roles[0] = Guid.NewGuid();

                if (provider != null)
                {
                    provider.RemoveUsersFromRoles("Rainbow", users, roles);
                }

                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest5{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 6.
        /// </summary>
        [Test]
        [Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest6()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new Guid[1];
                users[0] = new Guid("BE7DC028-7238-45D3-AF35-DD3FE4AEFB7E"); // "admin@rainbowportal.net";

                if (provider != null)
                {
                    var editors = provider.GetRoleByName("Rainbow", "salesman");
                    var roles = new Guid[1];
                    roles[0] = editors.Id;

                    provider.RemoveUsersFromRoles("Rainbow", users, roles);
                }

                Assert.Fail();
            }
            catch (RainbowRoleProviderException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in RemoveUsersFromRolesTest6" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 7.
        /// </summary>
        [Test]
        [Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest7()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var userId = new Guid("34ADB714-92B0-47ff-B5AF-5DB2E0D124A9"); // user@user.com;

                var users = new[] { userId };

                if (provider != null)
                {
                    var editors = provider.GetRoleByName("Rainbow", "editors");
                    var roles = new Guid[1];
                    roles[0] = editors.Id;

                    provider.AddUsersToRoles("Rainbow", users, roles);
                    Assert.IsTrue(provider.IsUserInRole("Rainbow", userId, editors.Id));

                    provider.RemoveUsersFromRoles("Rainbow", users, roles);
                    Assert.IsFalse(provider.IsUserInRole("Rainbow", userId, editors.Id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail("Error in RemoveUsersFromRolesTest7" + ex.Message, ex);
            }
        }

        /// <summary>
        /// The remove users from roles test 8.
        /// </summary>
        [Test]
        [Ignore("Temporarily until it will be fixed")]
        public void RemoveUsersFromRolesTest8()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                var users = new[] { "user@user.com" };

                var roles = new[] { "editors" };

                if (provider != null)
                {
                    provider.AddUsersToRoles(users, roles);
                    Assert.IsTrue(provider.IsUserInRole("user@user.com", "editors"));

                    provider.RemoveUsersFromRoles(users, roles);
                    Assert.IsFalse(provider.IsUserInRole("user@user.com", "editors"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RemoveUsersFromRolesTest8{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The role exists test 1.
        /// </summary>
        [Test]
        public void RoleExistsTest1()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.RoleExists("editors");
                    Assert.IsTrue(isInRole);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RoleExistsTest1{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// The role exists test invalid role name.
        /// </summary>
        [Test]
        public void RoleExistsTestInvalidRoleName()
        {
            try
            {
                var provider = Roles.Provider as RainbowRoleProvider;

                if (provider != null)
                {
                    var isInRole = provider.RoleExists("invalidRole");
                    Assert.IsFalse(isInRole);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail(string.Format("Error in RoleExistsTest2{0}", ex.Message), ex);
            }
        }

        #endregion

        /* 

        public abstract string[] FindUsersInRole( string portalAlias, string roleName, string usernameToMatch );

        rename role
         
         */
    }
}