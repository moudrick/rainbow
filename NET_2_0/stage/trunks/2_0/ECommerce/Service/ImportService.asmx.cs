using System;
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using MasterCSharp.WebServices;
using Rainbow.Helpers;
using Rainbow.ECommerce;

namespace Rainbow.Ecommerce.Services 
{
	[WebService(Namespace="http://rainbowportal.net/ecommerce/service/")]
	public class ImportService : WebService 
	{
		public AuthHeader Credentials;

		[AuthExtension]
		[SoapHeader("Credentials")]
		[WebMethod]
		[CompressionExtension]
		public void UpdateProducts(DataSet products)
		{
			ProductsDB p = new ProductsDB();
			foreach(DataRow dr in products.Tables[0].Rows)
			{
				if(bool.Parse(dr["deleted"].ToString()))
					p.DeleteProduct(int.Parse(dr["ProductId"].ToString()));
				else
				p.UpdateProduct(
					int.Parse(dr["ModuleId"].ToString()),
					int.Parse(dr["ProductId"].ToString()),
					int.Parse(dr["DisplayOrder"].ToString()),
					dr["MetadataXml"].ToString(),
					dr["ShortDescription"].ToString(),
					dr["LongDescription"].ToString(),
					dr["ModelName"].ToString(),
					dr["ModelNumber"].ToString(),
					double.Parse(dr["UnitPrice"].ToString()),
					byte.Parse(dr["FeaturedItem"].ToString()),
					int.Parse(dr["CategoryId"].ToString()),
					double.Parse(dr["Weight"].ToString()),
					double.Parse(dr["TaxRate"].ToString())
					);
			}
		}

		public ImportService()
		{
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion	
	}
}
