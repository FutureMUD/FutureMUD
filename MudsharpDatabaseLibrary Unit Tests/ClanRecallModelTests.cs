using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class ClanRecallModelTests
{
	[TestMethod]
	public void Upgrade_ExistingOfficesKeepPermissionsDisabledAndThresholdDefaultsToMajority()
	{
		var operations = new MudSharp.Migrations.ClanAppointmentRecall().UpOperations;
		var columns = operations.OfType<AddColumnOperation>().ToDictionary(x => (x.Table, x.Name));
		Assert.AreEqual(0UL, Convert.ToUInt64(columns[("Appointments", "CanParentDismiss")].DefaultValue));
		Assert.AreEqual(0UL, Convert.ToUInt64(columns[("Appointments", "CanBeRecalled")].DefaultValue));
		Assert.AreEqual(0UL, Convert.ToUInt64(columns[("Elections", "IsRecall")].DefaultValue));
		Assert.AreEqual(0.5, columns[("Appointments", "RecallThreshold")].DefaultValue);
		Assert.AreEqual(0.5, columns[("Elections", "RecallThreshold")].DefaultValue);
		Assert.IsTrue(columns[("Appointments", "CanInitiateRecallProgId")].IsNullable);
		Assert.IsTrue(columns[("Appointments", "RecallVotesProgId")].IsNullable);
		Assert.HasCount(2, operations.OfType<AddForeignKeyOperation>().ToList());
		Assert.IsTrue(operations.OfType<AddForeignKeyOperation>().All(x => x.OnDelete == ReferentialAction.SetNull));
		Assert.IsFalse(operations.OfType<DropTableOperation>().Any());
	}

	[TestMethod]
	public void RecallThreshold_ExplicitZeroIsSentOnInsertInsteadOfReceivingCompatibilityDefault()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		foreach (var type in new[] { typeof(Appointment), typeof(Election) })
		{
			var property = context.Model.FindEntityType(type)!.FindProperty("RecallThreshold")!;
			Assert.AreEqual(0.5, property.GetDefaultValue());
			Assert.AreEqual(0.5, property.Sentinel);
			Assert.AreNotEqual(0.0, property.Sentinel);
		}
		Assert.AreEqual(0.5, new Appointment().RecallThreshold);
		Assert.AreEqual(0.5, new Election().RecallThreshold);
	}
}
