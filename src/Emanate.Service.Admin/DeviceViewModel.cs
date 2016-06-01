using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;

namespace Emanate.Service.Admin
{
    public class DeviceViewModel : ViewModel, ISubscriber<ProfileAddedEvent>
    {
        private readonly IOutputDevice outputDevice;
        private readonly IOutputConfiguration configuration;
        private readonly IMediator mediator;

        public DeviceViewModel(IOutputDevice outputDevice, IOutputConfiguration configuration, IMediator mediator)
        {
            this.outputDevice = outputDevice;
            this.configuration = configuration;
            this.mediator = mediator;
            AvailableProfiles = new ObservableCollection<IProfile>(configuration.Profiles);

            mediator.Subscribe<ProfileAddedEvent>(this);
        }

        public ObservableCollection<IProfile> AvailableProfiles { get; }

        public IProfile Profile
        {
            get { return outputDevice.Profile; }
            set { outputDevice.Profile = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return outputDevice.Name; }
            set { outputDevice.Name = value; OnPropertyChanged(); }
        }

        public IOutputDevice OutputDevice => outputDevice;

        public List<InputSelector> InputSelectors { get; } = new List<InputSelector>();

        public void Handle(ProfileAddedEvent e)
        {
            AvailableProfiles.Add(e.Profile);
        }
    }
}