using System;
using Rainbow.Core;

namespace Rainbow.Data
{
	/// <summary>
	/// Summary description for Users.
	/// </summary>
	public abstract class UsersProvider : DataProvider
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected UsersProvider()
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="company" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="address" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="city" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="zip" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="countryID" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="stateID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="pIva" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="cFiscale" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="phone" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="fax" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="password" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="email" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sendNewsletter" type="bool">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value... the userID
		/// </returns>
		public abstract int Add(int portalID, string name, string company, string address, string city, string zip, string countryID, int stateID, string pIva, string cFiscale, string phone, string fax, string password, string email, Boolean sendNewsletter);

		/// <summary>
		///     
		/// </summary>
		/// <param name="fullName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="email" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="password" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value... the userID
		/// </returns>
		public abstract int Add(String fullName, String email, String password, int portalID);

		/// <summary>
		///     Deletes a user by their userID
		/// </summary>
		/// <param name="userID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public abstract void Remove(int userID);

		/// <summary>
		///     Gets a single user object
		/// </summary>
		/// <param name="email" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A User value...
		/// </returns>
		public abstract User GetSingleUser(string email, int portalID);
	}
}