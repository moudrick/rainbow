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
    public interface IPortal : IComparable, IComparable<IPortal>, IConvertible
    {
        // portal name handled by portal title
        //string PortalName { get; set; }

        //portal path information irrelevant to rest of application. should only be used within data implementation
        //string PortalPath { get; set; }
        //string PortalPathFull { get; set; }
        //string PortalPathRelative { get; }
        //string PortalSecurePath { get; }

        //TODO: this is just not implemented... is it in use?
        //XmlDocument PortalPagesXml { get; }

        /// <summary>
        /// Gets or sets the portal id.
        /// </summary>
        /// <value>The portal id.</value>
        int PortalId { get; set; }

        
        /// <summary>
        /// Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        string PortalTitle { get; set; }

        /// <summary>
        /// Gets or sets the portal alias.
        /// </summary>
        /// <value>The portal alias.</value>
        string PortalAlias { get; set; }

        
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
        string CurrentLayout { get; set; }

        /// <summary>
        /// Gets or sets the current theme default.
        /// </summary>
        /// <value>The current theme default.</value>
        Theme CurrentThemeDefault { get; set; }

        /// <summary>
        /// Gets or sets the current theme alternate.
        /// </summary>
        /// <value>The current theme alternate.</value>
        Theme CurrentThemeAlternate { get; set; }

        /// <summary>
        /// Gets or sets the terms of service.
        /// </summary>
        /// <value>The terms of service.</value>
        string TermsOfService { get; set; }


        /// <summary>
        /// Gets or sets the portal content language.
        /// </summary>
        /// <value>The portal content language.</value>
        CultureInfo PortalContentLanguage { get; set; }

        /// <summary>
        /// Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        CultureInfo PortalUILanguage { get; set; }
        
        /// <summary>
        /// Gets or sets the portal data formatting culture.
        /// </summary>
        /// <value>The portal data formatting culture.</value>
        CultureInfo PortalDataFormattingCulture { get; set; }

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
    }
}
