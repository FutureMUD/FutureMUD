#nullable enable

using System.IO;
using MudSharp.Form.Shape;

namespace RPI_Engine_Worldfile_Converter;

[Flags]
public enum RpiNpcActFlags : long
{
	None = 0,
	Memory = 1L << 0,
	Sentinel = 1L << 1,
	NoVnpcBuyers = 1L << 4,
	Aggressive = 1L << 5,
	BulkTrader = 1L << 9,
	Enforcer = 1L << 12,
	PackAnimal = 1L << 13,
	Vehicle = 1L << 14,
	Mount = 1L << 18,
	Venom = 1L << 19,
	Wildlife = 1L << 21,
	StayPut = 1L << 22,
}

[Flags]
public enum RpiNpcFlags : long
{
	None = 0,
	Keeper = 1L << 0,
	Leader1 = 1L << 9,
	Leader2 = 1L << 10,
	Variable = 1L << 28,
}

public enum RpiNpcSex
{
	Neutral = 0,
	Male = 1,
	Female = 2,
}

public enum RpiNpcClassification
{
	Standard,
	Helper,
	Placeholder,
	Test,
	Animal,
	Merchant,
	Vehicle,
}

public enum NpcConversionStatus
{
	Ready,
	Deferred,
	Unresolved,
}

public enum NpcTemplateKind
{
	Simple,
	Variable,
}

public sealed record RpiNpcParseWarning(string Code, string Message, int? LineNumber = null);

public sealed record RpiNpcBlockFailure(string SourceFile, int Zone, string? Header, string Message);

public sealed record RpiNpcSkillRecord(
	int SkillId,
	string SkillName,
	int Value,
	bool IsSpokenLanguage);

public sealed record RpiNpcClanMembershipRecord(
	string RankName,
	string ClanAlias);

public sealed record RpiNpcShopRecord(
	int ShopVnum,
	int StoreVnum,
	double Markup,
	double Discount,
	double EconMarkup1,
	double EconDiscount1,
	int EconFlags1,
	IReadOnlyList<string> AdditionalEconomyValues,
	IReadOnlyList<int> DeliveryVnums,
	IReadOnlyList<int> TradesIn);

public sealed record RpiNpcVenomRecord(
	int PoisonType,
	int Duration,
	int Latency,
	int Minute,
	int MaxPower,
	int LvlPower,
	int AtmPower,
	int Attack,
	int Decay,
	int Sustain,
	int Release,
	int Uses);

public sealed record RpiNpcMorphRecord(
	int Clock,
	int MorphTo,
	int MorphType);

public sealed record RpiNpcRecord
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string Keywords { get; init; }
	public required string ShortDescription { get; init; }
	public required string LongDescription { get; init; }
	public required string FullDescription { get; init; }
	public required long RawActFlags { get; init; }
	public required RpiNpcActFlags ActFlags { get; init; }
	public required long RawAffectedBy { get; init; }
	public required int Offense { get; init; }
	public required int LegacyRaceId { get; init; }
	public required int Armour { get; init; }
	public required string HitDiceExpression { get; init; }
	public required string DamageDiceExpression { get; init; }
	public required long BirthTimestamp { get; init; }
	public required int Position { get; init; }
	public required int DefaultPosition { get; init; }
	public required RpiNpcSex Sex { get; init; }
	public required int MerchSeven { get; init; }
	public required long MaterialsMask { get; init; }
	public required long VehicleType { get; init; }
	public required long BuyFlags { get; init; }
	public required int SkinnedVnum { get; init; }
	public required int Circle { get; init; }
	public required int Cell1 { get; init; }
	public required int CarcassVnum { get; init; }
	public required int Cell2 { get; init; }
	public required int PPoints { get; init; }
	public required int NaturalDelay { get; init; }
	public required int HelmRoom { get; init; }
	public required int BodyType { get; init; }
	public required int PoisonType { get; init; }
	public required int NaturalAttackType { get; init; }
	public required int AccessFlags { get; init; }
	public required int HeightInches { get; init; }
	public required int Frame { get; init; }
	public required int NoAccessFlags { get; init; }
	public required int Cell3 { get; init; }
	public required int RoomPos { get; init; }
	public required int Fallback { get; init; }
	public required int Strength { get; init; }
	public required int Intelligence { get; init; }
	public required int Will { get; init; }
	public required int Aura { get; init; }
	public required int Dexterity { get; init; }
	public required int Constitution { get; init; }
	public required int SpeaksSkillId { get; init; }
	public required int Agility { get; init; }
	public required long RawFlags { get; init; }
	public required RpiNpcFlags Flags { get; init; }
	public required int CurrencyType { get; init; }
	public required IReadOnlyList<RpiNpcSkillRecord> Skills { get; init; }
	public required IReadOnlyList<RpiNpcClanMembershipRecord> ClanMemberships { get; init; }
	public RpiNpcShopRecord? Shop { get; init; }
	public RpiNpcVenomRecord? Venom { get; init; }
	public RpiNpcMorphRecord? Morph { get; init; }
	public IReadOnlyList<RpiNpcParseWarning> ParseWarnings { get; init; } = Array.Empty<RpiNpcParseWarning>();

	public string SourceKey => $"{Path.GetFileName(SourceFile)}#{Vnum}";
}

public sealed record RpiParsedNpcCorpus(
	IReadOnlyList<RpiNpcRecord> Npcs,
	IReadOnlyList<RpiNpcBlockFailure> Failures);

public sealed record NpcConversionWarning(string Code, string Message);

public sealed record NpcGenderChance(
	Gender Gender,
	int Chance);

public sealed record NpcAttributeValue(
	string TraitName,
	double Value,
	string SourceAlias);

public sealed record NpcTraitValue(
	string TraitName,
	double Value,
	int SourceSkillId,
	string SourceSkillName,
	bool IsLanguage,
	bool IsPrimarySpokenLanguage,
	bool Resolved);

public sealed record ConvertedNpcDefinition
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string SourceKey { get; init; }
	public required string ZoneGroupKey { get; init; }
	public required string ZoneName { get; init; }
	public required NpcConversionStatus Status { get; init; }
	public required NpcTemplateKind TemplateKind { get; init; }
	public required RpiNpcClassification Classification { get; init; }
	public required string TemplateName { get; init; }
	public required string Keywords { get; init; }
	public required string ShortDescription { get; init; }
	public required string LongDescription { get; init; }
	public required string FullDescription { get; init; }
	public required string? RaceName { get; init; }
	public required string? EthnicityName { get; init; }
	public required string? CultureName { get; init; }
	public required string ResolutionEvidence { get; init; }
	public required string TechnicalName { get; init; }
	public required bool UsesSourceDerivedName { get; init; }
	public required Gender SimpleGender { get; init; }
	public required IReadOnlyList<NpcGenderChance> GenderChances { get; init; }
	public required double HeightMetres { get; init; }
	public required double WeightKilograms { get; init; }
	public required string BirthdayDate { get; init; }
	public required IReadOnlyList<NpcAttributeValue> Attributes { get; init; }
	public required IReadOnlyList<NpcTraitValue> Traits { get; init; }
	public required string? SpokenLanguageTraitName { get; init; }
	public required IReadOnlyList<string> ArtificialIntelligenceNames { get; init; }
	public required IReadOnlyList<string> DeferredBehaviorFlags { get; init; }
	public required bool HasShopData { get; init; }
	public required bool HasVenomData { get; init; }
	public required bool HasMorphData { get; init; }
	public required bool HasClanMemberships { get; init; }
	public required IReadOnlyList<NpcConversionWarning> Warnings { get; init; }
	public required long RawActFlags { get; init; }
	public required long RawFlags { get; init; }
	public required int LegacyRaceId { get; init; }
	public required int LegacyArmour { get; init; }
	public required int LegacyHeightInches { get; init; }
	public required int LegacyFrame { get; init; }
	public required long BirthTimestamp { get; init; }
	public required int NaturalDelay { get; init; }
	public required int PoisonType { get; init; }
	public required int NaturalAttackType { get; init; }
	public RpiNpcShopRecord? Shop { get; init; }
	public RpiNpcVenomRecord? Venom { get; init; }
	public RpiNpcMorphRecord? Morph { get; init; }
	public IReadOnlyList<RpiNpcClanMembershipRecord> ClanMemberships { get; init; } = Array.Empty<RpiNpcClanMembershipRecord>();
}

public sealed record NpcConversionResult(
	IReadOnlyList<ConvertedNpcDefinition> Npcs,
	IReadOnlyDictionary<string, int> DeferredReasonCounts,
	IReadOnlyDictionary<string, int> UnresolvedRaceCounts,
	IReadOnlyDictionary<string, int> UnresolvedCultureCounts);

public sealed record NpcAnalysisSummary(
	int TotalMobCount,
	int ParsedMobCount,
	int FailureCount,
	int ParseWarningCount,
	string BaselineStatus,
	int SimpleTemplateCount,
	int VariableTemplateCount,
	int DeferredTemplateCount,
	int ResolvedRaceCount,
	int ResolvedEthnicityCount,
	int ResolvedCultureCount,
	IReadOnlyDictionary<string, int> ClassificationCounts,
	IReadOnlyDictionary<string, int> StatusCounts,
	IReadOnlyDictionary<string, int> FeatureCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record ConverterExportNpc(RpiNpcRecord Source, ConvertedNpcDefinition Converted);

public sealed record NpcExportAuditEntry(
	string SourceKey,
	int Vnum,
	NpcConversionStatus Status,
	NpcTemplateKind TemplateKind,
	RpiNpcClassification Classification,
	string TemplateName,
	string? RaceName,
	string? EthnicityName,
	string? CultureName,
	IReadOnlyList<string> ArtificialIntelligenceNames,
	IReadOnlyList<string> WarningCodes);

public sealed record NpcExportAuditReport(
	DateTime GeneratedUtc,
	IReadOnlyList<NpcExportAuditEntry> Npcs);

public sealed record NpcExportReport(
	DateTime GeneratedUtc,
	string SourceDirectory,
	NpcAnalysisSummary Analysis,
	IReadOnlyList<RpiNpcBlockFailure> Failures,
	IReadOnlyList<FutureMudNpcValidationIssue> ValidationIssues,
	IReadOnlyList<ConverterExportNpc> Npcs);
