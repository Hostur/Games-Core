using System;

namespace Core.Attributes
{
  /// <summary>
  /// Each class and struct decorated by this attribute will be registered in dependency injection container.
  /// Don't use it for Game and Scriptable objects. These objects should be registered by hand in RegisteredAssemblyModule.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class |
                  AttributeTargets.Struct)]
  public sealed class CoreRegisterAttribute : Attribute
  {
    public bool IsSingletone { get; }
    public CoreRegisterAttribute(bool isSingletone = false)
    {
      IsSingletone = isSingletone;
    } 
  }
}
