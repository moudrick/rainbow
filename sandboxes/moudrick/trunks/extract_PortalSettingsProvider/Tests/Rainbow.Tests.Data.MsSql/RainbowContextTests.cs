using NUnit.Framework;
using Rainbow.Framework.Core.Configuration.Settings.Providers;

namespace Rainbow.Tests.Data.MsSql
{

    //TODO: move it to Core Tests
    [TestFixture]
    public class RainbowContextTests
    {
        [Test]
        public void Null()
        {
            Assert.IsNull(PortalProvider.Instance.CurrentPortal);
        }
    }
}
