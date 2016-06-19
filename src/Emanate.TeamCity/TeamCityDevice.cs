using System;
using System.Xml.Linq;
using Emanate.Core;

namespace Emanate.TeamCity
{
    public class TeamCityDevice : IInputDevice
    {
        public string Key { get; } = "teamcity";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication { get; set; }

        public XElement CreateMemento()
        {
            var deviceElement = new XElement("device");
            deviceElement.Add(new XAttribute("id", Id));
            deviceElement.Add(new XAttribute("name", Name));
            deviceElement.Add(new XAttribute("uri", Uri));
            deviceElement.Add(new XAttribute("polling-interval", PollingInterval));
            deviceElement.Add(new XAttribute("requires-authentication", RequiresAuthentication));
            if (RequiresAuthentication)
            {
                deviceElement.Add(new XAttribute("username", UserName));
                deviceElement.Add(new XAttribute("password", SimpleCrypto.EncryptDecrypt(Password)));
            }
            return deviceElement;
        }

        public void SetMemento(XElement deviceElement)
        {
            Id = Guid.Parse(deviceElement.Attribute("id").Value);
            Name = deviceElement.Attribute("name").Value;
            Uri = deviceElement.Attribute("uri").Value;
            PollingInterval = int.Parse(deviceElement.Attribute("polling-interval").Value);
            RequiresAuthentication = bool.Parse(deviceElement.Attribute("requires-authentication").Value);
            if (RequiresAuthentication)
            {
                UserName = deviceElement.Attribute("username").Value;
                Password = SimpleCrypto.EncryptDecrypt(deviceElement.Attribute("password").Value);
            }
        }
    }
}