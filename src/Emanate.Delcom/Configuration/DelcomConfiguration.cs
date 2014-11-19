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

        public IOutputProfile GenerateEmptyProfile(string newKey = "")
        {
            var defaultProfile = new MonitoringProfile
            {
                Key = newKey,
                HasRestrictedHours = false,
                StartTime = 0,
                EndTime = 24
            };

            foreach (BuildState buildState in Enum.GetValues(typeof(BuildState)))
                defaultProfile.States.Add(new ProfileState { BuildState = buildState });

            return defaultProfile;
        }

        public IOutputProfile AddDefaultProfile(string newKey)
        {
            var defaultProfile = GenerateEmptyProfile(newKey);
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
                profileElement.Add(new XAttribute("restrictedhours", profile.HasRestrictedHours));
                profileElement.Add(new XAttribute("starttime", profile.StartTime));
                profileElement.Add(new XAttribute("endtime", profile.EndTime));

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
            if (profilesElement != null)
            {
                foreach (var profileElement in profilesElement.Elements("profile"))
                {
                    var profile = new MonitoringProfile
                    {
                        Key = ParseOptionalString(profileElement, "key"),
                        Decay = ParseOptionalUint(profileElement, "decay"),
                        HasRestrictedHours = ParseOptionalBoolean(profileElement, "restrictedhours"),
                        StartTime = ParseOptionalUint(profileElement, "starttime"),
                        EndTime = ParseOptionalUint(profileElement, "endtime"),
                    };

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
            }

            var devicesElement = element.Element("devices");
            if (devicesElement != null)
            {
                foreach (var deviceElement in devicesElement.Elements("device"))
                {
                    var profileKey = ParseOptionalString(deviceElement, "profile");
                    if (string.IsNullOrWhiteSpace(profileKey))
                        continue;

                    var device = new DelcomDevice
                    {
                        Id = deviceElement.Attribute("id").Value,
                        Name = deviceElement.Attribute("name").Value,
                        Profile = Profiles.Single(p => p.Key == profileKey)
                    };

                    AddOutputDevice(device);
                }
            }

            for (uint i = 1; ; i++)
            {
                var delcom = new DelcomHid();
                if (delcom.OpenNthDevice(i) != 0)
                    break;

                var physicalDevice = new PhysicalDevice(delcom);
                var deviceId = physicalDevice.Name;

                var delcomDevice = outputDevices.OfType<DelcomDevice>().SingleOrDefault(d => d.Id == deviceId);
                if (delcomDevice != null)
                    delcomDevice.PhysicalDevice = physicalDevice;
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

        private static uint ParseOptionalUint(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                uint value;
                if (uint.TryParse(attribute.Value, out value))
                    return value;
            }
            return 0;
        }

        private static string ParseOptionalString(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                return attribute.Value;
            }
            return "";
        }
    }
}