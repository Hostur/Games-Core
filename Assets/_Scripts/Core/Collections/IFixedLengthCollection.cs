using System.Collections.Generic;

namespace Core.Collections
{
  public interface IFixedLengthCollection<T>
  {
    int Count { get; }
    int Capacity { get; }
    void Add(T element);
    void Clear();
    List<T> GetElements(int count);
    List<T> GetElements();
  }
}
