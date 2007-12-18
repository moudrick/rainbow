using System.Collections.Specialized;

namespace System.Configuration.Provider
{
	/// <summary>
	/// Summary description for ProviderBase.
	/// </summary>
	public abstract class ProviderBase
	{
		private string _name;

		/// <summary>
		/// 
		/// </summary>
		public virtual string Name
		{
			get { return this._name; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected ProviderBase()
		{
		}

		private bool _Initialized;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="config"></param>
		public virtual void Initialize(string name, NameValueCollection config)
		{
			lock (this)
			{
				if (this._Initialized)
					throw new InvalidOperationException("Provider Already Initialized");
				this._Initialized = true;
			}
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("Config provider name null or empty", "name");
			this._name = name;
		}
	}
}