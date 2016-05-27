using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IOutputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }

        ObservableCollection<IProfile> Profiles { get; }

        IEnumerable<IOutputDevice> OutputDevices { get; }

        event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;
    }
}
