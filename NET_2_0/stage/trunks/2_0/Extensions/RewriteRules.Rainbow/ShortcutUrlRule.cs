using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using RewriteRules.Rainbow.Tools;
using RulesEngine;
using Settings = Rainbow.Settings;

namespace RewriteRules.Rainbow
{
	/// <summary>
	/// ShortcutUrlRuleDefault Rule
	/// This Class is responsible for Splitting up rainbow urls into real urls for the Rainbow Portal
	/// </summary>
	public class ShortcutUrlRule : IRules
	{
//		private string GetExpandedName(string key)
//		{
//			switch ( key.ToLower() )
//			{
//				case "t" :
//					return "tabID";
//				case "i" :
//					return "itemID";
//				case "p" :
//					return "pageID";
//				case "m" :
//					return "modID";
//				default :
//					return key;
//			}
//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appl"></param>
		/// <param name="_path"></param>
		/// <param name="settingsSection"></param>
		/// <returns></returns>
		public string Execute(HttpApplication appl, string _path, string settingsSection)
		{
			Hashtable shortcuts = null;
			string returnVal = string.Empty;
			//return string.Empty;

			try
			{
				string qPart = string.Empty;
				int qPartPos = _path.LastIndexOf("/") + 1 ;

				// have we been sent here by an IIS 404 redirect?
				if ( _path.IndexOf("?404;http://") > -1 )
				{
					// example: /rainbow-dev/app_support/SmartError.aspx?404;http://localhost/rainbow-dev/sex

					qPart = qPartPos < _path.Length ? _path.Substring(qPartPos) : string.Empty;
					if ( qPart.Length > 0 )
					{
						//split the path into components
						string[] parts = _path.Split('/');

						StringBuilder sb = new StringBuilder(Engine.ApplicationPath(appl).ToString());
						sb.Append("/");
						sb.Append(General.DefaultPage);
						General.Splitter(parts, sb,_defaultSplitter,_pageidNoSplitter);
						return sb.ToString();

//						if ( HttpContext.Current.Cache["rainbow_shortcuts_" + Settings.Portal.UniqueID] == null )
//						{
//							shortcuts = GetShortcuts(Settings.Portal.UniqueID);
//						}
//						else
//						{
//							shortcuts = (Hashtable) HttpContext.Current.Cache["rainbow_shortcuts_" + Settings.Portal.UniqueID];
//						}
//						
//						if ( shortcuts.ContainsKey(qPart) )
//						{
//							returnVal = shortcuts[qPart].ToString();
//						}
//						return returnVal;
					}
					else
						return string.Empty;
					
				}
				else
				{
					return string.Empty;
				}
			}
			catch
			{
				return string.Empty;
			}
		
		}

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="portalID"></param>
//		/// <returns></returns>
//		private Hashtable GetShortcuts(string portalID)
//		{
//			string myPath = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "Portals/_" + portalID, "shortcuts/shortcuts.xml");
//			Hashtable _result = new Hashtable();
//			_result.Add("sex","/Home.aspx");
//			return _result;
//		}
	}
}