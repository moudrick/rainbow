using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Rainbow.DesktopModules;
using Rainbow.Configuration;
using Rainbow.UI.DataTypes;
using Esperantus;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// ShippingElectronicDelivery is always 0.
	/// It assumes you have no delivery costs.
	/// </summary>
	public class ShippingElectronicDelivery : ShippingBase
	{
		/// <summary>
		/// Each ShippingObject must implement this name.
		/// Using this name ShippingManager can
		/// istantiate the class.
		/// </summary>
		public override string Name
		{
			get{return "ElectronicDelivery";}
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
		public override Money Calculate(string CartID, string CountryID, int moduleID, WorkFlowVersion version)
		{
			//We simply use the GetTotal()
			//to get correct Money currency
			ShoppingCartDB cart = new ShoppingCartDB();
			Money cartAmount = cart.GetTotal(CartID, moduleID, version, true);
			
			//We zero the amount
			cartAmount.Amount = 0;

			//We do not call base process because we simply return 0
			return cartAmount;
		}
	}
}
