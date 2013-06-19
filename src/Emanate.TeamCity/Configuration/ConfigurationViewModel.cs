using Emanate.Core;

namespace Emanate.TeamCity.Configuration
{
    public class ConfigurationViewModel : ViewModel
    {
        private readonly TeamCityConfiguration configuration;

        public ConfigurationViewModel(TeamCityConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Name
        {
            get { return "TeamCity"; }
        }

        public string Uri
        {
            get { return configuration.Uri; }
            set { configuration.Uri = value; OnPropertyChanged("Uri"); }
        }

        public int PollingInterval
        {
            get { return configuration.PollingInterval; }
            set { configuration.PollingInterval = value; OnPropertyChanged("PollingInterval"); }
        }

        public string UserName
        {
            get { return configuration.UserName; }
            set { configuration.UserName = value; OnPropertyChanged("UserName"); }
        }

        public string Password
        {
            get { return configuration.Password; }
            set { configuration.Password = value; OnPropertyChanged("Password"); }
        }

        public bool RequiresAuthentication
        {
            get { return !configuration.IsUsingGuestAuthentication; }
            set { configuration.IsUsingGuestAuthentication = !value; OnPropertyChanged("RequiresAuthentication"); }
        }

        public string BuildsToMonitor
        {
            get { return configuration.BuildsToMonitor; }
            set { configuration.BuildsToMonitor = value; OnPropertyChanged("BuildsToMonitor"); }
        }
    }
}