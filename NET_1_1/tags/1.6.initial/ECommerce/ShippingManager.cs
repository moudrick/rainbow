using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Web;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// ShippingManager
	/// </summary>
	public class ShippingManager
	{
		/// <summary>
		/// Return available Shippings from current web.config/ShippingObjects section
		/// </summary>
		/// <returns></returns>
		public static ShippingBase[] GetShippings()
		{
			ArrayList Shippings = new ArrayList();

			// Get custom config
			NameValueCollection mySettings = (NameValueCollection) ConfigurationSettings.GetConfig("ShippingObjects");

			if (mySettings != null)
			{
				foreach(string ShippingType in mySettings.AllKeys)
				{
					string[] myType = mySettings[ShippingType].Split(new char[] {','});
					System.Diagnostics.Debug.Assert(myType.Length == 2, "Expected: Class,Assembly");
					
					string className = myType[0].Trim();
					string assembly = myType[1].Trim();

					//Istantiate the Shipping and add it to the list
					Shippings.Add(GetShipping(className, assembly));
				}
			}
			return (ShippingBase[]) Shippings.ToArray(typeof(ShippingBase));
		}

		/// <summary>
		/// GetShipping
		/// </summary>
		/// <param name="className">Class Name (with full namespace)</param>
		/// <param name="assembly">DLL Assembly</param>
		/// <returns></returns>
		public static ShippingBase GetShipping(string className, string assembly)
		{
			HttpRequest req = HttpContext.Current.Request;
			string appPath = req.ApplicationPath == "/" ? "" : req.ApplicationPath;

			//Search for assembly in bin dir
			string assemblyName = HttpContext.Current.Server.MapPath(appPath + "/bin/" + assembly);
			
			// Use reflection for loading assembly
			Assembly a = Assembly.LoadFrom(assemblyName);
			// Use reflection for create a Shipping instance
			ShippingBase Shipping = (ShippingBase) a.CreateInstance(className);

			return Shipping;
		}

		/// <summary>
		/// GetShipping
		/// </summary>
		/// <param name="name">A string with the name of the Shipping</param>
		/// <returns></returns>
		public static ShippingBase GetShipping(string name)
		{
			ShippingBase[] Shippings = GetShippings();

			//Cyle all available Shippings searching for given name
			foreach(ShippingBase Shipping in Shippings)
			{
				if (Shipping.Name == name)
					return Shipping;
			}
			return null; //Shipping not found
		}

		/// <summary>
		/// Get a reference to specified Shipping and
		/// load data for specified merchant in it
		/// </summary>
		/// <param name="ShippingType"></param>
		/// <param name="merchantID"></param>
		/// <returns></returns>
		public static ShippingBase GetShippingMerchant(string ShippingType, string merchantID)
		{
			// Create the specific Shipping using Shipping type
			ShippingBase Shipping = ShippingManager.GetShipping(ShippingType);

			//Set MerchantID
			Shipping.MerchantID = merchantID;

			return MerchantManager.LoadMerchantData(Shipping);
		}
	}
}