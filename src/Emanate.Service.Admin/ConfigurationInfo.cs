using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
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
        public OutputDeviceInfo(string name, IOutputDevice outputDevice, UserControl inputSelector)
        {
            Name = name;
            OutputDevice = outputDevice;
            InputSelector = inputSelector;
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public UserControl InputSelector { get; set; }
    }
}