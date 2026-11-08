#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Models;
using NativeExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonWorldfileNpcCombatTests
{
	[TestMethod]
	public void MeleeDamage_UsesLiveStrengthRacialBonusAndDistinctChannelsWithoutQualityOrDegree()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		using var fixture = new Fixture(source);
		fixture.Db.TraitDefinitions.Add(new TraitDefinition { Id = 990001, Name = "Strength", Type = 1 });
		fixture.Db.WeaponTypes.Add(new MudSharp.Models.WeaponType { Id = 990010, Name = "Longsword" });
		fixture.Db.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack { Id = 990010, Name = "native slash", WeaponTypeId = 990010 });
		fixture.Db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcDamageDefinitions", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(fixture.Writer, null);
		var item = source.Items.Single(x => x.Vnum == 1103) with { Values = [0, 1, 1, 5, 0, 0] };
		typeof(ArmageddonWorldfileWriter).GetMethod("CreateMeleeComponent", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(fixture.Writer, [item, item.Values, "fixture"]);
		fixture.Db.SaveChanges();
		var attack = fixture.Db.WeaponAttacks.Single(x => x.Name == "Armageddon Vnum 1103 attack");
		Assert.AreEqual(Math.PI / 2, attack.BaseAngleOfIncidence);
		var runtime = fixture.NativeRuntime();
		runtime.Actor.Values[fixture.Db.TraitDefinitions.Single(x => x.Alias == "racialdamage").Id] = 3;
		foreach (var strength in new[] { 10, 20, 50 })
		{
			runtime.Actor.Values[990001] = strength;
			var basis = Math.Max(1, 1 + 3 + source.CombatCalibration!.StrengthDamage![strength]);
			Assert.AreEqual(Math.Min(200, basis * 1.5), runtime.Expressions.Get(attack.DamageExpressionId).Evaluate(runtime.Actor.Mock.Object), 1e-8);
			Assert.AreEqual(Math.Min(200, basis * .4 * 1.13), runtime.Expressions.Get(attack.StunExpressionId).Evaluate(runtime.Actor.Mock.Object), 1e-8);
		}
		var formula = fixture.Db.TraitExpressions.Single(x => x.Id == attack.DamageExpressionId).Expression;
		Assert.IsFalse(formula.Contains("quality") || formula.Contains("degree"));
	}

	[TestMethod]
	public void DamageProfiles_RetainSourceFamilyDistinctionsAndSafeMissingRace()
	{
		Assert.AreEqual((0.25, 1.9), ArmageddonWorldfileWriter.SourceDamageMultipliers(0));
		Assert.AreEqual((0.9, 1.8), ArmageddonWorldfileWriter.SourceDamageMultipliers(1));
		Assert.AreEqual((1d, 0.4), ArmageddonWorldfileWriter.SourceDamageMultipliers(5));
		Assert.AreEqual((1d, 1.1), ArmageddonWorldfileWriter.SourceDamageMultipliers(15));
		Assert.AreEqual((0, 1, 0), ArmageddonWorldfileWriter.SourceNaturalDamage(null));
		var race = new ArmWorldRace(new("fixture", 1), 1, "fixture", "f", [], [], "2d12+-8", [], []);
		Assert.AreEqual((2, 12, -8), ArmageddonWorldfileWriter.SourceNaturalDamage(race));
	}

	[TestMethod]
	public void StrengthFormula_UsesThresholdsAndLiveEffectiveTrait()
	{
		using var fixture = new Fixture();
		var runtime = fixture.NativeRuntime();
		var skill = runtime.Weapon;
		var table = new int[101];
		for (var index = 1; index <= 100; index++) table[index] = index / 5 - 2;
		var expression = new NativeExpression(ArmageddonWorldfileWriter.StrengthDamageFormula(skill.Id, table), runtime.World.Object);
		foreach (var strength in new[] { 1d, 9.8, 10, 13.9, 14.1, 50, 100 })
		{
			runtime.Actor.Values[skill.Id] = strength;
			Assert.AreEqual((double)table[(int)Math.Floor(strength)], expression.Evaluate(runtime.Actor.Mock.Object));
		}
	}

	[TestMethod]
	public void PrivateCorpus_HealthPoolsIncludeEnduranceSizeAndSourceCaps()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		Assert.IsNull(ArmageddonWorldfileWriter.NpcDamageCalibrationError(source));
		foreach (var (vnum, hp, stun) in new[] { (49000, 114.8438, 105.8438), (50431, 129.6875, 110.8438),
			(7918, 191.6463, 185.4380), (24001, 235d, 235d), (34003, 211.7654, 237.2654) })
		{
			var npc = source.Npcs.Single(x => x.Vnum == vnum);
			var pools = ArmageddonWorldfileWriter.SourceNpcHealth(npc, source.Races.SingleOrDefault(x => x.Index == npc.Identity[6]),
				source.Guilds.SingleOrDefault(x => x.Index == npc.Identity[3]), source.PhysicalGenerators.Single(x => x.Race == npc.Identity[6]), source.CombatCalibration!);
			Assert.AreEqual(hp, pools.HitPoints, 0.01, "HP " + vnum);
			Assert.AreEqual(stun, pools.Stun, 0.01, "Stun " + vnum);
		}
	}

	[TestMethod]
	public void NaturalDamage_WrapsInheritedAttacksWithoutChangingNativeFallbackOrSpecialActions()
	{
		var tables = new ArmWorldCombatCalibration(new int[101], Enumerable.Repeat(1000, 21).ToArray(),
			Enumerable.Repeat(1000, 21).ToArray(), Enumerable.Repeat(2, 101).ToArray(), Enumerable.Repeat(1000, 21).ToArray(), new int[101]);
		using var fixture = new Fixture(new ArmageddonWorldfilePackage { CombatCalibration = tables });
		fixture.Db.TraitDefinitions.Add(new TraitDefinition { Id = 990001, Name = "Strength", Type = 1 });
		fixture.Db.SaveChanges();
		void Invoke(string method) => typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(fixture.Writer, null);
		Invoke("InstallNpcDamageDefinitions");
		var race = fixture.Db.Races.First();
		var raceMap = (Dictionary<int, Race>)typeof(ArmageddonWorldfileWriter).GetField("_races", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture.Writer)!;
		raceMap.Add(1, race);
		foreach (var (id, move) in new[] { (990010L, BuiltInCombatMoveType.NaturalWeaponAttack), (990011L, BuiltInCombatMoveType.BreathWeaponAttack) })
		{
			var expression = new TraitExpression { Id = id, Name = "original", Expression = "17" };
			fixture.Db.TraitExpressions.Add(expression);
			fixture.Db.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack { Id = id, Name = "fixture", MoveType = (int)move, BaseAngleOfIncidence = Math.PI / 4,
				DamageExpressionId = id, PainExpressionId = id, StunExpressionId = id });
			fixture.Db.RacesWeaponAttacks.Add(new RacesWeaponAttacks { RaceId = race.Id, WeaponAttackId = id, BodypartId = 1 });
		}
		fixture.Db.SaveChanges();
		Invoke("CalibrateNpcNaturalAttacks");
		Assert.AreEqual(990011L, fixture.Db.WeaponAttacks.Single(x => x.Id == 990011).DamageExpressionId);
		var runtime = fixture.NativeRuntime();
		var attack = fixture.Db.WeaponAttacks.Single(x => x.Id == 990010);
		var damage = runtime.Expressions.Get(attack.DamageExpressionId);
		var stun = runtime.Expressions.Get(attack.StunExpressionId);
		Assert.AreEqual(17d, damage.Evaluate(runtime.Actor.Mock.Object));
		runtime.Actor.SetProfile(50, 50);
		foreach (var (alias, value) in new[] { ("naturaldice", 1d), ("naturalsides", 1d), ("racialdamage", 3d), ("naturalphysical", 0.25), ("naturalstun", 1.9) })
			runtime.Actor.Values[fixture.Db.TraitDefinitions.Single(x => x.Alias == alias).Id] = value;
		Assert.AreEqual(4.5, damage.Evaluate(runtime.Actor.Mock.Object), 1e-8, "Neutral angle normalization is undone downstream: 6 raw × .25 × 1.5 × 2.");
		Assert.AreEqual(6 * 1.9 * 1.13 * 2, stun.Evaluate(runtime.Actor.Mock.Object), 1e-8);
		runtime.Actor.Values[fixture.Db.TraitDefinitions.Single(x => x.Alias == "naturaldice").Id] = 0;
		Assert.AreEqual(17d, damage.Evaluate(runtime.Actor.Mock.Object), "Missing historical race retains native natural damage.");
	}
}
