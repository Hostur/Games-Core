using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Core.Collections
{
  /// <summary>
  /// Class contains extensions from IEnumerable, IList, arrays and enums.
  /// </summary>
  public static class CollectionsExtension
  {
    private const byte ITERATION_STEPS_COUNT_16 = 16;
    private const byte ITERATION_STEPS_COUNT_15 = 15;
    private const byte TRUE = 1;
    private const byte FALSE = 0;

    /// <summary>
    /// Invoke action on each element of given collection.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">IEnumerable collection.</param>
    /// <param name="action">Action to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (var item in collection)
      {
        if (item != null)
          action(item);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Each<T>(this IEnumerable<T> collection, Func<T, bool> whereFunc, Action<T> action)
    {
      foreach (var item in collection)
      {
        if (item != null && whereFunc(item))
          action(item);
      }
    }

    /// <summary>
    /// Invoke action on each element of given collection.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Generic array collection.</param>
    /// <param name="action">Generic action to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ArrayEach<T>(this T[] collection, Action<T> action) where T : class
    {
      int i;
      for (i = 0; i < (collection.Length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
      {
        action(collection[i]);
        action(collection[i + 1]);
        action(collection[i + 2]);
        action(collection[i + 3]);
        action(collection[i + 4]);
        action(collection[i + 5]);
        action(collection[i + 6]);
        action(collection[i + 7]);
        action(collection[i + 8]);
        action(collection[i + 9]);
        action(collection[i + 10]);
        action(collection[i + 11]);
        action(collection[i + 12]);
        action(collection[i + 13]);
        action(collection[i + 14]);
        action(collection[i + 15]);
      }
      for (i = (collection.Length & ~ITERATION_STEPS_COUNT_15); i < collection.Length; i++)
      {
        action(collection[i]);
      }
    }

    /// <summary>
    /// Invoke action on each element of given collection.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Generic array collection.</param>
    /// <param name="whereFunc">Where statement.</param>
    /// <param name="action">Generic action to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ArrayEach<T>(ref T[] collection, Func<T, bool> whereFunc, Action<T> action) where T : class
    {
      int i;
      for (i = 0; i < (collection.Length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
      {
        if (whereFunc(collection[i])) action(collection[i]);
        if (whereFunc(collection[i + 1])) action(collection[i + 1]);
        if (whereFunc(collection[i + 2])) action(collection[i + 2]);
        if (whereFunc(collection[i + 3])) action(collection[i + 3]);
        if (whereFunc(collection[i + 4])) action(collection[i + 4]);
        if (whereFunc(collection[i + 5])) action(collection[i + 5]);
        if (whereFunc(collection[i + 6])) action(collection[i + 6]);
        if (whereFunc(collection[i + 7])) action(collection[i + 7]);
        if (whereFunc(collection[i + 8])) action(collection[i + 8]);
        if (whereFunc(collection[i + 9])) action(collection[i + 9]);
        if (whereFunc(collection[i + 10])) action(collection[i + 10]);
        if (whereFunc(collection[i + 11])) action(collection[i + 11]);
        if (whereFunc(collection[i + 12])) action(collection[i + 12]);
        if (whereFunc(collection[i + 13])) action(collection[i + 13]);
        if (whereFunc(collection[i + 14])) action(collection[i + 14]);
        if (whereFunc(collection[i + 15])) action(collection[i + 15]);
      }
      for (i = (collection.Length & ~ITERATION_STEPS_COUNT_15); i < collection.Length; i++)
      {
        if (whereFunc(collection[i])) action(collection[i]);
      }
    }

    /// <summary>
    /// Remove element from the given collection of structures.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Reference to the collection of structures.</param>
    /// <param name="index">Index of the element that you want to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAt<T>(ref T[] collection, int index)
    {
      Assert(collection, index);
      T[] result = new T[collection.Length - 1];

      Array.Copy(collection, 0, result, 0, index);
      Array.Copy(collection, index + 1, result, index, collection.Length - index - 1);
      collection = result;
    }

    /// <summary>
    /// Remove element from the given collection of structures.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Collection of the structures</param>
    /// <param name="index">Index of the element that you want to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAt<T>(ref List<T> collection, int index)
    {
      Assert(collection, index);
      var array = collection.ToArray();
      RemoveAt(ref array, index);
      collection = array.ToList();
    }

    /// <summary>
    /// Fast index of for collection of structures.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Reference to the collection of structures.</param>
    /// <param name="element">Element that you want to find.</param>
    /// <returns>Index of the given element or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this T[] collection, T element)
    {
      var comparer = EqualityComparer<T>.Default;
      int i;
      for (i = 0; i < (collection.Length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
      {
        if (comparer.Equals(collection[i], element)) return i;
        if (comparer.Equals(collection[i + 1], element)) return i + 1;
        if (comparer.Equals(collection[i + 2], element)) return i + 2;
        if (comparer.Equals(collection[i + 3], element)) return i + 3;
        if (comparer.Equals(collection[i + 4], element)) return i + 4;
        if (comparer.Equals(collection[i + 5], element)) return i + 5;
        if (comparer.Equals(collection[i + 6], element)) return i + 6;
        if (comparer.Equals(collection[i + 7], element)) return i + 7;
        if (comparer.Equals(collection[i + 8], element)) return i + 8;
        if (comparer.Equals(collection[i + 9], element)) return i + 9;
        if (comparer.Equals(collection[i + 10], element)) return i + 10;
        if (comparer.Equals(collection[i + 11], element)) return i + 11;
        if (comparer.Equals(collection[i + 12], element)) return i + 12;
        if (comparer.Equals(collection[i + 13], element)) return i + 13;
        if (comparer.Equals(collection[i + 14], element)) return i + 14;
        if (comparer.Equals(collection[i + 15], element)) return i + 15;
      }
      for (i = (collection.Length & ~ITERATION_STEPS_COUNT_15); i < collection.Length; i++)
      {
        if (comparer.Equals(collection[i], element)) return i;
      }

      return -1;
    }

    /// <summary>
    /// Get next element from the given collection.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Collection of the class instances.</param>
    /// <param name="currentElement">Current element that you want to lookup from.</param>
    /// <returns>Next element from the collection after current element. If current element was the last one function will return the first object from the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetNextElement<T>(this IList<T> collection, T currentElement)
    {
      collection.Assert(1);
      var index = collection.IndexOf(currentElement);
      if (index == -1)
      {
        throw new InvalidOperationException("Given element not found in given collection.");
      }
      ++index;
      if (index >= collection.Count)
      {
        index = 0;
      }

      return collection[index];
    }

    /// <summary>
    /// Get previous element from the given collection.
    /// </summary>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <param name="collection">Collection of the elements.</param>
    /// <param name="currentElement">Current element that you want to lookup from.</param>
    /// <returns>Previous element from the collection after current element. If current element was the first one function will return the last object from the list.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetPreviousElement<T>(this IList<T> collection, T currentElement)
    {
      collection.Assert(1);
      var index = collection.IndexOf(currentElement);
      if (index == -1)
      {
        throw new InvalidOperationException("Given element not found in given collection.");
      }
      --index;
      if (index < 0)
      {
        index = collection.Count - 1;
      }

      return collection[index];
    }

    /// <summary>
    /// Foreach implementation on enum type.
    /// </summary>
    /// <typeparam name="T">Type of enum.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ForEachEnum<T>() where T : struct
    {
      var list = EnumToList<T>();
      foreach (T value in list)
      {
        yield return value;
      }
    }

    /// <summary>
    /// Get next enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum.</typeparam>
    /// <param name="obj">Enum value.</param>
    /// <returns>Next enum value or the first one if given value was the last.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetNextEnum<T>(this T obj) where T : struct
    {
      try
      {
        var tmpList = EnumToList<T>();
        var index = tmpList.IndexOf(obj);
        if (index + 1 >= tmpList.Count)
        {
          return tmpList[0];
        }

        return tmpList[index + 1];
      }
      catch
      {
        return default(T);
      }
    }

    /// <summary>
    /// Get next enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum.</typeparam>
    /// <param name="obj">Enum value.</param>
    /// <returns>Next enum value or the same if it was the last one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetNextEnumWithoutLoop<T>(this T obj) where T : struct
    {
      try
      {
        var tmpList = EnumToList<T>();
        var index = tmpList.IndexOf(obj);
        if (index + 1 >= tmpList.Count)
        {
          return tmpList[tmpList.Count - 1];
        }
        return tmpList[index + 1];
      }
      catch
      {
        return default(T);
      }
    }

    /// <summary>
    /// Get previous value for given enum.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <param name="obj">Current enum value.</param>
    /// <returns>Previous enum value or last if function was called on the first element.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetPreviousEnum<T>(this T obj) where T : struct
    {
      try
      {
        var tmpList = EnumToList<T>();
        var index = tmpList.IndexOf(obj);
        if (index - 1 < 0)
        {
          return tmpList[tmpList.Count - 1];
        }

        return tmpList[index - 1];
      }
      catch
      {
        return default(T);
      }
    }

    /// <summary>
    /// Get previous value for given enum.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <param name="obj">Current enum value.</param>
    /// <returns>Previous enum value or the same if function was called on the first element.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetPreviousEnumWithoutLoop<T>(this T obj) where T : struct
    {
      try
      {
        var tmpList = EnumToList<T>();
        var index = tmpList.IndexOf(obj);
        return tmpList[index - 1];
      }
      catch
      {
        return default(T);
      }
    }

    /// <summary>
    /// Gets random enum value.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandomEnum<T>() where T : struct
    {
      var tmpList = EnumToList<T>();
      return tmpList[Random.Range(0, tmpList.Count)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexOfCurrentEnumValue<T>(this T obj) where T : struct
    {
      return EnumToList<T>().IndexOf(obj);
    }

    /// <summary>
    /// Gets random enum value.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <param name="maxIndexInclusive">Max enum element index that can be exceed.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandomEnum<T>(int maxIndexInclusive) where T : struct
    {
      var tmpList = EnumToList<T>();
      if (maxIndexInclusive > tmpList.Count)
      {
        throw new InvalidOperationException(
          $"Inclusive index '{maxIndexInclusive}' exceed '{typeof(T).Name}' values quantity.");
      }

      return tmpList[Random.Range(0, maxIndexInclusive + 1)];
    }

    /// <summary>
    /// Gets random enum value.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <param name="excludedElement">Element that should be excluded from randomization.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandomEnum<T>(this T excludedElement) where T : struct
    {
      var tmpList = EnumToList<T>();
      int index = tmpList.IndexOf(excludedElement);

      T result = excludedElement;
      while (tmpList.IndexOf(result) == index)
      {
        result = tmpList[Random.Range(0, tmpList.Count)];
      }
      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this List<T> list)
    {
      int listCount = list.Count;
      while (listCount > 1)
      {
        listCount--;
        int k = Random.Next(listCount + 1);
        T value = list[k];
        list[k] = list[listCount];
        list[listCount] = value;
      }
    }

    /// <summary>
    /// Cast enum type to list of enum values.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <returns>List of enum values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> EnumToList<T>() where T : struct
    {
      try
      {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
      }
      catch
      {
        throw new InvalidOperationException($"Type '{typeof(T).FullName}' is not an enum type.");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Assert<T>(this T[] collection, int index)
    {
      if (collection == null)
      {
        throw new NullReferenceException("Collection is null.");
      }
      if (collection.Length < index)
      {
        throw new IndexOutOfRangeException($"Collection length {collection.Length} should be at least {index}.");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Assert<T>(this IList<T> collection, int index)
    {
      if (collection == null)
      {
        throw new NullReferenceException("Collection is null.");
      }
      if (collection.Count < index)
      {
        throw new IndexOutOfRangeException($"Collection length {collection.Count} should be at least {index}.");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTrue(this byte value)
    {
      return value == TRUE;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFalse(this byte value)
    {
      return value == FALSE;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveIfExists(this Hashtable hashTable, string key)
    {
      if (hashTable.ContainsKey(key))
        hashTable.Remove(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddOrModify(this Hashtable hashTable, string key, object value)
    {
      if (string.IsNullOrEmpty(key) || value == null) return;
      if (hashTable.ContainsKey(key))
        hashTable[key] = value;
      else
        hashTable.Add(key, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddOrModify<K, V>(this Dictionary<K, V> dictionary, K key, V value)
    {
      if (dictionary.ContainsKey(key))
        dictionary[key] = value;
      else 
        dictionary.Add(key, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V GetIfExists<K, V>(this Dictionary<K, V> dictionary, K key) where V : class
    {
      if (dictionary.ContainsKey(key))
        return dictionary[key];
      return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetIfExists<K, V>(this Dictionary<K, V> dictionary, K key, out V value) where V : class
    {
      if (dictionary.ContainsKey(key))
      {
        value = dictionary[key];
        return true;
      }
      value = null;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddIfNotExists<K, V>(this Dictionary<K, V> dictionary, K key, V value)
    {
      if (!dictionary.ContainsKey(key))
        dictionary.Add(key, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveIfExists<K, V>(this Dictionary<K, V> dictionary, K key)
    {
      if (dictionary.ContainsKey(key))
        dictionary.Remove(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FirstCharToUpper(string value)
    {
      if (string.IsNullOrEmpty(value)) return value;
      return value[0].ToString().ToUpper() + value.Substring(1);
    }

    /// <summary>
    /// Random implementation that works for Mono.
    /// </summary>
    public static class Random
    {
      private static readonly double _pi2 = math.PI * 2;
      private static System.Random _random = new System.Random();

      public static int Range(int value1, int value2)
      {
        _random = new System.Random(Guid.NewGuid().ToByteArray()[0]);
        return _random.Next(value1, value2);
      }

      public static int Next(int maxValue)
      {
        return _random.Next(maxValue);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static float3 RandomPositionInRadius(float3 vector, float radius)
      {
        var angle = _random.NextDouble() * _pi2;
        var distance = math.sqrt(_random.NextDouble()) * radius;
        var x = vector.x + (distance * math.cos(angle));
        var z = vector.z + (distance * math.sin(angle));
        return new float3((float)x, vector.y, (float)z);
      }
    }
  }
}
