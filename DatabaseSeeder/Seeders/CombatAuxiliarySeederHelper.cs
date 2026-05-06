#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed record CombatAuxiliarySeedResult(int Actions, int Messages, int RaceLinks, int Tags, int Progs, int Strategies)
{
	public static CombatAuxiliarySeedResult Empty => new(0, 0, 0, 0, 0, 0);

	public CombatAuxiliarySeedResult Add(CombatAuxiliarySeedResult rhs)
	{
		return new CombatAuxiliarySeedResult(
			Actions + rhs.Actions,
			Messages + rhs.Messages,
			RaceLinks + rhs.RaceLinks,
			Tags + rhs.Tags,
			Progs + rhs.Progs,
			Strategies + rhs.Strategies);
	}

	public bool HasChanges => Actions + Messages + RaceLinks + Tags + Progs + Strategies > 0;
}

internal static class CombatAuxiliarySeederHelper
{
	private const long PositionStandingId = 1;
	private const long PositionSprawledId = 8;
	private const long PositionSwimmingId = 16;
	private const long PositionFloatingInWaterId = 17;
	private const long PositionFlyingId = 18;

	private sealed record ActionDefinition(
		string Name,
		CombatMoveIntentions Intentions,
		Func<TraitDefinition, IEnumerable<XElement>> Effects,
		string SuccessMessage,
		string FailureMessage,
		string? UsabilityProgName = null,
		double StaminaCost = 1.0,
		double BaseDelay = 1.0,
		double Weighting = 100.0,
		Difficulty MoveDifficulty = Difficulty.Normal);

	private static readonly string[] HumanAuxiliaryMoveNames =
	[
		"Circle to Flank",
		"Sidestep and Press",
		"Bind and Step",
		"Low Line Feint",
		"High Line Feint",
		"Distracting Flourish",
		"False Opening",
		"Retreating Guard",
		"Guarded Shuffle",
		"Shield Jostle",
		"Shield Glint",
		"Shoulder Check",
		"Foot Sweep",
		"Shove Off Balance",
		"Pommel Beat",
		"Wrist Check",
		"Beat the Weapon",
		"Hook and Pull",
		"Sand in the Eyes",
		"Dirt Kick"
	];

	private static readonly IReadOnlyDictionary<string, string[]> NonHumanActionRaceHints =
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["Canid Harry"] = ["Dog", "Wolf", "Coyote", "Dingo", "Jackal", "Hyena", "Fox"],
			["Feline Pounce Feint"] = ["Cat", "Lion", "Tiger", "Leopard", "Jaguar", "Cheetah", "Panther", "Sabretooth"],
			["Equine Shoulder Barge"] = ["Horse", "Pony", "Donkey", "Mule", "Zebra", "Camel", "Bull", "Cow", "Bison", "Buffalo", "Ox"],
			["Avian Wing Buffet"] = ["Eagle", "Falcon", "Hawk", "Owl", "Crow", "Raven", "Vulture", "Condor", "Swan", "Goose"],
			["Serpent Coil Feint"] = ["Snake", "Adder", "Anaconda", "Boa", "Cobra", "Mamba", "Python", "Rattlesnake", "Viper"],
			["Ursine Maul-Feint"] = ["Bear", "Ursine"],
			["Dragon Wing Shadow"] = ["Dragon", "Drake", "Wyvern"],
			["Gryphon Buffet"] = ["Gryphon", "Griffin"],
			["Unicorn Dazzling Feint"] = ["Unicorn", "Qilin", "Kirin"],
			["Hydra Many-Head Feint"] = ["Hydra"],
			["Basilisk Glare Feint"] = ["Basilisk", "Cockatrice"],
			["Myconid Spore Cloud"] = ["Myconid", "Mushroom", "Fungus"],
			["Servo Jostle"] = ["Robot", "Android", "Drone", "Automaton", "Construct", "Cyborg"],
			["Hydraulic Shove"] = ["Robot", "Android", "Drone", "Automaton", "Construct", "Cyborg"],
			["Sensor Flash"] = ["Robot", "Android", "Drone", "Automaton", "Construct", "Cyborg"],
			["Magnetic Wrench"] = ["Robot", "Android", "Drone", "Automaton", "Construct", "Cyborg"],
			["Ghostly Misdirection"] = ["Ghost", "Spirit", "Wraith", "Spectre", "Specter", "Shade", "Phantom"],
			["Infernal Glare"] = ["Demon", "Devil", "Fiend", "Infernal", "Balrog", "Imp"],
			["Angelic Dazzle"] = ["Angel", "Seraph", "Cherub", "Celestial"],
			["Werewolf Lunge Feint"] = ["Werewolf", "Lycanthrope"],
			["Undead Bone-Rattle"] = ["Skeleton", "Zombie", "Ghoul", "Mummy", "Lich", "Undead"],
			["Fiend Tail Hook"] = ["Fiend", "Demon", "Devil", "Imp", "Succubus", "Incubus", "Balrog"]
		};

	private static readonly IReadOnlyDictionary<string, double> StockAuxiliaryStrategyPercentages =
		new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
		{
			["Melee"] = 0.10,
			["Melee (Auto)"] = 0.10,
			["Cautious Melee"] = 0.12,
			["Cautious Melee (Auto)"] = 0.12,
			["Shielder"] = 0.15,
			["Shielder (Auto)"] = 0.15,
			["Skirmisher"] = 0.15,
			["Skirmisher (Auto)"] = 0.15,
			["Brawler"] = 0.18,
			["Brawler (Auto)"] = 0.18,
			["Pitfighter"] = 0.18,
			["Pitfighter (Auto)"] = 0.18,
			["Swarmer"] = 0.18,
			["Swarmer (Auto)"] = 0.18,
			["Outboxer"] = 0.15,
			["Outboxer (Auto)"] = 0.15,
			["Grappler"] = 0.18,
			["Grappler (Auto)"] = 0.18,
			["Bonebreaker"] = 0.18,
			["Bonebreaker (Auto)"] = 0.18,
			["Strangler"] = 0.18,
			["Strangler (Auto)"] = 0.18,
			["Beast Brawler"] = 0.18,
			["Beast Clincher"] = 0.18,
			["Beast Behemoth"] = 0.20,
			["Beast Skirmisher"] = 0.15,
			["Beast Swooper"] = 0.15,
			["Beast Drowner"] = 0.18,
			["Beast Dropper"] = 0.18,
			["Beast Physical Avoider"] = 0.20,
			["Construct Brawler"] = 0.18,
			["Construct Skirmisher"] = 0.15,
			["Construct Artillery"] = 0.20
		};

	internal static IReadOnlyCollection<string> HumanAuxiliaryMoveNamesForTesting => HumanAuxiliaryMoveNames;
	internal static IReadOnlyDictionary<string, string[]> NonHumanActionRaceHintsForTesting => NonHumanActionRaceHints;
	internal static IReadOnlyCollection<string> StockAuxiliaryProgNamesForTesting =>
		["Auxiliary_CanThrowSandOrDirt", "Auxiliary_CanShieldGlint"];

	public static CombatAuxiliarySeedResult EnsureStockAuxiliaryContent(FuturemudDatabaseContext context)
	{
		var tags = EnsureAuxiliaryTags(context);
		var progs = EnsureAuxiliaryProgs(context);
		var trait = GetAuxiliaryTrait(context);
		var actionCount = EnsureActions(context, BuildAllDefinitions(progs), progs, trait);
		context.SaveChanges();
		var messageCount = EnsureCombatMessages(context, BuildAllDefinitions(progs));
		var raceLinks = EnsureHumanoidAuxiliaryLinks(context);
		var strategies = ApplyStockAuxiliaryPercentages(context);
		context.SaveChanges();
		return new CombatAuxiliarySeedResult(actionCount, messageCount, raceLinks, tags, progs.Count, strategies);
	}

	public static CombatAuxiliarySeedResult EnsureAnimalAuxiliaryLinks(FuturemudDatabaseContext context)
	{
		var stock = EnsureStockAuxiliaryContent(context);
		var links = EnsureRaceLinks(context, NonHumanActionRaceHints.Keys.Take(6));
		context.SaveChanges();
		return stock.Add(new CombatAuxiliarySeedResult(0, 0, links, 0, 0, 0));
	}

	public static CombatAuxiliarySeedResult EnsureMythicalAuxiliaryLinks(FuturemudDatabaseContext context)
	{
		var stock = EnsureStockAuxiliaryContent(context);
		var links = EnsureRaceLinks(context, NonHumanActionRaceHints.Keys.Skip(6).Take(6));
		context.SaveChanges();
		return stock.Add(new CombatAuxiliarySeedResult(0, 0, links, 0, 0, 0));
	}

	public static CombatAuxiliarySeedResult EnsureRobotAuxiliaryLinks(FuturemudDatabaseContext context)
	{
		var stock = EnsureStockAuxiliaryContent(context);
		var links = EnsureRaceLinks(context, NonHumanActionRaceHints.Keys.Skip(12).Take(4));
		context.SaveChanges();
		return stock.Add(new CombatAuxiliarySeedResult(0, 0, links, 0, 0, 0));
	}

	public static CombatAuxiliarySeedResult EnsureSupernaturalAuxiliaryLinks(FuturemudDatabaseContext context)
	{
		var stock = EnsureStockAuxiliaryContent(context);
		var links = EnsureRaceLinks(context, NonHumanActionRaceHints.Keys.Skip(16));
		context.SaveChanges();
		return stock.Add(new CombatAuxiliarySeedResult(0, 0, links, 0, 0, 0));
	}

	public static int ApplyStockAuxiliaryPercentages(FuturemudDatabaseContext context)
	{
		var count = 0;
		foreach (var setting in context.CharacterCombatSettings.AsEnumerable()
		                       .Where(x => StockAuxiliaryStrategyPercentages.ContainsKey(x.Name)))
		{
			if (ApplyStockAuxiliaryPercentage(setting))
			{
				count++;
			}
		}

		return count;
	}

	public static bool ApplyStockAuxiliaryPercentage(CharacterCombatSetting setting)
	{
		if (!StockAuxiliaryStrategyPercentages.TryGetValue(setting.Name, out var desired))
		{
			return false;
		}

		var changed = Math.Abs(setting.AuxiliaryPercentage - desired) > 0.0001;
		var oldAuxiliary = setting.AuxiliaryPercentage;
		setting.AuxiliaryPercentage = desired;
		var total = setting.WeaponUsePercentage + setting.NaturalWeaponPercentage + setting.MagicUsePercentage +
		            setting.PsychicUsePercentage + setting.AuxiliaryPercentage;
		if (total > 1.0)
		{
			var excess = total - 1.0;
			if (setting.WeaponUsePercentage > 0.0)
			{
				var reduction = Math.Min(setting.WeaponUsePercentage, excess);
				setting.WeaponUsePercentage -= reduction;
				excess -= reduction;
			}

			if (excess > 0.0 && setting.NaturalWeaponPercentage > 0.0)
			{
				var reduction = Math.Min(setting.NaturalWeaponPercentage, excess);
				setting.NaturalWeaponPercentage -= reduction;
				excess -= reduction;
			}

			changed = true;
		}

		return changed || Math.Abs(oldAuxiliary - setting.AuxiliaryPercentage) > 0.0001;
	}

	private static int EnsureAuxiliaryTags(FuturemudDatabaseContext context)
	{
		var before = context.Tags.Local.Count + context.Tags.Count();
		var functions = EnsureTag(context, "Functions", null);
		EnsureTag(context, "Shiny", functions);
		EnsureTag(context, "Reflective", functions);
		context.SaveChanges();
		var after = context.Tags.Local.Count + context.Tags.Count();
		return Math.Max(0, after - before);
	}

	private static Tag EnsureTag(FuturemudDatabaseContext context, string name, Tag? parent)
	{
		var tag = context.Tags.Local.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
		          context.Tags.AsEnumerable().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (tag is not null)
		{
			if (parent is not null && tag.ParentId is null && tag.Parent is null)
			{
				tag.Parent = parent;
			}

			return tag;
		}

		tag = new Tag
		{
			Name = name,
			Parent = parent
		};
		context.Tags.Add(tag);
		return tag;
	}

	private static Dictionary<string, FutureProg> EnsureAuxiliaryProgs(FuturemudDatabaseContext context)
	{
		var sunId = context.Celestials
		                   .AsEnumerable()
		                   .Where(x => x.CelestialType?.Contains("Sun", StringComparison.OrdinalIgnoreCase) == true)
		                   .Select(x => x.Id)
		                   .FirstOrDefault();
		var result = new Dictionary<string, FutureProg>(StringComparer.OrdinalIgnoreCase)
		{
			["Auxiliary_CanThrowSandOrDirt"] = SeederRepeatabilityHelper.EnsureProg(
				context,
				"Auxiliary_CanThrowSandOrDirt",
				"Combat",
				"Auxiliary",
				ProgVariableTypes.Boolean,
				"Allows dirty sand and dirt auxiliary moves only in loose soil or sandy terrain, and blocks vacuum, space and underwater use.",
				@"return (istagged(@ch.Location.Terrain, ""Diggable Soil"") or istagged(@ch.Location.Terrain, ""Foragable Sand"")) and not istagged(@ch.Location.Terrain, ""Vacuum"") and not istagged(@ch.Location.Terrain, ""Space"") and not isunderwater(@ch.Location, @ch.Layer);",
				true,
				false,
				FutureProgStaticType.NotStatic,
				(ProgVariableTypes.Character, "ch"),
				(ProgVariableTypes.Item, "item"),
				(ProgVariableTypes.Character, "target")),
			["Auxiliary_CanShieldGlint"] = SeederRepeatabilityHelper.EnsureProg(
				context,
				"Auxiliary_CanShieldGlint",
				"Combat",
				"Auxiliary",
				ProgVariableTypes.Boolean,
				"Allows shield glint auxiliary moves only with a shiny or reflective wielded item, outdoors in light, with a sun-like celestial above about 15 degrees.",
				$@"return @ch.Location.Outdoors >= 2 and @ch.Location.Light > 100 and @ch.WieldedItems.any(item, istagged(@item, ""Shiny"") or istagged(@item, ""Reflective"")) and celestialelevation(@ch.Location, {sunId.ToString(System.Globalization.CultureInfo.InvariantCulture)}) > 0.26;",
				true,
				false,
				FutureProgStaticType.NotStatic,
				(ProgVariableTypes.Character, "ch"),
				(ProgVariableTypes.Item, "item"),
				(ProgVariableTypes.Character, "target"))
		};
		return result;
	}

	private static TraitDefinition GetAuxiliaryTrait(FuturemudDatabaseContext context)
	{
		var configured = context.StaticConfigurations
		                        .FirstOrDefault(x => x.SettingName == "DefaultAuxiliaryMoveTraitId");
		if (configured is not null && long.TryParse(configured.Definition, out var id))
		{
			var trait = context.TraitDefinitions.FirstOrDefault(x => x.Id == id);
			if (trait is not null)
			{
				return trait;
			}
		}

		var traits = context.TraitDefinitions.AsEnumerable().ToList();
		return traits.FirstOrDefault(x => x.Name.In("Brawling", "Dodge", "Agility", "Dexterity", "Speed")) ??
		       traits.First(x => x.Type == (int)TraitType.Skill || x.Type == (int)TraitType.Attribute);
	}

	private static IEnumerable<ActionDefinition> BuildAllDefinitions(IReadOnlyDictionary<string, FutureProg> progs)
	{
		foreach (var definition in BuildHumanDefinitions(progs))
		{
			yield return definition;
		}

		foreach (var definition in BuildNonHumanDefinitions())
		{
			yield return definition;
		}
	}

	private static IEnumerable<ActionDefinition> BuildHumanDefinitions(IReadOnlyDictionary<string, FutureProg> progs)
	{
		yield return Def("Circle to Flank", CombatMoveIntentions.Advantage | CombatMoveIntentions.Flank, t => [Facing(t)], "@ circle|circles warily around $1, looking for a flank.", "@ try|tries to circle $1, but cannot find a better angle.");
		yield return Def("Sidestep and Press", CombatMoveIntentions.Advantage | CombatMoveIntentions.Flank, t => [Facing(t), AttackerAdvantage(t, 0.2, 0.1)], "@ sidestep|sidesteps and press|presses $1 from an awkward angle.", "@ sidestep|sidesteps, but $1 turns with the pressure.");
		yield return Def("Bind and Step", CombatMoveIntentions.Advantage | CombatMoveIntentions.Hinder, t => [Delay(t, 1.0, 0.4, 4.0), Facing(t)], "@ bind|binds $1 up for a heartbeat and step|steps aside.", "@ try|tries to bind $1 up, but cannot make it stick.");
		yield return Def("Low Line Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction, t => [AttackerAdvantage(t, 0.25, 0.0)], "@ dip|dips into a low-line feint against $1.", "@ feint|feints low, but $1 does not bite.");
		yield return Def("High Line Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction, t => [AttackerAdvantage(t, 0.25, 0.0)], "@ threaten|threatens high, drawing $1's guard upward.", "@ threaten|threatens high, but $1 keeps a disciplined guard.");
		yield return Def("Distracting Flourish", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction | CombatMoveIntentions.Flashy, t => [AttackerAdvantage(t, 0.15, 0.25)], "@ flourish|flourishes with distracting movement at $1.", "@ flourish|flourishes, but $1 keeps focus.");
		yield return Def("False Opening", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction | CombatMoveIntentions.Risky, t => [AttackerAdvantage(t, 0.35, 0.0)], "@ offer|offers $1 a false opening.", "@ offer|offers a false opening, but $1 refuses it.");
		yield return Def("Retreating Guard", CombatMoveIntentions.Advantage | CombatMoveIntentions.Defensive, t => [AttackerAdvantage(t, 0.0, 0.35)], "@ retreat|retreats behind a guarded posture against $1.", "@ retreat|retreats, but $1 keeps the pressure on.");
		yield return Def("Guarded Shuffle", CombatMoveIntentions.Advantage | CombatMoveIntentions.Defensive | CombatMoveIntentions.Cautious, t => [AttackerAdvantage(t, 0.0, 0.25), Facing(t)], "@ shuffle|shuffles guardedly around $1.", "@ shuffle|shuffles, but $1 denies the angle.");
		yield return Def("Shield Jostle", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Shield | CombatMoveIntentions.Hinder, t => [Delay(t, 1.0, 0.5, 4.0), Stamina(t, 2.0, 1.0, 8.0)], "@ jostle|jostles $1 with a shield-side shove.", "@ try|tries to jostle $1, but cannot break their rhythm.");
		yield return Def("Shield Glint", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Shield | CombatMoveIntentions.Distraction | CombatMoveIntentions.Flashy, t => [Delay(t, 1.5, 0.5, 5.0)], "@ flash|flashes reflected light toward $1's eyes.", "@ angle|angles for a flash of light, but $1 is not caught by it.", "Auxiliary_CanShieldGlint");
		yield return Def("Shoulder Check", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [Stamina(t, 3.0, 1.0, 10.0), Delay(t, 0.5, 0.3, 3.0)], "@ check|checks $1 hard with a shoulder.", "@ try|tries to check $1, but cannot shift them.");
		yield return Def("Foot Sweep", CombatMoveIntentions.Trip | CombatMoveIntentions.Disadvantage, t => [PositionChange(t, 1.0, 0.4, 4.0)], "@ sweep|sweeps at $1's feet.", "@ sweep|sweeps low, but $1 stays upright.");
		yield return Def("Shove Off Balance", CombatMoveIntentions.Trip | CombatMoveIntentions.Hinder, t => [PositionChange(t, 1.5, 0.5, 5.0)], "@ shove|shoves $1 off balance.", "@ shove|shoves at $1, but cannot topple them.");
		yield return Def("Pommel Beat", CombatMoveIntentions.Disarm | CombatMoveIntentions.Disadvantage, t => [Disarm(t), Delay(t, 0.5, 0.2, 2.5)], "@ beat|beats at $1's weapon hand.", "@ beat|beats at $1's weapon hand, but $1 keeps hold.");
		yield return Def("Wrist Check", CombatMoveIntentions.Disarm | CombatMoveIntentions.Hinder, t => [Disarm(t), Stamina(t, 2.0, 1.0, 6.0)], "@ check|checks $1's wrist and weapon line.", "@ check|checks $1's wrist, but cannot open the grip.");
		yield return Def("Beat the Weapon", CombatMoveIntentions.Disarm | CombatMoveIntentions.Disadvantage, t => [Disarm(t)], "@ beat|beats $1's weapon aside.", "@ beat|beats at $1's weapon, but it stays in line.");
		yield return Def("Hook and Pull", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder | CombatMoveIntentions.Flank, t => [Delay(t, 1.0, 0.5, 4.0), Facing(t)], "@ hook|hooks and pull|pulls $1 out of line.", "@ hook|hooks at $1, but cannot drag them out of line.");
		yield return Def("Sand in the Eyes", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction | CombatMoveIntentions.Dirty, t => [Delay(t, 2.0, 0.5, 6.0)], "@ fling|flings grit toward $1's eyes.", "@ fling|flings grit, but $1 avoids the worst of it.", "Auxiliary_CanThrowSandOrDirt");
		yield return Def("Dirt Kick", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Dirty | CombatMoveIntentions.Hinder, t => [Delay(t, 1.5, 0.5, 5.0), Stamina(t, 2.0, 1.0, 6.0)], "@ kick|kicks dirt and grit toward $1.", "@ kick|kicks dirt, but $1 stays clear.", "Auxiliary_CanThrowSandOrDirt");
	}

	private static IEnumerable<ActionDefinition> BuildNonHumanDefinitions()
	{
		yield return Def("Canid Harry", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [Delay(t), Stamina(t)], "@ harry|harries $1 with darting snaps.", "@ snap|snaps and harry|harries, but $1 keeps pace.");
		yield return Def("Feline Pounce Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Flank, t => [Facing(t), AttackerAdvantage(t, 0.2, 0.0)], "@ bunch|bunches and feint|feints a sudden pounce at $1.", "@ feint|feints a pounce, but $1 does not give ground.");
		yield return Def("Equine Shoulder Barge", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [PositionChange(t), Stamina(t, 4.0, 1.0, 12.0)], "@ barge|barges shoulder-first into $1.", "@ barge|barges at $1, but cannot knock them aside.");
		yield return Def("Avian Wing Buffet", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t), Facing(t)], "@ buffet|buffets $1 with beating wings.", "@ beat|beats wings at $1, but $1 weathers it.");
		yield return Def("Serpent Coil Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction, t => [AttackerAdvantage(t, 0.3, 0.0), Delay(t, 0.5, 0.3, 3.0)], "@ coil|coils and feint|feints at $1.", "@ coil|coils in a feint, but $1 reads it.");
		yield return Def("Ursine Maul-Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Savage, t => [Stamina(t, 4.0, 1.0, 12.0), AttackerAdvantage(t, 0.2, 0.0)], "@ rear|rears and feint|feints a mauling rush at $1.", "@ feint|feints a mauling rush, but $1 stands firm.");
		yield return Def("Dragon Wing Shadow", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Flashy | CombatMoveIntentions.Distraction, t => [Delay(t, 2.0, 0.5, 7.0), Facing(t)], "@ cast|casts a sudden wing-shadow over $1.", "@ spread|spreads wings dramatically, but $1 is not wrong-footed.");
		yield return Def("Gryphon Buffet", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [Delay(t), PositionChange(t, 1.0, 0.4, 4.0)], "@ buffet|buffets $1 with a hard wing and shoulder rush.", "@ buffet|buffets at $1, but $1 keeps balance.");
		yield return Def("Unicorn Dazzling Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Flashy | CombatMoveIntentions.Distraction, t => [AttackerAdvantage(t, 0.3, 0.1), Delay(t)], "@ toss|tosses a dazzling horn-feint toward $1.", "@ toss|tosses a glittering feint, but $1 sees through it.");
		yield return Def("Hydra Many-Head Feint", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t, 2.0, 0.5, 8.0), Stamina(t)], "@ feint|feints with a confusion of snapping heads around $1.", "@ feint|feints with many heads, but $1 tracks the danger.");
		yield return Def("Basilisk Glare Feint", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t, 2.0, 0.5, 6.0)], "@ fix|fixes $1 with a terrible glare.", "@ glare|glares at $1, but $1 refuses the stare.");
		yield return Def("Myconid Spore Cloud", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [Delay(t, 2.0, 0.5, 7.0), Stamina(t, 3.0, 1.0, 9.0)], "@ puff|puffs a distracting spore cloud toward $1.", "@ release|releases spores, but $1 avoids the cloud.");
		yield return Def("Servo Jostle", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder, t => [Stamina(t), Delay(t)], "@ jostle|jostles $1 with precise servo pressure.", "@ jostle|jostles at $1, but cannot disrupt them.");
		yield return Def("Hydraulic Shove", CombatMoveIntentions.Trip | CombatMoveIntentions.Hinder, t => [PositionChange(t, 1.5, 0.5, 5.0)], "@ shove|shoves $1 with hydraulic force.", "@ shove|shoves hydraulically at $1, but $1 holds position.");
		yield return Def("Sensor Flash", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t, 1.5, 0.5, 5.0)], "@ pulse|pulses a sensor flash at $1.", "@ pulse|pulses a sensor flash, but $1 is not caught.");
		yield return Def("Magnetic Wrench", CombatMoveIntentions.Disarm | CombatMoveIntentions.Disadvantage, t => [Disarm(t)], "@ wrench|wrenches magnetically at $1's weapon.", "@ wrench|wrenches magnetically, but $1 keeps hold.");
		yield return Def("Ghostly Misdirection", CombatMoveIntentions.Advantage | CombatMoveIntentions.Distraction, t => [Facing(t), AttackerAdvantage(t, 0.25, 0.0)], "@ blur|blurs with ghostly misdirection around $1.", "@ blur|blurs aside, but $1 follows the motion.");
		yield return Def("Infernal Glare", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t, 2.0, 0.5, 6.0)], "@ level|levels an infernal glare at $1.", "@ glare|glares infernally, but $1 does not falter.");
		yield return Def("Angelic Dazzle", CombatMoveIntentions.Advantage | CombatMoveIntentions.Flashy | CombatMoveIntentions.Distraction, t => [AttackerAdvantage(t, 0.25, 0.25), Delay(t)], "@ dazzle|dazzles $1 with a sudden radiant feint.", "@ flare|flares radiantly, but $1 keeps focus.");
		yield return Def("Werewolf Lunge Feint", CombatMoveIntentions.Advantage | CombatMoveIntentions.Savage, t => [AttackerAdvantage(t, 0.3, 0.0), Facing(t)], "@ lunge|lunges in a savage feint at $1.", "@ lunge|lunges, but $1 reads the feint.");
		yield return Def("Undead Bone-Rattle", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Distraction, t => [Delay(t), Stamina(t, 2.0, 1.0, 6.0)], "@ rattle|rattles with unnerving dead motion before $1.", "@ rattle|rattles unnervingly, but $1 is not distracted.");
		yield return Def("Fiend Tail Hook", CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Hinder | CombatMoveIntentions.Flank, t => [Facing(t), PositionChange(t, 1.0, 0.4, 4.0)], "@ hook|hooks at $1 with a fiendish tail.", "@ hook|hooks with a tail, but $1 slips clear.");
	}

	private static ActionDefinition Def(string name, CombatMoveIntentions intentions, Func<TraitDefinition, IEnumerable<XElement>> effects,
		string success, string failure, string? prog = null)
	{
		return new ActionDefinition(name, intentions, effects, success, failure, prog);
	}

	private static XElement Common(string type, TraitDefinition trait, Difficulty difficulty, double flat, double perDegree,
		double maximum, OpposedOutcomeDegree minimum = OpposedOutcomeDegree.Marginal)
	{
		return new XElement("Effect",
			new XAttribute("type", type),
			new XAttribute("defensetrait", trait.Id),
			new XAttribute("defensedifficulty", (int)difficulty),
			new XAttribute("minimumdegree", (int)minimum),
			new XAttribute("flatamount", flat.ToString(System.Globalization.CultureInfo.InvariantCulture)),
			new XAttribute("perdegreeamount", perDegree.ToString(System.Globalization.CultureInfo.InvariantCulture)),
			new XAttribute("maximumamount", maximum.ToString(System.Globalization.CultureInfo.InvariantCulture)));
	}

	private static XElement Delay(TraitDefinition trait, double flat = 1.5, double perDegree = 0.5, double maximum = 6.0) =>
		Common("targetdelay", trait, Difficulty.Normal, flat, perDegree, maximum);

	private static XElement Stamina(TraitDefinition trait, double flat = 3.0, double perDegree = 1.0, double maximum = 10.0) =>
		Common("targetstamina", trait, Difficulty.Normal, flat, perDegree, maximum);

	private static XElement Facing(TraitDefinition trait)
	{
		var root = Common("facing", trait, Difficulty.Normal, 1.0, 0.0, 1.0);
		root.Add(new XAttribute("subject", "Attacker"), new XAttribute("direction", "Improve"));
		return root;
	}

	private static XElement PositionChange(TraitDefinition trait, double flat = 1.5, double perDegree = 0.5, double maximum = 5.0)
	{
		var root = Common("positionchange", trait, Difficulty.Normal, flat, perDegree, maximum);
		root.Add(new XAttribute("position", PositionSprawledId), new XAttribute("knockdown", true));
		return root;
	}

	private static XElement Disarm(TraitDefinition trait)
	{
		var root = Common("disarm", trait, Difficulty.Normal, 90.0, 0.0, 90.0);
		root.Add(new XAttribute("selection", "Best"));
		return root;
	}

	private static XElement AttackerAdvantage(TraitDefinition trait, double offense, double defense)
	{
		return new XElement("Effect",
			new XAttribute("type", "attackeradvantage"),
			new XAttribute("defensebonusperdegree", defense.ToString(System.Globalization.CultureInfo.InvariantCulture)),
			new XAttribute("offensebonusperdegree", offense.ToString(System.Globalization.CultureInfo.InvariantCulture)),
			new XAttribute("defensetrait", trait.Id),
			new XAttribute("defensedifficulty", (int)Difficulty.Normal),
			new XAttribute("allownegatives", true),
			new XAttribute("allowpositives", true));
	}

	private static int EnsureActions(FuturemudDatabaseContext context, IEnumerable<ActionDefinition> definitions,
		IReadOnlyDictionary<string, FutureProg> progs, TraitDefinition trait)
	{
		var count = 0;
		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		var definitionsList = definitions.ToList();
		foreach (var definition in definitionsList)
		{
			var action = context.CombatActions
			                    .Include(x => x.CombatMessagesCombatActions)
			                    .FirstOrDefault(x => x.Name == definition.Name &&
			                                         x.MoveType == (int)BuiltInCombatMoveType.AuxiliaryMove);
			if (action is null)
			{
				action = new CombatAction();
				context.CombatActions.Add(action);
				count++;
			}

			var prog = definition.UsabilityProgName is null || !progs.TryGetValue(definition.UsabilityProgName, out var definedProg)
				? alwaysTrue
				: definedProg;
			action.Name = definition.Name;
			action.BaseDelay = definition.BaseDelay;
			action.ExertionLevel = (int)ExertionLevel.Heavy;
			action.UsabilityProg = prog;
			action.UsabilityProgId = prog.Id == 0 ? null : prog.Id;
			action.Intentions = (long)definition.Intentions;
			action.MoveType = (int)BuiltInCombatMoveType.AuxiliaryMove;
			action.RecoveryDifficultyFailure = (int)Difficulty.Normal;
			action.RecoveryDifficultySuccess = (int)Difficulty.Easy;
			action.StaminaCost = definition.StaminaCost;
			action.Weighting = definition.Weighting;
			action.MoveDifficulty = (int)definition.MoveDifficulty;
			action.RequiredPositionStateIds =
				$"{PositionStandingId} {PositionFlyingId} {PositionFloatingInWaterId} {PositionSwimmingId}";
			action.TraitDefinition = trait;
			action.TraitDefinitionId = trait.Id;
			action.AdditionalInfo = new XElement("Root", definition.Effects(trait)).ToString(SaveOptions.DisableFormatting);
		}

		return count;
	}

	private static int EnsureCombatMessages(FuturemudDatabaseContext context, IEnumerable<ActionDefinition> definitions)
	{
		var count = 0;
		foreach (var definition in definitions)
		{
			var action = context.CombatActions.First(x => x.Name == definition.Name &&
			                                             x.MoveType == (int)BuiltInCombatMoveType.AuxiliaryMove);
			var existing = context.CombatMessages
			                      .Include(x => x.CombatMessagesCombatActions)
			                      .AsEnumerable()
			                      .FirstOrDefault(x => x.Type == (int)BuiltInCombatMoveType.AuxiliaryMove &&
			                                           x.CombatMessagesCombatActions.Any(y => y.CombatActionId == action.Id));
			if (existing is null)
			{
				existing = new CombatMessage
				{
					CombatMessagesCombatActions =
					[
						new CombatMessagesCombatActions
						{
							CombatAction = action,
							CombatActionId = action.Id
						}
					]
				};
				context.CombatMessages.Add(existing);
				count++;
			}

			existing.Type = (int)BuiltInCombatMoveType.AuxiliaryMove;
			existing.Outcome = null;
			existing.Message = definition.SuccessMessage;
			existing.FailureMessage = definition.FailureMessage;
			existing.Priority = 50;
			existing.Chance = 1.0;
			existing.Verb = null;
		}

		return count;
	}

	private static int EnsureHumanoidAuxiliaryLinks(FuturemudDatabaseContext context)
	{
		var raceHints = new[]
		{
			"Human", "Elf", "Dwarf", "Gnome", "Goblin", "Orc", "Hobbit", "Halfling", "Troll", "Ogre", "Half-Elf", "Half-Orc"
		};
		return EnsureRaceLinks(context, HumanAuxiliaryMoveNames, raceHints);
	}

	private static int EnsureRaceLinks(FuturemudDatabaseContext context, IEnumerable<string> actionNames)
	{
		var actionList = actionNames.ToList();
		var raceHints = actionList.SelectMany(x => NonHumanActionRaceHints[x]).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
		return EnsureRaceLinks(context, actionList, raceHints);
	}

	private static int EnsureRaceLinks(FuturemudDatabaseContext context, IEnumerable<string> actionNames, IReadOnlyCollection<string> raceHints)
	{
		var count = 0;
		var actions = context.CombatActions
		                     .AsEnumerable()
		                     .Where(x => actionNames.Contains(x.Name, StringComparer.OrdinalIgnoreCase) &&
		                                 x.MoveType == (int)BuiltInCombatMoveType.AuxiliaryMove)
		                     .ToList();
		var races = context.Races
		                   .AsEnumerable()
		                   .Where(x => raceHints.Any(y =>
			                   x.Name.Equals(y, StringComparison.OrdinalIgnoreCase) ||
			                   x.Name.Contains(y, StringComparison.OrdinalIgnoreCase)))
		                   .ToList();
		foreach (var race in races)
		{
			foreach (var action in actions)
			{
				if (context.RacesCombatActions.Local.Any(x => x.RaceId == race.Id && x.CombatActionId == action.Id) ||
				    context.RacesCombatActions.Any(x => x.RaceId == race.Id && x.CombatActionId == action.Id))
				{
					continue;
				}

				context.RacesCombatActions.Add(new RacesCombatActions
				{
					Race = race,
					RaceId = race.Id,
					CombatAction = action,
					CombatActionId = action.Id
				});
				count++;
			}
		}

		return count;
	}
}
