using System.Collections.ObjectModel;
using Emanate.Core;

namespace Emanate.TeamCity
{
    class InputSelectorViewModel : ViewModel
    {
        public override void Initialize()
        {
            var project = new ProjectViewModel();
            project.Id = "MyProjectId";
            project.Name = "MyProjectName";
            project.Configurations.Add(new ConfigurationViewModel { Id = "MyConfigId", Name = "MyConfigName" });
            project.Configurations.Add(new ConfigurationViewModel { Id = "MyConfigId2", Name = "MyConfigName2" });
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