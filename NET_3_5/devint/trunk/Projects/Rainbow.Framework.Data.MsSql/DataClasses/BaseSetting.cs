using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.MsSql
{
    partial class BaseSetting
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            switch (this.DataType)
            {
                case "bool":
                    return this.GetBool();
                case "int":
                    return this.GetInt32();
                default: //string
                    return this.SettingValue;
            }
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <returns></returns>
        public int GetInt32()
        {
            return int.Parse(this.SettingValue);
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <returns></returns>
        public bool GetBool()
        {
            return bool.Parse(this.SettingValue);
        }
    }
}
