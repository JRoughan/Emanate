using System.Collections.ObjectModel;
using Emanate.Core;

namespace Emanate.Delcom.Configuration
{
    public class MonitoringProfile : ViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private ObservableCollection<ProfileState> states = new ObservableCollection<ProfileState>();
        public ObservableCollection<ProfileState> States
        {
            get { return states; }
            set { states = value; OnPropertyChanged("States"); }
        }

        private uint decay;
        public uint Decay
        {
            get { return decay; }
            set { decay = value; OnPropertyChanged("Decay"); }
        }
    }
}