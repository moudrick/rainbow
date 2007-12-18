// ChargeCardVerification - Version 1.0
//
//	Author:  Steven Perry
//
// This class has been created to perform pre-verification, or internal as I 
//	call it, of credit card numbers.  Two checks are performed, the first is a
//	Regex pattern match based on the card type and the second is the Luhn test,
//	or sometimes more commonly known as Mod 10.  If either of these tests fail
//	an exception is thrown with an appropriate error message.  The routines
//	used in the enclosed functions were collected from many various places on
//	the internet.  None used in there entirely as I have taken "Poetic License"
//	with implementation.
// This class may be used as-is or you may change it to suite your needs.  I
//	make no claims to it's usefullness in any given situation.
// I will eventually make a custom Exception class to be used with this class,
//	but havn't had the time to do that.

// history : Thierry (Tiptopweb), IsValue returns a  bool value (no exceptions)
namespace Rainbow.ECommerce
{
	using System;
	using System.Text.RegularExpressions;
	/// <summary>
	/// Summary description for ChargeCardVerification.
	/// This class is used to perfrom preliminary verification of credit card 
	/// numbers.  It only checks against known patterns and the Mod 10 (Luhn 
	/// Test).  It does not perform any online, or live, validations.  This 
	/// purpose of this class is to attempt to validate a charge card number as 
	/// best possible before attempting to process it to the online processing 
	/// service.
	/// </summary>
	public class ChargeCardVerification
	{
		#region Class public enum stuctures
		/// <summary>
		/// ChargeCardType enum used to set the Card Type being checked.
		/// </summary>
		public enum ChargeCardType
		{
			MasterCard = 1,
			Visa = 2,
			AmericanExpress = 3,
			Discover = 4,
			DinersClub = 5,
			CarteBlanche = 6,
			EnRoute = 7,
			JCB = 8
		}
		#endregion

		#region Internal class member variables
		private string m_strCardNumber;		// Internal storage for credit card 
		//	number.
		private int m_nCardType;					// Internal storage for the Card Type.
		#endregion

		#region Class Constructors (ctor's)
		/// <summary>
		/// Constructor (ctor) for this class.  It initializes with default 
		/// parameters.
		/// </summary>
		public ChargeCardVerification()
		{
			m_nCardType = (int)ChargeCardType.MasterCard;
		}

		/// <summary>
		/// Constructor (ctor) with passed parameters to initialize the class.
		/// </summary>
		/// <param name="p_strCardNumber"></param>
		/// <param name="p_nCardType"></param>
		public ChargeCardVerification( string p_strCardNumber, int p_nCardType )
		{
			CardNumber = p_strCardNumber;
			CardType = p_nCardType;
		}
		#endregion

		#region Charge Card Validation Procedures
		/// <summary>
		/// Public function used to perform the actual validations on the card 
		/// number and throw errors to indicate that an error occured.
		/// </summary>
		public bool IsValid()
		{
			// Perform a Regex Pattern Match on the	card number.
			if( !PatternMatch() )	
				return false;	
			//throw new Exception( "Credit Card number does not pass a pattern match" );

			// Perform the Luhn (Mod 10) test on the card number.
			if( !LuhnTest() )								
				return false;	
			//throw new Exception( "Credit Card number is not valid" );

			return true;
		}

		/// <summary>
		/// Public function used to perform the actual validations on the card 
		/// number and throw errors to indicate that an error occured.
		/// </summary>
		/// <param name="p_strCardNumber"></param>
		/// <param name="p_nCardType"></param>
		public bool IsValid( string p_strCardNumber, int p_nCardType )
		{
			CardNumber = p_strCardNumber;
			CardType = p_nCardType;
			return IsValid();
		}

		/// <summary>
		/// Luhn or otherwise known as the Mod 10 test on the credit card number to
		/// validate it.
		/// </summary>
		/// <returns>bool - True if card is valid, otherwise false</returns>
		private bool LuhnTest()
		{
			// Store Card number in local temporary 
			string strCCNum = CardNumber;		
			//	storage.
			if( strCCNum.Length < 17 )			// Pad the card number to 17 digits.  
				//	Normally it's only padded to 16 
				//	digits but in C# indices start at 0 
				//	and since 0 is neither odd nor even 
				//	we add an additioanl character and 
				//	start our loop below from 0 (zero).
				strCCNum = strCCNum.PadLeft( 17, '0' );

			int nTotal = 0;
			for( int nNdx = 1; nNdx < strCCNum.Length; nNdx++ )
			{
				// Convert each digit to an integer.
				int nDigit = Convert.ToInt32( strCCNum.Substring( nNdx, 1 ) );

				// Multiply the digit by 1 if position is 
				//	even and 2 if odd.
				int nSum = nDigit * (1 + (nNdx % 2));

				if( nSum > 9 )								// if sum is greater then 9 then 
					nSum -= 9;									//	substract 9.

				nTotal += nSum;								// Add sum to total.
			}

			return ((nTotal % 10) == 0);		// Return true if the total Mod 10 = 0 
			//	otherwise false.
		}

		/// <summary>
		/// Executes a Regex pattern match on the supplied credit card number.
		/// </summary>
		/// <returns>bool true if pattern matches otherwise false.</returns>
		private bool PatternMatch()
		{
			// Retrieve Regex pattern for the card 
			//	type.
			string strPattern = GetRegexPattern( CardType );

			if( strPattern.Equals( "" ) )		// Make sure we have a pattern.
				return false;

			// Create the Regex class with the 
			//	pattern retrieved above.
			Regex regex = new Regex( strPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline );

			// Perform the pattern match and return 
			//	the result to the calling procedure.
			return regex.IsMatch( CardNumber );
		}

		/// <summary>
		/// Retrieves the Regex pattern for the given card type.
		/// </summary>
		/// <param name="p_cct">ChargeCardType</param>
		/// <returns>Regex string pattern for the card specified.</returns>
		public string GetRegexPattern( int p_cct )
		{
			string strPattern = "";

			switch( p_cct )
			{
				case (int)ChargeCardType.AmericanExpress:
					strPattern = "^(?:(?:[3][4|7])(?:\\d{13}))$";
					break;
				case (int)ChargeCardType.CarteBlanche:
					strPattern = "^(?:(?:[3](?:[0][0-5]|[6|8]))(?:\\d{11,12}))$";
					break;
				case (int)ChargeCardType.DinersClub:
					strPattern = "^(?:(?:[3](?:[0][0-5]|[6|8]))(?:\\d{11,12}))$";
					break;
				case (int)ChargeCardType.Discover:
					strPattern = "^(?:(?:6011)(?:\\d{12}))$";
					break;
				case (int)ChargeCardType.EnRoute:
					strPattern = "^(?:(?:[2](?:014|149))(?:\\d{11}))$";
					break;
				case (int)ChargeCardType.JCB:
					strPattern = "^(?:(?:(?:2131|1800)(?:\\d{11}))$|^(?:(?:3)(?:\\d{15})))$";
					break;
				case (int)ChargeCardType.MasterCard:
					strPattern = "^(?:(?:[5][1-5])(?:\\d{14}))$";
					break;
				case (int)ChargeCardType.Visa:
					strPattern = "^(?:(?:[4])(?:\\d{12}|\\d{15}))$";
					break;
			}

			return strPattern;
		}

		#endregion

		#region Class Member Variable Accessor Functions

		/// <summary>
		/// CardNumber accessor function.  Cleans up the card number before storing
		/// it by stripping non essential characters like '-', etc.
		/// </summary>
		public string CardNumber
		{
			get
			{
				return m_strCardNumber;
			}
			set
			{
				// Use Regex to strip the non-essential character.
				Regex regex = new Regex( "(\\-|\\s|\\D)*", RegexOptions.IgnoreCase | RegexOptions.Singleline );
				// Strip characters and store result.
				m_strCardNumber = regex.Replace( value, "" );
			}
		}

		/// <summary>
		/// CardType accessor function.
		/// </summary>
		public int CardType
		{
			get
			{
				return m_nCardType;
			}
			set
			{
				m_nCardType = value;
			}
		}
		#endregion
	}
}
