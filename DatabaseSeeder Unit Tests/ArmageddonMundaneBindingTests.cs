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
