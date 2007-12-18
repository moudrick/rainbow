using System.IO;
using System.Web;
using NUnit.Framework;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;

namespace Rainbow.Tests
{
    public static class FakeHttpContext
    {
        public static HttpContext GetNew()
        {
            return new HttpContext(GetFakeHttpRequest(), GetFakeHttpResponse());
        }

        private static HttpResponse GetFakeHttpResponse()
        {
            TextWriter writer = TextWriter.Null;
            return new HttpResponse(writer);
        }

        static HttpRequest GetFakeHttpRequest()
        {
            //http://kntajus.blogspot.com/2007/04/manually-creating-httprequest-for.html#links
            //HttpRequest httpRequest = new HttpRequest("default.aspx", "http://www.diuturnal.com/default.aspx", string.Empty);
            HttpRequest request = new HttpRequest("index.html", "http://localhost/fake/index.html", string.Empty);
            return request;
        }
    }

    [TestFixture]
    public class PortalSettingsProviderTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            // Set up initial database environment for testing purposes
            TestHelper.TearDownDB();
            TestHelper.RecreateDBSchema();
        }

        [Test]
        [Ignore("NotWorking")]
        public void PortalSettingsTest()
        {
            HttpContext.Current = FakeHttpContext.GetNew();
            PortalSettings portalSettings = PortalSettingsProvider.InstantiateNewPortalSettings(0, "Rainbow");
            Assert.IsNotNull(portalSettings);
        }
    }
}
