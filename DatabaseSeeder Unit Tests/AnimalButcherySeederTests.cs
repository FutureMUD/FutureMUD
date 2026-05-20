#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnimalButcherySeederTests
{
	private static readonly string[] UniversalBodypartAliases =
	[
		"head",
		"mouth",
		"beak",
		"mandibles",
		"abdomen",
		"thorax",
		"cephalothorax",
		"carapace",
		"body",
		"mantle",
		"ubody",
		"mbody",
		"stock",
		"rfpaw",
		"rfhoof",
		"rfrontflipper",
		"rclaw",
		"rtalons",
		"rleg1",
		"rwingbase",
		"lfpaw",
		"lfhoof",
		"lfrontflipper",
		"lclaw",
		"ltalons",
		"lleg1",
		"lwingbase",
		"rrpaw",
		"rrhoof",
		"rhindflipper",
		"rfoot",
		"rrclaw",
		"rleg4",
		"lrpaw",
		"lrhoof",
		"lhindflipper",
		"lfoot",
		"lrclaw",
		"lleg4",
		"rwing",
		"lwing",
		"tail",
		"ltail",
		"fluke",
		"caudalfin",
		"tailfan",
		"peduncle",
		"lbody",
		"hindbody",
		"rhorn",
		"rantler",
		"lhorn",
		"lantler",
		"horn",
		"rtusk",
		"ltusk",
		"rfang",
		"lfang",
		"stinger"
	];

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedFullCatalogueContext(FuturemudDatabaseContext context, bool includeCustomPigProfile = false)
	{
		SeedCorePrerequisites(context);

		var bodyKeys = AnimalSeeder.RaceTemplatesForTesting.Values
			.Select(x => x.BodyKey)
			.Concat(MythicalAnimalSeeder.TemplatesForTesting.Values.Select(x => x.BodyKey))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var bodiesByKey = new Dictionary<string, BodyProto>(StringComparer.OrdinalIgnoreCase);
		long nextBodyId = 1;
		long nextBodypartId = 1;
		foreach (var bodyKey in bodyKeys)
		{
			var body = new BodyProto
			{
				Id = nextBodyId++,
				Name = bodyKey,
				ConsiderString = string.Empty,
				WielderDescriptionSingle = "mouth",
				WielderDescriptionPlural = "mouths",
				WearSizeParameterId = 1,
				MinimumLegsToStand = 0,
				MinimumWingsToFly = 0,
				LegDescriptionSingular = "leg",
				LegDescriptionPlural = "legs",
				NameForTracking = bodyKey,
				PlanarData = string.Empty
			};
			context.BodyProtos.Add(body);
			bodiesByKey[bodyKey] = body;

			var displayOrder = 0;
			foreach (var alias in UniversalBodypartAliases)
			{
				body.BodypartProtos.Add(new BodypartProto
				{
					Id = nextBodypartId++,
					Body = body,
					BodyId = body.Id,
					Name = alias,
					Description = alias,
					SeverFormula = "0",
					DisplayOrder = displayOrder++,
					RelativeHitChance = 1,
					DefaultMaterialId = 1,
					Significant = true
				});
			}
		}

		var customProfile = includeCustomPigProfile
			? new RaceButcheryProfile
			{
				Id = 500,
				Name = "Custom Builder Butchery",
				Verb = 0,
				DifficultySkin = 0
			}
			: null;
		if (customProfile is not null)
		{
			context.RaceButcheryProfiles.Add(customProfile);
		}

		long nextRaceId = 1;
		foreach ((var raceName, var template) in AnimalSeeder.RaceTemplatesForTesting)
		{
			var race = BuildRace(nextRaceId++, raceName, bodiesByKey[template.BodyKey]);
			if (customProfile is not null && raceName == "Pig")
			{
				race.RaceButcheryProfile = customProfile;
				race.RaceButcheryProfileId = customProfile.Id;
			}

			context.Races.Add(race);
		}

		foreach ((var raceName, var template) in MythicalAnimalSeeder.TemplatesForTesting)
		{
			context.Races.Add(BuildRace(nextRaceId++, raceName, bodiesByKey[template.BodyKey]));
		}

		context.SaveChanges();
	}

	private static void SeedCorePrerequisites(FuturemudDatabaseContext context)
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

		context.GameItemComponentProtos.AddRange(
			Component(1, "Holdable"),
			Component(2, "Destroyable_Misc"),
			Component(3, "Stack_Pile"));

		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Animal Product" },
			new Tag { Id = 2, Name = "Cutting" },
			new Tag { Id = 3, Name = "Meat" },
			new Tag { Id = 4, Name = "Animal Skin" },
			new Tag { Id = 5, Name = "Thick Animal Skin" });

		var materialNames = AnimalButcherySeeder.StockItemSpecsForTesting
			.Select(x => x.Material)
			.Concat(["meat", "bone", "animal skin"])
			.Except(AnimalButcherySeeder.SignatureMaterialsForTesting.Keys, StringComparer.OrdinalIgnoreCase)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		long nextMaterialId = 1;
		foreach (var materialName in materialNames)
		{
			context.Materials.Add(Material(nextMaterialId++, materialName));
		}

		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1,
			Name = "Butchery",
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		});

		context.GameItemProtos.Add(new MudSharp.Models.GameItemProto
		{
			Id = 1,
			Name = "meat",
			Keywords = "rotten meat",
			MaterialId = 1,
			EditableItem = Editable(),
			RevisionNumber = 0,
			Size = (int)SizeCategory.Small,
			Weight = 100,
			ReadOnly = false,
			LongDescription = "Some rotten meat lies here.",
			BaseItemQuality = (int)ItemQuality.Standard,
			MorphEmote = "$0 $?1|morphs into $1|decays into nothing$.",
			ShortDescription = AnimalButcherySeeder.RottenMeatShortDescriptionForTesting,
			FullDescription = "This is a generic heap of rotten meat.",
			PermitPlayerSkins = false,
			CostInBaseCurrency = 0,
			IsHiddenFromPlayers = false
		});

		context.SaveChanges();
	}

	private static Race BuildRace(long id, string name, BodyProto body)
	{
		return new Race
		{
			Id = id,
			Name = name,
			Description = $"{name} test race",
			BaseBody = body,
			BaseBodyId = body.Id,
			AllowedGenders = "1 2 3 4",
			AttributeTotalCap = 1,
			IndividualAttributeCap = 1,
			DiceExpression = "1",
			IlluminationPerceptionMultiplier = 1.0,
			CorpseModelId = 1,
			DefaultHealthStrategyId = 1,
			CanUseWeapons = false,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SizeStanding = 1,
			SizeProne = 1,
			SizeSitting = 1,
			CommunicationStrategyType = "animal",
			DefaultHandedness = 1,
			HandednessOptions = "1",
			MaximumDragWeightExpression = "1",
			MaximumLiftWeightExpression = "1",
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1.0,
			EatCorpseEmoteText = string.Empty,
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = 0,
			TemperatureRangeCeiling = 40,
			BodypartSizeModifier = 0,
			BodypartHealthMultiplier = 1.0,
			BreathingVolumeExpression = "1",
			HoldBreathLengthExpression = "1",
			CanClimb = false,
			CanSwim = true,
			MaximumFoodSatiatedHours = 12.0,
			MaximumDrinkSatiatedHours = 12.0
		};
	}

	private static MudSharp.Models.GameItemComponentProto Component(long id, string name)
	{
		return new MudSharp.Models.GameItemComponentProto
		{
			Id = id,
			Name = name,
			Type = "Test",
			Description = $"{name} marker",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = Editable()
		};
	}

	private static MudSharp.Models.Material Material(long id, string name)
	{
		return new MudSharp.Models.Material
		{
			Id = id,
			Name = name,
			MaterialDescription = name,
			Type = 0,
			BehaviourType = (int)MaterialBehaviourType.Meat,
			Density = 1.0,
			Organic = true,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = "green"
		};
	}

	private static EditableItem Editable()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow,
			BuilderComment = "test",
			ReviewerAccountId = 1,
			ReviewerComment = "test",
			ReviewerDate = DateTime.UtcNow
		};
	}

	[TestMethod]
	public void AnimalRaceTemplates_AllMapToButcheryFamilies()
	{
		var unmapped = AnimalSeeder.RaceTemplatesForTesting
			.Where(x => !AnimalButcherySeeder.TryGetFamilyForTesting(x.Key, x.Value.BodyKey, false, out _))
			.Select(x => x.Key)
			.ToList();

		Assert.AreEqual(0, unmapped.Count, $"Unmapped animal races: {string.Join(", ", unmapped)}");
	}

	[TestMethod]
	public void MythicalRaceTemplates_OnlyBeastScopeMapsToButcheryFamilies()
	{
		var unclassified = MythicalAnimalSeeder.TemplatesForTesting.Keys
			.Except(AnimalButcherySeeder.IncludedMythicalBeastsForTesting, StringComparer.OrdinalIgnoreCase)
			.Except(AnimalButcherySeeder.ExcludedMythicalRacesForTesting, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, unclassified.Count, $"Unclassified mythical races: {string.Join(", ", unclassified)}");

		foreach ((var raceName, var template) in MythicalAnimalSeeder.TemplatesForTesting)
		{
			var maps = AnimalButcherySeeder.TryGetFamilyForTesting(raceName, template.BodyKey, true, out _);
			if (AnimalButcherySeeder.IncludedMythicalBeastsForTesting.Contains(raceName, StringComparer.OrdinalIgnoreCase))
			{
				Assert.IsTrue(maps, $"{raceName} should be assigned to a stock beast butchery family.");
				continue;
			}

			Assert.IsFalse(maps, $"{raceName} should remain outside the stock animal butchery package.");
		}
	}

	[TestMethod]
	public void StockItemDefinitions_SignatureModelStaysBelowItemCap()
	{
		var shortDescriptions = AnimalButcherySeeder.StockItemSpecsForTesting
			.Select(x => x.ShortDescription)
			.ToList();
		var duplicateProductKeys = AnimalButcherySeeder.FamiliesForTesting.Values
			.SelectMany(family => family.Products
				.GroupBy(product => product.Key, StringComparer.OrdinalIgnoreCase)
				.Where(group => group.Count() > 1)
				.Select(group => $"{family.Key}:{group.Key}"))
			.ToList();

		Assert.AreEqual(shortDescriptions.Count, shortDescriptions.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Stock butchery item short descriptions should be stable unique keys.");
		Assert.AreEqual(0, duplicateProductKeys.Count,
			$"Duplicate stock butchery product keys: {string.Join(", ", duplicateProductKeys)}");
		Assert.IsTrue(shortDescriptions.Count <= AnimalButcherySeeder.MaximumStockItemPrototypeCountForTesting,
			$"Expected no more than {AnimalButcherySeeder.MaximumStockItemPrototypeCountForTesting} stock butchery items, got {shortDescriptions.Count}.");
	}

	[TestMethod]
	public void SeedData_FullCatalogue_ResolvesProductsItemsMaterialsTagsAndMorphs()
	{
		using var context = BuildContext();
		SeedFullCatalogueContext(context);
		var seeder = new AnimalButcherySeeder();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
		seeder.SeedData(context, new Dictionary<string, string>());

		var rottenMeat = context.GameItemProtos.Single(x => x.ShortDescription == AnimalButcherySeeder.RottenMeatShortDescriptionForTesting);
		foreach (var spec in AnimalButcherySeeder.StockItemSpecsForTesting)
		{
			var item = context.GameItemProtos
				.Include(x => x.GameItemProtosTags)
				.ThenInclude(x => x.Tag)
				.Include(x => x.GameItemProtosGameItemComponentProtos)
				.ThenInclude(x => x.GameItemComponent)
				.Single(x => x.ShortDescription == spec.ShortDescription);
			Assert.IsTrue(context.Materials.Any(x => x.Id == item.MaterialId && x.Name == spec.Material), spec.ShortDescription);
			Assert.IsTrue(spec.Tags.All(tag => item.GameItemProtosTags.Any(x => x.Tag.Name == tag)), spec.ShortDescription);
			Assert.IsTrue(item.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "Holdable"), spec.ShortDescription);
			Assert.IsTrue(item.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "Destroyable_Misc"), spec.ShortDescription);
			Assert.IsTrue(item.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "Stack_Pile"), spec.ShortDescription);

			if (spec.Spoils)
			{
				Assert.AreEqual(rottenMeat.Id, item.MorphGameItemProtoId, spec.ShortDescription);
				Assert.AreEqual(AnimalButcherySeeder.SpoilSecondsForTesting, item.MorphTimeSeconds, spec.ShortDescription);
				continue;
			}

			Assert.IsNull(item.MorphGameItemProtoId, spec.ShortDescription);
			Assert.AreEqual(0, item.MorphTimeSeconds, spec.ShortDescription);
		}

		foreach (var product in context.ButcheryProducts
			         .Include(x => x.ButcheryProductItems)
			         .Include(x => x.ButcheryProductsBodypartProtos)
			         .Where(x => x.Name.StartsWith("Stock Butchery Product -")))
		{
			Assert.IsTrue(context.BodyProtos.Any(x => x.Id == product.TargetBodyId), product.Name);
			Assert.IsTrue(product.ButcheryProductsBodypartProtos.Any(), product.Name);
			foreach (var item in product.ButcheryProductItems)
			{
				Assert.IsTrue(context.GameItemProtos.Any(x => x.Id == item.NormalProtoId), product.Name);
				if (item.DamagedProtoId is not null)
				{
					Assert.IsTrue(context.GameItemProtos.Any(x => x.Id == item.DamagedProtoId), product.Name);
				}
			}
		}

		var unassignedAnimals = AnimalSeeder.RaceTemplatesForTesting.Keys
			.Where(x => context.Races.Single(race => race.Name == x).RaceButcheryProfileId is null)
			.ToList();
		Assert.AreEqual(0, unassignedAnimals.Count, $"Unassigned animal races: {string.Join(", ", unassignedAnimals)}");

		foreach (var raceName in AnimalButcherySeeder.IncludedMythicalBeastsForTesting)
		{
			Assert.IsNotNull(context.Races.Single(x => x.Name == raceName).RaceButcheryProfileId, raceName);
		}

		foreach (var raceName in AnimalButcherySeeder.ExcludedMythicalRacesForTesting)
		{
			Assert.IsNull(context.Races.Single(x => x.Name == raceName).RaceButcheryProfileId, raceName);
		}
	}

	[TestMethod]
	public void SeedData_Rerun_DoesNotDuplicateStockRows()
	{
		using var context = BuildContext();
		SeedFullCatalogueContext(context);
		var seeder = new AnimalButcherySeeder();

		seeder.SeedData(context, new Dictionary<string, string>());
		var itemCount = context.GameItemProtos.Count(x => x.GameItemProtosTags.Any(tag => tag.Tag.Name == "Butchery Output"));
		var productCount = context.ButcheryProducts.Count(x => x.Name.StartsWith("Stock Butchery Product -"));
		var profileCount = context.RaceButcheryProfiles.Count(x => x.Name.StartsWith("Stock Butchery -"));
		var linkCount = context.RaceButcheryProfilesButcheryProducts.Count();

		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(itemCount, context.GameItemProtos.Count(x => x.GameItemProtosTags.Any(tag => tag.Tag.Name == "Butchery Output")));
		Assert.AreEqual(productCount, context.ButcheryProducts.Count(x => x.Name.StartsWith("Stock Butchery Product -")));
		Assert.AreEqual(profileCount, context.RaceButcheryProfiles.Count(x => x.Name.StartsWith("Stock Butchery -")));
		Assert.AreEqual(linkCount, context.RaceButcheryProfilesButcheryProducts.Count());
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void SeedData_PreservesCustomBuilderProfileAssignments()
	{
		using var context = BuildContext();
		SeedFullCatalogueContext(context, includeCustomPigProfile: true);
		var seeder = new AnimalButcherySeeder();

		seeder.SeedData(context, new Dictionary<string, string>());

		var pig = context.Races.Include(x => x.RaceButcheryProfile).Single(x => x.Name == "Pig");
		Assert.AreEqual("Custom Builder Butchery", pig.RaceButcheryProfile.Name);
	}
}
