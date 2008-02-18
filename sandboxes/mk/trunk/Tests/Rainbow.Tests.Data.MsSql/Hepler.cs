using System.Configuration;

namespace Rainbow.Tests.Data.MsSql
{
    static class Hepler
    {
        public static readonly string RainbowWebApplicationRoot = 
            ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
    }
}
