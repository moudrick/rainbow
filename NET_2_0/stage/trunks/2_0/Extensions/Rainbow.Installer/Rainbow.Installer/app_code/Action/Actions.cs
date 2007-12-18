/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Initial coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2005/01/10
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using System.Web;
using System.Web.UI;

using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Rainbow.Settings;
using Rainbow.Configuration;


namespace Rainbow.Installer.Action
{
	/// <summary>
	/// Summary description for InstallAction.
	/// </summary>
	public abstract class ActionBase
	{
		internal const string ROLLBACK_EXTENSION = ".ROLLBACK";
		internal const string DELETED_EXTENSION = ".DELETED";

		internal string GetFolderBase(string folderIndentifer)
		{
			switch (folderIndentifer)
			{
				case "bin":
					return PhysicalFolders.Binaries;
					//return HttpContext.Current.Server.MapPath("~/Portals/Install/bin/");
					break;
				case "themes":
					return PhysicalFolders.Themes;
					break;
				case "layouts":
					return PhysicalFolders.Layouts;
					break;
				case "data":
					return PhysicalFolders.PortalData;
					break;
				case "portalData":
					return PhysicalFolders.PortalData;
					break;
				case "modules":
					return PhysicalFolders.Modules;
					break;
				case "portalThemes":
					return PhysicalFolders.Themes;
					break;
				case "portalLayouts":
					return PhysicalFolders.Layouts;
					break;
				case "install":
					return PhysicalFolders.Install;
					break;
				case "uninstall":
					return PhysicalFolders.Uninstall;
					break;
				case "rainbow":
					return PhysicalFolders.Application;
					break;
				default: //case "custom":case "temp"
					return PhysicalFolders.Temp;
					break;
			}
		}

		/// <summary>
		/// Get the script from a file
		/// </summary>
		/// <returns></returns>
		internal string GetScript(Stream stream)
		{
			string strScript = string.Empty;

			// Load script file
			using (System.IO.StreamReader objStreamReader = new StreamReader(stream))
			{
				strScript = objStreamReader.ReadToEnd();
				objStreamReader.Close();
			}
			return strScript + Environment.NewLine; //Append carriage for execute last command
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void DoAction()
		{ throw new ActionNotDefinedException();}
		/// <summary>
		/// 
		/// </summary>
		public virtual void Rollback()
		{ throw new ActionNotDefinedException();}
		/// <summary>
		/// 
		/// </summary>
		public virtual void Commit()
		{ throw new ActionNotDefinedException();}	
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class CreateFolder :ActionBase	
	{
		private string m_folderName;
		private string m_baseFolder;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseFolder"></param>
		/// <param name="folderName"></param>
		public CreateFolder(string baseFolder,string folderName)
		{
			m_baseFolder = baseFolder ;
			m_folderName = folderName;
		}
		public override void DoAction()
		{
			string _folder = GetFolderBase(m_baseFolder)+m_folderName;
			if (Directory.Exists(_folder))
				Directory.Move (_folder,_folder + ROLLBACK_EXTENSION);

			Directory.CreateDirectory(_folder);
		}
		public override void Rollback()
		{
			string _folder = GetFolderBase(m_baseFolder)+ m_folderName;

			Directory.Delete(_folder,false);

			if (Directory.Exists(_folder + ROLLBACK_EXTENSION))
				Directory.Move (_folder + ROLLBACK_EXTENSION, _folder);

		}
		public override void Commit()
		{
			string _folder = GetFolderBase(m_baseFolder) + m_folderName;
			if (Directory.Exists(_folder + ROLLBACK_EXTENSION))
				if (Directory.GetFiles(_folder + ROLLBACK_EXTENSION,"*.*").Length > 0)
                    Directory.Move (_folder + ROLLBACK_EXTENSION, GetFolderBase("install") +_folder.Replace(@"\","__").Replace(":",string.Empty)+"__DELETED" + DateTime.Now.ToString(".yyyyMMddHHmmss"));
				else
					Directory.Delete (_folder + ROLLBACK_EXTENSION);

		}



	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class CreateFile:ActionBase	
	{
		private byte[]  m_SourceBuffer;
		private string m_baseFolder;
		private string m_DestinationFile;
		public CreateFile(byte[]sourceBuffer, string baseFolder, string destinationFile)
		{
			m_SourceBuffer= sourceBuffer; 
			m_baseFolder = baseFolder;
			m_DestinationFile = destinationFile;
			//base.RollbackAction = new DeleteFile(m_baseFolder,m_DestinationFile);
		}

		public override void DoAction()
		{
			try
			{
				string _folder = GetFolderBase(m_baseFolder);
			
				if (File.Exists (_folder + m_DestinationFile ))
					File.Move(_folder + m_DestinationFile , _folder + m_DestinationFile + ROLLBACK_EXTENSION);
				//	throw new FieldAccessException("File '"+m_DestinationFile+"' already exist!");

				FileStream _fileStream = File.Create(_folder + m_DestinationFile);
				_fileStream.Write(m_SourceBuffer,0,m_SourceBuffer.Length);
				_fileStream.Close();

				//			if (m_DestinationFile.EndsWith(".dll"))
				//				Helper.TestAssembly(_folder + m_DestinationFile);
			}
			catch (Exception ex)
			{
				throw new InstallerException("Errolr creating file. ["+ m_baseFolder+ "|" + m_DestinationFile +"]",ex);
			}
			
		}

		public override void Commit()
		{
			string _folder = GetFolderBase(m_baseFolder);

			if(File.Exists(_folder + m_DestinationFile + ROLLBACK_EXTENSION))
				File.Delete( _folder + m_DestinationFile + ROLLBACK_EXTENSION);
		}

		public override void Rollback()
		{
			string _folder = GetFolderBase(m_baseFolder);

			if(File.Exists(_folder + m_DestinationFile))
				File.Delete( _folder + m_DestinationFile);
			if(File.Exists(_folder + m_DestinationFile +ROLLBACK_EXTENSION))
				File.Move(_folder + m_DestinationFile +ROLLBACK_EXTENSION , _folder + m_DestinationFile );
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class CopyFile:ActionBase	
	{
		private string m_SourceBaseFolder;
		private string m_SourceFile;
		private string m_DestinationBaseFolder;
		private string m_DestinationFile;
		public CopyFile(string sourceBaseFolder,string sourceFile, string destinationBaseFolder,string destinationFile)
		{
			m_SourceBaseFolder = sourceBaseFolder;
			m_SourceFile= sourceFile; 
			m_DestinationBaseFolder = destinationBaseFolder;
			m_DestinationFile = destinationFile;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class RegisterModule:ActionBase	
	{
		private string m_AssemblyFileName;
		private string m_FriedlyName ;
		private string m_ControlSource;
		private string m_MobileControlSource;
		private Guid 	m_ModuleGuid;
		private string 	m_ClassName;
		private bool m_IsAdminModule ;
		private bool m_IsSearchable;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyFileName"></param>
		/// <param name="friendlyName"></param>
		/// <param name="controlSource"></param>
		/// <param name="mobileControlSource"></param>
		public RegisterModule(string assemblyFileName, string moduleGuid,string friendlyName,string controlSource,string mobileControlSource)
		{
			m_AssemblyFileName = assemblyFileName;
			m_FriedlyName = friendlyName;
			m_ControlSource=  controlSource;
			m_MobileControlSource = mobileControlSource;
			m_ModuleGuid = new Guid(moduleGuid);
 		}
		public override void DoAction()
		{
			Rainbow.UI.WebControls.PortalModuleControl _control= null;
			//Rainbow.RainbowModuleAttribute _moduleAttribute = null;

			// get information about the module
			System.Reflection.Assembly _assembly =  System.Reflection.Assembly.LoadFrom(PhysicalFolders.Binaries + m_AssemblyFileName);
			// Get the type to use from the assembly.
			foreach (Type _assType in _assembly.GetTypes())
			{
				try
				{
					if (_assType.IsClass && _assType.BaseType.FullName == "Rainbow.UI.WebControls.PortalModuleControl")
					{
					//	foreach (object _object in _assType.GetCustomAttributes(typeof(Rainbow.RainbowModuleAttribute),true))
					//	{
					//		if (_object.ToString() == "Rainbow.RainbowModuleAttribute")
					//		{
					//			_moduleAttribute = (Rainbow.RainbowModuleAttribute)_object;
					//			if ( _moduleAttribute.ModuleId == m_ModuleGuid)
					//				break;
					//			else
					//				_moduleAttribute = null;
					//		}
						}
					//	if (_moduleAttribute != null)
					//	{
							// Create an instance of the PortalModuleControl class and get the guid.
							_control = (Rainbow.UI.WebControls.PortalModuleControl)  Activator.CreateInstance(_assType );
						//	_control.LoadControl(Rainbow.Settings.Path.WebPathCombine("DesktopModules",m_moduleFolder,_moduleAttribute.DesktopSource));
							if  (_control.GuidID== m_ModuleGuid)
							{
								m_ClassName =  _assType.FullName;
								break;
							}
							else
							{
								//_moduleAttribute = null;
                                _control = null;
							}
				//		}
				//	}
				}
				catch (Exception ex)
				{
					_control  = null;
					//do something usefull
				}

			}
			if (_control == null)
				throw new InstallerException("could not find module with Id [" + m_ModuleGuid.ToString() +"] within assembly ["+m_AssemblyFileName + "].");
			//
			// get the properties needed to install a module

			m_ModuleGuid = _control.GuidID;
			m_IsAdminModule= _control.AdminModule;
			m_IsSearchable = _control.Searchable;
			m_ClassName = _control.ToString();

			_control.Dispose();
			_assembly = null ;

			// Check mobile module
			if (m_MobileControlSource != null && m_MobileControlSource != string.Empty && m_MobileControlSource.ToLower().EndsWith(".ascx"))
			{
				//Check mobile module
				if (!System.IO.File.Exists(PhysicalFolders.Modules + m_MobileControlSource))
					throw new InstallerException("Mobile Control file not found");
			}

			// Now we add the definition to module list 
			Rainbow.Configuration.ModulesDB _modules = new Rainbow.Configuration.ModulesDB();

			// check wether the module is currently installed or not
			if (_modules.GetSingleModuleDefinition(m_ModuleGuid).Read())
				throw new InstallerException("Module ["+ m_FriedlyName+"] with given id ["+ m_ModuleGuid.ToString()+ "] is currently installed. pleas uninstall first.");
			
			// Add a new module definition to the database
			_modules.AddGeneralModuleDefinitions(m_ModuleGuid, 
				m_FriedlyName, 
				Rainbow.Settings.Path.WebPathCombine("DesktopModules", m_ControlSource), 
				(m_MobileControlSource != string.Empty)? Rainbow.Settings.Path.WebPathCombine("DesktopModules", m_MobileControlSource) : string.Empty, 
				m_AssemblyFileName, 
				m_ClassName, 
				m_IsAdminModule, 
				m_IsSearchable);

			// Delete definition
			//_modules.DeleteModuleDefinition(m_ModuleGuid);
		}
		public override void Rollback()
		{
			// Now we add the definition to module list 
			Rainbow.Configuration.ModulesDB _modules = new Rainbow.Configuration.ModulesDB();
			// Delete definition
			_modules.DeleteModuleDefinition(m_ModuleGuid);
		}
		public override void Commit()
		{
			// nothing to do, we already have add the control.
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ExecuteSqlScript:ActionBase	
	{
		string m_SqlScript;
		string m_TransactionName ;
		System.Data.SqlClient.SqlTransaction m_Transaction;
		System.Data.SqlClient.SqlConnection m_Connection;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scriptFile"></param>
		public ExecuteSqlScript(string scriptFile)
		{
			if (System.IO.File.Exists(scriptFile))
			{
				System.IO.TextReader _reader = System.IO.File.OpenText(scriptFile);
				m_SqlScript = _reader.ReadToEnd();
				_reader.Close();
				m_SqlScript += Environment.NewLine;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		internal ExecuteSqlScript(byte[] bytes)
		{
			if (bytes.Length > 0)
			{
				System.Text.StringBuilder _sb= new System.Text.StringBuilder();
				foreach (byte _byte in bytes)
					_sb.Append(Convert.ToChar(_byte));
				
				_sb.Append (Environment.NewLine);
				m_SqlScript = _sb.ToString();
			}
		}

		public override void DoAction()
		{
			try
			{
				m_TransactionName = "RBI_" + DateTime.Now.ToString("yyyyMMddhhmmssfff");
				m_Connection = new SqlConnection( Rainbow.Settings.Portal.ConnectionString);
				m_Connection.Open();
				m_Transaction = m_Connection.BeginTransaction(IsolationLevel.RepeatableRead,m_TransactionName );
				
				ExecuteScript(m_SqlScript,m_Transaction);
				
				//Rainbow.Helpers.DBHelper.ExecuteScript(m_sqlScriptFile,m_Connection);
			}
			catch (Exception ex)
			{
				if  (m_Transaction != null)
					m_Transaction.Rollback();

				if (m_Connection.State == ConnectionState.Open)
					m_Connection.Close();
				throw;
			}
			finally
			{
			}
		}
		public override void Rollback()
		{
			try
			{
				if  (m_Transaction != null)
					m_Transaction.Rollback();
				if (m_Connection.State == ConnectionState.Open)
					m_Connection.Close();

			}
			catch (Exception ex)
			{
				if (m_Connection.State == ConnectionState.Open)
					m_Connection.Close();
				throw;
			}
		}

		public override void Commit()
		{
			try
			{
				if  (m_Transaction != null)
					m_Transaction.Commit();
				if (m_Connection.State == ConnectionState.Open)
					m_Connection.Close();
			}
			catch (Exception ex)
			{
				if (m_Connection.State == ConnectionState.Open)
					m_Connection.Close();
				throw;

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlScript"></param>
		/// <param name="transaction"></param>
		private void  ExecuteScript(string sqlScript, System.Data.SqlClient.SqlTransaction transaction)
		{
			// Subdivide script based on GO keyword
			string[] _sqlCommands = System.Text.RegularExpressions.Regex.Split(sqlScript, "\\sGO\\s", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			//Wraps execution on a transaction 
			//so we know that the script runs or fails
			//Cycles command and execute them
			for (int s = 0; s <= _sqlCommands.GetUpperBound(0); s++)
			{
				string _SqlText = _sqlCommands[s].Trim();
				try 
				{
					if (_SqlText.Length > 0)
					{
						using ( SqlCommand  sqldbCommand = new SqlCommand()) 
						{
							sqldbCommand.Connection = transaction.Connection;
							sqldbCommand.CommandType = CommandType.Text;
							sqldbCommand.Transaction = transaction;
							sqldbCommand.CommandText = _SqlText;
							sqldbCommand.CommandTimeout = 150;
							sqldbCommand.ExecuteNonQuery();
						}
					}
				} 
				catch(Exception ex) 
				{
					// do something usefull
					//transaction.Rollback();
					throw;
				}
			}
		}

	}
	/// <summary>
	/// 
	/// </summary>
	// TODO: implement UnregisterModule
	public sealed class UnregisterModule:ActionBase	
	{
		private string m_AssemblyFileName;
		private string m_FriedlyName ;
		private string m_ControlSource;
		private string m_MobileControlSource;
		private Guid 	m_ModuleGuid;
		private string 	m_ClassName;
		private bool m_IsAdminModule ;
		private bool m_IsSearchable;


		public UnregisterModule(string moduleGuid)
		{
		 m_ModuleGuid= new Guid(moduleGuid);
		}
		public override void DoAction()
		{
			// get the properties needed to install a module

			// Now we delete the definition 
			Rainbow.Configuration.ModulesDB _modules = new Rainbow.Configuration.ModulesDB();
			
			SqlDataReader _reader =  _modules.GetSingleModuleDefinition (m_ModuleGuid);
			if (_reader.Read())
			{
				m_FriedlyName	= _reader["FriendlyName"].ToString();
				m_ControlSource	= _reader["DesktopSrc"].ToString();
				m_MobileControlSource	= _reader["MobileSrc"].ToString();
				m_IsAdminModule	=  bool.Parse(_reader["Admin"].ToString());
				m_AssemblyFileName	= _reader["AssemblyName"].ToString();
				m_ClassName	= _reader["ClassName"].ToString();
				m_IsSearchable	= bool.Parse( _reader["Searchable"].ToString());
			}
			else
				throw new InstallerException ("Module with given Id ["+m_ModuleGuid.ToString()+"]is not registered.");

			// Delete definition
			_modules.DeleteModuleDefinition(m_ModuleGuid);
		}
		public override void Rollback()
		{
			//TODO: we must get the m_AssemblyFileName,m_ClassName.. some where may be add out put to used sp
			// we add the definition back to module list 
			Rainbow.Configuration.ModulesDB _modules = new Rainbow.Configuration.ModulesDB();
			// Add a new module definition to the database
			_modules.AddGeneralModuleDefinitions(m_ModuleGuid, 
				m_FriedlyName, 
				m_ControlSource, 
				m_MobileControlSource, 
				m_AssemblyFileName, 
				m_ClassName, 
				m_IsAdminModule, 
				m_IsSearchable);

		}
		public override void Commit()
		{
			//do nothing 
			//			// Now we add the definition to module list 
			//			Rainbow.Configuration.ModulesDB _modules = new Rainbow.Configuration.ModulesDB();
			//			// delete the module definition
			//			_modules.DeleteModuleDefinition(m_ModuleGuid);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class RenameFile:ActionBase	
	{
		string m_baseFolder;
		string m_SourceFile;
		string m_DestinationFile;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseFolder"></param>
		/// <param name="sourcFile"></param>
		/// <param name="destinationFile"></param>
		public RenameFile(string baseFolder, string sourcFile, string destinationFile)
		{
			m_baseFolder = baseFolder;
			m_SourceFile= sourcFile;
			m_DestinationFile= destinationFile;
		}
		public override void DoAction()
		{
			try
			{
				string _folder = GetFolderBase(m_baseFolder);
			
				if (! File.Exists (_folder + m_SourceFile ))
					throw new InstallerException("Rename File failed. Source file '"+_folder + m_SourceFile+"' does not exist!");

					File.Copy (_folder + m_SourceFile , _folder + m_SourceFile + ROLLBACK_EXTENSION);
					File.Move (_folder + m_SourceFile , _folder + m_DestinationFile);
			}
			catch (Exception ex)
			{
				throw new InstallerException("Error renaming file. ["+ m_baseFolder+ "|" + m_SourceFile +"]",ex);
			}
		}
		public override void Rollback()
		{
			string _folder = GetFolderBase(m_baseFolder);

			if (File.Exists (_folder + m_SourceFile + ROLLBACK_EXTENSION))
				File.Move(_folder + m_SourceFile + ROLLBACK_EXTENSION,_folder + m_SourceFile);

			if (File.Exists (_folder + m_DestinationFile))
				File.Delete (_folder + m_DestinationFile);
		}
		public override void Commit()
		{
			string _folder = GetFolderBase(m_baseFolder);

			if (File.Exists (_folder + m_SourceFile + ROLLBACK_EXTENSION ))
				File.Delete(_folder + m_SourceFile + ROLLBACK_EXTENSION);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class DeleteFolder:ActionBase	
	{
		private string m_baseFolder;
		private string m_folderName;
		public DeleteFolder(string baseFolder,string folderName)
		{
			m_baseFolder = baseFolder;
			m_folderName = folderName;
		}
		public override void DoAction()
		{
			try
			{
				string _folder = GetFolderBase(m_baseFolder);
			
				if (! Directory.Exists (_folder + m_folderName ))
					throw new InstallerException("Delete Folder failed. Folder '"+_folder + m_folderName+"' does not exist!");

				Directory.Move (_folder + m_folderName , _folder + m_folderName + ROLLBACK_EXTENSION);
			}
			catch (Exception ex)
			{
				throw new InstallerException("Error deleting folder. ["+ m_baseFolder+ "|" + m_folderName +"]",ex);
			}
		}
		public override void Rollback()
		{
				string _folder = GetFolderBase(m_baseFolder);
				Directory.Move(_folder + m_folderName + ROLLBACK_EXTENSION,_folder + m_folderName);
		}
		public override void Commit()
		{
			string _folder = GetFolderBase(m_baseFolder);
			if (Directory.Exists (_folder + m_folderName + ROLLBACK_EXTENSION ))
				Directory.Delete(_folder + m_folderName);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class DeleteFile:ActionBase	
	{
		private string m_FileName;
		private string m_BaseFolder;
		public DeleteFile(string baseFolder, string fileName)
		{
			m_BaseFolder = baseFolder ;
			m_FileName = fileName;
		}
		public override void DoAction()
		{
			try
			{
				string _folder = GetFolderBase(m_BaseFolder);
			
				if (! File.Exists (_folder + m_FileName ))
					throw new InstallerException("Deleting file failed. File '"+_folder + m_FileName+"' does not exist!");

				File.Move (_folder + m_FileName , _folder + m_FileName + ROLLBACK_EXTENSION);
			}
			catch (Exception ex)
			{
				throw new InstallerException("Error renaming file. ["+ m_BaseFolder+ "|" + m_FileName +"]",ex);
			}
		}
		public override void Rollback()
		{
			string _folder = GetFolderBase(m_BaseFolder);
			if (File.Exists (_folder + m_FileName + ROLLBACK_EXTENSION))
				File.Move(_folder + m_FileName + ROLLBACK_EXTENSION,_folder + m_FileName);
		}
		public override void Commit()
		{
			string _folder = GetFolderBase(m_BaseFolder);
			if (File.Exists (_folder + m_FileName + ROLLBACK_EXTENSION ))
				File.Delete(_folder + m_FileName + ROLLBACK_EXTENSION);
		}

	}
}
