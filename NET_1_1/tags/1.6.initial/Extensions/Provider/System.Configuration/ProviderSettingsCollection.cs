using System.Collections;

namespace System.Configuration
{
	/// <summary>
	/// Summary description for ProviderSettingsCollection.
	/// </summary>
	public class ProviderSettingsCollection : CollectionBase
	{
		/// <summary>
		/// 
		/// </summary>
		public ProviderSettingsCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public ProviderSettingsCollection(ProviderSettings[] value)
		{
			this.AddRange(value);
		}

		/// <summary>
		/// 
		/// </summary>
		public ProviderSettings this[int index]
		{
			get { return ((ProviderSettings) base.List[index]); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(ProviderSettings value)
		{
			return base.List.Add(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(ProviderSettings[] value)
		{
			foreach (ProviderSettings providerSettings in value)
			{
				this.Add(providerSettings);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(ProviderSettingsCollection value)
		{
			foreach (ProviderSettings providerSettings in value)
			{
				this.Add(providerSettings);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(ProviderSettings value)
		{
			return base.List.Contains(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(ProviderSettings[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(ProviderSettings value)
		{
			return base.List.IndexOf(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, ProviderSettings value)
		{
			base.List.Insert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Remove(ProviderSettings value)
		{
			base.List.Remove(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Remove(string value)
		{
			int i;
			ProviderSettings providerSettings;
			int k = base.List.Count - 1;
			for (i = 0; (i <= k); i++)
			{
				providerSettings = ((ProviderSettings) base.List[i]);
				if (providerSettings.Name.Equals(value))
					base.List.RemoveAt(i);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public class ProviderSettingsEnumerator : IEnumerator
		{
			private IEnumerator _enumerator;

			/// <summary>
			/// 
			/// </summary>
			object IEnumerator.Current
			{
				get { return this._enumerator.Current; }
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
			public ProviderSettings Current
			{
				get { return (ProviderSettings) this._enumerator.Current; }
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="mappings"></param>
			public ProviderSettingsEnumerator(ProviderSettingsCollection mappings)
			{
				this._enumerator = mappings.GetEnumerator();
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				return this._enumerator.MoveNext();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Reset()
			{
				this._enumerator.Reset();
			}
		}
	}
}