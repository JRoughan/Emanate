using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IConfiguration { } 

    public interface IOutputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }
        string Name { get; }

        IOutputProfile GenerateEmptyProfile(string newName = "");
        ObservableCollection<IOutputProfile> Profiles { get; }

        IEnumerable<IOutputDevice> OutputDevices { get; }

        event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;
    }
}
