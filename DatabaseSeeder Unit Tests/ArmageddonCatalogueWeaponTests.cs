#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using DbWeaponType = MudSharp.Models.WeaponType;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueWeaponTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Weapons_ExactTypedNativeDependencies_StableRerunAndNoLiveEntities(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueWeapons.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(49, result.Identities.Count);
		Assert.AreEqual(113, db.GameItemProtos.Count());
		Assert.AreEqual(49, db.WeaponAttacks.Count());
		Assert.AreEqual(49, db.CombatMessages.Count());
		Assert.AreEqual(49, db.CombatMessagesWeaponAttacks.Count());
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
		Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRoles.Count());
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.MeleeItem))
		{
			var item = db.GameItemProtos.Single(x => x.Id == result.Identities[blueprint.Key]);
			Assert.AreEqual(blueprint.Configuration.GetProperty("weight_kilograms").GetDouble() * 1000, item.Weight);
			var attackId = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".attack").LogicalId!.Value;
			var attack = db.WeaponAttacks.Single(x => x.Id == attackId);
			var componentId = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".component").LogicalId!.Value;
			var definition = XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == componentId).Definition);
			Assert.AreEqual(attack.WeaponTypeId, (long?)definition.Element("WeaponType")!);
			Assert.AreEqual(blueprint.Configuration.GetProperty("native_damage_type").GetString(), ((MudSharp.Health.DamageType)attack.DamageType).ToString());
			Assert.IsTrue(db.CombatMessagesWeaponAttacks.Any(x => x.WeaponAttackId == attack.Id));
		}
		var owned = db.SeederManagedRecords.Where(x => x.Module.StartsWith("stock-catalogue")).ToArray();
		var before = owned.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueWeapons.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords.Where(x => x.Module.StartsWith("stock-catalogue")))
			Assert.AreEqual(before[record.StableKey], (record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		ArmageddonInstallation.Prepare(factory, mode);
	}

	[TestMethod]
	public void Weapons_RealNativeAttacksEvaluateAndMeleeComponentsLoad()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var result = ArmageddonCatalogueWeapons.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var world = new Mock<IFuturemud>();
		var traits = new All<ITraitDefinition>(); world.SetupGet(x => x.Traits).Returns(traits);
		foreach (var row in db.TraitDefinitions)
		{
			var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(row.Id); traits.Add(trait.Object);
		}
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.BodypartShapes).Returns(new All<MudSharp.Form.Shape.IBodypartShape>());
		var expressions = new All<ITraitExpression>(); world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		foreach (var row in db.TraitExpressions) expressions.Add(new MudSharp.Body.Traits.TraitExpression(row, world.Object));
		PositionState.SetupPositions();
		var attacks = new All<IWeaponAttack>(); world.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		foreach (var row in db.WeaponAttacks) attacks.Add(MudSharp.Combat.WeaponAttack.LoadWeaponAttack(row, world.Object));
		var types = new All<IWeaponType>(); world.SetupGet(x => x.WeaponTypes).Returns(types);
		foreach (var row in db.WeaponTypes.Include(x => x.WeaponAttacks)) types.Add(new MudSharp.Combat.WeaponType(row, world.Object));
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		actor.SetupGet(x => x.CombatSettings).Returns(new Mock<ICharacterCombatSettings>().Object);
		var manager = new GameItemComponentManager();
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.MeleeItem))
		{
			var attackId = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".attack").LogicalId!.Value;
			var attack = attacks.Get(attackId)!;
			Assert.AreEqual(blueprint.Configuration.GetProperty("native_attack_damage").GetDouble(), attack.Profile.DamageExpression.Evaluate(actor.Object));
			Assert.AreEqual(blueprint.Configuration.GetProperty("native_attack_pain").GetDouble(), attack.Profile.PainExpression.Evaluate(actor.Object));
			Assert.AreEqual(0.0, attack.Profile.StunExpression.Evaluate(actor.Object));
			Assert.IsTrue(attack.UsableAttack(actor.Object, null, null, AttackHandednessOptions.OneHandedOnly, false, BuiltInCombatMoveType.UseWeaponAttack));
			var componentId = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".component").LogicalId!.Value;
			var row = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == componentId);
			var component = manager.GetProto(row, world.Object);
			Assert.IsTrue(component is MudSharp.GameItems.Prototypes.MeleeWeaponGameItemComponentProto && component.CanSubmit(), blueprint.Key);
		}
	}

	[TestMethod]
	public void Weapons_BuilderEditsAndRenamedSharedCombatTypes_Preserved()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueWeapons.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var item = db.GameItemProtos.Single(x => x.Id == first.Identities["arm.item.spark_lash.g1"]);
		item.ShortDescription = "a builder's sparkling lash";
		var attackId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.spark_lash.g1.attack").LogicalId;
		db.WeaponAttacks.Single(x => x.Id == attackId).StaminaCost = 8;
		db.WeaponTypes.Single(x => x.Name == "Improvised Bludgeon").Name = "Builder renamed improvised combat";
		db.SaveChanges();
		var rerun = ArmageddonCatalogueWeapons.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("a builder's sparkling lash", db.GameItemProtos.Single(x => x.Id == item.Id).ShortDescription);
		Assert.AreEqual(8.0, db.WeaponAttacks.Single(x => x.Id == attackId).StaminaCost);
		Assert.IsTrue(rerun.Messages.Any(x => x.Contains("preserved builder edit")));
	}

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode, bool classes)
	{
		var db = ArmageddonStockCatalogueTests.Fixture(mode, classes);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		var traitId = db.Languages.Single().LinkedTraitId;
		foreach (var name in new[] { "Knife", "Longsword", "Club", "Improvised Bludgeon", "Short Spear" })
			db.WeaponTypes.Add(new DbWeaponType { Name = name, AttackTraitId = traitId, ParryTraitId = traitId, Reach = 1, StaminaPerParry = 1.5 });
		db.SaveChanges();
		return db;
	}
}
