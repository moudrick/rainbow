using System;
using System.Web;
using NUnit.Framework;
using Rainbow.Framework.Core;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Site.Configuration;
using Subtext.TestLibrary;

namespace Rainbow.Tests.Data.MsSql.PortalTests
{
    [TestFixture]
    public class DefaultPortalTests
    {
        HttpSimulator httpSimulator;
        Portal portalSettings;

        [SetUp]
        public void SetUp()
        {
            httpSimulator = new HttpSimulator("/Rainbow", Hepler.RainbowWebApplicationRoot);

            httpSimulator.SimulateRequest(new Uri("http://localhost/Rainbow/"));
            Assert.AreEqual(1882, VersionController.Instance.DatabaseVersion);
            portalSettings = PortalProvider.Instance.InstantiateNewPortalSettings(0, "Rainbow");
            Assert.IsNotNull(portalSettings);

            HttpContext.Current.Items["PortalSettings"] = portalSettings;
        }

        [TearDown]
        public void TearDown()
        {
            httpSimulator.Dispose();
        }

        [Test]
        public void PortalSettings()
        {

            Assert.AreEqual(0, portalSettings.PortalID);
            Assert.AreEqual("Rainbow Portal", portalSettings.PortalName);
            Assert.AreEqual("Rainbow", portalSettings.PortalAlias);
            Assert.AreEqual("Default", portalSettings.CurrentLayout);
            Assert.AreEqual(true, portalSettings.ShowPages);
            Assert.AreEqual(0, portalSettings.ActiveModule);
        }

        [Test]
        public void ActivePage()
        {
            PageSettings activePage = portalSettings.ActivePage;
            Assert.IsNotNull(activePage);
            Assert.AreEqual(1, activePage.PageID);
            Assert.AreEqual(1, activePage.PageOrder);
            Assert.AreEqual(0, activePage.ParentPageID);
            Assert.AreEqual("Home", activePage.PageName);
            Assert.AreEqual("All Users;", activePage.AuthorizedRoles);
            Assert.AreEqual("Portals/_Rainbow/", activePage.PortalPath);
            Assert.AreEqual("Home", activePage.MobilePageName);
            Assert.AreEqual(true, activePage.ShowMobile);

            Assert.IsNotNull(activePage.CustomSettings);

            Assert.IsNotNull(activePage.Modules);
        }

        [Test]
        public void Other()
        {
            Assert.IsNotNull(portalSettings.CustomSettings);
            Assert.IsNotNull(portalSettings.DesktopPages);
        }
    }
}