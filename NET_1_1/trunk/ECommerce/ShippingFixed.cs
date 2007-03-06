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
	/// Summary description for ShippingFixed.
	/// </summary>
	public class ShippingFixed : ShippingBase
	{
		/// <summary>
		/// Each ShippingObject must implement this name.
		/// Using this name ShippingManager can
		/// istantiate the class.
		/// </summary>
		public override string Name
		{
			get{return "FixedShipping";}
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
			ShoppingCartDB cart = new ShoppingCartDB();

			Money cartAmount = cart.GetTotal(CartID, moduleID, version, true);

			//Calulate percentage
			if (Rate > 0)
				cartAmount.Amount = cartAmount.Amount * (Rate / 100M);
			else
				cartAmount.Amount = 0;

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
		protected override Money PostprocessCheck(Money inputMoney)
		{
			if (inputMoney < MinValue)
				inputMoney = MinValue;

			if (inputMoney > MaxValue)
				inputMoney = MaxValue;

			return base.PostprocessCheck(inputMoney);
		}

		Percentage m_rate = new Percentage(0);
		/// <summary>
		/// Set here a value from 0 to 100.
		/// Shipping will be calculated as a percentage of the total cost.
		/// </summary>
		public Percentage Rate
		{
			get
			{
				return m_rate;
			}
			set
			{
				m_rate = value;
			}
		}

		Esperantus.Money m_maxValue = new Esperantus.Money(999999M, "USD");
		/// <summary>
		/// Total shipping will not be more than specified value.
		/// 0 means no limit.
		/// </summary>
		public Esperantus.Money MaxValue
		{
			get
			{
				return m_maxValue;
			}
			set
			{
				if (value < MinValue)
					throw new ArgumentException("MaxValue must be greater than MinValue");
				m_maxValue = value;
			}
		}

		Esperantus.Money m_minValue = new Esperantus.Money(0, "USD");
		/// <summary>
		/// Total shipping will not be less than specified value. 
		/// </summary>
		public Esperantus.Money MinValue
		{
			get
			{
				return m_minValue;
			}
			set
			{
				if (value > MaxValue)
					throw new ArgumentException("MaxValue must be greater than MinValue");
				m_minValue = value;
			}
		}
	}
}
