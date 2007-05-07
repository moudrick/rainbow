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
	public class PackageInformation 
	{
		public PackageInformation(string packageInformationFile)
		{
			if (File.Exists(packageInformationFile))
			{
				Stream stream= File.OpenRead(packageInformationFile);
				this.Load(stream);
				stream.Close();
			}
			else
				throw new Exception("no package information file at ["+ packageInformationFile +"].");

		}
		internal PackageInformation(Stream stream)	
		{
			this.Load(stream);				
		}

		private void Load(Stream stream)
		{
			//load definition xml
			System.Xml.XmlDocument _xmlDoc = new System.Xml.XmlDocument();
			_xmlDoc.Load(stream);
			
			XmlElement  _packageElement = (XmlElement)_xmlDoc.DocumentElement.GetElementsByTagName("package").Item(0);

			Identifier= _packageElement.GetAttribute("id");
			Version= _packageElement.GetAttribute("version");
			Caption = _packageElement.GetAttribute("caption");
			Author = _packageElement.GetAttribute("author");
			IsBeta = bool.Parse(_packageElement.GetAttribute("isBeta"));
			Description = _packageElement.GetAttribute("description");
			InformatioUrl = _packageElement.GetAttribute("informationUrl");
			PackageType = _packageElement.GetAttribute("type");					
		}


		public string Identifier;
		public string Version;
		public string Caption;
		public string Description;
		public string Author;
		public string InformatioUrl;
		public string PackageType;
		public bool  IsBeta;

	}
}
