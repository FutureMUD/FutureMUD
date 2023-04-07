using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework
{
    public struct ValueRange : IComparable, IComparable<ValueRange>, IComparable<double>
    {
        public double MinimumValue;
        public double MaximumValue;
        public int CompareTo(ValueRange other)
        {
            return MinimumValue == other.MinimumValue ? MaximumValue.CompareTo(other.MaximumValue) : MinimumValue.CompareTo(other.MinimumValue);
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is double d) {
                return ((IComparable<double>) this).CompareTo(d);
            }
            return CompareTo((ValueRange)obj);
        }

        int IComparable<double>.CompareTo(double other)
        {
            return (other < MinimumValue ? -1 : (other >= MaximumValue ? 1 : 0));
        }
    }
}
