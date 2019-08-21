using System;
using System.Collections.Generic;
using System.Linq;
using Development.Logging;

namespace Core.InternalCommunication
{
  public static class CoreGameEventsManager
  {
    private static readonly Dictionary<string, IList<EventHandler>> _eventsSubscribers =
      new Dictionary<string, IList<EventHandler>>(64);

    private static readonly Dictionary<string, IList<EventHandler>> _persistentEventSubscribers =
      new Dictionary<string, IList<EventHandler>>(64);

    /// <summary>
    /// Clear event subscriptions.
    /// </summary>
    /// <param name="hard">Value indicating whether persistent subscription should be cleared.</param>
    public static void Clear(bool hard = false)
    {
      _eventsSubscribers.Clear();
      if (hard)
      {
        _persistentEventSubscribers.Clear();
      }
    }

    /// <summary>
    /// Subscribe for T type of event.
    /// </summary>
    /// <typeparam name="T">Type of event you want to subscribe for.</typeparam>
    /// <param name="handler">Event handler that receive this event.</param>
    /// <param name="persistent">Value indicating whether this is a persistent type of subscription.
    /// That mean it is "harder" binded and clearing this subscription require additional parameter in Clear function.</param>
    public static void Subscribe<T>(EventHandler handler, bool persistent = false) where T : ICoreGameEvent
    {
      string key = typeof(T).FullName;
      InternalSubscription(key, handler, persistent);
    }

    /// <summary>
    /// Non generic subscription used by DI module.
    /// </summary>
    /// <param name="T">Event type.</param>
    /// <param name="handler">Handler for this event.</param>
    /// <param name="persistent">Value indicating whether this is a persistent type of subscription.
    /// That mean it is "harder" binded and clearing this subscription require additional parameter in Clear function.</param>
    public static void Subscribe(Type T, EventHandler handler, bool persistent = false)
    {
      string key = T.FullName;
      InternalSubscription(key, handler, persistent);
    }

    private static void InternalSubscription(string key, EventHandler handler, bool persistent)
    {
      var subscription = persistent ? _persistentEventSubscribers : _eventsSubscribers;

      if (!subscription.ContainsKey(key))
      {
        subscription.Add(key, new List<EventHandler>());
      }

      if (!subscription[key].Contains(handler))
      {
        subscription[key].Add(handler);
      }
    }

    /// <summary>
    /// Unsubscribe T type of event from your event handler.
    /// </summary>
    /// <typeparam name="T">Type of event that you are no longer interested in.</typeparam>
    /// <param name="handler">Event handler that subscribed this type of event.</param>
    public static void Unsubscribe<T>(EventHandler handler) where T : ICoreGameEvent
    {
      InternalUnsubscribe(typeof(T).FullName, handler);
    }

    /// <summary>
    /// Non generic unsubscription for given handler on given type used by DI module.
    /// </summary>
    /// <param name="T">Type of event.</param>
    /// <param name="handler">Handler for this event.</param>
    public static void Unsubscribe(Type T, EventHandler handler)
    {
      InternalUnsubscribe(T.FullName, handler);
    }

    private static void InternalUnsubscribe(string key, EventHandler handler)
    {
      if (_eventsSubscribers.ContainsKey(key))
      {
        _eventsSubscribers[key].Remove(handler);
      }
      if (_persistentEventSubscribers.ContainsKey(key))
      {
        _persistentEventSubscribers[key].Remove(handler);
      }
    }

    /// <summary>
    /// Event publication function.
    /// </summary>
    /// <typeparam name="T">Type of event that we are publishing.</typeparam>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event as self.</param>
    public static void Publish<T>(object sender, T e) where T : EventArgs
    {
      if (!(e is ICoreGameEvent))
        throw new Exception("Cannot publish event args which is not derived from CoreGameEvent.");

      string key = typeof(T).FullName;

      if (string.IsNullOrEmpty(key))
      {
        throw new Exception($"Cannot recognize {typeof(T)} assembly FullName.");
      }

      if (_eventsSubscribers.ContainsKey(key))
      {
        // Get copy of the list of subscribers
        var tmp = _eventsSubscribers[key].ToList();

        // Iterate from the last subscriber to the first one.
        for (int i = tmp.Count - 1; i >= 0; i--)
        {
          // If no one already consumed this event
          if (!((ICoreGameEvent)e).Consumed)
          {
            // If subscriber wasn't removed from the subscribes in meantime
            if(_eventsSubscribers[key].Contains(tmp[i]))
              tmp[i]?.Invoke(sender, e);
          }
        }
        
      }

      if (_persistentEventSubscribers.ContainsKey(key))
      {
        foreach (EventHandler eventHandler in _persistentEventSubscribers[key])
        {
          if (!((ICoreGameEvent)e).Consumed)
          {
            eventHandler?.Invoke(sender, e);
          }
        }
      }
    }
  }
}
