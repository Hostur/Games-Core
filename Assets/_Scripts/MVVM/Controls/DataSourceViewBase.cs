using System;
using Core;
using UnityEngine;

namespace MVVM.Controls
{
  public abstract class DataSourceViewBase<T> : CoreBehaviour, IDataSourceView<T>
  {
    public virtual void Bind(DataSource<T> source)
    {
      if (Source != null) throw new Exception("Cannot bind this data source view because it already have source assigned.");
      Source = source;
      Source.OnActiveElementChanged += AfterSelectionChanged;
      Source.OnDataSourceChange += OnDataSourceChanged;
      AfterBinding();
    }

    public virtual void AfterBinding()
    { }

    public DataSource<T> Source { get; private set; }
    public T Active => Source.Active;

    public void Select(int index)
    {
      BeforeSelectionChanged();
      Source.ViewSelection(index);
    }

    public virtual void AfterSelectionChanged(T active)
    {
    }

    public virtual void BeforeSelectionChanged()
    { }

    public virtual void OnDataSourceChanged()
    {
    }
  }
}
