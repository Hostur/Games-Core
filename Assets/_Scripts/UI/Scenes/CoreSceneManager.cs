using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Attributes;
using Core.Collections;
using Core.InternalCommunication;
using Development.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scenes
{
  public interface ISceneGoBack
  {
    void OnHardwareBackButtonClicked(object sender, EventArgs arg);
    bool CanGoBack { get; }
    void OnGoBack();
  }

  public class CoreSceneManager : CoreBehaviour
  {
    private static readonly Hashtable _params = new Hashtable();
    public static bool IsCurrentScene(string sceneName) => SceneManager.GetActiveScene().name.Equals(sceneName);
    public static bool ContainsParam(string key) => _params.ContainsKey(key);
    public static void RemoveParam(string key) => _params.RemoveIfExists(key);
    public static void AddParam(string key, object param) => _params.AddOrModify(key, param);

    private readonly Queue<SceneChangeEvent> _sceneChangesQueue = new Queue<SceneChangeEvent>();

    private SceneName _lastScene;

    [CoreInject]
    private CoreMainThreadActionsQueue _mainThreadActionsQueue;

    [CoreRegisterEventHandler(typeof(SceneChangeEvent))]
    private void OnSceneChangeEvent(object sender, EventArgs arg)
    {
      SceneChangeEvent e = arg as SceneChangeEvent;
      _sceneChangesQueue.Enqueue(e);
    }

    [CoreRegisterEventHandler(typeof(SceneUnloadEvent))]
    private void OnSceneUnloadEvent(object sender, EventArgs arg)
    {
      SceneUnloadEvent e = arg as SceneUnloadEvent;
      SceneManager.UnloadSceneAsync(e.Scene.ToString());
    }

    protected override void OnAwake()
    {
      base.OnAwake();
      DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
      if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.O))
        Publish(new HardwareBackButtonEvent());
    }

    private void LateUpdate()
    {
      if (_sceneChangesQueue.Count > 0)
      {
        StartCoroutine(LoadSceneInNextFrame(_sceneChangesQueue.Dequeue()));
      }
    }

    private IEnumerator LoadSceneInNextFrame(SceneChangeEvent e)
    {
      yield return new WaitForEndOfFrame();
      AddParams(e);
      var requestedScene = e.Scene.ToString();
      this.Log("Scene change requested: " + requestedScene, LogLevel.Info);

      if ((SceneName)Enum.Parse(typeof(SceneName), requestedScene) == SceneName.Back)
      {
        if (_lastScene != SceneName.Back)
        {
          if(_lastScene == SceneName.Initialization) throw new Exception("Can't load Initialization scene. Game should be initialized once. \n " +
                                                                                "Invoke scene loading with 'Back' parameter cause this problem.");
          SceneManager.LoadSceneAsync(_lastScene.ToString());
          _lastScene = SceneName.Back;
        }
        yield break;
      }

      string currentScene = SceneManager.GetActiveScene().name;

      if (currentScene != requestedScene)
      {
        _lastScene = (SceneName)Enum.Parse(typeof(SceneName), currentScene);

        if (requestedScene == "Initialization") throw new Exception("Can't load Initialization scene. Game should be initialized once.");
        SceneManager.LoadSceneAsync(requestedScene, e.LoadMode);
      }
    }

    private void AddParams(SceneChangeEvent e) => _params.AddOrModify(e.ParamKey, e.ParamValue);
  }

  public class SceneChangeEvent : CoreGameEvent
  {
    public SceneName Scene { get; }
    public string ParamKey { get; }
    public object ParamValue { get; }
    public LoadSceneMode LoadMode { get; }
    public SceneChangeEvent(SceneName scene, string paramKey = null, object paramValue = null, LoadSceneMode sceneLoadMode = LoadSceneMode.Single)
    {
      Scene = scene;
      ParamKey = paramKey;
      ParamValue = paramValue;
      LoadMode = sceneLoadMode;
    }
  }

  public class SceneUnloadEvent : CoreGameEvent
  {
    public SceneName Scene { get; }
    public SceneUnloadEvent(SceneName scene)
    {
      Scene = scene;
    }
  }
}
