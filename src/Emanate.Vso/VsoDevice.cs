using System;
using System.Xml.Linq;
using Emanate.Core;
using Serilog;

namespace Emanate.Vso
{
    public class VsoDevice : IInputDevice
    {
        public string Key { get; } = "vso";

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public void SetMemento(XElement deviceElement)
        {
            Log.Information("=> VsoDevice.SetMemento");

            Id = Guid.Parse(deviceElement.Attribute("id").Value);
            Name = deviceElement.Attribute("name").Value;
            Uri = deviceElement.Attribute("uri").Value;
            PollingInterval = int.Parse(deviceElement.Attribute("polling-interval").Value);
            UserName = deviceElement.Attribute("username").Value;
            Password = SimpleCrypto.EncryptDecrypt(deviceElement.Attribute("password").Value);
        }

        public XElement CreateMemento()
        {
            Log.Information("=> VsoDevice.CreateMemento");

            var deviceElement = new XElement("device");
            deviceElement.Add(new XAttribute("id", Id));
            deviceElement.Add(new XAttribute("name", Name));
            deviceElement.Add(new XAttribute("uri", Uri));
            deviceElement.Add(new XAttribute("polling-interval", PollingInterval));
            deviceElement.Add(new XAttribute("username", UserName));
            deviceElement.Add(new XAttribute("password", SimpleCrypto.EncryptDecrypt(Password)));
            return deviceElement;
        }
    }
}