#nullable enable

using MudSharp.Community;

namespace RPI_Engine_Worldfile_Converter;

internal sealed record CanonicalClanRule(string CanonicalAlias, string? FullNameOverride, IReadOnlyList<string> LegacyAliases);

public sealed class FutureMudClanTransformer
{
	private static readonly IReadOnlyDictionary<string, CanonicalClanRule> CanonicalRules =
		new Dictionary<string, CanonicalClanRule>(StringComparer.OrdinalIgnoreCase)
		{
			["mordor_char"] = new("mordor_char", "Minas Morgul", ["mm_denizens"]),
			["mm_denizens"] = new("mordor_char", "Minas Morgul", ["mm_denizens"]),
			["malred"] = new("malred", "Malred Family", ["housemalred"]),
			["housemalred"] = new("malred", "Malred Family", ["housemalred"]),
			["rogues"] = new("rogues", "Rogues' Fellowship", ["rouges"]),
			["rouges"] = new("rogues", "Rogues' Fellowship", ["rouges"]),
			["hawk_dove_2"] = new("hawk_dove_2", "Hawk and Dove", ["hawk_and_dove"]),
			["hawk_and_dove"] = new("hawk_dove_2", "Hawk and Dove", ["hawk_and_dove"]),
			["seekers"] = new("seekers", "Seekers", Array.Empty<string>()),
			["shadow-cult"] = new("shadow-cult", "Shadow Cult", Array.Empty<string>()),
			["tirithguard"] = new("tirithguard", "Minas Tirith Guard", Array.Empty<string>()),
			["eradan_battalion"] = new("eradan_battalion", "Eradan Battalion", Array.Empty<string>()),
		};

	private static readonly IReadOnlyList<string> AliasOnlyClans =
	[
		"seekers",
		"shadow-cult",
		"tirithguard",
		"eradan_battalion",
	];

	public ClanConversionResult Convert(RpiClanSourceDocument sourceDocument, RpiClanReferenceIndex references)
	{
		var sourceClans = BuildSourceDefinitions(sourceDocument, references);
		var converted = sourceClans
			.Select(x => ConvertClan(x, references))
			.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var importedAliases = BuildImportedAliasSet(sourceClans);
		var unresolved = references.ReferencesByAlias
			.Where(x => !importedAliases.Contains(ResolveCanonicalRule(x.Key).CanonicalAlias))
			.OrderByDescending(x => x.Value.AliasReferenceCount)
			.ThenBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => x.Value.AliasReferenceCount,
				StringComparer.OrdinalIgnoreCase);

		return new ClanConversionResult(sourceClans, converted, unresolved);
	}

	private static IReadOnlyList<RpiClanDefinition> BuildSourceDefinitions(
		RpiClanSourceDocument sourceDocument,
		RpiClanReferenceIndex references)
	{
		var importedCanonicalAliases = sourceDocument.HeaderEntries
			.Select(x => ResolveCanonicalRule(x.Alias).CanonicalAlias)
			.Concat(AliasOnlyClans.Select(x => ResolveCanonicalRule(x).CanonicalAlias))
			.Concat(references.ReferencesByAlias
				.Where(x => x.Value.ObservedSlots.Any(y => y.IsImportable()))
				.Select(x => ResolveCanonicalRule(x.Key).CanonicalAlias))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		List<RpiClanDefinition> definitions = [];
		foreach (var canonicalAlias in importedCanonicalAliases)
		{
			var rule = ResolveCanonicalRule(canonicalAlias);
			var aliases = sourceDocument.AliasSources.Keys
				.Where(x => ResolveCanonicalRule(x).CanonicalAlias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase))
				.Concat(references.ReferencesByAlias.Keys
					.Where(x => ResolveCanonicalRule(x).CanonicalAlias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase)))
				.Concat(rule.LegacyAliases)
				.Append(canonicalAlias)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderByDescending(x => x.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase))
				.ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList();

			var headerEntry = sourceDocument.HeaderEntries
				.FirstOrDefault(x => ResolveCanonicalRule(x.Alias).CanonicalAlias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase));

			List<string> warnings = [];
			var fullName = rule.FullNameOverride ??
			               headerEntry?.FullName ??
			               DiscoverFullName(canonicalAlias, references) ??
			               RpiClanRankSlots.TitleCaseAlias(canonicalAlias);

			if (headerEntry is null && rule.FullNameOverride is null)
			{
				warnings.Add($"Fell back to a title-cased full name for alias '{canonicalAlias}'.");
			}

			var hasSourceCodeAlias = aliases.Any(sourceDocument.AliasSources.ContainsKey);
			if (headerEntry is null && !hasSourceCodeAlias)
			{
				warnings.Add($"Synthesized clan '{canonicalAlias}' from structured worldfile references.");
			}

			var aliasRecords = aliases
				.Select(alias => new RpiClanAliasRecord(
					alias,
					alias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase),
					!alias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase),
					DetermineAliasSource(alias, canonicalAlias, headerEntry, sourceDocument, references)))
				.ToList();

			var displayNamesBySlot = MergeSlotValues(aliases, sourceDocument, useDisplayNames: true);
			var synonymsBySlot = MergeSlotValues(aliases, sourceDocument, useDisplayNames: false);
			foreach (var slot in Enum.GetValues<RpiClanRankSlot>().Where(x => x.IsImportable()))
			{
				if (!synonymsBySlot.TryGetValue(slot, out var synonyms))
				{
					synonyms = [];
				}

				var merged = synonyms
					.Concat(displayNamesBySlot.TryGetValue(slot, out var displayNames)
						? displayNames
						: Array.Empty<string>())
					.Append(slot.GenericName())
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();
				synonymsBySlot[slot] = merged;
			}

			definitions.Add(new RpiClanDefinition(
				canonicalAlias,
				fullName,
				headerEntry?.GroupId,
				aliasRecords,
				displayNamesBySlot,
				synonymsBySlot,
				warnings));
		}

		return definitions
			.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static ConvertedClanDefinition ConvertClan(
		RpiClanDefinition clan,
		RpiClanReferenceIndex references)
	{
		var observedSlots = clan.Aliases
			.Select(x => x.Alias)
			.Where(references.ReferencesByAlias.ContainsKey)
			.SelectMany(x => references.ReferencesByAlias[x].ObservedSlots)
			.Where(x => x.IsImportable())
			.Distinct()
			.ToHashSet();

		var importedSlots = DetermineImportedSlots(clan, observedSlots);
		var authoritativePaths = importedSlots
			.Select(x => x.Path())
			.Where(x => x != RpiClanRankPath.Common)
			.Distinct()
			.ToHashSet();
		if (authoritativePaths.Count == 0)
		{
			authoritativePaths = importedSlots.Select(x => x.Path()).Distinct().ToHashSet();
		}

		var pathMaxSlots = authoritativePaths
			.ToDictionary(
				x => x,
				x => importedSlots
					.Where(y => y.Path() == x)
					.OrderBy(y => y.SortOrder())
					.Last());

		var ranks = importedSlots
			.OrderBy(x => x.SortOrder())
			.Select((slot, index) => ConvertRank(clan, slot, index, pathMaxSlots))
			.ToList();

		return new ConvertedClanDefinition(
			clan.CanonicalAlias,
			clan.FullName,
			clan.GroupId,
			clan.Aliases.Where(x => x.IsLegacy).Select(x => x.Alias).ToList(),
			ranks,
			clan.Warnings.Select(x => new RpiClanConversionWarning("source-warning", x)).ToList());
	}

	private static ConvertedClanRankDefinition ConvertRank(
		RpiClanDefinition clan,
		RpiClanRankSlot slot,
		int order,
		IReadOnlyDictionary<RpiClanRankPath, RpiClanRankSlot> pathMaxSlots)
	{
		var displayNames = clan.DisplayNamesBySlot.TryGetValue(slot, out var names)
			? names
			: Array.Empty<string>();
		var primaryName = ChoosePrimaryName(slot, displayNames);
		var alternateNames = displayNames
			.Where(x => !x.Equals(primaryName, StringComparison.OrdinalIgnoreCase))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		var usesCustomName = !primaryName.Equals(slot.GenericName(), StringComparison.OrdinalIgnoreCase) ||
		                     alternateNames.Any(x => !x.Equals(slot.GenericName(), StringComparison.OrdinalIgnoreCase));

		var privileges = (long)(ClanPrivilegeType.CanViewClanStructure |
		                       ClanPrivilegeType.CanViewClanOfficeHolders |
		                       ClanPrivilegeType.CanViewMembers);
		if (slot.IsMilitaryPromotionRank())
		{
			privileges |= (long)(ClanPrivilegeType.CanInduct |
			                     ClanPrivilegeType.CanPromote |
			                     ClanPrivilegeType.CanDemote);
		}

		if (pathMaxSlots.TryGetValue(slot.Path(), out var maxSlot) && maxSlot == slot)
		{
			privileges = (long)ClanPrivilegeType.All;
		}

		var warnings = new List<RpiClanConversionWarning>();
		if (alternateNames.Count > 0)
		{
			warnings.Add(new RpiClanConversionWarning(
				"alternate-rank-names",
				$"Preserved alternate source rank names for {slot.GenericName()}: {string.Join(", ", alternateNames)}."));
		}

		return new ConvertedClanRankDefinition(
			slot,
			slot.Path(),
			order,
			primaryName,
			slot.Path().ToString(),
			privileges,
			usesCustomName,
			alternateNames,
			clan.SynonymsBySlot.TryGetValue(slot, out var synonyms)
				? synonyms
				: Array.Empty<string>(),
			warnings);
	}

	private static IReadOnlyCollection<RpiClanRankSlot> DetermineImportedSlots(
		RpiClanDefinition clan,
		IReadOnlySet<RpiClanRankSlot> observedSlots)
	{
		HashSet<RpiClanRankSlot> imported = [];

		foreach (var path in new[] { RpiClanRankPath.Common, RpiClanRankPath.Military, RpiClanRankPath.Guild })
		{
			var pathSlots = path.SlotsForPath();
			if (pathSlots.Count == 0)
			{
				continue;
			}

			var customSlots = pathSlots
				.Where(slot => HasCustomDisplayName(clan, slot))
				.OrderBy(x => x.SortOrder())
				.ToList();
			if (customSlots.Count > 0)
			{
				var highestCustom = customSlots.Last();
				foreach (var slot in pathSlots.Where(x => x.SortOrder() <= highestCustom.SortOrder()))
				{
					imported.Add(slot);
				}

				continue;
			}

			foreach (var slot in pathSlots.Where(observedSlots.Contains))
			{
				imported.Add(slot);
			}
		}

		return imported.OrderBy(x => x.SortOrder()).ToList();
	}

	private static bool HasCustomDisplayName(RpiClanDefinition clan, RpiClanRankSlot slot)
	{
		if (!clan.DisplayNamesBySlot.TryGetValue(slot, out var names))
		{
			return false;
		}

		return names.Any(x => !x.Equals(slot.GenericName(), StringComparison.OrdinalIgnoreCase));
	}

	private static string ChoosePrimaryName(RpiClanRankSlot slot, IReadOnlyCollection<string> displayNames)
	{
		var generic = slot.GenericName();
		var custom = displayNames.FirstOrDefault(x => !x.Equals(generic, StringComparison.OrdinalIgnoreCase));
		if (!string.IsNullOrWhiteSpace(custom))
		{
			return custom;
		}

		return displayNames.FirstOrDefault() ?? generic;
	}

	private static Dictionary<RpiClanRankSlot, IReadOnlyList<string>> MergeSlotValues(
		IReadOnlyCollection<string> aliases,
		RpiClanSourceDocument sourceDocument,
		bool useDisplayNames)
	{
		var result = new Dictionary<RpiClanRankSlot, IReadOnlyList<string>>();
		foreach (var slot in Enum.GetValues<RpiClanRankSlot>().Where(x => x.IsImportable()))
		{
			var values = aliases
				.Where(sourceDocument.AliasSources.ContainsKey)
				.SelectMany(alias => useDisplayNames
					? sourceDocument.AliasSources[alias].DisplayNamesBySlot.TryGetValue(slot, out var displayNames)
						? displayNames
						: Array.Empty<string>()
					: sourceDocument.AliasSources[alias].SynonymsBySlot.TryGetValue(slot, out var synonyms)
						? synonyms
						: Array.Empty<string>())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (values.Count > 0)
			{
				result[slot] = values;
			}
		}

		return result;
	}

	private static string DetermineAliasSource(
		string alias,
		string canonicalAlias,
		RpiClanHeaderEntry? headerEntry,
		RpiClanSourceDocument sourceDocument,
		RpiClanReferenceIndex references)
	{
		if (alias.Equals(canonicalAlias, StringComparison.OrdinalIgnoreCase))
		{
			return "canonical";
		}

		if (headerEntry is not null && alias.Equals(headerEntry.Alias, StringComparison.OrdinalIgnoreCase))
		{
			return "header-table";
		}

		if (sourceDocument.AliasSources.ContainsKey(alias))
		{
			return "source-code";
		}

		if (references.ReferencesByAlias.ContainsKey(alias))
		{
			return "worldfile-reference";
		}

		return "normalization-rule";
	}

	private static string? DiscoverFullName(string canonicalAlias, RpiClanReferenceIndex references)
	{
		return canonicalAlias switch
		{
			"seekers" => "Seekers",
			"shadow-cult" => "Shadow Cult",
			"tirithguard" => "Minas Tirith Guard",
			"eradan_battalion" => "Eradan Battalion",
			_ => references.ReferencesByAlias.ContainsKey(canonicalAlias)
				? RpiClanRankSlots.TitleCaseAlias(canonicalAlias)
				: null,
		};
	}

	private static CanonicalClanRule ResolveCanonicalRule(string alias)
	{
		return CanonicalRules.TryGetValue(alias, out var rule)
			? rule
			: new CanonicalClanRule(alias, null, Array.Empty<string>());
	}

	private static HashSet<string> BuildImportedAliasSet(IEnumerable<RpiClanDefinition> clans)
	{
		HashSet<string> aliases = [];
		foreach (var clan in clans)
		{
			foreach (var alias in clan.Aliases.Select(x => x.Alias).Append(clan.CanonicalAlias))
			{
				aliases.Add(alias);
			}
		}

		return aliases;
	}
}
