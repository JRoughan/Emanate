using Topshelf.HostConfigurators;

namespace Emanate.Service
{
    internal static class Settings
    {
        public static string ServiceName { get; } = "EmanateService";

        public static int Port { get; set; } = 44444;

        public static void Initialize(HostConfigurator hostConfigurator)
        {
            hostConfigurator.AddCommandLineDefinition("port", v => Settings.Port = int.Parse(v));
        }
    }
}