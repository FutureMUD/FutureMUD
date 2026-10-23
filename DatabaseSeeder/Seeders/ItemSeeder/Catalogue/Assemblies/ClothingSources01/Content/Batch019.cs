#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class ClothingSources01Stock
{
	public static CatalogueItemDefinition EarlymodernFootwearJoseonPaddedWintershoes() => new()
	{
		Metadata = new("earlymodern_footwear_joseon_padded_wintershoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of padded $colour cloth winter shoes",
		FullDescription = "A pair of padded $colour cloth winter shoes is cut and sewn from cotton so that the outward silhouette of the shoes remains clear. The cotton falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420,
		Cost = 54.0m,
		Skinnable = true,
		Material = "cotton",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "Padded closed footwear for Joseon winter household, scholar, market, road, and court-attendant use; colour and quilting pattern are skins.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearLowHuntingBoots() => new()
	{
		Metadata = new("earlymodern_footwear_low_hunting_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of low cuffed hunting boots",
		FullDescription = "A pair of low cuffed hunting boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 920,
		Cost = 110.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Hunting, rural estate, courier, forestry, and country-road footwear distinct from high riding boots.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearMaghrebiStitchedAnkleShoes() => new()
	{
		Metadata = new("earlymodern_footwear_maghrebi_stitched_ankle_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of stitched leather ankle shoes",
		FullDescription = "A pair of stitched leather ankle shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 58.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Closed ankle-height street and road footwear for Maghrebi urban, market, artisan, courier, and coastal contexts; distinct from pointed indoor slippers.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearNorthamericanFurlinedMoccasinboots() => new()
	{
		Metadata = new("earlymodern_footwear_northamerican_furlined_moccasinboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of $colour fur-lined moccasin boots",
		FullDescription = "Soft $colour deer leather forms the outer panels of these high-sided boots. Their rounded feet have flexible hide soles, joined to the uppers by closely spaced stitches rather than a separate rigid heel. A thick fur lining follows the interior and shows as a soft fringe at the upper openings. The cuffs fold against the legs, and narrow leather ties pass around them to draw the supple shafts together.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720,
		Cost = 86.0m,
		Skinnable = true,
		Material = "deer leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Strong", "Variable_LeatherColour"],
		BuilderNotes = "Canonical Renaissance stock item with an authored full description.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearPersianHeeledRidingshoes() => new()
	{
		Metadata = new("earlymodern_footwear_persian_heeled_ridingshoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of low-heeled $colour riding shoes",
		FullDescription = "A pair of low-heeled $colour riding shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 560,
		Cost = 120.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Persianate court, cavalry-retainer, hunting, courier, diplomatic, and mounted-merchant use.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearPilgrimRoadShoes() => new()
	{
		Metadata = new("earlymodern_footwear_pilgrim_road_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of stout pilgrim road shoes",
		FullDescription = "A pair of stout pilgrim road shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 690,
		Cost = 38.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Religious Clothing", "Institution / Religious"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_HeavyClothing", "Insulation_Minor"],
		BuilderNotes = "Shrine road, missionary, mendicant, courier, and long-distance foot travel; badges and religious identity remain separate.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearPointedWomensCourtshoes() => new()
	{
		Metadata = new("earlymodern_footwear_pointed_womens_courtshoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of pointed $colour court shoes",
		FullDescription = "A pair of pointed $colour court shoes is cut and sewn from silk so that the outward silhouette of the shoes remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 390,
		Cost = 360.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Early eighteenth-century court, assembly, bridal, diplomatic, and elite household footwear with shaped heel and fine upper.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearQingFeltWinterboots() => new()
	{
		Metadata = new("earlymodern_footwear_qing_felt_winterboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of felt-lined cloth winter boots",
		FullDescription = "A pair of felt-lined cloth winter boots is cut and sewn from felt so that the outward silhouette of the boots remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 980,
		Cost = 88.0m,
		Skinnable = false,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_LightClothing", "Insulation_Strong"],
		BuilderNotes = "Qing northern, banner household, merchant, travel, winter town, and frontier admission; official rank is not inferred.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearQingFlowerpotPlatformshoes() => new()
	{
		Metadata = new("earlymodern_footwear_qing_flowerpot_platformshoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of high platform court shoes",
		FullDescription = "A pair of high platform court shoes is shaped and bound from wood so that the outward silhouette of the shoes remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 920,
		Cost = 420.0m,
		Skinnable = false,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Qing Manchu female court and highest formal gate; central platform construction is integral and requires precise culture/date admission.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearRedheelCourtShoes() => new()
	{
		Metadata = new("earlymodern_footwear_redheel_court_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour red-heeled court shoes",
		FullDescription = "A pair of $colour red-heeled court shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 560,
		Cost = 480.0m,
		Skinnable = true,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "French-facing and imitative European court, diplomatic, and aristocratic gate; red heel is not ordinary-status footwear.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearRedseaStitchedHighlandShoes() => new()
	{
		Metadata = new("earlymodern_footwear_redsea_stitched_highland_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of stitched leather highland shoes",
		FullDescription = "A pair of stitched leather highland shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480,
		Cost = 34.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Closed stitched shoes for Ethiopian and Red Sea highland road, church-student, market, courier, and cool-ground use; local cut is required.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearRussianWovenBastShoes() => new()
	{
		Metadata = new("earlymodern_footwear_russian_woven_bast_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of woven bark-fibre shoes",
		FullDescription = "A pair of woven bark-fibre shoes is shaped and bound from barkcloth so that the outward silhouette of the shoes remains clear. The barkcloth keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280,
		Cost = 14.0m,
		Skinnable = false,
		Material = "barkcloth",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Work Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Light woven shoes for Russian and northern/eastern rural, market, poorer town, agricultural, and ordinary road use; exact bast weave is a skin.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSahelQuiltedRidingboots() => new()
	{
		Metadata = new("earlymodern_footwear_sahel_quilted_ridingboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of quilt-lined soft riding boots",
		FullDescription = "A pair of quilt-lined soft riding boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1080,
		Cost = 116.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_HeavyClothing", "Insulation_Strong"],
		BuilderNotes = "Quilt-lined boots for Sahelian and Hausa cavalry, courier, caravan, merchant, and court-retainer use under a narrower local culture manifest.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSeasiaRaisedCourtsandals() => new()
	{
		Metadata = new("earlymodern_footwear_seasia_raised_courtsandals", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "sandals",
		ShortDescription = "a pair of raised $colour court sandals",
		FullDescription = "A pair of raised $colour court sandals is shaped and bound from wood so that the outward silhouette of the sandals remains clear. The wood keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled. The sandals is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 720,
		Cost = 240.0m,
		Skinnable = true,
		Material = "wood",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Sandals", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Mainland and maritime Southeast Asian court, temple-service, dance, diplomatic, and elite household gate.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSilkLouisheelShoes() => new()
	{
		Metadata = new("earlymodern_footwear_silk_louisheel_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour silk high-heeled shoes",
		FullDescription = "A pair of $colour silk high-heeled shoes is cut and sewn from silk so that the outward silhouette of the shoes remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 420,
		Cost = 420.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Late seventeenth- and early eighteenth-century court and highest urban fashion gate; sculpted heel and silk upper are integral.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSilkcoveredLatchetShoes() => new()
	{
		Metadata = new("earlymodern_footwear_silkcovered_latchet_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of $colour silk-covered latchet shoes",
		FullDescription = "A pair of $colour silk-covered latchet shoes is cut and sewn from silk so that the outward silhouette of the shoes remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 430,
		Cost = 360.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Prosperous and elite female court, assembly, church, and formal household footwear; trim and buckle are skins/accessories.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSouthasianBacklessCourtMules() => new()
	{
		Metadata = new("earlymodern_footwear_southasian_backless_court_mules", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mules",
		ShortDescription = "a pair of embroidered $colour backless mules",
		FullDescription = "A pair of embroidered $colour backless mules is cut and sewn from silk so that the outward silhouette of the mules remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The mules is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 340,
		Cost = 140.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Backless indoor and ceremonial footwear for Mughal, Deccan, South Indian, court, merchant, and prosperous household admissions; embroidery is a skin.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearSteppeQuiltedRidingboots() => new()
	{
		Metadata = new("earlymodern_footwear_steppe_quilted_ridingboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of quilt-lined leather riding boots",
		FullDescription = "A pair of quilt-lined leather riding boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1180,
		Cost = 128.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_HeavyClothing", "Insulation_Strong"],
		BuilderNotes = "Quilt-lined riding boots for Inner Asian, caravan, courier, herding, diplomatic, and winter mounted contexts; the lining distinguishes them from Renaissance soft high boots.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearStiffJackboots() => new()
	{
		Metadata = new("earlymodern_footwear_stiff_jackboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of stiff high jackboots",
		FullDescription = "A pair of stiff high jackboots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800,
		Cost = 190.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Military Uniforms", "Functions / Military Equipment"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_High_Boots", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Cavalry, guard, officer, post rider, and hard campaign admission; military use is locally gated and not implied by the item alone.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearStrapBuckledHalfboots() => new()
	{
		Metadata = new("earlymodern_footwear_strap_buckled_halfboots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of strap-buckled leather half-boots",
		FullDescription = "A pair of strap-buckled leather half-boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 840,
		Cost = 92.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Boots", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Ankle-to-lower-calf boots secured by one or more side straps and buckles for town travel, coach service, couriers, hunting, and riding-adjacent use.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearTarredDeckShoes() => new()
	{
		Metadata = new("earlymodern_footwear_tarred_deck_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of tarred leather deck shoes",
		FullDescription = "A pair of tarred leather deck shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600,
		Cost = 46.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Maritime Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_HeavyClothing", "Insulation_Minor"],
		BuilderNotes = "Shipboard, fishing, dock, pilot, and coastal labour footwear; tarred appearance provides no waterproof mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearThicksoleWalkingShoes() => new()
	{
		Metadata = new("earlymodern_footwear_thicksole_walking_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of thick-soled walking shoes",
		FullDescription = "A pair of thick-soled walking shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620,
		Cost = 64.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Merchant, clerk, courier, pilgrim, traveller, and respectable urban use with more durable sole than court shoes.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearTiedLatchetShoes() => new()
	{
		Metadata = new("earlymodern_footwear_tied_latchet_shoes", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "shoes",
		ShortDescription = "a pair of tied latchet shoes",
		FullDescription = "A pair of tied latchet shoes is cut and stitched from leather so that the outward silhouette of the shoes remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The shoes is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520,
		Cost = 72.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Standard Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "Late seventeenth- and early eighteenth-century town, court-servant, merchant, lawyer, and prosperous household footwear before buckles dominate.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearTurncuffSeaBoots() => new()
	{
		Metadata = new("earlymodern_footwear_turncuff_sea_boots", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "boots",
		ShortDescription = "a pair of turn-cuffed high sea boots",
		FullDescription = "A pair of turn-cuffed high sea boots is cut and stitched from leather so that the outward silhouette of the boots remains clear. The leather shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape. The boots is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1420,
		Cost = 150.0m,
		Skinnable = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Maritime Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_High_Boots", "Armour_HeavyClothing", "Insulation_Moderate"],
		BuilderNotes = "Officer, pilot, boat crew, coastal courier, fishing, and foul-weather shipboard admission; no waterproof mechanic.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFootwearVelvetHouseSlippers() => new()
	{
		Metadata = new("earlymodern_footwear_velvet_house_slippers", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "slippers",
		ShortDescription = "a pair of $colour velvet house slippers",
		FullDescription = "A pair of $colour velvet house slippers is cut and sewn from velvet so that the outward silhouette of the slippers remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The slippers is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above. Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 260,
		Cost = 140.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Footwear", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Shoes", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "Prosperous household, scholar, invalid, private audience, and dressing-room footwear distinct from plain leather mules.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchClothingLaceTrimmedDayCap() => new()
	{
		Metadata = new("earlymodern_french_clothing_lace_trimmed_day_cap", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cap",
		ShortDescription = "a lace-trimmed linen day cap",
		FullDescription = "A lace-trimmed linen day cap is cut and sewn from linen so that the outward silhouette of the cap remains clear. The linen falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the cap around the head while leaving the characteristic outline plainly visible from the front and side. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140,
		Cost = 25.0m,
		Skinnable = false,
		Material = "linen",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Minor"],
		BuilderNotes = "New Early Modern prototype; used in 3 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchClothingLongSkirtedCoat() => new()
	{
		Metadata = new("earlymodern_french_clothing_long_skirted_coat", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "a long-skirted $colour broadcloth coat",
		FullDescription = "A long-skirted $colour broadcloth coat is cut and sewn from broadcloth so that the outward silhouette of the coat remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the coat carries the shape and surface detail that distinguish it at a glance. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500,
		Cost = 221.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Long_Coat", "Armour_HeavyClothing", "Insulation_Strong", "Variable_FineColour"],
		BuilderNotes = "New Early Modern prototype; used in 5 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchClothingShortCasaquinJacket() => new()
	{
		Metadata = new("earlymodern_french_clothing_short_casaquin_jacket", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "jacket",
		ShortDescription = "a short $colour flared jacket",
		FullDescription = "A short $colour flared jacket is cut and sewn from broadcloth so that the outward silhouette of the jacket remains clear. The broadcloth falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the jacket bends or fastens rather than spread as decoration. Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 107.0m,
		Skinnable = true,
		Material = "broadcloth",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New Early Modern prototype; used in 1 outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleCourtHeadwear() => new()
	{
		Metadata = new("earlymodern_french_noble_court_headwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "hat",
		ShortDescription = "a plumed $colour felt court hat",
		FullDescription = "A plumed $colour felt court hat is cut and sewn from felt so that the outward silhouette of the hat remains clear. The felt falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. Its crown, folds, or framing edges hold the hat around the head while leaving the characteristic outline plainly visible from the front and side. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Great,
		WeightInGrams = 232,
		Cost = 712.0m,
		Skinnable = true,
		Material = "felt",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Hat", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; headwear. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleFemaleCourtGown() => new()
	{
		Metadata = new("earlymodern_french_noble_female_court_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a finely draped $colour silk mantua gown",
		FullDescription = "A finely draped $colour silk mantua gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1127,
		Cost = 1280.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; female court. Used in 2 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleFemaleFormalGown() => new()
	{
		Metadata = new("earlymodern_french_noble_female_formal_gown", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "gown",
		ShortDescription = "a trained $colour silk court gown",
		FullDescription = "A trained $colour silk court gown is cut and sewn from silk so that the outward silhouette of the gown remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The contact points and fastenings are kept smooth, while the visible body of the gown carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1127,
		Cost = 3030.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Gown", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; female formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleFemaleTravelOuterwear() => new()
	{
		Metadata = new("earlymodern_french_noble_female_travel_outerwear", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "cloak",
		ShortDescription = "a hooded $colour velvet travelling cloak",
		FullDescription = "A hooded $colour velvet travelling cloak is cut and sewn from velvet so that the outward silhouette of the cloak remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the cloak its recognisable proportion when worn. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 1258,
		Cost = 1120.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Cloak_(Open)", "Armour_HeavyClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; female travel. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleJewellery() => new()
	{
		Metadata = new("earlymodern_french_noble_jewellery", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "brooch",
		ShortDescription = "a gem-set gold cravat brooch",
		FullDescription = "A gem-set gold cravat brooch is worked and joined from gold so that the outward silhouette of the brooch remains clear. The gold has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges. The contact points and fastenings are kept smooth, while the visible body of the brooch carries the shape and surface detail that distinguish it at a glance. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 93,
		Cost = 2245.0m,
		Skinnable = false,
		Material = "gold",
		Tags = ["Era / Early Modern Era", "Functions / Worn Items / Jewellery / Brooches", "Market / Jewellery / Noble Jewellery"],
		Components = ["Holdable", "Destroyable_Misc", "Wear_Brooch"],
		BuilderNotes = "New culture-family highest-echelon jewel prototype for French / Baroque court and urban. Exact rank, dynasty, office, and motif are skins. Used in 6 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleMaleCourtGarment() => new()
	{
		Metadata = new("earlymodern_french_noble_male_court_garment", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "coat",
		ShortDescription = "an embroidered $colour silk court coat",
		FullDescription = "An embroidered $colour silk court coat is cut and sewn from silk so that the outward silhouette of the coat remains clear. The silk falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The body and openings are proportioned for close, practical wear, with reinforcement placed where the coat bends or fastens rather than spread as decoration. Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Great,
		WeightInGrams = 857,
		Cost = 1028.0m,
		Skinnable = true,
		Material = "silk",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Jacket", "Armour_LightClothing", "Insulation_Minor", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; male court. Used in 3 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernFrenchNobleMaleFormalMantle() => new()
	{
		Metadata = new("earlymodern_french_noble_male_formal_mantle", "outfits", ["antiquity", "medieval", "renaissance", "earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitReferencedItemSpecs" , []),
		Noun = "mantle",
		ShortDescription = "a sweeping $colour velvet state mantle",
		FullDescription = "A sweeping $colour velvet state mantle is cut and sewn from velvet so that the outward silhouette of the mantle remains clear. The velvet falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges. The main panels settle from the shoulders into a controlled fall, and the hem and opening give the mantle its recognisable proportion when worn. The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 1057,
		Cost = 2635.0m,
		Skinnable = true,
		Material = "velvet",
		Tags = ["Era / Early Modern Era", "Market / Clothing / Luxury Clothing"],
		Components = ["Holdable", "Destroyable_Clothing", "Wear_Mantle", "Armour_LightClothing", "Insulation_Moderate", "Variable_FineColour"],
		BuilderNotes = "New culture-family highest-echelon prototype for French / Baroque court and urban; male formal. Used in 1 third-pass outfit(s).",
		AllowLegacyShortDescriptionMatch = false,
	};
}
