using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Rainbow.Framework.Design
{
	/// <summary>
	/// Picture Item
	/// </summary>
	public class PictureItem : UserControl
    //TODO: [moudrick] move it from Framewortk Core to Rainbow Web Controls library
	{
        XmlDocument metadata;

		protected HyperLink editLink;

	    /// <summary>
	    /// Gets the metadata.
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <returns>A string value...</returns>
	    public string GetMetadata(string key)
	    {
	        XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);
	        if (targetNode == null)
	        {
	            return null;
	        }
	        else
	        {
	            return targetNode.Value;
	        }
	    }

	    /// <summary>
	    /// Gets or sets the metadata.
	    /// </summary>
	    /// <value>The metadata.</value>
	    public XmlDocument Metadata
	    {
	        get { return metadata; }
	        set { metadata = value; }
	    }

	    #region Web Form Designer generated code
	    /// <summary>
	    /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
	    /// </summary>
	    /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
	    override protected void OnInit(EventArgs e)
	    {
	        base.OnInit(e);
	    }
	    #endregion
	}
}
