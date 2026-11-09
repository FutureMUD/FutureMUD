#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnimalSeederTemplateTests
{
	[TestMethod]
	public void ApplyMultiTargetAttackDefinition_RetagsLegacyTendrilMessage()
	{
		using var context = BuildContext();
		var attack = new MudSharp.Models.WeaponAttack
		{
			Id = 1,
			Name = "Tendril Lash",
			MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack,
			MaximumTargets = 1,
			RequiredPositionStateIds = string.Empty
		};
		var message = new MudSharp.Models.CombatMessage
		{
			Id = 1,
			Type = (int)BuiltInCombatMoveType.NaturalWeaponAttack,
			Message = "@ lash|lashes at $1.",
			FailureMessage = "@ miss|misses $1.",
			Chance = 1.0
		};
		context.WeaponAttacks.Add(attack);
		context.CombatMessages.Add(message);
		context.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
		{
			WeaponAttack = attack,
			CombatMessage = message
		});
		context.SaveChanges();

		AnimalSeeder.ApplyMultiTargetAttackDefinition(context, attack, 4,
			BuiltInCombatMoveType.PullToMeleeUnarmed);
		context.SaveChanges();

		Assert.AreEqual(4, attack.MaximumTargets);
		Assert.AreEqual((int)BuiltInCombatMoveType.PullToMeleeUnarmed, attack.MoveType);
		Assert.AreEqual(((int)Difficulty.Hard).ToString(), attack.AdditionalInfo);
		Assert.AreEqual((int)BuiltInCombatMoveType.PullToMeleeUnarmed,
			context.CombatMessages.Single().Type);
	}

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static MudSharp.Models.FutureProg CreateAnimalAiProg(long id, string name, ProgVariableTypes returnType, string text)
    {
        return new MudSharp.Models.FutureProg
        {
            Id = id,
            FunctionName = name,
            FunctionComment = $"{name} test prog",
            FunctionText = text,
            ReturnType = (long)returnType,
            Category = "Tests",
            Subcategory = "AnimalAI",
            Public = true,
            AcceptsAnyParameters = true,
            StaticType = (int)FutureProgStaticType.FullyStatic
        };
    }

	private static Material Material(long id, string name, string description)
	{
		return new Material
		{
			Id = id,
			Name = name,
			MaterialDescription = description,
			ResidueDesc = string.Empty,
			ResidueColour = string.Empty
		};
	}

	private static Liquid Liquid(long id, string name, string description, Material driedResidue)
	{
		return new Liquid
		{
			Id = id,
			Name = name,
			Description = description,
			LongDescription = description,
			TasteText = string.Empty,
			VagueTasteText = string.Empty,
			SmellText = string.Empty,
			VagueSmellText = string.Empty,
			DisplayColour = string.Empty,
			DampDescription = string.Empty,
			WetDescription = string.Empty,
			DrenchedDescription = string.Empty,
			DampShortDescription = string.Empty,
			WetShortDescription = string.Empty,
			DrenchedShortDescription = string.Empty,
			SurfaceReactionInfo = string.Empty,
			DriedResidue = driedResidue,
			DriedResidueId = driedResidue.Id
		};
	}

    private static void SeedAnimalAiPrerequisiteProgs(FuturemudDatabaseContext context)
    {
        context.FutureProgs.AddRange(
            CreateAnimalAiProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"),
            CreateAnimalAiProg(2, "AlwaysFalse", ProgVariableTypes.Boolean, "return false"),
            CreateAnimalAiProg(3, "AlwaysOne", ProgVariableTypes.Number, "return 1"));
        context.SaveChanges();
    }

    private static IReadOnlyList<TimeOfDay> ActiveTimesFor(AnimalActivityStrategyType activity, XElement root)
    {
        List<TimeOfDay> activeTimes = root.Element("Activity")?.Elements("ActiveTime")
            .Select(x => x.Value)
            .Where(x => Enum.TryParse(x, true, out TimeOfDay _))
            .Select(x => Enum.Parse<TimeOfDay>(x, true))
            .ToList() ?? new List<TimeOfDay>();

        if (activeTimes.Count > 0)
        {
            return activeTimes;
        }

        return activity switch
        {
            AnimalActivityStrategyType.Diurnal => [TimeOfDay.Dawn, TimeOfDay.Morning, TimeOfDay.Afternoon],
            AnimalActivityStrategyType.Nocturnal => [TimeOfDay.Dusk, TimeOfDay.Night],
            AnimalActivityStrategyType.Crepuscular => [TimeOfDay.Dawn, TimeOfDay.Dusk],
            _ => Enum.GetValues<TimeOfDay>()
        };
    }

    private static FuturemudDatabaseContext BuildExpandedAnimalCatalogueContext(
        bool includeBeetleBody = true,
        bool includeCentipedeBody = true,
        bool includeAcidSpit = true,
		bool includeMassiveClawSweep = true,
        string? omittedRaceName = null,
        bool beetleOnCorrectBody = true,
        bool includeDietSettings = false,
        bool includeUngulateWearProfiles = true)
    {
        FuturemudDatabaseContext context = BuildContext();
        long nextBodyId = 1;
        long nextRaceId = 1;
        Dictionary<string, BodyProto> bodies = new(StringComparer.OrdinalIgnoreCase);

        void AddBody(string name)
        {
            if (bodies.ContainsKey(name))
            {
                return;
            }

            BodyProto body = new()
            {
                Id = nextBodyId++,
                Name = name,
                ConsiderString = string.Empty,
                WielderDescriptionSingle = "mouth",
                WielderDescriptionPlural = "mouths",
                WearSizeParameterId = 1,
                MinimumLegsToStand = 4,
                MinimumWingsToFly = 0,
                LegDescriptionSingular = "leg",
                LegDescriptionPlural = "legs",
                NameForTracking = name
            };
            bodies[name] = body;
            context.BodyProtos.Add(body);
        }

        AddBody("Humanoid");
        AddBody("Quadruped Base");

        foreach (string bodyKey in AnimalSeeder.RaceTemplatesForTesting.Values
                     .Select(x => x.BodyKey)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if ((!includeBeetleBody && bodyKey == "Beetle") ||
                (!includeCentipedeBody && bodyKey == "Centipede"))
            {
                continue;
            }

            AddBody(bodyKey);
        }

        context.NameCultures.Add(new NameCulture
        {
            Name = "Simple",
            Definition = "<NameCulture />"
        });

        foreach (string modelName in AnimalSeeder.HeightWeightTemplatesForTesting.Keys)
        {
            context.HeightWeightModels.Add(new HeightWeightModel
            {
                Name = modelName,
                MeanHeight = 100,
                MeanBmi = 20,
                StddevHeight = 10,
                StddevBmi = 2,
                Bmimultiplier = 1.0
            });
        }

        if (includeAcidSpit)
        {
            context.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack
            {
                Name = "Acid Spit",
                AdditionalInfo = string.Empty,
                RequiredPositionStateIds = string.Empty
            });
        }

		if (includeMassiveClawSweep)
		{
			context.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack
			{
				Name = "Massive Claw Sweep",
				AdditionalInfo = string.Empty,
				RequiredPositionStateIds = string.Empty,
				MaximumTargets = 3
			});
		}

        BodyProto fallbackBody = bodies.TryGetValue("Insectoid", out BodyProto? insectoidBody)
            ? insectoidBody
            : bodies["Quadruped Base"];

        foreach ((string raceName, AnimalSeeder.AnimalRaceTemplate template) in AnimalSeeder.RaceTemplatesForTesting)
        {
            if (string.Equals(raceName, omittedRaceName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            BodyProto baseBody = raceName == "Beetle" && !beetleOnCorrectBody
                ? fallbackBody
                : bodies.GetValueOrDefault(template.BodyKey, fallbackBody);

            context.Races.Add(new Race
            {
                Id = nextRaceId++,
                Name = raceName,
                Description = $"{raceName} test race",
                BaseBodyId = baseBody.Id,
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
                CommunicationStrategyType = "humanoid",
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
                MaximumFoodSatiatedHours = template.MaximumFoodSatiatedHours,
                MaximumDrinkSatiatedHours = template.MaximumDrinkSatiatedHours,
                CanClimb = false,
                CanSwim = true,
                MinimumSleepingPosition = 1,
                ChildAge = 1,
                YouthAge = 2,
                YoungAdultAge = 3,
                AdultAge = 4,
                ElderAge = 5,
                VenerableAge = 6
            });
        }

        context.SaveChanges();

        if (includeUngulateWearProfiles &&
            bodies.TryGetValue(AnimalSeeder.StockUngulateWearBodyNameForTesting, out BodyProto? ungulateBody))
        {
            long nextWearProfileId = 1;
            foreach (string name in AnimalSeeder.StockUngulateWearProfileNamesForTesting)
            {
                context.WearProfiles.Add(new WearProfile
                {
                    Id = nextWearProfileId++,
                    Name = name,
                    BodyPrototypeId = ungulateBody.Id,
                    WearStringInventory = "worn on",
                    WearAction1st = "put",
                    WearAction3rd = "puts",
                    WearAffix = "on",
                    WearlocProfiles = "<Profiles />",
                    Type = "Direct",
                    Description = $"{name} test profile",
                    RequireContainerIsEmpty = false
                });
            }

            long nextComponentId = 1;
            foreach (string name in AnimalSeeder.StockUngulateWearComponentNamesForTesting)
            {
                context.GameItemComponentProtos.Add(new MudSharp.Models.GameItemComponentProto
                {
                    Id = nextComponentId++,
                    Name = name,
                    Type = "Wearable",
                    Description = $"{name} test component",
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
                });
            }

            context.SaveChanges();
        }

        if (includeDietSettings)
        {
            long nextMaterialId = 1;
            foreach (string materialName in NonHumanForageDietSeederHelper.StockCorpseMaterialNamesForTesting)
            {
                context.Materials.Add(new Material
                {
                    Id = nextMaterialId++,
                    Name = materialName,
                    MaterialDescription = materialName
                });
            }

            context.SaveChanges();

            foreach ((string raceName, AnimalSeeder.AnimalRaceTemplate template) in AnimalSeeder.RaceTemplatesForTesting)
            {
                Race? race = context.Races.FirstOrDefault(x => x.Name == raceName);
                if (race is null)
                {
                    continue;
                }

                NonHumanForageDietSeederHelper.ApplyDiet(
                    context,
                    race,
                    template.Size,
                    AnimalSeeder.GetDietProfilesForTesting(raceName));
            }

            context.SaveChanges();
        }

        return context;
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
        context.SaveChanges();
    }

	private static void AssertNoDuplicateNames(IEnumerable<string> names, string label)
	{
		var duplicates = names
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, duplicates.Count,
			$"Generated animal {label} names should be unique. Duplicates: {string.Join(", ", duplicates)}");
	}

	[TestMethod]
	public void EnsureAnimalNeedsModelConfiguration_RepairsRegisterProgAndStockRaceValuesIdempotently()
	{
		using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext();

		Assert.IsTrue(AnimalSeeder.HasMissingAnimalNeedsModelConfiguration(context));
		AnimalSeeder.EnsureAnimalNeedsModelConfiguration(context);
		context.SaveChanges();

		Assert.IsFalse(AnimalSeeder.HasMissingAnimalNeedsModelConfiguration(context));
		Assert.AreEqual(1, context.VariableDefinitions.AsEnumerable().Count(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			x.Property.Equals(AnimalSeeder.ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase)));
		Assert.AreEqual(1, context.VariableDefaults.AsEnumerable().Count(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			x.Property.Equals(AnimalSeeder.ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase) &&
			x.DefaultValue == "<var>False</var>"));
		Assert.AreEqual(AnimalSeeder.WhichNeedsModelFunctionText,
			context.FutureProgs.Single(x => x.FunctionName == "WhichNeedsModel").FunctionText);

		foreach ((string raceName, AnimalSeeder.AnimalRaceTemplate template) in AnimalSeeder.RaceTemplatesForTesting)
		{
			Race race = context.Races.Single(x => x.Name == raceName);
			bool configured = context.VariableValues.AsEnumerable().Any(x =>
				x.ReferenceType == (long)ProgVariableTypes.Race &&
				x.ReferenceId == race.Id &&
				x.ReferenceProperty.Equals(AnimalSeeder.ActiveNoThirstNeedsRegister,
					StringComparison.OrdinalIgnoreCase) &&
				x.ValueDefinition == "<var>True</var>");
			Assert.AreEqual(template.UsesActiveNoThirstNeeds, configured, raceName);
		}

		int definitions = context.VariableDefinitions.Count();
		int defaults = context.VariableDefaults.Count();
		int progs = context.FutureProgs.Count();
		int values = context.VariableValues.Count();
		AnimalSeeder.EnsureAnimalNeedsModelConfiguration(context);
		context.SaveChanges();
		Assert.AreEqual(definitions, context.VariableDefinitions.Count());
		Assert.AreEqual(defaults, context.VariableDefaults.Count());
		Assert.AreEqual(progs, context.FutureProgs.Count());
		Assert.AreEqual(values, context.VariableValues.Count());

		Race frog = context.Races.Single(x => x.Name == "Frog");
		context.VariableValues.Add(new VariableValue
		{
			ReferenceType = (long)ProgVariableTypes.Race,
			ReferenceId = frog.Id,
			ReferenceProperty = AnimalSeeder.ActiveNoThirstNeedsRegister,
			ValueType = (long)ProgVariableTypes.Boolean,
			ValueDefinition = "<var>True</var>"
		});
		context.SaveChanges();
		Assert.IsTrue(AnimalSeeder.HasMissingAnimalNeedsModelConfiguration(context),
			"A drifted non-aquatic stock value must make the AnimalSeeder repeatable again.");

		AnimalSeeder.EnsureAnimalNeedsModelConfiguration(context);
		context.SaveChanges();
		Assert.IsFalse(AnimalSeeder.HasMissingAnimalNeedsModelConfiguration(context));
	}

	[TestMethod]
	public void RepairLegacyAnimalFluidNames_RenamesSharedAdjectiveRowsForExistingCatalogues()
	{
		using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext();
		long nextMaterialId = 1;
		long nextLiquidId = 1;

		foreach ((string RaceName, string Adjective) in new[]
		         {
			         ("Kangaroo", "Macropod"),
			         ("Wallaby", "Macropod"),
			         ("Dog", "Canine"),
			         ("Dingo", "Canine")
		         })
		{
			var bloodResidue = Material(nextMaterialId++, $"dried {Adjective.ToLowerInvariant()} blood", "dried blood");
			var sweatResidue = Material(nextMaterialId++, $"dried {Adjective.ToLowerInvariant()} sweat", "dried sweat");
			var blood = Liquid(nextLiquidId++, $"{Adjective} Blood", "blood", bloodResidue);
			var sweat = Liquid(nextLiquidId++, $"{Adjective} Sweat", "sweat", sweatResidue);
			context.Materials.AddRange(bloodResidue, sweatResidue);
			context.Liquids.AddRange(blood, sweat);

			Race race = context.Races.Single(x => x.Name == RaceName);
			race.BloodLiquid = blood;
			race.BloodLiquidId = blood.Id;
			race.SweatLiquid = sweat;
			race.SweatLiquidId = sweat.Id;
		}

		context.SaveChanges();

		Assert.IsTrue(AnimalSeeder.HasLegacyAnimalFluidNamesForTesting(context),
			"The legacy adjective-keyed rows should be detected as stock catalogue drift.");

		AnimalSeeder.RepairLegacyAnimalFluidNamesForTesting(context);

		Assert.IsFalse(AnimalSeeder.HasLegacyAnimalFluidNamesForTesting(context),
			"The repair should leave the stock animal fluid keys in their current race-specific form.");
		foreach (string raceName in new[] { "Kangaroo", "Wallaby", "Dog", "Dingo" })
		{
			string bloodLiquidName = AnimalSeeder.RaceBloodLiquidNameForTesting(raceName);
			string sweatLiquidName = AnimalSeeder.RaceSweatLiquidNameForTesting(raceName);
			string bloodMaterialName = AnimalSeeder.RaceBloodMaterialNameForTesting(raceName);
			string sweatMaterialName = AnimalSeeder.RaceSweatMaterialNameForTesting(raceName);

			Assert.IsTrue(context.Liquids.Any(x => x.Name == bloodLiquidName));
			Assert.IsTrue(context.Liquids.Any(x => x.Name == sweatLiquidName));
			Assert.IsTrue(context.Materials.Any(x =>
				x.Name == bloodMaterialName &&
				x.MaterialDescription == "dried blood"));
			Assert.IsTrue(context.Materials.Any(x =>
				x.Name == sweatMaterialName &&
				x.MaterialDescription == "dried sweat"));
		}

		AssertNoDuplicateNames(context.Materials.Select(x => x.Name), "material");
		AssertNoDuplicateNames(context.Liquids.Select(x => x.Name), "liquid");
	}

    [TestMethod]
    public void StockAnimalAITemplates_AllDefinitionsUseValidAnimalSlotCombinations()
    {
        foreach (AnimalAIStockTemplateDefinition template in AnimalSeeder.StockAnimalAITemplatesForTesting)
        {
            XElement root = XElement.Parse(template.BuildDefinition(1, 2, 3));
            AnimalHomeStrategyType home =
                Enum.Parse<AnimalHomeStrategyType>(root.Element("Home")!.Attribute("type")!.Value);
            AnimalFeedingStrategyType feeding =
                Enum.Parse<AnimalFeedingStrategyType>(root.Element("Feeding")!.Attribute("type")!.Value);
            AnimalThreatStrategyType threat =
                Enum.Parse<AnimalThreatStrategyType>(root.Element("Threat")!.Attribute("type")!.Value);
            AnimalMovementStrategyType movement =
                Enum.Parse<AnimalMovementStrategyType>(root.Element("Movement")!.Attribute("type")!.Value);
            AnimalRefugeStrategyType refuge =
                Enum.Parse<AnimalRefugeStrategyType>(root.Element("Refuge")!.Attribute("type")!.Value);
            AnimalActivityStrategyType activity =
                Enum.Parse<AnimalActivityStrategyType>(root.Element("Activity")!.Attribute("type")!.Value);
            XElement water = root.Element("Water")!;
            AnimalWaterStrategyType waterStrategy = water.Attribute("type") is XAttribute waterType
                ? Enum.Parse<AnimalWaterStrategyType>(waterType.Value)
                : bool.Parse(water.Attribute("enabled")?.Value ?? "true")
                    ? AnimalWaterStrategyType.Drink
                    : AnimalWaterStrategyType.Off;
            XElement ecology = root.Element("Ecology") ?? new XElement("Ecology");
            bool ecologyNesting = bool.Parse(ecology.Element("NestingEnabled")?.Value ?? "false");
            bool ecologyParenting = bool.Parse(ecology.Element("ParentingEnabled")?.Value ?? "false");
            bool hasWaterRoomProg = long.Parse(root.Element("Movement")?.Element("AmphibiousWaterCellProg")?.Value ?? "0") > 0;
            bool hasNestSiteProg = long.Parse(ecology.Element("NestSiteProg")?.Value ?? "0") > 0;
            bool hasProtectProg = long.Parse(ecology.Element("ProtectProg")?.Value ?? "0") > 0;
            AnimalAwarenessStrategyType awareness =
                Enum.Parse<AnimalAwarenessStrategyType>(root.Element("Awareness")!.Attribute("type")!.Value);

            MethodInfo validateMethod = typeof(AnimalAI).GetMethod("ValidateConfiguration",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                [
                    typeof(AnimalHomeStrategyType),
                    typeof(AnimalFeedingStrategyType),
                    typeof(AnimalThreatStrategyType),
                    typeof(AnimalMovementStrategyType),
                    typeof(AnimalRefugeStrategyType),
                    typeof(AnimalActivityStrategyType),
                    typeof(IEnumerable<TimeOfDay>),
                    typeof(AnimalWaterStrategyType),
                    typeof(bool),
                    typeof(AnimalAwarenessStrategyType),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool)
                ],
                null)!;
            Assert.IsNotNull(validateMethod, "AnimalAI should expose its slot validation helper.");

            var result = ((bool Ready, string Reason))validateMethod.Invoke(null,
            [
                home,
                feeding,
                threat,
                movement,
                refuge,
                activity,
                ActiveTimesFor(activity, root),
                waterStrategy,
                hasWaterRoomProg,
                awareness,
                ecologyNesting,
                hasNestSiteProg,
                ecologyParenting,
                hasProtectProg
            ])!;

            Assert.IsTrue(result.Ready, $"{template.Name} should be a valid AnimalAI template: {result.Reason}");
        }
    }

    [TestMethod]
    public void SeedStockUngulateWearProfilesForTesting_RerunDoesNotDuplicateAndTargetsUngulate()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(includeUngulateWearProfiles: false);
        SeedAccount(context);

        Assert.IsTrue(AnimalSeeder.SeedStockUngulateWearProfilesForTesting(context),
            "First install should create missing ungulate wear profiles and components.");
        Assert.IsFalse(AnimalSeeder.SeedStockUngulateWearProfilesForTesting(context),
            "Second install should be a no-op.");

        BodyProto ungulateBody = context.BodyProtos.Single(x => x.Name == AnimalSeeder.StockUngulateWearBodyNameForTesting);
        foreach (string name in AnimalSeeder.StockUngulateWearProfileNamesForTesting)
        {
            WearProfile profile = context.WearProfiles.Single(x => x.Name == name);
            Assert.AreEqual(ungulateBody.Id, profile.BodyPrototypeId, $"{name} should target the ungulate body.");
            Assert.AreEqual("Direct", profile.Type);
            Assert.AreEqual("Profiles", XElement.Parse(profile.WearlocProfiles).Name.LocalName);
        }

        foreach (string name in AnimalSeeder.StockUngulateWearComponentNamesForTesting)
        {
            MudSharp.Models.GameItemComponentProto component = context.GameItemComponentProtos.Single(x => x.Name == name);
            Assert.AreEqual("Wearable", component.Type);
            XElement definition = XElement.Parse(component.Definition);
            Assert.AreEqual("true", definition.Attribute("DisplayInventoryWhenWorn")!.Value);
            Assert.IsNotNull(definition.Element("Profiles")!.Attribute("Default"), $"{name} should have a default profile.");
        }
    }

    [TestMethod]
    public void SeedAnimalAIStockTemplates_RerunRestoresMissingTemplatesWithoutDuplicates()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedAnimalAiPrerequisiteProgs(context);

        AnimalSeeder.SeedAnimalAIStockTemplatesForTesting(context);
        Assert.AreEqual(AnimalSeeder.StockAnimalAITemplateNamesForTesting.Count,
            context.ArtificialIntelligences.Count(),
            "Animal seeder should install one AI row per stock animal recommendation template.");

        ArtificialIntelligence removed = context.ArtificialIntelligences
            .First(x => x.Name == AnimalSeeder.StockAnimalAITemplateNamesForTesting.First());
        context.ArtificialIntelligences.Remove(removed);
        context.SaveChanges();

        AnimalSeeder.SeedAnimalAIStockTemplatesForTesting(context);

        foreach (string name in AnimalSeeder.StockAnimalAITemplateNamesForTesting)
        {
            ArtificialIntelligence ai = context.ArtificialIntelligences.Single(x => x.Name == name);
            Assert.AreEqual("Animal", ai.Type);
            Assert.AreEqual("Definition", XElement.Parse(ai.Definition).Name.LocalName);
        }
    }

    [TestMethod]
    public void ApplyDiet_Carnivore_AddsForageRowsCorpseSettingsAndPreservesCustomYields()
    {
        using FuturemudDatabaseContext context = BuildContext();
        Race wolf = new()
        {
            Id = 1,
            Name = "Wolf",
            Description = "Wolf test race",
            AllowedGenders = "1 2 3 4",
            AttributeTotalCap = 1,
            IndividualAttributeCap = 1,
            DiceExpression = "1",
            IlluminationPerceptionMultiplier = 1.0,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1",
            MaximumDragWeightExpression = "1",
            MaximumLiftWeightExpression = "1",
            RaceUsesStamina = true,
            BiteWeight = 1000,
            CanEatCorpses = false,
            EatCorpseEmoteText = string.Empty,
            CanEatMaterialsOptIn = false,
            BreathingVolumeExpression = "1",
            HoldBreathLengthExpression = "1"
        };
        context.Races.Add(wolf);

        long nextMaterialId = 1;
        foreach (string materialName in NonHumanForageDietSeederHelper.StockCorpseMaterialNamesForTesting)
        {
            context.Materials.Add(new Material
            {
                Id = nextMaterialId++,
                Name = materialName,
                MaterialDescription = materialName
            });
        }

        context.RaceEdibleForagableYields.Add(new RaceEdibleForagableYields
        {
            Race = wolf,
            RaceId = wolf.Id,
            YieldType = "builder-custom",
            BiteYield = 99.0,
            HungerPerYield = 1.0,
            EatEmote = "@ eat|eats {0}the custom yield."
        });
        context.SaveChanges();

        NonHumanForageDietSeederHelper.ApplyDiet(
            context,
            wolf,
            SizeCategory.Normal,
            AnimalSeeder.GetDietProfilesForTesting("Wolf"));
        context.SaveChanges();

        HashSet<string> wolfYields = context.RaceEdibleForagableYields
            .Where(x => x.RaceId == wolf.Id)
            .Select(x => x.YieldType)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        CollectionAssert.Contains(wolfYields.ToArray(), "tiny-fish");
        CollectionAssert.Contains(wolfYields.ToArray(), "builder-custom",
            "Stock diet refreshes should not remove builder-defined custom forage yields.");
        Assert.IsTrue(wolf.CanEatCorpses);
        Assert.AreEqual(
            NonHumanForageDietSeederHelper.CorpseBiteWeightForTesting(SizeCategory.Normal),
            wolf.BiteWeight);
        Assert.AreEqual(
            NonHumanForageDietSeederHelper.StockCorpseMaterialNamesForTesting.Count,
            context.RacesEdibleMaterials.Count(x => x.RaceId == wolf.Id));
    }

    [TestMethod]
    public void NonHumanAttributeScalingHelper_AlternateAttributeModelNames_MapToProfileBonuses()
    {
        NonHumanAttributeProfile profile = new(4, 2, -1, -2,
            WillpowerBonus: 3,
            PerceptionBonus: 2,
            AuraBonus: -4,
            IntelligenceDiceExpression: "2d3",
            AuraDiceExpression: "1d2");

        static TraitDefinition Attribute(string name)
        {
            return new TraitDefinition
            {
                Name = name
            };
        }

        Assert.AreEqual(4, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Strength"), profile));
        Assert.AreEqual(2, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Endurance"), profile));
        Assert.AreEqual(3, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Body"), profile));
        Assert.AreEqual(3, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Physique"), profile));
        Assert.AreEqual(-1, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Agility"), profile));
        Assert.AreEqual(-2, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Dexterity"), profile));
        Assert.AreEqual(3, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Willpower"), profile));
        Assert.AreEqual(2, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Perception"), profile));
        Assert.AreEqual(-4, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Aura"), profile));
        Assert.AreEqual(-4, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Luck"), profile));
        Assert.AreEqual(-4, NonHumanAttributeScalingHelper.GetAttributeBonus(Attribute("Spirit"), profile));
        Assert.AreEqual("2d3",
            NonHumanAttributeScalingHelper.GetAttributeDiceExpression(Attribute("Intelligence"), profile));
        Assert.AreEqual("1d2", NonHumanAttributeScalingHelper.GetAttributeDiceExpression(Attribute("Aura"), profile));
        Assert.AreEqual("1d2", NonHumanAttributeScalingHelper.GetAttributeDiceExpression(Attribute("Luck"), profile));
        Assert.AreEqual("1d2", NonHumanAttributeScalingHelper.GetAttributeDiceExpression(Attribute("Spirit"), profile));
    }

    [TestMethod]
    public void BuildAuditPartLookup_DuplicateInheritedAliases_PrefersMostSpecificBody()
    {
        List<BodypartProto> parts = new()
        {
            new() { BodyId = 2, Name = "carapace", Description = "base carapace" },
            new() { BodyId = 1, Name = "carapace", Description = "derived carapace" },
            new() { BodyId = 2, Name = "mouth", Description = "base mouth" }
        };

        IReadOnlyDictionary<string, BodypartProto> lookup = AnimalSeeder.BuildAuditPartLookup(new long[] { 1, 2 }, parts);

        Assert.AreEqual("derived carapace", lookup["carapace"].Description);
        Assert.AreEqual("base mouth", lookup["mouth"].Description);
    }

    [TestMethod]
    public void GetAvianSpinalOrganAliasForLimb_KnownLimbNames_ReturnExpectedMappings()
    {
        Assert.AreEqual("uspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Torso"));
        Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Right Wing"));
        Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Wing"));
        Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Leg"));
        Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Tail"));
        Assert.IsNull(AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Unknown Limb"));
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueMissingExpandedBodiesOrRace_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(
            includeBeetleBody: false,
            includeCentipedeBody: false,
            omittedRaceName: "Mammoth");

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Rerunnable animal seeders should advertise the expanded catalogue when beetle and centipede bodies or new races are missing.");
    }

	[TestMethod]
	public void ShouldSeedData_ExistingCatalogueMissingMassiveClawSweep_ReturnsExtraPackagesAvailable()
	{
		using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(
			includeMassiveClawSweep: false);

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
			"The multi-target natural attack should make the rerunnable animal combat package available.");
	}

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueWithBeetleStillUsingInsectoid_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(beetleOnCorrectBody: false);

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "A legacy beetle race that still points at the insectoid body should trigger the rerun path.");
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueWithLegacySatiationLimits_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext();
        Race camel = context.Races.Single(x => x.Name == "Camel");
        camel.MaximumFoodSatiatedHours = 16.0;
        camel.MaximumDrinkSatiatedHours = 8.0;
        context.SaveChanges();

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Legacy animal races using baseline satiation limits should trigger the rerun repair path.");
    }

    [TestMethod]
    public void ShouldSeedData_ExpandedCatalogueAlreadyPresent_ReturnsMayAlreadyBeInstalled()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(includeDietSettings: true);
        SeedAnimalAiPrerequisiteProgs(context);
        AnimalSeeder.SeedAnimalAIStockTemplatesForTesting(context);
		AnimalSeeder.EnsureAnimalNeedsModelConfiguration(context);
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
			"An otherwise current animal catalogue should advertise the newly available stock NPC skill packages.");
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueMissingAquaticNeedsConfiguration_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(includeDietSettings: true);
        SeedAnimalAiPrerequisiteProgs(context);
        AnimalSeeder.SeedAnimalAIStockTemplatesForTesting(context);

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Missing aquatic no-thirst configuration should advertise a safe Animal Seeder rerun.");
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueMissingUngulateWearProfiles_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(
            includeDietSettings: true,
            includeUngulateWearProfiles: false);
        SeedAnimalAiPrerequisiteProgs(context);
        AnimalSeeder.SeedAnimalAIStockTemplatesForTesting(context);

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Existing animal catalogues should advertise rerun stock when horse equipment wear profiles are missing.");
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueMissingStockAnimalAI_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext();

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Existing animal catalogues should advertise rerun stock when the individual AnimalAI examples are missing.");
    }
}
