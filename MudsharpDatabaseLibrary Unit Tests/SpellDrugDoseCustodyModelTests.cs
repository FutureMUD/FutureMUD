#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellDrugDoseCustodyModelTests
{
	[TestMethod]
	public void Migration_PreservesLegacyDosesAndAddsSeparateNullableOriginCustody()
	{
		var operations = new SpellDrugDoseCustody().UpOperations;
		var additions = operations.OfType<AddColumnOperation>().ToDictionary(x => x.Name);
		CollectionAssert.AreEquivalent(new[] { "SpellOriginId", "SpellOriginFamily", "SpellOriginFormula" }, additions.Keys.ToArray());
		Assert.IsTrue(additions["SpellOriginId"].IsNullable);
		Assert.IsTrue(additions["SpellOriginFamily"].IsNullable);
		Assert.IsTrue(additions["SpellOriginFormula"].IsNullable);
		var indexPosition = Array.FindIndex(operations.ToArray(), x => x is CreateIndexOperation);
		var keyChangePosition = Array.FindIndex(operations.ToArray(), x => x is SqlOperation);
		Assert.IsTrue(indexPosition >= 0 && keyChangePosition > indexPosition,
			"The BodyId foreign key needs an alternate index before its old primary is dropped.");
		StringAssert.Contains(operations.OfType<SqlOperation>().Single().Sql, "ADD PRIMARY KEY (`Id`)");
		Assert.IsTrue(operations.OfType<CreateIndexOperation>().Any(x => x.Table == "Bodies_DrugDoses" &&
			x.Columns.SequenceEqual(new[] { "BodyId" })));
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void Model_UsesPerDoseIdentityAndKeepsDrugAndBodyRelationships()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=127.0.0.1;database=unused;uid=unused;password=unused",
				ServerVersion.Parse("8.0.36-mysql")).Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(BodyDrugDose))!;
		CollectionAssert.AreEqual(new[] { nameof(BodyDrugDose.Id) },
			model.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(ValueGenerated.OnAdd, model.FindProperty(nameof(BodyDrugDose.Id))!.ValueGenerated);
		Assert.IsTrue(model.FindProperty(nameof(BodyDrugDose.SpellOriginId))!.IsNullable);
		Assert.IsTrue(model.FindProperty(nameof(BodyDrugDose.SpellOriginFamily))!.IsNullable);
		Assert.IsTrue(model.FindProperty(nameof(BodyDrugDose.SpellOriginFormula))!.IsNullable);
		Assert.AreEqual(2, model.GetForeignKeys().Count());
	}
}
