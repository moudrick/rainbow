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
	public class PackageFile 
	{
		private string m_FileName ;
		private byte[] m_FileBuffer;
		public PackageFile (string fileName,byte[] fileBuffer)
		{
			m_FileBuffer= fileBuffer;
			m_FileName  = fileName;
		}
		public string FileName
		{
			get
			{
				return m_FileName;
			}
		}

		public byte[] FileBuffer
		{
			get
			{
				return  m_FileBuffer ;
			}
		}
		public Stream DataStream
		{
			get
			{
				return new MemoryStream(m_FileBuffer);
			}
		}
	}

}
