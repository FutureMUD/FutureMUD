#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederModernPackageTests
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

    private static void SeedModernStaticConfiguration(FuturemudDatabaseContext context)
    {
        context.StaticConfigurations.AddRange(
            new StaticConfiguration
            {
                SettingName = "DefaultPowerSocketType",
                Definition = "NEMA 5-15"
            },
            new StaticConfiguration
            {
                SettingName = "DefaultGasSocketType",
                Definition = "BSP 5mm"
            },
            new StaticConfiguration
            {
                SettingName = "DefaultExternalOrganVenousConnector",
                Definition = "2-LargeVenousCatheter"
            },
            new StaticConfiguration
            {
                SettingName = "DefaultExternalOrganArterialConnector",
                Definition = "2-LargeArterialCatheter"
            });
    }

    private static Gas CreateGas(long id, string name)
    {
        return new Gas
        {
            Id = id,
            Name = name,
            Description = name,
            Density = 1.0,
            ThermalConductivity = 0.01,
            ElectricalConductivity = 0.0,
            Organic = false,
            SpecificHeatCapacity = 1.0,
            BoilingPoint = -100.0,
            DisplayColour = "white",
            SmellIntensity = 0.0,
            SmellText = name,
            VagueSmellText = name,
            Viscosity = 0.0,
            DrugGramsPerUnitVolume = 0.0,
            OxidationFactor = 0.0
        };
    }

	private static void SeedExternalOrganPrerequisites(FuturemudDatabaseContext context)
	{
		BodyProto body = new()
		{
			Id = 1,
			Name = "Organic Humanoid",
			ConsiderString = string.Empty,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WielderDescriptionPlural = "hands",
			WielderDescriptionSingle = "hand",
			NameForTracking = "Organic Humanoid"
		};
        context.BodyProtos.Add(body);
        context.BodypartProtos.AddRange(
            new BodypartProto
            {
                Id = 10,
                Body = body,
                BodyId = body.Id,
                Name = "heart",
                Description = "heart",
                BodypartType = (int)BodypartTypeEnum.Heart,
                IsOrgan = 1
            },
            new BodypartProto
            {
                Id = 11,
                Body = body,
                BodyId = body.Id,
                Name = "lung",
                Description = "lung",
                BodypartType = (int)BodypartTypeEnum.Lung,
                IsOrgan = 1
            },
            new BodypartProto
            {
                Id = 12,
                Body = body,
                BodyId = body.Id,
                Name = "kidney",
                Description = "kidney",
                BodypartType = (int)BodypartTypeEnum.Kidney,
                IsOrgan = 1
            });
    }

    private static void SeedModernPrerequisites(FuturemudDatabaseContext context, bool includeGases = false,
        bool includeExternalOrgans = false)
    {
        SeedAccount(context);
        SeedModernStaticConfiguration(context);

        Tag fuelTag = new() { Id = 1, Name = "Fuel" };
        context.Tags.Add(fuelTag);

        Liquid gasoline = new()
        {
            Id = 1,
            Name = "gasoline",
            Description = "gasoline",
            LongDescription = "gasoline",
            TasteText = "gasoline",
            VagueTasteText = "fuel",
            SmellText = "gasoline",
            VagueSmellText = "fuel",
            DisplayColour = "red",
            DampDescription = "gasoline-damp",
            WetDescription = "gasoline-wet",
            DrenchedDescription = "gasoline-drenched",
            DampShortDescription = "gasoline-damp",
            WetShortDescription = "gasoline-wet",
            DrenchedShortDescription = "gasoline-drenched",
            SurfaceReactionInfo = "gasoline"
        };
        Liquid kerosene = new()
        {
            Id = 2,
            Name = "kerosene",
            Description = "kerosene",
            LongDescription = "kerosene",
            TasteText = "kerosene",
            VagueTasteText = "fuel",
            SmellText = "kerosene",
            VagueSmellText = "fuel",
            DisplayColour = "yellow",
            DampDescription = "kerosene-damp",
            WetDescription = "kerosene-wet",
            DrenchedDescription = "kerosene-drenched",
            DampShortDescription = "kerosene-damp",
            WetShortDescription = "kerosene-wet",
            DrenchedShortDescription = "kerosene-drenched",
            SurfaceReactionInfo = "kerosene"
        };
        Liquid water = new()
        {
            Id = 3,
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
        };

        LiquidsTags gasolineTag = new() { Liquid = gasoline, LiquidId = gasoline.Id, Tag = fuelTag, TagId = fuelTag.Id };
        LiquidsTags keroseneTag = new() { Liquid = kerosene, LiquidId = kerosene.Id, Tag = fuelTag, TagId = fuelTag.Id };
        gasoline.LiquidsTags.Add(gasolineTag);
        kerosene.LiquidsTags.Add(keroseneTag);

        context.Liquids.AddRange(gasoline, kerosene, water);
        context.LiquidsTags.AddRange(gasolineTag, keroseneTag);

        if (includeGases)
        {
            context.Gases.AddRange(
                CreateGas(10, "Bronchodilator"),
                CreateGas(11, "General Anaesthetic"));
        }

        if (includeExternalOrgans)
        {
            SeedExternalOrganPrerequisites(context);
        }

        context.SaveChanges();
    }

    private static void SeedModernBaseContext(FuturemudDatabaseContext context)
    {
        SeedAccount(context);
        SeedModernStaticConfiguration(context);
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
        using FuturemudDatabaseContext context = BuildContext();
        SeedModernPrerequisites(context);

        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyModernPackagePresence(context));

        context.GameItemComponentProtos.Add(CreateComponentMarker(10, UsefulSeeder.StockModernItemMarkersForTesting.First()));
        context.SaveChanges();
        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyModernPackagePresence(context));

        context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos.ToList());
        long id = 20L;
        foreach (string name in UsefulSeeder.StockModernItemMarkersForTesting)
        {
            context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
        }

        context.GameItemComponentProtos.Add(CreateComponentMarker(id++, "FuelHeaterCooler_Test", "FuelHeaterCooler"));
        context.GameItemComponentProtos.Add(CreateComponentMarker(id, "FuelGenerator_Test", "Fuel Generator"));
        context.SaveChanges();

        Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyModernPackagePresence(context));
    }

    [TestMethod]
    public void SeedModernItemsForTesting_RerunDoesNotDuplicateAndCreatesExpandedModernCoverage()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedModernPrerequisites(context, includeGases: true, includeExternalOrgans: true);
        UsefulSeeder seeder = new();

        seeder.SeedModernItemsForTesting(context);
        seeder.SeedModernItemsForTesting(context);

        foreach (string name in UsefulSeeder.StockModernItemMarkersForTesting)
        {
            Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single modern marker named {name}.");
        }

        string[] additionalModernCoverage =
        [
            "Battery_LiIon_Pack",
            "BatteryPowered_1xLiIon",
            "BatteryPowered_2xLiIon",
            "BatteryCharger_LiIon_Single",
            "BatteryCharger_LiIon_Quad",
            "ElectricGridOutlet_Single",
            "ElectricGridOutlet_Quad",
            "GridPowerSupply_HighDraw",
            "UnlimitedGenerator_Admin",
            "CellularPhone_Smartphone",
            "CellPhoneTower_Local",
            "CellPhoneTower_Regional",
            "AnsweringMachine_Standard",
            "Tape_Cassette30",
            "Tape_Microcassette10",
            "ComputerHost_Server",
            "ComputerStorage_Portable",
            "ComputerStorage_Fixed",
            "ComputerTerminal_Desk",
            "ComputerTerminal_Kiosk",
            "NetworkAdapter_Wired",
            "NetworkSwitch_4Port",
            "NetworkSwitch_8Port",
            "WirelessModem_Local",
            "AutomationHousing_JunctionBox",
            "AutomationMountHost_2Bay",
            "AutomationMountHost_4Bay",
            "FileSignalGenerator_Text",
            "PushButton_Doorbell",
            "ToggleSwitch_Wall",
            "MotionSensor_Room",
            "MotionSensor_Perimeter",
            "LightSensor_Daylight",
            "RainSensor_Outdoor",
            "TemperatureSensor_Thermostat",
            "TimerSensor_Repeating",
            "Keypad_4Digit",
            "Keypad_6Digit",
            "SignalLight_Indicator",
            "SignalLight_Beacon",
            "RelaySwitch_Inline",
            "AlarmSiren_Indoor",
            "AlarmSiren_Outdoor",
            "ElectronicDoor_AutoSlide",
            "ElectronicLock_Maglock",
            "Microcontroller_Basic",
            "GasContainer_OxygenLarge",
            "GasContainer_Anaesthetic",
            "Rebreather_Standard",
            "Rebreather_Watertight",
            "ExternalInhaler_Medical",
            "InhalerGasCanister_Bronchodilator",
            "InhalerGasCanister_Anaesthetic",
            "IntegratedInhaler_Emergency",
            "Defibrillator_Clinical",
            "ExternalOrgan_HeartLungSupport_Human",
            "ExternalOrgan_Dialysis_Human"
        ];

        foreach (string name in additionalModernCoverage)
        {
            Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), $"Expected a single modern component named {name}.");
        }

        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_gasoline"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_kerosene"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "FuelGenerator_water"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name.StartsWith("Food_")));
        Assert.AreEqual(2, context.GameItemComponentProtos.Count(x => x.Type == "Fuel Generator"));
        Assert.AreEqual(4, context.GameItemComponentProtos.Count(x => x.Type == "ElectricHeaterCooler"));
        Assert.AreEqual(3, context.GameItemComponentProtos.Count(x => x.Type == "ConsumableHeaterCooler"));
        Assert.AreEqual(3, context.GameItemComponentProtos.Count(x => x.Type == "SolidFuelHeaterCooler"));
        Assert.AreEqual(4, context.GameItemComponentProtos.Count(x => x.Type == "FuelHeaterCooler"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelHeaterCooler_PortableHeater_gasoline"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelHeaterCooler_WorkshopStove_gasoline"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelHeaterCooler_PortableHeater_kerosene"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "FuelHeaterCooler_WorkshopStove_kerosene"));

        GameItemComponentProto gasolineHeater = context.GameItemComponentProtos.Single(x => x.Name == "FuelHeaterCooler_PortableHeater_gasoline");
        XElement gasolineHeaterDefinition = XElement.Parse(gasolineHeater.Definition);
        Assert.AreEqual("0", gasolineHeaterDefinition.Element("FuelMedium")?.Value);
        Assert.AreEqual("1", gasolineHeaterDefinition.Element("LiquidFuel")?.Value);

        GameItemComponentProto fireplace = context.GameItemComponentProtos.Single(x => x.Name == "SolidFuelHeaterCooler_Fireplace");
        XElement fireplaceDefinition = XElement.Parse(fireplace.Definition);
        Assert.AreEqual("1", fireplaceDefinition.Element("FuelTag")?.Value);

        GameItemComponentProto heartLungSupport = context.GameItemComponentProtos.Single(x => x.Name == "ExternalOrgan_HeartLungSupport_Human");
        XElement heartLungDefinition = XElement.Parse(heartLungSupport.Definition);
        Assert.AreEqual("1", heartLungDefinition.Element("Body")?.Value);
        Assert.IsTrue((heartLungDefinition.Element("Organs")?.Elements("Organ").Count() ?? 0) >= 2);
    }

    [TestMethod]
    public void ClassifyModernPackagePresence_AllModernMarkersWithoutFuelPrerequisites_ReturnsMayAlreadyInstalled()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedModernBaseContext(context);

        long id = 100L;
        foreach (string name in UsefulSeeder.StockModernItemMarkersForTesting)
        {
            context.GameItemComponentProtos.Add(CreateComponentMarker(id++, name));
        }

        context.SaveChanges();

        Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyModernPackagePresence(context));
    }

    [TestMethod]
    public void SeedModernItemsForTesting_MissingGasAndBodyPrerequisites_SkipsConditionalMedicalCoverage()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedModernPrerequisites(context);
        UsefulSeeder seeder = new();

        seeder.SeedModernItemsForTesting(context);

        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "GasContainer_OxygenSmall"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Rebreather_Standard"));
        Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "ExternalInhaler_Medical"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "InhalerGasCanister_Bronchodilator"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "InhalerGasCanister_Anaesthetic"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "IntegratedInhaler_Emergency"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "ExternalOrgan_HeartLungSupport_Human"));
        Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "ExternalOrgan_Dialysis_Human"));
    }
}
