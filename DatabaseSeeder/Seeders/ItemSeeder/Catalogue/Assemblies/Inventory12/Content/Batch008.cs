// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch008() =>
	[
		new("craft", "Carpentry and joinery/build vine teak courtyard gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_car_courtyard_gate_596"], ItemSeederOwnershipPolicy.StockAggregate, "2f1cefed89aa1c3ecce4daf233c58d847707068ea140385028a0154081b219f5"),
		new("craft", "Carpentry and joinery/build water-wave brass barred barrier", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_ind_barrier_516"], ItemSeederOwnershipPolicy.StockAggregate, "2dba1eb18618567357b73c57d693c99ab1885b2f2409584cd2ca0cd93fa4d300"),
		new("craft", "Carpentry and joinery/build water-wave pine lockable door pattern 3", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_stp_lockable_door_409"], ItemSeederOwnershipPolicy.StockAggregate, "9122f9623d07af38aee6b7d30700134238b614934a9c03d2ab18bc0ecf73756a"),
		new("craft", "Carpentry and joinery/build water-wave teak lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_nl_lattice_gate_26"], ItemSeederOwnershipPolicy.StockAggregate, "abfc81bacbac5c0ccc043204cdc5d6797827b5441ff197cbbf147d88fded3cf3"),
		new("craft", "Carpentry and joinery/build water-wave wrought iron garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_rus_garden_gate_162"], ItemSeederOwnershipPolicy.StockAggregate, "5fcdb9cfb88142514de5e912fe7f7cb7c53b322a81ce774d4427e5e9e6129676"),
		new("craft", "Carpentry and joinery/build wax-sealable coastal account chest", "crafts", ["renaissance"], ["item:renaissance_trade_south_asian_merchant_coastal_account_chest"], ItemSeederOwnershipPolicy.StockAggregate, "8a56cd23b5c96adcf912d9faa712a8a8929720ca4790683b792b34ca2d370c11"),
		new("craft", "Carpentry/carve pine household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0f48f8158c6fabef07c980912bbd73d33a6d5b4f075b6275747b64624337b8e"),
		new("craft", "Carpentry/make ash armour stand", "crafts", ["antiquity"], ["item:antiquity_ash_armour_stand"], ItemSeederOwnershipPolicy.StockAggregate, "7908511525ad48aabb15bcf3774c1146eee6beaf5370710122bf89ffedab023b"),
		new("craft", "Carpentry/make bronze-bound chest", "crafts", ["antiquity"], ["item:antiquity_bronze_bound_chest"], ItemSeederOwnershipPolicy.StockAggregate, "12b2ccf6aa29b47aba711a30108626727da1b942fe988894ae2bbcab9654e5e4"),
		new("craft", "Carpentry/make cedar scribe's box", "crafts", ["antiquity"], ["item:antiquity_cedar_scribe_box"], ItemSeederOwnershipPolicy.StockAggregate, "80c86b452bcd0529399b6647c45016b37ecf2757ad699690fadd2879a871d4ca"),
		new("craft", "Carpentry/make fine lacquerer's brush", "crafts", ["antiquity"], ["item:antiquity_lacquer_brush"], ItemSeederOwnershipPolicy.StockAggregate, "7e48aba81ef008e90c7ad0682c942e21d0bbe890d4807416931f4dca327a6bbb"),
		new("craft", "Carpentry/make flat wooden carrying tray", "crafts", ["antiquity"], ["item:antiquity_flat_wooden_carrying_tray"], ItemSeederOwnershipPolicy.StockAggregate, "8467697c2f7250a5fe734902c3c23fc527a35b5b126414814abe3ede5455105b"),
		new("craft", "Carpentry/make hide tanning rack", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:historic_tanning_rack"], ItemSeederOwnershipPolicy.StockAggregate, "88d5593c4a21219dafd68823a7d0f7a346c21dd8e6fd6f405d1fe4c6a81e5abd"),
		new("craft", "Carpentry/make large feasting table", "crafts", ["antiquity"], ["item:antiquity_large_feasting_table"], ItemSeederOwnershipPolicy.StockAggregate, "6d7382d03e3682f08b67dad9d4c3a35463b34a667adfb1a27c49d67612d92c3c"),
		new("craft", "Carpentry/make linen dye strainer", "crafts", ["antiquity"], ["item:antiquity_linen_dye_strainer"], ItemSeederOwnershipPolicy.StockAggregate, "a78eeac52752e7fe72ae1678d6fb438bffb18fa38853dbbe45f392fa22e5131d"),
		new("craft", "Carpentry/make lockable acacia courtyard door", "crafts", ["antiquity"], ["item:antiquity_lockable_acacia_courtyard_door"], ItemSeederOwnershipPolicy.StockAggregate, "63500b10ec1c781176f98fdd415c2d25863bbcbd1c0441a9a1b48013cbf45315"),
		new("craft", "Carpentry/make lockable merchant counter chest", "crafts", ["antiquity"], ["item:antiquity_market_counter_chest"], ItemSeederOwnershipPolicy.StockAggregate, "214ddcaeb975e0d0986b5bdd0b2d100243042a691b37337a08ab084bc6968716"),
		new("craft", "Carpentry/make oak cloth tenter frame", "crafts", ["antiquity"], ["item:antiquity_tenter_frame"], ItemSeederOwnershipPolicy.StockAggregate, "e5f182b3a2ae09dce8b626baecdcb3cd2c39a763b2a97ba071b154b298ceaf83"),
		new("craft", "Carpentry/make oak fuller's trough", "crafts", ["antiquity"], ["item:antiquity_fullers_trough"], ItemSeederOwnershipPolicy.StockAggregate, "35c51659a784fb774f04531f95f2f83aba0c305efd7439d86e58ac2a06bbf8f0"),
		new("craft", "Carpentry/make oak tanning paddle", "crafts", ["antiquity"], ["item:antiquity_oak_tanning_paddle"], ItemSeederOwnershipPolicy.StockAggregate, "4e02617ca24e2d9ce2cec7abda625d98da6bdb524ca8be0b54f86e70342798a0"),
		new("craft", "Carpentry/make oak weaver's sword", "crafts", ["antiquity"], ["item:antiquity_weavers_sword"], ItemSeederOwnershipPolicy.StockAggregate, "bdf51f6828abca28dce01b01487989bb93e9cbc1e79d13e8bd1b456976e4bcad"),
		new("craft", "Carpentry/make padded household daybed", "crafts", ["antiquity"], ["item:antiquity_padded_daybed"], ItemSeederOwnershipPolicy.StockAggregate, "a028034e6ef008cc45dc5abba14c56f951f8992d459c8b122ff227b0b643ee43"),
		new("craft", "Carpentry/make wooden kithara", "crafts", ["antiquity"], ["item:antiquity_kithara"], ItemSeederOwnershipPolicy.StockAggregate, "f7c4275d70a961cf63613c6acf95a60ec4b2829c99cce2e98401978f55edc051"),
		new("craft", "Carpentry/make wooden parchment stretching frame", "crafts", ["antiquity"], ["item:antiquity_parchment_stretching_frame"], ItemSeederOwnershipPolicy.StockAggregate, "29544a15c667152e3320aaf17ae39787f848a36cd32f65cbae21bca8b0d7c987"),
		new("craft", "Carpentry/make wooden pin-lock key", "crafts", ["antiquity"], ["item:antiquity_wooden_pin_lock_key"], ItemSeederOwnershipPolicy.StockAggregate, "251db77d7f4665d848a2f58631c2a068663a0fd7441cd7bf61066a347a161329"),
		new("craft", "Carpentry/prepare acacia furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "88168ead940f42439eb1ddc9c88d7a6ae81d3b267b31649cd29d0c6580fa126b"),
		new("craft", "Carpentry/prepare ash furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "17ffd507721b9ad813c8cdc073b41812f095aaf049ef08a56cfd3588033fd5b5"),
		new("craft", "Carpentry/prepare willow furniture timber stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8406ea55e429984d57b1ebfdf940ecdd0bbd83c85379bbf6f8297efcd8cb5b24"),
		new("craft", "Carpentry/saw ebony furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fda51e2394a19d3a89ee3161233dd451f5a941f08b8794773e49b0cf81cd819"),
		new("craft", "Carpentry/saw oak furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6ee40bf53668738c499582fcd8969e7be5b663d7e14698e35f2fe4481d7147d"),
		new("craft", "Carpentry/shape oak tool blank stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8f58f5d005d3b436f9dad1155a899b7d557bf5f4ef2a86f99021764aea9baa31"),
		new("craft", "Ceramic and glass work/form glass sherbet medium storage jar", "crafts", ["renaissance"], ["item:renaissance_container_ott_liquid_medium_storage_jar_02"], ItemSeederOwnershipPolicy.StockAggregate, "58843e42b7e0961f768cd8b15e0bc17bf16d4b989482c72ba5c9f1001e3fe421"),
		new("craft", "Ceramic and glass work/form joined double ceramic serving bowl", "crafts", ["renaissance"], ["item:renaissance_domestic_andean_storehouse_double_serving_bowl"], ItemSeederOwnershipPolicy.StockAggregate, "c81fe259c721a715aea15ae4456708881645f0202dbfd809a9b5503ebb57c314"),
		new("craft", "Ceramic and glass work/form lustre-painted faience serving bowl", "crafts", ["renaissance"], ["item:renaissance_domestic_persianate_court_lustre_bowl"], ItemSeederOwnershipPolicy.StockAggregate, "393d169552872130d17767394cf7348af63cc0e12fc5dbcc847e7043533349a1"),
		new("craft", "Ceramic and glass work/form porcelain tea medium storage jar", "crafts", ["renaissance"], ["item:renaissance_container_eal_liquid_medium_storage_jar_02"], ItemSeederOwnershipPolicy.StockAggregate, "27ff0cf156ac82558479746e866d48f0e1b71aa0fc549b2b48077e3b883055db"),
		new("craft", "Ceramic and glass work/form tall earthenware port water jar", "crafts", ["renaissance"], ["item:renaissance_domestic_southeast_asian_port_water_jar"], ItemSeederOwnershipPolicy.StockAggregate, "9249d93830e587e740c65d8d80ad1188f30aa1aae6850033ff89d59938ba777e"),
		new("craft", "Cleaning and personal care/make folding cotton comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0043_folding_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0043_folding_comb"], ItemSeederOwnershipPolicy.StockAggregate, "d61f43199e8d770cf3388536c381f29102ac3364ba1466719982036d58380714"),
		new("craft", "Cleaning and personal care/make portable ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0053_portable_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0053_portable_comb"], ItemSeederOwnershipPolicy.StockAggregate, "fd31b9a05d3acd2f64a3d3e7c95bbdbada7a619445ac4197841500d177ac6057"),
		new("craft", "Cleaning and personal care/make reinforced ceramic comb for domestic hygiene [craft_industrialised_cleaning_personal_care_0013_reinforced_comb]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_cleaning_personal_care_0013_reinforced_comb"], ItemSeederOwnershipPolicy.StockAggregate, "7701d260e0657a7a2cfa37a47c88028f35cb9208b21e80624363ea568ea55843"),
		new("craft", "Clothing, footwear and uniforms/make compact cotton vest for domestic wear [craft_industrialised_clothing_footwear_uniforms_0043_compact_vest]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_clothing_footwear_uniforms_0043_compact_vest"], ItemSeederOwnershipPolicy.StockAggregate, "c125b66ea04951fa71aa85302f9c2f70057797dfe5cce870fda69beddd9bb447"),
	];
}
