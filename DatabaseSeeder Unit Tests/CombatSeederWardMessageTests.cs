#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Database;
using System;
using System.Linq;
using DatabaseCombatMessage = MudSharp.Models.CombatMessage;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSeederWardMessageTests
{
	[TestMethod]
	public void EnsureWardCombatMessages_ExistingCombatMessages_AddsAndRetainsOneFallbackForEachWardStage()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.CombatMessages.Add(new DatabaseCombatMessage
		{
			Type = (int)BuiltInCombatMoveType.Dodge,
			Message = "#1 %1|dodge|dodges",
			FailureMessage = "#1 %1|fail|fails to dodge",
			Chance = 1.0,
			Priority = 1
		});
		context.SaveChanges();

		Assert.AreEqual(2, CombatSeeder.EnsureWardCombatMessages(context, SeedCombatMessageStyle.Compact));
		Assert.AreEqual(0, CombatSeeder.EnsureWardCombatMessages(context, SeedCombatMessageStyle.Compact));

		DatabaseCombatMessage wardDefense = context.CombatMessages.Single(x =>
			x.Type == (int)BuiltInCombatMoveType.WardDefense);
		DatabaseCombatMessage wardCounter = context.CombatMessages.Single(x =>
			x.Type == (int)BuiltInCombatMoveType.WardCounter);

		Assert.AreEqual(", but #1 %1|attempt|attempts to keep $0 at bay", wardDefense.Message);
		Assert.AreEqual(", and #1 %1|fail|fails to hold $0 at bay", wardCounter.Message);
		Assert.AreEqual(", but #1 %1|hold|holds $0 at bay", wardCounter.FailureMessage);
		Assert.AreEqual(3, context.CombatMessages.Count());
	}

	[TestMethod]
	public void EnsureClinchCombatMessages_AddsAndRetainsResistBreakClinchFallback()
	{
		using FuturemudDatabaseContext context = BuildContext();

		Assert.AreEqual(1, CombatSeeder.EnsureClinchCombatMessages(context, SeedCombatMessageStyle.Compact));
		Assert.AreEqual(0, CombatSeeder.EnsureClinchCombatMessages(context, SeedCombatMessageStyle.Compact));

		DatabaseCombatMessage message = context.CombatMessages.Single(x =>
			x.Type == (int)BuiltInCombatMoveType.ResistBreakClinch);
		Assert.AreEqual(", and #1 %1|keep|keeps $0 trapped in the clinch", message.Message);
		Assert.AreEqual(", but #1 %1|fail|fails to keep $0 trapped in the clinch", message.FailureMessage);
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}
}
