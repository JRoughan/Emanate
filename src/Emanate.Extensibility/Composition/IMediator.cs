namespace Emanate.Extensibility.Composition
{
    public interface IMediator
    {
        void Subscribe<TEvent>(ISubscriber<TEvent> subscriber)
            where TEvent : Event;

        void Publish<TEvent>(TEvent @event)
            where TEvent : Event;
    }
}