using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace MudSharp.Framework
{
    public class Counter<T> : IEnumerable<KeyValuePair<T,int>>, IReadOnlyDictionary<T,int>
    {
        private readonly Dictionary<T, int> _internal;

        public Counter()
        {
            _internal = new Dictionary<T, int>();
        }

        public Counter(IEqualityComparer<T> comparer)
        {
            _internal = new Dictionary<T, int>(comparer);
        }

        public Counter(Counter<T> rhs)
        {
            _internal = new Dictionary<T, int>(rhs._internal);
        }

        private void CheckOrInitialise(T thing)
        {
            if (!_internal.ContainsKey(thing))
            {
                _internal[thing] = 0;
            }
        }
        
        public int this[T key] {
            get {
                CheckOrInitialise(key);
                return _internal[key];
            }
            set {
                CheckOrInitialise(key);
                _internal[key] = value;
            }
        }

        public void Clear(){
			_internal.Clear();
		}

        public int Count(T thing)
        {
            CheckOrInitialise(thing);
            return _internal[thing];
        }

        public void Increment(T thing)
        {
            CheckOrInitialise(thing);
            _internal[thing] = _internal[thing] + 1;
        }

        public void Add(T thing, int count)
        {
            CheckOrInitialise(thing);
            _internal[thing] = _internal[thing] + count;
        }

        public void Remove(T thing)
        {
            _internal.Remove(thing);
        }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        public IReadOnlyDictionary<T, int> AsReadOnlyDictionary() => this;

        public Dictionary<T, int>.KeyCollection Keys => _internal.Keys;
        public Dictionary<T, int>.ValueCollection Values => _internal.Values;
        int IReadOnlyCollection<KeyValuePair<T, int>>.Count => _internal.Count;
        public bool ContainsKey(T key)
        {
            return _internal.ContainsKey(key);
        }

        bool IReadOnlyDictionary<T, int>.TryGetValue(T key, out int value)
        {
            return _internal.TryGetValue(key, out value);
        }

        public int ValueOrDefault(T key)
        {
            return this[key];
        }

        IEnumerable<T> IReadOnlyDictionary<T, int>.Keys => Keys;

        IEnumerable<int> IReadOnlyDictionary<T, int>.Values => Values;

        public static Counter<T> operator +(Counter<T> counter1, Counter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] += item.Value;
            }
            return counter1;
        }

        public static Counter<T> operator -(Counter<T> counter1, Counter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] -= item.Value;
            }
            return counter1;
        }

        public static Counter<T> operator *(Counter<T> counter1, Counter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] *= item.Value;
            }
            return counter1;
        }

        public static Counter<T> operator /(Counter<T> counter1, Counter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] /= item.Value;
            }
            return counter1;
        }
    }

    public class DoubleCounter<T> : IEnumerable<KeyValuePair<T,double>>, IReadOnlyDictionary<T,double>
    {
        private readonly Dictionary<T, double> _internal;

        public DoubleCounter()
        {
            _internal = new Dictionary<T, double>();
        }

        public DoubleCounter(IEqualityComparer<T> comparer)
        {
            _internal = new Dictionary<T, double>(comparer);
        }

        public DoubleCounter(DoubleCounter<T> rhs)
        {
            _internal = new Dictionary<T, double>(rhs._internal);
        }

        private void CheckOrInitialise(T thing)
        {
            if (!_internal.ContainsKey(thing))
            {
                _internal[thing] = 0;
            }
        }

        public double this[T key] {
            get {
                CheckOrInitialise(key);
                return _internal[key];
            }
            set {
                CheckOrInitialise(key);
                _internal[key] = value;
            }
        }
		
		public void Clear(){
			_internal.Clear();
		}

        public double Count(T thing)
        {
            CheckOrInitialise(thing);
            return _internal[thing];
        }

        public void Add(T thing, double count)
        {
            CheckOrInitialise(thing);
            _internal[thing] = _internal[thing] + count;
        }

        public void Remove(T thing)
        {
            _internal.Remove(thing);
        }

        public IEnumerator<KeyValuePair<T, double>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        public Dictionary<T,double>.KeyCollection Keys => _internal.Keys;
        public Dictionary<T, double>.ValueCollection Values => _internal.Values;

        public IReadOnlyDictionary<T, double> AsReadOnlyDictionary() => this;
        
        int IReadOnlyCollection<KeyValuePair<T, double>>.Count => _internal.Count;
        public bool ContainsKey(T key)
        {
            return _internal.ContainsKey(key);
        }

        bool IReadOnlyDictionary<T, double>.TryGetValue(T key, out double value)
        {
            return _internal.TryGetValue(key, out value);
        }

        public double ValueOrDefault(T key)
        {
            return this[key];
        }

        IEnumerable<T> IReadOnlyDictionary<T, double>.Keys => Keys;

        IEnumerable<double> IReadOnlyDictionary<T, double>.Values => Values;

        public static DoubleCounter<T> operator +(DoubleCounter<T> counter1, DoubleCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] += item.Value;
            }
            return counter1;
        }

        public static DoubleCounter<T> operator -(DoubleCounter<T> counter1, DoubleCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] -= item.Value;
            }
            return counter1;
        }

        public static DoubleCounter<T> operator *(DoubleCounter<T> counter1, DoubleCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] *= item.Value;
            }
            return counter1;
        }

        public static DoubleCounter<T> operator /(DoubleCounter<T> counter1, DoubleCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] /= item.Value;
            }
            return counter1;
        }
    }

    public class DecimalCounter<T> : IEnumerable<KeyValuePair<T, decimal>>, IReadOnlyDictionary<T, decimal>
    {
        private readonly Dictionary<T, decimal> _internal;

        public DecimalCounter()
        {
            _internal = new Dictionary<T, decimal>();
        }

        public DecimalCounter(IEqualityComparer<T> comparer)
        {
            _internal = new Dictionary<T, decimal>(comparer);
        }

        public DecimalCounter(DecimalCounter<T> rhs)
        {
            _internal = new Dictionary<T, decimal>(rhs._internal);
        }

        private void CheckOrInitialise(T thing)
        {
            if (!_internal.ContainsKey(thing))
            {
                _internal[thing] = 0;
            }
        }

        public decimal this[T key] {
            get {
                CheckOrInitialise(key);
                return _internal[key];
            }
            set {
                CheckOrInitialise(key);
                _internal[key] = value;
            }
        }
		
		public void Clear(){
			_internal.Clear();
		}

        public decimal Count(T thing)
        {
            CheckOrInitialise(thing);
            return _internal[thing];
        }

        public void Add(T thing, decimal count)
        {
            CheckOrInitialise(thing);
            _internal[thing] = _internal[thing] + count;
        }

        public void Remove(T thing)
        {
            _internal.Remove(thing);
        }

        public IEnumerator<KeyValuePair<T, decimal>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        public Dictionary<T, decimal>.KeyCollection Keys => _internal.Keys;
        public Dictionary<T, decimal>.ValueCollection Values => _internal.Values;

        public IReadOnlyDictionary<T, decimal> AsReadOnlyDictionary() => this;

        int IReadOnlyCollection<KeyValuePair<T, decimal>>.Count => _internal.Count;
        public bool ContainsKey(T key)
        {
            return _internal.ContainsKey(key);
        }

        bool IReadOnlyDictionary<T, decimal>.TryGetValue(T key, out decimal value)
        {
            return _internal.TryGetValue(key, out value);
        }

        public decimal ValueOrDefault(T key)
        {
            return this[key];
        }

        IEnumerable<T> IReadOnlyDictionary<T, decimal>.Keys => Keys;

        IEnumerable<decimal> IReadOnlyDictionary<T, decimal>.Values => Values;

        public void Add(DecimalCounter<T> other)
        {
            foreach (var item in other)
            {
                CheckOrInitialise(item.Key);
                _internal[item.Key] += item.Value;
            }
        }

        public void Subtract(DecimalCounter<T> other)
        {
            foreach (var item in other)
            {
                CheckOrInitialise(item.Key);
                _internal[item.Key] -= item.Value;
            }
        }

        public static DecimalCounter<T> operator +(DecimalCounter<T> counter1, DecimalCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] += item.Value;
            }
            return counter1;
        }

        public static DecimalCounter<T> operator -(DecimalCounter<T> counter1, DecimalCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] -= item.Value;
            }
            return counter1;
        }

        public static DecimalCounter<T> operator *(DecimalCounter<T> counter1, DecimalCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] *= item.Value;
            }
            return counter1;
        }

        public static DecimalCounter<T> operator /(DecimalCounter<T> counter1, DecimalCounter<T> counter2)
        {
            foreach (var item in counter2)
            {
                counter1[item.Key] /= item.Value;
            }
            return counter1;
        }
    }
}
