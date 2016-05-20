using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Serilog;

namespace Emanate.TeamCity
{
    public class TeamCityConfiguration : IInputConfiguration
    {
        private const string key = "teamcity";
        private const string name = "TeamCity";

        string IInputConfiguration.Key { get { return key; } }
        string IInputConfiguration.Name { get { return name; } }

        public Memento CreateMemento()
        {
            Log.Information("=> TeamCityConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("key", key));
            moduleElement.Add(new XAttribute("type", "input"));
            var devicesElement = new XElement("devices");
            foreach (var device in Devices)
            {
                var deviceElement = device.CreateMemento();
                moduleElement.Add(deviceElement);
            }
            moduleElement.Add(devicesElement);
            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            Log.Information("=> TeamCityConfiguration.SetMemento");
            if (memento.Key != key)
                throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            var element = memento.Element;
            var devicesElement = element.Element("devices");
            foreach (var deviceElement in devicesElement.Elements("device"))
            {
                var device = new TeamCityDevice();
                device.SetMemento(deviceElement);
                Devices.Add(device);
            }
        }

        public void AddDevice(TeamCityDevice device)
        {
            Devices.Add(device);
        }

        public List<TeamCityDevice> Devices { get; } = new List<TeamCityDevice>();

        public void RemoveDevice(TeamCityDevice device)
        {
            Devices.Remove(device);
        }
    }
}