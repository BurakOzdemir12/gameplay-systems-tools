using System.Collections.Generic;

namespace GameplaySystemsAndTools.Shared.Events
{
    /// <summary>
    /// Typed static event channel — the project's sanctioned decoupling backbone.
    /// One bus per event type; publishers and subscribers never know each other.
    /// </summary>
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

        // Reused snapshot list so handlers may subscribe/unsubscribe safely while a
        // publish is in flight (no collection-modified exception, no per-publish alloc).
        private static readonly List<IEventBinding<T>> snapshot = new List<IEventBinding<T>>();

        public static void Subscribe(EventBinding<T> binding) => bindings.Add(binding);
        public static void Unsubscribe(EventBinding<T> binding) => bindings.Remove(binding);

        public static void Publish(T @event)
        {
            snapshot.Clear();
            snapshot.AddRange(bindings);

            for (int i = 0; i < snapshot.Count; i++)
            {
                // Skip bindings removed by an earlier handler during this publish.
                if (!bindings.Contains(snapshot[i])) continue;

                snapshot[i].OnEvent.Invoke(@event);
                snapshot[i].OnEventNoArgs.Invoke();
            }
        }

        public static void Clear() => bindings.Clear();
    }
}
