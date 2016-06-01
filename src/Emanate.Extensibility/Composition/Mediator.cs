using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog;

namespace Emanate.Extensibility.Composition
{
    public class Mediator : IMediator
    {
        private readonly ConcurrentDictionary<Type, List<WeakReference<ISubscriber>>> subscribers = new ConcurrentDictionary<Type, List<WeakReference<ISubscriber>>>();

        public void Subscribe<TEvent>(ISubscriber<TEvent> subscriber)
            where TEvent : Event
        {
            Log.Information($"Subscribing {subscriber.GetType()} for {typeof(TEvent)}s");
            var typeSubscribers = subscribers.GetOrAdd(typeof(TEvent), t => new List<WeakReference<ISubscriber>>());
            typeSubscribers.Add(new WeakReference<ISubscriber>(subscriber));
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : Event
        {
            var typeSubscribers = subscribers.GetOrAdd(typeof(TEvent), t => new List<WeakReference<ISubscriber>>());
            foreach (var typeSubscriber in typeSubscribers)
            {
                ISubscriber subscriber;
                if (typeSubscriber.TryGetTarget(out subscriber))
                {
                    Log.Information($"Publishing {typeof(TEvent)} to {subscriber.GetType()}");
                    ((ISubscriber<TEvent>)subscriber).Handle(@event);
                }
            }
        }
    }
}