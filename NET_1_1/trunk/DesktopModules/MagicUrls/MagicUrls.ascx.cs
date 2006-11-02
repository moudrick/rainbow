using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Esperantus;
using Rainbow.UI.WebControls;
using Path = Rainbow.Settings.Path;

namespace Rainbow.DesktopModules
{
	/// <summary>
	///	Summary description for MagicUrls.
	/// </summary>
	public class MagicUrls : PortalModuleControl
	{
		/// <summary>
		/// 
		/// </summary>
		protected XmlEditGrid XmlEditGrid1;
		/// <summary>
		/// 
		/// </summary>
		protected PlaceHolder PlaceHolder1;
		/// <summary>
		/// 
		/// </summary>
		protected PlaceHolder PlaceHolder2;

		string myFile;
		string myFolder;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			string topText = Localize.GetString("MAGICURLS_TOPTEXT", "<b>MagicUrls</b> enables users to navigate to pages within a portal by appending just a number or a string to the base URL (like www.myportal.net/2291 or www.myportal.net/special). Numbers will always be interpreted automatically as a PageID. To use a string, enter it in the 'key' column and assign it to a PageID (e.g. 2291), an internal URL (e.g. ~/site/2291/special.aspx) or an external URL (e.g. http://www.microsoft.com) in the 'value' column. ");
			topText = String.Format("<p>{0}</p>",topText);
			PlaceHolder1.Controls.Add(new LiteralControl(topText));
			string btmText = Localize.GetString("MAGICURLS_BTMTEXT", "NOTE: for this feature to work, you must assign a Custom Error in IIS - map 404 errors to '/rainbow/app_support/SmartError.aspx' (assuming rainbow is the name of your virtual directory).");
			btmText = String.Format("<p>{0}</p>",btmText);
			PlaceHolder2.Controls.Add(new LiteralControl(btmText));
			
			if ( !this.Page.IsPostBack )
			{
				XmlDocument _checkXml = new XmlDocument();

				myFolder = Path.WebPathCombine(portalSettings.PortalPath, "MagicUrl");
				if ( !Directory.Exists(Server.MapPath(myFolder)) )
					Directory.CreateDirectory(Server.MapPath(myFolder));

				myFile = Path.WebPathCombine(myFolder, "MagicUrlList.xml");
				
				if ( !File.Exists(Server.MapPath(myFile)) )
				{
					StreamWriter sr = File.CreateText(Server.MapPath(myFile));
					sr.WriteLine("<?xml version=\"1.0\" standalone=\"yes\" ?>");
					sr.WriteLine("<MagicUrlList>");
					sr.WriteLine("<MagicUrl key=\"home\" value=\"0\"/>");
					sr.WriteLine("</MagicUrlList>");
					sr.Close();
				}
				else
				{
					_checkXml.Load(Server.MapPath(myFile));
					if ( !_checkXml.DocumentElement.HasChildNodes )
					{
						//Create a document fragment
						XmlDocumentFragment docFrag = _checkXml.CreateDocumentFragment();
 						docFrag.InnerXml ="<MagicUrl key=\"home\" value=\"0\"/>";
						_checkXml.DocumentElement.AppendChild(docFrag);
						XmlTextWriter x = new XmlTextWriter(Server.MapPath(myFile), Encoding.UTF8);
						_checkXml.WriteTo(x);
						x.Close();
					}
				}

				this.XmlEditGrid1.XmlFile = myFile;
			}
		}

		/// <summary>
		/// MagicUrls Constructor
		/// Set the Module Settings
		/// </summary>
		public MagicUrls()
		{
			this.SupportsWorkflow = false;
			this.EnableViewState = true;
		}

		/// <summary>
		/// General Module Def GUID
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{345EB057-F35F-4882-A3F1-38A504A6C382}");
			}
		}

		/// <summary>
		/// Admin Module
		/// </summary>
		public override bool AdminModule
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Searchable module
		/// </summary>
		public override bool Searchable
		{
			get
			{
				return false;
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			if ( !this.Page.IsCssFileRegistered("Mod_MagicUrls") )
				this.Page.RegisterCssFile("Mod_MagicUrls");


			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
