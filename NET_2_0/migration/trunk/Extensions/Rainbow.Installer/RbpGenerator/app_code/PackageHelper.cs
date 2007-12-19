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
using System.Text;
using ICSharpCode.SharpZipLib.Zip;


namespace Rainbow.Installer.RbpGenerator
{
	/// <summary>
	/// PackageHelper for Rainbow Installer Package Generator
	/// </summary>
	public class PackageHelper
	{
		public PackageHelper()
		{
		}


		/// <summary>
		///  generate the packagefile (rbp file)
		/// </summary>
		/// <param name="_definitionFileInfo">The FileInfo object of the Rainbow Installer DefinitionFile</param>
		public static void GeneratePackage (FileInfo _definitionFileInfo)
		{
			XmlDocument _xmlDoc ;
			XmlElement _installNode ;
			DirectoryInfo  _packageDirInfo ;
			string  _packageFileName;

		
			_xmlDoc = new XmlDocument();
			_xmlDoc.Load(_definitionFileInfo.FullName);
			_installNode = _xmlDoc.DocumentElement["install"];
			
			_packageDirInfo = _definitionFileInfo.Directory;
			_packageFileName = Path.Combine(_packageDirInfo.FullName , _packageDirInfo.Name +".rbp");

			if (File.Exists(_packageFileName))
				File.Delete(_packageFileName);


			// create zipfile
			FileStream _packageFileStream  =File.Create(_packageFileName);
			ZipOutputStream _zipOStream = new ZipOutputStream(_packageFileStream);

			// set zip properties
			_zipOStream.SetLevel(9);
			_zipOStream.SetComment(GetPackageComment(_xmlDoc.DocumentElement["package"]));

			// iterate though the definition file and add files to zip
			foreach(XmlNode _node in _installNode.ChildNodes)
			{
				if (_node.Name =="createFile")
				{
					FileInfo _fileInfo = new FileInfo(Path.Combine( _packageDirInfo.FullName,  _node.Attributes["fileName"].Value));
					FileStream _fileStream = File.OpenRead( _fileInfo.FullName );
                
					byte[] _buffer = new byte[_fileStream.Length];
					_fileStream.Read(_buffer , 0, _buffer.Length);
					_fileStream.Close();
	                    
					//now add the file
					ZipEntry _zipEntry = new ZipEntry(_fileInfo.FullName.Substring(_packageDirInfo.FullName.Length+1).Replace('\\','/'));
					_zipOStream.PutNextEntry(_zipEntry);
					_zipOStream.Write(_buffer , 0, _buffer.Length);
					//FileSystemEventArgs args = new FileSystemEventArgs( WatcherChangeTypes.Created, di.FullName, fi.Name );
				}
			} 
			_zipOStream.Finish();
			_zipOStream.Close();
			_packageFileStream.Close();	
		}


		/// <summary>
		/// create containing the comment text for the package zip file.
		/// </summary>
		/// <param name="packageInfo"></param>
		/// <returns>Returns a string </returns>
		private static string GetPackageComment(XmlNode packageInfo)
		{
			StringBuilder _sb = new StringBuilder();
			_sb.Append( "Rainbow Installer Package" +Environment.NewLine);
			_sb.Append( "-------------------------" +Environment.NewLine );
			_sb.Append(" this Package was generated with Rainbow Installer Pachagebuilder [ver.");
			_sb.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
			_sb.Append("]" +Environment.NewLine);
			_sb.Append(Environment.NewLine);
			_sb.Append("Package Information:" +Environment.NewLine);
			_sb.Append("Name:" + packageInfo.Attributes["caption"].Value +Environment.NewLine);
			_sb.Append("Description:" + packageInfo.Attributes["description"].Value +Environment.NewLine);
			_sb.Append("Version:" + packageInfo.Attributes["version"].Value +Environment.NewLine);
			_sb.Append("created:" + DateTime.Now.ToString("dd MMMM yyyy") +Environment.NewLine);
			_sb.Append("Author:" + packageInfo.Attributes["author"].Value +Environment.NewLine);
			_sb.Append("Information Url:" + packageInfo.Attributes["informationUrl"].Value +Environment.NewLine);

			
			_sb.Append("* " +Environment.NewLine);
			_sb.Append("* THE PACKAGE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED" +Environment.NewLine); 
			_sb.Append("* TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL " +Environment.NewLine);
			_sb.Append("* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF " +Environment.NewLine);
			_sb.Append("* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE PACKAGE OR THE USE OR OTHER " +Environment.NewLine);
			_sb.Append("* DEALINGS IN THE PACKAGE." +Environment.NewLine);

			return _sb.ToString();
		}

		
		#region unused code :-), may be useful sometime
		//		private static string GetModuleGuid(string assemblyFileName,string controlFileName)
		//		{
		//			// get information about the module
		//			System.Reflection.Assembly _assembly =  System.Reflection.Assembly.LoadFrom(PhysicalFolders.Binaries + m_AssemblyFileName);
		//			// Get the type to use from the assembly.
		//			foreach (Type _assType in _assembly.GetTypes())
		//			{
		//				try
		//				{
		//					if (_assType.IsClass && _assType.BaseType.FullName == "Rainbow.UI.WebControls.PortalModuleControl")
		//					{
		//						// Create an instance of the PortalModuleControl class.
		//						_control = (Rainbow.UI.WebControls.PortalModuleControl)  Activator.CreateInstance(_assType );
		//						_control.LoadControl(controlFileName);
		//						break;
		//					}
		//				}
		//				catch (Exception ex)
		//				{
		//					_control = null;
		//					//do something usefull
		//				}
		//
		//			}
		//			if (_control == null)
		//				throw new InstallerException("could not find '"+ m_ControlSource +"' in assembly '"+m_AssemblyFileName + "'.");
		//
		//			//
		//			// get the properties needed to install a module
		//
		//			m_ModuleGuid = _control.GuidID;
		//		}
		#endregion
	}
}
