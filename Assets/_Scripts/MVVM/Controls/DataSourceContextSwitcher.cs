using System;
using System.Collections.Generic;
using System.Linq;
using Core.DI;
using Resources;
using Resources.ResourcesManagement;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

namespace MVVM.Controls
{
  public abstract class DataSourceContextSwitcher<T> : DataSourceViewBase<T> where T : struct
  {
    [SerializeField]
    [Header("Type of asset bundle that represents sprites for this context elements.")]
    public string _bundleType;

    [SerializeField]
    private Image _activeImage;

    [SerializeField]
    private Button _forwardButton;

    [SerializeField]
    private Button _backwardButton;

    private List<SmartAsset<Sprite>> _sprites;

    private AssetBundleManager _assetBundleManager;

    public override void AfterBinding()
    {
      base.AfterBinding();
      _forwardButton.onClick.AddListener(GoForward);
      _backwardButton.onClick.AddListener(GoBackward);

      if (_assetBundleManager == null)
        _assetBundleManager = God.PrayFor<AssetBundleManager>();

      StartCoroutine(_assetBundleManager.GetAllAssetsFromBundleAsync<Sprite>(_bundleType, OnSpritesLoaded));
    }

    private void GoForward()
    {
      var index = Source.Data.IndexOf(Active);
      if (index == -1)
      {
        throw new InvalidOperationException("Given element not found in given collection.");
      }
      ++index;
      if (index >= Source.Data.Count)
      {
        index = 0;
      }
      Select(index);
      Refresh();
    }

    private void GoBackward()
    {
      var index = Source.Data.IndexOf(Active);
      if (index == -1)
      {
        throw new InvalidOperationException("Given element not found in given collection.");
      }
      --index;
      if (index < 0)
      {
        index = Source.Data.Count - 1;
      }
      Select(index);
      Refresh();
    }

    protected virtual void OnSpritesLoaded(List<SmartAsset<Sprite>> loadedSprites)
    {
      _sprites = loadedSprites;
      string element = Active.ToString();
      _activeImage.sprite = _sprites?.FirstOrDefault(s => s.Value.name == element)?.Value;
    }

    public virtual void SwitchContext(bool forward = true)
    {
      if (forward)
        GoForward();
      else
        GoBackward();
    }

    protected virtual void Refresh()
    {
      string element = Active.ToString();
      _activeImage.sprite = _sprites?.FirstOrDefault(s => s.Value.name == element)?.Value;
    }
  }
}
