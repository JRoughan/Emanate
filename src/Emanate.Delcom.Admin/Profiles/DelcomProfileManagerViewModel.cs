using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;

namespace Emanate.Delcom.Admin.Profiles
{
    public class DelcomProfileManagerViewModel : ViewModel, ISubscriber<ProfileAddedEvent>
    {
        private readonly DelcomConfiguration delcomConfiguration;
        private readonly IMediator mediator;

        public DelcomProfileManagerViewModel(DelcomConfiguration delcomConfiguration, IMediator mediator)
        {
            this.delcomConfiguration = delcomConfiguration;
            this.mediator = mediator;
            mediator.Subscribe<ProfileAddedEvent>(this);

            foreach (var profile in delcomConfiguration.Profiles.OfType<MonitoringProfile>())
                Profiles.Add(new DelcomProfileViewModel(profile));

            IsEditable = true;

            AddProfileCommand = new DelegateCommand(AddProfile);
        }

        public ICommand AddProfileCommand { get; private set; }

        private void AddProfile()
        {
            var addProfileViewModel = new AddProfileViewModel(delcomConfiguration, Profiles, mediator);
            var addProfileView = new AddProfileView { DataContext = addProfileViewModel };
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

        public void Handle(ProfileAddedEvent e)
        {
            Profiles.Add(new DelcomProfileViewModel((MonitoringProfile)e.Profile));
        }
    }
}