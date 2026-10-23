#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition EarlymodernGlobalmaritimeOfficerShipboardWaistcoat() => new()
	{
		Metadata = new("earlymodern_globalmaritime_officer_shipboard_waistcoat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "waistcoat",
		ShortDescription = "a plain $colour shipboard officer's waistcoat",
		FullDescription = "A plain $colour shipboard officer's waistcoat is cut and sewn from wool so that the outward silhouette of the waistcoat remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the waistcoat bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 500,
		Cost = 76.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Vest", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 9 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernGlobalmaritimeOfficerTropicalCompanyCoat() => new()
	{
		Metadata = new("earlymodern_globalmaritime_officer_tropical_company_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a light $colour company officer's coat",
		FullDescription = "A light $colour company officer's coat is cut and sewn from cotton so that the outward silhouette of the coat remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 920,
		Cost = 150.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Clothing / Military Uniforms"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New Early Modern military-uniform prototype; used in 1 military outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearAndeanTasseledFourcornerCap() => new()
	{
		Metadata = new("earlymodern_headwear_andean_tasseled_fourcorner_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a tasseled four-cornered $colour cap",
		FullDescription = "A tasseled four-cornered $colour cap is cut and sewn from camelid wool so that the outward silhouette of the cap remains clear. The camelid wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 230,
		Cost = 52.0m,
		Skinnable = true,
		Material = "camelid wool",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Locally woven four-cornered cap for Andean community, office, festival, market, and status admissions; tassels and motifs remain skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearCaribbeanPalmSunhat() => new()
	{
		Metadata = new("earlymodern_headwear_caribbean_palm_sunhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a broad woven palm sun hat",
		FullDescription = "A broad woven palm sun hat is shaped and bound from straw so that the outward silhouette of the hat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 20.0m,
		Skinnable = false,
		Material = "straw",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Reflector"],
		BuilderNotes = "Caribbean agricultural, fishing, boat, market, plantation-zone, and contact contexts; legal status is never inferred.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearChintiedRidingTurban() => new()
	{
		Metadata = new("earlymodern_headwear_chintied_riding_turban", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a chin-tied $colour riding turban",
		FullDescription = "A chin-tied $colour riding turban is cut and sewn from cotton so that the outward silhouette of the turban remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 330,
		Cost = 84.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "South Asian riding, courier, hunting, military-retainer, caravan, and long-road use; no armour claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearDutchWingedLinenCap() => new()
	{
		Metadata = new("earlymodern_headwear_dutch_winged_linen_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a winged white linen day cap",
		FullDescription = "A winged white linen day cap is cut and sewn from linen so that the outward silhouette of the cap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 130,
		Cost = 64.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Structured female day cap with projecting side wings for Dutch, Low Countries, Atlantic merchant, churchgoing, and prosperous household admission.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearEdoBlackSilkTownhood() => new()
	{
		Metadata = new("earlymodern_headwear_edo_black_silk_townhood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a black silk town hood",
		FullDescription = "A black silk town hood is cut and sewn from silk so that the outward silhouette of the hood remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140,
		Cost = 70.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Close Edo urban hood for merchants, travellers, performers, religious visitors, and cool-weather town use; exact tying and local name remain skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearEdoDeepTravelKasa() => new()
	{
		Metadata = new("earlymodern_headwear_edo_deep_travel_kasa", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a deep conical woven travel hat",
		FullDescription = "A deep conical woven travel hat is shaped and bound from straw so that the outward silhouette of the hat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 28.0m,
		Skinnable = false,
		Material = "straw",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Reflector"],
		BuilderNotes = "Edo road, courier, pilgrim, rural, rain, and incognito travel contexts; it grants no identity or weatherproof mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearEdoFlatWovenKasa() => new()
	{
		Metadata = new("earlymodern_headwear_edo_flat_woven_kasa", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a flat woven travel hat",
		FullDescription = "A flat woven travel hat is shaped and bound from straw so that the outward silhouette of the hat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300,
		Cost = 22.0m,
		Skinnable = false,
		Material = "straw",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Reflector"],
		BuilderNotes = "Edo traveller, porter, pilgrim, market, field, and boatman use; ties and weave are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearEthiopianWrappedWhiteHood() => new()
	{
		Metadata = new("earlymodern_headwear_ethiopian_wrapped_white_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a full white wrapped head hood",
		FullDescription = "A full white wrapped head hood is cut and sewn from cotton so that the outward silhouette of the hood remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 64.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Ethiopian highland, Red Sea, church-associated, court, travel, and household use; ordination is not inferred.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFoldedSafavidTurbancap() => new()
	{
		Metadata = new("earlymodern_headwear_folded_safavid_turbancap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "turban",
		ShortDescription = "a folded $colour turban cap",
		FullDescription = "A folded $colour turban cap is cut and sewn from silk so that the outward silhouette of the turban remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the turban around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 300,
		Cost = 180.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Turban", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Persianate court, Sufi, military-status, diplomatic, and urban elite gate; folds and colour require a local manifest.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFontangeLaceHeaddress() => new()
	{
		Metadata = new("earlymodern_headwear_fontange_lace_headdress", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "headdress",
		ShortDescription = "a tall lace-and-ribbon headdress",
		FullDescription = "A tall lace-and-ribbon headdress is cut and sewn from lace so that the outward silhouette of the headdress remains clear. The lace falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the headdress around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 190,
		Cost = 420.0m,
		Skinnable = false,
		Material = "lace",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Late seventeenth- and early eighteenth-century court or highest urban fashion gate; height, ribbon, and wire profile are integral.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFrontierLeatherRoadHood() => new()
	{
		Metadata = new("earlymodern_headwear_frontier_leather_road_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a fur-lined brown leather road hood",
		FullDescription = "A fur-lined brown leather road hood is cut and stitched from leather so that the outward silhouette of the hood remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 110.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_HeavyClothing", "Insulation_Strong"],
		BuilderNotes = "Close winter road hood for frontier couriers, mounted merchants, hunters, guards, and caravan travel; fur type and local cut remain skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFullbottomCourtWig() => new()
	{
		Metadata = new("earlymodern_headwear_fullbottom_court_wig", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "wig",
		ShortDescription = "a full-bottomed $colour court wig",
		FullDescription = "A full-bottomed $colour court wig is shaped and bound from horsehair so that the outward silhouette of the wig remains clear. The horsehair keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The contact points and fastenings are kept smooth, while the visible body of the wig carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 720,
		Cost = 640.0m,
		Skinnable = true,
		Material = "horsehair",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Wig", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour", "Wig"],
		BuilderNotes = "Highest court, diplomatic, judicial, senior office, and formal ceremonial admission; not ordinary bourgeois wear.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFuredgePersianCap() => new()
	{
		Metadata = new("earlymodern_headwear_furedge_persian_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a fur-edged $colour Persianate cap",
		FullDescription = "A fur-edged $colour Persianate cap is cut and sewn from velvet so that the outward silhouette of the cap remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 300,
		Cost = 150.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Safavid/post-Safavid court, merchant, diplomatic, scholar, and winter use; public text remains form-based.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFurlinedKavukCap() => new()
	{
		Metadata = new("earlymodern_headwear_furlined_kavuk_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a fur-lined $colour structured cap",
		FullDescription = "A fur-lined $colour structured cap is cut and sewn from broadcloth so that the outward silhouette of the cap remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 430,
		Cost = 160.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "Ottoman court, household, officer, diplomatic, winter, and senior urban use; exact kavuk form and entitlement are local.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearFurtrimMourningHood() => new()
	{
		Metadata = new("earlymodern_headwear_furtrim_mourning_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a black fur-trimmed mourning hood",
		FullDescription = "A black fur-trimmed mourning hood is cut and sewn from broadcloth so that the outward silhouette of the hood remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 430,
		Cost = 160.0m,
		Skinnable = false,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Ceremonial Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Strong"],
		BuilderNotes = "Court, elite, urban, widowhood, and formal mourning gate; exact entitlement and duration are local social rules.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearIberianSilkHairCaul() => new()
	{
		Metadata = new("earlymodern_headwear_iberian_silk_hair_caul", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "caul",
		ShortDescription = "a netted black silk hair caul",
		FullDescription = "A netted black silk hair caul is cut and sewn from silk so that the outward silhouette of the caul remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the caul around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 80,
		Cost = 120.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Close netted hair covering for Iberian and colonial urban, court, churchgoing, and prosperous household women; jewels and metallic thread remain skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearJoseonNarrowGat() => new()
	{
		Metadata = new("earlymodern_headwear_joseon_narrow_gat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a narrow-brimmed horsehair scholar hat",
		FullDescription = "A narrow-brimmed horsehair scholar hat is shaped and bound from horsehair so that the outward silhouette of the hat remains clear. The horsehair keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 100.0m,
		Skinnable = false,
		Material = "horsehair",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Joseon scholar, town, travel, teaching, and lesser-official admission distinct from broader formal hats.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearJoseonPaddedWinterhood() => new()
	{
		Metadata = new("earlymodern_headwear_joseon_padded_winterhood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a padded $colour winter hood",
		FullDescription = "A padded $colour winter hood is cut and sewn from cotton so that the outward silhouette of the hood remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360,
		Cost = 72.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "Joseon women, children, household, travel, and winter market contexts; exact outline and status are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearLaceLappetCap() => new()
	{
		Metadata = new("earlymodern_headwear_lace_lappet_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a lace-trimmed linen lappet cap",
		FullDescription = "A lace-trimmed linen lappet cap is cut and sewn from linen so that the outward silhouette of the cap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 95,
		Cost = 144.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Late seventeenth- and early eighteenth-century female day, court, prosperous household, and formal domestic cap with hanging lappets.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearLongStockingSeamanCap() => new()
	{
		Metadata = new("earlymodern_headwear_long_stocking_seaman_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a long tapered $colour knitted seaman cap",
		FullDescription = "A long tapered $colour knitted seaman cap is cut and sewn from wool so that the outward silhouette of the cap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 210,
		Cost = 30.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Maritime Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_BasicColour"],
		BuilderNotes = "Long tapered knitted cap for sailors, fishers, dockworkers, northern labourers, and night watches; the extended crown distinguishes it from close knitted caps.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearLongTasselledNightcap() => new()
	{
		Metadata = new("earlymodern_headwear_long_tasselled_nightcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "nightcap",
		ShortDescription = "a long tapered $colour wool nightcap",
		FullDescription = "A long tapered $colour wool nightcap is cut and sewn from wool so that the outward silhouette of the nightcap remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the nightcap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150,
		Cost = 48.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Long pointed or tapered sleeping cap for household, inn, travel, servant, merchant, and colder indoor use; tassel and stripes are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearMingFourcornerScholarcap() => new()
	{
		Metadata = new("earlymodern_headwear_ming_fourcorner_scholarcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a four-cornered black scholar cap",
		FullDescription = "A four-cornered black scholar cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 150,
		Cost = 120.0m,
		Skinnable = false,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Ceremonial Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Late-Ming survival, ritual, teacher, scholar, theatre, and local office gate; exact appendages are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearMughalJewelboundSkullcap() => new()
	{
		Metadata = new("earlymodern_headwear_mughal_jewelbound_skullcap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "skullcap",
		ShortDescription = "a fitted $colour silk court skullcap",
		FullDescription = "A fitted $colour silk court skullcap is cut and sewn from silk so that the outward silhouette of the skullcap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the skullcap carries the shape and surface detail that distinguish it at a glance. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 120,
		Cost = 160.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Skullcap", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Fitted Mughal and Indo-Persian court or household cap; jewels, pearls, embroidery, office, and whether it is worn alone remain skins and admissions.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearNarrowbrimTownHat() => new()
	{
		Metadata = new("earlymodern_headwear_narrowbrim_town_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a narrow-brimmed $colour felt town hat",
		FullDescription = "A narrow-brimmed $colour felt town hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280,
		Cost = 68.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Urban artisan, shopkeeper, clerk, merchant, and respectable servant headwear distinct from broad field hats.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearOttomanBroadMuslinHeadveil() => new()
	{
		Metadata = new("earlymodern_headwear_ottoman_broad_muslin_headveil", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "veil",
		ShortDescription = "a broad white cotton head veil",
		FullDescription = "A broad white cotton head veil is cut and sewn from cotton so that the outward silhouette of the veil remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the veil around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150,
		Cost = 52.0m,
		Skinnable = false,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Head_Veil", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Broad light head veil for locally admitted Ottoman, Balkan, Maghrebi, urban, household, religious, and market contexts; exact drape and status are local.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearOttomanQuiltedHouseCap() => new()
	{
		Metadata = new("earlymodern_headwear_ottoman_quilted_house_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a quilted $colour silk house cap",
		FullDescription = "A quilted $colour silk house cap is cut and sewn from silk so that the outward silhouette of the cap remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150,
		Cost = 90.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Informal indoor cap for Ottoman merchant, scholar, court-household, and prosperous urban admissions; exact quilting and embroidery are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearPaddedNavalWatchhood() => new()
	{
		Metadata = new("earlymodern_headwear_padded_naval_watchhood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a close $colour wool naval watch hood",
		FullDescription = "A close $colour wool naval watch hood is cut and sewn from wool so that the outward silhouette of the hood remains clear. The wool falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360,
		Cost = 48.0m,
		Skinnable = true,
		Material = "wool",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Maritime Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_HeavyClothing", "Insulation_Strong", "Variable_BasicColour"],
		BuilderNotes = "Close padded hood for cold deck watches, pilots, boat crews, and northern shipboard labour; it grants no rank or equipment mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearPhysiciansRoundCap() => new()
	{
		Metadata = new("earlymodern_headwear_physicians_round_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a full round $colour physician cap",
		FullDescription = "A full round $colour physician cap is cut and sewn from velvet so that the outward silhouette of the cap remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 180,
		Cost = 180.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Physician, jurist, professor, senior clerk, and learned-household dress where a full round cap is locally retained.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearPinnedRidingHood() => new()
	{
		Metadata = new("earlymodern_headwear_pinned_riding_hood", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hood",
		ShortDescription = "a close pinned $colour riding hood",
		FullDescription = "A close pinned $colour riding hood is cut and sewn from broadcloth so that the outward silhouette of the hood remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hood around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360,
		Cost = 84.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hood", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Female riding, coach, travel, country-house, and cold-weather hood; veil and cape length remain skins where coverage matches.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearPostilionLeatherCap() => new()
	{
		Metadata = new("earlymodern_headwear_postilion_leather_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a close brown leather postilion cap",
		FullDescription = "A close brown leather postilion cap is cut and stitched from leather so that the outward silhouette of the cap remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 300,
		Cost = 72.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate"],
		BuilderNotes = "Close secured cap for postilions, coach servants, couriers, riders, and exposed road work; no armour or weatherproofing mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearQingBroadConicalWorkhat() => new()
	{
		Metadata = new("earlymodern_headwear_qing_broad_conical_workhat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a broad conical woven work hat",
		FullDescription = "A broad conical woven work hat is cut and sewn from bamboo so that the outward silhouette of the hat remains clear. The bamboo falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340,
		Cost = 24.0m,
		Skinnable = false,
		Material = "bamboo",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Reflector"],
		BuilderNotes = "Farmer, boatman, porter, market, rural patrol, and traveller admission; no waterproof mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearQingSummerCourtHat() => new()
	{
		Metadata = new("earlymodern_headwear_qing_summer_court_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a conical woven summer court hat",
		FullDescription = "A conical woven summer court hat is shaped and bound from straw so that the outward silhouette of the hat remains clear. The straw keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 360,
		Cost = 320.0m,
		Skinnable = false,
		Material = "straw",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Ceremonial Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Qing court, banner office, formal audience, diplomatic, and seasonal gate; finial, rank, plume, and fringe are separate presentation.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernHeadwearQingWinterCourtHat() => new()
	{
		Metadata = new("earlymodern_headwear_qing_winter_court_hat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a fur-brimmed winter court hat",
		FullDescription = "A fur-brimmed winter court hat is cut and stitched from fur so that the outward silhouette of the hat remains clear. The fur shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 520,
		Cost = 420.0m,
		Skinnable = false,
		Material = "fur",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Headwear", "Market / Clothing / Ceremonial Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Strong"],
		BuilderNotes = "Qing winter court, banner office, diplomatic, and formal audience gate; finial and rank marks are separate.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
