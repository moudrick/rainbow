using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using MasterCSharp.WebServices;
using Rainbow.ECommerce;

namespace Rainbow.Ecommerce.Services 
{
	[WebService(Namespace="http://rainbowportal.net/ecommerce/service/")]
	[SoapDocumentService(ParameterStyle=SoapParameterStyle.Bare)]
	public class ExportService : WebService 
	{
		public AuthHeader Credentials;

		[AuthExtension]
		[SoapHeader("Credentials")]
		[WebMethod]
		[CompressionExtension]
		public DataSet GetCategories(int moduleId)
		{
			ProductsDB p = new ProductsDB();
			return p.GetProductsCategoryListDataSet(moduleId);
		}
		[AuthExtension]
		[SoapHeader("Credentials")]
		[WebMethod]
		[CompressionExtension]
		public DataSet GetProducts(int moduleId, int categoryId)
		{
			ProductsDB p = new ProductsDB();
			return p.GetProducts(moduleId, categoryId, Rainbow.Configuration.WorkFlowVersion.Staging);
		}

//		[WebMethod]
//		public DataSet GetProductsProduction(int moduleId, int categoryId)
//		{
//			ProductsDB p = new ProductsDB();
//			return p.GetProducts(moduleId, categoryId, Rainbow.Configuration.WorkFlowVersion.Production);
//		}
//
//		[WebMethod]
//		public DataSet GetProductsStaging(int moduleId, int categoryId)
//		{
//			ProductsDB p = new ProductsDB();
//			return p.GetProducts(moduleId, categoryId, Rainbow.Configuration.WorkFlowVersion.Staging);
//		}
//
//		[WebMethod]
//		public DataSet GetProductsPagedProduction(int moduleId, int categoryId, int page, int recordsPerPage)
//		{
//			ProductsDB p = new ProductsDB();
//			return p.GetProductsPaged(moduleId, categoryId, page, recordsPerPage, Rainbow.Configuration.WorkFlowVersion.Production);
//		}
//
//		[WebMethod]
//		public DataSet GetProductsPagedStaging(int moduleId, int categoryId, int page, int recordsPerPage)
//		{
//			ProductsDB p = new ProductsDB();
//			return p.GetProductsPaged(moduleId, categoryId, page, recordsPerPage, Rainbow.Configuration.WorkFlowVersion.Staging);
//		}

		public ExportService()
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
