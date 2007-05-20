using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Users.Data;

namespace Rainbow.Tests.MembershipProviderTests
{
	[TestFixture]
	public class UserDbTests : BaseMembershipProvideTestFixture
	{
		const string defaultEmailLogin = "admin@rainbowportal.net";

		[Test]
		[Category("NotWorking")]
		public void LoginDefaultCorrect()
		{
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
