#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalDoorSharedLockableGranaryDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_granary_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:823", []),
		Noun = "door",
		ShortDescription = "a lockable granary door",
		LongDescription = null,
		FullDescription = "This lockable granary door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 38000.0,
		Cost = 136.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Granary door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableHallDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_hall_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:852", []),
		Noun = "door",
		ShortDescription = "a lockable heavy hall door",
		LongDescription = null,
		FullDescription = "This lockable heavy hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 53500.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tough hall door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableInnChamberDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_inn_chamber_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:881", []),
		Noun = "door",
		ShortDescription = "a lockable inn chamber door",
		LongDescription = null,
		FullDescription = "This lockable inn chamber door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Inn chamber door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableIronboundDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_ironbound_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:910", []),
		Noun = "door",
		ShortDescription = "a lockable iron-bound door",
		LongDescription = null,
		FullDescription = "This lockable iron-bound door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 60500.0,
		Cost = 255.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Iron-bound door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableMillDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_mill_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:939", []),
		Noun = "door",
		ShortDescription = "a lockable millhouse door",
		LongDescription = null,
		FullDescription = "This lockable millhouse door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 39000.0,
		Cost = 132.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Millhouse or work-building door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockablePalisadeGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:968", []),
		Noun = "gate",
		ShortDescription = "a lockable palisade gate",
		LongDescription = null,
		FullDescription = "This lockable palisade gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 76000.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large palisade gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockablePantryDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_pantry_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:997", []),
		Noun = "door",
		ShortDescription = "a lockable pantry door",
		LongDescription = null,
		FullDescription = "This lockable pantry door is a large, workmanlike door built from ash boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25000.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pantry or buttery door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockablePlankHouseholdDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_plank_household_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1026", []),
		Noun = "door",
		ShortDescription = "a lockable plank household door",
		LongDescription = null,
		FullDescription = "This lockable plank household door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 39000.0,
		Cost = 132.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Household door with built-in lock behaviour for houses, shops, or private chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockablePortcullisGrate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_portcullis_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1055", []),
		Noun = "grate",
		ShortDescription = "a lockable portcullis-like grate",
		LongDescription = null,
		FullDescription = "This lockable portcullis-like grate is a huge, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 168000.0,
		Cost = 740.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_Huge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Huge lockable barred gate profile for strong entrances.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockablePosternGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_postern_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1084", []),
		Noun = "gate",
		ShortDescription = "a lockable postern gate",
		LongDescription = null,
		FullDescription = "This lockable postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 70000.0,
		Cost = 255.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Postern gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableShopDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_shop_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1113", []),
		Noun = "door",
		ShortDescription = "a lockable shopfront door",
		LongDescription = null,
		FullDescription = "This lockable shopfront door is a large, well-made door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 37000.0,
		Cost = 126.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shop or workshop door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableStableDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_stable_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1142", []),
		Noun = "door",
		ShortDescription = "a lockable stable door",
		LongDescription = null,
		FullDescription = "This lockable stable door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 41500.0,
		Cost = 142.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stable or byre door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableStoreRoomDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_store_room_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1171", []),
		Noun = "door",
		ShortDescription = "a lockable storeroom door",
		LongDescription = null,
		FullDescription = "This lockable storeroom door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 40000.0,
		Cost = 142.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Storeroom door with built-in locking behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableTowerStairDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_tower_stair_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1200", []),
		Noun = "door",
		ShortDescription = "a lockable tower stair door",
		LongDescription = null,
		FullDescription = "This lockable tower stair door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 48000.0,
		Cost = 185.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tower or stair door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableTownGateLeaf() => new()
	{
		Metadata = new("medieval_door_shared_lockable_town_gate_leaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1229", []),
		Noun = "gate",
		ShortDescription = "a lockable town-gate leaf",
		LongDescription = null,
		FullDescription = "This lockable town-gate leaf is a huge, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 142000.0,
		Cost = 540.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_Huge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Huge built-in-lock gate leaf for large entrances.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableWarehouseDoor() => new()
	{
		Metadata = new("medieval_door_shared_lockable_warehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1258", []),
		Noun = "door",
		ShortDescription = "a lockable warehouse door",
		LongDescription = null,
		FullDescription = "This lockable warehouse door is a very large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 67500.0,
		Cost = 225.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large warehouse door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableWicketGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_wicket_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1287", []),
		Noun = "gate",
		ShortDescription = "a lockable wicket gate",
		LongDescription = null,
		FullDescription = "This lockable wicket gate is a large, well-made gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 29000.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lockable gate for yards, gardens, and side entries.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedLockableYardGate() => new()
	{
		Metadata = new("medieval_door_shared_lockable_yard_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1316", []),
		Noun = "gate",
		ShortDescription = "a lockable yard gate",
		LongDescription = null,
		FullDescription = "This lockable yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 56000.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock yard gate with hinge-side security behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedMillDoor() => new()
	{
		Metadata = new("medieval_door_shared_mill_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1345", []),
		Noun = "door",
		ShortDescription = "a millhouse door",
		LongDescription = null,
		FullDescription = "This millhouse door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Working door for a millhouse, brewhouse, or comparable industrial building.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPalisadeGate() => new()
	{
		Metadata = new("medieval_door_shared_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1374", []),
		Noun = "gate",
		ShortDescription = "a palisade gate",
		LongDescription = null,
		FullDescription = "This palisade gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70000.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large timber gate for palisades, livestock yards, and enclosed compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPantryDoor() => new()
	{
		Metadata = new("medieval_door_shared_pantry_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1403", []),
		Noun = "door",
		ShortDescription = "a narrow pantry door",
		LongDescription = null,
		FullDescription = "This narrow pantry door is a large, workmanlike door built from ash boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 23000.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Practical household pantry or buttery door without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPlankHouseholdDoor() => new()
	{
		Metadata = new("medieval_door_shared_plank_household_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1432", []),
		Noun = "door",
		ShortDescription = "a plank household door",
		LongDescription = null,
		FullDescription = "This plank household door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36000.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Common exterior household door with board-and-batten construction and no built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPortcullisGrate() => new()
	{
		Metadata = new("medieval_door_shared_portcullis_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1461", []),
		Noun = "grate",
		ShortDescription = "a portcullis-like grate",
		LongDescription = null,
		FullDescription = "This portcullis-like grate is a huge, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 160000.0,
		Cost = 620.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Huge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Huge barred gate profile that can be seen and fired through.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPosternGate() => new()
	{
		Metadata = new("medieval_door_shared_postern_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1490", []),
		Noun = "gate",
		ShortDescription = "a postern gate",
		LongDescription = null,
		FullDescription = "This postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 65000.0,
		Cost = 175.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure small outer gate for side passages and wall openings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedPoultryYardGate() => new()
	{
		Metadata = new("medieval_door_shared_poultry_yard_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1519", []),
		Noun = "gate",
		ShortDescription = "a poultry yard gate",
		LongDescription = null,
		FullDescription = "This poultry yard gate is a large, plain gate built from willow boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Poor,
		WeightInGrams = 12000.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light weak gate for poultry yards and small garden enclosures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedSafeRoomDoor() => new()
	{
		Metadata = new("medieval_door_shared_safe_room_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1548", []),
		Noun = "door",
		ShortDescription = "a safe-room door",
		LongDescription = null,
		FullDescription = "This safe-room door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 70000.0,
		Cost = 310.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Very sturdy lockable door for treasuries, record rooms, and strong rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedShopDoor() => new()
	{
		Metadata = new("medieval_door_shared_shop_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1577", []),
		Noun = "door",
		ShortDescription = "a shopfront plank door",
		LongDescription = null,
		FullDescription = "This shopfront plank door is a large, workmanlike door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34000.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain shopfront or workshop door without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedSimpleCottageDoor() => new()
	{
		Metadata = new("medieval_door_shared_simple_cottage_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1606", []),
		Noun = "door",
		ShortDescription = "a simple cottage door",
		LongDescription = null,
		FullDescription = "This simple cottage door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain plank door for cottages, sheds, and modest outbuildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedStableDoor() => new()
	{
		Metadata = new("medieval_door_shared_stable_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1635", []),
		Noun = "door",
		ShortDescription = "a stable door",
		LongDescription = null,
		FullDescription = "This stable door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38000.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain stable or byre door without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedStairwellGrate() => new()
	{
		Metadata = new("medieval_door_shared_stairwell_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1664", []),
		Noun = "grate",
		ShortDescription = "a stairwell security grate",
		LongDescription = null,
		FullDescription = "This stairwell security grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 48000.0,
		Cost = 205.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure iron grate for stairwells, cellars, or narrow passages.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedStoreRoomDoor() => new()
	{
		Metadata = new("medieval_door_shared_store_room_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1693", []),
		Noun = "door",
		ShortDescription = "a storeroom door",
		LongDescription = null,
		FullDescription = "This storeroom door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 37000.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure storeroom door intended for pantries, workshops, and storage rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedTowerStairDoor() => new()
	{
		Metadata = new("medieval_door_shared_tower_stair_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1722", []),
		Noun = "door",
		ShortDescription = "a tower stair door",
		LongDescription = null,
		FullDescription = "This tower stair door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 45000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow secure door suitable for towers, stair passages, and defended inner rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedTownGateLeaf() => new()
	{
		Metadata = new("medieval_door_shared_town_gate_leaf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1751", []),
		Noun = "gate",
		ShortDescription = "a town-gate leaf",
		LongDescription = null,
		FullDescription = "This town-gate leaf is a huge, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Huge,
		Quality = ItemQuality.Good,
		WeightInGrams = 135000.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Huge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Huge tough gate leaf for town, castle, or compound entrances.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedTreasureRoomDoor() => new()
	{
		Metadata = new("medieval_door_shared_treasure_room_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1780", []),
		Noun = "door",
		ShortDescription = "a treasure-room door",
		LongDescription = null,
		FullDescription = "This treasure-room door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 76000.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy built-in-lock door for elite or institutional secure storage rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedWarehouseDoor() => new()
	{
		Metadata = new("medieval_door_shared_warehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1809", []),
		Noun = "door",
		ShortDescription = "a broad warehouse door",
		LongDescription = null,
		FullDescription = "This broad warehouse door is a very large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 62000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad movable warehouse door for stores, barns, and industrial buildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedWicketGate() => new()
	{
		Metadata = new("medieval_door_shared_wicket_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1838", []),
		Noun = "gate",
		ShortDescription = "a narrow wicket gate",
		LongDescription = null,
		FullDescription = "This narrow wicket gate is a large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pass-through gate for yards, gardens, and fenced closes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedWindowGrate() => new()
	{
		Metadata = new("medieval_door_shared_window_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1867", []),
		Noun = "grate",
		ShortDescription = "a barred window grate",
		LongDescription = null,
		FullDescription = "This barred window grate is a large, workmanlike grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable barred grate profile for large window or service openings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedWoolDoorwayHanging() => new()
	{
		Metadata = new("medieval_door_shared_wool_doorway_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1896", []),
		Noun = "hanging",
		ShortDescription = "a wool doorway hanging",
		LongDescription = null,
		FullDescription = "This wool doorway hanging is a large, workmanlike hanging made from woven wool. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft doorway barrier; weak door behaviour represents a non-secure hanging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSharedWroughtIronGrate() => new()
	{
		Metadata = new("medieval_door_shared_wrought_iron_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1925", []),
		Noun = "grate",
		ShortDescription = "a wrought iron barred grate",
		LongDescription = null,
		FullDescription = "This wrought iron barred grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 45000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Barred grate using gate behaviour so sight and ranged fire can pass through.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorReligiousChapelDoor() => new()
	{
		Metadata = new("medieval_door_religious_chapel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1954", []),
		Noun = "door",
		ShortDescription = "a chapel side door",
		LongDescription = null,
		FullDescription = "This chapel side door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 43000.0,
		Cost = 135.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy side door suitable for chapels, shrines, and small sanctuaries.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorReligiousLockableChapelDoor() => new()
	{
		Metadata = new("medieval_door_religious_lockable_chapel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:1983", []),
		Noun = "door",
		ShortDescription = "a lockable chapel side door",
		LongDescription = null,
		FullDescription = "This lockable chapel side door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 46000.0,
		Cost = 195.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Side door for a sanctuary or religious building with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorReligiousScreenGate() => new()
	{
		Metadata = new("medieval_door_religious_screen_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2012", []),
		Noun = "gate",
		ShortDescription = "a sanctuary screen gate",
		LongDescription = null,
		FullDescription = "This sanctuary screen gate is a large, well-made gate worked from wrought iron. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 40000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable open gate for separating sanctuary, choir, or inner precinct spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorReligiousVestryDoor() => new()
	{
		Metadata = new("medieval_door_religious_vestry_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2041", []),
		Noun = "door",
		ShortDescription = "a vestry store door",
		LongDescription = null,
		FullDescription = "This vestry store door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 41000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure built-in-lock door for vestment rooms, scripture storage, or treasury-adjacent rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernBoathouseDoor() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_boathouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2070", []),
		Noun = "door",
		ShortDescription = "a pine boathouse door",
		LongDescription = null,
		FullDescription = "This pine boathouse door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 54000.0,
		Cost = 118.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide timber door for boathouses and shore sheds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernForgeGrate() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_forge_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2099", []),
		Noun = "grate",
		ShortDescription = "a forge-yard iron grate",
		LongDescription = null,
		FullDescription = "This forge-yard iron grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 56000.0,
		Cost = 230.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rugged iron grate for forge yards and workshops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernFurDoorFlap() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_fur_door_flap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2128", []),
		Noun = "flap",
		ShortDescription = "a heavy fur door flap",
		LongDescription = null,
		FullDescription = "This heavy fur door flap is a large, workmanlike flap made from dressed fur. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cold-weather door flap with weak door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernLockablePalisadeGate() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_lockable_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2157", []),
		Noun = "gate",
		ShortDescription = "a lockable palisade gate",
		LongDescription = null,
		FullDescription = "This lockable palisade gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 84000.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large lockable palisade gate.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernLockableStaveDoor() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_lockable_stave_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2186", []),
		Noun = "door",
		ShortDescription = "a lockable stave-built door",
		LongDescription = null,
		FullDescription = "This lockable stave-built door is a large, well-made door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 124.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stave-built household door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernLonghouseHallDoor() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_longhouse_hall_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2215", []),
		Noun = "door",
		ShortDescription = "a longhouse hall door",
		LongDescription = null,
		FullDescription = "This longhouse hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 52000.0,
		Cost = 165.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy hall door for longhouses and large timber halls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernPalisadeGate() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_palisade_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2244", []),
		Noun = "gate",
		ShortDescription = "a spiked palisade gate",
		LongDescription = null,
		FullDescription = "This spiked palisade gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 78000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large timber gate for palisaded yards and enclosures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorNorthernNorthSeaNorthernStaveHouseDoor() => new()
	{
		Metadata = new("medieval_door_northern_north_sea_northern_stave_house_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2273", []),
		Noun = "door",
		ShortDescription = "a stave-built house door",
		LongDescription = null,
		FullDescription = "This stave-built house door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 33000.0,
		Cost = 74.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stave-built northern household door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternArchedOakDoor() => new()
	{
		Metadata = new("medieval_door_western_european_western_arched_oak_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2302", []),
		Noun = "door",
		ShortDescription = "an arched oak door",
		LongDescription = null,
		FullDescription = "This arched oak door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 47000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Arched plank door suitable for stone or timber-framed buildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternCastlePostern() => new()
	{
		Metadata = new("medieval_door_western_european_western_castle_postern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2331", []),
		Noun = "gate",
		ShortDescription = "a castle postern gate",
		LongDescription = null,
		FullDescription = "This castle postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 90000.0,
		Cost = 340.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy side gate suitable for fortified walls and closes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternChurchyardGrille() => new()
	{
		Metadata = new("medieval_door_western_european_western_churchyard_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2360", []),
		Noun = "grate",
		ShortDescription = "a churchyard iron grille",
		LongDescription = null,
		FullDescription = "This churchyard iron grille is a very large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 78000.0,
		Cost = 330.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large iron grille usable in churchyards, cloisters, or enclosed courts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternHalfTimberShopDoor() => new()
	{
		Metadata = new("medieval_door_western_european_western_half_timber_shop_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2389", []),
		Noun = "door",
		ShortDescription = "a half-timber shop door",
		LongDescription = null,
		FullDescription = "This half-timber shop door is a large, workmanlike door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "elm",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shop door for high-street, market, or craft-front buildings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternLockableArchedOakDoor() => new()
	{
		Metadata = new("medieval_door_western_european_western_lockable_arched_oak_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2418", []),
		Noun = "door",
		ShortDescription = "a lockable arched oak door",
		LongDescription = null,
		FullDescription = "This lockable arched oak door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50500.0,
		Cost = 205.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Arched oak door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternLockableManorGate() => new()
	{
		Metadata = new("medieval_door_western_european_western_lockable_manor_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2447", []),
		Noun = "gate",
		ShortDescription = "a lockable manorial gate",
		LongDescription = null,
		FullDescription = "This lockable manorial gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 82000.0,
		Cost = 310.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock gate for manorial yards and compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternManorGate() => new()
	{
		Metadata = new("medieval_door_western_european_western_manor_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2476", []),
		Noun = "gate",
		ShortDescription = "a manorial yard gate",
		LongDescription = null,
		FullDescription = "This manorial yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 76000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout manorial yard gate without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorWesternEuropeanWesternVaultDoor() => new()
	{
		Metadata = new("medieval_door_western_european_western_vault_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2505", []),
		Noun = "door",
		ShortDescription = "a stone-vault strong door",
		LongDescription = null,
		FullDescription = "This stone-vault strong door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 76000.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy lockable door for vaults, strongrooms, and record chambers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanBeadDoorHanging() => new()
	{
		Metadata = new("medieval_door_mediterranean_bead_door_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2534", []),
		Noun = "hanging",
		ShortDescription = "a shell-bead doorway hanging",
		LongDescription = null,
		FullDescription = "This shell-bead doorway hanging is a large, workmanlike hanging worked from bone. Strings of beads hang from a narrow header, leaving small gaps between each strand. The lower ends are uneven from movement through the passage. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Beaded doorway hanging for warm interiors and shaded rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanBronzeGrille() => new()
	{
		Metadata = new("medieval_door_mediterranean_bronze_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2563", []),
		Noun = "grate",
		ShortDescription = "a bronze barred grille",
		LongDescription = null,
		FullDescription = "This bronze barred grille is a large, well-made grate worked from bronze. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze grille for courtyards, store rooms, or service openings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanBronzeStuddedDoor() => new()
	{
		Metadata = new("medieval_door_mediterranean_bronze_studded_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2592", []),
		Noun = "door",
		ShortDescription = "a bronze-studded door",
		LongDescription = null,
		FullDescription = "This bronze-studded door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy cypress door with visible bronze studs and strapping.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanCypressCourtyardDoor() => new()
	{
		Metadata = new("medieval_door_mediterranean_cypress_courtyard_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2621", []),
		Noun = "door",
		ShortDescription = "a cypress courtyard door",
		LongDescription = null,
		FullDescription = "This cypress courtyard door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 39000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cypress-plank door for courtyards and warm-climate houses.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanLockableCypressDoor() => new()
	{
		Metadata = new("medieval_door_mediterranean_lockable_cypress_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2650", []),
		Noun = "door",
		ShortDescription = "a lockable cypress courtyard door",
		LongDescription = null,
		FullDescription = "This lockable cypress courtyard door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cypress courtyard door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanLockableVillaGate() => new()
	{
		Metadata = new("medieval_door_mediterranean_lockable_villa_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2679", []),
		Noun = "gate",
		ShortDescription = "a lockable villa gate",
		LongDescription = null,
		FullDescription = "This lockable villa gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 67500.0,
		Cost = 300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Courtyard gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanVillaGate() => new()
	{
		Metadata = new("medieval_door_mediterranean_villa_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2708", []),
		Noun = "gate",
		ShortDescription = "a villa courtyard gate",
		LongDescription = null,
		FullDescription = "This villa courtyard gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Courtyard gate for villa, bath, or urban compound settings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorMediterraneanWarehouseGate() => new()
	{
		Metadata = new("medieval_door_mediterranean_warehouse_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2737", []),
		Noun = "gate",
		ShortDescription = "a harbourside warehouse gate",
		LongDescription = null,
		FullDescription = "This harbourside warehouse gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 72000.0,
		Cost = 175.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large gate for harbourside stores and warehouse yards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateBazaarGrille() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_bazaar_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2766", []),
		Noun = "grate",
		ShortDescription = "a bazaar shop grille",
		LongDescription = null,
		FullDescription = "This bazaar shop grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 48000.0,
		Cost = 225.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable barred grille for market stalls and shops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateBronzeStuddedGate() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_bronze_studded_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2795", []),
		Noun = "gate",
		ShortDescription = "a bronze-studded compound gate",
		LongDescription = null,
		FullDescription = "This bronze-studded compound gate is a very large, well-made gate built from cedar boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 76000.0,
		Cost = 285.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large compound gate with bronze-studded visual treatment.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateCarvedCedarDoor() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_carved_cedar_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2824", []),
		Noun = "door",
		ShortDescription = "a carved cedar courtyard door",
		LongDescription = null,
		FullDescription = "This carved cedar courtyard door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 39000.0,
		Cost = 155.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved cedar door for urban houses, courtyards, and workshops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateCottonDoorCurtain() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_cotton_door_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2853", []),
		Noun = "curtain",
		ShortDescription = "a cotton doorway curtain",
		LongDescription = null,
		FullDescription = "This cotton doorway curtain is a large, workmanlike curtain made from woven cotton. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light cotton doorway barrier for interiors and screened passages.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateLatticeScreenDoor() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_lattice_screen_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2882", []),
		Noun = "screen",
		ShortDescription = "a carved lattice screen door",
		LongDescription = null,
		FullDescription = "This carved lattice screen door is a large, well-made screen built from cedar boards. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 19000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open lattice screen using gate behaviour so sight and missiles can pass through.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateLockableCedarDoor() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_lockable_cedar_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2911", []),
		Noun = "door",
		ShortDescription = "a lockable carved cedar door",
		LongDescription = null,
		FullDescription = "This lockable carved cedar door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 215.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved cedar door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateLockableCompoundGate() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_lockable_compound_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2940", []),
		Noun = "gate",
		ShortDescription = "a lockable compound gate",
		LongDescription = null,
		FullDescription = "This lockable compound gate is a very large, well-made gate built from cedar boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 82000.0,
		Cost = 380.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compound gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorIslamicateUrbanIslamicateStorehouseDoor() => new()
	{
		Metadata = new("medieval_door_islamicate_urban_islamicate_storehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2969", []),
		Noun = "door",
		ShortDescription = "a cedar storehouse door",
		LongDescription = null,
		FullDescription = "This cedar storehouse door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 43000.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure storehouse door without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianBambooScreenDoor() => new()
	{
		Metadata = new("medieval_door_south_asian_bamboo_screen_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:2998", []),
		Noun = "screen",
		ShortDescription = "a woven bamboo screen door",
		LongDescription = null,
		FullDescription = "This woven bamboo screen door is a large, workmanlike screen built from split bamboo. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6500.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light bamboo screen for shaded domestic passages.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianBazaarGrille() => new()
	{
		Metadata = new("medieval_door_south_asian_bazaar_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3027", []),
		Noun = "grate",
		ShortDescription = "an iron bazaar grille",
		LongDescription = null,
		FullDescription = "This iron bazaar grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50000.0,
		Cost = 225.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable open grille for bazaars, shops, and store counters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianCarvedTeakGate() => new()
	{
		Metadata = new("medieval_door_south_asian_carved_teak_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3056", []),
		Noun = "gate",
		ShortDescription = "a carved teak compound gate",
		LongDescription = null,
		FullDescription = "This carved teak compound gate is a very large, well-made gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 88000.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved teak compound gate without built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianCottonDoorCloth() => new()
	{
		Metadata = new("medieval_door_south_asian_cotton_door_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3085", []),
		Noun = "hanging",
		ShortDescription = "a cotton doorway cloth",
		LongDescription = null,
		FullDescription = "This cotton doorway cloth is a large, workmanlike hanging made from woven cotton. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple cloth doorway barrier for warm interiors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianLockableTeakDoor() => new()
	{
		Metadata = new("medieval_door_south_asian_lockable_teak_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3114", []),
		Noun = "door",
		ShortDescription = "a lockable teak courtyard door",
		LongDescription = null,
		FullDescription = "This lockable teak courtyard door is a large, well-made door built from teak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 48500.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Teak courtyard door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianLockableTeakGate() => new()
	{
		Metadata = new("medieval_door_south_asian_lockable_teak_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3143", []),
		Noun = "gate",
		ShortDescription = "a lockable carved teak gate",
		LongDescription = null,
		FullDescription = "This lockable carved teak gate is a very large, well-made gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 95000.0,
		Cost = 470.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved teak compound gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianStableGate() => new()
	{
		Metadata = new("medieval_door_south_asian_stable_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3172", []),
		Noun = "gate",
		ShortDescription = "a teak stable gate",
		LongDescription = null,
		FullDescription = "This teak stable gate is a very large, workmanlike gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 76000.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large teak gate for stables, compounds, or courtyards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSouthAsianTeakCourtyardDoor() => new()
	{
		Metadata = new("medieval_door_south_asian_teak_courtyard_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3201", []),
		Noun = "door",
		ShortDescription = "a teak courtyard door",
		LongDescription = null,
		FullDescription = "This teak courtyard door is a large, well-made door built from teak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 45000.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Strong teak door for courtyard houses and compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianBambooLatticeGate() => new()
	{
		Metadata = new("medieval_door_east_asian_bamboo_lattice_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3230", []),
		Noun = "gate",
		ShortDescription = "a bamboo lattice gate",
		LongDescription = null,
		FullDescription = "This bamboo lattice gate is a large, workmanlike gate built from split bamboo. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11000.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open bamboo lattice gate using see-and-fire-through gate behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianCompoundGate() => new()
	{
		Metadata = new("medieval_door_east_asian_compound_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3259", []),
		Noun = "gate",
		ShortDescription = "a timber compound gate",
		LongDescription = null,
		FullDescription = "This timber compound gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 78000.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large timber gate for compounds and yards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianIronGrille() => new()
	{
		Metadata = new("medieval_door_east_asian_iron_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3288", []),
		Noun = "grate",
		ShortDescription = "an iron lattice grille",
		LongDescription = null,
		FullDescription = "This iron lattice grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 205.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open iron grille using gate behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianLockablePanelDoor() => new()
	{
		Metadata = new("medieval_door_east_asian_lockable_panel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3317", []),
		Noun = "door",
		ShortDescription = "a lockable pine panel door",
		LongDescription = null,
		FullDescription = "This lockable pine panel door is a large, well-made door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Paneled pine door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianLockableStorehouseDoor() => new()
	{
		Metadata = new("medieval_door_east_asian_lockable_storehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3346", []),
		Noun = "door",
		ShortDescription = "a lockable storehouse door",
		LongDescription = null,
		FullDescription = "This lockable storehouse door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 50000.0,
		Cost = 215.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Storehouse door with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianPaperScreenDoor() => new()
	{
		Metadata = new("medieval_door_east_asian_paper_screen_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3375", []),
		Noun = "screen",
		ShortDescription = "a paper screen door",
		LongDescription = null,
		FullDescription = "This paper screen door is a large, workmanlike screen made from layered paper. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Delicate screen-door prototype with weak door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianPinePanelDoor() => new()
	{
		Metadata = new("medieval_door_east_asian_pine_panel_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3404", []),
		Noun = "door",
		ShortDescription = "a pine panel door",
		LongDescription = null,
		FullDescription = "This pine panel door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light paneled wooden door for domestic interiors and shops.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorEastAsianStorehouseDoor() => new()
	{
		Metadata = new("medieval_door_east_asian_storehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3433", []),
		Noun = "door",
		ShortDescription = "a heavy storehouse door",
		LongDescription = null,
		FullDescription = "This heavy storehouse door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 46000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Secure storehouse door for warehouses, compounds, or estate storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeCaravanseraiGate() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_caravanserai_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3462", []),
		Noun = "gate",
		ShortDescription = "a caravanserai yard gate",
		LongDescription = null,
		FullDescription = "This caravanserai yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 84000.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large secure gate for caravan yards and enclosed compounds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeCorralGate() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_corral_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3491", []),
		Noun = "gate",
		ShortDescription = "a horse-corral gate",
		LongDescription = null,
		FullDescription = "This horse-corral gate is a very large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 52000.0,
		Cost = 105.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad gate for horse corrals and animal yards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeFeltDoorFlap() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_felt_door_flap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3520", []),
		Noun = "flap",
		ShortDescription = "a felt tent door flap",
		LongDescription = null,
		FullDescription = "This felt tent door flap is a large, workmanlike flap made from pressed felt. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Felt door flap for tents, wagons, or temporary shelters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeIronBarrierGrate() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_iron_barrier_grate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3549", []),
		Noun = "grate",
		ShortDescription = "a wrought iron barrier grate",
		LongDescription = null,
		FullDescription = "This wrought iron barrier grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 52000.0,
		Cost = 230.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open iron barrier for stores, yards, and secure passages.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeLatticeDoorPanel() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_lattice_door_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3578", []),
		Noun = "screen",
		ShortDescription = "a wooden lattice door panel",
		LongDescription = null,
		FullDescription = "This wooden lattice door panel is a large, workmanlike screen built from ash boards. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 15000.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Gate_Normal_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light lattice barrier represented with gate behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeLeatherTentFlap() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_leather_tent_flap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3607", []),
		Noun = "flap",
		ShortDescription = "a leather tent door flap",
		LongDescription = null,
		FullDescription = "This leather tent door flap is a large, workmanlike flap made from worked leather. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3600.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Leather door flap for tents and portable camps.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeLockableCaravanseraiGate() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_lockable_caravanserai_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3636", []),
		Noun = "gate",
		ShortDescription = "a lockable caravanserai gate",
		LongDescription = null,
		FullDescription = "This lockable caravanserai gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Good,
		WeightInGrams = 90000.0,
		Cost = 380.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Caravanserai gate with built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDoorSteppeAndCaravanSteppeWagonGate() => new()
	{
		Metadata = new("medieval_door_steppe_and_caravan_steppe_wagon_gate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3665", []),
		Noun = "gate",
		ShortDescription = "a plank wagon-yard gate",
		LongDescription = null,
		FullDescription = "This plank wagon-yard gate is a very large, workmanlike gate built from pine boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
		Size = SizeCategory.VeryLarge,
		Quality = ItemQuality.Standard,
		WeightInGrams = 58000.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
		Components = [
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wide gate for wagon yards and caravan enclosures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedBarKeeperBrackets() => new()
	{
		Metadata = new("medieval_shared_bar_keeper_brackets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3694", []),
		Noun = "bracket",
		ShortDescription = "iron locking-bar brackets",
		LongDescription = null,
		FullDescription = "These iron locking-bar brackets are small, workmanlike bracket worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Inert brackets for a removable bar; they do not provide lock behaviour alone.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
