using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Esperantus;
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
		PortalSettings portalSettings;
		User user;

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

		/// <summary>
		///     
		/// </summary>
		/// <param name="textKey" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="translation" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string Localize(string textKey, string translation)
		{
			return Esperantus.Localize.GetString(textKey, translation);
		}

		# region Data Formatting

		/// <summary>
		///     
		/// </summary>
		/// <param name="myAmount" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="myCurrency" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatMoney(string myAmount, string myCurrency)
		{
			try
			{
				return new Money(Decimal.Parse(myAmount, CultureInfo.InvariantCulture.NumberFormat), myCurrency).ToString();
			}
			catch
			{
				return string.Empty;			
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="tempStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataScale" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="outputScale" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatTemp(string tempStr, string dataScale, string outputScale)
		{
			try
			{
				double conv;

				if ( dataScale == outputScale )
				{
					conv = double.Parse(tempStr,new CultureInfo(string.Empty));
					return conv.ToString("F0") + Convert.ToChar(176) + outputScale;
				}

				else if ( outputScale.ToUpper() == "C" )
				{
					conv = F2C( double.Parse(tempStr, new CultureInfo(string.Empty)) );
					return conv.ToString("F0") + Convert.ToChar(176) + "C";
				}

				else
				{
					conv = C2F( double.Parse(tempStr, new CultureInfo(string.Empty)) );
					return conv.ToString("F0") + Convert.ToChar(176) + "F";
				}
			}

			catch
			{
				return tempStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="c" type="double">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A double value...
		/// </returns>
		public double C2F(double c) 
		{
			return (1.8 * c) + 32;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="f" type="double">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A double value...
		/// </returns>
		public double F2C(double f) 
		{
			return (f - 32) / 1.8;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="dateStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="formatStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatDateTime(string dateStr, string dataCulture, string formatStr)
		{
			try
			{
				return FormatDateTime(dateStr, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatStr);
			}
			catch
			{
				return dateStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="dateStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="formatStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatDateTime(string dateStr, string formatStr)
		{
			try
			{
				return FormatDateTime(dateStr, portalSettings.PortalDataFormattingCulture.Name, portalSettings.PortalDataFormattingCulture.Name, formatStr);
			}
			catch
			{
				return dateStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="dateStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatDateTime(string dateStr)
		{

			try
			{
				return DateTime.Parse(dateStr).ToLongDateString();
			}

			catch
			{
				return dateStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="dateStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="outputCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="formatStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatDateTime(string dateStr, string dataCulture, string outputCulture, string formatStr)
		{

			try
			{
				DateTime conv;

				if ( dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower() )
				{
					conv = DateTime.ParseExact(dateStr,"mm/dd/yyyy hh:mm:ss" ,new CultureInfo(dataCulture, false),DateTimeStyles.AdjustToUniversal);
				}

				else
				{
					conv = DateTime.Parse(dateStr, new CultureInfo(dataCulture, false),DateTimeStyles.None);
				}

				if ( outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower() )
					return conv.ToString(formatStr);

				else
					return conv.ToString(formatStr, new CultureInfo(outputCulture, false));
			}

			catch
			{
				return dateStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="numberStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="formatStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatNumber(string numberStr, string dataCulture, string formatStr)
		{

			try
			{
				return FormatNumber(numberStr, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatStr);
			}

			catch
			{
				return numberStr;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="numberStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="outputCulture" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="formatStr" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string FormatNumber(string numberStr, string dataCulture, string outputCulture, string formatStr)
		{

			try
			{

				Double conv;

				if ( dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower() )
				{
					conv = Double.Parse(numberStr);
				}

				else
				{
					conv = Double.Parse(numberStr, new CultureInfo(dataCulture, false));
				}

				if ( outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower() )
					return conv.ToString(formatStr);

				else
					return conv.ToString(formatStr, new CultureInfo(outputCulture, false));
			}

			catch
			{
				return numberStr;
			}
		}
		# endregion

		# region Security

		/// <summary>
		///     
		/// </summary>
		/// <param name="authRoles" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		public bool CheckRoles(string authRoles)
		{
			return PortalSecurity.IsInRoles(authRoles);
		}
		# endregion

		# region Url Builder

		/// <summary>
		///     
		/// </summary>
		/// <param name="url" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="paramKey" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="paramValue" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string AddToUrl(string url, string paramKey, string paramValue)
		{

			if (url.IndexOf(paramKey) == -1)
			{

				if (url.IndexOf("?") > 0)
				{
					url = url.Trim() + "&" + paramKey.Trim() + "=" + paramValue.Trim();
				}

				else
				{
					url = url.Trim();
					url = url.Substring(0,url.LastIndexOf("/")) + "/" + paramKey.Trim() + "_" + paramValue.Trim() + url.Substring(url.LastIndexOf("/"));
				}
			}
			return url;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="targetPage" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="pageID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string BuildUrl(string targetPage, int pageID)
		{
//			targetPage = System.Text.RegularExpressions.Regex.Replace(targetPage,@"[\.\$\^\{\[\(\|\)\*\+\?!'""]",string.Empty);
//			targetPage = targetPage.Replace(" ","_").ToLower();
//			return Rainbow.HttpUrlBuilder.BuildUrl("~/" + targetPage + ".aspx", tabID);

			return HttpUrlBuilder.BuildUrl(string.Concat("~/", Clean(targetPage), ".aspx"), pageID);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="targetPage"></param>
		/// <param name="pageID"></param>
		/// <param name="pathTrace"></param>
		/// <returns></returns>
		public string BuildUrl(string targetPage, int pageID, string pathTrace)
		{
			return HttpUrlBuilder.BuildUrl(string.Concat("~/", Clean(targetPage), ".aspx"), pageID, Clean(pathTrace));
		}

		/// <summary>
		///         
		/// </summary>
		/// <param name="pathTrace" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string BuildUrl(int pageID, string pathTrace)
		{
			#region old
//			char[] _delim = new char[]{'/'};
//			pathTrace = pathTrace.TrimEnd(_delim);
//			pathTrace = pathTrace.TrimStart(_delim);
//			pathTrace = System.Text.RegularExpressions.Regex.Replace(pathTrace,@"[\.\$\^\{\[\(\|\)\*\+\?!'""]",string.Empty);
//			pathTrace = pathTrace.Replace(" ","_").ToLower();
//			string targetPage = null;
//			string customAttributes = null;
//
//			if (pathTrace.IndexOf("/") > 0)
//			{
//				targetPage = pathTrace.Substring(pathTrace.LastIndexOf("/") + 1);
//				customAttributes = pathTrace.Substring(0,pathTrace.Length - targetPage.Length - 1);
//			}
//
//			else
//			{
//				targetPage = pathTrace;
//			}
//			return Rainbow.HttpUrlBuilder.BuildUrl("~/" + targetPage + ".aspx", tabID, customAttributes);
			#endregion

			return HttpUrlBuilder.BuildUrl(pageID, Clean(pathTrace));

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pageID"></param>
		/// <returns></returns>
		public string BuildUrl(int pageID)
			{
			return HttpUrlBuilder.BuildUrl(pageID);
			}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="myText"></param>
		/// <returns></returns>
		private string Clean(string myText)
			{
			// is this faster/slower than using iteration over string?
			char mySeparator = '_';
			string singleSeparator = "_";
			string doubleSeparator = "__";
			//myText = Regex.Replace(myText.ToLower(), @"[^-'/\p{L}\p{N}]",singleSeparator);
            		myText = Regex.Replace(myText.ToLower(), @"[^-\p{L}\p{N}]",singleSeparator);

			return myText.Replace(doubleSeparator,singleSeparator).Trim(mySeparator);
		}

		#endregion

		# region Member Access - portalSettings

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalAlias()
		{
			return portalSettings.PortalAlias;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalID()
		{
			return portalSettings.PortalID.ToString();
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PageID()
		{
			return portalSettings.ActivePage.PageID.ToString();
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string TabTitle()
		{
			return portalSettings.ActivePage.PageName;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalContentLanguage()
		{
			return portalSettings.PortalContentLanguage.Name;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalUILanguage()
		{
			return portalSettings.PortalUILanguage.Name;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalDataFormattingCulture()
		{
			return portalSettings.PortalDataFormattingCulture.Name;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalLayoutPath()
		{
			return portalSettings.PortalLayoutPath;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalFullPath()
		{
			return portalSettings.PortalFullPath;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalName()
		{
			return portalSettings.PortalName;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string PortalTitle()
		{
			return portalSettings.PortalTitle;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string UserName()
		{
			return user.Name;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string UserEmail()
		{
			return user.Email;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string UserID()
		{
			return user.ID;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A System.Xml.XPath.XPathNodeIterator value...
		/// </returns>
		public XPathNodeIterator DesktopTabsXml()
		{
			return portalSettings.PortalPagesXml.CreateNavigator().Select("*");
		}

		#endregion

		#region Member Access - moduleSettings

		#endregion

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

			if(metadataXml != null && metadataXml.Length > 0)
			{

				try
				{
					myXmlDoc.LoadXml(metadataXml);
					XmlNode foundNode1 = myXmlDoc.SelectSingleNode("options/option1/selected");

					if(foundNode1 != null)
					{
						selectedOptions += foundNode1.InnerText;
					}
					XmlNode foundNode2 = myXmlDoc.SelectSingleNode("options/option2/selected");

					if(foundNode2 != null)
					{
						selectedOptions += " - " + foundNode2.InnerText;
					}
					XmlNode foundNode3 = myXmlDoc.SelectSingleNode("options/option3/selected");

					if(foundNode3 != null)
					{
						selectedOptions += " - " + foundNode3.InnerText;
					}
				}

				catch(Exception ex)
				{
					LogHelper.Logger.Log(LogLevel.Warn, "XSL failed. Metadata Was: '" + metadataXml + "'", ex);
				}
			}
			return selectedOptions;
		}

		#endregion

	}
}
