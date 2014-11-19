using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Delcom.Configuration
{
    public class DelcomConfiguration : IModuleConfiguration
    {
        private const string key = "delcom";
        private const string name = "Delcom";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }

        private readonly ObservableCollection<IOutputProfile> profiles = new ObservableCollection<IOutputProfile>();
        public ObservableCollection<IOutputProfile> Profiles
        {
            get { return profiles; }
        }

        private readonly ObservableCollection<IOutputDevice> outputDevices = new ObservableCollection<IOutputDevice>();
        public IEnumerable<IOutputDevice> OutputDevices
        {
            get { return outputDevices; }
        }

        public IOutputProfile GenerateDefaultProfile(string newKey, bool add)
        {
            var defaultProfile = new MonitoringProfile();

            defaultProfile.Key = newKey;
            foreach (BuildState buildState in Enum.GetValues(typeof(BuildState)))
            {
                defaultProfile.States.Add(new ProfileState { BuildState = buildState });
            }

            if (add)
                Profiles.Add(defaultProfile);

            return defaultProfile;
        }

        public event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        public void AddOutputDevice(IOutputDevice outputDevice)
        {
            outputDevices.Add(outputDevice);
            var handler = OutputDeviceAdded;
            if (handler != null)
                handler(this, new OutputDeviceEventArgs(this, outputDevice));
        }

        public event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;
        public void RemoveOutputDevice(DelcomDevice outputDevice)
        {
            outputDevices.Remove(outputDevice);
            var handler = OutputDeviceAdded;
            if (handler != null)
                handler(this, new OutputDeviceEventArgs(this, outputDevice));
        }


        public Memento CreateMemento()
        {
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("type", key));

            var profilesElement = new XElement("profiles");
            moduleElement.Add(profilesElement);
            foreach (var profile in Profiles.OfType<MonitoringProfile>())
            {
                var profileElement = new XElement("profile");
                profileElement.Add(new XAttribute("key", profile.Key));
                profileElement.Add(new XAttribute("decay", profile.Decay));

                foreach (var state in profile.States)
                {
                    var stateElement = new XElement("state");
                    stateElement.Add(new XAttribute("name", Enum.GetName(typeof(BuildState), state.BuildState)));
                    stateElement.Add(new XAttribute("green", state.Green));
                    stateElement.Add(new XAttribute("yellow", state.Yellow));
                    stateElement.Add(new XAttribute("red", state.Red));
                    stateElement.Add(new XAttribute("flash", state.Flash));
                    stateElement.Add(new XAttribute("buzzer", state.Buzzer));
                    profileElement.Add(stateElement);
                }

                profilesElement.Add(profileElement);
            }

            var devicesElement = new XElement("devices");
            moduleElement.Add(devicesElement);
            foreach (var device in OutputDevices)
            {
                var deviceElement = new XElement("device");
                deviceElement.Add(new XAttribute("name", device.Name));
                deviceElement.Add(new XAttribute("id", device.Id));
                deviceElement.Add(new XAttribute("profile", device.Profile.Key));

                devicesElement.Add(deviceElement);
            }

            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            if (memento.Type != key)
                throw new ArgumentException("Cannot load non-Delcom configuration");

            // TODO: Error handling
            var element = memento.Element;
            var profilesElement = element.Element("profiles");
            foreach (var profileElement in profilesElement.Elements("profile"))
            {
                var profile = new MonitoringProfile();
                profile.Key = profileElement.Attribute("key").Value;
                profile.Decay = uint.Parse(profileElement.Attribute("decay").Value);
                foreach (var stateElement in profileElement.Elements("state"))
                {
                    var state = new ProfileState
                    {
                        BuildState = ParseOptionalEnum(stateElement, "name", BuildState.Unknown),
                        Green = ParseOptionalBoolean(stateElement, "green"), 
                        Yellow = ParseOptionalBoolean(stateElement, "yellow"),
                        Red = ParseOptionalBoolean(stateElement, "red"),
                        Flash = ParseOptionalBoolean(stateElement, "flash"),
                        Buzzer = ParseOptionalBoolean(stateElement, "buzzer")
                    };
                    profile.States.Add(state);
                }
                Profiles.Add(profile);
            }

            var configuredDevices = new List<DelcomDevice>();

            var devicesElement = element.Element("devices");
            foreach (var deviceElement in devicesElement.Elements("device"))
            {
                var device = new DelcomDevice();
                device.Name = deviceElement.Attribute("name").Value;
                device.Id = deviceElement.Attribute("id").Value;

                var profileKey = deviceElement.Attribute("profile").Value;
                device.Profile = Profiles.Single(p => p.Key == profileKey);

                configuredDevices.Add(device);
            }

            for (uint i = 1; ; i++)
            {
                var delcom = new DelcomHid();
                if (delcom.OpenNthDevice(i) != 0)
                    break;

                var physicalDevice = new PhysicalDevice(delcom);
                var deviceId = physicalDevice.Name;

                var delcomDevice = configuredDevices.SingleOrDefault(d => d.Id == deviceId);

                if (delcomDevice != null)
                {
                    delcomDevice.PhysicalDevice = physicalDevice;
                    AddOutputDevice(delcomDevice);
                }
            }
        }

        private static bool ParseOptionalBoolean(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                bool value;
                if (bool.TryParse(attribute.Value, out value))
                    return value;
            }
            return false;
        }

        private static TEnum ParseOptionalEnum<TEnum>(XElement element, string attributeName, TEnum defaultValue)
            where TEnum : struct
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                TEnum value;
                if (Enum.TryParse(attribute.Value, out value))
                    return value;
            }
            return defaultValue;
        }
    }
}