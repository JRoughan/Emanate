using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;

namespace Emanate.Service.Admin
{
    public class DeviceViewModel : ViewModel
    {
        private readonly IOutputDevice outputDevice;
        private readonly IOutputConfiguration configuration;

        public DeviceViewModel(IOutputDevice outputDevice, IOutputConfiguration configuration)
        {
            this.outputDevice = outputDevice;
            this.configuration = configuration;
        }

        public ObservableCollection<IProfile> AvailableProfiles
        {
            get { return configuration.Profiles; }
        }

        public IProfile Profile
        {
            get { return outputDevice.Profile; }
            set { outputDevice.Profile = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return outputDevice.Name; }
            private set { outputDevice.Name = value; OnPropertyChanged(); }
        }

        public IOutputDevice OutputDevice => outputDevice;

        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source
    }
}