using System;
using System.Xml.Serialization;

namespace Rainbow.Framework.Design
{
    /// <summary>
    /// A single named HTML fragment
    /// </summary>
    [Serializable]
    public class ThemePart
    {
        string html;
        string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemePart"/> class.
        /// </summary>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemePart()
        {
            name = string.Empty;
            html = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemePart"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="html">The HTML.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemePart(string name, string html)
        {
            this.name = name;
            this.html = html;
        }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        /// <remarks>
        /// </remarks>
        public string Html
        {
            get { return html; }
            set { html = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute(DataType = "string")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
