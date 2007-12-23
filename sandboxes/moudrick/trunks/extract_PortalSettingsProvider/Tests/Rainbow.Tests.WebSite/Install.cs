using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WatiN.Core;

namespace Rainbow.Tests.WebSite
{
    [TestFixture]
    public class Install
    {
        [Test]
        public void Simple()
        {
            IE.Settings.WaitForCompleteTimeOut = 300;
            IE ie = new IE("http://localhost/Rainbow/Setup/Update.aspx");
            ie.GoTo("http://localhost/Rainbow/");
            ie.WaitForComplete();
            //Assert http://localhost/Rainbow/Setup/Update.aspx
            ie.Button(Find.ByName("UpdateDatabaseCommand")).Click();
            ie.WaitForComplete();
            ie.Button(Find.ByName("FinishButton")).Click();
            ie.WaitForComplete();

            ie.TextField(Find.ByName("ctl04$DesktopThreePanes1$ThreePanes$ctl03$email")).TypeText("admin@rainbowportal.net");
            ie.TextField(Find.ByName("ctl04$DesktopThreePanes1$ThreePanes$ctl03$password")).TypeText("admin");
            ie.Button(Find.ByName("ctl04$DesktopThreePanes1$ThreePanes$ctl03$LoginBtn")).Click();
            Assert.AreEqual("Administration ", 
                ie.Link(Find.ByUrl("http://localhost/Rainbow/site/100/Default.aspx")).Text, 
                @"innerText does not match");
        }
    }
}
