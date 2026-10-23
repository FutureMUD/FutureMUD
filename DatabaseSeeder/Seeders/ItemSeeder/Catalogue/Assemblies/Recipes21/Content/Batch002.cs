#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0035CompactSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0035_compact_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0035_compact_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0035_compact_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making compact mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact mild steel satchel for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2257.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2257 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0035_compact_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0035_compact_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeCompactLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0036CompactRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make compact leather register for domestic administration [craft_industrialised_office_school_mail_payment_0036_compact_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0036_compact_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make compact leather register for domestic administration [craft_industrialised_office_school_mail_payment_0036_compact_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a compact leather register for domestic administration from prepared leather stock.",
		Action = "making compact leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress compact leather register for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2298.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2298 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0036_compact_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a compact leather register for domestic administration (@industrialised_office_school_mail_payment_0036_compact_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0037HeavyDutyWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0037_heavy_duty_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0037_heavy_duty_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0037_heavy_duty_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making heavy-duty paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper wastepaper for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2339.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2339 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0037_heavy_duty_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0037_heavy_duty_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0038HeavyDutyLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0038_heavy_duty_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0038_heavy_duty_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0038_heavy_duty_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty oak ledger for domestic administration from prepared oak stock.",
		Action = "making heavy-duty oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak ledger for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2379.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2379 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0038_heavy_duty_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak ledger for domestic administration (@industrialised_office_school_mail_payment_0038_heavy_duty_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0039HeavyDutyNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0039_heavy_duty_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0039_heavy_duty_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0039_heavy_duty_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making heavy-duty mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel notebook for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2420.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2420 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0039_heavy_duty_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0039_heavy_duty_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0040HeavyDutyEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0040_heavy_duty_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0040_heavy_duty_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0040_heavy_duty_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty leather envelope for domestic administration from prepared leather stock.",
		Action = "making heavy-duty leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather envelope for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2461.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2461 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0040_heavy_duty_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather envelope for domestic administration (@industrialised_office_school_mail_payment_0040_heavy_duty_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0041HeavyDutyFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0041_heavy_duty_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0041_heavy_duty_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0041_heavy_duty_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty paper folder for domestic administration from prepared paper stock.",
		Action = "making heavy-duty paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2501.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2501 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0041_heavy_duty_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper folder for domestic administration (@industrialised_office_school_mail_payment_0041_heavy_duty_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0042HeavyDutyStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0042_heavy_duty_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0042_heavy_duty_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0042_heavy_duty_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty oak stamp for domestic administration from prepared oak stock.",
		Action = "making heavy-duty oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak stamp for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2542.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2542 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0042_heavy_duty_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak stamp for domestic administration (@industrialised_office_school_mail_payment_0042_heavy_duty_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0043HeavyDutyTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0043_heavy_duty_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0043_heavy_duty_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0043_heavy_duty_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making heavy-duty mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel tray for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2583.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2583 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0043_heavy_duty_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0043_heavy_duty_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0044HeavyDutyRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0044_heavy_duty_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0044_heavy_duty_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0044_heavy_duty_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty leather ruler for domestic administration from prepared leather stock.",
		Action = "making heavy-duty leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather ruler for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2624.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2624 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0044_heavy_duty_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather ruler for domestic administration (@industrialised_office_school_mail_payment_0044_heavy_duty_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyPaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0045HeavyDutyPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0045_heavy_duty_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0045_heavy_duty_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0045_heavy_duty_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty paper pen for domestic administration from prepared paper stock.",
		Action = "making heavy-duty paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty paper pen for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2664.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2664 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0045_heavy_duty_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty paper pen for domestic administration (@industrialised_office_school_mail_payment_0045_heavy_duty_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0046HeavyDutyPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0046_heavy_duty_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0046_heavy_duty_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0046_heavy_duty_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty oak pencil for domestic administration from prepared oak stock.",
		Action = "making heavy-duty oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak pencil for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2705.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2705 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0046_heavy_duty_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak pencil for domestic administration (@industrialised_office_school_mail_payment_0046_heavy_duty_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0047HeavyDutySatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0047_heavy_duty_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0047_heavy_duty_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0047_heavy_duty_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making heavy-duty mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty mild steel satchel for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2746.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2746 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0047_heavy_duty_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0047_heavy_duty_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeHeavyDutyLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0048HeavyDutyRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make heavy-duty leather register for domestic administration [craft_industrialised_office_school_mail_payment_0048_heavy_duty_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0048_heavy_duty_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty leather register for domestic administration [craft_industrialised_office_school_mail_payment_0048_heavy_duty_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a heavy-duty leather register for domestic administration from prepared leather stock.",
		Action = "making heavy-duty leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress heavy-duty leather register for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2786.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2786 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0048_heavy_duty_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty leather register for domestic administration (@industrialised_office_school_mail_payment_0048_heavy_duty_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0049FoldingWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0049_folding_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0049_folding_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0049_folding_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making folding paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding paper wastepaper for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2827.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2827 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0049_folding_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0049_folding_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0050FoldingLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0050_folding_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0050_folding_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0050_folding_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding oak ledger for domestic administration from prepared oak stock.",
		Action = "making folding oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding oak ledger for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2868.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2868 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0050_folding_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak ledger for domestic administration (@industrialised_office_school_mail_payment_0050_folding_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0051FoldingNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0051_folding_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0051_folding_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0051_folding_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making folding mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding mild steel notebook for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2908.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2908 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0051_folding_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0051_folding_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0052FoldingEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0052_folding_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0052_folding_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0052_folding_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding leather envelope for domestic administration from prepared leather stock.",
		Action = "making folding leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding leather envelope for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2949.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2949 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0052_folding_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather envelope for domestic administration (@industrialised_office_school_mail_payment_0052_folding_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0053FoldingFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0053_folding_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0053_folding_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0053_folding_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding paper folder for domestic administration from prepared paper stock.",
		Action = "making folding paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 2990.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2990 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0053_folding_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper folder for domestic administration (@industrialised_office_school_mail_payment_0053_folding_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0054FoldingStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0054_folding_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0054_folding_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0054_folding_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding oak stamp for domestic administration from prepared oak stock.",
		Action = "making folding oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding oak stamp for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3031.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3031 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0054_folding_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak stamp for domestic administration (@industrialised_office_school_mail_payment_0054_folding_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0055FoldingTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0055_folding_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0055_folding_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0055_folding_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making folding mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding mild steel tray for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3071.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3071 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0055_folding_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0055_folding_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0056FoldingRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0056_folding_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0056_folding_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0056_folding_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding leather ruler for domestic administration from prepared leather stock.",
		Action = "making folding leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding leather ruler for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3112.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3112 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0056_folding_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather ruler for domestic administration (@industrialised_office_school_mail_payment_0056_folding_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingPaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0057FoldingPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0057_folding_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0057_folding_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0057_folding_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding paper pen for domestic administration from prepared paper stock.",
		Action = "making folding paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding paper pen for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3153.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3153 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0057_folding_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding paper pen for domestic administration (@industrialised_office_school_mail_payment_0057_folding_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0058FoldingPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0058_folding_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0058_folding_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0058_folding_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding oak pencil for domestic administration from prepared oak stock.",
		Action = "making folding oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding oak pencil for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3193.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3193 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0058_folding_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding oak pencil for domestic administration (@industrialised_office_school_mail_payment_0058_folding_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0059FoldingSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0059_folding_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0059_folding_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0059_folding_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making folding mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding mild steel satchel for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3234.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3234 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0059_folding_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0059_folding_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeFoldingLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0060FoldingRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make folding leather register for domestic administration [craft_industrialised_office_school_mail_payment_0060_folding_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0060_folding_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make folding leather register for domestic administration [craft_industrialised_office_school_mail_payment_0060_folding_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a folding leather register for domestic administration from prepared leather stock.",
		Action = "making folding leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress folding leather register for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3275.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3275 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0060_folding_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a folding leather register for domestic administration (@industrialised_office_school_mail_payment_0060_folding_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortablePaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0061PortableWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0061_portable_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0061_portable_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0061_portable_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making portable paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable paper wastepaper for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3315.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3315 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0061_portable_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0061_portable_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0062PortableLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0062_portable_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0062_portable_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0062_portable_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable oak ledger for domestic administration from prepared oak stock.",
		Action = "making portable oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable oak ledger for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3356.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3356 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0062_portable_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak ledger for domestic administration (@industrialised_office_school_mail_payment_0062_portable_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0063PortableNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0063_portable_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0063_portable_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0063_portable_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making portable mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable mild steel notebook for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3397.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3397 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0063_portable_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0063_portable_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0064PortableEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0064_portable_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0064_portable_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0064_portable_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable leather envelope for domestic administration from prepared leather stock.",
		Action = "making portable leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable leather envelope for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3438.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3438 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0064_portable_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather envelope for domestic administration (@industrialised_office_school_mail_payment_0064_portable_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortablePaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0065PortableFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0065_portable_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0065_portable_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0065_portable_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable paper folder for domestic administration from prepared paper stock.",
		Action = "making portable paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3478.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3478 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0065_portable_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper folder for domestic administration (@industrialised_office_school_mail_payment_0065_portable_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0066PortableStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0066_portable_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0066_portable_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0066_portable_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable oak stamp for domestic administration from prepared oak stock.",
		Action = "making portable oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable oak stamp for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3519.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3519 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0066_portable_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak stamp for domestic administration (@industrialised_office_school_mail_payment_0066_portable_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0067PortableTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0067_portable_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0067_portable_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0067_portable_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making portable mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable mild steel tray for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3560.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3560 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0067_portable_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0067_portable_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0068PortableRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0068_portable_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0068_portable_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0068_portable_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable leather ruler for domestic administration from prepared leather stock.",
		Action = "making portable leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable leather ruler for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3600.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3600 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0068_portable_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather ruler for domestic administration (@industrialised_office_school_mail_payment_0068_portable_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortablePaperPenForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0069PortablePen() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0069_portable_pen]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0069_portable_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable paper pen for domestic administration [craft_industrialised_office_school_mail_payment_0069_portable_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable paper pen for domestic administration from prepared paper stock.",
		Action = "making portable paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable paper pen for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3641.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3641 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0069_portable_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable paper pen for domestic administration (@industrialised_office_school_mail_payment_0069_portable_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableOakPencilForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0070PortablePencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0070_portable_pencil]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0070_portable_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable oak pencil for domestic administration [craft_industrialised_office_school_mail_payment_0070_portable_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable oak pencil for domestic administration from prepared oak stock.",
		Action = "making portable oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable oak pencil for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3682.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3682 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0070_portable_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable oak pencil for domestic administration (@industrialised_office_school_mail_payment_0070_portable_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableMildSteelSatchelForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0071PortableSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0071_portable_satchel]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0071_portable_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable mild steel satchel for domestic administration [craft_industrialised_office_school_mail_payment_0071_portable_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making portable mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable mild steel satchel for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3722.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3722 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0071_portable_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable mild steel satchel for domestic administration (@industrialised_office_school_mail_payment_0071_portable_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakePortableLeatherRegisterForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0072PortableRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make portable leather register for domestic administration [craft_industrialised_office_school_mail_payment_0072_portable_register]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0072_portable_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make portable leather register for domestic administration [craft_industrialised_office_school_mail_payment_0072_portable_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a portable leather register for domestic administration from prepared leather stock.",
		Action = "making portable leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress portable leather register for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3763.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3763 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0072_portable_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a portable leather register for domestic administration (@industrialised_office_school_mail_payment_0072_portable_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopPaperWastepaperForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0073WorkshopWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0073_workshop_wastepaper]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0073_workshop_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper wastepaper for domestic administration [craft_industrialised_office_school_mail_payment_0073_workshop_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making workshop paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop paper wastepaper for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3804.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3804 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0073_workshop_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper wastepaper for domestic administration (@industrialised_office_school_mail_payment_0073_workshop_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopOakLedgerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0074WorkshopLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0074_workshop_ledger]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0074_workshop_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak ledger for domestic administration [craft_industrialised_office_school_mail_payment_0074_workshop_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop oak ledger for domestic administration from prepared oak stock.",
		Action = "making workshop oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop oak ledger for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3845.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3845 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0074_workshop_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak ledger for domestic administration (@industrialised_office_school_mail_payment_0074_workshop_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopMildSteelNotebookForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0075WorkshopNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0075_workshop_notebook]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0075_workshop_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel notebook for domestic administration [craft_industrialised_office_school_mail_payment_0075_workshop_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making workshop mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel notebook for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3885.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3885 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0075_workshop_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel notebook for domestic administration (@industrialised_office_school_mail_payment_0075_workshop_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopLeatherEnvelopeForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0076WorkshopEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0076_workshop_envelope]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0076_workshop_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather envelope for domestic administration [craft_industrialised_office_school_mail_payment_0076_workshop_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop leather envelope for domestic administration from prepared leather stock.",
		Action = "making workshop leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop leather envelope for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3926.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3926 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0076_workshop_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather envelope for domestic administration (@industrialised_office_school_mail_payment_0076_workshop_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopPaperFolderForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0077WorkshopFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0077_workshop_folder]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0077_workshop_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop paper folder for domestic administration [craft_industrialised_office_school_mail_payment_0077_workshop_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop paper folder for domestic administration from prepared paper stock.",
		Action = "making workshop paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 3967.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("3967 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0077_workshop_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop paper folder for domestic administration (@industrialised_office_school_mail_payment_0077_workshop_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopOakStampForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0078WorkshopStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0078_workshop_stamp]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0078_workshop_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop oak stamp for domestic administration [craft_industrialised_office_school_mail_payment_0078_workshop_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop oak stamp for domestic administration from prepared oak stock.",
		Action = "making workshop oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop oak stamp for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4007.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4007 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0078_workshop_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop oak stamp for domestic administration (@industrialised_office_school_mail_payment_0078_workshop_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopMildSteelTrayForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0079WorkshopTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0079_workshop_tray]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0079_workshop_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop mild steel tray for domestic administration [craft_industrialised_office_school_mail_payment_0079_workshop_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making workshop mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop mild steel tray for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4048.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4048 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0079_workshop_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop mild steel tray for domestic administration (@industrialised_office_school_mail_payment_0079_workshop_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeWorkshopLeatherRulerForDomesticAdministrationCraftIndustrialisedOfficeSchoolMailPayment0080WorkshopRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make workshop leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0080_workshop_ruler]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_office_school_mail_payment_0080_workshop_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make workshop leather ruler for domestic administration [craft_industrialised_office_school_mail_payment_0080_workshop_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a workshop leather ruler for domestic administration from prepared leather stock.",
		Action = "making workshop leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress workshop leather ruler for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 4089.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("4089 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_office_school_mail_payment_0080_workshop_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a workshop leather ruler for domestic administration (@industrialised_office_school_mail_payment_0080_workshop_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelPaperFolderForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0001TravelFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel paper folder for domestic administration [craft_industrial_office_school_mail_payment_0001_travel_folder]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel paper folder for domestic administration [craft_industrial_office_school_mail_payment_0001_travel_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel paper folder for domestic administration from prepared paper stock.",
		Action = "making travel paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 873.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("873 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0001_travel_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel paper folder for domestic administration (@industrial_office_school_mail_payment_0001_travel_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelOakStampForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0002TravelStamp() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel oak stamp for domestic administration [craft_industrial_office_school_mail_payment_0002_travel_stamp]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0002_travel_stamp"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel oak stamp for domestic administration [craft_industrial_office_school_mail_payment_0002_travel_stamp]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel oak stamp for domestic administration from prepared oak stock.",
		Action = "making travel oak stamp for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel oak stamp for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 914.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("914 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0002_travel_stamp", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel oak stamp for domestic administration (@industrial_office_school_mail_payment_0002_travel_stamp)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelMildSteelTrayForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0003TravelTray() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel mild steel tray for domestic administration [craft_industrial_office_school_mail_payment_0003_travel_tray]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0003_travel_tray"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel mild steel tray for domestic administration [craft_industrial_office_school_mail_payment_0003_travel_tray]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel mild steel tray for domestic administration from prepared mild steel stock.",
		Action = "making travel mild steel tray for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel mild steel tray for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 955.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("955 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0003_travel_tray", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel mild steel tray for domestic administration (@industrial_office_school_mail_payment_0003_travel_tray)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelLeatherRulerForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0004TravelRuler() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel leather ruler for domestic administration [craft_industrial_office_school_mail_payment_0004_travel_ruler]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0004_travel_ruler"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel leather ruler for domestic administration [craft_industrial_office_school_mail_payment_0004_travel_ruler]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel leather ruler for domestic administration from prepared leather stock.",
		Action = "making travel leather ruler for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel leather ruler for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 996.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("996 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0004_travel_ruler", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel leather ruler for domestic administration (@industrial_office_school_mail_payment_0004_travel_ruler)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelPaperPenForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0005TravelPen() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel paper pen for domestic administration [craft_industrial_office_school_mail_payment_0005_travel_pen]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0005_travel_pen"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel paper pen for domestic administration [craft_industrial_office_school_mail_payment_0005_travel_pen]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel paper pen for domestic administration from prepared paper stock.",
		Action = "making travel paper pen for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel paper pen for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1036.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1036 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0005_travel_pen", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel paper pen for domestic administration (@industrial_office_school_mail_payment_0005_travel_pen)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelOakPencilForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0006TravelPencil() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel oak pencil for domestic administration [craft_industrial_office_school_mail_payment_0006_travel_pencil]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0006_travel_pencil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel oak pencil for domestic administration [craft_industrial_office_school_mail_payment_0006_travel_pencil]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel oak pencil for domestic administration from prepared oak stock.",
		Action = "making travel oak pencil for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel oak pencil for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1077.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1077 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0006_travel_pencil", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel oak pencil for domestic administration (@industrial_office_school_mail_payment_0006_travel_pencil)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelMildSteelSatchelForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0007TravelSatchel() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel mild steel satchel for domestic administration [craft_industrial_office_school_mail_payment_0007_travel_satchel]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0007_travel_satchel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel mild steel satchel for domestic administration [craft_industrial_office_school_mail_payment_0007_travel_satchel]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel mild steel satchel for domestic administration from prepared mild steel stock.",
		Action = "making travel mild steel satchel for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel mild steel satchel for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1118.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1118 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0007_travel_satchel", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel mild steel satchel for domestic administration (@industrial_office_school_mail_payment_0007_travel_satchel)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeTravelLeatherRegisterForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0008TravelRegister() => new()
	{
		Metadata = new("Office, school, mail and payment/make travel leather register for domestic administration [craft_industrial_office_school_mail_payment_0008_travel_register]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0008_travel_register"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make travel leather register for domestic administration [craft_industrial_office_school_mail_payment_0008_travel_register]",
		Category = "Office, school, mail and payment",
		Blurb = "Make a travel leather register for domestic administration from prepared leather stock.",
		Action = "making travel leather register for domestic administration",
		ActiveCraftItemSdesc = "an in-progress travel leather register for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1158.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1158 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0008_travel_register", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a travel leather register for domestic administration (@industrial_office_school_mail_payment_0008_travel_register)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeUtilityPaperWastepaperForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0009UtilityWastepaper() => new()
	{
		Metadata = new("Office, school, mail and payment/make utility paper wastepaper for domestic administration [craft_industrial_office_school_mail_payment_0009_utility_wastepaper]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0009_utility_wastepaper"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility paper wastepaper for domestic administration [craft_industrial_office_school_mail_payment_0009_utility_wastepaper]",
		Category = "Office, school, mail and payment",
		Blurb = "Make an utility paper wastepaper for domestic administration from prepared paper stock.",
		Action = "making utility paper wastepaper for domestic administration",
		ActiveCraftItemSdesc = "an in-progress utility paper wastepaper for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1199.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1199 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0009_utility_wastepaper", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility paper wastepaper for domestic administration (@industrial_office_school_mail_payment_0009_utility_wastepaper)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeUtilityOakLedgerForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0010UtilityLedger() => new()
	{
		Metadata = new("Office, school, mail and payment/make utility oak ledger for domestic administration [craft_industrial_office_school_mail_payment_0010_utility_ledger]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0010_utility_ledger"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility oak ledger for domestic administration [craft_industrial_office_school_mail_payment_0010_utility_ledger]",
		Category = "Office, school, mail and payment",
		Blurb = "Make an utility oak ledger for domestic administration from prepared oak stock.",
		Action = "making utility oak ledger for domestic administration",
		ActiveCraftItemSdesc = "an in-progress utility oak ledger for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1240.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1240 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0010_utility_ledger", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility oak ledger for domestic administration (@industrial_office_school_mail_payment_0010_utility_ledger)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeUtilityMildSteelNotebookForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0011UtilityNotebook() => new()
	{
		Metadata = new("Office, school, mail and payment/make utility mild steel notebook for domestic administration [craft_industrial_office_school_mail_payment_0011_utility_notebook]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0011_utility_notebook"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility mild steel notebook for domestic administration [craft_industrial_office_school_mail_payment_0011_utility_notebook]",
		Category = "Office, school, mail and payment",
		Blurb = "Make an utility mild steel notebook for domestic administration from prepared mild steel stock.",
		Action = "making utility mild steel notebook for domestic administration",
		ActiveCraftItemSdesc = "an in-progress utility mild steel notebook for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1280.0, Material: "mild steel", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1280 grams of mild steel", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0011_utility_notebook", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility mild steel notebook for domestic administration (@industrial_office_school_mail_payment_0011_utility_notebook)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeUtilityLeatherEnvelopeForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0012UtilityEnvelope() => new()
	{
		Metadata = new("Office, school, mail and payment/make utility leather envelope for domestic administration [craft_industrial_office_school_mail_payment_0012_utility_envelope]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0012_utility_envelope"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility leather envelope for domestic administration [craft_industrial_office_school_mail_payment_0012_utility_envelope]",
		Category = "Office, school, mail and payment",
		Blurb = "Make an utility leather envelope for domestic administration from prepared leather stock.",
		Action = "making utility leather envelope for domestic administration",
		ActiveCraftItemSdesc = "an in-progress utility leather envelope for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1321.0, Material: "leather", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1321 grams of leather", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0012_utility_envelope", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility leather envelope for domestic administration (@industrial_office_school_mail_payment_0012_utility_envelope)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition OfficeSchoolMailAndPaymentMakeUtilityPaperFolderForDomesticAdministrationCraftIndustrialOfficeSchoolMailPayment0013UtilityFolder() => new()
	{
		Metadata = new("Office, school, mail and payment/make utility paper folder for domestic administration [craft_industrial_office_school_mail_payment_0013_utility_folder]", "crafts", ["industrial"], ["item:industrial_office_school_mail_payment_0013_utility_folder"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make utility paper folder for domestic administration [craft_industrial_office_school_mail_payment_0013_utility_folder]",
		Category = "Office, school, mail and payment",
		Blurb = "Make an utility paper folder for domestic administration from prepared paper stock.",
		Action = "making utility paper folder for domestic administration",
		ActiveCraftItemSdesc = "an in-progress utility paper folder for domestic administration craft",
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
			new CatalogueCommodityRecipeInput(Grams: 1362.0, Material: "paper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("1362 grams of paper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrial_office_school_mail_payment_0013_utility_folder", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x an utility paper folder for domestic administration (@industrial_office_school_mail_payment_0013_utility_folder)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};

	public static CatalogueRecipeDefinition ComputingAndNetworkingMakePlainCopperScrapForDomesticDataWorkCraftIndustrialisedComputingNetworking0001PlainScrap() => new()
	{
		Metadata = new("Computing and networking/make plain copper scrap for domestic data work [craft_industrialised_computing_networking_0001_plain_scrap]", "crafts", ["information", "modern", "nuclear"], ["item:industrialised_computing_networking_0001_plain_scrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = true,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make plain copper scrap for domestic data work [craft_industrialised_computing_networking_0001_plain_scrap]",
		Category = "Computing and networking",
		Blurb = "Make a plain copper scrap for domestic data work from prepared copper stock.",
		Action = "making plain copper scrap for domestic data work",
		ActiveCraftItemSdesc = "an in-progress plain copper scrap for domestic data work craft",
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
			new CatalogueCommodityRecipeInput(Grams: 985.0, Material: "copper", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("985 grams of copper", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_computing_networking_0001_plain_scrap", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a plain copper scrap for domestic data work (@industrialised_computing_networking_0001_plain_scrap)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
