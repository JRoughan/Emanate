using System.Collections.Generic;
using Emanate.Core.Output;
using Emanate.Model;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            InputDevices = new List<SourceDevice>();
            OutputDevices = new List<DisplayDevice>();
            Mappings = new List<DisplayConfiguration>();
        }

        public List<SourceDevice> InputDevices { get; private set; }
        public List<DisplayDevice> OutputDevices { get; private set; }
        public List<DisplayConfiguration> Mappings { get; private set; }
    }
}
