using System;
using System.Web;
using System.Xml;
using Rainbow.Configuration;
using Rainbow.Security;

namespace Rainbow.Helpers
{
	/// <summary>
	/// XslHelper object, designed to be imported into an XSLT transform
	/// via XsltArgumentList.AddExtensionObject(...). Provides transform with 
	/// access to various Rainbow functions, such as BuildUrl(), IsInRoles(), data 
	/// formatting, etc. (Jes1111)
	/// </summary>
	public class XslHelper
	{
		/// <summary>
		///     
		/// </summary>
		private PortalSettings portalSettings;

		/// <summary>
		///     
		/// </summary>
		private User user;

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public XslHelper()
		{
			if (HttpContext.Current != null)
			{
				portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				user = new User(HttpContext.Current.User.Identity.Name);
			}
		}

//#zone Removed Data formatting code (should be somewhere else if it isn't already)
//		# region Data Formatting
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="myAmount" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="myCurrency" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatMoney(string myAmount, string myCurrency)
//		{
//			try
//			{
//				return new Money(Decimal.Parse(myAmount, CultureInfo.InvariantCulture.NumberFormat), myCurrency).ToString();
//			}
//
//			catch
//			{
//				return string.Empty;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="tempString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="dataScale" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="outputScale" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatTemp(string tempString, string dataScale, string outputScale)
//		{
//			try
//			{
//				double conv;
//
//				if (dataScale == outputScale)
//				{
//					conv = double.Parse(tempString, new CultureInfo(string.Empty));
//					return conv.ToString("F0") + Convert.ToChar(176) + outputScale;
//				}
//
//				else if (outputScale.ToUpper() == "C")
//				{
//					conv = F2C(double.Parse(tempString, new CultureInfo(string.Empty)));
//					return conv.ToString("F0") + Convert.ToChar(176) + "C";
//				}
//
//				else
//				{
//					conv = C2F(double.Parse(tempString, new CultureInfo(string.Empty)));
//					return conv.ToString("F0") + Convert.ToChar(176) + "F";
//				}
//			}
//
//			catch
//			{
//				return tempString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="c" type="double">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A double value...
//		/// </returns>
//		public double C2F(double c)
//		{
//			return (1.8*c) + 32;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="f" type="double">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A double value...
//		/// </returns>
//		public double F2C(double f)
//		{
//			return (f - 32)/1.8;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="dateString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="dataCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="formatString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatDateTime(string dateString, string dataCulture, string formatString)
//		{
//			try
//			{
//				return FormatDateTime(dateString, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatString);
//			}
//
//			catch
//			{
//				return dateString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="dateString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="formatString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatDateTime(string dateString, string formatString)
//		{
//			try
//			{
//				return FormatDateTime(dateString, portalSettings.PortalDataFormattingCulture.Name, portalSettings.PortalDataFormattingCulture.Name, formatString);
//			}
//
//			catch
//			{
//				return dateString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="dateString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatDateTime(string dateString)
//		{
//			try
//			{
//				return DateTime.Parse(dateString).ToLongDateString();
//			}
//
//			catch
//			{
//				return dateString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="dateString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="dataCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="outputCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="formatString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatDateTime(string dateString, string dataCulture, string outputCulture, string formatString)
//		{
//			try
//			{
//				DateTime conv;
//
//				if (dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
//					conv = DateTime.ParseExact(dateString, "mm/dd/yyyy hh:mm:ss", new CultureInfo(dataCulture, false), DateTimeStyles.AdjustToUniversal);
//
//				else
//					conv = DateTime.Parse(dateString, new CultureInfo(dataCulture, false), DateTimeStyles.None);
//
//				if (outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
//					return conv.ToString(formatString);
//
//				else
//					return conv.ToString(formatString, new CultureInfo(outputCulture, false));
//			}
//
//			catch
//			{
//				return dateString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="numberString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="dataCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="formatString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatNumber(string numberString, string dataCulture, string formatString)
//		{
//			try
//			{
//				return FormatNumber(numberString, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatString);
//			}
//
//			catch
//			{
//				return numberString;
//			}
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// <param name="numberString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="dataCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="outputCulture" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <param name="formatString" type="string">
//		///     <para>
//		///         
//		///     </para>
//		/// </param>
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string FormatNumber(string numberString, string dataCulture, string outputCulture, string formatString)
//		{
//			try
//			{
//				Double conv;
//
//				if (dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
//					conv = Double.Parse(numberString);
//
//				else
//					conv = Double.Parse(numberString, new CultureInfo(dataCulture, false));
//
//				if (outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
//					return conv.ToString(formatString);
//
//				else
//					return conv.ToString(formatString, new CultureInfo(outputCulture, false));
//			}
//
//			catch
//			{
//				return numberString;
//			}
//		}
//
//		# endregion
//#endzone

//#zone Portal & Tab Settings duplication removed
//		# region Member Access - portalSettings
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalAlias()
//		{
//			return portalSettings.PortalAlias;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalID()
//		{
//			return portalSettings.PortalID.ToString();
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string TabID()
//		{
//			return portalSettings.ActiveTab.TabID.ToString();
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string TabTitle()
//		{
//			return portalSettings.ActiveTab.TabName;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalContentLanguage()
//		{
//			return portalSettings.PortalContentLanguage.Name;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalUILanguage()
//		{
//			return portalSettings.PortalUILanguage.Name;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalDataFormattingCulture()
//		{
//			return portalSettings.PortalDataFormattingCulture.Name;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalLayoutPath()
//		{
//			return portalSettings.PortalLayoutPath;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public Uri PortalFullPath()
//		{
//			return portalSettings.PortalFullPath;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalName()
//		{
//			return portalSettings.PortalName;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string PortalTitle()
//		{
//			return portalSettings.PortalTitle;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string UserName()
//		{
//			return user.Name;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string UserEmail()
//		{
//			return user.Email;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A string value...
//		/// </returns>
//		public string UserID()
//		{
//			return user.ID;
//		}
//
//		/// <summary>
//		///     
//		/// </summary>
//		/// 
//		/// <returns>
//		///     A System.Xml.XPath.XPathNodeIterator value...
//		/// </returns>
//		public XPathNodeIterator DesktopTabsXml()
//		{
//			return portalSettings.DesktopTabsXml.CreateNavigator().Select("*");
//		}
//
//		#endregion
//
//		#region Member Access - moduleSettings
//
//		#endregion
//#endzone

		#region Selected Options for Products module (ECommerce receipt)

		/// <summary>
		///     
		/// </summary>
		/// <param name="metadataXml" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string SelectedOptions(string metadataXml)
		{
			string selectedOptions = string.Empty;
			//Create a xml Document
			XmlDocument myXmlDoc = new XmlDocument();

			if (metadataXml != null && metadataXml.Length > 0)
			{
				try
				{
					myXmlDoc.LoadXml(metadataXml);
					XmlNode foundNode1 = myXmlDoc.SelectSingleNode("options/option1/selected");

					if (foundNode1 != null)
						selectedOptions += foundNode1.InnerText;
					XmlNode foundNode2 = myXmlDoc.SelectSingleNode("options/option2/selected");

					if (foundNode2 != null)
						selectedOptions += " - " + foundNode2.InnerText;
					XmlNode foundNode3 = myXmlDoc.SelectSingleNode("options/option3/selected");

					if (foundNode3 != null)
						selectedOptions += " - " + foundNode3.InnerText;
				}

				catch (Exception ex)
				{
					LogHelper.Logger.Log(LogLevel.Warn, "XSL failed. Metadata Was: '" + metadataXml + "'", ex);
				}
			}
			return selectedOptions;
		}

		#endregion
	}
}