using Emanate.Core;

namespace Emanate.TeamCity.InputSelector
{
    public class ProjectConfigurationViewModel : ViewModel
    {
        public ProjectConfigurationViewModel()
        {
            IsProfileTarget = true;
        }

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
            set { isSelected = value; this.OnPropertyChanged("IsSelected"); }
        }

        private bool isProfileTarget;
        public bool IsProfileTarget
        {
            get { return isProfileTarget; }
            set { isProfileTarget = value; OnPropertyChanged("IsProfileTarget"); }
        }

        private string profile;
        public string Profile
        {
            get { return profile; }
            set { profile = value; OnPropertyChanged("Profile"); }
        }
    }
}