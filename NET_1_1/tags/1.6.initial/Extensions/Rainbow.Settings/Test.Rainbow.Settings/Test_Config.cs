using System;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;

namespace Rainbow.Settings.Test
{
	/// <summary>
	/// Summary description for Test_UniqueID.
	/// </summary>
	[TestFixture]
	public class Config
	{
		public Config()
		{
		}

		[Test]
		public void GetInteger()
		{
			Settings.Config.SetReader(new Reader(new IntegerTestConfigReader()));
			
			XmlDocument testList = new XmlDocument();
			testList.Load(@"..\testList_GetInteger.xml");
			XPathNavigator xpn = testList.CreateNavigator();
			XPathNodeIterator xpi = xpn.Select("//test");

			string key;
			string settingValue;
			int defaultValue;
			int expected;
			int result;
			bool allowNegative;

			while ( xpi.MoveNext())
			{
				key = xpi.Current.GetAttribute("key","").ToString();
				settingValue = xpi.Current.GetAttribute("settingValue","").ToString();
				settingValue = settingValue == "null" ? null : settingValue;
				defaultValue = Int32.Parse(xpi.Current.GetAttribute("defaultValue","").ToString());
				allowNegative = bool.Parse(xpi.Current.GetAttribute("allowNegative","").ToString());
				expected = Int32.Parse(xpi.Current.GetAttribute("expected","").ToString());

				result = Settings.Config.GetInteger(key, defaultValue, allowNegative);

				Assert.AreEqual(expected,result,key);
			}

		}

		[Test]
		public void GetString()
		{
			Settings.Config.SetReader(new Reader(new StringTestConfigReader()));

			string srcFile = @"..\testList_GetString.xml";
 
			XmlDocument testList = new XmlDocument();
			testList.Load(srcFile);
			XPathNavigator xpn = testList.CreateNavigator();
			XPathNodeIterator xpi = xpn.Select("//test");

			string settingValue;
			string defaultValue;
			string expected;
			string result;
			bool allowEmpty;
			string key;

			while ( xpi.MoveNext())
			{
				key = xpi.Current.GetAttribute("key","").ToString();

				settingValue = xpi.Current.GetAttribute("settingValue","").ToString();
				settingValue = settingValue == "null" ? null : settingValue;

				defaultValue = xpi.Current.GetAttribute("defaultValue","").ToString();
				defaultValue = defaultValue == "null" ? null : defaultValue;
				
				allowEmpty = bool.Parse(xpi.Current.GetAttribute("allowEmpty","").ToString());
				
				expected = xpi.Current.GetAttribute("expected","").ToString();
				expected = expected == "null" ? null : expected;

				result = Settings.Config.GetString(key, defaultValue, allowEmpty);

				Assert.AreEqual(expected,result,key);								
			}

		}

		[Test]
		public void GetBoolean()
		{
			Settings.Config.SetReader(new Reader(new BooleanTestConfigReader()));

			XmlDocument testList = new XmlDocument();
			testList.Load(@"..\testList_GetBoolean.xml");
			XPathNavigator xpn = testList.CreateNavigator();
			XPathNodeIterator xpi = xpn.Select("//test");

			string settingValue;
			bool defaultValue;
			bool expected;
			bool result;
			string key;

			while ( xpi.MoveNext())
			{
				key = xpi.Current.GetAttribute("key","").ToString();

				settingValue = xpi.Current.GetAttribute("settingValue","").ToString();
				settingValue = settingValue == "null" ? null : settingValue;
				
				defaultValue = bool.Parse(xpi.Current.GetAttribute("defaultValue","").ToString());
				expected = bool.Parse(xpi.Current.GetAttribute("expected","").ToString());

				result = Settings.Config.GetBoolean(key,defaultValue);

				Assert.AreEqual(expected,result,key);								
			}

		}

	}
}
