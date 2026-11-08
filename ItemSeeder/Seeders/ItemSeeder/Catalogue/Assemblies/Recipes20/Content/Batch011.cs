#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes20Stock
{
	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyOakRazorForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0034HeavyDutyRazor() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0034_heavy_duty_razor]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0034_heavy_duty_razor"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak razor for domestic hygiene [craft_industrialised_cleaning_personal_care_0034_heavy_duty_razor]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty oak razor for domestic hygiene from prepared oak stock.",
		Action = "making heavy-duty oak razor for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak razor for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2105.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2105 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0034_heavy_duty_razor", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak razor for domestic hygiene (@industrialised_cleaning_personal_care_0034_heavy_duty_razor)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyCottonSpongeForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0035HeavyDutySponge() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0035_heavy_duty_sponge]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0035_heavy_duty_sponge"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton sponge for domestic hygiene [craft_industrialised_cleaning_personal_care_0035_heavy_duty_sponge]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty cotton sponge for domestic hygiene from prepared cotton stock.",
		Action = "making heavy-duty cotton sponge for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton sponge for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2146.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2146 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0035_heavy_duty_sponge", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton sponge for domestic hygiene (@industrialised_cleaning_personal_care_0035_heavy_duty_sponge)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyMildSteelClothForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0036HeavyDutyCloth() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0036_heavy_duty_cloth]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0036_heavy_duty_cloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel cloth for domestic hygiene [craft_industrialised_cleaning_personal_care_0036_heavy_duty_cloth]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty mild steel cloth for domestic hygiene from prepared mild steel stock.",
		Action = "making heavy-duty mild steel cloth for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel cloth for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2187.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2187 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0036_heavy_duty_cloth", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel cloth for domestic hygiene (@industrialised_cleaning_personal_care_0036_heavy_duty_cloth)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyCeramicBroomForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0037HeavyDutyBroom() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0037_heavy_duty_broom]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0037_heavy_duty_broom"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty ceramic broom for domestic hygiene [craft_industrialised_cleaning_personal_care_0037_heavy_duty_broom]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty ceramic broom for domestic hygiene from prepared ceramic stock.",
		Action = "making heavy-duty ceramic broom for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty ceramic broom for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2228.0, Material: "ceramic", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2228 grams of ceramic", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0037_heavy_duty_broom", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty ceramic broom for domestic hygiene (@industrialised_cleaning_personal_care_0037_heavy_duty_broom)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyOakMopForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0038HeavyDutyMop() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0038_heavy_duty_mop]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0038_heavy_duty_mop"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak mop for domestic hygiene [craft_industrialised_cleaning_personal_care_0038_heavy_duty_mop]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty oak mop for domestic hygiene from prepared oak stock.",
		Action = "making heavy-duty oak mop for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak mop for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2268.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2268 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0038_heavy_duty_mop", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak mop for domestic hygiene (@industrialised_cleaning_personal_care_0038_heavy_duty_mop)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyCottonBucketForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0039HeavyDutyBucket() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0039_heavy_duty_bucket]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0039_heavy_duty_bucket"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty cotton bucket for domestic hygiene [craft_industrialised_cleaning_personal_care_0039_heavy_duty_bucket]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty cotton bucket for domestic hygiene from prepared cotton stock.",
		Action = "making heavy-duty cotton bucket for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty cotton bucket for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2309.0, Material: "cotton", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2309 grams of cotton", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0039_heavy_duty_bucket", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty cotton bucket for domestic hygiene (@industrialised_cleaning_personal_care_0039_heavy_duty_bucket)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition CleaningAndPersonalCareMakeHeavyDutyMildSteelSoapdishForDomesticHygieneCraftIndustrialisedCleaningPersonalCare0040HeavyDutySoapdish() => new()
	{
		Metadata = new("Cleaning and personal care/make heavy-duty mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0040_heavy_duty_soapdish]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0040_heavy_duty_soapdish"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel soapdish for domestic hygiene [craft_industrialised_cleaning_personal_care_0040_heavy_duty_soapdish]",
		Category = "Cleaning and personal care",
		Blurb = "Make a heavy-duty mild steel soapdish for domestic hygiene from prepared mild steel stock.",
		Action = "making heavy-duty mild steel soapdish for domestic hygiene",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel soapdish for domestic hygiene craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2350.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2350 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_cleaning_personal_care_0040_heavy_duty_soapdish", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel soapdish for domestic hygiene (@industrialised_cleaning_personal_care_0040_heavy_duty_soapdish)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
