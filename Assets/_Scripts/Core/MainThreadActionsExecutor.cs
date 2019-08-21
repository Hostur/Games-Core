using System;
using System.Collections;
using Core.Attributes;
using Development.Logging;

#pragma warning disable 649

namespace Core
{
  /// <summary>
  /// Scene singletone responsible for executing all the actions and coroutines on the main thread.
  /// </summary>
  public class MainThreadActionsExecutor : CoreBehaviour
  {
    [CoreInject]
    private CoreMainThreadActionsQueue _mainThreadActionsQueue;

    private Action _reusableAction;
    private IEnumerator _reusableIEnumerator;

    protected override void OnAwake()
    {
      base.OnAwake();
      DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
      try
      {
        while (_mainThreadActionsQueue.Dequeue(out _reusableAction))
        {
          _reusableAction?.Invoke();
        }

        while (_mainThreadActionsQueue.Dequeue(out _reusableIEnumerator))
        {
          StartCoroutine(_reusableIEnumerator);
        }
      }
      catch (Exception e)
      {
        this.Log($"Exception occur during executing action or coroutine on the main thread. \n{e}", LogLevel.Error);
      }
    }
  }
}
