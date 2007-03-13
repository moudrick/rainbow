using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Settings;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Label = Esperantus.WebControls.Label;

namespace Rainbow
{
    /// <summary>
    /// Module print page
    /// </summary>
    public class recyclerViewPage : ViewItemPage
    {
		protected PlaceHolder PrintPlaceHolder;
		protected DropDownList ddTabs;
		protected LinkButton restoreButton;
		private int _moduleID;

		// TODO check if this works
		//protected ArrayList portalTabs;
		protected DataTable portalTabs;
		protected Panel pnlMain;
		protected Label Label1;
		protected Panel pnlError;
		protected ModuleSettings module;

		private void Page_Load(object sender, EventArgs e)
		{
			this.deleteButton.Visible = true;
			this.updateButton.Visible = false;
		
			try
			{
				_moduleID = int.Parse(this.Request.Params["mID"]);
		
				module = RecyclerDB.GetModuleSettingsForIndividualModule(_moduleID);
				if (RecyclerDB.ModuleIsInRecycler(_moduleID))
				{
					if (!Page.IsPostBack)
					{
						//load tab names for the dropdown list, then bind them
						// TODO check if this works
						//portalTabs = new PagesDB().GetPagesFlat(portalSettings.PortalID);
						portalTabs = new PagesDB().GetPagesFlatTable(portalSettings.PortalID);
						
						this.ddTabs.DataBind();

                        //on initial load, disable the restore button until they make a selection
						this.restoreButton.Enabled = false;
						this.ddTabs.Items.Insert(0,"--Choose a Tab to Restore this Module--");
					}			
					
					// create an instance of the module
					PortalModuleControl myPortalModule = (PortalModuleControl) LoadControl(Path.ApplicationRoot + "/" + module.DesktopSrc);
					myPortalModule.PortalID = portalSettings.PortalID;                                  
					myPortalModule.ModuleConfiguration = module;
					
					// add the module to the placeholder
					PrintPlaceHolder.Controls.Add(myPortalModule);
				}			
				else  //they're trying to view a module that isn't in the recycler - maybe a manual manipulation of the url...?
				{
					this.pnlMain.Visible = false;
					this.pnlError.Visible = true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			
		}

		protected override void OnDelete(EventArgs e)
		{
			base.OnDelete (e);
            
			ModulesDB modules = new ModulesDB();
			// TODO add userEmail and useRecycler
			modules.DeleteModule(_moduleID);

			this._moduleID = 0;

			this.RedirectBackToReferringPage();
		}


		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);

		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
			this.ddTabs.SelectedIndexChanged += new EventHandler(this.ddTabs_SelectedIndexChanged);
			this.restoreButton.Click += new EventHandler(this.restoreButton_Click);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		protected void ddTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ddTabs.SelectedIndex == 0)
				this.restoreButton.Enabled = false;
			else
				this.restoreButton.Enabled = true;
		}

		protected void restoreButton_Click(object sender, EventArgs e)
		{
			RecyclerDB.MoveModuleToNewTab(int.Parse(this.ddTabs.SelectedValue),this._moduleID);
			this.RedirectBackToReferringPage();
		}
    }
}