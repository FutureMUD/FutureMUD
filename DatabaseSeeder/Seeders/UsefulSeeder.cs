using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder : IDatabaseSeeder
{
	private static readonly string[] StockAiExampleNames =
	[
		"CommandableOwner",
		"CommandableClanOutranks",
		"BasicDoorguard",
		"SparPartner",
		"RandomWanderer",
		"AggressiveToAllOtherSpecies",
		"RescueClanBrothers",
		"VerminScavenge",
		"TrackingAggressiveToAllOtherSpecies",
		"BasicSelfCare",
		"ExampleArenaParticipant",
		"ExampleArborealWanderer",
		"ExampleDenBuilder",
		"ExampleLairScavenger"
	];

	private static readonly string[] StockItemMarkers =
	[
		"Container_Table",
		"Insulation_Minor",
		"Destroyable_Misc",
		"Torch_Infinite"
	];

	private static readonly string[] StockModernItemMarkers =
	[
		"Battery_AA",
		"BatteryPowered_4xAA",
		"BatteryPowered_4xAA_Connectable",
		"BatteryCharger_AA_4Bay",
		"ElectricGridCreator_Standard",
		"LiquidGridCreator_Standard",
		"TelecommunicationsGridCreator_Standard",
		"Telephone_Standard",
		"ElectricGridFeeder_Standard",
		"TelecommunicationsGridFeeder_Standard",
		"TelecommunicationsGridOutlet",
		"GridLiquidSource_Standard",
		"LiquidGridSupplier_Standard",
		"LiquidPump_Standard",
		"LiquidPump_Industrial",
		"LiquidConsumingProp_Standard",
		"LiquidConsumingProp_Basin",
		"BatteryPowered_LaptopStyle",
		"PowerSocket_Mains_Double",
		"PowerSupply_60W",
		"ElectricLight_Medium",
		"HandheldRadio_Standard",
		"ElectricHeaterCooler_SpaceHeater",
		"ElectricHeaterCooler_PortableCooler",
		"ConsumableHeaterCooler_SmallFire",
		"ConsumableHeaterCooler_Bonfire",
		"SolidFuelHeaterCooler_Fireplace",
		"SolidFuelHeaterCooler_WoodStove"
	];

	public bool SafeToRunMoreThanOnce => true;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("ai",
				"Do you want to install the stock AI example package? This includes repeatable command, combat, scavenging and wandering examples, plus the newer arena, arboreal, den-builder and lair-scavenger samples.\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => ClassifyAiPackagePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled,
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("terrain",
				"Do you want to install a collection of terrestrial terrain types?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.Terrains.Count() <= 1,
				(answer, context) =>
				{
						if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
						return (false, "Invalid answer");
				}),
			("covers",
				"Do you want to install a collection of simple ranged covers?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.RangedCovers.Count() <= 1,
				(answer, context) =>
				{
						if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
						return (false, "Invalid answer");
				}),
			("items",
				"#DItem Package 1#F\n\nDo you want to include a package of standard item definitions, which includes some commonly used item component types, including a wide selection of containers, liquid containers, doors, locks, keys, basic writing implements, insulation for clothing, components that let worn clothing hide or change characteristics (wigs, coloured contacts, etc), components that correct for myopia flaws, as well as identity obscurers (hoods, full helmets, niqabs, cloaks, etc.), destroyables, colour variables, further writing implements, tables and chairs, ranged covers, medical items, prosthetic limbs, dice, torches and lanterns, repair kits, water sources and smokeable tobacco.\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
				(context, questions) => true,
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("modernitems",
				"Do you want to install some common modern setting item component types like batteries, chargers, power plugs, powered lights, radios, electric heaters and coolers, fireplaces, campfires, grid creators, telephones, liquid grids and fuel generators?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => ClassifyModernPackagePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled,
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("tags",
				"Do you want to install pre-made tags for use with items, crafts and projects? The main reason not to do this is if you are planning on an implementation that substantially differs from the one that comes with this seeder.\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.Tags.All(x => x.Name != "Aluminothermic Welding Portion"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("autobuilder",
				"Do you want to install an auto builder that can generate random areas with randomised room descriptions?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.Terrains.Count() > 1 || questions["terrain"].EqualToAny("yes", "y"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("hints",
				"Do you want to install some newbie hints that will instruct new users about the key commands and engine concepts that they need to know?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.NewPlayerHints.Count() == 0,
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		context.Database.BeginTransaction();
		var errors = new List<string>();
		PrepareItemProtoCache(context);
		_tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

		if (questionAnswers["tags"].EqualToAny("yes", "y"))
		{
			SeedTags(context, errors);
		}

		if (questionAnswers["ai"].EqualToAny("yes", "y")) SeedAIExamples(context, errors);

		if (questionAnswers["items"].EqualToAny("yes", "y"))
		{
			SeedItemsPart1(context, questionAnswers, errors);
			SeedItemsPart2(context, questionAnswers, errors);
			SeedItemsPart3(context, questionAnswers, errors);
			SeedItemsPart4(context, questionAnswers, errors);
		}

		if (questionAnswers["modernitems"].EqualToAny("yes", "y")) SeedModernItems(context, errors);

		if (questionAnswers["terrain"].EqualToAny("yes", "y")) SeedTerrain(context, errors);

		if (questionAnswers["covers"].EqualToAny("yes", "y")) SeedRangedCovers(context, errors);

		if (questionAnswers["autobuilder"].EqualToAny("yes", "y"))
		{
			SeedTerrainAutobuilder(context, questionAnswers, errors);
		}

		if (questionAnswers["hints"].EqualToAny("yes", "y"))
		{
			SeedNewbieHints(context, errors);
		}

		context.Database.CommitTransaction();

		if (errors.Count == 0) return "The operation completed successfully.";

		return
			$"The operation completed with the following errors or warnings:\n\n{errors.ListToCommaSeparatedValues("\n")}";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		return CombinePackageStates(
			ClassifyAiPackagePresence(context),
			context.Terrains.Count() > 1 ? ShouldSeedResult.MayAlreadyBeInstalled : ShouldSeedResult.ReadyToInstall,
			SeederRepeatabilityHelper.ClassifyByPresence(
				StockItemMarkers.Select(marker => context.GameItemComponentProtos.Any(x => x.Name == marker))),
			ClassifyModernPackagePresence(context),
			context.Tags.Any(x => x.Name == "Functions")
				? ShouldSeedResult.MayAlreadyBeInstalled
				: ShouldSeedResult.ReadyToInstall);
	}

	public int SortOrder => 200;
	public string Name => "Kickstart";
	public string Tagline => "A collection of useful items to kickstart your MUD's building";

	public string FullDescription =>
		@"This package gives options for a bunch of things that are not absolutely essential and that you might want to implement differently, but that I have already gone to the effort of having set up and think you might like to use.

This includes things like some useful game item components, AI templates and the like.

Inside the package there are a few numbered #D""Core Item Packages""#3. The reason for this is that there have been updates to the useful seeder since its first release, and these sub-packages were for earlier adopters to update their existing MUDs with. I recommend that you install all of the Core Item Packages as they are appropriate for any MUD in nearly any setting.";

	private FuturemudDatabaseContext _context;

	private Account _dbaccount => _context.Accounts.First();

	private Dictionary<string, GameItemComponentProto> _itemProtos = new(StringComparer.OrdinalIgnoreCase);

	internal static ShouldSeedResult ClassifyAiPackagePresence(FuturemudDatabaseContext context)
	{
		return SeederRepeatabilityHelper.ClassifyByPresence(
			StockAiExampleNames.Select(name => context.ArtificialIntelligences.Any(x => x.Name == name)));
	}

	internal static IReadOnlyCollection<string> StockAiExampleNamesForTesting => StockAiExampleNames;
	internal static IReadOnlyCollection<string> StockModernItemMarkersForTesting => StockModernItemMarkers;

	internal static ShouldSeedResult ClassifyModernPackagePresence(FuturemudDatabaseContext context)
	{
		var presenceChecks = StockModernItemMarkers
			.Select(name => context.GameItemComponentProtos.Any(x => x.Name == name))
			.ToList();

		var fuelTag = context.Tags.FirstOrDefault(x => x.Name == "Fuel");
		if (fuelTag is not null && context.LiquidsTags.Any(x => x.TagId == fuelTag.Id))
		{
			presenceChecks.Add(context.GameItemComponentProtos.Any(x => x.Type == "FuelHeaterCooler"));
			presenceChecks.Add(context.GameItemComponentProtos.Any(x => x.Type == "Fuel Generator"));
		}

		return SeederRepeatabilityHelper.ClassifyByPresence(presenceChecks);
	}

	private static ShouldSeedResult CombinePackageStates(params ShouldSeedResult[] packageStates)
	{
		if (packageStates.All(x => x == ShouldSeedResult.ReadyToInstall))
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		if (packageStates.All(x => x == ShouldSeedResult.MayAlreadyBeInstalled))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return ShouldSeedResult.ExtraPackagesAvailable;
	}

	private GameItemComponentProto AddGameItemComponent(FuturemudDatabaseContext context, GameItemComponentProto component)
	{
		if (_itemProtos.TryGetValue(component.Name, out var existing))
		{
			return existing;
		}

		existing = context.GameItemComponentProtos
			.FirstOrDefault(x => x.Name == component.Name && x.EditableItem.RevisionStatus == 4);
		if (existing is not null)
		{
			_itemProtos[existing.Name] = existing;
			return existing;
		}

		context.GameItemComponentProtos.Add(component);
		_itemProtos[component.Name] = component;
		return component;
	}

	private void PrepareItemProtoCache(FuturemudDatabaseContext context)
	{
		_itemProtos = new Dictionary<string, GameItemComponentProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var item in context.GameItemComponentProtos.ToList())
		{
			if (item.EditableItem.RevisionStatus != 4)
			{
				continue;
			}

			_itemProtos[item.Name] = item;
		}
	}

	internal void SeedModernItemsForTesting(FuturemudDatabaseContext context)
	{
		_context = context;
		PrepareItemProtoCache(context);
		SeedModernItems(context, new List<string>());
		context.SaveChanges();
	}

	private GameItemComponentProto CreateItemProto(long id, DateTime now, string type, string name, string description, string definition)
	{
		var component = new GameItemComponentProto
		{
			Id = id,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = _dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = type,
			Name = name,
			Description = description,
			Definition = definition
		};
		return AddGameItemComponent(_context, component);
	}

	private void SeedTerrain(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.Terrains.Count() > 1)
		{
			errors.Add("Terrains were already installed, so did not add any new data.");
			return;
		}

		context.Terrains.Find(1L)!.DefaultTerrain = false;

		void AddTerrain(string name, string behaviour, double movementRate, double staminaCost,
			Difficulty hideDifficulty, Difficulty spotDifficulty, string? atmosphere, CellOutdoorsType outdoorsType,
			Color editorColour, string editorText = null, bool isdefault = false, IEnumerable<string>? tags = null)
		{
			context.Terrains.Add(new Terrain
			{
				Name = name,
				TerrainBehaviourMode = behaviour,
				MovementRate = movementRate,
				StaminaCost = staminaCost,
				HideDifficulty = (int)hideDifficulty,
				SpotDifficulty = (int)spotDifficulty,
				AtmosphereId = context.Gases.FirstOrDefault(x => x.Name == atmosphere)?.Id,
				AtmosphereType = "Gas",
				InfectionMultiplier = 1.0,
				InfectionType = (int)InfectionType.Simple,
				InfectionVirulence = (int)Difficulty.Normal,
				ForagableProfileId = 0,
				DefaultTerrain = isdefault,
				TerrainEditorColour = $"#{editorColour.R:X2}{editorColour.G:X2}{editorColour.B:X2}",
				TerrainEditorText = editorText,
				DefaultCellOutdoorsType = (int)outdoorsType,
				TagInformation = tags is not null ?
					tags.SelectNotNull(x => _tags[x]?.Id.ToString("F0")).ListToCommaSeparatedValues() :
					""
			});
			context.SaveChanges();
		}

		var poolwater = context.Liquids.First(x => x.Name == "pool water");
		var springwater = context.Liquids.First(x => x.Name == "spring water");
		var saltwater = context.Liquids.First(x => x.Name == "salt water");
		var brackishwater = context.Liquids.First(x => x.Name == "brackish water");
		var riverwater = context.Liquids.First(x => x.Name == "river water");
		var lakewater = context.Liquids.First(x => x.Name == "lake water");
		var swampwater = context.Liquids.First(x => x.Name == "swamp water");

		#region Urban

		AddTerrain("Residence", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.CornflowerBlue, "Re", true,
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Bedroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.MediumPurple, "Br",
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Kitchen", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.Orange, "Ki",
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Bathroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.SkyBlue, "To",
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Living Room", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.SeaGreen, "LR",
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Hallway", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Indoors, Color.CadetBlue, "Hw",
				tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Hall", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Indoors, Color.Teal, "Ha", tags: ["Urban", "Administrative", "Public"]);
		AddTerrain("Barracks", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Indoors, Color.OliveDrab, "Bk", tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Gymnasium", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Indoors, Color.Goldenrod, "Gy", tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Shopfront", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.SandyBrown, "Sf",
				tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Workshop", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.SaddleBrown, "Ws",
				tags: ["Urban", "Industrial", "Private"]);
		AddTerrain("Office", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.LightSteelBlue, "Of",
				tags: ["Urban", "Administrative", "Private"]);
		AddTerrain("Factory", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.Silver, "Fa",
				tags: ["Urban", "Industrial", "Private"]);
		AddTerrain("Warehouse", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.DarkGray, "Wh",
				tags: ["Urban", "Industrial", "Private"]);
		AddTerrain("Indoor Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
				"Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.Plum, "Im",
				tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Underground Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
				"Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.DarkOrchid, "Um",
				tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsWithWindows, Color.DimGray, "Ga",
				tags: ["Urban", "Industrial", "Private"]);
		AddTerrain("Underground Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
				"Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.DarkSlateGray, "Ug",
				tags: ["Urban", "Industrial", "Private"]);
		AddTerrain("Barn", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Indoors, Color.Brown, "Bn", tags: ["Rural"]);
		AddTerrain("Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsNoLight, Color.LightSlateGray, "Ce",
				tags: ["Urban", "Administrative", "Private"]);
		AddTerrain("Dank Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsNoLight, Color.Gray, "Dc",
				tags: ["Urban", "Administrative", "Private"]);
		AddTerrain("Dungeon", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsNoLight, Color.Indigo, "Du",
				tags: ["Urban", "Administrative", "Private"]);
		AddTerrain("Grotto", "cave", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsNoLight, Color.DarkSlateBlue, "Gr", tags: ["Rural"]);
		AddTerrain("Cellar", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
				 CellOutdoorsType.IndoorsNoLight, Color.BurlyWood, "Cl",
				 tags: ["Urban", "Residential", "Private"]);
		AddTerrain("Baths", "indoors", 0.5, 3.0, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
				 "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.LightBlue, "Bt",
				 tags: ["Urban", "Aquatic", "Commercial", "Public"]);
		AddTerrain("Indoor Pool", $"shallowwater {poolwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
				 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DeepSkyBlue, "IP",
				 tags: ["Urban", "Aquatic", "Private"]);
		AddTerrain("Indoor Spring", $"shallowwater {springwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
				 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.MediumAquamarine, "IS", tags: ["Rural", "Aquatic"]);

		AddTerrain("Rooftop", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.DarkSlateGray, tags: ["Urban", "Private"]);
		AddTerrain("Ghetto Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban", "Public"]);
		AddTerrain("Slum Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.Gray, tags: ["Urban", "Public"]);
		AddTerrain("Poor Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
		AddTerrain("Urban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGray, tags: ["Urban", "Public"]);
		AddTerrain("Suburban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightSlateGray, tags: ["Urban", "Public"]);
		AddTerrain("Wealthy Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Gainsboro, tags: ["Urban", "Public"]);
		AddTerrain("Village Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGray, tags: ["Urban", "Public"]);
		AddTerrain("Rural Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.WhiteSmoke, tags: ["Urban", "Public"]);

		AddTerrain("Marketplace", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.VeryEasy, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Courtyard", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Private"]);
		AddTerrain("Park", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Public", "Diggable Soil"]);
		AddTerrain("Garden", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
		AddTerrain("Lawn", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
		AddTerrain("Showground", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Automatic,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen,
				tags: ["Urban", "Commercial", "Public", "Diggable Soil"]);
		AddTerrain("Forum", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Public"]);
		AddTerrain("Public Square", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
				tags: ["Urban", "Administrative", "Public"]);
		AddTerrain("Outdoor Mall", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
				tags: ["Urban", "Commercial", "Public"]);
		AddTerrain("Alleyway", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
		AddTerrain("Garbage Dump", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
				tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
		AddTerrain("Midden Heap", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
				"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
				tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
		AddTerrain("Gatehouse", "indoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Trivial, "Breathable Atmosphere",
				CellOutdoorsType.IndoorsClimateExposed, Color.SlateGray,
				tags: ["Urban", "Administrative", "Private"]);
		AddTerrain("Battlement", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Trivial, "Breathable Atmosphere",
				CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Private"]);

		#endregion

		#region Roads

		AddTerrain("Animal Trail", "outdoors", 1.75, 10.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Trail", "outdoors", 1.6, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Dirt Road", "outdoors", 1.5, 10.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Compacted Dirt Road", "outdoors", 1.4, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
		AddTerrain("Gravel Road", "outdoors", 1.3, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
		AddTerrain("Cobblestone Road", "outdoors", 1.2, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
		AddTerrain("Asphalt Road", "outdoors", 1.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban"]);

		#endregion

		#region Terrestrial

		AddTerrain("Grasslands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Savannah", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Shrublands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Steppe", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Shortgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Tallgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Heath", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Pasture", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Meadow", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Field", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Tundra", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Flood Plain", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);

		AddTerrain("Hills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Foothills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Mound", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Drumlin", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Butte", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Kuppe", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Mesa", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Canyon", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Knoll", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Moor", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Tell", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Dunes", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);

		AddTerrain("Mountainside", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Mountain Pass", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Mountain Ridge", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
		AddTerrain("Cliff Face", "cliff", 5.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
		AddTerrain("Cliff Edge", "outdoors", 5.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);

		AddTerrain("Valley", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Vale", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Dell", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Glen", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Strath", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Combe", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Ravine", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Gorge", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Gully", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);

		AddTerrain("Boreal Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Broadleaf Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Temperate Coniferous Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Temperate Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Tropical Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Bramble", "talltrees", 3.0, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Plantation Forest", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Orchard", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Grove", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
		AddTerrain("Woodland", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);

		AddTerrain("Bog", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);
		AddTerrain("Salt Marsh", $"shallowwater {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Mangrove Swamp", $"shallowwatertrees {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);
		AddTerrain("Wetland", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);
		AddTerrain("Swamp Forest", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);
		AddTerrain("Tropical Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);
		AddTerrain("Temperate Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Diggable Soil", "Foragable Clay"]);

		AddTerrain("Sandy Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);
		AddTerrain("Rocky Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Diggable Soil"]);
		AddTerrain("Coastal Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);

		AddTerrain("Cave Entrance", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial"]);
		AddTerrain("Cave", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial"]);
		AddTerrain("Cavern", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial"]);
		AddTerrain("Cave Pool", $"watercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Aquatic"]);
		AddTerrain("Underground Water", $"deepunderwater {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
			Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Aquatic"]);

		#endregion

		#region Water

		AddTerrain("Sandy Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Sand"]);
		AddTerrain("Rocky Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
		AddTerrain("Beachrock", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
		AddTerrain("Riverbank", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Riparian", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Lake Shore", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);

		AddTerrain("Ocean Shallows", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Ocean Surf", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Ocean", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Mudflat", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Bay", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Lagoon", $"water {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Cove", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Tide Pool", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Shoal", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Coral Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Sound", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
		AddTerrain("Estuary", $"shallowwater {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Shallow River", $"shallowwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("River", $"water {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Deep River", $"deepwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Shallow Lake", $"shallowwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Lake", $"water {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Deep Lake", $"deepwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
		AddTerrain("Deep Ocean", $"verydeepunderwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane,
			Difficulty.Automatic, null, CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);

		#endregion

		#region Autobuilders

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Name = "Blank",
			TemplateType = "simple",
			Definition = @"<Template>
	<RoomName>An Unnamed Room</RoomName>
	<CellDescription>This room has not been given a description.</CellDescription>
	<ShowCommandByline>Create a blank, undescribed room</ShowCommandByline>
</Template>"
		});
		context.SaveChanges();

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Rectangle",
			Definition = "<Definition/>",
			TemplateType = "rectangle"
		});
		context.SaveChanges();

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Rectangle Diagonals",
			Definition = "<Definition/>",
			TemplateType = "rectangle diagonals"
		});
		context.SaveChanges();

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Terrain Rectangle",
			Definition = "<Definition connect_diagonals=\"false\"/>",
			TemplateType = "terrain rectangle"
		});
		context.SaveChanges();

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Terrain Rectangle Diagonals",
			Definition = "<Definition connect_diagonals=\"true\"/>",
			TemplateType = "terrain rectangle"
		});

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Feature Rectangle",
			Definition = "<Definition connect_diagonals=\"false\"/>",
			TemplateType = "terrain feature rectangle"
		});
		context.SaveChanges();

		context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
		{
			Name = "Feature Rectangle Diagonals",
			Definition = "<Definition connect_diagonals=\"true\"/>",
			TemplateType = "terrain feature rectangle"
		});
		context.SaveChanges();

		//context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		//{
		//    Name = "Variable",
		//    TemplateType = "room random description",
		//    Definition = @""
		//});
		//context.SaveChanges();

		#endregion
	}

	private void SeedModernItems(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		var now = DateTime.UtcNow;
		var dbaccount = context.Accounts.First();
		var nextId = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;
		var mainsSocketType = context.StaticConfigurations
			.FirstOrDefault(x => x.SettingName == "DefaultPowerSocketType")
			?.Definition ?? "NEMA 5-15";
		var fuelTag = context.Tags.FirstOrDefault(x => x.Name == "Fuel");

		GameItemComponentProto CreateModernComponent(string type, string name, string description, XElement definition)
		{
			return CreateComponent(context, ref nextId, dbaccount, now, type, name, description, definition.ToString());
		}

		void CreateBattery(string batteryType, double wattHours, double wattHoursPerQuality, bool rechargeable)
		{
			var name = rechargeable ? $"Battery_{batteryType}_Rechargeable" : $"Battery_{batteryType}";
			var description = rechargeable
				? $"Turns an item into a rechargeable {batteryType} battery."
				: $"Turns an item into a disposable {batteryType} battery.";
			CreateModernComponent("Battery", name, description,
				new XElement("Definition",
					new XElement("BatteryType", new XCData(batteryType)),
					new XElement("BaseWattHours", wattHours),
					new XElement("WattHoursPerQuality", wattHoursPerQuality),
					new XElement("Rechargable", rechargeable)
				));
		}

		void CreateBatteryPowered(string batteryType, int quantity, bool inSeries, bool transparent = false, string preposition = "in")
		{
			CreateModernComponent("BatteryPowered", $"BatteryPowered_{quantity}x{batteryType}",
				$"Turns an item into a {quantity}x {batteryType} battery powered device.",
				new XElement("Definition",
					new XElement("BatteryType", batteryType),
					new XElement("BatteryQuantity", quantity),
					new XElement("BatteriesInSeries", inSeries),
					new XElement("Transparent", transparent),
					new XElement("ContentsPreposition", preposition)
				));
		}

		void CreateBatteryCharger(string batteryType, int quantity, double wattage, double efficiency, bool transparent = true, string? suffix = null)
		{
			var bayName = suffix ?? $"{quantity}Bay";
			CreateModernComponent("BatteryCharger", $"BatteryCharger_{batteryType}_{bayName}",
				$"Turns an item into a charger for {quantity} {batteryType} batter{(quantity == 1 ? "y" : "ies")} at a time.",
				new XElement("Definition",
					new XElement("BatteryType", batteryType),
					new XElement("BatteryQuantity", quantity),
					new XElement("Wattage", wattage),
					new XElement("Efficiency", efficiency),
					new XElement("ContentsPreposition", "in"),
					new XElement("Transparent", transparent)
				));
		}

		XElement ConnectorDefinition(params ConnectorType[] connectors)
		{
			return new XElement("Definition",
				new XElement("Connectors",
					from connector in connectors
					select new XElement("Connection",
						new XAttribute("gender", (short)connector.Gender),
						new XAttribute("type", connector.ConnectionType),
						new XAttribute("powered", connector.Powered)
					)));
		}

		XElement ThermalDefinition(double ambientHeat, double intimateHeat, double immediateHeat, double proximateHeat,
			double distantHeat, double veryDistantHeat, string activeDescription, string inactiveDescription,
			params object[] extraElements)
		{
			return new XElement("Definition",
				new XElement("AmbientHeat", ambientHeat),
				new XElement("IntimateHeat", intimateHeat),
				new XElement("ImmediateHeat", immediateHeat),
				new XElement("ProximateHeat", proximateHeat),
				new XElement("DistantHeat", distantHeat),
				new XElement("VeryDistantHeat", veryDistantHeat),
				new XElement("ActiveDescriptionAddendum", new XCData(activeDescription)),
				new XElement("InactiveDescriptionAddendum", new XCData(inactiveDescription)),
				extraElements);
		}

		XElement SwitchableThermalDefinition(double ambientHeat, double intimateHeat, double immediateHeat,
			double proximateHeat, double distantHeat, double veryDistantHeat, string activeDescription,
			string inactiveDescription, string switchOnEmote, string switchOffEmote, params object[] extraElements)
		{
			var definition = ThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
				veryDistantHeat, activeDescription, inactiveDescription, extraElements);
			definition.Add(
				new XElement("SwitchOnEmote", new XCData(switchOnEmote)),
				new XElement("SwitchOffEmote", new XCData(switchOffEmote)));
			return definition;
		}

		void CreateElectricHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
			double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat, double wattage,
			string activeDescription, string inactiveDescription, string switchOnEmote, string switchOffEmote)
		{
			CreateModernComponent("ElectricHeaterCooler", name, description,
				SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
					veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
					new XElement("Wattage", wattage)));
		}

		void CreateConsumableHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
			double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat, int secondsOfFuel,
			string activeDescription, string inactiveDescription, string fuelExpendedEcho)
		{
			CreateModernComponent("ConsumableHeaterCooler", name, description,
				ThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat, veryDistantHeat,
					activeDescription, inactiveDescription,
					new XElement("SecondsOfFuel", secondsOfFuel),
					new XElement("SpentItemProto", 0),
					new XElement("FuelExpendedEcho", new XCData(fuelExpendedEcho))));
		}

		void CreateSolidFuelHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
			double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat,
			double maximumFuelWeight, double secondsPerUnitWeight, string activeDescription,
			string inactiveDescription, string switchOnEmote, string switchOffEmote)
		{
			CreateModernComponent("SolidFuelHeaterCooler", name, description,
				SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
					veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
					new XElement("FuelTag", fuelTag?.Id ?? 0),
					new XElement("MaximumFuelWeight", maximumFuelWeight),
					new XElement("SecondsPerUnitWeight", secondsPerUnitWeight)));
		}

		void CreateLiquidFuelHeaterCooler(string variantName, MudSharp.Models.Liquid liquid, string description,
			double ambientHeat,
			double intimateHeat, double immediateHeat, double proximateHeat, double distantHeat,
			double veryDistantHeat, double fuelPerSecond, string activeDescription, string inactiveDescription,
			string switchOnEmote, string switchOffEmote)
		{
			var safeFuelName = SanitizeComponentName(liquid.Name);
			CreateModernComponent("FuelHeaterCooler", $"FuelHeaterCooler_{variantName}_{safeFuelName}", description,
				SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
					veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
					new XElement("FuelMedium", 0),
					new XElement("LiquidFuel", liquid.Id),
					new XElement("GasFuel", 0),
					new XElement("FuelPerSecond", fuelPerSecond),
					new XElement("Connector",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false))));
		}

		void CreatePowerSocket(string name, int count)
		{
			CreateModernComponent("PowerSocket", name,
				$"Turns an item into a {count}-socket mains power outlet.",
				ConnectorDefinition(
					Enumerable.Range(0, count)
						.Select(_ => new ConnectorType(Gender.Female, mainsSocketType, true))
						.ToArray()));
		}

		void CreatePowerSupply(double wattage)
		{
			CreateModernComponent("PowerSupply", $"PowerSupply_{wattage:N0}W",
				$"Turns an item into a mains-powered device that draws {wattage:N0}W.",
				new XElement("Definition",
					new XElement("Wattage", wattage)));
		}

		string SanitizeComponentName(string text)
		{
			return new string(text
				.Select(x => char.IsLetterOrDigit(x) ? x : '_')
				.ToArray())
				.Trim('_')
				.Replace("__", "_");
		}

		CreateBattery("A", 6.0, 0.35, false);
		CreateBattery("A", 5.2, 0.3, true);
		CreateBattery("AA", 2.4, 0.12, false);
		CreateBattery("AA", 2.0, 0.1, true);
		CreateBattery("AAA", 1.2, 0.08, false);
		CreateBattery("AAA", 0.95, 0.06, true);
		CreateBattery("C", 8.0, 0.45, false);
		CreateBattery("C", 6.8, 0.35, true);
		CreateBattery("D", 18.0, 0.9, false);
		CreateBattery("D", 14.0, 0.7, true);
		CreateBattery("9V", 4.5, 0.25, false);
		CreateBattery("9V", 3.8, 0.2, true);
		CreateBattery("ButtonCell", 0.22, 0.015, false);
		CreateBattery("ButtonCell", 0.18, 0.01, true);
		CreateBattery("CarBattery", 480.0, 20.0, false);
		CreateBattery("CarBattery", 420.0, 15.0, true);

		CreateBatteryPowered("ButtonCell", 1, true);
		CreateBatteryPowered("ButtonCell", 2, true);
		CreateBatteryPowered("9V", 1, true);
		CreateBatteryPowered("9V", 2, true);
		CreateBatteryPowered("AAA", 1, true);
		CreateBatteryPowered("AAA", 2, true);
		CreateBatteryPowered("AAA", 3, true);
		CreateBatteryPowered("AAA", 4, true);
		CreateBatteryPowered("AA", 1, true);
		CreateBatteryPowered("AA", 2, true);
		CreateBatteryPowered("AA", 4, true);
		CreateBatteryPowered("AA", 6, true);
		CreateBatteryPowered("AA", 8, true);
		CreateBatteryPowered("A", 1, true);
		CreateBatteryPowered("A", 2, true);
		CreateBatteryPowered("A", 4, true);
		CreateBatteryPowered("C", 2, true);
		CreateBatteryPowered("C", 4, true);
		CreateBatteryPowered("D", 2, true);
		CreateBatteryPowered("D", 4, true);
		CreateBatteryPowered("D", 6, true);
		CreateBatteryPowered("CarBattery", 1, true, false, "in");

		CreateBatteryCharger("ButtonCell", 2, 1.0, 0.82);
		CreateBatteryCharger("9V", 2, 8.0, 0.85);
		CreateBatteryCharger("AAA", 2, 4.0, 0.88);
		CreateBatteryCharger("AAA", 4, 8.0, 0.9);
		CreateBatteryCharger("AAA", 8, 12.0, 0.92);
		CreateBatteryCharger("AA", 2, 6.0, 0.88);
		CreateBatteryCharger("AA", 4, 12.0, 0.9);
		CreateBatteryCharger("AA", 8, 18.0, 0.92);
		CreateBatteryCharger("A", 4, 14.0, 0.88);
		CreateBatteryCharger("C", 4, 30.0, 0.87, false);
		CreateBatteryCharger("D", 4, 50.0, 0.86, false);
		CreateBatteryCharger("CarBattery", 1, 180.0, 0.84, false, "Workshop");

		CreateModernComponent("Connectable", "Connectable_Male_To_MainsPlug",
			"Turns an item into a male connection to a standard mains plug.",
			ConnectorDefinition(new ConnectorType(Gender.Male, mainsSocketType, true)));
		CreateModernComponent("Connectable", "Connectable_MainsPlug_PassThrough",
			"Turns an item into a standard mains plug with a pass-through socket, like an extension lead.",
			ConnectorDefinition(
				new ConnectorType(Gender.Male, mainsSocketType, true),
				new ConnectorType(Gender.Female, mainsSocketType, true)));
		CreateModernComponent("Connectable", "Connectable_SingleFemale",
			"Turns an item into a female connection with a single female plug.",
			ConnectorDefinition(new ConnectorType(Gender.Female, mainsSocketType, true)));
		CreateModernComponent("Attachable Connectable", "AttachableConnectable_PowerLead",
			"Turns an item into an attachable mains lead or detachable power cable.",
			new XElement("Definition",
				new XElement("Connector",
					new XAttribute("gender", (short)Gender.Neuter),
					new XAttribute("type", "PowerLead"))));
		CreatePowerSocket("PowerSocket_Mains_Single", 1);
		CreatePowerSocket("PowerSocket_Mains_Double", 2);
		CreatePowerSocket("PowerSocket_Mains_Quad", 4);

		foreach (var wattage in new[] { 5.0, 15.0, 30.0, 60.0, 100.0, 250.0, 500.0, 1000.0, 1500.0, 2400.0 })
		{
			CreatePowerSupply(wattage);
		}

		CreateModernComponent("ElectricLight", "ElectricLight_Low",
			"Turns an item into a low power electric light source.",
			new XElement("Definition",
				new XElement("IlluminationProvided", 40),
				new XElement("Wattage", 5),
				new XElement("OnLightProg", 0),
				new XElement("OnOffProg", 0),
				new XElement("LightOnEmote", new XCData("@ glow|glows with a soft light.")),
				new XElement("LightOffEmote", new XCData("@ go|goes dark."))));
		CreateModernComponent("ElectricLight", "ElectricLight_Medium",
			"Turns an item into a medium brightness electric light source.",
			new XElement("Definition",
				new XElement("IlluminationProvided", 180),
				new XElement("Wattage", 15),
				new XElement("OnLightProg", 0),
				new XElement("OnOffProg", 0),
				new XElement("LightOnEmote", new XCData("@ light|lights up.")),
				new XElement("LightOffEmote", new XCData("@ go|goes dark."))));
		CreateModernComponent("ElectricLight", "ElectricLight_Bright",
			"Turns an item into a bright electric floodlight.",
			new XElement("Definition",
				new XElement("IlluminationProvided", 800),
				new XElement("Wattage", 60),
				new XElement("OnLightProg", 0),
				new XElement("OnOffProg", 0),
				new XElement("LightOnEmote", new XCData("@ flare|flares to life.")),
				new XElement("LightOffEmote", new XCData("@ dim|dims and go|goes dark."))));

		CreateModernComponent("PoweredProp", "PoweredProp_Switchable",
			"Turns an item into a general-purpose switchable powered prop or appliance.",
			new XElement("Definition",
				new XElement("Wattage", 250),
				new XElement("WattageDiscount", 12),
				new XElement("Switchable", true),
				new XElement("PowerOnEmote", new XCData("@ hum|hums as it powers on.")),
				new XElement("PowerOffEmote", new XCData("@ wind|winds down and power|powers off.")),
				new XElement("OnPoweredProg", 0),
				new XElement("OnUnpoweredProg", 0),
				new XElement("TenSecondProg", 0)));
		CreateModernComponent("PoweredProp", "PoweredProp_AlwaysOn",
			"Turns an item into an always-on powered prop such as signage or infrastructure.",
			new XElement("Definition",
				new XElement("Wattage", 40),
				new XElement("WattageDiscount", 5),
				new XElement("Switchable", false),
				new XElement("PowerOnEmote", new XCData("@ click|clicks softly as it powers up.")),
				new XElement("PowerOffEmote", new XCData("@ fall|falls silent as power is lost.")),
				new XElement("OnPoweredProg", 0),
				new XElement("OnUnpoweredProg", 0),
				new XElement("TenSecondProg", 0)));

		CreateModernComponent("HandheldRadio", "HandheldRadio_Standard",
			"Turns an item into a battery-powered handheld two-way radio.",
			new XElement("Definition",
				new XElement("WattageIdle", 1.5),
				new XElement("WattageTransmit", 80.0),
				new XElement("WattageReceive", 18.0),
				new XElement("BroadcastRange", 6000.0),
				new XElement("OnPowerOffEmote", new XCData("@ give|gives a small burst of static and power|powers down.")),
				new XElement("OnPowerOnEmote", new XCData("@ crackle|crackles to life.")),
				new XElement("TransmitPremote", new XCData("@ key|keys the transmitter on $1 and say|says")),
				new XElement("Channel", 476.525),
				new XElement("Channel", 477.125),
				new XElement("ChannelName", new XCData("Operations")),
				new XElement("ChannelName", new XCData("Security"))));
		CreateModernComponent("EarpieceRadio", "EarpieceRadio_Covert",
			"Turns an item into a covert receive-only earpiece radio.",
			new XElement("Definition",
				new XElement("WattageIdle", 0.25),
				new XElement("WattageReceive", 3.0),
				new XElement("OnPowerOffEmote", new XCData("@ click|clicks off.")),
				new XElement("OnPowerOnEmote", new XCData("@ emit|emits a brief burst of static.")),
				new XElement("Channel", 476.525),
				new XElement("Channel", 477.125),
				new XElement("ChannelName", new XCData("Operations")),
				new XElement("ChannelName", new XCData("Security"))));
		CreateModernComponent("ListeningBug", "ListeningBug_Covert",
			"Turns an item into a covert powered listening bug.",
			new XElement("Definition",
				new XElement("BroadcastFrequency", 433.920),
				new XElement("BroadcastRange", 800.0),
				new XElement("ListenSkillPerQuality", 4.0),
				new XElement("BaseListenSkill", 45.0),
				new XElement("PowerConsumptionInWatts", 0.0015)));
		CreateModernComponent("ElectricGridCreator", "ElectricGridCreator_Standard",
			"Turns an item into a creator for an electrical grid.",
			new XElement("Definition"));
		CreateModernComponent("LiquidGridCreator", "LiquidGridCreator_Standard",
			"Turns an item into a creator for a liquid grid.",
			new XElement("Definition"));
		CreateModernComponent("TelecommunicationsGridCreator", "TelecommunicationsGridCreator_Standard",
			"Turns an item into a creator for a telecommunications grid.",
			new XElement("Definition",
				new XElement("Prefix", "555"),
				new XElement("NumberLength", 6)));
		CreateModernComponent("Telephone", "Telephone_Standard",
			"Turns an item into a standard telephone.",
			ConnectorDefinition(
				new ConnectorType(Gender.Male, "TelephoneLine", true)));
		CreateModernComponent("ElectricGridFeeder", "ElectricGridFeeder_Standard",
			"Turns an item into a feeder for the electrical grid.",
			ConnectorDefinition(
				new ConnectorType(Gender.Male, mainsSocketType, true)));
		CreateModernComponent("TelecommunicationsGridFeeder", "TelecommunicationsGridFeeder_Standard",
			"Turns an item into a feeder for supplying power into the telecommunications grid.",
			new XElement("Definition",
				new XElement("Wattage", 20.0),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", mainsSocketType),
						new XAttribute("powered", true)))));
		CreateModernComponent("TelecommunicationsGridOutlet", "TelecommunicationsGridOutlet",
			"Turns an item into an outlet to plug a landline telephone into.",
			new XElement("Definition",
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "TelephoneLine"),
						new XAttribute("powered", true)))));
		CreateModernComponent("GridLiquidSource", "GridLiquidSource_Standard",
			"Turns an item into a liquid grid source and physical connector.",
			ConnectorDefinition(
				new ConnectorType(Gender.Male, "LiquidLine", false)));
		CreateModernComponent("LiquidGridSupplier", "LiquidGridSupplier_Standard",
			"Turns an item into a liquid grid supplier that feeds from a sibling liquid container.",
			new XElement("Definition"));
		CreateModernComponent("LiquidPump", "LiquidPump_Standard",
			"Turns an item into a powered liquid pump with input and output connectors.",
			new XElement("Definition",
				new XElement("FlowRate", 1.0),
				new XElement("Wattage", 25.0),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", mainsSocketType),
						new XAttribute("powered", true)),
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)),
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)))));
		CreateModernComponent("LiquidPump", "LiquidPump_Industrial",
			"Turns an item into a high-throughput powered liquid pump.",
			new XElement("Definition",
				new XElement("FlowRate", 5.0),
				new XElement("Wattage", 120.0),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", mainsSocketType),
						new XAttribute("powered", true)),
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)),
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)))));
		CreateModernComponent("LiquidConsumingProp", "LiquidConsumingProp_Standard",
			"Turns an item into a prop that steadily consumes liquid.",
			new XElement("Definition",
				new XElement("LiquidCapacity", 10.0),
				new XElement("ConsumptionPerSecond", 0.1),
				new XElement("Transparent", true),
				new XElement("CanBeEmptiedWhenInRoom", true),
				new XElement("ContentsPreposition", "in"),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)))));
		CreateModernComponent("LiquidConsumingProp", "LiquidConsumingProp_Basin",
			"Turns an item into a larger prop that steadily drains liquid for steady-state testing.",
			new XElement("Definition",
				new XElement("LiquidCapacity", 25.0),
				new XElement("ConsumptionPerSecond", 0.5),
				new XElement("Transparent", false),
				new XElement("CanBeEmptiedWhenInRoom", true),
				new XElement("ContentsPreposition", "in"),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "LiquidLine"),
						new XAttribute("powered", false)))));
		CreateModernComponent("BatteryPowered", "BatteryPowered_4xAA_Connectable",
			"Turns an item into a 4xAA battery powered device that can also charge from a power source.",
			new XElement("Definition",
				new XElement("BatteryType", "AA"),
				new XElement("BatteryQuantity", 4),
				new XElement("BatteriesInSeries", true),
				new XElement("ChargeWattage", 20.0),
				new XElement("Transparent", false),
				new XElement("ContentsPreposition", "in"),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", mainsSocketType),
						new XAttribute("powered", true)))));
		CreateModernComponent("BatteryPowered", "BatteryPowered_LaptopStyle",
			"Turns an item into a laptop-style lithium ion battery backed device with a charging connector.",
			new XElement("Definition",
				new XElement("BatteryType", "Li-Ion"),
				new XElement("BatteryQuantity", 1),
				new XElement("BatteriesInSeries", true),
				new XElement("ChargeWattage", 65.0),
				new XElement("Transparent", false),
				new XElement("ContentsPreposition", "in"),
				new XElement("Connectors",
					new XElement("Connection",
						new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "USB-C"),
						new XAttribute("powered", true)))));

		CreateElectricHeaterCooler("ElectricHeaterCooler_SpaceHeater",
			"Turns an item into a compact mains-powered space heater.",
			4.0, 12.0, 8.0, 4.0, 1.5, 0.5, 1500.0,
			"It radiates a steady wave of dry heat.",
			"It is cold and silent.",
			"@ click|clicks and begin|begins pushing out warm air.",
			"@ click|clicks off and the warm airflow fade|fades.");
		CreateElectricHeaterCooler("ElectricHeaterCooler_WallHeater",
			"Turns an item into a larger fixed electric wall heater.",
			6.0, 10.0, 7.5, 4.5, 2.5, 1.0, 2400.0,
			"It gives off a broad, comfortable warmth.",
			"It is currently inactive and cool to the touch.",
			"@ hum|hums softly as heating elements glow to life.",
			"@ tick|ticks quietly as the heating elements cool.");
		CreateElectricHeaterCooler("ElectricHeaterCooler_PortableCooler",
			"Turns an item into a portable electric cooler or compact air conditioner.",
			-3.5, -9.0, -6.0, -3.5, -1.5, -0.5, 900.0,
			"It pushes out a stream of chilled air.",
			"It is switched off and room temperature.",
			"@ whirr|whirrs to life and begin|begins venting chilled air.",
			"@ wind|winds down and the chilled airflow stop|stops.");
		CreateElectricHeaterCooler("ElectricHeaterCooler_IndustrialCooler",
			"Turns an item into an industrial electric cooling unit.",
			-6.0, -10.0, -8.0, -5.0, -3.0, -1.2, 3200.0,
			"It drones continuously while dumping cold air into the room.",
			"It is currently inactive and silent.",
			"@ thrum|thrums to life with a blast of refrigerated air.",
			"@ cycle|cycles down and the cold draft ebb|ebbs away.");

		CreateConsumableHeaterCooler("ConsumableHeaterCooler_SmallFire",
			"Turns an item into a small temporary fire such as a hurried cooking fire or signal flame.",
			2.0, 10.0, 7.0, 3.0, 1.0, 0.3, 900,
			"It burns with a modest crackling flame.",
			"It has burned down to cold ash.",
			"$0 gutter|gutters and collapse|collapses into dying embers.");
		CreateConsumableHeaterCooler("ConsumableHeaterCooler_LargeFire",
			"Turns an item into a larger campfire-sized temporary blaze.",
			5.0, 18.0, 14.0, 8.0, 3.0, 1.0, 3600,
			"It burns hot and bright, throwing out waves of heat.",
			"It has burned out.",
			"$0 spit|spits a final shower of sparks before the flames die away.");
		CreateConsumableHeaterCooler("ConsumableHeaterCooler_Bonfire",
			"Turns an item into a large temporary bonfire.",
			8.0, 25.0, 18.0, 10.0, 5.0, 2.0, 10800,
			"It roars with an intense bonfire heat.",
			"It has collapsed into a mound of cold charcoal and ash.",
			"$0 roar|roars one last time before collapsing into a heap of glowing embers.");

		CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_Brazier",
			"Turns an item into a solid-fuel brazier that accepts tagged fuel items.",
			3.0, 12.0, 8.0, 4.5, 2.0, 0.8, 8.0, 1100.0,
			"It is filled with glowing fuel and wafting heat.",
			"It is empty and cold.",
			"@ flare|flares up as the fuel catch|catches.",
			"@ dim|dims as the brazier is shut down.");
		CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_Fireplace",
			"Turns an item into a room-warming fireplace that burns tagged solid fuel.",
			6.0, 18.0, 12.0, 7.0, 3.5, 1.5, 35.0, 1800.0,
			"It crackles warmly with a bed of burning fuel.",
			"It is laid cold and dark.",
			"@ catch|catches and begin|begins to roar warmly up the flue.",
			"@ settle|settles down as the flames are banked.");
		CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_WoodStove",
			"Turns an item into a cast-iron stove that burns tagged solid fuel.",
			7.0, 16.0, 11.0, 6.5, 3.0, 1.2, 20.0, 2400.0,
			"It rings softly with stored heat from the burning fuel within.",
			"It is cold iron with no glow inside.",
			"@ rumble|rumbles as the stove draws properly and the fire takes.",
			"@ clank|clanks shut as the stove is damped down.");

		if (fuelTag is not null)
		{
			foreach (var liquid in context.Liquids
				         .Where(x => x.LiquidsTags.Any(y => y.TagId == fuelTag.Id))
				         .OrderBy(x => x.Name)
				         .ToList())
			{
				var safeName = SanitizeComponentName(liquid.Name);
				CreateLiquidFuelHeaterCooler("PortableHeater", liquid,
					$"Turns an item into a portable radiant heater that burns {liquid.Name} from a connected liquid supply.",
					4.0, 14.0, 9.0, 5.0, 2.0, 0.5, 0.00025,
					"It gives off a close, oily heat while the burner is lit.",
					"It is unlit and cool.",
					"@ hiss|hisses softly as the burner ignite|ignites.",
					"@ gutter|gutters and go|goes out.");
				CreateLiquidFuelHeaterCooler("WorkshopStove", liquid,
					$"Turns an item into a heavier workshop heater or stove that burns {liquid.Name} from a connected liquid supply.",
					6.5, 18.0, 12.0, 7.0, 3.0, 1.0, 0.00045,
					"It radiates a sustained mechanical heat from its burner chamber.",
					"It is currently inactive and cold.",
					"@ chuff|chuffs into life as the burner stabilise|stabilises.",
					"@ sputter|sputters and die|dies down.");
				CreateModernComponent("Fuel Generator", $"FuelGenerator_{safeName}",
					$"Turns an item into a portable generator fuelled by {liquid.Name}.",
					new XElement("Definition",
						new XElement("SwitchOnEmote", new XCData("@ pull|pulls $1 to life with a sputtering roar.")),
						new XElement("SwitchOffEmote", new XCData("@ shut|shuts $1 down.")),
						new XElement("FuelExpendedEmote", new XCData("@ cough|coughs, splutter|splatters and die|dies as it runs out of fuel.")),
						new XElement("FuelPerSecond", 0.0025 / 3600.0),
						new XElement("FuelCapacity", 20.0),
						new XElement("WattageProvided", 5000.0),
						new XElement("SwitchOnProg", 0),
						new XElement("SwitchOffProg", 0),
						new XElement("FuelOutProg", 0),
						new XElement("LiquidFuel", liquid.Id)));
			}
		}

		context.SaveChanges();
	}

	private GameItemComponentProto CreateComponent(FuturemudDatabaseContext context,
			ref long nextId, Account account, DateTime now, string type, string name,
			string description, string definition)
	{
		var component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = account.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = account.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = type,
			Name = name,
			Description = description,
			Definition = definition
		};

		return AddGameItemComponent(context, component);
	}

	private GameItemComponentProto CreateTorchComponent(FuturemudDatabaseContext context,
	ref long nextId, Account account, DateTime now, string name, string description,
	int illuminationProvided, int secondsOfFuel, bool requiresIgnitionSource,
	string lightEmote, string extinguishEmote, string tenPercentFuelEcho,
	string fuelExpendedEcho)
	{
		var definition = @$"<Definition>
<IlluminationProvided>{illuminationProvided}</IlluminationProvided>
<SecondsOfFuel>{secondsOfFuel}</SecondsOfFuel>
<RequiresIgnitionSource>{requiresIgnitionSource.ToString().ToLower()}</RequiresIgnitionSource>
<LightEmote><![CDATA[{lightEmote}]]></LightEmote>
<ExtinguishEmote><![CDATA[{extinguishEmote}]]></ExtinguishEmote>
<TenPercentFuelEcho><![CDATA[{tenPercentFuelEcho}]]></TenPercentFuelEcho>
<FuelExpendedEcho><![CDATA[{fuelExpendedEcho}]]></FuelExpendedEcho>
</Definition>";
		return CreateComponent(context, ref nextId, account, now, "Torch", name, description,
		definition);
	}

	private GameItemComponentProto CreateLanternComponent(FuturemudDatabaseContext context,
	ref long nextId, Account account, DateTime now, string name, string description,
	int illuminationProvided, double fuelCapacity, bool requiresIgnitionSource,
	string lightEmote, string extinguishEmote, string tenPercentFuelEcho,
	string fuelExpendedEcho, long liquidFuelId, double fuelPerSecond)
	{
		var definition = @$"<Definition>
<IlluminationProvided>{illuminationProvided}</IlluminationProvided>
<FuelCapacity>{fuelCapacity.ToString(System.Globalization.CultureInfo.InvariantCulture)}</FuelCapacity>
<RequiresIgnitionSource>{requiresIgnitionSource.ToString().ToLower()}</RequiresIgnitionSource>
<LightEmote><![CDATA[{lightEmote}]]></LightEmote>
<ExtinguishEmote><![CDATA[{extinguishEmote}]]></ExtinguishEmote>
<TenPercentFuelEcho><![CDATA[{tenPercentFuelEcho}]]></TenPercentFuelEcho>
<FuelExpendedEcho><![CDATA[{fuelExpendedEcho}]]></FuelExpendedEcho>
<LiquidFuel>{liquidFuelId}</LiquidFuel>
<FuelPerSecond>{fuelPerSecond.ToString(System.Globalization.CultureInfo.InvariantCulture)}</FuelPerSecond>
</Definition>";
		return CreateComponent(context, ref nextId, account, now, "Lantern", name, description,
		definition);
	}

	private GameItemComponentProto CreateWaterSourceComponent(FuturemudDatabaseContext context,
	ref long nextId, Account account, DateTime now, string name, string description,
	double liquidCapacity, long defaultLiquidId, double refillRate, bool useOnOffForRefill)
	{
		var definition = $"<Definition LiquidCapacity=\"{liquidCapacity.ToString(System.Globalization.CultureInfo.InvariantCulture)}\" Closable=\"false\" Transparent=\"false\" OnceOnly=\"false\" DefaultLiquid=\"{defaultLiquidId}\" RefillRate=\"{refillRate.ToString(System.Globalization.CultureInfo.InvariantCulture)}\" UseOnOffForRefill=\"{useOnOffForRefill.ToString().ToLower()}\" RefillingProg=\"0\" CanBeEmptiedWhenInRoom=\"false\" />";
		return CreateComponent(context, ref nextId, account, now, "WaterSource", name, description,
		definition);
	}

	private RangedCover CreateOrGetRangedCover(FuturemudDatabaseContext context, string name, int coverType,
		int coverExtent, int highestPositionState, string descriptionString, string actionDescriptionString,
		int maximumSimultaneousCovers, bool coverStaysWhileMoving)
	{
		var cover = context.RangedCovers.FirstOrDefault(x => x.Name == name);
		if (cover is not null)
		{
			return cover;
		}

		cover = new RangedCover
		{
			Name = name,
			CoverType = coverType,
			CoverExtent = coverExtent,
			HighestPositionState = highestPositionState,
			DescriptionString = descriptionString,
			ActionDescriptionString = actionDescriptionString,
			MaximumSimultaneousCovers = maximumSimultaneousCovers,
			CoverStaysWhileMoving = coverStaysWhileMoving
		};

		context.RangedCovers.Add(cover);
		return cover;
	}

	private void SeedItemsPart1(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
			ICollection<string> errors)
	{
		var now = DateTime.UtcNow;
		var dbaccount = context.Accounts.First();
		var nextId = context.GameItemComponentProtos.Max(x => x.Id) + 1;
		_context = context;


		#region Containers

		GameItemComponentProto CreateContainer(string name, string description, double weight, SizeCategory maxSize, bool closable, bool transparent, string preposition, bool onceOnly = false)
		{
			var once = onceOnly ? " OnceOnly=\"true\"" : string.Empty;
			return CreateItemProto(nextId++, now, "Container", name, description,
					$"<Definition Weight=\"{weight}\" MaxSize=\"{(int)maxSize}\" Preposition=\"{preposition}\" Closable=\"{closable}\" Transparent=\"{transparent}\"{once} />");
		}

		CreateContainer("Container_Table", "Allows a table to have items 'on' it", 200000, SizeCategory.Large, false, true, "on");
		CreateContainer("Container_Large_Table", "Allows a large table to have items 'on' it", 500000, SizeCategory.VeryLarge, false, true, "on");
		CreateContainer("Container_Small_Table", "Allows a small table to have items 'on' it", 50000, SizeCategory.Normal, false, true, "on");
		CreateContainer("Container_Carton", "A container for cartons of cigarettes, matches etc", 250, SizeCategory.Tiny, true, false, "in");
		CreateContainer("Container_Pocket", "A container for pockets in clothes", 500, SizeCategory.VerySmall, false, false, "in");
		CreateContainer("Container_LargePocket", "A container for large pockets in clothes", 2000, SizeCategory.Small, false, false, "in");
		CreateContainer("Container_Pocket_Closable", "A container for closable pockets in clothes", 500, SizeCategory.VerySmall, true, false, "in");
		CreateContainer("Container_LargePocket_Closable", "A container for closable large pockets in clothes", 2000, SizeCategory.Small, true, false, "in");
		CreateContainer("Container_Pouch", "A container for pouches", 1000, SizeCategory.VerySmall, true, false, "in");
		CreateContainer("Container_Baggie", "A container for see-through baggies", 1000, SizeCategory.VerySmall, true, true, "in");
		CreateContainer("Container_Sachet", "A container for single-use sachets", 1000, SizeCategory.VerySmall, true, false, "in", true);
		CreateContainer("Container_Purse", "A container for purses or handbags", 7000, SizeCategory.Small, true, false, "in");
		CreateContainer("Container_Plate", "A container for plates and similar", 1500, SizeCategory.VerySmall, false, false, "on");
		CreateContainer("Container_Tray", "A container for trays, platters, etc", 7000, SizeCategory.Small, false, false, "on");
		CreateContainer("Container_Tote", "A container for tote bags or shoulder bags", 20000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_PlasticBag", "A container for transparent plastic shopping bags", 10000, SizeCategory.Normal, false, true, "in");
		CreateContainer("Container_Sack", "A container for sturdy closable sacks and other similarly sized containers", 75000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_Pack", "A container for backpacks and similar", 75000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_Drum", "A container for standard sized drums (~55 Gal)", 250000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_Small_Drum", "A container for small sized drums (~25 Gal)", 100000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_Quiver", "A container for quivers", 10000, SizeCategory.Normal, true, false, "in");
		CreateContainer("Container_Hole", "A container for holes in the ground", 2000000, SizeCategory.VeryLarge, false, false, "in");
		CreateContainer("Container_Large_Hole", "A container for large holes in the ground", 5000000, SizeCategory.Huge, false, false, "in");
		CreateContainer("Container_Shipping_Container", "A container for standard 20ft shipping containers", 50000000, SizeCategory.Enormous, true, false, "in");
		CreateContainer("Container_Shipping_Container_Long", "A container for standard 40ft shipping containers", 100000000, SizeCategory.Enormous, true, false, "in");
		CreateContainer("Container_Shipping_Container_Large", "A container for larger shipping containers", 200000000, SizeCategory.Gigantic, true, false, "in");
		CreateContainer("Container_Shipping_Container_Small", "A container for small 10ft shipping containers", 25000000, SizeCategory.Huge, true, false, "in");
		CreateContainer("Container_Colossal", "A container with unthinkably large capacity", 1000000000, SizeCategory.Titanic, false, false, "in");
		CreateContainer("Container_Coffin", "A container for coffins designed to hold a human body", 250000, SizeCategory.Large, true, false, "in");
		CreateContainer("Container_Glass_Casket", "A container for see-through glass caskets designed to display a human body", 200000, SizeCategory.Large, true, true, "in");

		context.SaveChanges();

		#endregion

		#region Liquid Containers

		GameItemComponentProto CreateLiquidContainer(string name, string description, double capacity, bool closable, bool transparent, double weightLimit, bool onceOnly = false)
		{
			var once = onceOnly ? " OnceOnly=\"true\"" : string.Empty;
			return CreateItemProto(nextId++, now, "Liquid Container", name, description,
					$"<Definition LiquidCapacity=\"{capacity}\" Closable=\"{closable}\" Transparent=\"{transparent}\" WeightLimit=\"{weightLimit}\"{once} />");
		}

		CreateLiquidContainer("LContainer_ShotGlass", "A liquid container for a shot glass", 0.12, false, true, 1000);
		CreateLiquidContainer("LContainer_WhiskeyGlass", "A liquid container for a whiskey glass (or other small glass)", 0.25, false, true, 2000);
		CreateLiquidContainer("LContainer_DrinkingGlass", "A liquid container for a drinking glass (or other normal table glass)", 0.450, false, true, 4000);
		CreateLiquidContainer("LContainer_Pony", "A liquid container for a pony (1/4 pint glass)", 0.142, false, true, 4000);
		CreateLiquidContainer("LContainer_HalfPint", "A liquid container for a half pint glass", 0.284, false, true, 4000);
		CreateLiquidContainer("LContainer_Pint", "A liquid container for a US pint", 0.473, false, true, 6000);
		CreateLiquidContainer("LContainer_UKPint", "A liquid container for a UK pint", 0.568, false, true, 6000);
		CreateLiquidContainer("LContainer_Weizen", "A liquid container for a weizen glass (european 500ml glass)", 0.5, false, true, 7000);
		CreateLiquidContainer("LContainer_Stein", "A liquid container for a stein glass (european 1000ml glass)", 1.0, false, true, 14000);
		CreateLiquidContainer("LContainer_Yard", "A liquid container for a yard glass (2.5 imperial pints)", 1.4, false, true, 7000);
		CreateLiquidContainer("LContainer_Jug", "A liquid container for a glass jug, generally 40oz", 1.14, false, true, 7000);
		CreateLiquidContainer("LContainer_Flute", "A liquid container for a flute (champagne glass)", 0.180, false, true, 2000);
		CreateLiquidContainer("LContainer_Liqueur", "A liquid container for a small liqueur glass", 0.06, false, true, 2000);
		CreateLiquidContainer("LContainer_SherryGlass", "A liquid container for a copita, or a glass for drinking sherry", 0.180, false, true, 2000);
		CreateLiquidContainer("LContainer_SmallWineGlass", "A liquid container for a small wine glass, such as would typically be used for a white wine", 0.240, false, true, 2000);
		CreateLiquidContainer("LContainer_WineGlass", "A liquid container for a standard sized wine glass, such as would be typically used for red wine", 0.415, false, true, 4000);
		CreateLiquidContainer("LContainer_BeerBottle", "A liquid container for a single-use beer bottle (can't be re-sealed)", 0.375, true, true, 6000, true);
		CreateLiquidContainer("LContainer_SodaCan", "A liquid container for a single-use soda can (can't be re-sealed and isn't transparent)", 0.375, true, false, 6000, true);
		CreateLiquidContainer("LContainer_WineBottle", "A liquid container for a single-use wine bottle (can't be re-corked)", 0.75, true, true, 6000, true);
		CreateLiquidContainer("LContainer_Decanter", "A liquid container for a decanter for a bottle of wine", 0.75, true, true, 6000);
		CreateLiquidContainer("LContainer_Flask", "A liquid container for a typical hip flask", 0.236, true, false, 6000);
		CreateLiquidContainer("LContainer_Canteen", "A liquid container for a typical canteen", 1.0, true, false, 10000);
		CreateLiquidContainer("LContainer_Cask", "A liquid container for a typical wine cask ", 0.236, true, false, 6000);
		CreateLiquidContainer("LContainer_Tun", "A liquid container for a tun (252 US Gallon Barrel)", 960, true, false, 9600000);
		CreateLiquidContainer("LContainer_Butt", "A liquid container for a butt (126 US Gallon Barrel)", 480, true, false, 4800000);
		CreateLiquidContainer("LContainer_Puncheon", "A liquid container for a puncheon (84 US Gallon Barrel)", 320, true, false, 3200000);
		CreateLiquidContainer("LContainer_Hogshead", "A liquid container for a hogshead (63 US Gallon Barrel)", 240, true, false, 2400000);
		CreateLiquidContainer("LContainer_Tierce", "A liquid container for a tierce (42 US Gallon Barrel)", 160, true, false, 1600000);
		CreateLiquidContainer("LContainer_Barrel", "A liquid container for an English barrel (31.5 US Gallon Barrel)", 120, true, false, 1200000);
		CreateLiquidContainer("LContainer_Rundlet", "A liquid container for a rundlet (18 US Gallon Barrel)", 69, true, false, 690000);
		CreateLiquidContainer("LContainer_GallonCask", "A liquid container for a non-see through gallon-sized cask", 3.7, true, false, 37000);
		CreateLiquidContainer("LContainer_GallonBottle", "A liquid container for a gallon bottle like a milk bottle", 3.7, true, true, 37000);
		CreateLiquidContainer("LContainer_HalfGallonBottle", "A liquid container for a half gallon bottle like a milk bottle", 1.85, true, true, 18500);
		CreateLiquidContainer("LContainer_QuartBottle", "A liquid container for a one quart bottle like a milk bottle", 0.946, true, true, 18500);
		CreateLiquidContainer("LContainer_PintBottle", "A liquid container for a one pint bottle like a milk bottle", 0.473, true, true, 47300);
		CreateLiquidContainer("LContainer_20ozBottle", "A liquid container for a 20oz bottle", 0.591, true, true, 59100);
		CreateLiquidContainer("LContainer_40ozBottle", "A liquid container for a 40oz bottle", 1.182, true, true, 118200);
		CreateLiquidContainer("LContainer_QuartCarton", "A liquid container for a one quart carton", 0.946, true, false, 94600);
		CreateLiquidContainer("LContainer_PintCarton", "A liquid container for a one pint carton", 0.471, true, false, 47100);
		CreateLiquidContainer("LContainer_HalfPintCarton", "A liquid container for a one quart carton", 0.237, true, false, 23700);
		CreateLiquidContainer("LContainer_Waterskin", "A liquid container for a standard sized waterskin", 1.892, true, false, 189200);
		CreateLiquidContainer("LContainer_FuelCan", "A liquid container for a standard small sized fuel can (8L/2gal)", 8, true, false, 8000);
		CreateLiquidContainer("LContainer_JerryCan", "A liquid container for a standard sized jerry can (20L/5.4gal)", 20, true, false, 20000);
		CreateLiquidContainer("LContainer_Drum", "A liquid container for a standard 200L / 55gal drum", 200, true, false, 200000);
		CreateLiquidContainer("LContainer_Amphora_Sextarius", "A liquid container for an amphora in the roman sextarius (~0.96 pint)", 0.546, true, false, 5460);
		CreateLiquidContainer("LContainer_Amphora_Congius", "A liquid container for an amphora in the roman congius (~0.72 gallon)", 3.27, true, false, 32700);
		CreateLiquidContainer("LContainer_Amphora_Urna", "A liquid container for an amphora in the roman urna (~2.88 gallon)", 13.1, true, false, 131000);
		CreateLiquidContainer("LContainer_Amphora_Quadrantal", "A liquid container for an amphora in the roman quadrantal (~5.76 gallon)", 26.2, true, false, 262000);
		CreateLiquidContainer("LContainer_Amphora_Culeus", "A liquid container for an amphora in the roman culeus (~115 gallon)", 524, true, false, 524000);

		context.SaveChanges();
		#endregion

		#region Doors

		TraitDefinition? doorTrait =
			context.TraitDefinitions.FirstOrDefault(x => x.Name == "Constructing" || x.Name == "Construction") ??
			context.TraitDefinitions.FirstOrDefault(x => x.Name == "Labouring" || x.Name == "Labourer") ??
			context.TraitDefinitions.FirstOrDefault(x => x.Name == "Carpentry" || x.Name == "Carpenter");
		if (doorTrait == null)
		{
			var example =
				context.TraitDefinitions.FirstOrDefault(x => x.Type == 0 && x.TraitGroup != "Language");
			if (example != null)
			{
				var expression = new TraitExpression
				{
					Name = $"Construction Cap",
					Expression = example.Expression.Expression
				};
				context.TraitExpressions.Add(expression);
				doorTrait = new TraitDefinition
				{
					Name = "Construction",
					Type = 0,
					DecoratorId = context.TraitDecorators.First(x => x.Name == "Crafting Skill").Id,
					TraitGroup = "Combat",
					AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
					TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
					LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
					TeachDifficulty = 7,
					LearnDifficulty = 7,
					Hidden = false,
					Expression = expression,
					ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
					DerivedType = 0,
					ChargenBlurb = string.Empty,
					BranchMultiplier = 1.0
				};
				context.TraitDefinitions.Add(doorTrait);
				context.SaveChanges();
			}
		}


		if (doorTrait != null)
		{
			GameItemComponentProto CreateDoor(string name, string description, bool seeThrough, bool canFireThrough, bool canUninstall, Difficulty uninstallHinge, Difficulty uninstallNotHinge, bool canSmash, Difficulty smashDifficulty, string exitDescription)
			{
				return CreateItemProto(nextId++, now, "Door", name, description,
						$"<Definition SeeThrough=\"{seeThrough}\" CanFireThrough=\"{canFireThrough}\"><InstalledExitDescription>{exitDescription}</InstalledExitDescription><Uninstall CanPlayersUninstall=\"{canUninstall}\" UninstallDifficultyHingeSide=\"{(int)uninstallHinge}\" UninstallDifficultyNotHingeSide=\"{(int)uninstallNotHinge}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"{canSmash}\" SmashDifficulty=\"{(int)smashDifficulty}\" /></Definition>");
			}

			CreateDoor("Door_Normal", "This is an ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "door");
			CreateDoor("Door_Tough", "This is a tough door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "door");
			CreateDoor("Door_Secure", "This is a door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "door");
			CreateDoor("Door_Admin", "This is a door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door");
			CreateDoor("Door_Bad", "This is a bad door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door");
			CreateDoor("Gate_Normal", "This is an ordinary gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gate");
			CreateDoor("Gate_Tough", "This is a tough gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gate");
			CreateDoor("Gate_Secure", "This is a tough gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gate");
			CreateDoor("Gate_Admin", "This is a gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gate");
			CreateDoor("Gate_Bad", "This is a bad gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gate");
			CreateDoor("Door_Glass", "This is a door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door");
			CreateDoor("Door_Glass_Secure", "This is a door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "door");
			CreateDoor("Door_Glass_Admin", "This is a door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door");

			context.SaveChanges();

			GameItemComponentProto CreateLockableDoor(
				string name,
				string description,
				bool seeThrough,
				bool canFireThrough,
				bool canUninstall,
				Difficulty uninstallHinge,
				Difficulty uninstallNotHinge,
				bool canSmash,
				Difficulty smashDifficulty,
				string exitDescription,
				Difficulty force,
				Difficulty pick,
				string lockType
				)
			{
				return CreateItemProto(nextId++, now, "LockingDoor", name, description,
					$@"<Definition SeeThrough=""{seeThrough}"" CanFireThrough=""{canFireThrough}"">
  <ForceDifficulty>{(int)force}</ForceDifficulty>
  <PickDifficulty>{(int)pick}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[@ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[@ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0 is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0 is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>{lockType}</LockType>
  <InstalledExitDescription>{exitDescription}</InstalledExitDescription><Uninstall CanPlayersUninstall=""{canUninstall}"" UninstallDifficultyHingeSide=""{(int)uninstallHinge}"" UninstallDifficultyNotHingeSide=""{(int)uninstallNotHinge}"" UninstallTrait=""{doorTrait.Id}"" /><Smash CanPlayersSmash=""{canSmash}"" SmashDifficulty=""{(int)smashDifficulty}"" /></Definition>");
			}

			CreateLockableDoor("Door_Lockable_Normal", "This is an ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Tough", "This is a tough door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Secure", "This is a door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Admin", "This is a door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Bad", "This is a bad door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock");
			CreateLockableDoor("Gate_Lockable_Normal", "This is an ordinary gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Gate_Lockable_Tough", "This is a tough gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Gate_Lockable_Secure", "This is a tough gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock");
			CreateLockableDoor("Gate_Lockable_Admin", "This is a gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Gate_Lockable_Bad", "This is a bad gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Glass", "This is a door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Glass_Secure", "This is a door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock");
			CreateLockableDoor("Door_Lockable_Glass_Admin", "This is a door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock");
		}
		else
		{
			errors.Add("There was no valid trait supplied for door installation so no door components were created.");
		}

		#endregion

		#region Locks

		GameItemComponentProto CreateWardedLock(string name, string description, Difficulty force, Difficulty pick)
		{
			return CreateItemProto(nextId++, now, "Simple Lock", name, description,
					$@"<Definition>
  <ForceDifficulty>{(int)force}</ForceDifficulty>
  <PickDifficulty>{(int)pick}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
</Definition>");
		}

		GameItemComponentProto CreateLatch(string name, string description, Difficulty force, Difficulty pick)
		{
			return CreateItemProto(nextId++, now, "Latch", name, description,
					$@"<Definition>
  <ForceDifficulty>{(int)force}</ForceDifficulty>
  <PickDifficulty>{(int)pick}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
</Definition>");
		}

		GameItemComponentProto CreateSimpleKey(string name, string description, string lockType)
		{
			return CreateItemProto(nextId++, now, "Simple Key", name, description,
					@$"<Definition>
  <LockType>{lockType}</LockType>
</Definition>");
		}

		CreateWardedLock("Warded_Lock_Terrible", "This is a terrible simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Normal, Difficulty.VeryEasy);
		CreateWardedLock("Warded_Lock_Bad", "This is a bad simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Easy, Difficulty.Easy);
		CreateWardedLock("Warded_Lock_Normal", "This is a normal simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Hard, Difficulty.Normal);
		CreateWardedLock("Warded_Lock_Good", "This is a good simple lock in the 'warded' type (most pre-modern systems)", Difficulty.VeryHard, Difficulty.Hard);
		CreateWardedLock("Warded_Lock_Excellent", "This is an excellent simple lock in the 'warded' type (most pre-modern systems)", Difficulty.ExtremelyHard, Difficulty.VeryHard);
		CreateWardedLock("Warded_Lock_Master", "This is a masterful simple lock in the 'warded' type (most pre-modern systems)", Difficulty.ExtremelyHard, Difficulty.ExtremelyHard);
		CreateWardedLock("Warded_Lock_Legendary", "This is a legendary simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Insane, Difficulty.Insane);
		CreateSimpleKey("Warded_Key", "This is a key for locks in the 'warded' type (most pre-modern systems)", "Warded Lock");
		CreateLatch("Latch_Terrible", "This is a terrible quality simple latch (one-sided lock)", Difficulty.ExtremelyEasy, Difficulty.Easy);
		CreateLatch("Latch_Bad", "This is a bad quality simple latch (one-sided lock)", Difficulty.VeryEasy, Difficulty.Normal);
		CreateLatch("Latch_Normal", "This is a normal quality simple latch (one-sided lock)", Difficulty.Easy, Difficulty.Hard);
		CreateLatch("Latch_Good", "This is a good quality simple latch (one-sided lock)", Difficulty.Hard, Difficulty.Hard);
		CreateLatch("Latch_Excellent", "This is an excellent quality simple latch (one-sided lock)", Difficulty.VeryHard, Difficulty.VeryHard);
		CreateLatch("Latch_Master", "This is a masterful quality simple latch (one-sided lock)", Difficulty.ExtremelyHard, Difficulty.VeryHard);
		CreateLatch("Latch_Legendary", "This is a legendary quality simple latch (one-sided lock)", Difficulty.Insane, Difficulty.ExtremelyHard);
		CreateLatch("Latch_Admin", "This is a simple latch (one-sided lock) that cannot be picked or forced", Difficulty.Impossible, Difficulty.Impossible);

		context.SaveChanges();

		#endregion
		#region Writing Implements

		var holdable = context.GameItemComponentProtos.First(x => x.Type == "Holdable");
		var stack = context.GameItemComponentProtos.First(x => x.Name == "Stack_Number");
		var paperMaterial = context.Materials.First(x => x.Name == "Paper");

		GameItemComponentProto CreatePaperSheet(string name, string description, int maxCharacters)
		{
			return CreateItemProto(nextId++, now, "PaperSheet", name, description,
					@$"<Definition>
   <MaximumCharacterLengthOfText>{maxCharacters}</MaximumCharacterLengthOfText>
 </Definition>");
		}

		GameItemComponentProto CreateBiro(string name, string description, long colourId, int totalUses)
		{
			return CreateItemProto(nextId++, now, "Biro", name, description,
					@$"<Definition>
   <Colour>{colourId}</Colour>
   <TotalUses>{totalUses}</TotalUses>
 </Definition>");
		}

		GameItemComponentProto CreatePencil(string name, string description, long colourId, int usesBeforeSharpening, int totalUses)
		{
			return CreateItemProto(nextId++, now, "Pencil", name, description,
					@$"<Definition>
   <Colour>{colourId}</Colour>
   <UsesBeforeSharpening>{usesBeforeSharpening}</UsesBeforeSharpening>
   <TotalUses>{totalUses}</TotalUses>
 </Definition>");
		}

		GameItemComponentProto CreateBook(string name, string description, int pages, GameItemProto page)
		{
			var component = new GameItemComponentProto
			{
				Id = nextId++,
				RevisionNumber = 0,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = 4,
					BuilderAccountId = dbaccount.Id,
					BuilderDate = now,
					BuilderComment = "Auto-generated by the system",
					ReviewerAccountId = dbaccount.Id,
					ReviewerComment = "Auto-generated by the system",
					ReviewerDate = now
				},
				Type = "Book",
				Name = name,
				Description = description,
				Definition =
					$@"<Definition>
   <PaperProto>{page.Id}</PaperProto>
   <PageCount>{pages}</PageCount>
 </Definition>"
			};
			AddGameItemComponent(context, component);
			return component;
		}

		var paperA4 = CreatePaperSheet("Paper_A4", "This is a sheet of paper in A4 size (~ US Letter size)", 4160);

		var nextItemId = context.GameItemProtos.Max(x => x.Id) + 1;

		var a4paper = new GameItemProto
		{
			Id = nextItemId++,
			RevisionNumber = 0,
			Name = "sheet",
			Keywords = "sheet paper",
			MaterialId = paperMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = (int)SizeCategory.Tiny,
			Weight = 1,
			ReadOnly = false,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a sheet of paper",
			FullDescription = "This is a sheet of plain, unlined paper approximately 8 inches by 12 inches in size."
		};
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = holdable });
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = stack });
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = paperA4 });
		context.GameItemProtos.Add(a4paper);
		context.SaveChanges();

		var paperA3 = CreatePaperSheet("Paper_A3", "This is a sheet of paper in A3 size (~ US Ledger size)", 8320);

		var a3paper = new GameItemProto
		{
			Id = nextItemId++,
			RevisionNumber = 0,
			Name = "sheet",
			Keywords = "large sheet paper",
			MaterialId = paperMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = (int)SizeCategory.VerySmall,
			Weight = 2,
			ReadOnly = false,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a large sheet of paper",
			FullDescription = "This is a large sheet of plain, unlined paper approximately 12 inches by 16 inches in size."
		};
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = holdable });
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = stack });
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = paperA3 });
		context.GameItemProtos.Add(a3paper);
		context.SaveChanges();

		var paperA5 = CreatePaperSheet("Paper_A5", "This is a sheet of paper in A5 size (~ US Half Letter size)", 8320);

		var a5paper = new GameItemProto
		{
			Id = nextItemId++,
			RevisionNumber = 0,
			Name = "sheet",
			Keywords = "small sheet paper",
			MaterialId = paperMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = (int)SizeCategory.VerySmall,
			Weight = 0.5,
			ReadOnly = false,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a small sheet of paper",
			FullDescription = "This is a small sheet of plain, unlined paper approximately 5 inches by 8 inches in size."
		};
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = holdable });
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = stack });
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = paperA5 });
		context.GameItemProtos.Add(a5paper);
		context.SaveChanges();

		CreateBook("Book_20_Page", "This is a 20 page book of A4 pages", 20, a4paper);
		CreateBook("Book_40_Page", "This is a 40 page book of A4 pages", 40, a4paper);
		CreateBook("Book_90_Page", "This is a 90 page book of A4 pages", 90, a4paper);
		CreateBook("Book_200_Page", "This is a 200 page book of A4 pages", 200, a4paper);
		CreateBook("Book_500_Page", "This is a 500 page book of A4 pages", 500, a4paper);
		CreateBook("Book_1000_Page", "This is a 1000 page book of A4 pages", 1000, a4paper);
		CreateBook("Book_Small_20_Page", "This is a 20 page book of A5 pages", 20, a5paper);
		CreateBook("Book_Small_40_Page", "This is a 40 page book of A5 pages", 40, a5paper);
		CreateBook("Book_Small_90_Page", "This is a 90 page book of A5 pages", 90, a5paper);
		CreateBook("Book_Small_200_Page", "This is a 200 page book of A5 pages", 200, a5paper);
		CreateBook("Book_Small_500_Page", "This is a 500 page book of A5 pages", 500, a5paper);
		CreateBook("Book_Small_1000_Page", "This is a 1000 page book of A5 pages", 1000, a5paper);
		CreateBook("Book_Large_20_Page", "This is a 20 page book of A3 pages", 20, a3paper);
		CreateBook("Book_Large_40_Page", "This is a 40 page book of A3 pages", 40, a3paper);
		CreateBook("Book_Large_90_Page", "This is a 90 page book of A3 pages", 90, a3paper);
		CreateBook("Book_Large_200_Page", "This is a 200 page book of A3 pages", 200, a3paper);
		CreateBook("Book_Large_500_Page", "This is a 500 page book of A3 pages", 500, a3paper);
		CreateBook("Book_Large_1000_Page", "This is a 1000 page book of A3 pages", 1000, a3paper);

		CreateBiro("Biro_Black", "This is a standard black biro pen", context.Colours.First(x => x.Name == "black").Id, 110000);
		CreateBiro("Biro_Blue", "This is a standard blue biro pen", context.Colours.First(x => x.Name == "blue").Id, 110000);
		CreateBiro("Biro_Red", "This is a standard red biro pen", context.Colours.First(x => x.Name == "red").Id, 110000);
		CreatePencil("Pencil_Black", "This is a standard black pencil", context.Colours.First(x => x.Name == "black").Id, 11000, 220000);

		context.SaveChanges();

		#endregion

	}

	private void SeedItemsPart2(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
		ICollection<string> errors)
	{
		var now = DateTime.UtcNow;
		var dbaccount = context.Accounts.First();
		var nextId = context.GameItemComponentProtos.Max(x => x.Id) + 1;

		#region Insulation

		var component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "ClothingInsulation",
			Name = "Insulation_Minor",
			Description = "Makes garment a minor insulator",
			Definition = @"<Definition>
	<InsulatingDegrees>0.5</InsulatingDegrees>
	<ReflectingDegrees>0.5</ReflectingDegrees>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "ClothingInsulation",
			Name = "Insulation_Moderate",
			Description = "Makes garment a moderate insulator",
			Definition = @"<Definition>
	<InsulatingDegrees>1.0</InsulatingDegrees>
	<ReflectingDegrees>0.5</ReflectingDegrees>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "ClothingInsulation",
			Name = "Insulation_Strong",
			Description = "Makes garment a strong insulator",
			Definition = @"<Definition>
	<InsulatingDegrees>2.0</InsulatingDegrees>
	<ReflectingDegrees>0.5</ReflectingDegrees>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "ClothingInsulation",
			Name = "Insulation_Super",
			Description = "Makes garment a super insulator",
			Definition = @"<Definition>
	<InsulatingDegrees>4.0</InsulatingDegrees>
	<ReflectingDegrees>0.5</ReflectingDegrees>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "ClothingInsulation",
			Name = "Insulation_Reflector",
			Description = "Makes garment a good reflector (though not insulator)",
			Definition = @"<Definition>
	<InsulatingDegrees>0.0</InsulatingDegrees>
	<ReflectingDegrees>2.0</ReflectingDegrees>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Glasses

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Glasses",
			Name = "Glasses",
			Description = "Makes item correct Myopia flaws",
			Definition = @"<Definition></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region IdentityObscuring

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "IdentityObscurer",
			Name = "FullFaceObscurer",
			Description = "Makes garment hide wearer's identity and change descriptions, including whole face",
			Definition = @"<Definition>
	<RemovalEcho><![CDATA[revealing &his $eyeshape $eyecolour eyes, $nose,$?facialhairstyle[ $facialhaircolour $facialhairstyle,][] and $haircolour $hairstyle.]]></RemovalEcho>
	<ShortDescription><![CDATA[&a_an[$?height[$height[@, ]]$frame] @key figure]]></ShortDescription>
	<FullDescription><![CDATA[This individual has their identity obscured on account of the fact that they are wearing @item. You can still tell however that they are &height tall and $framefancy.]]></FullDescription>
	<Difficulty>9</Difficulty>
	<Keywords>
		<Keyword key=""hood"" value=""hooded""/>
		<Keyword key=""helm"" value=""helmed""/>
		<Keyword key=""helmet"" value=""helmeted""/>
		<Keyword key=""mask"" value=""masked""/>
		<Keyword key=""scarf"" value=""scarfed""/>
		<Keyword key=""veil"" value=""veiled""/>
		<Keyword key=""hijab"" value=""hijabed""/>
		<Keyword key=""niqab"" value=""niqabed""/>
		<Keyword key=""burka"" value=""burkad""/>
		<Keyword key=""balaclava"" value=""balaclavad""/>
	</Keywords>
	<Characteristics>
	</Characteristics>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "IdentityObscurer",
			Name = "EyesFreeObscurer",
			Description = "Makes garment hide wearer's identity and change descriptions, but not hiding eyes",
			Definition = @"<Definition>
	<RemovalEcho><![CDATA[revealing &his $nose,$?facialhairstyle[ $facialhaircolour $facialhairstyle,][] and $haircolour $hairstyle.]]></RemovalEcho>
	<ShortDescription><![CDATA[&a_an[$?height[$height[@, ]]$frame] $eyecolour-eyed @key figure]]></ShortDescription>
	<FullDescription><![CDATA[This individual has their identity obscured on account of the fact that they are wearing @item. You can still tell however that they are &height tall and $framefancy, and that they have $eyeshape, $eyecolour eyes.]]></FullDescription>
	<Difficulty>9</Difficulty>
	<Keywords>
		<Keyword key=""hood"" value=""hooded""/>
		<Keyword key=""helm"" value=""helmed""/>
		<Keyword key=""helmet"" value=""helmeted""/>
		<Keyword key=""mask"" value=""masked""/>
		<Keyword key=""scarf"" value=""scarfed""/>
		<Keyword key=""veil"" value=""veiled""/>
		<Keyword key=""hijab"" value=""hijabed""/>
		<Keyword key=""niqab"" value=""niqabed""/>
		<Keyword key=""burka"" value=""burkad""/>
		<Keyword key=""balaclava"" value=""balaclavad""/>
	</Keywords>
	<Characteristics>
	</Characteristics>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable Changer",
			Name = "Wig",
			Description = "Changes hair colour and style when worn",
			Definition = @$"<Definition TargetWearProfile="""">
	<Characteristic Profile=""{context.CharacteristicProfiles.First(x => x.Name == "All Hair Colours").Id}"" Value=""{context.CharacteristicDefinitions.First(x => x.Name == "Hair Colour").Id}""/>
	<Characteristic Profile=""{context.CharacteristicProfiles.First(x => x.Name == "All Hair Styles").Id}"" Value=""{context.CharacteristicDefinitions.First(x => x.Name == "Hair Style").Id}""/>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable Changer",
			Name = "Facial_Wig",
			Description = "Changes facial hair colour and style when worn",
			Definition = @$"<Definition TargetWearProfile="""">
	<Characteristic Profile=""{context.CharacteristicProfiles.First(x => x.Name == "All Facial Hair Colours").Id}"" Value=""{context.CharacteristicDefinitions.First(x => x.Name == "Facial Hair Colour").Id}""/>
	<Characteristic Profile=""{context.CharacteristicProfiles.First(x => x.Name == "All Facial Hair Styles").Id}"" Value=""{context.CharacteristicDefinitions.First(x => x.Name == "Facial Hair Style").Id}""/>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable Changer",
			Name = "ColouredContacts",
			Description = "Changes eye colour when worn",
			Definition = @$"<Definition TargetWearProfile="""">
	<Characteristic Profile=""{context.CharacteristicProfiles.First(x => x.Name == "All Eye Colours").Id}"" Value=""{context.CharacteristicDefinitions.First(x => x.Name == "Eye Colour").Id}""/>
</Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion
	}

	private void SeedItemsPart3(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
	{
		var now = DateTime.UtcNow;
		var dbaccount = context.Accounts.First();
		var nextId = context.GameItemComponentProtos.Max(x => x.Id) + 1;

		#region Destroyables

		var component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Misc",
			Description = "Makes an item destroyable. Relatively low HP, for miscellaneous items",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(pow(size,1.2) * (quality+1))]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.25"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0.25"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Weapon",
			Description = "Makes an item destroyable. Designed for Weapons.",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(20 * quality) + (pow(size,1.2) * (quality+1))]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.1"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0.25"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Armour",
			Description = "Makes an item destroyable. Designed for Armour.",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(100 * quality) + (pow(size,1.2) * (quality+1) * 10)]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.1"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0.1"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Clothing",
			Description = "Makes an item destroyable. Designed for Clothing.",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(2 * quality) + (pow(size,1.2) * (quality+1))]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.01"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Door",
			Description = "Makes an item destroyable. Designed for Doors.",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(20 * quality) + (pow(size,1.2) * (quality+1) * 5)]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.1"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0.25"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Destroyable",
			Name = "Destroyable_Furniture",
			Description = "Makes an item destroyable. Designed for Furniture.",
			Definition = @"<Definition>
   <HpExpression><![CDATA[(30 * quality) + (pow(size,1.2) * (quality+1) * 5)]]></HpExpression>
   <DamageMultipliers>
	 <DamageMultiplier type=""0"" multiplier=""0.25"" />
	 <DamageMultiplier type=""1"" multiplier=""0.25"" />
	 <DamageMultiplier type=""2"" multiplier=""0.1"" />
	 <DamageMultiplier type=""3"" multiplier=""0.05"" />
	 <DamageMultiplier type=""4"" multiplier=""0.05"" />
	 <DamageMultiplier type=""5"" multiplier=""0"" />
	 <DamageMultiplier type=""6"" multiplier=""0"" />
	 <DamageMultiplier type=""7"" multiplier=""0.25"" />
	 <DamageMultiplier type=""8"" multiplier=""0.25"" />
	 <DamageMultiplier type=""9"" multiplier=""0.05"" />
	 <DamageMultiplier type=""10"" multiplier=""0.25"" />
	 <DamageMultiplier type=""11"" multiplier=""0"" />
	 <DamageMultiplier type=""12"" multiplier=""0"" />
	 <DamageMultiplier type=""13"" multiplier=""0"" />
	 <DamageMultiplier type=""14"" multiplier=""0.25"" />
	 <DamageMultiplier type=""15"" multiplier=""0.25"" />
	 <DamageMultiplier type=""16"" multiplier=""0.05"" />
	 <DamageMultiplier type=""17"" multiplier=""0.10"" />
	 <DamageMultiplier type=""18"" multiplier=""0.10"" />
	 <DamageMultiplier type=""19"" multiplier=""0.00"" />
	 <DamageMultiplier type=""20"" multiplier=""0.05"" />
	 <DamageMultiplier type=""21"" multiplier=""0.25"" />
	 <DamageMultiplier type=""22"" multiplier=""0.25"" />
   </DamageMultipliers>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Variables

		var colour = context.CharacteristicDefinitions.First(x => x.Name == "Colour");
		var colour1 = context.CharacteristicDefinitions.First(x => x.Name == "Colour1");
		var colour2 = context.CharacteristicDefinitions.First(x => x.Name == "Colour2");
		var colour3 = context.CharacteristicDefinitions.First(x => x.Name == "Colour3");
		var allColours = context.CharacteristicProfiles.First(x => x.Name == "All_Colours");
		var basicColours = context.CharacteristicProfiles.First(x => x.Name == "Basic_Colours");
		var fineColours = context.CharacteristicProfiles.First(x => x.Name == "Fine_Colours");
		var drabColours = context.CharacteristicProfiles.First(x => x.Name == "Drab_Colours");

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_Colour",
			Description = "Gives an item a random variable called $colour.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_BasicColour",
			Description = "Gives an item a random variable called $colour from the Basic_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_FineColour",
			Description = "Gives an item a random variable called $colour from the Fine_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_DrabColour",
			Description = "Gives an item a random variable called $colour from the Drab_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_2Colour",
			Description = "Gives an item 2 random variables called $colour1 and $colour2.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour2.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_2BasicColour",
			Description = "Gives an item 2 random variables called $colour1 and $colour2 from the Basic_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour2.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_2FineColour",
			Description = "Gives an item 2 random variables called $colour1 and $colour2 from the Fine_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour2.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_2DrabColour",
			Description = "Gives an item 2 random variables called $colour1 and $colour2 from the Drab_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour2.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_3Colour",
			Description = "Gives an item 3 random variables called $colour1, $colour2, and $colour3",
			Definition = @$"<Definition>
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour2.Id}"" />
   <Characteristic Profile=""{allColours.Id}"" Value=""{colour3.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_3BasicColour",
			Description =
				"Gives an item 3 random variables called $colour1, $colour2, and $colour3 from the Basic_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour2.Id}"" />
   <Characteristic Profile=""{basicColours.Id}"" Value=""{colour3.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_3FineColour",
			Description =
				"Gives an item 3 random variables called $colour1, $colour2, and $colour3 from the Fine_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour2.Id}"" />
   <Characteristic Profile=""{fineColours.Id}"" Value=""{colour3.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Variable",
			Name = "Variable_3DrabColour",
			Description =
				"Gives an item 3 random variables called $colour1, $colour2, and $colour3 from the Drab_Colours list.",
			Definition = @$"<Definition>
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour1.Id}"" />
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour2.Id}"" />
   <Characteristic Profile=""{drabColours.Id}"" Value=""{colour3.Id}"" />
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Writing Implements

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Pencil",
			Name = "Variable_Pencil",
			Description = "Makes an item a pencil of variable colour. Use with VARIABLE_COLOUR component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>5000</TotalUses>
   <UsesBeforeSharpening>100</UsesBeforeSharpening>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Pencil",
			Name = "Variable_Pencil_Colour1",
			Description = "Makes an item a pencil of variable colour. Use with VARIABLE_COLOUR1 component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>5000</TotalUses>
   <UsesBeforeSharpening>100</UsesBeforeSharpening>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour1").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Crayon",
			Name = "Variable_Crayon",
			Description = "Makes an item a crayon of variable colour. Use with VARIABLE_COLOUR component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>1000</TotalUses>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Crayon",
			Name = "Variable_Crayon_Colour1",
			Description = "Makes an item a crayon of variable colour. Use with VARIABLE_COLOUR1 component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>1000</TotalUses>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour1").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Biro",
			Name = "Variable_Biro",
			Description = "Makes an item a biro of variable colour. Use with VARIABLE_COLOUR component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>110000</TotalUses>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Biro",
			Name = "Variable_Biro_Colour1",
			Description = "Makes an item a biro of variable colour. Use with VARIABLE_COLOUR1 component.",
			Definition = @$"<Definition>
   <Colour>0</Colour>
   <TotalUses>110000</TotalUses>
   <ColourCharacteristic>{context.CharacteristicDefinitions.First(x => x.Name == "Colour1").Id}</ColourCharacteristic>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Chairs

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Chair",
			Name = "Chair_Single",
			Description = "Makes an item a chair that can hold a single occupant",
			Definition = @"<Definition ChairSlotsUsed=""1"" ChairOccupantCapacity=""1"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Chair",
			Name = "Chair_Double",
			Description = "Makes an item a chair that can hold two occupants",
			Definition = @"<Definition ChairSlotsUsed=""2"" ChairOccupantCapacity=""2"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Chair",
			Name = "Chair_Triple",
			Description = "Makes an item a chair that can hold three occupants",
			Definition = @"<Definition ChairSlotsUsed=""3"" ChairOccupantCapacity=""3"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Chair",
			Name = "Chair_Quad",
			Description = "Makes an item a chair that can hold four occupants",
			Definition = @"<Definition ChairSlotsUsed=""4"" ChairOccupantCapacity=""4"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Ranged Cover

		var unflippedTable = CreateOrGetRangedCover(context, "Upright Table", 1, 1, 1,
			"using $?0|$0|a table|$ as cover",
			"@ move|moves behind $?1|$1|a nearby table|$ and use|uses it to obscure &0's profile",
			3, false);
		var flippedTable = CreateOrGetRangedCover(context, "Overturned Table", 1, 2, 3,
			"hiding behind $?0|$0|an overturned table|$ as cover",
			"@ duck|ducks behind $?1|$1|a nearby overturned table|$ and begin|begins to use it as cover",
			3, false);
		CreateOrGetRangedCover(context, "Uneven Ground", 0, 1, 6,
			"prone, using the uneven ground as cover",
			"@ go|goes prone and begin|begins to use the uneven ground as cover",
			0, true);
		var smokeCover = CreateOrGetRangedCover(context, "Smoke", 0, 2, 1,
			"obscured by $?0|$0|the smoke|$",
			"@ move|moves into $?1|$1|the smoke|$ and uses it to obscure &0's form",
			0, true);
		var sandbagCover = CreateOrGetRangedCover(context, "Sandbag", 1, 2, 3,
			"hiding behind $?0|$0|a sandbag barricade|$, using it as cover",
			"@ take|takes position behind $?1|$1|a sandbag barricade|$ and begin|begins to use it as cover",
			3, false);
		var treeCover = CreateOrGetRangedCover(context, "Tree", 1, 2, 1,
			"hiding behind $?0|$0|a tree|$ for cover",
			"@ slip|slips behind $?1|$1|a tree|$ and use|uses it to protect &0's vital areas",
			1, false);
		var bushesCover = CreateOrGetRangedCover(context, "Bushes", 0, 2, 1,
			"hiding in $?0|$0|the bushes|$ for cover",
			"@ take|takes position behind $?1|$1|a bush|$ and use|uses it to obscure &0's profile",
			2, false);
		CreateOrGetRangedCover(context, "Long Grass", 0, 2, 6,
			"hiding in $?0|$0|the long grass|$ for cover",
			"@ take|takes position in $?1|$1|the long grass|$ and use|uses it to obscure &0's profile",
			2, true);
		var doorwaysCover = CreateOrGetRangedCover(context, "Doorways", 0, 1, 1,
			"using a doorway as cover",
			"@ duck|ducks into a doorway and begin|begins to use it as cover",
			0, false);
		var rubbleCover = CreateOrGetRangedCover(context, "Rubble", 1, 2, 12,
			"slumped up against $?0|$0|some rubble|$ as cover",
			"@ slump|slumps up against $?1|$1|some rubble|$ and begin|begins to use it as cover",
			2, false);

		context.SaveChanges();

		CreateItemProto(nextId++, now, "Cover", "Cover_Smoke",
			@"Turns an item into ranged cover of type ""Smoke"".",
			$@"<Definition>
   <Cover>{smokeCover.Id}</Cover>
 </Definition>");
		CreateItemProto(nextId++, now, "Cover", "Cover_Sandbag",
			@"Turns an item into ranged cover of type ""Sandbag"".",
			$@"<Definition>
   <Cover>{sandbagCover.Id}</Cover>
 </Definition>");
		CreateItemProto(nextId++, now, "Cover", "Cover_Tree",
			@"Turns an item into ranged cover of type ""Tree"".",
			$@"<Definition>
   <Cover>{treeCover.Id}</Cover>
 </Definition>");
		CreateItemProto(nextId++, now, "Cover", "Cover_Bushes",
			@"Turns an item into ranged cover of type ""Bushes"".",
			$@"<Definition>
   <Cover>{bushesCover.Id}</Cover>
 </Definition>");
		CreateItemProto(nextId++, now, "Cover", "Cover_Doorways",
			@"Turns an item into ranged cover of type ""Doorways"".",
			$@"<Definition>
   <Cover>{doorwaysCover.Id}</Cover>
 </Definition>");
		CreateItemProto(nextId++, now, "Cover", "Cover_Rubble",
			@"Turns an item into ranged cover of type ""Rubble"".",
			$@"<Definition>
   <Cover>{rubbleCover.Id}</Cover>
 </Definition>");
		context.SaveChanges();

		#endregion
		#region Tables

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Four",
			Description = "Makes an item a table that can take four occupants",
			Definition = @$"<Definition MaximumChairSlots=""4"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Six",
			Description = "Makes an item a table that can take six occupants",
			Definition = @$"<Definition MaximumChairSlots=""6"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Eight",
			Description = "Makes an item a table that can take eight occupants",
			Definition = @$"<Definition MaximumChairSlots=""8"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Ten",
			Description = "Makes an item a table that can take ten occupants",
			Definition = @$"<Definition MaximumChairSlots=""10"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Twelve",
			Description = "Makes an item a table that can take twelve occupants",
			Definition = @$"<Definition MaximumChairSlots=""12"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Table",
			Name = "Table_Twenty",
			Description = "Makes an item a table that can take twenty occupants",
			Definition = @$"<Definition MaximumChairSlots=""20"">
  <Cover>
	<Flipped>{flippedTable.Id}</Flipped>
	<NotFlipped>{unflippedTable.Id}</NotFlipped>
	<Expression><![CDATA[0]]></Expression>
	<Message><![CDATA[@ try|tries to flip $1, but are|is not strong enough.]]></Message>
  </Cover>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Worn Expansion

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Gag",
			Name = "Gag",
			Description = "Makes an item a gag when worn over the mouth",
			Definition = @"<Definition />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Blindfold",
			Name = "Blindfold",
			Description = "Makes an item a blindfold when worn over the eyes",
			Definition = @"<Definition />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Restraint",
			Name = "Restraint",
			Description =
				@"Turns an item into a restraint when combined with the wearables that are named ""Bound <X>""",
			Definition = @"<Definition>
   <MinimumCreatureSize>4</MinimumCreatureSize>
   <MaximumCreatureSize>8</MaximumCreatureSize>
   <BreakoutDifficulty>7</BreakoutDifficulty>
   <OverpowerDifficulty>7</OverpowerDifficulty>
   <LimbType>0</LimbType>
   <LimbType>1</LimbType>
   <LimbType>2</LimbType>
   <LimbType>3</LimbType>
   <LimbType>4</LimbType>
   <LimbType>5</LimbType>
   <LimbType>6</LimbType>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Restraint",
			Name = "Restraint_Impossible",
			Description =
				@"Turns an item into an impossible to escape restraint when combined with the wearables that are named ""Bound <X>""",
			Definition = @"<Definition>
   <MinimumCreatureSize>4</MinimumCreatureSize>
   <MaximumCreatureSize>8</MaximumCreatureSize>
   <BreakoutDifficulty>11</BreakoutDifficulty>
   <OverpowerDifficulty>11</OverpowerDifficulty>
   <LimbType>0</LimbType>
   <LimbType>1</LimbType>
   <LimbType>2</LimbType>
   <LimbType>3</LimbType>
   <LimbType>4</LimbType>
   <LimbType>5</LimbType>
   <LimbType>6</LimbType>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Belt",
			Name = "Belt_2",
			Description = "Turns item into a belt that can have 2 attached items of up to size Small",
			Definition = @"<Definition MaximumNumberOfBeltedItems=""2"" MaximumSize=""5""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Belt",
			Name = "Belt_4",
			Description = "Turns item into a belt that can have 4 attached items of up to size Small",
			Definition = @"<Definition MaximumNumberOfBeltedItems=""4"" MaximumSize=""5""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Belt",
			Name = "Belt_6",
			Description = "Turns item into a belt that can have 6 attached items of up to size Small",
			Definition = @"<Definition MaximumNumberOfBeltedItems=""6"" MaximumSize=""5""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Belt",
			Name = "Belt_Large",
			Description = "Turns item into a belt that can have 1 attached item of up to size Normal",
			Definition = @"<Definition MaximumNumberOfBeltedItems=""1"" MaximumSize=""6""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Beltable",
			Name = "Beltable",
			Description = "Allows an item to be attached to a belt",
			Definition = @"<Definition/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Sheath",
			Name = "Sheath",
			Description = "Turns item into a sheath for melee weapons of up to size Normal",
			Definition = @"<Definition StealthDrawDifficulty=""6"" MaximumSize=""6""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Sheath",
			Name = "Sheath_Small",
			Description = "Turns item into a sheath for melee weapons of up to size Small",
			Definition = @"<Definition StealthDrawDifficulty=""4"" MaximumSize=""5""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Sheath",
			Name = "Sheath_Large",
			Description = "Turns item into a sheath for melee weapons of up to size Large",
			Definition = @"<Definition StealthDrawDifficulty=""8"" MaximumSize=""7""/>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Health-Related Items

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Immobilising",
			Name = "Limb_Immobilising",
			Description = "Item can be used to immobilise a broken limb (e.g. splint)",
			Definition = @"<Definition />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Crutch",
			Name = "Crutch",
			Description = "Item can be used as a crutch to walk when limbs are injured",
			Definition = @"<Definition />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_10ml",
			Description = "Turns item into a syringe that holds a 10ml dose",
			Definition = @"<Definition LiquidCapacity=""0.01"" Transparent=""true"" WeightLimit=""100"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_5ml",
			Description = "Turns item into a syringe that holds a 5ml dose",
			Definition = @"<Definition LiquidCapacity=""0.005"" Transparent=""true"" WeightLimit=""50"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_2ml",
			Description = "Turns item into a syringe that holds a 2ml dose",
			Definition = @"<Definition LiquidCapacity=""0.002"" Transparent=""true"" WeightLimit=""20"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_1ml",
			Description = "Turns item into a syringe that holds a 1ml dose",
			Definition = @"<Definition LiquidCapacity=""0.001"" Transparent=""true"" WeightLimit=""10"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_0.5ml",
			Description = "Turns item into a syringe that holds a 0.5ml dose",
			Definition = @"<Definition LiquidCapacity=""0.0005"" Transparent=""true"" WeightLimit=""5"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Syringe",
			Name = "Syringe_0.25ml",
			Description = "Turns item into a syringe that holds a 0.25ml dose",
			Definition = @"<Definition LiquidCapacity=""0.00025"" Transparent=""true"" WeightLimit=""2.5"" />"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "IVBag",
			Name = "IV_250ml",
			Description = "A 250ml IV Bag, such as a blood donation bag",
			Definition = @"<Definition LiquidCapacity=""0.25"" Closable=""true"" Transparent=""true"">
   <Connectors>
	 <Connection gender=""2"" type=""cannula"" />
   </Connectors>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "IVBag",
			Name = "IV_500ml",
			Description = "A 500ml IV Bag, such as a typical drug delivery IV",
			Definition = @"<Definition LiquidCapacity=""0.5"" Closable=""true"" Transparent=""true"">
   <Connectors>
	 <Connection gender=""2"" type=""cannula"" />
   </Connectors>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "IVBag",
			Name = "IV_1000ml",
			Description = "A 1000ml IV Bag, such as a typical saline bag",
			Definition = @"<Definition LiquidCapacity=""1.0"" Closable=""true"" Transparent=""true"">
   <Connectors>
	 <Connection gender=""2"" type=""cannula"" />
   </Connectors>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Drip",
			Name = "Drip",
			Description = "Turns the item into a drip, used to connect an IV Bag to a Cannula",
			Definition = @"<Definition>
   <Connectors>
	 <Connection gender=""2"" type=""cannula"" />
	 <Connection gender=""3"" type=""cannula"" />
   </Connectors>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Cannula",
			Name = "Cannula",
			Description = "Turns the item into a peripheral venous cannula for humanoids",
			Definition = @"<Definition>
   <Connectors>
	 <Connection gender=""3"" type=""cannula"" />
   </Connectors>
   <TargetBody>1</TargetBody>
   <ExternalDescription><![CDATA[a peripheral venous cannula]]></ExternalDescription>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Cannula",
			Name = "Cannula_Arterial",
			Description = "Turns the item into an arterial cannula for humanoids",
			Definition = @"<Definition>
   <Connectors>
	 <Connection gender=""3"" type=""LargeArterialCatheter"" />
   </Connectors>
   <TargetBody>1</TargetBody>
   <ExternalDescription><![CDATA[a large arterial cannula]]></ExternalDescription>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Cannula",
			Name = "Cannula_Venous",
			Description = "Turns the item into a venous cannula for humanoids",
			Definition = @"<Definition>
   <Connectors>
	 <Connection gender=""3"" type=""LargeVenousCatheter"" />
   </Connectors>
   <TargetBody>1</TargetBody>
   <ExternalDescription><![CDATA[a large venous cannula]]></ExternalDescription>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Bandage_Simple",
			Description = "Turns the item into a consumable single-use bandage for stopping bleeding",
			Definition = @"<Definition>
   <MaximumUses>1</MaximumUses>
   <Refillable>false</Refillable>
   <DifficultyStages>1</DifficultyStages>
   <TreatmentType>2</TreatmentType>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Bandage_Good",
			Description = "Turns the item into a good quality consumable single-use bandage for stopping bleeding",
			Definition = @"<Definition>
   <MaximumUses>1</MaximumUses>
   <Refillable>false</Refillable>
   <DifficultyStages>3</DifficultyStages>
   <TreatmentType>2</TreatmentType>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Bandage_Great",
			Description = "Turns the item into a great quality consumable single-use bandage for stopping bleeding",
			Definition = @"<Definition>
   <MaximumUses>1</MaximumUses>
   <Refillable>false</Refillable>
   <DifficultyStages>4</DifficultyStages>
   <TreatmentType>2</TreatmentType>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Suture_Kit",
			Description = "Turns the item into a multi-use kit for suturing wounds",
			Definition =
				@"<Definition>  <MaximumUses>20</MaximumUses>  <Refillable>true</Refillable>  <DifficultyStages>1</DifficultyStages>  <TreatmentType>4</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Suture_Kit_Good",
			Description = "Turns the item into a good quality multi-use kit for suturing wounds",
			Definition =
				@"<Definition>  <MaximumUses>20</MaximumUses>  <Refillable>true</Refillable>  <DifficultyStages>3</DifficultyStages>  <TreatmentType>4</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Suture_Single",
			Description = "Turns the item into a single-use consumable for suturing wounds",
			Definition =
				@"<Definition>  <MaximumUses>1</MaximumUses>  <Refillable>false</Refillable>  <DifficultyStages>2</DifficultyStages>  <TreatmentType>4</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Clean_Kit",
			Description = "Turns the item into a multi-use kit for cleaning wounds",
			Definition =
				@"<Definition>  <MaximumUses>20</MaximumUses>  <Refillable>true</Refillable>  <DifficultyStages>1</DifficultyStages>  <TreatmentType>3</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Clean_Single",
			Description = "Turns the item into a single-use consumable for cleaning wounds",
			Definition =
				@"<Definition>  <MaximumUses>1</MaximumUses>  <Refillable>false</Refillable>  <DifficultyStages>2</DifficultyStages>  <TreatmentType>3</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Antiseptic_Kit",
			Description = "Turns the item into a multi-use kit for antiseptic cleaning wounds",
			Definition =
				@"<Definition>  <MaximumUses>20</MaximumUses>  <Refillable>true</Refillable>  <DifficultyStages>1</DifficultyStages>  <TreatmentType>1</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Antiseptic_Single",
			Description = "Turns the item into a single-use consumable for antiseptic cleaning wounds",
			Definition =
				@"<Definition>  <MaximumUses>1</MaximumUses>  <Refillable>false</Refillable>  <DifficultyStages>2</DifficultyStages>  <TreatmentType>1</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Tend_Kit",
			Description = "Turns the item into a multi-use kit for tending wounds",
			Definition =
				@"<Definition>  <MaximumUses>20</MaximumUses>  <Refillable>true</Refillable>  <DifficultyStages>1</DifficultyStages>  <TreatmentType>11</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Treatment",
			Name = "Tend_Single",
			Description = "Turns the item into a single-use consumable for tending wounds",
			Definition =
				@"<Definition>  <MaximumUses>1</MaximumUses>  <Refillable>false</Refillable>  <DifficultyStages>2</DifficultyStages>  <TreatmentType>11</TreatmentType></Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Prosthetics
		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RHand",
			Description = "Turns the item into an obvious, non-functional prosthetic replacement for the right hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rhand").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RWrist",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the right hand (from the wrist)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rwrist").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RForearm",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the right forearm and hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rforearm").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RElbow",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the right forearm and hand (from the elbow)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "relbow").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RUpperArm",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the right arm and hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rupperarm").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LHand",
			Description = "Turns the item into an obvious, non-functional prosthetic replacement for the left hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lhand").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LWrist",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the left hand (from the wrist)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lwrist").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LForearm",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the left forearm and hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lforearm").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LElbow",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the left forearm and hand (from the elbow)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lelbow").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LUpperArm",
			Description =
				"Turns the item into an obvious, non-functional prosthetic replacement for the left arm and hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lupperarm").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_REye",
			Description = "Turns the item into an obvious, non-functional prosthetic replacement for the right eye",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "reye").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LEye",
			Description = "Turns the item into an obvious, non-functional prosthetic replacement for the left eye",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>false</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "leye").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RHand_Functional",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the right hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rhand").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LHand_Functional",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the left hand",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lhand").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RFoot",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the right foot",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rfoot").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RAnkle",
			Description =
				"Turns the item into an obvious, functional prosthetic replacement for the right foot (from ankle)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rankle").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RLowerLeg",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the right lower leg",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rcalf").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RKnee",
			Description =
				"Turns the item into an obvious, functional prosthetic replacement for the right lower leg (from knee)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rknee").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_RThigh",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the right leg",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "rthigh").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LFoot",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the left foot",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lfoot").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LAnkle",
			Description =
				"Turns the item into an obvious, functional prosthetic replacement for the left foot (from ankle)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lankle").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LLowerLeg",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the left lower leg",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lcalf").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LKnee",
			Description =
				"Turns the item into an obvious, functional prosthetic replacement for the left lower leg (from knee)",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lknee").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Prosthetic",
			Name = "Prosthetic_LThigh",
			Description = "Turns the item into an obvious, functional prosthetic replacement for the left leg",
			Definition = @$"<Definition>
   <Obvious>true</Obvious>
   <Functional>true</Functional>
   <TargetBody>1</TargetBody>
   <TargetBodypart>{context.BodypartProtos.First(x => x.Name == "lthigh").Id}</TargetBodypart>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion

		#region Dice

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d4",
			Description = "Turns an item into a fair 4-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d6",
			Description = "Turns an item into a fair 6-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
	 <Face><![CDATA[5]]></Face>
	 <Face><![CDATA[6]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d8",
			Description = "Turns an item into a fair 8-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
	 <Face><![CDATA[5]]></Face>
	 <Face><![CDATA[6]]></Face>
	 <Face><![CDATA[7]]></Face>
	 <Face><![CDATA[8]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>6</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>7</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d10",
			Description = "Turns an item into a fair 10-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
	 <Face><![CDATA[5]]></Face>
	 <Face><![CDATA[6]]></Face>
	 <Face><![CDATA[7]]></Face>
	 <Face><![CDATA[8]]></Face>
	 <Face><![CDATA[9]]></Face>
	 <Face><![CDATA[10]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>6</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>7</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>8</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>9</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d00",
			Description = "Turns an item into a fair 10-sided die marked for tens. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[00]]></Face>
	 <Face><![CDATA[10]]></Face>
	 <Face><![CDATA[20]]></Face>
	 <Face><![CDATA[30]]></Face>
	 <Face><![CDATA[40]]></Face>
	 <Face><![CDATA[50]]></Face>
	 <Face><![CDATA[60]]></Face>
	 <Face><![CDATA[70]]></Face>
	 <Face><![CDATA[80]]></Face>
	 <Face><![CDATA[90]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>6</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>7</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>8</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>9</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d12",
			Description = "Turns an item into a fair 12-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
	 <Face><![CDATA[5]]></Face>
	 <Face><![CDATA[6]]></Face>
	 <Face><![CDATA[7]]></Face>
	 <Face><![CDATA[8]]></Face>
	 <Face><![CDATA[9]]></Face>
	 <Face><![CDATA[10]]></Face>
	 <Face><![CDATA[11]]></Face>
	 <Face><![CDATA[12]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>6</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>7</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>8</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>9</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>10</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>11</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_d20",
			Description = "Turns an item into a fair 20-sided die. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[1]]></Face>
	 <Face><![CDATA[2]]></Face>
	 <Face><![CDATA[3]]></Face>
	 <Face><![CDATA[4]]></Face>
	 <Face><![CDATA[5]]></Face>
	 <Face><![CDATA[6]]></Face>
	 <Face><![CDATA[7]]></Face>
	 <Face><![CDATA[8]]></Face>
	 <Face><![CDATA[9]]></Face>
	 <Face><![CDATA[10]]></Face>
	 <Face><![CDATA[11]]></Face>
	 <Face><![CDATA[12]]></Face>
	 <Face><![CDATA[13]></Face>
	 <Face><![CDATA[14]]></Face>
	 <Face><![CDATA[15]]></Face>
	 <Face><![CDATA[16]]></Face>
	 <Face><![CDATA[17]]></Face>
	 <Face><![CDATA[18]]></Face>
	 <Face><![CDATA[19]]></Face>
	 <Face><![CDATA[20]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>6</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>7</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>8</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>9</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>10</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>11</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>12</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>13</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>14</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>15</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>16</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>17</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>18</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>19</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>20</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Dice",
			Name = "Dice_Fudge",
			Description =
				"Turns an item into a fair 6-sided die with 2 plus, 2 minus and 2 blank faces. Ideally combined with stackable.",
			Definition = @"<Definition>
   <Faces>
	 <Face><![CDATA[-]]></Face>
	 <Face><![CDATA[-]]></Face>
	 <Face><![CDATA[Blank]]></Face>
	 <Face><![CDATA[Blank]]></Face>
	 <Face><![CDATA[+]]></Face>
	 <Face><![CDATA[+]]></Face>
   </Faces>
   <Weights>
	 <Weight>
	   <Face>0</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>1</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>2</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>3</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>4</Face>
	   <Probability>1</Probability>
	 </Weight>
	 <Weight>
	   <Face>5</Face>
	   <Probability>1</Probability>
	 </Weight>
   </Weights>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		context.SaveChanges();

		#endregion
	}

	private void SeedItemsPart4(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
	{
		var now = DateTime.UtcNow;
		var dbaccount = context.Accounts.First();
		var nextId = context.GameItemComponentProtos.Max(x => x.Id) + 1;

		#region Lighting
		CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_Infinite",
				"Turns an item into an ever-burning torch.", 25, -1, false,
				"@ turn|turns on $1", "@ turn|turns off $1",
				"$0 begin|begins to flicker", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_1Hour",
				"Turns an item into a torch that burns for an hour.", 25, 3600, false,
				"@ turn|turns on $1", "@ turn|turns off $1",
				"$0 begin|begins to flicker", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_2Hour",
				"Turns an item into a torch that burns for two hours.", 25, 7200, false,
				"@ turn|turns on $1", "@ turn|turns off $1",
				"$0 begin|begins to flicker", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_3Hour",
				"Turns an item into a torch that burns for 3 hours.", 25, 10800, false,
				"@ turn|turns on $1", "@ turn|turns off $1",
				"$0 begin|begins to flicker", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "SignalFire",
				"Turns an item into a bright signal fire that burns for 3 hours.", 500, 10800, true,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Match",
				"Turns an item into a match that burns dimly for only a few seconds.", 5, 20, false,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle",
				"Turns an item into a candle that burns dimly for 12 hours.", 5, 43200, false,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Long",
				"Turns an item into a candle that burns very dimly for 48 hours.", 3, 172800, false,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Bright",
				"Turns an item into a candle that burns a little dimly for 6 hours.", 8, 21600, false,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Infinite",
				"Turns an item into an ever-burning candle.", 5, -1, false,
				"@ light|lights $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		CreateTorchComponent(context, ref nextId, dbaccount, now, "BrightCandle_Infinite",
				"Turns an item into an ever-burning bright candle.", 8, -1, false,
				"@ light|lights on $1", "@ extinguish|extinguishes $1",
				"$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

		var fuelLiquid = context.Liquids.FirstOrDefault(x => x.Name == "fuel") ??
			context.Liquids.First(x => x.Name == "water");
		CreateLanternComponent(context, ref nextId, dbaccount, now, "Lantern",
			"Turns an item into a lantern that burns any flammable fuel.",
			500, 0.2273046, false,
			"@ light|lights $1", "@ extinguish|extinguishes $1",
			"$0 begin|begins to splutter as the fuel runs low", "$0 have|has completely exhausted its fuel",
			fuelLiquid.Id, 0.000007892522);

		context.SaveChanges();
		#endregion

		#region Water Sources
		var waterLiquid = context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_WaterSource",
			"Turns an item into a self-refilling source of water.",
			1000000, waterLiquid.Id, 0.8333333333333334, false);

		var lakeLiquid = context.Liquids.FirstOrDefault(x => x.Name == "lake water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_LakeWaterSource",
			"Turns an item into a self-refilling source of lake water.",
			100000000, lakeLiquid.Id, 1000, false);

		var springLiquid = context.Liquids.FirstOrDefault(x => x.Name == "spring water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SpringWaterSource",
			"Turns an item into a self-refilling source of spring water.",
			100000000, springLiquid.Id, 1000, false);

		var riverLiquid = context.Liquids.FirstOrDefault(x => x.Name == "river water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_RiverWaterSource",
			"Turns an item into a self-refilling source of river water.",
			100000000, riverLiquid.Id, 1000, false);

		var liquid = context.Liquids.FirstOrDefault(x => x.Name == "swamp water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SwampWaterSource",
			"Turns an item into a self-refilling source of swamp water.",
			100000000, liquid.Id, 1000, false);

		liquid = context.Liquids.FirstOrDefault(x => x.Name == "brackish water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_BrackishWaterSource",
			"Turns an item into a self-refilling source of brackish water.",
			100000000, liquid.Id, 1000, false);

		liquid = context.Liquids.FirstOrDefault(x => x.Name == "salt water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SaltWaterSource",
			"Turns an item into a self-refilling source of salt water.",
			100000000, liquid.Id, 1000, false);

		var tapWaterLiquid = context.Liquids.FirstOrDefault(x => x.Name == "tap water") ??
			context.Liquids.First(x => x.Name == "water");
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_5L",
			"Turns an item into a 5L sink that can be filled up.",
			5, tapWaterLiquid.Id, 0.8333333333333334, true);
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_20L",
			"Turns an item into a 20L sink that can be filled up.",
			20, tapWaterLiquid.Id, 0.8333333333333334, true);
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_50L",
			"Turns an item into a 50L sink that can be filled up.",
			50, tapWaterLiquid.Id, 0.8333333333333334, true);
		CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Bathtub",
			"Turns an item into a 500L bathtub that can be filled up.",
			500, tapWaterLiquid.Id, 0.8333333333333334, true);

		context.SaveChanges();
		#endregion

		GameItemComponentProto component;
		#region Repair Kits

		var materials = context.Materials.AsEnumerable().DistinctBy(x => x.Name).ToDictionaryWithDefault(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var skills = context.TraitDefinitions.AsEnumerable().DistinctBy(x => x.Name).ToDictionaryWithDefault(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var damagetypes = new DamageType[]
		{
			DamageType.Slashing,
			DamageType.Chopping,
			DamageType.Crushing,
			DamageType.Piercing,
			DamageType.Ballistic,
			DamageType.Burning,
			DamageType.Freezing,
			DamageType.Chemical,
			DamageType.Shockwave,
			DamageType.Bite,
			DamageType.Claw,
			DamageType.Shearing,
			DamageType.BallisticArmourPiercing,
			DamageType.Wrenching,
			DamageType.Shrapnel,
			DamageType.Falling,
			DamageType.ArmourPiercing
		};

		void AddRepairKitType(string name, string description, WoundSeverity maximumSeverity, double repairPoints, long? traitId, double checkBonus, string[] materialBehaviourTypes, string[] requiredTags)
		{


			var repairMaterials = materials.Values.Where(x => materialBehaviourTypes.Any(y => y.Equals(((MaterialBehaviourType)(x.BehaviourType ?? 0)).DescribeEnum(), StringComparison.OrdinalIgnoreCase))).ToList();

			var repairComponent = new GameItemComponentProto
			{
				Id = nextId++,
				RevisionNumber = 0,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = 4,
					BuilderAccountId = dbaccount.Id,
					BuilderDate = now,
					BuilderComment = "Auto-generated by the system",
					ReviewerAccountId = dbaccount.Id,
					ReviewerComment = "Auto-generated by the system",
					ReviewerDate = now
				},
				Type = "RepairKit",
				Name = $"Repair_{name}",
				Description = $"Turns an item into {description}",
				Definition = new XElement("Definition",
						new XElement("MaximumSeverity", (int)maximumSeverity),
						new XElement("RepairPoints", repairPoints),
						new XElement("CheckTrait", traitId ?? skills.Values.First().Id),
						new XElement("CheckBonus", checkBonus),
						new XElement("Echoes",
							new XElement("Echo", new XCData("$0 take|takes up $2, rifling through it for the necessary tools to fix $1")),
							new XElement("Echo", new XCData("$0 begin|begins repairing $1 with $")),
							new XElement("Echo", new XCData("$0 continue|continues repairing $1 with $2")),
							new XElement("Echo", new XCData("$0 finish|finishes repairing $1, then place|places the tools back within $2 and pack|packs it away."))
						),
						new XElement("DamageTypes",
							from type in damagetypes
							select new XElement("DamageType", (int)type)
						),
						new XElement("Materials",
							from material in repairMaterials
							select new XElement("Material", material.Id)
						),
						new XElement("Tags",
							from tag in requiredTags
							select new XElement("Tag", _tags[tag].Id)
						)
					).ToString()
			};
			AddGameItemComponent(context, repairComponent);
		}

		AddRepairKitType("Cloth", "a repair kit that repairs cloth items", WoundSeverity.Grievous, 500, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 0.0, ["Fabric", "Hair", "Feather"], []);
		AddRepairKitType("Cloth_Good", "a good-quality repair kit that repairs cloth items", WoundSeverity.Horrifying, 750, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 1.0, ["Fabric", "Hair", "Feather"], []);
		AddRepairKitType("Cloth_Poor", "a poor-quality repair kit that repairs cloth items", WoundSeverity.Severe, 300, (skills["Tailoring"] ?? skills["Tailor"])?.Id, -1.0, ["Fabric", "Hair", "Feather"], []);

		AddRepairKitType("Leather", "a repair kit that repairs leather items", WoundSeverity.Grievous, 500, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 0.0, ["Leather", "Skin", "Flesh"], []);
		AddRepairKitType("Leather_Good", "a good-quality repair kit that repairs leather items", WoundSeverity.Horrifying, 750, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 1.0, ["Leather", "Skin", "Flesh"], []);
		AddRepairKitType("Leather_Poor", "a poor-quality repair kit that repairs leather items", WoundSeverity.Severe, 300, (skills["Tailoring"] ?? skills["Tailor"])?.Id, -1.0, ["Leather", "Skin", "Flesh"], []);

		AddRepairKitType("Metal_Armour", "a repair kit that repairs metal armour", WoundSeverity.Grievous, 1000, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Armour"]);
		AddRepairKitType("Metal_Armour_Good", "a good-quality repair kit that repairs metal armour", WoundSeverity.Horrifying, 1500, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Armour"]);
		AddRepairKitType("Metal_Armour_Poor", "a poor-quality repair kit that repairs metal armour", WoundSeverity.Severe, 600, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Armour"]);

		AddRepairKitType("Metal_Weapon", "a repair kit that repairs metal weapons", WoundSeverity.Grievous, 1000, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Weapons"]);
		AddRepairKitType("Metal_Weapon_Good", "a good-quality repair kit that repairs metal weapons", WoundSeverity.Horrifying, 1500, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Weapons"]);
		AddRepairKitType("Metal_Weapon_Poor", "a poor-quality repair kit that repairs metal weapons", WoundSeverity.Severe, 600, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Weapons"]);

		AddRepairKitType("Metal_Tool", "a repair kit that repairs metal tools", WoundSeverity.Grievous, 1000, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Tools"]);
		AddRepairKitType("Metal_Tool_Good", "a good-quality repair kit that repairs metal tools", WoundSeverity.Horrifying, 1500, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Tools"]);
		AddRepairKitType("Metal_Tool_Poor", "a poor-quality repair kit that repairs metal tools", WoundSeverity.Severe, 600, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Tools"]);

		AddRepairKitType("Universal", "a repair kit that repairs anything", WoundSeverity.Severe, 250, (skills["Salvaging"] ?? skills["Salvage"])?.Id, -1.0, [], []);
		AddRepairKitType("Universal_Good", "a good-quality repair kit that repairs anything", WoundSeverity.VerySevere, 350, (skills["Salvaging"] ?? skills["Salvage"])?.Id, 0.0, [], []);
		AddRepairKitType("Universal_Poor", "a poor-quality repair kit that repairs anything", WoundSeverity.Moderate, 150, (skills["Salvaging"] ?? skills["Salvage"])?.Id, -2.0, [], []);
		#endregion

		#region Additional Builder Examples

		GameItemComponentProto AddExtraComponent(string type, string name, string description, XElement definition)
		{
			return CreateComponent(context, ref nextId, dbaccount, now, type, name, description, definition.ToString());
		}

		AddExtraComponent("LockingContainer", "LockingContainer_Lockbox",
			"Turns an item into a small lockbox with a built-in lever lock.",
			new XElement("Definition",
				new XAttribute("Weight", 2500),
				new XAttribute("MaxSize", (int)SizeCategory.Tiny),
				new XAttribute("Preposition", "in"),
				new XAttribute("Transparent", false),
				new XElement("ForceDifficulty", (int)Difficulty.Hard),
				new XElement("PickDifficulty", (int)Difficulty.Normal),
				new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
				new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
				new XElement("LockEmoteNoActor", new XCData("@ click|clicks shut.")),
				new XElement("UnlockEmoteNoActor", new XCData("@ click|clicks open.")),
				new XElement("LockType", "Lever Lock")));
		AddExtraComponent("LockingContainer", "LockingContainer_Footlocker",
			"Turns an item into a large locking footlocker or strongbox.",
			new XElement("Definition",
				new XAttribute("Weight", 25000),
				new XAttribute("MaxSize", (int)SizeCategory.Normal),
				new XAttribute("Preposition", "in"),
				new XAttribute("Transparent", false),
				new XElement("ForceDifficulty", (int)Difficulty.VeryHard),
				new XElement("PickDifficulty", (int)Difficulty.Hard),
				new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
				new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
				new XElement("LockEmoteNoActor", new XCData("@ thunk|thunks shut.")),
				new XElement("UnlockEmoteNoActor", new XCData("@ clunk|clunks open.")),
				new XElement("LockType", "Ward Lock")));
		AddExtraComponent("LockingContainer", "LockingContainer_SafeChest",
			"Turns an item into a heavy safe-style locking chest.",
			new XElement("Definition",
				new XAttribute("Weight", 125000),
				new XAttribute("MaxSize", (int)SizeCategory.Large),
				new XAttribute("Preposition", "in"),
				new XAttribute("Transparent", false),
				new XElement("ForceDifficulty", (int)Difficulty.ExtremelyHard),
				new XElement("PickDifficulty", (int)Difficulty.VeryHard),
				new XElement("LockEmote", new XCData("@ spin|spins the tumblers on $1 and lock|locks it.")),
				new XElement("UnlockEmote", new XCData("@ work|works the tumblers on $1 and unlock|unlocks it.")),
				new XElement("LockEmoteNoActor", new XCData("@ seal|seals itself with a heavy metallic clunk.")),
				new XElement("UnlockEmoteNoActor", new XCData("@ release|releases its locking bolts with a heavy clunk.")),
				new XElement("LockType", "Safe Lock")));

		AddExtraComponent("Keyring", "Keyring_Small",
			"Turns an item into a small keyring for a handful of keys.",
			new XElement("Definition", new XElement("MaximumNumberOfKeys", 4)));
		AddExtraComponent("Keyring", "Keyring_Large",
			"Turns an item into a large janitorial-style keyring.",
			new XElement("Definition", new XElement("MaximumNumberOfKeys", 20)));

		AddExtraComponent("Locksmithing Tool", "Locksmithing_Poor",
			"Turns an item into a poor set of breakable locksmithing tools.",
			new XElement("Definition",
				new XElement("DifficultyAdjustment", -2),
				new XElement("UsableForInstallation", true),
				new XElement("UsableForConfiguration", true),
				new XElement("UsableForFabrication", false),
				new XElement("Breakable", true)));
		AddExtraComponent("Locksmithing Tool", "Locksmithing_Standard",
			"Turns an item into a standard set of locksmithing tools.",
			new XElement("Definition",
				new XElement("DifficultyAdjustment", 0),
				new XElement("UsableForInstallation", true),
				new XElement("UsableForConfiguration", true),
				new XElement("UsableForFabrication", true),
				new XElement("Breakable", true)));
		AddExtraComponent("Locksmithing Tool", "Locksmithing_Fine",
			"Turns an item into a fine set of locksmithing tools.",
			new XElement("Definition",
				new XElement("DifficultyAdjustment", 2),
				new XElement("UsableForInstallation", true),
				new XElement("UsableForConfiguration", true),
				new XElement("UsableForFabrication", true),
				new XElement("Breakable", false)));
		AddExtraComponent("Locksmithing Tool", "Locksmithing_Installation",
			"Turns an item into locksmithing tools intended for installation work.",
			new XElement("Definition",
				new XElement("DifficultyAdjustment", 1),
				new XElement("UsableForInstallation", true),
				new XElement("UsableForConfiguration", false),
				new XElement("UsableForFabrication", false),
				new XElement("Breakable", false)));
		AddExtraComponent("Locksmithing Tool", "Locksmithing_Fabrication",
			"Turns an item into locksmithing tools intended for lock and key fabrication.",
			new XElement("Definition",
				new XElement("DifficultyAdjustment", 1),
				new XElement("UsableForInstallation", false),
				new XElement("UsableForConfiguration", false),
				new XElement("UsableForFabrication", true),
				new XElement("Breakable", false)));

		AddExtraComponent("PencilSharpener", "PencilSharpener",
			"Turns an item into a pencil sharpener.",
			new XElement("Definition",
				new XElement("SharpenEmote", new XCData("$0 brace|braces $2 against $1 and sharpen|sharpens it to a fine point."))));

		var uprightTableCover = context.RangedCovers.FirstOrDefault(x => x.Name == "Upright Table");
		var overturnedTableCover = context.RangedCovers.FirstOrDefault(x => x.Name == "Overturned Table");
		if (uprightTableCover is not null && overturnedTableCover is not null)
		{
			AddExtraComponent("Bench", "Bench_Double",
				"Makes an item a compact bench with two seating positions and standard flippable cover behaviour.",
				new XElement("Definition",
					new XAttribute("MaximumChairSlots", 2),
					new XAttribute("Chair", 0),
					new XAttribute("ChairCount", 0),
					new XElement("Cover",
						new XElement("Flipped", overturnedTableCover.Id),
						new XElement("NotFlipped", uprightTableCover.Id),
						new XElement("Expression", new XCData("0")),
						new XElement("Message", new XCData("@ try|tries to flip $1, but are|is not strong enough.")))));
			AddExtraComponent("Bench", "Bench_Triple",
				"Makes an item a larger bench with three seating positions and standard flippable cover behaviour.",
				new XElement("Definition",
					new XAttribute("MaximumChairSlots", 3),
					new XAttribute("Chair", 0),
					new XAttribute("ChairCount", 0),
					new XElement("Cover",
						new XElement("Flipped", overturnedTableCover.Id),
						new XElement("NotFlipped", uprightTableCover.Id),
						new XElement("Expression", new XCData("0")),
						new XElement("Message", new XCData("@ try|tries to flip $1, but are|is not strong enough.")))));
		}

		AddExtraComponent("ClothingInsulation", "Insulation_Reflective_Strong",
			"Makes garment strongly reflective without adding much insulation.",
			new XElement("Definition",
				new XElement("InsulatingDegrees", 0.25),
				new XElement("ReflectingDegrees", 3.0)));
		AddExtraComponent("ClothingInsulation", "Insulation_Reflective_Extreme",
			"Makes garment extremely reflective for hot and bright environments.",
			new XElement("Definition",
				new XElement("InsulatingDegrees", 0.5),
				new XElement("ReflectingDegrees", 5.0)));
		AddExtraComponent("ClothingInsulation", "Insulation_Balanced_Warm",
			"Makes garment a warm and moderately reflective all-rounder.",
			new XElement("Definition",
				new XElement("InsulatingDegrees", 1.5),
				new XElement("ReflectingDegrees", 1.5)));
		AddExtraComponent("ClothingInsulation", "Insulation_Balanced_Heavy",
			"Makes garment heavily insulating but still somewhat reflective.",
			new XElement("Definition",
				new XElement("InsulatingDegrees", 3.0),
				new XElement("ReflectingDegrees", 1.25)));

		var eyeColour = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Eye Colour");
		var hairColour = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Hair Colour");
		var hairStyle = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Hair Style");
		var allEyeColours = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Eye Colours");
		var allHairColours = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Hair Colours");
		var allHairStyles = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Hair Styles");
		var hatWearProfile = context.WearProfiles.FirstOrDefault(x => x.Name == "Hat");
		var glassesWearProfile = context.WearProfiles.FirstOrDefault(x => x.Name == "Glasses");

		if (hairColour is not null && hairStyle is not null && eyeColour is not null)
		{
			AddExtraComponent("IdentityObscurer", "CharacteristicMaskingObscurer",
				"Makes garment hide identity while also masking hair and eye characteristics.",
				new XElement("Definition",
					new XElement("RemovalEcho", new XCData("revealing $haircolour $hairstyle hair and $eyecolour eyes.")),
					new XElement("ShortDescription", new XCData("&a_an masked stranger")),
					new XElement("FullDescription", new XCData("This individual is heavily disguised, leaving only a carefully controlled impression behind.")),
					new XElement("Difficulty", (int)Difficulty.VeryHard),
					new XElement("Keywords",
						new XElement("Keyword", new XAttribute("key", "mask"), new XAttribute("value", "masked")),
						new XElement("Keyword", new XAttribute("key", "hood"), new XAttribute("value", "hooded"))),
					new XElement("Characteristics",
						new XElement("Characteristic", new XAttribute("Definition", hairColour.Id), new XAttribute("Form", "dark")),
						new XElement("Characteristic", new XAttribute("Definition", hairStyle.Id), new XAttribute("Form", "cropped")),
						new XElement("Characteristic", new XAttribute("Definition", eyeColour.Id), new XAttribute("Form", "grey")))));
		}

		if (allHairColours is not null && allHairStyles is not null && hairColour is not null && hairStyle is not null)
		{
			AddExtraComponent("Variable Changer", "Wig_HatOnly",
				"Changes hair colour and style only when worn in a hat-like profile.",
				new XElement("Definition",
					new XAttribute("TargetWearProfile", hatWearProfile?.Id.ToString() ?? string.Empty),
					new XElement("Characteristic", new XAttribute("Profile", allHairColours.Id), new XAttribute("Value", hairColour.Id)),
					new XElement("Characteristic", new XAttribute("Profile", allHairStyles.Id), new XAttribute("Value", hairStyle.Id))));
		}

		if (allEyeColours is not null && eyeColour is not null)
		{
			AddExtraComponent("Variable Changer", "ColouredContacts_GlassesProfile",
				"Changes eye colour only when worn in a glasses-style wear profile.",
				new XElement("Definition",
					new XAttribute("TargetWearProfile", glassesWearProfile?.Id.ToString() ?? string.Empty),
					new XElement("Characteristic", new XAttribute("Profile", allEyeColours.Id), new XAttribute("Value", eyeColour.Id))));
			AddExtraComponent("Obscurer", "Obscurer_Eyes",
				"Obscures a wearer's eye colour without changing their whole identity.",
				new XElement("Definition",
					new XElement("RemovalEcho", new XCData("revealing $eyecolour eyes again.")),
					new XElement("Characteristic", new XAttribute("Form", "shadowed"), new XAttribute("Definition", eyeColour.Id))));
		}

		var bonusTrait = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Medicine")
			?? context.TraitDefinitions.FirstOrDefault(x => x.Name == "Search")
			?? context.TraitDefinitions.FirstOrDefault(x => x.Name == "Stealth")
			?? context.TraitDefinitions.FirstOrDefault();
		if (bonusTrait is not null)
		{
			AddExtraComponent("WornTraitChanger", "WornTraitChanger_Bonus",
				"Provides a small bonus to a common trait while worn.",
				new XElement("Definition",
					new XElement("Modifier",
						new XAttribute("trait", bonusTrait.Id),
						new XAttribute("bonus", 1.5),
						new XAttribute("context", (int)TraitBonusContext.None))));
			AddExtraComponent("WornTraitChanger", "WornTraitChanger_Penalty",
				"Provides a small penalty to a common trait while worn.",
				new XElement("Definition",
					new XElement("Modifier",
						new XAttribute("trait", bonusTrait.Id),
						new XAttribute("bonus", -1.0),
						new XAttribute("context", (int)TraitBonusContext.None))));
		}

		AddExtraComponent("Sheath", "Holster_Small",
			"Turns an item into a small firearm holster.",
			new XElement("Definition",
				new XAttribute("StealthDrawDifficulty", (int)Difficulty.Normal),
				new XAttribute("MaximumSize", (int)SizeCategory.Small),
				new XAttribute("DesignedForGuns", true)));
		AddExtraComponent("Sheath", "Holster_Large",
			"Turns an item into a large firearm holster.",
			new XElement("Definition",
				new XAttribute("StealthDrawDifficulty", (int)Difficulty.Hard),
				new XAttribute("MaximumSize", (int)SizeCategory.Normal),
				new XAttribute("DesignedForGuns", true)));

		AddExtraComponent("Restraint", "Restraint_ArmsOnly",
			"Turns an item into restraints intended for arms and appendages only.",
			new XElement("Definition",
				new XElement("MinimumCreatureSize", (int)SizeCategory.Small),
				new XElement("MaximumCreatureSize", (int)SizeCategory.Large),
				new XElement("BreakoutDifficulty", (int)Difficulty.Hard),
				new XElement("OverpowerDifficulty", (int)Difficulty.Hard),
				new XElement("LimbType", (int)LimbType.Arm),
				new XElement("LimbType", (int)LimbType.Appendage)));
		AddExtraComponent("Restraint", "Restraint_Hobbles",
			"Turns an item into leg restraints or hobbles.",
			new XElement("Definition",
				new XElement("MinimumCreatureSize", (int)SizeCategory.Small),
				new XElement("MaximumCreatureSize", (int)SizeCategory.VeryLarge),
				new XElement("BreakoutDifficulty", (int)Difficulty.Normal),
				new XElement("OverpowerDifficulty", (int)Difficulty.Hard),
				new XElement("LimbType", (int)LimbType.Leg)));
		AddExtraComponent("Restraint", "Restraint_Oversized",
			"Turns an item into oversized restraints for large creatures.",
			new XElement("Definition",
				new XElement("MinimumCreatureSize", (int)SizeCategory.Large),
				new XElement("MaximumCreatureSize", (int)SizeCategory.Gigantic),
				new XElement("BreakoutDifficulty", (int)Difficulty.Hard),
				new XElement("OverpowerDifficulty", (int)Difficulty.VeryHard),
				new XElement("LimbType", (int)LimbType.Arm),
				new XElement("LimbType", (int)LimbType.Leg),
				new XElement("LimbType", (int)LimbType.Appendage)));

		AddExtraComponent("DragAid", "DragAid_Sling",
			"Turns an item into a simple dragging sling.",
			new XElement("Definition",
				new XElement("MaximumUsers", 2),
				new XElement("EffortMultiplier", 1.5)));
		AddExtraComponent("DragAid", "DragAid_Harness",
			"Turns an item into a heavy dragging harness for team work.",
			new XElement("Definition",
				new XElement("MaximumUsers", 4),
				new XElement("EffortMultiplier", 2.5)));

		AddExtraComponent("WaterSource", "WaterSource_Canteen",
			"Turns an item into a transparent closable refill-on-toggle canteen.",
			new XElement("Definition",
				new XAttribute("LiquidCapacity", 1.0),
				new XAttribute("Closable", true),
				new XAttribute("Transparent", true),
				new XAttribute("OnceOnly", false),
				new XAttribute("DefaultLiquid", waterLiquid.Id),
				new XAttribute("RefillRate", 0.25),
				new XAttribute("UseOnOffForRefill", true),
				new XAttribute("RefillingProg", 0),
				new XAttribute("CanBeEmptiedWhenInRoom", true)));
		AddExtraComponent("WaterSource", "WaterSource_DisposableBottle",
			"Turns an item into a single-use disposable water source.",
			new XElement("Definition",
				new XAttribute("LiquidCapacity", 0.6),
				new XAttribute("Closable", true),
				new XAttribute("Transparent", true),
				new XAttribute("OnceOnly", true),
				new XAttribute("DefaultLiquid", waterLiquid.Id),
				new XAttribute("RefillRate", 0.0),
				new XAttribute("UseOnOffForRefill", false),
				new XAttribute("RefillingProg", 0),
				new XAttribute("CanBeEmptiedWhenInRoom", false)));
		var alwaysTrueProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue");
		if (alwaysTrueProg is not null)
		{
			AddExtraComponent("WaterSource", "WaterSource_ProgControlled",
				"Turns an item into a self-refilling water source controlled by a prog.",
				new XElement("Definition",
					new XAttribute("LiquidCapacity", 10.0),
					new XAttribute("Closable", false),
					new XAttribute("Transparent", false),
					new XAttribute("OnceOnly", false),
					new XAttribute("DefaultLiquid", waterLiquid.Id),
					new XAttribute("RefillRate", 1.0),
					new XAttribute("UseOnOffForRefill", false),
					new XAttribute("RefillingProg", alwaysTrueProg.Id),
					new XAttribute("CanBeEmptiedWhenInRoom", true)));
		}

		AddExtraComponent("Destroyable", "Destroyable_Glassware",
			"Turns an item into a fragile glass object.",
			new XElement("Definition",
				new XElement("HpExpression", new XCData("4 * quality")),
				new XElement("DamageMultipliers",
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Crushing), new XAttribute("multiplier", 1.6)),
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Ballistic), new XAttribute("multiplier", 1.2)),
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 0.1)))));
		AddExtraComponent("Destroyable", "Destroyable_Paper",
			"Turns an item into a fragile paper or parchment object.",
			new XElement("Definition",
				new XElement("HpExpression", new XCData("2 * quality")),
				new XElement("DamageMultipliers",
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 2.5)),
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Piercing), new XAttribute("multiplier", 1.1)))));
		AddExtraComponent("Destroyable", "Destroyable_WoodenHeavy",
			"Turns an item into a sturdy wooden object.",
			new XElement("Definition",
				new XElement("HpExpression", new XCData("15 * quality")),
				new XElement("DamageMultipliers",
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Chopping), new XAttribute("multiplier", 1.35)),
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 1.75)))));
		AddExtraComponent("Destroyable", "Destroyable_HeavyMetal",
			"Turns an item into a heavy metal object.",
			new XElement("Definition",
				new XElement("HpExpression", new XCData("30 * quality")),
				new XElement("DamageMultipliers",
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Crushing), new XAttribute("multiplier", 0.65)),
					new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 0.15)))));

		AddExtraComponent("Treatment", "FieldMedkit",
			"Turns the item into a multi-purpose field medkit.",
			new XElement("Definition",
				new XElement("MaximumUses", 25),
				new XElement("Refillable", true),
				new XElement("DifficultyStages", 2),
				new XElement("TreatmentType", 1),
				new XElement("TreatmentType", 2),
				new XElement("TreatmentType", 3),
				new XElement("TreatmentType", 4),
				new XElement("TreatmentType", 11)));
		AddExtraComponent("Treatment", "Bandage_Poor",
			"Turns the item into a poor quality bandage example.",
			new XElement("Definition",
				new XElement("MaximumUses", 1),
				new XElement("Refillable", false),
				new XElement("DifficultyStages", -1),
				new XElement("TreatmentType", 2)));
		AddExtraComponent("Treatment", "Treatment_AdminUnlimited",
			"Turns the item into an unlimited all-purpose treatment kit.",
			new XElement("Definition",
				new XElement("MaximumUses", -1),
				new XElement("Refillable", false),
				new XElement("DifficultyStages", 5),
				new XElement("TreatmentType", 1),
				new XElement("TreatmentType", 2),
				new XElement("TreatmentType", 3),
				new XElement("TreatmentType", 4),
				new XElement("TreatmentType", 11)));

		var prostheticBody = context.BodyProtos.FirstOrDefault(x => x.Name == "Organic Humanoid") ??
		                    context.BodyProtos.FirstOrDefault(x => x.Name == "Humanoid");
		var humanRace = context.Races.FirstOrDefault(x => x.Name == "Human") ?? context.Races.FirstOrDefault();
		if (prostheticBody is not null)
		{
			var leftHand = context.BodypartProtos.FirstOrDefault(x => x.Name == "lhand");
			var rightFoot = context.BodypartProtos.FirstOrDefault(x => x.Name == "rfoot");
			if (leftHand is not null)
			{
				AddExtraComponent("Prosthetic", "Prosthetic_LHand_Functional",
					"Turns the item into a functional prosthetic left hand.",
					new XElement("Definition",
						new XElement("Obvious", false),
						new XElement("Functional", true),
						new XElement("TargetBody", prostheticBody.Id),
						new XElement("TargetBodypart", leftHand.Id),
						new XElement("Gender", (int)Gender.Indeterminate),
						new XElement("Race", humanRace?.Id ?? 0L)));
			}

			if (rightFoot is not null)
			{
				AddExtraComponent("Prosthetic", "Prosthetic_RFoot_Ornamental",
					"Turns the item into an obvious ornamental prosthetic right foot.",
					new XElement("Definition",
						new XElement("Obvious", true),
						new XElement("Functional", false),
						new XElement("TargetBody", prostheticBody.Id),
						new XElement("TargetBodypart", rightFoot.Id),
						new XElement("Gender", (int)Gender.Female),
						new XElement("Race", humanRace?.Id ?? 0L)));
			}
		}

		var primaryClock = context.Clocks.FirstOrDefault();
		var defaultTimeZone = primaryClock is null
			? null
			: context.Timezones.FirstOrDefault(x => x.Id == primaryClock.PrimaryTimezoneId) ??
			  context.Timezones.FirstOrDefault(x => x.ClockId == primaryClock.Id);
		if (primaryClock is not null && defaultTimeZone is not null)
		{
			AddExtraComponent("TimePiece", "TimePiece_Standard",
				"Turns an item into a standard modern timepiece.",
				new XElement("Definition",
					new XElement("Clock", primaryClock.Id),
					new XElement("TimeZone", defaultTimeZone.Id),
					new XElement("PlayersCanSetTime", false),
					new XElement("TimeDisplayString", "$j:$m $i")));
			AddExtraComponent("TimePiece", "TimePiece_Adjustable",
				"Turns an item into an adjustable builder-facing timepiece.",
				new XElement("Definition",
					new XElement("Clock", primaryClock.Id),
					new XElement("TimeZone", defaultTimeZone.Id),
					new XElement("PlayersCanSetTime", true),
					new XElement("TimeDisplayString", "$h:$m:$s $t")));
		}

		context.SaveChanges();
		#endregion

		#region Smokeables
		var saveChangesRequired = false;
		var characterTypeDefinition = ProgVariableTypes.Character.ToStorageString();
		if (!context.VariableDefinitions.Any(x => x.OwnerTypeDefinition == characterTypeDefinition && x.Property == "nicotineuntil"))
		{
			context.VariableDefinitions.Add(new VariableDefinition
			{
				ContainedType = (long)ProgVariableTypes.DateTime,
				OwnerType = 8,
				Property = "nicotineuntil"
			});
			saveChangesRequired = true;
		}

		if (!context.VariableDefaults.Any(x => x.OwnerTypeDefinition == characterTypeDefinition && x.Property == "nicotineuntil"))
		{
			context.VariableDefaults.Add(new VariableDefault
			{
				OwnerType = 8,
				Property = "nicotineuntil",
				DefaultValue = "<var>01/01/0001 00:00:00</var>"
			});
			saveChangesRequired = true;
		}

		var smokeProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "OnSmokeCigarette");
		if (smokeProg is null)
		{
			smokeProg = new FutureProg
			{
				FunctionName = "OnSmokeCigarette",
				AcceptsAnyParameters = false,
				ReturnType = 0,
				Category = "Character",
				Subcategory = "Smoking",
				Public = false,
				FunctionComment = "This prog gives the character a 5 minute nicotene hit.",
				FunctionText = @"var NicotineUntil as datetime
NicotineUntil = ifnull(GetRegister(@ch, ""NicotineUntil""), now())
if (@nicotineuntil < now())
  NicotineUntil = now()
end if
SetRegister @ch ""NicotineUntil"" (@NicotineUntil + 5m)",
				StaticType = 0
			};
			smokeProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = smokeProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Character });
			smokeProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = smokeProg, ParameterIndex = 1, ParameterName = "item", ParameterType = (long)ProgVariableTypes.Item });
			context.FutureProgs.Add(smokeProg);
			saveChangesRequired = true;
		}

		if (saveChangesRequired)
		{
			context.SaveChanges();
		}
		component = new GameItemComponentProto
		{
			Id = nextId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Type = "Smokeable",
			Name = "Cigarette",
			Description = "Turns an item into a smokeable tobacco cigarette",
			Definition = @$"<Definition>
   <SecondsOfFuel>600</SecondsOfFuel>
   <SecondsPerDrag>10</SecondsPerDrag>
   <SecondsOfEffectPerSecondOfFuel>5</SecondsOfEffectPerSecondOfFuel>
   <OnDragProg>{smokeProg.Id}</OnDragProg>
   <PlayerDescriptionEffectString>The lingering, acrid smell of tobacco clings to this individual.</PlayerDescriptionEffectString>
   <RoomDescriptionEffectString>The lingering, acrid smell of tobacco hangs in the air here.</RoomDescriptionEffectString>
   <Drug>0</Drug>
   <GramsPerDrag>0</GramsPerDrag>
 </Definition>"
		};
		AddGameItemComponent(context, component);
		#endregion
		context.SaveChanges();
	}

	private void SeedTerrainAutobuilder(FuturemudDatabaseContext context,
			IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
	{

	}

	private void SeedRangedCovers(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.RangedCovers.Any())
		{
			errors.Add("Detected that ranged covers were already installed. Did not seed any covers.");
			return;
		}

		var covers = new List<(string Name, int Type, int Extent, int Position, string Desc, string Action, int Max, bool Moving)>
				{
						("Uneven Ground", 0, 0, 6, "prone, using the uneven ground as cover", "$0 go|goes prone and begin|begins to use the uneven ground as cover", 0, true),
						("Corridor Doorway", 0, 0, 1, "using a doorway as cover", "$0 duck|ducks into a doorway and begin|begins to use it as cover", 0, false),
						("Large Crater", 0, 1, 6, "prone, using the edge of a large crater as cover", "$0 go|goes prone and begin|begins to use the edge of a large crater as cover", 0, false),
						("Chunk of Rubble", 1, 1, 12, "slumped up against $?0|$0|a large chunk of rubble|$ as cover", "$0 slump|slumps up against $?1|$1|a large chunk of rubble|$ and begin|begins to use it as cover", 2, false),
						("Tree", 1, 1, 1, "hiding behind $?0|$0|a tree|$ for cover", "$0 slip|slips behind $?1|$1|a tree|$ and use|uses it to protect &0's vital areas", 1, false),
						("Bush", 0, 1, 3, "hiding behind $?0|$0|a bush|$ for cover", "$0 take|takes position behind $?1|$1|a bush|$ and use|uses it to obscure &0's profile", 1, false),
						("Refuse Heap", 1, 1, 6, "half-hidden within $?0|$0|a pile of refuse|$, using it as cover", "$0 dive|dives into $?1|$1|a pile of refuse|$, using it to provide cover", 0, false),
						("Upright Table", 1, 0, 1, "using $?0|$0|a table|$ as cover", "$0 move|moves behind $?1|$1|a nearby table|$ and use|uses it to obscure &0's profile", 3, false),
						("Overturned Table", 1, 1, 3, "hiding behind $?0|$0|an overturned table|$ as cover", "$0 duck|ducks behind $?1|$1|an overturned table|$ and begin|begins to use it as cover", 3, false),
						("Smoke", 0, 1, 1, "obscured by $?0|$0|the smoke|$", "$0 move|moves into $?1|$1|the smoke|$ and uses it to obscure &0's form", 0, true),
						("Sandbag", 1, 1, 3, "hiding behind $?0|$0|a sandbag barricade|$, using it as cover", "$0 take|takes position behind $?1|$1|a sandbag barricade|$ and begin|begins to use it as cover", 5, false),
						("Stone Wall", 1, 2, 1, "hiding behind $?0|$0|a stone wall|$ for cover", "$0 slip|slips behind $?1|$1|a stone wall|$ and use|uses it for protection", 0, false),
						("Rubble Wall", 1, 2, 1, "hiding behind $?0|$0|a rubble wall|$ for cover", "$0 hide|hides behind $?1|$1|a rubble wall|$", 0, false),
						("Small Rock", 1, 0, 3, "crouched behind $?0|$0|a small rock|$", "$0 crouch|crouches behind $?1|$1|a small rock|$", 1, false),
						("Large Rock", 1, 1, 3, "hiding behind $?0|$0|a large rock|$", "$0 slip|slips behind $?1|$1|a large rock|$", 1, false),
						("Fallen Log", 1, 1, 3, "hiding behind $?0|$0|a fallen log|$", "$0 slip|slips behind $?1|$1|a fallen log|$", 1, false),
						("Pile of Crates", 1, 1, 1, "hiding behind $?0|$0|a pile of crates|$", "$0 hide|hides behind $?1|$1|a pile of crates|$", 2, false),
						("Barrel Stack", 1, 1, 1, "hiding behind $?0|$0|a stack of barrels|$", "$0 slip|slips behind $?1|$1|a stack of barrels|$", 2, false),
						("Low Hedge", 0, 0, 1, "hiding behind $?0|$0|a low hedge|$", "$0 duck|ducks behind $?1|$1|a low hedge|$", 1, false),
						("Thick Hedge", 0, 1, 1, "hiding behind $?0|$0|a thick hedge|$", "$0 take|takes cover behind $?1|$1|a thick hedge|$", 1, false),
						("Tall Grass", 0, 1, 6, "hiding in $?0|$0|tall grass|$", "$0 slip|slips into $?1|$1|tall grass|$", 2, true),
						("Shrubs", 0, 1, 3, "hiding behind $?0|$0|some shrubs|$", "$0 crouch|crouches behind $?1|$1|some shrubs|$", 1, false),
						("Vehicle", 1, 2, 1, "using $?0|$0|a vehicle|$ as cover", "$0 take|takes cover behind $?1|$1|a vehicle|$", 2, false),
						("Broken Vehicle", 1, 2, 3, "hiding behind $?0|$0|a broken vehicle|$", "$0 crouch|crouches behind $?1|$1|a broken vehicle|$", 2, false),
						("Collapsed Building", 1, 2, 5, "sheltering in $?0|$0|a collapsed building|$", "$0 dive|dives into $?1|$1|a collapsed building|$", 0, false),
						("Window Frame", 1, 0, 1, "using $?0|$0|a window frame|$ as cover", "$0 use|uses $?1|$1|a window frame|$ for cover", 1, false),
						("Ruined Wall", 1, 2, 1, "hiding behind $?0|$0|a ruined wall|$", "$0 take|takes cover behind $?1|$1|a ruined wall|$", 0, false),
						("Stalagmites", 1, 1, 1, "hiding among $?0|$0|stalagmites|$", "$0 dart|darts among $?1|$1|stalagmites|$", 2, false),
						("Pile of Junk", 1, 1, 3, "hiding behind $?0|$0|a pile of junk|$", "$0 crouch|crouches behind $?1|$1|a pile of junk|$", 1, false),
						("Pile of Bones", 1, 0, 3, "hiding behind $?0|$0|a pile of bones|$", "$0 crouch|crouches behind $?1|$1|a pile of bones|$", 1, false),
						("Dead Body", 1, 0, 3, "using $?0|$0|a dead body|$ as cover", "$0 crouch|crouches behind $?1|$1|a dead body|$", 1, false),
						("Sand Dune", 0, 1, 1, "hiding behind $?0|$0|a sand dune|$", "$0 use|uses $?1|$1|a sand dune|$ for cover", 0, true),
						("Snow Drift", 0, 1, 1, "hiding behind $?0|$0|a snow drift|$", "$0 hide|hides behind $?1|$1|a snow drift|$", 0, true),
						("Fallen Statue", 1, 1, 3, "hiding behind $?0|$0|a fallen statue|$", "$0 crouch|crouches behind $?1|$1|a fallen statue|$", 1, false),
						("Street Corner", 1, 2, 1, "using $?0|$0|a street corner|$ as cover", "$0 lean|leans around $?1|$1|a street corner|$", 0, false),
						("Alley Trash Bin", 1, 1, 1, "hiding behind $?0|$0|a trash bin|$", "$0 duck|ducks behind $?1|$1|a trash bin|$", 1, false),
						("Park Bench", 1, 0, 1, "using $?0|$0|a park bench|$ as cover", "$0 sit|sits behind $?1|$1|a park bench|$", 1, false),
						("Street Lamp", 1, 0, 1, "using $?0|$0|a street lamp|$ as cover", "$0 dodge|dodges behind $?1|$1|a street lamp|$", 1, false),
						("Old Well", 1, 2, 1, "using $?0|$0|an old well|$ as cover", "$0 hide|hides by $?1|$1|an old well|$", 1, false),
						("Rock Outcropping", 1, 2, 1, "hiding behind $?0|$0|a rock outcropping|$", "$0 slip|slips behind $?1|$1|a rock outcropping|$", 1, false),
						("Fallen Tree", 1, 1, 3, "hiding behind $?0|$0|a fallen tree|$", "$0 duck|ducks behind $?1|$1|a fallen tree|$", 1, false),
						("Fallen Pillar", 1, 1, 3, "hiding behind $?0|$0|a fallen pillar|$", "$0 duck|ducks behind $?1|$1|a fallen pillar|$", 1, false),
						("Thick Smoke", 0, 2, 1, "obscured by $?0|$0|thick smoke|$", "$0 move|moves into $?1|$1|thick smoke|$", 0, true),
						("Dense Fog", 0, 2, 1, "obscured by $?0|$0|dense fog|$", "$0 move|moves into $?1|$1|dense fog|$", 0, true),
						("Tall Reeds", 0, 1, 6, "hiding in $?0|$0|tall reeds|$", "$0 slip|slips into $?1|$1|tall reeds|$", 2, true),
						("Thick Seaweed", 0, 1, 6, "hiding in $?0|$0|thick seaweed|$", "$0 slip|slips into $?1|$1|thick seaweed|$", 2, true),
						("Dense Vegetation", 0, 2, 3, "hiding in $?0|$0|dense vegetation|$", "$0 move|moves into $?1|$1|dense vegetation|$", 2, true),
						("Boulder Cluster", 1, 2, 3, "hiding behind $?0|$0|a cluster of boulders|$", "$0 move|moves behind $?1|$1|a cluster of boulders|$", 2, false),
						("Abandoned Cart", 1, 1, 1, "hiding behind $?0|$0|an abandoned cart|$", "$0 duck|ducks behind $?1|$1|an abandoned cart|$", 1, false),
						("Bushy Tree", 1, 1, 1, "hiding behind $?0|$0|a bushy tree|$", "$0 slip|slips behind $?1|$1|a bushy tree|$", 1, false),
						("Shrubbery", 0, 1, 1, "hiding behind $?0|$0|some shrubbery|$", "$0 duck|ducks behind $?1|$1|some shrubbery|$", 1, false),
						("Counter", 1, 0, 1, "using $?0|$0|a counter|$ as cover", "$0 duck|ducks behind $?1|$1|a counter|$", 2, false),
						("Desk", 1, 0, 1, "using $?0|$0|a desk|$ as cover", "$0 duck|ducks behind $?1|$1|a desk|$", 1, false),
						("Staircase", 1, 1, 3, "hiding behind $?0|$0|a staircase|$", "$0 duck|ducks behind $?1|$1|a staircase|$", 1, false),
						("Corner", 1, 2, 1, "using $?0|$0|a corner|$ as cover", "$0 press|presses into $?1|$1|a corner|$", 0, false)
				};

		foreach (var item in covers)
		{
			context.RangedCovers.Add(new RangedCover
			{
				Name = item.Name,
				CoverType = item.Type,
				CoverExtent = item.Extent,
				HighestPositionState = item.Position,
				DescriptionString = item.Desc,
				ActionDescriptionString = item.Action,
				MaximumSimultaneousCovers = item.Max,
				CoverStaysWhileMoving = item.Moving
			});
		}
		context.SaveChanges();

		var coversByName = context.RangedCovers.ToDictionary(x => x.Name, x => x);
		var tagsById = context.Tags.ToDictionary(x => x.Id, x => x.Name);

		var coversForTags = new Dictionary<string, string[]>
		{
			["Urban"] = new[]
				{
								"Corridor Doorway", "Window Frame",
								"Corner", "Street Corner", "Alley Trash Bin"
						},
			["Rural"] = new[]
				{
								"Tree", "Bush", "Bushy Tree", "Shrubbery", "Fallen Log", "Fallen Tree",
								"Old Well"
						},
			["Terrestrial"] = new[]
				{
								"Uneven Ground", "Large Crater", "Stone Wall", "Rubble Wall", "Small Rock",
								"Large Rock", "Boulder Cluster", "Rock Outcropping", "Sand Dune", "Snow Drift",
								"Low Hedge", "Thick Hedge", "Tall Grass", "Shrubs", "Fallen Pillar"
						},
			["Aquatic"] = new[] { "Thick Seaweed" },
			["Littoral"] = new[] { "Sand Dune", "Tall Reeds" },
			["Riparian"] = new[] { "Tall Reeds", "Dense Vegetation" }
		};

		foreach (var terrain in context.Terrains.ToList())
		{
			var tagNames = terrain.TagInformation?.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(x => long.TryParse(x, out var val) && tagsById.ContainsKey(val)
							? tagsById[val]
							: null)
					.Where(x => x != null)
					.ToList() ?? new List<string>();

			var coverIds = new HashSet<long>();
			foreach (var tag in tagNames)
			{
				if (!coversForTags.TryGetValue(tag!, out var names))
				{
					continue;
				}

				foreach (var name in names)
				{
					if (coversByName.TryGetValue(name, out var cover))
					{
						coverIds.Add(cover.Id);
					}
				}
			}

			foreach (var id in coverIds)
			{
				context.TerrainsRangedCovers.Add(new TerrainsRangedCovers
				{
					TerrainId = terrain.Id,
					RangedCoverId = id
				});
			}
		}

		context.SaveChanges();
	}

	private void SeedAIExamples(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		var alwaysTrue = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue");
		var alwaysFalse = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysFalse");
		if (alwaysTrue is null || alwaysFalse is null)
		{
			errors.Add(
				"Could not seed AI examples because the prerequisite AlwaysTrue or AlwaysFalse FutureProg was missing.");
			return;
		}

		EnsureVariableDefinition(context, ProgVariableTypes.Character, "npcownerid", ProgVariableTypes.Number);
		EnsureVariableDefault(context, ProgVariableTypes.Character, "npcownerid", "<var>0</var>");

		var ownerProg = EnsureAiProg(
			context,
			"IsOwnerCanCommand",
			"Commands",
			ProgVariableTypes.Boolean,
			"Determines if the character has been set as the owner of an NPC.",
			"""
			var ownerid as number
			ownerid = ifnull(getregister(@tch, "npcownerid"), 0)
			return ownerid == @ch.Id
			""",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Character, "tch"),
			(ProgVariableTypes.Text, "cmd"));
		var cantCommandOwnerProg = EnsureAiProg(
			context,
			"WhyCantCommandNPCOwnerAI",
			"Commands",
			ProgVariableTypes.Text,
			"Returns an error message when a player cannot command an NPC they do not own.",
			@"return ""You are not the owner of "" + HowSeen(@ch, @tch, false, true) + "" and so you cannot issue commands.""",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Character, "tch"),
			(ProgVariableTypes.Text, "cmd"));
		var outranksProg = EnsureAiProg(
			context,
			"OutranksCanCommand",
			"Commands",
			ProgVariableTypes.Boolean,
			"Determines if the character outranks the NPC in any clan and can therefore command them.",
			"""
			foreach (clan in @tch.clans)
				if (outranks(@ch, @tch, @clan))
					return true
				end if
			end foreach
			return false
			""",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Character, "tch"),
			(ProgVariableTypes.Text, "cmd"));
		var cantCommandOutrankProg = EnsureAiProg(
			context,
			"WhyCantCommandNPCClanOutranks",
			"Commands",
			ProgVariableTypes.Text,
			"Returns an error message when a player cannot command an NPC they do not outrank.",
			@"return ""You do not outrank "" + HowSeen(@ch, @tch, false, true) + "" in any clans.""",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Character, "tch"),
			(ProgVariableTypes.Text, "cmd"));
		var doorguardWillOpen = EnsureAiProg(
			context,
			"DoorguardWillOpenDoor",
			"Doorguard",
			ProgVariableTypes.Boolean,
			"Determines whether a doorguard will open a door for a person.",
			"return isclanbrother(@guard, @ch)",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardDelay = EnsureAiProg(
			context,
			"DoorguardActionDelay",
			"Doorguard",
			ProgVariableTypes.Number,
			"A delay in milliseconds between the action that triggers the doorguard and them taking the action.",
			"return 40+random(1,40)",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardCloseDelay = EnsureAiProg(
			context,
			"DoorguardCloseDelay",
			"Doorguard",
			ProgVariableTypes.Number,
			"A delay in milliseconds between opening the door and closing the door.",
			"return 10000",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardOpenDoor = EnsureAiProg(
			context,
			"DoorguardOpenDoor",
			"Doorguard",
			ProgVariableTypes.Void,
			"The actual action for the doorguard to take when opening the door.",
			"""
			// Assumes doorguard has a key in their inventory
			force @guard ("emote move|moves to open the door for ~" + bestkeyword(@guard, @ch))
			force @guard ("unlock " + @exit.keyword)
			force @guard ("open " + @exit.keyword)
			""",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardCloseDoor = EnsureAiProg(
			context,
			"DoorguardCloseDoor",
			"Doorguard",
			ProgVariableTypes.Void,
			"The actual action for the doorguard to take when closing the door.",
			"""
			// Assumes doorguard has a key in their inventory
			force @guard ("emote move|moves to close the door")
			force @guard ("close " + @exit.keyword)
			force @guard ("lock " + @exit.keyword)
			""",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardWontOpen = EnsureAiProg(
			context,
			"DoorguardWontOpen",
			"Doorguard",
			ProgVariableTypes.Void,
			"An action for the doorguard to take if someone nods or knocks but they cannot let them in.",
			"""
			if (isnull(@exit) or @exit.Origin == @guard.Location)
				force @guard ("tell " + bestkeyword(@guard, @ch) + " I'm not allowed to let you through")
			else
				force @guard ("yell I'm not allowed to let you through")
			end if
			""",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var doorguardWitnessStop = EnsureAiProg(
			context,
			"DoorguardWitnessStop",
			"Doorguard",
			ProgVariableTypes.Void,
			"An action for the doorguard to take if someone walks into a closed door.",
			"""
			if (@DoorguardWillOpenDoor(@guard, @ch, @exit))
				force @guard ("tell " + bestkeyword(@guard, @ch) + " Give me a nod and I'll open the door for you")
			end if
			""",
			(ProgVariableTypes.Character, "guard"),
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Exit, "exit"));
		var aggressorWillAttack = EnsureAiProg(
			context,
			"TargetIsOtherRace",
			"Aggressor",
			ProgVariableTypes.Boolean,
			"Determines whether the aggressor will attack someone.",
			"return @ch.Race != @tch.Race",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Character, "tch"));
		var rescuerWillRescue = EnsureAiProg(
			context,
			"RescuerWillRescue",
			"Combat",
			ProgVariableTypes.Boolean,
			"Determines whether a rescuer will rescue someone who is being attacked.",
			"return isclanbrother(@rescuer, @target)",
			(ProgVariableTypes.Character, "rescuer"),
			(ProgVariableTypes.Character, "target"));
		var verminWillScavenge = EnsureAiProg(
			context,
			"VerminWillScavenge",
			"Vermin",
			ProgVariableTypes.Boolean,
			"Determines whether a vermin AI will scavenge an item.",
			"return @item.isholdable and (@item.isfood or @item.iscorpse)",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Item, "item"));
		var verminOnScavenge = EnsureAiProg(
			context,
			"VerminOnScavenge",
			"Vermin",
			ProgVariableTypes.Void,
			"Fires when a scavenger AI decides to scavenge an item.",
			"""force @ch ("eat " + BestKeyword(@ch, @item))""",
			(ProgVariableTypes.Character, "ch"),
			(ProgVariableTypes.Item, "item"));
		var lairFallbackHome = EnsureAiProg(
			context,
			"LairScavengerFallbackHome",
			"Scavenger",
			ProgVariableTypes.Location,
			"Returns the NPC's current location as a fallback lair until it claims a den.",
			"return @ch.Location",
			(ProgVariableTypes.Character, "ch"));

		context.SaveChanges();

		EnsureArtificialIntelligence(
			context,
			"CommandableOwner",
			"Commandable",
			new XElement("Definition",
				new XElement("CanCommandProg", ownerProg.Id),
				new XElement("WhyCannotCommandProg", cantCommandOwnerProg.Id),
				new XElement("CommandIssuedEmote", "You issue the following command to $1: {0}"),
				new XElement("BannedCommands",
					new XElement("BannedCommand", "ignoreforce"),
					new XElement("BannedCommand", "return"))).ToString());
		EnsureArtificialIntelligence(
			context,
			"CommandableClanOutranks",
			"Commandable",
			new XElement("Definition",
				new XElement("CanCommandProg", outranksProg.Id),
				new XElement("WhyCannotCommandProg", cantCommandOutrankProg.Id),
				new XElement("CommandIssuedEmote", "You issue the following command to $1: {0}"),
				new XElement("BannedCommands",
					new XElement("BannedCommand", "ignoreforce"),
					new XElement("BannedCommand", "return"))).ToString());
		EnsureArtificialIntelligence(
			context,
			"BasicDoorguard",
			"Doorguard",
			new XElement("Definition",
				new XElement("WillOpenDoorForProg", doorguardWillOpen.Id),
				new XElement("WontOpenDoorForActionProg", doorguardWontOpen.Id),
				new XElement("OpenDoorActionProg", doorguardOpenDoor.Id),
				new XElement("CloseDoorActionProg", doorguardCloseDoor.Id),
				new XElement("BaseDelayProg", doorguardDelay.Id),
				new XElement("OpenCloseDelayProg", doorguardCloseDelay.Id),
				new XElement("OnWitnessDoorStopProg", doorguardWitnessStop.Id),
				new XElement("RespectGameRulesForOpeningDoors", false),
				new XElement("OwnSideOnly", false),
				new XElement("Social",
					new XAttribute("Trigger", "nod"),
					new XAttribute("TargettedOnly", true),
					new XAttribute("Direction", false))).ToString());
		EnsureArtificialIntelligence(
			context,
			"SparPartner",
			"CombatEnd",
			new XElement("Definition",
				new XElement("WillAcceptTruce", alwaysTrue.Id),
				new XElement("WillAcceptTargetIncapacitated", alwaysTrue.Id),
				new XElement("OnOfferedTruce", 0),
				new XElement("OnTargetIncapacitated", 0),
				new XElement("OnNoNaturalTargets", 0)).ToString());
		EnsureArtificialIntelligence(
			context,
			"RandomWanderer",
			"Wanderer",
			new XElement("Definition",
				new XElement("FutureProg", alwaysTrue.Id),
				new XElement("WanderTimeDiceExpression", "1d40+100"),
				new XElement("TargetBody", 0),
				new XElement("TargetSpeed", 0),
				new XElement("EmoteText", new XCData(string.Empty))).ToString());
		EnsureArtificialIntelligence(
			context,
			"AggressiveToAllOtherSpecies",
			"Aggressor",
			new XElement("Definition",
				new XElement("WillAttackProg", aggressorWillAttack.Id),
				new XElement("EngageDelayDiceExpression", "1d200+200"),
				new XElement("EngageEmote", new XCData("@ move|moves aggressively towards $1"))).ToString());
		EnsureArtificialIntelligence(
			context,
			"RescueClanBrothers",
			"Rescuer",
			new XElement("Definition",
				new XElement("IsFriendProg", rescuerWillRescue.Id)).ToString());
		EnsureArtificialIntelligence(
			context,
			"VerminScavenge",
			"Scavenge",
			new XElement("Definition",
				new XElement("WillScavengeItemProg", verminWillScavenge.Id),
				new XElement("OnScavengeItemProg", verminOnScavenge.Id),
				new XElement("ScavengeDelayDiceExpression", "1d30+30")).ToString());
		EnsureArtificialIntelligence(
			context,
			"TrackingAggressiveToAllOtherSpecies",
			"TrackingAggressor",
			new XElement("Definition",
				new XElement("WillAttackProg", aggressorWillAttack.Id),
				new XElement("EngageDelayDiceExpression", "1d200+200"),
				new XElement("EngageEmote", new XCData("@ move|moves aggressively towards $1")),
				new XElement("MaximumRange", 2),
				new XElement("PathingEnabledProg", alwaysTrue.Id),
				new XElement("OpenDoors", false),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", false),
				new XElement("CloseDoorsBehind", false),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", false)).ToString());
		EnsureArtificialIntelligence(
			context,
			"BasicSelfCare",
			"SelfCare",
			new XElement("Definition",
				new XElement("BindingDelayDiceExpression", "3000+1d2000"),
				new XElement("BleedingEmoteDelayDiceExpression", "3000+1d2000"),
				new XElement("BleedingEmote", new XCData(@"@ shout|shouts out, ""I'm bleeding!"""))).ToString());
		EnsureArtificialIntelligence(
			context,
			"ExampleArenaParticipant",
			"ArenaParticipant",
			new XElement("Definition",
				new XElement("UseAmbushMode", true),
				new XElement("HideDuringPreparation", true),
				new XElement("UseSubtleSneak", true),
				new XElement("EngageDelayDiceExpression", new XCData("1d200+200")),
				new XElement("EngageEmote", new XCData("@ stalk|stalks into position against $1")),
				new XElement("OpenDoors", true),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", false),
				new XElement("CloseDoorsBehind", false),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", false)).ToString());
		EnsureArtificialIntelligence(
			context,
			"ExampleArborealWanderer",
			"ArborealWanderer",
			new XElement("Definition",
				new XElement("WillWanderIntoCellProg", alwaysTrue.Id),
				new XElement("IsWanderingProg", alwaysTrue.Id),
				new XElement("AllowDescentProg", alwaysFalse.Id),
				new XElement("WanderTimeDiceExpression", new XCData("1d180+180")),
				new XElement("EmoteText", new XCData("@ spring|springs from branch to branch.")),
				new XElement("PreferredTreeLayer", (int)RoomLayer.HighInTrees),
				new XElement("SecondaryTreeLayer", (int)RoomLayer.InTrees),
				new XElement("OpenDoors", false),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", false),
				new XElement("CloseDoorsBehind", false),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", false)).ToString());
		EnsureArtificialIntelligence(
			context,
			"ExampleDenBuilder",
			"DenBuilder",
			new XElement("Definition",
				new XElement("DenCraftId", 0),
				new XElement("DenSiteProg", alwaysTrue.Id),
				new XElement("BuildEnabledProg", alwaysTrue.Id),
				new XElement("WillDefendDenProg", alwaysFalse.Id),
				new XElement("AnchorItemProg", 0),
				new XElement("OpenDoors", false),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", false),
				new XElement("CloseDoorsBehind", false),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", false)).ToString());
		EnsureArtificialIntelligence(
			context,
			"ExampleLairScavenger",
			"LairScavenger",
			new XElement("Definition",
				new XElement("WillScavengeItemProg", verminWillScavenge.Id),
				new XElement("ScavengingEnabledProg", alwaysTrue.Id),
				new XElement("HomeLocationProg", lairFallbackHome.Id),
				new XElement("OpenDoors", false),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", false),
				new XElement("CloseDoorsBehind", false),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", false)).ToString());

		context.SaveChanges();
	}

	internal void SeedAIExamplesForTesting(FuturemudDatabaseContext context)
	{
		SeedAIExamples(context, new List<string>());
	}

	private static void EnsureVariableDefinition(FuturemudDatabaseContext context, ProgVariableTypes ownerType,
		string property, ProgVariableTypes containedType)
	{
		var definition = context.VariableDefinitions.Local
			                 .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property) ??
		                 context.VariableDefinitions.AsEnumerable()
			                 .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property);
		if (definition is null)
		{
			definition = new VariableDefinition
			{
				OwnerType = (long)ownerType,
				Property = property,
				ContainedType = (long)containedType
			};
			context.VariableDefinitions.Add(definition);
			return;
		}

		definition.OwnerType = (long)ownerType;
		definition.Property = property;
		definition.ContainedType = (long)containedType;
	}

	private static void EnsureVariableDefault(FuturemudDatabaseContext context, ProgVariableTypes ownerType,
		string property, string defaultValue)
	{
		var variableDefault = context.VariableDefaults.Local
			                      .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property) ??
		                      context.VariableDefaults.AsEnumerable()
			                      .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property);
		if (variableDefault is null)
		{
			variableDefault = new VariableDefault
			{
				OwnerType = (long)ownerType,
				Property = property,
				DefaultValue = defaultValue
			};
			context.VariableDefaults.Add(variableDefault);
			return;
		}

		variableDefault.OwnerType = (long)ownerType;
		variableDefault.Property = property;
		variableDefault.DefaultValue = defaultValue;
	}

	private static FutureProg EnsureAiProg(FuturemudDatabaseContext context, string functionName, string subcategory,
		ProgVariableTypes returnType, string comment, string text,
		params (ProgVariableTypes Type, string Name)[] parameters)
	{
		return SeederRepeatabilityHelper.EnsureProg(
			context,
			functionName,
			"AI",
			subcategory,
			returnType,
			comment,
			text,
			true,
			false,
			FutureProgStaticType.NotStatic,
			parameters);
	}

	private static void EnsureArtificialIntelligence(FuturemudDatabaseContext context, string name, string type,
		string definition)
	{
		var ai = SeederRepeatabilityHelper.EnsureNamedEntity(
			context.ArtificialIntelligences,
			name,
			x => x.Name,
			() =>
			{
				var created = new ArtificialIntelligence();
				context.ArtificialIntelligences.Add(created);
				return created;
			});

		ai.Name = name;
		ai.Type = type;
		ai.Definition = definition;
	}

	private void SeedNewbieHints(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.NewPlayerHints.Any())
		{
			errors.Add("Detected that Newbie Hints were already installed. Did not seed any Newbie Hints.");
			return;
		}

		var index = 10000;

		void AddHint(string text, long? filterProgId = null, bool canRepeat = false)
		{
			context.NewPlayerHints.Add(new NewPlayerHint
			{
				Text = text,
				FilterProgId = filterProgId,
				Priority = index--,
				CanRepeat = canRepeat
			});
		}

		AddHint(@"You can use the #3help#0 command to get more information about the commands, the world, and many types of information.");
		AddHint(@"You can use the #3commands#0 command to get a list of all the commands that are available to you. In most cases, you can append #3?#0 to a command to get help about it, for example #3help ?#0 will show you the helpfile for the #3help#0 command.");
		AddHint(@"When describing command syntax, this engine uses #3<>#0 to show you text that you fill in, e.g. #3<target>#0 would be where you would put the keyword for something you're targeting.");
		AddHint(@"When describing command syntax, this engine uses #3[]#0 to show you that something is optional, e.g. #3[<target>]#0 would show you that supplying a target is optional, and you can leave it off.");
		AddHint(@"The #3score#0 command shows you many bits of key information about your character.");
		AddHint(@"The #3set#0 command allows you to edit both account settings and character settings. Use #3set#0 on its own to see your current settings, or #3set ?#0 to see help for the set command.");
		AddHint(@"The #3inventory#0 command allows you to see what you are carrying, wearing and wielding.");
		AddHint(@"The #3attributes#0 command allows you to see information about your attributes and their scores.");
		AddHint(@"The #3who#0 command shows you how many other players are currently online, as well as which game staff are online and available to assist you.");
		AddHint(@"If you need help from staff, you can use the #3petition#0 command. Petitions are shown to online staff, logged in a message board, and sent to their staff discord!");
		AddHint(@"You can use the #3notify#0 command to let people know that you're online and available for roleplay. Typically you would use this command when you're in a public place so that others could choose to have their characters also emerge into public for roleplay encounters. See #3notify ?#0 for information on how to use this command.");
		AddHint(@"There are numerous ways to roleplay with others: #3emote#0, #3say#0, #3tell#0, #3whisper#0, #3talk#0, and #3shout#0 are some of the key ones you should know about. See each of these command's respective helpfiles for more information.");
		AddHint(@"You can use the #3ooc#0 command to send an out of character message to everyone in your room, but use of this command is expected to be kept to a minimum.");
		AddHint(@"The #3look#0 command is used to look at the room you're in, as well as things and people that you can see. There are a few other extended uses of it like looking at graffiti, people's tattoos or scars, or inside things. See #3look ?#0 for more information and detailed syntax.");
		AddHint(@"The #3skills#0 command will show you which skills your character has and their current levels. You can alternatively use the #3skillcategories#0 command to see the same information, but with the skills grouped by category.");
		AddHint(@"You can use the #3skillevels#0 command to view the descriptors for a particular skill. Exact skill levels are typically hidden from you and you will instead see a descriptor representing a range.");
		AddHint(@"The #3survey#0 command will give you information about the location that you're currently in, such as terrain type, noise and light levels. You can also #3survey cover#0 to see available cover from ranged attacks in the location.");
		AddHint(@"There are several commands that let you see things in adjacent rooms. Having seen something with these commands is necessary before you can target things with a ranged attack.

The #3quickscan#0 (or #3qs#0) command is instant and can be used while moving, but only sees one room away and might miss details.
The #3scan#0 command takes time and cannot be used while moving, but sees up to two rooms away.
The #3longscan#0 command takes longer then scan but can potentially see much further, up to five rooms or more depending on your abilities.");
		AddHint("The #3point#0 command allows you to point out targets that you have seen with the various scan commands to other people in your room, so they can also target it.");
		AddHint("You can use the #3dub#0 command to remember a person or item with a custom keyword; for example, if you learn their name. You can use #3dubs#0 to see who and what you have dubbed. Some kinds of remotely-targeted commands like magical abilities require you to have dubbed your target first.");
		AddHint("You can use the #3alias#0 command to adopt a fake name that you may use in some contexts that would otherwise reveal your true name. You can use the #3names#0 command to see your real name and any aliases you have.");
		AddHint("You can use the #3introduce#0 command to mechanically let those present get a #3dub#0 on you with your currently adopted name. You should support the use of this command with some roleplay.");
		AddHint("The #3exits#0 command will show you what exits there are to leave the room you're currently in, and where they lead.");
		AddHint("You can use the #3count#0 command to count up the value of any money you are carrying on your person.");
		AddHint("The #3journal#0 command can be used to write in-character notes and observations for your character. Think of it like their mental diary of things they know and want to remember. Admins can view your journal and may use it to initiate plots for you.");
		AddHint("The #3plan#0 command allows you to set a short and a long term plan. These can remind you what you were up to between sessions, and staff can view these to help initiate plots.");
		AddHint("You can use the #3speed#0 command to set the speeds at which you will walk, crawl, swim, climb or even fly (if you're able).");
		AddHint("The #3socials#0 command will show you a list of special short-cut emotes that you can use to supplement your roleplay. The name of the social is the command to use it, and you can see more detailed information about it by using the #3social <which>#0 command.");
		AddHint("You can use the #3pmote#0 command to set a 'player emote' that others will see in your room description when they look at the room.");
		AddHint("You can use the #3omote#0 command to set an 'object emote' that others will see in a target item's room description when they look at the room.");
		AddHint("You can position yourself and items by using the #3position#0 command or one of its specific implementations (e.g. #3stand#0, #3kneel#0, #3sit#0, etc.). See #3position ?#0 for more information on this.");
	}
}







