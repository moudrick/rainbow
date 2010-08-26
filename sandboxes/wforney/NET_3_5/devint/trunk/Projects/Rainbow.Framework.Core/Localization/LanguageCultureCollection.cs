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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// LanguageCultureCollection
    /// </summary>
    [TypeConverter(typeof(TypeConverterLanguageCultureCollection))]
    public class LanguageCultureCollection : Collection<LanguageCultureItem>
    {
        private readonly char[] itemsSeparator = { ';' };
        private readonly char[] keyValueSeparator = { '=' };

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureCollection"/> class.
        /// </summary>
        public LanguageCultureCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureCollection"/> class.
        /// </summary>
        /// <param name="LanguageCultureCollection">The language culture collection.</param>
        public LanguageCultureCollection(string LanguageCultureCollection)
        {
            LanguageCultureCollection mylist = (LanguageCultureCollection)LanguageCultureCollection;

            foreach (LanguageCultureItem l in mylist)
                Add(l);
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
            foreach (LanguageCultureItem item in this)
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
            List<CultureInfo> cultures = new List<CultureInfo>();
            cultures.Add(culture);
            return GetBestMatching(cultures);
        }

        /// <summary>
        /// Returns the best possible LanguageCultureItem
        /// matching cultures in provided list
        /// </summary>
        /// <param name="cultures">The cultures.</param>
        /// <returns></returns>
        public LanguageCultureItem GetBestMatching(List<CultureInfo> cultures)
        {
            //If null return default
            if (cultures == null || cultures.Count == 0 || cultures[0] == null)
                return (LanguageCultureItem)this[0];

            //First pass, exact match
            foreach (CultureInfo culture in cultures)
            {
                foreach (LanguageCultureItem lci in this)
                {
                    if (culture.Name == lci.UICulture.Name)
                        return (LanguageCultureItem)lci;
                }
            }
            //Second pass, we may accept a parent match
            foreach (CultureInfo culture in cultures)
            {
                foreach (LanguageCultureItem lci in this)
                {
                    if (culture.Parent.Name == lci.UICulture.Name)
                        return lci;
                }
            }
            return null; //no applicable match
        }

        /// <summary>
        /// Returns a CultureInfo list matching language property
        /// </summary>
        /// <param name="addInvariantCulture">If true adds a row containing invariant culture</param>
        /// <returns></returns>
        public List<CultureInfo> ToUICultureArray(bool addInvariantCulture)
        {
            List<CultureInfo> cultures = new List<CultureInfo>();
            if (addInvariantCulture)
                cultures.Add(CultureInfo.InvariantCulture);

            foreach (LanguageCultureItem lci in this)
            {
                cultures.Add(lci.UICulture);
            }
            return cultures;
        }

        /// <summary>
        /// Returns a CultureInfo list matching language property
        /// </summary>
        /// <returns></returns>
        public List<CultureInfo> ToUICultureArray()
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
            return (LanguageCultureCollection)TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(languageList, typeof(LanguageCultureCollection));
        }

        /// <summary>
        /// Explicitly converts LanguageCultureCollection to String value
        /// </summary>
        /// <param name="langList">The lang list.</param>
        /// <returns>The result of the conversion.</returns>
        static public explicit operator String(LanguageCultureCollection langList)
        {
            return (string)TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(langList, typeof(string));
        }
    }
}
