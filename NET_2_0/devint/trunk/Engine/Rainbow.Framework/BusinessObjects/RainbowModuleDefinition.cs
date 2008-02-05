namespace Rainbow.Framework.BusinessObjects
{
    ///<summary>
    ///</summary>
    public class RainbowModuleDefinition
    {
        int generalModDefID;
        string friendlyName;
        string desktopSource;
        string mobileSource;
        string assemblyName;
        string className;
        bool admin;
        bool searchable;

        ///<summary>
        ///</summary>
        public int GeneralModDefID
        {
            get { return generalModDefID; }
            set { generalModDefID = value; }
        }

        ///<summary>
        ///</summary>
        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }

        ///<summary>
        ///</summary>
        public string DesktopSource
        {
            get { return desktopSource; }
            set { desktopSource = value; }
        }

        ///<summary>
        ///</summary>
        public string MobileSource
        {
            get { return mobileSource; }
            set { mobileSource = value; }
        }

        ///<summary>
        ///</summary>
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        ///<summary>
        ///</summary>
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        ///<summary>
        ///</summary>
        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }

        ///<summary>
        ///</summary>
        public bool Searchable
        {
            get { return searchable; }
            set { searchable = value; }
        }
    }
}
