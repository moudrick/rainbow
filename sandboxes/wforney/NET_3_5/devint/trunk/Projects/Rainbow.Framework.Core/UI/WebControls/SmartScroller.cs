namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// You can place this control an an aspx page (DesktopDefault.aspx for example) and it will retain scroll position on postback
    /// </summary>
    /// <remarks>
    /// This is a control found on the web but unfortunately I've lost the site address. If found please add here to give credit.
    /// </remarks>
    public class SmartScroller : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The html form.
        /// </summary>
        private HtmlForm theForm = new HtmlForm();

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.theForm = this.GetServerForm(this.Page.Controls);

            var hidScrollLeft = new HtmlInputHidden { ID = "scrollLeft" };

            var hidScrollTop = new HtmlInputHidden { ID = "scrollTop" };

            this.Controls.Add(hidScrollLeft);
            this.Controls.Add(hidScrollTop);

            var scriptString =
                @"
<script language = ""javascript"">
<!--
  function smartScroller_GetCoords()
  {
    var scrollX, scrollY;
    if (document.all)
    {
      if (!document.documentElement.scrollLeft)
        scrollX = document.body.scrollLeft;
      else
        scrollX = document.documentElement.scrollLeft;

      if (!document.documentElement.scrollTop)
        scrollY = document.body.scrollTop;
      else
        scrollY = document.documentElement.scrollTop;
    }
    else
    {
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
    }
    document.forms[""" +
                this.theForm.ClientID + @"""]." + hidScrollLeft.ClientID + @".value = scrollX;
    document.forms[""" +
                this.theForm.ClientID + @"""]." + hidScrollTop.ClientID +
                @".value = scrollY;
  }


  function smartScroller_Scroll()
  {
    var x = document.forms[""" +
                this.theForm.ClientID + @"""]." + hidScrollLeft.ClientID + @".value;
    var y = document.forms[""" +
                this.theForm.ClientID + @"""]." + hidScrollTop.ClientID +
                @".value;
    window.scrollTo(x, y);
  }

  
  window.onload = smartScroller_Scroll;
  window.onscroll = smartScroller_GetCoords;
  window.onclick = smartScroller_GetCoords;
  window.onkeypress = smartScroller_GetCoords;
// -->
</script>";

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SmartScroller", scriptString);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            this.Page.VerifyRenderingInServerForm(this);
            base.Render(writer);
        }

        /// <summary>
        /// Gets the server form.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The html form.</returns>
        private HtmlForm GetServerForm(ControlCollection parent)
        {
            foreach (Control child in parent)
            {
                var t = child.GetType();
                if (t == typeof(HtmlForm))
                {
                    return (HtmlForm)child;
                }

                if (t == typeof(CustomForm))
                {
                    return (CustomForm)child;
                }

                if (child.HasControls())
                {
                    return this.GetServerForm(child.Controls);
                }
            }

            return new HtmlForm();
        }

        #endregion
    }
}