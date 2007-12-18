using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace Rainbow.Core
{
	/// <summary>
	/// Summary description for Active.
	/// </summary>
	[Serializable]
	public class Active : System.ComponentModel.Component
	{
		protected static Rainbow.Core.Portal portal;
		protected static Rainbow.Core.Page page;

		public static Core.Portal	Portal	{ get { return portal; }	set { portal = value; }	}
		public static Core.Page		Page	{ get { return page; }		set { page = value; }	}

		#region Component Model
		private System.ComponentModel.IContainer components;

		private Active(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private Active()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
