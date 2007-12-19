using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;
using Rainbow.DesktopModules;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// This class manges all data of Merchant
	/// </summary>
	public class MerchantData
	{
		private string m_merchantID;
		/// <summary>
		/// MerchantID assigned from the Credit Institute to the Merchant,
		/// sometimes called VendorID or ShopID
		/// </summary>
		public string MerchantID
		{
			get {return m_merchantID;}
			set {m_merchantID = value;}
		}

		private string m_merchantEmail;
		/// <summary>
		/// Email of the merchant
		/// </summary>
		public string MerchantEmail
		{
			get {return m_merchantEmail;}
			set {m_merchantEmail = value;}
		}

		private string m_technicalEmail;
		/// <summary>
		/// Email of the merchant
		/// </summary>
		public string TechnicalEmail
		{
			get {return m_technicalEmail;}
			set {m_technicalEmail = value;}
		}

		private string m_merchantName;
		/// <summary>
		/// Full Name of the merchant
		/// </summary>
		public string MerchantName
		{
			get {return m_merchantName;}
			set {m_merchantName = value;}
		}

		private NameValueCollection m_customSettings = new NameValueCollection();
		/// <summary>
		/// Custom Settings
		/// </summary>
		public NameValueCollection CustomSettings
		{
			get {return m_customSettings;}
		}
	}
}