namespace UI.Scenes.Hub
{
  public class GameSceneRules : SceneRules
  {
    public override void OnGoBack()
    {
      if (!CanGoBack) return;
      Publish(new ShowPopupEvent(PopupOptions.PositiveNegative, "EXIT", "EXIT_WINDOW_DIALOG", OnPopupResult, true));
    }

    private void OnPopupResult(PopupResult result)
    {
      if (result.ResultType == PopupResultType.Positive)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }
  }
}
