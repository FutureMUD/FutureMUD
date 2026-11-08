// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch046() =>
	[
		new("item", "industrialised_weapons_police_military_0132_ventilated_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e8e8cd79357049bfbe693b5586107526975b5e2412290afd573f3d58f122c5b"),
		new("item", "industrialised_weapons_police_military_0145_stackable_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "350d8e3404c3256b78c06e92432f388e789f64f8de2b1112c075d76e6fe0705a"),
		new("item", "industrialised_weapons_police_military_0153_field_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9e13bc3f0d75f89f62c78c3c80bc6bf56c895e21c954e6e464706afdd2e67da"),
		new("item", "industrialised_weapons_police_military_0175_wall_mounted_cartridge", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "df0ca6ccbac789ee402c7064d304408439539245d78cd04af8dc10443f1ff307"),
		new("item", "industrialised_weapons_police_military_0189_travel_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7d6e4dbeb0ba9090b62abe053105de0820cc76de021e479e85f225a64f885fe"),
		new("item", "industrialised_weapons_police_military_0193_utility_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1cb21c632009a2ff791f3503d0e6d0abc94af7b9471f0784cbe8e17405046b4"),
		new("item", "industrialised_weapons_police_military_0203_plain_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5aa16733affc9dbe47758323971bbc1837431551b70c83f1675331218210cb74"),
		new("item", "industrialised_weapons_police_military_0224_compact_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "7248593d1696b3053880d63b0de3e38466cd31b2785358c4d942a6c65ef3d9e6"),
		new("item", "industrialised_weapons_police_military_0241_folding_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc3a5b04f7fd79f7e8ce71d64fdd3207a3180df700528315932541d0d5e352c9"),
		new("item", "industrialised_weapons_police_military_0252_portable_holster", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "11915618ea6c3af8a98092030ba96153bfc3423fcf3dd4e73c11bef74d393f50"),
		new("item", "jewellery_bone_hair_pin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d01cf9ea620d9c85c5e9a4cc451ddbb9f06a935b5e688e5a63867a699e4f0d4"),
		new("item", "jewellery_bronze_granule_headband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "97c7cecf1e4e56214892e25134da6e964cebb63447531d74b00a3455852d713b"),
		new("item", "jewellery_bronze_hoop_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "21357d572a28db2e8f569df068ebab78cae465d672b5de7378053962156b862a"),
		new("item", "jewellery_bronze_lobed_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "868a144eae1a9af9cc3e6c17d4b5396250145030e5a7b83fb73d5659ed209f68"),
		new("item", "jewellery_gold_carnelian_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "94229c6de18181c4b6b9d282c5002a393ba0039690b0bbb87d1f146f09f5e3fd"),
		new("item", "jewellery_gold_floral_brooch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "528b14e54772fb241d36d6f4fff685eb8eb54f45f68af235ff9943be693585a5"),
		new("item", "jewellery_gold_lion_girdle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6647416746e615fea29546e51ca0ad4310646e436c39ef52bd42f3fc729c929f"),
		new("item", "jewellery_penannular_bronze_brooch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b863ceb13952dc9877f6fb1754a5d6fa244e01bbbeb18306bc0af14bf596a42"),
		new("item", "medieval_anglo_danish_bronze_fitted_key_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9253da731370a686bc6030c59de4ba86e878e656cc4469b0e34ae92b590f391"),
		new("item", "medieval_anglo_norman_panelled_walnut_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b70a05c3149b47d8177c4bb17b51b8a99cb0e65578666544c83cfa3f92b94849"),
		new("item", "medieval_buddhist_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "362786c0e620c569b3bc8e4af79afbf42f44a94c1c9e6044ce5dd1822b853dde"),
		new("item", "medieval_child_silk_court_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e27b02cf3578cbec90051f7e9d62d3eb9b1562acc9b45924e4f61af7b59896f7"),
		new("item", "medieval_cotton_sari", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f8f15daffc91e8847f7857dae809b23c1c104e10465da90fa86868c9d99ed6f"),
		new("item", "medieval_decor_bed_curtain_pair", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65b8ee422c941a171c6e844efb56d417b470530dfe72a6c2181207fb8dac94a3"),
		new("item", "medieval_decor_east_asian_bamboo_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dea9954b967b72add95132ea6b74a24e25bed8da7fad3fc27f8b7f6bd2d97645"),
		new("item", "medieval_decor_islamicate_star_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d23ff9478f2b00e33a233a762be0e43d4d0dc67b565db73106aae9bf752633e8"),
		new("item", "medieval_decor_large_hall_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "40261af150a46be8f9ad99d60e117acbd8254a22e592b6fcc9838c82b4ef5559"),
		new("item", "medieval_decor_mediterranean_bronze_round_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "78ae8335fc54d8f8164059ca9b278810bc6b83b37c63fc2b24bb8145b6317be3"),
		new("item", "medieval_decor_northern_beast_carved_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c636d89a87d37cf81cf734c0a30047368aedb4ee73a7599446cb0ce249a8fd57"),
		new("item", "medieval_decor_south_asian_block_printed_floor_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "530e13fcb275cacd2c6cd9298f00c14dfa3b52530dcf4fec2d7afaf815781c0d"),
		new("item", "medieval_decor_south_asian_bronze_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a57120eab3a1f343fab20051fadb6224ddc30eb8944b45136832ef3d22a19d92"),
		new("item", "medieval_decor_woven_tapestry_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "27cb2714f4b07bdc5f0b0a36efa80678cad2aadf630252f4260bb1a6c10ab8cb"),
		new("item", "medieval_domestic_bamboo_shoulder_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efdca2478ea56b8b5cfd84c70c63e81191ff9874e3e302bb9ed2b97d52c99643"),
		new("item", "medieval_domestic_cedar_linen_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c124ef7ce5c07d2eb5052f24cddc374a4a3a89a9c70a4dbe261a3b5850ae2bb"),
		new("item", "medieval_domestic_coin_and_weight_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4b402eff0a90f2cf52bdfd9702e0859d98825b0b199d1bc6a85dbeec7ecbd68"),
		new("item", "medieval_domestic_dish_wall_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a036764524d2bf3de797086df97d6c8e5469e010c5a121043177b2f290a2075"),
		new("item", "medieval_domestic_forager_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b08c02ff8fa7646dec553b32245e16773e55ada0e64e3f780cbc8749b3ec857d"),
		new("item", "medieval_domestic_glass_fronted_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c197491239f47c4b264624e568078a3317ae809e5ade6b15d7a8bd3047f8ef10"),
		new("item", "medieval_domestic_kitchen_bowl_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc11bc85f0bac533bd0ca5e912d995475917134190fc9a904ab63f8d485aba76"),
		new("item", "medieval_domestic_linen_belt_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e3e99ce64e99dde104dc3cf6d08aaabd1d3a56383f63d43360e31e06772ce46"),
	];
}
