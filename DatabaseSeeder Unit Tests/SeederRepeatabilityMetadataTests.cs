#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederRepeatabilityMetadataTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void MetadataIdempotentSeeders_AdvertiseSafeReruns()
	{
		IDatabaseSeeder[] seeders =
		[
			new CoreDataSeeder(), new TimeSeeder(), new SkillSeeder(), new SkillPackageSeeder(), new ChargenSeeder(),
			new CultureSeeder(), new ArenaSeeder(), new HealthSeeder(), new WeatherSeeder(), new LawSeeder(),
			new AttributeSeeder(), new HumanSeeder(), new AnimalSeeder(), new CombatSeeder()
		];

		foreach (IDatabaseSeeder seeder in seeders)
		{
			Assert.AreEqual(SeederRepeatabilityMode.Idempotent, seeder.Metadata.RepeatabilityMode, seeder.Name);
			Assert.IsTrue(seeder.SafeToRunMoreThanOnce, seeder.Name);
		}

		IDatabaseSeeder itemSeeder = new ItemSeeder();
		Assert.AreEqual(SeederRepeatabilityMode.Idempotent, itemSeeder.Metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.FullReconcile, itemSeeder.Metadata.UpdateCapability);
		Assert.IsTrue(itemSeeder.SafeToRunMoreThanOnce);
	}

	[TestMethod]
	public void CoreSeeder_RerunHidesBootstrapQuestionsAndReportsFoundationUpdate()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Core",
			Choice = "gamename",
			Answer = "Existing World",
			Version = "test",
			DateTime = DateTime.UtcNow
		});
		context.SaveChanges();

		CoreDataSeeder seeder = new();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
		Assert.IsFalse(seeder.SeederQuestions.Any(x => x.Filter(context, new Dictionary<string, string>())));
	}

	[TestMethod]
	public void SeederAnswerMemory_RemembersLatestAnswerForTheOwningSeeder()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.SeederChoices.AddRange(
			new SeederChoice { Id = 1, Seeder = "Attributes", Choice = "choice", Answer = "soi", Version = "test", DateTime = DateTime.UtcNow.AddMinutes(-1) },
			new SeederChoice { Id = 2, Seeder = "Attributes", Choice = "choice", Answer = "simple", Version = "test", DateTime = DateTime.UtcNow });
		context.SaveChanges();

		Assert.AreEqual("simple", SeederAnswerMemory.GetLatestSeederAnswer(context, "Attributes", "choice"));
		Assert.AreEqual("simple", SeederAnswerMemory.GetLatestSeederAnswers(context, "Attributes")["choice"]);
	}
}
