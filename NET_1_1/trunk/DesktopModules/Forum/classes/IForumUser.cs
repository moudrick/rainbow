using System;
using System.Web;

namespace yaf
{
	public interface IForumUser
	{
		string Name
		{
			get;
		}
		string Email
		{
			get;
		}
		bool IsRainbowAdmin  //KAW
		{
			get;
		}
		bool IsAuthenticated
		{
			get;
		}
		object Location
		{
			get;
		}
		object HomePage
		{
			get;
		}
		bool CanLogin
		{
			get;
		}
		string Password
		{
			get;
		}
		void UpdateUserInfo(int userID);
	}

	public class WindowsUser : IForumUser
	{
		private	string	m_userName;
		private	string	m_email;
		private	bool	m_isAuthenticated;
		private bool    m_isAdmin;//KAW
		

		public WindowsUser() 
		{
			string userName = HttpContext.Current.User.Identity.Name;
			

			try
			{
				if(HttpContext.Current.User.Identity.IsAuthenticated)
				{
					string[] parts = userName.Split('\\');
					m_userName  = parts[parts.Length-1];
					if(parts.Length>1)
						m_email = String.Format("{0}@{1}",parts[1],parts[0]);
					else
						m_email = m_userName;
					
					m_isAuthenticated = true;
					m_isAdmin = HttpContext.Current.User.IsInRole("Admins");
					
					return;
				}
			}
			catch(Exception)
			{
			}
			m_userName = "";
			m_email = "";
			m_isAuthenticated = false;
			m_isAdmin = false;
			
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsRainbowAdmin
		{
			get
			{
				return m_isAdmin;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
		public string Password
		{
			get
			{
				return null;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
		}
	}
	
	public class FormsUser : IForumUser
	{
		private	string	m_userName;
		private	int		m_userID;
		private	int		m_boardID;
		private	bool	m_isAuthenticated;
		private bool    m_isAdmin;//Kaw
		

		public FormsUser() 
		{
			string userName = HttpContext.Current.User.Identity.Name;
			m_isAdmin = false;

			try
			{
				if(HttpContext.Current.User.Identity.IsAuthenticated)
				{
					string[] parts = userName.Split(';');
					if(parts.Length==3)
					{
						m_userID			= int.Parse(parts[0]);
						m_boardID			= int.Parse(parts[1]);
						m_userName			= parts[2];
						m_isAuthenticated	= true;
						m_isAdmin = HttpContext.Current.User.IsInRole("Admins");
						
						return;
					}
				}
			}
			catch(Exception)
			{
			}
			m_userName			= "";
			m_userID			= 0;
			m_boardID			= 0;
			m_isAuthenticated	= false;
			m_isAdmin = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}
		public bool IsRainbowAdmin
		{
			get
			{
				return m_isAdmin;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
		public string Password
		{
			get
			{
				return null;
			}
		}

		public void UpdateUserInfo(int userID) 
		{
		}
	}
	public class GuestUser : IForumUser
	{

		public string Name
		{
			get
			{
				return "";
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}
		public bool IsRainbowAdmin
		{
			get
			{
				return false;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
		public string Password
		{
			get
			{
				return null;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
		}
	}
}
