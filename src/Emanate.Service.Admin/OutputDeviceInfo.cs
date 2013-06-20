using System;
using System.Collections.ObjectModel;
using System.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class OutputDeviceInfo : ViewModel
    {
        private readonly IModuleConfiguration configuration;

        public OutputDeviceInfo(string name, IOutputDevice outputDevice, IModuleConfiguration configuration)
        {
            this.configuration = configuration;

            profile = AvailableProfiles.Single(p => p.Key.Equals(outputDevice.Profile, StringComparison.OrdinalIgnoreCase));

            Name = name;
            OutputDevice = outputDevice;
        }

        private IOutputProfile profile;
        public IOutputProfile Profile
        {
            get { return profile; }
            set { profile = value; OutputDevice.Profile = value.Key; OnPropertyChanged("Profile"); }
        }

        public ObservableCollection<IOutputProfile> AvailableProfiles
        {
            get { return configuration.Profiles; }
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source
    }
}