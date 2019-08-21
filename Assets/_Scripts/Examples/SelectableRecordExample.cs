using MVVM.Controls;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace Examples
{
  /// <summary>
  /// Record spawned by <see cref="ExampleDataSourceGridView"/> after binding when sprites for elements are loaded.
  /// </summary>
  public class SelectableRecordExample : SelectableRecord<DataSourceElementExample>
  {
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private Text _text;

    public override void Init(DataSourceElementExample value, int index, Sprite image = null)
    {
      base.Init(value, index, image);
      _text.text = value.Element.ToString();
      _button.onClick.AddListener(Select);
    }

    protected override void ApplyImage(Sprite sprite)
    {
      base.ApplyImage(sprite);
      _image.sprite = sprite;
    }
  }
}
