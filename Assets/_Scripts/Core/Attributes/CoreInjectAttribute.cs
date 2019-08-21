using System;

namespace Core.Attributes
{
  /// <summary>
  /// Attribute to decorate fields and properties that should be injected in Awake function.
  /// Requested class or struct should be decorated by <see cref="CoreRegisterAttribute"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field |
                  AttributeTargets.Property)]
  public class CoreInjectAttribute : Attribute
  {
  }
}
