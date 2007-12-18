using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;
using Rainbow.DesktopModules;

namespace Rainbow.ECommerce
{
	[Serializable()]      
	public class OrderDetail
	{
		public string productID;
		public string ModelName;
		public string ModelNumber;
		public string ModelOptions;    // from the metadata
		public int Quantity = 1;
		public Esperantus.Money UnitPrice;
		public double Weight = 0;

		public Esperantus.Money Total
		{
			get
			{
				return new Esperantus.Money(UnitPrice.Currency, UnitPrice.Amount * Quantity);
			}
			set	
			{
				// It is read-only because it is a calculated field.
				// We must specify Set because we want it be serialized
			}
		}
		
		// Constructor (overload)
		public OrderDetail()
		{
		}
		
		// Constructor (overload)
		public OrderDetail(string _productID, string _ModelName, string _ModelOptions, string _ModelNumber, int _Quantity, Esperantus.Money _UnitPrice, double _Weight)
		{
			productID = _productID;
			ModelName = _ModelName;
			ModelNumber = _ModelNumber;
			ModelOptions = _ModelOptions;
			Quantity = _Quantity;
			UnitPrice = _UnitPrice;
			Weight = _Weight;
		}
	}
}