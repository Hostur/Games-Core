using System;

namespace Core.InternalCommunication
{
  public abstract class CoreGameEvent : EventArgs, ICoreGameEvent
  {
    public string EventIdentifier => GetType().FullName;

    public bool Consumed { get; set; }

    public void Publish<T>(Type sender) where T : EventArgs
    {
      CoreGameEventsManager.Publish(sender, this as T);
    }

    public void Publish<T>() where T : EventArgs
    {
      CoreGameEventsManager.Publish(null, this as T);
    }
  }
}
