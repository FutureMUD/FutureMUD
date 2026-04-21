#nullable enable

using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiClanRegionReferenceScanner
{
	private static readonly Regex StructuredRankAliasPairRegex = new(
		@"'(?<rank>[^']+)'\s+(?<alias>[A-Za-z0-9_\-]+)",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex StructuredRankAliasLineRegex = new(
		@"^(?:'[^']+'\s+[A-Za-z0-9_\-]+\s*)+~?$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ClanRankCallRegex = new(
		@"clanrank\s*\(\s*(?<alias>[A-Za-z0-9_\-]+)\s*,\s*(?<rank>[A-Za-z0-9_\- ]+)\s*\)",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ClanCommandLineRegex = new(
		@"^\s*clan\s+(?:(?<verb>set)\s+)?(?<alias>[A-Za-z0-9_\-]+)\s+(?<rank>[A-Za-z0-9_\-]+)\b",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ClanPresenceRegex = new(
		@"clan\s*\(\s*-?\d+\s*,\s*(?<alias>[A-Za-z0-9_\-]+)\s*\)",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ClanEchoRegex = new(
		@"clan_echo\s+zone\s+(?<alias>[A-Za-z0-9_\-]+)",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly HashSet<string> PresenceOnlyCommandRanks =
	[
		"remove",
	];

	public RpiClanReferenceIndex Scan(
		string regionsDirectory,
		RpiClanSourceDocument sourceDocument,
		IEnumerable<RpiItemRecord>? items = null)
	{
		var references = new Dictionary<string, MutableReference>(StringComparer.OrdinalIgnoreCase);
		foreach (var file in EnumerateSourceFiles(regionsDirectory))
		{
			ScanFile(file, sourceDocument, references);
		}

		if (items is not null)
		{
		foreach (var item in items)
		{
			foreach (var clan in item.Clans)
			{
				if (ResolveObservedSlots(clan.Name, clan.Rank, sourceDocument).Count == 0)
				{
					continue;
				}

				Observe(
					references,
					sourceDocument,
						clan.Name,
						clan.Rank,
						$"{Path.GetFileName(item.SourceFile)}#{item.Vnum}");
				}
			}
		}

		return new RpiClanReferenceIndex(
			references
				.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					x => x.Key,
					x => x.Value.ToRecord(),
					StringComparer.OrdinalIgnoreCase));
	}

	private static IEnumerable<string> EnumerateSourceFiles(string regionsDirectory)
	{
		foreach (var pattern in new[] { "mobs.*", "rooms.*" })
		{
			foreach (var file in Directory.EnumerateFiles(regionsDirectory, pattern, SearchOption.TopDirectoryOnly))
			{
				yield return file;
			}
		}

		var craftsFile = Path.Combine(regionsDirectory, "crafts.txt");
		if (File.Exists(craftsFile))
		{
			yield return craftsFile;
		}
	}

	private static void ScanFile(
		string file,
		RpiClanSourceDocument sourceDocument,
		IDictionary<string, MutableReference> references)
	{
		var lineNumber = 0;
		foreach (var line in File.ReadLines(file))
		{
			lineNumber++;
			var sample = $"{Path.GetFileName(file)}:{lineNumber}";
			var trimmedLine = line.Trim();

			ScanStructuredRankAliasLine(trimmedLine, sourceDocument, references, sample);

			foreach (Match match in ClanRankCallRegex.Matches(line))
			{
				Observe(references, sourceDocument, match.Groups["alias"].Value, match.Groups["rank"].Value, sample);
			}

			if (TryParseClanCommand(trimmedLine, out var commandAlias, out var commandRank) &&
			    (commandRank is null || ResolveObservedSlots(commandAlias, commandRank, sourceDocument).Count > 0))
			{
				Observe(references, sourceDocument, commandAlias, commandRank, sample);
			}

			foreach (Match match in ClanPresenceRegex.Matches(line))
			{
				Observe(references, sourceDocument, match.Groups["alias"].Value, null, sample);
			}

			foreach (Match match in ClanEchoRegex.Matches(line))
			{
				Observe(references, sourceDocument, match.Groups["alias"].Value, null, sample);
			}
		}
	}

	private static void ScanStructuredRankAliasLine(
		string trimmedLine,
		RpiClanSourceDocument sourceDocument,
		IDictionary<string, MutableReference> references,
		string sample)
	{
		if (string.IsNullOrWhiteSpace(trimmedLine))
		{
			return;
		}

		string? structuredSegment = null;
		if (trimmedLine.StartsWith("clans:", StringComparison.OrdinalIgnoreCase))
		{
			structuredSegment = trimmedLine["clans:".Length..].Trim();
		}
		else if (trimmedLine.StartsWith("'", StringComparison.Ordinal) &&
		         StructuredRankAliasLineRegex.IsMatch(trimmedLine))
		{
			structuredSegment = trimmedLine.TrimEnd('~').Trim();
		}

		if (string.IsNullOrWhiteSpace(structuredSegment))
		{
			return;
		}

		foreach (Match match in StructuredRankAliasPairRegex.Matches(structuredSegment))
		{
			var alias = match.Groups["alias"].Value;
			var rank = match.Groups["rank"].Value;
			if (ResolveObservedSlots(alias, rank, sourceDocument).Count == 0)
			{
				continue;
			}

			Observe(references, sourceDocument, alias, rank, sample);
		}
	}

	private static bool TryParseClanCommand(string trimmedLine, out string alias, out string? rankText)
	{
		alias = string.Empty;
		rankText = null;
		if (string.IsNullOrWhiteSpace(trimmedLine))
		{
			return false;
		}

		var match = ClanCommandLineRegex.Match(trimmedLine);
		if (!match.Success)
		{
			return false;
		}

		alias = match.Groups["alias"].Value;
		var rank = match.Groups["rank"].Value;
		if (PresenceOnlyCommandRanks.Contains(rank))
		{
			rankText = null;
			return true;
		}

		rankText = rank;
		return true;
	}

	private static void Observe(
		IDictionary<string, MutableReference> references,
		RpiClanSourceDocument sourceDocument,
		string alias,
		string? rankText,
		string sample)
	{
		if (!references.TryGetValue(alias, out var reference))
		{
			reference = new MutableReference(alias);
			references[alias] = reference;
		}

		reference.AliasReferenceCount++;
		reference.AddSample(sample);

		foreach (var slot in ResolveObservedSlots(alias, rankText, sourceDocument))
		{
			reference.ObservedSlots.Add(slot);
		}
	}

	private static IReadOnlyCollection<RpiClanRankSlot> ResolveObservedSlots(
		string alias,
		string? rankText,
		RpiClanSourceDocument sourceDocument)
	{
		if (string.IsNullOrWhiteSpace(rankText))
		{
			return Array.Empty<RpiClanRankSlot>();
		}

		var normalizedExpression = NormalizeRankKey(rankText);
		var tokens = rankText
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(NormalizeRankKey)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToList();

		HashSet<RpiClanRankSlot> matches = [];
		foreach (var slot in Enum.GetValues<RpiClanRankSlot>().Where(x => x.IsImportable() || x == RpiClanRankSlot.Leadership))
		{
			var rankKeys = BuildRankKeys(alias, slot, sourceDocument);
			if (rankKeys.Contains(normalizedExpression))
			{
				matches.Add(slot);
				continue;
			}

			if (tokens.Any(rankKeys.Contains))
			{
				matches.Add(slot);
			}
		}

		return matches;
	}

	private static HashSet<string> BuildRankKeys(
		string alias,
		RpiClanRankSlot slot,
		RpiClanSourceDocument sourceDocument)
	{
		HashSet<string> keys = [NormalizeRankKey(slot.GenericName())];
		foreach (var defaultKey in GetDefaultRankKeys(slot))
		{
			keys.Add(defaultKey);
		}

		if (sourceDocument.AliasSources.TryGetValue(alias, out var source))
		{
			AddValues(keys, source.DisplayNamesBySlot, slot);
			AddValues(keys, source.SynonymsBySlot, slot);
		}

		return keys;
	}

	private static IReadOnlyCollection<string> GetDefaultRankKeys(RpiClanRankSlot slot)
	{
		return slot switch
		{
			RpiClanRankSlot.Membership => ["member"],
			RpiClanRankSlot.Leadership => ["leader"],
			RpiClanRankSlot.MemberObject => ["memberobj", "member-object"],
			RpiClanRankSlot.LeaderObject => ["leaderobj", "leader-object"],
			_ => Array.Empty<string>(),
		};
	}

	private static void AddValues(
		ISet<string> target,
		IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> values,
		RpiClanRankSlot slot)
	{
		if (!values.TryGetValue(slot, out var items))
		{
			return;
		}

		foreach (var item in items)
		{
			var normalized = NormalizeRankKey(item);
			if (!string.IsNullOrWhiteSpace(normalized))
			{
				target.Add(normalized);
			}
		}
	}

	private static string NormalizeRankKey(string value)
	{
		return Regex.Replace(value.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
	}

	private sealed class MutableReference
	{
		private readonly List<string> _samples = [];

		public MutableReference(string alias)
		{
			Alias = alias;
		}

		public string Alias { get; }
		public int AliasReferenceCount { get; set; }
		public HashSet<RpiClanRankSlot> ObservedSlots { get; } = [];

		public void AddSample(string sample)
		{
			if (_samples.Count >= 5 || _samples.Contains(sample, StringComparer.OrdinalIgnoreCase))
			{
				return;
			}

			_samples.Add(sample);
		}

		public RpiClanReferenceRecord ToRecord()
		{
			return new RpiClanReferenceRecord(
				Alias,
				AliasReferenceCount,
				ObservedSlots.OrderBy(x => x.SortOrder()).ToList(),
				_samples.ToList());
		}
	}
}
