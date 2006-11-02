using System;
using System.Security.Principal;
using System.Xml;

namespace Rainbow.Security
{
	/// <summary>
	/// User identity
	/// </summary>
	public class User : GenericIdentity
	{
		private string m_Name = string.Empty;
		private string m_Email;
		private string m_ID;
		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public User(string name) : base(name)
		{
			SetUserData(name);
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="type" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public User(string name, string type) : base(name, type)
		{
			SetUserData(name);
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="email" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="id" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public User(string name, string email, string id) : base(name)
		{
			m_Name = name;
			m_Email = email;
			m_ID = id;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		private void SetUserData(string name)
		{
			string[] uData = name.Split('|');
			if (uData.GetUpperBound(0) == 2)
			{
				m_Name = uData[0];
				m_Email = uData[1];
				m_ID = uData[2];
			}
			else if (uData.GetUpperBound(0) == 0)
			{
				m_Name = uData[0];
				m_Email = uData[0];
				m_ID = "0";
			}
			else
				throw new Exception("Invalid user");
		}
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public override string Name
		{
			get
			{
				return m_Name;
			}
		}
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public string Email
		{
			get
			{
				return m_Email;
			}
		}
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public string ID
		{
			get
			{
				return m_ID;
			}
		}
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public override string ToString()
		{
			return Name + "|" + Email + "|" + ID;
		}

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private XmlDocument m_userXmlData;
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     jes1111
		/// </remarks>
		public XmlDocument UserXmlData
		{
			get
			{
				return m_userXmlData;
			}
			set
			{
				m_userXmlData = value;
			}
		}
	}
}

