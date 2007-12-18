using Rainbow.Configuration;
using Rainbow.UI.DataTypes;

namespace Rainbow.DesktopModules.Version
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.IO;
	using System.Net;
	using System.Xml;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Rainbow.UI;
	using Rainbow.UI.WebControls;

	/// <summary>
	///		Summary description for VisaClassic.
	/// </summary>
	public class WeatherCom : PortalModuleControl
	{
		protected System.Web.UI.WebControls.Xml Xml1;

		private void Page_Load(object sender, System.EventArgs e)
		{
			DisplayWeather();
		}

		private void DisplayWeather()
		{
			try 
			{
				// Request URL
				string wsUrl = "http://xoap.weather.com/weather/local/" + 
					// City Code
					Settings["CityCode"].ToString() + 
					"?cc=*" + 
					// Forecast Days
					"&dayf=" + Settings["Forecast"].ToString() + 
					"&prod=xoap&par=1010760847&key=36e1f14b468962e2" +
					// Set Unit
					"&unit=" + Settings["Unit"].ToString();
					
				// Contact service for content
				HttpWebRequest wrq = (HttpWebRequest)WebRequest.Create(wsUrl);
				// Load response
				WebResponse resp = wrq.GetResponse();
				// Create new stream for XmlTextReader
				Stream str = resp.GetResponseStream();
				XmlTextReader reader = new XmlTextReader(str);
				reader.XmlResolver = null;
				// Create Xml document
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				Xml1.Document = doc;
				if(Settings["Unit"].ToString() == "m")
                    Xml1.TransformSource = "WeatherComM.xslt";
				else
					Xml1.TransformSource = "WeatherCom.xslt";
			}
			catch(Exception ex) 
			{
				ErrorHandler.Publish(LogLevel.Warn, "Weather.com Error", ex);
			}
		}

		public WeatherCom() 
		{
			SettingItem cityCode = new SettingItem(new StringDataType());
			cityCode.Required = false;
			cityCode.Value = "BKXX0001";
			this._baseSettings.Add("CityCode", cityCode);

			SettingItem forecast = new SettingItem(new StringDataType());
			forecast.Required = false;
			forecast.Value = "3";
			this.BaseSettings.Add("Forecast", forecast);

			SettingItem setUnit = new SettingItem(new StringDataType());
			setUnit.Required = false;
			setUnit.Value = "m";
			this._baseSettings.Add("Unit", setUnit);
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

		public override Guid GuidID
		{
			get
			{
				return new Guid("{078FDE24-45A0-4f70-A6C8-E7F2E498B9BC}");
			}
		}

	}
}
