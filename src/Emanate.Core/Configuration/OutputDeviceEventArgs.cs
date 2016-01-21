using System;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class OutputDeviceEventArgs : EventArgs
    {
        public OutputDeviceEventArgs(IOutputConfiguration moduleConfiguration, IOutputDevice outputDevice)
        {
            ModuleConfiguration = moduleConfiguration;
            OutputDevice = outputDevice;
        }

        public IOutputConfiguration ModuleConfiguration { get; private set; }
        public IOutputDevice OutputDevice { get; private set; }
    }
}