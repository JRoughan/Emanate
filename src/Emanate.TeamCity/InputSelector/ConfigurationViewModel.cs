using Emanate.Core;

namespace Emanate.TeamCity.InputSelector
{
    public class ConfigurationViewModel : ViewModel
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
            set { isSelected = value; this.OnPropertyChanged("IsSelected"); }
        }
    }
}