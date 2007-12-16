using System;
using System.Collections.Generic;
using System.Text;

namespace Rainbow.Framework.Core.BLL {
    /// <summary>
    /// 
    /// </summary>
    public class GeneralModuleDefinition {

        private int generalModDefID;
        private string friendlyName;
        private string desktopSource;
        private string mobileSource;
        private string assemblyName;
        private string className;
        private bool admin;
        private bool searchable;

        /// <summary>
        /// Gets or sets the general mod def ID.
        /// </summary>
        /// <value>The general mod def ID.</value>
        public int GeneralModDefID {
            get {
                return generalModDefID;
            }
            set {
                generalModDefID = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the friendly.
        /// </summary>
        /// <value>The name of the friendly.</value>
        public string FriendlyName {
            get {
                return friendlyName;
            }
            set {
                friendlyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the desktop source.
        /// </summary>
        /// <value>The desktop source.</value>
        public string DesktopSource {
            get {
                return desktopSource;
            }
            set {
                desktopSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the mobile source.
        /// </summary>
        /// <value>The mobile source.</value>
        public string MobileSource {
            get {
                return mobileSource;
            }
            set {
                mobileSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName {
            get {
                return assemblyName;
            }
            set {
                assemblyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName {
            get {
                return className;
            }
            set {
                className = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GeneralModuleDefinition"/> is admin.
        /// </summary>
        /// <value><c>true</c> if admin; otherwise, <c>false</c>.</value>
        public bool Admin {
            get {
                return admin;
            }
            set {
                admin = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GeneralModuleDefinition"/> is searchable.
        /// </summary>
        /// <value><c>true</c> if searchable; otherwise, <c>false</c>.</value>
        public bool Searchable {
            get {
                return searchable;
            }
            set {
                searchable = value;
            }
        }
    }
}
