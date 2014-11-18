using System.Collections.ObjectModel;
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

            Name = name;
            OutputDevice = outputDevice;
        }

        public ObservableCollection<IOutputProfile> AvailableProfiles
        {
            get { return configuration.Profiles; }
        }

        public IOutputProfile Profile
        {
            get { return OutputDevice.Profile; }
            set { OutputDevice.Profile = value; OnPropertyChanged(); }
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source
    }
}