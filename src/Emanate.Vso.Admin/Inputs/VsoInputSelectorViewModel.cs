using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Vso.Admin.Inputs
{
    public class VsoInputSelectorViewModel : ViewModel
    {
        private readonly IVsoConnection connection;

        public VsoInputSelectorViewModel(VsoDevice device)
        {
            connection = new VsoConnection(device);
        }

        public override async Task<InitializationResult> Initialize()
        {
            Log.Information("=> InputSelectorViewModel.Initialize");
            ProjectCollection projectCollection;
            try
            {
                projectCollection = await connection.GetProjects();
            }
            catch (WebException ex)
            {
                Log.Error("Could not get projects: " + ex.Message);
                HasBadConfiguration = true;
                return InitializationResult.Failed;
            }

            foreach (var project in projectCollection.Value)
            {
                var projectVm = new ProjectViewModel(project);

                BuildDefinitionCollection buildDefinitionCollection;
                try
                {
                    buildDefinitionCollection = await connection.GetBuildDefinitions(projectVm.Id);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not get build definitions: " + ex.Message);
                    HasBadConfiguration = true;
                    return InitializationResult.Failed;
                }

                foreach (var buildDefinition in buildDefinitionCollection.Value)
                {
                    var configuration = new ProjectConfigurationViewModel(projectVm, buildDefinition);
                    projectVm.Configurations.Add(configuration);
                }

                Projects.Add(projectVm);
            }
            return InitializationResult.Succeeded;
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

        public void SelectInputs(IEnumerable<string> inputs)
        {
            Log.Information("=> InputSelectorViewModel.SelectInputs");
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var inputInfo in inputs)
            {
                var parts = inputInfo.Split(':');
                var config = configurations.SingleOrDefault(c => c.Id.Equals(parts[1], StringComparison.OrdinalIgnoreCase) &&
                                                                 c.ProjectId.Equals(new Guid(parts[0])));
                if (config != null)
                    config.IsSelected = true;
            }
        }

        public IEnumerable<string> GetSelectedInputs()
        {
            Log.Information("=> InputSelectorViewModel.GetSelectedInputs");
            var configurations = Projects.SelectMany(p => p.Configurations).ToList();
            foreach (var configuration in configurations)
            {
                if (configuration.IsSelected)
                    yield return $"{configuration.ProjectId}:{configuration.Id}";
            }
        }
    }
}