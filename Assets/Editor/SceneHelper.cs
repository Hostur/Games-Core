using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using UI.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
  public class SceneHelper : EditorWindow
  {
    private const string SCENE_PATH = "Assets/_Scenes/{0}.unity";
    private struct SceneRecord
    {
      public string SceneName { get; }
      public string ScenePath { get; }

      public SceneRecord(string sceneName, string scenePath)
      {
        SceneName = sceneName;
        ScenePath = scenePath;
      }
    }

    private List<SceneRecord> _scenes;

    [MenuItem("Core/Views/Show scenes  %#e")]
    private static void InitializeWindow()
    {
      var window = (SceneHelper)GetWindow(typeof(SceneHelper));
      window.Show();
    }

    private AutocompleteSearchField _autocompleteSearchField;

    private void OnEnable()
    {
      _scenes = new List<SceneRecord>(30);
      foreach (var s in CollectionsExtension.ForEachEnum<SceneName>())
      {
        _scenes.Add(new SceneRecord(s.ToString(), string.Format(SCENE_PATH, s)));
      }

      if (_autocompleteSearchField == null) _autocompleteSearchField = new AutocompleteSearchField("Initialization");
      _autocompleteSearchField.OnInputChanged = OnInputChanged;
      _autocompleteSearchField.onConfirm = OnConfirm;
      _autocompleteSearchField.RefreshResult("Initialization");
    }

    private void OnGUI()
    {
      GUILayout.Label("Search for scene", EditorStyles.boldLabel);
      _autocompleteSearchField.OnGUI();
    }

    private void OnInputChanged(string searchString)
    {
      _autocompleteSearchField.ClearResults();
      if (!string.IsNullOrEmpty(searchString))
      {
        foreach (SceneRecord sceneRecord in _scenes.Where(s => s.SceneName.ToLower().StartsWith(searchString.ToLower())))
        {
          _autocompleteSearchField.AddResult(sceneRecord.ScenePath);
        }      
      }
    }

    private void OnConfirm(string result)
    {
      EditorSceneManager.OpenScene(result);
      Close();
    }
  }
}
