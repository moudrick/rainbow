using System.Configuration;
using System.Text;

namespace RewriteRules.Rainbow.Tools
{
	/// <summary>
	/// General Tool
	/// This Class is responsible for containing all the tools / useful methods
	/// for hanling urls. It also contains legacy methods such as add attribute and KeywordSplitter.
	/// Created by John Mandia www.totalingenuity.com, contributors to this and 
	/// previous versions are Jes, Manu and Cory. 
	/// </summary>
	sealed public class General
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private General() {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parts"></param>
		/// <param name="sb"></param>
		/// <param name="defaultSplitter"></param>
		/// <param name="pageidNoSplitter"></param>
		public static void Splitter(string[] parts, StringBuilder sb, string defaultSplitter, bool pageidNoSplitter)
		{
			//int _firstpos;
			sb.Append("?");
			int totalParts = parts.GetUpperBound(0);

			if(pageidNoSplitter)
			{
				// last part of the array will be the page name. The pageid will be just before that.
				
				int pageidPosition = (totalParts - 1);
				for (int i = 0; i < totalParts; i++)
				{   
					if(pageidPosition == i)
					{
						//sb.Append("tabid=");
						sb.Append("PageID=");
						sb.Append(parts[i].ToString());
					}
					else
					{
						sb.Append(parts[i].ToString().Replace(defaultSplitter, "="));
					}
					sb.Append("&");
				}
			}
			else
			{
				for (int i = 0; i < totalParts; i++)
				{                 
					sb.Append(parts[i].ToString().Replace(defaultSplitter, "="));
					sb.Append("&");
				}
			}
			
			// remove the extra &
			sb.Remove(sb.Length - 1, 1);
		}

		/// <summary>
		/// 
		/// </summary>
		public static string DefaultPage
		{
			get
			{
				string strTemp = ConfigurationSettings.AppSettings["HandlerTargetUrl"];
				if (strTemp.Length == 0 || strTemp == null) strTemp = "Default.aspx";
				return strTemp;
			}
		}

		/// <summary>
		/// Start Of Legacy Support Tools for previous Handler
		/// </summary>
		/// <param name="parts"></param>
		/// <param name="sb"></param>
		/// <param name="keyNumber"></param>
		/// <param name="attribute"></param>
		public static void AddAttribute(string[] parts, StringBuilder sb, int keyNumber, string attribute)
		{
			// The last one is the page so we do not consider it! (> and not >=)
			if (parts.GetUpperBound(0) > keyNumber && parts[keyNumber] != null)
			{
				sb.Append(attribute);
				sb.Append(parts[keyNumber]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="attribute"></param>
		/// <param name="value"></param>
		public static void AddAttribute(StringBuilder sb, string attribute, string value)
		{
			if (value != null && value.Length != 0)
			{
				sb.Append(attribute);
				sb.Append(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parts"></param>
		/// <param name="sb"></param>
		/// <param name="count"></param>
		public static void KeywordSplitter(string[] parts, StringBuilder sb, int count)
		{
			//char[] _equals = new char[] {'='};
			char[] _boundaryChars = new char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '='};
			int _firstpos;

			for (int i = count; i < parts.GetUpperBound(0); i++)
			{
				sb.Append("&");
				try
				{
					_firstpos = parts[i].IndexOfAny(_boundaryChars);
					if (_firstpos > 0)
					{
						if (parts[i].Substring(_firstpos, 1) == "_")
							parts[i] = parts[i].Substring(0, _firstpos) + "=" + parts[i].Substring(_firstpos + 1);
						else if (parts[i].Substring(_firstpos, 1) == "=")
						{
						} // do nothing
						else
							parts[i] = parts[i].Insert(_firstpos, "=");
					}
				}
				finally
				{
					sb.Append(parts[i]);
				}
			}
		}

		// End of Legacy Support Tools For Previous Handler
	}
}