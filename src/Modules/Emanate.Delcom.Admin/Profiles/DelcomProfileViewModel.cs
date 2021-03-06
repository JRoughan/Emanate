﻿using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Profiles
{
    public class DelcomProfileViewModel : ViewModel
    {
        private readonly MonitoringProfile monitoringProfile;

        public DelcomProfileViewModel(MonitoringProfile monitoringProfile)
        {
            this.monitoringProfile = monitoringProfile;
            foreach (var profileState in monitoringProfile.States)
                States.Add(new ProfileStateViewModel(profileState));
        }

        public string Name
        {
            get { return monitoringProfile.Name; }
            set { monitoringProfile.Name = value; OnPropertyChanged(); }
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

        public IProfile GetProfile()
        {
            return monitoringProfile;
        }
    }
}