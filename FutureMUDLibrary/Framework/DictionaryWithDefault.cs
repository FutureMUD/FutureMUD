using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework;

public static class DictionaryWithDefaultExtensions
{
	public static DictionaryWithDefault<TKey,TValue> ToDictionaryWithDefault<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keySelector) where TKey : notnull
	{
		return ToDictionaryWithDefault(collection, keySelector, x => x);
	}

	public static DictionaryWithDefault<TKey, TValue> ToDictionaryWithDefault<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keySelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		return ToDictionaryWithDefault(collection, keySelector, x => x, comparer);
	}

	public static DictionaryWithDefault<TKey, TValue> ToDictionaryWithDefault<TSource, TKey, TValue>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource,TValue> valueSelector) where TKey : notnull
	{
		return ToDictionaryWithDefault(collection, keySelector, valueSelector, null);
	}

	public static DictionaryWithDefault<TKey, TValue> ToDictionaryWithDefault<TSource, TKey, TValue>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		if (collection == null) throw new ArgumentNullException(nameof(collection));

		if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

		var dictionary = new DictionaryWithDefault<TKey, TValue>(comparer);
		foreach ( var item in collection)
		{
			var iKey = keySelector(item);
			var iValue = valueSelector(item);
			dictionary[iKey] = iValue;
		}

		return dictionary;
	}
}

/// <summary>
/// The DictionaryWithDefault class is a simple wrapper around the Dictionary class to allow you to access the indexer without throwing exceptions (return default value instead)
/// </summary>
/// <typeparam name="TKey">The dictionary's key type</typeparam>
/// <typeparam name="TValue">The dictionary's value type</typeparam>
public class DictionaryWithDefault<TKey,TValue> : IDictionary<TKey,TValue>, IReadOnlyDictionary<TKey, TValue>
{
	private IDictionary<TKey, TValue> _dictionary;
	
	public DictionaryWithDefault()
	{
		_dictionary = new Dictionary<TKey, TValue>();
	}

	public DictionaryWithDefault(IEqualityComparer<TKey> comparer)
	{
		_dictionary = comparer is not null ? 
			new Dictionary<TKey,TValue>(comparer) :
			new Dictionary<TKey, TValue>();
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
	}

	public bool ContainsKey(TKey key)
	{
		return _dictionary.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return _dictionary.Remove(key);
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		return _dictionary.TryGetValue(key, out value);
	}

	public TValue this[TKey key] { 
		get { 
			return _dictionary.TryGetValue(key, out TValue value) ? value : default;
		}
		set {
			_dictionary[key] = value;
		}
	}

	public ICollection<TKey> Keys => _dictionary.Keys;
	public ICollection<TValue> Values => _dictionary.Values;

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		_dictionary.Add(item);
	}

	public void Clear()
	{
		_dictionary.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return _dictionary.Contains(item);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_dictionary.CopyTo(array, arrayIndex);
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return _dictionary.Remove(item);
	}

	public int Count => _dictionary.Count;
	public bool IsReadOnly => _dictionary.IsReadOnly;

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
}
