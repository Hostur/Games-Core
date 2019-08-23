using System;
using System.Collections;
using Core.DI;
using Core.InternalCommunication;
using Core.Reflection;
using UnityEngine;

namespace Core
{
  /// <summary>
  /// Replace GetComponent() usage.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class GetAttribute : Attribute { }

  /// <summary>
  /// Replace FindObjectOfType() usage.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class FindAttribute : Attribute { }

  /// <summary>
  /// Base class that should be used instead of regular MonoBehaviour in most cases.
  /// </summary>
  public abstract class CoreBehaviour : MonoBehaviour, IDisposable
  {
    #region Behaviors

    private bool m_disposed;

    protected virtual void OnBeforeInjection()
    {
    }

    protected void Awake()
    {
      this.AssertSerializeFields();
      OnBeforeInjection();
      this.InjectComponents();
      this.FindComponents();
      this.ResolveMyDependencies();
      this.SubscribeMyEventHandlers();

      OnAwake();
    }

    //protected void SavePersistent<T>(ref T structure, string saveName) where T : struct
    //{
    //  new SaveableFile(saveName).Save(ref structure);
    //}

    protected virtual void Publish<T>(T gameEvent) where T : EventArgs, ICoreGameEvent
    {
      gameEvent.Publish<T>();
    }

    protected virtual void OnAwake()
    {
    }

    protected IEnumerator InvokeWithDelay(Action action, float delay)
    {
      yield return new WaitForSeconds(delay);
      action?.Invoke();
    }

    protected IEnumerator InvokeWithAtTheEndOfFrame(Action action)
    {
      yield return new WaitForEndOfFrame();
      action?.Invoke();
    }

    public void OnDestroy()
    {
      Dispose();
    }

    public void Dispose()
    {
      if (!m_disposed)
      {
        BeforeDispose();
        this.UnSubscribeMyEventHandlers();
        this.DisposeAllMembers();
        AfterDispose();
        m_disposed = true;
      }
    }

    protected virtual void BeforeDispose()
    {
    }

    protected virtual void AfterDispose()
    {
    }

    #endregion
  }
}
