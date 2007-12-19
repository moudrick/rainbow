using System.Xml;

namespace Rainbow.Settings
{
	/// <summary>
	/// Concrete Strategy - Reads from XML file of test values
	/// </summary>
	public class BooleanTestConfigReader : Strategy
	{
		/// <summary>
		/// Fetches value for key from XML file of test values
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>string value</returns>
		public string GetAppSetting(string key)
		{
			if (key != null && key.Length != 0)
			{
				XmlDocument testList = new XmlDocument();
				testList.Load(@"..\testList_GetBoolean.xml");
				XmlNode found = testList.SelectSingleNode("//test[@key='" + key + "']/@settingValue");

				return found.Value == "null" ? null : found.Value;
			}

			return null;
		}
	}
}