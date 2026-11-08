#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Moq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using NativeExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonWorldfileNpcCombatTests
{
	[TestMethod]
	public void PrivateCorpus_AllWeaponsKeepMeleeAndGainSourceThrowReachAndChannels()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		using var fixture = new Fixture(source);
		fixture.Db.TraitDefinitions.Add(new TraitDefinition { Id = 990001, Name = "Strength", Type = 1 });
		var identity = 990010L;
		foreach (var name in new[] { "Club", "Short Spear", "Long Spear", "Knife", "Axe", "Longsword" })
		{
			fixture.Db.WeaponTypes.Add(new MudSharp.Models.WeaponType { Id = identity, Name = name, AttackTraitId = 17 });
			fixture.Db.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack { Id = identity, WeaponTypeId = identity, Name = name });
			identity++;
		}
		fixture.Db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcDamageDefinitions", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(fixture.Writer, null);
		var create = typeof(ArmageddonWorldfileWriter).GetMethod("CreateMeleeComponent", BindingFlags.NonPublic | BindingFlags.Instance)!;
		foreach (var item in source.Items.Where(x => x.ItemType is 5 or 24))
		{
			var components = ((GameItemComponentProto?, GameItemComponentProto?))create.Invoke(fixture.Writer, [item, item.Values, "coverage"])!;
			Assert.AreEqual("ThrownWeapon", components.Item1!.Type, "Native combined component for " + item.Vnum);
		}
		fixture.Db.SaveChanges();
		var runtime = fixture.NativeRuntime(); runtime.Actor.Values[990001] = 20;
		foreach (var ranged in fixture.Db.RangedWeaponTypes.Where(x => x.Id != 88040))
		{
			var item = source.Items.Single(x => ranged.Name.StartsWith("Armageddon Vnum " + x.Vnum + " ", StringComparison.Ordinal));
			Assert.AreEqual(ArmageddonWorldfileWriter.SourceThrowRange(item), ranged.DefaultRangeInRooms);
			Assert.AreEqual("damage", ranged.PainBonusExpression);
			Assert.AreEqual(ArmageddonWorldfileWriter.RangedStunFormula, ranged.StunBonusExpression);
			var damage = new NativeExpression(ranged.DamageBonusExpression, runtime.World.Object)
				.EvaluateWith(runtime.Actor.Mock.Object, values: [("variable", 50), ("range", 0)]);
			Assert.IsTrue(double.IsFinite(damage) && damage >= 0, "Finite native throw formula for " + item.Vnum);
		}
	}

	[TestMethod]
	public void CombatSettings_ReuseStockTemplatesAndMakeStationaryVariantsSelectable()
	{
		using var fixture = new Fixture();
		var identity = 991000L;
		foreach (var name in new[] { "Melee (Auto)", "Brawler (Auto)", "Beast Brawler", "Musketeer (Auto)", "Shielder (Auto)" })
			fixture.Db.CharacterCombatSettings.Add(new CharacterCombatSetting { Id = identity++, Name = name, Description = "Stock " + name,
				GlobalTemplate = true, ClassificationsAllowed = "1", WeaponUsePercentage = name.Contains("Brawler") ? 0 : 1,
				MeleeAttackOrderPreference = "0 1 2 3 4",
				NaturalWeaponPercentage = name.Contains("Brawler") ? 1 : 0,
				MovementManagement = (int)AutomaticMovementSettings.FullyAutomatic, RangedManagement = (int)AutomaticRangedSettings.FullyAutomatic });
		fixture.Db.SaveChanges();
		var resolve = typeof(ArmageddonWorldfileWriter).GetMethod("NpcCombatSetting", BindingFlags.Instance | BindingFlags.NonPublic)!;
		CharacterCombatSetting Setting(string role, bool stationary, bool humanoid) => (CharacterCombatSetting)resolve.Invoke(fixture.Writer, [role, stationary, humanoid])!;
		Assert.AreEqual("Melee (Auto)", Setting("melee", false, true).Name);
		Assert.AreEqual("Beast Brawler", Setting("natural", false, false).Name);
		var stationary = Setting("ranged", true, true);
		Assert.IsTrue(stationary.GlobalTemplate);
		Assert.IsTrue(stationary.AvailabilityProgId.HasValue);
		Assert.IsFalse(stationary.AutomaticallyMoveTowardsTarget);
		Assert.IsFalse(stationary.MoveToMeleeIfCannotEngageInRangedCombat);
		Assert.AreEqual((int)PursuitMode.NeverPursue, stationary.PursuitMode);
		Assert.AreEqual((int)CombatStrategyMode.FireNoCover, stationary.PreferredRangedMode);
		Assert.AreEqual((int)AutomaticMovementSettings.FullyAutomatic, stationary.MovementManagement, "Can still close inside its room.");
		Assert.AreSame(stationary, Setting("ranged", true, false), "Broad eligibility does not need duplicate profiles per species.");
		var throwing = Setting("throw", false, true);
		Assert.AreEqual("Throw and Close (Auto)", throwing.Name);
		Assert.AreEqual((int)CombatStrategyMode.StandardMelee, throwing.PreferredMeleeMode);
		Assert.IsFalse(throwing.Name.Contains("Armageddon"));
		var runtime = fixture.NativeRuntime();
		var availability = new Mock<IFutureProg>(); availability.SetupGet(x => x.Id).Returns(stationary.AvailabilityProgId!.Value);
		availability.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		var progs = new All<IFutureProg>(); progs.Add(availability.Object); runtime.World.SetupGet(x => x.FutureProgs).Returns(progs);
		var native = new CharacterCombatSettings(stationary, runtime.World.Object);
		Assert.IsTrue(native.CanUse(runtime.Actor.Mock.Object), "The actual native selector must admit this NPC.");
		availability.Verify(x => x.Execute<bool?>(It.IsAny<object[]>()), Times.Once);
	}
}
