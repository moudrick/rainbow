/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Web.UI.MobileControls;

namespace Rainbow.UI.MobileControls.Globalized
{
	/// <summary>
	/// IGlobalizedMobilControls is a common in iterface used
	/// by all Globalized controls
	/// </summary>
	public interface IGlobalizedMobilControls
	{
		/// <summary>
		/// If the TextKey value is present the 
		/// text in control will be replaced with the Text
		/// matching TextKey in current language
		/// </summary>
		string TextKey	{get;set;}
	}
}
