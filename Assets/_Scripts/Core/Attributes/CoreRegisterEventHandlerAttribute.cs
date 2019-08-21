using System;

namespace Core.Attributes
{
  /// <summary>
  /// Use this attribute to request event handler registration.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class CoreRegisterEventHandlerAttribute : Attribute
  {
    public Type EventType { get; }
    public bool Persistent { get; }

    public CoreRegisterEventHandlerAttribute(Type eventType, bool persistent = false)
    {
      EventType = eventType;
      Persistent = persistent;
    }
  }
}
