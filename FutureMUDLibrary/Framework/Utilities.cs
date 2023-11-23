using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp.Framework {
    public static class Utilities {
        public static bool In<T>(this T thing, params T[] values){
            return values.Any(x => Equals(x, thing));
        }
        public enum ByteSplitOptions {
            PreserveDelimiter,
            DiscardDelimiter
        }

        public static string DescribeEnum(this Enum value, bool explodeCamelCase = false, ANSIColour colour = null)
        {
            var type = value.GetType();
            if (type.GetCustomAttribute<FlagsAttribute>() is not null)
            {
                value.GetSingleFlags()
                    .Where(x => value.HasFlag(x))
                    .Select(x => Enum.GetName(type, value).SplitCamelCase(explodeCamelCase).Colour(colour))
                    .ListToString();
            }

            return Enum.GetName(value.GetType(), value).SplitCamelCase(explodeCamelCase).Colour(colour);
        }

        public static bool TryParseEnum<T>(this string value, out T theEnum) where T : Enum {
            var type = typeof(T);
            var underlying = type.GetEnumUnderlyingType();
            if (underlying == typeof(long))
            {
                if (long.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(int))
            {
                if (int.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(short))
            {
                if (short.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(ulong))
            {
                if (ulong.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(uint))
            {
                if (uint.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(ushort))
            {
                if (ushort.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(byte))
            {
                if (byte.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else if (underlying == typeof(sbyte))
            {
                if (sbyte.TryParse(value, out var numvalue))
                {
                    if (Enum.IsDefined(type, numvalue))
                    {

                        theEnum = (T)(object)numvalue;
                        return true;
                    }
                }
            }
            else
            {
                throw new ArgumentException("The underlying type of the enum was an unsupported type.", nameof(theEnum));
            }

            var values = Enum.GetValues(type).OfType<T>().ToList();
            var names = values.Select(x => (Value: x, Name: Enum.GetName(type, x), Split: Enum.GetName(type, x).SplitCamelCase())).ToList();
            if (!names.Any(x => x.Name.EqualTo(value) || x.Split.EqualTo(value))) {
                theEnum = default;
                return false;
            }

            theEnum = names.First(x => x.Name.EqualTo(value) || x.Split.EqualTo(value)).Value;
            return true;
        }

        public static IEnumerable<T> GetAllFlags<T>(this T flags) where T : Enum{
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
            {
                if (flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

		public static T Stage<T>(this T enumValue, int steps) where T : Enum
		{
			var enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
			int currentIndex = Array.IndexOf(enumValues, enumValue);
			int newIndex = currentIndex + steps;

			// Clamp newIndex to be within the range of enum values
			newIndex = Math.Max(0, Math.Min(newIndex, enumValues.Length - 1));

			return enumValues[newIndex];
		}

		public static T StageUp<T>(this T enumValue, uint steps = 1) where T : Enum
		{
			var enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
			int currentIndex = Array.IndexOf(enumValues, enumValue);
			int nextIndex = currentIndex + (int)steps;

			// If nextIndex exceeds the range, return the last enum value
			return nextIndex < enumValues.Length ? enumValues[nextIndex] : enumValues.Last();
		}

		public static T StageDown<T>(this T enumValue, uint steps = 1) where T : Enum
		{
			var enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
			int currentIndex = Array.IndexOf(enumValues, enumValue);
			int nextIndex = currentIndex - (int)steps;

			// If nextIndex is below 0, return the first enum value
			return nextIndex >= 0 ? enumValues[nextIndex] : enumValues.First();
		}

		public static int StepsFrom<T>(this T value, T other) where T : Enum
        {
			var eType = Enum.GetUnderlyingType(typeof(T));
			var valueConverted = Convert.ChangeType(value, eType);
			var otherConverted = Convert.ChangeType(other, eType);
            return Convert.ToInt32(valueConverted) - Convert.ToInt32(otherConverted);
		}

		public static IEnumerable<T> GetSingleFlags<T>(this T flags) where T : Enum
        {
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
            {
                if (flags.HasFlag(value) && Convert.ToInt64(value).IsPowerOfTwo() && Convert.ToInt64(value) > 0)
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<Enum> GetFlags(this Enum flags) {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>()) {
                var bits = Convert.ToUInt64(value);
                while (flag < bits) {
                    flag <<= 1;
                }

                if ((flag == bits) && flags.HasFlag(value)) {
                    yield return value;
                }
            }
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static IEnumerable<byte[]> SplitDelimiter(this byte[] input, IEnumerable<byte> delimiters,
            ByteSplitOptions options = ByteSplitOptions.DiscardDelimiter) {
            var values = new List<byte[]>();
            var current = new List<byte>();
            foreach (var b in input) {
                if (!delimiters.Contains(b)) {
                    current.Add(b);
                }
                else {
                    if (options == ByteSplitOptions.PreserveDelimiter) {
                        current.Add(b);
                    }
                    values.Add(current.ToArray());
                    current.Clear();
                }
            }

            if (current.Any()) {
                values.Add(current.ToArray());
            }

            return values;
        }

        [StringFormatMethod("text")]
        public static StringBuilder AppendLineFormat(this StringBuilder sb, IFormatProvider format, string text,
            params object[] parameters) {
            return sb.AppendLine(string.Format(format, text, parameters));
        }

        [StringFormatMethod("text")]
        public static StringBuilder AppendLineFormat(this StringBuilder sb, string text, params object[] parameters) {
            return sb.AppendLine(string.Format(text, parameters));
        }

        private static Dictionary<Type, ConstructorInfo> _genericListConstructors = new();
        public static List<T> CreateList<T>(this T type) where T : Type
        {
            if (_genericListConstructors.ContainsKey(type))
            {
                return (List<T>)_genericListConstructors[type].Invoke(new object[] { });
            }
            Type listGenericType = typeof(List<>);
            Type listType = listGenericType.MakeGenericType(type);
            ConstructorInfo ci = listType.GetConstructor(new Type[] { });
            _genericListConstructors[type] = ci;
            return (List<T>)ci.Invoke(new object[] { });
        }

        public static string InnerXML(this XElement el)
        {
            var reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static string NowNoLonger(this bool item)
        {
	        return item ? "now" : "no longer";
        }
    }
}