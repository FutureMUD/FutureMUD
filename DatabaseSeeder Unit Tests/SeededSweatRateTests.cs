#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeededSweatRateTests
{

	[TestMethod]
	[DataRow("Werewolf")]
	[DataRow("Werewolf Hybrid")]
	[DataRow("Vampire")]
	public void SupernaturalRefresh_PreservesCustomSweatWhileNewRacesGetDefaults(string name)
	{
		using var context = Context();
		var seeder = new SupernaturalSeeder();
		var sweat = new Liquid { Id = 1, Name = "Sweat" };
		Set("_context", context); Set("_sweat", sweat);
		Set("_healthTrait", new TraitDefinition { Id = 1 });
		Set("_humanRace", new Race { HungerRate = 1, ThirstRate = 1 });
		var method = typeof(SupernaturalSeeder).GetMethod("ApplyNeedsProfile", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var template = SupernaturalSeeder.TemplatesForTesting[name];
		var custom = new Liquid { Id = 42, Name = "Custom secretion" };
		var race = new Race { SweatRateInLitresPerMinute = 0.007, SweatLiquid = custom };
		method.Invoke(seeder, new object[] { race, template, false });
		Assert.AreEqual(0.007, race.SweatRateInLitresPerMinute, 1e-12);
		Assert.AreSame(custom, race.SweatLiquid);
		method.Invoke(seeder, new object[] { race, template, true });
		var living = template.NeedsProfile == SupernaturalSeeder.SupernaturalNeedsProfile.Living;
		Assert.AreEqual(living ? 0.02 : 0, race.SweatRateInLitresPerMinute, 1e-12);
		Assert.AreSame(living ? sweat : null, race.SweatLiquid);
		void Set(string field, object value) => typeof(SupernaturalSeeder)
			.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(seeder, value);
	}

	private static FuturemudDatabaseContext Context(SaveChangesInterceptor? interceptor = null)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false));
		if (interceptor is not null) options.AddInterceptors(interceptor);
		return new FuturemudDatabaseContext(options.Options);
	}

	[TestMethod]
	public void NewNonSweater_PersistsExplicitZeroAndCannotRepairAnExistingRace()
	{
		var interceptor = new SweatSaveInterceptor();
		using var context = Context(interceptor);
		var race = new Race { Name = "New non-sweater", SweatRateInLitresPerMinute = 0 };
		context.Races.Add(race);
		SeededSweatRates.SaveNewRace(context, race);
		Assert.AreEqual(2, interceptor.Calls);
		Assert.IsTrue(interceptor.ExplicitZeroUpdate, "The SQL-default sentinel must be followed by an explicit zero UPDATE.");
		context.ChangeTracker.Clear();
		race = context.Races.Single();
		Assert.AreEqual(0.0, race.SweatRateInLitresPerMinute, 1e-12);
		race.SweatRateInLitresPerMinute = 0.007;
		context.SaveChanges();
		Assert.ThrowsExactly<InvalidOperationException>(() => SeededSweatRates.SaveNewRace(context, race));
		Assert.AreEqual(0.007, race.SweatRateInLitresPerMinute, 1e-12);
	}

	private sealed class SweatSaveInterceptor : SaveChangesInterceptor
	{
		public int Calls { get; private set; }
		public bool ExplicitZeroUpdate { get; private set; }
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			Calls++;
			ExplicitZeroUpdate |= eventData.Context!.ChangeTracker.Entries<Race>().Any(entry =>
				entry.State == EntityState.Modified && entry.Property(x => x.SweatRateInLitresPerMinute).IsModified &&
				entry.Entity.SweatRateInLitresPerMinute == 0);
			return result;
		}
	}
}
