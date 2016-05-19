using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Profiles
{
    public class AddProfileViewModel : ViewModel
    {
        private readonly ObservableCollection<MonitoringProfileViewModel> existingProfiles;

        public AddProfileViewModel(DelcomConfiguration delcomConfiguration, ObservableCollection<MonitoringProfileViewModel> existingProfiles)
        {
            this.existingProfiles = existingProfiles;

            var emptyProfile = (MonitoringProfile)delcomConfiguration.GenerateEmptyProfile();
            NewProfile = new MonitoringProfileViewModel(emptyProfile);
                
            CloneProfileCommand = new DelegateCommand<MonitoringProfileViewModel>(CloneProfile, p => p != null);
            SaveProfileCommand = new DelegateCommand<MonitoringProfileViewModel>(SaveProfile, CanSaveProfile);
        }

        private MonitoringProfileViewModel newProfile;
        public MonitoringProfileViewModel NewProfile
        {
            get { return newProfile; }
            set { newProfile = value; OnPropertyChanged(); }
        }

        public IEnumerable<MonitoringProfileViewModel> ExistingProfiles
        {
            get { return existingProfiles; }
        }

        public ICommand CloneProfileCommand { get; private set; }

        private void CloneProfile(MonitoringProfileViewModel profileToClone)
        {
            NewProfile.States.Clear();
            foreach (var state in profileToClone.States)
            {
                var newState = new ProfileStateViewModel(new ProfileState())
                    {
                        BuildState = state.BuildState,
                        Green = state.Green,
                        Yellow = state.Yellow,
                        Red = state.Red,
                        Flash = state.Flash
                    };
                NewProfile.States.Add(newState);
            }
        }

        public ICommand SaveProfileCommand { get; private set; }

        private bool CanSaveProfile(MonitoringProfileViewModel profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Key);
        }

        private void SaveProfile(MonitoringProfileViewModel profileViewModel)
        {
            existingProfiles.Add(profileViewModel);
        }
    }
}