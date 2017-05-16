using System;
using Emanate.Extensibility;

namespace Emanate.Vso.Admin.Inputs
{
    public class ProjectConfigurationViewModel : ViewModel
    {
        private readonly ProjectViewModel project;
        private readonly BuildDefinition buildDefinition;

        public ProjectConfigurationViewModel(ProjectViewModel project, BuildDefinition buildDefinition)
        {
            this.project = project;
            this.buildDefinition = buildDefinition;
        }

        public string Id
        {
            get { return buildDefinition.Id; }
            set { buildDefinition.Id = value; }
        }

        public string Name
        {
            get { return buildDefinition.Name; }
            set { buildDefinition.Name = value; OnPropertyChanged(); }
        }

        public Guid ProjectId => project.Id;

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

        public string Type
        {
            get { return buildDefinition.Type; }
            set { buildDefinition.Type = value; OnPropertyChanged(); }
        }
    }
}