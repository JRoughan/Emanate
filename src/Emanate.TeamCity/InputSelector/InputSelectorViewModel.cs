﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.TeamCity.InputSelector
{
    public class InputSelectorViewModel : ViewModel
    {
        private readonly ITeamCityConnection connection;

        public InputSelectorViewModel(ITeamCityConnection connection)
        {
            this.connection = connection;
        }

        public override void Initialize()
        {
            var projectsXml = connection.GetProjects();
            var foo = XElement.Parse(projectsXml);

            foreach (var projectElement in foo.Elements())
            {
                var project = new ProjectViewModel();
                project.Name = projectElement.Attribute("name").Value;

                var buildXml = connection.GetProject(projectElement.Attribute("id").Value);
                var buildRoot = XElement.Parse(buildXml);

                var buildElements = from buildTypesElement in buildRoot.Elements("buildTypes")
                                    from buildElement in buildTypesElement.Elements("buildType")
                                    select buildElement;

                foreach (var buildElement in buildElements)
                {
                    var configuration = new ProjectConfigurationViewModel();
                    configuration.Id = buildElement.Attribute("id").Value;
                    configuration.Name = buildElement.Attribute("name").Value;
                    project.Configurations.Add(configuration);
                }

                Projects.Add(project);
            }
        }

        private ObservableCollection<ProjectViewModel> projects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return projects; }
            set { projects = value; OnPropertyChanged("Projects"); }
        }

        private IOutputProfile profile;
        public IOutputProfile Profile
        {
            get { return profile; }
            set { profile = value; OnPropertyChanged("Profile"); }
        }

        private ObservableCollection<IOutputProfile> availableProfiles = new ObservableCollection<IOutputProfile>();
        public ObservableCollection<IOutputProfile> AvailableProfiles
        {
            get { return availableProfiles; }
            set { availableProfiles = value; OnPropertyChanged("AvailableProfiles"); }
        }

        public void SelectInputs(IEnumerable<InputInfo> inputs, IModuleConfiguration moduleConfiguration, string currentOutputProfile)
        {
            foreach (var profile in moduleConfiguration.Profiles)
            {
                AvailableProfiles.Add(profile);
                if (profile.Key.Equals(currentOutputProfile))
                    Profile = profile;
            }

            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var inputInfo in inputs)
            {
                var config = configurations.SingleOrDefault(c => c.Id.Equals(inputInfo.Id, StringComparison.OrdinalIgnoreCase));
                if (config != null)
                    config.IsSelected = true;
            }
        }
    }
}