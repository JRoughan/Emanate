using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
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
                    var coniguration = new ProjectConfigurationViewModel();
                    coniguration.Name = buildElement.Attribute("name").Value;
                    project.Configurations.Add(coniguration);
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
    }
}