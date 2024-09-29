using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MudSharp.Framework
{
	public class ReadOnlyCollectionDictionary<T, U> : IReadOnlyCollectionDictionary<T, U>
	{
		private CollectionDictionary<T, U> _cd;
		public ReadOnlyCollectionDictionary (CollectionDictionary<T, U> cd)
		{
			_cd = cd;
		}

		#region Implementation of IEnumerable
		private class ReadonlyEnumerator : IEnumerator<KeyValuePair<T, IEnumerable<U>>>
		{
			private T _currentT;
			private IEnumerable<U> _currentU;
			private readonly IEnumerator<KeyValuePair<T, List<U>>> _internalEnumerator;

			public ReadonlyEnumerator(Dictionary<T,List<U>> collection)
			{
				_internalEnumerator = collection.GetEnumerator();
			}

			#region Implementation of IEnumerator

			public bool MoveNext()
			{
				var node = _internalEnumerator.MoveNext();
				if (!node)
				{
					return false;
				}
				(_currentT, _currentU) = _internalEnumerator.Current;
				return true;
			}

			public void Reset()
			{
				(_currentT, _currentU) = (default, default);
				_internalEnumerator.Reset();
			}

			public KeyValuePair<T, IEnumerable<U>> Current => new(_currentT, _currentU);
			object IEnumerator.Current => new KeyValuePair<T, IEnumerable<U>>(_currentT, _currentU);

			#endregion

			#region Implementation of IDisposable

			public void Dispose()
			{
				_internalEnumerator.Dispose();
			}

			#endregion
		}
		
		public IEnumerator<KeyValuePair<T, IEnumerable<U>>> GetEnumerator()
		{
			return new ReadonlyEnumerator(_cd.ToDictionary());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IReadOnlyCollectionDictionary<T,U>

		public IEnumerable<U> this[T key] => _cd[key];

		public bool ContainsKey(T key)
		{
			return _cd.ContainsKey(key);
		}

		public bool ContainsValue(U value)
		{
			return _cd.ContainsValue(value);
		}

		public IEnumerable<T> Keys => _cd.Keys;
		public IEnumerable<IEnumerable<U>> Values => _cd.Values;
		public IReadOnlyDictionary<T, IEnumerable<U>> ToDictionary()
		{
			var dictionary = new Dictionary<T, IEnumerable<U>>();
			foreach (var (key, value) in _cd.ToDictionary())
			{
				dictionary[key] = value;
			}
			return dictionary;
		}

		public Type KeyType => _cd.KeyType;
		public Type ValueType => _cd.ValueType;

		#endregion
	}

	public interface ICollectionDictionary<T,U> : ICollectionDictionaryWithKey<T>, ICollectionDictionaryWithType<U>, IEnumerable<KeyValuePair<T, List<U>>>, ICollectionDictionary
	{
	}

	public interface ICollectionDictionaryWithKey<T>
	{
		IEnumerable<T> Keys { get; }
		IEnumerable<List<object>> Values { get; }
		IEnumerable<KeyValuePair<T, List<object>>> KeysAndValues { get; }
	}

	public interface ICollectionDictionaryWithType<U>
	{
		IEnumerable<object> Keys { get; }
		IEnumerable<List<U>> Values { get; }
		IEnumerable<KeyValuePair<object, List<U>>> KeysAndValues { get; }
	}

	public interface ICollectionDictionary
	{

		Type KeyType { get; }

		Type ValueType { get; }
		IEnumerable<object> Keys { get; }
		IEnumerable<List<object>> Values { get; }
		IEnumerable<KeyValuePair<object, List<object>>> KeysAndValues { get; }
	}

	public class CollectionDictionary<T, U> : ICollectionDictionary<T,U>
	{
		public CollectionDictionary()
		{
			 _internal = new Dictionary<T, List<U>>();
		}

		public CollectionDictionary(IEqualityComparer<T> comparer)
		{
			_internal  = new Dictionary<T, List<U>>(comparer);
		}

		public CollectionDictionary(IEnumerable<KeyValuePair<T, List<U>>> values)
		{
			_internal = new Dictionary<T, List<U>>();
			foreach (var value in values)
			{
				CheckOrCreateKey(value.Key);
				_internal[value.Key].AddRange(value.Value);
			}
		}

		public IReadOnlyCollectionDictionary<T, U> AsReadOnlyCollectionDictionary()
		{
			return new ReadOnlyCollectionDictionary<T, U>(this);
		}

		private readonly Dictionary<T, List<U>> _internal;

		private void CheckOrCreateKey(T key)
		{
			if (!_internal.ContainsKey(key))
			{
				_internal[key] = new List<U>();
			}
		}
		
		public void AddRange(IEnumerable<(T Key, U Value)> range){
			foreach (var item in range){
				CheckOrCreateKey(item.Key);
				_internal[item.Key].Add(item.Value);
			}
		}

		public void AddRange(T key, IEnumerable<U> range)
		{
			CheckOrCreateKey(key);
			foreach (var item in range)
			{
				_internal[key].Add(item);
			}
		}

		public void Add(T key, U value)
		{
			CheckOrCreateKey(key);
			_internal[key].Add(value);
		}

		public void Remove(T key, U value)
		{
			CheckOrCreateKey(key);
			_internal[key].Remove(value);
		}

		public void RemoveAt(T key, int index)
		{
			CheckOrCreateKey(key);
			_internal[key].RemoveAt(index);
		}

		public void RemoveRange(T key, int index, int count)
		{
			CheckOrCreateKey(key);
			_internal[key].RemoveRange(index, count);
		}

		public void RemoveRange(T key, IEnumerable<U> range)
		{
			CheckOrCreateKey(key);
			_internal[key].RemoveAll(range.Contains);
		}

		public void RemoveAll(T key, Predicate<U> predicate)
		{
			CheckOrCreateKey(key);
			_internal[key].RemoveAll(predicate);
		}

		public void RemoveAll(Predicate<U> predicate)
		{
			foreach (var item in _internal.ToList())
			{
				item.Value.RemoveAll(predicate);
			}
		}

		public void Remove(T key)
		{
			_internal.Remove(key);
		}

		public List<U> this[T key] {
			get {
				CheckOrCreateKey(key);
				return _internal[key];
			}
			set {
				CheckOrCreateKey(key);
				_internal[key] = value.ToList();
			}
		}
		
		public bool ContainsKey(T key)
		{
			return _internal.ContainsKey(key);
		}

		public bool ContainsValue(U value)
		{
			return _internal.Any(x => Equals(x.Value, value));
		}

		public void Clear()
		{
			_internal.Clear();
		}

		public bool SetValueAtIndex(T key, int index, U newValue)
		{
			CheckOrCreateKey(key);
			if (_internal[key].Count >= index || index < 0)
			{
				return false;
			}

			_internal[key][index] = newValue;
			return true;
		}

		public bool Swap(T key, int index1, int index2)
		{
			CheckOrCreateKey(key);
			if (index1 < 0 || index2 < 0 || index1 >= _internal[key].Count || index2 >= _internal[key].Count)
			{
				return false;
			}

			_internal[key].Swap(index1, index2);
			return true;
		}

		public IEnumerable<T> Keys => _internal.Keys;

		public IEnumerable<List<U>> Values => _internal.Values;


		IEnumerable<object> ICollectionDictionary.Keys => _internal.Keys.OfType<object>();
		IEnumerable<List<object>> ICollectionDictionary.Values => _internal.Values.Select(x => x.OfType<object>().ToList());
		IEnumerable<object> ICollectionDictionaryWithType<U>.Keys => _internal.Keys.OfType<object>();
		IEnumerable<List<object>> ICollectionDictionaryWithKey<T>.Values => _internal.Values.Select(x => x.OfType<object>().ToList());
		IEnumerable<KeyValuePair<object, List<object>>> ICollectionDictionary.KeysAndValues => _internal.Select(x => new KeyValuePair<object, List<object>>(x.Key, x.Value.OfType<object>().ToList()));
		IEnumerable<KeyValuePair<object, List<U>>> ICollectionDictionaryWithType<U>.KeysAndValues => _internal.Select(x => new KeyValuePair<object, List<U>>(x.Key, x.Value.ToList()));
		IEnumerable<KeyValuePair<T, List<object>>> ICollectionDictionaryWithKey<T>.KeysAndValues => _internal.Select(x => new KeyValuePair<T, List<object>>(x.Key, x.Value.OfType<object>().ToList()));

		public IEnumerator<KeyValuePair<T, List<U>>> GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		public Dictionary<T, List<U>> ToDictionary()
		{
			return _internal;
		}
		
		public Type KeyType => typeof(T);

		public Type ValueType => typeof(U);
	}
	
	public static class CollectionDictionaryExtensions{
		public static CollectionDictionary<TKey,TValue> ToCollectionDictionary<TKey,TValue>(this IEnumerable<(TKey Key, TValue Value)> enumerable, Func<(TKey, TValue), TKey> keySelector, Func<(TKey, TValue), TValue> valueSelector){
			var dict = new CollectionDictionary<TKey,TValue>();
			foreach (var item in enumerable){
				dict.Add(keySelector(item), valueSelector(item));
			}
			return dict;
		}
		
		public static CollectionDictionary<TKey,TValue> ToCollectionDictionary<TKey,TValue>(this IEnumerable<(TKey Key, TValue Value)> enumerable){
			var dict = new CollectionDictionary<TKey,TValue>();
			foreach (var item in enumerable){
				dict.Add(item.Key, item.Value);
			}
			return dict;
		}
		
		public static CollectionDictionary<TKey,TValue> ToCollectionDictionary<TKey,TValue>(this IEnumerable<Tuple<TKey,TValue>> enumerable){
			var dict = new CollectionDictionary<TKey,TValue>();
			foreach (var item in enumerable){
				dict.Add(item.Item1, item.Item2);
			}
			return dict;
		}
		
		public static CollectionDictionary<TKey,TValue> ToCollectionDictionary<TKey,TValue>(this IEnumerable<KeyValuePair<TKey,TValue>> enumerable){
			var dict = new CollectionDictionary<TKey,TValue>();
			foreach (var item in enumerable){
				dict.Add(item.Key, item.Value);
			}
			return dict;
		}

		public static CollectionDictionary<TKeyR,TValueR> ToCollectionDictionary<TKey,TValue,TKeyR,TValueR>(this CollectionDictionary<TKey,TValue> dictionary, Func<TKey,TKeyR> keySelector, Func<TValue,TValueR> valueSelector)
		{
			var dict = new CollectionDictionary<TKeyR, TValueR>();
			foreach (var item in dictionary)
			{
				foreach (var sub in item.Value)
				{
					dict.Add(keySelector(item.Key), valueSelector(sub));
				}
			}
			return dict;
		}
	}
}
