using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

using Rainbow.DesktopModules;
using Rainbow.Helpers;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Helper class for displaying the order in a page as 
	/// a block of html or send as html email
	/// </summary>
	public class OrderManager
	{
		/// <summary>
		/// Returns the order serialized as XML
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static string GetXML(Order o)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Order));
			StringWriter xmlw = new StringWriter();
			serializer.Serialize(xmlw, o);
			return(xmlw.ToString());
		}

		/// <summary>
		/// Returns the order as HTML
		/// </summary>
		/// <param name="o"></param>
		/// <param name="xsltdoc"></param>
		/// <returns></returns>
		public static string GetHTML(Order o, XmlDocument xsltdoc)
		{
			//XML order
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(GetXML(o));

			//Create a new XslTransform object
			XslTransform xslt = new XslTransform();

			// build parameter list to pass to stylesheet
			XsltArgumentList xslArg = new XsltArgumentList();

			// add the helper object
			XslHelper xslHelper = new XslHelper();
			xslArg.AddExtensionObject("urn:rainbow",xslHelper);

#if FW11
			//Load the stylesheet
			xslt.Load(xsltdoc, new XmlUrlResolver(), new System.Security.Policy.Evidence());
			//Transform the data
			XmlReader r = xslt.Transform(doc.CreateNavigator(), xslArg, new XmlUrlResolver());
#else
			//Load the stylesheet
			xslt.Load(xsltdoc);
			//Transform the data
			XmlReader r = xslt.Transform(doc.CreateNavigator(), xslArg);
#endif
			string result = "";
			try
			{
				r.Read();
				result = r.ReadOuterXml();
			}
			finally
			{
				r.Close(); //by Manu, fixed bug 807858
			}
			return(result);
		}
	}
}