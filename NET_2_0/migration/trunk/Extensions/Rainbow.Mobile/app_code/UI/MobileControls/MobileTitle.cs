/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * This code is partially based on the IbuySpy Mobile Portal Code. 
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.ComponentModel;

using Esperantus;


namespace Rainbow.UI.MobileControls
{
	/// <summary>
	///	The MobileTitle User Control is responsible for displaying the title of 
	///	each portal module within the mobile portal. 
	/// </summary>
	[	
	Description("The MobileModuleTitle Class is a MobileControl."),
	DefaultProperty("Text"),
	ParseChildren(true),
	//PersistChildren(false),
	//	ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)),
	ToolboxData("<{0}:MobileTitle runat=\"server\" />"),
	Designer("Rainbow.UI.MobileControls.Design.MobileTitleDesigner")
	]
	public class MobileTitle :  System.Web.UI.MobileControls.MobileUserControl
	{
		/// <summary>
		/// Text
		/// </summary>
		[
		System.ComponentModel.Description(""),
		System.ComponentModel.Browsable(true)
		]
		public String Text
		{
			get
			{
				// Load the Text from the ViewState property bag, 
				// If the Text property has not been explicitly specified, 
				// walk the parent control chain to find a MobilePortalModuleControl,
				// and obtain the title from the corresponding module.

				String text = (String)ViewState["Text"];
				try
				{
					if (text== null && this.Parent !=  null)
					{
						MobilePortalModuleControl module = null;
						Control control = this;
            
						while (module == null && (control = control.Parent) != null) 
						{
							module = control as MobilePortalModuleControl;
						}

						ViewState["Text"] = module.ModuleTitle;
						text = module.ModuleTitle;
					}
				}
				catch{}
				if (text == null)
					text = string.Empty ;

				return text;
			}
			set
			{
				// Save the Text to the ViewState property bag.
				ViewState["Text"] = value;
			}
		}


		#region  Appearance properties
		/// <summary>
		/// BreakAfter
		/// </summary>
		[
		System.ComponentModel.DefaultValue(true),
		System.ComponentModel.Browsable(true)
		]
		public bool BreakAfter
		{
			get 
			{
				// Load the BreakAfter from the ViewState property bag, 
				// defaulting to false.
				string s = (string)ViewState["BreakAfter"];
				bool retBool = (s== null) ? false:bool.Parse(s);
				return retBool;
			}

			set 
			{
				// Save the BreakAfter to the ViewState property bag.
				ViewState["BreakAfter"] = value.ToString();
			}
		}

	
		/// <summary>
		/// StyleReference
		/// </summary>
		public new string StyleReference
		{
				
			get 
			{
				// Load the StyleReference from the ViewState property bag, 
				// defaulting to an empty String.
				String s = (String)ViewState["StyleReference"];
				return s != null ? s : String.Empty;
			}

			set 
			{
				// Save the StyleReference to the ViewState property bag.
				ViewState["StyleReference"] = value;
			}
		}
//		/// <summary>
//		/// Style
//		/// </summary>
		protected internal virtual new System.Web.UI.MobileControls.Style Style
		{
			get
			{
				System.Web.UI.MobileControls.Style style = ((MobilePage)base.Page).MobileStyle(StyleReference);
				if (style==null)
					style =new System.Web.UI.MobileControls.Style();
				
				return style;
			}
		}

		#endregion

		
		public override void DataBind()
		{
			this.Controls.Clear();

			System.Web.UI.MobileControls.Label labelTitle = new System.Web.UI.MobileControls.Label();
			labelTitle.Text = Text;
			ApplyStyle(labelTitle);
			this.Controls.Add(labelTitle);

			// base.DataBind ();
		}

		/// <summary>
		/// The OnLoad event handler executes after the user control is loaded
		/// and inserted into the control tree.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			DataBind();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		private void ApplyStyle (MobileControl control)
		{
			System.Web.UI.MobileControls.Style style = ((MobilePage)base.Page).MobileStyle(StyleReference);
			if (style==null)
				style =new System.Web.UI.MobileControls.Style();
			
			control.Alignment = Style.Alignment;
			control.BackColor = Style.BackColor;
			control.Font.Bold = Style.Font.Bold;
			control.Font.Italic = Style.Font.Italic;
			control.Font.Name = Style.Font.Name;
			control.Font.Size = Style.Font.Size;
			control.ForeColor = Style.ForeColor;
			control.Wrapping = Style.Wrapping;
		}



		#region Web Form Designer generated code
		/// <summary>
		/// OnInit
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}