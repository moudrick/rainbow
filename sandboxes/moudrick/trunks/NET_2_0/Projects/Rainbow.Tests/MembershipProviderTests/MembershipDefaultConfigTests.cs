using System.Web.Security;
using NUnit.Framework;
using Rainbow.Framework.Providers.RainbowMembershipProvider;

namespace Rainbow.Tests.MembershipProviderTests
{
	[TestFixture]
	public class MembershipDefaultConfigTests
	{
		[Test]
		public void ApplicationNameTest()
		{
			Assert.AreEqual("Rainbow", Membership.ApplicationName);
		}

		[Test]
		public void EnablePasswordResetTest()
		{
			Assert.AreEqual(true, Membership.EnablePasswordReset);
		}

		[Test]
		public void EnablePasswordRetrievalTest()
		{
			Assert.AreEqual(false, Membership.EnablePasswordRetrieval);
		}

		[Test]
		public void HashAlgorithmTypeTest()
		{
			Assert.AreEqual("SHA1", Membership.HashAlgorithmType);
		}

		[Test]
		public void MaxInvalidPasswordAttemptsTest()
		{
			Assert.AreEqual(5, Membership.MaxInvalidPasswordAttempts);
		}

		[Test]
		public void MinRequiredNonAlphanumericCharactersTest()
		{
			Assert.AreEqual(1, Membership.MinRequiredNonAlphanumericCharacters);
		}

		[Test]
		public void MinRequiredPasswordLengthTest()
		{
			Assert.AreEqual(5, Membership.MinRequiredPasswordLength);
		}

		[Test]
		public void PasswordAttemptWindowTest()
		{
			Assert.AreEqual(15, Membership.PasswordAttemptWindow);
		}

		[Test]
		public void PasswordStrengthRegularExpressionTest()
		{
			Assert.AreEqual(string.Empty, Membership.PasswordStrengthRegularExpression);
		}

		[Test]
		public void ProviderTest()
		{
			Assert.AreEqual(typeof (RainbowSqlMembershipProvider), Membership.Provider.GetType());
		}

		[Test]
		public void ProvidersCountTest()
		{
			Assert.AreEqual(1, Membership.Providers.Count);
		}

		[Test]
		public void RequiresQuestionAndAnswerTest()
		{
			Assert.AreEqual(false, Membership.RequiresQuestionAndAnswer);
		}
	}
}
