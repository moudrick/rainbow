using System;
using System.Configuration;
using NUnit.Framework;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Settings;
using Subtext.TestLibrary;

namespace Rainbow.Tests.Data.MsSql
{
    [TestFixture]
    public class PortalSettingsProviderTests
    {
        [Test]
        public void PortalSettingsSimple()
        {
            string mapPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
            using (HttpSimulator simulator = new HttpSimulator("/Rainbow", mapPath))
            {
                simulator.SimulateRequest(new Uri("http://localhost/Rainbow/"));
                Assert.AreEqual(1882, Database.DatabaseVersion);
                PortalSettings portalSettings = PortalSettingsProvider.InstantiateNewPortalSettings(0, "Rainbow");
                Assert.IsNotNull(portalSettings);
            }
        }
    }
}