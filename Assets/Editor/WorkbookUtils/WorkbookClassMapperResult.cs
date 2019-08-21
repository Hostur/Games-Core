using System;
using System.Collections;
using System.Collections.Generic;

namespace Editor.WorkbookUtils
{
  public class WorkbookClassMapperResult : IEnumerable<KeyValuePair<Type, IList>>
  {
    private readonly Dictionary<Type, IList> _result;

    public IList this[Type t] => _result[t];

    public WorkbookClassMapperResult(Dictionary<Type, IList> result)
    {
      _result = result;
    }

    public IEnumerator<KeyValuePair<Type, IList>> GetEnumerator()
    {
      foreach (KeyValuePair<Type, IList> keyValuePair in _result)
      {
        yield return keyValuePair;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
