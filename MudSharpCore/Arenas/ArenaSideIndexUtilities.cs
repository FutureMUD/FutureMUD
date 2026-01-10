#nullable enable

using System;
using System.Globalization;

namespace MudSharp.Arenas;

internal static class ArenaSideIndexUtilities
{
	public static int ToDisplayIndex(int sideIndex)
	{
		return sideIndex + 1;
	}

	public static string ToDisplayString(IFormatProvider formatProvider, int sideIndex)
	{
		return ToDisplayIndex(sideIndex).ToString(formatProvider);
	}

	public static bool TryParseDisplayIndex(string? text, out int sideIndex)
	{
		sideIndex = 0;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var displayIndex) || displayIndex <= 0)
		{
			return false;
		}

		sideIndex = displayIndex - 1;
		return true;
	}
}
