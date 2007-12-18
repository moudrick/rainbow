using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;
using Path = Rainbow.Settings.Path;

namespace Rainbow.DesktopModules 
{

	/// <summary>
	/// Users Defined Table module
	/// Written by: Shaun Walker (IbuySpy Workshop)
	/// Moved into Rainbow by Jakob Hansen, hansen3000@hotmail.com
	/// </summary>
	[History("jminond", "march 2005", "Changes for moving Tab to Page")]
	[History("Ozan", "2004/07/04", "FIX: It does not show sort images when running root site instead of rainbow virtual folder.")]
	[History("Ender", "2003/03/18", "Added file and Xsl functionality")]
	[History("mario@hartmann.net", "2004/05/28", "Added image functionality")]
	[History("RSiera", "2004/11/29", "Fixed image functionality")]
	[History("RSiera", "2004/11/29", "Added DisplayAsXML functionality")]
	[History("RSiera", "2004/11/29", "Added UseDataOfOtherUDT functionality")]
	[History("RSiera", "2004/12/01", "Added Sorting functionality for XSL data (see example.txt)")]
	public class UserDefinedTable : PortalModuleControl 
	{
		protected DataGrid grdData;
		protected UserDefinedTableXML xmlControl;
		protected IHtmlEditor xmlText;
		protected LinkButton cmdManage;
		protected PlaceHolder	PlaceHolderOutput;

		/// <summary>
		/// The Page_Load event handler on this User Control is used to
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e) 
		{
			if (!Page.IsPostBack )
			{
				ViewState["SortField"] = string.Empty;
				ViewState["SortOrder"] = string.Empty;

				if ( IsEditable )
					cmdManage.Visible = true;
				else
					cmdManage.Visible = false;
			}
 
			if(Settings["XSLsrc"].ToString().Length >0 || bool.Parse(Settings["DisplayAsXML"].ToString())==true)
			{
				xmlControl = new UserDefinedTableXML(XmlDataView(),PageID,ModuleIDsrc(),IsEditable,GetSortField(),GetSortOrder());
				xmlControl.SortCommand += new UDTXSLSortEventHandler(xmlData_Sort);

				if(bool.Parse(Settings["DisplayAsXML"].ToString())==true)
				{
					//Show Raw XML
					xmlText = new TextEditor();
					xmlText.Text = xmlControl.InnerXml;
					xmlText.Width = 450;
					xmlText.Height = 400;				
					PlaceHolderOutput.Controls.Add((Control) xmlText);
				}
				else
				{
					//Show XSL data
					PlaceHolderOutput.Controls.Add(xmlControl);
					BindXSL();
				}	
			}
			else
			{
				//Show datagrid
				grdData = new DataGrid();
				grdData.BorderWidth = 0;
				grdData.CellPadding = 4;
				grdData.AutoGenerateColumns = false;
				grdData.HeaderStyle.CssClass = "NormalBold";
				grdData.ItemStyle.CssClass = "Normal";
				grdData.AllowSorting = true;
				grdData.SortCommand += new DataGridSortCommandEventHandler(grdData_Sort);
				PlaceHolderOutput.Controls.Add(grdData);
				BindGrid();
			}
			//Rob Siera - 04 nov 2004 - Show ManageButton only when having Permission to edit Properties
			cmdManage.Visible=ArePropertiesEditable;
        }

		
		private DataView XmlDataView()
		{
			UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();
			DataSet ds;

			ds = objUserDefinedTable.GetUserDefinedRows(ModuleIDsrc());

			// create a dataview to process the sort and filter options
			return new DataView(ds.Tables[0]);
		}

		public void BindXSL()
		{
			PortalUrlDataType pt = new PortalUrlDataType();
			pt.Value = Settings["XSLsrc"].ToString();
			string xslsrc = pt.FullPath;

			if ((xslsrc != null) && (xslsrc.Length != 0)) 
			{
				if  (File.Exists(Server.MapPath(xslsrc))) 
				{
					xmlControl.TransformSource = xslsrc;
					// Change - 28/Feb/2003 - Jeremy Esland
					// Builds cache dependency files list
					this.ModuleConfiguration.CacheDependency.Add(Server.MapPath(xslsrc));
				}
				else 
				{
					xmlControl.TransformSource = Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/default.xslt");
					Controls.Add(new LiteralControl("<br>" + "<span class='Error'>" + Localize.GetString("FILE_NOT_FOUND").Replace("%1%", xslsrc) + "<br>"));
				}
			}

		}


		private int ModuleIDsrc()
		{
			//Rob Siera - 04 nov 2004 - Adding possibility to use data of other UDT when XSL specified
			if(int.Parse(Settings["UDTsrc"].ToString())>0)
			{
				return int.Parse(Settings["UDTsrc"].ToString());
			}
			else
			{
				return ModuleID;
			}
		}
		public string GetSortField()
		{
			if(xmlControl!=null && xmlControl.SortField.Length != 0)
			{
				return xmlControl.SortField;
			}
			else if ( ViewState["SortField"].ToString().Length != 0  )
			{
				return ViewState["SortField"].ToString();
			} 
			else if ( Settings["SortField"].ToString().Length != 0 )
			{
				return Settings["SortField"].ToString();
			}
			return string.Empty;
		}
		public string GetSortOrder()
		{

			if(xmlControl!=null && xmlControl.SortOrder.Length != 0)
			{
				return xmlControl.SortOrder;
			}
			else if (  ViewState["SortOrder"].ToString().Length != 0 )
			{
				return ViewState["SortOrder"].ToString();
			} 
			else if ( Settings["SortField"].ToString().Length != 0 )
			{
				return Settings["SortOrder"].ToString();
			}
			return "ASC";
		}		


		protected void BindGrid()
		{
            UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();

            string strSortField = string.Empty;
            string strSortOrder = string.Empty;

			SqlDataReader dr;

            if ( ViewState["SortField"].ToString().Length != 0 && ViewState["SortOrder"].ToString().Length != 0 )
			{
                strSortField = ViewState["SortField"].ToString();
                strSortOrder = ViewState["SortOrder"].ToString();
            } 
			else
			{
                if ( Settings["SortField"].ToString().Length != 0 )
					strSortField = Settings["SortField"].ToString();

				if ( Settings["SortOrder"].ToString().Length != 0 )
                    strSortOrder = Settings["SortOrder"].ToString();
                else 
                    strSortOrder = "ASC";
            }

			grdData.Columns.Clear();

            dr = objUserDefinedTable.GetUserDefinedFields(ModuleID);
			try
			{
				while ( dr.Read())
				{
					DataGridColumn colField = null;
					if(dr["FieldType"].ToString() == "Image")
					{
						colField = new BoundColumn();
						((BoundColumn)colField).DataField = dr["FieldTitle"].ToString();
						((BoundColumn)colField).DataFormatString = "<img src=\"" +((SettingItem)Settings["ImagePath"]).FullPath + "/{0}" + "\" alt=\"{0}\" border =0>" ;
					}
					else if(dr["FieldType"].ToString() == "File")
					{
						colField = new HyperLinkColumn();
						((HyperLinkColumn)colField).DataTextField = dr["FieldTitle"].ToString();
						((HyperLinkColumn)colField).DataTextFormatString = "{0}";
						((HyperLinkColumn)colField).DataNavigateUrlFormatString = ((SettingItem) Settings["DocumentPath"]).FullPath + "/{0}";
						((HyperLinkColumn)colField).DataNavigateUrlField = dr["FieldTitle"].ToString();
					}
					else
					{
						colField = new BoundColumn();
						((BoundColumn)colField).DataField = dr["FieldTitle"].ToString();
						switch (dr["FieldType"].ToString())
						{ 
							case "DateTime":
								//Changed to Italian format as it is sayed to be the default (see intro of history.txt)
								//Better would be to make this follow the current culture - Rob Siera, 15 jan 2005
								((BoundColumn)colField).DataFormatString = "{0:dd MMM yyyy}";
								break;
							case "Int32":
								((BoundColumn)colField).DataFormatString = "{0:#,###,##0}";
								colField.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
								colField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
								break;
							case "Decimal":
								((BoundColumn)colField).DataFormatString = "{0:#,###,##0.00}";
								colField.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
								colField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
								break;
						}
					}

					colField.HeaderText = dr["FieldTitle"].ToString();
					if ( dr["FieldTitle"].ToString() == strSortField)
					{
						//  2004/07/04 by Ozan Sirin, FIX: It does not show sort images when running root site instead of rainbow virtual folder.
						if ( strSortOrder == "ASC" )
							colField.HeaderText += "<img src='" + Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortascending.gif") + "' border='0' alt='" + Localize.GetString("USERTABLE_SORTEDBY", "Sorted By", null) + " " + strSortField + " " + Localize.GetString("USERTABLE_INASCORDER", "In Ascending Order", null) + "'>";
						else
							colField.HeaderText += "<img src='" + Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortdescending.gif") +  "' border='0' alt='" + Localize.GetString("USERTABLE_SORTEDBY", "Sorted By", null) + " " + strSortField + " " + Localize.GetString("USERTABLE_INDSCORDER", "In Descending Order", null) + "'>";
					}
					colField.Visible = bool.Parse(dr["Visible"].ToString());
					colField.SortExpression = dr["FieldTitle"].ToString() + "|ASC";

					grdData.Columns.Add(colField);
				}
			}
			finally
			{
				dr.Close();
			}

			if(IsEditable)
			{
				HyperLinkColumn hc = new HyperLinkColumn();
				hc.Text = "Edit";
				hc.DataNavigateUrlField = "UserDefinedRowID";
				hc.DataNavigateUrlFormatString = HttpUrlBuilder.BuildUrl("~/DesktopModules/UserDefinedTable/UserDefinedTableEdit.aspx", PageID, "&mID=" + ModuleID + "&UserDefinedRowID={0}"); 
				grdData.Columns.Add(hc);
			}

            DataSet ds;
            ds = objUserDefinedTable.GetUserDefinedRows(ModuleID);

            // create a dataview to process the sort and filter options
			DataView dv;
			dv = new DataView(ds.Tables[0]);

            // sort data view
            if ( strSortField.Length != 0 && strSortOrder.Length != 0 )
                dv.Sort = strSortField + " " + strSortOrder;

            grdData.DataSource = dv;
            grdData.DataBind();
        }

		protected void grdData_Sort(Object source, DataGridSortCommandEventArgs e)
		{

			string[] strSort = e.SortExpression.Split('|');

            if ( strSort[0] == ViewState["SortField"].ToString() )
			{
                if ( ViewState["SortOrder"].ToString() == "ASC" )
                    ViewState["SortOrder"] = "DESC";
                else
                    ViewState["SortOrder"] = "ASC";
            } 
			else
			{
                ViewState["SortOrder"] = strSort[1];
            }

            ViewState["SortField"] = strSort[0];

            BindGrid();

        }

		protected void xmlData_Sort(Object source, UDTXSLSortEventArgs e)
		{
			xmlControl.SortField=e.SortField;
			xmlControl.SortOrder=e.SortOrder;

			BindXSL();

			ViewState["SortField"] = e.SortField;
			ViewState["SortOrder"] = e.SortOrder;
		}


		protected void cmdManage_Click(object sender, EventArgs e)
		{
			Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/UserDefinedTable/UserDefinedTableManage.aspx" ,PageID, "&mID=" + ModuleIDsrc() + "&def=Manage UDT"));
        }
		
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531021}");
			}
		}
        
		/// <summary>
		/// Public constructor. Sets base settings for module.
		/// </summary>
		public UserDefinedTable() 
		{
			SettingItem setSortField = new SettingItem(new StringDataType());
			setSortField.Required = false;
			setSortField.Value = string.Empty;
			setSortField.Order = 1;
			this._baseSettings.Add("SortField", setSortField);

			SettingItem setSortOrder = new SettingItem(new ListDataType("ASC;DESC"));
			setSortOrder.Required = true;
			setSortOrder.Value = "ASC";
			setSortOrder.Order = 2;
			this._baseSettings.Add("SortOrder", setSortOrder);

			SettingItem DocumentPath = new SettingItem(new PortalUrlDataType());
			DocumentPath.Required = true;
			DocumentPath.Value = "Documents";
			DocumentPath.Order = 3;
			this._baseSettings.Add("DocumentPath", DocumentPath);

			SettingItem ImagePath = new SettingItem(new PortalUrlDataType());
			ImagePath.Required = true;
			ImagePath.Value = "Images\\Default";
			ImagePath.Order = 4;
			this._baseSettings.Add("ImagePath", ImagePath);

			SettingItem XSLsrc = new SettingItem(new PortalUrlDataType());
			XSLsrc.Required = false;
			XSLsrc.Order = 5;
			this._baseSettings.Add("XSLsrc", XSLsrc);

			//Rob Siera - 04 nov 2004 - Adding possibility to use data of other UDT
			SettingItem UDTsrc = new SettingItem(new IntegerDataType());
			UDTsrc.Required = false;
			UDTsrc.Value = ModuleID.ToString();
			UDTsrc.EnglishName="XSL data";
			UDTsrc.Description="Specify ModuleID of a UserDefinedTable to be used as data source for XSL (see 'mID' parameter in edit URL). Specify 0 to reset to current module data.";
			UDTsrc.Order = 6;
			this._baseSettings.Add("UDTsrc", UDTsrc);

			//Rob Siera - 04 nov 2004 - Adding possibility to view data as raw XML
			SettingItem DisplayAsXML = new SettingItem(new BooleanDataType());
			DisplayAsXML.Required = false;
			DisplayAsXML.EnglishName="Display XML";
			DisplayAsXML.Description="Toggle to display data as XML. Helpfull to develop XSL file.";
			DisplayAsXML.Order = 7;
			this._baseSettings.Add("DisplayAsXML", DisplayAsXML);
		}
		
		
		public override void Install(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}

		
		public override void Uninstall(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}


		#region Web Form Designer generated code
		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			this.AddUrl = "~/DesktopModules/UserDefinedTable/UserDefinedTableEdit.aspx";
			base.OnInit(e);
		}

		private void InitializeComponent() 
		{
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

    }

}