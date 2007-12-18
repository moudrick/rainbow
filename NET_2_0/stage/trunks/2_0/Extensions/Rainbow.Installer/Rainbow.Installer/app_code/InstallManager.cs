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
using  System;	
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using  ICSharpCode.SharpZipLib.Zip;

using Rainbow.Installer.Log;
using Rainbow.Installer.Package;

namespace Rainbow.Installer 
{

	public class InstallManager 
	{
		#region private variables
		private InstallPackage m_installPackage;
		private LogList m_Log = new LogList();
		#endregion

		#region constructors
		public InstallManager(System.IO.Stream installPackageStream)
		{
			this.LoadPackage(installPackageStream);
		}
		
		public InstallManager(string installPackageFile)
		{
			if (File.Exists(installPackageFile))
			{
				FileStream _fileStream = File.OpenRead(installPackageFile);
				try
				{
					this.LoadPackage(_fileStream);
					_fileStream.Close();
				}
				catch (Exception ex )
				{
					_fileStream.Close();
					throw;
				}
			}
		}

		#endregion

		#region public 
		public LogList  Log {get{return  m_Log;}}
		public void Install()
		{
			if(!RunPackageNode("install"))
				throw new InstallerException("Install failed!");
		}		

		public void Uninstall()
		{
			if (!RunPackageNode("uninstall"))
				throw new InstallerException("Uninstall failed!");
		}
		public InstallPackage Package
		{
			get
			{
				return m_installPackage;
			}
		}

		#endregion

		#region private functions
		private void LoadPackage(Stream installStream)
		{
			m_installPackage = new InstallPackage();
			ZipInputStream _zipStream;

			if (installStream != null)
				_zipStream = new ZipInputStream(installStream);
			else
				throw new InstallerFileCorruptException();


			ZipEntry _entry = _zipStream.GetNextEntry();

			while (_entry != null )
			{
				if (! _entry.IsDirectory)
				{
					byte [] _buffer = new byte[_entry.Size];
					int _size = 0;
					while (_size < _buffer.Length)
					{
						_size += _zipStream.Read(_buffer, _size, _buffer.Length - _size);
					}     
					if (_size != _buffer.Length )
						throw new Exception("Could not read all the data: " + _buffer.Length + "/" + _size);

					m_installPackage.Add(new PackageFile(_entry.Name,_buffer));
				}
				_entry = _zipStream.GetNextEntry() ;
			}
		}

		private bool RunPackageNode(string xmlNodeToRun)
		{
			#region validate package definition
			//
			// validate .rbb file
			DefinitionValidator _Validator = new DefinitionValidator(this.Package.InstallerDefinitionFile.DataStream);
			if (! _Validator.IsValid())
			{
				m_Log.AddRange(_Validator.Errors);		
				return false;
			}
			#endregion

			#region prepare actionlist
			//load definition xml
			System.Xml.XmlDocument _xmlDoc = new System.Xml.XmlDocument();
			_xmlDoc.Load(this.Package.InstallerDefinitionFile.DataStream);

			//
			//prepare action list
			Action.ActionList	_actionList = new Action.ActionList();
			
			//
			//get the NodeList (i.e. Install or Uninstall)
			try
			{
				foreach (XmlNode _node in _xmlDoc.DocumentElement.GetElementsByTagName(xmlNodeToRun))
				{
					foreach (XmlNode _actionNode in _node.ChildNodes)
					{
						string _folderName ;
						string _fileName ;
						string _controlSource;
						string _guid ;
						string _baseFolder ;
						try
						{
							switch (_actionNode.Name)
							{
								case "createFolder":
									_folderName = GetAttributeValue ("folderName",_actionNode);
									_baseFolder = GetAttributeValue ("baseFolder",_actionNode);
									_actionList.Add(new Action.CreateFolder(_baseFolder,_folderName));
									break;
								case "createFile":
									_fileName = GetAttributeValue ("fileName",_actionNode); 
									string _destinationFileName = GetAttributeValue ("destinationFile",_actionNode,_fileName); 
									_baseFolder = GetAttributeValue ("destinationBaseFolder",_actionNode); 
									_actionList.Add(new Action.CreateFile(Package[_fileName].FileBuffer,_baseFolder,_destinationFileName));
									break;
								case "copyFile":
									_fileName = GetAttributeValue ("sourceFile",_actionNode); 
									_baseFolder = GetAttributeValue ("sourceBaseFolder",_actionNode); 
									string _destinationFile = GetAttributeValue ("destinationFile",_actionNode,_fileName); 
									string _destinationBaseFolder =GetAttributeValue ("destinationBaseFolder",_actionNode,_baseFolder); 
								
									_actionList.Add(new Action.CopyFile(_baseFolder,_fileName,_destinationBaseFolder ,_destinationFile));
									break;
								case "registerModule":
									string _friendlyName = GetAttributeValue ("friendlyName",_actionNode);  
									_controlSource = GetAttributeValue ("desktopControlSource",_actionNode); 
									string _mobileControlSource = GetAttributeValue ("mobileControlSource",_actionNode); 
									string _assemblyFileName = GetAttributeValue ("assemblyFileName",_actionNode); 
									_guid = GetAttributeValue ("moduleGuid",_actionNode); 
									_actionList.Add(new Action.RegisterModule(_assemblyFileName,_guid, _friendlyName,_controlSource,_mobileControlSource));
									break;
								case "executeSqlScript":
									_fileName = GetAttributeValue ("scriptFile",_actionNode);
									_actionList.Add(new Action.ExecuteSqlScript(Package[_fileName].FileBuffer));
									break;
								case "renameFile":
									_fileName = GetAttributeValue ("fileName",_actionNode);
									string _newName =GetAttributeValue ("newName",_actionNode); 
									_baseFolder = GetAttributeValue ("baseFolder",_actionNode); 
									_actionList.Add(new Action.RenameFile(_baseFolder,_fileName,_newName));
									break;
								case "deleteFile":
									_baseFolder = GetAttributeValue ("baseFolder",_actionNode); 
									_fileName = GetAttributeValue ("fileName",_actionNode); 
									_actionList.Add(new Action.DeleteFile(_baseFolder,_fileName));
									break;
								case "deleteFolder":
									_baseFolder = GetAttributeValue ("baseFolder",_actionNode); 
									_folderName = GetAttributeValue ("deleteFolder",_actionNode);
									_actionList.Add(new Action.DeleteFolder (_baseFolder,_folderName));
									break;
								case "unregisterModule":
									_guid = GetAttributeValue ("moduleGuid",_actionNode); 
									_actionList.Add(new Action.UnregisterModule(_guid));
									break;
								default:
									throw new InstallerException("unknonw action in installer file!");
									break;
							}
						}
						catch (Exception ex)
						{
							m_Log.Add(InstallerLogType.Error,ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				m_Log.Add(InstallerLogType.Error,ex.Message);
			}
			finally
			{
			}
		
			#endregion

			#region run actions
			//
			//run actions
			if (! m_Log.HasErrors)
			{
				_actionList.Run();
				m_Log.AddRange(_actionList.Log.ToArray());
			}

			#endregion

			return (!m_Log.HasErrors);
		}
		private string GetAttributeValue(string valueName , XmlNode node)
		{
			return GetAttributeValue (valueName,node,null);
		}
		private string GetAttributeValue(string valueName , XmlNode node, string defaultValue)
		{
			XmlAttribute  _attribute =  node.Attributes[valueName];
			if (_attribute == null)
			{
				if (defaultValue == null)
					throw new Exception("Attribute ["+ valueName +"] is not defined !");
				else
					return defaultValue;
			}
			else
				return _attribute.Value;
		}

		#endregion
	}
}