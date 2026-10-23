#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactMildSteelVestForDomesticProtectionCraftIndustrialisedPpeBallistic0029CompactVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0029_compact_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0029_compact_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0029_compact_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact mild steel vest for domestic protection from prepared mild steel stock.",
		Action = "making compact mild steel vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact mild steel vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1458.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1458 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0029_compact_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel vest for domestic protection (@industrialised_ppe_ballistic_0029_compact_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCompactLeatherCoatForDomesticProtectionCraftIndustrialisedPpeBallistic0030CompactCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make compact leather coat for domestic protection [craft_industrialised_ppe_ballistic_0030_compact_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0030_compact_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather coat for domestic protection [craft_industrialised_ppe_ballistic_0030_compact_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a compact leather coat for domestic protection from prepared leather stock.",
		Action = "making compact leather coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress compact leather coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1498.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1498 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0030_compact_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather coat for domestic protection (@industrialised_ppe_ballistic_0030_compact_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyCottonLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0031HeavyDutyLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0031_heavy_duty_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0031_heavy_duty_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0031_heavy_duty_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty cotton liner for domestic protection from prepared cotton stock.",
		Action = "making heavy-duty cotton liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1539.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1539 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0031_heavy_duty_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton liner for domestic protection (@industrialised_ppe_ballistic_0031_heavy_duty_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyWoolApronForDomesticProtectionCraftIndustrialisedPpeBallistic0032HeavyDutyApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty wool apron for domestic protection [craft_industrialised_ppe_ballistic_0032_heavy_duty_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0032_heavy_duty_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty wool apron for domestic protection [craft_industrialised_ppe_ballistic_0032_heavy_duty_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty wool apron for domestic protection from prepared wool stock.",
		Action = "making heavy-duty wool apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty wool apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1580.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1580 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0032_heavy_duty_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty wool apron for domestic protection (@industrialised_ppe_ballistic_0032_heavy_duty_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeHeavyDutyMildSteelGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0033HeavyDutyGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some heavy-duty mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0033_heavy_duty_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0033_heavy_duty_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0033_heavy_duty_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some heavy-duty mild steel gloves for domestic protection from prepared mild steel stock.",
		Action = "making some heavy-duty mild steel gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty mild steel gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1620.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1620 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0033_heavy_duty_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty mild steel gloves for domestic protection (@industrialised_ppe_ballistic_0033_heavy_duty_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeHeavyDutyLeatherBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0034HeavyDutyBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some heavy-duty leather boots for domestic protection [craft_industrialised_ppe_ballistic_0034_heavy_duty_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0034_heavy_duty_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty leather boots for domestic protection [craft_industrialised_ppe_ballistic_0034_heavy_duty_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some heavy-duty leather boots for domestic protection from prepared leather stock.",
		Action = "making some heavy-duty leather boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty leather boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1661.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1661 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0034_heavy_duty_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty leather boots for domestic protection (@industrialised_ppe_ballistic_0034_heavy_duty_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyCottonHelmetForDomesticProtectionCraftIndustrialisedPpeBallistic0035HeavyDutyHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty cotton helmet for domestic protection [craft_industrialised_ppe_ballistic_0035_heavy_duty_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0035_heavy_duty_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton helmet for domestic protection [craft_industrialised_ppe_ballistic_0035_heavy_duty_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty cotton helmet for domestic protection from prepared cotton stock.",
		Action = "making heavy-duty cotton helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1702.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1702 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0035_heavy_duty_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton helmet for domestic protection (@industrialised_ppe_ballistic_0035_heavy_duty_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeHeavyDutyWoolGaitersForDomesticProtectionCraftIndustrialisedPpeBallistic0036HeavyDutyGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some heavy-duty wool gaiters for domestic protection [craft_industrialised_ppe_ballistic_0036_heavy_duty_gaiters]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0036_heavy_duty_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some heavy-duty wool gaiters for domestic protection [craft_industrialised_ppe_ballistic_0036_heavy_duty_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some heavy-duty wool gaiters for domestic protection from prepared wool stock.",
		Action = "making some heavy-duty wool gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some heavy-duty wool gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1742.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1742 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0036_heavy_duty_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some heavy-duty wool gaiters for domestic protection (@industrialised_ppe_ballistic_0036_heavy_duty_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyMildSteelMaskForDomesticProtectionCraftIndustrialisedPpeBallistic0037HeavyDutyMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty mild steel mask for domestic protection [craft_industrialised_ppe_ballistic_0037_heavy_duty_mask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0037_heavy_duty_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel mask for domestic protection [craft_industrialised_ppe_ballistic_0037_heavy_duty_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty mild steel mask for domestic protection from prepared mild steel stock.",
		Action = "making heavy-duty mild steel mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1783.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1783 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0037_heavy_duty_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel mask for domestic protection (@industrialised_ppe_ballistic_0037_heavy_duty_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyLeatherVisorForDomesticProtectionCraftIndustrialisedPpeBallistic0038HeavyDutyVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty leather visor for domestic protection [craft_industrialised_ppe_ballistic_0038_heavy_duty_visor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0038_heavy_duty_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather visor for domestic protection [craft_industrialised_ppe_ballistic_0038_heavy_duty_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty leather visor for domestic protection from prepared leather stock.",
		Action = "making heavy-duty leather visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1824.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1824 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0038_heavy_duty_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather visor for domestic protection (@industrialised_ppe_ballistic_0038_heavy_duty_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyCottonVestForDomesticProtectionCraftIndustrialisedPpeBallistic0039HeavyDutyVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty cotton vest for domestic protection [craft_industrialised_ppe_ballistic_0039_heavy_duty_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0039_heavy_duty_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton vest for domestic protection [craft_industrialised_ppe_ballistic_0039_heavy_duty_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty cotton vest for domestic protection from prepared cotton stock.",
		Action = "making heavy-duty cotton vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1865.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1865 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0039_heavy_duty_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton vest for domestic protection (@industrialised_ppe_ballistic_0039_heavy_duty_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeHeavyDutyWoolCoatForDomesticProtectionCraftIndustrialisedPpeBallistic0040HeavyDutyCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make heavy-duty wool coat for domestic protection [craft_industrialised_ppe_ballistic_0040_heavy_duty_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0040_heavy_duty_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty wool coat for domestic protection [craft_industrialised_ppe_ballistic_0040_heavy_duty_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a heavy-duty wool coat for domestic protection from prepared wool stock.",
		Action = "making heavy-duty wool coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress heavy-duty wool coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1905.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1905 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0040_heavy_duty_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty wool coat for domestic protection (@industrialised_ppe_ballistic_0040_heavy_duty_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingMildSteelLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0041FoldingLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0041_folding_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0041_folding_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel liner for domestic protection [craft_industrialised_ppe_ballistic_0041_folding_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding mild steel liner for domestic protection from prepared mild steel stock.",
		Action = "making folding mild steel liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding mild steel liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1946.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1946 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0041_folding_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel liner for domestic protection (@industrialised_ppe_ballistic_0041_folding_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingLeatherApronForDomesticProtectionCraftIndustrialisedPpeBallistic0042FoldingApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding leather apron for domestic protection [craft_industrialised_ppe_ballistic_0042_folding_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0042_folding_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather apron for domestic protection [craft_industrialised_ppe_ballistic_0042_folding_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding leather apron for domestic protection from prepared leather stock.",
		Action = "making folding leather apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding leather apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1987.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1987 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0042_folding_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather apron for domestic protection (@industrialised_ppe_ballistic_0042_folding_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFoldingCottonGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0043FoldingGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some folding cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0043_folding_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0043_folding_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding cotton gloves for domestic protection [craft_industrialised_ppe_ballistic_0043_folding_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some folding cotton gloves for domestic protection from prepared cotton stock.",
		Action = "making some folding cotton gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some folding cotton gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2027.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2027 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0043_folding_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding cotton gloves for domestic protection (@industrialised_ppe_ballistic_0043_folding_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFoldingWoolBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0044FoldingBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some folding wool boots for domestic protection [craft_industrialised_ppe_ballistic_0044_folding_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0044_folding_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding wool boots for domestic protection [craft_industrialised_ppe_ballistic_0044_folding_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some folding wool boots for domestic protection from prepared wool stock.",
		Action = "making some folding wool boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some folding wool boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2068.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2068 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0044_folding_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding wool boots for domestic protection (@industrialised_ppe_ballistic_0044_folding_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingMildSteelHelmetForDomesticProtectionCraftIndustrialisedPpeBallistic0045FoldingHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0045_folding_helmet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0045_folding_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel helmet for domestic protection [craft_industrialised_ppe_ballistic_0045_folding_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding mild steel helmet for domestic protection from prepared mild steel stock.",
		Action = "making folding mild steel helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding mild steel helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2109.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2109 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0045_folding_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel helmet for domestic protection (@industrialised_ppe_ballistic_0045_folding_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFoldingLeatherGaitersForDomesticProtectionCraftIndustrialisedPpeBallistic0046FoldingGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some folding leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0046_folding_gaiters]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0046_folding_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some folding leather gaiters for domestic protection [craft_industrialised_ppe_ballistic_0046_folding_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some folding leather gaiters for domestic protection from prepared leather stock.",
		Action = "making some folding leather gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some folding leather gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2149.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2149 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0046_folding_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some folding leather gaiters for domestic protection (@industrialised_ppe_ballistic_0046_folding_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingCottonMaskForDomesticProtectionCraftIndustrialisedPpeBallistic0047FoldingMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0047_folding_mask]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0047_folding_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding cotton mask for domestic protection [craft_industrialised_ppe_ballistic_0047_folding_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding cotton mask for domestic protection from prepared cotton stock.",
		Action = "making folding cotton mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding cotton mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2190.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2190 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0047_folding_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding cotton mask for domestic protection (@industrialised_ppe_ballistic_0047_folding_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingWoolVisorForDomesticProtectionCraftIndustrialisedPpeBallistic0048FoldingVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding wool visor for domestic protection [craft_industrialised_ppe_ballistic_0048_folding_visor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0048_folding_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding wool visor for domestic protection [craft_industrialised_ppe_ballistic_0048_folding_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding wool visor for domestic protection from prepared wool stock.",
		Action = "making folding wool visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding wool visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2231.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2231 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0048_folding_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding wool visor for domestic protection (@industrialised_ppe_ballistic_0048_folding_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingMildSteelVestForDomesticProtectionCraftIndustrialisedPpeBallistic0049FoldingVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0049_folding_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0049_folding_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel vest for domestic protection [craft_industrialised_ppe_ballistic_0049_folding_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding mild steel vest for domestic protection from prepared mild steel stock.",
		Action = "making folding mild steel vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding mild steel vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2272.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2272 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0049_folding_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel vest for domestic protection (@industrialised_ppe_ballistic_0049_folding_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFoldingLeatherCoatForDomesticProtectionCraftIndustrialisedPpeBallistic0050FoldingCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make folding leather coat for domestic protection [craft_industrialised_ppe_ballistic_0050_folding_coat]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0050_folding_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather coat for domestic protection [craft_industrialised_ppe_ballistic_0050_folding_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a folding leather coat for domestic protection from prepared leather stock.",
		Action = "making folding leather coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress folding leather coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2312.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2312 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0050_folding_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather coat for domestic protection (@industrialised_ppe_ballistic_0050_folding_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePortableCottonLinerForDomesticProtectionCraftIndustrialisedPpeBallistic0051PortableLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make portable cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0051_portable_liner]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0051_portable_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable cotton liner for domestic protection [craft_industrialised_ppe_ballistic_0051_portable_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a portable cotton liner for domestic protection from prepared cotton stock.",
		Action = "making portable cotton liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress portable cotton liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2353.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2353 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0051_portable_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable cotton liner for domestic protection (@industrialised_ppe_ballistic_0051_portable_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakePortableWoolApronForDomesticProtectionCraftIndustrialisedPpeBallistic0052PortableApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make portable wool apron for domestic protection [craft_industrialised_ppe_ballistic_0052_portable_apron]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0052_portable_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable wool apron for domestic protection [craft_industrialised_ppe_ballistic_0052_portable_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a portable wool apron for domestic protection from prepared wool stock.",
		Action = "making portable wool apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress portable wool apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2394.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2394 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0052_portable_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable wool apron for domestic protection (@industrialised_ppe_ballistic_0052_portable_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomePortableMildSteelGlovesForDomesticProtectionCraftIndustrialisedPpeBallistic0053PortableGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some portable mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0053_portable_gloves]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0053_portable_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable mild steel gloves for domestic protection [craft_industrialised_ppe_ballistic_0053_portable_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some portable mild steel gloves for domestic protection from prepared mild steel stock.",
		Action = "making some portable mild steel gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some portable mild steel gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2434.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2434 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0053_portable_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable mild steel gloves for domestic protection (@industrialised_ppe_ballistic_0053_portable_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomePortableLeatherBootsForDomesticProtectionCraftIndustrialisedPpeBallistic0054PortableBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some portable leather boots for domestic protection [craft_industrialised_ppe_ballistic_0054_portable_boots]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_ppe_ballistic_0054_portable_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some portable leather boots for domestic protection [craft_industrialised_ppe_ballistic_0054_portable_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some portable leather boots for domestic protection from prepared leather stock.",
		Action = "making some portable leather boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some portable leather boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2475.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2475 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_ppe_ballistic_0054_portable_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some portable leather boots for domestic protection (@industrialised_ppe_ballistic_0054_portable_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldCottonLinerForDomesticProtectionCraftIndustrialPpeBallistic0001FieldLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make field cotton liner for domestic protection [craft_industrial_ppe_ballistic_0001_field_liner]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton liner for domestic protection [craft_industrial_ppe_ballistic_0001_field_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field cotton liner for domestic protection from prepared cotton stock.",
		Action = "making field cotton liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field cotton liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 318.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("318 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0001_field_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton liner for domestic protection (@industrial_ppe_ballistic_0001_field_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldWoolApronForDomesticProtectionCraftIndustrialPpeBallistic0002FieldApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make field wool apron for domestic protection [craft_industrial_ppe_ballistic_0002_field_apron]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0002_field_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field wool apron for domestic protection [craft_industrial_ppe_ballistic_0002_field_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field wool apron for domestic protection from prepared wool stock.",
		Action = "making field wool apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field wool apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 359.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("359 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0002_field_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field wool apron for domestic protection (@industrial_ppe_ballistic_0002_field_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFieldMildSteelGlovesForDomesticProtectionCraftIndustrialPpeBallistic0003FieldGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some field mild steel gloves for domestic protection [craft_industrial_ppe_ballistic_0003_field_gloves]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0003_field_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field mild steel gloves for domestic protection [craft_industrial_ppe_ballistic_0003_field_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some field mild steel gloves for domestic protection from prepared mild steel stock.",
		Action = "making some field mild steel gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some field mild steel gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 399.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("399 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0003_field_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field mild steel gloves for domestic protection (@industrial_ppe_ballistic_0003_field_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFieldLeatherBootsForDomesticProtectionCraftIndustrialPpeBallistic0004FieldBoots() => new()
	{
		Metadata = new("PPE and ballistic protection/make some field leather boots for domestic protection [craft_industrial_ppe_ballistic_0004_field_boots]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0004_field_boots"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field leather boots for domestic protection [craft_industrial_ppe_ballistic_0004_field_boots]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some field leather boots for domestic protection from prepared leather stock.",
		Action = "making some field leather boots for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some field leather boots for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 440.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("440 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0004_field_boots", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field leather boots for domestic protection (@industrial_ppe_ballistic_0004_field_boots)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldCottonHelmetForDomesticProtectionCraftIndustrialPpeBallistic0005FieldHelmet() => new()
	{
		Metadata = new("PPE and ballistic protection/make field cotton helmet for domestic protection [craft_industrial_ppe_ballistic_0005_field_helmet]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0005_field_helmet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton helmet for domestic protection [craft_industrial_ppe_ballistic_0005_field_helmet]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field cotton helmet for domestic protection from prepared cotton stock.",
		Action = "making field cotton helmet for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field cotton helmet for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 481.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("481 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0005_field_helmet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton helmet for domestic protection (@industrial_ppe_ballistic_0005_field_helmet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeFieldWoolGaitersForDomesticProtectionCraftIndustrialPpeBallistic0006FieldGaiters() => new()
	{
		Metadata = new("PPE and ballistic protection/make some field wool gaiters for domestic protection [craft_industrial_ppe_ballistic_0006_field_gaiters]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0006_field_gaiters"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some field wool gaiters for domestic protection [craft_industrial_ppe_ballistic_0006_field_gaiters]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some field wool gaiters for domestic protection from prepared wool stock.",
		Action = "making some field wool gaiters for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some field wool gaiters for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 521.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("521 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0006_field_gaiters", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some field wool gaiters for domestic protection (@industrial_ppe_ballistic_0006_field_gaiters)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldMildSteelMaskForDomesticProtectionCraftIndustrialPpeBallistic0007FieldMask() => new()
	{
		Metadata = new("PPE and ballistic protection/make field mild steel mask for domestic protection [craft_industrial_ppe_ballistic_0007_field_mask]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0007_field_mask"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field mild steel mask for domestic protection [craft_industrial_ppe_ballistic_0007_field_mask]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field mild steel mask for domestic protection from prepared mild steel stock.",
		Action = "making field mild steel mask for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field mild steel mask for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 562.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("562 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0007_field_mask", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field mild steel mask for domestic protection (@industrial_ppe_ballistic_0007_field_mask)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldLeatherVisorForDomesticProtectionCraftIndustrialPpeBallistic0008FieldVisor() => new()
	{
		Metadata = new("PPE and ballistic protection/make field leather visor for domestic protection [craft_industrial_ppe_ballistic_0008_field_visor]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0008_field_visor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field leather visor for domestic protection [craft_industrial_ppe_ballistic_0008_field_visor]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field leather visor for domestic protection from prepared leather stock.",
		Action = "making field leather visor for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field leather visor for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 603.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("603 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0008_field_visor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field leather visor for domestic protection (@industrial_ppe_ballistic_0008_field_visor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldCottonVestForDomesticProtectionCraftIndustrialPpeBallistic0009FieldVest() => new()
	{
		Metadata = new("PPE and ballistic protection/make field cotton vest for domestic protection [craft_industrial_ppe_ballistic_0009_field_vest]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0009_field_vest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field cotton vest for domestic protection [craft_industrial_ppe_ballistic_0009_field_vest]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field cotton vest for domestic protection from prepared cotton stock.",
		Action = "making field cotton vest for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field cotton vest for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 644.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("644 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0009_field_vest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field cotton vest for domestic protection (@industrial_ppe_ballistic_0009_field_vest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeFieldWoolCoatForDomesticProtectionCraftIndustrialPpeBallistic0010FieldCoat() => new()
	{
		Metadata = new("PPE and ballistic protection/make field wool coat for domestic protection [craft_industrial_ppe_ballistic_0010_field_coat]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0010_field_coat"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make field wool coat for domestic protection [craft_industrial_ppe_ballistic_0010_field_coat]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a field wool coat for domestic protection from prepared wool stock.",
		Action = "making field wool coat for domestic protection",
		ActiveCraftItemSdesc = "an in-progress field wool coat for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 684.0, Material: "wool", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("684 grams of wool", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0010_field_coat", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a field wool coat for domestic protection (@industrial_ppe_ballistic_0010_field_coat)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCountertopMildSteelLinerForDomesticProtectionCraftIndustrialPpeBallistic0011CountertopLiner() => new()
	{
		Metadata = new("PPE and ballistic protection/make countertop mild steel liner for domestic protection [craft_industrial_ppe_ballistic_0011_countertop_liner]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0011_countertop_liner"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop mild steel liner for domestic protection [craft_industrial_ppe_ballistic_0011_countertop_liner]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a countertop mild steel liner for domestic protection from prepared mild steel stock.",
		Action = "making countertop mild steel liner for domestic protection",
		ActiveCraftItemSdesc = "an in-progress countertop mild steel liner for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 725.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("725 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0011_countertop_liner", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop mild steel liner for domestic protection (@industrial_ppe_ballistic_0011_countertop_liner)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeCountertopLeatherApronForDomesticProtectionCraftIndustrialPpeBallistic0012CountertopApron() => new()
	{
		Metadata = new("PPE and ballistic protection/make countertop leather apron for domestic protection [craft_industrial_ppe_ballistic_0012_countertop_apron]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0012_countertop_apron"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make countertop leather apron for domestic protection [craft_industrial_ppe_ballistic_0012_countertop_apron]",
		Category = "PPE and ballistic protection",
		Blurb = "Make a countertop leather apron for domestic protection from prepared leather stock.",
		Action = "making countertop leather apron for domestic protection",
		ActiveCraftItemSdesc = "an in-progress countertop leather apron for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 766.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("766 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0012_countertop_apron", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a countertop leather apron for domestic protection (@industrial_ppe_ballistic_0012_countertop_apron)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PPEAndBallisticProtectionMakeSomeCountertopCottonGlovesForDomesticProtectionCraftIndustrialPpeBallistic0013CountertopGloves() => new()
	{
		Metadata = new("PPE and ballistic protection/make some countertop cotton gloves for domestic protection [craft_industrial_ppe_ballistic_0013_countertop_gloves]", "crafts", ["industrial"], ["item:industrial_ppe_ballistic_0013_countertop_gloves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make some countertop cotton gloves for domestic protection [craft_industrial_ppe_ballistic_0013_countertop_gloves]",
		Category = "PPE and ballistic protection",
		Blurb = "Make some countertop cotton gloves for domestic protection from prepared cotton stock.",
		Action = "making some countertop cotton gloves for domestic protection",
		ActiveCraftItemSdesc = "an in-progress some countertop cotton gloves for domestic protection craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 806.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("806 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_ppe_ballistic_0013_countertop_gloves", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x some countertop cotton gloves for domestic protection (@industrial_ppe_ballistic_0013_countertop_gloves)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0001PlainScrap() => new()
	{
		Metadata = new("Household furniture and storage/make plain mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0001_plain_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0001_plain_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making plain mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel scrap for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 429.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("429 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0002PlainChair() => new()
	{
		Metadata = new("Household furniture and storage/make plain cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0002_plain_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0002_plain_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0002_plain_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making plain cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain cast iron chair for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 470.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("470 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0002_plain_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0002_plain_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0003PlainStool() => new()
	{
		Metadata = new("Household furniture and storage/make plain oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0003_plain_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0003_plain_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0003_plain_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain oak stool for domestic furnishing from prepared oak stock.",
		Action = "making plain oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain oak stool for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 510.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("510 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0003_plain_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak stool for domestic furnishing (@industrialised_household_furniture_storage_0003_plain_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0004PlainTable() => new()
	{
		Metadata = new("Household furniture and storage/make plain mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0004_plain_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0004_plain_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0004_plain_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making plain mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel table for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 551.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("551 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0004_plain_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0004_plain_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0005PlainCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make plain cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0005_plain_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0005_plain_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0005_plain_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making plain cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain cast iron cabinet for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 592.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("592 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0005_plain_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0005_plain_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0006PlainShelf() => new()
	{
		Metadata = new("Household furniture and storage/make plain oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0006_plain_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0006_plain_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0006_plain_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making plain oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain oak shelf for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 632.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("632 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0006_plain_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0006_plain_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0007PlainBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make plain mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0007_plain_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0007_plain_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0007_plain_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making plain mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel bedstead for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 673.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("673 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0007_plain_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0007_plain_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0008PlainWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make plain cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0008_plain_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0008_plain_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0008_plain_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making plain cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain cast iron washstand for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 714.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("714 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0008_plain_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0008_plain_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0009PlainScreen() => new()
	{
		Metadata = new("Household furniture and storage/make plain oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0009_plain_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0009_plain_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0009_plain_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain oak screen for domestic furnishing from prepared oak stock.",
		Action = "making plain oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain oak screen for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 755.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("755 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0009_plain_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak screen for domestic furnishing (@industrialised_household_furniture_storage_0009_plain_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainMildSteelRackForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0010PlainRack() => new()
	{
		Metadata = new("Household furniture and storage/make plain mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0010_plain_rack]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0010_plain_rack"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain mild steel rack for domestic furnishing [craft_industrialised_household_furniture_storage_0010_plain_rack]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain mild steel rack for domestic furnishing from prepared mild steel stock.",
		Action = "making plain mild steel rack for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain mild steel rack for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 795.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("795 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0010_plain_rack", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain mild steel rack for domestic furnishing (@industrialised_household_furniture_storage_0010_plain_rack)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainCastIronChestForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0011PlainChest() => new()
	{
		Metadata = new("Household furniture and storage/make plain cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0011_plain_chest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0011_plain_chest"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain cast iron chest for domestic furnishing [craft_industrialised_household_furniture_storage_0011_plain_chest]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain cast iron chest for domestic furnishing from prepared cast iron stock.",
		Action = "making plain cast iron chest for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain cast iron chest for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 836.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("836 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0011_plain_chest", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain cast iron chest for domestic furnishing (@industrialised_household_furniture_storage_0011_plain_chest)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakePlainOakBenchForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0012PlainBench() => new()
	{
		Metadata = new("Household furniture and storage/make plain oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0012_plain_bench]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0012_plain_bench"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain oak bench for domestic furnishing [craft_industrialised_household_furniture_storage_0012_plain_bench]",
		Category = "Household furniture and storage",
		Blurb = "Make a plain oak bench for domestic furnishing from prepared oak stock.",
		Action = "making plain oak bench for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress plain oak bench for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 877.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("877 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0012_plain_bench", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain oak bench for domestic furnishing (@industrialised_household_furniture_storage_0012_plain_bench)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedMildSteelScrapForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0013ReinforcedScrap() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0013_reinforced_scrap]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0013_reinforced_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel scrap for domestic furnishing [craft_industrialised_household_furniture_storage_0013_reinforced_scrap]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced mild steel scrap for domestic furnishing from prepared mild steel stock.",
		Action = "making reinforced mild steel scrap for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel scrap for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 917.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("917 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0013_reinforced_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel scrap for domestic furnishing (@industrialised_household_furniture_storage_0013_reinforced_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedCastIronChairForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0014ReinforcedChair() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0014_reinforced_chair]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0014_reinforced_chair"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron chair for domestic furnishing [craft_industrialised_household_furniture_storage_0014_reinforced_chair]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced cast iron chair for domestic furnishing from prepared cast iron stock.",
		Action = "making reinforced cast iron chair for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron chair for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 958.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("958 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0014_reinforced_chair", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron chair for domestic furnishing (@industrialised_household_furniture_storage_0014_reinforced_chair)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedOakStoolForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0015ReinforcedStool() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0015_reinforced_stool]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0015_reinforced_stool"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak stool for domestic furnishing [craft_industrialised_household_furniture_storage_0015_reinforced_stool]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced oak stool for domestic furnishing from prepared oak stock.",
		Action = "making reinforced oak stool for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak stool for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 999.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("999 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0015_reinforced_stool", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak stool for domestic furnishing (@industrialised_household_furniture_storage_0015_reinforced_stool)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedMildSteelTableForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0016ReinforcedTable() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0016_reinforced_table]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0016_reinforced_table"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel table for domestic furnishing [craft_industrialised_household_furniture_storage_0016_reinforced_table]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced mild steel table for domestic furnishing from prepared mild steel stock.",
		Action = "making reinforced mild steel table for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel table for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1040.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1040 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0016_reinforced_table", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel table for domestic furnishing (@industrialised_household_furniture_storage_0016_reinforced_table)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedCastIronCabinetForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0017ReinforcedCabinet() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0017_reinforced_cabinet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0017_reinforced_cabinet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron cabinet for domestic furnishing [craft_industrialised_household_furniture_storage_0017_reinforced_cabinet]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced cast iron cabinet for domestic furnishing from prepared cast iron stock.",
		Action = "making reinforced cast iron cabinet for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron cabinet for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1080.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1080 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0017_reinforced_cabinet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron cabinet for domestic furnishing (@industrialised_household_furniture_storage_0017_reinforced_cabinet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedOakShelfForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0018ReinforcedShelf() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0018_reinforced_shelf]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0018_reinforced_shelf"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak shelf for domestic furnishing [craft_industrialised_household_furniture_storage_0018_reinforced_shelf]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced oak shelf for domestic furnishing from prepared oak stock.",
		Action = "making reinforced oak shelf for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak shelf for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1121.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1121 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0018_reinforced_shelf", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak shelf for domestic furnishing (@industrialised_household_furniture_storage_0018_reinforced_shelf)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedMildSteelBedsteadForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0019ReinforcedBedstead() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0019_reinforced_bedstead]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0019_reinforced_bedstead"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced mild steel bedstead for domestic furnishing [craft_industrialised_household_furniture_storage_0019_reinforced_bedstead]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced mild steel bedstead for domestic furnishing from prepared mild steel stock.",
		Action = "making reinforced mild steel bedstead for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced mild steel bedstead for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1162.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1162 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0019_reinforced_bedstead", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced mild steel bedstead for domestic furnishing (@industrialised_household_furniture_storage_0019_reinforced_bedstead)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedCastIronWashstandForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0020ReinforcedWashstand() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0020_reinforced_washstand]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0020_reinforced_washstand"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced cast iron washstand for domestic furnishing [craft_industrialised_household_furniture_storage_0020_reinforced_washstand]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced cast iron washstand for domestic furnishing from prepared cast iron stock.",
		Action = "making reinforced cast iron washstand for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced cast iron washstand for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1202.0, Material: "cast iron", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1202 grams of cast iron", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0020_reinforced_washstand", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced cast iron washstand for domestic furnishing (@industrialised_household_furniture_storage_0020_reinforced_washstand)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition HouseholdFurnitureAndStorageMakeReinforcedOakScreenForDomesticFurnishingCraftIndustrialisedHouseholdFurnitureStorage0021ReinforcedScreen() => new()
	{
		Metadata = new("Household furniture and storage/make reinforced oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0021_reinforced_screen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_household_furniture_storage_0021_reinforced_screen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make reinforced oak screen for domestic furnishing [craft_industrialised_household_furniture_storage_0021_reinforced_screen]",
		Category = "Household furniture and storage",
		Blurb = "Make a reinforced oak screen for domestic furnishing from prepared oak stock.",
		Action = "making reinforced oak screen for domestic furnishing",
		ActiveCraftItemSdesc = "an in-progress reinforced oak screen for domestic furnishing craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
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
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 1243.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1243 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_household_furniture_storage_0021_reinforced_screen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a reinforced oak screen for domestic furnishing (@industrialised_household_furniture_storage_0021_reinforced_screen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
