#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public sealed record PsionicCombatPower(string Name, string Verb, int Band, double Cost,
	MagicAttackRange Range, int Rooms, MagicAttackEffectType? Rider = null, MagicDefenseMode? Defense = null,
	double ReactionCost = 0, int Charges = 3, double Capacity = 30);

public static partial class PsionicStockContent
{
	public static IReadOnlyList<PsionicCombatPower> CombatPowers { get; } =
	[
		new("Force Strike", "forcestrike", 20, 3, MagicAttackRange.Melee, 0),
		new("Force Lance", "forcelance", 20, 5, MagicAttackRange.Ranged, 3),
		new("Psychic Trip", "psychictrip", 40, 4, MagicAttackRange.Ranged, 1, MagicAttackEffectType.Knockdown),
		new("Concussive Pulse", "concussivepulse", 40, 4, MagicAttackRange.Ranged, 1, MagicAttackEffectType.Stagger),
		new("Repulse", "repulse", 40, 5, MagicAttackRange.Melee, 0, MagicAttackEffectType.Pushback),
		new("Wrench", "wrench", 40, 6, MagicAttackRange.Ranged, 1, MagicAttackEffectType.Disarm),
		new("Draw Foe", "drawfoe", 40, 4, MagicAttackRange.Melee, 0, MagicAttackEffectType.Pull),
		new("Break Hold", "breakhold", 40, 4, MagicAttackRange.Melee, 0, MagicAttackEffectType.BreakClinch),
		new("Kinetic Parry", "kineticparry", 20, 2, MagicAttackRange.Melee, 0, Defense: MagicDefenseMode.Opposed, ReactionCost: 2),
		new("Phantom Doubles", "phantomdoubles", 60, 8, MagicAttackRange.Melee, 0, Defense: MagicDefenseMode.Charged, ReactionCost: 1),
		new("Kinetic Barrier", "kineticbarrier", 60, 10, MagicAttackRange.Melee, 0, Defense: MagicDefenseMode.Absorption, ReactionCost: 1, Capacity: 30)
	];
	public const string CombatDamageExpression = "4 + degree";
	public const string CombatPainExpression = "4 + degree";
	public const string CombatStunExpression = "6 + 2 * degree";
	public const double CombatAttackDelay = 3;
	public const double CombatAttackStamina = 2;
	public const int CombatDefenseSeconds = 120;
	public const double CombatDefenseUpkeep = 1;

	public static XElement CombatDefinition(PsionicCombatPower stock, long trait, long resource, long allowed,
		long error, long weaponAttack = 0)
	{
		var root = new XElement("Definition", new XElement("IsPsionic", true), new XElement("CanInvokePowerProg", allowed),
			new XElement("WhyCantInvokePowerProg", error),
			new XElement("PsionicTrace", new XElement("Enabled", true), new XElement("DurationSeconds", 900),
				new XElement("ReadDifficulty", (int)Difficulty.Hard), new XElement("Description", "a lingering psychic disturbance")),
			new XElement("InvocationCosts", new XElement("Verbs", new XElement("Verb", new XAttribute("verb", stock.Verb),
				new XElement("Cost", new XAttribute("resource", resource), stock.Cost)))));
		if (stock.Defense is { } defense)
		{
			root.Add(new XElement("BeginVerb", stock.Verb), new XElement("EndVerb", "end" + stock.Verb),
				new XElement("DefenseMode", defense), new XElement("DefenseTrait", trait), new XElement("DefenseDifficulty", (int)Difficulty.Normal),
				new XElement("Threats", defense == MagicDefenseMode.Absorption ? 15 : 31),
				new XElement("MaximumCharges", stock.Charges), new XElement("MaximumCapacity", stock.Capacity),
				new XElement("RequiresVision", true), new XElement("RequiresAttackerVision", defense == MagicDefenseMode.Charged),
				new XElement("ReactionStamina", 1), new XElement("ConcentrationPointsToSustain", 1), new XElement("SustainPenalty", -2),
				new XElement("Duration", CombatDefenseSeconds * 1000), new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
				new XElement("SustainResourceCosts", new XElement("Cost", new XAttribute("resource", resource), CombatDefenseUpkeep)));
			root.Element("InvocationCosts")!.Element("Verbs")!.Add(new XElement("Verb", new XAttribute("verb", "reaction"),
				new XElement("Cost", new XAttribute("resource", resource), stock.ReactionCost)));
		}
		else
		{
			root.Add(new XElement("Verb", stock.Verb), new XElement("WeaponAttack", weaponAttack), new XElement("AttackerTrait", trait),
				new XElement("MoveType", (int)BuiltInCombatMoveType.MagicPowerAttack), new XElement("Reach", 1),
				new XElement("AttackRange", stock.Range), new XElement("RangeInRooms", stock.Rooms), new XElement("DealsDamage", stock.Rider is null),
				new XElement("PowerIntentions", (long)(CombatMoveIntentions.Attack | (stock.Rider switch
				{
					MagicAttackEffectType.Knockdown => CombatMoveIntentions.Trip,
					MagicAttackEffectType.Disarm => CombatMoveIntentions.Disarm,
					null => CombatMoveIntentions.Wound,
					_ => CombatMoveIntentions.Disadvantage
				}))),
				new XElement("ValidDefenseTypes", new[] { DefenseType.Block, DefenseType.Dodge, DefenseType.Magic }
					.Concat(stock.Range == MagicAttackRange.Melee ? [DefenseType.Parry] : new DefenseType[0]).Select(x => new XElement("Defense", (int)x))));
			if (stock.Rider is { } rider)
				root.Add(new XElement("AttackEffects", new XElement("Effect", new XAttribute("type", rider),
					new XElement("Resistance", (int)Difficulty.Normal), new XElement("Strength", 1), new XElement("DurationSeconds", 2),
					new XElement("SuccessEmote", new XCData(PsionicPowerEmotes.Combat[stock.Verb]["RiderSuccess"])),
					new XElement("ResistEmote", new XCData("$1 resist|resists the pressure of $0's attack.")))));
		}
		foreach (var (field, echo) in PsionicPowerEmotes.Combat[stock.Verb].Where(x => x.Key != "RiderSuccess")) root.Add(new XElement(field, new XCData(echo)));
		return root;
	}
}
