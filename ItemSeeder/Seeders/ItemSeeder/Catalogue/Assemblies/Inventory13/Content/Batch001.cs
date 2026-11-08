// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory13Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch001() =>
	[
		new("component", "Antiquity_Reed_Pen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "645d9ea6adc816b45c2496b7c47ad2238af77c3f7d7408eee17c0b682ba8a671"),
		new("component", "Antiquity_Wooden_Block_Surface", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92c02fa0ca9334c288a74ba3960cd373e8addb66b1d8e0e434cc100ecf72b65f"),
		new("component", "PreparedFood_Antiquity_BrinedFruit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e35986ca7efd8033aebbfa4d9010df95208c3e1e24b36471617084802ea272d"),
		new("component", "PreparedFood_Antiquity_LuxuryStew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8262492c0ae40a3d77775b1020bffcd9f88b6d968933cf01aea2b8eef5dbed7"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_cape_pickled_fish_curry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "10a82abd61eb0f4224da3d6fc809c1dd4fb55bc2261d84f4bc705e49697ce38b"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_french_almond_macaron_pair", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "14b6ca20f6861e0651bbad51aa2ce95586bccb9d55ccd7901918041722c5e81f"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_german_potato_pancakes", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3ba8ca1d408c30f25d80899916dd10967818204bde5e9dfcdf42c76ae9b61c2"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_lowcountry_enslaved_rice_peas", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "61d8ca46371727dab4d2dbc43d47684eefe4ba7f4339b8c16bc9f003a6634e60"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mexican_candied_sweet_potato", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad7219037802e485c1800f0c69bdf97e7dc0f38acff7717ad6dfde55fa743a37"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_mughal_jalebi_yogurt_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "577095cd443deab73acf47a4ea94ac8ec43b7fb72856a8dbcded6224419ff28c"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_qing_tea_egg_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c39f600342ceb13026feeb357cbd82f9195e66655530c64b04bbbbb68241495"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_herb_plum_meat_stew", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d48a9a146b240ef660809668d53b2e3b01f3449d69037c6c7efb5eec352696e"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_safavid_rose_nut_nougat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "57156246711c05894582c6d0ef2f9f3562ae83166a6d346b1144d92ab3a1e783"),
		new("component", "PreparedFood_Catalogue_earlymodern_food_slave_ship_coarse_bean_ration", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6ed6327d04f7cb23ab8522f0a810196656c54da7acc8099fa16494fb56e88bf"),
		new("component", "PreparedFood_Catalogue_medieval_food_bread_bound_venison_mortrews", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ad44961f0194ea0d9a78a1327f250ad2dcd38fa66bc52c80b4c5ac44a286e2e"),
		new("component", "PreparedFood_Catalogue_medieval_food_cameline_sauced_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb0326c2be46112d38aaec81ac2c4464cd69f79c79a6bd86e3cb808f3d15576d"),
		new("component", "PreparedFood_Catalogue_medieval_food_castle_shaped_bread_subtlety", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "37e9af412a09351ee60f486921c74eb10fdd76a3f4aa250fcae47d793da3d3b2"),
		new("component", "PreparedFood_Catalogue_medieval_food_festival_rice_flour_fritter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "96ad9910c8c0c8444798e3f853e1a45fbbe6565a45c4f4c2b85c3e00fb19fb20"),
		new("component", "PreparedFood_Catalogue_medieval_food_garrison_wheat_harisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc05a0155bdc7394beae39b2926b09c6b29fc2d030b5e2929b747feaa5ec7e58"),
		new("component", "PreparedFood_Catalogue_medieval_food_guild_feast_pottage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "92b66e80bf860bec05b14e0d73448fbd242b122a2b34770e5ce57cef18898252"),
		new("component", "PreparedFood_Catalogue_medieval_food_kanem_court_sorghum_meat_pudding", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6dace833b140ffda808d4571451617e7fa685bc4b644bc3856f7c57cc9fe4a13"),
		new("component", "PreparedFood_Catalogue_medieval_food_narbaj_pomegranate_meat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da8a51f525bef15ee61307baf81692439d99affedf50b4ed26089763972ade37"),
		new("component", "PreparedFood_Catalogue_medieval_food_raised_meat_coffin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa4b807210db6e099116c071b2e7c4956aae1a2c21fb5f95dd4c72d33201d199"),
		new("component", "PreparedFood_Catalogue_medieval_food_sahel_royal_roasted_meat_powder", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5ab17d7088aa38e4fcd16975e4ec1b56c9c00e5167c37fdf3d39efbcc2b1223"),
		new("component", "PreparedFood_Catalogue_medieval_food_sikbaj_fish_fast_variant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8460c8e4dc94a187339e7e97e2c7b0a4dd1e8873722ba2f60ea559426b77c3de"),
		new("component", "PreparedFood_Catalogue_medieval_food_sultanate_spiced_meat_skewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5be2fa84cf078a24eb40699a6f204d548510e7780c63214bee1a6f88cdf5f374"),
		new("component", "PreparedFood_Catalogue_medieval_food_temple_ghee_rice_offering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3664721ee3ae015cf6aa44ec8d182ab712332a3e1f7970bff0c8d92b268b23fe"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_acrid_burnt_grain_ration", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0fa80c0ba3cdd4a667e03e9b423fd5153534fb5b881029be353aeb719ea4398a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_amaranth_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6f046c71dbb18eaf94407a4b3f6ee1433fe14e58f225bea95877f8b493bfc3e"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_anise_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dc20e7016225d601acb66bce373d1c907ff46091b5d6c02b2cfbbe63cf4d844"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_barley_and_leek_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58d3e834d1e3083cc8046a2c73397ba9247c28a4c0d062319ae6914ce8d0df4a"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_and_cabbage_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec9789e44185172d6dccda091cca934cc2227c90bd02459ce72d84452c366377"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_bean_filled_steamed_bun", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4776e5f9664878ac925678e35fdd1c0c8a0774ea8f187d21076f44d64c97736"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beet_greens_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fe9ca23b1cacca1fa84395a38444bf55fe28543b9e4728fa8e037cbf56e33fcf"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_barley_potage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16ac0ee92abd6c6f8eda7d62531f3197ce0e31966cc0491ae57e00d0ff53e40d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_beetroot_greens_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f4a8f30fa9909e8e844e7a04f11b008a4cacc5e52370859a6ad589d967e439d"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_berry_drop_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f18b55d7ca1f72b735362f5585a74b10e9c59a2a7bb88efc228a1bac71e4888"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_chickpeas", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dda115f60d9d620380caa2c331401b9f8377d8b78d00c2d1c3d6bd746f68fd9"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_fish_roe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa43338df6ab38852e80c3c0bcec10cd337f926dc7d9ae22bba4a7de04fccd50"),
		new("component", "PreparedFood_Catalogue_preindustrial_food_boiled_goat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "33606bf7079671e51698e1a661fb5318a9b5c344aef048f0b9adbdf4ded9508d"),
	];
}
