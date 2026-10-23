#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalMilitaryPlainWoodenCrossbow() => new()
	{
		Metadata = new("medieval_military_plain_wooden_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1791", []),
		Noun = "crossbow",
		ShortDescription = "a plain wooden crossbow",
		LongDescription = null,
		FullDescription = "This practical crossbow is built on a sturdy wooden tiller with a simple forward stirrup and a dark cord binding around the bow. The stock is undecorated, with a smooth channel cut for the bolt. It is made for guards, town watches, and infantry who need reliable power without expensive finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3100.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHornLathCrossbow() => new()
	{
		Metadata = new("medieval_military_horn_lath_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1811", []),
		Noun = "crossbow",
		ShortDescription = "a horn-lathed crossbow",
		LongDescription = null,
		FullDescription = "This crossbow has a polished wooden tiller and a layered horn lath bound tightly with sinew and cord. The trigger housing is neatly fitted, and the bolt groove is smooth and straight. It is compact for its strength, with the careful finish of a weapon made for professional service.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3350.0,
		Cost = 480.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHeavySiegeCrossbow() => new()
	{
		Metadata = new("medieval_military_heavy_siege_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1831", []),
		Noun = "crossbow",
		ShortDescription = "a heavy siege crossbow",
		LongDescription = null,
		FullDescription = "This heavy crossbow has a thick wooden tiller, broad bow assembly, and reinforced iron fittings at the front and trigger. The stock is long and weighty, built to withstand a powerful span and hard use from walls or prepared positions. It is not graceful, but it is formidable and deliberately overbuilt.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 640.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineSteelFittedCrossbow() => new()
	{
		Metadata = new("medieval_military_fine_steel_fitted_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1851", []),
		Noun = "crossbow",
		ShortDescription = "a fine steel-fitted crossbow",
		LongDescription = null,
		FullDescription = "This fine crossbow has a dark polished tiller, precise metal fittings, and a carefully seated bow assembly. The bolt channel is cleanly cut, and the trigger parts sit flush without visible looseness. It is a costly military weapon, finished for an officer, retainer, or wealthy marksman.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 3600.0,
		Cost = 900.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHuntingCrossbow() => new()
	{
		Metadata = new("medieval_military_hunting_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1871", []),
		Noun = "crossbow",
		ShortDescription = "a compact hunting crossbow",
		LongDescription = null,
		FullDescription = "This compact crossbow has a shorter wooden tiller and a neatly bound bowstave suitable for careful shooting at moderate range. Its fittings are simple but well aligned, and the stock is smooth from frequent handling. Though light enough for hunting, it remains a credible military weapon in close defences or ambushes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitarySmallLatchCrossbow() => new()
	{
		Metadata = new("medieval_military_small_latch_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1891", []),
		Noun = "crossbow",
		ShortDescription = "a small latch crossbow",
		LongDescription = null,
		FullDescription = "This small crossbow has a short tiller, a compact bow, and a simple latch mechanism set into the stock. The grip is shaped to be held close to the body, and the forward fittings are kept light. It sacrifices power for portability, making it a specialist sidearm rather than a battlefield mainstay.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Hand Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryCompactHandCrossbow() => new()
	{
		Metadata = new("medieval_military_compact_hand_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1911", []),
		Noun = "crossbow",
		ShortDescription = "a compact hand crossbow",
		LongDescription = null,
		FullDescription = "This compact hand crossbow has a polished wooden body, a short horn-faced bow, and clean metal trigger fittings. The bolt channel is shallow and precise, intended for small bolts at close range. It is carefully balanced for one-handed handling, though still visibly a light weapon.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 320.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Hand Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineHandCrossbow() => new()
	{
		Metadata = new("medieval_military_fine_hand_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1931", []),
		Noun = "crossbow",
		ShortDescription = "a fine hand crossbow",
		LongDescription = null,
		FullDescription = "This fine hand crossbow is built around a dark polished tiller with neat horn plates and small, well-fitted metal parts. Its compact bow is bound tightly and the grip is wrapped in smooth leather. The whole weapon is finished with unusual care, marking it as costly specialist gear.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 1200.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Crossbows" ],
		Components = [ "Holdable", "Hand Crossbow", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBraidedLinenSling() => new()
	{
		Metadata = new("medieval_military_braided_linen_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1951", []),
		Noun = "sling",
		ShortDescription = "a braided linen sling",
		LongDescription = null,
		FullDescription = "This sling is made from braided linen cords with a slightly wider cradle at the centre. The loop and release knot are plain, and the fibres have been rubbed smooth from use. It is cheap, light, and easily carried, the sort of weapon a herder, skirmisher, or poor levyman could bring to war.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Sling", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryLeatherPouchSling() => new()
	{
		Metadata = new("medieval_military_leather_pouch_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1971", []),
		Noun = "sling",
		ShortDescription = "a leather-pouch sling",
		LongDescription = null,
		FullDescription = "This sling has a small leather pouch stitched between two braided cords. The pouch is cupped to hold stones or lead bullets securely, while the release cord ends in a plain knot. It is simple but well made, suited to practiced skirmishers who value a dependable cast.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Sling", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryWoolBraidedSling() => new()
	{
		Metadata = new("medieval_military_wool_braided_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:1991", []),
		Noun = "sling",
		ShortDescription = "a wool-braided sling",
		LongDescription = null,
		FullDescription = "This sling is braided from coarse wool cord with a reinforced central cradle. The fibres are slightly fuzzy, but the braiding is tight and even enough for hard use. It is a humble weapon, easy to tuck into a belt or pouch when not in hand.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Sling", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineCordSling() => new()
	{
		Metadata = new("medieval_military_fine_cord_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2011", []),
		Noun = "sling",
		ShortDescription = "a fine cord sling",
		LongDescription = null,
		FullDescription = "This fine sling is braided from smooth hemp cord with a carefully shaped leather cradle. The loop sits neatly on the finger, and the release knot is small but easy to feel. It is still a simple weapon, but its balance and finish show an experienced slinger's preferences.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Sling", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryPlainStaffSling() => new()
	{
		Metadata = new("medieval_military_plain_staff_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2031", []),
		Noun = "staff sling",
		ShortDescription = "a plain staff sling",
		LongDescription = null,
		FullDescription = "This staff sling is made from an ash shaft with a short sling fixed near the end. The staff is long enough to give the cast leverage, and the sling pocket is made from rough leather. It is a practical missile weapon for defenders, ship crews, or light troops throwing heavier stones than a hand sling can easily manage.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Staff Sling", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHeavyOakStaffSling() => new()
	{
		Metadata = new("medieval_military_heavy_oak_staff_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2051", []),
		Noun = "staff sling",
		ShortDescription = "a heavy oak staff sling",
		LongDescription = null,
		FullDescription = "This heavy staff sling has a stout oak shaft, a thick leather pocket, and strong cordage at the head. The lower end is smoothed for a firm grip, while the upper fork is scarred from repeated casting. It is clumsy indoors but effective for hurling larger missiles from open ground or walls.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Staff Sling", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBambooStaffSling() => new()
	{
		Metadata = new("medieval_military_bamboo_staff_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2071", []),
		Noun = "staff sling",
		ShortDescription = "a bamboo staff sling",
		LongDescription = null,
		FullDescription = "This staff sling uses a long bamboo shaft with a narrow cord sling fastened at the end. The shaft is light, springy, and wrapped near the base to prevent the hand from slipping. Its construction favours a quick cast and easy carrying over sheer weight.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Other Weapons" ],
		Components = [ "Holdable", "Staff Sling", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryRoughFieldPointArrow() => new()
	{
		Metadata = new("medieval_military_rough_field_point_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2091", []),
		Noun = "arrow",
		ShortDescription = "a rough field-point arrow",
		LongDescription = null,
		FullDescription = "This arrow has a plain ash shaft, uneven fletching, and a small iron field point set without much polish. The shaft is straight enough to shoot, though the finish is crude. It is the sort of cheap arrow issued for practice, levy use, or rough campaign supply.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 32.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_FieldPointArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFieldPointArrow() => new()
	{
		Metadata = new("medieval_military_field_point_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2111", []),
		Noun = "arrow",
		ShortDescription = "a field-point arrow",
		LongDescription = null,
		FullDescription = "This arrow has a straight ash shaft, neat feather fletching, and a narrow field point fitted cleanly to the front. The shaft is smoothed and lightly sealed, with the nock cut square. It is a common utility arrow suitable for practice, hunting, and ordinary military shooting.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_FieldPointArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineFieldPointArrow() => new()
	{
		Metadata = new("medieval_military_fine_field_point_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2131", []),
		Noun = "arrow",
		ShortDescription = "a fine field-point arrow",
		LongDescription = null,
		FullDescription = "This fine arrow has a carefully matched birch shaft, clean fletching, and a polished field point seated without gaps. The nock is neatly reinforced and the whole shaft is smooth to the touch. It is made for dependable flight rather than mere bulk issue.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 33.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_FieldPointArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBroadheadArrow() => new()
	{
		Metadata = new("medieval_military_broadhead_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2151", []),
		Noun = "arrow",
		ShortDescription = "a broadhead arrow",
		LongDescription = null,
		FullDescription = "This arrow has a straight wooden shaft and a broad cutting head with sharpened wings. The fletching is tied down firmly, and the nock is cleanly cut. Its head is meant for cutting injury rather than practice shooting, giving it a more dangerous profile than a field point.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_BroadheadArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineBroadheadArrow() => new()
	{
		Metadata = new("medieval_military_fine_broadhead_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2171", []),
		Noun = "arrow",
		ShortDescription = "a fine broadhead arrow",
		LongDescription = null,
		FullDescription = "This fine broadhead arrow has a smooth birch shaft, close-set feathers, and a carefully sharpened cutting head. The head is seated squarely and bound below the socket to resist splitting. It is made for a well-equipped archer rather than hurried field issue.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 40.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_BroadheadArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBodkinArrow() => new()
	{
		Metadata = new("medieval_military_bodkin_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2191", []),
		Noun = "arrow",
		ShortDescription = "a bodkin arrow",
		LongDescription = null,
		FullDescription = "This arrow carries a narrow, angular bodkin head on a straight ash shaft. The fletching is trimmed low, and the point is made to drive forward without the wide cutting wings of a broadhead. It has a plain military finish suited to shooting at armoured or heavily clothed targets.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows", "Market / Military Goods / Ammunition / Arrows / Bodkin Arrows" ],
		Components = [ "Holdable", "Ammo_BodkinArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryTemperedBodkinArrow() => new()
	{
		Metadata = new("medieval_military_tempered_bodkin_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2211", []),
		Noun = "arrow",
		ShortDescription = "a tempered bodkin arrow",
		LongDescription = null,
		FullDescription = "This carefully made bodkin arrow has a narrow hardened point, a straight birch shaft, and tight, even fletching. The head is seated with a neat binding below the socket, and the shaft has been selected for stiffness. It is costly ammunition intended for serious war use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 39.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows", "Market / Military Goods / Ammunition / Arrows / Bodkin Arrows" ],
		Components = [ "Holdable", "Ammo_BodkinArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryTargetArrow() => new()
	{
		Metadata = new("medieval_military_target_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2231", []),
		Noun = "arrow",
		ShortDescription = "a target arrow",
		LongDescription = null,
		FullDescription = "This target arrow has a light wooden shaft, a small blunt-ended point, and simple, bright feather fletching. The head is made for marking and practice rather than deep injury. It is still straight and balanced enough for useful archery drill.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "poplar",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_TargetArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryPaddedTrainingArrow() => new()
	{
		Metadata = new("medieval_military_padded_training_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2251", []),
		Noun = "arrow",
		ShortDescription = "a padded training arrow",
		LongDescription = null,
		FullDescription = "This training arrow has a light shaft and a padded cloth-and-leather head bound around the point. The fletching is simple, and the padding is visibly meant to blunt the impact. It is suitable for controlled practice where ordinary sharp arrows would be too dangerous.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 48.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Arrows" ],
		Components = [ "Holdable", "Ammo_PaddedArrow", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryRoughFieldPointBolt() => new()
	{
		Metadata = new("medieval_military_rough_field_point_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2271", []),
		Noun = "bolt",
		ShortDescription = "a rough field-point bolt",
		LongDescription = null,
		FullDescription = "This crossbow bolt has a short wooden shaft, crude fletching, and a plain field point. The shaft is serviceable but not finely matched, with tool marks still visible along its length. It is cheap ammunition for practice, garrison stores, or hurried field supply.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 46.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts" ],
		Components = [ "Holdable", "Ammo_FieldPointBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFieldPointBolt() => new()
	{
		Metadata = new("medieval_military_field_point_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2291", []),
		Noun = "bolt",
		ShortDescription = "a field-point bolt",
		LongDescription = null,
		FullDescription = "This crossbow bolt has a straight ash shaft, low fletching, and a compact field point set cleanly at the front. Its short, stout build distinguishes it from an arrow. It is ordinary military ammunition, useful for practice, hunting, and general issue.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts" ],
		Components = [ "Holdable", "Ammo_FieldPointBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBroadheadBolt() => new()
	{
		Metadata = new("medieval_military_broadhead_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2311", []),
		Noun = "bolt",
		ShortDescription = "a broadhead bolt",
		LongDescription = null,
		FullDescription = "This bolt has a thick shaft and a broad cutting head fixed ahead of low fletching. The head flares wider than the shaft, making its purpose immediately visible. It is made for injury against exposed flesh or lightly protected targets rather than clean practice shooting.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 58.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts" ],
		Components = [ "Holdable", "Ammo_BroadheadBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBodkinBolt() => new()
	{
		Metadata = new("medieval_military_bodkin_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2331", []),
		Noun = "bolt",
		ShortDescription = "a bodkin bolt",
		LongDescription = null,
		FullDescription = "This crossbow bolt has a short, stout shaft and a narrow bodkin head with angular faces. The low fletching is tied firmly, and the shaft is heavy for its length. It is a plain war bolt, shaped for driving force rather than cutting breadth.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 56.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts", "Market / Military Goods / Ammunition / Bolts / Bodkin Bolts" ],
		Components = [ "Holdable", "Ammo_BodkinBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHardenedBodkinBolt() => new()
	{
		Metadata = new("medieval_military_hardened_bodkin_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2351", []),
		Noun = "bolt",
		ShortDescription = "a hardened bodkin bolt",
		LongDescription = null,
		FullDescription = "This well-made bodkin bolt has a dense birch shaft, compact fletching, and a hardened narrow head fitted squarely to the front. The point is cleanly forged and the shaft is selected for weight and straightness. It is expensive ammunition for a serious military crossbow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts", "Market / Military Goods / Ammunition / Bolts / Bodkin Bolts" ],
		Components = [ "Holdable", "Ammo_BodkinBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryTargetBolt() => new()
	{
		Metadata = new("medieval_military_target_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2371", []),
		Noun = "bolt",
		ShortDescription = "a target bolt",
		LongDescription = null,
		FullDescription = "This target bolt has a short wooden shaft, low fletching, and a small practice head. Its finish is plain but tidy, with enough straightness for repeated drill. The head is shaped for safer target use rather than battlefield injury.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 44.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "poplar",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts" ],
		Components = [ "Holdable", "Ammo_TargetBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryPaddedTrainingBolt() => new()
	{
		Metadata = new("medieval_military_padded_training_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2391", []),
		Noun = "bolt",
		ShortDescription = "a padded training bolt",
		LongDescription = null,
		FullDescription = "This training bolt has a stout shaft and a padded leather-and-cloth head bound over the front. The low fletching is simple, and the blunt head is visibly larger than a normal bolt point. It is intended for controlled crossbow drill where sharp bolts would be unacceptable.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bolts" ],
		Components = [ "Holdable", "Ammo_PaddedBolt", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitarySelectedSlingStone() => new()
	{
		Metadata = new("medieval_military_selected_sling_stone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2411", []),
		Noun = "bullet",
		ShortDescription = "a selected sling stone",
		LongDescription = null,
		FullDescription = "This sling stone is a smooth, rounded piece of dense stone chosen for its weight and shape. It fits neatly in the palm and has no sharp edges to catch in a sling pouch. Though humble, it is selected ammunition rather than a random pebble.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bullets" ],
		Components = [ "Holdable", "Ammo_SlingBullet", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryChippedSlingStone() => new()
	{
		Metadata = new("medieval_military_chipped_sling_stone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2431", []),
		Noun = "bullet",
		ShortDescription = "a chipped sling stone",
		LongDescription = null,
		FullDescription = "This sling stone has been roughly chipped into a more regular oval, leaving pale facets across its surface. It is heavier and more deliberate than a casual river stone. The shape gives it a consistent seat in a sling cradle while remaining cheap ammunition.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "flint",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bullets" ],
		Components = [ "Holdable", "Ammo_SlingBullet", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryLeadSlingBullet() => new()
	{
		Metadata = new("medieval_military_lead_sling_bullet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2451", []),
		Noun = "bullet",
		ShortDescription = "a lead sling bullet",
		LongDescription = null,
		FullDescription = "This sling bullet is cast from lead into a compact almond shape. Its surface is dull grey and slightly pitted from the mould, with enough weight in a small form to fly hard. It is purpose-made ammunition for a practiced slinger.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bullets" ],
		Components = [ "Holdable", "Ammo_LeadSlingBullet", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineCastLeadSlingBullet() => new()
	{
		Metadata = new("medieval_military_fine_cast_lead_sling_bullet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2471", []),
		Noun = "bullet",
		ShortDescription = "a fine cast lead sling bullet",
		LongDescription = null,
		FullDescription = "This lead sling bullet has a smooth, regular almond shape and a clean seam where the mould closed. It is compact, dense, and more carefully cast than ordinary bulk ammunition. The finish suggests a stock prepared for trained slingers rather than gathered in haste.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 62.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Ammunition", "Market / Military Goods / Ammunition", "Market / Military Goods / Ammunition / Bullets" ],
		Components = [ "Holdable", "Ammo_LeadSlingBullet", "Stack_Number", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryWornThrowingKnife() => new()
	{
		Metadata = new("medieval_military_worn_throwing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2491", []),
		Noun = "knife",
		ShortDescription = "a worn throwing knife",
		LongDescription = null,
		FullDescription = "This small throwing knife has a narrow wrought-iron blade and a bare tang wrapped in darkened cord. The point is still serviceable, though the edges are nicked from repeated impact. It is a cheap balanced weapon rather than a fine dagger.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Daggers" ],
		Components = [ "Holdable", "Throwing_Knife", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBalancedThrowingKnife() => new()
	{
		Metadata = new("medieval_military_balanced_throwing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2511", []),
		Noun = "knife",
		ShortDescription = "a balanced throwing knife",
		LongDescription = null,
		FullDescription = "This throwing knife has a straight carbon-steel blade, a short unguarded grip, and a simple flattened pommel to help its balance. It is spare and practical, with no hilt furniture to catch on release. The point is narrow and strong, built for repeated throws rather than ornament.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 210.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Daggers" ],
		Components = [ "Holdable", "Throwing_Knife", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBroadThrowingKnife() => new()
	{
		Metadata = new("medieval_military_broad_throwing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2531", []),
		Noun = "knife",
		ShortDescription = "a broad throwing knife",
		LongDescription = null,
		FullDescription = "This broad throwing knife has a leaf-shaped blade and a short wrapped grip balanced close to the hand. The blade is thicker through the centre than at the edges, giving it enough stiffness to survive rough use. It is plainly a missile weapon, not a table knife or delicate dagger.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Daggers" ],
		Components = [ "Holdable", "Throwing_Knife", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineSteelThrowingKnife() => new()
	{
		Metadata = new("medieval_military_fine_steel_throwing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2551", []),
		Noun = "knife",
		ShortDescription = "a fine steel throwing knife",
		LongDescription = null,
		FullDescription = "This fine throwing knife has a clean carbon-steel blade, even weight through the tang, and a close leather wrap that adds little bulk. The point is carefully ground and the edges are lightly sharpened without making the weapon fragile. Its balance shows skilled work rather than simple camp forging.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 220.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Daggers" ],
		Components = [ "Holdable", "Throwing_Knife", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryRoughThrowingHatchet() => new()
	{
		Metadata = new("medieval_military_rough_throwing_hatchet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2571", []),
		Noun = "axe",
		ShortDescription = "a rough throwing hatchet",
		LongDescription = null,
		FullDescription = "This small throwing axe has a short ash haft and a dark, uneven iron head. The edge is serviceable but roughly ground, and the haft shows dents from hard practice. Its balance is crude, but it remains light enough to throw and heavy enough to injure.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 620.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBalancedThrowingAxe() => new()
	{
		Metadata = new("medieval_military_balanced_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2591", []),
		Noun = "axe",
		ShortDescription = "a balanced throwing axe",
		LongDescription = null,
		FullDescription = "This throwing axe has a short ash haft, a compact carbon-steel head, and a slightly downturned beard below the edge. The haft is smoothed but undecorated, with just enough weight forward for a clean rotation. It is a practical skirmisher's weapon rather than a carpenter's tool.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBeardedThrowingAxe() => new()
	{
		Metadata = new("medieval_military_bearded_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2611", []),
		Noun = "axe",
		ShortDescription = "a bearded throwing axe",
		LongDescription = null,
		FullDescription = "This throwing axe has a broad bearded head set on a short wooden haft. The lower blade drops in a graceful curve, while the poll is kept narrow to save weight. It can serve in the hand, but its compact balance and shortened haft make its throwing purpose clear.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHeavyThrowingAxe() => new()
	{
		Metadata = new("medieval_military_heavy_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2631", []),
		Noun = "axe",
		ShortDescription = "a heavy throwing axe",
		LongDescription = null,
		FullDescription = "This heavy throwing axe has a stout haft and a thick wedge-like head with a broad cutting edge. It is short enough to throw but carries more mass than a light skirmisher's hatchet. The weapon looks punishing in close quarters and dangerous when hurled at short range.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1050.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineThrowingAxe() => new()
	{
		Metadata = new("medieval_military_fine_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2651", []),
		Noun = "axe",
		ShortDescription = "a fine throwing axe",
		LongDescription = null,
		FullDescription = "This fine throwing axe has a polished carbon-steel head, a carefully tapered ash haft, and a narrow leather grip. The edge is keen and the haft is weighted to bring the head around predictably. It is a costly weapon for a warrior who expects throwing axes to be more than expendable tools.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 780.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryLightRidersThrowingAxe() => new()
	{
		Metadata = new("medieval_military_light_riders_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2671", []),
		Noun = "axe",
		ShortDescription = "a light rider's throwing axe",
		LongDescription = null,
		FullDescription = "This light throwing axe has a slim haft, a narrow cutting head, and a compact profile that would not burden a mounted fighter. The grip is wrapped just below the head for control in close handling. It is made for quick use, whether carried at the belt or kept with other saddle weapons.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carbon steel",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Axes" ],
		Components = [ "Holdable", "Throwing_Axe", "Destroyable_Weapon", "Beltable" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryLightJavelin() => new()
	{
		Metadata = new("medieval_military_light_javelin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2691", []),
		Noun = "javelin",
		ShortDescription = "a light javelin",
		LongDescription = null,
		FullDescription = "This light javelin has an ash shaft, a narrow iron head, and a balanced grip point slightly behind the middle. The shaft is shorter and slimmer than a battlefield spear, making its throwing purpose obvious. It is a simple skirmishing weapon that can be carried in numbers.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryRoughJavelin() => new()
	{
		Metadata = new("medieval_military_rough_javelin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2711", []),
		Noun = "javelin",
		ShortDescription = "a rough javelin",
		LongDescription = null,
		FullDescription = "This rough javelin has a slightly crooked wooden shaft and a small wrought-iron head fixed with plain bindings. The point is sharp enough, but the balance is imperfect and the finish is crude. It is cheap levy or hunting gear pressed readily into war.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 900.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHeavyWarJavelin() => new()
	{
		Metadata = new("medieval_military_heavy_war_javelin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2731", []),
		Noun = "javelin",
		ShortDescription = "a heavy war javelin",
		LongDescription = null,
		FullDescription = "This heavy javelin has a stout ash shaft and a long, narrow steel head. The grip area is smoothed for a firm throw, and the shaft carries enough mass to hit hard at close range. It is closer to a light spear than a toy missile, meant for serious fighting.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1250.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBarbedThrowingSpear() => new()
	{
		Metadata = new("medieval_military_barbed_throwing_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2751", []),
		Noun = "javelin",
		ShortDescription = "a barbed throwing spear",
		LongDescription = null,
		FullDescription = "This throwing spear has a slim wooden shaft and a small barbed head with dark iron wings behind the point. The head is made to catch and tear rather than slip cleanly free. It is a harsh skirmishing weapon, especially dangerous against unarmoured troops and animals.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBambooWarDart() => new()
	{
		Metadata = new("medieval_military_bamboo_war_dart", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2771", []),
		Noun = "dart",
		ShortDescription = "a bamboo war dart",
		LongDescription = null,
		FullDescription = "This long war dart is made from a light bamboo shaft tipped with a narrow iron head. The rear is lightly fletched, and the grip area is marked by shallow wrapping. It is lighter than a javelin but still built as a battlefield missile rather than a toy or hunting reed.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryHorsemanThrowingSpear() => new()
	{
		Metadata = new("medieval_military_horseman_throwing_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2791", []),
		Noun = "javelin",
		ShortDescription = "a horseman's throwing spear",
		LongDescription = null,
		FullDescription = "This throwing spear has a straight ash shaft, a compact steel head, and a balance suited to quick casting from the hand. It is shorter than a thrusting spear and not as light as a dart, occupying the useful middle ground for mounted or mobile fighters. The finish is plain and military.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryFineBalancedJavelin() => new()
	{
		Metadata = new("medieval_military_fine_balanced_javelin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2811", []),
		Noun = "javelin",
		ShortDescription = "a fine balanced javelin",
		LongDescription = null,
		FullDescription = "This fine javelin has a straight selected ash shaft, a neatly fitted steel head, and a grip point subtly shaped into the wood. Its weight is even and deliberate, with no rough binding or warped section to spoil the throw. It is made for a skilled skirmisher rather than mass levy issue.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 880.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryShortThrowingSpear() => new()
	{
		Metadata = new("medieval_military_short_throwing_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2831", []),
		Noun = "spear",
		ShortDescription = "a short throwing spear",
		LongDescription = null,
		FullDescription = "This short throwing spear has a sturdy wooden shaft and a leaf-shaped head small enough to keep the weapon lively in the hand. It can serve briefly as a close weapon, but its balance favours casting before contact. The construction is plain, with strong bindings below the socket.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Spears" ],
		Components = [ "Holdable", "Throwing_Spear", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitaryBlackjackLeatherWrapped() => new()
	{
		Metadata = new("medieval_military_blackjack_leather_wrapped", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWeapons.cs:2851", []),
		Noun = "blackjack sap",
		ShortDescription = "a leather-covered blackjack",
		LongDescription = null,
		FullDescription = "A compact lead weight fills the rounded head of this short blackjack. Dark leather is stitched tightly around the weight and continues down into a narrow, flexible grip, with a small loop at its end. The smooth covering conceals the dense core beneath it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment / Military Weapons", "Market / Military Goods / Weapons", "Market / Military Goods / Weapons / Clubs" ],
		Components = [ "Holdable", "Melee_Blackjack", "Destroyable_Weapon" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlainParchmentSheet() => new()
	{
		Metadata = new("medieval_writing_plain_parchment_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:11", []),
		Noun = "sheet",
		ShortDescription = "a plain parchment sheet",
		LongDescription = null,
		FullDescription = "This plain parchment sheet is a plainly finished pale parchment leaf with a scraped, smoothed face and slightly darker edges. The worked skin shows faint grain and occasional knife or pumice marks when turned in the light. It is left blank for letters, accounts, charters, or later binding into a manuscript.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineParchmentSheet() => new()
	{
		Metadata = new("medieval_writing_fine_parchment_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:41", []),
		Noun = "sheet",
		ShortDescription = "a fine parchment sheet",
		LongDescription = null,
		FullDescription = "This fine parchment sheet is a carefully finished pale parchment leaf with a scraped, smoothed face and slightly darker edges. The worked skin shows faint grain and occasional knife or pumice marks when turned in the light. It is left blank for letters, accounts, charters, or later binding into a manuscript.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 6.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRuledParchmentSheet() => new()
	{
		Metadata = new("medieval_writing_ruled_parchment_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:71", []),
		Noun = "sheet",
		ShortDescription = "a ruled parchment sheet",
		LongDescription = null,
		FullDescription = "This ruled parchment sheet is a pale parchment leaf ruled with faint guide lines for an even hand. The ruling is light enough not to dominate the page, but clear enough to keep columns and margins disciplined. Its scraped surface has a slight tooth for ink and a tougher feel than paper.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScrapedParchmentBifolium() => new()
	{
		Metadata = new("medieval_writing_scraped_parchment_bifolium", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:101", []),
		Noun = "bifolium",
		ShortDescription = "a scraped parchment bifolium",
		LongDescription = null,
		FullDescription = "This scraped parchment bifolium is folded from a single sheet of pale scraped parchment, with a clean central crease and paired writing leaves. The surface is smooth but still shows faint grain, edge trimming, and the slight translucence of worked skin. It is prepared for compact manuscript work, account notes, or binding into a larger quire.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Bifolium_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineParchmentBifolium() => new()
	{
		Metadata = new("medieval_writing_fine_parchment_bifolium", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:131", []),
		Noun = "bifolium",
		ShortDescription = "a fine parchment bifolium",
		LongDescription = null,
		FullDescription = "This fine parchment bifolium is folded from a single sheet of pale scraped parchment, with a clean central crease and paired writing leaves. The surface is smooth but still shows faint grain, edge trimming, and the slight translucence of worked skin. It is prepared for compact manuscript work, account notes, or binding into a larger quire.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Bifolium_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBlankParchmentCharter() => new()
	{
		Metadata = new("medieval_writing_blank_parchment_charter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:161", []),
		Noun = "charter",
		ShortDescription = "a blank parchment charter",
		LongDescription = null,
		FullDescription = "This blank parchment charter is a broad prepared parchment leaf with squared edges and a clear open face for formal writing. The skin has been scraped thin and burnished, leaving a faintly mottled grain beneath the pale surface. There is enough lower margin for witness marks, folds, or a later seal attachment.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBlankParchmentWrit() => new()
	{
		Metadata = new("medieval_writing_blank_parchment_writ", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:191", []),
		Noun = "writ",
		ShortDescription = "a blank parchment writ",
		LongDescription = null,
		FullDescription = "This blank parchment writ is a trimmed parchment document blank with a firm, pale writing face. The edges are cut neat and the surface has been rubbed smooth so a quill can keep a narrow line. Its compact format suits orders, deeds, notices, and other short formal records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentDeedSheet() => new()
	{
		Metadata = new("medieval_writing_parchment_deed_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:221", []),
		Noun = "deed",
		ShortDescription = "a blank parchment deed",
		LongDescription = null,
		FullDescription = "This blank parchment deed is a trimmed parchment document blank with a firm, pale writing face. The edges are cut neat and the surface has been rubbed smooth so a quill can keep a narrow line. Its compact format suits orders, deeds, notices, and other short formal records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentLabelTag() => new()
	{
		Metadata = new("medieval_writing_parchment_label_tag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:251", []),
		Noun = "tag",
		ShortDescription = "a parchment label tag",
		LongDescription = null,
		FullDescription = "This parchment label tag is cut from pale parchment into a narrow durable tag with room for a short hand-written mark. A small pierced end or narrowed corner gives it a place for cord, thread, or attachment to a roll or packet. The surface is smoothed enough for ink while remaining tougher than ordinary paper.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlainRagPaperSheet() => new()
	{
		Metadata = new("medieval_writing_plain_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:281", []),
		Noun = "sheet",
		ShortDescription = "a plain rag-paper sheet",
		LongDescription = null,
		FullDescription = "This plain rag-paper sheet is a plain rag-paper leaf with a pale, lightly fibrous writing surface. Its edges are cut square and the sheet flexes softly rather than springing like parchment. It is ready for everyday letters, records, school exercises, or loose manuscript notes.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 1.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineRagPaperSheet() => new()
	{
		Metadata = new("medieval_writing_fine_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:311", []),
		Noun = "sheet",
		ShortDescription = "a fine rag-paper sheet",
		LongDescription = null,
		FullDescription = "This fine rag-paper sheet is a smooth rag-paper leaf with a clean pale surface and carefully trimmed edges. The sheet has enough body to resist tearing while remaining lighter and more flexible than parchment. It is suited to petitions, correspondence, copies, and other written work where a presentable page matters.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 4.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRuledRagPaperSheet() => new()
	{
		Metadata = new("medieval_writing_ruled_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:341", []),
		Noun = "sheet",
		ShortDescription = "a ruled rag-paper sheet",
		LongDescription = null,
		FullDescription = "This ruled rag-paper sheet is a rag-paper leaf with faint ruling lines laid across the writing face. The paper is light and slightly fibrous, with neat margins left around the working area. It is prepared for accounts, school exercises, or a copyist's orderly text.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.5,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFoldedRagPaperLetter() => new()
	{
		Metadata = new("medieval_writing_folded_rag_paper_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:371", []),
		Noun = "letter",
		ShortDescription = "a folded rag-paper letter",
		LongDescription = null,
		FullDescription = "This folded rag-paper letter is folded from rag paper into a compact document form with creases already set into the sheet. The paper is pale, lightly fibrous, and firm enough to take quill or reed-pen ink without immediately feathering. Its folds leave a natural outer face for an address, mark, or seal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagPaperAccountSlip() => new()
	{
		Metadata = new("medieval_writing_rag_paper_account_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:401", []),
		Noun = "slip",
		ShortDescription = "a rag-paper account slip",
		LongDescription = null,
		FullDescription = "This rag-paper account slip is a narrow strip of rag paper cut for short account entries or temporary notes. The surface is plain and pale, with a little fibre showing at the trimmed edges. It is small enough to tuck into a ledger, pouch, or bundle of records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagPaperPetitionLeaf() => new()
	{
		Metadata = new("medieval_writing_rag_paper_petition_leaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:431", []),
		Noun = "leaf",
		ShortDescription = "a petition paper leaf",
		LongDescription = null,
		FullDescription = "This petition paper leaf is a smooth rag-paper leaf with a clean pale surface and carefully trimmed edges. The sheet has enough body to resist tearing while remaining lighter and more flexible than parchment. It is suited to petitions, correspondence, copies, and other written work where a presentable page matters.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlainPapyrusSheet() => new()
	{
		Metadata = new("medieval_writing_plain_papyrus_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:461", []),
		Noun = "sheet",
		ShortDescription = "a plain papyrus sheet",
		LongDescription = null,
		FullDescription = "This plain papyrus sheet is made from pressed papyrus strips laid crosswise and burnished into a firm writing face. The pale surface shows fine plant fibres and a slight grid where the pith has been joined. It is ready for inked notes, accounts, letters, or attachment into a longer roll.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingMediterraneanPapyrusLetter() => new()
	{
		Metadata = new("medieval_writing_mediterranean_papyrus_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:491", []),
		Noun = "letter",
		ShortDescription = "a folded papyrus letter",
		LongDescription = null,
		FullDescription = "This folded papyrus letter is folded from pale papyrus, its outer face showing the crossing grain of pressed pith. The sheet is stiffer than rag paper and has a faint reedlike texture along the folds. It is meant for short correspondence, accounts, or compact Mediterranean-style record keeping.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 2.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPapyrusAccountSheet() => new()
	{
		Metadata = new("medieval_writing_papyrus_account_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:521", []),
		Noun = "sheet",
		ShortDescription = "a papyrus account sheet",
		LongDescription = null,
		FullDescription = "This papyrus account sheet is made from pressed papyrus strips laid crosswise and burnished into a firm writing face. The pale surface shows fine plant fibres and a slight grid where the pith has been joined. It is ready for inked notes, accounts, letters, or attachment into a longer roll.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.5,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianPaperSheet() => new()
	{
		Metadata = new("medieval_writing_east_asian_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:551", []),
		Noun = "sheet",
		ShortDescription = "an East Asian paper sheet",
		LongDescription = null,
		FullDescription = "This East Asian paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3.5,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineEastAsianPaperSheet() => new()
	{
		Metadata = new("medieval_writing_fine_east_asian_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:581", []),
		Noun = "sheet",
		ShortDescription = "a fine East Asian paper sheet",
		LongDescription = null,
		FullDescription = "This fine East Asian paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 3.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianCalligraphySheet() => new()
	{
		Metadata = new("medieval_writing_east_asian_calligraphy_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:611", []),
		Noun = "sheet",
		ShortDescription = "a calligraphy paper sheet",
		LongDescription = null,
		FullDescription = "This calligraphy paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 3.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianOfficialSlip() => new()
	{
		Metadata = new("medieval_writing_east_asian_official_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:641", []),
		Noun = "slip",
		ShortDescription = "an official paper slip",
		LongDescription = null,
		FullDescription = "This official paper slip is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2.5,
		Cost = 2.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBirchBarkLetter() => new()
	{
		Metadata = new("medieval_writing_birch_bark_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:671", []),
		Noun = "letter",
		ShortDescription = "a birch-bark letter",
		LongDescription = null,
		FullDescription = "This birch-bark letter is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBirchBarkAccountStrip() => new()
	{
		Metadata = new("medieval_writing_birch_bark_account_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:701", []),
		Noun = "strip",
		ShortDescription = "a birch-bark account strip",
		LongDescription = null,
		FullDescription = "This birch-bark account strip is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 0.8m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBirchBarkPracticeStrip() => new()
	{
		Metadata = new("medieval_writing_birch_bark_practice_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:731", []),
		Noun = "strip",
		ShortDescription = "a birch-bark practice strip",
		LongDescription = null,
		FullDescription = "This birch-bark practice strip is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 7.0,
		Cost = 0.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "birch",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPalmLeafStrip() => new()
	{
		Metadata = new("medieval_writing_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:761", []),
		Noun = "leaf",
		ShortDescription = "a palm-leaf manuscript strip",
		LongDescription = null,
		FullDescription = "This palm-leaf manuscript strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPiercedPalmLeafStrip() => new()
	{
		Metadata = new("medieval_writing_pierced_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:791", []),
		Noun = "leaf",
		ShortDescription = "a pierced palm-leaf strip",
		LongDescription = null,
		FullDescription = "This pierced palm-leaf strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 1.2m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingInkedPalmLeafStrip() => new()
	{
		Metadata = new("medieval_writing_inked_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:821", []),
		Noun = "leaf",
		ShortDescription = "an ink-ready palm-leaf strip",
		LongDescription = null,
		FullDescription = "This ink-ready palm-leaf strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 1.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooSlip() => new()
	{
		Metadata = new("medieval_writing_bamboo_slip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:851", []),
		Noun = "slip",
		ShortDescription = "a bamboo writing slip",
		LongDescription = null,
		FullDescription = "This bamboo writing slip is a slim bamboo strip planed smooth for writing or marking. The surface keeps a pale woody sheen and a slight lengthwise grain beneath the prepared face. It can take brush writing, inked labels, or shallow incised marks for record keeping.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooDocumentTag() => new()
	{
		Metadata = new("medieval_writing_bamboo_document_tag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:881", []),
		Noun = "tag",
		ShortDescription = "a bamboo document tag",
		LongDescription = null,
		FullDescription = "This bamboo document tag is a slim bamboo strip planed smooth for writing or marking. The surface keeps a pale woody sheen and a slight lengthwise grain beneath the prepared face. It can take brush writing, inked labels, or shallow incised marks for record keeping.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 0.8m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPotsherdOstracon() => new()
	{
		Metadata = new("medieval_writing_potsherd_ostracon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:911", []),
		Noun = "ostracon",
		ShortDescription = "a smoothed potsherd ostracon",
		LongDescription = null,
		FullDescription = "This smoothed potsherd ostracon is a broken ceramic writing piece selected for a flat, usable face. Its edges have been rubbed smoother while the writing surface remains hard, pale, and slightly uneven. It is useful for short notes, labels, accounts, or practice marks that do not need a full sheet.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 0.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fired clay",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Clay Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Ostracon_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlasteredOstracon() => new()
	{
		Metadata = new("medieval_writing_plastered_ostracon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:941", []),
		Noun = "ostracon",
		ShortDescription = "a pale plastered ostracon",
		LongDescription = null,
		FullDescription = "This pale plastered ostracon is a broken ceramic writing piece selected for a flat, usable face. Its edges have been rubbed smoother while the writing surface remains hard, pale, and slightly uneven. It is useful for short notes, labels, accounts, or practice marks that do not need a full sheet.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Clay Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Ostracon_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingNestedParchmentQuire() => new()
	{
		Metadata = new("medieval_writing_nested_parchment_quire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:971", []),
		Noun = "quire",
		ShortDescription = "a nested parchment quire",
		LongDescription = null,
		FullDescription = "This nested parchment quire is a nested gathering of blank leaves arranged for later sewing or binding. The folded edges sit together in a compact spine while the outer corners are trimmed into a tidy block. It is ready to become part of a codex, notebook, account book, or manuscript section.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagPaperQuire() => new()
	{
		Metadata = new("medieval_writing_rag_paper_quire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1000", []),
		Noun = "quire",
		ShortDescription = "a quire of rag-paper sheets",
		LongDescription = null,
		FullDescription = "This quire of rag-paper sheets is a nested gathering of blank leaves arranged for later sewing or binding. The folded edges sit together in a compact spine while the outer corners are trimmed into a tidy block. It is ready to become part of a codex, notebook, account book, or manuscript section.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooSlipBundle() => new()
	{
		Metadata = new("medieval_writing_bamboo_slip_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1029", []),
		Noun = "bundle",
		ShortDescription = "a bundle of bamboo slips",
		LongDescription = null,
		FullDescription = "This bundle of bamboo slips gathers several slim bamboo slips into an orderly stack. Each slip is straight, smooth, and narrow, with enough weight to hang or lie in a stable bundle. The group is useful for accounts, labels, records, or practice text that can be tied together afterward.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLooseScrollLabelTabs() => new()
	{
		Metadata = new("medieval_writing_loose_scroll_label_tabs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1058", []),
		Noun = "tabs",
		ShortDescription = "a bundle of scroll label tabs",
		LongDescription = null,
		FullDescription = "This bundle of scroll label tabs contains a stack of small blank labels cut from writing stock. The pieces are narrow and light, with enough surface for a title, seal note, tally mark, or short identifying hand. They are meant to be tied to scrolls, packets, bundles, or stored records.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
