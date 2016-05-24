using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.TeamCity.Admin.Inputs
{
    public class TeamCityInputSelectorViewModel : ViewModel
    {
        private readonly ITeamCityConnection connection;

        public TeamCityInputSelectorViewModel(TeamCityDevice device)
        {
            connection = new TeamCityConnection(device);
        }

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(() =>
            {
                Log.Information("=> InputSelectorViewModel.Initialize");
                string projectsXml;
                try
                {
                    projectsXml = connection.GetProjects();
                }
                catch (WebException ex)
                {
                    Log.Error("Could not get projects: " + ex.Message);
                    HasBadConfiguration = true;
                    return InitializationResult.Failed;
                }

                var projectsElement = XElement.Parse(projectsXml);
                foreach (var projectElement in projectsElement.Elements())
                {
                    var project = new ProjectViewModel();
                    project.Name = projectElement.GetAttributeString("name");
                    var projectId = projectElement.GetAttributeString("id");

                    var buildXml = connection.GetProject(projectId);
                    var buildRoot = XElement.Parse(buildXml);

                    var buildElements = from buildTypesElement in buildRoot.Elements("buildTypes")
                                        from buildElement in buildTypesElement.Elements("buildType")
                                        select buildElement;

                    foreach (var buildElement in buildElements)
                    {
                        var configuration = new ProjectConfigurationViewModel(project);
                        configuration.Id = buildElement.GetAttributeString("id");
                        configuration.Name = buildElement.GetAttributeString("name");
                        project.Configurations.Add(configuration);
                    }

                    Projects.Add(project);
                }
                return InitializationResult.Succeeded;
            });
            
        }

        private bool hasBadConfiguration;
        public bool HasBadConfiguration
        {
            get { return hasBadConfiguration; }
            set { hasBadConfiguration = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProjectViewModel> projects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return projects; }
            set { projects = value; OnPropertyChanged(); }
        }

        public void SelectInputs(IEnumerable<InputInfo> inputs)
        {
            Log.Information("=> InputSelectorViewModel.SelectInputs");
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
            Log.Information("=> InputSelectorViewModel.GetSelectedInputs");
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var configuration in configurations)
            {
                if (configuration.IsSelected)
                    yield return new InputInfo (configuration.Id);
            }
        }
    }
}