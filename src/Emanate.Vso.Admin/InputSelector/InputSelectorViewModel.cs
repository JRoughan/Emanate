using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Vso.Admin.InputSelector
{
    public class InputSelectorViewModel : ViewModel
    {
        private readonly IVsoConnection connection;

        public InputSelectorViewModel(IVsoConnection connection)
        {
            this.connection = connection;
        }

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(async () =>
            {
                Log.Information("=> InputSelectorViewModel.Initialize");
                dynamic projectRefs;
                try
                {
                    var rawProjects = await connection.GetProjects();
                    projectRefs = rawProjects["value"];
                }
                catch (WebException ex)
                {
                    Log.Error("Could not get projects: " + ex.Message);
                    HasBadConfiguration = true;
                    return InitializationResult.Failed;
                }

                foreach (dynamic projectRef in projectRefs)
                {
                    var projectVm = new ProjectViewModel();
                    projectVm.Name = projectRef["name"];
                    projectVm.Id = new Guid(projectRef["id"].Value);

                    var rawBuilds = await connection.GetBuildDefinitions(projectVm.Id);
                    var buildDefinitions = rawBuilds["value"];

                    foreach (var buildDefinition in buildDefinitions)
                    {
                        var configuration = new ProjectConfigurationViewModel(projectVm);
                        configuration.Id = buildDefinition["id"];
                        configuration.Name = buildDefinition["name"];
                        configuration.Type = buildDefinition["type"];
                        configuration.ProjectId = projectVm.Id;
                        projectVm.Configurations.Add(configuration);
                    }

                    Projects.Add(projectVm);
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
                var parts = inputInfo.Id.Split(':');
                var config = configurations.SingleOrDefault(c => c.Id.Equals(parts[1], StringComparison.OrdinalIgnoreCase) &&
                                                                 c.ProjectId.Equals(new Guid(parts[0])));
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
                    yield return new InputInfo {Source = "vso", Id = $"{configuration.ProjectId}:{configuration.Id}"};
            }
        }
    }
}