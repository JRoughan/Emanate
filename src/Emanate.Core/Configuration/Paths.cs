using System;
using System.IO;

namespace Emanate.Core.Configuration
{
    public static class Paths
    {
        private static readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Emanate");

        public static string ConfigFilePath { get; } = Path.Combine(configDir, "Configuration.xml");

        public static string AdminLogFilePath { get; } = Path.Combine(configDir, "Emanate.Service.Admin-{Date}.log");

        public static string ServiceLogFilePath { get; } = Path.Combine(configDir, "Emanate.Service-{Date}.log");
    }
}