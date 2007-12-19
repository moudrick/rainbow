using System;
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
	/// Default Rule
	/// This Class is responsible for Splitting up rainbow urls into real urls for the Rainbow Portal
	/// Created by John Mandia www.whitelightsolutions.com, contributors to this and 
	/// previous versions are Jes, Manu and Cory. 
	/// </summary>
	public class DefaultRule : IRules
	{
		public DefaultRule()
		{
		}

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

			string _handlerFlag = "portal";
			string _defaultSplitter = "__";
			bool _pageidNoSplitter = false;
		
			// collect settings
			if(Settings["handlerflag"]!= null)
			{
				_handlerFlag = ((string) Settings["handlerflag"]).ToLower(CultureInfo.InvariantCulture);
			}
			
			if(Settings["handlersplitter"]!= null)
			{
				_defaultSplitter = Settings["handlersplitter"].ToString();
			}
			else
			{
				if(ConfigurationSettings.AppSettings["HandlerDefaultSplitter"]!= null)
					_defaultSplitter = ConfigurationSettings.AppSettings["HandlerDefaultSplitter"].ToString();
			}

			if(Settings["pageidnosplitter"]!= null)
			{
				_pageidNoSplitter = bool.Parse((string) Settings["pageidnosplitter"]);
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
				General.Splitter(parts, sb,_defaultSplitter,_pageidNoSplitter);
				return sb.ToString();

			}
			else
				//return null if no changes
				return string.Empty;
		}

	}

}