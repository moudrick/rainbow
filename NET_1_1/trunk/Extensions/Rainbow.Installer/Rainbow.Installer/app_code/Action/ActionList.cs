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
using System.IO;
using System.Collections;
using System.Diagnostics;

using Rainbow.Installer.Action;
using Rainbow.Installer.Log;
using Rainbow.Settings;

namespace Rainbow.Installer.Action
{
	/// <summary>
	/// Summary description for ActionList.
	/// </summary>
	public class ActionList : ArrayList
	{
		private LogList m_Log ;
		internal ActionList(){}
		public new ActionBase this[int index]
		{
			get
			{
				return (ActionBase) base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		public int Add(ActionBase value)
		{
			return base.Add (value);
		}

		public LogList Log 
		{
			get
			{
				if (m_Log == null)
					m_Log = new LogList();
		
				return m_Log;
			}
		}
			   
		public bool Run()
		{
			int _counter;
			for(_counter = 0 ; _counter < this.Count; _counter++)
			{
				try
				{
					this[_counter].DoAction();
				}
				catch (InstallerException iex)
				{
					Log.Add(InstallerLogType.Error,"Installer error while running action [ "+_counter+"] "  , iex);
					break;
				}
				catch (Exception ex)
				{
					Log.Add(InstallerLogType.Error,"General error while running action [ "+_counter+"] "  , ex);
					break;
				}
			}

			if (Log.HasErrors)
			{
				while( _counter >= 0 )
				{
					try
					{
						this[_counter].Rollback();
					}
					catch (InstallerException iex)
					{
						Log.Add(InstallerLogType.Error,"Installer error while rolling back action [ "+_counter+"] "  , iex);						
					}
					catch (Exception ex)
					{
						Log.Add(InstallerLogType.Error,"Generel error while rolling back action [ "+_counter+"] "  , ex);						
						//break;
					}
					_counter--;
				}
			}
			else
			{
				for(_counter = 0 ; _counter < this.Count; _counter++)
				{
					try
					{
						this[_counter].Commit();
					}
					catch (InstallerException iex)
					{
						Log.Add(InstallerLogType.Error,"Installer error while committing action [ "+_counter+"] "  , iex);						
						//break;
					}
					catch (Exception ex)
					{
						Log.Add(InstallerLogType.Error,"Generel error while committing action [ "+_counter+"] "  , ex);						
						//break;
					}
				}
			}
			return (!Log.HasErrors);
		}
	}
}
