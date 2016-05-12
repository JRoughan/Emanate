using Emanate.Extensibility;

namespace Emanate.TeamCity.InputSelector
{
    public class ProjectConfigurationViewModel : ViewModel
    {
        private readonly ProjectViewModel project;

        public ProjectConfigurationViewModel(ProjectViewModel project)
        {
            this.project = project;
        }

        public string Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                var oldValue = isSelected;
                isSelected = value; OnPropertyChanged();

                if (isSelected != oldValue)
                {
                    project.CheckStatus();
                }
            }
        }
    }
}