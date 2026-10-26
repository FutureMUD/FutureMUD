using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.Framework
{
    public static class Utilities
    {
        /// <summary>
        /// Tests whether any candidate equals the receiver using object.Equals. Null matches null; an empty candidate array returns false.
        /// </summary>
        public static bool In<T>(this T thing, params T[] values)
        {
            return values.Any(x => Equals(x, thing));
        }
        public enum ByteSplitOptions
        {
            PreserveDelimiter,
            DiscardDelimiter
        }

        /// <summary>
        /// Returns the enum member name, optionally expanding camel case and applying a colour.
        /// </summary>
        public static string DescribeEnum(this Enum value, bool explodeCamelCase = false, ANSIColour colour = null)
        {
            Type type = value.GetType();
            if (type.GetCustomAttribute<FlagsAttribute>() is not null && !Enum.IsDefined(value.GetType(), value))
            {
                return value.GetSingleFlags()
                    .Where(x => value.HasFlag(x))
                    .Select(x => Enum.GetName(type, x).SplitCamelCase(explodeCamelCase).Colour(colour))
                    .ListToString();
            }

            return Enum.GetName(value.GetType(), value).SplitCamelCase(explodeCamelCase).Colour(colour);
        }

        /// <summary>
        /// Parses a declared numeric enum value, or an invariant-culture case-insensitive declared name with optional camel-case spaces. Returns false and default for unrecognised input.
        /// </summary>
        public static bool TryParseEnum<T>(this string value, out T theEnum) where T : Enum
        {
            Type type = typeof(T);
            Type underlying = type.GetEnumUnderlyingType();
            if (underlying == typeof(long))
            {
                if (long.TryParse(value, out long numvalue))
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
                if (int.TryParse(value, out int numvalue))
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
                if (short.TryParse(value, out short numvalue))
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
                if (ulong.TryParse(value, out ulong numvalue))
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
                if (uint.TryParse(value, out uint numvalue))
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
                if (ushort.TryParse(value, out ushort numvalue))
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
                if (byte.TryParse(value, out byte numvalue))
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
                if (sbyte.TryParse(value, out sbyte numvalue))
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

            List<T> values = Enum.GetValues(type).OfType<T>().ToList();
            List<(T Value, string Name, string Split)> names = values.Select(x => (Value: x, Name: Enum.GetName(type, x), Split: Enum.GetName(type, x).SplitCamelCase())).ToList();
            if (!names.Any(x => x.Name.EqualTo(value) || x.Split.EqualTo(value)))
            {
                theEnum = default;
                return false;
            }

            theEnum = names.First(x => x.Name.EqualTo(value) || x.Split.EqualTo(value)).Value;
            return true;
        }

        /// <summary>
        /// Parses an enum value and returns the supplied fallback when parsing fails.
        /// </summary>
        public static T ParseEnumWithDefault<T>(this string text, T fallback) where T : Enum
        {
            return TryParseEnum<T>(text, out T value) ? value : fallback;
        }

        /// <summary>
        /// Enumerates the enum values whose bits are present in the receiver.
        /// </summary>
        public static IEnumerable<T> GetAllFlags<T>(this T flags) where T : Enum
        {
            foreach (T value in Enum.GetValues(flags.GetType()).Cast<T>())
            {
                if (flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Moves through the enum’s declared values by the requested signed number of positions, clamping at either end.
        /// </summary>
        public static T Stage<T>(this T enumValue, int steps) where T : Enum
        {
            T[] enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
            int currentIndex = Array.IndexOf(enumValues, enumValue);
            int newIndex = currentIndex + steps;

            // Clamp newIndex to be within the range of enum values
            newIndex = Math.Max(0, Math.Min(newIndex, enumValues.Length - 1));

            return enumValues[newIndex];
        }

        /// <summary>
        /// Moves forward through the enum’s declared values, clamping at the final value.
        /// </summary>
        public static T StageUp<T>(this T enumValue, uint steps = 1) where T : Enum
        {
            T[] enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
            int currentIndex = Array.IndexOf(enumValues, enumValue);
            int nextIndex = currentIndex + (int)steps;

            // If nextIndex exceeds the range, return the last enum value
            return nextIndex < enumValues.Length ? enumValues[nextIndex] : enumValues.Last();
        }

        /// <summary>
        /// Moves backward through the enum’s declared values, clamping at the first value.
        /// </summary>
        public static T StageDown<T>(this T enumValue, uint steps = 1) where T : Enum
        {
            T[] enumValues = Enum.GetValues(enumValue.GetType()).Cast<T>().ToArray();
            int currentIndex = Array.IndexOf(enumValues, enumValue);
            int nextIndex = currentIndex - (int)steps;

            // If nextIndex is below 0, return the first enum value
            return nextIndex >= 0 ? enumValues[nextIndex] : enumValues.First();
        }

        /// <summary>
        /// Returns the numeric difference between this enum value and another.
        /// </summary>
        public static int StepsFrom<T>(this T value, T other) where T : Enum
        {
            Type eType = Enum.GetUnderlyingType(typeof(T));
            object valueConverted = Convert.ChangeType(value, eType);
            object otherConverted = Convert.ChangeType(other, eType);
            return Convert.ToInt32(valueConverted) - Convert.ToInt32(otherConverted);
        }

        /// <summary>
        /// Enumerates the individual non-zero flags present in the receiver.
        /// </summary>
        public static IEnumerable<T> GetSingleFlags<T>(this T flags) where T : Enum
        {
            foreach (T value in Enum.GetValues(flags.GetType()).Cast<T>())
            {
                if (flags.HasFlag(value) && Convert.ToInt64(value).IsPowerOfTwo() && Convert.ToInt64(value) > 0)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Enumerates the individual non-zero flags present in the enum value.
        /// </summary>
        public static IEnumerable<Enum> GetFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (Enum value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if ((flag == bits) && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Returns the single attribute of the requested type on the named enum field, or null when absent. Unnamed values and multiple matching attributes are unsupported.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        /// <summary>
        /// Splits a byte array at delimiter bytes and yields the segments between delimiters.
        /// </summary>
        public static IEnumerable<byte[]> SplitDelimiter(this byte[] input, IEnumerable<byte> delimiters,
            ByteSplitOptions options = ByteSplitOptions.DiscardDelimiter)
        {
            List<byte[]> values = new();
            List<byte> current = new();
            foreach (byte b in input)
            {
                if (!delimiters.Contains(b))
                {
                    current.Add(b);
                }
                else
                {
                    if (options == ByteSplitOptions.PreserveDelimiter)
                    {
                        current.Add(b);
                    }
                    values.Add(current.ToArray());
                    current.Clear();
                }
            }

            if (current.Any())
            {
                values.Add(current.ToArray());
            }

            return values;
        }

        /// <summary>
        /// Appends a formatted line to the string builder and returns the same builder for chaining.
        /// </summary>
        [StringFormatMethod("text")]
        public static StringBuilder AppendLineFormat(this StringBuilder sb, IFormatProvider format, string text,
            params object[] parameters)
        {
            return sb.AppendLine(string.Format(format, text, parameters));
        }

        /// <summary>
        /// Appends a formatted line to the string builder and returns the same builder for chaining.
        /// </summary>
        [StringFormatMethod("text")]
        public static StringBuilder AppendLineFormat(this StringBuilder sb, string text, params object[] parameters)
        {
            return sb.AppendLine(string.Format(text, parameters));
        }

        private static readonly Dictionary<Type, ConstructorInfo> _genericListConstructors = new();
        /// <summary>
        /// Creates an empty list whose element type is the supplied type.
        /// </summary>
        public static System.Collections.IList CreateList(this Type type)
        {
            if (_genericListConstructors.TryGetValue(type, out ConstructorInfo ctor))
            {
                return (System.Collections.IList)ctor.Invoke(Array.Empty<object>());
            }

            Type listType = typeof(List<>).MakeGenericType(type);
            ConstructorInfo ci = listType.GetConstructor(Type.EmptyTypes);
            _genericListConstructors[type] = ci;
            return (System.Collections.IList)ci.Invoke(Array.Empty<object>());
        }

        /// <summary>
        /// Returns the inner XML content of the element as a string.
        /// </summary>
        public static string InnerXML(this XElement el)
        {
            XmlReader reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        /// <summary>
        /// This is simply a convenience method to convert a boolean to "now" or "no longer", avoiding a ternary operator in string interpolations
        /// </summary>
        /// <param name="item">The boolean to use for Now/No Longer</param>
        /// <returns>true = now, false = no longer</returns>
        public static string NowNoLonger(this bool item)
        {
            return item ? "now" : "no longer";
        }
    }
}