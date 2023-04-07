using System;
using System.Collections;
using System.Collections.Generic;

namespace MudSharp.Framework
{
    public interface IReadOnlyCollectionDictionary<T, U> : IEnumerable<KeyValuePair<T, IEnumerable<U>>>
    {
        IEnumerable<U> this[T key] { get; }
        bool ContainsKey(T key);
        bool ContainsValue(U value);
        IEnumerable<T> Keys { get; }
        IEnumerable<IEnumerable<U>> Values { get; }
        IReadOnlyDictionary<T, IEnumerable<U>> ToDictionary();
        Type KeyType { get; }
        Type ValueType { get; }
    }
}