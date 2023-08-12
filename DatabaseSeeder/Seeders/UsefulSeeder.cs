using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Database;
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
			("terrain",
				"Do you want to install a collection of terrestrial terrain types?\n\nPlease answer #3yes#f or #3no#f: ",
				(context, questions) => context.Terrains.Count() <= 1,
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
				(context, questions) => context.Tags.All(x => x.Name != "Functions"),
				(answer, context) =>
				{
					if (answer.EqualToAny("yes", "y", "no", "n")) return (true, string.Empty);
					return (false, "Invalid answer");
				}),
			("doortrait", @"#DDoor Options#F

In FutureMUD, doors are usually built to be able to be removed by someone with the right skills - when open, unlocked and on the ""hinge side"". This can be disabled, and indeed one of the example components that will be installed is a door that cannot be removed. 

However, for the removable doors, you need to specify a trait (usually a skill) that people need to have to do the work.

What is the name of the trait you want to use for installing/uninstalling doors (e.g. construction or similar)? If the skill does not exist, one will be created with the name you specify. Leave blank to skip doors.

Please answer here: ",
				(context, questions) => questions["items"].EqualToAny("yes", "y"),
				(answer, context) => { return (true, string.Empty); })
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		var errors = new List<string>();
		if (questionAnswers["ai"].EqualToAny("yes", "y")) SeedAI(context, errors);

		if (questionAnswers["items"].EqualToAny("yes", "y")) SeedItemsPart1(context, questionAnswers, errors);

		if (questionAnswers["itemsp2"].EqualToAny("yes", "y")) SeedItemsPart2(context, questionAnswers, errors);

		if (questionAnswers["itemsp3"].EqualToAny("yes", "y")) SeedItemsPart3(context, questionAnswers, errors);

		if (questionAnswers["modernitems"].EqualToAny("yes", "y")) SeedModernItems(context, errors);

		if (questionAnswers["terrain"].EqualToAny("yes", "y")) SeedTerrain(context, errors);

		if (questionAnswers["tags"].EqualToAny("yes", "y"))
		{
			SeedTags(context, errors);
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
			context.Terrains.Count() > 1 &&
			!context.GameItemComponentProtos.All(x => x.Name != "Container_Table") &&
			!context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor") &&
			!context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc") &&
			!context.Tags.All(x => x.Name != "Functions"))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		if (context.ArtificialIntelligences.All(x => x.Name != "CommandableOwner") ||
			context.Terrains.Count() <= 1 ||
			context.GameItemComponentProtos.All(x => x.Name != "Container_Table") ||
			context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor") ||
			context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc") ||
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

	private void SeedTerrain(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.Terrains.Count() > 1)
		{
			errors.Add("Terrains were already installed, so did not add any new data.");
			return;
		}

		context.Terrains.Find(1)!.DefaultTerrain = false;

		void AddTerrain(string name, string behaviour, double movementRate, double staminaCost,
			Difficulty hideDifficulty, Difficulty spotDifficulty, string? atmosphere, CellOutdoorsType outdoorsType,
			Color editorColour, string editorText = null, bool isdefault = false)
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
				DefaultCellOutdoorsType = (int)outdoorsType
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
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "Re", true);
		AddTerrain("Bedroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "Br");
		AddTerrain("Kitchen", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "Ki");
		AddTerrain("Bathroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "To");
		AddTerrain("Living Room", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "LR");
		AddTerrain("Hallway", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "Hw");
		AddTerrain("Hall", "cave", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "Ha");
		AddTerrain("Barracks", "cave", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "Ba");
		AddTerrain("Gymnasium", "cave", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "Gy");
		AddTerrain("Shopfront", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Workshop", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Office", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Workshop", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Indoor Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
			"Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Underground Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
			"Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "St");
		AddTerrain("Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "Ga");
		AddTerrain("Underground Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
			"Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.DarkCyan, "Ga");
		AddTerrain("Barn", "cave", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "St");
		AddTerrain("Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsWithWindows, Color.DarkCyan, "Ja");
		AddTerrain("Dank Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Indoors, Color.DarkCyan, "Ja");
		AddTerrain("Dungeon", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsNoLight, Color.DarkCyan, "Du");
		AddTerrain("Grotto", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsNoLight, Color.DarkCyan, "Ca");
		AddTerrain("Cellar", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsNoLight, Color.DarkCyan, "Ca");
		AddTerrain("Baths", "indoors", 0.5, 3.0, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DarkCyan, "Ba");
		AddTerrain("Indoor Pool", $"shallowwater {poolwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DarkCyan, "Ba");
		AddTerrain("Indoor Spring", $"shallowwater {springwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DarkCyan, "Ba");

		AddTerrain("Rooftop", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Ghetto Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Slum Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Poor Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Urban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Suburban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Wealthy Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Village Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);
		AddTerrain("Rural Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkSlateGray);

		AddTerrain("Marketplace", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.VeryEasy, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Courtyard", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Park", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Garden", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Lawn", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Showground", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Forum", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Public Square", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Outdoor Mall", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Alleyway", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Garbage Dump", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Midden Heap", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray);
		AddTerrain("Gatehouse", "indoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Trivial, "Breathable Atmosphere",
			CellOutdoorsType.IndoorsClimateExposed, Color.SlateGray);
		AddTerrain("Battlement", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Trivial, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.SlateGray);

		#endregion

		#region Roads

		AddTerrain("Animal Trail", "outdoors", 1.75, 10.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Trail", "outdoors", 1.6, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Dirt Road", "outdoors", 1.5, 10.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Compacted Dirt Road", "outdoors", 1.4, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Gravel Road", "outdoors", 1.3, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Cobblestone Road", "outdoors", 1.2, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray);
		AddTerrain("Asphalt Road", "outdoors", 1.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray);

		#endregion

		#region Terrestrial

		AddTerrain("Grasslands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Savannah", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Shrublands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Steppe", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Shortgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Tallgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Heath", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Pasture", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Meadow", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Field", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Tundra", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Flood Plain", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);

		AddTerrain("Hills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Foothills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Mound", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Drumlin", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Butte", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Kuppe", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Mesa", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Canyon", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Knoll", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Moor", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Tell", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);
		AddTerrain("Dunes", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.OrangeRed);

		AddTerrain("Mountainside", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red);
		AddTerrain("Mountain Pass", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red);
		AddTerrain("Mountain Ridge", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red);
		AddTerrain("Cliff Face", "cliff", 5.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Red);
		AddTerrain("Cliff Edge", "outdoors", 5.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red);

		AddTerrain("Valley", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Vale", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Dell", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Glen", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Strath", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Combe", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Ravine", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Gorge", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);
		AddTerrain("Gully", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Beige);

		AddTerrain("Boreal Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Broadleaf Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Temperate Coniferous Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Temperate Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Tropical Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Bramble", "talltrees", 3.0, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Plantation Forest", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Orchard", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Grove", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);
		AddTerrain("Woodland", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen);

		AddTerrain("Bog", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Salt Marsh", $"shallowwater {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Mangrove Swamp", $"shallowwatertrees {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Wetland", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Swamp Forest", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Tropical Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);
		AddTerrain("Temperate Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
			Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple);

		AddTerrain("Sandy Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Rocky Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Coastal Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);

		AddTerrain("Cave Entrance", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Cave", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Cavern", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Cave Pool", $"watercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);
		AddTerrain("Underground Water", $"deepunderwater {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
			Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen);

		#endregion

		#region Water

		AddTerrain("Sandy Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Rocky Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Beachrock", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Riverbank", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
			CellOutdoorsType.Outdoors, Color.Yellow);
		AddTerrain("Lake Shore", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow);

		AddTerrain("Ocean Shallows", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Ocean Surf", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Ocean", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Mudflat", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Bay", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Lagoon", $"water {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Cove", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Tide Pool", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Shoal", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Coral Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Sound", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Estuary", $"shallowwater {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Shallow River", $"shallowwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("River", $"water {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Deep River", $"deepwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Shallow Lake", $"shallowwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Lake", $"water {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Deep Lake", $"deepwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
			"Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue);
		AddTerrain("Deep Ocean", $"verydeepunderwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane,
			Difficulty.Automatic, null, CellOutdoorsType.Outdoors, Color.DarkBlue);

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

		#region Containers

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
			Type = "Container",
			Name = "Container_Table",
			Description = "Allows a table to have items 'on' it",
			Definition =
				"<Definition Weight=\"200000\" MaxSize=\"7\" Preposition=\"on\" Closable=\"false\" Transparent=\"true\" />"
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
			Type = "Container",
			Name = "Container_Large_Table",
			Description = "Allows a large table to have items 'on' it",
			Definition =
				"<Definition Weight=\"500000\" MaxSize=\"8\" Preposition=\"on\" Closable=\"false\" Transparent=\"true\" />"
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
			Type = "Container",
			Name = "Container_Small_Table",
			Description = "Allows a small table to have items 'on' it",
			Definition =
				"<Definition Weight=\"50000\" MaxSize=\"6\" Preposition=\"on\" Closable=\"false\" Transparent=\"true\" />"
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
			Type = "Container",
			Name = "Container_Carton",
			Description = "A container for cartons of cigarettes, matches etc",
			Definition =
				$"<Definition Weight=\"250\" MaxSize=\"{(int)SizeCategory.Tiny}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Pocket",
			Description = "A container for pockets in clothes",
			Definition =
				$"<Definition Weight=\"500\" MaxSize=\"{(int)SizeCategory.VerySmall}\" Preposition=\"in\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Pouch",
			Description = "A container for pouches",
			Definition =
				$"<Definition Weight=\"1000\" MaxSize=\"{(int)SizeCategory.VerySmall}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Baggie",
			Description = "A container for see-through baggies",
			Definition =
				$"<Definition Weight=\"1000\" MaxSize=\"{(int)SizeCategory.VerySmall}\" Preposition=\"in\" Closable=\"true\" Transparent=\"true\" />"
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
			Type = "Container",
			Name = "Container_Sachet",
			Description = "A container for single-use sachets",
			Definition =
				$"<Definition Weight=\"1000\" MaxSize=\"{(int)SizeCategory.VerySmall}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" OnceOnly=\"true\"/>"
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
			Type = "Container",
			Name = "Container_Purse",
			Description = "A container for purses or handbags",
			Definition =
				$"<Definition Weight=\"7000\" MaxSize=\"{(int)SizeCategory.Small}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Plate",
			Description = "A container for plates and similar",
			Definition =
				$"<Definition Weight=\"1500\" MaxSize=\"{(int)SizeCategory.VerySmall}\" Preposition=\"on\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Tray",
			Description = "A container for trays, platters, etc",
			Definition =
				$"<Definition Weight=\"7000\" MaxSize=\"{(int)SizeCategory.Small}\" Preposition=\"on\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Tote",
			Description = "A container for tote bags or shoulder bags",
			Definition =
				$"<Definition Weight=\"20000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_PlasticBag",
			Description = "A container for transparent plastic shopping bags",
			Definition =
				$"<Definition Weight=\"10000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"false\" Transparent=\"true\" />"
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
			Type = "Container",
			Name = "Container_Sack",
			Description = "A container for sturdy closable sacks and other similarly sized containers",
			Definition =
				$"<Definition Weight=\"75000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Pack",
			Description = "A container for backpacks and similar",
			Definition =
				$"<Definition Weight=\"75000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Drum",
			Description = "A container for standard sized drums (~55 Gal)",
			Definition =
				$"<Definition Weight=\"250000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Small_Drum",
			Description = "A container for small sized drums (~25 Gal)",
			Definition =
				$"<Definition Weight=\"100000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Quiver",
			Description = "A container for quivers",
			Definition =
				$"<Definition Weight=\"10000\" MaxSize=\"{(int)SizeCategory.Normal}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Hole",
			Description = "A container for holes in the ground",
			Definition =
				$"<Definition Weight=\"2000000\" MaxSize=\"{(int)SizeCategory.VeryLarge}\" Preposition=\"in\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Large_Hole",
			Description = "A container for large holes in the ground",
			Definition =
				$"<Definition Weight=\"5000000\" MaxSize=\"{(int)SizeCategory.Huge}\" Preposition=\"in\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Shipping_Container",
			Description = "A container for standard 20ft shipping containers",
			Definition =
				$"<Definition Weight=\"50000000\" MaxSize=\"{(int)SizeCategory.Enormous}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Shipping_Container_Long",
			Description = "A container for standard 40ft shipping containers",
			Definition =
				$"<Definition Weight=\"100000000\" MaxSize=\"{(int)SizeCategory.Enormous}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Shipping_Container_Large",
			Description = "A container for larger shipping containers",
			Definition =
				$"<Definition Weight=\"200000000\" MaxSize=\"{(int)SizeCategory.Gigantic}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Shipping_Container_Small",
			Description = "A container for small 10ft shipping containers",
			Definition =
				$"<Definition Weight=\"25000000\" MaxSize=\"{(int)SizeCategory.Huge}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Colossal",
			Description = "A container with unthinkably large capacity",
			Definition =
				$"<Definition Weight=\"1000000000\" MaxSize=\"{(int)SizeCategory.Titanic}\" Preposition=\"in\" Closable=\"false\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Coffin",
			Description = "A container for coffins designed to hold a human body",
			Definition =
				$"<Definition Weight=\"250000\" MaxSize=\"{(int)SizeCategory.Large}\" Preposition=\"in\" Closable=\"true\" Transparent=\"false\" />"
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
			Type = "Container",
			Name = "Container_Glass_Casket",
			Description = "A container for see-through glass caskets designed to display a human body",
			Definition =
				$"<Definition Weight=\"200000\" MaxSize=\"{(int)SizeCategory.Large}\" Preposition=\"in\" Closable=\"true\" Transparent=\"true\" />"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion

		#region Liquid Containers

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
			Type = "Liquid Container",
			Name = "LContainer_ShotGlass",
			Description = "A liquid container for a shot glass",
			Definition =
				"<Definition LiquidCapacity=\"0.12\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"1000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_WhiskeyGlass",
			Description = "A liquid container for a whiskey glass (or other small glass)",
			Definition =
				"<Definition LiquidCapacity=\"0.25\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"2000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_DrinkingGlass",
			Description = "A liquid container for a drinking glass (or other normal table glass)",
			Definition =
				"<Definition LiquidCapacity=\"0.450\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"4000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Pony",
			Description = "A liquid container for a pony (1/4 pint glass)",
			Definition =
				"<Definition LiquidCapacity=\"0.142\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"4000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_HalfPint",
			Description = "A liquid container for a half pint glass",
			Definition =
				"<Definition LiquidCapacity=\"0.284\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"4000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Pint",
			Description = "A liquid container for a US pint",
			Definition =
				"<Definition LiquidCapacity=\"0.473\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"6000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_UKPint",
			Description = "A liquid container for a UK pint",
			Definition =
				"<Definition LiquidCapacity=\"0.568\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"6000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Weizen",
			Description = "A liquid container for a weizen glass (european 500ml glass)",
			Definition =
				"<Definition LiquidCapacity=\"0.5\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"7000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Stein",
			Description = "A liquid container for a stein glass (european 1000ml glass)",
			Definition =
				"<Definition LiquidCapacity=\"1.0\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"14000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Yard",
			Description = "A liquid container for a yard glass (2.5 imperial pints)",
			Definition =
				"<Definition LiquidCapacity=\"1.4\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"7000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Jug",
			Description = "A liquid container for a glass jug, generally 40oz",
			Definition =
				"<Definition LiquidCapacity=\"1.14\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"7000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Flute",
			Description = "A liquid container for a flute (champagne glass)",
			Definition =
				"<Definition LiquidCapacity=\"0.180\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"2000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Liqueur",
			Description = "A liquid container for a small liqueur glass",
			Definition =
				"<Definition LiquidCapacity=\"0.06\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"2000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_SherryGlass",
			Description = "A liquid container for a copita, or a glass for drinking sherry",
			Definition =
				"<Definition LiquidCapacity=\"0.180\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"2000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_SmallWineGlass",
			Description = "A liquid container for a small wine glass, such as would typically be used for a white wine",
			Definition =
				"<Definition LiquidCapacity=\"0.240\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"2000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_WineGlass",
			Description =
				"A liquid container for a standard sized wine glass, such as would be typically used for red wine",
			Definition =
				"<Definition LiquidCapacity=\"0.415\" Closable=\"false\" Transparent=\"true\" WeightLimit=\"4000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_BeerBottle",
			Description = "A liquid container for a single-use beer bottle (can't be re-sealed)",
			Definition =
				"<Definition LiquidCapacity=\"0.375\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"6000\" OnceOnly=\"true\"/>"
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
			Type = "Liquid Container",
			Name = "LContainer_SodaCan",
			Description = "A liquid container for a single-use soda can (can't be re-sealed and isn't transparent)",
			Definition =
				"<Definition LiquidCapacity=\"0.375\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"6000\" OnceOnly=\"true\"/>"
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
			Type = "Liquid Container",
			Name = "LContainer_WineBottle",
			Description = "A liquid container for a single-use wine bottle (can't be re-corked)",
			Definition =
				"<Definition LiquidCapacity=\"0.75\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"6000\" OnceOnly=\"true\"/>"
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
			Type = "Liquid Container",
			Name = "LContainer_Decanter",
			Description = "A liquid container for a decanter for a bottle of wine",
			Definition =
				"<Definition LiquidCapacity=\"0.75\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"6000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Flask",
			Description = "A liquid container for a typical hip flask",
			Definition =
				"<Definition LiquidCapacity=\"0.236\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"6000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Canteen",
			Description = "A liquid container for a typical canteen",
			Definition =
				"<Definition LiquidCapacity=\"1.0\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"10000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Cask",
			Description = "A liquid container for a typical wine cask ",
			Definition =
				"<Definition LiquidCapacity=\"0.236\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"6000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Tun",
			Description = "A liquid container for a tun (252 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"960\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"9600000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Butt",
			Description = "A liquid container for a butt (126 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"480\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"4800000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Puncheon",
			Description = "A liquid container for a puncheon (84 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"320\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"3200000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Hogshead",
			Description = "A liquid container for a hogshead (63 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"240\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"2400000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Tierce",
			Description = "A liquid container for a tierce (42 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"160\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"1600000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Barrel",
			Description = "A liquid container for an English barrel (31.5 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"120\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"1200000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Rundlet",
			Description = "A liquid container for a rundlet (18 US Gallon Barrel)",
			Definition =
				"<Definition LiquidCapacity=\"69\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"690000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_GallonCask",
			Description = "A liquid container for a non-see through gallon-sized cask",
			Definition =
				"<Definition LiquidCapacity=\"3.7\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"37000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_GallonBottle",
			Description = "A liquid container for a gallon bottle like a milk bottle",
			Definition =
				"<Definition LiquidCapacity=\"3.7\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"37000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_HalfGallonBottle",
			Description = "A liquid container for a half gallon bottle like a milk bottle",
			Definition =
				"<Definition LiquidCapacity=\"1.85\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"18500\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_QuartBottle",
			Description = "A liquid container for a one quart bottle like a milk bottle",
			Definition =
				"<Definition LiquidCapacity=\"0.946\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"18500\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_PintBottle",
			Description = "A liquid container for a one pint bottle like a milk bottle",
			Definition =
				"<Definition LiquidCapacity=\"0.473\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"47300\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_20ozBottle",
			Description = "A liquid container for a 20oz bottle",
			Definition =
				"<Definition LiquidCapacity=\"0.591\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"59100\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_40ozBottle",
			Description = "A liquid container for a 40oz bottle",
			Definition =
				"<Definition LiquidCapacity=\"1.182\" Closable=\"true\" Transparent=\"true\" WeightLimit=\"118200\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_QuartCarton",
			Description = "A liquid container for a one quart carton",
			Definition =
				"<Definition LiquidCapacity=\"0.946\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"94600\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_PintCarton",
			Description = "A liquid container for a one pint carton",
			Definition =
				"<Definition LiquidCapacity=\"0.471\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"47100\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_HalfPintCarton",
			Description = "A liquid container for a one quart carton",
			Definition =
				"<Definition LiquidCapacity=\"0.237\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"23700\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Waterskin",
			Description = "A liquid container for a standard sized waterskin",
			Definition =
				"<Definition LiquidCapacity=\"1.892\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"189200\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Amphora_Sextarius",
			Description = "A liquid container for an amphora in the roman sextarius (~0.96 pint)",
			Definition =
				"<Definition LiquidCapacity=\"0.546\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"5460\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Amphora_Congius",
			Description = "A liquid container for an amphora in the roman congius (~0.72 gallon)",
			Definition =
				"<Definition LiquidCapacity=\"3.27\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"32700\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Amphora_Urna",
			Description = "A liquid container for an amphora in the roman urna (~2.88 gallon)",
			Definition =
				"<Definition LiquidCapacity=\"13.1\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"131000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Amphora_Quadrantal",
			Description = "A liquid container for an amphora in the roman quadrantal (~5.76 gallon)",
			Definition =
				"<Definition LiquidCapacity=\"26.2\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"262000\" />"
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
			Type = "Liquid Container",
			Name = "LContainer_Amphora_Culeus",
			Description = "A liquid container for an amphora in the roman culeus (~115 gallon)",
			Definition =
				"<Definition LiquidCapacity=\"524\" Closable=\"true\" Transparent=\"false\" WeightLimit=\"524000\" />"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion

		#region Doors

		var doorTraitText = questionAnswers["doortrait"];
		TraitDefinition? doorTrait = null;
		if (!string.IsNullOrEmpty(doorTraitText))
		{
			doorTrait = context.TraitDefinitions.FirstOrDefault(x => x.Name == doorTraitText);
			if (doorTrait == null)
			{
				var example =
					context.TraitDefinitions.FirstOrDefault(x => x.Type == 0 && x.TraitGroup != "Language");
				if (example != null)
				{
					var expression = new TraitExpression
					{
						Name = $"{doorTraitText} Cap",
						Expression = example.Expression.Expression
					};
					context.TraitExpressions.Add(expression);
					doorTrait = new TraitDefinition
					{
						Name = doorTraitText,
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
		}

		if (doorTrait != null)
		{
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
				Type = "Door",
				Name = "Door_Normal",
				Description = "This is an ordinary door that can be smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"false\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Easy}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Insane}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.Normal}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Tough",
				Description = "This is a tough door that can be smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"false\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Insane}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.ExtremelyHard}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Secure",
				Description = "This is a door that can be smashed and uninstalled only from the hinge side",
				Definition =
					$"<Definition SeeThrough=\"false\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.ExtremelyHard}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Admin",
				Description = "This is a door that cannot be removed or smashed by players",
				Definition =
					$"<Definition SeeThrough=\"false\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"false\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Impossible}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"false\" SmashDifficulty=\"{(int)Difficulty.Impossible}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Bad",
				Description = "This is a bad door that can be smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"false\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.VeryHard}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.VeryEasy}\" /></Definition>"
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
				Type = "Door",
				Name = "Gate_Normal",
				Description = "This is an ordinary gate that can be seen and fired through, smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"true\"><InstalledExitDescription>gate</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Easy}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Insane}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.Normal}\" /></Definition>"
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
				Type = "Door",
				Name = "Gate_Tough",
				Description = "This is a tough gate that can be seen and fired through, smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"true\"><InstalledExitDescription>gate</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Insane}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.ExtremelyHard}\" /></Definition>"
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
				Type = "Door",
				Name = "Gate_Secure",
				Description =
					"This is a tough gate that can be seen and fired through, smashed and uninstalled from the hinge side only",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"true\"><InstalledExitDescription>gate</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.ExtremelyHard}\" /></Definition>"
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
				Type = "Door",
				Name = "Gate_Admin",
				Description = "This is a gate that cannot be smashed or uninstalled by players",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"true\"><InstalledExitDescription>gate</InstalledExitDescription><Uninstall CanPlayersUninstall=\"false\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Impossible}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"false\" SmashDifficulty=\"{(int)Difficulty.Impossible}\" /></Definition>"
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
				Type = "Door",
				Name = "Gate_Bad",
				Description = "This is a bad gate that can be seen and fired through, smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"true\"><InstalledExitDescription>gate</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.VeryHard}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.VeryEasy}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Glass",
				Description = "This is a door that can be seen through, smashed and uninstalled",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.VeryHard}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.VeryEasy}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Glass_Secure",
				Description = "This is a door that can be seen through, smashed and uninstalled from hinge side only",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"true\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Normal}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"true\" SmashDifficulty=\"{(int)Difficulty.VeryEasy}\" /></Definition>"
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
				Type = "Door",
				Name = "Door_Glass_Admin",
				Description = "This is a door that can be seen through, but not smashed or uninstalled",
				Definition =
					$"<Definition SeeThrough=\"true\" CanFireThrough=\"false\"><InstalledExitDescription>door</InstalledExitDescription><Uninstall CanPlayersUninstall=\"false\" UninstallDifficultyHingeSide=\"{(int)Difficulty.Impossible}\" UninstallDifficultyNotHingeSide=\"{(int)Difficulty.Impossible}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"false\" SmashDifficulty=\"{(int)Difficulty.Impossible}\" /></Definition>"
			};
			context.GameItemComponentProtos.Add(component);
			context.SaveChanges();
		}
		else
		{
			errors.Add("There was no valid trait supplied for door installation so no door components were created.");
		}

		#endregion

		#region Locks

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
			Type = "Simple Lock",
			Name = "Warded_Lock_Terrible",
			Description = "This is a terrible simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Normal}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.VeryEasy}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Bad",
			Description = "This is a bad simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Easy}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Easy}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Normal",
			Description = "This is a normal simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Hard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Normal}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Good",
			Description = "This is a good simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.VeryHard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Hard}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Excellent",
			Description = "This is an excellent simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.ExtremelyHard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.VeryHard}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Master",
			Description = "This is a masterful simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.ExtremelyHard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.ExtremelyHard}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Lock",
			Name = "Warded_Lock_Legendary",
			Description = "This is a legendary simple lock in the 'warded' type (most pre-modern systems)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Insane}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Insane}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
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
			Type = "Simple Key",
			Name = "Warded_Key",
			Description = "This is a key for locks in the 'warded' type (most pre-modern systems)",
			Definition =
				@"<Definition>
  <LockType>Warded Lock</LockType>
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
			Type = "Latch",
			Name = "Latch_Terrible",
			Description = "This is a terrible quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.ExtremelyEasy}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Easy}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Bad",
			Description = "This is a bad quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.VeryEasy}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Normal}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Normal",
			Description = "This is a normal quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Easy}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Hard}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Good",
			Description = "This is a good quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Hard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Hard}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Excellent",
			Description = "This is an excellent quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.VeryHard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.VeryHard}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Master",
			Description = "This is a masterful quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.ExtremelyHard}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.VeryHard}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Legendary",
			Description = "This is a legendary quality simple latch (one-sided lock)",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Insane}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.ExtremelyHard}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
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
			Type = "Latch",
			Name = "Latch_Admin",
			Description = "This is a simple latch (one-sided lock) that cannot be picked or forced",
			Definition =
				$@"<Definition>
  <ForceDifficulty>{(int)Difficulty.Impossible}</ForceDifficulty>
  <PickDifficulty>{(int)Difficulty.Impossible}</PickDifficulty>
  <LockEmote><![CDATA[@ latch|latches $1$?2| on $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlatch|unlatches $1$?2| on $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ open|opens]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ close|closes]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is latched from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlatched from the other side.]]></UnlockEmoteOtherSide>
</Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

		#endregion

		#region Writing Implements

		var holdable = context.GameItemComponentProtos.First(x => x.Type == "Holdable");
		var stack = context.GameItemComponentProtos.First(x => x.Name == "Stack_Number");
		var paperMaterial = context.Materials.First(x => x.Name == "Paper");

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
			Type = "PaperSheet",
			Name = "Paper_A4",
			Description = "This is a sheet of paper in A4 size (~ US Letter size)",
			Definition =
				@"<Definition>
   <MaximumCharacterLengthOfText>4160</MaximumCharacterLengthOfText>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

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
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a4paper, GameItemComponent = holdable });
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a4paper, GameItemComponent = stack });
		a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a4paper, GameItemComponent = component });
		context.GameItemProtos.Add(a4paper);
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
			Type = "PaperSheet",
			Name = "Paper_A3",
			Description = "This is a sheet of paper in A3 size (~ US Ledger size)",
			Definition =
				@"<Definition>
   <MaximumCharacterLengthOfText>8320</MaximumCharacterLengthOfText>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

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
			FullDescription =
				"This is a large sheet of plain, unlined paper approximately 12 inches by 16 inches in size."
		};
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a3paper, GameItemComponent = holdable });
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a3paper, GameItemComponent = stack });
		a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a3paper, GameItemComponent = component });
		context.GameItemProtos.Add(a3paper);
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
			Type = "PaperSheet",
			Name = "Paper_A5",
			Description = "This is a sheet of paper in A5 size (~ US Half Letter size)",
			Definition =
				@"<Definition>
   <MaximumCharacterLengthOfText>2080</MaximumCharacterLengthOfText>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		context.SaveChanges();

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
			Size = (int)SizeCategory.Tiny,
			Weight = 0.5,
			ReadOnly = false,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a small sheet of paper",
			FullDescription =
				"This is a small sheet of plain, unlined paper approximately 5 inches by 8 inches in size."
		};
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a5paper, GameItemComponent = holdable });
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a5paper, GameItemComponent = stack });
		a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{ GameItemProto = a5paper, GameItemComponent = component });
		context.GameItemProtos.Add(a5paper);
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
			Type = "Book",
			Name = "Book_20_Page",
			Description = "This is a 20 page book of A4 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a4paper.Id}</PaperProto>
   <PageCount>20</PageCount>
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
			Type = "Book",
			Name = "Book_40_Page",
			Description = "This is a 40 page book of A4 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a4paper.Id}</PaperProto>
   <PageCount>40</PageCount>
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
			Type = "Book",
			Name = "Book_100_Page",
			Description = "This is a 100 page book of A4 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a4paper.Id}</PaperProto>
   <PageCount>100</PageCount>
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
			Type = "Book",
			Name = "Book_Small_20_Page",
			Description = "This is a 20 page book of A5 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a5paper.Id}</PaperProto>
   <PageCount>20</PageCount>
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
			Type = "Book",
			Name = "Book_Small_40_Page",
			Description = "This is a 40 page book of A5 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a5paper.Id}</PaperProto>
   <PageCount>40</PageCount>
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
			Type = "Book",
			Name = "Book_Small_100_Page",
			Description = "This is a 100 page book of A5 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a5paper.Id}</PaperProto>
   <PageCount>100</PageCount>
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
			Type = "Book",
			Name = "Book_Large_20_Page",
			Description = "This is a 20 page book of A3 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a3paper.Id}</PaperProto>
   <PageCount>20</PageCount>
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
			Type = "Book",
			Name = "Book_Large_40_Page",
			Description = "This is a 40 page book of A3 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a3paper.Id}</PaperProto>
   <PageCount>40</PageCount>
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
			Type = "Book",
			Name = "Book_Large_100_Page",
			Description = "This is a 100 page book of A3 pages",
			Definition =
				$@"<Definition>
   <PaperProto>{a3paper.Id}</PaperProto>
   <PageCount>100</PageCount>
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
			Name = "Biro_Black",
			Description = "This is a standard black biro pen",
			Definition =
				$@"<Definition>
   <Colour>{context.Colours.First(x => x.Name == "black").Id}</Colour>
   <TotalUses>110000</TotalUses>
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
			Name = "Biro_Blue",
			Description = "This is a standard blue biro pen",
			Definition =
				$@"<Definition>
   <Colour>{context.Colours.First(x => x.Name == "blue").Id}</Colour>
   <TotalUses>110000</TotalUses>
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
			Name = "Biro_Red",
			Description = "This is a standard red biro pen",
			Definition =
				$@"<Definition>
   <Colour>{context.Colours.First(x => x.Name == "red").Id}</Colour>
   <TotalUses>110000</TotalUses>
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
			Name = "Pencil_Black",
			Description = "This is a standard black pencil",
			Definition =
				$@"<Definition>
   <Colour>{context.Colours.First(x => x.Name == "black").Id}</Colour>
   <UsesBeforeSharpening>11000</UsesBeforeSharpening>
   <TotalUses>220000</TotalUses>
 </Definition>"
		};
		context.GameItemComponentProtos.Add(component);
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
			Name = "Torch_Infinite",
			Description = "Turns an item into an ever-burning torch.",
			Definition = @"<Definition>
   <IlluminationProvided>25</IlluminationProvided>
   <SecondsOfFuel>-1</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ turn|turns on $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ turn|turns off $1]]></ExtinguishEmote>
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
			Name = "Torch_1Hour",
			Description = "Turns an item into a torch that burns for an hour.",
			Definition = @"<Definition>
   <IlluminationProvided>25</IlluminationProvided>
   <SecondsOfFuel>3600</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ turn|turns on $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ turn|turns off $1]]></ExtinguishEmote>
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
			Name = "Torch_2Hour",
			Description = "Turns an item into a torch that burns for two hours.",
			Definition = @"<Definition>
   <IlluminationProvided>25</IlluminationProvided>
   <SecondsOfFuel>7200</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ turn|turns on $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ turn|turns off $1]]></ExtinguishEmote>
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
			Name = "Torch_3Hour",
			Description = "Turns an item into a torch that burns for 3 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>25</IlluminationProvided>
   <SecondsOfFuel>10800</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ turn|turns on $1]]></LightEmote>
   <ExtinguishEmote><![CDATA[@ turn|turns off $1]]></ExtinguishEmote>
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
			Name = "Candle",
			Description = "Turns an item into a candle that burns dimly for 12 hours.",
			Definition = @"<Definition>
   <IlluminationProvided>5</IlluminationProvided>
   <SecondsOfFuel>43200</SecondsOfFuel>
   <RequiresIgnitionSource>false</RequiresIgnitionSource>
   <LightEmote><![CDATA[@ light|lights on $1]]></LightEmote>
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
   <LightEmote><![CDATA[@ light|lights on $1]]></LightEmote>
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
   <LightEmote><![CDATA[@ light|lights on $1]]></LightEmote>
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
   <LightEmote><![CDATA[@ light|lights on $1]]></LightEmote>
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

		var materials = context.Materials.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var skills = context.TraitDefinitions.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
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
			Type = "RepairKit",
			Name = "Repair_Cloth",
			Description = "Turns an item into a repair kit that can repair cloth items",
			Definition = @$"<Definition>
  <MaximumSeverity>7</MaximumSeverity>
  <RepairPoints>500</RepairPoints>
  <CheckTrait>{skills[questionAnswers["repairskillcloth"]].Id}</CheckTrait>
  <CheckBonus>0</CheckBonus>
  <Echoes>
    <Echo><![CDATA[$0 take|takes up $2, rifling through it for the necessary tools to fix $1]]></Echo>
    <Echo><![CDATA[$0 arrange|arranges a cloth patch over the damage, stitching it onto $1 with a needle and thread from $2]]></Echo>
    <Echo><![CDATA[$0 continue|continues stitching until the damage is less noticeable, and $1 seems sturdier for it]]></Echo>
    <Echo><![CDATA[$0 place|places the tools back within $2 and pack|packs it away.]]></Echo>
  </Echoes>
  <Materials>
    <Material>{materials["broadcloth"]}</Material>
	<Material>{materials["burlap"]}</Material>
	<Material>{materials["canvas"]}</Material>
	<Material>{materials["cotton"]}</Material>
	<Material>{materials["denim"]}</Material>
	<Material>{materials["felt"]}</Material>
	<Material>{materials["fur"]}</Material>
	<Material>{materials["hemp"]}</Material>
	<Material>{materials["hessian"]}</Material>
	<Material>{materials["jute"]}</Material>
	<Material>{materials["linen"]}</Material>
	<Material>{materials["silk"]}</Material>
	<Material>{materials["tweed"]}</Material>
	<Material>{materials["wool"]}</Material>
	<Material>{materials["cashmere"]}</Material>
	<Material>{materials["mohair"]}</Material>
  </Materials>
  <DamageTypes>
    <DamageType>2</DamageType>
    <DamageType>1</DamageType>
    <DamageType>3</DamageType>
	<DamageType>4</DamageType>
    <DamageType>16</DamageType>
    <DamageType>9</DamageType>
    <DamageType>10</DamageType>
    <DamageType>15</DamageType>
    <DamageType>16</DamageType>
    <DamageType>8</DamageType>
    <DamageType>0</DamageType>
    <DamageType>18</DamageType>
    <DamageType>20</DamageType>
  </DamageTypes>
  <Tags />
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
			Type = "RepairKit",
			Name = "Repair_Cloth_Minor",
			Description = "Turns an item into a field repair kit that can repair minor damage to cloth items",
			Definition = @$"<Definition>
  <MaximumSeverity>4</MaximumSeverity>
  <RepairPoints>100</RepairPoints>
  <CheckTrait>{skills[questionAnswers["repairskillcloth"]].Id}</CheckTrait>
  <CheckBonus>2</CheckBonus>
  <Echoes>
    <Echo><![CDATA[$0 take|takes up $2, rifling through it for the necessary tools to fix $1]]></Echo>
    <Echo><![CDATA[$0 arrange|arranges a cloth patch over the damage, stitching it onto $1 with a needle and thread from $2]]></Echo>
    <Echo><![CDATA[$0 continue|continues stitching until the damage is less noticeable, and $1 seems sturdier for it]]></Echo>
    <Echo><![CDATA[$0 place|places the tools back within $2 and pack|packs it away.]]></Echo>
  </Echoes>
  <Materials>
    <Material>{materials["broadcloth"]}</Material>
	<Material>{materials["burlap"]}</Material>
	<Material>{materials["canvas"]}</Material>
	<Material>{materials["cotton"]}</Material>
	<Material>{materials["denim"]}</Material>
	<Material>{materials["felt"]}</Material>
	<Material>{materials["fur"]}</Material>
	<Material>{materials["hemp"]}</Material>
	<Material>{materials["hessian"]}</Material>
	<Material>{materials["jute"]}</Material>
	<Material>{materials["linen"]}</Material>
	<Material>{materials["silk"]}</Material>
	<Material>{materials["tweed"]}</Material>
	<Material>{materials["wool"]}</Material>
	<Material>{materials["cashmere"]}</Material>
	<Material>{materials["mohair"]}</Material>
  </Materials>
  <DamageTypes>
    <DamageType>2</DamageType>
    <DamageType>1</DamageType>
    <DamageType>3</DamageType>
	<DamageType>4</DamageType>
    <DamageType>16</DamageType>
    <DamageType>9</DamageType>
    <DamageType>10</DamageType>
    <DamageType>15</DamageType>
    <DamageType>16</DamageType>
    <DamageType>8</DamageType>
    <DamageType>0</DamageType>
    <DamageType>18</DamageType>
    <DamageType>20</DamageType>
  </DamageTypes>
  <Tags />
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
			Type = "RepairKit",
			Name = "Repair_Leather",
			Description = "Turns an item into a repair kit that can repair minor damage to leather items",
			Definition = @$"<Definition>
  <MaximumSeverity>7</MaximumSeverity>
  <RepairPoints>500</RepairPoints>
  <CheckTrait>{skills[questionAnswers["repairskillleather"]].Id}</CheckTrait>
  <CheckBonus>0</CheckBonus>
  <Echoes>
    <Echo><![CDATA[$0 take|takes up $2, rifling through it for the necessary tools to fix $1]]></Echo>
    <Echo><![CDATA[$0 arrange|arranges a leather patch over the damage, stitching it onto $1 with a needle and thread from $2]]></Echo>
    <Echo><![CDATA[$0 continue|continues stitching until the damage is less noticeable, and $1 seems sturdier for it]]></Echo>
    <Echo><![CDATA[$0 place|places the tools back within $2 and pack|packs it away.]]></Echo>
  </Echoes>
  <Materials>
    <Material>{materials["animal skin"]}</Material>
	<Material>{materials["cow hide"]}</Material>
	<Material>{materials["deer hide"]}</Material>
	<Material>{materials["bear hide"]}</Material>
	<Material>{materials["dog hide"]}</Material>
	<Material>{materials["cat hide"]}</Material>
	<Material>{materials["fox hide"]}</Material>
	<Material>{materials["pig hide"]}</Material>
	<Material>{materials["wolf hide"]}</Material>
	<Material>{materials["snake hide"]}</Material>
	<Material>{materials["alligator hide"]}</Material>
	<Material>{materials["crocodile hide"]}</Material>
	<Material>{materials["lion hide"]}</Material>
	<Material>{materials["tiger hide"]}</Material>
	<Material>{materials["rabbit hide"]}</Material>
	<Material>{materials["small mammal hide"]}</Material>
	<Material>{materials["leather"]}</Material>
	<Material>{materials["cow leather"]}</Material>
	<Material>{materials["deer leather"]}</Material>
	<Material>{materials["bear leather"]}</Material>
	<Material>{materials["dog leather"]}</Material>
	<Material>{materials["cat leather"]}</Material>
	<Material>{materials["fox leather"]}</Material>
	<Material>{materials["pig leather"]}</Material>
	<Material>{materials["wolf leather"]}</Material>
	<Material>{materials["snake leather"]}</Material>
	<Material>{materials["alligator leather"]}</Material>
	<Material>{materials["crocodile leather"]}</Material>
	<Material>{materials["lion leather"]}</Material>
	<Material>{materials["tiger leather"]}</Material>
	<Material>{materials["rabbit leather"]}</Material>
	<Material>{materials["small mammal leather"]}</Material>
  </Materials>
  <DamageTypes>
    <DamageType>2</DamageType>
    <DamageType>1</DamageType>
    <DamageType>3</DamageType>
	<DamageType>4</DamageType>
    <DamageType>16</DamageType>
    <DamageType>9</DamageType>
    <DamageType>10</DamageType>
    <DamageType>15</DamageType>
    <DamageType>16</DamageType>
    <DamageType>8</DamageType>
    <DamageType>0</DamageType>
    <DamageType>18</DamageType>
    <DamageType>20</DamageType>
  </DamageTypes>
  <Tags />
</Definition>"
		};
		context.GameItemComponentProtos.Add(component);
		#endregion

		#region Smokeables
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
			{ FutureProg = smokeProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)FutureProgVariableTypes.Character });
		smokeProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = smokeProg, ParameterIndex = 1, ParameterName = "item", ParameterType = (long)FutureProgVariableTypes.Item });
		context.FutureProgs.Add(smokeProg);

		var prog = new FutureProg
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
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)FutureProgVariableTypes.Character });
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 1, ParameterName = "item", ParameterType = (long)FutureProgVariableTypes.Item });
		context.FutureProgs.Add(prog);
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
   <OnDragProg>109</OnDragProg>
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

	private void SeedAI(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		if (context.ArtificialIntelligences.Any(x => x.Name == "CommandableOwner"))
		{
			errors.Add("Detected that AIs were already installed. Did not seed any AIs.");
			return;
		}

		context.VariableDefinitions.Add(new VariableDefinition
		{
			ContainedType = (long)FutureProgVariableTypes.Number,
			OwnerType = (long)FutureProgVariableTypes.Character,
			Property = "npcownerid"
		});
		context.VariableDefaults.Add(new VariableDefault
		{
			OwnerType = (long)FutureProgVariableTypes.Character,
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
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		ownerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = ownerProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)FutureProgVariableTypes.Text
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
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		cantCommandProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)FutureProgVariableTypes.Text
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
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		isClanBrotherProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isClanBrotherProg,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)FutureProgVariableTypes.Text
		});
		context.FutureProgs.Add(isClanBrotherProg);

		var cantCommandProgClanBrother = new FutureProg
		{
			FunctionText = @"return ""You do not outrank "" + HowSeen(@ch, @tch, false, true) + "" in any clans.""",
			FunctionName = "WhyCantCommandNPCClanOutranks",
			Category = "AI",
			Subcategory = "Commands",
			FunctionComment = "Returns an error message when a player cannot command an NPC they do not outrank",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		cantCommandProgClanBrother.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cantCommandProgClanBrother,
			ParameterIndex = 2,
			ParameterName = "cmd",
			ParameterType = (long)FutureProgVariableTypes.Text
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
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardWillOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardWillOpen,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ReturnType = (long)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardDelay,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
		});
		context.FutureProgs.Add(doorguardDelay);

		var doorguardCloseDelay = new FutureProg
		{
			FunctionText = @"return 10000",
			FunctionName = "DoorguardCloseDelay",
			Category = "AI",
			Subcategory = "Doorguard",
			FunctionComment = "A delay in milliseconds between opening the door and closing the door",
			ReturnType = (long)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 0,
			ParameterName = "guard",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardCloseDelay.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDelay,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardOpenDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardOpenDoor,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardOpenDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardOpenDoor,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardCloseDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDoor,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorguardCloseDoor.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorguardCloseDoor,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorGuardWontOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWontOpen,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorGuardWontOpen.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWontOpen,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorGuardWitnessStop.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWitnessStop,
			ParameterIndex = 1,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		doorGuardWitnessStop.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = doorGuardWitnessStop,
			ParameterIndex = 2,
			ParameterName = "exit",
			ParameterType = (long)FutureProgVariableTypes.Exit
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
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0
		};
		aggressorWillAttack.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = aggressorWillAttack,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		aggressorWillAttack.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = aggressorWillAttack,
			ParameterIndex = 1,
			ParameterName = "tch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		context.FutureProgs.Add(aggressorWillAttack);

		context.SaveChanges();

		ai = new ArtificialIntelligence
		{
			Name = "AggressiveToAllOtherSpecies",
			Type = "Aggressor",
			Definition = @$"<Definition>
   <WillAttackProg>{aggressorWillAttack.Id}</WillAttackProg>
   <EngageDelayDiceExpression>1d3+3</EngageDelayDiceExpression>
   <EngageEmote><![CDATA[@ move|moves aggressively towards $1]]></EngageEmote>
 </Definition>"
		};
		context.ArtificialIntelligences.Add(ai);
		context.SaveChanges();
	}

	private Dictionary<string, MudSharp.Models.Tag> _tags = new(StringComparer.OrdinalIgnoreCase);

	private void AddTag(FuturemudDatabaseContext context, string name, string parent)
	{
		var tag = new MudSharp.Models.Tag
		{
			Name = name,
			Parent = _tags.ValueOrDefault(parent, null)
		};
		context.Tags.Add(tag);
	}

	private void SeedTags(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		AddTag(context, "Functions", "");

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
		AddTag(context, "Strap", "Tie");
		AddTag(context, "Tie Wire", "Tie");
		AddTag(context, "Tie Rope", "Tie");
		AddTag(context, "Band", "Tie");
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
		AddTag(context, "Boning Knife", "Cooking Knife");
		AddTag(context, "Utility Knife", "Cooking Knife");
		AddTag(context, "Oyster Knife", "Cooking Knife");
		AddTag(context, "Cleaver", "Cooking Knife");
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
		AddTag(context, "Rivet Gun", "Construction Tools");
		AddTag(context, "Construction Stapler", "Construction Tools");
		AddTag(context, "Mallet", "Construction Tools");
		AddTag(context, "Wrench", "Construction Tools");
		AddTag(context, "Torque Wrench", "Wrench");
		AddTag(context, "Saw", "Construction Tools");
		AddTag(context, "Chisel", "Construction Tools");
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
		
		AddTag(context, "Navigational Tool", "Scientific Tools");
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

		context.SaveChanges();
	}
}