using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;
using RewriteRules.Rainbow.Tools;
using RulesEngine;

namespace RewriteRules.Rainbow
{
	/// <summary>
	/// This is a legacy Rule which supports all of the functionality created by John Mandia, Manu, Jes and Cory.
	/// </summary>
	public class LegacyRule : IRules
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appl"></param>
		/// <param name="_path"></param>
		/// <param name="settingsSection"></param>
		/// <returns></returns>
		public string Execute(HttpApplication appl, string _path, string settingsSection)
		{
			// Collect the settings for this rule
			IDictionary Settings = (IDictionary) ConfigurationSettings.GetConfig(settingsSection);

			string _handlerFlag = "go";
			bool _keywordSplitter = false;
			bool _aliasinurl = true;
			
			// collect settings
			if(Settings["handlerflag"]!= null)
			{
				_handlerFlag = ((string) Settings["handlerflag"]).ToLower(CultureInfo.InvariantCulture);
			}
			
			if(Settings["keywordsplitter"]!= null)
			{
				_keywordSplitter = bool.Parse(Settings["keywordsplitter"].ToString());
			}

			if(Settings["aliasinurl"] != null)
			{
				_aliasinurl = bool.Parse(Settings["aliasinurl"].ToString());
			}
			else
			{
				if(ConfigurationSettings.AppSettings["UseAlias"]!= null)
					_aliasinurl = bool.Parse(ConfigurationSettings.AppSettings["UseAlias"]);
			}

			//determine if the user is navigating to a url to be handled
			string _startpath = (Engine.ApplicationPath(appl).ToString().ToLower(CultureInfo.InvariantCulture) + "/" + _handlerFlag + "/");
			string path = _path;

			if (path.ToLower(CultureInfo.InvariantCulture).StartsWith(_startpath))
			{
				//Remove start tag...
				path = path.Substring(_startpath.Length);

				//split the path into components
				string[] parts = path.Split('/');

				StringBuilder sb = new StringBuilder(Engine.ApplicationPath(appl).ToString());
				sb.Append("/");
				sb.Append(General.DefaultPage);

				if (_aliasinurl)
				{
					General.AddAttribute(parts, sb, 0, "?Alias=");
					//General.AddAttribute(parts, sb, 1, "&TabID=");
					General.AddAttribute(parts, sb, 1, "&PageID=");
					General.AddAttribute(parts, sb, 2, "&lang=");
					// jes1111
					if (_keywordSplitter)
						General.KeywordSplitter(parts, sb, 3);
					else
						General.AddAttribute(parts, sb, 3, "&");
				}
				else
				{
					//Cory Isakson fix
					//General.AddAttribute(parts, sb, 0, "?TabID=");
					General.AddAttribute(parts, sb, 0, "?PageID=");
					General.AddAttribute(parts, sb, 1, "&lang=");
					// jes1111
					if (_keywordSplitter)
						General.KeywordSplitter(parts, sb, 2);
					else
						General.AddAttribute(parts, sb, 2, "&");
				}
				return sb.ToString();
			}
			else
				//return null if no changes
				return "";
		}

	}

}