using System;
using System.Collections.ObjectModel;
using Emanate.Extensibility;

namespace Emanate.Vso.Admin.Inputs
{
    public class ProjectViewModel : ViewModel
    {
        private readonly Project project;

        public ProjectViewModel(Project project)
        {
            this.project = project;
            configurations = new ObservableCollection<ProjectConfigurationViewModel>();
            configurations.CollectionChanged += (s, e) => CheckStatus();
        }

        public Guid Id
        {
            get { return project.Id; }
            set { project.Id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return project.Name; }
            set { project.Name = value; OnPropertyChanged(); }
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