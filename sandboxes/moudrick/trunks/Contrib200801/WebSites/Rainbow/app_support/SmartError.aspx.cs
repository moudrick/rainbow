using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Rainbow.Framework;
using Rainbow.Framework.Core;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Settings.Cache;
using Label = System.Web.UI.WebControls.Label;
using Page = Rainbow.Framework.Web.UI.Page;
using Path = Rainbow.Framework.Path;

namespace Rainbow.Error
{
    /// <summary>
    /// Smart Error page - Jes1111
    /// </summary>
	public class SmartError : Page
	{
        /// <summary>
        /// Determines whether the specified STR is integer.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// 	<c>true</c> if the specified STR is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(string str)
        {
            int aux;
            return int.TryParse(str, out aux);
        }

		protected PlaceHolder PageContent;

		protected Label Label1;
		protected Label Label2;
		protected Label Label3;
		//protected Esperantus.WebControls.HyperLink ReturnHome;

		protected Label myTest;
		protected Label myTest2;

		protected const int _LOGLEVEL_ = 0;
		protected const int _GUID_ = 1;
		protected const int _RENDEREDEVENT_ = 2;

		/// <summary>
		/// Handles the Error event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void Page_Error(object sender,EventArgs e)
		{
			Response.Redirect("~/app_support/GeneralError.html", true);
		}

		/// <summary>
		/// Handles OnLoad event at Page level<br/>
		/// Performs OnLoad actions that are common to all Pages.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// load the dedicated CSS
		    if (!IsCssFileRegistered("SmartError"))
		    {
		        RegisterCssFile("Mod_SmartError");
            }

			ArrayList storedError = null;
			StringBuilder sb = new StringBuilder(); // to build response text
			int httpStatusCode = (int)HttpStatusCode.InternalServerError; // default value
			string renderedEvent;
			string validStatus = "301;307;403;404;410;500;501;502;503;504";

			if ( Request.QueryString[0] != null )
			{
				// is this a "MagicUrl" request
				if ( Request.QueryString[0].StartsWith("404;http://") )
				{
					Hashtable magicUrlList;
					string redirectUrl = string.Empty;
				    int qPartPos = Request.QueryString[0].LastIndexOf("/") + 1 ;
					string qPart = qPartPos < Request.QueryString[0].Length ? Request.QueryString[0].Substring(qPartPos) : string.Empty;
				    if (qPart.Length > 0)
				    {
				        if (IsInteger(qPart))
				        {
				            redirectUrl = HttpUrlBuilder.BuildUrl(Int32.Parse(qPart));
				        }
				        else
				        {
				            magicUrlList = GetMagicUrlList(RainbowContext.Current.UniqueID);
				            if (magicUrlList != null && magicUrlList.ContainsKey(HttpUtility.HtmlEncode(qPart)))
				            {
				                redirectUrl = HttpUtility.HtmlDecode(magicUrlList[HttpUtility.HtmlEncode(qPart)].ToString());
				                if (IsInteger(redirectUrl))
				                {
				                    redirectUrl = HttpUrlBuilder.BuildUrl(Int32.Parse(redirectUrl));
				                }
				            }
				        }
				        if (redirectUrl.Length != 0)
				        {
				            Response.Redirect(redirectUrl, true);
				        }
				        else
				        {
				            httpStatusCode = (int) HttpStatusCode.NotFound;
				        }
				    }

				}
				// get status code from querystring
				else if ( IsInteger(Request.QueryString[0]) && validStatus.IndexOf(Request.QueryString[0]) > -1 )
				{
					httpStatusCode = int.Parse(Request.QueryString[0]);
				}
			}

			// get stored error
			if (Request.QueryString["eid"] != null && Request.QueryString["eid"].Length > 0)
			{
				storedError = (ArrayList)CurrentCache.Get(Request.QueryString["eid"]);
			}
			if ( storedError != null && storedError[_RENDEREDEVENT_] != null )
				renderedEvent = storedError[_RENDEREDEVENT_].ToString();
			else
				renderedEvent = @"<p>No exception event stored or cache has expired.</p>";


			// get home link
			string homeUrl = HttpUrlBuilder.BuildUrl();

			// try localizing message
			try
			{
				switch ( httpStatusCode )
				{
					case (int)HttpStatusCode.NotFound : // 404
					case (int)HttpStatusCode.Gone : // 410
					case (int)HttpStatusCode.MovedPermanently : // 301
					case (int)HttpStatusCode.TemporaryRedirect : // 307
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_404HEADING","Page Not Found", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_404TEXT","We're sorry, but there is no page that matches your entry. It is possible you typed the address incorrectly, or the page may no longer exist. You may wish to try another entry or choose from the links below, which we hope will help you find what you’re looking for.", null));
						break;
					case (int)HttpStatusCode.Forbidden : // 403
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_403HEADING","Not Authorised", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_403TEXT","You do not have the required authority for the requested page or action.", null));
						break;
					default :
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_500HEADING","Our Apologies", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_500TEXT","We're sorry, but we were unable to service your request. It's possible that the problem is a temporary condition.", null));
						break;
				}
				sb.AppendFormat("<p><a href=\"{0}\">{1}</a></p>", homeUrl,General.GetString("HOME","Home Page",null));
			}
			catch // default to english message
			{
				switch ( httpStatusCode )
				{
					case (int)HttpStatusCode.NotFound :
						sb.Append("<h3>Page Not Found</h3>");
						sb.Append("<p>We're sorry, but there is no page that matches your entry. It is possible you typed the address incorrectly, or the page may no longer exist. You may wish to try another entry or choose from the links below, which we hope will help you find what you’re looking for.</p>");
						break;
					case (int)HttpStatusCode.Forbidden :
						sb.Append("<h3>Not Authorised</h3>");
						sb.Append("<p>You do not have the required authority for the requested page or action.</p>");
						break;
					default :
						sb.Append("<h3>Our Apologies</h3>");
						sb.AppendFormat("<p>We're sorry, but we were unable to service your request. It's possible that the problem is a temporary condition.</p>");
						break;
				}
				sb.AppendFormat("<p><a href=\"{0}\">{1}</a></p>",homeUrl, "Home Page");
			}

			// find out if user is on allowed IP Address
			if ( Request.UserHostAddress != null
				&& Request.UserHostAddress.Length > 0 )
			{
				// construct IPList
				string[] lockKeyHolders = Config.LockKeyHolders.Split(new char[]{';'}); //ConfigurationSettings.AppSettings["LockKeyHolders"].Split(new char[]{';'});
				IPList ipList = new IPList();
				try
				{
					foreach ( string lockKeyHolder in lockKeyHolders )
					{
						if ( lockKeyHolder.IndexOf("-") > -1 )
							ipList.AddRange(lockKeyHolder.Substring(0, lockKeyHolder.IndexOf("-")), lockKeyHolder.Substring(lockKeyHolder.IndexOf("-") + 1));
						else
							ipList.Add(lockKeyHolder);
					}

					// check if requestor's IP address is in allowed list
					if ( ipList.CheckNumber(Request.UserHostAddress) )
					{
						// we can show error details
						sb.AppendFormat("<h3>{0} - {1}</h3>",General.GetString("SMARTERROR_SUPPORTDETAILS_HEADING","Support Details", null), httpStatusCode);
						sb.Append(renderedEvent);
					}
				}
				catch
				{
                    ;// if there was a problem, let's assume that user is not authorised
				}
			}
			PageContent.Controls.Add(new LiteralControl(sb.ToString()));
			Response.StatusCode = httpStatusCode;
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

        Hashtable GetMagicUrlList(string portalID)
        {
            Hashtable result = new Hashtable();
            if (Cache["rainbow_MagicUrlList_" + portalID] == null)
            {
                string myPath =
                    Server.MapPath(Path.WebPathCombine(portalSettings.PortalFullPath, "MagicUrl/MagicUrlList.xml"));
                if (File.Exists(myPath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(myPath);
                    XmlNodeList xnl = xmlDoc.SelectNodes("/MagicUrlList/MagicUrl");
                    foreach (XmlNode node in xnl)
                    {
                        try
                        {
                            result.Add(node.Attributes["key"].Value,
                                       HttpUtility.HtmlDecode(node.Attributes["value"].Value));
                        }
                        catch {;}
                    }
                    Cache.Insert("rainbow_MagicUrlList_" + RainbowContext.Current.UniqueID,
                                 result,
                                 new CacheDependency(myPath));
                }
            }
            else
            {
                result = (Hashtable) Cache["rainbow_MagicUrlList_" + RainbowContext.Current.UniqueID];
            }
            return result;
        }
       
		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			//ReturnHome.NavigateUrl = HttpUrlBuilder.BuildUrl();
		
			base.OnInit(e);
		}

        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
            this.Error += new EventHandler(this.Page_Error);
		}
		#endregion
    }
}
