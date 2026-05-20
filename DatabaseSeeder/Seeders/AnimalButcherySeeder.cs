#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Work.Butchering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public sealed class AnimalButcherySeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Stock Butchery";
	private const string RottenMeatShortDescription = "some rotten meat";
	private const int SpoilSeconds = 86400;
	private const int MaximumStockItemPrototypeCount = 220;

	private const string ButcheryOutputTag = "Butchery Output";
	private const string RawMeatCutTag = "Raw Meat Cut";
	private const string RawHideTag = "Raw Hide";
	private const string OffalTag = "Offal";
	private const string TrophyPartTag = "Trophy Part";
	private const string VenomOrganTag = "Venom Organ";
	private const string CraftingAnimalProductTag = "Crafting Animal Product";

	private static readonly string[] Head = ["head"];
	private static readonly string[] Mouth = ["mouth", "beak", "mandibles"];
	private static readonly string[] Torso = ["abdomen", "thorax", "cephalothorax", "carapace", "body", "mantle", "ubody", "mbody", "stock"];
	private static readonly string[] RightFore = ["rfpaw", "rfhoof", "rfrontflipper", "rclaw", "rtalons", "rleg1", "rwingbase"];
	private static readonly string[] LeftFore = ["lfpaw", "lfhoof", "lfrontflipper", "lclaw", "ltalons", "lleg1", "lwingbase"];
	private static readonly string[] RightHind = ["rrpaw", "rrhoof", "rhindflipper", "rfoot", "rrclaw", "rleg4"];
	private static readonly string[] LeftHind = ["lrpaw", "lrhoof", "lhindflipper", "lfoot", "lrclaw", "lleg4"];
	private static readonly string[] RightWing = ["rwing", "rwingbase"];
	private static readonly string[] LeftWing = ["lwing", "lwingbase"];
	private static readonly string[] Tail = ["tail", "ltail", "fluke", "caudalfin", "tailfan", "peduncle", "lbody", "hindbody"];
	private static readonly string[] RightHorn = ["rhorn", "rantler"];
	private static readonly string[] LeftHorn = ["lhorn", "lantler"];
	private static readonly string[] Horn = ["horn", "rhorn", "lhorn", "rantler", "lantler"];
	private static readonly string[] RightTusk = ["rtusk"];
	private static readonly string[] LeftTusk = ["ltusk"];
	private static readonly string[] RightFang = ["rfang", "mouth"];
	private static readonly string[] LeftFang = ["lfang", "mouth"];
	private static readonly string[] Stinger = ["stinger"];
	private static readonly string[] Shell = ["carapace", "body"];

	private const string GlobalOffal = "global:offal";
	private const string GlobalBones = "global:bones";
	private const string GlobalFat = "global:fat";
	private const string GlobalVenomGland = "global:venom-gland";
	private const string GlobalTeeth = "global:teeth";
	private const string GlobalClaws = "global:claws";
	private const string GlobalHorns = "global:horns";
	private const string GlobalAntlers = "global:antlers";
	private const string GlobalTusks = "global:tusks";
	private const string GlobalFeathers = "global:feathers";
	private const string GlobalBeak = "global:beak";
	private const string GlobalScales = "global:scales";
	private const string GlobalChitin = "global:chitin";
	private const string GlobalShell = "global:shell";
	private const string GlobalFins = "global:fins";
	private const string GlobalBlubber = "global:blubber";
	private const string GlobalInkSac = "global:ink-sac";
	private const string GlobalSilkGland = "global:silk-gland";
	private const string GlobalIchor = "global:ichor";
	private const string GlobalToxinSac = "global:toxin-sac";
	private const string GlobalBaleen = "global:baleen";
	private const string GlobalMandibles = "global:mandibles";
	private const string GlobalStinger = "global:stinger";
	private const string GlobalFireGland = "global:fire-gland";
	private const string GlobalPhoenixAsh = "global:phoenix-ash";

	private static readonly IReadOnlyDictionary<string, StockButcheryFamilySpec> Families = BuildFamilies();
	private static readonly IReadOnlyDictionary<string, StockButcheryItemSpec> GlobalItems = BuildGlobalItems();
	private static readonly IReadOnlyDictionary<string, StockButcheryMaterialSpec> SignatureMaterials = BuildSignatureMaterials();

	private static readonly HashSet<string> IncludedMythicalBeasts = new(StringComparer.OrdinalIgnoreCase)
	{
		"Dragon",
		"Griffin",
		"Hippogriff",
		"Unicorn",
		"Pegasus",
		"Warg",
		"Dire-Wolf",
		"Dire-Bear",
		"Eastern Dragon",
		"Manticore",
		"Wyvern",
		"Fell Beast",
		"Phoenix",
		"Basilisk",
		"Cockatrice",
		"Giant Beetle",
		"Giant Ant",
		"Giant Mantis",
		"Giant Spider",
		"Giant Scorpion",
		"Giant Centipede",
		"Giant Worm",
		"Colossal Worm",
		"Ankheg",
		"Hippocamp",
		"Pegacorn",
		"Qilin",
		"Garuda",
		"Giant Eagle",
		"Bunyip",
		"Yacumama"
	};

	private static readonly HashSet<string> ExcludedMythicalRaces = new(StringComparer.OrdinalIgnoreCase)
	{
		"Minotaur",
		"Naga",
		"Mermaid",
		"Selkie",
		"Myconid",
		"Plantfolk",
		"Ent",
		"Huorn",
		"Dryad",
		"Owlkin",
		"Avian Person",
		"Centaur"
	};

	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
		Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		[];

	public int SortOrder => 410;
	public string Name => "Animal Butchery";
	public string Tagline => "Stock butchery profiles, carcass cuts, hides and trophy parts for animal races";

	public string FullDescription =>
		@"This package installs default butchery profiles for the stock animal catalogue and non-humanoid mythical beasts.

It creates reusable butchery outputs, raw carcass cuts, hides, glands, trophies and family-level products while keeping the item catalogue intentionally compact. Soft organic outputs spoil into the generic rotten meat item, while durable materials such as bone, tusk, horn, shell, chitin, feather and scale remain stable crafting products.";

	public bool SafeToRunMoreThanOnce => true;

	internal static IReadOnlyDictionary<string, StockButcheryFamilySpec> FamiliesForTesting => Families;
	internal static IReadOnlyDictionary<string, StockButcheryItemSpec> GlobalItemsForTesting => GlobalItems;
	internal static IReadOnlyDictionary<string, StockButcheryMaterialSpec> SignatureMaterialsForTesting => SignatureMaterials;
	internal static IReadOnlyCollection<string> IncludedMythicalBeastsForTesting => IncludedMythicalBeasts;
	internal static IReadOnlyCollection<string> ExcludedMythicalRacesForTesting => ExcludedMythicalRaces;
	internal static int MaximumStockItemPrototypeCountForTesting => MaximumStockItemPrototypeCount;
	internal static int SpoilSecondsForTesting => SpoilSeconds;
	internal static string RottenMeatShortDescriptionForTesting => RottenMeatShortDescription;

	internal static IReadOnlyList<StockButcheryItemSpec> StockItemSpecsForTesting =>
		GlobalItems.Values
			.Concat(Families.Values.SelectMany(x => x.Items))
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToList();

	internal static bool TryGetFamilyForTesting(string raceName, string bodyKey, bool mythical, out string familyKey)
	{
		return TryGetFamilyKey(raceName, bodyKey, mythical, out familyKey);
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!PrerequisitesMet(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		List<RaceAssignment> assignments = GetRaceAssignments(context).ToList();
		if (!assignments.Any())
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		if (HasMissingStockData(context, assignments))
		{
			return HasAnyStockData(context) ? ShouldSeedResult.ExtraPackagesAvailable : ShouldSeedResult.ReadyToInstall;
		}

		return ShouldSeedResult.MayAlreadyBeInstalled;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!PrerequisitesMet(context))
		{
			return "The Animal Butchery package cannot be installed because its prerequisites are not present.";
		}

		var assignments = GetRaceAssignments(context).ToList();
		if (!assignments.Any())
		{
			return "There are no stock animal or beast-only mythical races available for the Animal Butchery package.";
		}

		EnsureButcheryTags(context);
		foreach (var material in SignatureMaterials.Values)
		{
			EnsureMaterial(context, material);
		}
		context.SaveChanges();

		var itemProtos = EnsureItems(context);
		var productsByFamily = EnsureProducts(context, assignments, itemProtos);
		var profiles = EnsureProfiles(context, productsByFamily);
		AssignProfiles(context, assignments, profiles);
		context.SaveChanges();

		var assignedCount = assignments.Count(x => x.Race.RaceButcheryProfileId == null ||
		                                         IsStockProfile(x.Race.RaceButcheryProfile));

		return
			$"The operation completed successfully. Installed or refreshed {profiles.Count} stock butchery profiles for {assignedCount} stock races.";
	}

	private static bool PrerequisitesMet(FuturemudDatabaseContext context)
	{
		return context.Accounts.Any() &&
		       context.GameItemComponentProtos.Any(x => x.Name == "Holdable") &&
		       context.GameItemComponentProtos.Any(x => x.Name == "Destroyable_Misc") &&
		       context.GameItemComponentProtos.Any(x => x.Name == "Stack_Pile") &&
		       context.GameItemProtos.Any(x => x.ShortDescription == RottenMeatShortDescription) &&
		       context.Tags.Any(x => x.Name == "Animal Product") &&
		       context.Tags.Any(x => x.Name == "Cutting") &&
		       context.Materials.Any(x => x.Name == "meat") &&
		       context.Materials.Any(x => x.Name == "bone") &&
		       context.Materials.Any(x => x.Name == "animal skin") &&
		       context.TraitDefinitions.Any(x => x.Name == "Butchery") &&
		       context.Races.Any() &&
		       context.BodyProtos.Any();
	}

	private static bool HasAnyStockData(FuturemudDatabaseContext context)
	{
		return context.RaceButcheryProfiles.Any(x => x.Name.StartsWith(StockPrefix)) ||
		       context.ButcheryProducts.Any(x => x.Name.StartsWith(StockPrefix)) ||
		       context.GameItemProtos.Any(x => x.GameItemProtosTags.Any(tag => tag.Tag.Name == ButcheryOutputTag));
	}

	private static bool HasMissingStockData(FuturemudDatabaseContext context, IReadOnlyList<RaceAssignment> assignments)
	{
		var requiredFamilies = assignments
			.Where(x => x.Race.RaceButcheryProfileId == null || IsStockProfile(x.Race.RaceButcheryProfile))
			.Select(x => x.Family.Key)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (requiredFamilies.Any(familyKey =>
			    !context.RaceButcheryProfiles.Any(x => x.Name == ProfileName(Families[familyKey]))))
		{
			return true;
		}

		if (assignments.Any(x => x.Race.RaceButcheryProfileId == null))
		{
			return true;
		}

		var requiredItems = GlobalItems.Values
			.Concat(requiredFamilies.SelectMany(x => Families[x].Items))
			.Select(x => x.ShortDescription)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return requiredItems.Any(item => !context.GameItemProtos.Any(x => x.ShortDescription == item));
	}

	private static IReadOnlyDictionary<string, StockButcheryMaterialSpec> BuildSignatureMaterials()
	{
		return new Dictionary<string, StockButcheryMaterialSpec>(StringComparer.OrdinalIgnoreCase)
		{
			["dragon meat"] = new("dragon meat", MaterialBehaviourType.Meat, 1.35, true, "Meat"),
			["draconic hide"] = new("draconic hide", MaterialBehaviourType.Skin, 1.55, true, "Thick Animal Skin"),
			["draconic scale"] = new("draconic scale", MaterialBehaviourType.Scale, 1.35, true, "Animal Product"),
			["giant arthropod chitin"] = new("giant arthropod chitin", MaterialBehaviourType.Shell, 1.35, true, "Animal Product"),
			["mythic feather"] = new("mythic feather", MaterialBehaviourType.Feather, 1.15, true, "Animal Product"),
			["blubber"] = new("blubber", MaterialBehaviourType.Grease, 0.92, true, "Animal Product")
		};
	}

	private static IReadOnlyDictionary<string, StockButcheryItemSpec> BuildGlobalItems()
	{
		return new Dictionary<string, StockButcheryItemSpec>(StringComparer.OrdinalIgnoreCase)
		{
			[GlobalOffal] = Item(GlobalOffal, "offal", "a heap of raw animal offal",
				"A wet heap of organs, connective tissue and dark meat has been gathered together from a carcass.",
				SizeCategory.Normal, 1800, 2M, "meat", true, OffalTag),
			[GlobalBones] = Item(GlobalBones, "bones", "a bundle of cleaned animal bones",
				"A bundle of pale animal bones has been tied together for later use in crafting, stock or disposal.",
				SizeCategory.Normal, 2200, 3M, "bone", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalFat] = Item(GlobalFat, "fat", "a slab of raw animal fat",
				"A pale slab of animal fat and connective tissue has been trimmed away from a carcass.",
				SizeCategory.Small, 1000, 2M, "meat", true, OffalTag),
			[GlobalVenomGland] = Item(GlobalVenomGland, "gland", "a raw venom gland",
				"A small, delicate venom gland has been cut free with enough tissue around it to keep it intact.",
				SizeCategory.VerySmall, 80, 12M, "meat", true, VenomOrganTag),
			[GlobalTeeth] = Item(GlobalTeeth, "teeth", "a handful of animal teeth",
				"A handful of teeth has been pulled from a carcass and cleaned of the worst clinging tissue.",
				SizeCategory.VerySmall, 120, 4M, "tooth", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalClaws] = Item(GlobalClaws, "claws", "a set of animal claws",
				"A set of claws has been trimmed from a carcass, each one still hard and sharp enough for craft use.",
				SizeCategory.VerySmall, 160, 5M, "claw", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalHorns] = Item(GlobalHorns, "horns", "a pair of animal horns",
				"A pair of horns has been cut free from the skull and cleaned for later crafting or display.",
				SizeCategory.Small, 900, 15M, "horn", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalAntlers] = Item(GlobalAntlers, "antlers", "a pair of animal antlers",
				"A branching pair of antlers has been removed in one piece from the skull.",
				SizeCategory.Normal, 1400, 18M, "antler", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalTusks] = Item(GlobalTusks, "tusks", "a pair of animal tusks",
				"A pair of tusks has been worked free, still carrying faint marks where they sat in the jaw.",
				SizeCategory.Normal, 2600, 30M, "tusk", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalFeathers] = Item(GlobalFeathers, "feathers", "a bundle of usable feathers",
				"A bundle of feathers has been sorted out, with the best shafts and vanes kept for later use.",
				SizeCategory.Small, 120, 4M, "feather", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalBeak] = Item(GlobalBeak, "beak", "a cleaned animal beak",
				"A hard beak has been removed and cleaned for ornament, tool work or other craft use.",
				SizeCategory.VerySmall, 180, 5M, "beak", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalScales] = Item(GlobalScales, "scales", "a pile of usable scales",
				"A pile of scales has been scraped free and sorted by the pieces least cracked by the work.",
				SizeCategory.Small, 500, 8M, "scale", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalChitin] = Item(GlobalChitin, "chitin", "a stack of chitin plates",
				"A stack of hard chitin plates has been pried free from an arthropod body.",
				SizeCategory.Normal, 1800, 10M, "chitin", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalShell] = Item(GlobalShell, "shell", "a stack of shell plates",
				"A stack of shell plates has been separated out for craft or trade use.",
				SizeCategory.Normal, 2200, 10M, "shell", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalFins] = Item(GlobalFins, "fins", "a set of raw fins",
				"A set of fins has been cut from an aquatic carcass, still slick and raw.",
				SizeCategory.Small, 450, 5M, "fin", true, RawMeatCutTag),
			[GlobalBlubber] = Item(GlobalBlubber, "blubber", "a slab of raw blubber",
				"A thick slab of oily blubber has been cut free from beneath the skin.",
				SizeCategory.Normal, 3500, 8M, "blubber", true, RawMeatCutTag, OffalTag),
			[GlobalInkSac] = Item(GlobalInkSac, "sac", "a raw ink sac",
				"A dark ink sac has been cut away carefully, still heavy with usable pigment.",
				SizeCategory.VerySmall, 180, 10M, "meat", true, OffalTag),
			[GlobalSilkGland] = Item(GlobalSilkGland, "gland", "a raw silk gland",
				"A soft silk gland has been cut from the body with strands of sticky fibre still clinging to it.",
				SizeCategory.VerySmall, 150, 12M, "meat", true, OffalTag),
			[GlobalIchor] = Item(GlobalIchor, "sac", "a raw ichor sac",
				"A membrane sac of strange ichor has been separated from the surrounding tissue.",
				SizeCategory.Small, 350, 10M, "meat", true, OffalTag),
			[GlobalToxinSac] = Item(GlobalToxinSac, "sac", "a raw toxin sac",
				"A fragile toxin sac has been trimmed away and kept intact enough for later processing.",
				SizeCategory.VerySmall, 120, 10M, "meat", true, VenomOrganTag),
			[GlobalBaleen] = Item(GlobalBaleen, "baleen", "a stack of baleen plates",
				"A stack of long baleen plates has been cut free and bundled together.",
				SizeCategory.Large, 4000, 35M, "keratin", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalMandibles] = Item(GlobalMandibles, "mandibles", "a pair of hard mandibles",
				"A pair of hard mandibles has been removed from the head and cleaned of clinging tissue.",
				SizeCategory.Small, 650, 8M, "chitin", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalStinger] = Item(GlobalStinger, "stinger", "a barbed animal stinger",
				"A hard, barbed stinger has been cut away intact.",
				SizeCategory.VerySmall, 90, 8M, "chitin", false, TrophyPartTag, CraftingAnimalProductTag),
			[GlobalFireGland] = Item(GlobalFireGland, "gland", "a raw draconic fire gland",
				"A dense, hot-smelling organ has been trimmed from a draconic carcass with great care.",
				SizeCategory.Small, 700, 30M, "dragon meat", true, OffalTag),
			[GlobalPhoenixAsh] = Item(GlobalPhoenixAsh, "ash", "a pinch of phoenix ash",
				"A small pinch of pale, fine ash remains from the hottest part of the phoenix's body.",
				SizeCategory.VerySmall, 10, 25M, "other", false, TrophyPartTag)
		};
	}

	private static IReadOnlyDictionary<string, StockButcheryFamilySpec> BuildFamilies()
	{
		var families = new Dictionary<string, StockButcheryFamilySpec>(StringComparer.OrdinalIgnoreCase);

		void Add(StockButcheryFamilySpec family)
		{
			families[family.Key] = family;
		}

		Add(LargeQuadruped("small-mammal", "Small Mammal", "game mammal", "small mammal hide", SizeCategory.Small, "small mammal",
			includeFat: false, includeHead: false));
		Add(LargeQuadruped("canid", "Canid", "dog", "dog hide", SizeCategory.Normal, "canid")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("claws", GlobalClaws, Groups(RightFore, LeftFore), "trophy"));
		Add(LargeQuadruped("felid", "Felid", "cat", "cat hide", SizeCategory.Normal, "feline")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("claws", GlobalClaws, Groups(RightFore, LeftFore), "trophy"));
		Add(LargeQuadruped("bear", "Bear", "game mammal", "bear hide", SizeCategory.Large, "bear")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("claws", GlobalClaws, Groups(RightFore, LeftFore), "trophy"));
		Add(LargeQuadruped("suid", "Pig", "pork", "pig hide", SizeCategory.Normal, "pig")
			.WithTrophy("tusks", GlobalTusks, Groups(RightTusk, LeftTusk), "trophy"));
		Add(LargeQuadruped("cervid", "Deer", "venison", "deer hide", SizeCategory.Normal, "venison")
			.WithTrophy("antlers", GlobalAntlers, Groups(RightHorn, LeftHorn), "trophy"));
		Add(LargeQuadruped("bovine", "Bovine", "beef", "cow hide", SizeCategory.Large, "bovine")
			.WithTrophy("horns", GlobalHorns, Groups(RightHorn, LeftHorn), "trophy"));
		Add(LargeQuadruped("ovine-caprine", "Ovine and Caprine", "mutton", "animal skin", SizeCategory.Normal, "sheep or goat")
			.WithTrophy("horns", GlobalHorns, Groups(RightHorn, LeftHorn), "trophy"));
		Add(LargeQuadruped("equine", "Equine", "horse", "animal skin", SizeCategory.Large, "horse"));
		Add(LargeQuadruped("camelid", "Camelid", "camel", "animal skin", SizeCategory.Large, "camel"));
		Add(LargeQuadruped("pachyderm", "Pachyderm", "game mammal", "elephant hide", SizeCategory.Huge, "pachyderm")
			.WithTrophy("tusks", GlobalTusks, Groups(RightTusk, LeftTusk), "trophy")
			.WithTrophy("horn", GlobalHorns, Groups(Horn), "trophy"));
		Add(LargeQuadruped("marsupial", "Marsupial", "kangaroo", "animal skin", SizeCategory.Normal, "marsupial"));
		Add(LargeQuadruped("odd-mammal", "Game Mammal", "game mammal", "animal skin", SizeCategory.Normal, "game mammal"));

		Add(Bird("small-bird", "Small Bird", "game bird", SizeCategory.Small, "small bird", includeLegs: false));
		Add(Bird("fowl", "Fowl", "chicken", SizeCategory.Normal, "fowl"));
		Add(Bird("waterfowl", "Waterfowl", "duck", SizeCategory.Normal, "waterfowl"));
		Add(Bird("raptor", "Raptor", "game bird", SizeCategory.Normal, "raptor")
			.WithTrophy("talons", GlobalClaws, Groups(RightHind, LeftHind), "trophy")
			.WithTrophy("beak", GlobalBeak, Groups(Mouth), "trophy"));
		Add(Bird("flightless-bird", "Flightless Bird", "ostrich", SizeCategory.Large, "flightless bird"));

		Add(Serpent("large-serpent", "Large Serpent", "snake", "snake hide", "serpent"));
		Add(Serpent("venomous-serpent", "Venomous Serpent", "snake", "snake hide", "venomous serpent")
			.WithTrophy("venom", GlobalVenomGland, Groups(RightFang, LeftFang), "venom"));
		Add(Reptile("lizard", "Lizard", "lizard", "animal skin", "lizard")
			.WithTrophy("scales", GlobalScales, Groups(Torso), "shell"));
		Add(Reptile("crocodilian", "Crocodilian", "alligator", "alligator hide", "crocodilian")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("scales", GlobalScales, Groups(Torso), "shell"));
		Add(Chelonian());
		Add(Anuran());

		Add(Fish("fish", "Fish", "fish", "fish"));
		Add(Fish("shark", "Shark", "shark", "shark")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("fins", GlobalFins, Groups(Tail), "trophy"));
		Add(Cetacean("cetacean", "Cetacean", "whale", "cetacean")
			.WithTrophy("blubber", GlobalBlubber, Groups(Torso), "blubber")
			.WithTrophy("baleen", GlobalBaleen, Groups(Mouth), "trophy"));
		Add(Cetacean("pinniped", "Pinniped", "game mammal", "pinniped")
			.WithTrophy("blubber", GlobalBlubber, Groups(Torso), "blubber")
			.WithTrophy("tusks", GlobalTusks, Groups(RightTusk, LeftTusk), "trophy"));
		Add(Crustacean());
		Add(Cephalopod());
		Add(Jellyfish());

		Add(Arthropod("insect", "Insect", "insect", SizeCategory.VerySmall, "insect")
			.WithTrophy("stinger", GlobalStinger, Groups(Stinger), "trophy")
			.WithTrophy("venom", GlobalVenomGland, Groups(Stinger), "venom"));
		Add(Arthropod("giant-arthropod", "Giant Arthropod", "insect", SizeCategory.Large, "giant arthropod", "giant arthropod chitin")
			.WithTrophy("mandibles", GlobalMandibles, Groups(Mouth), "trophy")
			.WithTrophy("venom", GlobalVenomGland, Groups(RightFang, LeftFang), "venom")
			.WithTrophy("silk", GlobalSilkGland, Groups(Torso), "silk"));
		Add(Worm());

		Add(LargeQuadruped("draconic", "Draconic Beast", "dragon meat", "draconic hide", SizeCategory.Huge, "dragon", "draconic scale")
			.WithTrophy("horns", GlobalHorns, Groups(RightHorn, LeftHorn), "trophy")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("fire-gland", GlobalFireGland, Groups(Torso), "organs"));
		Add(Bird("mythic-avian", "Mythic Avian", "game bird", SizeCategory.Large, "mythic avian", featherMaterial: "mythic feather")
			.WithTrophy("talons", GlobalClaws, Groups(RightHind, LeftHind), "trophy")
			.WithTrophy("beak", GlobalBeak, Groups(Mouth), "trophy")
			.WithTrophy("phoenix-ash", GlobalPhoenixAsh, Groups(Torso), "trophy"));
		Add(LargeQuadruped("mythic-equine", "Mythic Equine", "horse", "animal skin", SizeCategory.Large, "mythic equine", "draconic scale")
			.WithTrophy("horn", GlobalHorns, Groups(Horn), "trophy")
			.WithTrophy("feathers", GlobalFeathers, Groups(RightWing, LeftWing), "plume"));
		Add(LargeQuadruped("chimera", "Chimera", "game mammal", "lion hide", SizeCategory.Large, "chimera")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy")
			.WithTrophy("claws", GlobalClaws, Groups(RightFore, LeftFore), "trophy")
			.WithTrophy("venom", GlobalVenomGland, Groups(Tail), "venom"));
		Add(LargeQuadruped("aquatic-mythic", "Aquatic Mythic Beast", "game mammal", "animal skin", SizeCategory.Large, "aquatic mythic beast")
			.WithTrophy("blubber", GlobalBlubber, Groups(Torso), "blubber")
			.WithTrophy("teeth", GlobalTeeth, Groups(Mouth), "trophy"));

		return families;
	}

	private static StockButcheryFamilySpec LargeQuadruped(
		string key,
		string displayName,
		string meatMaterial,
		string hideMaterial,
		SizeCategory size,
		string noun,
		string? scaleMaterial = null,
		bool includeFat = true,
		bool includeHead = true)
	{
		var builder = new FamilyBuilder(key, displayName);
		var forequarter = builder.AddItem("forequarter", "forequarter", $"a raw {noun} forequarter",
			$"A raw {noun} forequarter has been cut away with shoulder, forelimb and dense working muscle still together.",
			size, 6500, 12M, meatMaterial, true, RawMeatCutTag);
		var hindquarter = builder.AddItem("hindquarter", "hindquarter", $"a raw {noun} hindquarter",
			$"A raw {noun} hindquarter has been separated as a heavy haunch of meat, bone and connective tissue.",
			size, 7500, 14M, meatMaterial, true, RawMeatCutTag);
		var hide = builder.AddItem("hide", "hide", $"a raw {noun} hide",
			$"A raw {noun} hide has been peeled away in a broad sheet, still damp and unprocessed.",
			size, 3200, 16M, hideMaterial, true, RawHideTag);
		var damagedHide = builder.AddItem("damaged-hide", "hide", $"a ragged raw {noun} hide",
			$"This raw {noun} hide is ragged, torn and poorly removed, but still has usable patches.",
			size, 1800, 5M, hideMaterial, true, RawHideTag);

		builder.AddProduct("forequarters", forequarter, 2, Groups(RightFore, LeftFore));
		builder.AddProduct("hindquarters", hindquarter, 2, Groups(RightHind, LeftHind));
		if (includeHead)
		{
			var head = builder.AddItem("head", "head", $"a raw {noun} head",
				$"A raw {noun} head has been cut free, with the skull, jaws and remaining tissue still intact.",
				SizeCategory.Normal, 2500, 5M, meatMaterial, true, RawMeatCutTag);
			builder.AddProduct("head", head, 1, Groups(Head));
		}

		builder.AddProduct("hide", hide, 1, Groups(Torso), isPelt: true, damagedItemKey: damagedHide,
			damagedQuantity: 1, damageThreshold: 0.35);
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		if (includeFat)
		{
			builder.AddProduct("fat", GlobalFat, 1, Groups(Torso), "organs");
		}

		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		if (scaleMaterial is not null)
		{
			var scales = builder.AddItem("scales", "scales", $"a pile of {noun} scales",
				$"A pile of {noun} scales has been scraped free and sorted for the pieces least damaged by removal.",
				SizeCategory.Small, 800, 15M, scaleMaterial, false, TrophyPartTag, CraftingAnimalProductTag);
			builder.AddProduct("scales", scales, 1, Groups(Torso), "shell");
		}

		return builder.Build();
	}

	private static StockButcheryFamilySpec Bird(
		string key,
		string displayName,
		string meatMaterial,
		SizeCategory size,
		string noun,
		bool includeLegs = true,
		string featherMaterial = "feather")
	{
		var builder = new FamilyBuilder(key, displayName);
		var breast = builder.AddItem("breast", "breast", $"a raw {noun} breast",
			$"A raw {noun} breast has been cut from the body, with lean pale meat still clinging to the bone.",
			size, 1200, 5M, meatMaterial, true, RawMeatCutTag);
		var wings = builder.AddItem("wings", "wings", $"a pair of raw {noun} wings",
			$"A pair of raw {noun} wings has been cut free, with feathers, skin and wing meat still attached.",
			size, 900, 4M, meatMaterial, true, RawMeatCutTag);
		var feathers = builder.AddItem("feathers", "feathers", $"a bundle of {noun} feathers",
			$"A bundle of {noun} feathers has been sorted out for the largest and cleanest usable plumes.",
			SizeCategory.Small, 120, 6M, featherMaterial, false, TrophyPartTag, CraftingAnimalProductTag);

		builder.AddProduct("breast", breast, 1, Groups(Torso));
		builder.AddProduct("wings", wings, 1, Groups(RightWing, LeftWing));
		builder.AddProduct("plumes", feathers, 1, Groups(RightWing, LeftWing), "plume", isPelt: true,
			damagedItemKey: GlobalFeathers, damagedQuantity: 1, damageThreshold: 0.45);
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		if (includeLegs)
		{
			var legs = builder.AddItem("legs", "legs", $"a pair of raw {noun} drumsticks",
				$"A pair of raw {noun} drumsticks has been cut free with skin, sinew and bone still attached.",
				SizeCategory.Small, 800, 4M, meatMaterial, true, RawMeatCutTag);
			builder.AddProduct("drumsticks", legs, 1, Groups(RightHind, LeftHind));
		}

		return builder.Build();
	}

	private static StockButcheryFamilySpec Serpent(string key, string displayName, string meatMaterial, string hideMaterial, string noun)
	{
		var builder = new FamilyBuilder(key, displayName);
		var meat = builder.AddItem("meat", "length", $"a raw length of {noun} meat",
			$"A long raw section of {noun} meat has been cut away in a dense, boned length.",
			SizeCategory.Normal, 2800, 8M, meatMaterial, true, RawMeatCutTag);
		var skin = builder.AddItem("skin", "skin", $"a raw {noun} skin",
			$"A raw {noun} skin has been peeled away in a long patterned sheet.",
			SizeCategory.Normal, 900, 12M, hideMaterial, true, RawHideTag);
		var damagedSkin = builder.AddItem("damaged-skin", "skin", $"a torn raw {noun} skin",
			$"This raw {noun} skin is torn in several places, but some patterned sections remain usable.",
			SizeCategory.Small, 450, 4M, hideMaterial, true, RawHideTag);

		builder.AddProduct("meat", meat, 3, Groups(Torso));
		builder.AddProduct("skin", skin, 1, Groups(Torso), isPelt: true, damagedItemKey: damagedSkin,
			damagedQuantity: 1, damageThreshold: 0.35);
		builder.AddProduct("head", GlobalTeeth, 1, Groups(Mouth), "trophy");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Reptile(string key, string displayName, string meatMaterial, string hideMaterial, string noun)
	{
		var builder = new FamilyBuilder(key, displayName);
		var haunch = builder.AddItem("haunch", "haunch", $"a raw {noun} haunch",
			$"A raw {noun} haunch has been separated with tough muscle and bone still together.",
			SizeCategory.Normal, 2200, 7M, meatMaterial, true, RawMeatCutTag);
		var tail = builder.AddItem("tail", "tail", $"a raw {noun} tail",
			$"A raw {noun} tail has been cut away as a firm length of meat and sinew.",
			SizeCategory.Normal, 1800, 6M, meatMaterial, true, RawMeatCutTag);
		var skin = builder.AddItem("skin", "skin", $"a raw {noun} skin",
			$"A raw {noun} skin has been peeled away, scaly and damp from the carcass.",
			SizeCategory.Normal, 1000, 10M, hideMaterial, true, RawHideTag);
		var damagedSkin = builder.AddItem("damaged-skin", "skin", $"a torn raw {noun} skin",
			$"This raw {noun} skin is torn and ragged but still has usable patches.",
			SizeCategory.Small, 500, 3M, hideMaterial, true, RawHideTag);

		builder.AddProduct("haunches", haunch, 2, Groups(RightHind, LeftHind));
		builder.AddProduct("tail", tail, 1, Groups(Tail));
		builder.AddProduct("skin", skin, 1, Groups(Torso), isPelt: true, damagedItemKey: damagedSkin,
			damagedQuantity: 1, damageThreshold: 0.35);
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Chelonian()
	{
		var builder = new FamilyBuilder("chelonian", "Chelonian");
		var meat = builder.AddItem("meat", "meat", "a heap of raw turtle meat",
			"A heap of raw turtle meat has been cut away from shell and limbs.",
			SizeCategory.Normal, 2600, 8M, "turtle", true, RawMeatCutTag);
		builder.AddProduct("meat", meat, 1, Groups(Torso));
		builder.AddProduct("shell", GlobalShell, 1, Groups(Shell), "shell");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Anuran()
	{
		var builder = new FamilyBuilder("anuran", "Anuran");
		var legs = builder.AddItem("legs", "legs", "a pair of raw frog legs",
			"A pair of raw frog legs has been cut away, small but recognisably meaty.",
			SizeCategory.Small, 250, 3M, "frog", true, RawMeatCutTag);
		var skin = builder.AddItem("skin", "skin", "a raw amphibian skin",
			"A damp amphibian skin has been peeled away in a delicate sheet.",
			SizeCategory.VerySmall, 80, 2M, "skin", true, RawHideTag);
		builder.AddProduct("legs", legs, 1, Groups(RightHind, LeftHind));
		builder.AddProduct("skin", skin, 1, Groups(Torso), isPelt: true, damagedItemKey: skin,
			damagedQuantity: 1, damageThreshold: 0.5);
		builder.AddProduct("toxin", GlobalToxinSac, 1, Groups(Torso), "venom");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Fish(string key, string displayName, string meatMaterial, string noun)
	{
		var builder = new FamilyBuilder(key, displayName);
		var fillet = builder.AddItem("fillet", "fillet", $"a raw {noun} fillet",
			$"A raw {noun} fillet has been cut cleanly away from the bones.",
			SizeCategory.Small, 800, 5M, meatMaterial, true, RawMeatCutTag);
		var head = builder.AddItem("head", "head", $"a raw {noun} head",
			$"A raw {noun} head has been removed with jaws, gills and scraps of flesh still attached.",
			SizeCategory.Small, 450, 2M, meatMaterial, true, RawMeatCutTag);
		builder.AddProduct("fillets", fillet, 2, Groups(Torso));
		builder.AddProduct("head", head, 1, Groups(Head));
		builder.AddProduct("scales", GlobalScales, 1, Groups(Torso), "shell");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Cetacean(string key, string displayName, string meatMaterial, string noun)
	{
		var builder = new FamilyBuilder(key, displayName);
		var slab = builder.AddItem("slab", "slab", $"a raw {noun} meat slab",
			$"A heavy raw slab of {noun} meat has been cut free in a dense red sheet.",
			SizeCategory.Large, 9000, 18M, meatMaterial, true, RawMeatCutTag);
		var hide = builder.AddItem("hide", "hide", $"a raw {noun} hide",
			$"A raw {noun} hide has been peeled back in a thick, damp sheet.",
			SizeCategory.Large, 8000, 18M, "animal skin", true, RawHideTag);
		var damagedHide = builder.AddItem("damaged-hide", "hide", $"a ragged raw {noun} hide",
			$"This raw {noun} hide is ragged and scarred from rough removal.",
			SizeCategory.Normal, 4000, 6M, "animal skin", true, RawHideTag);
		builder.AddProduct("slabs", slab, 3, Groups(Torso));
		builder.AddProduct("hide", hide, 1, Groups(Torso), isPelt: true, damagedItemKey: damagedHide,
			damagedQuantity: 1, damageThreshold: 0.35);
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		builder.AddProduct("bones", GlobalBones, 1, Groups(Head, Torso), "trophy");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Crustacean()
	{
		var builder = new FamilyBuilder("crustacean", "Crustacean");
		var meat = builder.AddItem("meat", "meat", "a heap of raw shellfish meat",
			"A heap of pale raw shellfish meat has been picked from shell and limbs.",
			SizeCategory.Small, 900, 8M, "shellfish", true, RawMeatCutTag);
		builder.AddProduct("meat", meat, 1, Groups(Torso));
		builder.AddProduct("claws", meat, 1, Groups(RightFore, LeftFore));
		builder.AddProduct("shell", GlobalShell, 1, Groups(Shell), "shell");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Cephalopod()
	{
		var builder = new FamilyBuilder("cephalopod", "Cephalopod");
		var tentacles = builder.AddItem("tentacles", "tentacles", "a heap of raw tentacles",
			"A heap of raw tentacles has been cut free, still slick and curling slightly.",
			SizeCategory.Normal, 1800, 8M, "squid", true, RawMeatCutTag);
		var mantle = builder.AddItem("mantle", "mantle", "a raw cephalopod mantle",
			"A raw cephalopod mantle has been separated as a thick sheet of pale meat.",
			SizeCategory.Normal, 2200, 9M, "squid", true, RawMeatCutTag);
		builder.AddProduct("tentacles", tentacles, 1, Groups(Tail));
		builder.AddProduct("mantle", mantle, 1, Groups(Torso));
		builder.AddProduct("ink", GlobalInkSac, 1, Groups(Torso), "ink");
		builder.AddProduct("beak", GlobalBeak, 1, Groups(Mouth), "trophy");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Jellyfish()
	{
		var builder = new FamilyBuilder("jellyfish", "Jellyfish");
		var bell = builder.AddItem("bell", "bell", "a raw jellyfish bell",
			"A translucent jellyfish bell has been cut away as a quivering mass.",
			SizeCategory.Small, 600, 4M, "meat", true, RawMeatCutTag);
		builder.AddProduct("bell", bell, 1, Groups(Torso));
		builder.AddProduct("tendrils", GlobalToxinSac, 1, Groups(Tail), "venom");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Arthropod(
		string key,
		string displayName,
		string meatMaterial,
		SizeCategory size,
		string noun,
		string chitinMaterial = "chitin")
	{
		var builder = new FamilyBuilder(key, displayName);
		var meat = builder.AddItem("meat", "meat", $"a heap of raw {noun} flesh",
			$"A heap of raw {noun} flesh has been cut from inside the hard outer body.",
			size, 1000, 6M, meatMaterial, true, RawMeatCutTag);
		var chitin = builder.AddItem("chitin", "chitin", $"a stack of {noun} chitin plates",
			$"A stack of hard {noun} chitin plates has been pried free and sorted.",
			size, 1600, 10M, chitinMaterial, false, TrophyPartTag, CraftingAnimalProductTag);
		builder.AddProduct("meat", meat, 1, Groups(Torso));
		builder.AddProduct("chitin", chitin, 1, Groups(Torso), "shell");
		builder.AddProduct("offal", GlobalOffal, 1, Groups(Torso), "organs");
		return builder.Build();
	}

	private static StockButcheryFamilySpec Worm()
	{
		var builder = new FamilyBuilder("worm", "Giant Worm");
		var flesh = builder.AddItem("flesh", "flesh", "a slab of raw worm flesh",
			"A slick slab of raw worm flesh has been cut from a dense, boneless body.",
			SizeCategory.Large, 5000, 8M, "game mammal", true, RawMeatCutTag);
		var hide = builder.AddItem("hide", "hide", "a raw worm hide",
			"A rubbery raw worm hide has been stripped away in a heavy sheet.",
			SizeCategory.Large, 3500, 8M, "animal skin", true, RawHideTag);
		builder.AddProduct("flesh", flesh, 3, Groups(Torso));
		builder.AddProduct("hide", hide, 1, Groups(Torso), isPelt: true, damagedItemKey: hide,
			damagedQuantity: 1, damageThreshold: 0.5);
		builder.AddProduct("ichor", GlobalIchor, 1, Groups(Torso), "organs");
		return builder.Build();
	}

	private static StockButcheryItemSpec Item(
		string key,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		double weight,
		decimal cost,
		string material,
		bool spoils,
		params string[] tags)
	{
		return new StockButcheryItemSpec(
			key,
			noun,
			shortDescription,
			$"{CapitaliseFirstLetter(shortDescription)} lies here.",
			fullDescription,
			size,
			weight,
			cost,
			material,
			spoils,
			[ButcheryOutputTag, .. tags]
		);
	}

	private static IReadOnlyList<IReadOnlyList<string>> Groups(params string[][] groups)
	{
		return groups.Select(x => (IReadOnlyList<string>)x).ToList();
	}

	private static string NormaliseSubcategory(string? subcategory)
	{
		return string.IsNullOrWhiteSpace(subcategory) ? string.Empty : subcategory.ToLowerInvariant();
	}

	private static string CapitaliseFirstLetter(string text)
	{
		return string.IsNullOrEmpty(text) ? text : $"{char.ToUpperInvariant(text[0])}{text[1..]}";
	}

	private static bool TryGetFamilyKey(string raceName, string bodyKey, bool mythical, out string familyKey)
	{
		if (mythical)
		{
			if (!IncludedMythicalBeasts.Contains(raceName))
			{
				familyKey = string.Empty;
				return false;
			}

			familyKey = raceName switch
			{
				"Dragon" or "Eastern Dragon" or "Wyvern" or "Fell Beast" or "Basilisk" => "draconic",
				"Phoenix" or "Cockatrice" or "Garuda" or "Giant Eagle" or "Griffin" or "Hippogriff" => "mythic-avian",
				"Unicorn" or "Pegasus" or "Pegacorn" or "Qilin" or "Hippocamp" => "mythic-equine",
				"Warg" or "Dire-Wolf" => "canid",
				"Dire-Bear" => "bear",
				"Manticore" => "chimera",
				"Giant Beetle" or "Giant Ant" or "Giant Mantis" or "Giant Spider" or "Giant Scorpion" or "Giant Centipede" or "Ankheg" => "giant-arthropod",
				"Giant Worm" or "Colossal Worm" => "worm",
				"Bunyip" => "aquatic-mythic",
				"Yacumama" => "large-serpent",
				_ => FamilyKeyForBody(bodyKey, SizeCategory.Normal)
			};
			return Families.ContainsKey(familyKey);
		}

		familyKey = raceName switch
		{
			"Rabbit" or "Hare" or "Mouse" or "Rat" or "Guinea Pig" or "Hamster" or "Shrew" or "Ferret" or
				"Stoat" or "Weasel" or "Polecat" or "Mink" or "Beaver" or "Otter" or "Red Panda" => "small-mammal",
			"Dog" or "Wolf" or "Fox" or "Coyote" or "Jackal" or "Hyena" or "Dingo" => "canid",
			"Cat" or "Lion" or "Tiger" or "Sabretooth Tiger" or "Cheetah" or "Leopard" or "Panther" or "Jaguar" => "felid",
			"Bear" or "Giant Panda" => "bear",
			"Pig" or "Boar" or "Warthog" => "suid",
			"Deer" or "Moose" or "Elk" or "Reindeer" => "cervid",
			"Cow" or "Ox" or "Bison" or "Buffalo" => "bovine",
			"Sheep" or "Goat" => "ovine-caprine",
			"Horse" or "Donkey" or "Mule" => "equine",
			"Llama" or "Alpaca" or "Camel" => "camelid",
			"Rhinocerous" or "Elephant" or "Mammoth" or "Oliphant" or "Hippopotamus" or "Giraffe" => "pachyderm",
			"Kangaroo" or "Wallaby" or "Koala" or "Wombat" or "Tasmanian Devil" => "marsupial",
			"Python" or "Tree Python" or "Boa" or "Anaconda" => "large-serpent",
			"Cobra" or "Adder" or "Rattlesnake" or "Viper" or "Mamba" or "Coral Snake" or "Moccasin" or "Gila Monster" => "venomous-serpent",
			"Crocodile" or "Alligator" or "Caiman" => "crocodilian",
			"Turtle" or "Tortoise" => "chelonian",
			"Frog" or "Toad" => "anuran",
			"Chicken" or "Turkey" or "Quail" or "Grouse" or "Pheasant" or "Peacock" or "Hoatzin" or "Lyrebird" => "fowl",
			"Duck" or "Mandarin Duck" or "Goose" or "Swan" or "Pelican" or "Penguin" => "waterfowl",
			"Emu" or "Ostrich" or "Moa" or "Rhea" or "Cassowary" or "Kiwi" => "flightless-bird",
			"Vulture" or "Hawk" or "Eagle" or "Falcon" or "Owl" or "Condor" or "Albatross" => "raptor",
			"Shark" => "shark",
			"Dolphin" or "Porpoise" or "Orca" or "Baleen Whale" or "Toothed Whale" => "cetacean",
			"Sea Lion" or "Seal" or "Walrus" => "pinniped",
			"Crab" or "Giant Crab" or "Small Crab" or "Lobster" or "Shrimp" or "Prawn" or "Crayfish" => "crustacean",
			"Octopus" or "Squid" or "Giant Squid" => "cephalopod",
			"Jellyfish" => "jellyfish",
			"Spider" or "Tarantula" or "Scorpion" => "giant-arthropod",
			_ => FamilyKeyForBody(bodyKey, SizeCategory.Normal)
		};
		return Families.ContainsKey(familyKey);
	}

	private static string FamilyKeyForBody(string bodyKey, SizeCategory size)
	{
		return bodyKey switch
		{
			"Avian" => size >= SizeCategory.Large ? "flightless-bird" : "small-bird",
			"Serpentine" => "large-serpent",
			"Reptilian" => "lizard",
			"Chelonian" => "chelonian",
			"Crocodilian" => "crocodilian",
			"Anuran" => "anuran",
			"Piscine" => "fish",
			"Cetacean" => "cetacean",
			"Pinniped" => "pinniped",
			"Decapod" or "Malacostracan" => "crustacean",
			"Cephalopod" => "cephalopod",
			"Jellyfish" => "jellyfish",
			"Insectoid" or "Winged Insectoid" or "Beetle" or "Centipede" or "Arachnid" or "Scorpion" => "giant-arthropod",
			"Vermiform" => "worm",
			"Ungulate" => "bovine",
			"Toed Quadruped" or "Quadruped Base" => "odd-mammal",
			_ => "odd-mammal"
		};
	}

	private static IEnumerable<RaceAssignment> GetRaceAssignments(FuturemudDatabaseContext context)
	{
		var races = context.Races
			.Include(x => x.BaseBody)
			.Include(x => x.RaceButcheryProfile)
			.ToList();
		var raceByName = races.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

		foreach ((string raceName, var template) in AnimalSeeder.RaceTemplatesForTesting)
		{
			if (!raceByName.TryGetValue(raceName, out var race))
			{
				continue;
			}

			if (TryGetFamilyKey(raceName, template.BodyKey, false, out var familyKey))
			{
				yield return new RaceAssignment(race, Families[familyKey], template.BodyKey);
			}
		}

		foreach ((string raceName, var template) in MythicalAnimalSeeder.TemplatesForTesting)
		{
			if (!raceByName.TryGetValue(raceName, out var race))
			{
				continue;
			}

			if (TryGetFamilyKey(raceName, template.BodyKey, true, out var familyKey))
			{
				yield return new RaceAssignment(race, Families[familyKey], template.BodyKey);
			}
		}
	}

	private static void EnsureButcheryTags(FuturemudDatabaseContext context)
	{
		var root = EnsureTag(context, ButcheryOutputTag, "Animal Product");
		EnsureTag(context, RawMeatCutTag, root.Name);
		EnsureTag(context, RawHideTag, root.Name);
		EnsureTag(context, OffalTag, root.Name);
		EnsureTag(context, TrophyPartTag, root.Name);
		EnsureTag(context, VenomOrganTag, root.Name);
		EnsureTag(context, CraftingAnimalProductTag, root.Name);
		context.SaveChanges();
	}

	private static Tag EnsureTag(FuturemudDatabaseContext context, string name, string? parentName = null)
	{
		var tag = context.Tags
			.Include(x => x.Parent)
			.FirstOrDefault(x => x.Name == name);
		if (tag is null)
		{
			tag = new Tag
			{
				Name = name,
				Parent = parentName is null ? null : context.Tags.First(x => x.Name == parentName)
			};
			context.Tags.Add(tag);
			context.SaveChanges();
			return tag;
		}

		if (parentName is not null && tag.Parent?.Name != parentName)
		{
			tag.Parent = context.Tags.First(x => x.Name == parentName);
		}

		return tag;
	}

	private static void EnsureMaterial(FuturemudDatabaseContext context, StockButcheryMaterialSpec spec)
	{
		var tag = context.Tags.First(x => x.Name == spec.TagName);
		var material = context.Materials
			.Include(x => x.MaterialsTags)
			.FirstOrDefault(x => x.Name == spec.Name);

		if (material is null)
		{
			material = new Material
			{
				Name = spec.Name,
				MaterialDescription = spec.Name,
				Type = 0,
				BehaviourType = (int)spec.BehaviourType,
				Density = 1000 * spec.RelativeDensity,
				Organic = spec.Organic,
				Absorbency = spec.BehaviourType is MaterialBehaviourType.Feather or MaterialBehaviourType.Skin ? 0.2 : 0.05,
				ShearYield = spec.BehaviourType == MaterialBehaviourType.Meat ? 10000 : 20000,
				ImpactYield = spec.BehaviourType == MaterialBehaviourType.Meat ? 10000 : 50000,
				ElectricalConductivity = 0.0001,
				ThermalConductivity = 0.14,
				SpecificHeatCapacity = 500,
				SolventVolumeRatio = 1.0,
				ResidueSdesc = string.Empty,
				ResidueDesc = string.Empty,
				ResidueColour = "white"
			};
			context.Materials.Add(material);
		}
		else
		{
			material.MaterialDescription = spec.Name;
			material.BehaviourType = (int)spec.BehaviourType;
			material.Organic = spec.Organic;
		}

		if (material.MaterialsTags.All(x => x.TagId != tag.Id))
		{
			material.MaterialsTags.Add(new MaterialsTags
			{
				Material = material,
				Tag = tag
			});
		}
	}

	private static IReadOnlyDictionary<string, GameItemProto> EnsureItems(FuturemudDatabaseContext context)
	{
		var account = context.Accounts.First();
		var rottenMeat = context.GameItemProtos.First(x => x.ShortDescription == RottenMeatShortDescription);
		var components = context.GameItemComponentProtos.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var tags = context.Tags.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var materials = context.Materials.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var specs = GlobalItems.Values
			.Concat(Families.Values.SelectMany(x => x.Items))
			.ToDictionary(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
		var existingItems = context.GameItemProtos
			.Include(x => x.GameItemProtosTags)
			.Include(x => x.GameItemProtosGameItemComponentProtos)
			.ToDictionary(x => x.ShortDescription, x => x, StringComparer.OrdinalIgnoreCase);
		var nextId = context.GameItemProtos.Any() ? context.GameItemProtos.Max(x => x.Id) + 1 : 1;
		var now = DateTime.UtcNow;
		var results = new Dictionary<string, GameItemProto>(StringComparer.OrdinalIgnoreCase);

		foreach (var spec in specs.Values)
		{
			if (!materials.TryGetValue(spec.Material, out var material))
			{
				throw new InvalidOperationException($"Animal butchery item {spec.ShortDescription} requires missing material {spec.Material}.");
			}

			if (!existingItems.TryGetValue(spec.ShortDescription, out var item))
			{
				item = new GameItemProto
				{
					Id = nextId++,
					EditableItem = new EditableItem
					{
						RevisionNumber = 0,
						RevisionStatus = 4,
						BuilderAccountId = account.Id,
						BuilderDate = now,
						BuilderComment = "Auto-generated by the Animal Butchery seeder",
						ReviewerAccountId = account.Id,
						ReviewerDate = now,
						ReviewerComment = "Auto-generated by the Animal Butchery seeder"
					}
				};
				context.GameItemProtos.Add(item);
				existingItems[spec.ShortDescription] = item;
			}

			item.Name = spec.Noun.ToLowerInvariant();
			item.Keywords = new ExplodedString(spec.ShortDescription.Strip_A_An()).Words.Distinct()
				.ListToCommaSeparatedValues(" ");
			item.MaterialId = material.Id;
			item.RevisionNumber = 0;
			item.Size = (int)spec.Size;
			item.Weight = spec.WeightInGrams;
			item.ReadOnly = false;
			item.LongDescription = spec.LongDescription;
			item.BaseItemQuality = (int)ItemQuality.Standard;
			item.ShortDescription = spec.ShortDescription;
			item.FullDescription = spec.FullDescription;
			item.PermitPlayerSkins = false;
			item.CostInBaseCurrency = spec.Cost;
			item.IsHiddenFromPlayers = false;
			item.MorphGameItemProtoId = spec.Spoils ? rottenMeat.Id : null;
			item.MorphTimeSeconds = spec.Spoils ? SpoilSeconds : 0;
			item.MorphEmote = spec.Spoils
				? $"$0 decay|decays into {RottenMeatShortDescription}."
				: "$0 $?1|morphs into $1|decays into nothing$.";

			foreach (var tagName in spec.Tags)
			{
				var tag = tags[tagName];
				if (item.GameItemProtosTags.All(x => x.TagId != tag.Id))
				{
					item.GameItemProtosTags.Add(new GameItemProtosTags
					{
						GameItemProto = item,
						Tag = tag,
						GameItemProtoRevisionNumber = item.RevisionNumber
					});
				}
			}

			foreach (var componentName in new[] { "Holdable", "Destroyable_Misc", "Stack_Pile" })
			{
				var component = components[componentName];
				if (item.GameItemProtosGameItemComponentProtos.All(x => x.GameItemComponentProtoId != component.Id))
				{
					item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = item,
						GameItemComponent = component,
						GameItemProtoRevision = item.RevisionNumber,
						GameItemComponentRevision = component.RevisionNumber
					});
				}
			}

			results[spec.Key] = item;
		}

		context.SaveChanges();
		return results;
	}

	private static IReadOnlyDictionary<string, IReadOnlyList<ButcheryProducts>> EnsureProducts(
		FuturemudDatabaseContext context,
		IReadOnlyList<RaceAssignment> assignments,
		IReadOnlyDictionary<string, GameItemProto> itemProtos)
	{
		var productsByName = context.ButcheryProducts
			.Include(x => x.ButcheryProductItems)
			.Include(x => x.ButcheryProductsBodypartProtos)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var bodies = context.BodyProtos
			.Include(x => x.BodypartProtos)
			.Include(x => x.CountsAs)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var bodyparts = context.BodypartProtos.ToList();
		var results = new Dictionary<string, IReadOnlyList<ButcheryProducts>>(StringComparer.OrdinalIgnoreCase);

		foreach (var familyGroup in assignments
			         .Where(x => x.Race.RaceButcheryProfileId == null || IsStockProfile(x.Race.RaceButcheryProfile))
			         .GroupBy(x => x.Family.Key, StringComparer.OrdinalIgnoreCase))
		{
			var family = Families[familyGroup.Key];
			var familyProducts = new List<ButcheryProducts>();
			foreach (var bodyKey in familyGroup.Select(x => x.BodyKey).Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!bodies.TryGetValue(bodyKey, out var body))
				{
					continue;
				}

				var partLookup = BuildBodypartLookup(context, body, bodyparts);
				foreach (var productSpec in family.Products)
				{
					var requiredParts = ResolveRequiredParts(partLookup, productSpec.RequiredPartGroups);
					if (requiredParts is null)
					{
						continue;
					}

					var name = ProductName(family, body, productSpec);
					if (!productsByName.TryGetValue(name, out var product))
					{
						product = new ButcheryProducts
						{
							Name = name
						};
						context.ButcheryProducts.Add(product);
						productsByName[name] = product;
					}

					product.TargetBody = body;
					product.TargetBodyId = body.Id;
					product.IsPelt = productSpec.IsPelt;
					product.Subcategory = productSpec.Subcategory;
					product.CanProduceProgId = null;

					context.ButcheryProductItems.RemoveRange(product.ButcheryProductItems);
					product.ButcheryProductItems.Clear();
					context.ButcheryProductsBodypartProtos.RemoveRange(product.ButcheryProductsBodypartProtos);
					product.ButcheryProductsBodypartProtos.Clear();

					foreach (var part in requiredParts)
					{
						product.ButcheryProductsBodypartProtos.Add(new ButcheryProductsBodypartProtos
						{
							ButcheryProduct = product,
							BodypartProto = part
						});
					}

					product.ButcheryProductItems.Add(new ButcheryProductItems
					{
						ButcheryProduct = product,
						NormalProtoId = itemProtos[productSpec.ItemKey].Id,
						NormalQuantity = productSpec.Quantity,
						DamagedProtoId = productSpec.DamagedItemKey is null ? null : itemProtos[productSpec.DamagedItemKey].Id,
						DamagedQuantity = productSpec.DamagedQuantity,
						DamageThreshold = productSpec.DamageThreshold
					});

					familyProducts.Add(product);
				}
			}

			results[family.Key] = familyProducts
				.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.Select(x => x.First())
				.ToList();
		}

		context.SaveChanges();
		return results;
	}

	private static Dictionary<string, BodypartProto> BuildBodypartLookup(
		FuturemudDatabaseContext context,
		BodyProto body,
		IReadOnlyList<BodypartProto> bodyparts)
	{
		var bodies = new List<BodyProto>();
		var current = body;
		while (current is not null)
		{
			bodies.Add(current);
			current = current.CountsAs ?? (current.CountsAsId is null
				? null
				: context.BodyProtos.FirstOrDefault(x => x.Id == current.CountsAsId.Value));
		}

		var bodyOrder = bodies
			.Select((item, index) => (item.Id, index))
			.ToDictionary(x => x.Id, x => x.index);

		return bodyparts
			.Where(x => bodyOrder.ContainsKey(x.BodyId))
			.OrderBy(x => bodyOrder[x.BodyId])
			.ThenBy(x => x.DisplayOrder ?? int.MaxValue)
			.ThenBy(x => x.Id)
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
	}

	private static IReadOnlyList<BodypartProto>? ResolveRequiredParts(
		IReadOnlyDictionary<string, BodypartProto> partLookup,
		IReadOnlyList<IReadOnlyList<string>> requiredPartGroups)
	{
		var parts = new List<BodypartProto>();
		foreach (var group in requiredPartGroups)
		{
			var part = group
				.Select(alias => partLookup.GetValueOrDefault(alias))
				.FirstOrDefault(x => x is not null);
			if (part is null)
			{
				return null;
			}

			if (parts.All(x => x.Id != part.Id))
			{
				parts.Add(part);
			}
		}

		return parts.Count == 0 ? null : parts;
	}

	private static IReadOnlyDictionary<string, RaceButcheryProfile> EnsureProfiles(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, IReadOnlyList<ButcheryProducts>> productsByFamily)
	{
		var profilesByName = context.RaceButcheryProfiles
			.Include(x => x.RaceButcheryProfilesBreakdownChecks)
			.Include(x => x.RaceButcheryProfilesBreakdownEmotes)
			.Include(x => x.RaceButcheryProfilesSkinningEmotes)
			.Include(x => x.RaceButcheryProfilesButcheryProducts)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var trait = context.TraitDefinitions.First(x => x.Name == "Butchery");
		var cutting = context.Tags.First(x => x.Name == "Cutting");
		var results = new Dictionary<string, RaceButcheryProfile>(StringComparer.OrdinalIgnoreCase);

		foreach ((var familyKey, var products) in productsByFamily)
		{
			if (!products.Any())
			{
				continue;
			}

			var family = Families[familyKey];
			var name = ProfileName(family);
			if (!profilesByName.TryGetValue(name, out var profile))
			{
				profile = new RaceButcheryProfile
				{
					Name = name
				};
				context.RaceButcheryProfiles.Add(profile);
				profilesByName[name] = profile;
			}

			profile.Verb = (int)ButcheryVerb.Butcher;
			profile.RequiredToolTagId = cutting.Id;
			profile.DifficultySkin = (int)Difficulty.Normal;
			profile.CanButcherProgId = null;
			profile.WhyCannotButcherProgId = null;

			context.RaceButcheryProfilesBreakdownChecks.RemoveRange(profile.RaceButcheryProfilesBreakdownChecks);
			context.RaceButcheryProfilesBreakdownEmotes.RemoveRange(profile.RaceButcheryProfilesBreakdownEmotes);
			context.RaceButcheryProfilesSkinningEmotes.RemoveRange(profile.RaceButcheryProfilesSkinningEmotes);
			context.RaceButcheryProfilesButcheryProducts.RemoveRange(profile.RaceButcheryProfilesButcheryProducts);
			profile.RaceButcheryProfilesBreakdownChecks.Clear();
			profile.RaceButcheryProfilesBreakdownEmotes.Clear();
			profile.RaceButcheryProfilesSkinningEmotes.Clear();
			profile.RaceButcheryProfilesButcheryProducts.Clear();

			var categories = products
				.Where(x => !x.IsPelt)
				.Select(x => NormaliseSubcategory(x.Subcategory))
				.Append(string.Empty)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();

			foreach (var category in categories)
			{
				profile.RaceButcheryProfilesBreakdownChecks.Add(new RaceButcheryProfilesBreakdownChecks
				{
					RaceButcheryProfile = profile,
					Subcageory = category,
					TraitDefinition = trait,
					Difficulty = (int)(string.IsNullOrEmpty(category) ? Difficulty.Normal : Difficulty.Hard)
				});

				foreach ((var emote, var delay, var order) in BreakdownEmotesFor(category).Select((x, i) => (x.Emote, x.Delay, i + 1)))
				{
					profile.RaceButcheryProfilesBreakdownEmotes.Add(new RaceButcheryProfilesBreakdownEmotes
					{
						RaceButcheryProfile = profile,
						Subcategory = category,
						Emote = emote,
						Delay = delay,
						Order = order
					});
				}
			}

			if (products.Any(x => x.IsPelt))
			{
				foreach ((var emote, var delay, var order) in SkinningEmotes().Select((x, i) => (x.Emote, x.Delay, i + 1)))
				{
					profile.RaceButcheryProfilesSkinningEmotes.Add(new RaceButcheryProfilesSkinningEmotes
					{
						RaceButcheryProfile = profile,
						Subcategory = string.Empty,
						Emote = emote,
						Delay = delay,
						Order = order
					});
				}
			}

			foreach (var product in products.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
			{
				profile.RaceButcheryProfilesButcheryProducts.Add(new RaceButcheryProfilesButcheryProducts
				{
					RaceButcheryProfile = profile,
					ButcheryProduct = product
				});
			}

			results[familyKey] = profile;
		}

		context.SaveChanges();
		return results;
	}

	private static IEnumerable<(string Emote, double Delay)> BreakdownEmotesFor(string category)
	{
		var action = category switch
		{
			"organs" => "sorts through the exposed organs of",
			"trophy" => "works useful hard parts free from",
			"venom" => "carefully cuts delicate glands from",
			"shell" => "separates plates and outer covering from",
			"plume" => "sorts and bundles the best feathers from",
			"blubber" => "trims thick blubber away from",
			"ink" => "works around the ink sac inside",
			"silk" => "draws out the silk-bearing tissues of",
			_ => "cuts into"
		};

		return
		[
			($"$0 {action} $1 with $2.", 8.0),
			($"$0 finishes the butchery work on $1.", 8.0)
		];
	}

	private static IEnumerable<(string Emote, double Delay)> SkinningEmotes()
	{
		return
		[
			("$0 starts working the hide and outer covering away from $1 with $2.", 10.0),
			("$0 finishes skinning $1.", 10.0)
		];
	}

	private static void AssignProfiles(
		FuturemudDatabaseContext context,
		IReadOnlyList<RaceAssignment> assignments,
		IReadOnlyDictionary<string, RaceButcheryProfile> profiles)
	{
		foreach (var assignment in assignments)
		{
			if (assignment.Race.RaceButcheryProfileId is not null && !IsStockProfile(assignment.Race.RaceButcheryProfile))
			{
				continue;
			}

			if (!profiles.TryGetValue(assignment.Family.Key, out var profile))
			{
				continue;
			}

			assignment.Race.RaceButcheryProfile = profile;
			assignment.Race.RaceButcheryProfileId = profile.Id;
		}
	}

	private static string ProfileName(StockButcheryFamilySpec family)
	{
		return $"{StockPrefix} - {family.DisplayName}";
	}

	private static string ProductName(StockButcheryFamilySpec family, BodyProto body, StockButcheryProductSpec product)
	{
		return $"{StockPrefix} Product - {family.Key} - {body.Name} - {product.Key}";
	}

	private static bool IsStockProfile(RaceButcheryProfile? profile)
	{
		return profile is not null && profile.Name.StartsWith(StockPrefix, StringComparison.OrdinalIgnoreCase);
	}

	internal sealed record StockButcheryMaterialSpec(
		string Name,
		MaterialBehaviourType BehaviourType,
		double RelativeDensity,
		bool Organic,
		string TagName
	);

	internal sealed record StockButcheryItemSpec(
		string Key,
		string Noun,
		string ShortDescription,
		string LongDescription,
		string FullDescription,
		SizeCategory Size,
		double WeightInGrams,
		decimal Cost,
		string Material,
		bool Spoils,
		IReadOnlyList<string> Tags
	);

	internal sealed record StockButcheryProductSpec(
		string Key,
		string ItemKey,
		int Quantity,
		IReadOnlyList<IReadOnlyList<string>> RequiredPartGroups,
		string Subcategory = "",
		bool IsPelt = false,
		string? DamagedItemKey = null,
		int DamagedQuantity = 0,
		double DamageThreshold = 0.5
	);

	internal sealed record StockButcheryFamilySpec(
		string Key,
		string DisplayName,
		IReadOnlyList<StockButcheryItemSpec> Items,
		IReadOnlyList<StockButcheryProductSpec> Products
	)
	{
		internal StockButcheryFamilySpec WithTrophy(
			string key,
			string itemKey,
			IReadOnlyList<IReadOnlyList<string>> requiredPartGroups,
			string subcategory)
		{
			return this with
			{
				Products = Products
					.Append(new StockButcheryProductSpec(key, itemKey, 1, requiredPartGroups, subcategory))
					.ToList()
			};
		}
	}

	private sealed class FamilyBuilder
	{
		private readonly string _key;
		private readonly string _displayName;
		private readonly List<StockButcheryItemSpec> _items = new();
		private readonly List<StockButcheryProductSpec> _products = new();

		internal FamilyBuilder(string key, string displayName)
		{
			_key = key;
			_displayName = displayName;
		}

		internal string AddItem(
			string localKey,
			string noun,
			string shortDescription,
			string fullDescription,
			SizeCategory size,
			double weight,
			decimal cost,
			string material,
			bool spoils,
			params string[] tags)
		{
			var key = $"{_key}:{localKey}";
			_items.Add(Item(key, noun, shortDescription, fullDescription, size, weight, cost, material, spoils, tags));
			return key;
		}

		internal void AddProduct(
			string key,
			string itemKey,
			int quantity,
			IReadOnlyList<IReadOnlyList<string>> requiredPartGroups,
			string subcategory = "",
			bool isPelt = false,
			string? damagedItemKey = null,
			int damagedQuantity = 0,
			double damageThreshold = 0.5)
		{
			_products.Add(new StockButcheryProductSpec(
				key,
				itemKey,
				quantity,
				requiredPartGroups,
				NormaliseSubcategory(subcategory),
				isPelt,
				damagedItemKey,
				damagedQuantity,
				damageThreshold
			));
		}

		internal StockButcheryFamilySpec Build()
		{
			return new StockButcheryFamilySpec(_key, _displayName, _items, _products);
		}
	}

	private sealed record RaceAssignment(
		Race Race,
		StockButcheryFamilySpec Family,
		string BodyKey
	);
}
