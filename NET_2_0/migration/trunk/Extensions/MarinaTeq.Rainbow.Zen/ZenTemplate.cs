using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
//using System.Web.UI.Design;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.WebControls
{
    /// <summary>
    /// DesktopPanes design support class for Visual Studio. Pane Template.
    /// </summary>
	public class ZenTemplate : Control, INamingContainer
	{
		private ZenLayout parent;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent"></param>
		public ZenTemplate(ZenLayout parent)
		{
			this.parent = parent;
		}
	}
}