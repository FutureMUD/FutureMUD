#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSupportInstallerTests
{
	[DataTestMethod]
	[DataRow(TraitOwnerScope.Body, TraitOwnerScope.Body)]
	[DataRow(TraitOwnerScope.Character, TraitOwnerScope.Body)]
	[DataRow(TraitOwnerScope.Body, TraitOwnerScope.Character)]
	[DataRow(TraitOwnerScope.Character, TraitOwnerScope.Character)]
	public void MasterIntegration_SeparateNativeParrySupportsLegacyAndCurrentOwnership(TraitOwnerScope scope, TraitOwnerScope attackScope)
	{
		long parryId = 0;
		using var db = FixtureWithCombat(ArmageddonProgressionMode.Simple, false, context =>
		{
			context.TraitDefinitions.AddRange(
				new TraitDefinition { Name = "Strength", Alias = "str", Type = 1 },
				new TraitDefinition { Name = "Agility", Alias = "agi", Type = 1 });
			context.TraitDecorators.Add(new TraitDecorator { Name = "Veterancy Skill" });
			context.SaveChanges();
			var seed = typeof(CombatSeeder).GetMethod("SeedUniversalSkills", BindingFlags.NonPublic | BindingFlags.Instance)!;
			var skills = (IReadOnlyDictionary<string, TraitDefinition>)seed.Invoke(new CombatSeeder(),
				[context, new Dictionary<string, string> { ["parryoption"] = "yes" }])!;
			var parry = skills["Parry"];
			Assert.AreEqual((int)TraitOwnerScope.Character, parry.OwnerScope);
			parry.OwnerScope = (int)scope; // Retain compatibility with existing body-owned foundations too.
			parryId = parry.Id;
			foreach (var weapon in context.WeaponTypes)
			{
				weapon.ParryTraitId = parryId;
				context.TraitDefinitions.Single(x => x.Id == weapon.AttackTraitId).OwnerScope = (int)attackScope;
			}
			context.SaveChanges();
			context.ChangeTracker.Clear();
		});
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(49, db.SeederManagedRecords.Count(x => x.StableKey.EndsWith(".weapon")));
		var result = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(65, db.SeederManagedRecords.Count(x => x.StableKey.EndsWith(".weapon")));
		Assert.IsTrue(db.WeaponTypes.All(x => x.ParryTraitId == parryId));
		Assert.AreEqual((int)scope, db.TraitDefinitions.Single(x => x.Id == parryId).OwnerScope);
		Assert.IsTrue(db.WeaponTypes.All(x => db.TraitDefinitions.Any(t => t.Id == x.AttackTraitId && t.OwnerScope == (int)attackScope)));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSupportInstaller.Install(factory).Status);
		Assert.AreEqual(0, db.Characters.Count());
		Assert.AreEqual(0, db.GameItems.Count());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MasterIntegration_UnknownCombatSkillOwnershipStillRefusesWeapons(bool invalidAttack)
	{
		using var db = ArmageddonCatalogueWeaponTests.Fixture(ArmageddonProgressionMode.Simple, false);
		var parry = new TraitDefinition { Name = "Invalid foundation", Type = 0, OwnerScope = 99 };
		db.TraitDefinitions.Add(parry);
		db.SaveChanges();
		foreach (var weapon in db.WeaponTypes)
		{
			if (invalidAttack) weapon.AttackTraitId = parry.Id;
			else weapon.ParryTraitId = parry.Id;
		}
		db.SaveChanges();
		var result = ArmageddonCatalogueWeapons.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreNotEqual(ArmageddonInstallStatus.Completed, result.Status);
		Assert.IsTrue(result.Messages.Any(x => x.Contains("native attack/parry skills")));
		Assert.AreEqual(0, db.WeaponAttacks.Count());
	}
}
