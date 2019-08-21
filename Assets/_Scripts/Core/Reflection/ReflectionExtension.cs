using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Reflection
{
  /// <summary>
  /// Extension for common reflection usage.
  /// </summary>
  public static class ReflectionExtension
  {
    private const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// Gets fields with given attribute type.
    /// </summary>
    /// <typeparam name="T">Typeof class which you want to search in.</typeparam>
    /// <typeparam name="TA">Attribute type.</typeparam>
    /// <param name="obj">Instance which you are search in.</param>
    /// <param name="flags">Reflection binding flags.</param>
    /// <returns>Array of field info.</returns>
    public static FieldInfo[] GetFieldsWithAttribute<T, TA>(
      this T obj,
      BindingFlags flags = DefaultFlags) where TA : Attribute
    {
      return obj.GetType().GetFields(flags).Where(p => p.IsDefined(typeof(TA), false)).ToArray();
    }

    /// <summary>
    /// Gets properties with given attribute type.
    /// </summary>
    /// <typeparam name="T">Typeof class which you want to search in.</typeparam>
    /// <typeparam name="TA">Attribute type.</typeparam>
    /// <param name="obj">Instance which you are search in.</param>
    /// <param name="flags">Reflection binding flags.</param>
    /// <returns>Array of property info.</returns>
    public static PropertyInfo[] GetPropertiesWithAttribute<T, TA>(
      this T obj,
      BindingFlags flags = DefaultFlags) where TA : Attribute
    {
      return obj.GetType().GetProperties(flags).Where(p => p.IsDefined(typeof(TA), false)).ToArray();
    }

    /// <summary>
    /// Check if given object is decorated by out Attribute param.
    /// </summary>
    /// <typeparam name="T">Type o object.</typeparam>
    /// <typeparam name="TA">Type o attribute.</typeparam>
    /// <param name="obj">Object that you want to search for attribute on.</param>
    /// <param name="attr">Out attribute result if exists.</param>
    /// <returns>Value indicating whether attribute is defined on given object.</returns>
    public static bool TryToGetAttributeOfType<T, TA>(this T obj, out TA attr) where TA : Attribute
    {
      var result = (TA)obj.GetType().GetCustomAttributes(typeof(TA), true).FirstOrDefault();
      return (attr = result) != null;
    }

    /// <summary>
    /// Gets all the properties that can be threat like TP.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TP">Type of property - for example <see cref="IDisposable"/> if you want to retrieve all the disposable properties.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of properties.</returns>
    public static PropertyInfo[] GetAssignableProperties<T, TP>(this T obj)
    {
      return obj.GetType().GetProperties(DefaultFlags).Where(p => typeof(TP).IsAssignableFrom(p.PropertyType))
        .ToArray();
    }

    /// <summary>
    /// Gets all the fields that can be threat like TF.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TF">Type of field - for example <see cref="IDisposable"/> if you want to retrieve all the disposable fields.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of fields.</returns>
    public static FieldInfo[] GetAssignableFields<T, TF>(this T obj)
    {
      return obj.GetType().GetFields(DefaultFlags).Where(f => typeof(TF).IsAssignableFrom(f.FieldType)).ToArray();
    }

    /// <summary>
    /// Gets all the properties decorated with given attribute.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TP">Type of attribute above property.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of properties.</returns>
    public static PropertyInfo[] GetPropertiesWithAttribute<T, TP>(this T obj) where TP : Attribute
    {
      return obj.GetType().GetProperties(DefaultFlags).Where(p => p.IsDefined(typeof(TP), false)).ToArray();
    }

    /// <summary>
    /// Gets all the fields decorated with given attribute.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TF">Type of attribute above field.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of fields.</returns>
    public static FieldInfo[] GetFieldsWithAttribute<T, TF>(this T obj) where TF : Attribute
    {
      return obj.GetType().GetFields(DefaultFlags).Where(f => f.IsDefined(typeof(TF), false)).ToArray();
    }

    /// <summary>
    /// Dispose each disposable field and property on this object.
    /// </summary>
    /// <typeparam name="T">Type of this object.</typeparam>
    /// <param name="obj">Object instance.</param>
    public static void DisposeAllMembers<T>(this T obj)
    {
      foreach (var propertyInfo in GetAssignableProperties<T, IDisposable>(obj))
        ((IDisposable)propertyInfo.GetValue(obj))?.Dispose();

      foreach (var fieldInfo in GetAssignableFields<T, IDisposable>(obj))
        ((IDisposable)fieldInfo.GetValue(obj))?.Dispose();
    }

    /// <summary>
    /// Gets all the methods decorated with given attribute.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TM">Type of attribute above method.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of methods.</returns>
    public static MethodInfo[] GetMethodsWithAttribute<T, TM>(this T obj) where TM : Attribute
    {
      return obj.GetType().GetMethods(DefaultFlags).Where(m => m.IsDefined(typeof(TM), false)).ToArray();
    }

    /// <summary>
    /// Gets all members info decorated with given attribute.
    /// </summary>
    /// <typeparam name="T">Type of object that we are searching on.</typeparam>
    /// <typeparam name="TF">Type of attribute above member.</typeparam>
    /// <param name="obj">Object instance that you want to search on.</param>
    /// <returns>Array of members info.</returns>
    public static MemberInfo[] GetMembersInfoWithAttribute<T, TF>(this T obj) where TF : Attribute
    {
      return obj.GetType().GetMembers(DefaultFlags).Where(f => f.IsDefined(typeof(TF), false)).ToArray();
    }

    /// <summary>
    /// Search assemblies for types decorated by given attribute.
    /// </summary>
    /// <typeparam name="T">Attribute type.</typeparam>
    /// <returns>Array of types which are decorated by given attribute.</returns>
    public static TypeWithAttribute<T>[] GetAllTypesByAttribute<T>() where T : Attribute
    {
      List<TypeWithAttribute<T>> result = new List<TypeWithAttribute<T>>(48);
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        IEnumerable<Type> tmpTypes;
        try
        {
          tmpTypes = assembly.GetTypes().Where(t => t.IsDefined(typeof(T)));
          foreach (Type tmpType in tmpTypes)
          {
            T t = tmpType.GetCustomAttribute<T>(true);
            result.Add(new TypeWithAttribute<T>(tmpType, t));
          }
        }
        catch
        {
          // Ignore assembly loading type exception.
          continue;
        }
      }
      return result.ToArray();
    }
  }

  public struct TypeWithAttribute<T>
  {
    public Type Type;
    public T Attribute;

    public TypeWithAttribute(Type type, T attribute)
    {
      Type = type;
      Attribute = attribute;
    }
  }
}