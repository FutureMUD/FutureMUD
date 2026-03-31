#nullable enable

using System;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederModernPackageTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedModernPrerequisites(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "DefaultPowerSocketType",
			Definition = "NEMA 5-15"
		});

		var fuelTag = new Tag { Id = 1, Name = "Fuel" };
		context.Tags.Add(fuelTag);

		var gasoline = new Liquid { Id = 1, Name = "gasoline" };
		var kerosene = new Liquid { Id = 2, Name = "kerosene" };
		var water = new Liquid { Id = 3, Name = "water" };

		var gasolineTag = new LiquidsTags { Liquid = gasoline, LiquidId = gasoline.Id, Tag = fuelTag, TagId = fuelTag.Id };
		var keroseneTag = new LiquidsTags { Liquid = kerosene, LiquidId = kerosene.Id, Tag = fuelTag, TagId = fuelTag.Id };
		gasoline.LiquidsTags.Add(gasolineTag);
		kerosene.LiquidsTags.Add(keroseneTag);

		context.Liquids.AddRange(gasoline, kerosene, water);
		context.LiquidsTags.AddRange(gasolineTag, keroseneTag);
		context.SaveChanges();
	}

	private static void SeedModernBaseContext(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});

		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "DefaultPowerSocketType",
			Definition = "NEMA 5-15"
		});

		context.SaveChanges();
	}

	private static GameItemComponentProto CreateComponentMarker(long id, string name, string type = "Test")
	{
		return new GameItemComponentProto
		{
			Id = id,
			Name = name,
			Type = type,
			Description = $"{name} marker",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = "test",
				ReviewerAccountId = 1,
				ReviewerComment = "test",
				ReviewerDate = DateTime.UtcNow
			}
		};
	}

	[TestMethod]
	public void ClassifyModernPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using var context = BuildContext();
		SeedModernPrerequisites(context);

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyModernPackagePresence(context));

		context.GameItemComponentProtos.Add(CreateComponentMarker(10, UsefulSeeder.StockModernItemMarkersForTesting.First()));
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyModernPackagePresence(context));

		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos.ToList());
		var id = 20L;
		foreach (var name in UsefulSeeder.StockModernItemMarkersForTesting)
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.GameItemComponentProtos.Add(CreateComponentMarker(id, "FuelGenerator_Test", "Fuel Generator"));
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyModernPackagePresence(context));
	}

	[TestMethod]
	public void SeedModernItemsForTesting_RerunDoesNotDuplicateAndCreatesFuelGeneratorsForTaggedLiquids()
	{
		using var context = BuildContext();
		SeedModernPrerequisites(context);
		var seeder = new UsefulSeeder();

		seeder.SeedModernItemsForTesting(context);
		seeder.SeedModernItemsForTesting(context);

		foreach (var name in UsefulSeeder.StockModernItemMarkersForTesting)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single modern marker named {name}.");
		}

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_gasoline"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_kerosene"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_water"));
		Assert.AreEqual(2, context.GameItemComponentProtos.Count(x => x.Type == "Fuel Generator"));
	}

	[TestMethod]
	public void ClassifyModernPackagePresence_AllModernMarkersWithoutFuelPrerequisites_ReturnsMayAlreadyInstalled()
	{
		using var context = BuildContext();
		SeedModernBaseContext(context);

		var id = 100L;
		foreach (var name in UsefulSeeder.StockModernItemMarkersForTesting)
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyModernPackagePresence(context));
	}
}
