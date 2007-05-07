using System.Web;
using Rainbow.Settings;

namespace Rainbow.Configuration
{
	public class LogCodeVersionProperty
	{
		public override string ToString()
		{
			try
			{
				return Portal.CodeVersion.ToString();
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class LogUserNameProperty
	{
		public override string ToString()
		{
			try
			{
				return HttpContext.Current.User.Identity.Name;
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class LogRewrittenUrlProperty
	{
		public override string ToString()
		{
			try
			{
				return HttpContext.Current.Server.HtmlDecode(HttpContext.Current.Request.Url.ToString());
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class LogUserAgentProperty
	{
		public override string ToString()
		{
			try
			{
				return HttpContext.Current.Request.UserAgent;
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class LogUserLanguagesProperty
	{
		public override string ToString()
		{
			try
			{
				return string.Join(";", HttpContext.Current.Request.UserLanguages);
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class LogUserIpProperty
	{
		public override string ToString()
		{
			try
			{
				return HttpContext.Current.Request.UserHostAddress;
			}
			catch
			{
				return "not available";
			}
		}
	}

	public class PortalAliasProperty
	{
		public override string ToString()
		{
			try
			{
				return Portal.UniqueID;
			}
			catch
			{
				return "not available";
			}

		}
	}


}