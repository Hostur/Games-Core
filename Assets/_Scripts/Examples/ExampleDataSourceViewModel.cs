using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Collections;
using MVVM;
using MVVM.Controls;

namespace Examples
{
  [CoreRegister(false)]
  public class ExampleDataSourceViewModel : ViewModelBase
  {
    public DataSource<DataSourceElementExample> ExampleDataSource;

    public ExampleDataSourceViewModel()
    {
      var elements = CollectionsExtension.EnumToList<DataSourceElementExample.ElementType>();
      List<DataSourceElementExample> list = new List<DataSourceElementExample>(elements.Count);
      foreach (DataSourceElementExample.ElementType element in elements)
        list.Add(new DataSourceElementExample(element));

      ExampleDataSource = new DataSource<DataSourceElementExample>(list);
    }

    public override void Refresh()
    {
      base.Refresh();
      ExampleDataSource.Refresh();
    }
  }

  public class DataSourceElementExample
  {
    public enum ElementType { A, B, C }
    public DataSourceElementExample(ElementType element) => Element = element;
    public ElementType Element { get; }
  }
}
