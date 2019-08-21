using System;
using System.Linq;
using System.Reflection;
using Development.Logging;

namespace Core.Reflection
{
  public static class ReflectionParser
  {
    private static readonly Type SBYTE_TYPE = typeof(sbyte);
    private static readonly Type BYTE_TYPE = typeof(byte);
    private static readonly Type SHORT_TYPE = typeof(short);
    private static readonly Type USHORT_TYPE = typeof(ushort);
    private static readonly Type FLOAT_TYPE = typeof(float);
    private static readonly Type INT_TYPE = typeof(int);
    private static readonly Type UINT_TYPE = typeof(uint);
    private static readonly Type LONG_TYPE = typeof(long);
    private static readonly Type ULONG_TYPE = typeof(ulong);
    private static readonly Type DOUBLE_TYPE = typeof(double);

    private static readonly Type[] NumericTypes = new Type[]
    {
      SBYTE_TYPE, BYTE_TYPE, SHORT_TYPE, USHORT_TYPE,
      FLOAT_TYPE, INT_TYPE, UINT_TYPE, LONG_TYPE, ULONG_TYPE, DOUBLE_TYPE
    };

    private static readonly Type STRING_TYPE = typeof(string);
    private static readonly Type BOOLEAN_TYPE = typeof(string);

    /// <summary>
    /// Check if given type can be parsed into this field type.
    /// </summary>
    /// <param name="field">Field info you want to cast into.</param>
    /// <param name="type">Type of value that you want to set on this field.</param>
    /// <returns>Value indicating whether this type can be set on this field.</returns>
    public static bool CanAssignTypeToField(this FieldInfo field, Type type)
    {
      try
      {
        if (field.FieldType.IsNumeric() && type.IsNumeric())
        {
          return true;
        }

        if (field.FieldType == STRING_TYPE && type == STRING_TYPE)
        {
          return true;
        }

        if (field.FieldType == BOOLEAN_TYPE && type == BOOLEAN_TYPE)
        {
          return true;
        }

        return false;
      }
      catch (Exception e)
      {
        field.Log("Exception occur during ReflectionParser.CanAssignTypeToField: " + e, LogLevel.Error);
        return false;
      }
    }

    /// <summary>
    /// Check if given type can be parsed into this property type.
    /// </summary>
    /// <param name="property">Property info you want to cast into.</param>
    /// <param name="type">Type of value that you want to set on this property.</param>
    /// <returns>Value indicating whether this type can be set on this property.</returns>
    public static bool CanAssignTypeToProperty(this PropertyInfo property, Type type)
    {
      if (property.PropertyType.IsNumeric() && type.IsNumeric())
      {
        return true;
      }

      if (property.PropertyType == STRING_TYPE && type == STRING_TYPE)
      {
        return true;
      }

      if (property.PropertyType == BOOLEAN_TYPE && type == BOOLEAN_TYPE)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Set value on the given field.
    /// </summary>
    /// <param name="field">Field you want to set value on.</param>
    /// <param name="inputObject">Parent of the field (object instance).</param>
    /// <param name="fieldValue">Value for this field.</param>
    public static bool SetValue(this FieldInfo field, object inputObject, object fieldValue)
    {
      try
      {
        if (!CanAssignTypeToField(field, fieldValue.GetType()))
        {
          return false;
        }

        fieldValue = Convert.ChangeType(fieldValue, field.FieldType);
        field.SetValue(inputObject, fieldValue);
        return true;
      }
      catch (Exception e)
      {
        field.Log("Exception occur during ReflectionParser.SetValue: " + e, LogLevel.Error);
        return false;
      }
    }

    /// <summary>
    /// Set value on the given property.
    /// </summary>
    /// <param name="property">Property you want to set value on.</param>
    /// <param name="inputObject">Parent of the property (object instance).</param>
    /// <param name="propertyValue">Value for this property.</param>
    public static bool SetValue(this PropertyInfo property, object inputObject, object propertyValue)
    {
      if (!CanAssignTypeToProperty(property, propertyValue.GetType()))
      {
        return false;
      }
      Type propertyType = property.PropertyType;

      //Convert.ChangeType does not handle conversion to nullable types
      //if the property type is nullable, we need to get the underlying type of the property
      var targetType = IsNullable(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;

      //Returns an System.Object with the specified System.Type and whose value is
      //equivalent to the specified object.
      propertyValue = Convert.ChangeType(propertyValue,
        targetType ?? throw new InvalidOperationException("Field value cannot be nullable"));

      property.SetValue(inputObject, propertyValue, null);
      return false;
    }

    /// <summary>
    /// Check whether given type is numeric.
    /// </summary>
    public static bool IsNumeric(this Type type)
    {
      return NumericTypes.Contains(type);
    }

    /// <summary>
    /// Check whether given type is nullable.
    /// </summary>
    private static bool IsNullable(this Type type)
    {
      // Abort if no type supplied
      if (type == null)
        return false;

      // If this is not a value type, it is a reference type, so it is automatically nullable
      // (NOTE: All forms of Nullable<T> are value types)
      if (!type.IsValueType)
        return true;

      // Report whether TypeToTest is a form of the Nullable<> type
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
  }
}
