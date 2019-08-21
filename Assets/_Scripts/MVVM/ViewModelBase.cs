using System;
using System.Linq.Expressions;
using System.Reflection;
using Core.DI;
using Core.Reflection;
using Development.Logging;
using MVVM.Controls;

namespace MVVM
{
  /// <summary>
  /// Base logic class that is under each <see cref="View{T}"/>.
  /// This class implementations should provides all the functionality and data that is required by view.
  /// View can't have any business logic. All the required backend logic should be injected into view model.
  /// Each view model should be registered as instance per dependency.
  /// </summary>
  public abstract class ViewModelBase : IDisposable
  {
    private bool _initialized;

    /// <summary>
    /// On default initialize function bind all the view model events handlers.
    /// </summary>
    public virtual void Initialize()
    {
      if (_initialized) return;
      this.SubscribeMyEventHandlers();
      _initialized = true;
    }

    /// <summary>
    /// Delegate that view model can use to enable and disable parent view.
    /// </summary>
    public Action<bool> OnEnableDisable;

    /// <summary>
    /// Function called from base class deconstructor.
    /// </summary>
    protected virtual void OnDestroy() { }

    /// <summary>
    /// Each <see cref="View{T}"/> call this function OnAwake.
    /// It should refresh all the binded properties.
    /// </summary>
    public virtual void Refresh()
    {
    }

    /// <summary>
    /// This function should be called in property setters.
    /// It provides new property value to associated view that binds to this property.
    /// DON'T CALL IT IF VALUE DOESN'T CHANGED. It triggering UI changes which are expensive.
    /// </summary>
    /// <param name="property">Property that changed.</param>
    protected void FireOnPropertyChanged<T>(Expression<Func<T>> property)
    {
      var memberExpression = property.Body as MemberExpression;
      if (memberExpression == null)
      {
        throw new Exception("Fail to invoke FireOnPropertyChanged. Expression body is no valid.");
      }
      _onPropertyChanged?.Invoke(memberExpression.Member as PropertyInfo);
    }

    /// <summary>
    /// Assign associated view on property changed notification.
    /// The only place for using this function is <see cref="View{T}"/>
    /// </summary>
    /// <param name="onChangedHandler">Action to invoke when property changed.</param>
    public void AssignOnPropertyChanged(Action<PropertyInfo> onChangedHandler)
    {
      _onPropertyChanged += onChangedHandler;
    }

    /// <summary>
    /// This function can be called from View to bind <see cref="IDataSourceView{T}"/> with <see cref="DataSource{T}"/> on the ViewModel side.
    /// </summary>
    /// <typeparam name="T">T type shared between <see cref="IDataSourceView{T}"/> and <see cref="DataSource{T}"/></typeparam>
    /// <param name="dataSourceView">Control which can visualize <see cref="DataSource{T}"/>.</param>
    /// <param name="dataSource">Source of data - some T collection.</param>
    /// <returns>Value indicating whether you bind successfully.</returns>
    public bool BindToDataSource<T>(IDataSourceView<T> dataSourceView, DataSource<T> dataSource)
    {
      try
      {
        dataSourceView.Bind(dataSource);
        return true;
      }
      catch (Exception e)
      {
        this.Log("Exception occur during data source binding: " + e, LogLevel.Error);
        return false;
      }
    }

    /// <summary>
    /// Property changes subscribers.
    /// </summary>
    private Action<PropertyInfo> _onPropertyChanged;

    public void Dispose()
    {
      this.DisposeAllMembers();
      this.UnSubscribeMyEventHandlers();
    }
  }
}
