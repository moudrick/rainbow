using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace Rainbow.Framework.ReWrite
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExUrlMappingConfigHandler : IConfigurationSectionHandler
    {
        private XmlNode _Section;

        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section"></param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            _Section = section;
            return this;
        }

        /// <summary>
        /// Enableds this instance.
        /// </summary>
        /// <returns></returns>
        internal bool Enabled()
        {
            if (_Section.Attributes.GetNamedItem("enabled").Value.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Mappeds the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        internal string MappedUrl(string url)
        {
            Regex oReg;

            foreach (XmlNode x in _Section.ChildNodes)
            {
                oReg = new Regex(x.Attributes.GetNamedItem("url").Value.ToLower());

                if (oReg.Match(url).Success)
                {
                    return oReg.Replace(url, x.Attributes.GetNamedItem("mappedUrl").Value.ToLower());
                }
            }

            return "";
        }
    }
}