using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework {
    public class RankedRange<T> {
        protected List<Range<T>> _range = new();

        public RankedRange(bool openLowerBound = true, bool openUpperBound = true) {
            OpenLowerBound = openLowerBound;
            OpenUpperBound = openUpperBound;
        }

        public IEnumerable<Range<T>> Ranges => _range;

        public int Count => _range.Count;

        /// <summary>
        ///     Where does the next range above the current ranges need to begin?
        /// </summary>
        public double NextUpperBound
        {
            get
            {
                var last = _range.LastOrDefault();
                if (last != null) {
                    return last.UpperBound;
                }
                return 0;
            }
        }

        /// <summary>
        ///     Where does the next range below the current ranges need to begin?
        /// </summary>
        public double NextLowerBound
        {
            get
            {
                var first = _range.FirstOrDefault();
                return first?.LowerBound ?? 0;
            }
        }

        /// <summary>
        ///     Will we default anything below our current bottom range as that bottom range?
        /// </summary>
        public bool OpenLowerBound { get; protected set; }

        /// <summary>
        ///     Will we default anything above our current top range as that top range?
        /// </summary>
        public bool OpenUpperBound { get; protected set; }

        public T Find(double value) {
            return FindRange(value).Value;
        }

        public Range<T> FindRange(double value) {
            // If we have an OpenLowerBound, then return the first item with where our value is less than or equal to the item's UpperBound;
            // If we have an OpenUpperBound, then return the first item with where our value is greater than or equal to the item's LowerBound;
            // If we have neither, then return the first item our value is within the UpperBound and LowerBound of.
            return _range.FirstOrDefault(x => (value <= x.UpperBound) && (value >= x.LowerBound))
                   ??
                   (OpenUpperBound && (value > _range.Max(x => x.UpperBound))
                       ? _range.FirstMax(x => x.UpperBound)
                       : (OpenLowerBound && (value < _range.Min(x => x.LowerBound))
                           ? _range.FirstMin(x => x.LowerBound)
                           : null));
        }

        public void Add(T item, double lowerBound, double upperBound) {
            if (lowerBound > upperBound) {
                throw new ApplicationException(
                    "Abort on RankedRange.Add : attempted to add a range where the lowerBound was greater than the upperBound.");
            }

            if (_range.Count != 0) {
                if ((lowerBound != NextUpperBound) && (upperBound != NextLowerBound)) {
                    throw new ApplicationException(
                        "Abort on RankedRange.Add : attempted to add a non-consecutive range.");
                }
            }

            _range.Add(new Range<T>(item, lowerBound, upperBound));
            Sort();
        }

        /// <summary>
        ///     Sort all of our ranges by their lower bound.
        /// </summary>
        protected void Sort() {
            _range.Sort((r1, r2) => r1.LowerBound.CompareTo(r2.LowerBound));
        }

		/// <summary>
		/// Safely changes the upper bounds of an element in the ranked range.
		/// </summary>
		/// <param name="value">The value whose bounds are to change</param>
		/// <param name="newUpperBound">The new upper bound of the value</param>
		/// <returns>True if the change was valid and was made, false if the change couldn't be made</returns>
		public bool ChangeUpperBounds(T value, double newUpperBound)
        {
	        var range = _range.FirstOrDefault(x => x.Value.Equals(value));
	        if (range is null)
	        {
		        return false;
	        }

	        if (newUpperBound <= range.LowerBound)
	        {
		        return false;
	        }

	        if (_range.Last() == range)
	        {
		        if (!OpenUpperBound)
		        {
			        _range[^1] = new Range<T>(range.Value, range.LowerBound, newUpperBound);
		        }

		        return true;
	        }

	        var index = _range.IndexOf(range);
	        var next = _range[index + 1];
	        if (newUpperBound >= next.UpperBound)
	        {
		        return false;
	        }
	        _range[index] = new Range<T>(range.Value, range.LowerBound, newUpperBound);
	        _range[index + 1] = new Range<T>(next.Value, newUpperBound, next.UpperBound);
	        return true;
        }

		/// <summary>
		/// Safely changes the lower bounds of an element in the ranked range.
		/// </summary>
		/// <param name="value">The value whose bounds are to change</param>
		/// <param name="newLowerBound">The new lower bound of the value</param>
		/// <returns>True if the change was valid and was made, false if the change couldn't be made</returns>
		public bool ChangeLowerBounds(T value, double newLowerBound)
        {
	        var range = _range.FirstOrDefault(x => x.Value.Equals(value));
	        if (range is null)
	        {
		        return false;
	        }

	        if (newLowerBound >= range.UpperBound)
	        {
		        return false;
	        }

	        if (_range.First() == range)
	        {
		        if (!OpenLowerBound)
		        {
			        _range[^1] = new Range<T>(range.Value, newLowerBound, range.UpperBound);
		        }

		        return true;
	        }

	        var index = _range.IndexOf(range);
	        var last = _range[index - 1];
	        if (newLowerBound <= last.LowerBound)
	        {
		        return false;
	        }
	        _range[index] = new Range<T>(range.Value, newLowerBound, range.UpperBound);
	        _range[index - 1] = new Range<T>(last.Value, last.LowerBound, newLowerBound);
	        return true;
		}
    }

    public class Range<T> {
        public Range(T value, double lowerBound, double upperBound) {
            Value = value;
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public T Value { get; protected set; }

        public double LowerBound { get; protected set; }

        public double UpperBound { get; protected set; }
    }
}