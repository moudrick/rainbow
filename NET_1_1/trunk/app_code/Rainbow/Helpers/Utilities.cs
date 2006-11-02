using System;
using System.Globalization;
using System.Net;

namespace Rainbow.Helpers 
{ 
	/// <summary>
	/// General utility methods
	/// </summary>
	public class Utils 
	{ 

		/// <summary>
		/// Tests whether a string represents a positive integer
		/// </summary>
		/// <param name="str">string to test</param>
		/// <returns>boolean true/false</returns>
		public static bool IsInteger(string str) 
		{
			if (str == null)
				return false;

			foreach(char c in str) 
			{
				if(!Char.IsNumber(c)) 
				{
					return false;
				}
			}
			return true;

		}

		/// <summary>
		/// Tests a string to see if it is equivalent to a boolean value (i.e. guarantees that
		/// Boolean.Parse will not fail)
		/// </summary>
		/// <param name="str">the string to be tested</param>
		/// <returns>a boolean value</returns>
		public static bool IsBoolean(string str)
		{
			if (str == null)
				return false;

			if (string.Compare(str, Boolean.TrueString, true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
			if (string.Compare(str, Boolean.FalseString, true, CultureInfo.InvariantCulture) != 0)
			{
				str = str.Trim();
				if (string.Compare(str, Boolean.TrueString, true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				if (string.Compare(str, Boolean.FalseString, true, CultureInfo.InvariantCulture) != 0)
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// Tests a string to ensure that it is equivalent to a valid HTTP Status code
		/// </summary>
		/// <param name="str">the string to be tested</param>
		/// <returns>a boolean value</returns>
		public static bool IsHttpStatusCode(string str)
		{
			if (str == null)
				return false;

			if (Enum.IsDefined(typeof (HttpStatusCode), str))
				return true;

			return false;
		}

		/// <summary>
		/// Tests a string value to ensure that it is not null and not an empty string
		/// </summary>
		/// <param name="str">the string to be tested</param>
		/// <returns>a boolean value</returns>
		public static bool IsString(string str)
		{
			if (str != null && str.Length > 0)
				return true;
			return false;
		}


	} 
}