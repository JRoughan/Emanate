using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class DelcomConfigurationViewModel : ViewModel
    {
        private readonly DelcomConfiguration delcomConfiguration;

        public DelcomConfigurationViewModel(DelcomConfiguration delcomConfiguration)
        {
            this.delcomConfiguration = delcomConfiguration;
            IsEditable = delcomConfiguration != null;

            AddProfileCommand = new DelegateCommand(AddProfile);
        }

        public ICommand AddProfileCommand { get; private set; }

        private void AddProfile()
        {
            var addProfileViewModel = new AddProfileViewModel(Profiles);
            var addProfileView = new AddProfileView { DataContext = addProfileViewModel };
            addProfileView.Owner = Application.Current.MainWindow;
            addProfileView.ShowDialog();
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged("IsEditable"); }
        }

        public ObservableCollection<IOutputProfile> Profiles
        {
            get { return delcomConfiguration.Profiles; }
        }
    }
}