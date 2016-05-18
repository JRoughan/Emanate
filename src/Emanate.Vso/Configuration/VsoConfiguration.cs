using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Serilog;

namespace Emanate.Vso.Configuration
{
    public class VsoConfiguration : IInputConfiguration
    {
        private const string key = "vso";
        private const string name = "Visual Studio Online";

        string IInputConfiguration.Key { get { return key; } }
        string IInputConfiguration.Name { get { return name; } }

        public Memento CreateMemento()
        {
            Log.Information("=> VsoConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("key", key));
            moduleElement.Add(new XAttribute("type", "input"));
            var devicesElement = new XElement("devices");
            foreach (var device in Devices)
            {
                var deviceElement = new XElement("device");
                deviceElement.Add(new XAttribute("id", device.Id));
                deviceElement.Add(new XAttribute("name", device.Name));
                deviceElement.Add(new XAttribute("uri", device.Uri));
                deviceElement.Add(new XAttribute("polling-interval", device.PollingInterval));
                deviceElement.Add(new XAttribute("username", device.UserName));
                deviceElement.Add(new XAttribute("password", EncryptDecrypt(device.Password)));
                moduleElement.Add(deviceElement);
            }
            moduleElement.Add(devicesElement);

            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            Log.Information("=> VsoConfiguration.SetMemento");
            if (memento.Key != key)
                throw new ArgumentException("Cannot load non-Visual Studio Online configuration");

            // TODO: Error handling
            var element = memento.Element;
            var devicesElement = element.Element("devices");
            foreach (var deviceElement in devicesElement.Elements("device"))
            {
                var device = new VsoDevice
                {
                    Id = Guid.Parse(deviceElement.Attribute("id").Value),
                    Name = deviceElement.Attribute("name").Value,
                    Uri = deviceElement.Attribute("uri").Value,
                    PollingInterval = int.Parse(deviceElement.Attribute("polling-interval").Value),
                    UserName = deviceElement.Attribute("username").Value,
                    Password = EncryptDecrypt(deviceElement.Attribute("password").Value),
                };
                Devices.Add(device);
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

        public void AddDevice(VsoDevice deviceInfo)
        {
            Devices.Add(deviceInfo);
        }

        public List<VsoDevice> Devices { get; } = new List<VsoDevice>();

        public void RemoveDevice(VsoDevice deviceInfo)
        {
            Devices.Remove(deviceInfo);
        }
    }
}