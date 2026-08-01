#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederDependencyRegistryTests
{
	[TestMethod]
	public void DependencyPlan_ForAllSeeders_IsCompleteAndAcyclic()
	{
		var seeders = GetSeeders();

		SeederDependencyPlan plan = SeederMetadataRegistry.GetDependencyPlan(seeders);

		Assert.AreEqual(0, plan.Errors.Count, string.Join(Environment.NewLine, plan.Errors));
		Assert.AreEqual(seeders.Count, plan.OrderedSeeders.Count);
		Assert.AreEqual(seeders.Count, plan.OrderedSeeders.Select(x => x.GetType()).Distinct().Count());
	}

	[TestMethod]
	public void DependencyPlan_PlacesSharedFoundationsBeforeTheirConsumers()
	{
		SeederDependencyPlan plan = SeederMetadataRegistry.GetDependencyPlan(GetSeeders());
		Dictionary<Type, int> positions = plan.OrderedSeeders
			.Select((seeder, index) => new { SeederType = seeder.GetType(), Index = index })
			.ToDictionary(x => x.SeederType, x => x.Index);

		Assert.IsTrue(positions[typeof(UsefulSeeder)] < positions[typeof(CombatSeeder)]);
		Assert.IsTrue(positions[typeof(UsefulSeeder)] < positions[typeof(ItemSeeder)]);
		Assert.IsTrue(positions[typeof(UsefulSeeder)] < positions[typeof(EconomySeeder)]);
		Assert.IsTrue(positions[typeof(AnimalSeeder)] < positions[typeof(MythicalAnimalSeeder)]);
		Assert.IsTrue(positions[typeof(MythicalAnimalSeeder)] < positions[typeof(SupernaturalSeeder)]);
	}

	[TestMethod]
	public void CombatMetadata_ReportsItsSharedSkillAndUsefulTagRequirements()
	{
		SeederMetadata metadata = SeederMetadataRegistry.GetMetadata(new CombatSeeder());
		string descriptions = string.Join(" ", metadata.Prerequisites.Select(x => x.Description));

		StringAssert.Contains(descriptions, "Shared skill infrastructure");
		StringAssert.Contains(descriptions, "crossbow spanning-tool tags");
		CollectionAssert.Contains(metadata.RequiredSeederTypes.ToList(), typeof(UsefulSeeder));
	}

	[TestMethod]
	public void CombatSeeder_RefusesDirectExecutionWhenItsMetadataPrerequisitesAreMissing()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		string result = new CombatSeeder().SeedData(context, new Dictionary<string, string>());

		StringAssert.StartsWith(result, "Combat cannot be installed because the following prerequisites are missing:");
	}

	private static List<IDatabaseSeeder> GetSeeders()
	{
		Type seederInterface = typeof(IDatabaseSeeder);
		return Assembly.GetAssembly(seederInterface)!
			.GetTypes()
			.Where(x => !x.IsAbstract && seederInterface.IsAssignableFrom(x))
			.Select(Activator.CreateInstance)
			.OfType<IDatabaseSeeder>()
			.ToList();
	}
}
