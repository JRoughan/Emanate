using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;

namespace Emanate.Delcom.Configuration
{
    // TODO: Split this into a config class and a VM
    public class DelcomConfiguration : ViewModel, IModuleConfiguration
    {
        private const string key = "delcom";
        private const string name = "Delcom";

        string IModuleConfiguration.Key { get { return key; } }
        string IModuleConfiguration.Name { get { return name; } }
        Type IModuleConfiguration.GuiType { get { return typeof(ConfigurationView); } }

        public DelcomConfiguration()
        {
            IsEditable = true;
        }

        private ObservableCollection<MonitoringProfile> profiles = new ObservableCollection<MonitoringProfile>();
        public ObservableCollection<MonitoringProfile> Profiles
        {
            get { return profiles; }
            set { profiles = value; OnPropertyChanged("Profiles"); }
        }

        public XElement ToXml()
        {
            IsEditable = false;

            var element = new XElement(key);
            var profilesElement = new XElement("profiles");
            element.Add(profilesElement);

            foreach (var profile in Profiles)
            {
                var profileElement = new XElement("profile");
                profileElement.Add(new XAttribute("name", profile.Name));
                profileElement.Add(new XElement("decay", profile.Decay));

                foreach (var state in profile.States)
                {
                    var stateElement = new XElement("state");
                    stateElement.Add(new XAttribute("name", state.Name));
                    stateElement.Add(new XAttribute("green", state.Green));
                    stateElement.Add(new XAttribute("yellow", state.Yellow));
                    stateElement.Add(new XAttribute("red", state.Red));
                    stateElement.Add(new XAttribute("flash", state.Flash));
                    profileElement.Add(stateElement);
                }

                profilesElement.Add(profileElement);
            }

            IsEditable = true;

            return element;
        }

        public void FromXml(XElement element)
        {
            IsEditable = false;

            // TODO
            //if (element.Name != key)
            //    throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            var profilesElement = element.Element("profiles");
            foreach (var profileElement in profilesElement.Elements("profile"))
            {
                var profile = new MonitoringProfile();
                profile.Name = profileElement.Attribute("name").Value;
                profile.Decay = uint.Parse(profileElement.Element("decay").Value);
                foreach (var stateElement in profileElement.Elements("state"))
                {
                    var state = new ProfileState();
                    state.Name = stateElement.Attribute("name").Value;
                    state.Green = bool.Parse(stateElement.Attribute("green").Value);
                    state.Yellow = bool.Parse(stateElement.Attribute("yellow").Value);
                    state.Red = bool.Parse(stateElement.Attribute("red").Value);
                    state.Flash = bool.Parse(stateElement.Attribute("flash").Value);
                    profile.States.Add(state);
                }
                Profiles.Add(profile);
            }

            IsEditable = true;
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged("IsEditable"); }
        }
    }
}