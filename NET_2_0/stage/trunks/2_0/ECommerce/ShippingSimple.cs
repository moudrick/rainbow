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
	/// Summary description for ShippingSimple.
	/// </summary>
	public class ShippingSimple : ShippingBase
	{
		/// <summary>
		/// Each ShippingObject must implement this name.
		/// Using this name ShippingManager can
		/// istantiate the class.
		/// </summary>
		public override string Name
		{
			get{return "SimpleShipping";}
		}

		private ShippingItems m_Rates; 
		/// <summary>
		/// Sample
		/// US,CA;30;€ 30|IT;20;€ 120
		/// </summary>
		public string Rates
		{
			get{return m_Rates.ToString();}
			set{m_Rates = new ShippingItems(value);}
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
		public override Money Calculate(string cartID, string countryID, int moduleID, WorkFlowVersion version)
		{
			ShoppingCartDB cart = new ShoppingCartDB();

			Money cartAmount = cart.GetTotal(cartID, moduleID, version, true);

			double weight = 0;
			SqlDataReader sr = cart.GetItems(cartID, moduleID, version);
			try
			{
				while(sr.Read())
				{
					weight += Double.Parse(sr["Weight"].ToString());
				}
			}
			finally
			{
				sr.Close();
			}

			return Calculate(countryID,cartAmount, weight);

//			if (weight == 0)
//			{
//				cartAmount.Amount = 0;
//				return PostprocessCheck(cartAmount);
//			}
//
//			ShippingItem foundItem = null;
//
//			foreach(ShippingItem i in m_Rates)
//			{
//				if (i.DestinationCountry.IndexOf(countryID) >= 0)
//				{
//					foundItem = i;
//					break;
//				}
//			}
//
//			if (foundItem == null)
//				throw new InvalidDestinationException("I cannot find any suitable shipping price");
//			
//			//Free orders above
//			if (foundItem.FreeAbove.Amount > 0 && cartAmount.Amount >= foundItem.FreeAbove) 
//			{
//				cartAmount.Amount = 0;
//				return PostprocessCheck(cartAmount);
//			}
//
//			cartAmount.Amount = foundItem.Calculate(weight);
//
//			return PostprocessCheck(cartAmount);
		}

		/// <summary>
		/// Calculates the shipping cost of the cart.
		/// </summary>
		/// <param name="CartID"></param>
		/// <param name="CountryID"></param>
		/// <param name="moduleID"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		public override Money Calculate(string countryID, Money cartAmount, double weight)
		{
			if (weight == 0)
			{
				cartAmount.Amount = 0;
				return PostprocessCheck(cartAmount);
			}

			ShippingItem foundItem = null;

			foreach(ShippingItem i in m_Rates)
			{
				if (i.DestinationCountry.IndexOf(countryID) >= 0)
				{
					foundItem = i;
					break;
				}
			}

			if (foundItem == null)
				throw new InvalidDestinationException("I cannot find any suitable shipping price");
			
			//Free orders above
			if (foundItem.FreeAbove.Amount > 0 && cartAmount.Amount >= foundItem.FreeAbove) 
			{
				cartAmount.Amount = 0;
				return PostprocessCheck(cartAmount);
			}

			cartAmount.Amount = foundItem.Calculate(weight);

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
			if(inputMoney < 0)
				throw new InvalidDestinationException();

			return inputMoney;
		}

	}

	public class ShippingItem
	{
		/// <summary>
		/// Sample: US,CA;30;€ 30
		/// </summary>
		/// <param name="items"></param>
		public ShippingItem(string item)
		{
			string[] itemParts = item.Split(';');
			if (itemParts.Length < 3 || itemParts.Length > 4)
				throw new ArgumentOutOfRangeException("item", item, "A Shipping Item must have exactly 3 or 4 parts");

			DestinationCountry = itemParts[0];
			PackageWeight = double.Parse(itemParts[1]);
			Price = Esperantus.Money.Parse(itemParts[2]);
			if(itemParts.Length == 4 && itemParts[3].Length > 0)
				FreeAbove = Esperantus.Money.Parse(itemParts[3]);
			else
			{
				//same as price but 0 amount
				FreeAbove = Esperantus.Money.Parse(itemParts[2]);
				FreeAbove.Amount = 0;
			}
		}
			
		/// <summary>
		/// Destination countries, comma separated. Eg: US,CA...
		/// </summary>
		public string DestinationCountry;

		/// <summary>
		/// Max package weight. If it is 30 kg and the order is 45 the price is doubled.
		/// </summary>
		public double PackageWeight;

		/// <summary>
		/// The price
		/// </summary>
		public Esperantus.Money Price;

		/// <summary>
		/// Orders above this value will be free
		/// </summary>
		public Esperantus.Money FreeAbove;

		public decimal Calculate(double actualWeight)
		{
			int PackageNumber = (int)(actualWeight / PackageWeight);
			PackageNumber += (actualWeight % PackageWeight) > 0 ? 1 : 0;

			return Price.Amount * (decimal) PackageNumber;
		}

		public override string ToString()
		{
			if(FreeAbove.Amount > 0)
			{
				return DestinationCountry + ";" + PackageWeight.ToString() + ";" + Price.ToString() + ";" + FreeAbove.ToString();
			}
			{
				return DestinationCountry + ";" + PackageWeight.ToString() + ";" + Price.ToString();
			}
		}
	}

	public class ShippingItems : System.Collections.CollectionBase
	{
		/// <summary>
		/// Sample: US,CA;30;€ 30|IT;20;€ 120
		/// </summary>
		/// <param name="items"></param>
		public ShippingItems(string items)
		{
			string[] itemsList = items.Split('|');

			foreach(string itemString in itemsList)
			{
				ShippingItem newItem = new ShippingItem(itemString);
				this.Add(newItem);
			}			
		}

		public void Add(ShippingItem s)
		{
			InnerList.Add(s);			
		}

		public ShippingItem this[int index]
		{
			get
			{
				return (ShippingItem) InnerList[index];
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder toReturn = new System.Text.StringBuilder();

			for (int i=0; i < InnerList.Count; i++)
			{
				if (i > 0) toReturn.Append("|");
				toReturn.Append(this[i].ToString());
			}

			return toReturn.ToString();
		}
	}
}