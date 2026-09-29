#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

internal sealed record MonsterAIRecommendation(string Pack, string Race, string Disposition,
	IReadOnlyList<string> Profiles, string Movement, bool CanUseWeapons, IReadOnlyList<string> NativeAttacks,
	IReadOnlyList<string> RequiredBindings, IReadOnlyList<string> AuxiliaryAIs, string CombatSetup,
	IReadOnlyList<string> Limitations);

/// <summary>Additive named stock definitions. No race, NPC/template attachment, needs model or Wildlife row is converted.</summary>
internal static class MonsterAIStockTemplates
{
	internal const string LairGuardian = "Monster - Lair Guardian";
	internal const string NightStalker = "Monster - Night Stalker";
	internal const string ConditionalHunter = "Monster - Conditional Hunter";
	internal const string AerialHunter = "Monster - Aerial Hunter";
	internal const string AquaticAmbusher = "Monster - Aquatic Ambusher";
	internal const string TrapAmbusher = "Monster - Trap Ambusher";
	internal const string VenomAmbusher = "Monster - Venom Ambusher";
	internal const string BurrowAmbusher = "Monster - Burrow Ambusher";
	internal const string RelentlessPursuer = "Monster - Relentless Pursuer";
	internal const string BoundHaunt = "Monster - Bound Haunt";
	internal const string ArmedGuardian = "Monster - Armed Guardian";
	internal const string PoweredGuardian = "Monster - Powered Guardian";
	internal static IReadOnlyList<string> Names { get; } = [LairGuardian, NightStalker, ConditionalHunter, AerialHunter,
		AquaticAmbusher, TrapAmbusher, VenomAmbusher, BurrowAmbusher, RelentlessPursuer, BoundHaunt, ArmedGuardian, PoweredGuardian];
	internal static IReadOnlyList<MonsterAIRecommendation> Recommendations => MythicalAnimalSeeder.MonsterRecommendations
		.Concat(SupernaturalSeeder.MonsterRecommendations).OrderBy(x => x.Pack, StringComparer.Ordinal)
		.ThenBy(x => x.Race, StringComparer.Ordinal).ToList();
	internal static string RecommendationManifestJson() => JsonSerializer.Serialize(new
	{
		Version = 1,
		Controller = "Monster",
		DefaultNeedsModel = "NoNeeds",
		StockOwnership = "Only these exact Monster - names are reconciled. Differently named clones and all attachments are preserved.",
		GroupPolicy = "Individual Monster AI only. Existing Wildlife groups retain Animal AI.",
		Profiles = Names.Order(StringComparer.Ordinal).ToArray(),
		Recommendations
	}, new JsonSerializerOptions { WriteIndented = true }) + "\n";

	internal static IEnumerable<string> NamesForPack(string pack) => Recommendations.Where(x => x.Pack == pack)
		.SelectMany(x => x.Profiles).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal);
	internal static bool HasMissing(FuturemudDatabaseContext context, string pack) =>
		NamesForPack(pack).Any(name => !context.ArtificialIntelligences.Any(x => x.Name == name && x.Type == "Monster"));

	internal static void Seed(FuturemudDatabaseContext context, string pack)
	{
		foreach (var name in NamesForPack(pack))
		{
			var matches = context.ArtificialIntelligences.Where(x => x.Name == name).ToList();
			if (matches.Count > 1 || matches.Any(x => x.Type != "Monster"))
				throw new InvalidOperationException($"Cannot reconcile stock Monster AI '{name}': duplicate name or incompatible AI type. Rename the conflicting builder definition first.");
			var ai = matches.SingleOrDefault();
			if (ai is null)
			{
				ai = new ArtificialIntelligence { Name = name, Type = "Monster" };
				context.ArtificialIntelligences.Add(ai);
			}
			ai.Definition = Definition(name).ToString();
		}
		context.SaveChanges();
	}

	internal static XElement Definition(string name)
	{
		if (!Names.Contains(name)) throw new ArgumentOutOfRangeException(nameof(name));
		var guardian = name is LairGuardian or ArmedGuardian or PoweredGuardian or BoundHaunt;
		var trap = name == TrapAmbusher;
		var ambush = name is NightStalker or AquaticAmbusher or VenomAmbusher or BurrowAmbusher or AerialHunter;
		var relentless = name == RelentlessPursuer;
		var movement = name == AerialHunter ? "Fly" : name == AquaticAmbusher ? "Amphibious" : "Ground";
		var home = trap || name == BurrowAmbusher ? "Denning" : "None";
		var followup = name is AerialHunter or AquaticAmbusher ? "Extract" : name is TrapAmbusher or VenomAmbusher ? "VenomWithdrawal" : "Fight";
		return new XElement("Definition",
			new XElement("Movement", new XAttribute("type", movement), new XElement("Range", 10), new XElement("WanderChancePerMinute", 0),
				new XElement("TargetFlyingLayer", "InAir"), new XElement("TargetRestingLayer", "GroundLevel")),
			new XElement("Home", new XAttribute("type", home)),
			new XElement("Awareness", new XAttribute("type", "None"), new XElement("Senses", ambush ? "Stalking" : "Vigilant"), new XElement("Range", 5)),
			new XElement("Hunting", new XAttribute("version", 1), new XAttribute("enabled", true),
				new XElement("Opening", trap ? "TrapWait" : ambush ? "Ambush" : "Direct"), new XElement("Followup", followup),
				new XElement("PreferredLayer", name == AerialHunter ? "HighInAir" : name == AquaticAmbusher ? "Underwater" : ""),
				new XElement("People", "Eligible"), new XElement("Starvation", 0), new XElement("Engage", relentless ? 0 : 50),
				new XElement("Abandon", relentless ? 0 : 25), new XElement("Confidence", relentless ? 100 : 0),
				new XElement("Range", relentless ? 8 : 5), new XElement("TimeoutSeconds", relentless ? 900 : 300),
				new XElement("LostSeconds", relentless ? 120 : 60)),
			new XElement("Monster", new XAttribute("version", 1),
				new XElement("Motive", guardian ? "Territory" : name == ConditionalHunter ? "Condition" : "Scheduled"),
				new XElement("Motive", "Provocation"), new XElement("Feeding", "Off"), new XElement("SameRaceAllies", true),
				new XElement("ActivityWindow", new XAttribute("version", 1), name == NightStalker ? new XElement("Time", "Night") : null),
				new XElement("ReturnHome", true), new XElement("WarningSeconds", guardian ? 20 : 0), new XElement("GuardRange", 0),
				new XElement("WarningEmote", new XCData("@ turn|turns towards $1 and issue|issues a threatening warning.")),
				new XElement("CooldownSeconds", relentless ? 30 : 60), new XElement("TrapProvokes", trap)),
			new XElement("OpenDoors", name is ArmedGuardian or PoweredGuardian), new XElement("UseKeys", name is ArmedGuardian or PoweredGuardian));
	}

	internal static MonsterAIRecommendation Recommendation(string pack, string race, string disposition, string[] profiles,
		string movement, bool weapons, IEnumerable<string> attacks, params string[] limitations)
	{
		var bindings = new List<string> { "Clone a stock definition; choose eligible targets/allies and the NPC's needs model explicitly." };
		if (profiles.Any(x => x is LairGuardian or ArmedGuardian or PoweredGuardian or BoundHaunt)) bindings.Add("Bind the home-location prog to the authored defended area; review warning and pursuit radii.");
		if (profiles.Contains(ConditionalHunter)) bindings.Add("Bind a Boolean(Character) activity condition before use; optionally select game calendar dates or one local moon.");
		if (profiles.Any(x => x is TrapAmbusher or BurrowAmbusher)) bindings.Add("Bind a home, shelter craft/site and anchor where construction is wanted; trap construction needs its own valid native trap setup.");
		if (profiles.Contains(AerialHunter)) bindings.Add("Use flying terrain/layers, compatible hauling anatomy and sufficient lift capacity; dropping uses the NPC's Dropper combat strategy.");
		if (profiles.Contains(AquaticAmbusher)) bindings.Add("Use suitable aquatic habitat/layers and compatible hauling attacks; select Drowner combat settings when appropriate.");
		if (profiles.Contains(PoweredGuardian)) bindings.Add("Attach authored MagicAttackPower/psionic combat powers and compatible combat percentages/resources separately.");
		var combat = weapons
			? "Natural attacks are present. Weapon use requires actual equipment, skills and compatible normal combat settings; no gear or powers are granted."
			: "Use the existing natural attacks and anatomy. This race template does not permit weapons; no powers are granted.";
		return new MonsterAIRecommendation(pack, race, disposition, profiles, movement, weapons,
			attacks.Distinct().Order(StringComparer.Ordinal).ToArray(), bindings,
			profiles.Contains(TrapAmbusher) ? ["NaturalTrapAI, configured for an actual owned capture"] : [], combat,
			limitations.Concat(["Individual AI only; do not attach Monster AI to an existing Wildlife group."]).ToArray());
	}
}
