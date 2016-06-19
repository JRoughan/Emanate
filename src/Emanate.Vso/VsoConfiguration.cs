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
        public string Key { get; } = "vso";

        public Memento CreateMemento()
        {
            Log.Information("=> VsoConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("key", Key));
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
            if (memento.Key != Key)
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
                    devices.Add(device);
                }
            }
        }

        public void AddDevice(VsoDevice deviceInfo)
        {
            devices.Add(deviceInfo);
        }

        private readonly List<VsoDevice> devices = new List<VsoDevice>();
        public IEnumerable<IInputDevice> Devices => devices;

        public void RemoveDevice(VsoDevice deviceInfo)
        {
            devices.Remove(deviceInfo);
        }
    }
}