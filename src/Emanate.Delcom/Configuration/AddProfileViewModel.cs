using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class AddProfileViewModel : ViewModel
    {
        private readonly ObservableCollection<IOutputProfile> existingProfiles;

        public AddProfileViewModel(ObservableCollection<IOutputProfile> existingProfiles)
        {
            this.existingProfiles = existingProfiles;

            NewProfile = new MonitoringProfile();
            var allStates = existingProfiles.OfType<MonitoringProfile>().SelectMany(p => p.States).Select(s => s.Name).Distinct();
            foreach (var stateName in allStates.Distinct())
            {
                NewProfile.States.Add(new ProfileState { Name = stateName});
            }

            CloneProfileCommand = new DelegateCommand<MonitoringProfile>(CloneProfile, p => p != null);
            SaveProfileCommand = new DelegateCommand<MonitoringProfile>(SaveProfile, CanSaveProfile);
        }

        private MonitoringProfile newProfile;
        public MonitoringProfile NewProfile
        {
            get { return newProfile; }
            set { newProfile = value; OnPropertyChanged("NewProfile"); }
        }

        public IEnumerable<IOutputProfile> ExistingProfiles
        {
            get { return existingProfiles; }
        }

        public ICommand CloneProfileCommand { get; private set; }

        private void CloneProfile(MonitoringProfile profileToClone)
        {
            NewProfile.States.Clear();
            foreach (var state in profileToClone.States)
            {
                var newState = new ProfileState
                    {
                        Name = state.Name,
                        Green = state.Green,
                        Yellow = state.Yellow,
                        Red = state.Red,
                        Flash = state.Flash
                    };
                NewProfile.States.Add(newState);
            }
        }

        public ICommand SaveProfileCommand { get; private set; }

        private bool CanSaveProfile(MonitoringProfile profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Key);
        }

        private void SaveProfile(MonitoringProfile profile)
        {
            existingProfiles.Add(profile);
        }
    }
}