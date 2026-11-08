#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes23Stock
{
	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyPaperManualForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0036HeavyDutyManual() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty paper manual for domestic public display [craft_industrialised_printed_media_signage_0036_heavy_duty_manual]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0036_heavy_duty_manual"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper manual for domestic public display [craft_industrialised_printed_media_signage_0036_heavy_duty_manual]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty paper manual for domestic public display from prepared paper stock.",
		Action = "making heavy-duty paper manual for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper manual for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4187.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4187 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0036_heavy_duty_manual", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper manual for domestic public display (@industrialised_printed_media_signage_0036_heavy_duty_manual)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyCottonPamphletForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0037HeavyDutyPamphlet() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty cotton pamphlet for domestic public display [craft_industrialised_printed_media_signage_0037_heavy_duty_pamphlet]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0037_heavy_duty_pamphlet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton pamphlet for domestic public display [craft_industrialised_printed_media_signage_0037_heavy_duty_pamphlet]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty cotton pamphlet for domestic public display from prepared cotton stock.",
		Action = "making heavy-duty cotton pamphlet for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton pamphlet for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4227.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4227 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0037_heavy_duty_pamphlet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton pamphlet for domestic public display (@industrialised_printed_media_signage_0037_heavy_duty_pamphlet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyOakTicketForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0038HeavyDutyTicket() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty oak ticket for domestic public display [craft_industrialised_printed_media_signage_0038_heavy_duty_ticket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0038_heavy_duty_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak ticket for domestic public display [craft_industrialised_printed_media_signage_0038_heavy_duty_ticket]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty oak ticket for domestic public display from prepared oak stock.",
		Action = "making heavy-duty oak ticket for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak ticket for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4268.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4268 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0038_heavy_duty_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak ticket for domestic public display (@industrialised_printed_media_signage_0038_heavy_duty_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyPaperLabelForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0039HeavyDutyLabel() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty paper label for domestic public display [craft_industrialised_printed_media_signage_0039_heavy_duty_label]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0039_heavy_duty_label"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper label for domestic public display [craft_industrialised_printed_media_signage_0039_heavy_duty_label]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty paper label for domestic public display from prepared paper stock.",
		Action = "making heavy-duty paper label for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper label for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4309.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4309 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0039_heavy_duty_label", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper label for domestic public display (@industrialised_printed_media_signage_0039_heavy_duty_label)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeHeavyDutyCottonChartForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0040HeavyDutyChart() => new()
	{
		Metadata = new("Printed media and signage/make heavy-duty cotton chart for domestic public display [craft_industrialised_printed_media_signage_0040_heavy_duty_chart]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0040_heavy_duty_chart"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton chart for domestic public display [craft_industrialised_printed_media_signage_0040_heavy_duty_chart]",
		Category = "Printed media and signage",
		Blurb = "Make a heavy-duty cotton chart for domestic public display from prepared cotton stock.",
		Action = "making heavy-duty cotton chart for domestic public display",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton chart for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4349.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4349 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0040_heavy_duty_chart", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton chart for domestic public display (@industrialised_printed_media_signage_0040_heavy_duty_chart)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeFoldingOakWastepaperForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0041FoldingWastepaper() => new()
	{
		Metadata = new("Printed media and signage/make folding oak wastepaper for domestic public display [craft_industrialised_printed_media_signage_0041_folding_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0041_folding_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak wastepaper for domestic public display [craft_industrialised_printed_media_signage_0041_folding_wastepaper]",
		Category = "Printed media and signage",
		Blurb = "Make a folding oak wastepaper for domestic public display from prepared oak stock.",
		Action = "making folding oak wastepaper for domestic public display",
		ActiveCraftItemSdesc = "an in-progress folding oak wastepaper for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4390.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4390 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0041_folding_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak wastepaper for domestic public display (@industrialised_printed_media_signage_0041_folding_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeFoldingPaperPosterForDomesticPublicDisplayCraftIndustrialisedPrintedMediaSignage0042FoldingPoster() => new()
	{
		Metadata = new("Printed media and signage/make folding paper poster for domestic public display [craft_industrialised_printed_media_signage_0042_folding_poster]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_printed_media_signage_0042_folding_poster"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper poster for domestic public display [craft_industrialised_printed_media_signage_0042_folding_poster]",
		Category = "Printed media and signage",
		Blurb = "Make a folding paper poster for domestic public display from prepared paper stock.",
		Action = "making folding paper poster for domestic public display",
		ActiveCraftItemSdesc = "an in-progress folding paper poster for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4431.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4431 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_printed_media_signage_0042_folding_poster", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper poster for domestic public display (@industrialised_printed_media_signage_0042_folding_poster)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeEconomyOakManualForDomesticPublicDisplayCraftIndustrialPrintedMediaSignage0001EconomyManual() => new()
	{
		Metadata = new("Printed media and signage/make economy oak manual for domestic public display [craft_industrial_printed_media_signage_0001_economy_manual]", "crafts", ["industrial"], ["item:industrial_printed_media_signage_0001_economy_manual"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak manual for domestic public display [craft_industrial_printed_media_signage_0001_economy_manual]",
		Category = "Printed media and signage",
		Blurb = "Make an economy oak manual for domestic public display from prepared oak stock.",
		Action = "making economy oak manual for domestic public display",
		ActiveCraftItemSdesc = "an in-progress economy oak manual for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2762.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2762 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_printed_media_signage_0001_economy_manual", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak manual for domestic public display (@industrial_printed_media_signage_0001_economy_manual)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeEconomyPaperPamphletForDomesticPublicDisplayCraftIndustrialPrintedMediaSignage0002EconomyPamphlet() => new()
	{
		Metadata = new("Printed media and signage/make economy paper pamphlet for domestic public display [craft_industrial_printed_media_signage_0002_economy_pamphlet]", "crafts", ["industrial"], ["item:industrial_printed_media_signage_0002_economy_pamphlet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy paper pamphlet for domestic public display [craft_industrial_printed_media_signage_0002_economy_pamphlet]",
		Category = "Printed media and signage",
		Blurb = "Make an economy paper pamphlet for domestic public display from prepared paper stock.",
		Action = "making economy paper pamphlet for domestic public display",
		ActiveCraftItemSdesc = "an in-progress economy paper pamphlet for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2803.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2803 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_printed_media_signage_0002_economy_pamphlet", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy paper pamphlet for domestic public display (@industrial_printed_media_signage_0002_economy_pamphlet)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeEconomyCottonTicketForDomesticPublicDisplayCraftIndustrialPrintedMediaSignage0003EconomyTicket() => new()
	{
		Metadata = new("Printed media and signage/make economy cotton ticket for domestic public display [craft_industrial_printed_media_signage_0003_economy_ticket]", "crafts", ["industrial"], ["item:industrial_printed_media_signage_0003_economy_ticket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy cotton ticket for domestic public display [craft_industrial_printed_media_signage_0003_economy_ticket]",
		Category = "Printed media and signage",
		Blurb = "Make an economy cotton ticket for domestic public display from prepared cotton stock.",
		Action = "making economy cotton ticket for domestic public display",
		ActiveCraftItemSdesc = "an in-progress economy cotton ticket for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2844.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2844 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_printed_media_signage_0003_economy_ticket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy cotton ticket for domestic public display (@industrial_printed_media_signage_0003_economy_ticket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition PrintedMediaAndSignageMakeEconomyOakLabelForDomesticPublicDisplayCraftIndustrialPrintedMediaSignage0004EconomyLabel() => new()
	{
		Metadata = new("Printed media and signage/make economy oak label for domestic public display [craft_industrial_printed_media_signage_0004_economy_label]", "crafts", ["industrial"], ["item:industrial_printed_media_signage_0004_economy_label"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make economy oak label for domestic public display [craft_industrial_printed_media_signage_0004_economy_label]",
		Category = "Printed media and signage",
		Blurb = "Make an economy oak label for domestic public display from prepared oak stock.",
		Action = "making economy oak label for domestic public display",
		ActiveCraftItemSdesc = "an in-progress economy oak label for domestic public display craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2884.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2884 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_printed_media_signage_0004_economy_label", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an economy oak label for domestic public display (@industrial_printed_media_signage_0004_economy_label)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
