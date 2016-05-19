using System.Collections.ObjectModel;
using Emanate.Core.Output;
using Emanate.Extensibility;

namespace Emanate.Delcom
{
    public class MonitoringProfile : ViewModel, IOutputProfile
    {
        private string key;
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProfileState> states = new ObservableCollection<ProfileState>();
        public ObservableCollection<ProfileState> States
        {
            get { return states; }
            set { states = value; OnPropertyChanged(); }
        }

        private uint decay;
        public uint Decay
        {
            get { return decay; }
            set { decay = value; OnPropertyChanged(); }
        }

        private bool hasRestrictedHours;
        public bool HasRestrictedHours
        {
            get { return hasRestrictedHours; }
            set { hasRestrictedHours = value; OnPropertyChanged(); }
        }

        private uint startTime;
        public uint StartTime
        {
            get { return startTime; }
            set { startTime = value; OnPropertyChanged(); }
        }

        private uint endTime;
        public uint EndTime
        {
            get { return endTime; }
            set { endTime = value; OnPropertyChanged(); }
        }
    }
}