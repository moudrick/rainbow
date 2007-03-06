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
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Schema;

using  ICSharpCode.SharpZipLib.Zip;

using Rainbow.Installer.Log;


namespace Rainbow.Installer
{
	/// <summary>
	/// Summary description for DefinitionValidator.
	/// </summary>
	public class DefinitionValidator
	{
		private Stream m_DefinitionStream;
		private Log.LogList m_ErrorLog  = new Log.LogList();
		public Log.LogList Errors
		{
			get
			{
				return m_ErrorLog;
			}
		}
		
		public DefinitionValidator(Stream definitionStream)
		{
			m_DefinitionStream = definitionStream;
		}

		public  bool IsValid()
		{
			//return true;

			FileStream _schemaStream =null;
			try
			{

				XmlTextReader _xmlReader = new XmlTextReader(this.m_DefinitionStream);
				//Create a validating reader
				XmlValidatingReader _vReader ;
				_vReader = new XmlValidatingReader(_xmlReader);

            
				string _schemaFileName =  PhysicalFolders.Application + @"\app_support\Installer\rbd.2.0.xsd";
				_schemaStream = File.OpenRead(_schemaFileName);
				XmlSchema _xmlSchema =   XmlSchema.Read(_schemaStream,null);
				_schemaStream.Close();
			
				_vReader.Schemas.Add(_xmlSchema);
				_vReader.ValidationType = ValidationType.Schema ;

				//Set the validation event handler.
				_vReader.ValidationEventHandler+=new ValidationEventHandler(_vReader_ValidationEventHandler);

				//Read and validate the XML data.
				while(_vReader.Read())
				{}

				//Close the reader.
				_vReader.Close();

			}
			catch (Exception ex)
			{
				if (_schemaStream!= null)
					_schemaStream.Close();
				throw;
			}

			return (m_ErrorLog.Count == 0);
		}

		private void _vReader_ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			m_ErrorLog.Add(InstallerLogType.Info,e.Message);
		}

	}
}
