using System.Windows.Controls;
using Emanate.Core.Configuration;

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
}