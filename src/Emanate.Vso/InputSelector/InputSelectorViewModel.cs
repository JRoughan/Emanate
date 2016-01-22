using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Emanate.Vso.InputSelector
{
    public class InputSelectorViewModel : ViewModel
    {
        private readonly IVsoConnection connection;

        public InputSelectorViewModel(IVsoConnection connection)
        {
            this.connection = connection;
        }

        public override async Task<bool> Initialize()
        {
            Trace.TraceInformation("=> InputSelectorViewModel.Initialize");
            IEnumerable<TeamProjectReference> projectRefs;
            try
            {
                projectRefs = await connection.GetProjects();
            }
            catch (WebException ex)
            {
                Trace.TraceError("Could not get projects: " + ex.Message);
                HasBadConfiguration = true;
                return false;
            }

            foreach (var projectRef in projectRefs)
            {
                var projectVm = new ProjectViewModel();
                projectVm.Name = projectRef.Name;

                var buildDefinitions = await connection.GetBuildDefinitions(projectRef.Id);

                //var buildRoot = XElement.Parse(buildXml);

                //var buildElements = from buildTypesElement in buildRoot.Elements("buildTypes")
                //                    from buildElement in buildTypesElement.Elements("buildType")
                //                    select buildElement;

                foreach (var buildDefinition in buildDefinitions)
                {
                    var configuration = new ProjectConfigurationViewModel(projectVm);
                    configuration.Id = buildDefinition.Id.ToString();
                    configuration.Name = buildDefinition.Name;
                    configuration.ProjectId = buildDefinition.Project.Id;
                    projectVm.Configurations.Add(configuration);
                }

                Projects.Add(projectVm);
            }
            return true;
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
            Trace.TraceInformation("=> InputSelectorViewModel.SelectInputs");
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var inputInfo in inputs)
            {
                var config = configurations.SingleOrDefault(c => c.Id.Equals(inputInfo.Id, StringComparison.OrdinalIgnoreCase) &&
                                                                 c.ProjectId.Equals(inputInfo.ProjectId));
                if (config != null)
                    config.IsSelected = true;
            }
        }

        public IEnumerable<InputInfo> GetSelectedInputs()
        {
            Trace.TraceInformation("=> InputSelectorViewModel.GetSelectedInputs");
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var configuration in configurations)
            {
                if (configuration.IsSelected)
                    yield return new InputInfo {Source = "vso", Id = configuration.Id, ProjectId = configuration.ProjectId};
            }
        }
    }
}