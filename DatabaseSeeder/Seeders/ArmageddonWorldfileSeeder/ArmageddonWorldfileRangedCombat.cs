#nullable enable

using System;
using System.Globalization;
using System.Linq;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private long? _rangedArcheryId;
	private long RangedArcheryId => _rangedArcheryId ??= NpcSkillBindings(_db.TraitDefinitions.ToArray(),
		_db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == ArmageddonSkillPackage.Module).ToArray())["Archery"];

	private const string RangedLocationScale = "if(range>0,1.5,1)";
	// Physical damage has already received the averaged remote HP location multiplier.
	internal const string RangedStunFormula = "damage*if(range>0,1.13/3,1)";
	internal static string RangedCriticalMean(string skill) => $"(1+if(({skill})>0,(min(100,max(0,floor({skill})))+1)/501,0))";

	private string LauncherDamageFormula(int[] values)
	{
		var pull = values[1];
		if (pull is < 0 or > 100) throw new InvalidOperationException($"Source launcher pull {pull} is outside the qualified Strength table.");
		var strengthBonus = _source.CombatCalibration!.StrengthDamage![pull];
		var pullBonus = $"if(floor(armrange_strength:{_damageStrengthId})>{pull + 1},{strengthBonus},0)";
		return $"(({pullBonus})+floor(max(0,variable)/7))*{RangedCriticalMean("variable")}*{RangedLocationScale}+if(range=0,2,0)";
	}

	internal static int SourceThrowRange(ArmWorldItem source) => source.ItemType == 5 && (source.Flags[1] & (1 << 11)) != 0 ? 1 : 0;

	private string ThrownDamageFormula(ArmWorldItem source, int[] values)
	{
		var dice = source.ItemType == 24 ? values[0] : values[1];
		var sides = source.ItemType == 24 ? values[1] : values[2];
		var roll = dice > 0 && sides > 0 ? $"dice({dice},{sides})" : "0";
		var bonus = source.ItemType == 24 ? 0 : values[0];
		var strength = $"max(1,floor(armrange_strength:{_damageStrengthId}))";
		var skillBonus = $"max(0,min(100,floor(variable)-1))/100*(({strength})+1)/2";
		return $"max(0,({roll})+({bonus})+({StrengthDamageFormula(_damageStrengthId, _source.CombatCalibration!.StrengthDamage!)})+" +
			$"armrange_racialdamage:{_npcDamageTraits!.Bonus.Id}{{raw,noimprove,nobranch}}+({skillBonus}))*{RangedLocationScale}";
	}

	private void ConfigureSourceLauncher(RangedWeaponTypes ranged, int[] values)
	{
		ranged.FireTraitId = ranged.OperateTraitId = RangedArcheryId;
		ranged.DamageBonusExpression = LauncherDamageFormula(values);
		ranged.PainBonusExpression = "damage";
		ranged.StunBonusExpression = RangedStunFormula;
	}
}
