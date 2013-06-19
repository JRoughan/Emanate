using System.Collections.ObjectModel;
using Emanate.Core;

namespace Emanate.TeamCity.InputSelector
{
    public class ProjectViewModel : ViewModel
    {
        public string Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value; this.OnPropertyChanged("IsSelected");
                foreach (var configuration in configurations)
                {
                    configuration.IsSelected = value;
                }
            }
        }

        private ObservableCollection<ProjectConfigurationViewModel> configurations = new ObservableCollection<ProjectConfigurationViewModel>();
        public ObservableCollection<ProjectConfigurationViewModel> Configurations
        {
            get { return configurations; }
            set { configurations = value; OnPropertyChanged("Configurations"); }
        }
    }
}