using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Rainbow.Framework.Design;
using Rainbow.Framework.Scheduler;

namespace Rainbow.Framework.Data.Entities
{
    /// <summary>
    /// Portal Interface
    /// </summary>
    public interface IPortal : IEntity, ISecuredEntity, IComparable<IPortal>
    {
        /// <summary>
        /// Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the portal alias.
        /// </summary>
        /// <value>The portal alias.</value>
        string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is always show edit button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is always show edit button; otherwise, <c>false</c>.
        /// </value>
        bool IsAlwaysShowEditButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show pages.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is show pages; otherwise, <c>false</c>.
        /// </value>
        bool IsShowPages { get; set; }

        /// <summary>
        /// Gets or sets the current layout.
        /// </summary>
        /// <value>The current layout.</value>
        ILayout Layout { get; set; }

        /// <summary>
        /// Gets or sets the theme primary.
        /// </summary>
        /// <value>The theme primary.</value>
        ITheme ThemePrimary { get; set; }

        /// <summary>
        /// Gets or sets the theme secondary.
        /// </summary>
        /// <value>The theme secondary.</value>
        ITheme ThemeSecondary { get; set; }

        /// <summary>
        /// Gets or sets the terms of service.
        /// </summary>
        /// <value>The terms of service.</value>
        string TermsOfService { get; set; }


        /// <summary>
        /// Gets or sets the portal content language.
        /// </summary>
        /// <value>The portal content language.</value>
        CultureInfo ContentLanguage { get; set; }

        /// <summary>
        /// Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        CultureInfo UILanguage { get; set; }

        /// <summary>
        /// Gets or sets the portal data formatting culture.
        /// </summary>
        /// <value>The portal data formatting culture.</value>
        CultureInfo DataFormattingCulture { get; set; }

        #region Collections

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        IEnumerable<IPage> Pages { get; }

        /// <summary>
        /// Gets the pages mobile.
        /// </summary>
        /// <value>The pages mobile.</value>
        IEnumerable<IPage> PagesMobile { get; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        IEnumerable<IPortalSetting> Settings { get; }

        #endregion
    }
}
