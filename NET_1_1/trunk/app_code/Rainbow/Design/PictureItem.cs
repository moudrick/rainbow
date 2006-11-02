using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Rainbow.Configuration;

namespace Rainbow.Design
{

	/// <summary>
	/// PictureItem
	/// </summary>
	public class PictureItem : UserControl 
	{

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		protected HyperLink editLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private XmlDocument metadata;

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="bydefault" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string GetCurrentImageFromTheme (string name, string bydefault) 
		{
			// Obtain PortalSettings from Current Context
			if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
			{
				PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				return pS.GetCurrentTheme().GetImage(name, bydefault).ImageUrl;
			}
			return bydefault;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string GetMetadata(string key)
		{
			XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);

			if (targetNode == null)
			{
				return null;
			}

			else
			{
				return targetNode.Value;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public XmlDocument Metadata
		{
			get { return this.metadata; }
			set { this.metadata = value; }
		}

		#region Web Form Designer generated code
		/// <summary>
		///     
		/// </summary>
		/// <param name="e" type="System.EventArgs">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion

	}
}
