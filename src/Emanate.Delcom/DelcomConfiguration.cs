using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomConfiguration : IOutputConfiguration
    {
        private const string key = "delcom";
        private const string name = "Delcom";

        string IOutputConfiguration.Key { get { return key; } }
        string IOutputConfiguration.Name { get { return name; } }

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
            Log.Information("=> DelcomConfiguration.GenerateEmptyProfile");
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
            Log.Information("=> DelcomConfiguration.AddDefaultProfile");
            var defaultProfile = (MonitoringProfile)GenerateEmptyProfile(newKey);
            foreach (var profileState in defaultProfile.States)
            {
                switch (profileState.BuildState)
                {
                    case BuildState.Unknown:
                    case BuildState.Error:
                        profileState.Red = true;
                        profileState.Flash = true;
                        break;
                    case BuildState.Failed:
                        profileState.Red = true;
                        break;
                    case BuildState.Running:
                        profileState.Yellow = true;
                        profileState.Flash = true;
                        break;
                    case BuildState.Succeeded:
                        profileState.Green = true;
                        break;
                }
            }
            Profiles.Add(defaultProfile);
            return defaultProfile;
        }

        public event EventHandler<OutputDeviceEventArgs> OutputDeviceAdded;
        public void AddOutputDevice(IOutputDevice outputDevice)
        {
            Log.Information("=> DelcomConfiguration.AddOutputDevice");
            outputDevices.Add(outputDevice);
            OutputDeviceAdded?.Invoke(this, new OutputDeviceEventArgs(this, outputDevice));
        }

        public event EventHandler<OutputDeviceEventArgs> OutputDeviceRemoved;
        public void RemoveOutputDevice(DelcomDevice outputDevice)
        {
            Log.Information("=> DelcomConfiguration.RemoveOutputDevice");
            outputDevices.Remove(outputDevice);
            OutputDeviceAdded?.Invoke(this, new OutputDeviceEventArgs(this, outputDevice));
        }

        public Memento CreateMemento()
        {
            Log.Information("=> DelcomConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            // TODO: Move key and type tagging to CareTaker
            moduleElement.Add(new XAttribute("key", key));
            moduleElement.Add(new XAttribute("type", "output"));

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
            Log.Information("=> DelcomConfiguration.SetMemento");
            if (memento.Key != key)
                Log.Warning("Possible attempt to load non-Delcom configuration");

            var element = memento.Element;
            var profilesElement = element.Element("profiles");
            if (profilesElement != null)
            {
                foreach (var profileElement in profilesElement.Elements("profile"))
                {
                    var profile = new MonitoringProfile
                    {
                        Key = profileElement.GetAttributeString("key"),
                        Decay = profileElement.GetAttributeUint("decay"),
                        HasRestrictedHours = profileElement.GetAttributeBoolean("restrictedhours"),
                        StartTime = profileElement.GetAttributeUint("starttime"),
                        EndTime = profileElement.GetAttributeUint("endtime"),
                    };

                    foreach (var stateElement in profileElement.Elements("state"))
                    {
                        var state = new ProfileState
                        {
                            BuildState = stateElement.GetAttributeEnum("name", BuildState.Unknown),
                            Green = stateElement.GetAttributeBoolean("green"),
                            Yellow = stateElement.GetAttributeBoolean("yellow"),
                            Red = stateElement.GetAttributeBoolean("red"),
                            Flash = stateElement.GetAttributeBoolean("flash"),
                            Buzzer = stateElement.GetAttributeBoolean("buzzer")
                        };
                        profile.States.Add(state);
                    }

                    Profiles.Add(profile);
                }
            }
            else
                Log.Warning("Missing element: profiles");

            var devicesElement = element.Element("devices");
            if (devicesElement != null)
            {
                foreach (var deviceElement in devicesElement.Elements("device"))
                {
                    var profileKey = deviceElement.GetAttributeString("profile");
                    if (string.IsNullOrWhiteSpace(profileKey))
                    {
                        Log.Warning("Ignoring invalid profile key");
                        continue;
                    }

                    var device = new DelcomDevice
                    {
                        Id = deviceElement.GetAttributeString("id"),
                        Name = deviceElement.GetAttributeString("name"),
                        Profile = Profiles.Single(p => p.Key == profileKey)
                    };

                    AddOutputDevice(device);
                }
            }
            else
                Log.Warning("Missing element: devices");

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
    }
}