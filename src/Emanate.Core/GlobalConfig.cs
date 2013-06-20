using System.Collections.Generic;
using Emanate.Core.Configuration;
using Emanate.Service.Admin;

namespace Emanate.Core
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            ModuleConfigurations = new List<IModuleConfiguration>();
            OutputDevices = new List<OutputDeviceInfo>();
        }

        public List<IModuleConfiguration> ModuleConfigurations { get; set; }
        public List<OutputDeviceInfo> OutputDevices { get; set; }
    }
}
