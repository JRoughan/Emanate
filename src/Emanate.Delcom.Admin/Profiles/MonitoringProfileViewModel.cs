using System.Collections.ObjectModel;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Profiles
{
    public class MonitoringProfileViewModel : ViewModel
    {
        private readonly MonitoringProfile monitoringProfile;

        public MonitoringProfileViewModel(MonitoringProfile monitoringProfile)
        {
            this.monitoringProfile = monitoringProfile;
            foreach (var profileState in monitoringProfile.States)
                States.Add(new ProfileStateViewModel(profileState));
        }

        public string Key
        {
            get { return monitoringProfile.Key; }
            set { monitoringProfile.Key = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProfileStateViewModel> States { get; } = new ObservableCollection<ProfileStateViewModel>();

        public uint Decay
        {
            get { return monitoringProfile.Decay; }
            set { monitoringProfile.Decay = value; OnPropertyChanged(); }
        }

        public bool HasRestrictedHours
        {
            get { return monitoringProfile.HasRestrictedHours; }
            set { monitoringProfile.HasRestrictedHours = value; OnPropertyChanged(); }
        }

        public uint StartTime
        {
            get { return monitoringProfile.StartTime; }
            set { monitoringProfile.StartTime = value; OnPropertyChanged(); }
        }

        public uint EndTime
        {
            get { return monitoringProfile.EndTime; }
            set { monitoringProfile.EndTime = value; OnPropertyChanged(); }
        }
    }
}