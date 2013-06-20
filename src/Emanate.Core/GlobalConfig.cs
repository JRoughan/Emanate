using System.Collections.Generic;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Core
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            ModuleConfigurations = new List<IModuleConfiguration>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<IModuleConfiguration> ModuleConfigurations { get; set; }
        public List<IOutputDevice> OutputDevices { get; set; }
    }
}
