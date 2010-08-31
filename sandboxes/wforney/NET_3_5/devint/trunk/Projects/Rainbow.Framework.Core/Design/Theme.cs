namespace Rainbow.Framework.Design
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls.WebParts;

    using Rainbow.Framework.Interfaces;

    using Image = System.Web.UI.WebControls.Image;

    /// <summary>
    /// The Theme class encapsulates all the settings
    ///     of the currently selected theme
    /// </summary>
    /// <remarks>
    /// WLF: Themes are going to be completely different under the new system. I am realizing how limiting they are right now.
    /// </remarks>
    [History("bja", "2003/04/26", "C1: [Future] Added minimize color for title bar")]
    public class Theme : ITheme
    {
        #region Constants and Fields

        /// <summary>
        /// The button path.
        /// </summary>
        private string buttonPath;

        /// <summary>
        /// The image path.
        /// </summary>
        private string imagePath;

        /// <summary>
        /// The images.
        /// </summary>
        private Dictionary<string, Image> images;

        /// <summary>
        /// The style sheet file name.
        /// </summary>
        private string styleSheetFileName = "Portal.css";

        /// <summary>
        /// The style sheet path.
        /// </summary>
        private string styleSheetPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Theme"/> class.
        /// </summary>
        public Theme()
        {
            this.Type = "classic";
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the button directory
        /// </summary>
        /// <value>The button path.</value>
        public string ButtonPath
        {
            get
            {
                return string.IsNullOrEmpty(this.buttonPath) ? this.DefaultButtonPath : this.buttonPath;
            }

            set
            {
                this.buttonPath = value;
            }
        }

        /// <summary>
        ///     Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Gets the default button path.
        /// </summary>
        /// <value>The default button path.</value>
        public string DefaultButtonPath
        {
            get
            {
                return "~/Design/Themes/Default/icon";
            }
        }

        /// <summary>
        ///     Gets the default image path.
        /// </summary>
        /// <value>The default image path.</value>
        public string DefaultImagePath
        {
            get
            {
                return "~/Design/Themes/Default/img";
            }
        }

        /// <summary>
        ///     Gets the default CSS path.
        /// </summary>
        /// <value>The default CSS path.</value>
        public string DefaultStyleSheetPath
        {
            get
            {
                return string.Format("~/Design/Themes/{0}/mod", this.Name);
            }
        }

        /// <summary>
        ///     Gets or sets the Theme physical file name.
        ///     Set at runtime using Physical Path. NonSerialized.
        /// </summary>
        /// <value>The name of the theme file.</value>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.WebPath.ToString()))
                {
                    throw new ArgumentNullException("this.WebPath", "Value cannot be null!");
                }

                // Try to get current theme from public folder
                return System.IO.Path.Combine(this.Path, "Theme.xml");
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The theme id.</value>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the image directory
        /// </summary>
        /// <value>The image path.</value>
        public string ImagePath
        {
            get
            {
                return string.IsNullOrEmpty(this.imagePath) ? this.DefaultImagePath : this.imagePath;
            }

            set
            {
                this.imagePath = value;
            }
        }

        /// <summary>
        ///     Gets the images.
        /// </summary>
        /// <value>The images.</value>
        public Dictionary<string, Image> Images
        {
            get
            {
                return this.images ?? (this.images = new Dictionary<string, Image>());
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     This should be set true if you want to delete something. The record should only be removed from the database after being
        ///     dumped from the recycler. We need a Destroy function on the data source for the actual deletion.
        /// </remarks>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified { get; set; }

        /// <summary>
        ///     Gets or sets the color of the minimize.
        /// </summary>
        /// <value>The color of the minimize.</value>
        public Color MinimizeColor { get; set; }

        /// <summary>
        ///     Gets or sets the Theme Name (must be the directory in which is located)
        /// </summary>
        /// <value>The theme name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the object type id.
        /// </summary>
        /// <value>The object type id.</value>
        public Guid ObjectTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the parts.
        /// </summary>
        /// <value>The parts.</value>
        public Hashtable Parts { get; set; }

        /// <summary>
        ///     Gets or sets Current Phisical Path. Readonly.
        /// </summary>
        /// <value>The theme path.</value>
        public string Path
        {
            get
            {
                return HttpContext.Current.Server.MapPath(this.WebPath.ToString());
            }

            set
            {
                throw new NotSupportedException("Cannot set path here. It is calculated from theme folder.");
            }
        }

        /// <summary>
        ///     Gets or sets the name of the style sheet file.
        /// </summary>
        /// <value>The name of the style sheet file.</value>
        public string StyleSheetFileName
        {
            get
            {
                return Configuration.Path.WebPathCombine(this.WebPath.ToString(), this.styleSheetFileName);
            }

            set
            {
                this.styleSheetFileName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the CSS directory
        /// </summary>
        /// <value>The CSS path.</value>
        public string StyleSheetPath
        {
            get
            {
                return string.IsNullOrEmpty(this.styleSheetPath) ? this.WebPath.ToString() : this.styleSheetPath;
            }

            set
            {
                this.styleSheetPath = value;
            }
        }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The theme type.</value>
        /// <remarks>
        ///     classic/zen/new
        /// </remarks>
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets Current Web Path.
        /// </summary>
        /// <value>The web path.</value>
        public Uri WebPath { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="name">
        /// The name..
        /// </param>
        /// <returns>
        /// The image.
        /// </returns>
        public Image GetImage(string name)
        {
            return this.GetImage(name, this.DefaultImagePath);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="name">
        /// The name..
        /// </param>
        /// <param name="imagePath">
        /// The image path.
        /// </param>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        public Image GetImage(string name, string imagePath)
        {
            Image img;

            if (this.Images.ContainsKey(name))
            {
                img = this.Images[name];
                img.ImageUrl = Configuration.Path.WebPathCombine(this.WebPath.ToString(), img.ImageUrl);
            }
            else
            {
                img = new Image
                    {
                        ImageUrl =
                            Configuration.Path.WebPathCombine(
                                this.DefaultButtonPath.Replace("~", Configuration.Path.ApplicationRoot), imagePath)
                    };
            }

            return img;
        }

        /// <summary>
        /// Gets the literal image.
        /// </summary>
        /// <param name="name">
        /// The name..
        /// </param>
        /// <param name="defaultImagePath">
        /// The default image path.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string GetImageHtml(string name, string defaultImagePath)
        {
            var img = this.GetImage(name, defaultImagePath);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var xtw = new XhtmlTextWriter(sw);

            img.RenderControl(xtw);

            return sb.ToString();

            // return string.Format("<img alt=\"\" src=\"{0}\" width=\"{1}\" height=\"{2}\">",
            // img.ImageUrl, img.Width.ToString(), img.Height.ToString());
        }

        /// <summary>
        /// Gets the literal control.
        /// </summary>
        /// <param name="name">
        /// The name..
        /// </param>
        /// <returns>
        /// A System.Web.UI.LiteralControl value...
        /// </returns>
        public LiteralControl GetLiteralControl(string name)
        {
            return new LiteralControl(this.GetThemePart(name));
        }

        /// <summary>
        /// Gets the theme part.
        /// </summary>
        /// <param name="name">
        /// The name..
        /// </param>
        /// <returns>
        /// The get theme part.
        /// </returns>
        /// <remarks>
        /// added: Jes1111 - 2004/08/27
        ///     Part of Zen support
        /// </remarks>
        public string GetThemePart(string name)
        {
            if (!this.Parts.ContainsKey(name))
            {
                return string.Empty;
            }

            var part = (Part)this.Parts[name];

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var xtw = new XhtmlTextWriter(sw);

            part.RenderControl(xtw);

            return sb.ToString();
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance. 
        /// </exception>
        public int CompareTo(object obj)
        {
            return this.Name.CompareTo(((ITheme)obj).Name);
        }

        #endregion

        #region IComparable<ITheme>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ITheme other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion

        #region IConvertible

        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
        /// </returns>
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A Boolean value equivalent to the value of this instance.
        /// </returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 8-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A Unicode character equivalent to the value of this instance.
        /// </returns>
        public char ToChar(IFormatProvider provider)
        {
            return this.Name.ToCharArray(0, 1)[0];
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
        /// </returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return this.CreatedOn;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 16-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 32-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 64-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 8-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return this.Name;
        }

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="conversionType">
        /// The <see cref="T:System.Type"/> to which the value of this instance is converted.
        /// </param>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.
        /// </returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 16-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 32-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// An 64-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}