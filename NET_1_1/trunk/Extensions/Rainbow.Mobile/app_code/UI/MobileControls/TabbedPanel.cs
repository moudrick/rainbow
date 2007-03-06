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
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.MobileControls.Adapters;

using Esperantus;



namespace Rainbow.UI.MobileControls
{

	/// <summary>
    /// The TabbedPanel Class is a control that inherits from MultiPanel,
    /// and provides the ability for the user to switch between panels.
    /// The TabbedPanel also has adapters defined for custom rendering.
    ///</summary>
	[	
	Description("The TabbedPanel Class is a control that inherits from MultiPanel and provides the ability for the user to switch between panels."),
	DefaultProperty("Title"),
	ParseChildren(true),
	PersistChildren(true),
	//	ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)),
	ToolboxData("<{0}:TabbedPanel runat=\"server\"></{0}:TabbedPanel>"),
	Designer("Rainbow.UI.MobileControls.Design.TabbedPanelDesigner")
	]
	public class TabbedPanel : MultiPanel, IPostBackEventHandler 
	{

//		protected new int ActivePaneIndex 
//		{
//			get
//			{
//				base.ActivePaneIndex = 0;
//				for (int i=0 ;  i < this.Panes.Count ; i++)
//					if (this.Panes[i].TabId == this.TabId) base.ActivePaneIndex = i ;
//
//				return  base.ActivePaneIndex;
//			}
//		}


		/// <summary>
		/// OnRender is called by the framework to render the control.
		/// The TabbedPanel's OnRender method overrides the behavior
		/// of MultiPanel, and directly calls the adapter to do rendering.
		/// </summary>
		/// <param name="writer"></param>
		protected override void OnRender(HtmlTextWriter writer) 
		{
			
			Adapter.Render(writer);
		}
	   

		private Style activeTabStyle ;
		/// <summary>
		/// Get the Style , defaulting
		/// to an new empty Style.
		/// </summary>
		[Browsable( false )] 
		public Style ActiveTabStyle
		{
			get
			{
				// Get the color from the ViewState property bag, defaulting
				// to an empty color.
				if (activeTabStyle == null) 
				{
					try
					{
						StyleSheet styleSheet = (StyleSheet) Page.FindControl("MobileStyleSheet");
						activeTabStyle =styleSheet[ActiveTabStyleReference];
					}
					catch
					{
						activeTabStyle = new Style();
					}
				}
				return  activeTabStyle;
			}
			set 
			{
				// Save the color in the ViewState property bag.
				activeTabStyle = value;
			}
		}


		private Style tabStyle;
		/// <summary>
		/// Get the Style, defaulting
		/// to an new empty Style.
		/// </summary>
		[Browsable( false )]
		public Style TabStyle
		{
			get
			{
				if (tabStyle == null)
				{
					try
					{
						StyleSheet styleSheet = (StyleSheet) Page.FindControl("MobileStyleSheet");
						tabStyle =styleSheet[TabStyleReference];
					}
					catch
					{
						tabStyle = new Style();
					}
				}
			return  tabStyle;
			}
			set 
			{
				// Save the color in the ViewState property bag.
				tabStyle  = value;
			}
		}


		/// <summary>
		/// Gets or sets the background color used for each tab label, when
		/// tabbed rendering is used.
		/// </summary>
		public String TabStyleReference
	{
		get 
		{
			// Get the color from the ViewState property bag, defaulting
			// to an empty color.
			Object o = ViewState["TabStyleReference"];
			return o != null ? (string)o : string.Empty;
		}

		set 
		{
		    // Save the color in the ViewState property bag.
		    ViewState["TabStyleReference"] = value;
		}
	}


		/// <summary>
		/// Gets or sets the background color used for each tab label, when
		/// tabbed rendering is used.
		/// </summary>
		public String ActiveTabStyleReference
		{
			get 
			{
				// Get the color from the ViewState property bag, defaulting
				// to an empty color.
				Object o = ViewState["ActiveTabStyleReference"];
				return o != null ? (string)o : string.Empty;
			}
		
			set 
			{
				// Save the color in the ViewState property bag.
				ViewState["ActiveTabStyleReference"] = value;
			}
		}
		

		#region TabColors
//		/// <summary>
//		/// Gets or sets the background color used for each tab label, when
//		/// tabbed rendering is used.
//		/// </summary>
//		public Color TabColor 
//	{
//		get 
//		{
//			// Get the color from the ViewState property bag, defaulting
//			// to an empty color.
//			Object o = ViewState["TabColor"];
//			return o != null ? (Color)o : Color.Empty;
//		}
//
//		set 
//        {
//            // Save the color in the ViewState property bag.
//            ViewState["TabColor"] = value;
//        }
//    }
//
//
//		/// <summary>
//		/// Gets or sets the text color used for each tab label, when
//		/// tabbed rendering is used.
//		/// </summary>
//        public Color TabTextColor 
//        {
//            get 
//            {
//                // Get the color from the ViewState property bag, defaulting
//                // to an empty color.
//                Object o = ViewState["TabTextColor"];
//                return o != null ? (Color)o : Color.Empty;
//            }
//
//            set 
//            {
//                // Save the color in the ViewState property bag.
//                ViewState["TabTextColor"] = value;
//            }
//        }
//
//
//		/// <summary>
//		/// Gets or sets the background color used for the active tab label, when
//		/// tabbed rendering is used.
//		/// </summary>
//        public Color ActiveTabColor 
//        {
//            get 
//            {
//                // Get the color from the ViewState property bag, defaulting
//                // to an empty color.
//                Object o = ViewState["ActiveTabColor"];
//                return o != null ? (Color)o : Color.Empty;
//            }
//
//            set 
//            {
//                // Save the color in the ViewState property bag.
//                ViewState["ActiveTabColor"] = value;
//            }
//        }
//
//
//		/// <summary>
//		/// Gets or sets the text color used for the active tab label, when
//		// tabbed rendering is used.
//		/// </summary>
//        public Color ActiveTabTextColor 
//        {
//            get 
//            {
//                // Get the color from the ViewState property bag, defaulting
//                // to an empty color.
//                Object o = ViewState["ActiveTabTextColor"];
//                return o != null ? (Color)o : Color.Empty;
//            }
//
//            set 
//            {
//                // Save the color in the ViewState property bag.
//                ViewState["ActiveTabTextColor"] = value;
//            }
//        }
//
		
		#endregion

		/// <summary>
		/// Gets or sets the number of tabs to be displayed per row, when
		/// tabbed rendering is used.
		/// </summary>
        public int TabsPerRow 
        {
            get 
            {
                // Get the value from the ViewState property bag, defaulting
                // to 4.
                Object o = ViewState["TabsPerRow"];
                return o != null ? (int)o : 4;
            }

            set 
            {
                // Save the value in the ViewState property bag.
                ViewState["TabsPerRow"] = value;
            }
        }


		/// <summary>
		/// RaisePostBackEvent is called by the framework when the control
		/// is to receive a postback event. Responds to the event by 
		/// using the event information to switch to another active pane.
		/// </summary>
		/// <param name="eventArgument"></param>
        public virtual void RaisePostBackEvent(String eventArgument) 
        {
            TabEventArgs e = new TabEventArgs();
			e.TabIndex = base.ActivePaneIndex ;
			e.TabId = base.ActivePane.TabId ;
            // Call Deactivate event handler.
            OnTabDeactivate(e);

            //base.ActivePaneIndex = Int32.Parse(eventArgument);
			base.ActivePaneTabID = Int32.Parse(eventArgument);

			e.TabIndex = base.ActivePaneIndex ;
			e.TabId = base.ActivePane.TabId ;
            // Call Activate event handler.
            OnTabActivate(e);
        }


		
		/// <summary>
		/// TabbedPanelEventHandler
		/// </summary>
		public delegate void TabbedPanelEventHandler( object source, TabEventArgs e );
		/// <summary>
		/// TabbedPanelEventHandler
		/// </summary>
        public event TabbedPanelEventHandler TabActivate ;
        /// <summary>
        /// TabbedPanelEventHandler
        /// </summary>
		public event TabbedPanelEventHandler TabDeactivate;
		
		/// <summary>
		/// TabEventArgs
		/// </summary>
		public class TabEventArgs : EventArgs
		{
		public int TabId;
		public int TabIndex;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabId"></param>
			public TabEventArgs (int tabId,int tabIndex)
			{
				this.TabId = tabId;
				this.TabIndex = tabIndex;
			}
			public TabEventArgs ()
			{
				this.TabId = 0;
				this.TabIndex=0;
			}
		}
		

		/// <summary>
		/// OnTabActivate is called when a child pane is newly activated
		/// as a result of user interaction, and raises the TabActivate event.
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnTabActivate(TabEventArgs e) 
        {
            if (TabActivate != null) 
            {
                TabActivate(this, e);
            }
        }

        //*********************************************************************
		/// <summary>
		/// OnTabDeactivate is called when a child pane is deactivated
		/// as a result of user interaction, and raises the TabDeactivate event.
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnTabDeactivate(TabEventArgs e) 
        {
            if (TabDeactivate != null) 
            {
                TabDeactivate(this, e);
            }
        }
    }

}

