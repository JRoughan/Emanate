using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomConfiguration : IOutputConfiguration
    {
        private const string key = "delcom";
        string IOutputConfiguration.Key { get { return key; } }

        private readonly List<IProfile> profiles = new List<IProfile>();
        public IEnumerable<IProfile> Profiles
        {
            get { return profiles; }
        }

        private readonly List<IOutputDevice> outputDevices = new List<IOutputDevice>();
        public IEnumerable<IOutputDevice> OutputDevices
        {
            get { return outputDevices; }
        }

        public IProfile GenerateEmptyProfile(string newName = "")
        {
            Log.Information("=> DelcomConfiguration.GenerateEmptyProfile");
            var defaultProfile = new MonitoringProfile
            {
                Id = Guid.NewGuid(),
                Name = newName,
                HasRestrictedHours = false,
                StartTime = 0,
                EndTime = 24
            };

            foreach (BuildState buildState in Enum.GetValues(typeof(BuildState)))
                defaultProfile.States.Add(new ProfileState { BuildState = buildState });

            return defaultProfile;
        }
        
        public IProfile AddDefaultProfile(string newKey)
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
            profiles.Add(defaultProfile);
            return defaultProfile;
        }

        public void AddOutputDevice(IOutputDevice outputDevice)
        {
            Log.Information("=> DelcomConfiguration.AddOutputDevice");
            outputDevices.Add(outputDevice);
        }

        public void RemoveOutputDevice(DelcomDevice outputDevice)
        {
            Log.Information("=> DelcomConfiguration.RemoveOutputDevice");
            outputDevices.Remove(outputDevice);
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
            foreach (var profile in Profiles)
            {
                var profileElement = profile.CreateMemento(); 
                profilesElement.Add(profileElement);
            }

            var devicesElement = new XElement("devices");
            moduleElement.Add(devicesElement);
            foreach (var device in OutputDevices)
            {
                var deviceElement = device.CreateMemento();
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
                    var profile = new MonitoringProfile();
                    profile.SetMemento(profileElement);
                    profiles.Add(profile);
                }
            }
            else
                Log.Warning("Missing element: profiles");

            var devicesElement = element.Element("devices");
            if (devicesElement != null)
            {
                foreach (var deviceElement in devicesElement.Elements("device"))
                {
                    var device = new DelcomDevice();
                    device.SetMemento(deviceElement);
                    device.Profile = Profiles.Single(p => p.Id == device.ProfileId);
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
                var deviceId = physicalDevice.Id;

                var delcomDevice = outputDevices.OfType<DelcomDevice>().SingleOrDefault(d => d.PhysicalDeviceId == deviceId);
                if (delcomDevice != null)
                    delcomDevice.PhysicalDevice = physicalDevice;
            }
        }
    }
}