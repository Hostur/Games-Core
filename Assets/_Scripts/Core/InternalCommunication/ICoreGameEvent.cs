using System;

namespace Core.InternalCommunication
{
  public interface ICoreGameEvent
  {
    string EventIdentifier { get; }
    bool Consumed { get; set; }
    void Publish<T>(Type sender) where T : EventArgs;
    void Publish<T>() where T : EventArgs;
  }
}
