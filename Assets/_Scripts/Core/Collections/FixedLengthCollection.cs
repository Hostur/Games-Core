using System.Collections.Generic;

namespace Core.Collections
{
  public abstract class FixedLengthCollection<T> : IFixedLengthCollection<T>
  {
    protected readonly List<T> Elements;

    protected FixedLengthCollection(int capacity)
    {
      Elements = new List<T>(capacity);
      Capacity = capacity;
    }

    public int Count => Elements.Count;
    public int Capacity { get; }

    public void Add(T element)
    {
      if (Elements.Count >= Capacity)
      {
        Elements.RemoveAt(0);
      }
      Elements.Add(element);

    }

    public void Clear()
    {
      Elements.Clear();
    }

    public List<T> GetElements(int count)
    {
      List<T> result = new List<T>(count);
      int counter = 0;
      for (int i = Elements.Count - 1; i >= 0 && ++counter <= count; i--)
      {
        result.Add(Elements[i]);
      }

      return result;

    }

    public List<T> GetElements()
    {
      List<T> result = new List<T>(Elements.Count);
      for (int i = Elements.Count - 1; i >= 0; i--)
      {
        result.Add(Elements[i]);
      }

      return result;
    }
  }
}
