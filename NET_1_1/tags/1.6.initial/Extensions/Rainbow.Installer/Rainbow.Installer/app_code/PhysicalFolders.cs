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
using System.IO;
using System.Web;
using System.Web.UI;

using System.Data.SqlClient;

using Rainbow.Settings;

namespace Rainbow.Installer
{
	public enum Folder
	{
	/// <summary>
	/// The Rainbow or application folder, default to ~/ 
	/// </summary>
	Temp,
	/// <summary>
	/// The Rainbow or application folder, default to ~/ 
	/// </summary>
	RainbowApplication,
	/// <summary>
	/// The Installer folder, default to ~/Install 
	/// </summary>
	Installer,
	/// <summary>
	/// The binaries folder, default to ~/bin 
	/// </summary>
	Binaries,
	/// <summary>
	/// The base modules folder, default to ~/DesktopModules 
	/// (maybe Modules soon or Rainbow_Modules) 
	/// </summary>
	Modules,
	/// <summary>
	/// The dir that contains themes, default to ~/Design 
	/// </summary>
	Design,
	/// <summary>
	/// The dir that contains themes, default to ~/Design/Themes 
	/// </summary>
	Themes,
	/// <summary>
	/// The dir that contains layouts, default to ~/Design/Layouts 
	/// </summary>
	Layouts,
	/// <summary>
	/// The dir that contains current portal custom data 
	/// </summary>
	PortalData,
	/// <summary>
	/// The dir that contains current portal themes, default to currentPortal/Themes 
	/// </summary>
	PortalThemes,

	/// <summary>
	/// The dir that contains current portal layouts, default to currentPortal/Layouts 
	/// </summary>
	PortalLayouts
	}


//	public class PhysicalFolder (Folder folder)
//	{
//		/// <summary>
//		/// The Rainbow or application folder, default to ~/ 
//		/// </summary>
//		public static string Temp
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath +@"Temp\";}
//		}
//		/// <summary>
//		/// The Rainbow or application folder, default to ~/ 
//		/// </summary>
//		public static string RainbowApplication
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath ;}
//		}
//		/// <summary>
//		/// The Installer folder, default to ~/Install 
//		/// </summary>
//		public static string Installer
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Install\";}
//		}
//		/// <summary>
//		/// The binaries folder, default to ~/bin 
//		/// </summary>
//		public static string Binaries
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"bin\";}
//		}
//		/// <summary>
//		/// The base modules folder, default to ~/DesktopModules 
//		/// (maybe Modules soon or Rainbow_Modules) 
//		/// </summary>
//		public static string Modules
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"DesktopModules\";}
//		}
//		/// <summary>
//		/// The dir that contains themes, default to ~/Design 
//		/// </summary>
//		public static string Design
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Design\";}
//		}
//		/// <summary>
//		/// The dir that contains themes, default to ~/Design/Themes 
//		/// </summary>
//		public static string Themes
//		{
//			get{return Design +@"Themes\";}
//		}
//		/// <summary>
//		/// The dir that contains layouts, default to ~/Design/Layouts 
//		/// </summary>
//		public static string Layouts
//		{
//			get{return Design +@"DesktopLayouts\";}
//		}
//		/// <summary>
//		/// The dir that contains current portal custom data 
//		/// </summary>
//		public static string PortalData
//		{
//			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Portals\";}
//		}
//		/// <summary>
//		/// The dir that contains current portal themes, default to currentPortal/Themes 
//		/// </summary>
//		public static string PortalThemes
//		{
//			get{return Themes;}
//		}
//		/// <summary>
//		/// The dir that contains current portal layouts, default to currentPortal/Layouts 
//		/// </summary>
//		public static string PortalLayouts
//		{
//			get{return Layouts;}
//		}
//	}
//

	public class PhysicalFolders
	{
		/// <summary>
		/// The Rainbow or application folder, default to ~/ 
		/// </summary>
		public static string Temp
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath +@"Temp\";}
		}
		/// <summary>
		/// The Rainbow or application folder, default to ~/ 
		/// </summary>
		public static string Application
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath ;}
		}
		/// <summary>
		/// The Installer folder, default to ~/Installer\install
		/// </summary>
		public static string Install
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Installer\install\";}
		}
		/// <summary>
		/// The Installer folder, default to ~/Installer\install
		/// </summary>
		public static string Uninstall
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Installer\uninstall\";}
		}
		/// <summary>
		/// The binaries folder, default to ~/bin 
		/// </summary>
		public static string Binaries
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"bin\";}
		}
		/// <summary>
		/// The base modules folder, default to ~/DesktopModules 
		/// (maybe Modules soon or Rainbow_Modules) 
		/// </summary>
		public static string Modules
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"DesktopModules\";}
		}
		/// <summary>
		/// The dir that contains themes, default to ~/Design 
		/// </summary>
		public static string Design
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Design\";}
		}
		/// <summary>
		/// The dir that contains themes, default to ~/Design/Themes 
		/// </summary>
		public static string Themes
		{
			get{return Design +@"Themes\";}
		}
		/// <summary>
		/// The dir that contains layouts, default to ~/Design/Layouts 
		/// </summary>
		public static string Layouts
		{
			get{return Design +@"DesktopLayouts\";}
		}
		/// <summary>
		/// The dir that contains current portal custom data 
		/// </summary>
		public static string PortalData
		{
			get{return Rainbow.Settings.Path.ApplicationPhysicalPath + @"Portals\";}
		}
		/// <summary>
		/// The dir that contains current portal themes, default to currentPortal/Themes 
		/// </summary>
		public static string PortalThemes
		{
			get{return Themes;}
		}
		/// <summary>
		/// The dir that contains current portal layouts, default to currentPortal/Layouts 
		/// </summary>
		public static string PortalLayouts
		{
			get{return Layouts;}
		}
	}


}


