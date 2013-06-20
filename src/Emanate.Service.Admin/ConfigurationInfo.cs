using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class ConfigurationInfo
    {
        public ConfigurationInfo(string name, UserControl gui, IModuleConfiguration moduleConfiguration)
        {
            ModuleConfiguration = moduleConfiguration;
            Name = name;
            Gui = gui;
        }

        public string Name { get; private set; }
        public UserControl Gui { get; private set; }
        public IModuleConfiguration ModuleConfiguration { get; private set; }
    }

    public class OutputDeviceInfo : ViewModel
    {
        public OutputDeviceInfo(string name, IOutputDevice outputDevice, IModuleConfiguration configuration)
        {
            Name = name;
            OutputDevice = outputDevice;

            foreach (var outputProfile in configuration.Profiles)
            {
                AvailableProfiles.Add(outputProfile);
                if (outputProfile.Key.Equals(outputDevice.Profile, StringComparison.OrdinalIgnoreCase))
                    Profile = outputProfile;
            }
        }

        private IOutputProfile profile;
        public IOutputProfile Profile
        {
            get { return profile; }
            set { profile = value; OutputDevice.Profile = value.Key; OnPropertyChanged("Profile"); }
        }

        private ObservableCollection<IOutputProfile> availableProfiles = new ObservableCollection<IOutputProfile>();
        public ObservableCollection<IOutputProfile> AvailableProfiles
        {
            get { return availableProfiles; }
            set { availableProfiles = value; OnPropertyChanged("AvailableProfiles"); }
        }

        public string Name { get; private set; }
        public IOutputDevice OutputDevice { get; set; }
        public InputSelector InputSelector { get; set; } // TODO: This should handle more than one input source
    }
}