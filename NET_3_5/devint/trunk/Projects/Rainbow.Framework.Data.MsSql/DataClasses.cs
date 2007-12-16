namespace Rainbow.Framework.Data.MsSql
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    partial class DataClassesDataContext
    {
        /// <summary>
        /// Get All Membership Users For Application with paging support
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="userList">The user list.</param>
        /// <returns>count of users</returns>
        public int aspnet_Membership_GetAllUsers(string applicationName, int pageIndex, int pageSize, out List<User> userList)
        {
            userList = new List<User>();

            Guid? applicationId = GetApplicationId(applicationName);

            if (applicationId == null)
            {
                return 0;
            }

            int pageLowerBound, pageUpperBound, totalRecords;

            pageLowerBound = pageSize * pageIndex;
            pageUpperBound = pageSize - 1 + pageLowerBound;

            DataClassesDataContext db = new DataClassesDataContext();

            var piQuery = from u in db.Users
                          where u.aspnet_Membership.ApplicationId == applicationId
                          orderby u.UserName
                          select u;
            totalRecords = piQuery.Count();

            Dictionary<int, User> users = new Dictionary<int, User>();
            int i = 1;
            foreach (User u in piQuery)
            {
                users.Add(i, u);
                i++;
            }

            var ulQuery = from u in users
                          where u.Key >= pageLowerBound && u.Key <= pageUpperBound
                          orderby u.Value.UserName
                          select u;

            foreach (var u in ulQuery)
            {
                userList.Add(u.Value);
            }

            return totalRecords;
        }

        /// <summary>
        /// Finds Membership User(s) By Name
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="userNameToMatch">The user name to match.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="userList">The user list.</param>
        /// <returns>count of users</returns>
        public int aspnet_Membership_FindUsersByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize, out List<User> userList)
        {
            userList = new List<User>();

            Guid? applicationId = GetApplicationId(applicationName);

            if (applicationId == null)
            {
                return 0;
            }

            int pageLowerBound, pageUpperBound, totalRecords;

            pageLowerBound = pageSize * pageIndex;
            pageUpperBound = pageSize - 1 + pageLowerBound;

            DataClassesDataContext db = new DataClassesDataContext();
            var piQuery = from u in db.Users
                          where u.aspnet_Membership.ApplicationId == applicationId &&
                                u.aspnet_User.LoweredUserName.StartsWith(userNameToMatch)
                          orderby u.UserName
                          select u;
            totalRecords = piQuery.Count();

            Dictionary<int, User> users = new Dictionary<int, User>();
            int i = 1;
            foreach (User u in piQuery)
            {
                users.Add(i, u);
                i++;
            }

            var ulQuery = from u in users
                          where u.Key >= pageLowerBound && u.Key <= pageUpperBound
                          orderby u.Value.UserName
                          select u;

            foreach (var u in ulQuery)
            {
                userList.Add(u.Value);
            }

            return totalRecords;
        }

        /// <summary>
        /// Finds Membership User(s) By Email
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="emailToMatch">The email to match.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="userList">The user list.</param>
        /// <returns>count of users</returns>
        public int aspnet_Membership_FindUsersByEmail(string applicationName, string emailToMatch, int pageIndex, int pageSize, out List<User> userList)
        {
            userList = new List<User>();

            Guid? applicationId = GetApplicationId(applicationName);

            if (applicationId == null)
            {
                return 0;
            }

            int pageLowerBound, pageUpperBound, totalRecords;

            pageLowerBound = pageSize * pageIndex;
            pageUpperBound = pageSize - 1 + pageLowerBound;

            DataClassesDataContext db = new DataClassesDataContext();
            var piQuery = from u in db.Users
                          where u.aspnet_Membership.ApplicationId == applicationId &&
                                (string.IsNullOrEmpty(emailToMatch) ? u.Email == null : u.aspnet_Membership.LoweredEmail.StartsWith(emailToMatch))
                          orderby u.aspnet_Membership.LoweredEmail
                          select u;

            totalRecords = piQuery.Count();

            Dictionary<int, User> users = new Dictionary<int, User>();
            int i = 1;
            foreach (User u in piQuery)
            {
                users.Add(i, u);
                i++;
            }

            var ulQuery = from u in users
                          where u.Key >= pageLowerBound && u.Key <= pageUpperBound
                          orderby u.Value.UserName
                          select u;

            foreach (var u in ulQuery)
            {
                userList.Add(u.Value);
            }

            return totalRecords;
        }

        /// <summary>
        /// Gets the application id.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public Guid? GetApplicationId(string applicationName)
        {
            Guid? applicationId = null;
            DataClassesDataContext db = new DataClassesDataContext();

            var appIdQuery = from appId in db.aspnet_Applications
                             where appId.LoweredApplicationName == applicationName.ToLower()
                             select appId;

            applicationId = appIdQuery.Single().ApplicationId;

            return applicationId;
        }
    }
}