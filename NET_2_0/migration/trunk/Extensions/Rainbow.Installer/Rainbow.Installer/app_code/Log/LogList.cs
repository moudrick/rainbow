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
using System.Collections; 



namespace Rainbow.Installer.Log
{
	/// <summary>
	/// Summary description for LogList.
	/// </summary>
	public class LogList :ArrayList
	{
		internal LogList()
		{
		}

		public bool HasErrors
		{
			get
			{
				foreach(LogEntry _entry in this)
				{
					if (_entry.LogType == InstallerLogType.Error)
						return true;
				}
					return false;
			}
		}
		
		public new LogEntry this[int index]
		{
			get
			{
				return (LogEntry) base[index];
			}
			set
			{
				base[index] = value;
			}
		
		}
		public  int Add(InstallerLogType logType, string description, Exception innerException)
		{
			return this.Add (new LogEntry(logType ,description,innerException));
		}
		public  int Add(InstallerLogType logType, string description)
		{
			return this.Add (logType ,description,null);
		}
		public  int Add(LogEntry logEntry)
		{
			return base.Add (logEntry);
		}


	}
}
