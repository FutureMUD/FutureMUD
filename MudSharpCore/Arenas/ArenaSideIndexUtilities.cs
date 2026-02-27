#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

	public static IReadOnlyDictionary<int, int> ResolveEvenlySpacedStartCells(
		IReadOnlyList<int>? orderedSideIndices,
		int arenaCellCount,
		int rotationOffset)
	{
		var result = new Dictionary<int, int>();
		if (orderedSideIndices is null || arenaCellCount <= 0)
		{
			return result;
		}

		var sides = orderedSideIndices
			.Distinct()
			.ToList();
		if (sides.Count == 0)
		{
			return result;
		}

		var normalisedRotation = rotationOffset % arenaCellCount;
		if (normalisedRotation < 0)
		{
			normalisedRotation += arenaCellCount;
		}

		for (var i = 0; i < sides.Count; i++)
		{
			var baseIndex = i * arenaCellCount / sides.Count;
			result[sides[i]] = (baseIndex + normalisedRotation) % arenaCellCount;
		}

		return result;
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
