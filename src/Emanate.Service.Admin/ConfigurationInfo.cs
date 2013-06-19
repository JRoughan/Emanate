using System.Windows.Controls;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, UserControl gui)
        {
            Name = name;
            Gui = gui;
        }

        public string Name { get; private set; }
        public UserControl Gui { get; private set; }
    }
}