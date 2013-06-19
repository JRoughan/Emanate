using System.Windows.Controls;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, UserControl gui, IModuleConfiguration moduleConfiguration)
        {
            ModuleConfiguration = moduleConfiguration;
            Name = name;
            Gui = gui;
        }

        public string Name { get; private set; }
        public UserControl Gui { get; private set; }
        public IModuleConfiguration ModuleConfiguration { get; private set; }
    }

    public class OutputDeviceInfo
    {
        public OutputDeviceInfo(string name, IOutputDevice outputDevice)
        {
            Name = name;
            OutputDevice = outputDevice;
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source

        public void AddInputSelector(InputSelector inputSelector)
        {
            InputSelector = inputSelector;
        }
    }
}