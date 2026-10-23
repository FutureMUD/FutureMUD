#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Authored data hidden by the original first-registration-wins craft identity collision.
/// Retained for review; installing it requires a deliberate new stock identity.</summary>
internal static class SuppressedLegacyRecipeVariants
{
	public static CatalogueRecipeDefinition SuppressedKushiteTallArrowQuiver() => new()
	{
		Metadata = new("Leatherworking/assemble a tall leather arrow quiver", "crafts", ["antiquity"], ["item:antiquity_accessory_common_tall_arrow_quiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.DependencyReviewed, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Antiquity.cs:140", []),
		RequiredAnyEra = ["antiquity"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "assemble a tall leather arrow quiver",
		Category = "Leatherworking",
		Blurb = "assemble a tall leather arrow quiver from prepared leather container pieces",
		Action = "assemble a tall leather arrow quiver",
		ActiveCraftItemSdesc = "leather container pieces being assembled into a tall leather arrow quiver",
		Gate = CatalogueRecipeGates.AncientHideAndLeatherworkingLeathermaking25,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 3,
		Interruptable = false,
		Phases = [
			new(30, "$0 mark|marks the leather panels, straps, flap, and closure lines.", "$0 mark|marks the leather panels, straps, flap, and closure lines.", ExertionLevel.Stasis, 0),
			new(40, "$0 punch|punches stitch holes with $t1 and finish|finishes the edges with $t3 and $t4.", "$0 punch|punches stitch holes with $t1 and finish|finishes the edges with $t3 and $t4.", ExertionLevel.Stasis, 0),
			new(50, "$0 stitch|stitches the container on $t2, drawing the thongs snug.", "$0 stitch|stitches on $t2, but the container twists out of shape.", ExertionLevel.Stasis, 0),
			new(25, "$0 test|tests the closure and set|sets aside $p1.", "$0 salvage|salvages only $f1 from the failed container work.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 560.0, Material: "leather", PileTag: "Leather Panel", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("560 grams of leather", ["piletag Leather Panel", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 200.0, Material: "leather", PileTag: "Leather Strap", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("200 grams of leather", ["piletag Leather Strap", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 70.0, Material: "leather", PileTag: "Leather Thong", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null), new("Fine Colour", null)]) { Compatibility = new("70 grams of leather", ["piletag Leather Thong", "characteristic Colour any", "characteristic Fine Colour any"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 35.0, Material: "linen", PileTag: "Spun Yarn", CharacteristicMode: CatalogueCharacteristicMode.Specific, Characteristics: [new("Colour", null)]) { Compatibility = new("35 grams of linen", ["piletag Spun Yarn", "characteristic Colour any"]), QualityWeight = 1 },
			new CatalogueCommodityRecipeInput(Grams: 35.0, Material: "beeswax", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("35 grams of beeswax", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Awl Punch", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Awl Punch tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Leather Stitching Pony", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Leather Stitching Pony tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Edge Beveller", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Edge Beveller tag", []), QualityWeight = 1, UseToolDuration = true },
			new CatalogueTagRecipeTool(Tag: "Leather Creaser", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Leather Creaser tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "antiquity_accessory_kushite_tall_arrow_quiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a tall leather arrow quiver (@antiquity_accessory_kushite_tall_arrow_quiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
