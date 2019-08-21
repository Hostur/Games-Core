using System;
using Core.DI;
using Resources.ResourcesManagement;

namespace Resources
{
  public class SmartAsset<T> : IDisposable where T : UnityEngine.Object
  {
    private bool _disposed;

    public T Value { get; private set; }

    public string AssetName { get; private set; }

    public string AssetBundleName { get; private set; }

    public SmartAsset(T value, string assetName, string assetBundleName)
    {
      Value = value;
      AssetName = assetName;
      AssetBundleName = assetBundleName.ToLower();
    }

    ~SmartAsset()
    {
      Dispose();
    }

    public void Dispose()
    {
      if (!_disposed)
      {
        BeforeDispose();
        God.PrayFor<AssetBundleManager>().UnloadAssetFromBundle(this);
        Value = null;
        _disposed = true;
        AfterDispose();
      }
    }

    protected virtual void BeforeDispose()
    {
    }

    protected virtual void AfterDispose()
    {
    }
  }
}
