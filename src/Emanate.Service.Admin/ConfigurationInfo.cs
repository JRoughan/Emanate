using System.Windows.Controls;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, UserControl configurationEditor, UserControl deviceManager)
        {
            Name = name;
            ConfigurationEditor = configurationEditor;
            DeviceManager = deviceManager;
        }

        public string Name { get; private set; }
        public UserControl ConfigurationEditor { get; private set; }
        public UserControl DeviceManager { get; private set; }
    }
}
