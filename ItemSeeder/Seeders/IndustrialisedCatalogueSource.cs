#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Reads authored TSV without modifying prose. Source locations survive conversion to typed rows.
/// Both packaged installation and developer checks use this parser; refresh never authors content.
/// </summary>
internal sealed record IndustrialisedCatalogueSource(string Name, string Text)
{
	internal IEnumerable<T> Read<T>(IReadOnlyList<string> headers, Func<string, int, string[], T> parse)
	{
		using var reader = new StringReader(Text);
		var header = reader.ReadLine()?.TrimStart('\uFEFF').Split('\t') ?? [];
		if (!header.SequenceEqual(headers, StringComparer.Ordinal))
		{
			throw new InvalidDataException($"{Name}:1: invalid header. Expected: {string.Join("\\t", headers)}");
		}

		var lineNumber = 1;
		while (reader.ReadLine() is { } line)
		{
			lineNumber++;
			if (line.Length == 0)
			{
				continue;
			}

			var fields = line.Split('\t');
			if (fields.Length != headers.Count)
			{
				throw new InvalidDataException($"{Name}:{lineNumber}: has {fields.Length} columns; expected {headers.Count}.");
			}

			T row;
			try
			{
				row = parse(Name, lineNumber, fields);
			}
			catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
			{
				throw new InvalidDataException($"{Name}:{lineNumber}: {ex.Message}", ex);
			}

			yield return row;
		}
	}
}

internal static class IndustrialisedCatalogueValues
{
	internal static IReadOnlyList<string> List(string text)
	{
		if (text.Length == 0)
		{
			return Array.Empty<string>();
		}

		var values = text.Split(';', StringSplitOptions.TrimEntries);
		if (values.Any(string.IsNullOrEmpty) || values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != values.Length)
		{
			throw new FormatException($"List '{text}' contains an empty or duplicate entry; use an empty cell for no values.");
		}

		return Array.AsReadOnly(values);
	}

	internal static T EnumValue<T>(string text) where T : struct, Enum
	{
		if (!Enum.GetNames<T>().Contains(text, StringComparer.OrdinalIgnoreCase) || !Enum.TryParse<T>(text, true, out var value))
		{
			throw new FormatException($"Unknown {typeof(T).Name} '{text}'; use a named enum value.");
		}

		return value;
	}

	internal static int Int(string text) => int.Parse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
	internal static decimal Decimal(string text) => decimal.Parse(text,
		NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
	internal static double Double(string text)
	{
		var value = double.Parse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
			CultureInfo.InvariantCulture);
		return double.IsFinite(value) ? value : throw new FormatException($"Non-finite number '{text}' is not allowed.");
	}
}
