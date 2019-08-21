using UnityEditor;
using UnityEngine;

namespace Editor
{
  public class BundleBuilder : EditorWindow
  {
    private Vector2 scrollPosition;

    [MenuItem("Core/AssetBundles Menu")]
    private static void InitializeWindow()
    {
      var window = (BundleBuilder)GetWindow(typeof(BundleBuilder));
      window.Show();
    }

    private void OnEnable()
    {
      titleContent = new GUIContent("AssetBundles");
    }

    private void OnGUI()
    {
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      DrawTittle();
      DrawAssetBundlesButtons();

      DrawHorizontalBar();
      DrawBuildAllAssetBundlesButton();

      EditorGUILayout.EndScrollView();
    }

    private static void DrawTittle()
    {
      GUI.skin.label.wordWrap = true;
      GUILayout.Label("Click on the asset bundle button to rebuild");
    }

    private static void DrawBuildAllAssetBundlesButton()
    {
      if (GUILayout.Button("Build all asset bundles."))
      {
        BuildAllAssetBundles();
      }
    }

    private static void DrawHorizontalBar()
    {
      EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
    }

    private static void DrawAssetBundlesButtons()
    {
      foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
      {
        if (GUILayout.Button(assetBundleName))
        {
          BuildGivenAssetBundle(assetBundleName);
        }
      }
    }

    [MenuItem("Core/Build All bundles &%k")]
    private static void BuildAllAssetBundles()
    {
      BuildPipeline.BuildAssetBundles(
        "Assets/AssetBundles",
        BuildAssetBundleOptions.None,
        BuildTarget.StandaloneWindows64);
      Debug.Log("All AssetBundles were rebuilt.");
    }

    private static void BuildGivenAssetBundle(string assetBundleName)
    {
      var assetBundlesToBuild = new AssetBundleBuild[1];
      assetBundlesToBuild[0] = new AssetBundleBuild();
      assetBundlesToBuild[0].assetBundleName = assetBundleName;
      assetBundlesToBuild[0].assetNames =
        AssetDatabase.GetAssetPathsFromAssetBundle(
          assetBundleName);
      BuildPipeline.BuildAssetBundles(
        "Assets/AssetBundles",
        assetBundlesToBuild,
        BuildAssetBundleOptions.None,
        BuildTarget.StandaloneWindows64);
      Debug.Log("\"" + assetBundleName + "\" AssetBundle was rebuilt.");
    }
  }
}