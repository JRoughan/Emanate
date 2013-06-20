using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
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

    public class TeamCityConfiguration : IModuleConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }

        public ObservableCollection<IOutputProfile> Profiles
        {
            get
            {
                throw new NotSupportedException("TeamCity module does not support profiles");
            }
        }

        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication { get; set; }

        public Memento CreateMemento()
        {
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("type", key));
            moduleElement.Add(new XElement("uri", Uri));
            moduleElement.Add(new XElement("polling-interval", PollingInterval));
            moduleElement.Add(new XElement("requires-authentication", RequiresAuthentication));
            moduleElement.Add(new XElement("username", RequiresAuthentication ? UserName : ""));
            moduleElement.Add(new XElement("password", RequiresAuthentication ? EncryptDecrypt(Password) : ""));

            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            if (memento.Type != key)
                throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            var element = memento.Element;
            Uri = element.Element("uri").Value;
            PollingInterval = int.Parse(element.Element("polling-interval").Value);
            RequiresAuthentication = bool.Parse(element.Element("requires-authentication").Value);
            if (RequiresAuthentication)
            {
                UserName = element.Element("username").Value;
                Password = EncryptDecrypt(element.Element("password").Value);
            }
        }

        // TODO: Extrmemely simplistic encrytion used here - will keep honest people honest but not much else
        private static string EncryptDecrypt(string text)
        {
            var outSb = new StringBuilder(text.Length);
            char c;
            for (int i = 0; i < text.Length; i++)
            {
                c = text[i];
                c = (char)(c ^ 129);
                outSb.Append(c);
            }
            return outSb.ToString();
        }
    }
}