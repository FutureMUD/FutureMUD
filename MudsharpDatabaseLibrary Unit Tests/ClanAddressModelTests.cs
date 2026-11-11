#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class ClanAddressModelTests
{
	[TestMethod]
	public void Upgrade_OptionalAddresses_DoNotPopulateExistingTitlesAndProtectPredicates()
	{
		var operations = new MudSharp.Migrations.ClanFormsOfAddress().UpOperations;
		Assert.IsFalse(operations.OfType<InsertDataOperation>().Any());
		Assert.IsFalse(operations.OfType<DropTableOperation>().Any());
		var columns = operations.OfType<AddColumnOperation>().ToList();
		Assert.AreEqual(true, columns.Single(x => x.Name == "ShowFormsOfAddressInLook").DefaultValue);
		Assert.AreEqual(0, columns.Single(x => x.Name == "AddressPriority").DefaultValue);
		foreach (var table in operations.OfType<CreateTableOperation>())
		{
			Assert.IsTrue(table.Columns.Single(x => x.Name == "Text").IsNullable, "Explicit none needs nullable text.");
			Assert.AreEqual(ReferentialAction.Restrict, table.ForeignKeys.Single(x => x.PrincipalTable == "FutureProgs").OnDelete);
			Assert.AreEqual(ReferentialAction.Cascade, table.ForeignKeys.Single(x => x.PrincipalTable != "FutureProgs").OnDelete);
		}
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void Model_StableIndependentRulesAndLookOptOut_HaveCorrectKeysAndDefaults()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		foreach (var type in new[] { typeof(RankFormOfAddress), typeof(AppointmentFormOfAddress) })
		{
			var model = context.Model.FindEntityType(type)!;
			Assert.AreEqual("Id", model.FindPrimaryKey()!.Properties.Single().Name);
			Assert.AreEqual(typeof(Guid), model.FindProperty("Id")!.ClrType);
			Assert.AreEqual(DeleteBehavior.Restrict, model.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(FutureProg)).DeleteBehavior);
		}
		var look = context.Model.FindEntityType(typeof(Clan))!.FindProperty(nameof(Clan.ShowFormsOfAddressInLook))!;
		Assert.AreEqual(true, look.GetDefaultValue());
		Assert.AreEqual(true, look.Sentinel, "Explicit false must be sent on insert, not replaced by the true database default.");
	}
}
