namespace MVVM.Controls
{
  public interface IDataSourceView<T>
  {
    /// <summary>
    /// Kind of constructor.
    /// </summary>
    /// <param name="source">Data source that is used for binding this view.</param>
    void Bind(DataSource<T> source);

    /// <summary>
    /// Function is triggered after binding.
    /// </summary>
    void AfterBinding();

    /// <summary>
    /// Underlying source accessor.
    /// </summary>
    DataSource<T> Source { get; }

    /// <summary>
    /// Provides possibility to change active source element.
    /// </summary>
    /// <param name="index">Element index.</param>
    void Select(int index);

    /// <summary>
    /// Accessor for source active element.
    /// </summary>
    T Active { get; }

    /// <summary>
    /// After selection that is triggered after Select function but also can be triggered by source as self.
    /// </summary>
    /// <param name="active">Current active element.</param>
    void AfterSelectionChanged(T active);

    /// <summary>
    /// Triggered just before AfterSelectionChanged.
    /// Can be used for UnSelecting all the other elements.
    /// </summary>
    void BeforeSelectionChanged();
  }
}
