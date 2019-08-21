using System;
using System.Collections;
using System.Collections.Generic;
using Core.Attributes;
using Development.Logging;
using UnityEngine;

namespace Resources
{
  namespace ResourcesManagement
  {
    using Object = UnityEngine.Object;

    [CoreRegister(true)]
    public class AssetBundleManager
    {
      public Dictionary<string, AssetBundleRecord> _assetBundles;

      public AssetBundleManager()
      {
        _assetBundles = new Dictionary<string, AssetBundleRecord>(64);
      }

      public AssetBundle GetAssetBundle(string key)
      {
        key = key.ToLower();
        if (_assetBundles.ContainsKey(key))
        {
          return _assetBundles[key].AssetBundle;
        }

        _assetBundles.Add(key, new AssetBundleRecord(key));
        return _assetBundles[key].AssetBundle;
      }

      public Dictionary<string, AssetBundleRecord> GetAssetBundleRecords()
      {
        return _assetBundles;
      }

      public IEnumerator GetAssetBundleAsync(string key, Action<AssetBundle> result)
      {
        key = key.ToLower();
        yield return null;
        if (!_assetBundles.ContainsKey(key))
        {
          _assetBundles.Add(key, new AssetBundleRecord(key, false));
        }

        if (!_assetBundles[key].Loaded)
        {
          yield return _assetBundles[key].LoadAssetBundleAsync();
        }

        while (!_assetBundles[key].Loaded)
        {
          yield return null;
        }

        result.Invoke(_assetBundles[key].AssetBundle);
      }

      public IEnumerator GetAllAssetsFromBundleAsync<T>(string assetBundleName, Action<List<SmartAsset<T>>> result) where T : Object
      {
        assetBundleName = assetBundleName.ToLower();
        AssetBundle bundle = null;
        yield return GetAssetBundleAsync(assetBundleName, assetBundle => bundle = assetBundle);

        while (bundle == null)
        {
          yield return null;
        }

        yield return LoadAllAssetsFromBundleAsync(bundle, result);
      }

      private IEnumerator LoadAllAssetsFromBundleAsync<T>(AssetBundle bundle, Action<List<SmartAsset<T>>> result) where T : Object
      {
        List<SmartAsset<T>> tmpResult = new List<SmartAsset<T>>(bundle.GetAllAssetNames().Length);
        foreach (string assetName in bundle.GetAllAssetNames())
        {
          tmpResult.Add(GetAssetFromBundle<T>(bundle.name, assetName));
        }
        result?.Invoke(tmpResult);
        yield return null;
      }

      public SmartAsset<T> GetAssetFromBundle<T>(string assetBundleName, string assetName) where T : Object
      {
        assetBundleName = assetBundleName.ToLower();
        try
        {
          if (_assetBundles.ContainsKey(assetBundleName))
          {
            if (_assetBundles[assetBundleName].AssetBundle == null || !_assetBundles[assetBundleName].Loaded)
            {
              _assetBundles[assetBundleName].UnloadAssetBundle();
              _assetBundles.Remove(assetBundleName);
              _assetBundles.Add(assetBundleName, new AssetBundleRecord(assetBundleName));
            }
            _assetBundles[assetBundleName].IncreaseRefCount();
            return new SmartAsset<T>(_assetBundles[assetBundleName].AssetBundle.LoadAsset<T>(assetName), assetName, assetBundleName);
          }

          _assetBundles.Add(assetBundleName, new AssetBundleRecord(assetBundleName));
          _assetBundles[assetBundleName].IncreaseRefCount();
          return new SmartAsset<T>(_assetBundles[assetBundleName].AssetBundle.LoadAsset<T>(assetName), assetName, assetBundleName);
        }
        catch (Exception e)
        {
          throw new Exception($"Exception occur during loading asset '{assetName}' from bundle '{assetBundleName}'. \n {e.Message}");
        }
      }

      public IEnumerator GetAssetFromBundleAsync<T>(string assetBundleName, string assetName, Action<SmartAsset<T>> assetToLoad)
        where T : Object
      {
        if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(assetName))
        {
          this.Log("Asset bundle name and asset name cannot be empty.", LogLevel.Error);
          yield break;
        }

        assetBundleName = assetBundleName.ToLower();
        if (!_assetBundles.ContainsKey(assetBundleName))
        {
          _assetBundles.Add(assetBundleName, new AssetBundleRecord(assetBundleName, false));
        }

        if (!_assetBundles[assetBundleName].Loaded)
        {
          yield return _assetBundles[assetBundleName].LoadAssetBundleAsync();
        }

        try
        {
          var bundle = _assetBundles[assetBundleName];
          if (bundle == null || !bundle.Loaded)
          {
            throw new Exception($"Bundle {assetBundleName} not loaded.");
          }

          _assetBundles[assetBundleName].IncreaseRefCount();
          assetToLoad(new SmartAsset<T>(_assetBundles[assetBundleName].AssetBundle.LoadAsset<T>(assetName), assetName, assetBundleName));
        }
        catch (Exception e)
        {
          _assetBundles[assetBundleName].Loaded = false;
          this.Log($"Exception occur in AssetBundleManager.GetAssetFromBundleAsync called with : {assetBundleName} asset {assetName}. \n {e}", LogLevel.Error);
        }
      }

      public void UnloadAssetFromBundle<T>(SmartAsset<T> smartAsset) where T : UnityEngine.Object
      {
        _assetBundles[smartAsset.AssetBundleName].DecreaseRefCount();
      }

      public class AssetBundleRecord
      {
        private const string ASSET_BUNDLES_PATH = "Assets/AssetBundles/";
        private readonly string name;
        public bool Loaded;
        private bool _loading = false;
        private AssetBundle assetBundle;
        public AssetBundle AssetBundle
        {
          get
          {
            if (Loaded)
              return assetBundle;

            LoadAssetBundle();
            return assetBundle;
          }

          private set => assetBundle = value;
        }

        private int _refCount;
        internal void IncreaseRefCount()
        {
          ++_refCount;
        }

        internal void DecreaseRefCount()
        {
          if (--_refCount <= 0)
          {
            UnloadAssetBundle();
          }
        }

        internal AssetBundleRecord(string key, bool assetBundleShouldBeLoadedSynchronically = true)
        {
          name = key;
          if (assetBundleShouldBeLoadedSynchronically)
          {
            LoadAssetBundle();
          }
        }

        public void UnloadAssetBundle(bool destroy = false)
        {
          try
          {
            if (Loaded)
            {
              if (AssetBundle == null)
              {
                throw new Exception("Asset bundle is null and loaded during unloading.");
              }
              AssetBundle.Unload(destroy);
              Loaded = false;
            }
          }
          catch (Exception e)
          {
            throw new Exception("Exception occur during unloading asset bundle: " + assetBundle?.name + "\n " + e.Message);
          }
        }

        private void LoadAssetBundle()
        {
          if (_loading) return;
          _loading = true;
          AssetBundle = AssetBundle.LoadFromFile(ASSET_BUNDLES_PATH + name);
          _loading = false;
          Loaded = true;
        }

        public IEnumerator LoadAssetBundleAsync()
        {
          if (_loading) yield break;
          _loading = true;
          var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(ASSET_BUNDLES_PATH + name);
          yield return assetBundleCreateRequest;
          AssetBundle = assetBundleCreateRequest.assetBundle;
          _loading = false;
          Loaded = true;
          yield return null;
        }

        public IEnumerator LoadAssetBundleAsync(Action<bool> onDone)
        {
          if (_loading) yield break;
          _loading = true;
          var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(ASSET_BUNDLES_PATH + name);
          assetBundleCreateRequest.completed += operation => onDone(true);
          yield return assetBundleCreateRequest;
          AssetBundle = assetBundleCreateRequest.assetBundle;
          _loading = false;
          Loaded = true;
        }
      }
    }
  }
}