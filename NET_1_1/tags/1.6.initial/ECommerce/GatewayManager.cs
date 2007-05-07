using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Web;

using Rainbow.ECommerce.Gateways;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// GatewayManager
	/// </summary>
	public class GatewayManager
	{
		/// <summary>
		/// Return available Gateways from current web.config/BankGateways section
		/// </summary>
		/// <returns></returns>
		public static Rainbow.ECommerce.Gateways.GatewayBase[] GetGateways()
		{
			ArrayList Gateways = new ArrayList();

			// Get custom config
			NameValueCollection mySettings = (NameValueCollection) ConfigurationSettings.GetConfig("BankGateways");

			if (mySettings != null)
			{
				foreach(string GatewayType in mySettings.AllKeys)
				{
					string[] myType = mySettings[GatewayType].Split(new char[] {','});
					System.Diagnostics.Debug.Assert(myType.Length == 2, "Expected: Class,Assembly");
					
					string className = myType[0].Trim();
					string assembly = myType[1].Trim();

					//Istantiate the Gateway and add it to the list
					Gateways.Add(GetGateway(className, assembly));
				}
			}
			return (Rainbow.ECommerce.Gateways.GatewayBase[]) Gateways.ToArray(typeof(Rainbow.ECommerce.Gateways.GatewayBase));
		}

		/// <summary>
		/// GetGateway
		/// </summary>
		/// <param name="className">Class Name (with full namespace)</param>
		/// <param name="assembly">DLL Assembly</param>
		/// <returns></returns>
		public static Rainbow.ECommerce.Gateways.GatewayBase GetGateway(string className, string assembly)
		{
			HttpRequest req = HttpContext.Current.Request;
			string appPath = req.ApplicationPath == "/" ? "" : req.ApplicationPath;

			//Search for assembly in bin dir
			string assemblyName = HttpContext.Current.Server.MapPath(appPath + "/bin/" + assembly);

			// Use reflection for loading assembly
			Assembly a = Assembly.LoadFrom(assemblyName);
			//Rainbow.Helpers.LogHelper.Log.Debug("Succesfully used reflection for loading assembly: '" + assemblyName + "'");

			Rainbow.ECommerce.Gateways.GatewayBase Gateway;
			object o = null;
			try
			{
				// Use reflection for create a Gateway instance
				o = a.CreateInstance(className);
				Gateway = (Rainbow.ECommerce.Gateways.GatewayBase) o;
			}
			catch(System.InvalidCastException ex)
			{
				string classtype = string.Empty;
				if (o != null && o.GetType() != null)
					classtype = o.GetType().FullName;

				throw new System.Reflection.TargetInvocationException(
					"Use reflection for create a Gateway instance failed. " +
					"Class name was '" + className + "'. " + 
					"Class type was '" + classtype + "'. "
					, ex);			
			}

			return Gateway;
		}

		/// <summary>
		/// GetGateway
		/// </summary>
		/// <param name="name">A string with the name of the Gateway</param>
		/// <returns></returns>
		public static Rainbow.ECommerce.Gateways.GatewayBase GetGateway(string name)
		{
			Rainbow.ECommerce.Gateways.GatewayBase[] Gateways = GetGateways();

			//Cyle all available Gateways searching for given name
			foreach(Rainbow.ECommerce.Gateways.GatewayBase Gateway in Gateways)
			{
				if (Gateway.Name == name)
					return Gateway;
			}
			return null; //Gateway not found
		}

		/// <summary>
		/// Get a reference to specified Gateway and
		/// load data for specified merchant in it
		/// </summary>
		/// <param name="GatewayType"></param>
		/// <param name="merchantID"></param>
		/// <returns></returns>
		public static Rainbow.ECommerce.Gateways.GatewayBase GetGatewayMerchant(string GatewayType, string merchantID)
		{
			// Create the specific Gateway using Gateway type
			Rainbow.ECommerce.Gateways.GatewayBase Gateway = GatewayManager.GetGateway(GatewayType);

			//Set MerchantID
			Gateway.MerchantID = merchantID;

			return MerchantManager.LoadMerchantData(Gateway);
		}
	}
}