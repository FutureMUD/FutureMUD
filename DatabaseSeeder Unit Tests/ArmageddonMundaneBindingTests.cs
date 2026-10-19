#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonMundaneBindingTests
{
	[TestMethod]
	public void SourceImpacts_BindSeparateCanonicalActionsAndManualCommands()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var root = new Race { Name = "Organic Humanoid" }; db.Races.Add(root);
		foreach (var verb in new[] { "bash", "kick" }) db.ManualCombatCommands.Add(new ManualCombatCommand { Name = verb, PrimaryVerb = verb });
		var original = new CombatAction { Name = "Bash", UsabilityProgId = 2 }; db.CombatActions.Add(original); db.SaveChanges();
		var skills = ArmageddonSkillPackage.Resolve(db);
		ArmageddonSourceImpacts.InstallFresh(db, skills);
		Assert.AreEqual(2, db.RacesCombatActions.Count(x => x.RaceId == root.Id));
		foreach (var verb in new[] { "bash", "kick" })
		{
			var command = db.ManualCombatCommands.Single(x => x.PrimaryVerb == verb);
			var action = db.CombatActions.Single(x => x.Id == command.CombatActionId);
			Assert.AreEqual(skills["arm.skill." + verb].Id, action.TraitDefinitionId);
			Assert.IsTrue(command.PlayerUsable && command.NpcUsable);
			Assert.IsNull(command.WeaponAttackId);
			Assert.AreEqual(1, action.MaximumTargets);
			var profile = XElement.Parse(action.AdditionalInfo).Element("PhysicalImpact")!;
			Assert.AreEqual(verb == "bash", (bool)profile.Attribute("seatonmiss")!);
			Assert.AreEqual(verb == "bash", (bool)profile.Attribute("seatonhit")!);
			Assert.AreEqual(verb == "kick", (bool)profile.Attribute("shieldblock")!);
			Assert.IsTrue((bool)profile.Attribute("initiatecombat")!);
			Assert.AreEqual(skills["arm.skill.guarding"].Id, (long)profile.Attribute("guardtrait")!);
		}
		Assert.AreEqual(2L, original.UsabilityProgId);
		Assert.AreEqual(6, db.SeederManagedRecords.Count(x => x.StableKey.StartsWith("arm.action.") || x.StableKey.StartsWith("arm.command.")));
	}

	[TestMethod]
	public void OpeningProfiles_BindSpecificSkillsAndCommandsWithoutChangingDamage()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		foreach (var name in new[] { "Knife Backstab", "Dagger Backstab", "Blackjack Sapping Blow" })
			db.WeaponAttacks.Add(new WeaponAttack { Name = name, FutureProgId = 2, AdditionalInfo = "<Backstab><Reel>0.75</Reel></Backstab>", DamageExpressionId = 42 });
		db.SaveChanges();
		var skills = ArmageddonSkillPackage.Resolve(db);
		ArmageddonMundaneBindings.BindOpeningAttacks(db, skills); db.SaveChanges();
		foreach (var row in db.WeaponAttacks)
		{
			var sap = row.Name == "Blackjack Sapping Blow";
			var xml = XElement.Parse(row.AdditionalInfo);
			Assert.AreEqual(skills[sap ? "arm.skill.sap" : "arm.skill.backstab"].Id, (long)xml.Element("Proficiency")!);
			Assert.AreEqual(sap ? "sap" : "backstab", xml.Element("Command")!.Value);
			Assert.AreEqual("0.75", xml.Element("Reel")!.Value);
			Assert.AreEqual(42L, row.DamageExpressionId);
			Assert.AreEqual(1L, row.FutureProgId);
		}
	}

	[TestMethod]
	public void EarlyChecks_UseGrantedSkillsAndSeparateRidingControlFromFlee()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var skills = ArmageddonSkillPackage.Resolve(db);
		string Formula(CheckType type) => db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)type).TraitExpression.Expression;
		foreach (var type in new[] { CheckType.SearchForTracksCheck, CheckType.SearchForTracksByScentScheck }) Assert.AreEqual($"hunt:{skills["arm.skill.hunt"].Id}", Formula(type));
		foreach (var type in new[] { CheckType.FleeMeleeCheck, CheckType.OpposeFleeMeleeCheck, CheckType.FleeMovementMountedCheck, CheckType.FleeMovementUnmountedCheck })
			Assert.AreEqual($"flee:{skills["arm.skill.flee"].Id}", Formula(type));
		foreach (var type in new[] { CheckType.RescueCheck, CheckType.OpposeRescueCheck }) Assert.AreEqual($"rescue:{skills["arm.skill.rescue"].Id}", Formula(type));
		Assert.AreEqual($"ride:{skills["arm.skill.ride"].Id}", Formula(CheckType.MountControlCheck));
		Assert.AreEqual($"ride:{skills["arm.skill.ride"].Id}", Formula(CheckType.AvoidMountFallCheck));
		StringAssert.Contains(Formula(CheckType.MountedChargeCheck), $"charge:{skills["arm.skill.charge"].Id}");
		StringAssert.Contains(Formula(CheckType.VehicleChargeCheck), $"drive:{skills["arm.skill.pilot"].Id}");
		StringAssert.Contains(Formula(CheckType.PursuitMovementUnmountedCheck), $"veterancy:{skills["arm.scaffold.veterancy"].Id}");
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, new ArmageddonSettingSeeder().ShouldSeedData(db));
	}

	[TestMethod]
	public void CheckHandoff_ChangesTemplateWithoutReplacingBuilderFormulaOrPointer()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var check = db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.FleeMeleeCheck);
		var id = check.TraitExpressionId;
		check.TraitExpression.Expression = "73";
		var template = new CheckTemplate { Name = "Custom handoff template" }; db.CheckTemplates.Add(template); db.SaveChanges();
		var messages = new List<string>();
		ArmageddonMundaneBindings.EnsureCheck(db, CheckType.FleeMeleeCheck, "0", template.Id, Difficulty.Impossible, messages);
		Assert.AreEqual(id, check.TraitExpressionId);
		Assert.AreEqual("73", check.TraitExpression.Expression);
		Assert.AreEqual(template.Id, check.CheckTemplateId);
		var custom = new TraitExpression { Name = "Builder alternate", Expression = "42" }; db.TraitExpressions.Add(custom); db.SaveChanges();
		check.TraitExpressionId = custom.Id; db.SaveChanges();
		ArmageddonMundaneBindings.EnsureCheck(db, CheckType.FleeMeleeCheck, "0", template.Id, Difficulty.Impossible, messages);
		Assert.AreEqual(custom.Id, check.TraitExpressionId);
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void DisarmBinding_ChangesOnlyItsAttackerAndDisarmDefenseDependencies()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var skills = ArmageddonSkillPackage.Resolve(db);
		var scaffold = skills["arm.scaffold.auxiliary"].Id;
		foreach (var name in new[] { "Pommel Beat", "Wrist Check", "Beat the Weapon" }) db.CombatActions.Add(new CombatAction
		{
			Name = name, TraitDefinitionId = scaffold,
			AdditionalInfo = $"<Effects><Effect type='disarm' defensetrait='{scaffold}'/><Effect type='targetdelay' defensetrait='{scaffold}'/></Effects>"
		});
		db.SaveChanges();
		ArmageddonMundaneBindings.BindDisarm(db, skills); db.SaveChanges();
		foreach (var action in db.CombatActions)
		{
			Assert.AreEqual(skills["arm.skill.disarm"].Id, action.TraitDefinitionId);
			var xml = XElement.Parse(action.AdditionalInfo);
			Assert.AreEqual(skills["arm.skill.parry"].Id, (long)xml.Elements().Single(x => (string?)x.Attribute("type") == "disarm").Attribute("defensetrait")!);
			Assert.AreEqual(scaffold, (long)xml.Elements().Single(x => (string?)x.Attribute("type") == "targetdelay").Attribute("defensetrait")!);
		}
	}

	[TestMethod]
	public void OptionalTrapCheck_RejectsUnownedCheckBeforeCreatingOwnershipOrChangingIt()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var expression = new TraitExpression { Name = "Builder trap formula", Expression = "73" };
		db.TraitExpressions.Add(expression); db.SaveChanges();
		var check = new Check { Type = (int)CheckType.SetTrapCheck, TraitExpressionId = expression.Id,
			CheckTemplateId = db.CheckTemplates.First().Id, MaximumDifficultyForImprovement = (int)Difficulty.Normal };
		db.Checks.Add(check); db.SaveChanges();
		var count = db.TraitExpressions.Count();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonMundaneBindings.EnsureCheck(db,
			CheckType.SetTrapCheck, "0", check.CheckTemplateId, Difficulty.Impossible, new List<string>(), rejectUnowned: true));
		Assert.AreEqual(expression.Id, check.TraitExpressionId);
		Assert.AreEqual("73", expression.Expression);
		Assert.AreEqual((int)Difficulty.Normal, check.MaximumDifficultyForImprovement);
		Assert.AreEqual(count, db.TraitExpressions.Count());
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.check.settrapcheck.binding"));
	}

	[TestMethod]
	public void OptionalTrapChecks_ReuseCanonicalCharacterSkillWithoutCreatingStockTraps()
	{
		using var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		var skillCount = db.TraitDefinitions.Count();
		var trap = (TraitDefinition)typeof(TrapSeeder).GetMethod("EnsureTrapsSkill", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [db])!;
		Assert.AreEqual(ArmageddonSkillPackage.Resolve(db)["arm.skill.trap"].Id, trap.Id);
		typeof(TrapSeeder).GetMethod("EnsureChecks", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [db, trap]);
		Assert.AreEqual(skillCount, db.TraitDefinitions.Count());
		Assert.IsFalse(db.TraitDefinitions.Any(x => x.Name == "Traps"));
		Assert.AreEqual($"traps:{trap.Id}", db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.SetTrapCheck).TraitExpression.Expression);
	}
}
