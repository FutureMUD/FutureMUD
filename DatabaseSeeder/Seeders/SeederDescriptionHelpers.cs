#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal sealed record StockDescriptionVariant(
	string ShortDescription,
	string FullDescription
);

internal static class SeederDescriptionHelpers
{
	internal static IReadOnlyList<StockDescriptionVariant> BuildVariantList(
		params (string ShortDescription, string FullDescription)[] variants)
	{
		return variants
			.Select(x => new StockDescriptionVariant(x.ShortDescription, x.FullDescription))
			.ToArray();
	}

	internal static string JoinParagraphs(params string[] paragraphs)
	{
		return string.Join(
			"\n\n",
			paragraphs.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())
		);
	}

	internal static int CountParagraphs(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return 0;
		}

		return text
			.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Length;
	}

	internal static bool HasMinimumParagraphs(string? text, int minimum = 3)
	{
		return CountParagraphs(text) >= minimum;
	}

	internal static string TrimLeadingSentencePrefix(string text, string prefix)
	{
		return text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? text[prefix.Length..].TrimStart()
			: text;
	}

	internal static string EnsureTrailingPeriod(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}

		var trimmed = text.Trim();
		return trimmed.EndsWith(".") || trimmed.EndsWith("!") || trimmed.EndsWith("?")
			? trimmed
			: $"{trimmed}.";
	}
}
