using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.TeamCity.Configuration
{
    // TODO: Split this into a config class and a VM
    public class TeamCityConfiguration : ViewModel, IModuleConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }
        Type IModuleConfiguration.GuiType { get { return typeof(ConfigurationView); } }

        public TeamCityConfiguration()
        {
            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
            IsEditable = true;
        }

        public IEnumerable<IOutputProfile> Profiles
        {
            get
            {
                throw new NotSupportedException("TeamCity module does not support profiles");
            }
        }

        public string uri;
        public string Uri
        {
            get { return uri; }
            set { uri = value; OnPropertyChanged("Uri"); }
        }

        public int pollingInterval;
        public int PollingInterval
        {
            get { return pollingInterval; }
            set { pollingInterval = value; OnPropertyChanged("PollingInterval"); }
        }

        public string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged("UserName"); }
        }

        public string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }

        public bool requiresAuthentication;
        public bool RequiresAuthentication
        {
            get { return requiresAuthentication; }
            set { requiresAuthentication = value; OnPropertyChanged("RequiresAuthentication"); }
        }

        public string BuildsToMonitor
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public XElement ToXml()
        {
            IsEditable = false;

            var element = new XElement(key);
            element.Add(new XElement("uri", Uri));
            element.Add(new XElement("polling-interval", PollingInterval));
            element.Add(new XElement("requires-authentication", RequiresAuthentication));
            element.Add(new XElement("username", RequiresAuthentication ? UserName : ""));
            element.Add(new XElement("password", RequiresAuthentication ? Password : "")); // TODO: Encrypt password

            IsEditable = true;

            return element;
        }

        public void FromXml(XElement element)
        {
            IsEditable = false;

            // TODO
            //if (element.Name != key)
            //    throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            Uri = element.Element("uri").Value;
            PollingInterval = int.Parse(element.Element("polling-interval").Value);
            RequiresAuthentication = bool.Parse(element.Element("requires-authentication").Value);
            if (RequiresAuthentication)
            {
                UserName = element.Element("username").Value;
                Password = element.Element("password").Value;
            }

            IsEditable = true;
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
            var connection = new TeamCityConnection(this);
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
    }
}