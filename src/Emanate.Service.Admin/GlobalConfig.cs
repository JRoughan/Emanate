using System.Collections.Generic;

namespace Emanate.Service.Admin
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            ModuleConfigurations = new List<ConfigurationInfo>();
            OutputDevices = new List<OutputDeviceInfo>();
        }

        public List<ConfigurationInfo> ModuleConfigurations { get; set; }
        public List<OutputDeviceInfo> OutputDevices { get; set; }
    }
}
