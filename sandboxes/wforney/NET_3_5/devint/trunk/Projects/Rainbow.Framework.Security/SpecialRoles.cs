namespace Rainbow.Framework.Security
{
    using System;
    using System.ComponentModel;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Special Roles
    /// </summary>
    public class SpecialRoles
    {
        #region Enums

        /// <summary>
        /// Special roles used by the Rainbow system with arbitrary
        ///     value for their ID by Brian Kierstead 4/15/2005
        /// </summary>
        public enum SpecialPortalRoles
        {
            /// <summary>
            /// The all users.
            /// </summary>
            [Description("All Users")]
            AllUsers = -1, 

            /// <summary>
            /// The authenticated users.
            /// </summary>
            [Description("Authenticated Users")]
            AuthenticatedUsers = -2, 

            /// <summary>
            /// The unauthenticated users.
            /// </summary>
            [Description("Unauthenticated Users")]
            UnauthenticatedUsers = -3
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve the description tag from the enum
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The get description.
        /// </returns>
        public static string GetDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Return the description for the enum entry with value index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The get role name.
        /// </returns>
        public static string GetRoleName(int index)
        {
            var s = Enum.GetName(typeof(SpecialPortalRoles), index);
            var desc = (SpecialPortalRoles)Enum.Parse(typeof(SpecialPortalRoles), s);
            return GetDescription(desc);
        }

        /// <summary>
        /// Add the special roles found in SpecialPortalRoles
        /// </summary>
        /// <param name="listRoles">
        /// The list roles.
        /// </param>
        public static void populateSpecialRoles(ref CheckBoxList listRoles)
        {
            foreach (var s in Enum.GetNames(typeof(SpecialPortalRoles)))
            {
                var desc = (SpecialPortalRoles)Enum.Parse(typeof(SpecialPortalRoles), s);
                var stringDesc = GetDescription(desc);
                listRoles.Items.Add(
                    new ListItem(stringDesc, ((int)Enum.Parse(typeof(SpecialPortalRoles), s)).ToString()));
            }
        }

        #endregion
    }
}