using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Collections;
using Development.Logging;
using MVVM;
using UI;
using UI.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.MVVM
{
  public struct ViewDependencies
  {
    public string View;
    public string Scene;
    public string GameObject;
  }

  public class StaticViewAnalyzer : EditorWindow
  {
    private const string SCENE_PATH = "Assets/_Scenes/{0}.unity";
    private static string _openedFromScene;
    private static List<Scene> _scenes;
    private static Scene _currentScene;
    private List<ViewDependencies> _viewsDependencies;
    private bool _waitForSceneAnalyze;
    private IEnumerator<SceneName> _enumerator;
    private Vector2 _scrollPosition;
    private bool _finished;
    [MenuItem("Core/Views/RunStaticAnalyzer")]
    private static void InitializeWindow()
    {
      _openedFromScene = EditorSceneManager.GetActiveScene().name;
      var window = (StaticViewAnalyzer) GetWindow(typeof(StaticViewAnalyzer));
      window.Show();
    }

    private void OnEnable()
    {
      try
      {
        titleContent = new GUIContent("Views analyzer");
        _viewsDependencies = new List<ViewDependencies>();
        EditorSceneManager.sceneOpened += OnSceneLoaded;
        _enumerator = CollectionsExtension.ForEachEnum<SceneName>().GetEnumerator();
        CheckNextScene();
      }
      catch
      {
        this.Log("Static analyzer failed because of Unity :). Try again.", LogLevel.EditorInfo);
      }
    }

    private void CheckNextScene()
    {
      try
      {
        if (_enumerator.MoveNext())
          EditorSceneManager.OpenScene(string.Format(SCENE_PATH, _enumerator.Current));
        else
        {
          _viewsDependencies = _viewsDependencies.OrderBy(c => c.View).ToList();
          EditorSceneManager.sceneOpened -= OnSceneLoaded;
          _finished = true;
        }
      }
      catch
      {
        CheckNextScene();
      }
    }

    private void OnSceneLoaded(Scene scene, OpenSceneMode mode)
    {
      _currentScene = scene;
      var obj = FindObjectsOfType<CoreBehaviour>().Where(c => c.GetType().GetInterfaces().Contains(typeof(IView)));
      foreach (var view in obj)
      {
        _viewsDependencies.Add(new ViewDependencies
        {
          View = view.GetType().Name,
          GameObject = view.gameObject.name,
          Scene = scene.name
        });
      }

      CheckNextScene();
    }

    private void OnGUI()
    {
      if (_finished)
      {
        if(_viewsDependencies.Count == 0) this.Close();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        DrawViewsDependencies();
        EditorGUILayout.LabelField(string.Empty, GUI.skin.verticalSlider);
        EditorGUILayout.EndScrollView();
      }
    }

    private void DrawViewsDependencies()
    {
      foreach (ViewDependencies viewDependenciese in _viewsDependencies)
      {
        DrawViewDependency(viewDependenciese);
      }
    }

    private void DrawViewDependency(ViewDependencies dependencies)
    {
      GUILayout.BeginHorizontal();
      GUIStyle s = new GUIStyle() { fixedWidth = 200};
      GUILayout.Label(dependencies.View, s);
      GUILayout.Label(dependencies.Scene, s);
      GUILayout.Label(dependencies.GameObject, s);
      if (GUILayout.Button("Show"))
      {
        EditorSceneManager.OpenScene(string.Format(SCENE_PATH, dependencies.Scene));
        Selection.objects = new[] {GameObject.Find(dependencies.GameObject)};
      }
      GUILayout.EndHorizontal();
    }
  }
}
