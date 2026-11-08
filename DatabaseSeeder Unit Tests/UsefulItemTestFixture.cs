#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MaterialBehaviourType = MudSharp.Form.Material.MaterialBehaviourType;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using TraitType = MudSharp.Body.Traits.TraitType;
using SizeCategory = MudSharp.GameItems.SizeCategory;

namespace MudSharp_Unit_Tests;

internal static class UsefulItemTestFixture
{
	internal static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
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
	}

	internal static void SeedGeneralPrerequisites(FuturemudDatabaseContext context)
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

	internal static MarketCategory CreateMarketCategory(long id, string name)
	{
		return new MarketCategory
		{
			Id = id,
			Name = name,
			Description = $"{name} test category",
			ElasticityFactorAbove = 0.5,
			ElasticityFactorBelow = 0.5,
			MarketCategoryType = 0,
			Tags = "<Tags />",
			CombinationCategories = "<Components />"
		};
	}

	internal static void SeedMarketCategories(FuturemudDatabaseContext context)
	{
		string[] names =
		[
			"Staple Food",
			"Standard Food",
			"Beer",
			"Wine",
			"Luxury Wares",
			"Luxury Clothing",
			"Luxury Furniture",
			"Luxury Decorations",
			"Military Goods",
			"Weapons",
			"Armour",
			"Ammunition",
			"Shields"
		];

		context.MarketCategories.AddRange(names.Select((name, index) => CreateMarketCategory(index + 1, name)));
		context.SaveChanges();
	}

	internal static void EnsureComponentMarkers(FuturemudDatabaseContext context, params string[] names)
	{
		var nextId = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;
		foreach (var name in names)
		{
			if (context.GameItemComponentProtos.Any(x => x.Name == name))
			{
				continue;
			}

			context.GameItemComponentProtos.Add(CreateComponentMarker(nextId++, name));
		}

		context.SaveChanges();
	}

	internal static GameItemComponentProto CreateComponentMarker(long id, string name, string type = "Test")
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

	internal static GameItemProto LoadItem(FuturemudDatabaseContext context, string uniqueName)
	{
		return context.GameItemProtos
		              .Include(x => x.GameItemProtosGameItemComponentProtos)
		              .ThenInclude(x => x.GameItemComponent)
		              .Single(x => x.UniqueName == uniqueName);
	}

	internal static string[] ComponentNames(GameItemProto item)
	{
		return item.GameItemProtosGameItemComponentProtos
		           .Select(x => x.GameItemComponent.Name)
		           .ToArray();
	}
}
