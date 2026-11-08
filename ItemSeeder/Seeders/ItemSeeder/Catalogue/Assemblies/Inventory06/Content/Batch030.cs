// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch030() =>
	[
		new("item", "earlymodern_door_mca_shutter_613", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1612f7345f28a47c5f44a06c24a672db27b024d276a7e3c56dfbd59883b4035d"),
		new("item", "earlymodern_door_ming_hinge_pair_317", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e71c73094798facbf91eab5f6561682bcdf991898c99d5f261f17db1acc25d8c"),
		new("item", "earlymodern_door_mrd_escutcheon_262", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb6b0baecbe10456eb322aba246efcca82ab6476aceab751574ff43d06248609"),
		new("item", "earlymodern_door_mrd_panel_door_248", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7e0ee3b0d734e21701a4c275f8ac31f7a93fd4d0c23b86fe7f3c44f9c079a8d"),
		new("item", "earlymodern_door_nl_lattice_gate_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "efcc88f0b579cbf1e0a39d5d8b080f97c8fb9b6490e2f01395c2428edeb78b08"),
		new("item", "earlymodern_door_nl_master_warded_lock_37", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "04b76cd4ed1a617e81641fcca8f5b6517216dadc6ce93ae8ddc48e86f4e38173"),
		new("item", "earlymodern_door_ott_keyring_185", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "df5f902f18c11e043ac809af191b656b6eb2982bab5a490cecf4deac29e94e16"),
		new("item", "earlymodern_door_ott_secure_door_175", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "636d2f16f0bcbb6bff2f0f6ee887935c931e5275267b6b5f4f535748771a09fa"),
		new("item", "earlymodern_door_rus_warded_key_171", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb52c6c94220b959d177013e8343030b0ff286657d42cbadc6de632bd4636a76"),
		new("item", "earlymodern_door_ryu_hinge_pair_374", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb8e36e2b75241a1205d34fd3e33d223564b37e6bf59e394101d707a9ab6713e"),
		new("item", "earlymodern_door_ryu_keyring_375", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b216f090677397b7459e7bfd57e0eb178d5d12b4dd94cd2766e3e15e779830b"),
		new("item", "earlymodern_door_ryu_panel_door_362", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "394c110992bccf0107e591ceac95370bbe1be0adf1f2761ee3c0b275557d051f"),
		new("item", "earlymodern_door_ryu_warded_key_380", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "29d3ce8e5b4dcc71df34e904cf9fb34325ff7883b74e17ab51d7581b659db225"),
		new("item", "earlymodern_door_saf_shutter_214", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dcd2f8530b08f94f6c77cfa25af20416f2a2f46554c026608cab7f5205385df"),
		new("item", "earlymodern_door_sahel_shutter_480", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fe3614a96427e53bc19877a02542686bdc4aff81aabadf9eadf961cd073df46"),
		new("item", "earlymodern_door_scb_secure_door_118", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f92f3d4883e2b453caf0840394f798f0a2cedd7bcebff75092e5c26292013ad7"),
		new("item", "earlymodern_door_sea_screen_386", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d94d0f55f040df3d06f539f8d9ccbe6862d767f8393490f58df0b57d3cc59b7e"),
		new("item", "earlymodern_door_spa_courtyard_gate_540", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c8f479a33ffc7fc66a8a68b942fb5a53b690fa6b35a30bcd8fb2a368a1eb4af"),
		new("item", "earlymodern_door_swa_lattice_gate_520", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60e2e8dfaccb6856a8fac3f78293476147f1bcc9a51fe6f4d659a895eabdb8af"),
		new("item", "earlymodern_door_swa_master_warded_lock_531", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac831eb20c55d42e01ad647ba1ce8341fdf1bbf6244001f895cd7856bf34a363"),
		new("item", "earlymodern_drug_calomel_purge_packet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "da013dcd7459acc15b61cf261f460f5bda1fc424db059119f57de4b795a0fcf4"),
		new("item", "earlymodern_edo_military_jinbaori", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f52e4bf111a24b0a1046ac9fb09ea8e14e1f60dcd711d2953ad5263f7dee77a1"),
		new("item", "earlymodern_edo_noble_court_headwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a81e355d3bdbeafe7b126fed37f415db0eea28ba5ddbb7fba7c628113c96d0cd"),
		new("item", "earlymodern_edo_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6263583adebc138e611340f9c61259eb3df20fdb7fe272371da92fe4c680fd66"),
		new("item", "earlymodern_ethiopian_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d1eeb586035c5d58d4e84db61a6d7d2ea88f502f87e7511e4d39b7e695d945f"),
		new("item", "earlymodern_food_andean_potato_cheese_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b575be391223439f2b7d1688141691ddb9e8a62a3e8bed2d396c0cc05eefd36"),
		new("item", "earlymodern_food_brazilian_cassava_fish_pirao", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee1ce1d300e5e2faa8b0b5f89d959195959d1b1b12684ee84bcbdf3a6c1589f1"),
		new("item", "earlymodern_food_british_naval_duff_pudding", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ef442a7b204805bf21fc078a723a05dd0d39485a1b073b6014bfe5a8eeb34c0"),
		new("item", "earlymodern_food_dutch_poffertjes_plate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bea4fb6d4a4934ed80e869a2a25ebd9ecb98cd6990f77943b45a57cd1c5e4a17"),
		new("item", "earlymodern_food_french_brown_ragout_meat", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5b66aa56170526263e17d2cd2603243e5306cd98020017a71995c93fa7eb642"),
		new("item", "earlymodern_food_german_coffee_house_kugelhopf", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e1b1e49760732a55593bf32043fcab7a6237d3eaf90ab211d5e7541e8d054a2"),
		new("item", "earlymodern_food_joseon_royal_neobiani", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cfd9517cfa13f8e2d87978213999f743f00115f0f1988bb40b19b6237c37955"),
		new("item", "earlymodern_food_mexican_barbacoa_consomme_bowl", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "460681ca51ad074b5f51937a7320338b39369d7fcdb1a261331e9a75506a6c0f"),
		new("item", "earlymodern_food_mughal_kofta_nut_sauce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1b1549984e9e40333618450b0b45578c83693e2bf6065a00761818e397459aa"),
		new("item", "earlymodern_food_mughal_tomato_meat_qorma", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "00b2249e29a1b065e4990b8ba7e1488ec7f9fa849735838391defb3999d003cf"),
		new("item", "earlymodern_food_naval_lobscouse_pot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "87ed174601110d24ccdd52137ee32b1e8ff4823d3c361a30afffd049dc55fa69"),
		new("item", "earlymodern_food_punch_house_salted_meat_pie", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f60d8e9efcecb6dfcb6cb73a230677455d8c4e22ad8f7302b8d14346f6353874"),
		new("item", "earlymodern_food_swahili_portuguese_custard_pastry", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb78f3a901083af88e81f698aff953113cb64f592abb02388b0be1010fbbc860"),
		new("item", "earlymodern_footwear_broadtoe_work_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "009268af0bd27c8bcc5bcc14d724650efa38cdac4b9332deae23b5dc8a4f637e"),
		new("item", "earlymodern_footwear_feltlined_winter_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "781ea8ce14d091604407af9bbddac4c46798b6a1b823e802cb842976fd0f72e7"),
	];
}
