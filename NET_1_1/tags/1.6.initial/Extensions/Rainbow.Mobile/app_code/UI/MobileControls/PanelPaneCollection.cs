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
	/// The PanelPaneCollection Class is used to keep a collection of
	/// child panes of a MultiPanel control. The class implements 
	/// ICollection, so it can be used as a general collection.
	/// </summary>
	public class PanelPaneCollection : ICollection 
	{
		// Private instance variables.
		private MultiPanel _parent;
		private ArrayList _items = new ArrayList();

		// Can only be instantiated by MultiPanel.
		internal PanelPaneCollection(MultiPanel parent) 
		{
			// Save off reference to parent control.
			_parent = parent;
		}

		/// <summary>
		/// Adds a pane to the collection.
		/// </summary>
		/// <param name="pane"></param>
		public void Add(IPanelPane pane) 
		{
			// Add the pane to the parent's child controls collection.
			_parent.Controls.Add((Control)pane);
			_items.Add(pane);
		}

		/// <summary>
		/// Adds a pane to the collection, but does not add it to the parent's
		/// controls. This is called by the parent control itself to add 
		/// panes.
		/// </summary>
		/// <param name="pane"></param>
		internal void AddInternal(IPanelPane pane) 
		{
			_items.Add(pane);
		}

		/// <summary>
		/// Removes a pane from the collection.
		/// </summary>
		/// <param name="pane"></param>
		public void Remove(IPanelPane pane) 
		{
			// Remove the pane from the parent's child controls collection.
			_parent.Controls.Remove((Control)pane);
			_items.Remove(pane);
		}

		/// <summary>
		/// Removes all panes from the collection.
		/// </summary>
		public void Clear() 
		{
			// Remove all child controls from the parent.
			foreach (Control pane in _items) 
			{
				_parent.Controls.Remove(pane);
			}
			_items.Clear();
		}

		/// <summary>
		/// Returns a pane by index.
		/// </summary>
		public IPanelPane this[int index] 
		{
			get 
			{
				return (IPanelPane)_items[index];
			}
		}

		/// <summary>
		/// Returns the number of panes in the collection.
		/// </summary>
		public int Count 
		{
			get 
			{
				return _items.Count;
			}
		}

		/// <summary>
		/// Returns the index of a given pane.
		/// </summary>
		/// <param name="pane"></param>
		/// <returns></returns>
		public int IndexOf(IPanelPane pane) 
		{
			return _items.IndexOf(pane);
		}

		/// <summary>
		/// Returns whether the collection is read-only.
		/// </summary>
		public bool IsReadOnly 
		{
			get 
			{
				return _items.IsReadOnly;
			}
		}

		/// <summary>
		/// Returns whether the collection is synchronized.
		/// </summary>
		public bool IsSynchronized 
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the collection's synchronization root.
		/// </summary>
		public Object SyncRoot 
		{
			get 
			{
				return this;
			}
		}

		/// <summary>
		/// Copies the contents of the collection to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index) 
		{
			foreach (Object item in _items) 
			{
				array.SetValue (item, index++);
			}
		}

		/// <summary>
		/// Returns an object capable of enumerating the collection.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator GetEnumerator() 
		{
			return _items.GetEnumerator ();
		}

	
	
		/// <summary>
		/// Returns a MobilePortalTab as IPanelPane by TabId.
		/// </summary>
		public IPanelPane GetPane( int tabId)
		{
			if (tabId == 0)
				return  (IPanelPane)  _items[0];

			foreach (MobilePortalTab item in _items)
			{
				if (item.TabId == tabId)
				{
					return (IPanelPane) item;
				}
			}
			return null;
		}
	}
}

