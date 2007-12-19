using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Security;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Summary description for ZenNavigation.
	/// </summary>
	public class ZenNavigation : WebControl //, INavigation
	{
		protected PortalSettings portalSettings;
		protected XmlDocument PortalPagesXml;

		public ZenNavigation()
		{
			this.EnableViewState = false; 
			this.Load += new System.EventHandler(this.LoadControl);
		} 

		private string _containerCssClass = string.Empty;
		public virtual string ContainerCssClass
		{
			get{return _containerCssClass;}
			set{_containerCssClass = value;}
		} 

//		private string _urlStyle = string.Empty;
//		public virtual string UrlStyle
//		{
//			get{return _urlStyle;}
//			set{_urlStyle = value;}
//		}

		private string _xsltFile = "BindAll";
		public virtual string XsltFile
		{
			get
			{
				return _xsltFile;
			}
			set
			{
				_xsltFile = value;
			}
		}
		private BindOption _bind = BindOption.BindOptionTop;
		public virtual BindOption Bind
		{
			get{return _bind;}
			set{_bind = value;}
		}

		private bool _usePageNameInUrl = true;
		public virtual bool UsePageNameInUrl
		{
			get{return _usePageNameInUrl;}
			set{_usePageNameInUrl = value;}
		}

		private bool _usePathTraceInUrl = true;
		public virtual bool UsePathTraceInUrl
		{
			get{return _usePathTraceInUrl;}
			set{_usePathTraceInUrl = value;}
		}

		private void LoadControl(object sender, System.EventArgs e) 
		{
			// Obtain PortalSettings from Current Context
			portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

			this.PortalPagesXml = portalSettings.PortalPagesXml;
				
			//base.DataBind();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			// get XSLT stylesheet
			XslTransform xslDoc = new XslTransform();
			try
			{
				string myFilePath = HttpContext.Current.Server.MapPath(string.Concat(Rainbow.Settings.Path.ApplicationRoot, "/app_support/ZenNavigation/", XsltFile, ".xslt"));
				xslDoc.Load(myFilePath);
			}
			catch(Exception e)
			{
				throw new ApplicationException("Cannot load specified XsltFile: " + e.Message);
			}

			// build parameter list to pass to stylesheet
			XsltArgumentList xslArg = new XsltArgumentList();
			xslArg.AddParam("ActivePageId","",portalSettings.ActivePage.PageID);
			xslArg.AddParam("ContainerCssClass","",this.ContainerCssClass);
			xslArg.AddParam("UsePageNameInUrl","",this.UsePageNameInUrl.ToString().ToLower());
			xslArg.AddParam("UsePathTraceInUrl","",this.UsePathTraceInUrl.ToString().ToLower());
			// add the helper object
			XslHelper xslHelper = new XslHelper();
			xslArg.AddExtensionObject("urn:rainbow",xslHelper);
			// do the transform
			XmlUrlResolver myResolver = new XmlUrlResolver();
			StringWriter result = new StringWriter();
			xslDoc.Transform(this.PortalPagesXml,xslArg,result,myResolver);
		
			string myResult = result.ToString();
			writer.Write(myResult);

		}

		private string CleanPageName(string targetPage)
		{
			string splitter = ConfigurationSettings.AppSettings["HandlerDefaultSplitter"];
			if (splitter == string.Empty || splitter == null)
				splitter = "__";
			targetPage = System.Text.RegularExpressions.Regex.Replace(targetPage,@"[\.\$\^\{\[\(\|\)\*\+\?!'""]",splitter);
			targetPage = targetPage.Replace(" ",splitter).ToLower();
			return targetPage;
		}

		
		#region INavigation implementation 
		//		private BindOption _bind = BindOption.BindOptionTop; 
		private int _definedParentTab = -1;
		//		private bool _autoBind = true; 
		//		/// <summary> 
		//		/// Indicates if control should bind when loads 
		//		/// </summary> 
		//		[ 
		//		Category("Data"), 
		//		PersistenceMode(PersistenceMode.Attribute) 
		//		] 
		//		public bool AutoBind 
		//		{ 
		//			get{return _autoBind;} 
		//			set{_autoBind = value;} 
		//		} 
		//
		//		/// <summary> 
		//		/// Describes how this control should bind to db data 
		//		/// </summary> 
		//		[ 
		//		Category("Data"), 
		//		PersistenceMode(PersistenceMode.Attribute) 
		//		] 
		//		public BindOption Bind 
		//		{ 
		//			get {return _bind;} 
		//			set 
		//			{ 
		//				if(_bind != value) 
		//				{ 
		//					_bind = value; 
		//				} 
		//			} 
		//		} 
		/// <summary>
		/// defines the parentTabID when using BindOptionDefinedParent
		/// </summary>
		[
		Category("Data"),
		PersistenceMode(PersistenceMode.Attribute)
		]
		public int ParentTabID
		{ 
			get {return _definedParentTab;}
			set
			{
				if(_definedParentTab != value)
				{
					_definedParentTab = value;
				}
			}
		}
		#endregion 
	}
}
