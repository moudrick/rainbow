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

namespace Rainbow.Installer.Log
{
	/// <summary>
	/// Summary description for LogEntry.
	/// </summary>
	public class LogEntry
	{
		public LogEntry (InstallerLogType logType,string description):this(logType,description,null){}
		public LogEntry(InstallerLogType logType,string description,Exception exception)
		{
			m_LogType = logType;
			m_Description = description;
			m_TimeStamp = DateTime.Now;
			m_InnerException = exception;
		}	
		private InstallerLogType m_LogType;
		private string m_Description;
		private DateTime m_TimeStamp;
		private Exception m_InnerException;

		public InstallerLogType LogType
		{
			get
			{
				return m_LogType;
			}
		}
		public string Description
		{
			get
			{
				return m_Description;
			}
		}
		public Exception InnerException
		{
			get
			{
				return m_InnerException;
			}
		}
		public DateTime TimeStamp
		{
			get
			{
				return m_TimeStamp;
			}
		}
	}


	public enum InstallerLogType	
	{Info = 0 ,Warning,Error,Start,End}
}

