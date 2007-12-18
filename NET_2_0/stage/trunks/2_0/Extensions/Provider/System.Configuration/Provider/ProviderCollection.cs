using System.Collections;

namespace System.Configuration.Provider
{
	/// <summary>
	/// Summary description for ProviderCollection.
	/// </summary>
	public class ProviderCollection : CollectionBase
	{
		/// <summary>
		/// 
		/// </summary>
		public ProviderCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public ProviderCollection(ProviderBase[] value)
		{
			this.AddRange(value);
		}

		/// <summary>
		/// 
		/// </summary>
		public ProviderBase this[int index]
		{
			get { return ((ProviderBase) base.List[index]); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(ProviderBase value)
		{
			return base.List.Add(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(ProviderBase[] value)
		{
			foreach (ProviderBase provider in value)
			{
				this.Add(provider);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(ProviderCollection value)
		{
			foreach (ProviderBase provider in value)
			{
				this.Add(provider);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(ProviderBase value)
		{
			return base.List.Contains(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(ProviderBase[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(ProviderBase value)
		{
			return base.List.IndexOf(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, ProviderBase value)
		{
			base.List.Insert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Remove(ProviderBase value)
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
			ProviderBase provider;
			int k = base.List.Count - 1;
			for (i = 0; (i <= k); i++)
			{
				provider = ((ProviderBase) base.List[i]);
				if (provider.Name.Equals(value))
					base.List.RemoveAt(i);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public class ProviderEnumerator : IEnumerator
		{
			private IEnumerator _enumerator;

			/// <summary>
			/// Implement the IEnumerator interface member explicitly.
			/// </summary>
			object IEnumerator.Current
			{
				get { return this._enumerator.Current; }
			}

			/// <summary>
			/// Implement the strongly typed member.    
			/// </summary>
			/// <value>
			///     <para>
			///         
			///     </para>
			/// </value>
			/// <remarks>
			///     
			/// </remarks>
			public ProviderBase Current
			{
				get { return (ProviderBase) this._enumerator.Current; }
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="mappings"></param>
			public ProviderEnumerator(ProviderCollection mappings)
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