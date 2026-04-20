#nullable enable

using System.IO;

namespace RPI_Engine_Worldfile_Converter;

public enum RPIItemType
{
	Undefined,
	Light,
	Scroll,
	Wand,
	Staff,
	Weapon,
	Shield,
	Missile,
	Treasure,
	Armor,
	Potion,
	Worn,
	Other,
	Trash,
	Trap,
	Container,
	Note,
	Liquid_container,
	Key,
	Food,
	Money,
	Ore,
	Board,
	Fountain,
	Grain,
	Perfume,
	Pottery,
	Salt,
	Zone,
	Plant,
	Component,
	Herb,
	Salve,
	Poison,
	Lockpick,
	Wind_inst,
	Percu_inst,
	String_inst,
	Fur,
	Woodcraft,
	Spice,
	Tool,
	Usury_note,
	Bridle,
	Ticket,
	Skull,
	Dye,
	Cloth,
	Ingot,
	Timber,
	Fluid,
	Liquid_Fuel,
	Remedy,
	Parchment,
	Book,
	Writing_inst,
	Ink,
	Quiver,
	Sheath,
	Keyring,
	Bullet,
	NPC_Object,
	Dwelling,
	Unused,
	Repair,
	Tossable,
	DO_NOT_USE,
	MerchTicket,
	RoomRental,
}

[Flags]
public enum RPIExtraBits : uint
{
	Destroyed = 1u << 0,
	Pitchable = 1u << 1,
	Invisible = 1u << 2,
	Magic = 1u << 3,
	Nodrop = 1u << 4,
	Benign = 1u << 5,
	Getaffect = 1u << 6,
	Dropaffect = 1u << 7,
	Multiaffect = 1u << 8,
	Wearaffect = 1u << 9,
	Wieldaffect = 1u << 10,
	Hitaffect = 1u << 11,
	Ok = 1u << 12,
	NoPurge = 1u << 13,
	ItemLeader = 1u << 14,
	ItemMember = 1u << 15,
	ItemOmni = 1u << 16,
	Illegal = 1u << 17,
	Poisoned = 1u << 18,
	Mask = 1u << 19,
	Mount = 1u << 20,
	Table = 1u << 21,
	Stack = 1u << 22,
	VnpcDwelling = 1u << 23,
	Loads = 1u << 24,
	Variable = 1u << 25,
	Timer = 1u << 26,
	PCSold = 1u << 27,
	Thrown = 1u << 28,
	NewSkills = 1u << 29,
	Pitched = 1u << 30,
	IsVnpc = 1u << 31,
}

[Flags]
public enum RPIWearBits : long
{
	Take = 1 << 0,
	Finger = 1 << 1,
	Neck = 1 << 2,
	Body = 1 << 3,
	Head = 1 << 4,
	Legs = 1 << 5,
	Feet = 1 << 6,
	Hands = 1 << 7,
	Arms = 1 << 8,
	Wshield = 1 << 9,
	About = 1 << 10,
	Waist = 1 << 11,
	Wrist = 1 << 12,
	Wield = 1 << 13,
	Unused1 = 1 << 14,
	Unused2 = 1 << 15,
	Unused3 = 1 << 16,
	Sheath = 1 << 17,
	Belt = 1 << 18,
	Back = 1 << 19,
	Blindfold = 1 << 20,
	Throat = 1 << 21,
	Ears = 1 << 22,
	Shoulder = 1 << 23,
	Ankle = 1 << 24,
	Hair = 1 << 25,
	Face = 1 << 26,
	Armband = 1 << 27,
}

public enum RPIMaterial
{
	None,
	Textile,
	Leather,
	Wood,
	Metal,
	Stone,
	Glass,
	Parchment,
	Liquid,
	Vegetation,
	Ceramic,
	Other,
	Meat,
}

[Flags]
public enum RPIEconFlags
{
	Sinda = 1 << 0,
	Noldo = 1 << 1,
	Orkish = 1 << 2,
	Dwarvish = 1 << 3,
	Beorian = 1 << 4,
	Marachain = 1 << 5,
	Haladin = 1 << 6,
	Generic = 1 << 7,
	Junk = 1 << 8,
	Fine = 1 << 9,
	Poor = 1 << 10,
	Raw = 1 << 11,
	Cooked = 1 << 12,
	Admin1 = 1 << 13,
	BUG = 1 << 14,
	Practice = 1 << 15,
	Used = 1 << 16,
	Admin2 = 1 << 17,
	NoBarter = 1 << 18,
}

public enum RPISkill
{
	None,
	Brawling,
	LightEdge,
	MediumEdge,
	HeavyEdge,
	LightBlunt,
	MediumBlunt,
	HeavyBlunt,
	LightPierce,
	MediumPierce,
	HeavyPierce,
	Staff,
	Polearm,
	Thrown,
	Blowgun,
	Sling,
	Shortbow,
	Longbow,
	Crossbow,
	Dual,
	Block,
	Parry,
	Subdue,
	Disarm,
	Sneak,
	Hide,
	Steal,
	Pick,
	Search,
	Listen,
	Forage,
	Ritual,
	Scan,
	Backstab,
	Barter,
	Ride,
	Climb,
	Swimming,
	Hunt,
	Skin,
	Sail,
	Poisoning,
	Alchemy,
	Herbalism,
	Clairvoyance,
	DangerSense,
	EmpathicHeal,
	Hex,
	MentalBolt,
	Prescience,
	Sensitivity,
	Telepathy,
	Seafaring,
	Dodge,
	Tame,
	Break,
	Metalcraft,
	Woodcraft,
	Textilecraft,
	Cookery,
	Baking,
	Hideworking,
	Stonecraft,
	Candlery,
	Brewing,
	Distilling,
	Literacy,
	Dyecraft,
	Apothecary,
	Glasswork,
	Gemcraft,
	Milling,
	Mining,
	Perfumery,
	Pottery,
	Tracking,
	Farming,
	Healing,
	SpeakAtliduk,
	SpeakAdunaic,
	SpeakHaradaic,
	SpeakWestron,
	SpeakDunael,
	SpeakLabba,
	SpeakNorliduk,
	SpeakRohirric,
	SpeakTalathic,
	SpeakUmitic,
	SpeakNahaiduk,
	SpeakPukael,
	SpeakSindarin,
	SpeakQuenya,
	SpeakSilvan,
	SpeakKhuzdul,
	SpeakOrkish,
	SpeakBlackSpeech,
	ScriptSarati,
	ScriptTengwar,
	ScriptBeleriandTengwar,
	ScriptCerthasDaeron,
	ScriptAngerthasDaeron,
	ScriptQuenyanTengwar,
	ScriptAngerthasMoria,
	ScriptGondorianTengwar,
	ScriptArnorianTengwar,
	ScriptNumenianTengwar,
	ScriptNorthernTengwar,
	ScriptAngerthasErebor,
	BlackWise,
	GreyWise,
	WhiteWise,
	Runecasting,
	Gambling,
	Bonecarving,
	Gardening,
	Sleight,
	Astronomy,
}

public sealed record RpiRawOvalValues(int Oval0, int Oval1, int Oval2, int Oval3, int Oval4, int Oval5)
{
	public IReadOnlyList<int> AsList => [Oval0, Oval1, Oval2, Oval3, Oval4, Oval5];
}

public sealed record RpiNumericTail(
	double Farthings,
	int Clock,
	int MorphTo,
	int ItemWear,
	int RawMaterialValue,
	int Reserved0,
	int Reserved1);

public sealed record RpiItemParseWarning(string Code, string Message, int? LineNumber = null);

public sealed record RpiItemBlockFailure(string SourceFile, int Zone, string? Header, string Message);

public sealed record RpiExtraDescriptionRecord(string Keyword, string Description);

public sealed record RpiAffectRecord(int RawLocation, int Modifier)
{
	public RPISkill? Skill =>
		RawLocation >= 10000 && Enum.IsDefined(typeof(RPISkill), RawLocation - 10000)
			? (RPISkill)(RawLocation - 10000)
			: null;
}

public sealed record RpiClanRecord(string Name, string Rank);

public sealed record RpiPoisonRecord(
	int PoisonType,
	int Duration,
	int Latency,
	int Minute,
	int MaxPower,
	int LevelPower,
	int AtmospherePower,
	int Attack,
	int Decay,
	int Sustain,
	int Release,
	int Uses);

public sealed record RpiWeaponData(
	int Hands,
	int DamageDice,
	int DamageSides,
	RPISkill UseSkill,
	int HitType,
	int DamageBonus,
	bool IsRangedWeapon,
	string? BowType,
	int AccuracyModifier,
	int DamageModifier,
	int Status);

public sealed record RpiArmourData(int ArmourValue, int ArmourType, string? ArmourFamilyName);

public sealed record RpiContainerData(
	int Capacity,
	int Flags,
	int KeyVnum,
	int PickPenalty,
	int ReservedValue,
	int SpecialValue,
	bool IsTableContainer);

public sealed record RpiLightData(int Capacity, int Hours, int LiquidValue, bool IsLit);

public sealed record RpiDrinkContainerData(int Capacity, int Volume, int LiquidValue, bool IsInfiniteSource);

public sealed record RpiFoodData(
	int FoodValue,
	int SpellOne,
	int SpellTwo,
	int SpellThree,
	int SpellFour,
	int Bites);

public sealed record RpiRepairKitData(
	int UsesRemaining,
	int MendingBonus,
	int RequiredSkillValue,
	int ToolSkillValue,
	int RepairItemTypeValue);

public sealed record RpiKeyData(int KeyType, int KeyedToVnum);

public sealed record RpiWritingData(int Pages, int PageCapacityHint, bool IsBook);

public sealed record RpiAmmoData(int DamageDice, int DamageSides, bool IsBullet);

public sealed record RpiItemRecord
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string RawName { get; init; }
	public required IReadOnlyList<string> NameKeywords { get; init; }
	public required string ShortDescription { get; init; }
	public required string LongDescription { get; init; }
	public required string FullDescription { get; init; }
	public required RPIItemType ItemType { get; init; }
	public required RPIExtraBits ExtraBits { get; init; }
	public required RPIWearBits WearBits { get; init; }
	public required RpiRawOvalValues RawOvals { get; init; }
	public required IReadOnlyList<int> RawStateValues { get; init; }
	public required IReadOnlyList<double> RawTailValues { get; init; }
	public required int Weight { get; init; }
	public required double SilverValue { get; init; }
	public required int RoomPosition { get; init; }
	public required int Activation { get; init; }
	public required int Quality { get; init; }
	public required long RawEconFlags { get; init; }
	public required int Size { get; init; }
	public required int Count { get; init; }
	public required RpiNumericTail NumericTail { get; init; }
	public required RPIMaterial InferredMaterial { get; init; }
	public string? QualityKeyword { get; init; }
	public string? DescKeys { get; init; }
	public string? InkColour { get; init; }
	public IReadOnlyList<RpiExtraDescriptionRecord> ExtraDescriptions { get; init; } = Array.Empty<RpiExtraDescriptionRecord>();
	public IReadOnlyList<RpiAffectRecord> Affects { get; init; } = Array.Empty<RpiAffectRecord>();
	public IReadOnlyList<RpiClanRecord> Clans { get; init; } = Array.Empty<RpiClanRecord>();
	public IReadOnlyList<RpiPoisonRecord> Poisons { get; init; } = Array.Empty<RpiPoisonRecord>();
	public IReadOnlyList<string> LegacyTrailerLines { get; init; } = Array.Empty<string>();
	public IReadOnlyList<RpiItemParseWarning> ParseWarnings { get; init; } = Array.Empty<RpiItemParseWarning>();
	public RpiWeaponData? WeaponData { get; init; }
	public RpiArmourData? ArmourData { get; init; }
	public RpiContainerData? ContainerData { get; init; }
	public RpiLightData? LightData { get; init; }
	public RpiDrinkContainerData? DrinkContainerData { get; init; }
	public RpiFoodData? FoodData { get; init; }
	public RpiRepairKitData? RepairKitData { get; init; }
	public RpiKeyData? KeyData { get; init; }
	public RpiWritingData? WritingData { get; init; }
	public RpiAmmoData? AmmoData { get; init; }

	public string SourceKey => $"{Path.GetFileName(SourceFile)}#{Vnum}";

	public override string ToString()
	{
		return $"{SourceKey} {ShortDescription}";
	}
}

public sealed record RpiParsedCorpus(
	IReadOnlyList<RpiItemRecord> Items,
	IReadOnlyList<RpiItemBlockFailure> Failures);
