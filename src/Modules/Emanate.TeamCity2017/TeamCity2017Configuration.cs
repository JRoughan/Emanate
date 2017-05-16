using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Serilog;

namespace Emanate.TeamCity2017
{
    public class TeamCity2017Configuration : IInputConfiguration
    {
        private const string key = "teamcity2017";

        string IInputConfiguration.Key { get { return key; } }

        public Memento CreateMemento()
        {
            Log.Information("=> TeamCity2017Configuration.CreateMemento");
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
            Log.Information("=> TeamCity2017Configuration.SetMemento");
            if (memento.Key != key)
                throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            var element = memento.Element;
            var devicesElement = element.Element("devices");
            foreach (var deviceElement in devicesElement.Elements("device"))
            {
                var device = new TeamCity2017Device();
                device.SetMemento(deviceElement);
                devices.Add(device);
            }
        }

        public void AddDevice(TeamCity2017Device device)
        {
            devices.Add(device);
        }

        private readonly List<TeamCity2017Device> devices = new List<TeamCity2017Device>();
        public IEnumerable<IInputDevice> Devices => devices;

        public void RemoveDevice(TeamCity2017Device device)
        {
            devices.Remove(device);
        }
    }
}