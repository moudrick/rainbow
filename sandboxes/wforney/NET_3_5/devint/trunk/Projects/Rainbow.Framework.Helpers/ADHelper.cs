namespace Rainbow.Framework.Helpers
{
    using System;
    using System.Collections;
    using System.Data;
    using System.DirectoryServices;
    using System.Linq;
    using System.Web.Caching;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data.Types;

    /// <summary>
    /// This class contains functions for Active Directory support
    /// </summary>
    [History("gman3001", "2004/10/26", 
        "Added Method to retrieve a list of all AD Groups that a user account is a member of.")]
    public class ADHelper
    {
        #region Enums

        /// <summary>
        /// The ad account type.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public enum AdAccountType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0, 

            /// <summary>
            /// The user.
            /// </summary>
            User = 1, 

            /// <summary>
            /// The group.
            /// </summary>
            Group = 2
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This function returns an EmailAddressList object.
        /// </summary>
        /// <param name="emailAccount">
        /// Windows user or group
        /// </param>
        /// <returns>
        /// EmailAddressList
        /// </returns>
        public static EmailAddressList GetEmailAddresses(string emailAccount)
        {
            // Lookup the domain in which we must look for the user or group
            var account = emailAccount.Split("\\".ToCharArray());
            if (account.Length != 2)
            {
                return new EmailAddressList(); // not a valid windows account!
            }

            DirectoryEntry rootEntry = null;
            string[] domains = Settings.ActiveDirectoryDomainNameService.Split(";".ToCharArray());

            // jes1111 - ConfigurationSettings.AppSettings["ADdns"].Split(";".ToCharArray());
            foreach (var t in domains.Where(t => !t.Trim().ToLower().StartsWith("winnt://")))
            {
                // NT domains do not keep track of email addresses
                rootEntry = GetDomainRoot(t);
                if (GetNetbiosName(rootEntry).Trim().ToLower() == account[0].Trim().ToLower())
                {
                    break;
                }
                
                rootEntry = null;
            }

            // Unknown domain : return empty list
            if (rootEntry == null)
            {
                return new EmailAddressList();
            }

            // Domain found: lets lookup the object
            var directorySearcher = new DirectorySearcher(rootEntry)
                {
                    Filter =
                        "(&(|(objectClass=group)(&(objectClass=user)(objectCategory=person)))(sAMAccountName=" +
                        account[1] + "))"
                };
            directorySearcher.PropertiesToLoad.Add("mail");
            directorySearcher.PropertiesToLoad.Add("objectClass");
            directorySearcher.PropertiesToLoad.Add("member");

            DirectoryEntry entry;
            try
            {
                entry = directorySearcher.FindOne().GetDirectoryEntry();
            }
            catch
            {
                throw new Exception("Could not get users/groups from domain '" + account[0] + "'.");
            }

            // determine accounttype
            var accounttype = GetAccountType(entry);

            var eal = new EmailAddressList();

            // object is user --> retrieve its emailaddress and return
            if (accounttype == AdAccountType.User)
            {
                try
                {
                    eal.Add(entry.Properties["mail"][0]);
                }
                catch
                {
                }

                return eal;
            }

            // object is group --> retrieve all users that are contained 
            // in the group or in groups of the group 
            GetUsersInGroup(entry, eal, new ArrayList());
            return eal;
        }

        /// <summary>
        /// Gets the member list.
        /// </summary>
        /// <param name="refresh">
        /// if set to <c>true</c> [refresh].
        /// </param>
        /// <param name="adDomain">
        /// The AD domain.
        /// </param>
        /// <param name="appCache">
        /// The app cache.
        /// </param>
        /// <returns>
        /// A System.Data.DataTable value...
        /// </returns>
        public static DataTable GetMemberList(bool refresh, string adDomain, Cache appCache)
        {
            // see if we want to refresh, if not, get it from the cache if available
            if (! refresh)
            {
                var tmp = appCache.Get("ADUsersAndGroups" + adDomain);
                if (tmp != null)
                {
                    return ((DataSet)tmp).Tables[0];
                }
            }

            // create dataset
            using (var ds = new DataSet())
            {
                using (var dt = new DataTable())
                {
                    ds.Tables.Add(dt);

                    var dc = new DataColumn("DisplayName", typeof(string));
                    dt.Columns.Add(dc);
                    dc = new DataColumn("AccountName", typeof(string));
                    dt.Columns.Add(dc);
                    dc = new DataColumn("AccountType", typeof(string));
                    dt.Columns.Add(dc);

                    // add built in users first
                    dt.Rows.Add(new object[] { "Admins", "Admins", "group" });
                    dt.Rows.Add(new object[] { "All Users", "All Users", "group" });
                    dt.Rows.Add(new object[] { "Authenticated Users", "Authenticated Users", "group" });
                    dt.Rows.Add(new object[] { "Unauthenticated Users", "Unauthenticated Users", "group" });

                    // construct root entry
                    using (var rootEntry = GetDomainRoot(adDomain))
                    {
                        if (adDomain.Trim().ToLower().StartsWith("ldap://"))
                        {
                            var domainName = GetNetbiosName(rootEntry);

                            // get users/groups
                            var directorySearcher = new DirectorySearcher(rootEntry)
                                { Filter = "(|(objectClass=group)(&(objectClass=user)(objectCategory=person)))" };
                            directorySearcher.PropertiesToLoad.Add("cn");
                            directorySearcher.PropertiesToLoad.Add("objectClass");
                            directorySearcher.PropertiesToLoad.Add("sAMAccountName");

                            try
                            {
                                SearchResultCollection searchResultCollection = directorySearcher.FindAll();
                                foreach (SearchResult resEnt in searchResultCollection)
                                {
                                    var entry = resEnt.GetDirectoryEntry();
                                    var name = (string)entry.Properties["cn"][0];
                                    var abbreviation = (string)entry.Properties["sAMAccountName"][0];
                                    var accounttype = GetAccountType(entry);
                                    dt.Rows.Add(
                                        new object[] { name, domainName + "\\" + abbreviation, accounttype.ToString() });
                                }
                            }
                            catch
                            {
                                throw new Exception("Could not get users/groups from domain '" + adDomain + "'.");
                            }
                        }
                        else if (adDomain.Trim().ToLower().StartsWith("winnt://"))
                        {
                            var domainName = rootEntry.Name;

                            // Get the users
                            rootEntry.Children.SchemaFilter.Add("user");
                            foreach (var fullname in
                                rootEntry.Children.Cast<DirectoryEntry>().Select(
                                    user => (string)user.Properties["FullName"][0]))
                            {
                                // var accountname = user.Name;
                                dt.Rows.Add(
                                    new object[]
                                        {
                                            fullname, domainName + "\\" + fullname, AdAccountType.User.ToString() 
                                        });
                            }

                            // Get the users
                            rootEntry.Children.SchemaFilter.Add("group");
                            foreach (DirectoryEntry user in rootEntry.Children)
                            {
                                var fullname = user.Name;
                                
                                // var accountname = user.Name;
                                dt.Rows.Add(
                                    new object[]
                                        {
                                           fullname, domainName + "\\" + fullname, AdAccountType.Group.ToString() 
                                        });
                            }
                        }
                    }

                    // add dataset to the cache
                    appCache.Insert("ADUsersAndGroups" + adDomain, ds);

                    // return datatable
                    return dt;
                }
            }
        }

        // Added by gman3001: 2004/10/26
        /// <summary>
        /// This function returns an array of strings consisting of the AD groups that this user belongs to.
        /// </summary>
        /// <param name="userAccount">
        /// Windows user or group
        /// </param>
        /// <returns>
        /// Groups string array
        /// </returns>
        public static string[] GetUserGroups(string userAccount)
        {
            var userGroups = new ArrayList();
            if (userAccount != null)
            {
                // Lookup the domain in which we must look for the user or group
                var account = userAccount.Split("\\".ToCharArray());
                if (account.Length != 2)
                {
                    return (string[])userGroups.ToArray(typeof(string)); // not a valid windows account!
                }

                DirectoryEntry rootEntry = null;
                string[] domains = Settings.ActiveDirectoryDomainNameService.Split(";".ToCharArray());

                // jes1111 - ConfigurationSettings.AppSettings["ADdns"].Split(";".ToCharArray());
                foreach (string t in domains.Where(t => !t.Trim().ToLower().StartsWith("winnt://")))
                {
                    // NT domains do not keep track of email addresses
                    rootEntry = GetDomainRoot(t);
                    if (GetNetbiosName(rootEntry).Trim().ToLower() == account[0].Trim().ToLower())
                    {
                        break;
                    }
                    
                    rootEntry = null;
                }

                // Unknown domain : return empty list
                if (rootEntry == null)
                {
                    return (string[])userGroups.ToArray(typeof(string));
                }

                // Domain found: lets lookup the object
                var directorySearcher = new DirectorySearcher(rootEntry)
                    {
                        Filter =
                            "(&(|(objectClass=group)(&(objectClass=user)(objectCategory=person)))(sAMAccountName=" +
                            account[1] + "))"
                    };
                directorySearcher.PropertiesToLoad.Add("objectClass");
                directorySearcher.PropertiesToLoad.Add("member");
                directorySearcher.PropertiesToLoad.Add("sAMAccountName");
                directorySearcher.PropertiesToLoad.Add("displayName");

                DirectoryEntry entry;
                try
                {
                    entry = directorySearcher.FindOne().GetDirectoryEntry();
                }
                catch
                {
                    throw new Exception(
                        "Could not get users/groups from domain '" + account[0] +
                        "'. Either the user/group does not exist, or you do not have the necessary permissions in the Active Directory.");
                }

                // determine accounttype
                var accounttype = GetAccountType(entry);

                // object is user --> retrieve the name of the groups it is a member of and return
                if (accounttype == AdAccountType.User)
                {
                    try
                    {
                        var values = entry.Properties["sAMAccountName"];
                        if (values != null && values.Count > 0)
                        {
                            string accountPath = string.Format("{0}\\{1}", account[0], values[0]);
                            userGroups.Add(accountPath);
                        }

                        // Add generic system groups to the group list, since this user was authenticated when they entered the website
                        // this user is in the 'Authenticated Users' and 'All Users' groups by default
                        userGroups.Add("Authenticated Users");
                        userGroups.Add("All Users");

                        var rootPath = entry.Path;
                        rootPath = rootPath.Substring(0, rootPath.IndexOf("/", 7) + 1);

                        for (var i = 0; i < entry.Properties["memberOf"].Count; i++)
                        {
                            var currentEntry = new DirectoryEntry(rootPath + entry.Properties["memberOf"][i]);
                            if (GetAccountType(currentEntry) == AdAccountType.Group)
                            {
                                // add to group list if this is a group
                                values = currentEntry.Properties["sAMAccountName"];
                                if (values != null && values.Count > 0)
                                {
                                    var groupName = string.Format("{0}\\{1}", account[0], values[0]);
                                    if (!userGroups.Contains(groupName))
                                    {
                                        userGroups.Add(groupName);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return (string[])userGroups.ToArray(typeof(string));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the type of the account.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// A Rainbow.Framework.Helpers.ADHelper.ADAccountType value...
        /// </returns>
        private static AdAccountType GetAccountType(DirectoryEntry entry)
        {
            var accounttype = AdAccountType.User;
            var objectClass = entry.Properties["objectClass"];
            for (var i = 0; i < objectClass.Count; i ++)
            {
                if ((string)objectClass[i] == "group")
                {
                    accounttype = AdAccountType.Group;
                    break;
                }
            }

            return accounttype;
        }

        /// <summary>
        /// Gets the domain root.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <returns>
        /// A System.DirectoryServices.DirectoryEntry value...
        /// </returns>
        private static DirectoryEntry GetDomainRoot(string domain)
        {
            // 2004-07-28, Leo Duran, Fix for IIS not being run on the AD Computer
            return Settings.EnableActiveDirectoryUser
                       ? new DirectoryEntry(domain, Settings.ActiveDirectoryUserName, Settings.ActiveDirectoryUserPassword)
                       : new DirectoryEntry(domain);

            // End 2004-07-28, Leo Duran
        }

        /// <summary>
        /// Gets the name of the netbios.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        private static string GetNetbiosName(DirectoryEntry root)
        {
            var path = root.Path.Substring(7);

            // find domain netbios name
            DirectoryEntry entry;

            // 2004-07-28, Leo Duran, Fix for IIS not being run on the AD Computer
            if (Settings.EnableActiveDirectoryUser)
            {
                entry =
                    new DirectoryEntry(
                        "LDAP://" + path.Insert(path.IndexOf("/") + 1, "CN=Partitions, CN=Configuration,"),
                        Settings.ActiveDirectoryUserName,
                        Settings.ActiveDirectoryUserPassword);
            }
            else
            {
                entry =
                    new DirectoryEntry(
                        "LDAP://" + path.Insert(path.IndexOf("/") + 1, "CN=Partitions, CN=Configuration,"));
            }

            // End 2004-07-28, Leo Duran
            var myS = new DirectorySearcher(entry) { Filter = "(&(objectClass=top)(nETBIOSName=*))" };
            myS.PropertiesToLoad.Add("nETBIOSName");

            try
            {
                entry = myS.FindOne().GetDirectoryEntry();
                return (string)entry.Properties["nETBIOSName"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Domain could not be contacted.", ex);
            }
        }

        /// <summary>
        /// Gets the users in group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="eal">
        /// The eal.
        /// </param>
        /// <param name="searchedGroups">
        /// The searched groups.
        /// </param>
        private static void GetUsersInGroup(DirectoryEntry group, EmailAddressList eal, ArrayList searchedGroups)
        {
            // Search all users/groups in directoryentry
            var rootPath = group.Path;
            rootPath = rootPath.Substring(0, rootPath.IndexOf("/", 7) + 1);
            searchedGroups.Add(group.Path);

            for (var i = 0; i < group.Properties["member"].Count; i++)
            {
                var currentEntry = new DirectoryEntry(rootPath + group.Properties["member"][i]);
                if (GetAccountType(currentEntry) == AdAccountType.User)
                {
                    // add to eal
                    var values = currentEntry.Properties["mail"];
                    if (values.Count > 0)
                    {
                        var email = (string)values[0];
                        if (!eal.Contains(email))
                        {
                            try
                            {
                                eal.Add(email);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else
                {
                    // see if we already had the group
                    if (!searchedGroups.Contains(currentEntry.Path))
                    {
                        GetUsersInGroup(currentEntry, eal, searchedGroups);
                    }
                }
            }
        }

        #endregion
    }
}