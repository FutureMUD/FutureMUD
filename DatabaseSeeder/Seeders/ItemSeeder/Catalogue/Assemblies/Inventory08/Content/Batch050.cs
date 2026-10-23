// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_jewellery_head_fresh_blossom_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "71efdb01f54c9ebaba5cf0832d52741f0542665c5456d4c08a6695383109b850"),
		new("item", "medieval_jewellery_head_garnet_set_diadem", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bbe59d2dc2ecaa0d32dea5163af557cdba5c30c08716c70ee9b828410adadbc"),
		new("item", "medieval_jewellery_head_pair_of_gilt_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba902e92a66bcb026034ee631ae6cf0035d7d4ffb506c3e479b52df48c9c6f4a"),
		new("item", "medieval_jewellery_head_pearl_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b01eb4ffb3e1fccef59a23b96fd88534da9a6e991fcf6a28b8e13e9fb3d171b"),
		new("item", "medieval_jewellery_limb_faience_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bac949628f90b1425d34d2ea1fefad7d26c12a50948da8320b5f1e2a3ec7494f"),
		new("item", "medieval_jewellery_limb_motif_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "26f8aa557b406edd242be74768e576c096b681900b4775bdae338f77acae3c46"),
		new("item", "medieval_jewellery_limb_plain_gold_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5202371b117f9c3455c358775eeb9f8f0313409572b2da067e9ecb24b158c6d"),
		new("item", "medieval_jewellery_limb_red_coral_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "284c8aa8027ff1bc3b324a081c3d4e499706360faefe19d19a80f17d3800ca89"),
		new("item", "medieval_jewellery_limb_tiny_pearl_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "85d40e1a7d70b74cb09a2468ce7ba502fa03795fd35c6d6a80af11a51a8163b8"),
		new("item", "medieval_jewellery_pin_iron_dress_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e81f1e2078f3f948e9920ec3e90351c1dc44cfcbd66c6db375d034704e1175"),
		new("item", "medieval_jewellery_ring_cowrie_shell_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bfd90fff0d223af08aba9865e016ec6deaff218ac925715a7b2a006317d8dd10"),
		new("item", "medieval_jewellery_ring_motif_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1020429e1e3ad024d7203503bb9d2ab890d82a7377674cd882a8f5173c1aef8"),
		new("item", "medieval_jewellery_waist_bell_less_silver_girdle_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e869c9a5b6474e1d5830c2717a4af0bc62e76a55338c6c6a84a1c186161e4973"),
		new("item", "medieval_jewellery_waist_brass_belt_jewel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dfb9b29e71d94658e5b3f76fda58496b193b8fa4207d379117252717f1e71ec"),
		new("item", "medieval_jewellery_waist_brass_merchant_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "155223d48e7984e3652da829dd4fa6248e43ceeeeb7c4b849bc4e3ea02c11043"),
		new("item", "medieval_jewellery_waist_gold_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "156b2b392d285a42bad2204f6f61dfaae853db70326bde3a1f11c93d622b9097"),
		new("item", "medieval_light_islamicate_courtyard_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7334ae93be953e2e12506e5f9926520d718de68e7b33885b2a83fa53fc855789"),
		new("item", "medieval_light_linen_wick_night_light", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "18929963b92ca20db0d212922c58e62df13ef6a71a3ede2372495b472328382c"),
		new("item", "medieval_light_linen_work_camisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c90819143e50b08994597ebbdd4d70e2f72e2813e7d4e38d02bcb840736f54fb"),
		new("item", "medieval_light_mediterranean_marble_lamp_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddd150130efbf2c67e1b9adffebdf7ca00682a86b7fd3e81ee67fba8a5c05639"),
		new("item", "medieval_light_mediterranean_olivewood_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28ba12c56f7f49a1efb8928938b66882f3373b21e48e90e5f5b6f0ecd2597beb"),
		new("item", "medieval_light_northern_birch_splint_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbe5b8854060303db1ac87a2d9f49ffef7dc71f7b3e66ff18cef4299b7680ae5"),
		new("item", "medieval_light_northern_long_hemp_rushlight", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc9a9151d32c38b53200bf2f38f01be5ec9fd5933b285fec220724e60be89c88"),
		new("item", "medieval_light_thick_household_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "77f74fabecd13a2d1688f81143b7a945044a02e3150c64d7d351a6b8c6ff78ef"),
		new("item", "medieval_locking_domestic_heavy_safe_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3b49d1c5fa7bceade363e59e107715b3e00f1606fc27214cb38bcb232c03b00"),
		new("item", "medieval_locking_personal_jewel_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e73b736ef6c9653daefa00ba42cef51fcca9b5495f12a00e6933677e526efd96"),
		new("item", "medieval_locking_trade_guild_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "76c6b81f3ea6a60422c682708093443fce8b6ada8d67cc23a1cbdcaf8e77bfb5"),
		new("item", "medieval_medical_apothecary_sorting_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "243ab8e1cffb98e7bd5037d12f5c63154232a2334d3c946b258b941d8efbd476"),
		new("item", "medieval_medical_left_glass_eye", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b000f6d30b20946fcc31606df79188c401d73effc8063cb12ff134490abfa085"),
		new("item", "medieval_medical_shipboard_wound_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a29cfd764ceec05201573f039d6c02e45b86a92dbb1e62e40f3469d59b1030a8"),
		new("item", "medieval_medical_staff_crutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f19630d6ece50c93d7ee2b465e78a461a88e933c07a605d22006443841fa16f"),
		new("item", "medieval_mediterranean_bronze_box_lock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "74c30833fa8902a01950fa06cdcdbe6240de30594ba2a64e907dad36696a2c5f"),
		new("item", "medieval_military_broadhead_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfdbbcbc1ad1c9c3d3e46bad861c048c7c450f4b1b09015122db7b1c311f5d57"),
		new("item", "medieval_military_crucible_steel_dagger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d67246d6570773ca9bb833be5124f4792db7e963e8dc1648ba9c9bbdced49a0b"),
		new("item", "medieval_military_fine_horn_recurve_bow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc743e95d89fdfc37752ba3c542e591c39d5ad36924e14f6b957085b0803fde4"),
		new("item", "medieval_military_fine_lacquered_bowcase", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2d11f71cbe42283829ae075bdcc18ecd545555bb25dcdacfec8cffb455cd312"),
		new("item", "medieval_military_lacquered_lamellar_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df10182746533b9d5bd24980850900c8ac75f2a7234a939cf3b08ee9b995ae94"),
		new("item", "medieval_military_lacquered_wicker_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6163573ccfe39630169b919fdf3c8214378cb2c422f3dff6336772ef57ed9ae1"),
		new("item", "medieval_military_leather_poleyns", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3dfc6c22514e23a3f0a1b44d8a52d7c683860eb6e6af0f340f401ce10ed52435"),
		new("item", "medieval_military_mounted_harness_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa6bf665f6104384af48269d195425c59a4efe3aa1322c383021725aa62d65c2"),
	];
}
