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

namespace Rainbow.Installer
{
	/// <summary>
	/// </summary>
	public class InstallerException: Exception
	{
		private Action.ActionBase  m_performedAction ;
		public  InstallerException():this( string.Empty,null){}
		public  InstallerException(string message):this(message,null){}
		public  InstallerException(string message,Exception exception):this(message,exception,null){}
		public  InstallerException(string message,Exception exception, Action.ActionBase performedAction):base(message,exception)
		{
			m_performedAction =  performedAction ;
		}
		public  InstallerException(Action.ActionBase performedAction)
		{
			m_performedAction =  performedAction ;
		}

	}

	public class ActionNotDefinedException	:InstallerException
	{
		public ActionNotDefinedException ():base("Action is not defined!"){}
	}
	public class InstallerFileCorruptException	:InstallerException	{}
//	public class ActionNotAllowedException	:InstallerException	{}
//	public class CommitNotAllowedException	:InstallerException{}
//	public class RollbackNotAllowedException:InstallerException{}
//	public class RollbackNotDefinedException:InstallerException{}
	
}
