using System;
using System.Web;
using NUnit.Framework;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Subtext.TestLibrary;

namespace Rainbow.Tests.Data.MsSql.PortalSettingsTests
{
    [TestFixture]
    public class PortalCrudTests
    {
        [Test]
        public void Create()
        {
            HttpSimulator httpSimulator;

            httpSimulator = new HttpSimulator("/Rainbow", Hepler.RainbowWebApplicationRoot);
            using (httpSimulator)
            {
                httpSimulator.SimulateRequest(new Uri("http://localhost/Rainbow/"));

                HttpContext.Current.Items["PortalSettings"] = PortalProvider.Instance
                    .InstantiateNewPortalSettings(0, "Rainbow");

                int newProtalId = PortalProvider.Instance.CreatePortal(0, "newPortalAlias", "newPortalName", "newPortalPath");
                PortalSettings newPortal = PortalProvider.Instance.InstantiateNewPortalSettings(newProtalId);
                Assert.IsNotNull(newPortal);
            }
        }
    }
}
