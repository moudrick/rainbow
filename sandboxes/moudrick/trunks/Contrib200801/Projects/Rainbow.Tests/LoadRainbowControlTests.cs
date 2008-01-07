using System.Configuration;
using NUnit.Framework;
using Subtext.TestLibrary;

namespace Rainbow.Tests
{
    [TestFixture]
    public class LoadRainbowControlTests
    {
        [Test]
        [Ignore("NotWorking")]
        public void Simple()
        {
            string mapPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
            using (HttpSimulator simulator = new HttpSimulator("/Rainbow", mapPath))
            {
                simulator.SimulateRequest();

                string desktopSource = @"DesktopModules/CoreModules/AddModule/Viewer.ascx";
                string controlFullPath = Rainbow.Framework.Path.ApplicationRoot + "/" + desktopSource;
                System.Web.UI.Page page = new System.Web.UI.Page();
                Assert.IsNotNull(page);
                System.Web.UI.Control myControl = page.LoadControl(controlFullPath);
                Assert.IsTrue(myControl is Rainbow.Framework.Web.UI.WebControls.PortalModuleControl);
            }
        }
    }
}
