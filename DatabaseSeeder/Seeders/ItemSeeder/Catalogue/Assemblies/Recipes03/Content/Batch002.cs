#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes03Stock
{
	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadBackedSeax() => new()
	{
		Metadata = new("Military Equipment/make broad-backed seax", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_seax_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad-backed seax",
		Category = "Military Equipment",
		Blurb = "make a broad-backed seax",
		Action = "makeing a broad-backed seax",
		ActiveCraftItemSdesc = "an in-progress broad-backed seax craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_seax_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad-backed seax (@medieval_military_broad_seax_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadSeaxSheath() => new()
	{
		Metadata = new("Military Equipment/make broad seax sheath", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_seax_sheath"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad seax sheath",
		Category = "Military Equipment",
		Blurb = "make a broad seax sheath",
		Action = "makeing a broad seax sheath",
		ActiveCraftItemSdesc = "an in-progress broad seax sheath craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_seax_sheath", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad seax sheath (@medieval_military_broad_seax_sheath)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadThrowingKnife() => new()
	{
		Metadata = new("Military Equipment/make broad throwing knife", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_throwing_knife"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad throwing knife",
		Category = "Military Equipment",
		Blurb = "make a broad throwing knife",
		Action = "makeing a broad throwing knife",
		ActiveCraftItemSdesc = "an in-progress broad throwing knife craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 195.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("195 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_throwing_knife", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad throwing knife (@medieval_military_broad_throwing_knife)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadWarArrowQuiver() => new()
	{
		Metadata = new("Military Equipment/make broad war-arrow quiver", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_war_arrow_quiver"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad war-arrow quiver",
		Category = "Military Equipment",
		Blurb = "make a broad war-arrow quiver",
		Action = "makeing a broad war-arrow quiver",
		ActiveCraftItemSdesc = "an in-progress broad war-arrow quiver craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_war_arrow_quiver", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad war-arrow quiver (@medieval_military_broad_war_arrow_quiver)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadWarRoundShield() => new()
	{
		Metadata = new("Military Equipment/make broad war round shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_war_round_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad war round shield",
		Category = "Military Equipment",
		Blurb = "make a broad war round shield",
		Action = "makeing a broad war round shield",
		ActiveCraftItemSdesc = "an in-progress broad war round shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3150.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3150 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_war_round_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad war round shield (@medieval_military_broad_war_round_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadWarSword() => new()
	{
		Metadata = new("Military Equipment/make broad war sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_war_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad war sword",
		Category = "Military Equipment",
		Blurb = "make a broad war sword",
		Action = "makeing a broad war sword",
		ActiveCraftItemSdesc = "an in-progress broad war sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1028.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1028 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_war_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad war sword (@medieval_military_broad_war_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadWeaponBaldric() => new()
	{
		Metadata = new("Military Equipment/make broad weapon baldric", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_weapon_baldric"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad weapon baldric",
		Category = "Military Equipment",
		Blurb = "make a broad weapon baldric",
		Action = "makeing a broad weapon baldric",
		ActiveCraftItemSdesc = "an in-progress broad weapon baldric craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_weapon_baldric", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad weapon baldric (@medieval_military_broad_weapon_baldric)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBroadWeaponBelt() => new()
	{
		Metadata = new("Military Equipment/make broad weapon belt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_broad_weapon_belt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make broad weapon belt",
		Category = "Military Equipment",
		Blurb = "make a broad weapon belt",
		Action = "makeing a broad weapon belt",
		ActiveCraftItemSdesc = "an in-progress broad weapon belt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 420.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("420 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_broad_weapon_belt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a broad weapon belt (@medieval_military_broad_weapon_belt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBronzeFittedSwordScabbard() => new()
	{
		Metadata = new("Military Equipment/make bronze-fitted sword scabbard", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bronze_fitted_sword_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze-fitted sword scabbard",
		Category = "Military Equipment",
		Blurb = "make a bronze-fitted sword scabbard",
		Action = "makeing a bronze-fitted sword scabbard",
		ActiveCraftItemSdesc = "an in-progress bronze-fitted sword scabbard craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 615.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("615 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bronze_fitted_sword_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze-fitted sword scabbard (@medieval_military_bronze_fitted_sword_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBronzeHeadedMace() => new()
	{
		Metadata = new("Military Equipment/make bronze-headed mace", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bronze_headed_mace"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze-headed mace",
		Category = "Military Equipment",
		Blurb = "make a bronze-headed mace",
		Action = "makeing a bronze-headed mace",
		ActiveCraftItemSdesc = "an in-progress bronze-headed mace craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 885.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("885 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bronze_headed_mace", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze-headed mace (@medieval_military_bronze_headed_mace)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBronzeScaleCuirass() => new()
	{
		Metadata = new("Military Equipment/make bronze scale cuirass", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bronze_scale_cuirass"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bronze scale cuirass",
		Category = "Military Equipment",
		Blurb = "make a bronze scale cuirass",
		Action = "makeing a bronze scale cuirass",
		ActiveCraftItemSdesc = "an in-progress bronze scale cuirass craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4200.0, Material: "bronze", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4200 grams of bronze", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bronze_scale_cuirass", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bronze scale cuirass (@medieval_military_bronze_scale_cuirass)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBucklerCarryingPouch() => new()
	{
		Metadata = new("Military Equipment/make buckler carrying pouch", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_buckler_pouch"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make buckler carrying pouch",
		Category = "Military Equipment",
		Blurb = "make a buckler carrying pouch",
		Action = "makeing a buckler carrying pouch",
		ActiveCraftItemSdesc = "an in-progress buckler carrying pouch craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 315.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("315 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_buckler_pouch", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a buckler carrying pouch (@medieval_military_buckler_pouch)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBundleOfHempBowstrings() => new()
	{
		Metadata = new("Military Equipment/make bundle of hemp bowstrings", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bundle_of_hemp_bowstrings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bundle of hemp bowstrings",
		Category = "Military Equipment",
		Blurb = "make a bundle of hemp bowstrings",
		Action = "makeing a bundle of hemp bowstrings",
		ActiveCraftItemSdesc = "an in-progress bundle of hemp bowstrings craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 34.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("34 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bundle_of_hemp_bowstrings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bundle of hemp bowstrings (@medieval_military_bundle_of_hemp_bowstrings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeBundleOfLinenBowstrings() => new()
	{
		Metadata = new("Military Equipment/make bundle of linen bowstrings", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_bundle_of_linen_bowstrings"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make bundle of linen bowstrings",
		Category = "Military Equipment",
		Blurb = "make a bundle of linen bowstrings",
		Action = "makeing a bundle of linen bowstrings",
		ActiveCraftItemSdesc = "an in-progress bundle of linen bowstrings craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 30.0, Material: "linen", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30 grams of linen", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_bundle_of_linen_bowstrings", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a bundle of linen bowstrings (@medieval_military_bundle_of_linen_bowstrings)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakePlainCampaignPavise() => new()
	{
		Metadata = new("Military Equipment/make plain campaign pavise", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_campaign_pavise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain campaign pavise",
		Category = "Military Equipment",
		Blurb = "make a plain campaign pavise",
		Action = "makeing a plain campaign pavise",
		ActiveCraftItemSdesc = "an in-progress plain campaign pavise craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5400.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5400 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_campaign_pavise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain campaign pavise (@medieval_military_campaign_pavise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeWovenCaneShield() => new()
	{
		Metadata = new("Military Equipment/make woven cane shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_cane_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make woven cane shield",
		Category = "Military Equipment",
		Blurb = "make a woven cane shield",
		Action = "makeing a woven cane shield",
		ActiveCraftItemSdesc = "an in-progress woven cane shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1200.0, Material: "cane", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1200 grams of cane", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_cane_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a woven cane shield (@medieval_military_cane_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCanvasLongbowSleeve() => new()
	{
		Metadata = new("Military Equipment/make canvas longbow sleeve", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_canvas_longbow_sleeve"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make canvas longbow sleeve",
		Category = "Military Equipment",
		Blurb = "make a canvas longbow sleeve",
		Action = "makeing a canvas longbow sleeve",
		ActiveCraftItemSdesc = "an in-progress canvas longbow sleeve craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 270.0, Material: "canvas", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("270 grams of canvas", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_canvas_longbow_sleeve", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a canvas longbow sleeve (@medieval_military_canvas_longbow_sleeve)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeLightCavalryAdarga() => new()
	{
		Metadata = new("Military Equipment/make light cavalry adarga", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_cavalry_adarga"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make light cavalry adarga",
		Category = "Military Equipment",
		Blurb = "make a light cavalry adarga",
		Action = "makeing a light cavalry adarga",
		ActiveCraftItemSdesc = "an in-progress light cavalry adarga craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1125.0, Material: "rawhide", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1125 grams of rawhide", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_cavalry_adarga", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a light cavalry adarga (@medieval_military_cavalry_adarga)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeChippedSlingStone() => new()
	{
		Metadata = new("Military Equipment/make chipped sling stone", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_chipped_sling_stone"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make chipped sling stone",
		Category = "Military Equipment",
		Blurb = "make a chipped sling stone",
		Action = "makeing a chipped sling stone",
		ActiveCraftItemSdesc = "an in-progress chipped sling stone craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 71.0, Material: "flint", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("71 grams of flint", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_chipped_sling_stone", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a chipped sling stone (@medieval_military_chipped_sling_stone)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCoatOfPlates() => new()
	{
		Metadata = new("Military Equipment/make coat of plates", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_coat_of_plates"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make coat of plates",
		Category = "Military Equipment",
		Blurb = "make a coat of plates",
		Action = "makeing a coat of plates",
		ActiveCraftItemSdesc = "an in-progress coat of plates craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 5700.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("5700 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_coat_of_plates", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a coat of plates (@medieval_military_coat_of_plates)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCompactHandCrossbow() => new()
	{
		Metadata = new("Military Equipment/make compact hand crossbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_compact_hand_crossbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact hand crossbow",
		Category = "Military Equipment",
		Blurb = "make a compact hand crossbow",
		Action = "makeing a compact hand crossbow",
		ActiveCraftItemSdesc = "an in-progress compact hand crossbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 975.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("975 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_compact_hand_crossbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact hand crossbow (@medieval_military_compact_hand_crossbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCompactHeaterShield() => new()
	{
		Metadata = new("Military Equipment/make compact heater shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_compact_heater_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact heater shield",
		Category = "Military Equipment",
		Blurb = "make a compact heater shield",
		Action = "makeing a compact heater shield",
		ActiveCraftItemSdesc = "an in-progress compact heater shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2100.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2100 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_compact_heater_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact heater shield (@medieval_military_compact_heater_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCompactKiteShield() => new()
	{
		Metadata = new("Military Equipment/make compact kite shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_compact_kite_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact kite shield",
		Category = "Military Equipment",
		Blurb = "make a compact kite shield",
		Action = "makeing a compact kite shield",
		ActiveCraftItemSdesc = "an in-progress compact kite shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 3225.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3225 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_compact_kite_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact kite shield (@medieval_military_compact_kite_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeConicalSteelHelm() => new()
	{
		Metadata = new("Military Equipment/make conical steel helm", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_conical_steel_helm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make conical steel helm",
		Category = "Military Equipment",
		Blurb = "make a conical steel helm",
		Action = "makeing a conical steel helm",
		ActiveCraftItemSdesc = "an in-progress conical steel helm craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1013.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1013 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_conical_steel_helm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a conical steel helm (@medieval_military_conical_steel_helm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeColourCrossbowmanSPavise() => new()
	{
		Metadata = new("Military Equipment/make $colour crossbowman's pavise", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_crossbowmans_pavise"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make $colour crossbowman's pavise",
		Category = "Military Equipment",
		Blurb = "make a $colour crossbowman's pavise",
		Action = "makeing a $colour crossbowman's pavise",
		ActiveCraftItemSdesc = "an in-progress $colour crossbowman's pavise craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4875.0, Material: "poplar", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4875 grams of poplar", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_crossbowmans_pavise", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a $colour crossbowman's pavise (@medieval_military_crossbowmans_pavise)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCrossbowCarryingCase() => new()
	{
		Metadata = new("Military Equipment/make crossbow carrying case", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_crossbow_carrying_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make crossbow carrying case",
		Category = "Military Equipment",
		Blurb = "make a crossbow carrying case",
		Action = "makeing a crossbow carrying case",
		ActiveCraftItemSdesc = "an in-progress crossbow carrying case craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1125.0, Material: "cow leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1125 grams of cow leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_crossbow_carrying_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a crossbow carrying case (@medieval_military_crossbow_carrying_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCrossbowRack() => new()
	{
		Metadata = new("Military Equipment/make crossbow rack", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_crossbow_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make crossbow rack",
		Category = "Military Equipment",
		Blurb = "make a crossbow rack",
		Action = "makeing a crossbow rack",
		ActiveCraftItemSdesc = "an in-progress crossbow rack craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21000.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21000 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_crossbow_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a crossbow rack (@medieval_military_crossbow_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCrucibleSteelDagger() => new()
	{
		Metadata = new("Military Equipment/make crucible-steel dagger", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_crucible_steel_dagger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make crucible-steel dagger",
		Category = "Military Equipment",
		Blurb = "make a crucible-steel dagger",
		Action = "makeing a crucible-steel dagger",
		ActiveCraftItemSdesc = "an in-progress crucible-steel dagger craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 233.0, Material: "crucible steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("233 grams of crucible steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_crucible_steel_dagger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a crucible-steel dagger (@medieval_military_crucible_steel_dagger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeCrucibleSteelSword() => new()
	{
		Metadata = new("Military Equipment/make crucible-steel sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_crucible_steel_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make crucible-steel sword",
		Category = "Military Equipment",
		Blurb = "make a crucible-steel sword",
		Action = "makeing a crucible-steel sword",
		ActiveCraftItemSdesc = "an in-progress crucible-steel sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 810.0, Material: "crucible steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("810 grams of crucible steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_crucible_steel_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a crucible-steel sword (@medieval_military_crucible_steel_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeDoorBoardShield() => new()
	{
		Metadata = new("Military Equipment/make door-board shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_door_board_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make door-board shield",
		Category = "Military Equipment",
		Blurb = "make a door-board shield",
		Action = "makeing a door-board shield",
		ActiveCraftItemSdesc = "an in-progress door-board shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4875.0, Material: "wood", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4875 grams of wood", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_door_board_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a door-board shield (@medieval_military_door_board_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeDoubleHangerSwordBelt() => new()
	{
		Metadata = new("Military Equipment/make double-hanger sword belt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_double_hanger_sword_belt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make double-hanger sword belt",
		Category = "Military Equipment",
		Blurb = "make a double-hanger sword belt",
		Action = "makeing a double-hanger sword belt",
		ActiveCraftItemSdesc = "an in-progress double-hanger sword belt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 390.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("390 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_double_hanger_sword_belt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a double-hanger sword belt (@medieval_military_double_hanger_sword_belt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeSimpleMetalCouters() => new()
	{
		Metadata = new("Military Equipment/make simple metal couters", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_early_metal_couters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make simple metal couters",
		Category = "Military Equipment",
		Blurb = "make a pair of simple metal couters",
		Action = "makeing a pair of simple metal couters",
		ActiveCraftItemSdesc = "an in-progress pair of simple metal couters craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_early_metal_couters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of simple metal couters (@medieval_military_early_metal_couters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeSimplePoleyns() => new()
	{
		Metadata = new("Military Equipment/make simple poleyns", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_early_poleyns"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make simple poleyns",
		Category = "Military Equipment",
		Blurb = "make a pair of simple poleyns",
		Action = "makeing a pair of simple poleyns",
		ActiveCraftItemSdesc = "an in-progress pair of simple poleyns craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 615.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("615 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_early_poleyns", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a pair of simple poleyns (@medieval_military_early_poleyns)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeElmSelfLongbow() => new()
	{
		Metadata = new("Military Equipment/make elm self longbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_elm_self_longbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make elm self longbow",
		Category = "Military Equipment",
		Blurb = "make an elm self longbow",
		Action = "makeing an elm self longbow",
		ActiveCraftItemSdesc = "an in-progress elm self longbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 705.0, Material: "elm", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("705 grams of elm", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_elm_self_longbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an elm self longbow (@medieval_military_elm_self_longbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFeltRidingArmourCoat() => new()
	{
		Metadata = new("Military Equipment/make felt riding armour coat", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_felt_riding_armour"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make felt riding armour coat",
		Category = "Military Equipment",
		Blurb = "make a felt riding armour coat",
		Action = "makeing a felt riding armour coat",
		ActiveCraftItemSdesc = "an in-progress felt riding armour coat craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2925.0, Material: "felt", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2925 grams of felt", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_felt_riding_armour", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a felt riding armour coat (@medieval_military_felt_riding_armour)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFieldMattockWeapon() => new()
	{
		Metadata = new("Military Equipment/make field mattock weapon", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_field_mattock_weapon"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field mattock weapon",
		Category = "Military Equipment",
		Blurb = "make a field mattock weapon",
		Action = "makeing a field mattock weapon",
		ActiveCraftItemSdesc = "an in-progress field mattock weapon craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1575.0, Material: "wrought iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1575 grams of wrought iron", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_field_mattock_weapon", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field mattock weapon (@medieval_military_field_mattock_weapon)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFieldPointArrow() => new()
	{
		Metadata = new("Military Equipment/make field-point arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_field_point_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field-point arrow",
		Category = "Military Equipment",
		Blurb = "make a field-point arrow",
		Action = "makeing a field-point arrow",
		ActiveCraftItemSdesc = "an in-progress field-point arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 26.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("26 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_field_point_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field-point arrow (@medieval_military_field_point_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFieldPointBolt() => new()
	{
		Metadata = new("Military Equipment/make field-point bolt", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_field_point_bolt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field-point bolt",
		Category = "Military Equipment",
		Blurb = "make a field-point bolt",
		Action = "makeing a field-point bolt",
		ActiveCraftItemSdesc = "an in-progress field-point bolt craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 38.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("38 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_field_point_bolt", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field-point bolt (@medieval_military_field_point_bolt)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineAxeHeadedPolearm() => new()
	{
		Metadata = new("Military Equipment/make fine axe-headed polearm", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_axe_headed_polearm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine axe-headed polearm",
		Category = "Military Equipment",
		Blurb = "make a fine axe-headed polearm",
		Action = "makeing a fine axe-headed polearm",
		ActiveCraftItemSdesc = "an in-progress fine axe-headed polearm craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1650.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1650 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_axe_headed_polearm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine axe-headed polearm (@medieval_military_fine_axe_headed_polearm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineBalancedJavelin() => new()
	{
		Metadata = new("Military Equipment/make fine balanced javelin", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_balanced_javelin"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine balanced javelin",
		Category = "Military Equipment",
		Blurb = "make a fine balanced javelin",
		Action = "makeing a fine balanced javelin",
		ActiveCraftItemSdesc = "an in-progress fine balanced javelin craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 660.0, Material: "ash", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("660 grams of ash", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_balanced_javelin", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine balanced javelin (@medieval_military_fine_balanced_javelin)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineBoltCase() => new()
	{
		Metadata = new("Military Equipment/make fine bolt case", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_bolt_case"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine bolt case",
		Category = "Military Equipment",
		Blurb = "make a fine bolt case",
		Action = "makeing a fine bolt case",
		ActiveCraftItemSdesc = "an in-progress fine bolt case craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 540.0, Material: "goat leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("540 grams of goat leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_bolt_case", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine bolt case (@medieval_military_fine_bolt_case)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineBroadheadArrow() => new()
	{
		Metadata = new("Military Equipment/make fine broadhead arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_broadhead_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine broadhead arrow",
		Category = "Military Equipment",
		Blurb = "make a fine broadhead arrow",
		Action = "makeing a fine broadhead arrow",
		ActiveCraftItemSdesc = "an in-progress fine broadhead arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 30.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("30 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_broadhead_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine broadhead arrow (@medieval_military_fine_broadhead_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineCastLeadSlingBullet() => new()
	{
		Metadata = new("Military Equipment/make fine cast lead sling bullet", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_cast_lead_sling_bullet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine cast lead sling bullet",
		Category = "Military Equipment",
		Blurb = "make a fine cast lead sling bullet",
		Action = "makeing a fine cast lead sling bullet",
		ActiveCraftItemSdesc = "an in-progress fine cast lead sling bullet craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 47.0, Material: "lead", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("47 grams of lead", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_cast_lead_sling_bullet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine cast lead sling bullet (@medieval_military_fine_cast_lead_sling_bullet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineCordSling() => new()
	{
		Metadata = new("Military Equipment/make fine cord sling", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_cord_sling"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine cord sling",
		Category = "Military Equipment",
		Blurb = "make a fine cord sling",
		Action = "makeing a fine cord sling",
		ActiveCraftItemSdesc = "an in-progress fine cord sling craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 64.0, Material: "hemp", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("64 grams of hemp", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_cord_sling", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine cord sling (@medieval_military_fine_cord_sling)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineArmourDisplayStand() => new()
	{
		Metadata = new("Military Equipment/make fine armour display stand", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_display_armour_stand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine armour display stand",
		Category = "Military Equipment",
		Blurb = "make a fine armour display stand",
		Action = "makeing a fine armour display stand",
		ActiveCraftItemSdesc = "an in-progress fine armour display stand craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 21000.0, Material: "walnut", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("21000 grams of walnut", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_display_armour_stand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine armour display stand (@medieval_military_fine_display_armour_stand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineColourDaggerSheath() => new()
	{
		Metadata = new("Military Equipment/make fine $colour dagger sheath", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_dyed_dagger_sheath"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine $colour dagger sheath",
		Category = "Military Equipment",
		Blurb = "make a fine $colour dagger sheath",
		Action = "makeing a fine $colour dagger sheath",
		ActiveCraftItemSdesc = "an in-progress fine $colour dagger sheath craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 158.0, Material: "goat leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("158 grams of goat leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_dyed_dagger_sheath", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour dagger sheath (@medieval_military_fine_dyed_dagger_sheath)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineColourSwordScabbard() => new()
	{
		Metadata = new("Military Equipment/make fine $colour sword scabbard", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_dyed_sword_scabbard"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine $colour sword scabbard",
		Category = "Military Equipment",
		Blurb = "make a fine $colour sword scabbard",
		Action = "makeing a fine $colour sword scabbard",
		ActiveCraftItemSdesc = "an in-progress fine $colour sword scabbard craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 585.0, Material: "goat leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("585 grams of goat leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_dyed_sword_scabbard", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour sword scabbard (@medieval_military_fine_dyed_sword_scabbard)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineFieldPointArrow() => new()
	{
		Metadata = new("Military Equipment/make fine field-point arrow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_field_point_arrow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine field-point arrow",
		Category = "Military Equipment",
		Blurb = "make a fine field-point arrow",
		Action = "makeing a fine field-point arrow",
		ActiveCraftItemSdesc = "an in-progress fine field-point arrow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 25.0, Material: "birch", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("25 grams of birch", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_field_point_arrow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine field-point arrow (@medieval_military_fine_field_point_arrow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFinelyFittedGreatHelm() => new()
	{
		Metadata = new("Military Equipment/make finely fitted great helm", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_greathelm"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make finely fitted great helm",
		Category = "Military Equipment",
		Blurb = "make a finely fitted great helm",
		Action = "makeing a finely fitted great helm",
		ActiveCraftItemSdesc = "an in-progress finely fitted great helm craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1988.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1988 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_greathelm", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a finely fitted great helm (@medieval_military_fine_greathelm)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineHandCrossbow() => new()
	{
		Metadata = new("Military Equipment/make fine hand crossbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_hand_crossbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine hand crossbow",
		Category = "Military Equipment",
		Blurb = "make a fine hand crossbow",
		Action = "makeing a fine hand crossbow",
		ActiveCraftItemSdesc = "an in-progress fine hand crossbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 900.0, Material: "horn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("900 grams of horn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_hand_crossbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine hand crossbow (@medieval_military_fine_hand_crossbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineHornNockedLongbow() => new()
	{
		Metadata = new("Military Equipment/make fine horn-nocked longbow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_horn_nocked_longbow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine horn-nocked longbow",
		Category = "Military Equipment",
		Blurb = "make a fine horn-nocked longbow",
		Action = "makeing a fine horn-nocked longbow",
		ActiveCraftItemSdesc = "an in-progress fine horn-nocked longbow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 735.0, Material: "yew", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("735 grams of yew", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_horn_nocked_longbow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine horn-nocked longbow (@medieval_military_fine_horn_nocked_longbow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineHornRecurvedBow() => new()
	{
		Metadata = new("Military Equipment/make fine horn recurved bow", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_horn_recurve_bow"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine horn recurved bow",
		Category = "Military Equipment",
		Blurb = "make a fine horn recurved bow",
		Action = "makeing a fine horn recurved bow",
		ActiveCraftItemSdesc = "an in-progress fine horn recurved bow craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 465.0, Material: "horn", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("465 grams of horn", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_horn_recurve_bow", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine horn recurved bow (@medieval_military_fine_horn_recurve_bow)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineKnightlyHeaterShield() => new()
	{
		Metadata = new("Military Equipment/make fine knightly heater shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_knightly_heater_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine knightly heater shield",
		Category = "Military Equipment",
		Blurb = "make a fine knightly heater shield",
		Action = "makeing a fine knightly heater shield",
		ActiveCraftItemSdesc = "an in-progress fine knightly heater shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2400.0, Material: "linden", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2400 grams of linden", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_knightly_heater_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine knightly heater shield (@medieval_military_fine_knightly_heater_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineLacqueredBowcase() => new()
	{
		Metadata = new("Military Equipment/make fine lacquered bowcase", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_lacquered_bowcase"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine lacquered bowcase",
		Category = "Military Equipment",
		Blurb = "make a fine lacquered bowcase",
		Action = "makeing a fine lacquered bowcase",
		ActiveCraftItemSdesc = "an in-progress fine lacquered bowcase craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 570.0, Material: "lacquer", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("570 grams of lacquer", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_lacquered_bowcase", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine lacquered bowcase (@medieval_military_fine_lacquered_bowcase)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineColourLeatherAdarga() => new()
	{
		Metadata = new("Military Equipment/make fine $colour leather adarga", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_leather_adarga"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine $colour leather adarga",
		Category = "Military Equipment",
		Blurb = "make a fine $colour leather adarga",
		Action = "makeing a fine $colour leather adarga",
		ActiveCraftItemSdesc = "an in-progress fine $colour leather adarga craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1050.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1050 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_leather_adarga", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour leather adarga (@medieval_military_fine_leather_adarga)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineColourCaparison() => new()
	{
		Metadata = new("Military Equipment/make fine $colour caparison", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_livery_caparison"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine $colour caparison",
		Category = "Military Equipment",
		Blurb = "make a fine $colour caparison",
		Action = "makeing a fine $colour caparison",
		ActiveCraftItemSdesc = "an in-progress fine $colour caparison craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2400.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2400 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_livery_caparison", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine $colour caparison (@medieval_military_fine_livery_caparison)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineLongGrippedWarSword() => new()
	{
		Metadata = new("Military Equipment/make fine long-gripped war sword", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_long_gripped_war_sword"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine long-gripped war sword",
		Category = "Military Equipment",
		Blurb = "make a fine long-gripped war sword",
		Action = "makeing a fine long-gripped war sword",
		ActiveCraftItemSdesc = "an in-progress fine long-gripped war sword craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1485.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1485 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_long_gripped_war_sword", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine long-gripped war sword (@medieval_military_fine_long_gripped_war_sword)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFineLongWarAxe() => new()
	{
		Metadata = new("Military Equipment/make fine long war axe", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_long_war_axe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine long war axe",
		Category = "Military Equipment",
		Blurb = "make a fine long war axe",
		Action = "makeing a fine long war axe",
		ActiveCraftItemSdesc = "an in-progress fine long war axe craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1380.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1380 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_long_war_axe", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine long war axe (@medieval_military_fine_long_war_axe)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFinePointedKiteShield() => new()
	{
		Metadata = new("Military Equipment/make fine pointed kite shield", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_pointed_kite_shield"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine pointed kite shield",
		Category = "Military Equipment",
		Blurb = "make a fine pointed kite shield",
		Action = "makeing a fine pointed kite shield",
		ActiveCraftItemSdesc = "an in-progress fine pointed kite shield craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 4050.0, Material: "linden", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4050 grams of linden", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_pointed_kite_shield", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine pointed kite shield (@medieval_military_fine_pointed_kite_shield)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition MilitaryEquipmentMakeFinePolishedDhal() => new()
	{
		Metadata = new("Military Equipment/make fine polished dhal", "crafts", ["earlymodern", "medieval", "renaissance"], ["item:medieval_military_fine_polished_dhal"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Medieval.cs:893", []),
		RequiredAnyEra = ["medieval"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make fine polished dhal",
		Category = "Military Equipment",
		Blurb = "make a fine polished dhal",
		Action = "makeing a fine polished dhal",
		ActiveCraftItemSdesc = "an in-progress fine polished dhal craft",
		Gate = CatalogueRecipeGates.MedievalEquipmentCraftingWeaponcrafting10,
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
			new(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock and lay|lays out the work.", ExertionLevel.Stasis, 0),
			new(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes and fit|fits the prepared pieces with $t1.", ExertionLevel.Stasis, 0),
			new(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1350.0, Material: "carbon steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1350 grams of carbon steel", []), QualityWeight = 1 },
		],
		Tools = [
			new CatalogueTagRecipeTool(Tag: "Hammer", State: DesiredItemState.Held) { Compatibility = new("Held - an item with the Hammer tag", []), QualityWeight = 1, UseToolDuration = true },
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "medieval_military_fine_polished_dhal", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a fine polished dhal (@medieval_military_fine_polished_dhal)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
