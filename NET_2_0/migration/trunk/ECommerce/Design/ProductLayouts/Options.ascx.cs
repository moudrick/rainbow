using Rainbow.Configuration;

namespace Rainbow.ECommerce.Design
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;

	/// <summary>
	///		Summary description for Options.
	/// </summary>
	public class Options : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DropDownList ddOptions3;
		protected System.Web.UI.WebControls.DropDownList ddOptions1;
		protected System.Web.UI.WebControls.DropDownList ddOptions2;
		protected System.Web.UI.WebControls.Label lblOptions;

		// used to modify the SetOptions string
		protected System.Xml.XmlDocument xmlOptionsDoc;

		// This will accept an xmlText string and populate the option drop down
		private string setOptions = "";
		public string SetOptions
		{
			set
			{
				setOptions = value; // save it
				setOptionValues(value);
			}
			get
			{
				// this will be used to get back the selected options

				// get the options as an XML document
				xmlOptionsDoc = getOptionsXml(setOptions);

				// no need to delete the existing selection (we do not have any)

				XmlNodeList nodeList = xmlOptionsDoc.GetElementsByTagName("options");

				if(ddOptions1.SelectedItem != null)
				{
					XmlElement option = xmlOptionsDoc.CreateElement("option1");
					XmlElement optionValue = xmlOptionsDoc.CreateElement("selected");
					optionValue.InnerText = ddOptions1.SelectedItem.Value;
					option.AppendChild(optionValue);
					nodeList.Item(0).AppendChild(option);
				}

				if(ddOptions2.SelectedItem != null)
				{
					XmlElement option = xmlOptionsDoc.CreateElement("option2");
					XmlElement optionValue = xmlOptionsDoc.CreateElement("selected");
					optionValue.InnerText = ddOptions2.SelectedItem.Value;
					option.AppendChild(optionValue);
					nodeList.Item(0).AppendChild(option);
				}

				if(ddOptions3.SelectedItem != null)
				{
					XmlElement option = xmlOptionsDoc.CreateElement("option3");
					XmlElement optionValue = xmlOptionsDoc.CreateElement("selected");
					optionValue.InnerText = ddOptions3.SelectedItem.Value;
					option.AppendChild(optionValue);
					nodeList.Item(0).AppendChild(option);
				}

				// return the new xml string
				return getXmlString(xmlOptionsDoc);
			}
		}

		//This will set the options title label
		public string OptionTitle
		{
			set
			{
				lblOptions.Text = value;
			}
		}
		/// <summary>
		/// protected XmlDocument getOptionsXml(string)
		/// provide a string representing an xml file or null for a new document
		/// </summary>
		/// <param name="myXmlString"></param>
		/// <returns>an Xml file</returns>
		protected void setOptionValues(string myXmlString)
		{
			//Create an xml Document
			XmlDocument myXmlDoc = new XmlDocument();

			try
			{
				ddOptions1.Visible = false;
				ddOptions2.Visible = false;
				ddOptions3.Visible = false;
				lblOptions.Visible = false;

				//if(myXmlString != null || myXmlString.Length > 0)
				if(myXmlString != null && myXmlString.Length > 0)
				{
					//We will create the xml document from the data
					//Create our Xml Document from the db data
					myXmlDoc.LoadXml(myXmlString);
			
					//-------------------------------------
					//Now fill the Drop down Box: option 1
					ddOptions1.Items.Clear();
					XmlNodeList nodeList1 = myXmlDoc.GetElementsByTagName("option1");

					//Check if we need to hide the options
					if(!(nodeList1 == null || nodeList1.Count == 0))
					{
						foreach(XmlNode node in nodeList1)
							ddOptions1.Items.Add(node["value"].InnerText);

						ddOptions1.Visible = true;
						lblOptions.Visible = true;
					}
			
					//-------------------------------------
					//Now fill the Drop down Box: option 2
					ddOptions2.Items.Clear();
					XmlNodeList nodeList2 = myXmlDoc.GetElementsByTagName("option2");

					//Check if we need to hide the options
					if(!(nodeList2 == null || nodeList2.Count == 0))
					{
						foreach(XmlNode node in nodeList2)
							ddOptions2.Items.Add(node["value"].InnerText);

						ddOptions2.Visible = true;
						lblOptions.Visible = true;
					}
			
					//-------------------------------------
					//Now fill the Drop down Box: option 3
					ddOptions3.Items.Clear();
					XmlNodeList nodeList3 = myXmlDoc.GetElementsByTagName("option3");

					//Check if we need to hide the options
					if(!(nodeList3 == null || nodeList3.Count == 0))
					{
						foreach(XmlNode node in nodeList3)
							ddOptions3.Items.Add(node["value"].InnerText);

						ddOptions3.Visible = true;
						lblOptions.Visible = true;
					}
				}
				else
				{
					ddOptions1.Items.Clear();
					ddOptions2.Items.Clear();
					ddOptions3.Items.Clear();
				}
			}
			catch(Exception ex)
			{
				Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error, "Error setOptionValues", ex);


				// hide everything
				ddOptions1.Items.Clear();
				ddOptions2.Items.Clear();
				ddOptions3.Items.Clear();

				ddOptions1.Visible = false;
				ddOptions2.Visible = false;
				ddOptions3.Visible = false;
				lblOptions.Visible = false;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		
		public String ImageProduct
		{
			get
			{
				if(ViewState["ImageProductControl"] != null)
					return ViewState["ImageProductControl"].ToString();
				else
					return null;
			}
			set
			{
				ViewState["ImageProductControl"] = value;
			}
		}

				
		protected ImageButton ImageProductButton
		{
			get
			{
				if(ImageProduct != null)
					return (ImageButton) Parent.FindControl(ImageProduct);
				else
					return null;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        #region XML Helper functions

		private string getXmlString(XmlDocument myXmlDoc)
		{
			string searchString = "options";
			string xmlString = "";
			XmlNode foundNode = myXmlDoc.SelectSingleNode(searchString);
			if(foundNode != null)
				xmlString = myXmlDoc.FirstChild.OuterXml + foundNode.OuterXml;
			return xmlString;
		}

		/// <summary>
		/// protected XmlDocument getOptionsXml(string)
		/// provide a string representing an xml file or null for a new document
		/// </summary>
		/// <param name="myXmlString"></param>
		/// <returns>an Xml file</returns>
		protected XmlDocument getOptionsXml(string myXmlString)
		{
			//Create a xml Document
			XmlDocument myXmlDoc = new XmlDocument();

			//We will need to check to see if the Database contains options data
			//If it does we will create the xmlOptionsDoc from the data
			//else we will reate an xml Document from scratch


			if(myXmlString == null || myXmlString.Length == 0)
			{
				//Create our Xml Declaration
				XmlDeclaration newDec = myXmlDoc.CreateXmlDeclaration("1.0","UTF-8",null);
				//Now add it to the Xml Document
				myXmlDoc.AppendChild(newDec);
				//Now create the root Element
				XmlElement newRoot = myXmlDoc.CreateElement("options");
				//Now add it to the Xml Document
				myXmlDoc.AppendChild(newRoot);	
			}
			else
			{
				//We will create the xml document from the data
				//Create our Xml Document from the db data
				myXmlDoc.LoadXml(myXmlString);
			}
			//myXmlDoc is either populated or an empty Xml formated file
			return myXmlDoc;
		}

#endregion
	}
}
