using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    /// <summary>
    /// Interface for a Page
    /// </summary>
    /// <remarks>
    /// Pages can hold settings, modules, other pages.
    /// Pages can have multiple parent pages within a single portal/site.
    ///     This allows the same page to be reused in multiple places on a single portal/site.
    /// Pages can have multiple parent portals (sites) that they belong to.
    ///     This allows the same page to be reused in multiple sites as well such that 3 domains could have the same contact page.
    /// </remarks>
    public interface IPage : IEntity, ISecuredEntity, IComparable<IPage>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        #region Mobile Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show mobile.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is show mobile; otherwise, <c>false</c>.
        /// </value>
        bool IsShowMobile { get; set; }

        /// <summary>
        /// Gets or sets the name mobile.
        /// </summary>
        /// <value>The name mobile.</value>
        string NameMobile { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets or sets the authorized roles.
        /// </summary>
        /// <value>The authorized roles.</value>
        string AuthorizedRoles { get; set; }

        /// <summary>
        /// Currents the theme.
        /// </summary>
        /// <returns></returns>
        ITheme Theme { get; set; }

        #region Navigation Related

        /// <summary>
        /// Gets or sets the menu image.
        /// </summary>
        /// <value>The menu image.</value>
        string MenuImage { get; set; }

        /// <summary>
        /// Gets the menu group.
        /// </summary>
        /// <value>The menu group.</value>
        IEnumerable<IPage> MenuGroup { get; }

        /// <summary>
        /// Gets or sets the nest level.
        /// </summary>
        /// <value>The nest level.</value>
        int NestLevel { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        ILayout Layout { get; set; }

        #region Collections

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <value>The modules.</value>
        IEnumerable<IModule> Modules { get; }

        /// <summary>
        /// Gets the page settings.
        /// </summary>
        /// <value>The page settings.</value>
        IEnumerable<IPageSetting> PageSettings { get; }

        /// <summary>
        /// Gets the child pages.
        /// </summary>
        /// <value>The child pages.</value>
        IEnumerable<IPage> ChildPages { get; }

        /// <summary>
        /// Gets the parent pages.
        /// </summary>
        /// <value>The parent pages.</value>
        IEnumerable<IPage> ParentPages { get; }

        /// <summary>
        /// Gets the parent portals.
        /// </summary>
        /// <value>The parent portals.</value>
        IEnumerable<IPortal> ParentPortals { get; }

        #endregion
    }
}
