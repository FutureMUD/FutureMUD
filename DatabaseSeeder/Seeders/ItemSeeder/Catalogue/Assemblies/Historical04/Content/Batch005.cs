#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical04Stock
{
	public static CatalogueItemDefinition MedievalSharedBarrelBitKey() => new()
	{
		Metadata = new("medieval_shared_barrel_bit_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3722", []),
		Noun = "key",
		ShortDescription = "a barrel-bit warded key",
		LongDescription = null,
		FullDescription = "This barrel-bit warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Key with a visibly thick bit and simple ward cuts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedBrassKeyring() => new()
	{
		Metadata = new("medieval_shared_brass_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3751", []),
		Noun = "keyring",
		ShortDescription = "a brass keyring",
		LongDescription = null,
		FullDescription = "This brass keyring is a tiny, well-made keyring worked from brass. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small keyring made in a finer visible material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedBroadCofferLock() => new()
	{
		Metadata = new("medieval_shared_broad_coffer_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3781", []),
		Noun = "lock",
		ShortDescription = "a broad coffer lock",
		LongDescription = null,
		FullDescription = "This broad coffer lock is a small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad lock face suitable for larger coffer lids and household storage chests.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedBronzeKeyring() => new()
	{
		Metadata = new("medieval_shared_bronze_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3810", []),
		Noun = "keyring",
		ShortDescription = "a bronze keyring",
		LongDescription = null,
		FullDescription = "This bronze keyring is a tiny, workmanlike keyring worked from bronze. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 105.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze keyring variant for household key bundles.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedBronzeWardedKey() => new()
	{
		Metadata = new("medieval_shared_bronze_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3840", []),
		Noun = "key",
		ShortDescription = "a bronze warded key",
		LongDescription = null,
		FullDescription = "This bronze warded key is a tiny, workmanlike key worked from bronze. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze key variant for households where bronze fittings are preferred.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCasketHookLatch() => new()
	{
		Metadata = new("medieval_shared_casket_hook_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3869", []),
		Noun = "latch",
		ShortDescription = "a small casket hook latch",
		LongDescription = null,
		FullDescription = "This small casket hook latch is a very small, workmanlike latch worked from brass. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small hook-and-eye latch for caskets, small cupboards, and light compartment doors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedChestHaspSet() => new()
	{
		Metadata = new("medieval_shared_chest_hasp_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3898", []),
		Noun = "hasp",
		ShortDescription = "an iron chest hasp latch",
		LongDescription = null,
		FullDescription = "This iron chest hasp latch is a very small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional hasp latch set for chests, coffers, trunks, and similar storage containers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCofferHaspLatch() => new()
	{
		Metadata = new("medieval_shared_coffer_hasp_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3927", []),
		Noun = "latch",
		ShortDescription = "a stout coffer hasp latch",
		LongDescription = null,
		FullDescription = "This stout coffer hasp latch is a small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 740.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stouter hasp latch for a coffer, strong chest, or merchant storage box.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCompactBoxLock() => new()
	{
		Metadata = new("medieval_shared_compact_box_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3956", []),
		Noun = "lock",
		ShortDescription = "a compact iron box lock",
		LongDescription = null,
		FullDescription = "This compact iron box lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact lock profile for small boxes, tool chests, or travelling cases.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCrudeCasketLock() => new()
	{
		Metadata = new("medieval_shared_crude_casket_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:3985", []),
		Noun = "lock",
		ShortDescription = "a crude little casket lock",
		LongDescription = null,
		FullDescription = "This crude little casket lock is a tiny, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 320.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Bad"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small low-grade warded lock for caskets, small boxes, or similar fitted storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCrudeOneSidedLatch() => new()
	{
		Metadata = new("medieval_shared_crude_one_sided_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4014", []),
		Noun = "latch",
		ShortDescription = "a crude one-sided latch",
		LongDescription = null,
		FullDescription = "This crude one-sided latch is a very small, plain latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 560.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Bad"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low-grade one-sided latch for simple closures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCupboardLatch() => new()
	{
		Metadata = new("medieval_shared_cupboard_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4043", []),
		Noun = "latch",
		ShortDescription = "a small cupboard latch",
		LongDescription = null,
		FullDescription = "This small cupboard latch is a very small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small hook-and-eye latch for cupboards, shutters, light hatches, and similar one-sided closures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedCupboardLock() => new()
	{
		Metadata = new("medieval_shared_cupboard_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4072", []),
		Noun = "lock",
		ShortDescription = "an iron cupboard lock",
		LongDescription = null,
		FullDescription = "This iron cupboard lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose lock intended for cupboard, hutch, or cabinet installation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedDoorHaspSet() => new()
	{
		Metadata = new("medieval_shared_door_hasp_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4101", []),
		Noun = "hasp",
		ShortDescription = "an iron door hasp set",
		LongDescription = null,
		FullDescription = "This iron door hasp set is a small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
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
		BuilderNotes = "Inert door hasp hardware intended to work with a separate loose lock.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedDoorLatch() => new()
	{
		Metadata = new("medieval_shared_door_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4129", []),
		Noun = "latch",
		ShortDescription = "an iron door latch",
		LongDescription = null,
		FullDescription = "This iron door latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy sliding bar latch for one-sided door closure.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedDrawboltLatch() => new()
	{
		Metadata = new("medieval_shared_drawbolt_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4158", []),
		Noun = "latch",
		ShortDescription = "an iron drawbolt latch",
		LongDescription = null,
		FullDescription = "This iron drawbolt latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 840.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Drawbolt-style one-sided door or shutter latch.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedDropBarLatch() => new()
	{
		Metadata = new("medieval_shared_drop_bar_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4187", []),
		Noun = "latch",
		ShortDescription = "a drop-bar iron latch",
		LongDescription = null,
		FullDescription = "This drop-bar iron latch is a small, well-made latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Weighty drop-bar latch for yard gates, stable gates, and palisade gates.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedExcellentBronzeWardedLock() => new()
	{
		Metadata = new("medieval_shared_excellent_bronze_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4216", []),
		Noun = "lock",
		ShortDescription = "an excellent bronze warded lock",
		LongDescription = null,
		FullDescription = "This excellent bronze warded lock is a very small, finely made lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 880.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Excellent"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "High-grade bronze lock for elite furniture, treasuries, or guarded storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedFineBrassWardedKey() => new()
	{
		Metadata = new("medieval_shared_fine_brass_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4245", []),
		Noun = "key",
		ShortDescription = "a fine brass warded key",
		LongDescription = null,
		FullDescription = "This fine brass warded key is a tiny, well-made key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine key form with cleaner shaping and higher-status material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedFineBrassWardedLock() => new()
	{
		Metadata = new("medieval_shared_fine_brass_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4274", []),
		Noun = "lock",
		ShortDescription = "a fine brass warded lock",
		LongDescription = null,
		FullDescription = "This fine brass warded lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine decorative lock whose behaviour remains a simple warded lock.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedFineChamberLatch() => new()
	{
		Metadata = new("medieval_shared_fine_chamber_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4303", []),
		Noun = "latch",
		ShortDescription = "a fine brass chamber latch",
		LongDescription = null,
		FullDescription = "This fine brass chamber latch is a very small, well-made latch worked from brass. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 480.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made one-sided chamber-door latch with bright brass presentation.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedFreshReplacementKey() => new()
	{
		Metadata = new("medieval_shared_fresh_replacement_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4332", []),
		Noun = "key",
		ShortDescription = "a fresh-cut replacement key",
		LongDescription = null,
		FullDescription = "This fresh-cut replacement key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Freshly shaped key blank cut into an ordinary warded key profile.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHatchHookLatch() => new()
	{
		Metadata = new("medieval_shared_hatch_hook_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4361", []),
		Noun = "latch",
		ShortDescription = "an iron hatch hook latch",
		LongDescription = null,
		FullDescription = "This iron hatch hook latch is a small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hook latch for a small hatch, shuttered service opening, or light container lid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHeavyGateKey() => new()
	{
		Metadata = new("medieval_shared_heavy_gate_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4390", []),
		Noun = "key",
		ShortDescription = "a heavy iron gate key",
		LongDescription = null,
		FullDescription = "This heavy iron gate key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large key form suitable for gates or exterior locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHeavyGateLock() => new()
	{
		Metadata = new("medieval_shared_heavy_gate_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4419", []),
		Noun = "lock",
		ShortDescription = "a heavy iron gate lock",
		LongDescription = null,
		FullDescription = "This heavy iron gate lock is a small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stouter loose warded lock suited to yard gates, pens, and exterior barriers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHeavyStewardKeyring() => new()
	{
		Metadata = new("medieval_shared_heavy_steward_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4448", []),
		Noun = "keyring",
		ShortDescription = "a heavy steward's keyring",
		LongDescription = null,
		FullDescription = "This heavy steward's keyring is a very small, well-made keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large, sturdy keyring for a steward, gatekeeper, storekeeper, or head servant.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHookLatch() => new()
	{
		Metadata = new("medieval_shared_hook_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4478", []),
		Noun = "latch",
		ShortDescription = "a hook-and-eye latch",
		LongDescription = null,
		FullDescription = "This hook-and-eye latch is a very small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small hook-and-eye latch for light lids, cupboard leaves, and small hatches.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHouseholdDoorKey() => new()
	{
		Metadata = new("medieval_shared_household_door_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4507", []),
		Noun = "key",
		ShortDescription = "a large household door key",
		LongDescription = null,
		FullDescription = "This large household door key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Larger key silhouette for ordinary household door locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedHouseholdDoorLock() => new()
	{
		Metadata = new("medieval_shared_household_door_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4536", []),
		Noun = "lock",
		ShortDescription = "an iron household door lock",
		LongDescription = null,
		FullDescription = "This iron household door lock is a small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose lock hardware for ordinary doors; built-in lockable doors use door components instead.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedIronHasp() => new()
	{
		Metadata = new("medieval_shared_iron_hasp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4565", []),
		Noun = "hasp",
		ShortDescription = "an iron hasp latch",
		LongDescription = null,
		FullDescription = "This iron hasp latch is a very small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional hasp latch for a chest, trunk, coffer, or other lockable container surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedIronHaspLock() => new()
	{
		Metadata = new("medieval_shared_iron_hasp_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4594", []),
		Noun = "lock",
		ShortDescription = "an iron hasp lock",
		LongDescription = null,
		FullDescription = "This iron hasp lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose lock intended to work with a visible hasp or staple fitting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedIronStrikePlate() => new()
	{
		Metadata = new("medieval_shared_iron_strike_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4623", []),
		Noun = "plate",
		ShortDescription = "an iron strike plate",
		LongDescription = null,
		FullDescription = "This iron strike plate is a very small, workmanlike plate worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 6.0m,
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
		BuilderNotes = "Passive receiving plate for a bolt, latch, or lock tongue.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedIronWardedPadlock() => new()
	{
		Metadata = new("medieval_shared_iron_warded_padlock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4651", []),
		Noun = "lock",
		ShortDescription = "an iron warded padlock",
		LongDescription = null,
		FullDescription = "This iron warded padlock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Padlock-like warded lock for hasps, chains, and temporary closures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedKeyholeEscutcheon() => new()
	{
		Metadata = new("medieval_shared_keyhole_escutcheon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4680", []),
		Noun = "escutcheon",
		ShortDescription = "a keyhole escutcheon plate",
		LongDescription = null,
		FullDescription = "This keyhole escutcheon plate is a tiny, workmanlike escutcheon worked from brass. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small protective keyhole surround for furniture or door hardware.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLargeKeeperKeyring() => new()
	{
		Metadata = new("medieval_shared_large_keeper_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4708", []),
		Noun = "keyring",
		ShortDescription = "a large keeper's keyring",
		LongDescription = null,
		FullDescription = "This large keeper's keyring is a very small, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large keyring component for many household, yard, or storehouse keys.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLargeKeepersKey() => new()
	{
		Metadata = new("medieval_shared_large_keepers_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4738", []),
		Noun = "key",
		ShortDescription = "a large keeper's key",
		LongDescription = null,
		FullDescription = "This large keeper's key is a very small, well-made key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 125.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large key silhouette for a custodian, steward, or household keeper.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLinkedKeyChain() => new()
	{
		Metadata = new("medieval_shared_linked_key_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4767", []),
		Noun = "keyring",
		ShortDescription = "an iron linked key chain",
		LongDescription = null,
		FullDescription = "This iron linked key chain is a very small, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Chain-like keyring form using the large keyring component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLockPlate() => new()
	{
		Metadata = new("medieval_shared_lock_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4797", []),
		Noun = "plate",
		ShortDescription = "an iron lock plate",
		LongDescription = null,
		FullDescription = "This iron lock plate is a very small, workmanlike plate worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 9.0m,
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
		BuilderNotes = "Decorative or protective lock plate; no locking behaviour by itself.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLockStraps() => new()
	{
		Metadata = new("medieval_shared_lock_straps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4825", []),
		Noun = "strap",
		ShortDescription = "a pair of iron lock straps",
		LongDescription = null,
		FullDescription = "This pair of iron lock straps is very small, workmanlike strap worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 12.0m,
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
		BuilderNotes = "Reinforcing straps for lock plates, chests, and doors; no independent lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLongWardedKey() => new()
	{
		Metadata = new("medieval_shared_long_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4853", []),
		Noun = "key",
		ShortDescription = "a long iron warded key",
		LongDescription = null,
		FullDescription = "This long iron warded key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Longer key form for deeper lock plates, large chests, or door hardware.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedLoopBowBrassKey() => new()
	{
		Metadata = new("medieval_shared_loop_bow_brass_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4882", []),
		Noun = "key",
		ShortDescription = "a loop-bowed brass key",
		LongDescription = null,
		FullDescription = "This loop-bowed brass key is a tiny, workmanlike key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Brass key variant with a broad loop bow; behaviour remains a warded key.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedMasterCutWardedLock() => new()
	{
		Metadata = new("medieval_shared_master_cut_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4911", []),
		Noun = "lock",
		ShortDescription = "a master-cut warded lock",
		LongDescription = null,
		FullDescription = "This master-cut warded lock is a very small, excellent lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 950.0,
		Cost = 200.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Master"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Exceptional loose warded lock for high-security or prestige use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedPalisadeGateDropbar() => new()
	{
		Metadata = new("medieval_shared_palisade_gate_dropbar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4940", []),
		Noun = "latch",
		ShortDescription = "a palisade gate drop-bar",
		LongDescription = null,
		FullDescription = "This palisade gate drop-bar is a large, workmanlike latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Weighty drop-bar latch for palisade gates, yard gates, and fortified outer barriers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedPlainWardedKey() => new()
	{
		Metadata = new("medieval_shared_plain_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4969", []),
		Noun = "key",
		ShortDescription = "a plain iron warded key",
		LongDescription = null,
		FullDescription = "This plain iron warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General-purpose key for the warded mechanical lock family.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedPlainWardedLock() => new()
	{
		Metadata = new("medieval_shared_plain_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:4998", []),
		Noun = "lock",
		ShortDescription = "a plain iron warded lock",
		LongDescription = null,
		FullDescription = "This plain iron warded lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General-purpose pre-modern warded lock for household use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedPortcullisPawl() => new()
	{
		Metadata = new("medieval_shared_portcullis_pawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5027", []),
		Noun = "pawl",
		ShortDescription = "a portcullis winch pawl",
		LongDescription = null,
		FullDescription = "This portcullis winch pawl is a medium-sized, well-made pawl worked from wrought iron. A toothed pawl sits on a pivot plate, with a short handle for lifting it clear. The striking edge is chipped from repeated catching. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Portcullis_Pawl"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Winch pawl or brake latch for holding a portcullis, grate, or similar heavy barrier in place.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedReinforcedIronLatch() => new()
	{
		Metadata = new("medieval_shared_reinforced_iron_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5056", []),
		Noun = "latch",
		ShortDescription = "a reinforced iron latch",
		LongDescription = null,
		FullDescription = "This reinforced iron latch is a small, finely made latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 1500.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Excellent"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "High-grade one-sided latch with heavier plates and reinforcement.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedRepairedWardedLock() => new()
	{
		Metadata = new("medieval_shared_repaired_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5085", []),
		Noun = "lock",
		ShortDescription = "a visibly repaired warded lock",
		LongDescription = null,
		FullDescription = "This visibly repaired warded lock is a very small, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 880.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Bad"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Serviceable but rough lock with visible patched plates and replacement rivets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedRingBowKey() => new()
	{
		Metadata = new("medieval_shared_ring_bow_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5114", []),
		Noun = "key",
		ShortDescription = "a ring-bowed iron key",
		LongDescription = null,
		FullDescription = "This ring-bowed iron key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Key with a simple ring-shaped bow for tying, hanging, or carrying on a keyring.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedRustSpottedWardedLock() => new()
	{
		Metadata = new("medieval_shared_rust_spotted_warded_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5143", []),
		Noun = "lock",
		ShortDescription = "a rust-spotted warded lock",
		LongDescription = null,
		FullDescription = "This rust-spotted warded lock is a very small, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 820.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Terrible"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose warded lock in visibly poor condition; not tied to any particular door or container.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedRustedOneSidedLatch() => new()
	{
		Metadata = new("medieval_shared_rusted_one_sided_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5172", []),
		Noun = "latch",
		ShortDescription = "a rusted one-sided latch",
		LongDescription = null,
		FullDescription = "This rusted one-sided latch is a very small, plain latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 520.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Terrible"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough one-sided latch in poor condition.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedShopShutterBarLatch() => new()
	{
		Metadata = new("medieval_shared_shop_shutter_bar_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5201", []),
		Noun = "latch",
		ShortDescription = "a shop-shutter bar latch",
		LongDescription = null,
		FullDescription = "This shop-shutter bar latch is a medium-sized, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy sliding bar latch for shop shutters, booth doors, and other broad hinged panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedShopShutterLock() => new()
	{
		Metadata = new("medieval_shared_shop_shutter_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5230", []),
		Noun = "lock",
		ShortDescription = "a shop-shutter warded lock",
		LongDescription = null,
		FullDescription = "This shop-shutter warded lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 25.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Loose lock for shutters, stalls, booths, and small shopfront closures.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedShutterLatch() => new()
	{
		Metadata = new("medieval_shared_shutter_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5259", []),
		Noun = "latch",
		ShortDescription = "an iron shutter latch",
		LongDescription = null,
		FullDescription = "This iron shutter latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sliding bar latch for interior shutters and door-like panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSimpleIronLatch() => new()
	{
		Metadata = new("medieval_shared_simple_iron_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5288", []),
		Noun = "latch",
		ShortDescription = "a simple iron latch",
		LongDescription = null,
		FullDescription = "This simple iron latch is a very small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ordinary one-sided latch for doors, cupboards, and shutters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSimpleKeyHoop() => new()
	{
		Metadata = new("medieval_shared_simple_key_hoop", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5317", []),
		Noun = "keyring",
		ShortDescription = "a simple iron key hoop",
		LongDescription = null,
		FullDescription = "This simple iron key hoop is a tiny, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain hoop-style keyring for a small set of keys.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSlidingBoltLatch() => new()
	{
		Metadata = new("medieval_shared_sliding_bolt_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5347", []),
		Noun = "latch",
		ShortDescription = "a sliding bolt latch",
		LongDescription = null,
		FullDescription = "This sliding bolt latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sliding bolt latch for doors, shutters, and internal barriers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSmallChamberLock() => new()
	{
		Metadata = new("medieval_shared_small_chamber_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5376", []),
		Noun = "lock",
		ShortDescription = "a small brass chamber lock",
		LongDescription = null,
		FullDescription = "This small brass chamber lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cleaner, finer lock suited to private chambers, cabinets, or elite household boxes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSmallChestKey() => new()
	{
		Metadata = new("medieval_shared_small_chest_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5405", []),
		Noun = "key",
		ShortDescription = "a small iron chest key",
		LongDescription = null,
		FullDescription = "This small iron chest key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ordinary key profile for chest and coffer locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSmallChestLock() => new()
	{
		Metadata = new("medieval_shared_small_chest_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5434", []),
		Noun = "lock",
		ShortDescription = "a small iron chest lock",
		LongDescription = null,
		FullDescription = "This small iron chest lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Ordinary loose warded lock sized for a chest or compact coffer.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedSmallIronKeyring() => new()
	{
		Metadata = new("medieval_shared_small_iron_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5463", []),
		Noun = "keyring",
		ShortDescription = "a small iron keyring",
		LongDescription = null,
		FullDescription = "This small iron keyring is a tiny, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small keyring component for a handful of keys.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedStableDoorLatch() => new()
	{
		Metadata = new("medieval_shared_stable_door_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5493", []),
		Noun = "latch",
		ShortDescription = "a stable-door iron latch",
		LongDescription = null,
		FullDescription = "This stable-door iron latch is a small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy one-sided sliding bar latch for stable doors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedStaplePlate() => new()
	{
		Metadata = new("medieval_shared_staple_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5522", []),
		Noun = "staple",
		ShortDescription = "an iron staple plate",
		LongDescription = null,
		FullDescription = "This iron staple plate is a very small, workmanlike staple worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 6.0m,
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
		BuilderNotes = "Inert staple plate or catch for a hasp, chain, or lock fitting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedStorehouseKey() => new()
	{
		Metadata = new("medieval_shared_storehouse_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5550", []),
		Noun = "key",
		ShortDescription = "a stout storehouse key",
		LongDescription = null,
		FullDescription = "This stout storehouse key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 125.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout warded key for storeroom, granary, or warehouse locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedStorehouseLock() => new()
	{
		Metadata = new("medieval_shared_storehouse_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5579", []),
		Noun = "lock",
		ShortDescription = "a stout storehouse lock",
		LongDescription = null,
		FullDescription = "This stout storehouse lock is a small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1900.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy lock profile for storerooms, granaries, warehouses, or strong cupboards.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedStrongboxLock() => new()
	{
		Metadata = new("medieval_shared_strongbox_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5608", []),
		Noun = "lock",
		ShortDescription = "a good iron strongbox lock",
		LongDescription = null,
		FullDescription = "This good iron strongbox lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made loose lock for a strongbox or money chest.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedTagHoledWardedKey() => new()
	{
		Metadata = new("medieval_shared_tag_holed_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5637", []),
		Noun = "key",
		ShortDescription = "a tag-holed warded key",
		LongDescription = null,
		FullDescription = "This tag-holed warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Key with a pierced bow for tying on a tag, cord, or keyring; no writing is assumed.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedTinyCasketKey() => new()
	{
		Metadata = new("medieval_shared_tiny_casket_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5666", []),
		Noun = "key",
		ShortDescription = "a tiny warded casket key",
		LongDescription = null,
		FullDescription = "This tiny warded casket key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small key form for caskets and miniature locks; mechanical key family remains warded.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedTrunkHaspLatch() => new()
	{
		Metadata = new("medieval_shared_trunk_hasp_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5695", []),
		Noun = "latch",
		ShortDescription = "an iron trunk hasp latch",
		LongDescription = null,
		FullDescription = "This iron trunk hasp latch is a small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional hasp latch sized for trunks, blanket chests, and travelling storage boxes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedWinchBrakePawl() => new()
	{
		Metadata = new("medieval_shared_winch_brake_pawl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5724", []),
		Noun = "pawl",
		ShortDescription = "a heavy winch brake pawl",
		LongDescription = null,
		FullDescription = "This heavy winch brake pawl is a medium-sized, well-made pawl worked from wrought iron. A toothed pawl sits on a pivot plate, with a short handle for lifting it clear. The striking edge is chipped from repeated catching. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2300.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Portcullis_Pawl"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy pawl for a hoist, winch, gate lift, or portcullis-like mechanism.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedWornOldWardedKey() => new()
	{
		Metadata = new("medieval_shared_worn_old_warded_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5753", []),
		Noun = "key",
		ShortDescription = "a worn old warded key",
		LongDescription = null,
		FullDescription = "This worn old warded key is a tiny, plain key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 45.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Old key form with softened edges; no guaranteed special compatibility beyond the warded key component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSharedYardGateLatch() => new()
	{
		Metadata = new("medieval_shared_yard_gate_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5782", []),
		Noun = "latch",
		ShortDescription = "a yard-gate iron latch",
		LongDescription = null,
		FullDescription = "This yard-gate iron latch is a small, well-made latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Gate drop-bar latch for yards, byres, pens, and outer service gates.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernShipChestLock() => new()
	{
		Metadata = new("medieval_northern_ship_chest_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5811", []),
		Noun = "lock",
		ShortDescription = "an iron ship-chest lock",
		LongDescription = null,
		FullDescription = "This iron ship-chest lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Regional-form addition for sea chests, travel trunks, and damp-weather storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNorthernStaveDoorLatch() => new()
	{
		Metadata = new("medieval_northern_stave_door_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5840", []),
		Noun = "latch",
		ShortDescription = "a stave-door iron latch",
		LongDescription = null,
		FullDescription = "This stave-door iron latch is a small, well-made latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy one-sided bar latch for plank or stave-built doors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWesternHallKeeperKeyring() => new()
	{
		Metadata = new("medieval_western_hall_keeper_keyring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5869", []),
		Noun = "keyring",
		ShortDescription = "a hall-keeper's keyring",
		LongDescription = null,
		FullDescription = "This hall-keeper's keyring is a very small, well-made keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large keyring form for hall, store, and chamber key management.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWesternMouldedChamberLock() => new()
	{
		Metadata = new("medieval_western_moulded_chamber_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5899", []),
		Noun = "lock",
		ShortDescription = "a moulded brass chamber lock",
		LongDescription = null,
		FullDescription = "This moulded brass chamber lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine domestic chamber-lock form for urban or manor interiors.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMediterraneanBronzeBoxLock() => new()
	{
		Metadata = new("medieval_mediterranean_bronze_box_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5928", []),
		Noun = "lock",
		ShortDescription = "a bronze box lock",
		LongDescription = null,
		FullDescription = "This bronze box lock is a very small, workmanlike lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze-fitted box lock for Mediterranean-facing household storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMediterraneanBronzeRingBowKey() => new()
	{
		Metadata = new("medieval_mediterranean_bronze_ring_bow_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5957", []),
		Noun = "key",
		ShortDescription = "a bronze ring-bow key",
		LongDescription = null,
		FullDescription = "This bronze ring-bow key is a tiny, workmanlike key worked from bronze. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 58.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bronze key form with a clear ring bow for tying or hanging.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicateBrassScrollworkLock() => new()
	{
		Metadata = new("medieval_islamicate_brass_scrollwork_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:5986", []),
		Noun = "lock",
		ShortDescription = "a brass scrollwork warded lock",
		LongDescription = null,
		FullDescription = "This brass scrollwork warded lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 860.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative brass lock form for urban chests, cabinets, or storerooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIslamicateCourtyardLatch() => new()
	{
		Metadata = new("medieval_islamicate_courtyard_latch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6015", []),
		Noun = "latch",
		ShortDescription = "a brass courtyard latch",
		LongDescription = null,
		FullDescription = "This brass courtyard latch is a very small, well-made latch worked from brass. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 650.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Drop-bar style latch for a courtyard gate or heavy outer screen.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSouthAsianBrassCourtyardLock() => new()
	{
		Metadata = new("medieval_south_asian_brass_courtyard_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6044", []),
		Noun = "lock",
		ShortDescription = "a brass courtyard lock",
		LongDescription = null,
		FullDescription = "This brass courtyard lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 920.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Brass lock form for courtyard gates, strong cupboards, or household boxes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSouthAsianBroadLoopKey() => new()
	{
		Metadata = new("medieval_south_asian_broad_loop_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6073", []),
		Noun = "key",
		ShortDescription = "a broad-loop brass key",
		LongDescription = null,
		FullDescription = "This broad-loop brass key is a tiny, well-made key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad-loop brass key silhouette for regional household locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEastAsianBronzeCabinetLock() => new()
	{
		Metadata = new("medieval_east_asian_bronze_cabinet_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6102", []),
		Noun = "lock",
		ShortDescription = "a bronze cabinet lock",
		LongDescription = null,
		FullDescription = "This bronze cabinet lock is a very small, workmanlike lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact bronze lock form for cabinet, chest, or document-storage furniture.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalEastAsianPiercedKeyholePlate() => new()
	{
		Metadata = new("medieval_east_asian_pierced_keyhole_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6131", []),
		Noun = "escutcheon",
		ShortDescription = "a pierced copper keyhole plate",
		LongDescription = null,
		FullDescription = "This pierced copper keyhole plate is a tiny, workmanlike escutcheon worked from copper. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative keyhole plate; passive hardware without lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSteppePackChestKey() => new()
	{
		Metadata = new("medieval_steppe_pack_chest_key", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6159", []),
		Noun = "key",
		ShortDescription = "a stout pack-chest key",
		LongDescription = null,
		FullDescription = "This stout pack-chest key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout warded key form for portable chest and travel storage locks.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSteppeTravelCofferLock() => new()
	{
		Metadata = new("medieval_steppe_travel_coffer_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalDoorsLocksStrongboxes.cs:6188", []),
		Noun = "lock",
		ShortDescription = "a travel-coffer iron lock",
		LongDescription = null,
		FullDescription = "This travel-coffer iron lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Robust loose lock for portable coffers, packs, wagons, and travelling storage.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareAleJug() => new()
	{
		Metadata = new("medieval_tableware_ale_jug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:11", []),
		Noun = "jug",
		ShortDescription = "a brown ale jug",
		LongDescription = null,
		FullDescription = "This brown ale jug is a small, workmanlike jug formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stoneware",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Jug sized for carrying ale to a table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareAleTray() => new()
	{
		Metadata = new("medieval_tableware_ale_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:42", []),
		Noun = "tray",
		ShortDescription = "an ale-cup tray",
		LongDescription = null,
		FullDescription = "This ale-cup tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Service tray for ale cups, tankards, or small mugs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareAlmondMilkBowl() => new()
	{
		Metadata = new("medieval_tableware_almond_milk_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:73", []),
		Noun = "bowl",
		ShortDescription = "an almond-milk bowl",
		LongDescription = null,
		FullDescription = "This almond-milk bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "earthenware",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shallow bowl for thick prepared foods; not a liquid-storage vessel.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareAshTrencher() => new()
	{
		Metadata = new("medieval_tableware_ash_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:104", []),
		Noun = "trencher",
		ShortDescription = "an ashwood trencher",
		LongDescription = null,
		FullDescription = "This ashwood trencher is a small, workmanlike trencher built from ash boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable ash trencher for solid foods and dry table portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBeechTrencher() => new()
	{
		Metadata = new("medieval_tableware_beech_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:135", []),
		Noun = "trencher",
		ShortDescription = "a beechwood trencher",
		LongDescription = null,
		FullDescription = "This beechwood trencher is a small, workmanlike trencher built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain reusable eating trencher suited to hall or tavern service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassBeaker() => new()
	{
		Metadata = new("medieval_tableware_brass_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:166", []),
		Noun = "beaker",
		ShortDescription = "a brass beaker",
		LongDescription = null,
		FullDescription = "This brass beaker is a very small, well-made beaker worked from brass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 330.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal beaker for a prosperous household table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassDrinkingCup() => new()
	{
		Metadata = new("medieval_tableware_brass_drinking_cup", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:197", []),
		Noun = "cup",
		ShortDescription = "a brass drinking cup",
		LongDescription = null,
		FullDescription = "This brass drinking cup is a very small, well-made cup worked from brass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright metal cup for domestic or guest service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassEwer() => new()
	{
		Metadata = new("medieval_tableware_brass_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:228", []),
		Noun = "ewer",
		ShortDescription = "a brass table ewer",
		LongDescription = null,
		FullDescription = "This brass table ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright ewer for water, wine, or wash service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassFlagon() => new()
	{
		Metadata = new("medieval_tableware_brass_flagon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:259", []),
		Noun = "flagon",
		ShortDescription = "a brass table flagon",
		LongDescription = null,
		FullDescription = "This brass table flagon is a small, well-made flagon worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright flagon for drink service at a hall table.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassGoblet() => new()
	{
		Metadata = new("medieval_tableware_brass_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:290", []),
		Noun = "goblet",
		ShortDescription = "a brass table goblet",
		LongDescription = null,
		FullDescription = "This brass table goblet is a small, well-made goblet worked from brass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 500.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright goblet for formal drink service.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassHandBasin() => new()
	{
		Metadata = new("medieval_tableware_brass_hand_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:321", []),
		Noun = "basin",
		ShortDescription = "a brass hand basin",
		LongDescription = null,
		FullDescription = "This brass hand basin is a medium-sized, well-made basin worked from brass. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3000.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal basin for handwashing before meals or chamber use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassOilFlask() => new()
	{
		Metadata = new("medieval_tableware_brass_oil_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:351", []),
		Noun = "flask",
		ShortDescription = "a brass oil flask",
		LongDescription = null,
		FullDescription = "This brass oil flask is a very small, well-made flask worked from brass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Metal flask for oil or other small-volume household liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassPitcher() => new()
	{
		Metadata = new("medieval_tableware_brass_pitcher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:382", []),
		Noun = "pitcher",
		ShortDescription = "a brass water pitcher",
		LongDescription = null,
		FullDescription = "This brass water pitcher is a small, well-made pitcher worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bright metal pitcher for guest service or washing water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablewareBrassServingPlatter() => new()
	{
		Metadata = new("medieval_tableware_brass_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFood.cs:413", []),
		Noun = "platter",
		ShortDescription = "a brass serving platter",
		LongDescription = null,
		FullDescription = "This brass serving platter is a medium-sized, well-made platter worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished brass platter for display and service.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
