using Emanate.Extensibility;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, ProfileManager profileManager, DeviceManager deviceManager)
        {
            Name = name;
            ProfileManager = profileManager;
            DeviceManager = deviceManager;
        }

        public string Name { get; private set; }
        public ProfileManager ProfileManager { get; private set; }
        public DeviceManager DeviceManager { get; private set; }
    }
}
