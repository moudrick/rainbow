using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;

using Esperantus;

namespace Rainbow.UI.WebControls
{
	// define a custom event class with the ProductID as a property
	public class LinkGoToDetailsEventArgs : EventArgs
	{
		// Product ID as a property
		private int _productID;
		public int ProductID
		{
			get { return _productID; }
			set { _productID = value; }
		}

		// default constructor
		public LinkGoToDetailsEventArgs()
		{}

		// constructor from a productID
		public LinkGoToDetailsEventArgs(int productID)
		{
			_productID = productID;
		}
	}

	// define a new delegate that use our custom eventArg class above
	public delegate void LinkGoToDetailsEventHandler(object sender, LinkGoToDetailsEventArgs e);

	/// <summary>
	/// Summary description for ProductLinkGoToDetails.
	/// </summary>
	// Implement the IPostBackEventHandler to intercept the click event from the HyperLink
	// when the page is posted back, 
	public class ProductLinkGoToDetails :
		Esperantus.WebControls.LinkButton, IPostBackEventHandler
		{
		// Product ID as a property
		private int _productID;
		public int ProductID
		{
			get { return _productID; }
			set { _productID = value; }
		}

		// create a custom event for this control
		// we use 'Event Bubbling' to allow a child control to propagate events up
		// its containment hierarchy.

		// Raise an event with the ProductID as a parameter
		protected virtual void OnLinkGoToDetails()
		{
			RaiseBubbleEvent(this, new LinkGoToDetailsEventArgs(ProductID)); 
		}

		// called because we implemented the IPostBackEventHandler
		// called when the user click on the button
		public void RaisePostBackEvent(string eventArg)
		{
			// decode the argument
			// raise the event
			OnLinkGoToDetails();
		}
	}
}
