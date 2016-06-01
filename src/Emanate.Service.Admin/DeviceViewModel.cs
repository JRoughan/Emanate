using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;

namespace Emanate.Service.Admin
{
    public class DeviceViewModel : ViewModel, ISubscriber<ProfileAddedEvent>, ISubscriber<ProfileDeletedEvent>
    {
        private readonly IOutputDevice outputDevice;

        public DeviceViewModel(IOutputDevice outputDevice, IOutputConfiguration configuration, IMediator mediator)
        {
            this.outputDevice = outputDevice;
            AvailableProfiles = new ObservableCollection<IProfile>(configuration.Profiles);

            mediator.Subscribe<ProfileAddedEvent>(this);
            mediator.Subscribe<ProfileDeletedEvent>(this);
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

        public void Handle(ProfileDeletedEvent e)
        {
            AvailableProfiles.Remove(e.Profile);
        }
    }
}