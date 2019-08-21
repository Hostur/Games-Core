using Development.Logging;
using MVVM;
using MVVM.Controls;
using UnityEngine;
#pragma warning disable 649

namespace Examples
{
  public class ExampleDataSourceView : View<ExampleDataSourceViewModel>
  {
    [SerializeField] private ExampleDataSourceGridView _dataSourceGridView;

    protected override void OnAwake()
    {
      base.OnAwake();
      if (!ViewModel.BindToDataSource(_dataSourceGridView, ViewModel.ExampleDataSource))
        this.Log("Cannot bind do data source.", LogLevel.Error);
    }
  }
}
