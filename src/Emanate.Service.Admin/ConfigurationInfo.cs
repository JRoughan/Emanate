using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, IEnumerable<ConfigurationProperty> properties)
        {
            Name = name;
            Properties = new ObservableCollection<ConfigurationProperty>(properties);
        }

        public string Name { get; private set; }
        public ObservableCollection<ConfigurationProperty> Properties { get; private set; }
    }
}