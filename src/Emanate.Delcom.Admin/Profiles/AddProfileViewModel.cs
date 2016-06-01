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

        private bool CanSaveProfile(DelcomProfileViewModel profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Key);
        }

        private void SaveProfile(DelcomProfileViewModel profileViewModel)
        {
            mediator.Publish(new ProfileAddedEvent(profileViewModel.GetProfile()));
        }
    }
}