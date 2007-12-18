namespace Rainbow.ECommerce.DesktopModules
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
	public class OptionsEdit : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox newOptionBox;
		protected Esperantus.WebControls.Button btnAddOption;
		protected System.Web.UI.WebControls.ListBox lbOptions;
		protected Esperantus.WebControls.Button btnDeleteOption;
		protected System.Xml.XmlDocument xmlOptionsDoc;
		protected string lbOptionsSelectedItem;

		// to use several of these controls for the same product
		protected string optionName;
		public string OptionName
		{
			set
			{
				optionName = value;
			}
			get
			{
				return optionName;
			}
		}

		protected static string optionString;
		public string OptionString
		{
			set
			{
				optionString = value;
				//displayOptions(xmlOptionsDoc);
			}
			get{return optionString;}

		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			//Get the xml Document of Options

			//This will make sure that xmlOptionsDoc is not null and current
			xmlOptionsDoc = getOptionsXml(optionString);
			if(Page.IsPostBack == false)
				displayOptions(xmlOptionsDoc);
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
			this.btnAddOption.Click += new System.EventHandler(this.btnAddOption_Click);
			this.btnDeleteOption.Click += new System.EventHandler(this.btnDeleteOption_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnAddOption_Click(object sender, System.EventArgs e)
		{
			//Add the value in newOptionsBox to the xmlOptionsDoc
			//This will add a new option into the list box and xml file
			string tempOption = newOptionBox.Text.Trim();
			//check to make sure a value exists before processing
			if(tempOption != null)
			{
				//Make sure the option does not already exist
				//Create a search String for the option we are going to delete
				string searchString = "options/" + OptionName + "[value='" + tempOption + "']";
				XmlNode foundNode = xmlOptionsDoc.SelectSingleNode(searchString);
				if(foundNode == null)
				{
					XmlNodeList nodeList = xmlOptionsDoc.GetElementsByTagName("options");
					XmlElement option = xmlOptionsDoc.CreateElement(OptionName);
					XmlElement optionValue = xmlOptionsDoc.CreateElement("value");
					optionValue.InnerText = tempOption;
					option.AppendChild(optionValue);
					nodeList.Item(0).AppendChild(option);
					//Clear the text box for new entry
					newOptionBox.Text = "";
					displayOptions(xmlOptionsDoc);
				}
			}

		}

		private void btnDeleteOption_Click(object sender, System.EventArgs e)
		{
			//Create a search String for the option we are going to delete
			string searchString = "options/" + OptionName + "[value='" + lbOptions.SelectedItem + "']";
			XmlNode foundNode = xmlOptionsDoc.SelectSingleNode(searchString);
			//Only try to delete if it exists
			if(foundNode != null)
			{
				xmlOptionsDoc["options"].RemoveChild(foundNode);
				//Refresh the listbox
				displayOptions(xmlOptionsDoc);
			}
		}

		#region options methods
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

		/// <summary>
		/// protected displayOptions(XmlDocument)
		/// fill the options in the listbox
		/// </summary>
		/// <param name="myXmlDoc"></param>
		protected void displayOptions(XmlDocument myXmlDoc)
		{
			lbOptions.Items.Clear();
			//XmlNodeList nodeList = new XmlNodeList();
			XmlNodeList nodeList = myXmlDoc.GetElementsByTagName(OptionName);
			foreach(XmlNode node in nodeList)
				lbOptions.Items.Add(node["value"].InnerText);
			//Now refresh the optionString with the updated values
			optionString = getXmlString(myXmlDoc);
		}

		#endregion


	}
}
