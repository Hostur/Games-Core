using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Balance;
using Editor.WorkbookUtils.Loaders;
using UnityEditor;
using UnityEngine;

namespace Editor.WorkbookUtils
{
  public class ExcelAssetsPostprocessor : AssetPostprocessor
  {
    private static async void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
      if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
        return;

      importedAssets = importedAssets.Where(ExcelFileLoader.IsExcelFile).ToArray();
      if (importedAssets.Length == 0)
      {
        return;
      }

      UnityEngine.Debug.Log("Postprocessing excel files...");
      try
      {
        WorkbookClassMapperResult result = await new WorkbookClassMapper().MapTypes(importedAssets).ConfigureAwait(true);
        foreach (KeyValuePair<Type, IList> keyValuePair in result)
        {
          CreateAsset(keyValuePair);
        }
      }
      catch (Exception e)
      {
        UnityEngine.Debug.Log("Excel balance files export failed: " + e);
      }
      // save
      AssetDatabase.SaveAssets();
    }

    private static void CreateAsset(KeyValuePair<Type, IList> output)
    {
      ClassMappingAttribute attr = output.Key.GetCustomAttribute<ClassMappingAttribute>(true);
      var asset = ScriptableObject.CreateInstance(attr.ScriptableType);
      IInitializableCollection collection = (IInitializableCollection)asset;
      if (collection == null) throw new Exception("ClassMappingAttribute scriptable type must be a ScriptableCollection implementation.");
      AssetDatabase.DeleteAsset(collection.OutputPath());

      collection.Initialize(output);

      AssetDatabase.CreateAsset(asset, collection.OutputPath() + ".asset");
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      UnityEngine.Debug.Log("<color=blue> [GAMESCORE] </color> Created scriptable collection: " + collection.OutputPath());
    }
  }
}