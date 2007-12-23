using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using NUnit.Framework;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Core.DAL;
using Rainbow.Framework.Settings;
using Subtext.TestLibrary;

namespace Rainbow.Tests
{
    [TestFixture]
    public class Fixture
    {
        [Test]
        public void Test()
        {
            string mapPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
            using (HttpSimulator simulator = new HttpSimulator("/Rainbow", mapPath))
            {
                simulator.SimulateRequest();

                string desktopSource = @"DesktopModules/CoreModules/AddModule/Viewer.ascx";
                string controlFullPath = Path.ApplicationRoot + "/" + desktopSource;
                System.Web.UI.Page page = new System.Web.UI.Page();
                Assert.IsNotNull(page);
                System.Web.UI.Control myControl = page.LoadControl(controlFullPath);
                Assert.IsTrue(myControl is Rainbow.Framework.Web.UI.WebControls.PortalModuleControl);
            }
        }
    }

    [TestFixture]
    public class PortalSettingsProviderTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            TestHelper.TearDownDB();
            //TestHelper.RecreateDBSchema();
            string mapPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
            using (HttpSimulator simulator = new HttpSimulator("/Rainbow", mapPath))
            {
                simulator.SimulateRequest();

                DatabaseUpdater updater = new DatabaseUpdater(mapPath + @"Setup\Scripts\", mapPath);
                updater.PreviewUpdate();
                updater.PerformUpdate();
                //Assert.AreEqual(0, updater.Errors.Count);
            }
        }

        [Test]
        public void PortalSettingsTest()
        {
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost/Rainbow/"));
                Assert.AreEqual(1882, Database.DatabaseVersion);
                PortalSettings portalSettings = PortalSettingsProvider.InstantiateNewPortalSettings(0, "Rainbow");
                Assert.IsNotNull(portalSettings);
            }
        }

        [Test]
        public void CanSimulateFormPost()
        {
            using (HttpSimulator simulator = new HttpSimulator())
            {
                NameValueCollection form = new NameValueCollection();
                form.Add("Test1", "Value1");
                form.Add("Test2", "Value2");
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"), form);

                Assert.AreEqual("Value1", HttpContext.Current.Request.Form["Test1"]);
                Assert.AreEqual("Value2", HttpContext.Current.Request.Form["Test2"]);
            }

            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SetFormVariable("Test1", "Value1")
                  .SetFormVariable("Test2", "Value2")
                  .SimulateRequest(new Uri("http://localhost/Test.aspx"));

                Assert.AreEqual("Value1", HttpContext.Current.Request.Form["Test1"]);
                Assert.AreEqual("Value2", HttpContext.Current.Request.Form["Test2"]);
            }
        }
    }
}
