using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Configuration
{
    public class TeamCityConfiguration : IInputConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IInputConfiguration.Key { get { return key; } }
        string IInputConfiguration.Name { get { return name; } }

        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication { get; set; }

        public Memento CreateMemento()
        {
            Trace.TraceInformation("=> TeamCityConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("key", key));
            moduleElement.Add(new XAttribute("type", "input"));
            moduleElement.Add(new XElement("uri", Uri));
            moduleElement.Add(new XElement("polling-interval", PollingInterval));
            moduleElement.Add(new XElement("requires-authentication", RequiresAuthentication));
            moduleElement.Add(new XElement("username", RequiresAuthentication ? UserName : ""));
            moduleElement.Add(new XElement("password", RequiresAuthentication ? EncryptDecrypt(Password) : ""));

            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            Trace.TraceInformation("=> TeamCityConfiguration.SetMemento");
            if (memento.Key != key)
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