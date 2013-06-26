using Emanate.Core.Input;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class ProfileState : ViewModel
    {
        private BuildState buildState;
        public BuildState BuildState
        {
            get { return buildState; }
            set { buildState = value; OnPropertyChanged("BuildState"); }
        }

        private bool green;
        public bool Green
        {
            get { return green; }
            set { green = value; OnPropertyChanged("Green"); }
        }

        private bool yellow;
        public bool Yellow
        {
            get { return yellow; }
            set { yellow = value; OnPropertyChanged("Yellow"); }
        }

        private bool red;
        public bool Red
        {
            get { return red; }
            set { red = value; OnPropertyChanged("Red"); }
        }

        private bool flash;
        public bool Flash
        {
            get { return flash; }
            set { flash = value; OnPropertyChanged("Flash"); }
        }
    }
}