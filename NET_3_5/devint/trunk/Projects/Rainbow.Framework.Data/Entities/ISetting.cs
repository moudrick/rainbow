using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    /// <summary>
    /// ISetting is the basic interface for any type of setting stored by the framework
    /// </summary>
    public interface ISetting : IEntity, ISecuredEntity, IComparable<ISetting>
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Data Type
        /// </summary>
        string DataType { get; set; }
        /// <summary>
        /// Minimum Value
        /// </summary>
        int MinValue { get; set; }
        /// <summary>
        /// Maximum Value
        /// </summary>
        int MaxValue { get; set; }
        /// <summary>
        /// Order
        /// </summary>
        int Order { get; set; }
        /// <summary>
        /// Is Required?
        /// </summary>
        bool IsRequired { get; set; }
        /// <summary>
        /// Type of Setting
        /// </summary>
        ISettingType Type { get; set; }
        /// <summary>
        /// Setting Group
        /// </summary>
        ISettingGroup Group { get; set; }
        /// <summary>
        /// Short Description
        /// </summary>
        string ShortDescription { get; set; }
        /// <summary>
        /// Long Description
        /// </summary>
        string LongDescription { get; set; }
    }
}
