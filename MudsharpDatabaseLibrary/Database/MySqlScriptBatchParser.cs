#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.Database;

internal readonly record struct MySqlScriptBatch(string Script, string Delimiter);

internal static partial class MySqlScriptBatchParser
{
	public static IReadOnlyList<MySqlScriptBatch> Parse(string script)
	{
		ArgumentNullException.ThrowIfNull(script);
		List<MySqlScriptBatch> batches = [];
		StringBuilder currentBatch = new();
		var delimiter = ";";
		using StringReader reader = new(script);
		while (reader.ReadLine() is { } line)
		{
			var match = DelimiterDirectiveRegex().Match(line);
			if (!match.Success)
			{
				currentBatch.AppendLine(line);
				continue;
			}

			AddBatchIfPresent(batches, currentBatch, delimiter);
			delimiter = match.Groups["delimiter"].Value;
		}

		AddBatchIfPresent(batches, currentBatch, delimiter);
		return batches;
	}

	private static void AddBatchIfPresent(List<MySqlScriptBatch> batches, StringBuilder currentBatch,
		string delimiter)
	{
		if (currentBatch.Length == 0)
		{
			return;
		}

		var script = currentBatch.ToString();
		currentBatch.Clear();
		if (!string.IsNullOrWhiteSpace(script))
		{
			batches.Add(new MySqlScriptBatch(script, delimiter));
		}
	}

	[GeneratedRegex(@"^\s*DELIMITER\s+(?<delimiter>\S+)\s*$", RegexOptions.IgnoreCase)]
	private static partial Regex DelimiterDirectiveRegex();
}
