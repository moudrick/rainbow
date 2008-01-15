using System;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.DataTypes;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Content.Web.Modules
{
    /// <summary>
    /// Quiz Module
    /// </summary>
    public partial class Quiz : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            lnkQuiz.Text = Settings["QuizName"].ToString();
            lnkQuiz.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Quiz/QuizPage.aspx", "mID=" + ModuleID);
        }

        /// <summary>
        /// Contstructor
        /// </summary>
        public Quiz()
        {
            SettingItem quizName = new SettingItem(new StringDataType());
            quizName.Required = true;
            quizName.Order = 1;
            quizName.Value = "About Australia (Demo1)";
            baseSettings.Add("QuizName", quizName);

            Portal portal = PortalProvider.Instance.CurrentPortal;
            PortalUrl portalUrl = portal != null ? portal.PortalUrl : new PortalUrl(string.Empty);
            SettingItem xmLsrc = new SettingItem(portalUrl);
            xmLsrc.Required = true;
            xmLsrc.Order = 2;
            xmLsrc.Value = "/Quiz/Demo1.xml";
            baseSettings.Add("XMLsrc", xmLsrc);
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531050}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}
