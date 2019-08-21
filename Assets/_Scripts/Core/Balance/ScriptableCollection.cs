using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Balance
{
  public abstract class ScriptableCollection<T> : ScriptableObject, IInitializableCollection, IEnumerable<T>
  {
    private const string BALANCE_PATH = "Assets/Resources/Balance/";
    [SerializeField] public T[] Entities;

    public void Initialize(KeyValuePair<Type, IList> data)
    {
      Entities = new T[data.Value.Count];
      for (int i = 0; i < data.Value.Count; i++)
      {
        Entities[i] = (T) data.Value[i];
      }
    }

    public string OutputPath() => BALANCE_PATH + ConcretePath;

    /// <summary>
    /// Final path that be used after Assets/Balance/{ConcretePath}.
    /// It should include .asset extension at the end.
    /// </summary>
    protected abstract string ConcretePath { get; }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T entity in Entities)
      {
        yield return entity;
      }
    }
  }
}