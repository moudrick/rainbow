using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using Rainbow.Framework;
using Rainbow.Framework.Core.DAL;
using Rainbow.Framework.Settings;
using History=Rainbow.Framework.History;

namespace Rainbow.Setup 
{
    /// <summary>
    /// Summary description for Setup.
    /// This code copyright 2003 by DUEMETRI
    /// Exclusive use with Rainbowportal
    /// Any other use strictly prohibited
    /// </summary>
    [History( "jminond", "2006/02/22", "Converted to partial class" )]
    public partial class Update : Page 
    {
        #region Web Form Designer generated code

        protected override void OnInit( EventArgs e ) {
            //always modified
            Response.AddHeader( "Last-Modified", DateTime.Now.ToUniversalTime().ToString() + " GMT" );
            //HTTP/1.1 
            Response.AddHeader( "Cache-Control", "no-cache, must-revalidate" );
            //HTTP/1.0 
            Response.AddHeader( "Pragma", "no-cache" );
            //last ditch attempt!
            Response.Expires = -100;

            this.authenticateUser.Click += new EventHandler( this.authenticateUser_Click );
            this.Button1.Click += new EventHandler( this.FinishButton_Click );
            this.UpdateDatabaseCommand.Click += new EventHandler( this.UpdateDatabaseCommand_Click );
            this.FinishButton.Click += new EventHandler( this.FinishButton_Click );
            this.Load += new EventHandler( this.Update_Load );
            base.OnInit( e );
        }

        #endregion

        DatabaseUpdater updater;

        /// <summary>
        /// Handles the Load event of the Update control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Update_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                //jes1111 - if (ConfigurationSettings.AppSettings["UpdateUserName"] == null || ConfigurationSettings.AppSettings["UpdateUserName"].Length == 0)
                if (Config.UpdateUserName == string.Empty)
                {
                    //No login is shown if no user is specified
                    //by manu
                    AuthenticationPanel.Visible = false;
                    InfoPanel.Visible = true;
                    UpdatePanel.Visible = true;
                }
                else {
                    AuthenticationPanel.Visible = true;
                    InfoPanel.Visible = false;
                    UpdatePanel.Visible = false;
                }
            }
            else 
            {
                //Hide infopanel when update starts....
                InfoPanel.Visible = false;
            }

            dbNeedsUpdate.Visible = false;

            updater = new DatabaseUpdater(Server.MapPath(Path.ApplicationRoot));
            updater.PreviewUpdate();
            lblVersion.Text = updater.InitialStatusReport;
            if (updater.UpdateList.Count > 0)
            {
                //Some update to do
                dlScripts.DataSource = updater.UpdateList; //we bind a simple list
                dlScripts.DataBind();
                dbNeedsUpdate.Visible = true;
                dbNoUpdate.Visible = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the UpdateDatabaseCommand control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateDatabaseCommand_Click( object sender, EventArgs e ) 
        {
            updater.PerformUpdate();

            dbUpdateResult.Visible = false;
            dbNeedsUpdate.Visible = false;
            Status.Visible = true;

            if (updater.Messages.Count > 0)
            {
                dlErrors.DataSource = updater.Messages;
                dlErrors.DataBind();
                dlErrors.Visible = true;
                dlErrors.ForeColor = Color.Green;
                dbUpdateResult.Visible = true;

                dbUpdateResult.Visible = true;
                dbNeedsUpdate.Visible = false;
            }

            if (updater.Errors.Count > 0)
            {
                dlErrors.DataSource = updater.Errors;
                dlErrors.DataBind();
                dlErrors.Visible = true;
                dlErrors.ForeColor = Color.Red;

                dbUpdateResult.Visible = true;
                dbNeedsUpdate.Visible = false;
                Status.Visible = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the FinishButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FinishButton_Click( object sender, EventArgs e ) {
            ErrorHandler.Publish( LogLevel.Info, "Update complete" );

            //Global.dbNeedsUpdate = false;
            Response.Redirect( Path.ApplicationRoot + "/Default.aspx" );
        }

        /// <summary>
        /// Handles the Click event of the authenticateUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void authenticateUser_Click( object sender, EventArgs e ) {
            // jes1111 - if (ConfigurationSettings.AppSettings["UpdateUserName"] != null)
            string providedUser = Config.UpdateUserName;

            //jes1111 - if (ConfigurationSettings.AppSettings["UpdatePassword"] != null)
            string providedPassword = Config.UpdatePassword;

            //			if(providedUser.ToLower().Equals(updateUsername.Text.ToLower()) && providedPassword.Equals(updatePassword.Text))
            if ( String.Compare( providedUser, updateUsername.Text, true ) == 0 &&
                String.Compare( providedPassword, updatePassword.Text ) == 0 ) {
                AuthenticationPanel.Visible = false;
                UpdatePanel.Visible = true;
            }
            else {
                loginError.Visible = true;
                ErrorHandler.Publish( LogLevel.Warn,
                    "Someone has incorrectly tried to log into the setup / update page. User IP: '" +
                    HttpContext.Current.Request.UserHostAddress +
                    "' Username Entered: '" + updateUsername.Text +
                    "' Password Entered: '" + updatePassword.Text + "'" );
            }
        }
    }
}
