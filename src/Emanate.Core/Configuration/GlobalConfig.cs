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
            InputDevices = new List<IDevice>();
            OutputDevices = new List<IOutputDevice>();
            Mappings = new List<Mapping>();
        }


        public List<IModule> InputModules { get; private set; }
        public List<IModule> OutputModules { get; private set; }

        public List<IInputConfiguration> InputConfigurations { get; private set; }
        public List<IOutputConfiguration> OutputConfigurations { get; private set; }

        public List<IDevice> InputDevices { get; private set; }
        public List<IOutputDevice> OutputDevices { get; private set; }

        public List<Mapping> Mappings { get; private set; }
    }
}
