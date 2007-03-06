using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Rainbow.Settings;

namespace Rainbow.Settings.Test
{
	/// <summary>
	/// Summary description for Test_UniqueID.
	/// </summary>
	[TestFixture]
	public class UniqueID
	{
		public UniqueID(){}
		[Test]
		public void FindAliasFromUri()
		{
			XmlDocument testList = new XmlDocument();
			testList.Load(@"..\testList_GetAliasFromUri.xml");
			XPathNavigator xpn = testList.CreateNavigator();
			XPathNodeIterator xpi = xpn.Select("//test");

			Uri uri;
			bool expected;
			string refActual = null;
			string actual = null;
			string sld = "aero;biz;com;coop;info;museum;name;net;org;pro;gov;edu;mil;int;co;ac;sch;nhs;police;mod;ltd;plc;me";
			bool removeTLD = true;
			bool removeWWW = true;
			string key;

			while ( xpi.MoveNext())
			{
				key = xpi.Current.GetAttribute("key","").ToString();
				uri = new Uri(xpi.Current.GetAttribute("url","").ToString());
				expected = bool.Parse(xpi.Current.GetAttribute("expected","").ToString());
				actual = xpi.Current.GetAttribute("actual","").ToString();
				removeTLD = bool.Parse(xpi.Current.GetAttribute("removeTLD","").ToString());
				removeWWW = bool.Parse(xpi.Current.GetAttribute("removeWWW","").ToString());

				
				Assert.AreEqual(expected,Portal.FindAliasFromUri(uri,ref refActual,"rainbow",removeWWW,removeTLD,sld));
				Assert.AreEqual(actual,refActual,key);
			}
		
		}
	}
}
