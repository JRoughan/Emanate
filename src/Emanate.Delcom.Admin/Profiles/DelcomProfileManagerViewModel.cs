﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Profiles
{
    public class DelcomProfileManagerViewModel : ViewModel
    {
        private readonly DelcomConfiguration delcomConfiguration;

        public DelcomProfileManagerViewModel(DelcomConfiguration delcomConfiguration)
        {
            this.delcomConfiguration = delcomConfiguration;
            foreach (var profile in delcomConfiguration.Profiles.OfType<MonitoringProfile>())
                Profiles.Add(new DelcomProfileViewModel(profile));

            IsEditable = true;

            AddProfileCommand = new DelegateCommand(AddProfile);
        }

        public ICommand AddProfileCommand { get; private set; }

        private void AddProfile()
        {
            var addProfileViewModel = new AddProfileViewModel(delcomConfiguration, Profiles);
            var addProfileView = new Profiles.AddProfileView { DataContext = addProfileViewModel };
            addProfileView.Owner = Application.Current.MainWindow;
            addProfileView.ShowDialog();
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DelcomProfileViewModel> Profiles { get; } = new ObservableCollection<DelcomProfileViewModel>();
    }
}