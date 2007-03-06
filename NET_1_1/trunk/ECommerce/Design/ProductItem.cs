using System;
using System.Xml;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Configuration;
using Esperantus;
using Rainbow.DesktopModules;
using Rainbow.Design;

namespace Rainbow.Design
{
    /// <summary>
    /// ProductItem
    /// </summary>
    public class ProductItem : UserControl, INamingContainer
    {
        private XmlDocument metadata;

		public string CurrencySymbol = "EUR";

		//The following added by Kelsey
		//This will support the product options
		//Split into a flag and the string to reduce the amount of times
		//The metadata is checked.

		//Xml string representing all the options
		private string myOptionString = null;

		public string OptionString
		{
			get
			{
				return myOptionString;
			}
		}

		//Do we actually have some options and set the myOptionString
		public bool ProductHasOptions
		{
			get
			{
				bool hasOptions = true;
				//Set the string with the xml string
				myOptionString = GetMetadata("ProductOptions");
				//if < 66 list is empty
				if (myOptionString == null || myOptionString.Length <= 60)
					hasOptions = false;
				return hasOptions;
			}
		}
		//End of additions by Kelsey

        public XmlDocument Metadata
        {
            get { return this.metadata; }
            set { this.metadata = value; }
        }

		public int ProductID
		{
			get 
			{ 
				try
				{
					return int.Parse(GetMetadata("ProductID")); 
				}
				catch
				{
					return 0;
				}
			}
		}

		public decimal UnitPrice
		{
			get 
			{ 
				try
				{
					return decimal.Parse(GetMetadata("UnitPrice"));
				}
				catch
				{
					return -1;
				}
			}
		}

		public decimal TaxRate
		{
			get 
			{ 
				try
				{
					return decimal.Parse(GetMetadata("TaxRate")); 
				}
				catch
				{
					return 0;
				}
			}
		}

		public decimal Weight
		{
			get 
			{ 
				try
				{
					return decimal.Parse(GetMetadata("Weight")); 
				}
				catch
				{
					return 0;
				}
			}
		}


		/// <summary>
		/// returns image path of thumbnail
		/// </summary>
		/// <param name="returnModelNumber">If true it returns model Number if image is missing</param>
		/// <returns></returns>
		public string GetThumbnailImageUrl(bool returnModelNumber)
		{
try{
			if(returnModelNumber && GetMetadata("ThumbnailFilename") != null)
				return GetMetadata("ShopPath") + "/" + GetMetadata("ThumbnailFilename");
			else if(GetMetadata("ModelNumber") != null)
				return GetMetadata("ShopPath") + "/" + GetMetadata("ModelNumber") + ".jpg";
			else
				return string.Empty;
}
catch
{return "";}
		}

		/// <summary>
		/// returns width of thumbnail
		/// </summary>
		/// <returns></returns>
		public Unit GetThumbnailImageWidth()
		{
			if (GetMetadata("ThumbnailFilename") != null)
				return Unit.Parse("0" + GetMetadata("ThumbnailWidth")+ "px");
			else
				return new Unit("");
		}

		/// <summary>
		/// returns width of thumbnail
		/// </summary>
		/// <returns></returns>
		public Unit GetThumbnailImageHeight()
		{
			if (GetMetadata("ThumbnailFilename") != null)
				return Unit.Parse("0" + GetMetadata("ThumbnailHeight")+ "px");
			else
				return new Unit("");
		}

		/// <summary>
		/// returns image path of Modified
		/// </summary>
		/// <param name="returnModelNumber">If true it returns model name if image is missing</param>
		/// <returns></returns>
		public string GetModifiedImageUrl(bool returnModelNumber)
		{
try{
			if(returnModelNumber && GetMetadata("ModifiedFilename") != null)
				return GetMetadata("ShopPath") + "/" + GetMetadata("ModifiedFilename");
			else if(GetMetadata("ModelNumber") != null)
				return GetMetadata("ShopPath") + "/" + GetMetadata("ModelNumber") + ".jpg";
			else
				return string.Empty;
}
catch
{return "";}
		}

		/// <summary>
		/// returns width of Modified
		/// </summary>
		/// <returns></returns>
		public Unit GetModifiedImageWidth()
		{
			if (GetMetadata("ModifiedFilename") != null)
				return Unit.Parse("0" + GetMetadata("ModifiedWidth")+ "px");
			else
				return new Unit("");
		}

		/// <summary>
		/// returns width of Modified
		/// </summary>
		/// <returns></returns>
		public Unit GetModifiedImageHeight()
		{
			if (GetMetadata("ModifiedFilename") != null)
				return Unit.Parse("0" + GetMetadata("ModifiedHeight")+ "px");
			else
				return new Unit("");
		}

		public string GetMetadata(string key)
        {
            XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);
            if (targetNode == null)
            {
                return null;
            }
            else
            {
                return targetNode.Value;
            }
        }

		public string DisplayPrice(decimal price)
		{
			try
			{
				Esperantus.Money myMoney = new Esperantus.Money(price, CurrencySymbol);
				return myMoney.ToString();
			}
			catch
			{
				return "error";
			}
		}

		public string DisplayPrice(decimal price, decimal taxrate)
		{
			try
			{
				price += ((price * taxrate) / 100M);

				Esperantus.Money myMoney = new Esperantus.Money(price, CurrencySymbol);
				return myMoney.ToString();
			}
			catch
			{
				return "error";
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="bydefault" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string GetCurrentImageFromTheme (string name, string bydefault) 
		{
			// Obtain PortalSettings from Current Context
			if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
			{
				PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				return pS.GetCurrentTheme().GetImage(name, bydefault).ImageUrl;
			}
			return bydefault;
		}

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			
		}
		#endregion
    }
}