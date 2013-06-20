using System.Collections.ObjectModel;
using Emanate.Core;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class MonitoringProfile : ViewModel, IOutputProfile
    {
        private string key;
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged("Key"); }
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