using System;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace Rainbow.Framework.Design
{
    /// <summary>
    /// A single named Image
    /// </summary>
    [Serializable]
    public class ThemeImage
    {
        double height;
        string imageUrl;
        string name;
        double width;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeImage"/> class.
        /// </summary>
        internal ThemeImage()
        {
        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ThemeImage"/> class.
//        /// </summary>
//        /// <param name="name">The name.</param>
//        /// <param name="imageUrl">The image URL.</param>
//        /// <param name="width">The width.</param>
//        /// <param name="height">The height.</param>
//        public ThemeImage(string name, string imageUrl, double width, double height)
//        {
//            this.name = name;
//            this.imageUrl = imageUrl;
//            this.width = width;
//            this.height = height;
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ThemeImage"/> class.
//        /// </summary>
//        /// <param name="name">The name.</param>
//        /// <param name="img">The img.</param>
//        public ThemeImage(string name, Image img)
//        {
//            this.name = name;
//            imageUrl = img.ImageUrl;
//            width = img.Width.Value;
//            height = img.Height.Value;
//        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        public Image GetImage()
        {
            using (Image img = new Image())
            {
                img.ImageUrl = ImageUrl;
                img.Width = new Unit(Width);
                img.Height = new Unit(Height);
                return img;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [XmlAttribute]
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        [XmlAttribute]
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute(DataType = "string")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [XmlAttribute]
        public double Width
        {
            get { return width; }
            set { width = value; }
        }
    }
}
