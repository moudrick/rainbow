namespace Rainbow.Framework.Web.UI.WebControls
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// TextEditor is a simple implementation for an html editor. 
    ///     Currently implements text only.
    /// </summary>
    public class TextEditor : TextBox, IHtmlEditor
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "TextEditor" /> class.
        /// </summary>
        public TextEditor()
        {
            this.TextMode = TextBoxMode.MultiLine;
            this.CssClass = "NormalTextBox";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Control Image Folder
        /// </summary>
        /// <value>The image folder</value>
        public string ImageFolder
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        #endregion
    }
}