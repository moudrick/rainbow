using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Settings;

namespace Rainbow.Setup
{
	/// <summary>
	/// Summary description for Setup.
	/// This code copyright 2003 by DUEMETRI
	/// Exclusive use with Rainbowportal
	/// Any other use strictly prohibited
	/// </summary>
	public class Update : Page
	{
		[Serializable]
		private class UpdateEntry :IComparable 
		{
			/// <summary>
			/// IComparable.CompareTo implementation.
			/// </summary>
			public int CompareTo(object obj) 
			{
				if(obj is UpdateEntry) 
				{
					UpdateEntry upd = (UpdateEntry) obj;
					if (VersionNumber.CompareTo(upd.VersionNumber) == 0) //Version numbers are equal
						return Version.CompareTo(upd.Version);
					else
						return VersionNumber.CompareTo(upd.VersionNumber);
				}
				throw new ArgumentException("object is not a UpdateEntry");    
			}

			public int VersionNumber = 0;
			public string Version = string.Empty;
			public string ScriptName = string.Empty;
			public DateTime Date;
			public ArrayList Modules = new ArrayList();
			public bool Apply = false;
		}


		protected HtmlTableRow Status;
		protected Button UpdateDatabaseCommand;
		protected Label Label2;
		protected Label lblError;
		protected DataList dlScripts;
		protected DataList dlErrors;
		protected HtmlTableRow dbUpdateResult;
		protected HtmlTableRow dbNeedsUpdate;
		protected HtmlTableRow dbNoUpdate;
		protected Label lblVersion;
		protected Button FinishButton;
		protected Button Button1;
		protected DataList dlMessages;
		protected Panel UpdatePanel;
		protected Panel AuthenticationPanel;
		protected TextBox updateUsername;
		protected TextBox updatePassword;
		protected Button authenticateUser;
		protected Label loginError;
		protected Panel InfoPanel;
		protected Label lblStatus;

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//always modified
			Response.AddHeader("Last-Modified", DateTime.Now.ToUniversalTime().ToString() + " GMT");
			//HTTP/1.1 
			Response.AddHeader("Cache-Control", "no-cache, must-revalidate");
			//HTTP/1.0 
			Response.AddHeader("Pragma", "no-cache");
			//last ditch attempt!
			Response.Expires = -100;

			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.authenticateUser.Click += new EventHandler(this.authenticateUser_Click);
			this.Button1.Click += new EventHandler(this.FinishButton_Click);
			this.UpdateDatabaseCommand.Click += new EventHandler(this.UpdateDatabaseCommand_Click);
			this.FinishButton.Click += new EventHandler(this.FinishButton_Click);
			this.Load += new EventHandler(this.Update_Load);

		}
		#endregion

		private UpdateEntry[] scriptsList;

		/// <summary>
		/// This property returns db version.
		/// It does not rely on cached value and always gets the actual value.
		/// </summary>
		private int DatabaseVersion
		{
			get
			{
				//Clear version cache so we are sure we update correctly
				HttpContext.Current.Application.Lock();
				HttpContext.Current.Application[Database.dbKey] = null;
				HttpContext.Current.Application.UnLock();
				return Database.DatabaseVersion;
			}
		}

		private void Update_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				//jes1111 - if (ConfigurationSettings.AppSettings["UpdateUserName"] == null || ConfigurationSettings.AppSettings["UpdateUserName"].Length == 0)
				if ( Config.UpdateUserName == string.Empty )
				{
					//No login is shown if no user is specified
					//by manu
					AuthenticationPanel.Visible = false;
					InfoPanel.Visible = true;
					UpdatePanel.Visible = true;
				}
				else
				{
					AuthenticationPanel.Visible = true;
					InfoPanel.Visible = false;
					UpdatePanel.Visible = false;
				}
			}
			else
			{
				//Hide infopanel when update starts....
				InfoPanel.Visible = false;
			}
			
			int dbVersion = DatabaseVersion;

			if (dbVersion < 1519 && dbVersion > 1114)
			{
				LogHelper.Logger.Log(LogLevel.Error, "Unsupported version " + dbVersion  + " detected.");
				throw new Exception("Version before 1519 are not supported anymore. Please upgrade to a newer version or upgrade manually.");
			}

			dbNeedsUpdate.Visible = false;

			if(dbVersion < 1114)
                lblVersion.Text = "Empty/New database - CodeVersion: " + Portal.CodeVersion.ToString();
			else
                lblVersion.Text = "dbVersion: " + dbVersion.ToString() + " - CodeVersion: " + Portal.CodeVersion.ToString();

			// ******************************
			// New code starts here - Jes1111
			// ******************************
			// this is not a performance-sensitive routine, so XmlDocument is sufficient
			XmlDocument myDoc = new  XmlDocument();
			ArrayList tempScriptsList = new ArrayList();

			if (dbVersion < Portal.CodeVersion)
			{
				LogHelper.Logger.Log(LogLevel.Debug, "db:" + dbVersion + " Code:" + Portal.CodeVersion);

				// load the history file
				string myDocPath = Server.MapPath(Path.ApplicationRoot + "/Setup/Scripts/History.xml");
				myDoc.Load(myDocPath);
                	
				// get a list of <Release> nodes
				XmlNodeList releases = myDoc.DocumentElement.SelectNodes("Release");

				// iterate over the <Release> nodes
				// (we can do this because XmlNodeList implements IEnumerable)
				foreach (XmlNode release in releases)
				{
					UpdateEntry myUpdate = new UpdateEntry();

					// get the header information
					// we check for null to avoid exception if any of these nodes are not present
					if (release.SelectSingleNode("ID") != null)
						myUpdate.VersionNumber = Int32.Parse(release.SelectSingleNode("ID/text()").Value);
					if (release.SelectSingleNode("Version") != null)
						myUpdate.Version = release.SelectSingleNode("Version/text()").Value;
					if (release.SelectSingleNode("Script") != null)
						myUpdate.ScriptName = release.SelectSingleNode("Script/text()").Value;
					if (release.SelectSingleNode("Date") != null)
						myUpdate.Date = DateTime.Parse(release.SelectSingleNode("Date/text()").Value);

					//We should apply this patch
					if (dbVersion < myUpdate.VersionNumber)
					{
						//Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);

						myUpdate.Apply = true;

						// get a list of <Installer> nodes
						XmlNodeList installers = release.SelectNodes("Modules/Installer/text()");

						// iterate over the <Installer> Nodes (in original document order)
						// (we can do this because XmlNodeList implements IEnumerable)
						foreach (XmlNode installer in installers)
						{
							//and build an ArrayList of the scripts... 
							myUpdate.Modules.Add(installer.Value);
							//Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
						}
						tempScriptsList.Add(myUpdate);
					}
				}

				//If we have some version to apply...
				if (tempScriptsList.Count > 0)
				{
					scriptsList = (UpdateEntry[]) tempScriptsList.ToArray(typeof(UpdateEntry));

					//by Manu. Versions are sorted by version number
					Array.Sort(scriptsList); 

					//Create a flat version for binding
					int currentVersion = 0;
					ArrayList databindList = new ArrayList();
					foreach(UpdateEntry myUpdate in scriptsList)
					{
						if (myUpdate.Apply)
						{
							if (currentVersion != myUpdate.VersionNumber)
							{
								databindList.Add("Version: " + myUpdate.VersionNumber);
								currentVersion = myUpdate.VersionNumber;
							}

							if (myUpdate.ScriptName.Length > 0)
								databindList.Add("-- Script: " + myUpdate.ScriptName);

							foreach(string moduleInstaller in myUpdate.Modules)
							{
								if (moduleInstaller.Length > 0)
									databindList.Add("-- Module: " + moduleInstaller);
							}
						}
					}

					if (databindList.Count > 0)
					{
						//Some update to do
						dlScripts.DataSource = databindList; //we bind a simple list
						dlScripts.DataBind();
						dbNeedsUpdate.Visible = true;
						dbNoUpdate.Visible = false;
					}
				}
				else
				{
					//No update is needed
				}
			}
		}

		private void UpdateDatabaseCommand_Click(object sender, EventArgs e)
		{
			ArrayList errors = new ArrayList();
			ArrayList messages = new ArrayList();

			foreach(UpdateEntry myUpdate in scriptsList)
			{
				//Version check (a script may update more than one version at once)
				if (myUpdate.Apply && DatabaseVersion < myUpdate.VersionNumber && DatabaseVersion < Portal.CodeVersion)
				{
					//It may be a module update only
					if (myUpdate.ScriptName.Length > 0)
					{
						string currentScriptName = Server.MapPath(System.IO.Path.Combine(Path.ApplicationRoot + "/Setup/Scripts/", myUpdate.ScriptName));
						LogHelper.Logger.Log(LogLevel.Info, "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " + myUpdate.VersionNumber + " - Applying: " + currentScriptName);
						ArrayList myErrors = DBHelper.ExecuteScript(currentScriptName, true);
						errors.AddRange(myErrors);
					}

					//Installing modules
					foreach (string moduleInstaller in myUpdate.Modules)
					{
						string currentModuleInstaller = Server.MapPath(System.IO.Path.Combine(Path.ApplicationRoot + "/", moduleInstaller));
						
						try
						{ 
							ModuleInstall.InstallGroup(currentModuleInstaller, true);
						}
						catch (Exception ex)
						{
							if (ex.InnerException != null) 
							{
								// Display more meaningful error message if InnerException is defined
								LogHelper.Logger.Log(LogLevel.Warn, "Exception in UpdateDatabaseCommand installing module: " + currentModuleInstaller, ex.InnerException);
								errors.Add("Exception in UpdateDatabaseCommand installing module: " + currentModuleInstaller + "<br/>" + ex.InnerException.Message + "<br/>" + ex.InnerException.StackTrace );
							}
							else 
							{
								LogHelper.Logger.Log(LogLevel.Warn, "Exception in UpdateDatabaseCommand installing module: " + currentModuleInstaller, ex);
								errors.Add(ex.Message);
							}
						}
					}

					//Display errors if any
					if (errors.Count > 0)
					{
						string currentError = myUpdate.ScriptName;
						errors.Insert(0, "<P>" + currentError + "</P>");
						LogHelper.Logger.Log(LogLevel.Error, "Version " + myUpdate.Version + " completed with errors.  - " + currentError);
						break;
					}
					else
					{
						//Update db with version
						string versionUpdater = "INSERT INTO [rb_Versions] ([Release],[Version],[ReleaseDate]) VALUES('" + myUpdate.VersionNumber + "','" + myUpdate.Version + "', CONVERT(datetime, '" + myUpdate.Date.Month + "/" + myUpdate.Date.Day + "/" + myUpdate.Date.Year + "', 101))";
						DBHelper.ExeSQL(versionUpdater);
						LogHelper.Logger.Log(LogLevel.Info, "Version number: " + myUpdate.Version + " applied successfully.");

						//Mark this update as done
						LogHelper.Logger.Log(LogLevel.Info, "Sucessfully applied version: " + myUpdate.Version);
					}
				}
				else
				{
					//Skipped
					string skippedMessage = 
						"Skipping: "
						+ myUpdate.Version
						+ " DbVersion ("
						+ DatabaseVersion
						+ ") "
						+ " Codeversion ("
						+ Portal.CodeVersion
						+ ")"
						;
					messages.Add(skippedMessage);
					LogHelper.Logger.Log(LogLevel.Info, "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " + myUpdate.VersionNumber + " - Skipping: " + myUpdate.Version);
				}
			}

			dbUpdateResult.Visible = false;
			dbNeedsUpdate.Visible = false;
			Status.Visible = true;
			
			if (messages.Count > 0)
			{
				dlErrors.DataSource = messages;
				dlErrors.DataBind();
				dlErrors.Visible = true;
				dlErrors.ForeColor = Color.Green;
				dbUpdateResult.Visible = true;

				dbUpdateResult.Visible = true;
				dbNeedsUpdate.Visible = false;
			}

			if (errors.Count > 0)
			{
				dlErrors.DataSource = errors;
				dlErrors.DataBind();
				dlErrors.Visible = true;
				dlErrors.ForeColor = Color.Red;

				dbUpdateResult.Visible = true;
				dbNeedsUpdate.Visible = false;
				Status.Visible = false;
			}
		}

		private void FinishButton_Click(object sender, EventArgs e)
		{
			LogHelper.Logger.Log(LogLevel.Info, "Update complete");

			//Global.dbNeedsUpdate = false;
			Response.Redirect(Path.ApplicationRoot + "/Default.aspx");
		}

		private void authenticateUser_Click(object sender, EventArgs e)
		{
			string providedUser = string.Empty;
			string providedPassword = string.Empty;

			// jes1111 - if (ConfigurationSettings.AppSettings["UpdateUserName"] != null)
			providedUser = Config.UpdateUserName;

			//jes1111 - if (ConfigurationSettings.AppSettings["UpdatePassword"] != null)
			providedPassword = Config.UpdatePassword;

//			if(providedUser.ToLower().Equals(updateUsername.Text.ToLower()) && providedPassword.Equals(updatePassword.Text))
			if(String.Compare(providedUser, updateUsername.Text, true) == 0 && String.Compare(providedPassword, updatePassword.Text) == 0)
			{
					AuthenticationPanel.Visible = false;
					UpdatePanel.Visible=true;
			}
			else
			{
				loginError.Visible = true;
				LogHelper.Logger.Log(LogLevel.Warn, "Someone has incorrectly tried to log into the setup / update page. User IP: '" + HttpContext.Current.Request.UserHostAddress.ToString() + "' Username Entered: '" + updateUsername.Text.ToString() + "' Password Entered: '" + updatePassword.Text.ToString() + "'");
			}
		}
	}
}