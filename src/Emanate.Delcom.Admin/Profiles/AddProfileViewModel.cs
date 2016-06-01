using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;

namespace Emanate.Delcom.Admin.Profiles
{
    public class AddProfileViewModel : ViewModel
    {
        private readonly ObservableCollection<DelcomProfileViewModel> existingProfiles;
        private readonly IMediator mediator;

        public AddProfileViewModel(DelcomConfiguration delcomConfiguration, ObservableCollection<DelcomProfileViewModel> existingProfiles, IMediator mediator)
        {
            this.existingProfiles = existingProfiles;
            this.mediator = mediator;

            var emptyProfile = (MonitoringProfile)delcomConfiguration.GenerateEmptyProfile();
            NewProfile = new DelcomProfileViewModel(emptyProfile);
                
            CloneProfileCommand = new DelegateCommand<DelcomProfileViewModel>(CloneProfile, p => p != null);
            SaveProfileCommand = new DelegateCommand<DelcomProfileViewModel>(SaveProfile, CanSaveProfile);
        }

        private DelcomProfileViewModel newProfile;
        public DelcomProfileViewModel NewProfile
        {
            get { return newProfile; }
            set { newProfile = value; OnPropertyChanged(); }
        }

        public IEnumerable<DelcomProfileViewModel> ExistingProfiles
        {
            get { return existingProfiles; }
        }

        public ICommand CloneProfileCommand { get; private set; }

        private void CloneProfile(DelcomProfileViewModel profileToClone)
        {
            var profile = (MonitoringProfile)NewProfile.GetProfile();
            profile.States.Clear();
            NewProfile.States.Clear();
            foreach (var state in profileToClone.States)
            {
                var profileState = new ProfileState
                {
                    BuildState = state.BuildState,
                    Green = state.Green,
                    Yellow = state.Yellow,
                    Red = state.Red,
                    Flash = state.Flash
                };
                profile.States.Add(profileState);
                NewProfile.States.Add(new ProfileStateViewModel(profileState));
            }
        }

        public ICommand SaveProfileCommand { get; private set; }

        private bool CanSaveProfile(DelcomProfileViewModel profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Name);
        }

        private void SaveProfile(DelcomProfileViewModel profileViewModel)
        {
            mediator.Publish(new ProfileAddedEvent(profileViewModel.GetProfile()));
        }
    }
}