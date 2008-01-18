using NUnit.Framework;
using Rainbow.Framework.Providers;
using Subtext.TestLibrary;

namespace Rainbow.Tests.Data.MsSql
{
    //TODO: move it to Core Tests
    [TestFixture]
    public class RainbowContextTests
    {
        [Test]
        public void Null()
        {
            using (HttpSimulator httpSimulator = new HttpSimulator())
            {
                httpSimulator.SimulateRequest();
                Assert.IsNull(PortalProvider.Instance.CurrentPortal);
            }
        }
    }
}
