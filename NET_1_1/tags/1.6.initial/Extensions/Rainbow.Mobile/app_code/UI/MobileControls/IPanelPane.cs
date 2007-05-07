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


[assembly:TagPrefix("Rainbow.MobileControls", "portal")]
namespace Rainbow.UI.MobileControls
{
	/// <summary>
	/// The IPanelPane interface must be implemented by any control 
	/// that needs to be a child pane of a MultiPanel or derivative
	/// control. The interface has the following members:
	///</summary>
	public interface IPanelPane 
    {
		///<summary>
		///Returns the title of the pane.
		/// </summary>
		String Title { get; }
		///<summary>
		///Returns the TabId of the pane.
		/// </summary>
		int TabId{get;}
	}
 }

