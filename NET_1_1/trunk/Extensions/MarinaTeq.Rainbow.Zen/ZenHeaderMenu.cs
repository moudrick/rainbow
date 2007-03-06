using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

using Rainbow;
using Rainbow.UI;
using Rainbow.UI.WebControls;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Summary description for ZenHeaderMenu.
	/// </summary>
	public class ZenHeaderMenu : Rainbow.UI.WebControls.HeaderMenu
	{
		/// <summary>
		/// 
		/// </summary>
		public ZenHeaderMenu()
		{
		}

		private string _buttonsCssClass;
		private string _labelsCssClass;

		/// <summary>
		/// 
		/// </summary>
		public string ButtonsCssClass
		{
			get {return _buttonsCssClass;}
			set {_buttonsCssClass = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public string LabelsCssClass
		{
			get {return _labelsCssClass;}
			set {_labelsCssClass = value;}
		}
				
		/// <summary>
		/// Overrides Render to produce simple unordered list for Zen
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			ArrayList _labels = new ArrayList();
			ArrayList _buttons = new ArrayList();
			ArrayList items = (ArrayList)this.DataSource;

			foreach ( string item in items )
			{
				if ( item.StartsWith("<a") )
					_buttons.Add(item);
				else
					_labels.Add(item);
			}

			if ( _labels.Count > 0 )
			{
				writer.Write("<div class=\"");
				writer.Write(this.LabelsCssClass);
				writer.Write("\">");
				writer.Write("<ul class=\"zen-hdrmenu-labels\">");
				foreach ( string _label in _labels )
				{
					writer.Write("<li>");
					writer.Write(_label);
					writer.Write("</li>");
				}
				writer.Write("</ul>");
				writer.Write("</div>");
			}

			if ( _buttons.Count > 0 )
			{
				writer.Write("<div class=\"");
				writer.Write(this.ButtonsCssClass);
				writer.Write("\">");
				writer.Write("<ul class=\"zen-hdrmenu-btns\">");
				foreach ( string _button in _buttons )
				{
					writer.Write("<li>");
					writer.Write(_button);
					writer.Write("</li>");
				}
				writer.Write("</ul>");
				writer.Write("</div>");
			}
		}

	}
}
