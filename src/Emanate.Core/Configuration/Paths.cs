using System;
using System.IO;

namespace Emanate.Core.Configuration
{
    public static class Paths
    {
        private static readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Emanate");
        private static readonly string configFilePath = Path.Combine(configDir, "Configuration.xml");
        private static readonly string adminLogFilePath = Path.Combine(configDir, "Emanate.Service.Admin-{Date}.log");
        private static readonly string serviceLogFilePath = Path.Combine(configDir, "Emanate.Service-{Date}.log");

        public static string ConfigFolder => configDir;
        public static string ConfigFilePath => configFilePath;
        public static string AdminLogFilePath => adminLogFilePath;
        public static string ServiceLogFilePath => serviceLogFilePath;
    }
}