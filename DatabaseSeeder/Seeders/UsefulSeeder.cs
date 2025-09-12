using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public class UsefulSeeder : IDatabaseSeeder
{
	public bool SafeToRunMoreThanOnce => true;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("ai", "Do you want to install some basic AIs?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.ArtificialIntelligences.All(x => x.Name != "CommandableOwner"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("ai2", "Do you want to install some further examples of AIs?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.ArtificialIntelligences.All(x => x.Name != "Rescuer"),
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
								"#DItem Package 1#F\n\nItem Package 1 includes some commonly used item component types, including a wide selection of containers, liquid containers, doors, locks, keys and basic writing implements.\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
								(context, questions) => context.GameItemComponentProtos.All(x => x.Name != "Container_Table"),
								(answer, context) =>
								{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("itemsp2",
				"#DItem Package 2#f\n\nItem Package 2 includes some further items such as insulation for clothing, components that let worn clothing hide or change characteristics (wigs, coloured contacts, etc), components that correct for myopia flaws, as well as identity obscurers (hoods, full helmets, niqabs, cloaks, etc.)\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
				(context, questions) => context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("itemsp3",
				"#DItem Package 3#f\n\nItem Package 3 includes some further useful items, such as destroyables, colour variables, further writing implements, tables and chairs, ranged covers, medical items, prosthetic limbs, and dice.\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
				(context, questions) => context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("itemsp4",
				"#DItem Package 4#f\n\nItem Package 4 includes some further useful items, such as torches and lanterns, repair kits, water sources and smokeable tobacco.\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
				(context, questions) => context.GameItemComponentProtos.All(x => x.Name != "Torch_Infinite"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("modernitems",
				"[Not Yet Implemented] Do you want to install some common modern setting item component types like batteries and power plugs?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => false,
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("tags",
				"Do you want to install pre-made tags for use with items, crafts and projects? The main reason not to do this is if you are planning on an implementation that substantially differs from the one that comes with this seeder.\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => true,
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
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		var errors = new List<string>();

		if (questionAnswers["tags"].EqualToAny("yes", "y"))
		{
			SeedTags(context, errors);
		}

		if (questionAnswers["ai"].EqualToAny("yes", "y")) SeedAIPart1(context, errors);

		if (questionAnswers["ai2"].EqualToAny("yes", "y")) SeedAIPart2(context, errors);

		if (questionAnswers["items"].EqualToAny("yes", "y")) SeedItemsPart1(context, questionAnswers, errors);

		if (questionAnswers["itemsp2"].EqualToAny("yes", "y")) SeedItemsPart2(context, questionAnswers, errors);

		if (questionAnswers["itemsp3"].EqualToAny("yes", "y")) SeedItemsPart3(context, questionAnswers, errors);

		if (questionAnswers["itemsp4"].EqualToAny("yes", "y")) SeedItemsPart4(context, questionAnswers, errors);

		if (questionAnswers["modernitems"].EqualToAny("yes", "y")) SeedModernItems(context, errors);

		if (questionAnswers["terrain"].EqualToAny("yes", "y")) SeedTerrain(context, errors);

		if (questionAnswers["covers"].EqualToAny("yes", "y")) SeedRangedCovers(context, errors);

		if (questionAnswers["autobuilder"].EqualToAny("yes", "y"))
		{
			SeedTerrainAutobuilder(context, questionAnswers, errors);
		}

		context.Database.CommitTransaction();

		if (errors.Count == 0) return "The operation completed successfully.";

		return
			$"The operation completed with the following errors or warnings:\n\n{errors.ListToCommaSeparatedValues("\n")}";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (!context.ArtificialIntelligences.All(x => x.Name != "CommandableOwner") &&
			!context.ArtificialIntelligences.All(x => x.Name != "Rescuer") &&
			context.Terrains.Count() > 1 &&
			!context.GameItemComponentProtos.All(x => x.Name != "Container_Table") &&
			!context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor") &&
			!context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc") &&
			!context.GameItemComponentProtos.All(x => x.Name != "Torch_Infinite") &&
			!context.Tags.All(x => x.Name != "Functions"))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		if (context.ArtificialIntelligences.All(x => x.Name != "CommandableOwner") ||
			context.ArtificialIntelligences.All(x => x.Name != "Rescuer") ||
			context.Terrains.Count() <= 1 ||
			context.GameItemComponentProtos.All(x => x.Name != "Container_Table") ||
			context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor") ||
			context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc") ||
			context.GameItemComponentProtos.All(x => x.Name != "Torch_Infinite") ||
			context.Tags.All(x => x.Name != "Functions"))
		{
			return ShouldSeedResult.ExtraPackagesAvailable;
		}

		return ShouldSeedResult.ReadyToInstall;
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
		_context.GameItemComponentProtos.Add(component);
		return component;
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
		// Not yet implemented
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

		context.GameItemComponentProtos.Add(component);
		return component;
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
<FuelCapacity>{fuelCapacity}</FuelCapacity>
<RequiresIgnitionSource>{requiresIgnitionSource.ToString().ToLower()}</RequiresIgnitionSource>
<LightEmote><![CDATA[{lightEmote}]]></LightEmote>
<ExtinguishEmote><![CDATA[{extinguishEmote}]]></ExtinguishEmote>
<TenPercentFuelEcho><![CDATA[{tenPercentFuelEcho}]]></TenPercentFuelEcho>
<FuelExpendedEcho><![CDATA[{fuelExpendedEcho}]]></FuelExpendedEcho>
<LiquidFuel>{liquidFuelId}</LiquidFuel>
<FuelPerSecond>{fuelPerSecond}</FuelPerSecond>
</Definition>";
		return CreateComponent(context, ref nextId, account, now, "Lantern", name, description,
		definition);
	}

	private GameItemComponentProto CreateWaterSourceComponent(FuturemudDatabaseContext context,
	ref long nextId, Account account, DateTime now, string name, string description,
	double liquidCapacity, long defaultLiquidId, double refillRate, bool useOnOffForRefill)
	{
		var definition = $"<Definition LiquidCapacity=\"{liquidCapacity}\" Closable=\"false\" Transparent=\"false\" OnceOnly=\"false\" DefaultLiquid=\"{defaultLiquidId}\" RefillRate=\"{refillRate}\" UseOnOffForRefill=\"{useOnOffForRefill.ToString().ToLower()}\" RefillingProg=\"0\" CanBeEmptiedWhenInRoom=\"false\" />";
		return CreateComponent(context, ref nextId, account, now, "WaterSource", name, description,
		definition);
	}

	private void SeedItemsPart1(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
			ICollection<string> errors)
	{
		if (context.GameItemComponentProtos.Any(x => x.Name == "Container_Table"))
		{
			errors.Add("Detected that items were already installed. Did not seed any items.");
			return;
		}

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
			context.GameItemComponentProtos.Add(component);
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
		if (context.GameItemComponentProtos.Any(x => x.Name == "Insulation_Minor"))
		{
			errors.Add("Detected that items were already installed. Did not seed any items from package 2.");
			return;
		}

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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion
	}

	private void SeedItemsPart3(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
	{
		if (context.GameItemComponentProtos.Any(x => x.Name == "Destroyable_Misc"))
		{
			errors.Add("Detected that items were already installed. Did not seed any items from package 3.");
			return;
		}

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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion

		#region Ranged Cover

		var unflippedTable = new RangedCover
		{
			Name = "Upright Table",
			CoverType = 1,
			CoverExtent = 1,
			HighestPositionState = 1,
			DescriptionString = "using $?0|$0|a table|$ as cover",
			ActionDescriptionString =
				"@ move|moves behind $?1|$1|a nearby table|$ and use|uses it to obscure &0's profile",
			MaximumSimultaneousCovers = 3,
			CoverStaysWhileMoving = false
		};
		context.RangedCovers.Add(unflippedTable);
		context.SaveChanges();

		var flippedTable = new RangedCover
		{
			Name = "Overturned Table",
			CoverType = 1,
			CoverExtent = 2,
			HighestPositionState = 3,
			DescriptionString = "hiding behind $?0|$0|an overturned table|$ as cover",
			ActionDescriptionString =
				"@ duck|ducks behind $?1|$1|a nearby overturned table|$ and begin|begins to use it as cover",
			MaximumSimultaneousCovers = 3,
			CoverStaysWhileMoving = false
		};
		context.RangedCovers.Add(flippedTable);
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Uneven Ground",
			CoverType = 0,
			CoverExtent = 1,
			HighestPositionState = 6,
			DescriptionString = "prone, using the uneven ground as cover",
			ActionDescriptionString = "@ go|goes prone and begin|begins to use the uneven ground as cover",
			MaximumSimultaneousCovers = 0,
			CoverStaysWhileMoving = true
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Smoke",
			CoverType = 0,
			CoverExtent = 2,
			HighestPositionState = 1,
			DescriptionString = "obscured by $?0|$0|the smoke|$",
			ActionDescriptionString = "@ move|moves into $?1|$1|the smoke|$ and uses it to obscure &0's form",
			MaximumSimultaneousCovers = 0,
			CoverStaysWhileMoving = true
		});
		context.SaveChanges();

		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Smoke",
			Description = @"Turns an item into ranged cover of type ""Smoke"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Smoke").Id}</Cover>
 </Definition>"
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Sandbag",
			CoverType = 1,
			CoverExtent = 2,
			HighestPositionState = 3,
			DescriptionString = "hiding behind $?0|$0|a sandbag barricade|$, using it as cover",
			ActionDescriptionString =
				"@ take|takes position behind $?1|$1|a sandbag barricade|$ and begin|begins to use it as cover",
			MaximumSimultaneousCovers = 3,
			CoverStaysWhileMoving = false
		});
		context.SaveChanges();

		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Sandbag",
			Description = @"Turns an item into ranged cover of type ""Sandbag"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Sandbag").Id}</Cover>
 </Definition>"
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Tree",
			CoverType = 1,
			CoverExtent = 2,
			HighestPositionState = 1,
			DescriptionString = "hiding behind $?0|$0|a tree|$ for cover",
			ActionDescriptionString = "@ slip|slips behind $?1|$1|a tree|$ and use|uses it to protect &0's vital areas",
			MaximumSimultaneousCovers = 1,
			CoverStaysWhileMoving = false
		});
		context.SaveChanges();
		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Tree",
			Description = @"Turns an item into ranged cover of type ""Tree"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Tree").Id}</Cover>
 </Definition>"
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Bushes",
			CoverType = 0,
			CoverExtent = 2,
			HighestPositionState = 1,
			DescriptionString = "hiding in $?0|$0|the bushes|$ for cover",
			ActionDescriptionString =
				"@ take|takes position behind $?1|$1|a bush|$ and use|uses it to obscure &0's profile",
			MaximumSimultaneousCovers = 2,
			CoverStaysWhileMoving = false
		});
		context.SaveChanges();

		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Bushes",
			Description = @"Turns an item into ranged cover of type ""Bushes"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Bushes").Id}</Cover>
 </Definition>"
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Long Grass",
			CoverType = 0,
			CoverExtent = 2,
			HighestPositionState = 6,
			DescriptionString = "hiding in $?0|$0|the long grass|$ for cover",
			ActionDescriptionString =
				"@ take|takes position in $?1|$1|the long grass|$ and use|uses it to obscure &0's profile",
			MaximumSimultaneousCovers = 2,
			CoverStaysWhileMoving = true
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Doorways",
			CoverType = 0,
			CoverExtent = 1,
			HighestPositionState = 1,
			DescriptionString = "using a doorway as cover",
			ActionDescriptionString = "@ duck|ducks into a doorway and begin|begins to use it as cover",
			MaximumSimultaneousCovers = 0,
			CoverStaysWhileMoving = false
		});
		context.SaveChanges();

		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Doorways",
			Description = @"Turns an item into ranged cover of type ""Doorways"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Doorways").Id}</Cover>
 </Definition>"
		});
		context.SaveChanges();

		context.RangedCovers.Add(new RangedCover
		{
			Name = "Rubble",
			CoverType = 1,
			CoverExtent = 2,
			HighestPositionState = 12,
			DescriptionString = "slumped up against $?0|$0|some rubble|$ as cover",
			ActionDescriptionString =
				"@ slump|slumps up against $?1|$1|some rubble|$ and begin|begins to use it as cover",
			MaximumSimultaneousCovers = 2,
			CoverStaysWhileMoving = false
		});
		context.SaveChanges();

		context.GameItemComponentProtos.Add(new GameItemComponentProto
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
			Type = "Cover",
			Name = "Cover_Rubble",
			Description = @"Turns an item into ranged cover of type ""Rubble"".",
			Definition = @$"<Definition>
   <Cover>{context.RangedCovers.First(x => x.Name == "Rubble").Id}</Cover>
 </Definition>"
		});
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
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
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion
	}

	private void SeedItemsPart4(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
	{
		if (context.GameItemComponentProtos.Any(x => x.Name == "Torch_Infinite"))
		{
			errors.Add("Detected that items were already installed. Did not seed any items from package 4.");
			return;
		}

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
			Type = "Torch",
			Name = "SignalFire",
			Description = "Turns an item into a bright signal fire that burns for 3 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>500</IlluminationProvided>
   <SecondsOfFuel>10800</SecondsOfFuel>
   <RequiresIgnitionSource>true</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "Match",
			Description = "Turns an item into a match that burns dimly for only a few seconds.",
			Definition = @"<Definition>
   <IlluminationProvided>5</IlluminationProvided>
   <SecondsOfFuel>20</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "Candle",
			Description = "Turns an item into a candle that burns dimly for 12 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>5</IlluminationProvided>
   <SecondsOfFuel>43200</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "Candle_Long",
			Description = "Turns an item into a candle that burns very dimly for 48 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>3</IlluminationProvided>
   <SecondsOfFuel>172800</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "Candle_Bright",
			Description = "Turns an item into a candle that burns a little dimly for 6 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>8</IlluminationProvided>
   <SecondsOfFuel>21600</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "Candle_Infinite",
			Description = "Turns an item into an ever-burning candle.",
			Definition = @"<Definition>
   <IlluminationProvided>5</IlluminationProvided>
   <SecondsOfFuel>-1</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "Torch",
			Name = "BrightCandle_Infinite",
			Description = "Turns an item into an ever-burning bright candle.",
			Definition = @"<Definition>
   <IlluminationProvided>8</IlluminationProvided>
   <SecondsOfFuel>-1</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights on $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to flicker as it has almost totally burned down]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely burned out]]></FuelExpendedEcho>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

		var fuelLiquid = context.Liquids.FirstOrDefault(x => x.Name == "fuel") ??
			context.Liquids.First(x => x.Name == "water");

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
			Type = "Lantern",
			Name = "Lantern",
			Description = "Turns an item into a lantern that burns any flammable fuel.",
			Definition = @$"<Definition>
   <IlluminationProvided>500</IlluminationProvided>
   <FuelCapacity>0.2273046</FuelCapacity>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ extinguish|extinguishes $1]]></ExtinguishEmote>
   <TenPercentFuelEcho><![CDATA[$0 begin|begins to splutter as the fuel runs low]]></TenPercentFuelEcho>
   <FuelExpendedEcho><![CDATA[$0 have|has completely exhausted its fuel]]></FuelExpendedEcho>
   <LiquidFuel>{fuelLiquid.Id}</LiquidFuel>
   <FuelPerSecond>0.000007892522</FuelPerSecond>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);

		context.SaveChanges();
		#endregion

		#region Water Sources
		var waterLiquid = context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_WaterSource",
			Description = "Turns an item into a self-refilling source of water.",
			Definition = @$"<Definition LiquidCapacity=""1000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{waterLiquid.Id}"" RefillRate=""0.8333333333333334"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		var lakeLiquid = context.Liquids.FirstOrDefault(x => x.Name == "lake water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_LakeWaterSource",
			Description = "Turns an item into a self-refilling source of lake water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{lakeLiquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		var springLiquid = context.Liquids.FirstOrDefault(x => x.Name == "spring water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_SpringWaterSource",
			Description = "Turns an item into a self-refilling source of spring water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{springLiquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		var riverLiquid = context.Liquids.FirstOrDefault(x => x.Name == "river water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_RiverWaterSource",
			Description = "Turns an item into a self-refilling source of river water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{riverLiquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		var liquid = context.Liquids.FirstOrDefault(x => x.Name == "swamp water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_SwampWaterSource",
			Description = "Turns an item into a self-refilling source of swamp water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		liquid = context.Liquids.FirstOrDefault(x => x.Name == "brackish water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_BrackishWaterSource",
			Description = "Turns an item into a self-refilling source of brackish water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		liquid = context.Liquids.FirstOrDefault(x => x.Name == "salt water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Infinite_SaltWaterSource",
			Description = "Turns an item into a self-refilling source of salt water.",
			Definition = @$"<Definition LiquidCapacity=""100000000"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""1000"" UseOnOffForRefill=""false"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

		liquid = context.Liquids.FirstOrDefault(x => x.Name == "tap water") ??
			context.Liquids.First(x => x.Name == "water");
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
			Type = "WaterSource",
			Name = "Sink_5L",
			Description = "Turns an item into a 5L sink that can be filled up.",
			Definition = @$"<Definition LiquidCapacity=""5"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""0.8333333333333334"" UseOnOffForRefill=""true"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "WaterSource",
			Name = "Sink_20L",
			Description = "Turns an item into a 20L sink that can be filled up.",
			Definition = @$"<Definition LiquidCapacity=""20"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""0.8333333333333334"" UseOnOffForRefill=""true"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "WaterSource",
			Name = "Sink_50L",
			Description = "Turns an item into a 50L sink that can be filled up.",
			Definition = @$"<Definition LiquidCapacity=""50"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""0.8333333333333334"" UseOnOffForRefill=""true"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);

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
			Type = "WaterSource",
			Name = "Bathtub",
			Description = "Turns an item into a 500L bathtub that can be filled up.",
			Definition = @$"<Definition LiquidCapacity=""500"" Closable=""false"" Transparent=""false"" OnceOnly=""false"" DefaultLiquid=""{liquid.Id}"" RefillRate=""0.8333333333333334"" UseOnOffForRefill=""true"" RefillingProg=""0"" CanBeEmptiedWhenInRoom=""false"" />"
		};
		context.GameItemComponentProtos.Add(component);
		#endregion

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
			context.GameItemComponentProtos.Add(repairComponent);
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

		#region Smokeables
		context.VariableDefinitions.Add(new VariableDefinition
		{
			ContainedType = (long)ProgVariableTypes.DateTime,
			OwnerType = 8,
			Property = "nicotineuntil"
		});
		context.VariableDefaults.Add(new VariableDefault
		{
			OwnerType = 8,
			Property = "nicotineuntil",
			DefaultValue = "<var>01/01/0001 00:00:00</var>"
		});
		context.SaveChanges();
		var smokeProg = new FutureProg
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
		context.GameItemComponentProtos.Add(component);
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

	private void SeedAIPart1(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.ArtificialIntelligences.Any(x => x.Name == "CommandableOwner"))
		{
			errors.Add("Detected that Part1 AIs were already installed. Did not seed any AIs.");
			return;
		}

		context.VariableDefinitions.Add(new VariableDefinition
		{
			ContainedType = (long)ProgVariableTypes.Number,
			OwnerType = (long)ProgVariableTypes.Character,
			Property = "npcownerid"
		});
		context.VariableDefaults.Add(new VariableDefault
		{
			OwnerType = (long)ProgVariableTypes.Character,
			Property = "npcownerid",
			DefaultValue = "<var>0</var>"
		});
		var ownerProg = new FutureProg
		{
			FunctionText = @"var ownerid as number
ownerid = ifnull(getregister(@ch, ""npcownerid""),0)
return @ownerid == @tch.Id",
			FunctionName = "IsOwnerCanCommand",
			Category = "AI",
			Subcategory = "Commands",
			FunctionComment = "Determines if the character has been set as the owner of an NPC",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)ProgVariableTypes.Text
		});
		context.FutureProgs.Add(ownerProg);

		var cantCommandProg = new FutureProg
		{
			FunctionText =
				@"return ""You are not the owner of "" + HowSeen(@ch, @tch, false, true) + "" and so you cannot issue commands.""",
			FunctionName = "WhyCantCommandNPCOwnerAI",
			Category = "AI",
			Subcategory = "Commands",
			FunctionComment = "Returns an error message when a player cannot command an NPC they do not own",
			ReturnType = (long)ProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)ProgVariableTypes.Text
		});
		context.FutureProgs.Add(cantCommandProg);
		context.SaveChanges();

		var ai = new ArtificialIntelligence
		{
			Name = "CommandableOwner",
			Type = "Commandable",
			Definition = @$"<Definition>
   <CanCommandProg>{ownerProg.Id}</CanCommandProg>
   <WhyCannotCommandProg>{cantCommandProg.Id}</WhyCannotCommandProg>
   <CommandIssuedEmote>You issue the following command to $1: {{0}}</CommandIssuedEmote>
   <BannedCommands>
	 <BannedCommand>ignoreforce</BannedCommand>
	 <BannedCommand>return</BannedCommand>
   </BannedCommands>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		var isClanBrotherProg = new FutureProg
		{
			FunctionText = @"foreach (clan in @tch.clans)
	if (outranks(@tch, @ch, @clan))
		return true
	end if
end foreach
return false",
			FunctionName = "OutranksCanCommand",
			Category = "AI",
			Subcategory = "Commands",
			FunctionComment =
				"Determines if the character outranks the NPC in any clan and can therefore command them.\nNote: When using this AI for real, you might want to restrict it to SPECIFIC clans in your world.",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)ProgVariableTypes.Text
		});
		context.FutureProgs.Add(isClanBrotherProg);

		var cantCommandProgClanBrother = new FutureProg
		{
			FunctionText = @"return ""You do not outrank "" + HowSeen(@ch, @tch, false, true) + "" in any clans.""",
			FunctionName = "WhyCantCommandNPCClanOutranks",
			Category = "AI",
			Subcategory = "Commands",
			FunctionComment = "Returns an error message when a player cannot command an NPC they do not outrank",
			ReturnType = (long)ProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)ProgVariableTypes.Text
		});
		context.FutureProgs.Add(cantCommandProgClanBrother);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "CommandableClanOutranks",
			Type = "Commandable",
			Definition = @$"<Definition>
   <CanCommandProg>{isClanBrotherProg.Id}</CanCommandProg>
   <WhyCannotCommandProg>{cantCommandProgClanBrother.Id}</WhyCannotCommandProg>
   <CommandIssuedEmote>You issue the following command to $1: {{0}}</CommandIssuedEmote>
   <BannedCommands>
	 <BannedCommand>ignoreforce</BannedCommand>
	 <BannedCommand>return</BannedCommand>
   </BannedCommands>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		var doorguardWillOpen = new FutureProg
		{
			FunctionText = @"return isclanbrother(@guard, @ch)",
			FunctionName = "DoorguardWillOpenDoor",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment =
				"Determines whether a doorguard will open a door for a person.\nNote: You may want to restrict this to particular clans when you write your own AIs",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardWillOpen);

		var doorguardDelay = new FutureProg
		{
			FunctionText = @"return 40+random(1,40)",
			FunctionName = "DoorguardActionDelay",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment =
				"A delay in milliseconds between the action that triggers the doorguard and them taking the action",
			ReturnType = (long)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardDelay);

		var doorguardCloseDelay = new FutureProg
		{
			FunctionText = @"return 10000",
			FunctionName = "DoorguardCloseDelay",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment = "A delay in milliseconds between opening the door and closing the door",
			ReturnType = (long)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardCloseDelay);

		var doorguardOpenDoor = new FutureProg
		{
			FunctionText = @"// Assumes doorguard has a key in their inventory
force @guard (""emote move|moves to open the door for ~"" + bestkeyword(@guard, @ch))
force @guard (""unlock "" + @exit.keyword)
force @guard (""open "" + @exit.keyword)

// If you wanted to just open the door without worrying about keys you could do the following:
// setlocked(@exit.door, false, false)
// setopen(@exit.door, true, false)",
			FunctionName = "DoorguardOpenDoor",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment =
				"The actual action for the doorguard to take when opening the door. Customise for locks, emotes, or even alternate methods of opening",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardOpenDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardOpenDoor,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardOpenDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardOpenDoor,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardOpenDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardOpenDoor,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardOpenDoor);

		var doorguardCloseDoor = new FutureProg
		{
			FunctionText = @"// Assumes doorguard has a key in their inventory
force @guard (""emote move|moves to close the door"")
force @guard (""close "" + @exit.keyword)
force @guard (""lock "" + @exit.keyword)

// If you wanted to just close the door without worrying about keys you could do the following:
// setopen(@exit.door, false, false)
// setlocked(@exit.door, true, false)",
			FunctionName = "DoorguardCloseDoor",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment =
				"The actual action for the doorguard to take when closing the door. Customise for locks, emotes, or even alternate methods of closing",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardCloseDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDoor,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardCloseDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDoor,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorguardCloseDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDoor,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardCloseDoor);

		var doorGuardWontOpen = new FutureProg
		{
			FunctionText = @"// We need to know whether they knocked or nodded
if (isnull(@exit) or @exit.Origin == @guard.Location)
	force @guard (""tell "" + bestkeyword(@guard, @ch) + "" I'm not allowed to let you through"")
else
	force @guard (""yell I'm not allowed to let you through"")
end if",
			FunctionName = "DoorguardWontOpen",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment = "An action for the doorguard to take if someone nods/knocks but they can't let them in",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorGuardWontOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWontOpen,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorGuardWontOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWontOpen,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorGuardWontOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWontOpen,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorGuardWontOpen);

		var doorGuardWitnessStop = new FutureProg
		{
			FunctionText = @"if (@DoorguardWillOpenDoor(@guard, @ch, @exit))
	force @guard (""tell "" + bestkeyword(@guard, @ch) + "" Give me a nod and I'll open the door for you"")
end if",
			FunctionName = "DoorguardWitnessStop",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment = "An action for the doorguard to take if someone walks into a closed door",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorGuardWitnessStop.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWitnessStop,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorGuardWitnessStop.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWitnessStop,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		doorGuardWitnessStop.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWitnessStop,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)ProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorGuardWitnessStop);

		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "BasicDoorguard",
			Type = "Doorguard",
			Definition = @$"<Definition>
	<WillOpenDoorForProg>{doorguardWillOpen.Id}</WillOpenDoorForProg>
	<WontOpenDoorForActionProg>{doorGuardWontOpen.Id}</WontOpenDoorForActionProg>
	<OpenDoorActionProg>{doorguardOpenDoor.Id}</OpenDoorActionProg>
	<CloseDoorActionProg>{doorguardCloseDoor.Id}</CloseDoorActionProg>
	<BaseDelayProg>{doorguardDelay.Id}</BaseDelayProg>
	<OpenCloseDelayProg>{doorguardCloseDelay.Id}</OpenCloseDelayProg>
	<OnWitnessDoorStopProg>{doorGuardWitnessStop.Id}</OnWitnessDoorStopProg>
	<RespectGameRulesForOpeningDoors>false</RespectGameRulesForOpeningDoors>
	<OwnSideOnly>false</OwnSideOnly>
	<Social Trigger=""nod"" TargettedOnly=""true"" Direction=""false""/>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "SparPartner",
			Type = "CombatEnd",
			Definition = @$"<Definition>
	<WillAcceptTruce>{context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id}</WillAcceptTruce>
	<WillAcceptTargetIncapacitated>{context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id}</WillAcceptTargetIncapacitated>
	<OnOfferedTruce>0</OnOfferedTruce>
	<OnTargetIncapacitated>0</OnTargetIncapacitated>
	<OnNoNaturalTargets>0</OnNoNaturalTargets>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "RandomWanderer",
			Type = "Wanderer",
			Definition = @$"<Definition>  
	<FutureProg>{context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id}</FutureProg>  
	<WanderTimeDiceExpression>1d40+100</WanderTimeDiceExpression>
	<TargetBody>0</TargetBody>
	<TargetSpeed>0</TargetSpeed>
	<EmoteText><![CDATA[]]></EmoteText>
</Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		var aggressorWillAttack = new FutureProg
		{
			FunctionText = @"return @ch.Race != @tch.Race",
			FunctionName = "TargetIsOtherRace",
			Category = "AI",
			Subcategory = "Aggressor",
			FunctionComment = "Determines whether the aggressor will attack someone",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		aggressorWillAttack.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = aggressorWillAttack,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		aggressorWillAttack.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = aggressorWillAttack,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		context.FutureProgs.Add(aggressorWillAttack);

		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "AggressiveToAllOtherSpecies",
			Type = "Aggressor",
			Definition = @$"<Definition>
   <WillAttackProg>{aggressorWillAttack.Id}</WillAttackProg>
   <EngageDelayDiceExpression>1d200+200</EngageDelayDiceExpression>
   <EngageEmote><![CDATA[@ move|moves aggressively towards $1]]></EngageEmote>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();
	}

	private void SeedAIPart2(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.ArtificialIntelligences.Any(x => x.Name == "Rescuer"))
		{
			errors.Add("Detected that Part2 AIs were already installed. Did not seed any AIs.");
			return;
		}

		var rescuerWillRescue = new FutureProg
		{
			FunctionText = @"return isclanbrother(@rescuer, @target)",
			FunctionName = "RescuerWillRescue",
			Category = "AI",
			Subcategory = "Combat",
			FunctionComment =
				"Determines whether a rescuer will rescue someone who is being attacked",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		rescuerWillRescue.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = rescuerWillRescue,
			ParameterIndex = 0,
			ParameterName = "rescuer",
			ParameterType = (long)ProgVariableTypes.Character
		});
		rescuerWillRescue.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = rescuerWillRescue,
			ParameterIndex = 1,
			ParameterName = "target",
			ParameterType = (long)ProgVariableTypes.Character
		});
		context.FutureProgs.Add(rescuerWillRescue);
		context.SaveChanges();

		var ai = new ArtificialIntelligence
		{
			Name = "RescueClanBrothers",
			Type = "Rescuer",
			Definition = @$"<AI><IsFriendProg>{rescuerWillRescue.Id}</IsFriendProg></AI>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		var verminWillScavenge = new FutureProg
		{
			FunctionText = @"return @item.isholdable and (@item.isfood or @item.iscorpse)",
			FunctionName = "VerminWillScavenge",
			Category = "AI",
			Subcategory = "Vermin",
			FunctionComment =
				"Determines whether a vermin AI will scavenge an item",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		verminWillScavenge.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = verminWillScavenge,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		verminWillScavenge.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = verminWillScavenge,
			ParameterIndex = 1,
			ParameterName = "item",
			ParameterType = (long)ProgVariableTypes.Item
		});
		context.FutureProgs.Add(verminWillScavenge);

		var verminOnScavenge = new FutureProg
		{
			FunctionText = @"force @ch (""eat "" + BestKeyword(@ch, @item))",
			FunctionName = "VerminOnScavenge",
			Category = "AI",
			Subcategory = "Vermin",
			FunctionComment =
				"Fires when a scavenger AI decides to scavenge an item",
			ReturnType = (long)ProgVariableTypes.Void,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		verminOnScavenge.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = verminOnScavenge,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Character
		});
		verminOnScavenge.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = verminOnScavenge,
			ParameterIndex = 1,
			ParameterName = "item",
			ParameterType = (long)ProgVariableTypes.Item
		});
		context.FutureProgs.Add(verminOnScavenge);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "VerminScavenge",
			Type = "Scavenge",
			Definition = @$"<Definition>
  <WillScavengeItemProg>{verminWillScavenge.Id}</WillScavengeItemProg>
  <OnScavengeItemProg>{verminOnScavenge.Id}</OnScavengeItemProg>
  <ScavengeDelayDiceExpression>1d30+30</ScavengeDelayDiceExpression>
</Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "TrackingAggressiveToAllOtherSpecies",
			Type = "TrackingAggressor",
			Definition = @$"<Definition>
   <WillAttackProg>{context.FutureProgs.First(x => x.FunctionName == "TargetIsOtherRace").Id}</WillAttackProg>
   <EngageDelayDiceExpression>1d200+200</EngageDelayDiceExpression>
   <EngageEmote><![CDATA[@ move|moves aggressively towards $1]]></EngageEmote>
   <MaximumRange>2</MaximumRange>
   <PathingEnabledProg>{context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id}</PathingEnabledProg>
   <OpenDoors>false</OpenDoors>
   <UseKeys>false</UseKeys>
   <SmashLockedDoors>false</SmashLockedDoors>
   <CloseDoorsBehind>false</CloseDoorsBehind>
   <UseDoorguards>false</UseDoorguards>
   <MoveEvenIfObstructionInWay>false</MoveEvenIfObstructionInWay>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "BasicSelfCare",
			Type = "SelfCare",
			Definition = @$"<definition>
	<BindingDelayDiceExpression>3000+1d2000</BindingDelayDiceExpression>
	<BleedingEmoteDelayDiceExpression>3000+1d2000</BleedingEmoteDelayDiceExpression>
	<BleedingEmote><![CDATA[@ shout|shouts out, ""I'm bleeding!""]]></BleedingEmote>
</definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();
	}


	private DictionaryWithDefault<string, MudSharp.Models.Tag> _tags = new(StringComparer.OrdinalIgnoreCase);

	private void AddTag(FuturemudDatabaseContext context, string name, string parent)
	{
		if (_tags.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			return;
		}

		var tag = new MudSharp.Models.Tag
		{
			Name = name,
			Parent = _tags[parent]
		};
		_tags[name] = tag;
		context.Tags.Add(tag);
	}

	private void SeedTags(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		_tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		// Terrain
		AddTag(context, "Terrain", "");
		AddTag(context, "Wild", "Terrain");
		AddTag(context, "Human Influenced", "Terrain");
		AddTag(context, "Urban", "Human Influenced");
		AddTag(context, "Rural", "Human Influenced");
		AddTag(context, "Public", "Urban");
		AddTag(context, "Private", "Urban");
		AddTag(context, "Commercial", "Urban");
		AddTag(context, "Residential", "Urban");
		AddTag(context, "Administrative", "Urban");
		AddTag(context, "Industrial", "Urban");
		AddTag(context, "Natural", "Urban");
		AddTag(context, "Diggable Soil", "Terrain");
		AddTag(context, "Foragable Clay", "Terrain");
		AddTag(context, "Foragable Sand", "Terrain");
		AddTag(context, "Terrestrial", "Wild");
		AddTag(context, "Riparian", "Wild");
		AddTag(context, "Littoral", "Wild");
		AddTag(context, "Aquatic", "Wild");

		// Eras
		AddTag(context, "Era", "");
		AddTag(context, "Stone Age Era", "Era");
		AddTag(context, "Bronze Age Era", "Era");
		AddTag(context, "Iron Age Era", "Era");
		AddTag(context, "Antiquity Era", "Era");
		AddTag(context, "Dark Ages Era", "Era");
		AddTag(context, "Medieval Era", "Era");
		AddTag(context, "Renaissance Era", "Era");
		AddTag(context, "Colonial Era", "Era");
		AddTag(context, "Industrial Era", "Era");
		AddTag(context, "Modern Era", "Era");
		AddTag(context, "Nuclear Era", "Era");
		AddTag(context, "Information Age Era", "Era");
		AddTag(context, "Near Future Era", "Era");
		AddTag(context, "Far Future Era", "Era");

		// Functions
		AddTag(context, "Functions", "");

		AddTag(context, "Material Functions", "Functions");
		AddTag(context, "Kindling", "Material Functions");
		AddTag(context, "Firewood", "Material Functions");
		AddTag(context, "Meltable", "Material Functions");
		AddTag(context, "Salvagable Fabric", "Material Functions");
		AddTag(context, "Commoditisable", "Material Functions");
		AddTag(context, "Padding", "Material Functions");
		AddTag(context, "Hot Fire", "Material Functions");
		AddTag(context, "String", "Material Functions");
		AddTag(context, "Debris", "Material Functions");
		AddTag(context, "Tanning Agent", "Material Functions");
		AddTag(context, "Ore Deposit", "Material Functions");
		AddTag(context, "Ignition Source", "Material Functions");
		AddTag(context, "Fire", "Material Functions");
		AddTag(context, "Musket Wadding", "Material Functions");

		AddTag(context, "Repairing", "Functions");
		AddTag(context, "Sharpening", "Functions");

		// Clothing
		AddTag(context, "Worn Items", "Functions");
		AddTag(context, "Footwear", "Worn Items");
		AddTag(context, "Socks", "Worn Items");
		AddTag(context, "Legwear", "Worn Items");
		AddTag(context, "Bodywear", "Worn Items");
		AddTag(context, "Underwear", "Worn Items");
		AddTag(context, "Belts", "Worn Items");
		AddTag(context, "Gloves", "Worn Items");
		AddTag(context, "Hats", "Worn Items");
		AddTag(context, "Spectacles", "Worn Items");
		AddTag(context, "Goggles", "Worn Items");
		AddTag(context, "Scarves", "Worn Items");
		AddTag(context, "Headwear", "Worn Items");
		AddTag(context, "Fashion Accessories", "Worn Items");
		AddTag(context, "Jewellery", "Worn Items");
		AddTag(context, "Rings", "Jewellery");
		AddTag(context, "Necklaces", "Jewellery");
		AddTag(context, "Bracelets", "Jewellery");
		AddTag(context, "Anklets", "Jewellery");
		AddTag(context, "Earrings", "Jewellery");
		AddTag(context, "Piercings", "Jewellery");

		// Separation
		AddTag(context, "Separation", "Functions");
		AddTag(context, "Cutting", "Separation");
		AddTag(context, "Shearing", "Separation");
		AddTag(context, "Shaving", "Separation");
		AddTag(context, "Precision Cutting", "Cutting");
		AddTag(context, "Filleting", "Cutting");
		AddTag(context, "Wood Cutting", "Cutting");
		AddTag(context, "Metal Cutting", "Cutting");
		AddTag(context, "Stone Cutting", "Cutting");
		AddTag(context, "Knife", "Cutting");
		AddTag(context, "Scissors", "Shearing");
		AddTag(context, "Shears", "Shearing");
		AddTag(context, "Guillotine", "Shearing");
		AddTag(context, "Safety Razor", "Shaving");
		AddTag(context, "Razorblade", "Shaving");
		AddTag(context, "Crude", "Shaving");

		// Joining
		AddTag(context, "Joining", "Functions");
		AddTag(context, "Adhesion", "Joining");
		AddTag(context, "Clamping", "Joining");
		AddTag(context, "Fastening", "Joining");
		AddTag(context, "Crimping", "Joining");
		AddTag(context, "Welding", "Joining");
		AddTag(context, "Tie", "Joining");
		AddTag(context, "Pin", "Joining");
		AddTag(context, "Sewing", "Joining");
		AddTag(context, "Surgical Suturing", "Sewing");
		AddTag(context, "Glue", "Adhesion");
		AddTag(context, "Cement", "Adhesion");
		AddTag(context, "Joining Tape", "Adhesion");
		AddTag(context, "Nail", "Fastening");
		AddTag(context, "Bolt", "Fastening");
		AddTag(context, "Nut", "Fastening");
		AddTag(context, "Washer", "Fastening");
		AddTag(context, "Screw", "Fastening");
		AddTag(context, "Rivet", "Fastening");
		AddTag(context, "Clamp", "Clamping");
		AddTag(context, "Peg", "Clamping");
		AddTag(context, "Clip", "Clamping");
		AddTag(context, "Tie Strap", "Tie");
		AddTag(context, "Tie Wire", "Tie");
		AddTag(context, "Tie Rope", "Tie");
		AddTag(context, "Tie Band", "Tie");
		AddTag(context, "Wire Crimp", "Crimping");
		AddTag(context, "Cable Crimp", "Crimping");
		AddTag(context, "Metal Crimp", "Crimping");
		AddTag(context, "Jewellers Crimp", "Crimping");
		AddTag(context, "Soldering Wire", "Welding");
		AddTag(context, "Solderer", "Welding");
		AddTag(context, "Arc Welding Wire", "Welding");
		AddTag(context, "Arc Welder", "Welding");
		AddTag(context, "Flashbutt Welder", "Welding");
		AddTag(context, "Aluminothermic Welding Portion", "Welding");
		AddTag(context, "Aluminothermic Welding Crucible", "Welding");
		AddTag(context, "Braze Welding Rod", "Welding");
		AddTag(context, "Braze Welder", "Welding");
		AddTag(context, "Flux", "Welding");

		// Containers
		AddTag(context, "Container", "Functions");
		AddTag(context, "Porous Container", "Container");
		AddTag(context, "Watertight Container", "Container");
		AddTag(context, "Airtight Container", "Container");
		AddTag(context, "Open Container", "Container");

		// Tools
		AddTag(context, "Tools", "Functions");

		// Cooking
		AddTag(context, "Cooking", "Tools");

		// Cooking Utensils
		AddTag(context, "Cooking Utensils", "Cooking");
		AddTag(context, "Spatula", "Cooking Utensils");
		AddTag(context, "Mixer", "Cooking Utensils");
		AddTag(context, "Egg Beater", "Cooking Utensils");
		AddTag(context, "Whisk", "Cooking Utensils");
		AddTag(context, "Rolling Pin", "Cooking Utensils");
		AddTag(context, "Piping Bag", "Cooking Utensils");
		AddTag(context, "Cooking Spoon", "Cooking Utensils");
		AddTag(context, "Stirrer", "Cooking Utensils");
		AddTag(context, "Cooking Tongs", "Cooking Utensils");
		AddTag(context, "Skewer", "Cooking Utensils");
		AddTag(context, "Potato Masher", "Cooking Utensils");
		AddTag(context, "Potato Ricer", "Cooking Utensils");
		AddTag(context, "Peeler", "Cooking Utensils");
		AddTag(context, "Swivel Peeler", "Peeler");
		AddTag(context, "Julienne Peeler", "Cooking Utensils");
		AddTag(context, "Paring Knife", "Cooking Utensils");
		AddTag(context, "Chopping Board", "Cooking Utensils");
		AddTag(context, "Pizza Peel", "Cooking Utensils");
		AddTag(context, "Skimmer Ladle", "Cooking Utensils");
		AddTag(context, "Cake Lifter", "Cooking Utensils");
		AddTag(context, "Ladle", "Cooking Utensils");
		AddTag(context, "Serving Spoon", "Cooking Utensils");
		AddTag(context, "Mortar and Pestle", "Cooking Utensils");
		AddTag(context, "Triturator", "Cooking Utensils");
		AddTag(context, "Can Opener", "Cooking Utensils");
		AddTag(context, "Colander", "Cooking Utensils");
		AddTag(context, "Measuring Cup", "Cooking Utensils");
		AddTag(context, "Cookie Cutter", "Cooking Utensils");
		AddTag(context, "Pizza Cutter", "Cooking Utensils");
		AddTag(context, "Pizza Stone", "Cooking Utensils");
		AddTag(context, "Mixing Bowl", "Cooking Utensils");
		AddTag(context, "Basting Brush", "Cooking Utensils");
		AddTag(context, "Sifter", "Cooking Utensils");
		AddTag(context, "Icing Comb", "Cooking Utensils");
		AddTag(context, "Icing Spatula", "Spatula");
		AddTag(context, "Turning Spatula", "Spatula");
		AddTag(context, "Scoop", "Cooking Utensils");
		AddTag(context, "Icecream Scoop", "Scoop");
		AddTag(context, "Bulk Scoop", "Scoop");
		AddTag(context, "Grater", "Cooking Utensils");
		AddTag(context, "Box Grater", "Grater");
		AddTag(context, "Hand Grater", "Grater");
		AddTag(context, "Zester", "Grater");
		AddTag(context, "Spiralizer", "Cooking Utensils");
		AddTag(context, "Bamboo Mat", "Cooking Utensils");
		AddTag(context, "Meat Tenderiser", "Cooking Utensils");
		AddTag(context, "Cooking Knife", "Cooking Utensils");
		AddTag(context, "Santoku Knife", "Cooking Knife");
		AddTag(context, "Carving Knife", "Cooking Knife");
		AddTag(context, "Chef's Knife", "Cooking Knife");
		AddTag(context, "Bread Knife", "Cooking Knife");
		AddTag(context, "Butter Knife", "Cooking Knife");
		AddTag(context, "Steak Knife", "Cooking Knife");
		AddTag(context, "Serrated Knife", "Cooking Knife");
		AddTag(context, "Filleting Knife", "Cooking Knife");
		AddTag(context, "Utility Knife", "Cooking Knife");
		AddTag(context, "Oyster Knife", "Cooking Knife");
		AddTag(context, "Carving Fork", "Cooking Utensils");
		AddTag(context, "Garlic Press", "Cooking Utensils");
		AddTag(context, "Juice Press", "Cooking Utensils");
		AddTag(context, "Cherry Pitter", "Cooking Utensils");
		AddTag(context, "Nut Cracker", "Cooking Utensils");
		AddTag(context, "Fish Scaler", "Cooking Utensils");
		AddTag(context, "Marinade Injector", "Cooking Utensils");
		AddTag(context, "Cooling Rack", "Cooking Utensils");
		AddTag(context, "Drying Rack", "Cooking Utensils");
		AddTag(context, "Honey Dipper", "Cooking Utensils");
		AddTag(context, "Fishbone Tweezers", "Cooking Utensils");
		AddTag(context, "Seafood Pick", "Cooking Utensils");
		AddTag(context, "Dough Scraper", "Cooking Utensils");
		AddTag(context, "Sponge Slicer", "Cooking Utensils");
		AddTag(context, "Kitchen Funnel", "Cooking Utensils");
		AddTag(context, "Cookware", "Cooking");
		AddTag(context, "Cooking Pot", "Cookware");
		AddTag(context, "Cooking Pan", "Cookware");
		AddTag(context, "Cooking Tray", "Cookware");
		AddTag(context, "Bakeware", "Cookware");
		AddTag(context, "Frypan", "Cooking Pan");
		AddTag(context, "Saucepan", "Cooking Pan");
		AddTag(context, "Skillet", "Cooking Pan");
		AddTag(context, "Saute Pan", "Cooking Pan");
		AddTag(context, "Paella Pan", "Cooking Pan");
		AddTag(context, "Wok", "Cooking Pan");
		AddTag(context, "Stock Pot", "Cooking Pot");
		AddTag(context, "Boiling Pot", "Cooking Pot");
		AddTag(context, "Stew Pot", "Cooking Pot");
		AddTag(context, "Cauldron", "Cooking Pot");
		AddTag(context, "Baking Sheet", "Cooking Tray");
		AddTag(context, "Baking Tray", "Cooking Tray");
		AddTag(context, "Patisserie Tray", "Cooking Tray");
		AddTag(context, "Roasting Pan", "Cooking Tray");
		AddTag(context, "Rack Roasting Pan", "Cooking Tray");
		AddTag(context, "Deep Roasting Pan", "Cooking Tray");
		AddTag(context, "Pizza Tray", "Cooking Tray");
		AddTag(context, "Cake Tin", "Bakeware");
		AddTag(context, "Round Cake Tin", "Cake Tin");
		AddTag(context, "Square Cake Tin", "Cake Tin");
		AddTag(context, "Rectangular Cake Tin", "Cake Tin");
		AddTag(context, "Quiche Tin", "Bakeware");
		AddTag(context, "Muffin Pan", "Bakeware");
		AddTag(context, "Loaf Pan", "Bakeware");

		// Cleaning
		AddTag(context, "Cleaning", "Tools");
		AddTag(context, "Broom", "Cleaning");
		AddTag(context, "Millet Broom", "Broom");
		AddTag(context, "Straw Broom", "Broom");
		AddTag(context, "Indoor Broom", "Broom");
		AddTag(context, "Outdoor Broom", "Broom");
		AddTag(context, "Shop Broom", "Broom");
		AddTag(context, "Dustpan and Broom", "Broom");
		AddTag(context, "Mop", "Cleaning");
		AddTag(context, "Brush", "Cleaning");
		AddTag(context, "Scrub Brush", "Brush");
		AddTag(context, "Grout Brush", "Brush");
		AddTag(context, "Toilet Brush", "Brush");
		AddTag(context, "Wirebrush", "Brush");
		AddTag(context, "Bottlebrush", "Brush");
		AddTag(context, "Stem Brush", "Brush");
		AddTag(context, "Drain Brush", "Brush");
		AddTag(context, "Duster", "Cleaning");
		AddTag(context, "Cleaning Rag", "Cleaning");
		AddTag(context, "Scourer", "Cleaning");
		AddTag(context, "Steel Wool", "Cleaning");
		AddTag(context, "Dishcloth", "Cleaning");
		AddTag(context, "Sponge", "Cleaning");
		AddTag(context, "Chamois", "Cleaning");
		AddTag(context, "Squeegee", "Cleaning");
		AddTag(context, "Soap", "Cleaning");

		// Digging
		AddTag(context, "Digging", "Tools");
		AddTag(context, "Hoe", "Digging");
		AddTag(context, "Mattock", "Digging");
		AddTag(context, "Shovel", "Digging");
		AddTag(context, "Spade", "Digging");

		// Construction Tools
		AddTag(context, "Construction Tools", "Tools");
		AddTag(context, "Spanner", "Construction Tools");
		AddTag(context, "Shifter Spanner", "Construction Tools");
		AddTag(context, "Screwdriver", "Construction Tools");
		AddTag(context, "Hammer", "Construction Tools");
		AddTag(context, "Riveter", "Construction Tools");
		AddTag(context, "Rivet Gun", "Construction Tools");
		AddTag(context, "Construction Stapler", "Construction Tools");
		AddTag(context, "Mallet", "Construction Tools");
		AddTag(context, "Wrench", "Construction Tools");
		AddTag(context, "Torque Wrench", "Wrench");
		AddTag(context, "Saw", "Construction Tools");
		AddTag(context, "Chisel", "Construction Tools");
		AddTag(context, "Pliers", "Construction Tools");
		AddTag(context, "Trowel", "Construction Tools");
		AddTag(context, "Stringline", "Construction Tools");
		AddTag(context, "Spirit Level", "Construction Tools");
		AddTag(context, "Straightedge", "Construction Tools");
		AddTag(context, "Construction Ruler", "Construction Tools");
		AddTag(context, "Wedge", "Construction Tools");
		AddTag(context, "Ladder", "Construction Tools");
		AddTag(context, "Stepladder", "Ladder");
		AddTag(context, "Extension Ladder", "Ladder");
		AddTag(context, "A-Frame Ladder", "Ladder");
		AddTag(context, "Cement Mixer", "Construction Tools");
		AddTag(context, "Wheelbarrow", "Construction Tools");

		// Metalworking Tools
		AddTag(context, "Metalworking Tools", "Tools");
		AddTag(context, "Anvil", "Metalworking Tools");
		AddTag(context, "Forge", "Metalworking Tools");
		AddTag(context, "Bellows", "Metalworking Tools");
		AddTag(context, "Crucible", "Metalworking Tools");
		AddTag(context, "Forge Tongs", "Metalworking Tools");
		AddTag(context, "Forge Hammer", "Metalworking Tools");

		// Textilecraft Tools
		AddTag(context, "Textilecraft Tools", "Tools");
		AddTag(context, "Awl", "Textilecraft Tools");
		AddTag(context, "Burnisher", "Textilecraft Tools");
		AddTag(context, "Creaser", "Textilecraft Tools");
		AddTag(context, "Thread", "Textilecraft Tools");
		AddTag(context, "Sewing Needle", "Textilecraft Tools");
		AddTag(context, "Beading Needle", "Textilecraft Tools");
		AddTag(context, "Seam Ripper", "Textilecraft Tools");
		AddTag(context, "Fabric Pin", "Textilecraft Tools");
		AddTag(context, "Pinking Shears", "Textilecraft Tools");
		AddTag(context, "Tracer Wheel", "Textilecraft Tools");
		AddTag(context, "Sewing Machine", "Textilecraft Tools");
		AddTag(context, "Loom", "Textilecraft Tools");
		AddTag(context, "Knitting Needle", "Textilecraft Tools");
		AddTag(context, "Dress Form", "Textilecraft Tools");

		// Woodcrafting Tools
		AddTag(context, "Woodcrafting Tools", "Tools");
		AddTag(context, "Splitting Axe", "Woodcrafting Tools");
		AddTag(context, "Felling Axe", "Woodcrafting Tools");
		AddTag(context, "Tomahawk Axe", "Woodcrafting Tools");
		AddTag(context, "Lathe", "Woodcrafting Tools");
		AddTag(context, "Wood Chisel", "Woodcrafting Tools");
		AddTag(context, "Planer", "Woodcrafting Tools");
		AddTag(context, "Splitting Awl", "Woodcrafting Tools");
		AddTag(context, "Adze", "Woodcrafting Tools");
		AddTag(context, "Wood Auger", "Woodcrafting Tools");
		AddTag(context, "Saws", "Woodcrafting Tools");
		AddTag(context, "Bow Saw", "Saws");
		AddTag(context, "Hack Saw", "Saws");
		AddTag(context, "Hand Saw", "Saws");
		AddTag(context, "Fine Saw", "Saws");
		AddTag(context, "Crosscut Saw", "Saws");
		AddTag(context, "Pruning Saw", "Saws");
		AddTag(context, "Forest Saw", "Saws");
		AddTag(context, "Circular Saw", "Saws");
		AddTag(context, "Jig Saw", "Saws");
		AddTag(context, "Chain Saw", "Saws");
		AddTag(context, "Wood Clamp", "Woodcrafting Tools");
		AddTag(context, "Carving Drum Gauge", "Woodcrafting Tools");
		AddTag(context, "Carving Spoon", "Woodcrafting Tools");
		AddTag(context, "Wood File", "Woodcrafting Tools");
		AddTag(context, "Sandpaper", "Woodcrafting Tools");
		AddTag(context, "Rasp", "Woodcrafting Tools");
		AddTag(context, "Trammel", "Woodcrafting Tools");

		// Tattoos
		AddTag(context, "Tattooing Tools", "Tools");
		AddTag(context, "Tattooing Needle", "Tattooing Tools");

		AddTag(context, "Leatherworking Tools", "Tools");
		AddTag(context, "Awl Punch", "Leatherworking Tools");
		AddTag(context, "Leather Stitching Pony", "Leatherworking Tools");
		AddTag(context, "Edge Beveller", "Leatherworking Tools");
		AddTag(context, "Leather Gouge", "Leatherworking Tools");
		AddTag(context, "Leather Creaser", "Leatherworking Tools");

		AddTag(context, "Stoneworking Tools", "Tools");
		AddTag(context, "Stone Chisel", "Stoneworking Tools");
		AddTag(context, "Stone Mallet", "Stoneworking Tools");
		AddTag(context, "Plug and Feathers", "Stoneworking Tools");
		AddTag(context, "Bush Hammer", "Stoneworking Tools");

		AddTag(context, "Spinning Tools", "Textilecraft Tools");
		AddTag(context, "Distaff", "Spinning Tools");
		AddTag(context, "Spindle", "Spinning Tools");
		AddTag(context, "Drop Spindle", "Spinning Tools");
		AddTag(context, "Spinner's Weights", "Spinning Tools");

		AddTag(context, "Weaving Tools", "Textilecraft Tools");
		AddTag(context, "Hand Loom", "Weaving Tools");
		AddTag(context, "Tablet Weaving Cards", "Weaving Tools");
		AddTag(context, "Weaver's Sword", "Weaving Tools");
		AddTag(context, "Warping Board", "Weaving Tools");

		AddTag(context, "Fletching Tools", "Woodcrafting Tools");
		AddTag(context, "Arrow Jig", "Fletching Tools");
		AddTag(context, "Fletching Clamp", "Fletching Tools");
		AddTag(context, "Shaft Straightener", "Fletching Tools");

		AddTag(context, "Bowyer Tools", "Woodcrafting Tools");
		AddTag(context, "Bow Press", "Bowyer Tools");
		AddTag(context, "Tillering Stick", "Bowyer Tools");
		AddTag(context, "Bow Scale", "Bowyer Tools");

		AddTag(context, "Papermaking Tools", "Tools");
		AddTag(context, "Mould and Deckle", "Papermaking Tools");
		AddTag(context, "Press Felt", "Papermaking Tools");
		AddTag(context, "Hollander Beater", "Papermaking Tools");

		AddTag(context, "Glassblowing Tools", "Tools");
		AddTag(context, "Blowpipe", "Glassblowing Tools");
		AddTag(context, "Pontil Rod", "Glassblowing Tools");
		AddTag(context, "Marver Table", "Glassblowing Tools");
		AddTag(context, "Jacks", "Glassblowing Tools");
		AddTag(context, "Paper Pads", "Glassblowing Tools");
		AddTag(context, "Blocks", "Glassblowing Tools");

		AddTag(context, "Locksmithing Tools", "Tools");
		AddTag(context, "Lockpick", "Locksmithing Tools");
		AddTag(context, "Torsion Wrench", "Locksmithing Tools");
		AddTag(context, "Locksmith's File", "Locksmithing Tools");
		AddTag(context, "Locksmith's Tweezers", "Locksmithing Tools");
		AddTag(context, "Locksmithing Jig", "Locksmithing Tools");
		AddTag(context, "Key Gauge", "Locksmithing Tools");
		AddTag(context, "Impressioning File", "Locksmithing Tools");
		AddTag(context, "Safe Dial Manipulator", "Locksmithing Tools");

		AddTag(context, "Gunsmithing Tools", "Tools");
		AddTag(context, "Barrel Reamer", "Gunsmithing Tools");
		AddTag(context, "Gun Drill", "Gunsmithing Tools");
		AddTag(context, "Bore Snake", "Gunsmithing Tools");
		AddTag(context, "Tamping Rod", "Gunsmithing Tools");
		AddTag(context, "Gun Vise", "Gunsmithing Tools");
		AddTag(context, "Mainspring Vise", "Gunsmithing Tools");
		AddTag(context, "Breech Plug Wrench", "Gunsmithing Tools");
		AddTag(context, "Rammer", "Gunsmithing Tools");
		AddTag(context, "Bullet Mould", "Gunsmithing Tools");
		AddTag(context, "Patch Cutter", "Gunsmithing Tools");
		AddTag(context, "Ball Puller", "Gunsmithing Tools");

		AddTag(context, "Tanning Tools", "Tools");
		AddTag(context, "Hide Scraper", "Tanning Tools");
		AddTag(context, "Tanning Beam", "Tanning Tools");
		AddTag(context, "Tanning Paddle", "Tanning Tools");
		AddTag(context, "Tanning Rack", "Tanning Tools");
		AddTag(context, "Leather Dehairing Knife", "Tanning Tools");
		AddTag(context, "Brain Tanning Bucket", "Tanning Tools");

		AddTag(context, "Armouring Tools", "Tools");
		AddTag(context, "Plate Snips", "Armouring Tools");
		AddTag(context, "Armourer's Stake", "Armouring Tools");
		AddTag(context, "Armourer's Anvil", "Armouring Tools");
		AddTag(context, "Dishing Form", "Armouring Tools");
		AddTag(context, "Raising Hammer", "Armouring Tools");
		AddTag(context, "Planishing Hammer", "Armouring Tools");
		AddTag(context, "Ball Stake", "Armouring Tools");
		AddTag(context, "T-Stake", "Armouring Tools");
		AddTag(context, "Armourer's Forming Bags", "Armouring Tools");
		AddTag(context, "Armourer's Pliers", "Armouring Tools");

		AddTag(context, "Weaponsmithing Tools", "Tools");
		AddTag(context, "Swordsmith's Hammer", "Weaponsmithing Tools");
		AddTag(context, "Sword Anvil", "Weaponsmithing Tools");
		AddTag(context, "Fuller Tool", "Weaponsmithing Tools");
		AddTag(context, "Tang Punch", "Weaponsmithing Tools");
		AddTag(context, "Sword Vise", "Weaponsmithing Tools");
		AddTag(context, "Quenching Trough", "Weaponsmithing Tools");
		AddTag(context, "Pommel Tightening Jig", "Weaponsmithing Tools");
		AddTag(context, "Crossguard Fixture", "Weaponsmithing Tools");
		AddTag(context, "Forge Bellows", "Weaponsmithing Tools");
		AddTag(context, "Grindstone", "Weaponsmithing Tools");

		AddTag(context, "Butcher Tools", "Tools");

		// Field Dressing
		AddTag(context, "Field Dressing Tools", "Butcher Tools");
		AddTag(context, "Skinning Knife", "Field Dressing Tools");
		AddTag(context, "Fleshing Knife", "Field Dressing Tools");
		AddTag(context, "Boning Knife", "Field Dressing Tools");
		AddTag(context, "Carcass Hook", "Field Dressing Tools");
		AddTag(context, "Gut Hook Knife", "Field Dressing Tools");
		AddTag(context, "Meat Saw", "Field Dressing Tools");
		AddTag(context, "Pelting Blade", "Field Dressing Tools");
		AddTag(context, "Splitting Saw", "Field Dressing Tools");

		// Cutting and Portioning
		AddTag(context, "Meat Cutting Tools", "Butcher Tools");
		AddTag(context, "Butcher's Knife", "Meat Cutting Tools");
		AddTag(context, "Cimeter Knife", "Meat Cutting Tools");
		AddTag(context, "Cleaver", "Meat Cutting Tools");
		AddTag(context, "Breaking Knife", "Meat Cutting Tools");
		AddTag(context, "Trimming Knife", "Meat Cutting Tools");
		AddTag(context, "Bone Saw", "Meat Cutting Tools");
		AddTag(context, "Portioning Blade", "Meat Cutting Tools");

		// Processing and Preparation
		AddTag(context, "Meat Processing Tools", "Butcher Tools");
		AddTag(context, "Meat Grinder", "Meat Processing Tools");
		AddTag(context, "Tenderizing Mallet", "Meat Processing Tools");
		AddTag(context, "Slicing Machine", "Meat Processing Tools");
		AddTag(context, "Stuffing Funnel", "Meat Processing Tools");
		AddTag(context, "Larding Needle", "Meat Processing Tools");

		// Cleaning and Maintenance
		AddTag(context, "Butcher Cleaning Tools", "Butcher Tools");
		AddTag(context, "Carcass Scraper", "Butcher Cleaning Tools");

		// Special Tools
		AddTag(context, "Special Butcher Tools", "Butcher Tools");
		AddTag(context, "Saw Guide", "Special Butcher Tools");
		AddTag(context, "Meat Injector", "Special Butcher Tools");
		AddTag(context, "Bone Dust Scraper", "Special Butcher Tools");
		AddTag(context, "Sinew Remover", "Special Butcher Tools");

		// Storage and Organization
		AddTag(context, "Meat Storage Tools", "Butcher Tools");
		AddTag(context, "Butcher Hook", "Meat Storage Tools");
		AddTag(context, "Hanging Rack", "Meat Storage Tools");
		AddTag(context, "Cooler Box", "Meat Storage Tools");
		AddTag(context, "Meat Bin", "Meat Storage Tools");

		// Medical Tools
		AddTag(context, "Medical Tools", "Tools");
		AddTag(context, "Stethoscope", "Medical Tools");
		AddTag(context, "Blood Pressure Monitor", "Medical Tools");
		AddTag(context, "Ophthalmoscope", "Medical Tools");
		AddTag(context, "Otascope", "Medical Tools");
		AddTag(context, "Dermatoscope", "Medical Tools");
		AddTag(context, "Electrocardiogram", "Medical Tools");
		AddTag(context, "Electroencephalogram", "Medical Tools");
		AddTag(context, "Glucometer", "Medical Tools");
		AddTag(context, "Spirometer", "Medical Tools");
		AddTag(context, "Mechanical Scale", "Medical Tools");
		AddTag(context, "Height Measuring Scale", "Medical Tools");
		AddTag(context, "Tendon Hammer", "Medical Tools");
		AddTag(context, "Human Blood Typing", "Medical Tools");

		// Surgical Tools
		AddTag(context, "Surgical Tools", "Tools");
		AddTag(context, "Scalpel", "Surgical Tools");
		AddTag(context, "Bonesaw", "Surgical Tools");
		AddTag(context, "Forceps", "Surgical Tools");
		AddTag(context, "Tissue Forceps", "Forceps");
		AddTag(context, "Dissecting Forceps", "Forceps");
		AddTag(context, "Kelly Forceps", "Forceps");
		AddTag(context, "DeBakey Forceps", "Forceps");
		AddTag(context, "Towel Clamp", "Surgical Tools");
		AddTag(context, "Intestinal Clamp", "Surgical Tools");
		AddTag(context, "Arterial Clamp", "Surgical Tools");
		AddTag(context, "Curette", "Surgical Tools");
		AddTag(context, "Surgical Retractor", "Surgical Tools");
		AddTag(context, "Speculum", "Surgical Tools");
		AddTag(context, "Surgical Suture Needle", "Surgical Tools");
		AddTag(context, "Ski Suture Needle", "Surgical Suture Needle");
		AddTag(context, "Canoe Suture Needle", "Surgical Suture Needle");
		AddTag(context, "Atraumatic Suture Needle", "Surgical Suture Needle");
		AddTag(context, "Trocar", "Surgical Tools");
		AddTag(context, "Absorbable Suture", "Surgical Suturing");
		AddTag(context, "Non-Absorbable Suture", "Surgical Suturing");
		AddTag(context, "Plain Gut Suture", "Absorbable Suture");
		AddTag(context, "Chromic Gut Suture", "Absorbable Suture");
		AddTag(context, "Fast Gut Suture", "Absorbable Suture");
		AddTag(context, "Synthetic Monofilament Suture", "Absorbable Suture");
		AddTag(context, "Synthetic Polyfilament Suture", "Absorbable Suture");
		AddTag(context, "Silk Suture", "Non-Absorbable Suture");
		AddTag(context, "Nylon Monofilament Suture", "Non-Absorbable Suture");
		AddTag(context, "Nylon Polyfilament Suture", "Non-Absorbable Suture");
		AddTag(context, "Surgical Steel Suture", "Non-Absorbable Suture");
		AddTag(context, "Surgical Stapler", "Surgical Tools");
		AddTag(context, "Surgical Staples", "Surgical Tools");

		// Science Tools
		AddTag(context, "Scientific Tools", "Tools");

		AddTag(context, "Plane Table", "Scientific Tools");
		AddTag(context, "Measurement Tools", "Scientific Tools");
		AddTag(context, "Micrometer", "Measurement Tools");
		AddTag(context, "Tribometer", "Measurement Tools");
		AddTag(context, "Absorption Wavemeter", "Measurement Tools");
		AddTag(context, "Accelerometer", "Measurement Tools");
		AddTag(context, "Pressure Gauge", "Measurement Tools");
		AddTag(context, "Voltmeter", "Measurement Tools");
		AddTag(context, "Flow Meter", "Measurement Tools");
		AddTag(context, "Capacitance Meter", "Measurement Tools");
		AddTag(context, "Chondrometer", "Measurement Tools");
		AddTag(context, "Deposit Gauge", "Measurement Tools");
		AddTag(context, "Diffusion Tube", "Measurement Tools");
		AddTag(context, "Dynameter", "Measurement Tools");
		AddTag(context, "Watch", "Measurement Tools");
		AddTag(context, "Stopwatch", "Watch");
		AddTag(context, "Wristwatch", "Watch");
		AddTag(context, "Chronograph", "Watch");
		AddTag(context, "Actinometer", "Measurement Tools");
		AddTag(context, "Aerometer", "Measurement Tools");
		AddTag(context, "Hydrometer", "Measurement Tools");
		AddTag(context, "Bolometer", "Measurement Tools");
		AddTag(context, "Butyrometer", "Measurement Tools");
		AddTag(context, "Calorimeter", "Measurement Tools");
		AddTag(context, "Anemometer", "Measurement Tools");
		AddTag(context, "Auxanometer", "Measurement Tools");
		AddTag(context, "Electrometer", "Measurement Tools");
		AddTag(context, "Electroscope", "Measurement Tools");
		AddTag(context, "Eudiometer", "Measurement Tools");
		AddTag(context, "Explosimeter", "Measurement Tools");
		AddTag(context, "Force Gauge", "Measurement Tools");
		AddTag(context, "Faraday Cup", "Measurement Tools");
		AddTag(context, "Geiger Counter", "Measurement Tools");
		AddTag(context, "Kofler Bench", "Measurement Tools");
		AddTag(context, "Load Cell", "Measurement Tools");
		AddTag(context, "Magnetometer", "Measurement Tools");
		AddTag(context, "Mass Spectrometer", "Measurement Tools");
		AddTag(context, "Multimeter", "Measurement Tools");
		AddTag(context, "Optometer", "Measurement Tools");
		AddTag(context, "Osmometer", "Measurement Tools");
		AddTag(context, "Pedometer", "Measurement Tools");
		AddTag(context, "Penetrometer", "Measurement Tools");
		AddTag(context, "PH Meter", "Measurement Tools");
		AddTag(context, "Phoropter", "Measurement Tools");
		AddTag(context, "Radar", "Measurement Tools");
		AddTag(context, "Radar Speed Gun", "Measurement Tools");
		AddTag(context, "Radiosonde", "Measurement Tools");
		AddTag(context, "Refractometer", "Measurement Tools");
		AddTag(context, "Seismometer", "Measurement Tools");
		AddTag(context, "Speedometer", "Measurement Tools");
		AddTag(context, "Stereoautograph", "Measurement Tools");
		AddTag(context, "Strain Gauge", "Measurement Tools");
		AddTag(context, "Tachometer", "Measurement Tools");
		AddTag(context, "Water Sensor", "Measurement Tools");
		AddTag(context, "Rain Gauge", "Measurement Tools");
		AddTag(context, "Thermometer", "Measurement Tools");
		AddTag(context, "Barometer", "Measurement Tools");


		AddTag(context, "Surveying Equipment", "Measurement Tools");
		AddTag(context, "Levelling Rod", "Surveying Equipment");
		AddTag(context, "Theodolyte", "Surveying Equipment");
		AddTag(context, "Plumb Bob", "Surveying Equipment");

		AddTag(context, "Navigational Tools", "Scientific Tools");
		AddTag(context, "Airspeed Indicator", "Navigational Tools");
		AddTag(context, "Altimeter", "Navigational Tools");
		AddTag(context, "Backstaff", "Navigational Tools");
		AddTag(context, "Compass", "Navigational Tools");
		AddTag(context, "Pelorus", "Navigational Tools");
		AddTag(context, "Sextant", "Navigational Tools");
		AddTag(context, "Variometer", "Navigational Tools");
		AddTag(context, "Orrery", "Navigational Tools");
		AddTag(context, "Astrolabe", "Navigational Tools");
		AddTag(context, "Nocturlabe", "Navigational Tools");
		AddTag(context, "Sundial", "Navigational Tools");
		AddTag(context, "Telescope", "Navigational Tools");
		AddTag(context, "Periscope", "Navigational Tools");

		AddTag(context, "Specialised Vessels", "Scientific Tools");
		AddTag(context, "Beaker", "Specialised Vessels");
		AddTag(context, "Volumetric Flask", "Specialised Vessels");
		AddTag(context, "Test Tube", "Specialised Vessels");
		AddTag(context, "Graduated Cylinder", "Specialised Vessels");
		AddTag(context, "Burette", "Specialised Vessels");
		AddTag(context, "Volumetric Pipette", "Specialised Vessels");
		AddTag(context, "Evaporating Dish", "Specialised Vessels");
		AddTag(context, "Petri Dish", "Specialised Vessels");
		AddTag(context, "Watch Glass", "Specialised Vessels");
		AddTag(context, "Titration Flask", "Specialised Vessels");
		AddTag(context, "Vacuum Flask", "Specialised Vessels");
		AddTag(context, "Retort", "Specialised Vessels");
		AddTag(context, "Round-Bottom Flask", "Specialised Vessels");
		AddTag(context, "Dropper", "Specialised Vessels");
		AddTag(context, "Laboratory Funnel", "Specialised Vessels");
		AddTag(context, "Vial", "Specialised Vessels");
		AddTag(context, "Reagent Bottle", "Specialised Vessels");


		AddTag(context, "Laboratory Equipment", "Scientific Tools");
		AddTag(context, "Laboratory Burner", "Laboratory Equipment");
		AddTag(context, "Bunsen Burner", "Laboratory Burner");
		AddTag(context, "Alcohol Burner", "Laboratory Burner");
		AddTag(context, "Meker Burner", "Laboratory Burner");
		AddTag(context, "Dessicator", "Laboratory Equipment");
		AddTag(context, "Heating Mantle", "Laboratory Equipment");
		AddTag(context, "Laboratory Hot Plate", "Laboratory Equipment");
		AddTag(context, "Laboratory Oven", "Laboratory Equipment");
		AddTag(context, "Laboratory Kiln", "Laboratory Equipment");
		AddTag(context, "Vacuum Dry Box", "Laboratory Equipment");
		AddTag(context, "Beaker Clamp", "Laboratory Equipment");
		AddTag(context, "Burette Clamp", "Laboratory Equipment");
		AddTag(context, "Flask Clamp", "Laboratory Equipment");
		AddTag(context, "Test Tube Rack", "Laboratory Equipment");
		AddTag(context, "Test Tube Holder", "Laboratory Equipment");
		AddTag(context, "Laboratory Tripod", "Laboratory Equipment");

		AddTag(context, "Grooming", "Tools");
		AddTag(context, "Hairbrush", "Grooming");
		AddTag(context, "Comb", "Grooming");
		AddTag(context, "Hair Scissors", "Grooming");
		AddTag(context, "Shaving Razor", "Grooming");
		AddTag(context, "Tweezers", "Grooming");
		AddTag(context, "Hair Curlers", "Grooming");
		AddTag(context, "Hairdryer", "Grooming");
		AddTag(context, "Hair Straightener", "Grooming");
		AddTag(context, "Detangling Brush", "Grooming");
		AddTag(context, "Detangling Comb", "Grooming");
		AddTag(context, "Sectioning Clips", "Grooming");
		AddTag(context, "Edge Brush", "Grooming");
		AddTag(context, "Diffuser", "Grooming");

		// Market Items
		AddTag(context, "Market", "");

		AddTag(context, "Nourishment", "Market");
		AddTag(context, "Staple Food", "Nourishment");
		AddTag(context, "Standard Food", "Nourishment");
		AddTag(context, "Luxury Food", "Nourishment");

		AddTag(context, "Clothing", "Market");
		AddTag(context, "Simple Clothing", "Clothing");
		AddTag(context, "Standard Clothing", "Clothing");
		AddTag(context, "Luxury Clothing", "Clothing");
		AddTag(context, "Winter Clothing", "Clothing");
		AddTag(context, "Military Uniforms", "Clothing");

		AddTag(context, "Domestic Heating", "Market");
		AddTag(context, "Combustion Heating", "Domestic Heating");
		AddTag(context, "Oil Heating", "Domestic Heating");
		AddTag(context, "Electric Heating", "Domestic Heating");

		AddTag(context, "Intoxicants", "Market");
		AddTag(context, "Wine", "Intoxicants");
		AddTag(context, "Beer", "Intoxicants");
		AddTag(context, "Mead", "Intoxicants");
		AddTag(context, "Spirits", "Intoxicants");

		AddTag(context, "Luxury Drinks", "Market");
		AddTag(context, "Tea", "Luxury Drinks");
		AddTag(context, "Coffee", "Luxury Drinks");

		AddTag(context, "Household Goods", "Market");
		AddTag(context, "Simple Furniture", "Household Goods");
		AddTag(context, "Standard Furniture", "Household Goods");
		AddTag(context, "Luxury Furniture", "Household Goods");
		AddTag(context, "Simple Decorations", "Household Goods");
		AddTag(context, "Standard Decorations", "Household Goods");
		AddTag(context, "Luxury Decorations", "Household Goods");
		AddTag(context, "Simple Wares", "Household Goods");
		AddTag(context, "Standard Wares", "Household Goods");
		AddTag(context, "Luxury Wares", "Household Goods");

		AddTag(context, "Military Goods", "Market");
		AddTag(context, "Weapons", "Military Goods");
		AddTag(context, "Spears", "Weapons");
		AddTag(context, "Swords", "Weapons");
		AddTag(context, "Clubs", "Weapons");
		AddTag(context, "Axes", "Weapons");
		AddTag(context, "Maces", "Weapons");
		AddTag(context, "Daggers", "Weapons");
		AddTag(context, "Crossbows", "Weapons");
		AddTag(context, "Bows", "Weapons");
		AddTag(context, "Guns", "Weapons");
		AddTag(context, "Hammers", "Weapons");
		AddTag(context, "Polearms", "Weapons");
		AddTag(context, "Other Weapons", "Weapons");
		AddTag(context, "Ammunition", "Military Goods");
		AddTag(context, "Arrows", "Ammunition");
		AddTag(context, "Bolts", "Ammunition");
		AddTag(context, "Bullets", "Ammunition");
		AddTag(context, "Blackpowder", "Ammunition");
		AddTag(context, "Armour", "Military Goods");
		AddTag(context, "Leather Armour", "Armour");
		AddTag(context, "Mail Armour", "Armour");
		AddTag(context, "Plate Armour", "Armour");
		AddTag(context, "Primitive Armour", "Armour");
		AddTag(context, "Shields", "Armour");

		AddTag(context, "Transportation", "Market");
		AddTag(context, "Cargo Transportation", "Transportation");
		AddTag(context, "Cart Haulage", "Cargo Transportation");
		AddTag(context, "Manual Haulage", "Cargo Transportation");
		AddTag(context, "Mule Haulage", "Cargo Transportation");
		AddTag(context, "Ship Haulage", "Cargo Transportation");
		AddTag(context, "Passenger Transportation", "Transportation");
		AddTag(context, "Cart Passage", "Passenger Transportation");
		AddTag(context, "Horse Passage", "Passenger Transportation");
		AddTag(context, "Wagon Passage", "Passenger Transportation");
		AddTag(context, "Ship Passage", "Passenger Transportation");

		AddTag(context, "Medicine", "Market");
		AddTag(context, "Simple Medicine", "Medicine");
		AddTag(context, "Standard Medicine", "Medicine");
		AddTag(context, "High-Quality Medicine", "Medicine");

		AddTag(context, "Warehousing", "Market");

		AddTag(context, "Professional Tools", "Market");
		AddTag(context, "Primitive Tools", "Professional Tools");
		AddTag(context, "Simple Tools", "Professional Tools");
		AddTag(context, "Standard Tools", "Professional Tools");
		AddTag(context, "High-Quality Tools", "Professional Tools");

		AddTag(context, "Raw Materials", "Market");
		AddTag(context, "Lumber", "Raw Materials");
		AddTag(context, "Straw", "Raw Materials");
		AddTag(context, "Cloth", "Raw Materials");
		AddTag(context, "Stone Blocks", "Raw Materials");
		AddTag(context, "Sand", "Raw Materials");
		AddTag(context, "Clay", "Raw Materials");
		AddTag(context, "Aggregate", "Raw Materials");
		AddTag(context, "Cement Mineral", "Raw Materials");
		AddTag(context, "Steel", "Raw Materials");
		AddTag(context, "Copper", "Raw Materials");
		AddTag(context, "Gold", "Raw Materials");
		AddTag(context, "Silver", "Raw Materials");
		AddTag(context, "Bronze", "Raw Materials");
		AddTag(context, "Brass", "Raw Materials");
		AddTag(context, "Lead", "Raw Materials");

		AddTag(context, "Lighting", "Market");
		AddTag(context, "Candles", "Lighting");
		AddTag(context, "Torches", "Lighting");
		AddTag(context, "Lamps", "Lighting");

		AddTag(context, "Consumables", "");
		AddTag(context, "Padded Vest", "Consumables");
		AddTag(context, "Padded Gloves", "Consumables");
		AddTag(context, "Padded Trousers", "Consumables");
		AddTag(context, "Thick Leather", "Consumables");
		AddTag(context, "Armouring Rings", "Consumables");
		AddTag(context, "Armouring Studs", "Consumables");
		AddTag(context, "Armouring Scales", "Consumables");
		AddTag(context, "Wire", "Consumables");
		AddTag(context, "Sheet Metal", "Consumables");
		AddTag(context, "Deer Hindquarter", "Consumables");
		AddTag(context, "Deer Forequarter", "Consumables");
		AddTag(context, "Rump", "Consumables");
		AddTag(context, "Tenderloin", "Consumables");
		AddTag(context, "Pig Hindquarter", "Consumables");
		AddTag(context, "Pig Forequarter", "Consumables");
		AddTag(context, "Entrails", "Consumables");
		AddTag(context, "Suet", "Consumables");
		AddTag(context, "Plank", "Consumables");
		AddTag(context, "Log", "Consumables");
		AddTag(context, "Dirt", "Consumables");
		AddTag(context, "Nuts", "Consumables");
		AddTag(context, "Rabbit Roast", "Consumables");
		AddTag(context, "Deer Roast", "Consumables");
		AddTag(context, "Pig Roast", "Consumables");
		AddTag(context, "Grove Trees", "Consumables");
		AddTag(context, "Tree", "Consumables");
		AddTag(context, "Trunk", "Consumables");
		AddTag(context, "Open Grave", "Consumables");
		AddTag(context, "Thick Hide", "Consumables");
		AddTag(context, "Branch", "Consumables");
		AddTag(context, "Short Shaft", "Consumables");
		AddTag(context, "Knapped Stone", "Consumables");
		AddTag(context, "Reeds", "Consumables");
		AddTag(context, "Sword Blade", "Consumables");
		AddTag(context, "Knife Blade", "Consumables");
		AddTag(context, "Mace Head", "Consumables");
		AddTag(context, "Axe Head", "Consumables");
		AddTag(context, "Pole", "Consumables");
		AddTag(context, "Grass", "Consumables");
		AddTag(context, "Tusk", "Consumables");
		AddTag(context, "Spearhead", "Consumables");
		AddTag(context, "Mould Sheet Metal", "Consumables");
		AddTag(context, "Mould Sword Blade", "Consumables");
		AddTag(context, "Mould Knife Blade", "Consumables");
		AddTag(context, "Mould Mace Head", "Consumables");
		AddTag(context, "Mould Axe Head", "Consumables");
		AddTag(context, "Mould Spearhead", "Consumables");
		AddTag(context, "Construction Brick", "Consumables");
		AddTag(context, "Fletching", "Consumables");
		AddTag(context, "Sword Grip", "Consumables");
		AddTag(context, "Curved Leather Piece", "Consumables");
		AddTag(context, "Long Shaft", "Consumables");
		AddTag(context, "Drawplate", "Consumables");
		AddTag(context, "Mould Brick", "Consumables");

		AddTag(context, "Pottery Tools", "Tools");
		AddTag(context, "Potter's Wheel", "Pottery Tools");
		AddTag(context, "Clay Knife", "Pottery Tools");
		AddTag(context, "Potter's Rib", "Pottery Tools");
		AddTag(context, "Loop Tool", "Pottery Tools");
		AddTag(context, "Needle Tool", "Pottery Tools");
		AddTag(context, "Wire Cutter", "Pottery Tools");
		AddTag(context, "Clay Stamp", "Pottery Tools");
		AddTag(context, "Pug Mill", "Pottery Tools");
		AddTag(context, "Slip Trailer", "Pottery Tools");
		AddTag(context, "Hump Mold", "Pottery Tools");
		AddTag(context, "Press Mold", "Pottery Tools");
		AddTag(context, "Extruder", "Pottery Tools");
		AddTag(context, "Slab Roller", "Pottery Tools");
		AddTag(context, "Kiln", "Pottery Tools");

		AddTag(context, "Smelting Tools", "Tools");
		AddTag(context, "Smelting Furnace", "Smelting Tools");
		AddTag(context, "Crucible Tongs", "Smelting Tools");
		AddTag(context, "Slag Skimmer", "Smelting Tools");
		AddTag(context, "Furnace Bellows", "Smelting Tools");
		AddTag(context, "Ore Crusher", "Smelting Tools");
		AddTag(context, "Ore Roaster", "Smelting Tools");
		AddTag(context, "Slag Hammer", "Smelting Tools");
		AddTag(context, "Bloom Tongs", "Smelting Tools");
		AddTag(context, "Charging Bucket", "Smelting Tools");
		AddTag(context, "Tap Rod", "Smelting Tools");

		context.SaveChanges();
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