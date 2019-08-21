using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MVVM.Controls
{
  public class DataSourceSimpleDropdown<T> : Dropdown, IDataSourceView<T>
  {
    #region IDataSourceView  

    public virtual void Bind(DataSource<T> source)
    {
      if (Source != null) throw new Exception("Cannot bind this data source view because it already have source assigned.");
      Source = source;
      Source.OnActiveElementChanged += AfterSelectionChanged;
      Source.OnDataSourceChange += OnDataSourceChanged;
      onValueChanged.AddListener(Select);

      var op = new List<Dropdown.OptionData>(Source.Data.Count);
      for (int i = 0; i < Source.Data.Count; i++)
      {
        op.Add(new Dropdown.OptionData(Source.Data[i].ToString()));
      }
      ClearOptions();
      AddOptions(op);
      onValueChanged?.Invoke(0);
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
    #endregion
  }
}
