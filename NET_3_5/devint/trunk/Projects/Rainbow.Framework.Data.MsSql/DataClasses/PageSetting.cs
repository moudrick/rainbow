using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Types;
using Rainbow.Framework.Configuration;

namespace Rainbow.Framework.Data.MsSql
{
    partial class PageSetting : IComparable<PageSetting>
    {
        /// <summary>
        /// Gets or sets the base setting.
        /// </summary>
        /// <value>The base setting.</value>
        public BaseSetting BaseSetting
        {
            get
            {
                try
                {
                    using (DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString))
                    {
                        db.Log = new DebuggerWriter();
                        return db.BaseSettings.Single(s => s.SettingName == this.SettingName || s.EnglishName == this.EnglishName);
                    }
                }
                catch { return null; }
            }
            set
            {
                using (DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString))
                {
                    db.Log = new DebuggerWriter();
                    db.BaseSettings.SingleOrDefault(s => s.SettingName == this.SettingName || s.EnglishName == this.EnglishName) = value;
                }
            }
        }

        #region IComparable<PageSetting> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(PageSetting other)
        {
            if (other == null) return 1;
            int compareOrder = other.SettingOrder.Value;
            if (SettingOrder == compareOrder) return 0;
            if (SettingOrder < compareOrder) return -1;
            if (SettingOrder > compareOrder) return 1;
            return 0;
        }

        #endregion
    }
}
