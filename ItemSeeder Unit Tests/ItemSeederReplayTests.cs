#if DEBUG
#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederReplayTests
{
	private static FuturemudDatabaseContext Context(string databaseName) => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(databaseName)
		.Options);

	private static string NewDatabaseName() => Guid.NewGuid().ToString("N");

	private static IDatabaseSeeder[] ItemSeeders() =>
		SeederPackageDiscovery.GetEnabledSeeders(typeof(ItemSeeder).Assembly).ToArray();

	[TestMethod]
	public void ItemReplayProfiles_ValidateAsAnIndependentProductInventory()
	{
		var seeders = ItemSeeders();
		Assert.AreEqual(1, seeders.Length);
		Assert.IsInstanceOfType<ItemSeeder>(seeders[0]);

		foreach (SeederReplayProfile profile in ItemSeederReplayProfiles.All)
		{
			SeederReplayValidationResult validation = ItemSeederReplayProfiles.Runner.Validate(profile, seeders);
			Assert.IsTrue(validation.IsValid,
				$"{profile.Id}:{Environment.NewLine}{string.Join(Environment.NewLine, validation.Errors)}");
		}
	}

	[TestMethod]
	public void ReplayRequiresAWorldBootstrappedByDatabaseSeeder()
	{
		string databaseName = NewDatabaseName();
		using var context = Context(databaseName);
		SeederReplayProfile profile = ItemSeederReplayProfiles.All.Single(x => x.Id == "medieval-standard");

		SeederReplayRunResult result = ItemSeederReplayProfiles.Run(profile, ItemSeeders(), () => Context(databaseName),
			new Version(1, 0));

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Failure!, "prepared local world");
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void PreparedArmageddonWorld_SkipsGenericItemProfileWithoutWrites()
	{
		string databaseName = NewDatabaseName();
		using (var context = Context(databaseName))
		{
			context.Accounts.Add(new Account { Id = 1, Name = "Debug World", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "metric" });
			context.SeederChoices.Add(new SeederChoice
			{
				Seeder = "Skill Package",
				Choice = "skillpreset",
				Answer = "Armageddon",
				Version = "test",
				DateTime = DateTime.UtcNow
			});
			context.SaveChanges();
		}

		SeederReplayProfile profile = ItemSeederReplayProfiles.All.Single(x => x.Id == "armageddon-native");
		SeederReplayRunResult result = ItemSeederReplayProfiles.Run(profile, ItemSeeders(), () => Context(databaseName),
			new Version(1, 0));

		Assert.IsTrue(result.Success, result.Failure);
		CollectionAssert.AreEqual(new[] { "Items" }, result.NotApplicableSeeders.ToArray());
		Assert.AreEqual(0, result.CompletedSeeders.Count);
	}
}
#endif
