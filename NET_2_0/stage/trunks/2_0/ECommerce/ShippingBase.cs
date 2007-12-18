using System;
using System.Configuration;
using System.Data;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Rainbow.DesktopModules;
using Rainbow.Configuration;
using Rainbow.UI.DataTypes;
using Esperantus;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Summary description for ShippingBase.
	/// </summary>
	public abstract class ShippingBase
	{
		/// <summary>
		/// Each ShippingObject must implement this name.
		/// Using this name Gateway manager can
		/// istantiate the class.
		/// </summary>
		public abstract string Name
		{
			get;
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

		/// <summary>
		/// Calculates the shipping cost of the cart.
		/// This will more likely change in future.
		/// </summary>
		/// <param name="CartID"></param>
		/// <param name="CountryID"></param>
		/// <param name="moduleID"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		public virtual Money Calculate(string CartID, string CountryID, int moduleID, WorkFlowVersion version)
		{
			ShoppingCartDB cart = new ShoppingCartDB();

			Money cartAmount = cart.GetTotal(CartID, moduleID, version, true);

			return PostprocessCheck(cartAmount);
		}

		/// <summary>
		/// Calculates the shipping cost of the cart.
		/// This passes cart values directly
		/// </summary>
		/// <param name="CartID"></param>
		/// <param name="CountryID"></param>
		/// <param name="moduleID"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		public virtual Money Calculate(string countryID, Money cartAmount, double weight)
		{
			return PostprocessCheck(cartAmount);
		}

		/// <summary>
		/// PostprocessCheck verifies that cost makes sense.
		/// Can be overridden in derived class for add more checks.
		/// It is recommended calling 
		/// return base.PostprocessCheck(inputMoney)
		/// in all derived classes. 
		/// </summary>
		/// <param name="inputMoney"></param>
		/// <returns></returns>
		protected virtual Money PostprocessCheck(Money inputMoney)
		{
			if(inputMoney == 0)
				throw new InvalidPackageException();

			if(inputMoney < 0)
				throw new InvalidDestinationException();
			
			return inputMoney;
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