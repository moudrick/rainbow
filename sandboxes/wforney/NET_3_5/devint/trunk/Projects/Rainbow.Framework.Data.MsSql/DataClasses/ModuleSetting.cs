using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Rainbow.Framework.Configuration.Cache;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Data;

namespace Rainbow.Framework.Data.MsSql
{
    [History("gman3001", "2004/10/06", "Added GetModuleDesktopSrc method to return the source path of a Module specified by its ID")]
    [History("Jes1111", "2003/04/24", "Added Cacheable property")]
    [History("bill@improvtech.com", "2007/12/19", "Moved some ModuleSettings stuff here and got rid of ModuleSettings class")]
    partial class ModuleSetting
    {
        #region Public Fields

        //// Change 28/Feb/2003 - Jeremy Esland - Cache
        //// used to store list of files for cache dependency -
        //// optionally filled by module code
        //// read by code in CachedPortalModuleControl
        ///// <summary>
        ///// String array of cache dependency files
        ///// </summary>
        //public List<string> CacheDependency = new List<string>();

        //// Jes1111
        ///// <summary>
        ///// Is Cacheable?
        ///// </summary>
        //public bool Cacheable;

        #endregion

        /// <summary>
        /// The UpdateModuleSetting Method updates a single module setting
        /// in the ModuleSettings database table and removes from cache.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdateModuleSetting(int moduleId, string key, string value)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            db.Log = new DebuggerWriter();
            var q = db.ModuleSettings.Where(ms => ms.ModuleId == moduleId && ms.SettingName == key).SingleOrDefault();
            q.ModuleId = moduleId;
            q.SettingName = key;
            q.SettingValue = value;
            db.SubmitChanges();

            //Invalidate cache
            CurrentCache.Remove(Key.ModuleSettings(moduleId));
        }
    }
}
