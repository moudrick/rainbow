namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    /// <summary>
    /// Rainbow.Framework.Web.UI.WebControls Inherits and extends
    ///     <see cref="System.Web.UI.WebControls.LinkButton"/>
    ///     We add properties for default text, and TextKey which results in a search for resources.
    /// </summary>
    [History("Jonathan F. Minond", "2/2/2006", "Created to extend asp.net 2.0 control Localize")]
    [DefaultProperty("TextKey")]
    [ToolboxData("<{0}:LinkButton TextKey='' runat=server></{0}:LinkButton>")]
    public class LinkButton : System.Web.UI.WebControls.LinkButton
    {
        #region Constants and Fields

        /// <summary>
        /// The defaultText.
        /// </summary>
        private string defaultText = string.Empty;

        /// <summary>
        /// The key string.
        /// </summary>
        private string key = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a defeault text value
        /// </summary>
        /// <value></value>
        /// <returns>The text caption displayed on the <see cref = "T:System.Web.UI.WebControls.LinkButton"></see> control. The default value is an empty string ("").</returns>
        [ToolboxItem("text")]
        public override string Text
        {
            get
            {
                return this.defaultText;
            }

            set
            {
                this.defaultText = value;
            }
        }

        /// <summary>
        ///     Gets or sets the resource key
        /// </summary>
        /// <value>The text key.</value>
        [ToolboxItem("textkey")]
        public string TextKey
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Before rendering, set the keys for the text
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> object that contains the event data.
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            base.Text = this.key.Length != 0 || this.defaultText.Length <= 0
                            ? General.GetString(this.key, this.defaultText)
                            : this.defaultText;

            base.OnPreRender(e);
        }

        #endregion
    }
}