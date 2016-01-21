using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            InputConfigurations = new List<IInputConfiguration>();
            OututConfigurations = new List<IOutputConfiguration>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<IInputConfiguration> InputConfigurations { get; private set; }
        public List<IOutputConfiguration> OututConfigurations { get; private set; }
        public List<IOutputDevice> OutputDevices { get; private set; }
    }
}
