using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web.SessionState;

namespace Rainbow.ECommerce.Gateways
{
	/// <summary>
	/// This is just an example of how to access a COM object from an .asp page
	/// Need to be improved, very dirty, Thierry (Tiptopweb)
	/// </summary>
	public class GatewayEmail : Rainbow.ECommerce.Gateways.GatewayBase
	{
		/// <summary>
		/// GatewayEmail
		/// </summary>
		public override string Name
		{
			get
			{
				return "SecureEmail";
			}
		}

		/// <summary>
		/// Send email using ASPEncrypt.dll (COM object)
		/// </summary>
		/// <returns></returns>
		public string Process()
		{
			// send email using ASPEncrypt.dll (COM object)

			string strOrder ="encrypted order " + this.OrderID;

			// call a normal asp page using COM components
			WebClient wc = new WebClient();

			// all parameters to be posted
			NameValueCollection nvc = new NameValueCollection();
			nvc.Add("SmtpHost", "mail.portal.net");
			nvc.Add("CertFile", _CertFile);
			nvc.Add("EmailFrom", "shop@portal.net");
			nvc.Add("EmailFromName", "Shop Portal");
			nvc.Add("EmailSubject", "Order " + this.OrderID );
			nvc.Add("EmailBody", strOrder);

			// upload and get result
			wc.BaseAddress = "http" + "://www.portal.net";
			byte[] responseArray = wc.UploadValues("/DesktopModules/ECommerce/SendEncryptedEmail.asp", "POST", nvc);
			return Encoding.ASCII.GetString( responseArray );
		}

		// additional fields for Email Gateway
		private string _CertFile = "";
		private string _SmtpHost = "";
		private string _EmailFrom = "";
		private string _EmailName = "";
		private string _EmailBody = "";

		/// <summary>
		/// Get, set Certificate file (public key) to send email to
		/// </summary>
		public string CertFile
		{
			get{return _CertFile;}
			set{_CertFile = value;}
		}

		/// <summary>
		/// Get, set Smtp Host
		/// </summary>
		public string SmtpHost
		{
			get{return _SmtpHost;}
			set{_SmtpHost = value;}
		}

		/// <summary>
		/// Get, set Email From
		/// </summary>
		public string EmailFrom
		{
			get{return _EmailFrom;}
			set{_EmailFrom = value;}
		}

		/// <summary>
		/// Get, set Email Name
		/// </summary>
		public string EmailName
		{
			get{return _EmailName;}
			set{_EmailName = value;}
		}

		/// <summary>
		/// Get, set Email Name
		/// </summary>
		public string EmailBody
		{
			get{return _EmailBody;}
			set{_EmailBody = value;}
		}

		private string _CreditInstitute = "";

		/// <summary>
		/// Get, set Credit Institute name
		/// </summary>
		public override string CreditInstitute
		{
			get{return _CreditInstitute;}
			set{_CreditInstitute = value;}
		}

		/// <summary>
		/// URL of the Credit Institute gateway
		/// </summary>
		protected override string URLGateway
		{
			get
			{
				return this.URLOk + "?GATEWAY=EMAIL";
			}
		}

		/// <summary>
		/// Not Implemented
		/// </summary>
		public override void GetRequest()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not Implemented
		/// </summary>
		public override void PostRequest()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the form for contacting the Credit Institute gateway
		/// </summary>
		/// <returns></returns>
		public override System.Web.UI.LiteralControl GetForm()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Used in response page to decypt an analyze the server response
		/// </summary>
		public override void AnalyzeURL()
		{
			throw new NotImplementedException();
		}
	}
}
