﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// Il codice sorgente è stato generato automaticamente da Microsoft.VSDesigner, versione 1.1.4322.2032.
// 
namespace WinClientEcommerce.exportService {
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ExportServiceSoap", Namespace="http://localhost/rainbow/ecommerce/service/")]
    public class ExportService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public ExportService() {
            string urlSetting = System.Configuration.ConfigurationSettings.AppSettings["WinClientEcommerce.exportService.ExportService"];
            if ((urlSetting != null)) {
                this.Url = string.Concat(urlSetting, "");
            }
            else {
                this.Url = "http://localhost/Rainbow/ecommerce/service/exportservice.asmx";
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/rainbow/ecommerce/service/GetCategories", RequestNamespace="http://localhost/rainbow/ecommerce/service/", ResponseNamespace="http://localhost/rainbow/ecommerce/service/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetCategories(int moduleId) {
            object[] results = this.Invoke("GetCategories", new object[] {
                        moduleId});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetCategories(int moduleId, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetCategories", new object[] {
                        moduleId}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndGetCategories(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/rainbow/ecommerce/service/GetProducts", RequestNamespace="http://localhost/rainbow/ecommerce/service/", ResponseNamespace="http://localhost/rainbow/ecommerce/service/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetProducts(int moduleId, int categoryId) {
            object[] results = this.Invoke("GetProducts", new object[] {
                        moduleId,
                        categoryId});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetProducts(int moduleId, int categoryId, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetProducts", new object[] {
                        moduleId,
                        categoryId}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndGetProducts(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
    }
}
