using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, IEnumerable<ConfigProperty> properties)
        {
            Name = name;
            Properties = new ObservableCollection<ConfigProperty>(properties);
        }

        public string Name { get; private set; }
        public ObservableCollection<ConfigProperty> Properties { get; private set; }
    }

    public class ConfigProperty
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
    }
}