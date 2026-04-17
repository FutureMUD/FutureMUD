using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework
{
    public class CircularRange<T>
    {
        protected readonly List<BoundRange<T>> _ranges = new();
        public IEnumerable<BoundRange<T>> Ranges => _ranges;

        public CircularRange()
        {
        }

        public CircularRange(double ceiling, IEnumerable<(T Item, double lowerBound)> items)
        {
            List<(T Item, double lowerBound)> sorted = items.OrderBy(x => x.lowerBound).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                if (i + 1 == sorted.Count)
                {
                    _ranges.Add(new BoundRange<T>(this, sorted[i].Item, sorted[i].lowerBound, ceiling));
                    if (sorted[0].lowerBound != 0.0)
                    {
                        _ranges.Add(new BoundRange<T>(this, sorted[i].Item, 0.0, sorted[0].lowerBound));
                    }
                }
                else
                {
                    _ranges.Add(new BoundRange<T>(this, sorted[i].Item, sorted[i].lowerBound, sorted[i + 1].lowerBound));
                }
            }
            Floor = 0;
            Ceiling = ceiling;
            Circumference = ceiling;
        }

        public double Circumference { get; protected set; }
        public double Floor { get; protected set; }
        public double Ceiling { get; protected set; }

        private double Normalise(double value)
        {
            while ((value < Floor) || (value >= Ceiling))
            {
                value += (value < Floor ? 1 : -1) * Circumference;
            }
            return value;
        }

        public T Get(double value)
        {
            if (double.IsNaN(value))
            {
                value = 0.0;
            }
            double normalVal = Normalise(value);
            BoundRange<T> range = _ranges.FirstOrDefault(x => (normalVal >= x.LowerLimit) && (normalVal < x.UpperLimit));
            return range != null ? range.Value : _ranges.First().Value;
        }

        public double RangeFraction(double value)
        {
            double normalVal = Normalise(value);
            try
            {
                BoundRange<T> range = _ranges.First(x => (normalVal >= x.LowerLimit) && (normalVal < x.UpperLimit));
                return (normalVal - range.LowerLimit) / (range.UpperLimit - range.LowerLimit);
            }
            catch (InvalidOperationException)
            {
                throw new ApplicationException($"Failed to find circular range value for {value} (normalised: {normalVal}). Ranges were: \n{Ranges.Select(x => $"{x.LowerLimit} - {x.UpperLimit}: {x.Value}").ListToString(conjunction: "", separator: " ")}");
            }
        }

		public void Add(BoundRange<T> value)
		{
			_ranges.Add(value);
		}

		public bool Remove(BoundRange<T> value)
		{
			var result = _ranges.Remove(value);
			if (result)
			{
				ResetBounds();
			}

			return result;
		}

		public void RemoveAt(int index)
		{
			_ranges.RemoveAt(index);
			ResetBounds();
		}

		private void ResetBounds()
		{
			if (!_ranges.Any())
			{
				Floor = 0.0;
				Ceiling = 1.0;
				Circumference = 1.0;
				return;
			}

			Floor = _ranges.Min(x => x.LowerLimit);
			Ceiling = _ranges.Max(x => x.UpperLimit);
			Circumference = Ceiling - Floor;
		}

		public void Sort()
		{
			_ranges.Sort((x, y) => x.LowerLimit.CompareTo(y.LowerLimit));
			ResetBounds();
		}
	}

    public class BoundRange<T>
    {
        public BoundRange(CircularRange<T> parent, T value, double lower, double upper)
        {
            Value = value;
            LowerLimit = lower;
            UpperLimit = upper;
            Parent = parent;
        }

        public CircularRange<T> Parent { get; protected set; }
        public T Value { get; protected set; }
        public double LowerLimit { get; protected set; }
        public double UpperLimit { get; protected set; }

        public double Length
        {
            get
            {
                if (UpperLimit > LowerLimit)
                {
                    return UpperLimit - LowerLimit;
                }

                if (UpperLimit == LowerLimit)
                {
                    return 0.0;
                }

                return (UpperLimit - Parent.Floor) + (Parent.Ceiling - LowerLimit);
            }
        }
    }
}
