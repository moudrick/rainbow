using System;
using System.Collections.Generic;
using System.Text;

namespace Rainbow.Framework.Providers.RainbowRoleProvider {

    public class RainbowRole {

        public RainbowRole( Guid roleId, string roleName ) {
            this.id = roleId;
            this.name = roleName;
            this.description = string.Empty;
        }

        public RainbowRole(Guid roleId, string roleName, string roleDescription) {
            this.id = roleId;
            this.name = roleName;
            this.description = roleDescription;
        }

        private Guid id;

        public Guid Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        private string name;

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        private string description;

        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }
    }
}
