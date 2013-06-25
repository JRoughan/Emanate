﻿using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            ModuleConfigurations = new List<IModuleConfiguration>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<IModuleConfiguration> ModuleConfigurations { get; private set; }
        public List<IOutputDevice> OutputDevices { get; private set; }
    }
}
