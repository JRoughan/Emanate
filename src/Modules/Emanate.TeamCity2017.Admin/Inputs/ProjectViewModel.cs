using System.Collections.ObjectModel;
using Emanate.Extensibility;

namespace Emanate.TeamCity.Admin.Inputs
{
    public class ProjectViewModel : ViewModel
    {
        public ProjectViewModel()
        {
            configurations = new ObservableCollection<ProjectConfigurationViewModel>();
            configurations.CollectionChanged += (s, e) => CheckStatus();
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private bool? isSelected;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value.HasValue && (!isSelected.HasValue || isSelected.Value != value.Value))
                {
                    foreach (var configuration in configurations)
                        configuration.IsSelected = value.Value;
                }
                isSelected = value; OnPropertyChanged();
            }
        }

        private ObservableCollection<ProjectConfigurationViewModel> configurations;
        public ObservableCollection<ProjectConfigurationViewModel> Configurations
        {
            get { return configurations; }
            set { configurations = value; OnPropertyChanged(); }
        }

        public void CheckStatus()
        {
            bool? projectSelection = null;
            foreach (var configuration in Configurations)
            {
                if (configuration.IsSelected)
                {
                    if ((projectSelection.HasValue && !projectSelection.Value))
                    {
                        projectSelection = null;
                        break;
                    }
                    projectSelection = true;
                }
                else
                {
                    if (projectSelection.HasValue && projectSelection.Value)
                    {
                        projectSelection = null;
                        break;
                    }
                    projectSelection = false;
                }

            }

            if (isSelected != projectSelection)
            {
                isSelected = projectSelection;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}