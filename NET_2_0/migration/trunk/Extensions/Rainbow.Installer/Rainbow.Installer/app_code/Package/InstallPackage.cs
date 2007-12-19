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
using System.Collections;
using System.Xml;

namespace Rainbow.Installer.Package
{
	/// <summary>
	/// Summary description for InstallerFile.
	/// </summary>
	public class InstallPackage 
	{
		#region private variables
		private Hashtable m_HashTable ;
		private string m_InstallerDefinitionFileKey;
		private PackageInformation m_Information;
		private ArrayList m_ModuleList;

		#endregion

		#region internal properties
		/// <summary>
		/// 
		/// </summary>
		internal InstallPackage()
		{
			m_HashTable = new Hashtable();	
		}

		#endregion

		#region public properties
		/// <summary>
		/// 
		/// </summary>
		public PackageInformation Information
		{
			get
			{
				if (m_Information == null)
				{
					m_Information = new PackageInformation(InstallerDefinitionFile.DataStream);
				}
				return m_Information;
			}
		}	

		/// <summary>
		/// 
		/// </summary>
		public PackageFile InstallerDefinitionFile
		{
			get
			{
				return this[m_InstallerDefinitionFileKey];
			}
		}

		public ArrayList ModuleList
		{
			get
			{
				if (m_ModuleList== null)
				{
					m_ModuleList = new ArrayList();
					//load definition xml
					System.Xml.XmlDocument _xmlDoc = new System.Xml.XmlDocument();
					_xmlDoc.Load(InstallerDefinitionFile.DataStream);
			
					foreach(XmlNode _node in _xmlDoc.SelectNodes("installer/install/registerModule"))
						m_ModuleList.Add(_node.Attributes["friendlyName"].Value);
			
				}
			return m_ModuleList;
			}
		}

//		public ModuleList Modules
//		{
//			get
//			{
//				if (m_ModuleList == null)
//				{
//					m_ModuleList = new PackageInformation(InstallerDefinitionFile.DataStream);
//				}
//				return m_ModuleList;
//			}
//		}
		#endregion

		#region public methods
		public  void Add(PackageFile value)
		{
			if (value.FileName.EndsWith(".rbd"))
				m_InstallerDefinitionFileKey = value.FileName;

				m_HashTable.Add (value.FileName.Replace('\\','/'), value);
		}
		
		public PackageFile this[string key]
		{
			get
			{
				key = key.Replace('\\','/');
				if ( m_HashTable.ContainsKey(key))
					return (PackageFile) m_HashTable[key];
				else
					throw new Exception("Missing file ["+ key +"] in package.");
			}
			set
			{
				m_HashTable[key] = value;
			}
		}
		#endregion
	
	}
}
