using System;
using System.Windows.Input;
using Emanate.Service.Admin;

namespace Emanate.TeamCity.Configuration
{
    public class TeamCityConfigurationViewModel : ViewModel
    {
        private readonly TeamCityConfiguration teamCityConfiguration;

        public TeamCityConfigurationViewModel(TeamCityConfiguration teamCityConfiguration)
        {
            this.teamCityConfiguration = teamCityConfiguration;
            IsEditable = teamCityConfiguration != null;

            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
        }

        public string Uri
        {
            get { return teamCityConfiguration.Uri; }
            set { teamCityConfiguration.Uri = value; OnPropertyChanged("Uri"); }
        }

        public int PollingInterval
        {
            get { return teamCityConfiguration.PollingInterval; }
            set { teamCityConfiguration.PollingInterval = value; OnPropertyChanged("PollingInterval"); }
        }

        public string UserName
        {
            get { return teamCityConfiguration.UserName; }
            set { teamCityConfiguration.UserName = value; OnPropertyChanged("UserName"); }
        }

        public string Password
        {
            get { return teamCityConfiguration.Password; }
            set { teamCityConfiguration.Password = value; OnPropertyChanged("Password"); }
        }

        public bool RequiresAuthentication
        {
            get { return teamCityConfiguration.RequiresAuthentication; }
            set { teamCityConfiguration.RequiresAuthentication = value; OnPropertyChanged("RequiresAuthentication"); }
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged("IsEditable"); }
        }

        private bool? isTestSuccessful;
        public bool? IsTestSuccessful
        {
            get { return isTestSuccessful; }
            set { isTestSuccessful = value; OnPropertyChanged("IsTestSuccessful"); }
        }

        public ICommand TestConnectionCommand { get; set; }

        private bool isTesting;
        private bool CanTestConnection()
        {
            return !isTesting;
        }

        private void TestConnection()
        {
            isTesting = true;
            IsEditable = false;
            IsTestSuccessful = null;
            var connection = new TeamCityConnection(teamCityConfiguration);
            try
            {
                IsTestSuccessful = connection.GetProjects() != null;
            }
            catch (Exception)
            {
                IsTestSuccessful = false;
            }
            finally
            {
                isTesting = false;
                IsEditable = true;
            }
        }
    }
}