#nullable enable

using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Character;

public static partial class NpcArchiveReferencePolicy
{
	/// <summary>AI codecs are selected by the persisted loader discriminator, never by the shared XML root.</summary>
	public static bool HasReferenceOrUncertainty(Db.ArtificialIntelligence row, long characterId, long bodyId,
		out string diagnostic, params long[] additionalPhysicalIds)
	{
		diagnostic = string.Empty;
		var schema = row.Type switch
		{
			"Judge" => JudgeSchema,
			"Mount" => MountSchema,
			"Animal" => AnimalSchema,
			"Monster" => MonsterSchema,
			_ => null
		};
		if (schema is null)
		{
			return HasReferenceOrUncertainty(row.Definition, characterId, bodyId, additionalPhysicalIds);
		}

		try
		{
			if (!string.IsNullOrWhiteSpace(row.Definition) && schema(XElement.Parse(row.Definition))) return false;
		}
		catch (XmlException) { }
		catch (OverflowException) { } // The native dice validator parses bounded integer tokens.
		catch (FormatException) { }
		diagnostic = $"Unrecognized or malformed {row.Type} AI definition contract";
		return true;
	}

	// These exact loaders store static definitions/progs and tuning or narrative values. Actors are runtime inputs.
	// Every container and scalar is checked, including values without a colliding physical ID.
	private static readonly Func<XElement, bool> JudgeSchema = AiSchema("Definition", [], AiFields(
		("IdentityProg WarnEchoProg WarnStartMoveEchoProg FailToComplyEchoProg ThrowInPrisonEchoProg", AiProgNameOrId),
		("IntroductionDelay ChargesDelay PleaDelay CaseDelayPerCrime ClosingArgumentDelay VerdictDelay SentencingDelay", AiSeconds),
		("TrialIntroductionEmote TrialChargesEmote TrialPleaEmote TrialDefaultPleaEnteredEmote TrialCaseEmote " +
		 "TrialClosingArgumentsEmote TrialEndArgumentsEmote TrialVerdictGuiltyEmote TrialVerdictNotGuiltyEmote " +
		 "TrialSentencingEmote TrialEndFreeToGo TrialEndRemandedIntoCustody TrialEndRemandedAwaitingExecution " +
		 "AlertEmote DistantAlertEmote", AiText)),
		required: "IntroductionDelay ChargesDelay PleaDelay CaseDelayPerCrime ClosingArgumentDelay VerdictDelay SentencingDelay " +
		          "TrialIntroductionEmote TrialChargesEmote TrialPleaEmote TrialDefaultPleaEnteredEmote TrialCaseEmote " +
		          "TrialClosingArgumentsEmote TrialEndArgumentsEmote TrialVerdictGuiltyEmote TrialVerdictNotGuiltyEmote " +
		          "TrialSentencingEmote TrialEndFreeToGo TrialEndRemandedIntoCustody TrialEndRemandedAwaitingExecution");

	private static readonly Func<XElement, bool> MountSchema = AiSchema("Definition", [], AiFields(
		("PermitRiderProg PermitControlProg WhyCannotPermitRiderProg", AiIdentifier),
		("MountNonConsensualMountDifficulty MountControlDifficulty MountResistBuckDifficulty", AiDifficulty),
		("MaximumNumberOfRiders", AiPositiveInteger),
		("RawMountEmote RawDismountEmote RawControlDeniedEmote RawBuckEmote", AiText)),
		required: "PermitRiderProg PermitControlProg WhyCannotPermitRiderProg MountNonConsensualMountDifficulty " +
		          "MountControlDifficulty MountResistBuckDifficulty MaximumNumberOfRiders RawMountEmote RawDismountEmote " +
		          "RawControlDeniedEmote RawBuckEmote");

	private static readonly Func<XElement, bool> MovementSchema = AiSchema("Movement",
		[("type", EnumValue<AnimalMovementStrategyType>)], AiFields(
			("Range", AiInteger), ("AmphibiousWaterBias WanderChancePerMinute", AiNumber), ("WanderEmote", AiText),
			("MovementEnabledProg MovementCellProg PreferredHabitatProg ToleratedHabitatProg AmphibiousLandCellProg " +
			 "AmphibiousWaterCellProg AllowDescentProg", AiIdentifier),
			("TargetFlyingLayer TargetRestingLayer PreferredTreeLayer SecondaryTreeLayer", EnumValue<RoomLayer>)));

	private static readonly Func<XElement, bool> HomeSchema = AiSchema("Home",
		[("type", EnumValue<AnimalHomeStrategyType>)], AiFields(
			("SuitableTerritoryProg DesiredTerritorySizeProg BurrowCraftId BurrowSiteProg BuildEnabledProg HomeLocationProg AnchorItemProg", AiIdentifier),
			("WillShareTerritory WillShareTerritoryWithOtherRaces AllowGroupShelterSharing", AiBoolean)));

	private static readonly Func<XElement, bool> AwarenessSchema = AiSchema("Awareness",
		[("type", EnumValue<AnimalAwarenessStrategyType>)], AiFields(
			("ThreatProg AvoidCellProg", AiIdentifier), ("Range MemoryMinutes", AiInteger),
			("Senses", EnumValue<AnimalSensesStrategyType>)));

	private static readonly Func<XElement, bool> RefugeSchema = AiSchema("Refuge",
		[("type", EnumValue<AnimalRefugeStrategyType>)], AiFields(
			("Layer", EnumValue<RoomLayer>), ("CellProg", AiIdentifier), ("ReturnSeconds", AiInteger)));

	private static readonly Func<XElement, bool> HuntingSchema = AiHuntingSchema();

	private static readonly Func<XElement, bool> FeedingSchema = AiSchema("Feeding",
		[("type", EnumValue<AnimalFeedingStrategyType>)], AiFields(
			("WillAttackProg", AiIdentifier), ("UseActiveNeeds", AiBoolean),
			("EngageDelayDiceExpression", AiDice), ("EngageEmote", AiText)));

	private static readonly Func<XElement, bool> WaterSchema = AiSchema("Water",
		[("type", EnumValue<AnimalWaterStrategyType>), ("enabled", AiBoolean)], []);

	private static readonly Func<XElement, bool> ThreatSchema = AiSchema("Threat",
		[("type", EnumValue<AnimalThreatStrategyType>)], AiFields(
			("OrdinaryResponse HungryPreyResponse AttackedResponse TerritoryResponse ParentingResponse SeasonalResponse", EnumValue<AnimalThreatResponseType>),
			("PostureEmote", AiText), ("PostureDurationDiceExpression", AiDice)));

	private static readonly Func<XElement, bool> ActivitySchema = AiSchema("Activity",
		[("type", EnumValue<AnimalActivityStrategyType>)], AiFields(
			("SleepEnabled", AiBoolean), ("DormancyMode", EnumValue<AnimalDormancyMode>), ("RestEmote", AiText),
			("DormantSeasonGroup AggressiveSeasonGroup NestingSeasonGroup", AiName), ("ActiveTime", EnumValue<TimeOfDay>)),
		repeated: "DormantSeasonGroup AggressiveSeasonGroup NestingSeasonGroup ActiveTime");

	private static readonly Func<XElement, bool> EcologySchema = AiEcologySchema();
	private static readonly Func<XElement, bool> WindowSchema = AiWindowSchema();
	private static readonly Func<XElement, bool> MonsterOptionsSchema = AiMonsterOptionsSchema();
	private static readonly Func<XElement, bool> AnimalSchema = AiCreatureSchema(false);
	private static readonly Func<XElement, bool> MonsterSchema = AiCreatureSchema(true);

	private static Func<XElement, bool> AiCreatureSchema(bool monster)
	{
		var fields = AiFields(("OpenDoors UseKeys SmashLockedDoors CloseDoorsBehind UseDoorguards MoveEvenIfObstructionInWay", AiBoolean),
			("DoorSmashDelayProg", AiIdentifier));
		fields.Add("Movement", MovementSchema); fields.Add("Home", HomeSchema);
		fields.Add("Awareness", AwarenessSchema); fields.Add("Refuge", RefugeSchema); fields.Add("Hunting", HuntingSchema);
		if (monster) fields.Add("Monster", MonsterOptionsSchema);
		else
		{
			fields.Add("Feeding", FeedingSchema); fields.Add("Water", WaterSchema); fields.Add("Threat", ThreatSchema);
			fields.Add("Activity", ActivitySchema); fields.Add("Ecology", EcologySchema);
		}
		return AiSchema("Definition", [], fields);
	}

	private static Func<XElement, bool> AiHuntingSchema()
	{
		var fields = AiFields(("People", EnumValue<AnimalPeoplePreyPolicy>), ("Selection", EnumValue<AnimalPreySelection>),
			("Opening", EnumValue<AnimalHuntOpening>), ("Followup", EnumValue<AnimalHuntFollowup>),
			("PreferredLayer", x => x.Length == 0 || EnumValue<RoomLayer>(x)), ("Opportunistic", AiBoolean),
			("Engage Abandon Starvation Confidence Range TimeoutSeconds LostSeconds", AiNumber),
			("MinimumSize MaximumSize", AiNullableInteger), ("ClassificationProg EligibilityProg PreferenceProg", AiIdentifier));
		foreach (var name in new[] { "Include", "Exclude" })
		{
			fields.Add(name, AiUniqueList(name, "Race", [], element => AiLeaf(element, AiPositiveIdentifier),
				x => long.Parse(x.Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)));
		}
		fields.Add("Prefer", AiUniqueList("Prefer", "Race", ["id"],
			x => AiLeaf(x, AiNumber, "id") && AiPositiveIdentifier((string?)x.Attribute("id") ?? ""),
			x => long.Parse(x.Attribute("id")!.Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)));
		string[] weights = ["size", "injury", "vulnerability", "tactic", "support", "weapons", "owninjury", "fatigue"];
		fields.Add("Weights", AiUniqueList("Weights", "Weight", ["name"],
			x => AiLeaf(x, AiNumber, "name") && weights.Contains((string?)x.Attribute("name"), StringComparer.OrdinalIgnoreCase),
			x => x.Attribute("name")!.Value));
		return AiSchema("Hunting", [("version", AiVersion), ("enabled", AiBoolean)], fields);
	}

	private static Func<XElement, bool> AiEcologySchema()
	{
		var fields = AiFields(("ShelterEnabled SeasonalEnabled NestingEnabled ParentingEnabled", AiBoolean),
			("ShelterNeededProg ShelterCellProg SeasonalCellProg NestSiteProg ProtectProg", AiIdentifier));
		fields.Add("SeasonalHabitat", x => AiLeaf(x, AiIdentifier, "seasonGroup") && AiName((string?)x.Attribute("seasonGroup") ?? ""));
		var schema = AiSchema("Ecology", [], fields, repeated: "SeasonalHabitat");
		return x => schema(x) && x.Elements("SeasonalHabitat").Select(y => y.Attribute("seasonGroup")!.Value.Trim())
			.Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Elements("SeasonalHabitat").Count();
	}

	private static Func<XElement, bool> AiMonsterOptionsSchema()
	{
		var fields = AiFields(("Motive", x => EnumValue<MonsterMotive>(x) &&
			Enum.Parse<MonsterMotive>(x, true) is not (MonsterMotive.None or MonsterMotive.SelfDefence)),
			("Feeding", EnumValue<MonsterFeedingMode>), ("SameRaceAllies DefenceUsesWindow TrapProvokes ReturnHome", AiBoolean),
			("AllyProg", AiIdentifier), ("GuardRange WarningSeconds ProvocationSeconds CooldownSeconds FeedingBites FeedingSeconds", AiNumber),
			("EngageDelay", AiDice), ("EngageEmote WarningEmote", AiText), ("LoadError", x => x.Length == 0));
		fields.Add("ActivityWindow", WindowSchema);
		return AiSchema("Monster", [("version", AiVersion)], fields, repeated: "Motive");
	}

	private static Func<XElement, bool> AiWindowSchema()
	{
		var schema = AiSchema("ActivityWindow", [("version", AiVersion)], AiFields(
			("Time", EnumValue<TimeOfDay>), ("Phase", EnumValue<MoonPhase>), ("Season Month", AiName),
			("Calendar Moon ConditionProg", AiIdentifier), ("FirstDay LastDay", x => x.Length == 0 || AiPositiveInteger(x)),
			("LoadError", x => x.Length == 0)), repeated: "Time Phase Season Month");
		return x => schema(x) && (string.IsNullOrEmpty(x.Element("FirstDay")?.Value) == string.IsNullOrEmpty(x.Element("LastDay")?.Value)) &&
			(!AiPositiveInteger(x.Element("FirstDay")?.Value ?? "") ||
			 int.Parse(x.Element("FirstDay")!.Value, CultureInfo.InvariantCulture) <= int.Parse(x.Element("LastDay")!.Value, CultureInfo.InvariantCulture)) &&
			(!x.Elements("Phase").Any() || AiPositiveIdentifier(x.Element("Moon")?.Value ?? "")) &&
			(!(x.Elements("Month").Any() || AiPositiveInteger(x.Element("FirstDay")?.Value ?? "")) ||
			 AiPositiveIdentifier(x.Element("Calendar")?.Value ?? ""));
	}

	private static Dictionary<string, Func<XElement, bool>> AiFields(params (string Names, Func<string, bool> Validate)[] groups) =>
		groups.SelectMany(group => group.Names.Split(' ').Select(name =>
			new KeyValuePair<string, Func<XElement, bool>>(name, x => AiLeaf(x, group.Validate))))
			.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

	private static Func<XElement, bool> AiSchema(string name, (string Name, Func<string, bool> Validate)[] attributes,
		Dictionary<string, Func<XElement, bool>> fields, string required = "", string repeated = "")
	{
		var attributeNames = attributes.Select(x => x.Name).ToArray();
		var fieldNames = fields.Keys.ToArray();
		var requiredNames = required.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var repeatedNames = repeated.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		return x => HasShape(x, name, attributeNames, fieldNames, repeatedNames) &&
			attributes.All(a => x.Attribute(a.Name) is not { } value || a.Validate(value.Value)) &&
			requiredNames.All(n => x.Element(n) is not null) && x.Elements().All(child => fields[child.Name.LocalName](child));
	}

	private static Func<XElement, bool> AiUniqueList(string name, string itemName, string[] attributes,
		Func<XElement, bool> validate, Func<XElement, string> key) => x =>
		HasShape(x, name, [], [itemName], itemName) && x.Elements().All(item =>
			item.Attributes().All(a => a.Name.Namespace == XNamespace.None && attributes.Contains(a.Name.LocalName)) && validate(item)) &&
		x.Elements().Select(key).Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Elements().Count();

	private static bool AiLeaf(XElement element, Func<string, bool> validate, params string[] attributes) =>
		element.Attributes().All(a => a.Name.Namespace == XNamespace.None && attributes.Contains(a.Name.LocalName)) &&
		element.Nodes().All(x => x is XText or XComment) && validate(element.Value);

	private static bool AiText(string value) => true;
	private static bool AiName(string value) => !string.IsNullOrWhiteSpace(value);
	private static bool AiBoolean(string value) => bool.TryParse(value, out _);
	private static bool AiVersion(string value) => value == "1";
	private static bool AiIdentifier(string value) => long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) && id >= 0;
	private static bool AiPositiveIdentifier(string value) => AiIdentifier(value) && long.Parse(value, CultureInfo.InvariantCulture) > 0;
	private static bool AiInteger(string value) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
	private static bool AiPositiveInteger(string value) => AiInteger(value) && int.Parse(value, CultureInfo.InvariantCulture) > 0;
	private static bool AiNullableInteger(string value) => value.Length == 0 || AiInteger(value);
	private static bool AiNumber(string value) => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number);
	private static bool AiSeconds(string value) => AiNumber(value) && double.Parse(value, CultureInfo.InvariantCulture) >= 0 &&
		double.Parse(value, CultureInfo.InvariantCulture) < TimeSpan.MaxValue.TotalSeconds;
	private static bool AiDifficulty(string value) => AiInteger(value) && EnumValue<Difficulty>(value);
	private static bool AiDice(string value) => Dice.TryValidateDiceExpression(value, out _);
	private static bool AiProgNameOrId(string value) => long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
		? id >= 0 : AiName(value); // Enforcer/Judge explicitly support legacy prog names through GetByName.
}
