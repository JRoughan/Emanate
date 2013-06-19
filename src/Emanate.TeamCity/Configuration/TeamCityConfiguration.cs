using System;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Configuration
{
    public class TeamCityConfiguration : ViewModel, IModuleConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }
        Type IModuleConfiguration.GuiType { get { return typeof(ConfigurationView); } }

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
            var element = new XElement(key);
            element.Add(new XElement("uri", Uri));
            element.Add(new XElement("polling-interval", PollingInterval));
            element.Add(new XElement("requires-authentication", RequiresAuthentication));
            element.Add(new XElement("username", RequiresAuthentication ? UserName : ""));
            element.Add(new XElement("password", RequiresAuthentication ? Password : "")); // TODO: Encrypt password

            return element;
        }

        public void FromXml(XElement element)
        {
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
        }
    }
}

//<?xml version="1.0" encoding="utf-8" ?>
//<emanate>
//  <modules>
//    <teamcity>
//      <uri>http://teamcity</uri>
//      <requires-authentication>true</requires-authentication>
//      <username>TFSBuild</username>
//      <password>&lt;sysadm1n&gt;</password>
//      <polling-interval>30</polling-interval>
//    </teamcity>
//  </modules>
//</emanate>