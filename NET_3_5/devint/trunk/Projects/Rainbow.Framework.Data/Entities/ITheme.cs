using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Web.UI;

namespace Rainbow.Framework.Data.Entities
{
    /// <summary>
    /// The majority of this could be obsoleted by built-in theme support in ASP.NET v2 and some little programming tricks
    /// to programmatically switch themes. We still need to have this for persistence into the database (eventually).
    /// </summary>
    public interface ITheme : IEntity, IComparable<ITheme>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        #region Paths & Files

        /// <summary>
        /// Gets or sets the web path.
        /// </summary>
        /// <value>The web path.</value>
        Uri WebPath { get; set; }

        /// <summary>
        /// Current Phisical Path. Readonly.
        /// </summary>
        /// <value>The path.</value>
        System.IO.Path Path { get; set; }

        /// <summary>
        /// Get the Theme physical file name.
        /// Set at runtime using Physical Path. NonSerialized.
        /// </summary>
        /// <value>The name of the theme file.</value>
        string FileName { get; set; }

        #endregion

        #region Buttons

        /// <summary>
        /// Gets the default button path.
        /// </summary>
        /// <value>The default button path.</value>
        string DefaultButtonPath { get; }

        /// <summary>
        /// Gets or sets the button directory
        /// </summary>
        /// <value>The button path.</value>
        string ButtonPath { get; set; }

        #endregion

        #region StyleSheet

        /// <summary>
        /// Gets the default CSS path.
        /// </summary>
        /// <value>The default CSS path.</value>
        string DefaultStyleSheetPath { get; }

        /// <summary>
        /// Gets or sets the CSS directory
        /// </summary>
        /// <value>The CSS path.</value>
        string StyleSheetPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the style sheet file.
        /// </summary>
        /// <value>The name of the style sheet file.</value>
        string StyleSheetFileName { get; set; }

        #endregion

        #region Images

        /// <summary>
        /// Gets the default image path.
        /// </summary>
        /// <value>The default image path.</value>
        string DefaultImagePath { get; }

        /// <summary>
        /// Gets or sets the image directory
        /// </summary>
        /// <value>The image path.</value>
        string ImagePath { get; set; }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <value>The images.</value>
        Dictionary<string, System.Web.UI.WebControls.Image> Images { get; }

        #endregion

        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>The parts.</value>
        Hashtable Parts { get; set; }


        /// <summary>
        /// Gets or sets the color of the minimize.
        /// </summary>
        /// <value>The color of the minimize.</value>
        Color MinimizeColor { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <remarks>
        /// classic/zen/new
        /// </remarks>
        /// <value>The type.</value>
        string Type { get; set; }
    }
}
