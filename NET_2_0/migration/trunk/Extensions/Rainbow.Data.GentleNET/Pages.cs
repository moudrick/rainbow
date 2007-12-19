using System;
using System.Collections;
using System.ComponentModel;
using System.Xml;
using Rainbow.Core;

namespace Rainbow.Data.GentleNET
{
	/// <summary>
	/// Summary description for Tabs.
	/// </summary>
	public class Pages : PagesProvider
	{
		private const string strAllUsers = "All Users;";

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public override int Add(int portalID, String tabName, int tabOrder)
		{
			return Add(portalID, tabName, strAllUsers, tabOrder);
		}


		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="Roles"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public override int Add(int portalID, String tabName, string roles, int tabOrder)
		{
			if (tabName == null || tabName.Length == 0) tabName = "New Tab";
			if (tabName.Length > 50) tabName = tabName.Substring(0, 49);

			rb_Tabs t = new rb_Tabs(portalID, tabName, roles, tabOrder);
			t.Persist();

			return t.TabID;
		}


		/// <summary>
		/// The DeleteTab method deletes the selected tab from the portal.<br />
		/// DeleteTab Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		public override void Remove(int tabID)
		{
			rb_Tabs t = rb_Tabs.Retrieve(tabID);

			if (t != null)
				t.Remove();
		}


		/// <summary>
		/// Update Method<br />
		/// was UpdateTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabID"></param>
		/// <param name="parentTabID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <param name="authorizedRoles"></param>
		/// <param name="mobileTabName"></param>
		/// <param name="showMobile"></param>
		public override void Update(int portalID, int tabID, int parentTabID,
		                            String tabName, int tabOrder, String authorizedRoles,
		                            String mobileTabName, bool showMobile)
		{
			//Fixes a missing tab name
			if (tabName == null || tabName.Length == 0) tabName = "&nbsp;";
			if (tabName.Length > 50) tabName = tabName.Substring(0, 49);

			rb_Tabs t = rb_Tabs.Retrieve(tabID);
			if (t != null)
			{
				t.PortalID = portalID;
				t.ParentTabID = parentTabID;
				t.TabName = tabName;
				t.TabOrder = tabOrder;
				t.AuthorizedRoles = authorizedRoles;
				t.MobileTabName = mobileTabName;
				t.ShowMobile = showMobile;

				t.Persist();
			}
		}


		/// <summary>
		/// The UpdateTabOrder method changes the position of the tab with respect
		/// to other tabs in the portal.<br />
		/// UpdateTabOrder Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="tabOrder"></param>
		public override void UpdateOrder(int tabID, int tabOrder)
		{
			rb_Tabs t = rb_Tabs.Retrieve(tabID);
			if (t != null)
			{
				t.TabOrder = tabOrder;

				t.Persist();
			}
		}


		/// <summary>
		/// This user control will render the breadcrumb navigation for the current tab.
		/// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
		/// </summary>
		/// <param name="tabID">ID of the tab</param>
		/// <returns></returns>
		public override ArrayList Crumbs(int tabID)
		{
			string crumbsXML = string.Empty;
			int parentTabID;
			string tabName;
			int level = 20; //First Child in the branch is Crumb 20.

			//Get First Parent Tab ID if there is one
			rb_Tabs t = rb_Tabs.Retrieve(tabID);
			if (t != null)
			{
				parentTabID = t.ParentTabID;
				tabName = t.TabName;
				//Build first Crumb
				crumbsXML = "<root><crumb TabID='" + tabID.ToString() +
					"' Level='" + level.ToString() + "'>" +
					tabName + "</crumb>";

				while (t != null)
				{
					level--;
					tabID = parentTabID;
					t = rb_Tabs.Retrieve(tabID);
					parentTabID = t.ParentTabID;
					tabName = t.TabName;
					crumbsXML += "<crumb TabID='" + tabID.ToString() +
						"' Level='" + level.ToString() + "'>" +
						tabName + "</crumb>";
				}

				crumbsXML += "</root>";
			}


			// Build a Hashtable from the XML string returned
			ArrayList Crumbs = new ArrayList();
			XmlDocument CrumbXML = new XmlDocument();
			CrumbXML.LoadXml(crumbsXML.Replace("&", "&amp;"));

			//Iterate through the Crumbs XML
			foreach (XmlNode node in CrumbXML.FirstChild.ChildNodes)
			{
				Page page = new Page();
				page.Id = Int16.Parse(node.Attributes.GetNamedItem("tabID").Value);
				page.Name = node.InnerText;
				page.Order = Int16.Parse(node.Attributes.GetNamedItem("level").Value);
				Crumbs.Add(page);
			}

			//Return the Crumb Tab Items as an arraylist 
			return Crumbs;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="pageId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public override Hashtable Settings(int pageId)
		{
			Hashtable hts = new Hashtable();

			rb_Tabs t = rb_Tabs.Retrieve(pageId);
			IList tset = t.referencedrb_TabSettingsUsingrb_Tabs();

			if (tset != null)
			{
				foreach (rb_TabSettings i in tset)
				{
					hts.Add(i.SettingName, i.SettingValue);
				}
			}

			return hts;
		}

		#region Component Model

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Pages(IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public Pages()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		/// <summary>
		///     Gets a Page object based on the page id #
		/// </summary>
		/// <param name="pageId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Page value...
		/// </returns>
		public override Page Page(int pageId)
		{
			throw new NotImplementedException();
		}
	}
}