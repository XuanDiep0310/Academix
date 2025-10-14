using Microsoft.Extensions.Configuration;
using System.IO;

namespace Academix.WinApp.Utils
{
    public static class Config
    {
        private static IConfigurationRoot configuration;

        static Config()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
        }

        public static string Get(string key)
        {
            return configuration[key];
        }
    }
}
