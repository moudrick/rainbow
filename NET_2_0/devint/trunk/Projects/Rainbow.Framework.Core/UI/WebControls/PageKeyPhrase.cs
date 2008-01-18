using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Created By john.mandia@whitelightsolutions.com - This control is used by the PageKeyPhrase Module.
    /// </summary>
    public class PageKeyPhrase : Label
    {
        readonly Portal portalSettings = PortalProvider.Instance.CurrentPortal;
        string tabKeyPhrase;
        string currentLanguage;

        /// <summary>
        /// Stores current Tab Key Phrase
        /// </summary>
        /// <value>The tab key phrase.</value>
        public string TabKeyPhrase
        {
            get
            {
                currentLanguage = "TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name;
                if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture
                    && portalSettings.ActivePage.CustomSettings[currentLanguage] != null)
                {
                    tabKeyPhrase =
                        (string) ViewState["TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name];
                    if (tabKeyPhrase != null)
                        return tabKeyPhrase;
                    else
                    {
                        // Try to get this tab's key phrase
                        tabKeyPhrase = portalSettings.ActivePage.CustomSettings[currentLanguage].ToString();

                        if (tabKeyPhrase == string.Empty && portalSettings.CustomSettings != null)
                        {
                            tabKeyPhrase = portalSettings.ActivePage.CustomSettings["TabKeyPhrase"].ToString();

                            if (tabKeyPhrase == string.Empty)
                                tabKeyPhrase = portalSettings.CustomSettings["SITESETTINGS_PAGE_KEY_PHRASE"].ToString();
                        }

                        return tabKeyPhrase;
                    }
                }
                else
                {
                    tabKeyPhrase = (string) ViewState["TabKeyPhrase"];
                    if (tabKeyPhrase != null)
                        return tabKeyPhrase;
                    else
                    {
                        if (portalSettings.ActivePage.CustomSettings["TabKeyPhrase"] != null)
                        {
                            // Try to get this tab's key phrase
                            tabKeyPhrase = portalSettings.ActivePage.CustomSettings["TabKeyPhrase"].ToString();

                            if (tabKeyPhrase == string.Empty && portalSettings.CustomSettings != null)
                            {
                                tabKeyPhrase = portalSettings.CustomSettings["SITESETTINGS_PAGE_KEY_PHRASE"].ToString();
                            }

                            return tabKeyPhrase;
                        }
                        return string.Empty;
                    }
                }
            }
            set
            {
                if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture)
                {
                    ViewState["TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name] = value;
                }
                else
                {
                    ViewState["TabKeyPhrase"] = value;
                }
            }
        }

        /// <summary>
        /// Load event handler
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Text = TabKeyPhrase;

            base.DataBind();
        }
    }
}