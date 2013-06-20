using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class DelcomConfigurationViewModel : ViewModel
    {
        private readonly DelcomConfiguration delcomConfiguration;

        public DelcomConfigurationViewModel(DelcomConfiguration delcomConfiguration)
        {
            this.delcomConfiguration = delcomConfiguration;
            IsEditable = delcomConfiguration != null;

            AddProfileCommand = new DelegateCommand(AddProfile);
        }

        public ICommand AddProfileCommand { get; private set; }

        private void AddProfile()
        {
            var addProfileViewModel = new AddProfileViewModel(Profiles);
            var addProfileView = new AddProfileView { DataContext = addProfileViewModel };
            addProfileView.Owner = Application.Current.MainWindow;
            addProfileView.ShowDialog();
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged("IsEditable"); }
        }

        public ObservableCollection<IOutputProfile> Profiles
        {
            get { return delcomConfiguration.Profiles; }
        }
    }

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
        }
    }
}