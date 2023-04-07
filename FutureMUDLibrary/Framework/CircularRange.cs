using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework {
    public class CircularRange<T> {
        protected readonly List<BoundRange<T>> _ranges = new();
        public IEnumerable<BoundRange<T>> Ranges => _ranges;

        public CircularRange()
        {
        }

        public CircularRange(double ceiling, IEnumerable<(T Item, double lowerBound)> items)
        {
            var sorted = items.OrderBy(x => x.lowerBound).ToList();
            for (var i = 0; i < sorted.Count; i++) {
                if (i + 1 == sorted.Count)
                {
                    _ranges.Add(new BoundRange<T>(sorted[i].Item, sorted[i].lowerBound, ceiling));
                    _ranges.Add(new BoundRange<T>(sorted[i].Item, 0.0, sorted[0].lowerBound));
                }
                else
                {
                    _ranges.Add(new BoundRange<T>(sorted[i].Item, sorted[i].lowerBound, sorted[i + 1].lowerBound));
                }
            }
            Floor = 0;
            Ceiling = ceiling;
            Circumference = ceiling;
        }

        public double Circumference { get; protected set; }
        public double Floor { get; protected set; }
        public double Ceiling { get; protected set; }

        private double Normalise(double value) {
            while ((value < Floor) || (value >= Ceiling)) {
                value += (value < Floor ? 1 : -1)*Circumference;
            }
            return value;
        }

        public T Get(double value) {
            if (double.IsNaN(value)) {
                value = 0.0;
            }
            var normalVal = Normalise(value);
            var range = _ranges.FirstOrDefault(x => (normalVal >= x.LowerLimit) && (normalVal < x.UpperLimit));
            return range != null ? range.Value : _ranges.First().Value;
        }

        public double RangeFraction(double value) {
            var normalVal = Normalise(value);
            try {
                var range = _ranges.First(x => (normalVal >= x.LowerLimit) && (normalVal < x.UpperLimit));
                return (normalVal - range.LowerLimit)/(range.UpperLimit - range.LowerLimit);
            }
            catch (InvalidOperationException) {
                throw new ApplicationException($"Failed to find circular range value for {value} (normalised: {normalVal}). Ranges were: \n{Ranges.Select(x => $"{x.LowerLimit} - {x.UpperLimit}: {x.Value}").ListToString(conjunction: "", separator: " ")}");
            }
        }

        public void Add(BoundRange<T> value) {
            _ranges.Add(value);
        }

        public void Sort() {
            _ranges.Sort((x, y) => x.LowerLimit.CompareTo(y.LowerLimit));
            Floor = _ranges.Min(x => x.LowerLimit);
            Ceiling = _ranges.Max(x => x.UpperLimit);
            Circumference = Ceiling - Floor;
        }
    }

    public class BoundRange<T> {
        public BoundRange(T value, double lower, double upper) {
            Value = value;
            LowerLimit = lower;
            UpperLimit = upper;
        }

        public T Value { get; protected set; }
        public double LowerLimit { get; protected set; }
        public double UpperLimit { get; protected set; }
    }
}