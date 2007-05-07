using System.Collections.Specialized;
using System.Xml;

namespace System.Configuration
{
	/// <summary>
	/// ProviderSettings
	/// </summary>
	public class ProviderSettings
	{
		private string _name;
		private string _providerType;
		private NameValueCollection _propertyNameCollection = new NameValueCollection();

		/// <summary>
		/// Gets the _name of the provider
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { throw new NotImplementedException("ProviderSettings.Name"); }
		}

		/// <summary>
		/// Gets the type of the provider
		/// </summary>
		public string Type
		{
			get { return _providerType; }
			set { throw new NotImplementedException("ProviderSettings.Type"); }
		}

		/// <summary>
		/// Gets a collection of user-defined parameters for the provider
		/// </summary>
		public NameValueCollection Parameters
		{
			get { return _propertyNameCollection; }
		}

		/// <summary>
		/// NotImplemented
		/// </summary>
		public ProviderSettings()
		{
			throw new NotImplementedException("ProviderSettings.ctor");
		}

		/// <summary>
		/// Creates a new ProviderSettings instance with no user-defined parameters 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public ProviderSettings(string name, string type)
		{
			this._name = name;
			this._providerType = type;
		}

		/// <summary>
		/// Gets all attributes from current config excluding name and type
		/// </summary>
		/// <param name="attributes"></param>
		public ProviderSettings(XmlAttributeCollection attributes)
		{
			// Set the _name of the provider
			_name = attributes["name"].Value;

			// Set the type of the provider
			_providerType = attributes["type"].Value;

			// Store all the attributes in the attributes bucket
			foreach (XmlAttribute attribute in attributes)
			{
				if ((attribute.Name != "name") && (attribute.Name != "type"))
					_propertyNameCollection.Add(attribute.Name, attribute.Value);
			}
		}
	}
}