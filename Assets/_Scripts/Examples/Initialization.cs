
using System.Threading.Tasks;
using Core;
using Core.Attributes;
using Development.Logging;
using UI.Scenes;
#pragma warning disable 649

namespace Examples
{
  public class Initialization : CoreBehaviour
  {
    [CoreInject]
    private CoreMainThreadActionsQueue _coreMainThreadActionsQueue;

    protected override void OnAwake()
    {
      base.OnAwake();
      Task.Run(async () => await InitializeExternalDependencies().ConfigureAwait(false)).ConfigureAwait(false);
    }

    private async Task InitializeExternalDependencies()
    {
      // For example initialize some web service providers
      this.Log("Initializing..", LogLevel.DevelopmentInfo);
      await Task.Delay(2000).ConfigureAwait(false);
      _coreMainThreadActionsQueue.Enqueue(() => Publish(new SceneChangeEvent(SceneName.MainMenu)));
    }
  }
}
