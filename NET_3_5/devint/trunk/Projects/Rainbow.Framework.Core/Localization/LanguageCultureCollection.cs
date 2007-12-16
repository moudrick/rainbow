// Esperantus - The Web translator
// Copyright (C) 2003 Emmanuele De Andreis
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Emmanuele De Andreis (manu-dea@hotmail dot it)

using System;
using System.Reflection;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;

namespace Rainbow.Framework.Web.UI.WebControls
{
	/// <summary>
	/// LanguageCultureCollection
	/// </summary>
	[TypeConverter(typeof(TypeConverterLanguageCultureCollection))]
	public class LanguageCultureCollection : System.Collections.CollectionBase, ICollection
	{
		private readonly char[] itemsSeparator = {';'};
		private readonly char[] keyValueSeparator = {'='};

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureCollection"/> class.
        /// </summary>
		public LanguageCultureCollection()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureCollection"/> class.
        /// </summary>
        /// <param name="LanguageCultureCollection">The language culture collection.</param>
		public LanguageCultureCollection(string LanguageCultureCollection)
		{
			LanguageCultureCollection mylist = (LanguageCultureCollection) LanguageCultureCollection;

			foreach(LanguageCultureItem l in mylist)
				Add(l);
		}

        /// <summary>
        /// Gets the <see cref="Rainbow.Framework.Web.UI.WebControls.LanguageCultureItem"/> with the specified i.
        /// </summary>
        /// <value></value>
		public LanguageCultureItem this[Int32 i]
		{
			get{return (LanguageCultureItem) InnerList[i];}
//			set{InnerList[i] = value;}		
		}

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
		public void Add(LanguageCultureItem item)
		{
			InnerList.Add(item);
		}

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
		public void Insert(Int32 index, LanguageCultureItem item)
		{
			InnerList.Insert(index, item);
		}

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool Contains(LanguageCultureItem item)
		{
			return InnerList.Contains(item);
		}

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
		public virtual int IndexOf(LanguageCultureItem item)
		{
			return InnerList.IndexOf(item);
		}

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
		public void Remove(LanguageCultureItem item)
		{
			InnerList.Remove(item);
		}

		// Provide the explicit interface member for ICollection.
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			InnerList.CopyTo(array, arrayIndex);
		}

		// Provide the strongly typed member for ICollection.
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
		public void CopyTo(LanguageCultureItem[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach(LanguageCultureItem item in InnerList)
			{
				s.Append(item.UICulture.Name);
				s.Append(keyValueSeparator);
				s.Append(item.Culture.Name);
				s.Append(itemsSeparator);
			}
			return s.ToString();
		}

        /// <summary>
        /// Returns the best possible LanguageCultureItem
        /// matching the provided culture
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
		public LanguageCultureItem GetBestMatching(CultureInfo culture)
		{
			return GetBestMatching(new CultureInfo[] {culture});		
		}

        /// <summary>
        /// Returns the best possible LanguageCultureItem
        /// matching cultures in provided list
        /// </summary>
        /// <param name="cultures">The cultures.</param>
        /// <returns></returns>
		public LanguageCultureItem GetBestMatching(CultureInfo[] cultures)
		{
			//If null return default
			if (cultures == null || cultures.Length == 0 || cultures[0] == null)
				return (LanguageCultureItem) InnerList[0];

			//First pass, exact match
			foreach(CultureInfo culture in cultures)
			{
				for(Int32 i = 0; i < InnerList.Count; i++)
				{
					if (culture.Name == ((LanguageCultureItem) InnerList[i]).UICulture.Name)
						return (LanguageCultureItem) InnerList[i];
				}
			}
			//Second pass, we may accept a parent match
			foreach(CultureInfo culture in cultures)
			{
				for(Int32 i = 0; i < InnerList.Count; i++)
				{
					if (culture.Parent.Name == ((LanguageCultureItem) InnerList[i]).UICulture.Name)
						return (LanguageCultureItem) InnerList[i];
				}
			}
			return null; //no applicable match
		}

        /// <summary>
        /// Returns a CultureInfo list matching language property
        /// </summary>
        /// <param name="addInvariantCulture">If true adds a row containing invariant culture</param>
        /// <returns></returns>
		public CultureInfo[] ToUICultureArray(bool addInvariantCulture)
		{
			ArrayList cultures = new ArrayList();
			if (addInvariantCulture)
				cultures.Add(CultureInfo.InvariantCulture);

			for(Int32 i = 0; i < InnerList.Count; i++)
			{
				cultures.Add(((LanguageCultureItem) InnerList[i]).UICulture);
			}
			return (CultureInfo[]) cultures.ToArray(typeof(CultureInfo));
		}

        /// <summary>
        /// Returns a CultureInfo list matching language property
        /// </summary>
        /// <returns></returns>
		public CultureInfo[] ToUICultureArray()
		{
			return ToUICultureArray(false);
		}

        /// <summary>
        /// Explicitly converts String to LanguageCultureCollection value
        /// </summary>
        /// <param name="languageList">The language list.</param>
        /// <returns>The result of the conversion.</returns>
		static public explicit operator LanguageCultureCollection(string languageList)
		{
			return (LanguageCultureCollection) TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(languageList, typeof(LanguageCultureCollection));
		}

        /// <summary>
        /// Explicitly converts LanguageCultureCollection to String value
        /// </summary>
        /// <param name="langList">The lang list.</param>
        /// <returns>The result of the conversion.</returns>
		static public explicit operator String(LanguageCultureCollection langList)
		{
			return (string) TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(langList, typeof(string));
		}
	}
}
