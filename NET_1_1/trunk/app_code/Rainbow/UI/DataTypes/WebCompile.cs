using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;
using System.Web;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Timer = System.Timers.Timer;

namespace Rainbow.UI.DataTypes.WebCompile
{
	/// <summary>
	/// Summary description for WebCompile.
	/// This class added to the Rainbow Namespace Aug_28_2003 by Cory Isakson
	/// Code taken from Paul Wilson's article at:
	/// http://www.aspalliance.com/PaulWilson/Articles/?id=12
	/// </summary>
	public class GlobalBase : HttpApplication
	{
		private static bool needsCompile;
		private static string applicationPath;
		private static string physicalPath;
		private static string applicationURL;
		
		private static Thread thread;
		private static System.Timers.Timer timer;
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Elapsed;

		static GlobalBase()
		{
//			#if DEBUG
//				needsCompile = false;
//			#else  
//				needsCompile = true;
//			#endif 
			needsCompile = Rainbow.Settings.Config.EnableWebCompile;
			applicationPath = string.Empty;
			physicalPath = string.Empty;
			applicationURL = string.Empty;
			
			thread = null;
			timer = null;
		}

		/// <summary>
		/// Override and Indicate Time in Minutes to Force the Keep-Alive
		/// </summary>
		protected virtual int KeepAliveMinutes 
		{
			get { return 15; }
		}

		/// <summary>
		/// Override and Indicate Files to Skip with Semi-Colon Delimiter
		/// </summary>
		protected virtual string SkipFiles 
		{
			get { return @""; }
		}

		/// <summary>
		/// Override and Indicate Folders to Skip with Semi-Colon Delimiter
		/// </summary>
		protected virtual string SkipFolders 
		{
			get { return @""; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void Init() 
		{
			if (GlobalBase.needsCompile) 
			{
				GlobalBase.needsCompile = false;

				applicationPath = HttpContext.Current.Request.ApplicationPath;
				if (!applicationPath.EndsWith("/")) { applicationPath += "/";	}

				string server = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
				bool https = HttpContext.Current.Request.ServerVariables["HTTPS"] != "off";
				applicationURL = (https ? "https://" : "http://") + server + applicationPath;

				physicalPath = HttpContext.Current.Request.PhysicalApplicationPath;
				thread = new Thread(new ThreadStart(CompileApp));
				thread.Start();

				if (this.KeepAliveMinutes > 0) 
				{
					timer = new Timer(60000 * this.KeepAliveMinutes);
					timer.Elapsed += new ElapsedEventHandler(KeepAlive);
					timer.Start();
				}
			}
		}

		private void KeepAlive(Object sender, ElapsedEventArgs e) 
		{
			timer.Enabled = false;
			if (this.Elapsed != null) { this.Elapsed(this, e); }
			timer.Enabled = true;
			string url = applicationURL;
			using (HttpWebRequest.Create(url).GetResponse()) {}
			LogHelper.Logger.Log(LogLevel.Debug, "Timer: " + url);
		}

		private void CompileApp() 
		{
			CompileFolder(physicalPath);
		}

		private void CompileFolder(string Folder) 
		{
			foreach (string file in Directory.GetFiles(Folder, "*.as?x")) 
			{
				CompileFile(file);
			}

			foreach (string folder in Directory.GetDirectories(Folder)) 
			{
				bool skipFolder = false;
				foreach (string item in this.SkipFolders.Split(';')) 
				{
					if (item.Length != 0 && folder.ToUpper().EndsWith(item.ToUpper())) 
					{
						skipFolder = true;
						break;
					}
				}
				if (!skipFolder) 
				{
					CompileFolder(folder);
				}
			}
		}

		private void CompileFile(string File) 
		{
			bool skipFile = false;
			File = File.ToLower();
			foreach (string item in this.SkipFiles.Split(';')) 
			{
				if (item.Length != 0 && File.EndsWith(item.ToLower())) 
				{
					skipFile = true;
					break;
				}
			}

			if (!skipFile) 
			{
				string path = File.Remove(0, physicalPath.Length);
				if (File.EndsWith(".ascx")) 
				{
					string virtualPath = applicationPath + path.Replace(@"\", "/");
					using (System.Web.UI.Page controlLoader = new System.Web.UI.Page()) 
					{
						try 
						{
							controlLoader.LoadControl(virtualPath);
						}
						finally 
						{
							LogHelper.Logger.Log(LogLevel.Debug, "Control: " + virtualPath);
						}
					}
				}
				else if (!File.EndsWith(".asax")) 
				{
					string url = applicationURL + path.Replace(@"\", "/");
						// 02_09_2003 User Agent Setting added - Cory Isakson
						HttpWebRequest wc = (HttpWebRequest) WebRequest.Create(url);
						wc.UserAgent="Rainbow_Web_Compile";
						wc.GetResponse();
						
					LogHelper.Logger.Log(LogLevel.Debug, "Page: " + url);
				}
			}
		}
	}
}