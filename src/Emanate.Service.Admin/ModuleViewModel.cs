using Emanate.Extensibility;

namespace Emanate.Service.Admin
{
    public class ModuleViewModel
    {
        public ModuleViewModel(string name, ProfileManager profileManager, DeviceManager deviceManager, InputSelector inputSelector)
        {
            Name = name;
            ProfileManager = profileManager;
            DeviceManager = deviceManager;
            InputSelector = inputSelector;
        }

        public string Name { get; private set; }
        public ProfileManager ProfileManager { get; private set; }
        public DeviceManager DeviceManager { get; private set; }
        public InputSelector InputSelector { get; private set; }
    }
}
