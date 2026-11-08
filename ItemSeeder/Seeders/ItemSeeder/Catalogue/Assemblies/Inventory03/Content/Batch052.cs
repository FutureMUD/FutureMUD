// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_water_timber_spring_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d6afac31ba9f84d8a360adc10e2f6fd393b4b6c58298d400972c88aaaefdbd3"),
		new("item", "medieval_wool_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "64e9ba8cf5859395810e48c4d1afb2243ac02284a0a8ae525eb03b5856d4a6dc"),
		new("item", "medieval_wool_work_pellote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "eee3c384ce5724b1bf6a3584e2279c8350b33c2dcc7ab111b0e51577ff7c4872"),
		new("item", "medieval_writing_blank_parchment_writ", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "67d2f9e81af929e555027eea3c869f42ed44dc94b5a686963ecadd9e50cddd78"),
		new("item", "medieval_writing_bronze_pen_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6465ebf2e16ab30a629807647145735803137c426887e50ac7a63abfe331dce6"),
		new("item", "medieval_writing_loose_scroll_label_tabs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa913aa182e51e274c1fcb31ea0fb4477ea27fa1a96130c00010db4c2e0bd65e"),
		new("item", "medieval_writing_parchment_pounce_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e002d54f98a7828986068b94484b7cc51c8c48df8cda67cb3eb9bd10fa700378"),
		new("item", "medieval_writing_plastered_ostracon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbc526a73aa21225ff56f1b6a173e2680422c84a94643ca52012a800b5c26734"),
		new("item", "medieval_writing_qalam", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "66c92e4de86598631dccb454cb4358a2e70bf21ea6112b52e64f4c44bfdd1e53"),
		new("item", "medieval_writing_rag_paper_quire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec53a2493335278fdbdc22a12eb929309b8a2704770b092d216bf27471b34e73"),
		new("item", "medieval_writing_scribes_sloped_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53da55d05cdcbc340a62ef000f019d65f6b6dd3178cc3c4d05d4df4fa27a6177"),
		new("item", "medieval_writing_sealable_archive_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84a0e7517d603efa00fa06928d070aee855739691cf1c800e7f4baef361fd577"),
		new("item", "medieval_writing_sealable_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "424e3139aab2f2e9b8eaf8d9cdd3531b452f7a4026be24db4adf50bf5467202a"),
		new("item", "medieval_writing_sealable_parchment_charter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "05402221d0cde93822464a4980f7e2b587446b9deac7cab8df952b82eb9d12f8"),
		new("item", "medieval_writing_small_archive_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fef0ac6b1ab242ba2f3cdb60a40800e6d77107f46fcadf26b3eb13921cf884c9"),
		new("item", "medieval_writing_watermark_wire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4959a7280eebeba6c12ae2c92f33389261a4cd518fd4d6ecb9ee35b651ac17a"),
		new("item", "medieval_writing_wax_sealable_document", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "49194de1957540e90f60d882727a6b2cbd181bf8479140f00b4d373a291ba870"),
		new("item", "preindustrial_door_hook_latch_71", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c26b1cf86768e33705c1236c1c48d34e78054bc1445aab40b41ceb29b497c87"),
		new("item", "preindustrial_door_ring_key_80", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b768327076c54ce87910f2031cbfd69567d4b6e5af5a655bdf1327e85518bb71"),
		new("item", "preindustrial_door_screen_20", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d8cd5b307a35b9ad03c961c76c434cb50f0e5b49d70ebac71849b31c7864013"),
		new("item", "preindustrial_door_shutter_47", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b9a3844697eb12590fe8aa58a64cb06bd92318857076ca3fbb68b370f2636ae"),
		new("item", "preindustrial_door_strike_plate_87", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "64ba28cdb9b7624739ed93c774642be1d473717a790f909b7eb11556a7dd7d1a"),
		new("item", "preindustrial_food_acrid_burnt_grain_ration", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a80be3ddae23a861437e9366087c98160e1c0d20f8ca1ea9b3d3577bf721da7"),
		new("item", "preindustrial_food_ale_soaked_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db5c417a5af3f17d8e84404e048e799132c3a50934c9d6803b0c670518a0163b"),
		new("item", "preindustrial_food_asparagus_and_herb_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cde1d380c0e8b56af7f7aa395326841323d220b0d8b4b6f0caa3d33d17ba6569"),
		new("item", "preindustrial_food_baked_squash_halves", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d4edb7d972468d1be15d3797f723fb6615521645795aa44b2c97e159641136b2"),
		new("item", "preindustrial_food_barley_garden_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "225eb85cc9ada1c65b1d2bb408e6ca7d86b3a155488a375547d8b9b4f48d9d18"),
		new("item", "preindustrial_food_bean_and_cabbage_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63261168b59f77c63ebff0529fb4994de2099d6b63a275d63745faa208de609f"),
		new("item", "preindustrial_food_beer_mash_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd0b00a481e9e314323e4f8fe94cf22c8c419fdfdd6547899d8d9086af4b0145"),
		new("item", "preindustrial_food_beet_greens_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a70fc23f13b8c804f32033d9542a2af181b457028a9ddfc359a9ee5aaa1e4cb4"),
		new("item", "preindustrial_food_beetroot_and_lentil_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f06f52000aee896bd06df38cfce5d92b600e687d1385931707ae236634c1f1d6"),
		new("item", "preindustrial_food_beetroot_barley_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c227a3e94d4b0064fcdfd30e71cc94d815d5b4845b9ddf5fe541efc9c61f2b3f"),
		new("item", "preindustrial_food_beetroot_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fec524bd8706eaafe9880c1ac09c972a216b2bc17ec0c283ec2b0c48f35bf003"),
		new("item", "preindustrial_food_berry_drop_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8225b400be86c88be7292d428219748a89c4cd8b876c85841f86d8c2e67a8a17"),
		new("item", "preindustrial_food_boiled_goat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdee3530ee246780f6666bbe9f79c01f627be63998b84bcd33c828fa501ecb22"),
		new("item", "preindustrial_food_boiled_millet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ea02d99cf71e3d60385eb4d36d79b4bf5d190b7a417cf62ac18cefeb206f220"),
		new("item", "preindustrial_food_boiled_parsnip_batons", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0540b4e1b35d9ed41aa23d97111d05f9295a7573d2dfb883168f334af715137"),
		new("item", "preindustrial_food_boiled_shellfish_platter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b107b096f5db6441284007e339a7a95a6084200eaf32fe69750fdc15c6e7b49"),
		new("item", "preindustrial_food_braised_baby_aubergines", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48d29d8f73af7e43ccb0d12ced1c27454e57d5334563e03d4a4561cd719b4186"),
		new("item", "preindustrial_food_braised_cheek", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7e6a30da0d2eb931ab587aa31424aafc65ad2df38e9c5509476bd27399230bf"),
	];
}
