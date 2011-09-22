using System;

namespace Emanate.Service.Admin
{
    internal class PluginType
    {
        public PluginType(Type type)
        {
            ActualType = type;
        }

        public Type ActualType { get; private set; }

        public string Name { get; set; }
    }
}