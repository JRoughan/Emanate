using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.TeamCity.Configuration
{
    public class TeamCityConfiguration : IModuleConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }

        // TODO: Split interfaces so input modules don't need NotSupportedExceptions

        public ObservableCollection<IOutputProfile> Profiles
        {
            get
            {
                throw new NotSupportedException("TeamCity module does not support profiles");
            }
        }

        public IEnumerable<IOutputDevice> OutputDevices
        {
            get { yield break; }
        }

        public event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        public event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;


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
            foreach (var c in text)
            {
                var xored = (char)(c ^ 129);
                outSb.Append(xored);
            }
            return outSb.ToString();
        }
    }
}