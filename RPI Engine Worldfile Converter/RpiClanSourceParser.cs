#nullable enable

using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiClanSourceParser
{
	private static readonly Regex HeaderRegex = new(
		@"^\|\s*(?<group>\d+)\s*\|\s*(?<name>[^|]+?)\s*\|\s*(?<alias>[A-Za-z0-9_\-]+)\s*$",
		RegexOptions.Compiled);

	private static readonly Regex FlagRegex = new(
		@"flags\s*==\s*(?<flag>CLAN_[A-Z_]+)",
		RegexOptions.Compiled);

	public RpiClanSourceDocument Parse(string sourceFile)
	{
		if (!File.Exists(sourceFile))
		{
			throw new FileNotFoundException("Could not find clan source file.", sourceFile);
		}

		var lines = File.ReadAllLines(sourceFile);
		var warnings = new List<string>();
		var headers = ParseHeaders(lines);
		var displayNamesByAlias = ParseDisplayFunction(lines, warnings);
		var synonymsByAlias = ParseSynonymFunction(lines, warnings);
		var aliasNames = headers
			.Select(x => x.Alias)
			.Concat(displayNamesByAlias.Keys)
			.Concat(synonymsByAlias.Keys)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var aliases = new Dictionary<string, RpiClanAliasSource>(StringComparer.OrdinalIgnoreCase);
		foreach (var alias in aliasNames)
		{
			displayNamesByAlias.TryGetValue(alias, out var displayNames);
			synonymsByAlias.TryGetValue(alias, out var synonyms);

			aliases[alias] = new RpiClanAliasSource(
				alias,
				NormalizeValues(displayNames),
				NormalizeValues(synonyms));
		}

		return new RpiClanSourceDocument(sourceFile, headers, aliases, warnings);
	}

	private static IReadOnlyList<RpiClanHeaderEntry> ParseHeaders(IReadOnlyList<string> lines)
	{
		List<RpiClanHeaderEntry> headers = [];

		foreach (var line in lines)
		{
			var match = HeaderRegex.Match(line);
			if (!match.Success)
			{
				continue;
			}

			headers.Add(new RpiClanHeaderEntry(
				int.Parse(match.Groups["group"].Value),
				match.Groups["name"].Value.Trim(),
				match.Groups["alias"].Value.Trim()));
		}

		return headers;
	}

	private static Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>> ParseDisplayFunction(
		IReadOnlyList<string> lines,
		ICollection<string> warnings)
	{
		var result = new Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>>(StringComparer.OrdinalIgnoreCase);
		var range = FindFunctionRange(lines, "get_clan_rank_name (CHAR_DATA *ch, char * clan, int flags)");
		if (range is null)
		{
			warnings.Add("Could not locate get_clan_rank_name(CHAR_DATA*, char*, int) in clan.cpp.");
			return result;
		}

		var starts = FindBranchStarts(lines, range.Value.Start, range.Value.End, includeMembershipFallback: true);
		for (var i = 0; i < starts.Count; i++)
		{
			var start = starts[i];
			var end = i + 1 < starts.Count ? starts[i + 1] - 1 : range.Value.End;
			if (!TryParseDisplayBranchSlot(lines[start], out var slot))
			{
				continue;
			}

			ParseDisplayBranch(lines, start, end, slot, result);
		}

		return result;
	}

	private static void ParseDisplayBranch(
		IReadOnlyList<string> lines,
		int start,
		int end,
		RpiClanRankSlot slot,
		Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>> result)
	{
		for (var i = start; i <= end; i++)
		{
			if (!lines[i].Contains("if", StringComparison.Ordinal))
			{
				continue;
			}

			var conditionStart = i;
			var conditionBuilder = lines[i];
			while (conditionStart < end &&
			       !conditionBuilder.Contains('{') &&
			       !conditionBuilder.Contains("return", StringComparison.Ordinal))
			{
				conditionStart++;
				conditionBuilder += $" {lines[conditionStart]}";
			}

			if (!conditionBuilder.Contains("clan", StringComparison.Ordinal))
			{
				i = conditionStart;
				continue;
			}

			var aliases = ExtractComparands(conditionBuilder, "clan");
			if (aliases.Count == 0)
			{
				i = conditionStart;
				continue;
			}

			if (conditionBuilder.Contains("return", StringComparison.Ordinal))
			{
				var inlineReturns = ExtractReturnStrings(conditionBuilder);
				AddDisplayNames(result, aliases, slot, inlineReturns);
				i = conditionStart;
				continue;
			}

			if (!conditionBuilder.Contains('{'))
			{
				i = conditionStart;
				continue;
			}

			var blockStart = conditionStart;
			var blockEnd = FindBlockEnd(lines, blockStart, end);
			var returnValues = ExtractReturnStrings(string.Join(Environment.NewLine, lines.Skip(blockStart).Take(blockEnd - blockStart + 1)));
			AddDisplayNames(result, aliases, slot, returnValues);
			i = blockEnd;
		}
	}

	private static Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>> ParseSynonymFunction(
		IReadOnlyList<string> lines,
		ICollection<string> warnings)
	{
		var result = new Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>>(StringComparer.OrdinalIgnoreCase);
		var range = FindFunctionRange(lines, "clan_flags_to_value (char *flag_names, char *clan_name)");
		if (range is null)
		{
			warnings.Add("Could not locate clan_flags_to_value(char*, char*) in clan.cpp.");
			return result;
		}

		var starts = FindConditionalBranchStarts(lines, range.Value.Start, range.Value.End);
		for (var i = 0; i < starts.Count; i++)
		{
			var start = starts[i];
			var end = i + 1 < starts.Count ? starts[i + 1] - 1 : range.Value.End;
			var text = string.Join(" ", lines.Skip(start).Take(end - start + 1).Select(x => x.Trim()));
			var slot = TryParseSynonymBranchSlot(text);
			if (slot is null)
			{
				continue;
			}

			var aliases = ExtractComparands(text, "clan_name");
			var tokens = ExtractComparands(text, "buf");
			if (aliases.Count == 0 || tokens.Count == 0)
			{
				continue;
			}

			foreach (var alias in aliases)
			{
				if (!result.TryGetValue(alias, out var slotMap))
				{
					slotMap = new Dictionary<RpiClanRankSlot, List<string>>();
					result[alias] = slotMap;
				}

				if (!slotMap.TryGetValue(slot.Value, out var values))
				{
					values = [];
					slotMap[slot.Value] = values;
				}

				foreach (var token in tokens.Where(x => !string.IsNullOrWhiteSpace(x)))
				{
					if (!values.Contains(token, StringComparer.OrdinalIgnoreCase))
					{
						values.Add(token);
					}
				}
			}
		}

		return result;
	}

	private static IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> NormalizeValues(
		Dictionary<RpiClanRankSlot, List<string>>? values)
	{
		values ??= [];
		return values
			.OrderBy(x => x.Key.SortOrder())
			.ToDictionary(
				x => x.Key,
				x => (IReadOnlyList<string>)x.Value
					.Where(y => !string.IsNullOrWhiteSpace(y))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList());
	}

	private static void AddDisplayNames(
		Dictionary<string, Dictionary<RpiClanRankSlot, List<string>>> result,
		IReadOnlyCollection<string> aliases,
		RpiClanRankSlot slot,
		IReadOnlyCollection<string> values)
	{
		if (values.Count == 0)
		{
			return;
		}

		foreach (var alias in aliases)
		{
			if (!result.TryGetValue(alias, out var slotMap))
			{
				slotMap = new Dictionary<RpiClanRankSlot, List<string>>();
				result[alias] = slotMap;
			}

			if (!slotMap.TryGetValue(slot, out var names))
			{
				names = [];
				slotMap[slot] = names;
			}

			foreach (var value in values)
			{
				if (!names.Contains(value, StringComparer.OrdinalIgnoreCase))
				{
					names.Add(value);
				}
			}
		}
	}

	private static (int Start, int End)? FindFunctionRange(IReadOnlyList<string> lines, string signature)
	{
		var signatureLine = -1;
		for (var i = 0; i < lines.Count; i++)
		{
			if (lines[i].Contains(signature, StringComparison.Ordinal))
			{
				signatureLine = i;
				break;
			}
		}

		if (signatureLine == -1)
		{
			return null;
		}

		var braceDepth = 0;
		var bodyStart = -1;
		for (var i = signatureLine; i < lines.Count; i++)
		{
			braceDepth += Count(lines[i], '{');
			if (bodyStart == -1 && braceDepth > 0)
			{
				bodyStart = i;
			}

			braceDepth -= Count(lines[i], '}');
			if (bodyStart != -1 && braceDepth == 0)
			{
				return (bodyStart, i);
			}
		}

		return null;
	}

	private static List<int> FindBranchStarts(
		IReadOnlyList<string> lines,
		int start,
		int end,
		bool includeMembershipFallback)
	{
		List<int> starts = [];
		for (var i = start; i <= end; i++)
		{
			var line = lines[i];
			if (!line.Contains("if", StringComparison.Ordinal))
			{
				continue;
			}

			if (FlagRegex.IsMatch(line))
			{
				starts.Add(i);
				continue;
			}

			if (includeMembershipFallback && line.Contains("flags > 0", StringComparison.Ordinal))
			{
				starts.Add(i);
			}
		}

		return starts;
	}

	private static List<int> FindConditionalBranchStarts(IReadOnlyList<string> lines, int start, int end)
	{
		List<int> starts = [];
		for (var i = start; i <= end; i++)
		{
			var line = lines[i].TrimStart();
			if (line.StartsWith("if (", StringComparison.Ordinal) ||
			    line.StartsWith("else if", StringComparison.Ordinal))
			{
				starts.Add(i);
			}
		}

		return starts;
	}

	private static bool TryParseDisplayBranchSlot(string line, out RpiClanRankSlot slot)
	{
		slot = RpiClanRankSlot.Membership;
		var match = FlagRegex.Match(line);
		if (match.Success && RpiClanRankSlots.TryParseFlag(match.Groups["flag"].Value, out slot))
		{
			return true;
		}

		if (line.Contains("flags > 0", StringComparison.Ordinal))
		{
			slot = RpiClanRankSlot.Membership;
			return true;
		}

		return false;
	}

	private static RpiClanRankSlot? TryParseSynonymBranchSlot(string text)
	{
		var match = Regex.Match(text, @"flags\s*\|\=\s*(?<flag>CLAN_[A-Z_]+)");
		if (!match.Success)
		{
			return null;
		}

		return RpiClanRankSlots.TryParseFlag(match.Groups["flag"].Value, out var slot) ? slot : null;
	}

	private static int FindBlockEnd(IReadOnlyList<string> lines, int start, int end)
	{
		var braceDepth = 0;
		for (var i = start; i <= end; i++)
		{
			braceDepth += Count(lines[i], '{');
			braceDepth -= Count(lines[i], '}');
			if (braceDepth == 0)
			{
				return i;
			}
		}

		return end;
	}

	private static int Count(string text, char needle)
	{
		var count = 0;
		foreach (var value in text)
		{
			if (value == needle)
			{
				count++;
			}
		}

		return count;
	}

	private static List<string> ExtractComparands(string text, string variable)
	{
		var regex = new Regex($@"!str_cmp\s*\(\s*{Regex.Escape(variable)}\s*,\s*""(?<value>[^""]+)""\s*\)");
		return regex.Matches(text)
			.Select(x => x.Groups["value"].Value.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static List<string> ExtractReturnStrings(string text)
	{
		return Regex.Matches(text, @"return\s+""(?<value>[^""]+)""\s*;")
			.Select(x => x.Groups["value"].Value.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
	}
}
