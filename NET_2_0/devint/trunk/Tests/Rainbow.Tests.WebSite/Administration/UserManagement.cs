using System.Reflection;
using NUnit.Framework;
using WatiN.Core;

namespace Rainbow.Tests.WebSite.Administration
{
    [TestFixture]
    public class UserManagement
    {
        [Test]
        public void AddRole()
        {
            IE ie = new IE();
            ie.ClearCookies("http://localhost/Rainbow/");
            ie.GoTo("http://localhost/Rainbow/site/100/Default.aspx");

            ie.TextField(Find.ByName("ctl02$email")).TypeText("admin@rainbowportal.net");
            ie.TextField(Find.ByName("ctl02$password")).TypeText("admin");
            ie.Button(Find.ByName("ctl02$LoginBtn")).Click();

            //ie.Link(Find.ByUrl("http://localhost/Rainbow/site/100/Default.aspx")).Click();
            ie.Link(Find.ByText("Administration ")).Click();
            ie.Div(Find.ByValue("User Roles")).Click();
            ie.TextField(Find.ByName("ctl04$DesktopThreePanes1$ThreePanes$ctl03$txtNewRole")).TypeText("NewRole");
            ie.Link(Find.ByText("Add New Role")).Click();

            Button buttonEdit = ie.Button(Find.ByName("ctl04$DesktopThreePanes1$ThreePanes$ctl03$rolesList$ctl01$ImageButton2"));
            Assert.AreEqual("http://localhost/Rainbow/Design/Themes/Default/icon/Edit.gif", GetAttributeValue(buttonEdit, "src"), @"src does not match");
            Assert.AreEqual("Edit this item", GetAttributeValue(buttonEdit, "alt"), @"alt does not match");

            Button buttonDelete = ie.Button(Find.ById("ctl04_DesktopThreePanes1_ThreePanes_ctl03_rolesList_ctl01_ImageButton1"));
            Assert.AreEqual("http://localhost/Rainbow/Design/Themes/Default/icon/Delete.gif", GetAttributeValue(buttonDelete, "src"), @"src does not match");
            Assert.AreEqual("Delete this item", GetAttributeValue(buttonDelete, "alt"), @"alt does not match");
            Link link = ie.Link(Find.ById("ctl04_DesktopThreePanes1_ThreePanes_ctl03_rolesList_ctl01_Name"));
            Assert.AreEqual("NewRole", GetAttributeValue(link, "innerText"), @"innerText does not match");


            Link link1 = ie.Link(Find.ByText("NewRole"));
            Button buttonDelete1 = ie.Button(Find.ById(GetSiblingId(link1.Id, "ImageButton1")));
            Assert.AreEqual("Delete this item", GetAttributeValue(buttonDelete1, "alt"), @"alt does not match");
        }

        static string GetSiblingId(string id, string siblingId)
        {
            return id.Substring(0, id.LastIndexOf("_")) + siblingId;
        }

        static object GetAttributeValue(Element element, string name)
        {
            object htmlElement = element.HTMLElement;
            PropertyInfo member = htmlElement.GetType().GetProperty(name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return member.GetValue(htmlElement, null);
        }
    }
}
