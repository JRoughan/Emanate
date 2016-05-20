using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            InputModules = new List<IModule>();
            OutputModules = new List<IModule>();
            InputConfigurations = new List<IInputConfiguration>();
            OutputConfigurations = new List<IOutputConfiguration>();
            OutputDevices = new List<IOutputDevice>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<IModule> InputModules { get; private set; }
        public List<IModule> OutputModules { get; private set; }

        public List<IInputConfiguration> InputConfigurations { get; private set; }
        public List<IOutputConfiguration> OutputConfigurations { get; private set; }
        public List<IOutputDevice> OutputDevices { get; private set; }
    }
}
