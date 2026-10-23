// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory14Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch064() =>
	[
		new("item", "renaissance_repair_rough_printing_equipment_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e33afe9cab27307cac404c2cece7690cc95ce493f6882b778f5f208e3ae968b9"),
		new("item", "renaissance_shared_clothing_crossfront_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "faa0464f0d29ca2d068158e5fd72f2659ba9274937bc359414baac21516f199b"),
		new("item", "renaissance_shared_clothing_fur_lined_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78ea5f890a2dd9b708faee59d1f6a8653447d723d93eddbbee5f95e172f95411"),
		new("item", "renaissance_shared_clothing_leather_sandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e56f92852ba93a98469640ef974c9d76714d1f96482784343775eef22e4b942c"),
		new("item", "renaissance_shared_clothing_short_head_veil", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8ba250b2025bcf439a4301a691eca92411c38247b62286f900fab0dcfc73a70"),
		new("item", "renaissance_shared_clothing_sleeveless_overvest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36bfeecdca2958c67435b514cf7fcb975806829e62ea3993fb00db4201b20a91"),
		new("item", "renaissance_shared_clothing_straw_brimmed_hat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab67c999c7508a926eecaa2758ee3b01f4deb6771427ec5d38a319454c2867a9"),
		new("item", "renaissance_shared_clothing_textile_sandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76629517e8337d1152751139b8b0fc94e3c9ce840041c1b5cd7a72e4fdb0331d"),
		new("item", "renaissance_southasian_shoulderdraped_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44ba556d27dc23c74d9516baeb64ba36bc01ae69eae78f2b4bd4328dc9779094"),
		new("item", "renaissance_southasian_split_riding_waistcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "738aabb3c25636f655f59fbe2af6b042baf63e05460eb26aaaf6279b1fbef574"),
		new("item", "renaissance_trade_andean_storehouse_metal_ornament_casket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "681e57078eb8dd51ddf82882d430d63fa0614f0745573317460923245684c2d8"),
		new("item", "renaissance_trade_japanese_ryukyuan_tea_utensil_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d81559c523e373eb84ab7424700e727267387566c5f0b1bdf9ad3f9d95ec7838"),
		new("item", "renaissance_trade_japanese_ryukyuan_tribute_document_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8feaf4fa1b56d9e4ed8d75f23d70e835bac23e344f3ae00cdb5fafee229f7f0"),
		new("item", "renaissance_trade_steppe_caravan_courier_dispatch_tube", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e6cfa00eaddec6c79f2a0323148042c5b3ce0136883703f1cfb85b50ee47b0a"),
		new("item", "renaissance_trade_western_counting_house_correspondence_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8f5508ade439f0056f63858858f64dd3547b81b8122d453b282e72782f4041c"),
		new("item", "renaissance_western_front_laced_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7d8e66e6482f5dbfc5ff87c2b26f95fedca981ae595efc8f996914c762eba38"),
		new("item", "renaissance_western_rolled_velvet_balzo", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a178d469ef6ecbbd340f1b0bae7a71dac92afec2a1f842eb24015f7e03e5a6e"),
		new("item", "renaissance_writing_commonplace_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aadc72201bf3d2311e0175b758eb3821fd2adfb762da2ece7bbf3ba85b882ad4"),
		new("item", "renaissance_writing_etching_needle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd6e816a4025b61b4ecad9ce35bb4a31ef880421257bbb45ac3c3d0491880f8"),
		new("item-skin", "antiquity_skin_vestal_white_priestly_veil", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:adjacent_antiquity_priestly_linen_veil"], ItemSeederOwnershipPolicy.StockAggregate, "82c08cc27fe46039f48cad94972394e84acfa459a3cddd5bd3261c9849b40373"),
		new("item-skin", "earlymodern_skin_judicial_full_sleeved_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_institution_academic_robe"], ItemSeederOwnershipPolicy.StockAggregate, "46aa34b3dad8f608994fe4c84e842e2b8f1c260e66eeb6e4de5c6900c9135656"),
		new("knowledge", "Anatolian Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8be29af24007e5bff75fa16a0ba2b5328e8e439ba7205204eddce4e91460a489"),
		new("knowledge", "Etruscan Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7808c147bd92dd678fe2a27f3d3473cd67c513225601eb025138b57cac4ff9cc"),
		new("knowledge", "Medieval Equipment Crafting", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e550c97be8ca399ba37d96190cb01e6c411d94c6103e141934595d12ce9947d"),
		new("knowledge", "Persian Textilecraft", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "254d005d5abefa6edc436243fda0ef7be0654e3784f4d0b7c0e17e493ec98f98"),
		new("knowledge", "Renaissance and Early Modern Locksmithing", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dad205894e9df245a61f711b18e33fe686cba829290d96d1b7afc2b951cc0c2"),
		new("liquid", "French court vanilla milk chocolate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3869718f0a7af04e0d23220340954c7ec0917ccec61217fc8ac858a05dd1630d"),
		new("liquid", "Ming smoked-plum tea liquor", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7f7b637e9d9ccf6b750aaf2a296ed1a1166448cd2663c0f0f52be6695ec4e70"),
		new("liquid", "Spanish Atlantic cinnamon chocolate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7e957eac2deac54cf000f154ec4d8db1020c9e914fd714233e3279ed33fca8d"),
		new("liquid", "barley beer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "abd7124948557976cc9ecf528bf166a5fe5a3dedae49c4df2f17f138102f7820"),
		new("liquid", "citrus-peel aqua-vitae cordial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1644907132b2069aa909caa68c57cbf5353573e42e0e2604dd2cbfadacaef1a"),
		new("liquid", "clear grain spirit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2ffeabd3492b9483a877e84bd94a89381f79f54273ad7fdde50349bdea12a2c"),
		new("liquid", "fish savour sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "616771afff7d19311c294e876a53601a798467976433b49cb85a7cf9e9a65f9f"),
		new("liquid", "fish-day saffron broth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c47949e0196b3778fd2d032543c11d2b3d98ac355e63c818e23a41777baf85a5"),
		new("liquid", "fruit-honey ferment", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2a2c0b66f34e541b7817c7182e069a1d90594832a817d36c67470c10e2a241b"),
		new("liquid", "grape reduction syrup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b2733de8b395dc87129f2d465e11af2b5cea83aadac90596cf7e511af7d06ce"),
		new("liquid", "ground nut sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1204f2cebbe249fc3a69dc2a57891f302a1dd786dae492bc7063fb226bf40909"),
		new("liquid", "maize water drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cde010fad565fb5dfee4e9e0761300f4a8c3f7a38537862ab01c8b222edfab3f"),
		new("liquid", "monastic bitter-herb wine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e952381d0026c2fb8d243bf257fb2c55549c15518c70015118ac66a3e22b177"),
		new("liquid", "pear wine", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f73021a6ef35e14f891c63978b2a141ebb2b1bbd9f663c15dd1050ef3f3ed72"),
	];
}
