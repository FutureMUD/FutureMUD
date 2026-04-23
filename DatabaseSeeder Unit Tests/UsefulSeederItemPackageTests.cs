#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederItemPackageTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedAccount(FuturemudDatabaseContext context)
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
	}

	private static void SeedGeneralPrerequisites(FuturemudDatabaseContext context)
	{
		SeedAccount(context);

		context.Liquids.Add(new Liquid
		{
			Id = 1,
			Name = "water",
			Description = "water",
			LongDescription = "water",
			TasteText = "water",
			VagueTasteText = "water",
			SmellText = "water",
			VagueSmellText = "water",
			DisplayColour = "blue",
			DampDescription = "water-damp",
			WetDescription = "water-wet",
			DrenchedDescription = "water-drenched",
			DampShortDescription = "water-damp",
			WetShortDescription = "water-wet",
			DrenchedShortDescription = "water-drenched",
			SurfaceReactionInfo = "water"
		});

		context.Clocks.Add(new Clock
		{
			Id = 1,
			Definition = "<Clock />",
			Seconds = 0,
			Minutes = 0,
			Hours = 12,
			PrimaryTimezoneId = 1
		});
		context.Timezones.Add(new Timezone
		{
			Id = 1,
			Name = "UTC",
			Description = "Test timezone",
			OffsetHours = 0,
			OffsetMinutes = 0,
			ClockId = 1
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
	public void ClassifyItemPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyItemPackagePresence(context));

		context.GameItemComponentProtos.Add(CreateComponentMarker(10, UsefulSeeder.StockItemMarkersForTesting.First()));
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyItemPackagePresence(context));

		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos.ToList());
		long id = 20L;
		foreach (string name in UsefulSeeder.StockItemMarkersForTesting)
		{
			context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
		}

		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyItemPackagePresence(context));
	}

	[TestMethod]
	public void SeedGeneralCoverageForTesting_RerunDoesNotDuplicateAndCreatesExpandedCoverage()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		UsefulSeeder seeder = new();

		seeder.SeedGeneralCoverageForTesting(context);
		seeder.SeedGeneralCoverageForTesting(context);

		string[] expectedNames =
		[
			"Smokeable_Cigar",
			"Smokeable_Cigarillo",
			"Smokeable_PipeBowl",
			"DragAid_Stretcher",
			"DragAid_Sled",
			"DragAid_Travois",
			"Treatment_AntiInflammatory_Single",
			"Treatment_AntiInflammatory_Kit",
			"TimePiece_PocketWatch",
			"TimePiece_WallClock"
		];

		foreach (string name in expectedNames)
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single general component named {name}.");
		}

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Cigarette"));
		Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "OnSmokeCigarette"));
		Assert.AreEqual(1, context.VariableDefinitions.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(1, context.VariableDefaults.Count(x => x.Property == "nicotineuntil"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name.StartsWith("Food_")));
	}
}
