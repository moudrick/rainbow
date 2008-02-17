using System.Configuration.Provider;
using System.Web.Caching;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Providers
{
    public abstract class BaseRainbowProvider : ProviderBase
    {
        /// <summary>
        /// Gets the current cache.
        /// </summary>
        /// <value>The current cache.</value>
        protected static Cache Cache
        {
            get
            {
                return RainbowContext.Current.Cache;
            }
        }

        //TODO: [moudrick] uncomment after all config sections renamed, remove duplications in inherited classes
//            where ProviderType : ProviderBase
//        public static ProviderType Instance
//        {
//            get
//            {
//                return ProviderConfiguration.GetDefaultProviderFromCache<ProviderType>(
//                    typeof(ProviderType).GetName(), CurrentCache);
//            }
//        }
    }
}
