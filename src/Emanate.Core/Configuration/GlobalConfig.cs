using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            InputModules = new List<IInputModule>();
            OutputModules = new List<IOutputModule>();
            InputConfigurations = new List<IInputConfiguration>();
            OutputConfigurations = new List<IOutputConfiguration>();
            OutputDevices = new List<IOutputDevice>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<IInputModule> InputModules { get; private set; }
        public List<IOutputModule> OutputModules { get; private set; }

        public List<IInputConfiguration> InputConfigurations { get; private set; }
        public List<IOutputConfiguration> OutputConfigurations { get; private set; }
        public List<IOutputDevice> OutputDevices { get; private set; }
    }
}
