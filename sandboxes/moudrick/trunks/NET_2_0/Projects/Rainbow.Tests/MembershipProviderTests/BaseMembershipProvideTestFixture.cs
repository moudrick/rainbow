using NUnit.Framework;

namespace Rainbow.Tests.MembershipProviderTests 
{
    public abstract class BaseMembershipProvideTestFixture 
	{
        [TestFixtureSetUp]
        public virtual void FixtureSetUp() {
            // Set up initial database environment for testing purposes
            TestHelper.TearDownDB();
            TestHelper.RecreateDBSchema();
        }
    }
}


