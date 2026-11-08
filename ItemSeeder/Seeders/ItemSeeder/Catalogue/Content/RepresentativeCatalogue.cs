#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

// Literal records maintained here. Old source definitions remain a temporary differential oracle for stages 1-2.
internal static partial class RepresentativeCatalogue
{
	internal static CatalogueItemDefinition Braies() => new()
	{
		Metadata = new("medieval_linen_braies", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate,
			CatalogueReadiness.ProductionReady, "ItemSeeder.MedievalClothing.cs/SeedMedievalClothing", []),
		Noun = "braies", ShortDescription = "a $colour pair of linen braies", LongDescription = null,
		FullDescription = "These $colour linen braies are cut as loose under-breeches with a gathered waist, roomy seat, and short legs. The linen is light enough to sit beneath hose or trousers, with practical seams kept flat against the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small, Quality = ItemQuality.Standard, WeightInGrams = 160.0, Cost = 6.0m,
		Skinnable = true, HiddenFromPlayers = false, Material = "linen",
		Tags = ["Market / Clothing / Standard Clothing", "Functions / Worn Items / Bodywear"],
		Components = ["Holdable", "Wear_Shorts", "Destroyable_Clothing", "Insulation_Minor", "Armour_LightClothing", "Variable_BasicColour"],
		MorphToStableReference = null, MorphEmote = null, MorphSeconds = 0, DestroyedItemStableReference = null,
		BuilderNotes = null, AllowLegacyShortDescriptionMatch = true
	};

	internal static CatalogueOutfitDefinition EarlyAngloSaxon() => new(
		new("medieval_outfit_0001", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"],
			["item:medieval_linen_undertunic", "item:medieval_narrow_wool_trousers", "item:medieval_plain_leather_belt",
			 "item:medieval_rectangular_wool_cloak", "item:medieval_soft_leather_shoes", "item:medieval_wool_leg_wraps", "item:medieval_wool_tunic"], ItemSeederOwnershipPolicy.StockAggregate,
			CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs[0]", []),
		"Medieval Early Anglo-Saxon common male", "Builder-facing medieval clothing outfit for Early Anglo-Saxon common male.",
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_narrow_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_leg_wraps", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_rectangular_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]);

	internal static CatalogueCraftDefinition WickerBeehive() => new()
	{
		Metadata = new("Basketry/weave wicker beehive", "crafts", ["antiquity"], ["item:antiquity_wicker_beehive"],
			ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.Crafting.AntiquityApiary.cs/SeedAntiquityApiaryCrafts", []),
		UseExplicitStableReference = false, PreserveAuthoredText = false, Name = "weave wicker beehive", Category = "Basketry",
		Blurb = "weave and seal a wicker beehive", Action = "weaving a wicker beehive", ActiveCraftItemSdesc = "a wicker hive under construction",
		Knowledge = "Ancient Apiculture", KnowledgeSubtype = "General", Trait = "Basketry", MinimumTraitValue = 15,
		OnStartProg = null, OnFinishProg = null, OnCancelProg = null, Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail, FreeChecks = 5, FailPhase = 3, Interruptable = false,
		Phases =
		[
			new() { Seconds = 35, Echo = "$0 soak|soaks and flex|flexes the splints from $i1, sorting them by thickness.", FailEcho = "$0 kink|kinks too many splints while preparing them.", Exertion = ExertionLevel.Stasis, Stamina = 0 },
			new() { Seconds = 45, Echo = "$0 weave|weaves the hive body upward in a tight coil, packing each course with $t2.", FailEcho = "$0 weave|weaves the hive body unevenly and leaves gaps in the coil.", Exertion = ExertionLevel.Stasis, Stamina = 0 },
			new() { Seconds = 35, Echo = "$0 bind|binds the lip with $i2 and smear|smears $i3 into the seams.", FailEcho = "$0 bind|binds the lip poorly and the hive will not hold its shape.", Exertion = ExertionLevel.Stasis, Stamina = 0 },
			new() { Seconds = 25, Echo = "$0 set|sets aside $p1 after checking the entrance and sealed crown.", FailEcho = "$0 set|sets aside only $f1 after the hive body collapses.", Exertion = ExertionLevel.Stasis, Stamina = 0 }
		],
		Inputs = [new(1800.0, "willow", "Basketry Splint", 1.0), new(150.0, "linen", "Spun Yarn", 1.0), new(120.0, "pitch", "Prepared Pitch", 1.0)],
		Tools = [new("Basket Knife", DesiredItemState.Held, 1.0, true), new("Packing Bone", DesiredItemState.Held, 1.0, true)],
		Products = [new("antiquity_wicker_beehive", 1, false, null)], FailProducts = []
	};
}
