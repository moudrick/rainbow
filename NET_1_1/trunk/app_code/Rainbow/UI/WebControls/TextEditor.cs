using System.Web.UI.WebControls;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// TextEditor is a simple implementation for an html editor. 
	/// Currently implements text only.
	/// </summary>
	public class TextEditor : TextBox, IHtmlEditor
	{
		public TextEditor()
		{
			TextMode = TextBoxMode.MultiLine;
			CssClass = "NormalTextBox";
		}

		public string ImageFolder 
		{
			get { return string.Empty; }
			set {;}
		}
	}
}