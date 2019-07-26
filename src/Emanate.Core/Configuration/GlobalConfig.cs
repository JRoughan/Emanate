using System.Collections.Generic;
using Emanate.Core.Output;
using Emanate.Model;

namespace Emanate.Core.Configuration
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            InputModules = new List<IModule>();
            OutputModules = new List<IModule>();
            InputDevices = new List<SourceDevice>();
            OutputDevices = new List<DisplayDevice>();
            Mappings = new List<Mapping>();
        }


        public List<IModule> InputModules { get; private set; }
        public List<IModule> OutputModules { get; private set; }

        public List<SourceDevice> InputDevices { get; private set; }
        public List<DisplayDevice> OutputDevices { get; private set; }

        public List<Mapping> Mappings { get; private set; }
    }
}
