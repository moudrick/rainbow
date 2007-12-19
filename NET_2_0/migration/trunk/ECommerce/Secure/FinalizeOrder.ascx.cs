using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Mail;

using Rainbow.ECommerce.Gateways;

namespace Rainbow.ECommerce
{
	/// <summary>
	///	FinalizeOrder is an user control. Can be easly added to any finalize page.
	///	It takes care for calling proper methods on gateways and show oncreen messsages.
	/// </summary>
	public abstract class FinalizeOrder : System.Web.UI.UserControl
	{
		protected Esperantus.WebControls.Label MessageLabel;
		protected System.Web.UI.WebControls.PlaceHolder receiptPlaceHolder;
	
		/// <summary>
		/// This piece of code verifies data against stored order.
		/// Then sends emails to vendor and client about the transaction.
		/// It is common to ALL gateways.
		/// It is secure because we do not move from page.
		/// </summary>
		/// <param name="gateway">The current gateway</param>
		public void ProcessCheckOut(Rainbow.ECommerce.Gateways.GatewayBase gateway)
		{
			try
			{
				//Istantiate a new order object
				Order o = new Order();

				//Process the order
				SetMessage(o.Finalize(gateway, true));
			}
			catch(Exception err)
			{
				SetError(err.Message);
			}
		}

		public void SetMessage(string message)
		{
			MessageLabel.CssClass = "SubHead";
			MessageLabel.Text = message;
		}

		public void SetError(string error)
		{
			MessageLabel.CssClass = "Error";
			MessageLabel.Text = error;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: questa chiamata è richiesta da Progettazione Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///	Metodo necessario per il supporto della finestra di progettazione. Non modificare
		///	il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
