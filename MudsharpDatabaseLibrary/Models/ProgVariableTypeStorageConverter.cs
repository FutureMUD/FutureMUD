#nullable enable

using System;
using System.Globalization;

namespace MudSharp.Models;

internal static class ProgVariableTypeStorageConverter
{
	private const string StoragePrefix = "v1:";

	public static string FromLegacyLong(long value)
	{
		return $"{StoragePrefix}{value.ToString("x", System.Globalization.CultureInfo.InvariantCulture)}";
	}

	public static long ToLegacyLong(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0L;
		}

		var definition = value.Trim();
		if (definition.StartsWith(StoragePrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			definition = definition[StoragePrefix.Length..];
		}

		return long.Parse(definition, NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
	}
}
