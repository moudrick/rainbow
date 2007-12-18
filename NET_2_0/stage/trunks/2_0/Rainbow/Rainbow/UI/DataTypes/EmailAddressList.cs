using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// This implementation of the ArrayList class, allows only valid 
	/// emailadresses to be added to the collection
	/// </summary>
	public class EmailAddressList: ArrayList
	{
		private static Regex _regex;
		private static Regex emailaddress
		{
			get
			{
				if (_regex == null)
					_regex = new Regex("^([a-zA-Z0-9_\\.\\-])+\\@(([a-zA-Z0-9\\-])+\\.)+([a-zA-Z0-9]{2,4})+$", RegexOptions.Compiled & RegexOptions.IgnoreCase);
				
				return _regex;
			}
		}

		public override int Add(object value)
		{
			// Check if the value isn't null
			if ( value == null )
				throw new ArgumentNullException("value", "You can not add null email-addresses to the collection.");
			// Check if the value is a string
			if ( ! (value is string) )
				throw new ArgumentOutOfRangeException("value", "Only string values are allowed.");
			// Check if the value can be matched to the regular expression
			if ( ! emailaddress.IsMatch((string)value) )
				throw new ArgumentException("value", "Only valid email-addresses are allowed.");
			// This is a valid email address
			return base.Add(value);
		}
	}
}
