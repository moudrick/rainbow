using System;
using System.Collections;
using System.Web.UI.WebControls;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Web.UI;
using History = Rainbow.Framework.History;
using System.Web.Security;

namespace Rainbow.Content.Web.Modules 
{
    /// <summary>
    /// The SecurityRoles.aspx page is used to create and edit
    /// security roles within the Portal application.
    /// </summary>
    [History( "jminond", "march 2005", "Changes for moving Tab to Page" )]
    public partial class SecurityRoles : EditItemPage {
        private Guid roleId = Guid.Empty;

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override ArrayList AllowedModules {
            get {
                ArrayList al = new ArrayList();
                al.Add( "A406A674-76EB-4BC1-BB35-50CD2C251F9C" );
                return al;
            }
        }

        #region Event handlers

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to populate the role information for the page
        /// </summary>
        protected override void OnLoad( EventArgs e ) {
            base.OnLoad( e );

            if ( Request.Params[ "roleID" ] != null ) {
                roleId = new Guid( Request.Params[ "roleID" ] );
            }

            // If this is the first visit to the page, bind the role data to the datalist
            if ( !Page.IsPostBack ) {
                BindData();
            }
        }

        /// <summary>
        /// The Save_Click server event handler on this page is used
        /// to save the current security settings to the configuration system
        /// </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click(Object Sender, EventArgs e)
        {
            // Navigate back to admin page
            Response.Redirect(HttpUrlBuilder.BuildUrl(PageID));
        }

        /// <summary>
        /// The AddUser_Click server event handler is used to add
        /// a new user to this security role.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddUser_Click(Object sender, EventArgs e)
        {
            //get user id from dropdownlist of existing users
            Guid userID = new Guid(allUsers.SelectedItem.Value);
            if (!userID.Equals(Guid.Empty))
            {
                AccountSystem.Instance.AddUserRole(roleId, userID);
            }
            BindData(); // Rebind list
        }

        /// <summary>
        /// The usersInRole_ItemCommand server event handler on this page
        /// is used to handle the user editing and deleting roles
        /// from the usersInRole asp:datalist control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataListCommandEventArgs"/> instance containing the event data.</param>
        protected void usersInRole_ItemCommand(object sender, DataListCommandEventArgs e)
        {
            Label lblUserEmail = (Label) e.Item.FindControl("lblUserEmail");
            RainbowUser user = (RainbowUser) Membership.GetUser(lblUserEmail.Text);

            if (e.CommandName == "delete")
            {
                AccountSystem.Instance.DeleteUserRole(roleId, user.ProviderUserKey); // update database
                usersInRole.EditItemIndex = -1;  // Ensure that item is not editable
                BindData(); // Repopulate list
            }
        }

        #endregion

        /// <summary>
        /// The BindData helper method is used to bind the list of
        /// security roles for this portal to an asp:datalist server control
        void BindData()
        {
            // add the role name to the title
            if (roleId != Guid.Empty)
            {
                RainbowRoleProvider roleProvider = (RainbowRoleProvider) System.Web.Security.Roles.Provider;
                RainbowRole role = roleProvider.GetRoleById(roleId);

                title.InnerText = General.GetString("ROLE_MEMBERSHIP") + role.Name;
            }

            string portalAlias = PortalProvider.Instance.CurrentPortal.PortalAlias;
            usersInRole.DataSource = RainbowRoleProvider.Instance.GetUsersInRole(portalAlias, roleId);
            usersInRole.DataBind();

            allUsers.DataSource = RainbowMembershipProvider.Instance.GetUsers(portalAlias);
            allUsers.DataBind();
        }
    }
}
