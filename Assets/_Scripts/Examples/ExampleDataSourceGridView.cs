using System.Collections.Generic;
using Development.Logging;
using MVVM.Controls;
using Resources;
using UnityEngine;
#pragma warning disable 649

namespace Examples
{
  /// <summary>
  /// This element is owned by <see cref="ExampleDataSourceView"/> and its content is bind through <see cref="ExampleDataSourceViewModel"/> from <see cref="DataSource{T}"/>.
  /// </summary>
  public class ExampleDataSourceGridView : DataSourceGridView<DataSourceElementExample>
  {
    [SerializeField] private SelectableRecordExample _selectableRecordExample;

    protected override void OnSpritesLoaded(List<SmartAsset<Sprite>> loadedSprites = null)
    {
      base.OnSpritesLoaded(loadedSprites);
      int i = 0;
      foreach (SmartAsset<Sprite> loadedSprite in loadedSprites)
      {
        var record = Instantiate(_selectableRecordExample);
        record.transform.SetParent(GridTransform, false);
        record.Init(Source.Data[i], i, loadedSprites[i].Value);
        record.OnSelection += Select;
        Records.Add(record);
        i++;
      }
    }

    public override void AfterSelectionChanged(DataSourceElementExample active)
    {
      base.AfterSelectionChanged(active);
      this.Log(active.Element.ToString(), LogLevel.EditorInfo);
    }
  }
}
