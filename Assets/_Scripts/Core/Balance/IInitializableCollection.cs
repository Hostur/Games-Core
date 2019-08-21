using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Balance
{
  public interface IInitializableCollection
  {
    void Initialize(KeyValuePair<Type, IList> data);
    string OutputPath();
  }
}
