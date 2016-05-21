using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Serilog;

namespace Emanate.Vso
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
                var deviceElement = device.CreateMemento();
                devicesElement.Add(deviceElement);
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
            if (devicesElement != null)
            {
                foreach (var deviceElement in devicesElement.Elements("device"))
                {
                    var device = new VsoDevice();
                    device.SetMemento(deviceElement);
                    Devices.Add(device);
                }
            }
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