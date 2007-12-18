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
	/// <summary>
	/// A collection of orders detail
	/// </summary>
	[Serializable()]      
	public class OrderDetails : CollectionBase
	{
		public OrderDetail this[int index]
		{
			get
			{
				return((OrderDetail) InnerList[index]);
			}
		}

		public int Add(OrderDetail od)
		{
			return(InnerList.Add(od));
		}

		public void Load(string OrderID)
		{
			//Clear InnerList collection
			InnerList.Clear();

			OrdersDB order = new OrdersDB();

			SqlDataReader drDetails = order.GetOrderDetails(OrderID);
			try
			{
				while (drDetails.Read())
				{
					OrderDetail od = new OrderDetail();
					od.productID = drDetails["ProductID"].ToString();
					od.ModelName = drDetails["ModelName"].ToString();
					od.ModelNumber = drDetails["ModelNumber"].ToString();
					od.Quantity = Int32.Parse(drDetails["Quantity"].ToString());
					od.UnitPrice = new Esperantus.Money(Decimal.Parse(drDetails["UnitPrice"].ToString()), drDetails["ISOCurrencySymbol"].ToString());
					od.Weight = double.Parse(drDetails["Weight"].ToString());;
				
					// extract options from metadata
					od.ModelOptions = OrdersDB.DecodeOptions(drDetails["MetadataXml"].ToString());
				
					// Add New Order Detail
					this.Add(od);
				}
			}
			finally
			{
				// always call Close when done reading.
				drDetails.Close();
			}
		}

		public double TotalWeight
		{
			get
			{
				double myWeight = 0;
				foreach(OrderDetail od in InnerList)
				{
					// old: myWeight += od.Weight;
					myWeight += (od.Quantity * od.Weight); // by Christian
				}
				return myWeight;
			}
			set
			{
				// It is read-only because it is a calculated field.
				// We must specify Set because we want it be serialized
			}
		}
	}
}