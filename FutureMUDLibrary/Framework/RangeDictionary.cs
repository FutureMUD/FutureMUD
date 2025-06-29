using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework
{
    public class RangeDictionary<T> : IDictionary<ValueRange, T>, IReadOnlyDictionary<double, T>
    {
        private ValueRange[] _ranges = new ValueRange[0];
        private T[] _values = new T[0];

        #region IDictionary<ValueRange,T> Implementation
        public bool ContainsKey(ValueRange key)
        {
            return _ranges.Contains(key);
        }

        public void Add(ValueRange key, T value)
        {
            if (_ranges.Contains(key))
            {
                throw new ArgumentException("There is already a key with that value in this RangeDictionary.", nameof(key));
            }
            Array.Resize(ref _ranges, _ranges.Length + 1);
            Array.Resize(ref _values, _values.Length + 1);
            _ranges[_ranges.Length] = key;
            _values[_values.Length] = value;
            OnCollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Remove(ValueRange key)
        {
            var index = -1;
            for (var i = 0; i < _ranges.Length; i++)
            {
                if (_ranges[i].Equals(key))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return false;
            }

            _ranges = _ranges.Except(key).ToArray();
            _values = _values.Where(x => index-- != 0).ToArray();
            OnCollectionChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public bool TryGetValue(ValueRange key, out T value)
        {
            for (var i = 0; i < _ranges.Length; i++)
            {
                if (_ranges[i].Equals(key))
                {
                    value = _values[i];
                    return true;
                }
            }

            value = default;
            return false;
        }

        public T this[ValueRange key] {
            get {
                for (var i = 0; i < _ranges.Length; i++)
                {
                    if (_ranges[i].Equals(key))
                    {
                        return _values[i];
                    }
                }

                throw new KeyNotFoundException("No such ValueRange");
            }
            set {
                for (var i = 0; i < _ranges.Length; i++)
                {
                    if (_ranges[i].Equals(key))
                    {
                        _values[i] = value;
                        OnCollectionChanged?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }

                Add(key, value);
            }
        }

        public ICollection<ValueRange> Keys => _ranges;

        public ICollection<T> Values => _values;

        public void Add(KeyValuePair<ValueRange, T> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _ranges = new ValueRange[0];
            _values = new T[0];
            OnCollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(KeyValuePair<ValueRange, T> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<ValueRange, T>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < _ranges.Length)
            {
                throw new ArgumentException("The destination array is too small.", nameof(array));
            }

            for (var i = 0; i < _ranges.Length; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<ValueRange, T>(_ranges[i], _values[i]);
            }
        }

        public bool Remove(KeyValuePair<ValueRange, T> item)
        {
            var index = -1;
            for (var i = 0; i < _ranges.Length; i++)
            {
                if (_ranges[i].Equals(item.Key) && _values[i].Equals(item.Value))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return false;
            }

            _ranges = _ranges.Except(item.Key).ToArray();
            _values = _values.Where(x => index-- != 0).ToArray();
            OnCollectionChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public int Count => _ranges.Length;

        public bool IsReadOnly => false;

        private event EventHandler OnCollectionChanged;

        private class RangeDictionaryEnumerator : IEnumerator<KeyValuePair<ValueRange, T>>
        {
            public RangeDictionaryEnumerator(RangeDictionary<T> dictionary)
            {
                CurrentIndex = -1;
                Dictionary = dictionary;
                Dictionary.OnCollectionChanged += Dictionary_OnCollectionChanged;
            }

            private bool _collectionChanged;

            private void Dictionary_OnCollectionChanged(object sender, EventArgs e)
            {
                _collectionChanged = true;
            }

            public RangeDictionary<T> Dictionary;
            public int CurrentIndex;
            private KeyValuePair<ValueRange, T> _current;
            public KeyValuePair<ValueRange, T> Current => _current;

            public bool MoveNext()
            {
                if (_collectionChanged)
                {
                    throw new InvalidOperationException("The underlying collection has changed; enumeration cannot continue.");
                }

                if (++CurrentIndex < Dictionary._ranges.Length)
                {
                    _current = new KeyValuePair<ValueRange, T>(Dictionary._ranges[CurrentIndex], Dictionary._values[CurrentIndex]);
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                CurrentIndex = -1;
                _current = default;
            }

            object IEnumerator.Current => Current;

            private bool disposedValue = false;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    _current = default;
                    Dictionary.OnCollectionChanged -= Dictionary_OnCollectionChanged;
                }

                disposedValue = true;
            }

            ~RangeDictionaryEnumerator()
            {
                Dispose(false);
            }
        }

        IEnumerator<KeyValuePair<ValueRange, T>> IEnumerable<KeyValuePair<ValueRange, T>>.GetEnumerator() => new RangeDictionaryEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new RangeDictionaryEnumerator(this);
        #endregion

        #region IReadOnlyDictionary<double,T> Implementation
        public bool ContainsKey(double key)
        {
            return _ranges.Any(x => x.MaximumValue > key && x.MinimumValue <= key);
        }

        public bool TryGetValue(double key, out T value)
        {
            var index = -1;
            for (var i = 0; i < _ranges.Length; i++)
            {
                if (_ranges[i].MaximumValue > key && _ranges[i].MinimumValue <= key)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                value = default;
                return false;
            }

            value = _values[index];
            return true;
        }

        public T this[double key] {
            get {
                var index = -1;
                for (var i = 0; i < _ranges.Length; i++)
                {
                    if (_ranges[i].MaximumValue > key && _ranges[i].MinimumValue <= key)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    throw new KeyNotFoundException("No such range.");
                }

                return _values[index];
            }
        }

        IEnumerable<double> IReadOnlyDictionary<double, T>.Keys => _ranges.Select(x => x.MinimumValue);

        IEnumerable<T> IReadOnlyDictionary<double, T>.Values => _values;

        int IReadOnlyCollection<KeyValuePair<double, T>>.Count => _ranges.Length;

        private class RangeDictionaryDoubleEnumerator : IEnumerator<KeyValuePair<double,T>>
        {
            public RangeDictionaryDoubleEnumerator(RangeDictionary<T> dictionary)
            {
                _currentIndex = -1;
                Dictionary = dictionary;
                Dictionary.OnCollectionChanged += Dictionary_OnCollectionChanged;
            }

            private bool _collectionChanged;

            private void Dictionary_OnCollectionChanged(object sender, EventArgs e)
            {
                _collectionChanged = true;
            }

            public RangeDictionary<T> Dictionary;
            private int _currentIndex;
            private KeyValuePair<double, T> _current;
            public KeyValuePair<double, T> Current => _current;

            public bool MoveNext()
            {
                if (_collectionChanged)
                {
                    throw new InvalidOperationException("The underlying collection has changed; enumeration cannot continue.");
                }

                if (++_currentIndex < Dictionary._ranges.Length)
                {
                    _current = new KeyValuePair<double, T>(Dictionary._ranges[_currentIndex].MinimumValue, Dictionary._values[_currentIndex]);
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _currentIndex = -1;
                _current = default;
            }

            object IEnumerator.Current => Current;

            private bool disposedValue = false;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    _current = default;
                    Dictionary.OnCollectionChanged -= Dictionary_OnCollectionChanged;
                }

                disposedValue = true;
            }

            ~RangeDictionaryDoubleEnumerator()
            {
                Dispose(false);
            }

        }

        IEnumerator<KeyValuePair<double, T>> IEnumerable<KeyValuePair<double, T>>.GetEnumerator()
        {
            return new RangeDictionaryDoubleEnumerator(this);
        }
        #endregion
    }
}
