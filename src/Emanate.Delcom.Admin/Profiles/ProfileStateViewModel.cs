using Emanate.Core.Input;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Profiles
{
    public class ProfileStateViewModel : ViewModel
    {
        private readonly ProfileState profileState;

        public ProfileStateViewModel(ProfileState profileState)
        {
            this.profileState = profileState;
        }

        public BuildState BuildState
        {
            get { return profileState.BuildState; }
            set { profileState.BuildState = value; OnPropertyChanged(); }
        }

        public bool Green
        {
            get { return profileState.Green; }
            set { profileState.Green = value; OnPropertyChanged(); }
        }

        public bool Yellow
        {
            get { return profileState.Yellow; }
            set { profileState.Yellow = value; OnPropertyChanged(); }
        }

        public bool Red
        {
            get { return profileState.Red; }
            set { profileState.Red = value; OnPropertyChanged(); }
        }

        public bool Flash
        {
            get { return profileState.Flash; }
            set { profileState.Flash = value; OnPropertyChanged(); }
        }

        public bool Buzzer
        {
            get { return profileState.Buzzer; }
            set { profileState.Buzzer = value; OnPropertyChanged(); }
        }
    }
}