namespace Rainbow.Framework.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Rainbow.Framework.Data.DataSources;
    using Rainbow.Framework.Interfaces;

    /// <summary>
    /// Generic Portals Data Access Class
    /// </summary>
    public static class Portals
    {
        #region Constants and Fields

        /// <summary>
        ///     source provider instance
        /// </summary>
        private static readonly PortalProvider Source = PortalProvider.Instance();

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a Portal to the data source.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="contentLanguage">
        /// The content Language.
        /// </param>
        /// <param name="createdOn">
        /// The created On.
        /// </param>
        /// <param name="dataFormattingCulture">
        /// The data Formatting Culture.
        /// </param>
        /// <param name="isAlwaysShowEditButton">
        /// The is Always Show Edit Button.
        /// </param>
        /// <param name="isDeleted">
        /// The is Deleted.
        /// </param>
        /// <param name="isShowPages">
        /// The is Show Pages.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="objectTypeId">
        /// The object Type Id.
        /// </param>
        /// <param name="tos">
        /// The tos.
        /// </param>
        /// <param name="themePrimary">
        /// The theme Primary.
        /// </param>
        /// <param name="themeSecondary">
        /// The theme Secondary.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="uiLanguage">
        /// The ui Language.
        /// </param>
        public static void Add(
            Guid id, 
            string alias, 
            CultureInfo contentLanguage, 
            DateTime? createdOn, 
            CultureInfo dataFormattingCulture, 
            bool? isAlwaysShowEditButton, 
            bool? isDeleted, 
            bool? isShowPages, 
            ILayout layout, 
            Guid objectTypeId, 
            string tos, 
            ITheme themePrimary, 
            ITheme themeSecondary, 
            string title, 
            CultureInfo uiLanguage)
        {
            // add params as appropriate
            var record = Source.CreateNew(); // create new entity in memory

            // add values
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

            Source.Add(record); // add record to data source in memory
            Source.CommitChanges(); // write changes back to data source
        }

        /// <summary>
        /// Loads all.
        /// </summary>
        /// <returns>List of IPortal</returns>
        public static List<IPortal> LoadAll()
        {
            return new List<IPortal>(Source.GetAll());
        }

        /// <summary>
        /// The load by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// An IPortal
        /// </returns>
        public static IPortal LoadById(Guid id)
        {
            return Source.GetById(id);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public static void Remove(Guid id)
        {
            var record = Source.GetById(id); // grab portal from data source as IPortal interface object

            Source.Remove(record); // delete record from data source in memory
            Source.CommitChanges(); // write changes back to data source
        }

        #endregion
    }
}