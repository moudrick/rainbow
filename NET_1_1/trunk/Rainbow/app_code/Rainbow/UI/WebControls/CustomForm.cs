using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// This is a server form that is able to post to a different page than than the one it is on.
	/// Simply set the Action property to whatever page you want.
	/// This control is available from the great MetaBuilders site http://www.metabuilders.com
	/// </summary>
	/// <example>
	/// The following is an example page which posts a server form to google's search engine.
	/// <code>
	/// <![CDATA[
	/// <%@ Register TagPrefix="rbc" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
	/// <html><body>
	///		<rbc:CustomForm runat="server" Method="Get" Action="http://www.google.com/search">
	///			<asp:TextBox id="q" runat="server"></asp:TextBox>
	///			<asp:Button runat="server" Text="Google Search" />
	///		</rbc:CustomForm>
	/// </body></html>
	/// ]]>
	/// </code>
	/// </example>
	public class CustomForm : HtmlForm 
	{

		/// <summary>
		/// Overridden to render custom Action attribute
		/// </summary>
		protected override void RenderAttributes(HtmlTextWriter writer) 
		{

			// From HtmlForm, with changes to Action
			writer.WriteAttribute("name", this.Name);
			this.Attributes.Remove("name");

			writer.WriteAttribute("method", this.Method);
			this.Attributes.Remove("method");

			writer.WriteAttribute("action", this.ResolveUrl(this.Action), true);
			this.Attributes.Remove("action");

			string submitEvent = this.Page_ClientOnSubmitEvent;
			if (submitEvent != null && submitEvent.Length > 0) 
			{
				if (this.Attributes["onsubmit"] != null) 
				{
					submitEvent = submitEvent + this.Attributes["onsubmit"];
					this.Attributes.Remove("onsubmit");
				}
				writer.WriteAttribute("language", "javascript");
				writer.WriteAttribute("onsubmit", submitEvent);
			}

			writer.WriteAttribute("id", this.ClientID);
			

			// From HtmlContainerControl
			this.ViewState.Remove("innerhtml");


			// From HtmlControl
			this.Attributes.Render(writer);
		}

		/// <summary>
		/// Gets or sets the target url of the form post.
		/// </summary>
		/// <remarks>Leave blank to postback to the same page.</remarks>
		[
			Description("Gets or sets the target url of the form post."),
				Category("Behavior"),
				DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual String Action 
		{
			get 
			{
				if(this.ViewState["Action"] != null)
				{
					return (String) this.ViewState["Action"];
				}
				return this.GetBaseActionAttribute();
			}
			set 
			{
				this.ViewState["Action"] = value;
			}
		}

		/// <summary>
		/// Uses reflection to get the result of the private implementation of GetActionAttribute of the base class.
		/// </summary>
		/// <returns>Returns the normal action attribute of a server form.</returns>
		private String GetBaseActionAttribute() 
		{
			Type formType = typeof(HtmlForm);
			MethodInfo actionMethod = formType.GetMethod("GetActionAttribute", BindingFlags.Instance | BindingFlags.NonPublic );
			Object result = actionMethod.Invoke(this,null);
			return (String)result;
		}

		/// <summary>
		/// Uses reflection to access the ClientOnSubmitEvent property of the Page class
		/// </summary>
		private String Page_ClientOnSubmitEvent 
		{
			get 
			{
				return (String)GetHiddenProperty( this.Page, typeof(System.Web.UI.Page), "ClientOnSubmitEvent" );
			}
		}

		/// <summary>
		/// Uses reflection to access any property on an object, even tho the property is marked protected, internal, or private.
		/// </summary>
		/// <param name="target">The object being accessed</param>
		/// <param name="targetType">The Type to examine. Usually the Type of target arg, but sometimes a superclass of it.</param>
		/// <param name="propertyName">The name of the property to access.</param>
		/// <returns>The value of the property, or null if the property is not found.</returns>
		private Object GetHiddenProperty(Object target, Type targetType, String propertyName ) 
		{
			PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic );
			if ( property != null ) 
			{
				return property.GetValue(target, null);
			} 
			else 
			{
				return null;
			}
		}
	}
}