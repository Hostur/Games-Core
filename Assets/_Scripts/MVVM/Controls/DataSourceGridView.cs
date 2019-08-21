using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.DI;
using Resources;
using Resources.ResourcesManagement;
using UnityEngine;
#pragma warning disable 649

namespace MVVM.Controls
{
  public class DataSourceGridView<T> : DataSourceViewBase<T> where T : class
  {
    [SerializeField]
    private bool _autoRefresh;

    [SerializeField]
    protected Transform GridTransform;

    [SerializeField]
    [Header("Bundle type that represents sprites for row elements.")]
    protected string _bundleType;

    private AssetBundleManager _assetBundleManager;

    protected List<SmartAsset<Sprite>> Sprites;

    protected List<ISelectableRecord<T>> Records = new List<ISelectableRecord<T>>(50);

    private void Update()
    {
      if (_autoRefresh)
        Refresh();
      OnUpdate();
    }

    public override void AfterBinding()
    {
      //Source data count == 0 do wywalenia ? nie odswieza sie kiedy usunie się ostatni element
      if (Source == null /*|| Source.Data.Count == 0*/) return;

      if (_assetBundleManager == null)
        _assetBundleManager = God.PrayFor<AssetBundleManager>();

      if (Sprites == null)
      {
        StartCoroutine(_assetBundleManager.GetAllAssetsFromBundleAsync<Sprite>(_bundleType, OnSpritesLoaded));
      }
      else
      {
        OnSpritesLoaded();
      }
      if (Source.Data.Count > 0)
        Select(0);
    }

    protected virtual void OnSpritesLoaded(List<SmartAsset<Sprite>> loadedSprites = null)
    {
      if (loadedSprites != null)
        Sprites = loadedSprites;
    }

    public override void BeforeSelectionChanged()
    {
      base.BeforeSelectionChanged();
      Records?.ForEach(r => r.UnSelect());
    }

    public override void AfterSelectionChanged(T active)
    {
      base.AfterSelectionChanged(active);
      Records?.FirstOrDefault(r => r.Value == active)?.AfterSelection();
    }

    public virtual void Refresh() { Records?.ForEach(r => r.Refresh()); }  

    protected virtual void OnUpdate() { }
  }

  public abstract class SelectableRecord<T> : CoreBehaviour, ISelectableRecord<T>
  {
    public T Value { get; private set; }
    public int Index { get; private set; }
    public Action<int> OnSelection { get; set; }

    public virtual void Init(T value, int index, Sprite image = null)
    {
      Value = value;
      Index = index;
      if (image != null)
      {
        ApplyImage(image);
      }
    }

    public virtual void Destroy()
    {
      Destroy(gameObject);
    }

    public virtual void Select()
    {
      OnSelection?.Invoke(Index);
    }

    public virtual void AfterSelection()
    {

    }

    public virtual void UnSelect()
    {

    }

    protected virtual void ApplyImage(Sprite sprite)
    {

    }

    public virtual void Refresh()
    {

    }
  }

  public interface ISelectableRecord<T>
  {
    T Value { get; }
    int Index { get; }
    Action<int> OnSelection { get; set; }
    void Init(T value, int index, Sprite image = null);
    void Destroy();
    void Select();
    void AfterSelection();
    void UnSelect();
    void Refresh();
  }
}
