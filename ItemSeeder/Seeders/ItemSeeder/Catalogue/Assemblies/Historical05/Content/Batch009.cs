#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalMedicalLeechJar() => new()
	{
		Metadata = new("medieval_medical_leech_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2103", []),
		Noun = "jar",
		ShortDescription = "a small leech jar",
		LongDescription = null,
		FullDescription = "This glazed ceramic jar has a perforated lid tied down with cord. Water marks stain the inside, and the rim is broad enough for a hand to reach in carefully. It is a medical storage vessel rather than a drinking jar.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 750.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glazed ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Surgical Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeScalpel() => new()
	{
		Metadata = new("medieval_medical_bronze_scalpel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2131", []),
		Noun = "scalpel",
		ShortDescription = "a bronze surgical scalpel",
		LongDescription = null,
		FullDescription = "A small bronze blade is set into a plain handle and honed to a narrow working edge. The blade is leaf-shaped and darkened near the socket from age and oil. It is a surgical knife, not a weapon meant for battle.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Scalpel"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWroughtIronSurgicalKnife() => new()
	{
		Metadata = new("medieval_medical_wrought_iron_surgical_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2159", []),
		Noun = "knife",
		ShortDescription = "a wrought-iron surgical knife",
		LongDescription = null,
		FullDescription = "This small wrought-iron knife has a narrow blade and a wrapped wooden grip. The edge is keen but plain, with tool marks still visible along the spine. It is built for incision and scraping rather than ornament.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Scalpel"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeForceps() => new()
	{
		Metadata = new("medieval_medical_bronze_forceps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2187", []),
		Noun = "forceps",
		ShortDescription = "a pair of bronze forceps",
		LongDescription = null,
		FullDescription = "These bronze forceps are narrow, springy, and flattened at the tips. The metal shows polished bright places where fingers press them together. They are useful for grasping cloth, splinters, or small pieces during wound work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Forceps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeSurgicalProbe() => new()
	{
		Metadata = new("medieval_medical_bronze_surgical_probe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2215", []),
		Noun = "probe",
		ShortDescription = "a bronze surgical probe",
		LongDescription = null,
		FullDescription = "A slender bronze probe has a rounded end at one side and a flattened spatulate end at the other. The shaft is straight, smooth, and slightly dark in the middle. It is made for feeling, lifting, and applying rather than cutting.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Surgical Probe"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeArterialClamp() => new()
	{
		Metadata = new("medieval_medical_bronze_arterial_clamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2243", []),
		Noun = "clamp",
		ShortDescription = "a bronze arterial clamp",
		LongDescription = null,
		FullDescription = "This bronze clamp has flattened jaws and a crude locking bend behind them. It is not delicate, but the bite is firm and the spring returns when released. The tool is meant for serious wound work where hands alone are not enough.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 125.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Arterial Clamp"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalSmallBonesaw() => new()
	{
		Metadata = new("medieval_medical_small_bonesaw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2271", []),
		Noun = "saw",
		ShortDescription = "a small surgical bonesaw",
		LongDescription = null,
		FullDescription = "A narrow metal saw blade is fixed into a wooden grip, with fine teeth running along one edge. The blade is shorter than a carpenter’s saw and easier to control close to the body. Oil darkens the join between metal and handle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Bonesaw"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBronzeCauteryIron() => new()
	{
		Metadata = new("medieval_medical_bronze_cautery_iron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2299", []),
		Noun = "iron",
		ShortDescription = "a bronze cautery iron",
		LongDescription = null,
		FullDescription = "A bronze working end sits on a long handle meant to keep the hand away from heat. The tip is flattened, dark, and smoke-stained, while the grip has been wrapped in old leather. It is a stern-looking surgical tool.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Cautery Iron"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWroughtIronCauteryBrazier() => new()
	{
		Metadata = new("medieval_medical_wrought_iron_cautery_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2327", []),
		Noun = "brazier",
		ShortDescription = "a small cautery brazier",
		LongDescription = null,
		FullDescription = "This small wrought-iron brazier has a shallow fire bowl and three short feet. The rim is blackened by charcoal and ash, but there is no special incense fitting. It is meant to heat tools at the edge of a work space.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalApothecaryBalanceScale() => new()
	{
		Metadata = new("medieval_medical_apothecary_balance_scale", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2354", []),
		Noun = "scale",
		ShortDescription = "a small apothecary balance scale",
		LongDescription = null,
		FullDescription = "A compact balance scale folds into a little wooden case with two shallow pans. The beam is fine bronze and the weights sit in their own recesses. It is delicate enough for powders, resins, and measured medicine stock.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalSurgicalNeedleCase() => new()
	{
		Metadata = new("medieval_medical_surgical_needle_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2381", []),
		Noun = "case",
		ShortDescription = "a narrow surgical needle case",
		LongDescription = null,
		FullDescription = "This narrow case is carved from bone and fitted with a tight stopper. The inside is long enough for curved needles and slim probes, while the outside is polished by handling. A cord loop lets it hang inside a surgical roll.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalApothecarySortingTray() => new()
	{
		Metadata = new("medieval_medical_apothecary_sorting_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2409", []),
		Noun = "tray",
		ShortDescription = "a shallow apothecary sorting tray",
		LongDescription = null,
		FullDescription = "A shallow wooden tray is divided into small sections for herbs, powders, and tiny packets. The surface is stained by saffron, resin, and dark medicine dust. It is light enough to lift but broad enough to work on beside a mortar.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHoneyPoulticePot() => new()
	{
		Metadata = new("medieval_drug_honey_poultice_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2439", []),
		Noun = "pot",
		ShortDescription = "a pot of honey poultice",
		LongDescription = null,
		FullDescription = "A small fired-clay pot is capped with waxed cloth over a sticky honey poultice. Sweetness leaks through the wrap, mingled with a faint herbal smell. A flat spatula mark remains in the surface of the mixture.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fired clay",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antiseptic Dressing"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Honey_Poultice"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugGarlicSalvePot() => new()
	{
		Metadata = new("medieval_drug_garlic_salve_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2469", []),
		Noun = "pot",
		ShortDescription = "a pungent garlic salve pot",
		LongDescription = null,
		FullDescription = "This thumb-sized ceramic pot is sealed with beeswax and cloth. The garlic smell pushes through the seal, sharp and earthy. Dark green flecks of herb cling beneath the rim where the salve has been stirred.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugAloeBurnSalveJar() => new()
	{
		Metadata = new("medieval_drug_aloe_burn_salve_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2498", []),
		Noun = "jar",
		ShortDescription = "a jar of aloe burn salve",
		LongDescription = null,
		FullDescription = "A small glazed jar contains a cool green salve under a neat cloth cap. The contents are glossy and smooth where the jar has been levelled. It smells mild, fresh, and faintly bitter.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glazed ceramic",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Aloe_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHerbalBurnSalvePot() => new()
	{
		Metadata = new("medieval_drug_herbal_burn_salve_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2527", []),
		Noun = "pot",
		ShortDescription = "a pot of green burn salve",
		LongDescription = null,
		FullDescription = "This squat pot holds a thick green-brown salve with a resin sheen. A strip of linen is tied over the top and darkened by oil at the edges. The smell is grassy, bitter, and slightly smoky.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 230.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Herbal_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugYarrowStypticCake() => new()
	{
		Metadata = new("medieval_drug_yarrow_styptic_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2556", []),
		Noun = "cake",
		ShortDescription = "a pressed yarrow styptic cake",
		LongDescription = null,
		FullDescription = "Crushed yarrow has been pressed into a small greenish cake and wrapped in linen. The surface is rough and fibrous, with a bitter smell that clings to the cloth. It is shaped to be rubbed or broken into a dressing.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "yarrow",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Yarrow_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugAlumStypticCone() => new()
	{
		Metadata = new("medieval_drug_alum_styptic_cone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2586", []),
		Noun = "cone",
		ShortDescription = "a pale alum styptic cone",
		LongDescription = null,
		FullDescription = "A small cone of pale mineral paste has dried hard and chalky. It is wrapped in parchment with the tip protected by a twist of cloth. The surface leaves faint white dust on the fingers when handled.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "alum",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Alum_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugYarrowLinenStypticRoll() => new()
	{
		Metadata = new("medieval_drug_yarrow_linen_styptic_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2615", []),
		Noun = "roll",
		ShortDescription = "a yarrow-linen styptic roll",
		LongDescription = null,
		FullDescription = "A narrow linen roll is dusted with crushed yarrow through its inner layers. The outside looks like a normal small bandage, but the green flecks show at the tucked end. It smells dry, bitter, and clean.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
		Cost = 11.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Bandage"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Yarrow_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugAlumWoolPressurePad() => new()
	{
		Metadata = new("medieval_drug_alum_wool_pressure_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2646", []),
		Noun = "pad",
		ShortDescription = "an alum-dusted wool pressure pad",
		LongDescription = null,
		FullDescription = "Packed wool is wrapped in linen and dusted inside with pale alum powder. The pad is thicker than a common dressing and meant to be pressed down firmly. A dry mineral smell mixes with the wool.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 125.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Tend Kit"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single",
				"TopicalCream_Alum_Styptic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHoneyAloeBurnDressing() => new()
	{
		Metadata = new("medieval_drug_honey_aloe_burn_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2677", []),
		Noun = "dressing",
		ShortDescription = "a honey-and-aloe burn dressing",
		LongDescription = null,
		FullDescription = "This soft dressing is slick with a honeyed green salve and folded into a waxed wrapper. It is moist but not dripping, with a sweet smell beneath the plant bitterness. The pad is broad enough for a careful burn covering.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great",
				"TopicalCream_Aloe_Burn_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugGarlicHoneyPoulticePacket() => new()
	{
		Metadata = new("medieval_drug_garlic_honey_poultice_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2709", []),
		Noun = "packet",
		ShortDescription = "a garlic-honey poultice packet",
		LongDescription = null,
		FullDescription = "A waxed cloth packet holds a thick mixture of honey, garlic, and crushed herbs. The smell is both sweet and sharp, with a sticky stain darkening the fold. It is small enough for a single dressing rather than a jar of repeated use.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "honey",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugWillowBarkLozengePacket() => new()
	{
		Metadata = new("medieval_drug_willow_bark_lozenge_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2738", []),
		Noun = "packet",
		ShortDescription = "a packet of willow-bark lozenges",
		LongDescription = null,
		FullDescription = "Several dark, bitter lozenges are wrapped in a small paper fold. Their surfaces are rough with powdered bark, honey, and a little spice to bind them. The packet is meant for measured swallowing rather than chewing as sweets.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Willow_Bark_Tea"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMandrakeSleepPellets() => new()
	{
		Metadata = new("medieval_drug_mandrake_sleep_pellets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2767", []),
		Noun = "packet",
		ShortDescription = "a packet of mandrake pellets",
		LongDescription = null,
		FullDescription = "Tiny dark pellets sit inside a red-tied paper packet, each one rolled from powdered root and honey. The packet smells bitter and earthy even when closed. A careful hand has made the pellets nearly equal in size.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 35.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mandrake",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mandrake_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMintLozengePacket() => new()
	{
		Metadata = new("medieval_drug_mint_lozenge_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2796", []),
		Noun = "packet",
		ShortDescription = "a packet of mint lozenges",
		LongDescription = null,
		FullDescription = "Pale green lozenges are folded into a little paper packet with mint crumbs caught at the creases. They smell fresh, sweet, and cooling. The pieces are plain medicine-shop fare rather than confectionery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mint",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mint_Infusion"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEphedraHoneyCakes() => new()
	{
		Metadata = new("medieval_drug_ephedra_honey_cakes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2825", []),
		Noun = "packet",
		ShortDescription = "a packet of ephedra honey cakes",
		LongDescription = null,
		FullDescription = "Small honey-bound cakes are speckled with chopped ephedra and wrapped in oiled paper. The cakes are dry at the surface but still sticky at the edges. Their smell is grassy, sweet, and faintly resinous.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ephedra",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Ephedra_Brew"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugFoxgloveWarningBeads() => new()
	{
		Metadata = new("medieval_drug_foxglove_warning_beads", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2854", []),
		Noun = "packet",
		ShortDescription = "a red-tied packet of foxglove beads",
		LongDescription = null,
		FullDescription = "Several tiny dark beads are tied into a warning-marked packet with red thread. The beads are very small, carefully rolled, and bitter-smelling. The presentation makes clear that these are not ordinary household lozenges.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "foxglove",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Foxglove_Tincture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPoppyLatexBolus() => new()
	{
		Metadata = new("medieval_drug_poppy_latex_bolus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2883", []),
		Noun = "bolus",
		ShortDescription = "a dark poppy-latex bolus",
		LongDescription = null,
		FullDescription = "A small dark bolus is wrapped in oiled cloth and tied shut with thread. It feels resinous under the wrapping, and its bitter smell is heavy and unmistakable. The size is deliberately small, as though meant to be measured with care.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 25.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Pill_Poppy_Latex_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMintGingerPastilles() => new()
	{
		Metadata = new("medieval_drug_mint_ginger_pastilles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2912", []),
		Noun = "packet",
		ShortDescription = "a packet of mint-ginger pastilles",
		LongDescription = null,
		FullDescription = "Round pastilles of mint, ginger, and honey are stacked in a paper fold. They smell warming and fresh at once, with ginger fibres visible in the surface. The packet is sized for a traveller or sickroom attendant.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ginger",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mint_and_Ginger_Tonic"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugTheriacElectuaryBall() => new()
	{
		Metadata = new("medieval_drug_theriac_electuary_ball", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2941", []),
		Noun = "ball",
		ShortDescription = "a dark theriac electuary ball",
		LongDescription = null,
		FullDescription = "A glossy ball of dark electuary is wrapped in a square of oiled parchment. The smell is rich with honey, spice, bitterness, and resin. It is carefully rounded and sealed as a valued apothecary dose.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 40.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "spice",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Theriac_Electuary"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugTheriacTravelPacket() => new()
	{
		Metadata = new("medieval_drug_theriac_travel_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:2971", []),
		Noun = "packet",
		ShortDescription = "a sealed theriac travel packet",
		LongDescription = null,
		FullDescription = "This small packet contains several pea-sized electuary doses separated by waxed folds. The outer wrapping is tied twice and smells strongly of spice. It is made for cautious travel use rather than an open shop jar.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 75.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "spice",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Pill_Theriac_Electuary"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPoppyHoneySleepBolus() => new()
	{
		Metadata = new("medieval_drug_poppy_honey_sleep_bolus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3001", []),
		Noun = "bolus",
		ShortDescription = "a poppy-honey sleep bolus",
		LongDescription = null,
		FullDescription = "Honey, resin, and powdered herb have been rolled into a dark, tacky bolus. It is wrapped alone in oiled cloth and tied with a black thread. The smell is heavy, sweet, and bitter enough to discourage casual tasting.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "honey",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Pill_Poppy_Latex_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEmptyGlassMedicineVial() => new()
	{
		Metadata = new("medieval_drug_empty_glass_medicine_vial", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3030", []),
		Noun = "vial",
		ShortDescription = "an empty glass medicine vial",
		LongDescription = null,
		FullDescription = "This small glass vial has a narrow neck and a plain stopper fitted with wax around the rim. The glass is faintly green and thick for its size. It is empty, clean, and ready for a measured medicine.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Medicine_Vial_30ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEmptyCeramicMedicineBottle() => new()
	{
		Metadata = new("medieval_drug_empty_ceramic_medicine_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3057", []),
		Noun = "bottle",
		ShortDescription = "an empty ceramic medicine bottle",
		LongDescription = null,
		FullDescription = "A little ceramic bottle has a rounded shoulder, narrow mouth, and cork stopper under a cloth tie. The outer surface is unglazed except for a smooth lip. It is sturdy enough for travel and opaque enough to protect its contents from casual inspection.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Bottle_100ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEmptyLeatherMedicineFlask() => new()
	{
		Metadata = new("medieval_drug_empty_leather_medicine_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3084", []),
		Noun = "flask",
		ShortDescription = "an empty leather medicine flask",
		LongDescription = null,
		FullDescription = "This small leather flask has a wooden stopper and a waxed seam along one side. It is soft-sided, dark from oiling, and fitted with a cord loop. It is made for travelling medicine rather than table drink.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Flask_250ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHornMedicineVial() => new()
	{
		Metadata = new("medieval_drug_horn_medicine_vial", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3111", []),
		Noun = "vial",
		ShortDescription = "a small horn medicine vial",
		LongDescription = null,
		FullDescription = "A polished horn vial is capped with a tight wooden plug and sealed with a strip of cloth. The translucent brown walls hide most of the contents but catch light at the thin edges. It is a humble alternative to glass.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Vial_30ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugWillowBarkTeaFlask() => new()
	{
		Metadata = new("medieval_drug_willow_bark_tea_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3138", []),
		Noun = "flask",
		ShortDescription = "a flask of willow-bark tea",
		LongDescription = null,
		FullDescription = "A small stoppered flask is filled with a dark bitter tea. The outside is plain leather over a rigid liner, with a willow chip tied to the neck as a sign. The vessel smells faintly of bark and steeped water.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Willow_Bark_Tea_250ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMandrakeDraughtBottle() => new()
	{
		Metadata = new("medieval_drug_mandrake_draught_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3167", []),
		Noun = "bottle",
		ShortDescription = "a bottle of mandrake draught",
		LongDescription = null,
		FullDescription = "A small opaque ceramic bottle is stoppered and tied with a red warning thread. Bitter earthy smells cling around the waxed mouth. The bottle is compact and carefully sealed, meant for a measured draught rather than table use.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 190.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mandrake_Draught_100ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMintInfusionFlask() => new()
	{
		Metadata = new("medieval_drug_mint_infusion_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3196", []),
		Noun = "flask",
		ShortDescription = "a flask of mint infusion",
		LongDescription = null,
		FullDescription = "This small leather flask carries a pale herbal infusion and a tied sprig of mint at the stopper. The vessel is travel-worn but clean, with wax pressed into the seam. A fresh green smell escapes when it is handled.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 270.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mint_Infusion_250ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugEphedraBrewFlask() => new()
	{
		Metadata = new("medieval_drug_ephedra_brew_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3225", []),
		Noun = "flask",
		ShortDescription = "a flask of ephedra brew",
		LongDescription = null,
		FullDescription = "A dark leather flask is stoppered tightly over a bitter herbal brew. Dried jointed twigs are tied to the neck for identification. The vessel is compact, practical, and clearly meant for a road kit or healer’s pack.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 285.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Ephedra_Brew_250ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugFoxgloveTinctureVial() => new()
	{
		Metadata = new("medieval_drug_foxglove_tincture_vial", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3254", []),
		Noun = "vial",
		ShortDescription = "a red-marked foxglove tincture vial",
		LongDescription = null,
		FullDescription = "This narrow glass vial is sealed with wax and tied with red thread around the neck. A dark tincture clings to the inside in a thin line. The small size and careful markings warn that it is a measured medicine.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Medicine_Foxglove_Tincture_30ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPoppyLatexDraughtBottle() => new()
	{
		Metadata = new("medieval_drug_poppy_latex_draught_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3283", []),
		Noun = "bottle",
		ShortDescription = "a bottle of poppy-latex draught",
		LongDescription = null,
		FullDescription = "A small stoppered bottle holds a darkened draught with a bitter resin smell at the mouth. The wax seal is thick and slightly stained. Its compact form suggests careful dosing rather than casual drinking.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 190.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Poppy_Latex_Draught_100ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMintGingerTonicFlask() => new()
	{
		Metadata = new("medieval_drug_mint_ginger_tonic_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3312", []),
		Noun = "flask",
		ShortDescription = "a flask of mint-and-ginger tonic",
		LongDescription = null,
		FullDescription = "This small flask is filled with a warming herbal tonic and tied with a narrow green cord. The stopper smells of mint and ginger even before opening. Its leather covering is worn smooth by travel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mint_and_Ginger_Tonic_250ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugTheriacSyrupBottle() => new()
	{
		Metadata = new("medieval_drug_theriac_syrup_bottle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3341", []),
		Noun = "bottle",
		ShortDescription = "a bottle of theriac syrup",
		LongDescription = null,
		FullDescription = "A small glazed bottle is sealed with wax over a dark, thick syrup. Spice, honey, and resin smells gather around the neck. The bottle is decorated only by a dark thread and a careful apothecary knot.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 210.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glazed ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Theriac_Syrup_100ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPlainMedicineBottleSet() => new()
	{
		Metadata = new("medieval_drug_plain_medicine_bottle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3371", []),
		Noun = "set",
		ShortDescription = "a set of empty medicine bottles",
		LongDescription = null,
		FullDescription = "Three small ceramic bottles sit together in a padded linen wrap. Each bottle has a stopper, a cloth tie, and enough space for a simple mark or label. The set is empty and ready for an apothecary or infirmary shelf.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Bottle_100ml"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMandrakeSmokeCone() => new()
	{
		Metadata = new("medieval_drug_mandrake_smoke_cone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3398", []),
		Noun = "cone",
		ShortDescription = "a mandrake smoke cone",
		LongDescription = null,
		FullDescription = "A dark little cone of powdered root and resin is wrapped in thin paper. It smells earthy and bitter, with flecks of pale mandrake visible in the pressed surface. The cone is made for personal smoke rather than broad room fumigation.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 25.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mandrake",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Mandrake_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHenbaneSmokeRoll() => new()
	{
		Metadata = new("medieval_drug_henbane_smoke_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3427", []),
		Noun = "roll",
		ShortDescription = "a henbane smoke roll",
		LongDescription = null,
		FullDescription = "Dried henbane leaves are rolled into a narrow paper twist and tied at both ends. The roll is dark, brittle, and acrid-smelling even before it is lit. A black thread marks it apart from ordinary incense.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "henbane",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Henbane_Smoke"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugBronchialSmokePellets() => new()
	{
		Metadata = new("medieval_drug_bronchial_smoke_pellets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3456", []),
		Noun = "packet",
		ShortDescription = "a packet of bronchial smoke pellets",
		LongDescription = null,
		FullDescription = "Several small pellets of herb and resin sit in a waxed packet. They smell sharp, resinous, and green, with a little dust clinging to the paper. The pellets are meant to be smoked one at a time.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Bronchial_Smoke"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugSoporificSmokeCone() => new()
	{
		Metadata = new("medieval_drug_soporific_smoke_cone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3485", []),
		Noun = "cone",
		ShortDescription = "a soporific smoke cone",
		LongDescription = null,
		FullDescription = "This small black cone is pressed from bitter herbs and resin and wrapped alone in oiled paper. Its smell is heavy, sweet, and unsettling. A dark warning knot closes the packet.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Soporific_Fumes"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPlainFumigationPastilles() => new()
	{
		Metadata = new("medieval_drug_plain_fumigation_pastilles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3514", []),
		Noun = "packet",
		ShortDescription = "a packet of fumigation pastilles",
		LongDescription = null,
		FullDescription = "Small brown pastilles of resin, herb, and charcoal are folded in a linen packet. They have a dry incense smell and crumble slightly at the edges. The packet is made as fuel stock for a fumigation vessel, not as a medicine by itself.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugResinHerbFumigationPacket() => new()
	{
		Metadata = new("medieval_drug_resin_herb_fumigation_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3542", []),
		Noun = "packet",
		ShortDescription = "a resin-herb fumigation packet",
		LongDescription = null,
		FullDescription = "Chunks of resin and bitter herbs are mixed in a waxed paper packet. The pieces are irregular but dry, with charcoal dust darkening the fold. It is marked as fumigation stock rather than food or perfume.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "resin",
		Tags = [
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugMandrakeFumigationCenser() => new()
	{
		Metadata = new("medieval_drug_mandrake_fumigation_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3570", []),
		Noun = "censer",
		ShortDescription = "a bronze mandrake fumigation censer",
		LongDescription = null,
		FullDescription = "This small bronze censer has pierced vents and a darkened bowl for burning fumigation stock. Bitter earthy smoke stains the inner rim, and a short handle lets it be moved carefully. It is a drug-bearing fumigation vessel, not an ordinary incense ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Mandrake_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugHenbaneFumigationBowl() => new()
	{
		Metadata = new("medieval_drug_henbane_fumigation_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3599", []),
		Noun = "bowl",
		ShortDescription = "a ceramic henbane fumigation bowl",
		LongDescription = null,
		FullDescription = "A squat ceramic bowl is pierced around the upper wall and blackened inside. The outside is plain, but a black cord around the foot warns that it is not ordinary household incense gear. Its shape keeps the smoke low and close.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 780.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"IncenseBurner_Henbane_Smoke"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugBronchialFumigationCenser() => new()
	{
		Metadata = new("medieval_drug_bronchial_fumigation_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3628", []),
		Noun = "censer",
		ShortDescription = "a herbal bronchial fumigation censer",
		LongDescription = null,
		FullDescription = "This bronze censer is broad, low, and pierced with crescent vents around the lid. Old greenish smoke stains the seams where the lid lifts free. It is built for room fumigation with sharp herbal smoke.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Bronchial_Smoke"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugSoporificSickroomBrazier() => new()
	{
		Metadata = new("medieval_drug_soporific_sickroom_brazier", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3657", []),
		Noun = "brazier",
		ShortDescription = "a soporific sickroom brazier",
		LongDescription = null,
		FullDescription = "A small bronze brazier stands on three feet with a perforated cover over the bowl. Dark, sweet-smelling soot marks the vents, and a black thread is tied to the handle. It is meant for controlled sickroom fumigation rather than cheerful household scent.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Soporific_Fumes"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDrugPortableFumigationCasket() => new()
	{
		Metadata = new("medieval_drug_portable_fumigation_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3686", []),
		Noun = "casket",
		ShortDescription = "a portable fumigation casket",
		LongDescription = null,
		FullDescription = "This small ceramic casket has holes cut through the lid and a fire-darkened cup inside. The outer walls are wrapped with a thin leather carrying strap. It is compact enough for travel while still functioning as a fumigation vessel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"IncenseBurner_Mandrake_Draught"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalPaddedWillowSplints() => new()
	{
		Metadata = new("medieval_medical_padded_willow_splints", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3717", []),
		Noun = "splints",
		ShortDescription = "a pair of padded willow splints",
		LongDescription = null,
		FullDescription = "Two straight willow boards are smoothed, drilled for ties, and padded with folded linen. The pair is light but stiff enough to hold a forearm or shin in place. Old pressure marks show where cords have been knotted before.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalReedFingerSplints() => new()
	{
		Metadata = new("medieval_medical_reed_finger_splints", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3747", []),
		Noun = "splints",
		ShortDescription = "a bundle of reed finger splints",
		LongDescription = null,
		FullDescription = "Thin reed splints are bundled with narrow linen ties, each one trimmed for a finger or toe. The pieces are light, smooth, and slightly flexible. They look meant for small fractures rather than heavy limb binding.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalPaddedLegSplints() => new()
	{
		Metadata = new("medieval_medical_padded_leg_splints", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3777", []),
		Noun = "splints",
		ShortDescription = "a pair of padded leg splints",
		LongDescription = null,
		FullDescription = "Long wooden splints are lined with wool padding and tied with broad linen straps. They are longer and heavier than arm splints, with enough stiffness for a lower leg. The padding is replaceable but worn smooth in places.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLinenArmSling() => new()
	{
		Metadata = new("medieval_medical_linen_arm_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3807", []),
		Noun = "sling",
		ShortDescription = "a plain linen arm sling",
		LongDescription = null,
		FullDescription = "A broad triangle of linen is hemmed at the edges and fitted with two simple ties. The cloth is soft where it would cradle an arm against the body. It is plain enough for any infirmary or household chest.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLeatherShoulderSling() => new()
	{
		Metadata = new("medieval_medical_leather_shoulder_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3837", []),
		Noun = "sling",
		ShortDescription = "a leather shoulder support sling",
		LongDescription = null,
		FullDescription = "This sling uses soft leather straps and a linen cradle to hold an injured arm tight against the chest. The shoulder band is broad, spreading weight more comfortably than a thin cord. It is made for longer wear than a simple cloth triangle.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalBambooTravelSplints() => new()
	{
		Metadata = new("medieval_medical_bamboo_travel_splints", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3867", []),
		Noun = "splints",
		ShortDescription = "a set of bamboo travel splints",
		LongDescription = null,
		FullDescription = "Split bamboo lengths are smoothed and stacked inside a cloth sleeve. The pieces are light, stiff, and easy to carry on the road. Linen cords threaded through the sleeve keep the set together.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalForkedOakCrutch() => new()
	{
		Metadata = new("medieval_medical_forked_oak_crutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3897", []),
		Noun = "crutch",
		ShortDescription = "a forked oak walking crutch",
		LongDescription = null,
		FullDescription = "This crutch is cut from oak with a natural fork padded by wrapped wool. The shaft is smoothed by hand and slightly polished where it is gripped. It is sturdy but heavier than a simple cane.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalPaddedWillowCrutch() => new()
	{
		Metadata = new("medieval_medical_padded_willow_crutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3925", []),
		Noun = "crutch",
		ShortDescription = "a padded willow crutch",
		LongDescription = null,
		FullDescription = "A light willow crutch has a carved fork, a leather grip, and a linen pad tied across the top. The shaft flexes slightly but remains straight. It is easier to carry than oak and made for everyday convalescence.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalStaffCrutch() => new()
	{
		Metadata = new("medieval_medical_staff_crutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3953", []),
		Noun = "crutch",
		ShortDescription = "a staff-like healer’s crutch",
		LongDescription = null,
		FullDescription = "This straight wooden crutch looks almost like a walking staff, with a side grip and a padded upper brace. The end is capped in worn leather to keep it from splitting. It is simple, portable, and useful on uneven roads.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalCanvasFieldStretcher() => new()
	{
		Metadata = new("medieval_medical_canvas_field_stretcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:3981", []),
		Noun = "stretcher",
		ShortDescription = "a canvas field stretcher",
		LongDescription = null,
		FullDescription = "Two wooden poles run through a broad linen-canvas bed stitched at the edges. The handles are worn smooth, and the cloth sags slightly between them. It is made for carrying wounded people over streets, camps, or fields.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Stretcher"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoodenLitter() => new()
	{
		Metadata = new("medieval_medical_wooden_litter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4009", []),
		Noun = "litter",
		ShortDescription = "a simple wooden casualty litter",
		LongDescription = null,
		FullDescription = "This wooden litter has side rails, a slatted bed, and cord lashings to keep the frame from twisting. It is heavier than a canvas stretcher but steadier under a large patient. The rails show handwear from repeated carrying.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7600.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Stretcher"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalDragSling() => new()
	{
		Metadata = new("medieval_medical_drag_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4037", []),
		Noun = "sling",
		ShortDescription = "a casualty drag sling",
		LongDescription = null,
		FullDescription = "A broad leather-and-linen sling is fitted with long hauling straps and a reinforced cradle. It is meant to pull someone clear when four carriers are not available. The underside is scuffed from contact with ground and floors.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Sling"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoodenTravois() => new()
	{
		Metadata = new("medieval_medical_wooden_travois", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4065", []),
		Noun = "travois",
		ShortDescription = "a wooden casualty travois",
		LongDescription = null,
		FullDescription = "Two long wooden poles are lashed together with crosspieces and a net of hemp cord. The frame narrows at one end for pulling by hand or beast. It is rough, field-made, and useful across open ground.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Travois"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWinterCasualtySled() => new()
	{
		Metadata = new("medieval_medical_winter_casualty_sled", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4093", []),
		Noun = "sled",
		ShortDescription = "a low casualty sled",
		LongDescription = null,
		FullDescription = "This low sled has upturned wooden runners and a lashed linen bed between them. The front is fitted with a hauling rope, and the runners are polished by use. It is made for snow, mud, or smooth floors rather than stairs.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8200.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Sled"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoundedHarness() => new()
	{
		Metadata = new("medieval_medical_wounded_harness", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4121", []),
		Noun = "harness",
		ShortDescription = "a wounded-man hauling harness",
		LongDescription = null,
		FullDescription = "A set of broad leather straps is fitted with buckles, loops, and a chest cradle. It lets rescuers drag or support a wounded person without gripping clothing alone. The leather is stiff but well oiled.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Harness"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalWoodenPegFoot() => new()
	{
		Metadata = new("medieval_medical_wooden_peg_foot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4149", []),
		Noun = "foot",
		ShortDescription = "a simple wooden peg foot",
		LongDescription = null,
		FullDescription = "A carved wooden peg foot is shaped to strap below the leg with a leather cuff. The end is broad and worn flat from ground contact. It is functional in the simplest way, with no attempt to mimic toes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RFoot"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLeftLowerLegProsthesis() => new()
	{
		Metadata = new("medieval_medical_left_lower_leg_prosthesis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4178", []),
		Noun = "leg",
		ShortDescription = "a left wooden lower-leg prosthesis",
		LongDescription = null,
		FullDescription = "This left lower-leg prosthesis is carved from wood and fitted with leather straps at the top. The shaft narrows toward a blunt walking end. Padding inside the cuff has been replaced more than once.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_LLowerLeg"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalRightLowerLegProsthesis() => new()
	{
		Metadata = new("medieval_medical_right_lower_leg_prosthesis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4207", []),
		Noun = "leg",
		ShortDescription = "a right wooden lower-leg prosthesis",
		LongDescription = null,
		FullDescription = "This right lower-leg prosthesis combines a wooden shaft, leather cuff, and simple walking end. The fittings are practical rather than beautiful, with visible peg holes and cord repairs. It is meant for use, not display.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RLowerLeg"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLeftPlainHandProsthesis() => new()
	{
		Metadata = new("medieval_medical_left_plain_hand_prosthesis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4236", []),
		Noun = "hand",
		ShortDescription = "a plain left wooden hand",
		LongDescription = null,
		FullDescription = "A left wooden hand is carved into a fixed open shape and fitted with a leather wrist socket. The fingers do not move, but the palm is shaped enough to fill a sleeve or rest against an object. The surface is rubbed smooth but undecorated.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_LHand"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalRightHookHand() => new()
	{
		Metadata = new("medieval_medical_right_hook_hand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4265", []),
		Noun = "hook",
		ShortDescription = "a right bronze hook hand",
		LongDescription = null,
		FullDescription = "A bronze hook is mounted into a leather wrist cuff with buckled straps. The hook is blunt at the inside curve and polished bright where it would catch cord or handle. It is one of the more practical artificial hands in the catalogue.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Prosthetic_RHand_Functional"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalLeftGlassEye() => new()
	{
		Metadata = new("medieval_medical_left_glass_eye", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4294", []),
		Noun = "eye",
		ShortDescription = "a small left glass eye",
		LongDescription = null,
		FullDescription = "A small glass eye is shaped as a smooth convex piece with a dark painted centre. It is not lifelike in close inspection, but it has enough shine and colour to fill an empty socket under normal light. A padded scrap of cloth protects it.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Prosthetic_LEye"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMedicalRightOrnamentalFoot() => new()
	{
		Metadata = new("medieval_medical_right_ornamental_foot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalMedical.cs:4322", []),
		Noun = "foot",
		ShortDescription = "an ornamental right prosthetic foot",
		LongDescription = null,
		FullDescription = "This right prosthetic foot is carved from wood with a stylised instep and simple toe marks. The shape is more presentable than practical, with leather straps set for fitting to a lower-leg piece. It has been polished to a dark sheen.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 560.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
		Components = [
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RFoot_Ornamental"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairRoughClothMendingPouch() => new()
	{
		Metadata = new("medieval_repair_rough_cloth_mending_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:12", []),
		Noun = "pouch",
		ShortDescription = "a rough cloth-mending pouch",
		LongDescription = null,
		FullDescription = "This small pouch holds coarse thread, mismatched patches, bone needles, and a lump of beeswax. It is poor but useful for keeping clothing and cloth gear together. The outer pouch is itself patched in two places.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 320.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth_Poor",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairClothMendingRoll() => new()
	{
		Metadata = new("medieval_repair_cloth_mending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:40", []),
		Noun = "roll",
		ShortDescription = "a cloth-mending roll",
		LongDescription = null,
		FullDescription = "A linen roll opens to show thread, patches, needles, and small shears wrapped in ordered rows. The supplies are ordinary but clean and easy to find. It is suitable for household clothing, tents, and cloth packs.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairFineClothMendingCase() => new()
	{
		Metadata = new("medieval_repair_fine_cloth_mending_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:68", []),
		Noun = "case",
		ShortDescription = "a fine cloth-mending case",
		LongDescription = null,
		FullDescription = "This neat leather case holds dyed silk thread, fine linen patches, polished needles, and a small smoothing bone. The interior loops keep the tools from tangling. It is fit for better garments and household textiles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 560.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairRoughLeatherRepairPouch() => new()
	{
		Metadata = new("medieval_repair_rough_leather_repair_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:96", []),
		Noun = "pouch",
		ShortDescription = "a rough leather repair pouch",
		LongDescription = null,
		FullDescription = "This pouch carries heavy thonging, scrap leather, a blunt awl, and waxed thread. The tools are crude and scarred, but they are enough for a field repair. It smells of oil, hide, and dust.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 620.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather_Poor",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairLeatherRepairRoll() => new()
	{
		Metadata = new("medieval_repair_leather_repair_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:124", []),
		Noun = "roll",
		ShortDescription = "a leather repair roll",
		LongDescription = null,
		FullDescription = "A sturdy leather roll holds awls, thonging, waxed thread, spare patches, and a small burnisher. Each tool has a simple loop to keep it in place. It is a standard kit for belts, bags, shoes, and harness.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairSaddlerLeatherCase() => new()
	{
		Metadata = new("medieval_repair_saddler_leather_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:152", []),
		Noun = "case",
		ShortDescription = "a saddler’s leather repair case",
		LongDescription = null,
		FullDescription = "This better case is stocked with fine awls, strong waxed cord, dyed leather patches, and polished bone tools. The stitching is neat and the contents are chosen for more careful repair work. It suits a saddler or prosperous leatherworker.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1450.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairSimpleWoodRepairPouch() => new()
	{
		Metadata = new("medieval_repair_simple_wood_repair_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:180", []),
		Noun = "pouch",
		ShortDescription = "a simple wood-repair pouch",
		LongDescription = null,
		FullDescription = "Small pegs, wedges, glue scrap, and a blunt carving knife are kept in this rough pouch. The contents are cheap but useful for tightening loose wooden goods. Wood dust has settled into the seams.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 700.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood_Poor",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairWoodworkerRepairBox() => new()
	{
		Metadata = new("medieval_repair_woodworker_repair_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:208", []),
		Noun = "box",
		ShortDescription = "a woodworker’s repair box",
		LongDescription = null,
		FullDescription = "This small wooden box holds pegs, wedges, resin glue, clamps, cord, and a few small shaping tools. The lid is scarred by use as a work surface. It is meant for ordinary furniture, handles, and wooden gear.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairJoinerRepairChest() => new()
	{
		Metadata = new("medieval_repair_joiner_repair_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:236", []),
		Noun = "chest",
		ShortDescription = "a joiner’s fine repair chest",
		LongDescription = null,
		FullDescription = "A well-fitted oak chest contains matched pegs, wedges, glue cakes, clamps, chisels, and smoothing tools. The compartments are tidy and labelled by shape rather than writing. It is built for careful repair of valued wooden goods.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairPotMendersPouch() => new()
	{
		Metadata = new("medieval_repair_pot_menders_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:264", []),
		Noun = "pouch",
		ShortDescription = "a pot-mender’s ceramic repair pouch",
		LongDescription = null,
		FullDescription = "This pouch contains cord clamps, resin, spare sherds, and small smoothing stones for cracked pots. The contents are humble and dusty. It looks like something carried by a travelling mender rather than a wealthy workshop.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Ceramic",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairFineCeramicMendingCase() => new()
	{
		Metadata = new("medieval_repair_fine_ceramic_mending_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:292", []),
		Noun = "case",
		ShortDescription = "a fine ceramic mending case",
		LongDescription = null,
		FullDescription = "A padded case contains better resin, fine clamps, smooth sherd patches, and polishing cloths for glazed wares. The supplies are arranged to avoid scratching fragile surfaces. It is a careful kit for good household vessels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Ceramic_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairStoneMasonPatchKit() => new()
	{
		Metadata = new("medieval_repair_stone_mason_patch_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:320", []),
		Noun = "kit",
		ShortDescription = "a stone mason’s patch kit",
		LongDescription = null,
		FullDescription = "A heavy kit holds wedges, stone dust, resin, small chisels, and rough cloth for patching chipped stone goods. It is too weighty for delicate work but sturdy enough for everyday repairs. The pouch is powdered grey inside.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Stone",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairBoneHornMendingRoll() => new()
	{
		Metadata = new("medieval_repair_bone_horn_mending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:348", []),
		Noun = "roll",
		ShortDescription = "a bone-and-horn mending roll",
		LongDescription = null,
		FullDescription = "A cloth roll holds small drills, sinew, resin, horn shavings, and polished bone slips. It is meant for combs, handles, fittings, and other hard organic pieces. The smell is dry, sharp, and faintly gluey.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Hard_Organic",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairFineHardOrganicCase() => new()
	{
		Metadata = new("medieval_repair_fine_hard_organic_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:376", []),
		Noun = "case",
		ShortDescription = "a fine hard-organic repair case",
		LongDescription = null,
		FullDescription = "This neat case contains matched horn slivers, bone patches, fine cord, and smooth polishing tools. Each small piece is wrapped so it does not scratch the next. It is meant for valued horn, bone, shell, or antler goods.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Hard_Organic_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairBlacksmithMetalKit() => new()
	{
		Metadata = new("medieval_repair_blacksmith_metal_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:404", []),
		Noun = "kit",
		ShortDescription = "a blacksmith’s metal repair kit",
		LongDescription = null,
		FullDescription = "A heavy kit contains small clamps, rivets, wire, wedges, and dark repair stock for common metal goods. The pouch is scorched in places and leaves a soot smell on the hands. It is practical rather than portable elegance.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3100.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairMetalToolRepairRoll() => new()
	{
		Metadata = new("medieval_repair_metal_tool_repair_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:432", []),
		Noun = "roll",
		ShortDescription = "a metal tool repair roll",
		LongDescription = null,
		FullDescription = "This roll carries rivets, wedges, pins, a file, binding wire, and small grips for repairing metal tools. The leather is dark with oil and the file has its own protective flap. It is meant for tools, not weapons or armour.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / General Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Tool",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairFineMetalToolCase() => new()
	{
		Metadata = new("medieval_repair_fine_metal_tool_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:460", []),
		Noun = "case",
		ShortDescription = "a fine metal tool repair case",
		LongDescription = null,
		FullDescription = "A sturdy case holds better files, fitted rivets, small clamps, and polished metal pins. Everything sits in tight loops so the edges do not damage the leather. It is a workshop-quality kit for valued tools.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Tool_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairArmourersFieldKit() => new()
	{
		Metadata = new("medieval_repair_armourers_field_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:488", []),
		Noun = "kit",
		ShortDescription = "an armourer’s field repair kit",
		LongDescription = null,
		FullDescription = "This heavy kit holds rivets, leather lacing, wire, buckles, and a small hammer wrapped in cloth. It is designed for damaged armour on campaign. The outside bears dents and dark metal stains.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Armour",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairWeaponMendingRoll() => new()
	{
		Metadata = new("medieval_repair_weapon_mending_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:516", []),
		Noun = "roll",
		ShortDescription = "a weapon-mending repair roll",
		LongDescription = null,
		FullDescription = "A leather roll contains wedges, pins, grip cord, rivets, and small metal fittings for weapon repairs. The tools are compact and hard-worn. It is aimed at keeping field weapons usable, not forging new blades.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Weapon",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairFineWeaponMendingCase() => new()
	{
		Metadata = new("medieval_repair_fine_weapon_mending_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:544", []),
		Noun = "case",
		ShortDescription = "a fine weapon-mending case",
		LongDescription = null,
		FullDescription = "This fitted case holds fine rivets, polished pins, strong binding wire, grip leather, and small finishing tools. The supplies are carefully sorted and better made than a field kit. It belongs to a serious armoury or weapon shop.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Weapon_Good",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairGlassPoorLeadPatchPouch() => new()
	{
		Metadata = new("medieval_repair_glass_poor_lead_patch_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:572", []),
		Noun = "pouch",
		ShortDescription = "a rough glass patching pouch",
		LongDescription = null,
		FullDescription = "This poor pouch holds scrap lead came, resin, cloth pads, and a few cloudy glass chips. It is enough for crude patches on small panes or vessels. The contents clink softly through the padding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Poor,
		WeightInGrams = 850.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Glass_Poor",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRepairGlassMendersCase() => new()
	{
		Metadata = new("medieval_repair_glass_menders_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalRepairKits.cs:600", []),
		Noun = "case",
		ShortDescription = "a glass-mender’s repair case",
		LongDescription = null,
		FullDescription = "A padded case contains lead strips, resin, smooth cloth, spare glass pieces, and small setting tools. The compartments keep hard edges away from the glass. It is a standard kit for fragile vessels and panes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Repair_Glass",
				"Container_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
