#nullable enable

using System.Globalization;

namespace RPI_Engine_Worldfile_Converter;

public enum RpiClanRankSlot
{
	Membership,
	Recruit,
	Private,
	Corporal,
	Sergeant,
	Lieutenant,
	Captain,
	General,
	Commander,
	Apprentice,
	Journeyman,
	Master,
	Leadership,
	MemberObject,
	LeaderObject,
}

public enum RpiClanRankPath
{
	Common,
	Military,
	Guild,
	Ignored,
}

public sealed record RpiClanHeaderEntry(int GroupId, string FullName, string Alias);

public sealed record RpiClanAliasSource(
	string Alias,
	IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> DisplayNamesBySlot,
	IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> SynonymsBySlot);

public sealed record RpiClanSourceDocument(
	string SourceFile,
	IReadOnlyList<RpiClanHeaderEntry> HeaderEntries,
	IReadOnlyDictionary<string, RpiClanAliasSource> AliasSources,
	IReadOnlyList<string> ParseWarnings);

public sealed record RpiClanAliasRecord(string Alias, bool IsCanonical, bool IsLegacy, string Source);

public sealed record RpiClanDefinition(
	string CanonicalAlias,
	string FullName,
	int? GroupId,
	IReadOnlyList<RpiClanAliasRecord> Aliases,
	IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> DisplayNamesBySlot,
	IReadOnlyDictionary<RpiClanRankSlot, IReadOnlyList<string>> SynonymsBySlot,
	IReadOnlyList<string> Warnings)
{
	public string SourceKey => CanonicalAlias;
}

public sealed record RpiClanReferenceRecord(
	string Alias,
	int AliasReferenceCount,
	IReadOnlyCollection<RpiClanRankSlot> ObservedSlots,
	IReadOnlyList<string> Samples);

public sealed record RpiClanReferenceIndex(
	IReadOnlyDictionary<string, RpiClanReferenceRecord> ReferencesByAlias);

public sealed record RpiClanConversionWarning(string Code, string Message);

public sealed record ConvertedClanRankDefinition(
	RpiClanRankSlot Slot,
	RpiClanRankPath Path,
	int Order,
	string Name,
	string RankPath,
	long Privileges,
	bool UsesCustomName,
	IReadOnlyList<string> AlternateNames,
	IReadOnlyList<string> Synonyms,
	IReadOnlyList<RpiClanConversionWarning> Warnings);

public sealed record ConvertedClanDefinition(
	string CanonicalAlias,
	string FullName,
	int? GroupId,
	IReadOnlyList<string> LegacyAliases,
	IReadOnlyList<ConvertedClanRankDefinition> Ranks,
	IReadOnlyList<RpiClanConversionWarning> Warnings)
{
	public string SourceKey => CanonicalAlias;
}

public sealed record ClanConversionResult(
	IReadOnlyList<RpiClanDefinition> SourceClans,
	IReadOnlyList<ConvertedClanDefinition> ConvertedClans,
	IReadOnlyDictionary<string, int> UnresolvedAliasCounts);

public sealed record ClanAnalysisSummary(
	string SourceFile,
	int SourceClanCount,
	int ImportedClanCount,
	int ImportedRankCount,
	string BaselineStatus,
	IReadOnlyDictionary<string, int> PerClanRankCounts,
	IReadOnlyDictionary<string, int> PerPathCounts,
	IReadOnlyDictionary<string, int> PerSlotCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> UnresolvedAliasCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record ConverterExportClan(RpiClanDefinition Source, ConvertedClanDefinition Converted);

public sealed record ClanExportReport(
	DateTime GeneratedUtc,
	string SourceFile,
	string RegionsDirectory,
	ClanAnalysisSummary Analysis,
	IReadOnlyList<string> ParseWarnings,
	IReadOnlyList<FutureMudClanValidationIssue> ValidationIssues,
	IReadOnlyList<ConverterExportClan> Clans);

public static class RpiClanRankSlots
{
	private static readonly IReadOnlyDictionary<RpiClanRankSlot, string> GenericNames =
		new Dictionary<RpiClanRankSlot, string>
		{
			[RpiClanRankSlot.Membership] = "Membership",
			[RpiClanRankSlot.Recruit] = "Recruit",
			[RpiClanRankSlot.Private] = "Private",
			[RpiClanRankSlot.Corporal] = "Corporal",
			[RpiClanRankSlot.Sergeant] = "Sergeant",
			[RpiClanRankSlot.Lieutenant] = "Lieutenant",
			[RpiClanRankSlot.Captain] = "Captain",
			[RpiClanRankSlot.General] = "General",
			[RpiClanRankSlot.Commander] = "Commander",
			[RpiClanRankSlot.Apprentice] = "Apprentice",
			[RpiClanRankSlot.Journeyman] = "Journeyman",
			[RpiClanRankSlot.Master] = "Master",
			[RpiClanRankSlot.Leadership] = "Leader",
			[RpiClanRankSlot.MemberObject] = "Member Object",
			[RpiClanRankSlot.LeaderObject] = "Leader Object",
		};

	private static readonly IReadOnlyList<RpiClanRankSlot> OrderedImportableSlots =
	[
		RpiClanRankSlot.Membership,
		RpiClanRankSlot.Recruit,
		RpiClanRankSlot.Private,
		RpiClanRankSlot.Corporal,
		RpiClanRankSlot.Sergeant,
		RpiClanRankSlot.Lieutenant,
		RpiClanRankSlot.Captain,
		RpiClanRankSlot.General,
		RpiClanRankSlot.Commander,
		RpiClanRankSlot.Apprentice,
		RpiClanRankSlot.Journeyman,
		RpiClanRankSlot.Master,
		RpiClanRankSlot.Leadership,
	];

	public static IReadOnlyList<RpiClanRankSlot> CommonSlots { get; } =
	[
		RpiClanRankSlot.Membership,
		RpiClanRankSlot.Leadership,
	];
	public static IReadOnlyList<RpiClanRankSlot> MilitarySlots { get; } =
	[
		RpiClanRankSlot.Recruit,
		RpiClanRankSlot.Private,
		RpiClanRankSlot.Corporal,
		RpiClanRankSlot.Sergeant,
		RpiClanRankSlot.Lieutenant,
		RpiClanRankSlot.Captain,
		RpiClanRankSlot.General,
		RpiClanRankSlot.Commander,
	];

	public static IReadOnlyList<RpiClanRankSlot> GuildSlots { get; } =
	[
		RpiClanRankSlot.Apprentice,
		RpiClanRankSlot.Journeyman,
		RpiClanRankSlot.Master,
	];

	public static string GenericName(this RpiClanRankSlot slot)
	{
		return GenericNames[slot];
	}

	public static RpiClanRankPath Path(this RpiClanRankSlot slot)
	{
		return slot switch
		{
			RpiClanRankSlot.Membership or
			RpiClanRankSlot.Leadership => RpiClanRankPath.Common,
			RpiClanRankSlot.Recruit or
			RpiClanRankSlot.Private or
			RpiClanRankSlot.Corporal or
			RpiClanRankSlot.Sergeant or
			RpiClanRankSlot.Lieutenant or
			RpiClanRankSlot.Captain or
			RpiClanRankSlot.General or
			RpiClanRankSlot.Commander => RpiClanRankPath.Military,
			RpiClanRankSlot.Apprentice or
			RpiClanRankSlot.Journeyman or
			RpiClanRankSlot.Master => RpiClanRankPath.Guild,
			_ => RpiClanRankPath.Ignored,
		};
	}

	public static bool IsImportable(this RpiClanRankSlot slot)
	{
		return slot.Path() != RpiClanRankPath.Ignored;
	}

	public static int SortOrder(this RpiClanRankSlot slot)
	{
		for (var i = 0; i < OrderedImportableSlots.Count; i++)
		{
			if (OrderedImportableSlots[i] == slot)
			{
				return i;
			}
		}

		return int.MaxValue;
	}

	public static bool IsMilitaryPromotionRank(this RpiClanRankSlot slot)
	{
		return slot is RpiClanRankSlot.Corporal or
			RpiClanRankSlot.Sergeant or
			RpiClanRankSlot.Lieutenant or
			RpiClanRankSlot.Captain or
			RpiClanRankSlot.General or
			RpiClanRankSlot.Commander;
	}

	public static IReadOnlyList<RpiClanRankSlot> SlotsForPath(this RpiClanRankPath path)
	{
		return path switch
		{
			RpiClanRankPath.Common => CommonSlots,
			RpiClanRankPath.Military => MilitarySlots,
			RpiClanRankPath.Guild => GuildSlots,
			_ => Array.Empty<RpiClanRankSlot>(),
		};
	}

	public static bool TryParseFlag(string? flagName, out RpiClanRankSlot slot)
	{
		slot = RpiClanRankSlot.Membership;
		if (string.IsNullOrWhiteSpace(flagName))
		{
			return false;
		}

		switch (flagName.Trim().ToUpperInvariant())
		{
			case "CLAN_MEMBER":
				slot = RpiClanRankSlot.Membership;
				return true;
			case "CLAN_LEADER":
				slot = RpiClanRankSlot.Leadership;
				return true;
			case "CLAN_MEMBER_OBJ":
				slot = RpiClanRankSlot.MemberObject;
				return true;
			case "CLAN_LEADER_OBJ":
				slot = RpiClanRankSlot.LeaderObject;
				return true;
			case "CLAN_RECRUIT":
				slot = RpiClanRankSlot.Recruit;
				return true;
			case "CLAN_PRIVATE":
				slot = RpiClanRankSlot.Private;
				return true;
			case "CLAN_CORPORAL":
				slot = RpiClanRankSlot.Corporal;
				return true;
			case "CLAN_SERGEANT":
				slot = RpiClanRankSlot.Sergeant;
				return true;
			case "CLAN_LIEUTENANT":
				slot = RpiClanRankSlot.Lieutenant;
				return true;
			case "CLAN_CAPTAIN":
				slot = RpiClanRankSlot.Captain;
				return true;
			case "CLAN_GENERAL":
				slot = RpiClanRankSlot.General;
				return true;
			case "CLAN_COMMANDER":
				slot = RpiClanRankSlot.Commander;
				return true;
			case "CLAN_APPRENTICE":
				slot = RpiClanRankSlot.Apprentice;
				return true;
			case "CLAN_JOURNEYMAN":
				slot = RpiClanRankSlot.Journeyman;
				return true;
			case "CLAN_MASTER":
				slot = RpiClanRankSlot.Master;
				return true;
			default:
				return false;
		}
	}

	public static string TitleCaseAlias(string alias)
	{
		var textInfo = CultureInfo.InvariantCulture.TextInfo;
		return string.Join(
			" ",
			ExpandAliasTokens(alias
				.Replace('_', ' ')
				.Replace('-', ' ')
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			.Select((x, index) => TitleCaseAliasWord(textInfo, x, index)));
	}

	private static string TitleCaseAliasWord(TextInfo textInfo, string word, int index)
	{
		if (index > 0 && word is "and" or "of")
		{
			return word;
		}

		return textInfo.ToTitleCase(word.ToLowerInvariant());
	}

	private static IEnumerable<string> ExpandAliasTokens(IReadOnlyList<string> tokens)
	{
		for (var i = 0; i < tokens.Count; i++)
		{
			var token = tokens[i].ToLowerInvariant();
			if (i + 1 < tokens.Count &&
			    token == "m" &&
			    tokens[i + 1].Equals("t", StringComparison.OrdinalIgnoreCase))
			{
				yield return "minas";
				yield return "tirith";
				i++;
				continue;
			}

			if (i + 1 < tokens.Count &&
			    token == "m" &&
			    tokens[i + 1].Equals("m", StringComparison.OrdinalIgnoreCase))
			{
				yield return "minas";
				yield return "morgul";
				i++;
				continue;
			}

			foreach (var expanded in ExpandAliasToken(token))
			{
				yield return expanded;
			}
		}
	}

	private static IEnumerable<string> ExpandAliasToken(string token)
	{
		return token switch
		{
			"mt" => ["minas", "tirith"],
			"osgi" => ["osgiliath"],
			"mm" => ["minas", "morgul"],
			"te" => ["tur", "edendor"],
			"bn" => ["black", "numenorean"],
			"com" => ["cult", "of", "morgoth"],
			"fj" => ["fahad", "jafari"],
			"sak" => ["saklithan"],
			"hd" => ["hawk", "and", "dove"],
			_ => [token],
		};
	}
}
