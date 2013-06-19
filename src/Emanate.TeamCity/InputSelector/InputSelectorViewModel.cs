using System.Collections.ObjectModel;
using Emanate.Core;

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
            //connection.GetProjects();
            var project = new ProjectViewModel();
            project.Id = "MyProjectId";
            project.Name = "MyProjectName";
            project.Configurations.Add(new ProjectConfigurationViewModel { Id = "MyConfigId", Name = "MyConfigName" });
            project.Configurations.Add(new ProjectConfigurationViewModel { Id = "MyConfigId2", Name = "MyConfigName2" });
            Projects.Add(project);
        }

        private ObservableCollection<ProjectViewModel> projects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return projects; }
            set { projects = value; OnPropertyChanged("Projects"); }
        }
    }
}