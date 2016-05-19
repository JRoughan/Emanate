using Emanate.Core.Input;

namespace Emanate.Delcom
{
    public class ProfileState
    {
        public BuildState BuildState { get; set; }

        public bool Green { get; set; }

        public bool Yellow { get; set; }

        public bool Red { get; set; }

        public bool Flash { get; set; }

        public bool Buzzer { get; set; }
    }
}