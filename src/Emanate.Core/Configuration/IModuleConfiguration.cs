using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IModuleConfiguration : IOriginator
    {
        string Key { get; }
        string Name { get; }

        ObservableCollection<IOutputProfile> Profiles { get; }

        IEnumerable<IOutputDevice> OutputDevices { get; }

        event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;
    }

    public class OutputDeviceEventArgs : EventArgs
    {
        public OutputDeviceEventArgs(IModuleConfiguration moduleConfiguration, IOutputDevice outputDevice)
        {
            ModuleConfiguration = moduleConfiguration;
            OutputDevice = outputDevice;
        }

        public IModuleConfiguration ModuleConfiguration { get; private set; }
        public IOutputDevice OutputDevice { get; private set; }
    }
}
