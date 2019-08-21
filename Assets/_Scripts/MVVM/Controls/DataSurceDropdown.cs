using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.DI;
using Resources;
using Resources.ResourcesManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.Controls
{
  public abstract class DataSourceDropdown<T> : Dropdown, IDataSourceView<T>, IDisposable
  {
    [SerializeField]
    [Header("Bundle type that represents sprites for dropdown elements.")]
    public string _bundleType;

    private AssetBundleManager _assetBundleManager;

    private List<SmartAsset<Sprite>> _sprites;

    protected override void OnDisable()
    {
      base.OnDisable();
      Dispose();
    }

    protected override void Awake()
    {
      base.Awake();
      onValueChanged.AddListener(Select);
    }

    protected virtual void OnSpritesLoaded(List<SmartAsset<Sprite>> loadedSprites)
    {
      _sprites = loadedSprites;
      var op = new List<Dropdown.OptionData>(Source.Data.Count);
      for (int i = 0; i < Source.Data.Count; i++)
      {
        string element = Source.Data[i].ToString();
        Sprite sprite = _sprites?.FirstOrDefault(s => s.Value.name == element)?.Value;
        op.Add(new Dropdown.OptionData(Source.Data[i].ToString(), sprite));
      }
      ClearOptions();
      AddOptions(op);
    }

    #region IDataSourceView  

    public virtual void Bind(DataSource<T> source)
    {
      if (Source != null) throw new Exception("Cannot bind this data source view because it already have source assigned.");
      Source = source;
      Source.OnActiveElementChanged += AfterSelectionChanged;
      Source.OnDataSourceChange += OnDataSourceChanged;
      if (_assetBundleManager == null)
        _assetBundleManager = God.PrayFor<AssetBundleManager>();
      StartCoroutine(_assetBundleManager.GetAllAssetsFromBundleAsync<Sprite>(_bundleType, OnSpritesLoaded));
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

    public void Dispose()
    {
      _sprites?.Each(s => s?.Dispose());
    }
  }
}
