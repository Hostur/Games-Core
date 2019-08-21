using UnityEditor;

namespace UI.Scenes.Initialization
{
  public class InitializationSceneRules : SceneRules
  {
    public override void OnGoBack()
    {
      if (!CanGoBack) return;
#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#else
     UnityEngine.Application.Quit();
#endif
    }
  }
}
