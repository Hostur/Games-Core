using System;
using System.Linq;
using System.Reflection;
using Core.Attributes;

namespace MVVM
{
  /// <summary>
  /// Bind <see cref="View{T}"/> property with associated <see cref="ViewModelBase"/> property.
  /// It requires on <see cref="ViewModelBase"/> implementation to implement property of the same type and name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyBindAttribute : Attribute
  {
  }

  /// <summary>
  /// Load image from given asset bundle by image name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class LazyImageAttribute : Attribute
  {
    public string AssetBundle { get; }
    public string Name { get; }

    public LazyImageAttribute(string assetBundle, string name)
    {
      AssetBundle = assetBundle;
      Name = name;
    }
  }

  /// <summary>
  /// Bind <see cref="View{T}"/> button with associated <see cref="ViewModelBase"/> function (OnClick) and boolean property
  /// that indicates whether button should be interactable.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class ButtonBindAttribute : Attribute
  {
    public string OnButtonClickFunctionName { get; }
    public string OnButtonEnableChangedPropertyName { get; }

    /// <param name="onButtonClickFunctionName"><see cref="ViewModelBase"/> function that will be invoked on button click.</param>
    /// <param name="onButtonEnableChangedPropertyName"><see cref="ViewModelBase"/> boolean property that will be binded with button interactable setter.</param>
    public ButtonBindAttribute(string onButtonClickFunctionName, string onButtonEnableChangedPropertyName = null)
    {
      OnButtonClickFunctionName = onButtonClickFunctionName;
      OnButtonEnableChangedPropertyName = onButtonEnableChangedPropertyName;
    }
  }

  [AttributeUsage(AttributeTargets.Field)]
  public class InputFieldBindAttribute : Attribute
  {
    public string ViewModelStringProperty { get; }

    public InputFieldBindAttribute(string viewModelStringProperty)
    {
      ViewModelStringProperty = viewModelStringProperty;
    }
  }

  [AttributeUsage(AttributeTargets.Field)]
  public class ToggleBindAttribute : Attribute
  {
    public string ViewModelStringProperty { get; }

    public ToggleBindAttribute(string viewModelStringProperty)
    {
      ViewModelStringProperty = viewModelStringProperty;
    }
  }

  [AttributeUsage(AttributeTargets.Field)]
  public class TextBindAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SelectableElementAttribute : Attribute
  {
    public int Order { get; }
    public bool ActionOnFocus { get; }
    public SelectableElementAttribute(int order, bool actionOnFocus = false)
    {
      Order = order;
      ActionOnFocus = actionOnFocus;
    }
  }

  public abstract partial class View<T> where T : ViewModelBase
  {
    /// <summary>
    /// Gets a method info from given view model by its name.
    /// </summary>
    /// <typeparam name="TT">View model type.</typeparam>
    /// <param name="name">Function name (provided by <see cref="ButtonBindAttribute"/>).</param>
    /// <param name="vieModel">View model.</param>
    /// <param name="vieModelMethod">Result with method.</param>
    /// <returns>Value indicating whether function was found.</returns>
    private bool TryToGetAssignableFunction<TT>(string name, TT vieModel, out MethodInfo vieModelMethod)
      where TT : ViewModelBase
    {
      vieModelMethod = typeof(TT).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      return vieModelMethod != null;
    }

    /// <summary>
    /// Try to get field from given view model that fit view field by name and type.
    /// </summary>
    /// <typeparam name="TT">View model type.</typeparam>
    /// <param name="property">View field that we want to bind with view model field.</param>
    /// <param name="vieModel">View model.</param>
    /// <param name="vieModelProperty">Result with view model field.</param>
    /// <returns>Value indicating whether view model field was found.</returns>
    private bool TryToGetAssignableMemberInfo<TT>(PropertyInfo property, TT vieModel, out PropertyInfo vieModelProperty)
      where TT : ViewModelBase
    {
      vieModelProperty = typeof(TT).GetProperty(property.Name,
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      if (vieModelProperty == null)
      {
        return false;
      }

      if (vieModelProperty.PropertyType == property.PropertyType)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Try to get field from given view model that fit view field by name and type.
    /// </summary>
    /// <typeparam name="TT">View model type.</typeparam>
    /// <param name="field">View field that we want to bind with view model field.</param>
    /// <param name="vieModel">View model.</param>
    /// <param name="vieModelProperty">Result with view model field.</param>
    /// <param name="typeCheck"></param>
    /// <returns>Value indicating whether view model field was found.</returns>
    private bool TryToGetAssignableMemberInfo<TT>(FieldInfo field, TT vieModel, out PropertyInfo vieModelProperty, bool typeCheck = true)
      where TT : ViewModelBase
    {
      vieModelProperty = typeof(TT).GetProperty(field.Name,
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      if (vieModelProperty == null)
      {
        return false;
      }

      return !typeCheck || vieModelProperty.PropertyType == field.FieldType;
    }

    /// <summary>
    /// Try to get field from given view model that fit view field by name and type.
    /// </summary>
    /// <typeparam name="TT">View model type.</typeparam>
    /// <param name="propertyName">View field name that we want to bind with view model field.</param>
    /// <param name="vieModel">View model.</param>
    /// <param name="vieModelProperty">Result with view model field.</param>
    /// <returns>Value indicating whether view model field was found.</returns>
    private bool TryToGetAssignableMemberInfo<TT, PT>(string propertyName, TT vieModel, out PropertyInfo vieModelProperty)
      where TT : ViewModelBase
    {
      vieModelProperty = typeof(TT).GetProperty(propertyName,
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      if (vieModelProperty == null)
      {
        return false;
      }

      if (vieModelProperty.PropertyType == typeof(PT))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Try to get view model boolean field by field name.
    /// This field provides boolean value that indicates whether binded button should be interactable.
    /// </summary>
    /// <typeparam name="TT">View model type.</typeparam>
    /// <param name="propertyName">Boolean field name.</param>
    /// <param name="vieModel">View model.</param>
    /// <param name="vieModelProperty">Result with boolean field.</param>
    /// <returns>Value indicating whether field was found.</returns>
    private bool TryToGetAssignableButtonEnableMemberInfo<TT>(string propertyName, TT vieModel,
      out PropertyInfo vieModelProperty) where TT : ViewModelBase
    {
      vieModelProperty = typeof(TT).GetProperty(propertyName,
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      if (vieModelProperty == null)
      {
        return false;
      }

      if (vieModelProperty.PropertyType == typeof(bool))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// View shouldn't catch events by itself. This function helps to avoid overt events subscription.
    /// </summary>
    private void CheckForEventHandlerRegistration()
    {
      if (GetType()
        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        .Any(m => m.IsDefined(typeof(CoreRegisterEventHandlerAttribute), false)))
      {
        throw new Exception($"View: '{GetType().FullName}' trying to register events handler.\n" +
                            "You shouldn't catch any events on view instead of that register event handler on view model \n" +
                            "and bind view properties with view model that apply changes on its properties.");
      }
    }
  }
}
