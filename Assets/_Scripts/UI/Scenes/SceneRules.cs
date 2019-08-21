using System;
using Core;
using Core.Attributes;
using Development.Logging;
using UnityEngine;
#pragma warning disable 649

namespace UI.Scenes
{
  public abstract class SceneRules : CoreBehaviour, ISceneGoBack
  {
    [SerializeField] private bool _canGoBack;

    public bool CanGoBack => _canGoBack;

    [CoreRegisterEventHandler(typeof(HardwareBackButtonEvent))]
    public void OnHardwareBackButtonClicked(object sender, EventArgs arg)
    {
      this.Log("Scene rules hardware back button catch.", LogLevel.EditorInfo);
      OnGoBack();
    }

    public abstract void OnGoBack();
  }
}
