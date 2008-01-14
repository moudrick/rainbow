using System;
using System.Web;
using NUnit.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Subtext.TestLibrary;

namespace Rainbow.Tests.Data.MsSql.PortalTests
{
    [TestFixture]
    public class PortalCrudTests : BaseProviderTestFixture
    {
        [Test]
        [Category("NotWorking")]
        public void Create()
        {
            HttpSimulator httpSimulator = new HttpSimulator("/Rainbow", 
                Hepler.RainbowWebApplicationRoot);
            using (httpSimulator)
            {
                httpSimulator.SimulateRequest(new Uri("http://localhost/Rainbow/"));

                HttpContext.Current.Items["PortalSettings"] = PortalProvider.Instance
                    .InstantiateNewPortal(0, "Rainbow");

                //System.Web.Profile.SqlProfileProvider
                int newProtalId = PortalProvider.Instance.CreatePortal(0, "newPortalAlias", "newPortalName", "newPortalPath");
                Portal newPortal = PortalProvider.Instance.InstantiateNewPortal(newProtalId);
                Assert.IsNotNull(newPortal);
            }
        }
    }
}
