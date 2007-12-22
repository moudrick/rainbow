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
    public class PortalSettingsProviderTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest();
                // Set up initial database environment for testing purposes
                TestHelper.TearDownDB();

                DatabaseUpdater updater = new DatabaseUpdater(ConfigurationManager.AppSettings["RainbowWebApplicationRoot"]);
                updater.PreviewUpdate();
                updater.PerformUpdate();
                Assert.AreEqual(0, updater.Errors.Count);
                //TestHelper.RecreateDBSchema();
            }   
        }

        [Test]
        public void PortalSettingsTest()
        {
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"));
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
