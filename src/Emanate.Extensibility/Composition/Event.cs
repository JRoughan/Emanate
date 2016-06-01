using Emanate.Core;

namespace Emanate.Extensibility.Composition
{
    public abstract class Event { }

    public class ProfileAddedEvent : Event
    {
        public ProfileAddedEvent(IProfile profile)
        {
            Profile = profile;
        }
        public IProfile Profile { get; private set; }
    }
}
