﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;
using Rainbow.Framework.Data.DataSources;
using System.Threading;
using System.Globalization;

namespace Rainbow.Framework.Data
{
    /// <summary>
    /// Generic Portals Data Access Class
    /// </summary>
    public partial class Portals
    {
        static PortalProvider source = PortalProvider.Instance();

        /// <summary>
        /// Add a Portal to the data source.
        /// </summary>
        /// <param name="Id">The portal id.</param>
        /// <param name="name">The name.</param>
        static void Add(Guid id, string alias, CultureInfo contentLanguage, DateTime? createdOn,
            CultureInfo dataFormattingCulture, bool? isAlwaysShowEditButton, bool? isDeleted, bool? isShowPages,
            ILayout layout, Guid objectTypeId, string tos, ITheme themePrimary, ITheme themeSecondary,
            string title, CultureInfo uiLanguage) //add params as appropriate
        {
            IPortal record = source.CreateNew() as IPortal;  //create new entity in memory

            //add values
            record.Id = id;
            record.Alias = alias;
            record.ContentLanguage = contentLanguage;
            record.CreatedOn = createdOn ?? DateTime.Now;
            record.DataFormattingCulture = dataFormattingCulture;
            record.IsAlwaysShowEditButton = isAlwaysShowEditButton ?? false;
            record.IsDeleted = isDeleted ?? false;
            record.IsShowPages = isShowPages ?? true;
            record.Layout = layout;
            record.ObjectTypeId = objectTypeId;
            record.TermsOfService = tos ?? string.Empty;
            record.ThemePrimary = themePrimary;
            record.ThemeSecondary = themeSecondary;
            record.Title = title ?? string.Empty;
            record.UILanguage = uiLanguage;

            source.Add(ref record);     //add record to data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        static void Remove(Guid Id)
        {
            IPortal record = source.GetById(Id) as IPortal;  //grab portal from data source as IPortal interface object

            source.Remove(record);      //delete record from data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        static List<IPortal> LoadAll()
        {
            return source.GetAll() as List<IPortal>;
        }

        static IPortal LoadById(Guid id)
        {
            return source.GetById(id);
        }
    }
}
