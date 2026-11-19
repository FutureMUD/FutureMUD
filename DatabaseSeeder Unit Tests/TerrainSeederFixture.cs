#nullable enable

using System;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

internal static class TerrainSeederFixture
{
	private static readonly Lazy<InMemorySeederSnapshot> WithoutAccount = new(() => Capture(withAccount: false));
	private static readonly Lazy<InMemorySeederSnapshot> WithAccount = new(() => Capture(withAccount: true));

	internal static FuturemudDatabaseContext CreateSeeded(bool withAccount = false)
	{
		var snapshot = withAccount ? WithAccount.Value : WithoutAccount.Value;
		return snapshot.RestoreInto(CreateEmpty());
	}

	private static InMemorySeederSnapshot Capture(bool withAccount)
	{
		using var context = CreateEmpty();
		if (withAccount)
		{
			SeedAccount(context);
		}

		SeedTerrainFoundations(context);
		return InMemorySeederSnapshot.Capture(context);
	}

	internal static OwnedInMemorySeederContext CreateEmpty()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new OwnedInMemorySeederContext(options);
	}

	internal static void SeedAccount(FuturemudDatabaseContext context)
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
		context.SaveChanges();
	}

	private static void SeedMaterials(FuturemudDatabaseContext context)
	{
		CoreDataSeeder seeder = new();
		typeof(CoreDataSeeder)
			.GetMethod("SeedMaterials", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, [context]);
	}

	private static void SeedVoidTerrain(FuturemudDatabaseContext context)
	{
		context.Terrains.Add(new Terrain
		{
			Id = 1,
			Name = "Void",
			HideDifficulty = 0,
			SpotDifficulty = 0,
			InfectionType = 0,
			InfectionVirulence = 0,
			InfectionMultiplier = 0,
			StaminaCost = 0,
			TerrainANSIColour = "7",
			TerrainEditorColour = "#FFFFFFFF",
			TerrainBehaviourMode = "outdoors",
			DefaultTerrain = true,
			MovementRate = 0,
			ForagableProfileId = 0,
			AtmosphereType = "Gas",
			DefaultRoomOutdoorsType = 0,
			GravityModel = (int)MudSharp.Construction.GravityModel.Normal,
			TerrainEditorText = "Vo",
			CanHaveTracks = false,
			TrackIntensityMultiplierVisual = 1.0,
			TrackIntensityMultiplierOlfactory = 1.0,
			TagInformation = string.Empty
		});
		context.SaveChanges();
	}

	internal static void SeedTerrainFoundations(FuturemudDatabaseContext context)
	{
		SeedMaterials(context);
		SeedVoidTerrain(context);
		CoreDataSeeder.SeedTerrainFoundationsForTesting(context);
	}
}
