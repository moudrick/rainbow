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
	/// The MultiPanel Class is a control that inherits from 
	/// ChildPanel, and can manage one or more child controls or "panes".
	///</summary>
    public class MultiPanel : ChildPanel 
    {
		// Collection of panes.
        private PanelPaneCollection _panes;
		/// <summary>
		/// Returns the collection of child panes.
		/// </summary>
		[Browsable( false )]
		public PanelPaneCollection Panes 
        {
            get 
            {
                // If not yet created, create the collection.
                if (_panes == null) 
                {
                    _panes = new PanelPaneCollection(this);
                }
                return _panes;
            }
        }

		/// <summary>
		/// Get or set the currently active child pane.
		/// </summary>
		[Browsable( false )]
		public IPanelPane ActivePane 
        {
            get 
            {
                // Get the index of the active pane, and use it to
                // look up the active pane.
                int index = ActivePaneIndex;
                return (index != -1) ? Panes[index] : null;
            }

            set 
            {
                // Find the index of the given pane, and use it to
                // set the active pane index.
                int index = Panes.IndexOf(value);
                if (index == -1) 
                {
                    throw new Exception("Pane not in Panes collection");
                }
                ActivePaneIndex = index;
            }
        }
		/// <summary>
		/// Get or set the index of the currently active child pane.
		/// </summary>
		[Browsable( false )]
		public int ActivePaneTabID 
		{
			get 
			{
				// Get the index from the ViewState property bag, defaulting
				// to the first pane if not found.
				Object o = ViewState["ActivePaneTabID"];
				if (o != null) 
				{
					return (int)o;
				}
				else 
				{
					return (Panes.Count > 0) ? 0 : -1;
				}
			}

			set 
			{
				// Make sure index is within range.
				int index = 0;
				foreach(IPanelPane pane in Panes)
				{
					if (pane.TabId == value)
					{
						// Set the index in the ViewState property bag.
						ViewState["ActivePaneTabID"] = value;
						ActivePaneIndex = index;
						return ;
					}
				index ++;
				}           
				throw new Exception("Active pane TabId does not exist");
            
			}
		}
		/// <summary>
		/// Get or set the index of the currently active child pane.
		/// </summary>
		[Browsable( false )]
		public int ActivePaneIndex 
        {
            get 
            {
                // Get the index from the ViewState property bag, defaulting
                // to the first pane if not found.
                Object o = ViewState["ActivePaneIndex"];
                if (o != null) 
                {
                    return (int)o;
                }
                else 
                {
                    return (Panes.Count > 0) ? 0 : -1;
                }
            }

            set 
            {
                // Make sure index is within range.
                if (value < 0 || value >= Panes.Count) 
                {
                    throw new Exception("Active pane index out of range");
                }

                // Set the index in the ViewState property bag.
                ViewState["ActivePaneIndex"] = value;
            }
        }

		/// <summary>
		/// AddParsedSubObject is called by the framework when a child
		/// control is being added to the control from the persistence format.
		/// AddParsedSubObject below checks if the added control is a 
		/// child pane, and automatically adds it to the Panes collection.
		/// </summary>
		/// <param name="obj"></param>
        protected override void AddParsedSubObject(Object obj) 
        {
            IPanelPane pane = obj as IPanelPane;
        
            // Only allow panes as children.
            if (pane == null) 
            {
                throw new Exception("A MultiPanel control can only contain panes.");
            }

            // Add the pane to the Panes collection.
            Panes.AddInternal(pane);
            base.AddParsedSubObject(obj);
        }

		/// <summary>
		/// OnRender is called by the framework to render the control.
		/// By default, OnRender of a MultiPanel only renders the active 
		/// child pane. Specialized versions of the control, such as
		/// TabbedPanel and ContentsPanel, have different behavior.
		/// </summary>
		/// <param name="writer"></param>
        protected override void OnRender(HtmlTextWriter writer) 
        {
            ((Control)ActivePane).RenderControl(writer);
        }

		/// <summary>
		/// PaginateRecursive is called by the framework to recursively
		/// paginate children. For MultiPanel controls, PaginateRecursive
		/// only paginates the active child pane.
		/// </summary>
		/// <param name="pager"></param>
        public override void PaginateRecursive(ControlPager pager) 
        {
            Control activePane = (Control)ActivePane;

            // Active pane may not be a mobile control (e.g. it may be
            // a user control).
            MobileControl mobileCtl = activePane as MobileControl;

            if (mobileCtl != null) 
            {
                // Paginate the children.
                mobileCtl.PaginateRecursive(pager);

                // Set own first and last page from results of child
                // pagination.
                this.FirstPage = mobileCtl.FirstPage;
                this.LastPage = pager.PageCount;
            }
            else 
            {
                // Call the DoPaginateChildren utility method to 
                // paginate a non-mobile child.
                int firstAssignedPage = -1;
                DoPaginateChildren(pager, activePane, ref firstAssignedPage);

                // Set own first and last page from results of child
                // pagination.
                if (firstAssignedPage != -1) 
                {
                    this.FirstPage = firstAssignedPage;
                }
                else 
                {
                    this.FirstPage = pager.GetPage(100);
                }
                this.LastPage = pager.PageCount;
            }
        }

		/// <summary>
		/// The DoPaginateRecursive method paginates non-mobile child
		/// controls, looking for mobile controls inside them.
		/// </summary>
		/// <param name="pager"></param>
		/// <param name="ctl"></param>
		/// <param name="firstAssignedPage"></param>
        private static void DoPaginateChildren(ControlPager pager, Control ctl, ref int firstAssignedPage) 
        {
//			if (ctl == null)
//				return;

            // Search all children of the control.
            if (ctl != null && ctl.HasControls()) 
            {
                foreach (Control child in ctl.Controls) 
                {
                    if (child.Visible) 
                    {
                        // Look for a visible mobile control.
                        MobileControl mobileCtl = child as MobileControl;
                        if (mobileCtl != null) 
                        {
                            // Paginate the mobile control.
                            mobileCtl.PaginateRecursive(pager);

                            // If this is the first control being paginated,
                            // set the first assigned page.
                            if (firstAssignedPage == -1) 
                            {
                                firstAssignedPage = mobileCtl.FirstPage;
                            }
                        }
                        else if (child is UserControl) 
                        {
                            // Continue paginating user controls, which may contain
                            // their own mobile children.
                            DoPaginateChildren(pager, child, ref firstAssignedPage);
                        }
                    }
                }
            }
        }

    }


}

