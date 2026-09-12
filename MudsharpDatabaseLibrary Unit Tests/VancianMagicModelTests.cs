using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

#nullable enable
namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class VancianMagicModelTests
{
	[TestMethod]
	public void UpgradeDefaultsDoNotGrantKnowledgeOrScrollEligibility()
	{
		var migration = new VancianMagic(); var columns = migration.UpOperations.OfType<AddColumnOperation>().ToArray();
		Assert.AreEqual(0,columns.Single(x => x.Name == "SpellLevel").DefaultValue);
		Assert.AreEqual(false,columns.Single(x => x.Name == "ScrollInscriptionAllowed").DefaultValue);
		Assert.AreEqual(2,migration.UpOperations.OfType<CreateTableOperation>().Count());
		Assert.AreEqual(0,migration.UpOperations.OfType<InsertDataOperation>().Count());
		Assert.AreEqual(0,migration.UpOperations.OfType<SqlOperation>().Count());
	}
	[TestMethod]
	public void IdentityCapabilityKeyHasConcurrencyAndTombstonesOutliveDeletedOwnersAndItems()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var ledger = context.Model.FindEntityType(typeof(CharacterMagicCapabilityState))!;
		CollectionAssert.AreEqual(new[] { "CharacterId","MagicCapabilityId" },ledger.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.IsTrue(ledger.FindProperty("StateVersion")!.IsConcurrencyToken); Assert.AreEqual("longtext",ledger.FindProperty("Definition")!.GetColumnType());
		Assert.AreEqual(2,ledger.GetForeignKeys().Count()); Assert.IsTrue(ledger.GetForeignKeys().All(x => x.DeleteBehavior == DeleteBehavior.Cascade));
		var operations = context.Model.FindEntityType(typeof(VancianMagicOperation))!;
		Assert.AreEqual(0,operations.GetForeignKeys().Count()); Assert.AreEqual(1,operations.FindPrimaryKey()!.Properties.Count);
		Assert.AreEqual("Id",operations.FindPrimaryKey()!.Properties.Single().Name);
		var spell = context.Model.FindEntityType(typeof(MagicSpell))!;
		Assert.AreEqual(0,spell.FindProperty("SpellLevel")!.GetDefaultValue()); Assert.AreEqual(false,spell.FindProperty("ScrollInscriptionAllowed")!.GetDefaultValue());
	}
}
