using System;
using System.Collections;
using System.Collections.Generic;
using Core.Attributes;
using UnityEngine;

namespace Core
{
  /// <summary>
  /// Store actions and coroutines that can be executed in main game loop.
  /// </summary>
  [CoreRegister(true)]
  public sealed class CoreMainThreadActionsQueue
  {
    private readonly Queue<Action> _threadedActions;
    private readonly Queue<IEnumerator> _coroutines;

    public CoreMainThreadActionsQueue()
    {
      _threadedActions = new Queue<Action>(48);
      _coroutines = new Queue<IEnumerator>(24);
    }

    public void Enqueue(Action action)
    {
      lock (_threadedActions)
      {
        _threadedActions.Enqueue(action);
      }
    }

    public bool Dequeue(out Action result)
    {
      lock (_threadedActions)
      {
        if (_threadedActions.Count > 0)
        {
          result = _threadedActions.Dequeue();
          return true;
        }
      }

      result = null;
      return false;
    }

    public void Enqueue(IEnumerator coroutine)
    {
      lock (_coroutines)
      {
        _coroutines.Enqueue(coroutine);
      }
    }

    public void EnqueueWithFrameDelay(IEnumerator coroutine)
    {
      lock (_coroutines)
      {
        _coroutines.Enqueue(PerformWithFrameDelay(coroutine));
      }
    }

    public void EnqueueWithDelay(IEnumerator coroutine, float delay)
    {
      lock (_coroutines)
      {
        _coroutines.Enqueue(PerformWithDelay(coroutine, delay));
      }
    }

    private IEnumerator PerformWithFrameDelay(IEnumerator coroutine)
    {
      yield return new WaitForEndOfFrame();
      yield return coroutine;
    }

    private IEnumerator PerformWithDelay(IEnumerator coroutine, float delay)
    {
      yield return new WaitForSeconds(delay);
      yield return coroutine;
    }

    public bool Dequeue(out IEnumerator result)
    {
      lock (_coroutines)
      {
        if (_coroutines.Count > 0)
        {
          result = _coroutines.Dequeue();
          return true;
        }
      }

      result = null;
      return false;
    }
  }
}
