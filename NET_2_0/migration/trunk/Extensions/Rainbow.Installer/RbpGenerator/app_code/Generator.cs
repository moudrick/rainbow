/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;

using System.IO;
using ICSharpCode.SharpZipLib.Zip;


namespace Rainbow.Installer.RbpGenerator
{
	/// <summary>
	/// RbpGenerator for Rainbow Portal Framework Installer Packages
	/// A straight forward quick and dirty application for generation the Rainbow Package file.
	/// 2005/01/10 Mario(at)Hartmann.net
	/// </summary>
	class ConsoleApplication
	{
	private static XmlDocument  m_xmlDoc = new XmlDocument();
	private static XmlElement  m_packageNode;
	private static XmlElement  m_installNode;
	private static XmlElement  m_uninstallNode;
	private static	Hashtable m_fileList = new Hashtable();
	private static	string m_rbpType;
	private static	string m_baseFolder = string.Empty;
		/// <summary>
		/// The main entry point for the RbpGenerator.
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		static void Main(string[] args)
		{
			string  _rbdFileName = string.Empty;
			bool generateRbd = true;

			m_baseFolder = string.Empty;

			// get the command parameter
			foreach(string _arg in args)
			{
				if  (_arg.ToLower() ==("-userbd"))
				{
					generateRbd = false;
				}
				else if (_arg.ToLower().StartsWith("-base:"))
				{
					m_baseFolder = _arg.ToLower().Substring(6);
				}
			}			
			//
			//
			WriteIntro();
			//
			//
			if (args.Length == 0 )
				WriteUsageInfo();
			//
			//get the basefolder
			if 	(m_baseFolder == string.Empty)
			{
				Console.Write("Packege source folder:");
				m_baseFolder = Console.ReadLine ();
			}
			//
			// set rbd file 
			if (Directory.GetFiles(m_baseFolder,"*.rbd").Length == 1)
				_rbdFileName = Directory.GetFiles(m_baseFolder,"*.rbd")[0];
			else if (Directory.GetFiles(m_baseFolder,"*.rbd").Length > 1)
			{
				Console.WriteLine ("Multiple Definitionfiles found in source folder ["+m_baseFolder+"]. exiting...");
				Console.ReadLine();
				return ;
			}
			else
			{
				_rbdFileName = m_baseFolder+ m_baseFolder.Substring (m_baseFolder.LastIndexOf(@"\")) +".rbd";
			}
			//
			//
			if (! (File.Exists(_rbdFileName) &&  generateRbd == false))
			{
				//
				PrepareRbdFile(_rbdFileName);
				//
				//set the global xml nodes
				m_packageNode	=	m_xmlDoc.DocumentElement["package"];
				m_installNode	=	m_xmlDoc.DocumentElement["install"];
				m_uninstallNode	=	m_xmlDoc.DocumentElement["uninstall"];
				//
				AddPackageInfos();
				//
				PrepareFileList(m_baseFolder);
				//
				AddFolderList();
				//
				AddFileList();
				//
				AddSqlScriptList();
				//
				AddControlList();
				//
				ReverseUninstallNode();
				// now save the new xmldoument
				m_xmlDoc.Save(_rbdFileName);
				//
			}
			// get the rbd file info bject
			if (!File.Exists(_rbdFileName))
			{
				Console.WriteLine("Rainbow Installer Definition file ["+_rbdFileName+"] does not exist."); 
				return ;
			}
			//
			//
			PackageHelper.GeneratePackage(new FileInfo(_rbdFileName));
			//
			WriteExtro();
			//
			//wait for exit.
			Console.ReadLine();
		}
		
		#region Main functions
		/// <summary>
		/// write usage info info to the screen
		/// </summary>
		private static void WriteUsageInfo()
		{
			System.Text.StringBuilder _sb = new System.Text.StringBuilder();
			_sb.Append(Environment.NewLine);
			_sb.Append("Usage: rbpGenerator.exe [-base:<directory>] [-userbd]" + Environment.NewLine);
			_sb.Append(Environment.NewLine);
			_sb.Append("<directory> directory path to package source folder"+ Environment.NewLine);
			_sb.Append("-userbd use existing definitionfile in sourcefolder "+ Environment.NewLine);
			_sb.Append( Environment.NewLine +  Environment.NewLine);
			Console.WriteLine(_sb.ToString());

		}
		/// <summary>
		/// write intro info to the screen
		/// </summary>
		private static void WriteIntro()
		{
			System.Text.StringBuilder _sb = new System.Text.StringBuilder();
			_sb.Append("*******************************************" + Environment.NewLine);
			_sb.Append("**     Rainbow PackageFile Generator     **"+ Environment.NewLine);
			_sb.Append("**              coded by                 **"+ Environment.NewLine);
			_sb.Append("**        mario(at)hartmann.net          **"+ Environment.NewLine);
			_sb.Append("**      http://mario.hartmann.net        **"+ Environment.NewLine);
			_sb.Append("*******************************************"+ Environment.NewLine);
			Console.WriteLine(_sb.ToString());

		}
		/// <summary>
		/// prepare the rbp file
		/// </summary>
		/// <param name="rbdFileName"></param>
		private static void PrepareRbdFile(string rbdFileName)
		{
			//
			// get or create the rbd file
			if (File.Exists(rbdFileName))
			{
				m_xmlDoc.Load(rbdFileName);
			}
			else // file does not exist, so create rootelement 
			{
				XmlElement   _installerNode =  m_xmlDoc.CreateElement("installer"); //,"http://schemas.mhsl.net/Rainbow/rbd.2.0.xsd");
				m_xmlDoc.AppendChild( _installerNode);
				SetAttribute(_installerNode,"version","2.0");
			}
			//
			// set the used nodes
			if (m_xmlDoc.DocumentElement["package"]== null)
				 m_xmlDoc.DocumentElement.AppendChild(m_xmlDoc.CreateElement("package"));

			
			if (m_xmlDoc.DocumentElement["install"]!= null)
				 m_xmlDoc.DocumentElement.RemoveChild( m_xmlDoc.DocumentElement["install"]);

			m_xmlDoc.DocumentElement.AppendChild(m_xmlDoc.CreateElement("install"));


			if (m_xmlDoc.DocumentElement["uninstall"]!= null)
				m_xmlDoc.DocumentElement.RemoveChild( m_xmlDoc.DocumentElement["uninstall"]);

				m_xmlDoc.DocumentElement.AppendChild(m_xmlDoc.CreateElement("uninstall"));

			m_xmlDoc.Save(rbdFileName);
		}
		/// <summary>
		/// add the package information to the package nnode of rbd file
		/// </summary>
		private static void AddPackageInfos()
		{
			Console.WriteLine("Package Information");
			GetAttribute (m_packageNode,"caption");
			GetAttribute (m_packageNode,"description");
			GetAttribute (m_packageNode,"version" );
			GetAttribute (m_packageNode,"author");
			GetAttribute (m_packageNode,"informationUrl");
			//get the package type
			string m_rbpType =  (m_packageNode.Attributes["type"]==null)? "module": m_packageNode.Attributes["type"].Value;
			Console.Write("type of package [(m)odule,(t)heme,(l)ayout]:("+ m_rbpType +")");
			string _answer = Console.ReadLine();
			switch(_answer.ToLower())
			{
				case "l":
					m_rbpType = "layout";
					break;
				case "t":
					m_rbpType = "theme";
					break;
				default: //case "m":
					m_rbpType = "module";
					break;
			}
			SetAttribute(m_packageNode,"type",m_rbpType);
			SetAttribute (m_packageNode,"isBeta","true");

			XmlAttribute _attribute = m_packageNode.Attributes["id"];
			if (_attribute == null)
				SetAttribute (m_packageNode,"id",Guid.NewGuid().ToString());
			else
				SetAttribute (m_packageNode,"id",  _attribute.Value);

		}

		/// <summary>
		/// get the folder and files below the given path and add them into a hashtable
		/// </summary>
		/// <param name="folder">parentfolder</param>
		private static void PrepareFileList(string folder)
		{
			System.IO.DirectoryInfo _dirInfo = new System.IO.DirectoryInfo(folder);
			GetFileList (_dirInfo);
		}

		/// <summary>
		/// add the folders to create & delete to the install/uninstall node
		/// </summary>
		private static void AddFolderList()
		{
			string _moduleFolder = m_baseFolder.Substring(m_baseFolder.LastIndexOf(@"\")+1);

			Hashtable _folders = new Hashtable();

			foreach(System.IO.FileInfo  _fileInfo in m_fileList.Keys)
			{
				string _folderName =  _moduleFolder + _fileInfo.DirectoryName.Substring(m_baseFolder.Length);

				if (_folders.ContainsKey(_folderName)!= true  && _folderName != string.Empty )
					_folders.Add(_folderName,m_fileList[_fileInfo]);
			}

			foreach(string  _string in _folders.Keys)
			{
				XmlElement _createElement = m_xmlDoc.CreateElement("createFolder");
				XmlElement _deleteElement = m_xmlDoc.CreateElement("deleteFolder");

					
				SetAttribute(_createElement,"folderName",_string);
				SetAttribute(_deleteElement,"folderName",_string);
			

				SetAttribute(_createElement,"baseFolder",(string)_folders[_string]);
				SetAttribute(_deleteElement,"baseFolder",(string)_folders[_string]);

				m_installNode.AppendChild((XmlNode)_createElement);
				m_uninstallNode.AppendChild((XmlNode)_deleteElement);

			}

		}

		/// <summary>
		/// add files to the install/uninstall node
		/// </summary>
		private static void AddFileList()
		{
			string _moduleFolder = m_baseFolder.Substring(m_baseFolder.LastIndexOf(@"\")+1);

			foreach(System.IO.FileInfo  _fileInfo in m_fileList.Keys)
			{
				XmlElement _createFileElement = m_xmlDoc.CreateElement("createFile");
				XmlElement _deleteFileElement = m_xmlDoc.CreateElement("deleteFile");

				SetAttribute(_createFileElement,"fileName", _fileInfo.FullName.Substring(m_baseFolder.Length+1));
				SetAttribute(_deleteFileElement,"fileName",_moduleFolder +@"\"+ _fileInfo.FullName.Substring(m_baseFolder.Length+1));

				if (m_fileList[_fileInfo].ToString().ToLower() =="uninstall" ||
					m_fileList[_fileInfo].ToString().ToLower() =="bin" )
				{
					SetAttribute(_createFileElement,"destinationFile",_fileInfo.Name);
					SetAttribute(_createFileElement,"destinationBaseFolder",m_fileList[_fileInfo].ToString().ToLower());
					SetAttribute(_deleteFileElement,"baseFolder",m_fileList[_fileInfo].ToString().ToLower());
				}
				else
				{
					SetAttribute(_createFileElement,"destinationFile",_moduleFolder +@"\"+ _fileInfo.FullName.Substring(m_baseFolder.Length+1));
					SetAttribute(_createFileElement,"destinationBaseFolder",(string)m_fileList[_fileInfo]);
					SetAttribute(_deleteFileElement,"baseFolder",(string)m_fileList[_fileInfo]);
				}

				m_installNode.AppendChild((XmlNode)_createFileElement);
				m_uninstallNode.AppendChild((XmlNode)_deleteFileElement);
			}

		}

		/// <summary>
		/// add scripts to run during installation/uninstallation to install/uninstallnode
		/// </summary>
		private static void AddSqlScriptList()
		{
			string _answer;
			XmlElement _element;
			Console.WriteLine("******* SQL scripts");
			foreach(System.IO.FileInfo  _fileInfo in m_fileList.Keys)
			{
				if (_fileInfo.Extension.ToLower()== ".sql")
				{
					Console.Write("add file {"+ _fileInfo.Name +"} as SQL script! [(I)nstall,(U)nistall,(N)o]");
					_answer = Console.ReadLine();
					if (_answer.ToLower() =="i")
					{
						_element = m_installNode.OwnerDocument.CreateElement("executeSqlScript");
						m_installNode.AppendChild( _element);
						SetAttribute(_element, "scriptFile", _fileInfo.Name);
					}
					else if (_answer.ToLower() == "u" )
					{
						_element = m_uninstallNode.OwnerDocument.CreateElement("executeSqlScript");
						m_uninstallNode.AppendChild( _element);
						SetAttribute(_element, "scriptFile", _fileInfo.Name);
					}
				}
			}
		}

		/// <summary>
		/// add list of rainbow portal modules to register or unregister
		/// </summary>
		private static void AddControlList ()
		{
			string _answer;
			string _moduleFolder = m_baseFolder.Substring(m_baseFolder.LastIndexOf(@"\")+1);

			Console.WriteLine("******* user controls");
			foreach(System.IO.FileInfo  _fileInfo in m_fileList.Keys)
			{
				if (_fileInfo.Extension.ToLower() ==".ascx")
				{
					Console.Write("add file {"+ _fileInfo.Name +"} as rainbow module to register ? [(Y)es,(N)o]");
					_answer = Console.ReadLine();
					Console.WriteLine();
					if (_answer.ToLower() == "y" )
					{
						XmlElement _registerElement = m_xmlDoc.CreateElement("registerModule");
						SetAttribute(_registerElement, "desktopControlSource",_moduleFolder +@"\"+ _fileInfo.Name);
						GetAttribute(_registerElement, "friendlyName",m_packageNode.Attributes["caption"].Value);
						GetAttribute(_registerElement, "mobileControlSource");
                        GetAttribute(_registerElement, "assemblyFileName");

						//string moduleGuid = GetModuleGuid(m_fileList[_registerElement.Attributes["assemblyFileName"]]);
						//if (moduleGuid != string.Empty)
                        //   SetAttribute(_registerElement, "ModuleGuid",moduleGuid);
						//else
							GetAttribute(_registerElement, "moduleGuid");


						m_installNode.AppendChild((XmlNode)_registerElement);

						XmlElement _unRegisterElement = m_xmlDoc.CreateElement("unregisterModule");
						SetAttribute(_unRegisterElement, "moduleGuid",_registerElement.Attributes["moduleGuid"].Value);
						m_uninstallNode.AppendChild((XmlNode)_unRegisterElement);
					}
				}
			}
		}

		/// <summary>
		///  to reverse the node order to ensure correct working unsinstall routine
		/// </summary>
		private static void ReverseUninstallNode()
		{
			XmlElement _currentUninstall = m_uninstallNode;
			XmlElement _reversedUninstall = m_xmlDoc.CreateElement("uninstall");
			XmlNode  _node;
			 _node = _currentUninstall.LastChild;

			while(_node != null)
			{
				_reversedUninstall.AppendChild(_node.Clone());
				_node = _node.PreviousSibling;
			}
			m_xmlDoc.DocumentElement.RemoveChild(_currentUninstall);
			m_xmlDoc.DocumentElement.AppendChild(_reversedUninstall);
			m_uninstallNode = _reversedUninstall;

		}
	
		/// <summary>
		/// write extro info to the screen
		/// </summary>
		private static void WriteExtro()
		{
			System.Text.StringBuilder _sb = new System.Text.StringBuilder();
			_sb.Append("*******************************************" + Environment.NewLine);
			_sb.Append("**     Rainbow PackageFile Generator     **"+ Environment.NewLine);
			_sb.Append("**  ready, finished generating package   **"+ Environment.NewLine);
			_sb.Append("*******************************************"+ Environment.NewLine);
			_sb.Append("press enter for exit!"+ Environment.NewLine);

			Console.Write(_sb.ToString());

		}
	

		#region some helper functions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_xmlElement"></param>
		/// <param name="attribName"></param>
		/// <param name="attribValue"></param>
		private static void SetAttribute (XmlElement  _xmlElement, string attribName, string attribValue)
		{
			XmlAttribute _attribute = _xmlElement.Attributes[attribName];
			if (_attribute == null)
			{
				_attribute = m_xmlDoc.CreateAttribute(attribName);
				_xmlElement.Attributes.Append(_attribute);
			}
			_attribute.Value =attribValue;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="attribName"></param>
		private static void GetAttribute (XmlElement  xmlElement, string attribName)
		{
			string _value = (xmlElement.Attributes[attribName]==null)? string.Empty: xmlElement.Attributes[attribName].Value;
			GetAttribute (xmlElement,attribName,_value);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="attribName"></param>
		/// <param name="defaultValue"></param>
		private static void GetAttribute (XmlElement  xmlElement, string attribName,string defaultValue)
		{
			XmlAttribute _atribute;

			if (xmlElement.Attributes[attribName] == null)
			{
				_atribute = m_xmlDoc.CreateAttribute(attribName);
				xmlElement.Attributes.Append (_atribute);
			}
			else
				_atribute = xmlElement.Attributes[attribName];
            
			Console.Write(attribName+":["+ defaultValue +"]");

			string _tmpValue = Console.ReadLine();
			if (_tmpValue == string.Empty)
				_atribute.Value = defaultValue ;
			else
				_atribute.Value = _tmpValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dirInfo"></param>
		private static void GetFileList(System.IO.DirectoryInfo dirInfo)
		{
			AddFilesToList (dirInfo);

			foreach (System.IO.DirectoryInfo _subDirInfo in dirInfo.GetDirectories())
			{
				if (!(_subDirInfo.Name.ToLower() == "cvs" || 
					_subDirInfo.Name.ToLower() == "save" || 
					_subDirInfo.Name.ToLower() == "obj" ))
				{
					GetFileList (_subDirInfo );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dirInfo"></param>
		private static void AddFilesToList(System.IO.DirectoryInfo dirInfo)
		{
			string _answer;
			foreach (System.IO.FileInfo  _fileInfo in dirInfo.GetFiles())
			{
				if (_fileInfo.Extension.ToLower() == ".rbd")
				{
					m_fileList.Add(_fileInfo,"uninstall");
				}
				else if (_fileInfo.Extension.ToLower() == ".dll" || 
					_fileInfo.Extension.ToLower() == ".pdb")
				{
					m_fileList.Add(_fileInfo,"bin");
				}
				else if ( !(_fileInfo.Extension.ToLower() == ".cs"  || 
					_fileInfo.Extension.ToLower() == ".user" ||
					_fileInfo.Extension.ToLower() == ".csproj"||
					_fileInfo.Extension.ToLower() == ".resx"||
					_fileInfo.Extension.ToLower() == ".rbp"||
					_fileInfo.Extension.ToLower() == ".sln" ||
					_fileInfo.Extension.ToLower() == ".suo" ))
				{

					string _fileTmp = _fileInfo.FullName.Remove(0,m_baseFolder.Length);
					Console.Write("add {"+ _fileTmp +"} to list ? [(m)odule,(l)ayout,(t)heme,(n)o]:");
					_answer =  Console.ReadLine();
					switch (_answer.ToLower())
					{
						case "m": m_fileList.Add(_fileInfo,"modules");break;
						case "l": m_fileList.Add(_fileInfo,"layouts");break;
						case "t": m_fileList.Add(_fileInfo,"themes");break;
					}
				}
			}
		}

		#endregion

		#endregion
	}
}
