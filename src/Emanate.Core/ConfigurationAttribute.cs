using System;

namespace Emanate.Core
{
    public class ConfigurationAttribute : Attribute
    {
        public string Name { get; private set; }

        public ConfigurationAttribute(string name)
        {
            Name = name;
        }
    }
}