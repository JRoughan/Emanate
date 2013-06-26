using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Emanate.Core.Input;
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

            foreach (BuildState buildState in Enum.GetValues(typeof(BuildState)))
            {
                NewProfile.States.Add(new ProfileState { BuildState = buildState });
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