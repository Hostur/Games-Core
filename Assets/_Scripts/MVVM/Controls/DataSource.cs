using System;
using System.Collections.Generic;

namespace MVVM.Controls
{
  public class DataSource<T>
  {
    public List<T> Data { get; private set; }

    public T Active { get; private set; }

    public void ViewSelection(int index)
    {
      if (index >= Data.Count)
        throw new Exception("Invalid operation exception. \n" +
                            $"SetActive {index} exceed DataSource length {Data.Count}.");

      Active = Data[index];
      OnActiveElementChanged?.Invoke(Active);
    }

    public Action<T> OnActiveElementChanged;

    public Action OnDataSourceChange;

    public void RefreshDataSource(List<T> newValues)
    {
      Data = newValues;
      OnDataSourceChange?.Invoke();
      if (newValues.Count > 0)
        ViewSelection(0);
    }

    public DataSource(List<T> values)
    {
      Data = values;
      if (values.Count > 0)
        ViewSelection(0);
    }

    public void Refresh()
    {
      OnActiveElementChanged?.Invoke(Active);
    }
  }
}
