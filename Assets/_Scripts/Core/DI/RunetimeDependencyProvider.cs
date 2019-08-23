using System;
using System.Linq;
using System.Reflection;
using Core.Attributes;
using Core.InternalCommunication;
using Core.Reflection;
using Development.Logging;
using UnityEngine;

namespace Core.DI
{
  /// <summary>
  /// Self documented class.
  /// </summary>
  public static class RuntimeDependencyProvider
  {
    public static void ResolveMyDependencies<T>(this T obj)
    {
      foreach (FieldInfo fieldInfo in obj.GetFieldsWithAttribute<T, CoreInjectAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          fieldInfo.SetValue(obj, God.PrayFor(fieldInfo.FieldType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting field {fieldInfo.FieldType} into {typeof(T)}: {e.ToString()}");
        }
        
      }
      foreach (PropertyInfo propertyInfo in obj.GetPropertiesWithAttribute<T, CoreInjectAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          propertyInfo.SetValue(obj, God.PrayFor(propertyInfo.PropertyType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting property {propertyInfo.PropertyType} into {typeof(T)}: {e.ToString()}");
        }
      }
    }

    public static void InjectComponents<T>(this T obj) where T : CoreBehaviour
    {
      foreach (FieldInfo fieldInfo in obj.GetFieldsWithAttribute<T, GetAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          fieldInfo.SetValue(obj, obj.GetComponent(fieldInfo.FieldType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting field {fieldInfo.FieldType} into {typeof(T)}: {e.ToString()}");
        }
      }
      foreach (PropertyInfo propertyInfo in obj.GetPropertiesWithAttribute<T, GetAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          propertyInfo.SetValue(obj, obj.GetComponent(propertyInfo.PropertyType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting property {propertyInfo.PropertyType} into {typeof(T)}: {e.ToString()}");
        }
      }
    }

    public static void FindComponents<T>(this T obj) where T : CoreBehaviour
    {
      foreach (FieldInfo fieldInfo in obj.GetFieldsWithAttribute<T, FindAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          fieldInfo.SetValue(obj, GameObject.FindObjectOfType(fieldInfo.FieldType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting field {fieldInfo.FieldType} into {typeof(T)}: {e.ToString()}");
        }
      }
      foreach (PropertyInfo propertyInfo in obj.GetPropertiesWithAttribute<T, GetAttribute>().Where(f => f.GetValue(obj) == null))
      {
        try
        {
          propertyInfo.SetValue(obj, GameObject.FindObjectOfType(propertyInfo.PropertyType));
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during injecting property {propertyInfo.PropertyType} into {typeof(T)}: {e.ToString()}");
        }
      }
    }

    public static void SubscribeMyEventHandlers<T>(this T obj)
    {
      var methods = obj.GetMethodsWithAttribute<T, CoreRegisterEventHandlerAttribute>();

      for (int i = 0; i < methods.Length; i++)
      {
        var attribute = methods[i].GetCustomAttribute<CoreRegisterEventHandlerAttribute>();

        EventHandler handler = (EventHandler)Delegate.CreateDelegate(
          typeof(EventHandler), obj, methods[i]);

        CoreGameEventsManager.Subscribe(attribute.EventType, handler, attribute.Persistent);
      }
    }

    public static void UnSubscribeMyEventHandlers<T>(this T obj)
    {
      var methods = obj.GetMethodsWithAttribute<T, CoreRegisterEventHandlerAttribute>();

      for (int i = 0; i < methods.Length; i++)
      {
        var attribute = methods[i].GetCustomAttribute<CoreRegisterEventHandlerAttribute>();

        EventHandler handler = (EventHandler)Delegate.CreateDelegate(
          typeof(EventHandler), obj, methods[i]);

        CoreGameEventsManager.Unsubscribe(attribute.EventType, handler);
      }
    }

    public static void AssertSerializeFields<T>(this T obj) where T : CoreBehaviour
    {
      foreach (FieldInfo fieldInfo in obj.GetFieldsWithAttribute<T, SerializeField>().Where(f => f.GetValue(obj) == null))
        obj.Log($"SerializeField {fieldInfo.Name} not assigned.", LogLevel.Error);
    }
  }
}
