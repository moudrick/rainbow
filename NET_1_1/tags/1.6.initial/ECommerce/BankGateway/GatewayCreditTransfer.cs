using System;
using System.Collections;
using System.Collections.Specialized;
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

namespace Rainbow.ECommerce.Gateways
{
    /// <summary>
    /// Gateway class to generic credit institute, wire transfer mode
    /// </summary>
    public class GatewayCreditTransfer : Rainbow.ECommerce.Gateways.GatewayBase
    {
		/// <summary>
		/// Ganeric Gateway contructor
		/// </summary>
		public GatewayCreditTransfer()
		{
			//We assume to use GatewayWT.aspx for both
			this.URLOk = "GatewayCreditTransferPage.aspx"; 
			this.URLKo = "GatewayCreditTransferPage.aspx"; 
		}

		/// <summary>
		/// CreditTransfer
		/// </summary>
		public override string Name
		{
			get
			{
				return "CreditTransfer";
			}
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
                return this.URLOk;
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
			if(this.OrderID == null || this.OrderID.Length == 0) //Generate New order if is missing
				throw new ArgumentNullException("OrderID", "Please provide an order ID before requesting form");

			StringBuilder s = new StringBuilder("");

			s.Append("<FORM NAME='GatewayCreditTransfer' ACTION='" + URLGateway + "' METHOD='POST'>");

			s.Append(GetHidden("MERCHANT_ID", this.MerchantID, true).Text);
			s.Append(GetHidden("ORDER_ID", this.OrderID, true).Text);
			s.Append(GetHidden("AMOUNT", this.Price.ToString("N", new System.Globalization.CultureInfo("en-US").NumberFormat), true).Text);
			s.Append(GetHidden("CURRENCY", this.Price.Info.ISOCurrencySymbol, true).Text);
			s.Append(GetHidden("EMAIL", this.BuyerEmail, false).Text);
			s.Append(GetHidden("LANGUAGE", this.Language.Name, false).Text);
			s.Append(GetHidden("URLOK", this.URLOk, true).Text);
			s.Append(GetHidden("URLKO", this.URLKo, true).Text);
			s.Append("<INPUT TYPE='SUBMIT' VALUE='" + SubmitButtonText + "'>");

			s.Append("</FORM>");

			LiteralControl myForm = new LiteralControl(s.ToString());
			return(myForm);
        }
		
        /// <summary>
        /// Used in response page to decrypt an analyze the server response
        /// </summary>
        public override void AnalyzeURL()
        {
			//Get querystring
			if(HttpContext.Current != null)
			{
				System.Web.HttpRequest queryString = HttpContext.Current.Request;

				// Verify if there are errors
				this.TransactionID = queryString["TRANSACTION_ID"];
				this.AuthCode = queryString["COD_AUT"];
				this.MerchantID = queryString["MERCHANT_ID"];
				this.OrderID = queryString["ORDER_ID"];

				//This should work with any currency
				if (queryString["AMOUNT"].ToString() != "")
					this.Price = new Esperantus.Money(decimal.Parse(queryString["AMOUNT"].ToString(), new System.Globalization.CultureInfo("en-US").NumberFormat), queryString["CURRENCY"].ToString());
				else
					this.Price = new Esperantus.Money(0M, queryString["CURRENCY"].ToString());
			}
		}
    }
}
