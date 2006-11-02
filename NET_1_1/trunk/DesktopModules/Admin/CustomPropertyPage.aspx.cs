using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI;
using HyperLink = Esperantus.WebControls.HyperLink;
using LinkButton = Esperantus.WebControls.LinkButton;
using Literal = Esperantus.WebControls.Literal;
using Page = System.Web.UI.Page;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Summary description for Property Page
	/// </summary>
	public class PageCustomPropertyPage : PropertyPageCustom
	{
		protected PlaceHolder PlaceHolderButtons;
        protected Panel EditPanel;
        protected HyperLink adminPropertiesButton;
		protected Literal Literal1;
		protected PlaceHolder AddEditControl;
		protected SettingsTable EditTable;
        protected LinkButton saveAndCloseButton;

		#region Web Form Designer generated code
        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//Controls must be created here
			updateButton = new LinkButton();
			updateButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(updateButton);

			PlaceHolderButtons.Controls.Add(new LiteralControl("&#160;"));
			saveAndCloseButton = new LinkButton();
			saveAndCloseButton.TextKey = "SAVE_AND_CLOSE";
			saveAndCloseButton.Text = "Save and close";
			saveAndCloseButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(saveAndCloseButton);
			this.saveAndCloseButton.Click += new EventHandler(this.saveAndCloseButton_Click);

			PlaceHolderButtons.Controls.Add(new LiteralControl("&#160;"));

			cancelButton = new LinkButton();
			cancelButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(cancelButton);

			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.EditTable.UpdateControl += new UpdateControlEventHandler(this.EditTable_UpdateControl);
			this.Load += new EventHandler(this.PageCustomPropertyPage_Load);

		}
		#endregion

        private void PageCustomPropertyPage_Load(object sender, EventArgs e)
        {
			EditTable.DataSource = new SortedList(ModuleSettingsCustom.GetModuleUserSettings(this.ModuleID, int.Parse(PortalSettings.CurrentUser.Identity.ID),this));
            EditTable.DataBind();
        }

		private void saveAndCloseButton_Click(object sender, EventArgs e)
		{
			OnUpdate(e);
			if (Page.IsValid == true) 
				Response.Redirect(HttpUrlBuilder.BuildUrl("~/Default.aspx", PageID));
        }    
   
		/// <summary>
		/// Persists the changes to database
		/// </summary>
        protected override void OnUpdate(EventArgs e) 
        {
			base.OnUpdate(e);

            // Only Update if Input Data is Valid
            if (Page.IsValid == true) 
            {
                // Update settings in the database
                EditTable.UpdateControls();
            }
        }

        protected override void OnCancel(EventArgs e) 
        {
			Response.Redirect(HttpUrlBuilder.BuildUrl("~/Default.aspx", PageID));
		}

        private void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
        {
            ModuleSettingsCustom.UpdateCustomModuleSetting(ModuleID, int.Parse(PortalSettings.CurrentUser.Identity.ID), e.CurrentItem.EditControl.ID, e.CurrentItem.Value);
        }
	}
}