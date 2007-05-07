using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;

using Esperantus;

namespace Rainbow.UI.WebControls
{
	// define a custom event class with the ProductID as a property
	public class AddToCartEventArgs : EventArgs
	{
		// Product ID as a property
		private int _productID;
		public int ProductID
		{
			get { return _productID; }
			set { _productID = value; }
		}

		// default constructor
		public AddToCartEventArgs()
		{}

		// constructor from a productID
		public AddToCartEventArgs(int productID)
		{
			_productID = productID;
		}
	}

	// define a new delegate that use our custom eventArg class above
	public delegate void AddToCartEventHandler(object sender, AddToCartEventArgs e);

	/// <summary>
	/// Summary description for ProductAddToCart.
	/// </summary>
	// Implement the IPostBackEventHandler to intercept the click event from the linkButton
	// when the page is posted back, 
	public class ProductAddToCart :
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
		protected virtual void OnAddToCart()
		{
			RaiseBubbleEvent(this, new AddToCartEventArgs(ProductID)); 
		}

		// called because we implemented the IPostBackEventHandler
		// called when the user click on the button
		public void RaisePostBackEvent(string eventArg)
		{
			// decode the argument
			// raise the event
			OnAddToCart();
		}
	}
}
