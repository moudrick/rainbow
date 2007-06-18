using System.IO;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Users.Data;

namespace Rainbow.Tests.MembershipProviderTests
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
            return new HttpRequest("index.html", "http://localhost/fake/index.html", string.Empty);
        }
    }

    [TestFixture]
    public class MockupTests 
    {
        [Test]
        [Category("NotWorking")]
        public void FakeContext()
        {
            HttpContext.Current = FakeHttpContext.GetNew();
        }
    }

    [TestFixture]
    public class PortalSettingsTests
    {
        [Test]
        [Category("NotWorking")]
        public void DefaultPortal()
        {
            HttpContext.Current = FakeHttpContext.GetNew();
            //check actual DbVersion
            PortalSettings portalSettings = new PortalSettings(0, "rainbow");
            Assert.IsNotNull(portalSettings);
        }
    }

	[TestFixture]
	public class UserDbTests : BaseMembershipProvideTestFixture
	{
		const string defaultEmailLogin = "admin@rainbowportal.net";

		[Test]
		[Category("NotWorking")]
		public void LoginDefaultCorrect()
		{
            HttpContext.Current = FakeHttpContext.GetNew();
			HttpContext.Current.Items.Add("PortalSettings", new PortalSettings(0, "rainbow"));
			UsersDB db = new UsersDB();
			MembershipUser user = db.Login(defaultEmailLogin, "admin");
			Assert.IsNotNull(user);
		}

		[Test]
		[Category("NotWorking")]
		public void LoginDefaultIncorrect()
		{
			UsersDB db = new UsersDB();
			MembershipUser user = db.Login(defaultEmailLogin, "admin1");
			Assert.IsNull(user);
		}
	}
}
