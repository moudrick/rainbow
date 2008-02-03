using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    public interface IModule : IEntity, ISecuredEntity, IComparable<IModule>
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is collapsable; otherwise, <c>false</c>.
        /// </value>
        bool IsCollapsable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show every where.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is show every where; otherwise, <c>false</c>.
        /// </value>
        bool IsShowEveryWhere { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show mobile.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is show mobile; otherwise, <c>false</c>.
        /// </value>
        bool IsShowMobile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is workflow supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is workflow supported; otherwise, <c>false</c>.
        /// </value>
        bool IsWorkflowSupported { get; set; }

        #region workflow - redesign how this works
        byte? WorkflowState { get; set; }
        string StagingLastEditor { get; set; }
        DateTime StagingLastModified { get; set; }
        bool IsNewVersion { get; set; }
        #endregion

        #region Collections

        /// <summary>
        /// Gets the parent pages.
        /// </summary>
        /// <value>The parent pages.</value>
        IEnumerable<IPage> ParentPages { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        IEnumerable<IItem> Items { get; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        IEnumerable<IModuleSetting> Settings { get; }

        #endregion
    }
}
