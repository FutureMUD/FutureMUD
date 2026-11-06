#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private sealed record NpcDamageTraits(TraitDefinition Dice, TraitDefinition Sides, TraitDefinition Bonus,
		TraitDefinition Physical, TraitDefinition Stun);
	private NpcDamageTraits? _npcDamageTraits;
	private long _damageStrengthId;

	internal static string? NpcDamageCalibrationError(ArmageddonWorldfilePackage source) =>
		source.CombatCalibration is not { StrengthDamage.Length: 101, EnduranceSaves.Length: 101, EnduranceAgePermille.Length: 21 } ||
		source.PhysicalGenerators.Any(x => x.SizeBonuses is null || x.SizeBonuses.Count == 0 ||
			x.SizeBonuses.Any(y => y.Key < 0 || !double.IsFinite(y.Value) || y.Value <= 0) || Math.Abs(x.SizeBonuses.Values.Sum() - 1) > 1e-8)
			? "Regenerate the private worldfile package: Strength damage and Endurance calibration tables are required."
			: null;

	internal static (double HitPoints, double Stun) SourceNpcHealth(ArmWorldNpc npc, ArmWorldRace? race,
		ArmWorldGuild? guild, ArmWorldPhysical physical, ArmWorldCombatCalibration tables)
	{
		var endurance = SourceNpcStatDistribution(npc.Age, race, guild, 3, tables.EnduranceAgePermille!);
		double hp = 0, stun = 0;
		foreach (var (end, probability) in endurance)
			foreach (var (sizeBonus, sizeProbability) in physical.SizeBonuses!)
			{
				var modifier = tables.EnduranceSaves![end] + sizeBonus;
				hp += Math.Max(1, Math.Min(npc.Combat[4] + modifier, 10 * end)) * probability * sizeProbability;
				for (var die = 1; die <= 10; die++)
				{
					if (npc.Identity[3] != 4)
						stun += Math.Clamp(75 + end + die + modifier, 2 * end, 10 * end) * probability * sizeProbability / 10;
					else
						for (var first = 1; first <= 4; first++)
							for (var second = 1; second <= 4; second++)
								stun += Math.Clamp(75 + end + die + first + second + modifier, 2 * end, 10 * end) * probability * sizeProbability / 160;
				}
			}
		return (Math.Round(hp, 6), Math.Round(stun, 6));
	}

	private void InstallNpcDamageDefinitions()
	{
		if (NpcDamageCalibrationError(_source) is { } error) throw new InvalidOperationException(error);
		_damageStrengthId = _db.TraitDefinitions.Where(x => x.Type == 1)
			.OrderBy(x => x.Name == "Strength" ? 0 : 1)
			.First(x => x.Name == "Strength" || x.Name == "Upper Body Strength").Id;
		var profile = _npcCombatTraits!.Profile;
		TraitDefinition Rating(string name, string alias)
		{
			if (_db.TraitDefinitions.Any(x => x.Name == name)) throw new InvalidOperationException($"{name} already exists; this import is one-shot.");
			var cap = new TraitExpression { Id = Next<TraitExpression>(), Name = name + " Cap", Expression = "1000000" };
			_db.TraitExpressions.Add(cap);
			var trait = (TraitDefinition)_db.Entry(profile).CurrentValues.ToObject();
			trait.Id = Next<TraitDefinition>(); trait.Name = name; trait.Alias = alias; trait.ExpressionId = cap.Id;
			trait.ChargenBlurb = "Fixed imported damage profile; not a trainable skill.";
			_db.TraitDefinitions.Add(trait);
			Record("combat.damage." + alias, nameof(TraitDefinition), trait.Id, null, new { name, Trainable = false });
			return trait;
		}
		_npcDamageTraits = new(Rating("Worldfile Natural Dice", "naturaldice"), Rating("Worldfile Natural Sides", "naturalsides"),
			Rating("Worldfile Racial Damage", "racialdamage"), Rating("Worldfile Natural Physical", "naturalphysical"), Rating("Worldfile Natural Stun", "naturalstun"));
		Flush();
	}

	internal static (int Dice, int Sides, int Bonus) SourceNaturalDamage(ArmWorldRace? race)
	{
		if (race is null) return (0, 1, 0); // Source race zero has no usable attack allocation.
		var match = Regex.Match(race.Attack, @"^\s*(\d+)d(\d+)\+(-?\d+)\s*$", RegexOptions.IgnoreCase);
		if (!match.Success) throw new InvalidOperationException($"Unsupported natural damage generator for source race {race.Index}.");
		return (int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
			int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
	}

	// Item family indices; race records store the corresponding 300-based attack enum.
	internal static (double Physical, double Stun) SourceDamageMultipliers(int family) => family switch
	{
		1 => (0.9, 1.8),
		4 => (1, 0.5),
		5 or 6 or 7 => (1, 0.4),
		8 or 10 or 11 => (1, 0.7),
		12 => (1, 1.3),
		2 or 3 or 9 or 13 or 14 or 15 or 16 or 17 => (1, 1.1),
		_ => (0.25, 1.9)
	};

	private void ApplyNpcDamageProfile(ArmWorldNpc npc, ArmWorldRace? race, IDictionary<long, double> skills)
	{
		var traits = _npcDamageTraits!;
		var attack = SourceNaturalDamage(race);
		var factors = SourceDamageMultipliers((race?.Identity.ElementAtOrDefault(2) ?? 300) - 300);
		skills[traits.Dice.Id] = attack.Dice; skills[traits.Sides.Id] = attack.Sides; skills[traits.Bonus.Id] = attack.Bonus;
		skills[traits.Physical.Id] = factors.Physical; skills[traits.Stun.Id] = factors.Stun;
		if (race is null) Adapt("npc natural damage", npc.Vnum, "Source race zero has no allocated natural attack; retained native anatomy damage and zero racial damage bonus.");
	}

	internal static string StrengthDamageFormula(long strength, int[] table)
	{
		if (table.Length != 101) throw new ArgumentException("Strength damage needs 101 entries.", nameof(table));
		var value = table[100].ToString(CultureInfo.InvariantCulture);
		for (var index = 99; index >= 1; index--)
			if (table[index] != table[index + 1])
				value = $"if(floor(armdamage_strength:{strength})<= {index},{table[index].ToString(CultureInfo.InvariantCulture)},{value})";
		return value;
	}

	private string MeleeDamageRoll(int[] values)
	{
		var roll = values[1] > 0 && values[2] > 0 ? $"dice({values[1]},{values[2]})+({values[0]})" : "0";
		return $"max(1,({roll})+({StrengthDamageFormula(_damageStrengthId, _source.CombatCalibration!.StrengthDamage!)})+armdamage_racialdamage:{_npcDamageTraits!.Bonus.Id}{{raw,noimprove,nobranch}})";
	}

	private void CalibrateNpcNaturalAttacks()
	{
		var raceIds = new HashSet<long>();
		foreach (var race in _races.Values)
		{
			Race? current = race;
			while (current is not null && raceIds.Add(current.Id))
				current = current.ParentRaceId is { } parent ? _db.Races.Single(x => x.Id == parent) : null;
		}
		var attackIds = _db.RacesWeaponAttacks.Where(x => raceIds.Contains(x.RaceId)).Select(x => x.WeaponAttackId).Distinct().ToArray();
		var traits = _npcDamageTraits!;
		var roll = $"max(1,dice(armdamage_naturaldice:{traits.Dice.Id}{{raw,noimprove,nobranch}},armdamage_naturalsides:{traits.Sides.Id}{{raw,noimprove,nobranch}})+armdamage_racialdamage:{traits.Bonus.Id}{{raw,noimprove,nobranch}}+({StrengthDamageFormula(_damageStrengthId, _source.CombatCalibration!.StrengthDamage!)}))";
		foreach (var attack in _db.WeaponAttacks.Where(x => attackIds.Contains(x.Id) && x.MoveType == (int)BuiltInCombatMoveType.NaturalWeaponAttack).ToArray())
		{
			if (!double.IsFinite(attack.BaseAngleOfIncidence) || attack.BaseAngleOfIncidence <= 0)
				throw new InvalidOperationException($"Native natural attack {attack.Id} needs a positive incidence angle for damage calibration.");
			var normalization = (Math.PI / (2 * attack.BaseAngleOfIncidence)).ToString("R", CultureInfo.InvariantCulture);
			foreach (var channel in new[] { "damage", "pain", "stun" })
			{
				var originalId = channel == "damage" ? attack.DamageExpressionId : channel == "pain" ? attack.PainExpressionId : attack.StunExpressionId;
				var original = _db.TraitExpressions.Include(x => x.TraitExpressionParameters).Single(x => x.Id == originalId);
				var multiplier = channel == "stun" ? $"armdamage_naturalstun:{traits.Stun.Id}{{raw,noimprove,nobranch}}" : $"armdamage_naturalphysical:{traits.Physical.Id}{{raw,noimprove,nobranch}}";
				var locationMean = channel == "stun" ? "1.13" : "1.5";
				var expression = new TraitExpression
				{
					Id = Next<TraitExpression>(), Name = $"Worldfile natural {attack.Id} {channel}",
					Expression = $"if(armdamage_profile:{_npcCombatTraits!.Profile.Id}{{raw,noimprove,nobranch}}>0 and armdamage_naturaldice:{traits.Dice.Id}{{raw,noimprove,nobranch}}>0,min(200,({roll})*({multiplier})*{locationMean})*{normalization},({original.Expression}))"
				};
				foreach (var parameter in original.TraitExpressionParameters)
					expression.TraitExpressionParameters.Add(new TraitExpressionParameters { Parameter = parameter.Parameter,
						TraitDefinitionId = parameter.TraitDefinitionId, CanImprove = parameter.CanImprove, CanBranch = parameter.CanBranch });
				_db.TraitExpressions.Add(expression);
				if (channel == "damage") attack.DamageExpressionId = expression.Id;
				else if (channel == "pain") attack.PainExpressionId = expression.Id;
				else attack.StunExpressionId = expression.Id;
				Record($"combat.natural.{attack.Id}.{channel}", nameof(TraitExpression), expression.Id, null,
					new { OriginalExpressionId = original.Id, Formula = expression.Expression, Scope = "Profiled NPC ordinary natural attacks; original native fallback otherwise." });
			}
		}
		Flush();
	}
}
