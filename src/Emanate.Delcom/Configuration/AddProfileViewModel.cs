using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Emanate.Core;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class AddProfileViewModel : ViewModel
    {
        public AddProfileViewModel(DelcomConfiguration delcomConfiguration)
        {
            var allStates = new List<string>();
            foreach (var profile in delcomConfiguration.Profiles)
            {
                ExistingProfiles.Add(profile);
                allStates.AddRange(profile.States.Select(s => s.Name));
            }
            
            NewProfile = new MonitoringProfile();
            foreach (var stateName in allStates.Distinct())
            {
                NewProfile.States.Add(new ProfileState { Name = stateName});
            }

            CloneProfileCommand = new DelegateCommand<MonitoringProfile>(CloneProfile, p => p != null);
        }

        private MonitoringProfile newProfile;
        public MonitoringProfile NewProfile
        {
            get { return newProfile; }
            set { newProfile = value; OnPropertyChanged("NewProfile"); }
        }

        private ObservableCollection<MonitoringProfile> existingProfiles = new ObservableCollection<MonitoringProfile>();
        public ObservableCollection<MonitoringProfile> ExistingProfiles
        {
            get { return existingProfiles; }
            set { existingProfiles = value; OnPropertyChanged("ExistingProfiles"); }
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
    }
}