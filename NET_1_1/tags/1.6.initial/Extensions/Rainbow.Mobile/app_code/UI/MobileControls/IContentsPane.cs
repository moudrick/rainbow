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
	/// The IContentsPane interface must be implemented by any control 
    /// that needs to be a child pane of a ContentsPanel control.
    ///</summary>
    public interface IContentsPane : IPanelPane 
    {
		/// <summary>
		/// Title property
		/// Returns the title of the pane
		/// </summary>
		string ModuleTitle {get;}
		
		/// <summary>
		///  Id property
		/// Returns the id of the pane
		/// </summary>
		int ModuleId {get;}
		
		/// <summary>
		/// OnSetSummaryMode method
		/// Called when the ContentsPane control switches
		/// from summary view to item details view.
		/// </summary>
		void OnSetSummaryMode();
    }
 }

