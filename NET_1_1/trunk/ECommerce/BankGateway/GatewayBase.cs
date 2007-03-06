using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Net;
using System.IO;

namespace Rainbow.ECommerce.Gateways
{
	/// <summary>
	/// Rainbow.ECommerce.Gateways.GatewayBase is an abstract class used to build up
	/// custom gateway solutions for manage communication
	/// with secure Credit Institute servers
	/// </summary>
	public abstract class GatewayBase
	{
		/// <summary>
		/// Each Gateway must implement this name.
		/// Using this name GatewayManager can
		/// istantiate the class.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		private bool m_testMode = false;
		/// <summary>
		/// If true Gateway runs in test mode.
		/// Some Gateways could have not a test mode.
		/// </summary>
		public bool TestMode
		{
			get {return m_testMode;}
			set {m_testMode = value;}
		}

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

		private string m_merchantName = string.Empty;
		/// <summary>
		/// Friendly name
		/// </summary>
		public string MerchantName
		{
			get {return m_merchantName;}
			set {m_merchantName = value;}
		}

		private string m_merchantPrefix = string.Empty;
		/// <summary>
		/// A code or alphanumeric characters prefix to indicate the vendor
		/// </summary>
		public string MerchantPrefix
		{
			get {return m_merchantPrefix;}
			set {m_merchantPrefix = value;}
		}

		private string m_orderID;
		/// <summary>
		/// Current order ID, could be assigned or auto-generated
		/// </summary>
		public string OrderID
		{
			get {return m_orderID;}
			set {m_orderID = value.Trim();}
		}

		private Esperantus.Money m_price;
		/// <summary>
		/// Total to pay
		/// </summary>
		public Esperantus.Money Price
		{
			get {return m_price;}
			set {m_price = value;}
		}

		private string m_buyerEmail;
		/// <summary>
		/// Email of the buyer
		/// </summary>
		public string BuyerEmail
		{
			get
			{
				return m_buyerEmail;
			}
			set 
			{	
				//Checks if Email is specified and valid
				m_buyerEmail = CheckEmail(value);
			}
		}

		private string m_buyerName;
		/// <summary>
		/// Full Name of the buyer
		/// </summary>
		public string BuyerName
		{
			get {return m_buyerName;}
			set {m_buyerName = value;}
		}

		string _merchantEmail;
		/// <summary>
		/// The email of the vendor
		/// </summary>
		public string MerchantEmail
		{
			get
			{
				return _merchantEmail;
			}
			set
			{
				//Checks if Email is specified and valid
				_merchantEmail = CheckEmail(value);
			}
		}
        
		string _technicalEmail;

		/// <summary>
		/// The email of the technical
		/// </summary>
		public string TechnicalEmail
		{
			get
			{
				return _technicalEmail;
			}
			set
			{
				//Checks if Email is specified and valid
				_technicalEmail = CheckEmail(value);
			}
		}
				
		/// <summary>
		/// If valid reurn email, if not raise an exception
		/// </summary>
		/// <param name="settingName"></param>
		/// <returns></returns>
		private string CheckEmail(string settingName)
		{
			if (
				settingName == null || 
				settingName == string.Empty || 
				settingName.IndexOf('@') < 0 ||
				settingName.IndexOf('@') >= settingName.LastIndexOf('.')
				)
			{
				throw new ArgumentException("Null or invalid email", settingName);
			}
			return settingName;
		}

		private string m_urlOk;
		/// <summary>
		/// The URL used to communicate with the Credit Institute (SUCCESS PAGE)
		/// </summary>
		public string URLOk
		{
			get {return m_urlOk;}
			set {m_urlOk = value;}
		}

		private string m_urlKo;
		/// <summary>
		/// The URL used to communicate with the Credit Institute (FAIL PAGE)
		/// </summary>
		public string URLKo
		{
			get {return m_urlKo;}
			set {m_urlKo = value;}
		}

		private string m_urlAck;
		/// <summary>
		/// The URL used to communicate with the Credit Institute (SERVER to SERVER communication)
		/// </summary>
		public string URLAck
		{
			get {return m_urlAck;}
			set {m_urlAck = value;}
		}

		private string m_paymentType;
		/// <summary>
		/// Type of payment
		/// </summary>
		public string PaymentType
		{
			get {return m_paymentType;}
			set {m_paymentType = value;}
		}

		private string m_authCode;
		/// <summary>
		/// Authorization code assigned from the Credit Institute
		/// </summary>
		public string AuthCode
		{
			get {return m_authCode;}
			set {m_authCode = value;}
		}

		private string m_transactionID;
		/// <summary>
		/// TransactionID code assigned from the Credit Institute
		/// </summary>
		public string TransactionID
		{
			get {return m_transactionID;}
			set {m_transactionID = value;}
		}

		private NameValueCollection m_customSettings = new NameValueCollection();
		/// <summary>
		/// Custom Settings
		/// </summary>
		public NameValueCollection CustomSettings
		{
			get {return m_customSettings;}
		}

		/// <summary>
		/// Last error code
		/// </summary>
		public string ErrorCode = "";

		/// <summary>
		/// Description of the laste error occurred
		/// </summary>
		public string ErrorDescription = "";

		/// <summary>
		/// Text of the submit button (for localization purpose)
		/// </summary>
		public string SubmitButtonText = "Submit";

		CultureInfo m_language = System.Threading.Thread.CurrentThread.CurrentUICulture;
		/// <summary>
		/// Gateway language
		/// </summary>
		public CultureInfo Language
		{
			get {return m_language;}
			set {m_language = value;}
		}

		/// <summary>
		/// Name of the Credit Institute
		/// </summary>
		public abstract string CreditInstitute
		{
			get;
			set;
		}

		/// <summary>
		/// URL of the Credit Institute gateway
		/// </summary>
		protected abstract string URLGateway
		{
			get;
		}

		/// <summary>
		/// Gets the form for contacting the Credit Institute gateway
		/// </summary>
		/// <returns></returns>
		public System.Web.UI.LiteralControl GetForm(bool AutogenerateID)
		{
			if (AutogenerateID)
				this.OrderID = GetNewOrderID();
			return GetForm();
		}

		/// <summary>
		/// Gets the form for contacting the Credit Institute gateway
		/// </summary>
		/// <returns></returns>
		public System.Web.UI.LiteralControl GetForm(string orderID)
		{
			this.OrderID = orderID;
			return GetForm();
		}

		/// <summary>
		/// Gets the form for contacting the Credit Institute gateway
		/// </summary>
		/// <returns></returns>
		abstract public System.Web.UI.LiteralControl GetForm();

		/// <summary>
		/// Contacts the server using GET METHOD
		/// </summary>
		public virtual void GetRequest()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Contacts the server using POST METHOD
		/// </summary>
		public virtual void PostRequest()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Request url
		/// </summary>
		public virtual string GetRequestUrl()
		{
			throw new NotImplementedException();
		}
       
		/// <summary>
		/// Used in response page to decypt an analyze the server response
		/// </summary>
		abstract public void AnalyzeURL();

		/// <summary>
		/// Gets a key pair value as HIDDEN input field 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="required"></param>
		/// <returns></returns>
		protected LiteralControl GetHidden(string name, string value, bool required)
		{
			if (value == null | value == "")
			{
				if(required)
				{
					throw(new System.ArgumentOutOfRangeException(name, "Required value"));
				}
				else
				{
					return new LiteralControl();
				}
			}
			else
			{
				LiteralControl h = new LiteralControl("<INPUT TYPE='HIDDEN' NAME='" + name + "' ID='" + this.Name + "_" + name + "' VALUE='" + value + "'>");
				return h;            
			}
		}

		/// <summary>
		/// Gets a key pair value as parameter suitable for Http requests
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="required"></param>
		/// <returns></returns>
		protected string GetUrlParameter(string name, string value, bool required)
		{
			if (value == null || value == "")
			{
				if(required)
				{
					throw(new System.ArgumentOutOfRangeException(name, "Required value"));
				}
				else
				{
					return "";
				}
			}
			else
			{
				string h = string.Concat(name, "=", value, "&");
				return h;            
			}
		}

		/// <summary>
		/// Retrive a parameters string encrypted with MAC code
		/// </summary>
		/// <returns></returns>
		protected string GetHashMD5(string inputString, MD5Encoding encodingType)
		{
			// Get an ASCII encoder
			System.Text.ASCIIEncoding encoder = new ASCIIEncoding();

			// This is one implementation of the abstract class MD5.
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(encoder.GetBytes(inputString));

			// Get MD5 Hash
			string md5Result = string.Empty;
			if (encodingType == MD5Encoding.Base64)
				md5Result = Convert.ToBase64String(result);
			else if (encodingType == MD5Encoding.Exadecimal)
				md5Result = ToHexadecimal(result);
			else
				throw new ArgumentException("No suitable outputt specified", "encodingType");

			// Output result for diagnostic purposes (debug mode only)
			System.Diagnostics.Debug.WriteLine(inputString + "=" + md5Result);

			return(md5Result);
		}

		public static string ToHexadecimal(byte[] bytes)
		{
			StringBuilder hexString = new StringBuilder("");
			for (int i=0; i < bytes.Length; i++)
			{
				hexString.Append(bytes[i].ToString("X2"));
			}
			return hexString.ToString();
		}

		/// <summary>
		/// Generate a new OrderID using custom Merchant prefix
		/// </summary>
		/// <returns></returns>
		public string GetNewOrderID()
		{
			return Rainbow.ECommerce.Gateways.GatewayBase.GetNewOrderID(MerchantPrefix);
		}

		/// <summary>
		/// Generate a new OrderID
		/// </summary>
		/// <returns></returns>
		public static string GetNewOrderID(string Prefix)
		{
			StringBuilder code = new StringBuilder(Prefix);

			//Add date: eg: 200408251255
			code.Append(DateTime.Now.Year.ToString());
			code.Append(DateTime.Now.Month.ToString().PadLeft(2, '0'));
			code.Append(DateTime.Now.Day.ToString().PadLeft(2, '0'));
			code.Append(DateTime.Now.Hour.ToString().PadLeft(2, '0'));
			code.Append(DateTime.Now.Minute.ToString().PadLeft(2, '0'));
			code.Append(DateTime.Now.Second.ToString().PadLeft(2, '0'));

			code.Append(new Random(unchecked((int)DateTime.Now.Ticks)).Next(999999).ToString().PadLeft(6, '0'));

			return code.ToString();
		}

		/// <summary>
		/// GetHttpPage connects to internet using http protocol GET method 
		/// and retrieves the page at the specified URI.
		/// </summary>
		/// <param name="Url">URI of the page</param>
		/// <returns></returns>
		protected string GetHttpPage(Uri Url)
		{
			// Create the Web Request Object 
			WebRequest request = HttpWebRequest.Create(Url);
			// Read the response from the Web Server
			return retrieveFromURL (request);
		}

		/// <summary>
		/// PostHttpPage connects to internet using http protocol POST method 
		/// and retrieves the page at the specified URI.
		/// </summary>
		/// <param name="Url">URI of the page</param>
		/// <param name="value">Data to post</param>
		/// <returns></returns>
		protected string PostHttpPage(Uri Url, string value)
		{
			// Create the Web Request Object 
			WebRequest request = HttpWebRequest.Create(Url);
			// Specify that you want to POST data
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			if (value != null) 
			{
				// write out the data to the web server
				writeToURL (request, value);
			}
			else 
			{
				request.ContentLength = 0;
			}
			// Read the response from the Web Server
			return retrieveFromURL (request);
		}

		/// <summary>
		/// retrieveFromURL is a method that retrieves the contents of
		/// a specified URL in response to a request
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private string retrieveFromURL(WebRequest request)
		{
			// Get the Web Response Object from the request
			WebResponse response = request.GetResponse();
			// Get the Stream Object from the response
			Stream responseStream = response.GetResponseStream();
			// Create a stream reader and associate it with the stream object
			StreamReader reader = new StreamReader (responseStream);
			// Read the entire stream
			string content = reader.ReadToEnd();
			// Dispose the reader
			reader.Close();
			reader = null;
			// Return the result
			return content;
		}

		/// <summary>
		/// writeToURL is a method that writes the contents of
		/// a specified URL to the web
		/// </summary>
		/// <param name="request">URL to write to</param>
		/// <param name="data">The data to write</param>
		/// <returns></returns>
		private void writeToURL(WebRequest request, string data) 
		{
			byte [] bytes = null;
			// Get the data that is being posted (or sent) to the server
			bytes = System.Text.Encoding.ASCII.GetBytes(data);
			request.ContentLength = bytes.Length;
			// Get an output stream from the request object
			Stream outputStream = request.GetRequestStream();
			// Post the data out to the stream
			outputStream.Write (bytes, 0, bytes.Length);
			// Close the output stream and send the data out to the web server
			outputStream.Close ();
		}
	}

	public enum MD5Encoding
	{
		Base64,
		Exadecimal
	}
}