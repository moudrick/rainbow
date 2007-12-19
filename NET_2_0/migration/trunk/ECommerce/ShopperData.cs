using System;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// A data class that encapsulates details about
	/// a particular customer 
	/// </summary>
	[Serializable()]      
	public class ShopperData
	{
		/// <summary>
		/// FullName
		/// </summary>
		public string FullName;
		/// <summary>
		/// Company
		/// </summary>
		public string Company;
		public string Address;
		public string ZipCode;
		public string City;
		public string Country;
		public string CountryID;
		public string StateID;
		public string State;
		public string Phone;
		public string Fax;
		public string PIva;
		public string CodFisc;
		public string EMail;
		public string Note;
	}
}
