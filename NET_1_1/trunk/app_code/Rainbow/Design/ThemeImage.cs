using System;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace Rainbow.Design
{

	/// <summary>
	/// A single named Image
	/// </summary>
	[Serializable]
	public class ThemeImage
	{

		private double _Height;
		private string _ImageUrl;
		private string _Name;
		private double _Width;

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemeImage()
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="imageUrl" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="width" type="double">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="height" type="double">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemeImage(string name, string imageUrl, double width, double height)
		{
			_Name = name;
			_ImageUrl = imageUrl;
			_Width = width;
			_Height = height;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="img" type="System.Web.UI.WebControls.Image">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemeImage(string name, Image img)
		{
			_Name = name;
			_ImageUrl = img.ImageUrl;
			_Width = img.Width.Value;
			_Height = img.Height.Value;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A System.Web.UI.WebControls.Image value...
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
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute]
		public double Height
		{
			get {return _Height;}
			set {_Height = value;}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute]
		public string ImageUrl
		{
			get {return _ImageUrl;}
			set {_ImageUrl = value;}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute(DataType = "string")]
		public string Name
		{
			get {return _Name;}
			set {_Name = value;}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute]
		public double Width
		{
			get {return _Width;}
			set {_Width = value;}
		}
	}
}
