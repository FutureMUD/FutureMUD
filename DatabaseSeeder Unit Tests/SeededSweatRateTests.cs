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
	private static readonly Dictionary<string, double> AnimalRates = new()
	{
		["Horse"] = 0.20, ["Donkey"] = 0.08, ["Mule"] = 0.14,
		["Cow"] = 0.02, ["Ox"] = 0.02, ["Bison"] = 0.015, ["Buffalo"] = 0.005,
		["Sheep"] = 0.003, ["Goat"] = 0.003, ["Deer"] = 0.005, ["Reindeer"] = 0.005,
		["Moose"] = 0.01, ["Elk"] = 0.01, ["Llama"] = 0.005, ["Alpaca"] = 0.003,
		["Camel"] = 0.025, ["Giraffe"] = 0.015, ["Macaque"] = 0.001,
		["Rhinocerous"] = 0.01, ["Tapir"] = 0.005, ["Capybara"] = 0.001,
		["Kangaroo"] = 0.005, ["Wallaby"] = 0.002
	};

	[TestMethod]
	public void AllAnimalTemplates_HaveSpeciesRatesAndNoHumanFallback()
	{
		foreach (var template in AnimalSeeder.RaceTemplatesForTesting.Values)
		{
			var expected = AnimalRates.GetValueOrDefault(template.Name);
			Assert.AreEqual(expected, template.SweatRateInLitresPerMinute, 1e-12, template.Name);
			Assert.IsTrue(expected == 0 || template.Sweats, template.Name);
		}
		Assert.IsTrue(AnimalRates.Keys.All(AnimalSeeder.RaceTemplatesForTesting.ContainsKey));
		var source = SeederSourceTestHelper.ReadSeederSource("AnimalSeeder.Races.cs");
		Assert.IsTrue(source.Contains("SweatLiquid = raceTemplate?.SweatRateInLitresPerMinute > 0.0 ? sweat : null"));
		Assert.IsTrue(source.Contains("SweatRateInLitresPerMinute = raceTemplate?.SweatRateInLitresPerMinute ?? 0.0"));
	}

	[TestMethod]
	public void MythicalCatalogue_UsesAnatomyRatherThanSapienceForSweating()
	{
		var expected = new Dictionary<string, double>
		{
			["Unicorn"] = 0.20, ["Pegasus"] = 0.20, ["Pegacorn"] = 0.20, ["Centaur"] = 0.20,
			["Hippogriff"] = 0.10, ["Qilin"] = 0.02, ["Minotaur"] = 0.02,
			["Selkie"] = 0.02, ["Naga"] = 0.01
		};
		foreach (var template in MythicalAnimalSeeder.TemplatesForTesting.Values)
		{
			Assert.AreEqual(expected.GetValueOrDefault(template.Name), SeededSweatRates.Mythical(template.Name),
				1e-12, template.Name);
		}
		Assert.IsTrue(expected.Keys.All(MythicalAnimalSeeder.TemplatesForTesting.ContainsKey));
		var source = SeederSourceTestHelper.ReadSeederSource("MythicalAnimalSeeder.cs");
		Assert.IsTrue(source.Contains("SweatLiquid = SeededSweatRates.Mythical(template.Name) > 0.0 ? _sweat : null"));
	}

	[TestMethod]
	[DataRow(0.02, 0.48, 0.84, 1.08)]
	[DataRow(0.20, 4.8, 8.4, 10.8)]
	[DataRow(0.003, 0.072, 0.126, 0.162)]
	[DataRow(0.001, 0.024, 0.042, 0.054)]
	[DataRow(0.0, 0.0, 0.0, 0.0)]
	public void Rates_ProduceExpectedSteadyStateLitresPerRealHour(double rate, double heavy,
		double veryHeavy, double extremelyHeavy)
	{
		Assert.AreEqual(heavy, rate * 0.4 * 60, 1e-12);
		Assert.AreEqual(veryHeavy, rate * 0.7 * 60, 1e-12);
		Assert.AreEqual(extremelyHeavy, rate * 0.9 * 60, 1e-12);
		// Lock the inspected runtime contract; this task deliberately does not change it.
		var root = Directory.GetParent(Directory.GetParent(SeederSourceTestHelper.GetSeederSourcePath("SeededSweatRates.cs"))!.FullName)!.Parent!.Parent!.FullName;
		var source = File.ReadAllText(Path.Combine(root, "MudSharpCore", "Body", "Implementations", "BodyMovement.cs"));
		var start = source.IndexOf("public void StaminaMinuteHeartbeat()", StringComparison.Ordinal);
		var heartbeat = source[start..];
		Assert.IsTrue(heartbeat.Contains("double multiplier = 0.4;"));
		Assert.IsTrue(heartbeat.Contains("multiplier = 0.7;"));
		Assert.IsTrue(heartbeat.Contains("multiplier = 0.9;"));
		Assert.IsTrue(heartbeat.Contains("Race.SweatRateInLitresPerMinute * multiplier"));
		Assert.IsTrue(heartbeat.Contains("ThirstPoints = -5.0 / 12.0 * multiplier"));
		Assert.IsTrue(heartbeat.IndexOf("WaterLitres = -1 * sweatTotal", StringComparison.Ordinal) <
			heartbeat.IndexOf("sweatTotal *= 0.5", StringComparison.Ordinal));
		Assert.IsTrue(heartbeat.Contains("Race.SweatLiquid != null && !isDead"));
	}

	[TestMethod]
	public void HumanAncestry_ExplicitlySeedsCorrectRateOnEveryLevel()
	{
		Assert.AreEqual(0.02, SeededSweatRates.Human, 1e-12);
		var source = SeederSourceTestHelper.ReadSeederSource("HumanSeeder.cs");
		foreach (var variable in new[] { "humanoidRace", "organicHumanoidRace", "human" })
		{
			var start = source.IndexOf($"Race {variable} = new()", StringComparison.Ordinal);
			Assert.IsTrue(start >= 0, variable);
			var block = source[start..source.IndexOf("};", start, StringComparison.Ordinal)];
			Assert.IsTrue(block.Contains("SweatRateInLitresPerMinute = SeededSweatRates.Human"), variable);
			Assert.IsTrue(block.Contains("SweatLiquid = sweat"), variable);
			if (variable == "organicHumanoidRace") Assert.IsTrue(block.Contains("ParentRace = humanoidRace"));
			if (variable == "human") Assert.IsTrue(block.Contains("ParentRace = organicHumanoidRace"));
		}
		var heritage = SeederSourceTestHelper.ReadSeederSource("CultureSeeder.Heritage.MiddleEarth.cs");
		foreach (var field in new[] { "MiddleEarthElf", "Hobbit", "Dwarf", "Orc", "Troll" })
			Assert.IsTrue(heritage.Contains($"SweatRateInLitresPerMinute = SeededSweatRates.{field}"), field);
		CollectionAssert.AreEqual(new[] { 0.01, 0.01, 0.015, 0.02, 0.06 }, new[] {
			SeededSweatRates.MiddleEarthElf, SeededSweatRates.Hobbit, SeededSweatRates.Dwarf,
			SeededSweatRates.Orc, SeededSweatRates.Troll });
	}

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

	[TestMethod]
	public void DocumentedInventory_CoversEveryAnimalAndMythicalTemplateWithCorrectHourlyRates()
	{
		var root = Directory.GetParent(Directory.GetParent(SeederSourceTestHelper.GetSeederSourcePath("SeededSweatRates.cs"))!.FullName)!.Parent!.Parent!.FullName;
		var rows = File.ReadAllLines(Path.Combine(root, "Design Documents", "Seeding", "Seeded_Sweat_Rates.tsv"))
			.Skip(1).Select(line => line.Split('\t')).ToArray();
		foreach (var group in rows.GroupBy(row => row[0]))
		{
			Assert.AreEqual(group.Count(), group.Select(row => row[1]).Distinct().Count(), group.Key);
		}
		Check("Animal", AnimalSeeder.RaceTemplatesForTesting.ToDictionary(pair => pair.Key, pair => pair.Value.SweatRateInLitresPerMinute));
		Check("Mythical", MythicalAnimalSeeder.TemplatesForTesting.Keys.ToDictionary(name => name, SeededSweatRates.Mythical));
		void Check(string seeder, Dictionary<string, double> expected)
		{
			var actual = rows.Where(row => row[0] == seeder).ToDictionary(row => row[1]);
			CollectionAssert.AreEquivalent(expected.Keys.ToArray(), actual.Keys.ToArray(), seeder);
			foreach (var (name, rate) in expected)
			{
				var values = actual[name].Skip(2).Select(value => double.Parse(value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
				Assert.AreEqual(4, values.Length, name);
				var wanted = new[] { rate, rate * 24, rate * 42, rate * 54 };
				for (var index = 0; index < wanted.Length; index++) Assert.AreEqual(wanted[index], values[index], 1e-9, name);
			}
		}
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
