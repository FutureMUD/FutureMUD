#nullable enable

using System;
using System.Globalization;
using System.Text;

namespace MudSharp.Planes;

public static class PlaneDisplayFormat
{
	public const int MaximumFormatLength = 1024;
	public const int MaximumFields = 16;
	public const int MaximumAlignment = 128;
	public const int MaximumOutputLength = 16384;

	public static bool TryFormat(string? format, string value, out string result)
	{
		result = value;
		if (string.IsNullOrWhiteSpace(format) || format.Length > MaximumFormatLength) return false;
		try
		{
			if (CompositeFormat.Parse(format).MinimumArgumentCount != 1) return false;
			var fields = 0;
			for (var i = 0; i < format.Length; i++)
			{
				if (format[i] != '{') continue;
				if (i + 1 < format.Length && format[i + 1] == '{')
				{
					i++;
					continue;
				}
				var end = format.IndexOf('}', i + 1);
				if (end < 0 || ++fields > MaximumFields) return false;
				var field = format.AsSpan(i + 1, end - i - 1);
				var colon = field.IndexOf(':');
				if (colon >= 0) field = field[..colon];
				var comma = field.IndexOf(',');
				if (comma >= 0 && (!int.TryParse(field[(comma + 1)..], NumberStyles.Integer,
					CultureInfo.InvariantCulture, out var alignment) ||
					alignment < -MaximumAlignment || alignment > MaximumAlignment)) return false;
				i = end;
			}
			if (fields == 0 || format.Length + (long)fields * Math.Max(value.Length, MaximumAlignment) > MaximumOutputLength)
				return false;
			result = string.Format(CultureInfo.InvariantCulture, format, value);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
	}
}
