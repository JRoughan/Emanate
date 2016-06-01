namespace Emanate.Extensibility.Composition
{

    public interface ISubscriber {  }

    public interface ISubscriber<in TEvent> : ISubscriber
        where TEvent : Event
    {
        void Handle(TEvent e);
    }
}