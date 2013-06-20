using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Emanate.Core;
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
            string projectsXml;
            try
            {
                projectsXml = connection.GetProjects();
            }
            catch (WebException)
            {
                HasBadConfiguration = true;
                return;
            }

            var projectsElement = XElement.Parse(projectsXml);
            foreach (var projectElement in projectsElement.Elements())
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

        private bool hasBadConfiguration;
        public bool HasBadConfiguration
        {
            get { return hasBadConfiguration; }
            set { hasBadConfiguration = value; OnPropertyChanged("HasBadConfiguration"); }
        }

        private ObservableCollection<ProjectViewModel> projects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return projects; }
            set { projects = value; OnPropertyChanged("Projects"); }
        }

        public void SelectInputs(IEnumerable<InputInfo> inputs)
        {
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var inputInfo in inputs)
            {
                var config = configurations.SingleOrDefault(c => c.Id.Equals(inputInfo.Id, StringComparison.OrdinalIgnoreCase));
                if (config != null)
                    config.IsSelected = true;
            }
        }

        public IEnumerable<InputInfo> GetSelectedInputs()
        {
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var configuration in configurations)
            {
                if (configuration.IsSelected)
                    yield return new InputInfo {Source = "teamcity", Id = configuration.Id};
            }
        }
    }
}