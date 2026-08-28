#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCSkillPackageModelTests
{
	[TestMethod]
	public void Model_UsesNormalisedEntriesAndRequiredDeletionRules()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IDesignTimeModel>().Model;

		var package = model.FindEntityType(typeof(NpcSkillPackage));
		var entry = model.FindEntityType(typeof(NpcSkillPackageSkill));
		Assert.IsNotNull(package);
		Assert.IsNotNull(entry);
		Assert.IsTrue(package.GetIndexes().Single(x => x.Properties.Single().Name == nameof(NpcSkillPackage.Name)).IsUnique);
		CollectionAssert.AreEquivalent(
			new[] { nameof(NpcSkillPackageSkill.NpcSkillPackageId), nameof(NpcSkillPackageSkill.TraitDefinitionId) },
			entry.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());

		var packageForeignKey = entry.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(NpcSkillPackage));
		var traitForeignKey = entry.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(TraitDefinition));
		Assert.AreEqual(DeleteBehavior.Cascade, packageForeignKey.DeleteBehavior);
		Assert.AreEqual(DeleteBehavior.Restrict, traitForeignKey.DeleteBehavior);

		var raceNavigation = model.FindEntityType(typeof(Race))!.FindSkipNavigation(nameof(Race.NpcSkillPackages));
		Assert.IsNotNull(raceNavigation);
		Assert.AreEqual(DeleteBehavior.Cascade, raceNavigation.ForeignKey.DeleteBehavior);
		Assert.AreEqual(DeleteBehavior.Cascade, raceNavigation.Inverse.ForeignKey.DeleteBehavior);
	}

	[TestMethod]
	public void Migration_CreatesPackageEntryAndRaceLinkTables()
	{
		var builder = new MigrationBuilder("MySql");
		var migration = new AddNPCSkillPackages();
		var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(up);
		up.Invoke(migration, [builder]);

		var tables = builder.Operations.OfType<CreateTableOperation>().ToDictionary(x => x.Name);
		Assert.IsTrue(tables.ContainsKey("NPCSkillPackages"));
		Assert.IsTrue(tables.ContainsKey("NPCSkillPackageSkills"));
		Assert.IsTrue(tables.ContainsKey("Races_NPCSkillPackages"));
		Assert.AreEqual(ReferentialAction.Cascade,
			tables["NPCSkillPackageSkills"].ForeignKeys.Single(x => x.PrincipalTable == "NPCSkillPackages").OnDelete);
		Assert.AreEqual(ReferentialAction.Restrict,
			tables["NPCSkillPackageSkills"].ForeignKeys.Single(x => x.PrincipalTable == "TraitDefinitions").OnDelete);
	}
}
