using Emanate.Core.Input;
using Emanate.Extensibility;

namespace Emanate.Delcom.Configuration
{
    public class ProfileState : ViewModel
    {
        private BuildState buildState;
        public BuildState BuildState
        {
            get { return buildState; }
            set { buildState = value; OnPropertyChanged(); }
        }

        private bool green;
        public bool Green
        {
            get { return green; }
            set { green = value; OnPropertyChanged(); }
        }

        private bool yellow;
        public bool Yellow
        {
            get { return yellow; }
            set { yellow = value; OnPropertyChanged(); }
        }

        private bool red;
        public bool Red
        {
            get { return red; }
            set { red = value; OnPropertyChanged(); }
        }

        private bool flash;
        public bool Flash
        {
            get { return flash; }
            set { flash = value; OnPropertyChanged(); }
        }

        private bool buzzer;
        public bool Buzzer
        {
            get { return buzzer; }
            set { buzzer = value; OnPropertyChanged(); }
        }
    }
}