using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;

namespace Emanate.Service.Admin
{
    public class OutputDeviceInfo : ViewModel
    {
        private readonly IOutputConfiguration configuration;

        public OutputDeviceInfo(string name, IOutputDevice outputDevice, IOutputConfiguration configuration)
        {
            this.configuration = configuration;

            Name = name;
            OutputDevice = outputDevice;
        }

        public ObservableCollection<IProfile> AvailableProfiles
        {
            get { return configuration.Profiles; }
        }

        public IProfile Profile
        {
            get { return OutputDevice.Profile; }
            set { OutputDevice.Profile = value; OnPropertyChanged(); }
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source
    }
}