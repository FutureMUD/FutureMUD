// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch065() =>
	[
		new("item", "renaissance_military_takouba_straight_blade", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6e0c61be92bce078fec22355daf9734b61d573f85b0c2d13b8a829195b5b4d69"),
		new("item", "renaissance_military_zischagge_hungarian", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "457e86caf1f9612a22594870cfd0e8e5478fd1c1cc2828f51d7fb87c9ca15a40"),
		new("item", "renaissance_ming_long_sidefastened_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fabe1fc2742648274d3a39da82f71d5408a3e74ec1843ef3633780e2ba68a91"),
		new("item", "renaissance_ming_winged_officialhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1afa999806677fe97413d822dd264f4c457710bba304f92e2c83eb0d18b4b3f5"),
		new("item", "renaissance_northamerican_soft_hide_winterhood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff1c14a517d3bd0539ba0f707d70448f76a547d67d39566ba2ddd73b3d7c43e1"),
		new("item", "renaissance_ottoman_collarless_inner_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "89c57d5ccbb2d3fb01dbf07e6d644f7e50eecbcad39f9c249d4593ef408be813"),
		new("item", "renaissance_ottoman_dervish_wool_cloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c40d3e40d0056fd9ad3839a2d93f4af5681b86886aacb5e2e0c259a8e83390f"),
		new("item", "renaissance_ottoman_full_salwar_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "84bc4dd516d20e2764643bba9da6a1eaeaf51200ec41de7674c2073b849bfa79"),
		new("item", "renaissance_ottoman_small_court_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d35380f2e5e773af0b66b78c0f3575a326bb3d31f76175c071ff4189a19647e9"),
		new("item", "renaissance_persianate_hardsole_ridingshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbd19eb46f535ee5d0df9ac4df9dc045ba7c888dbbbbe297789581d2730deefb"),
		new("item", "renaissance_personal_elite_household_fan_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a192344a61046fb62f0fe9b849c12c63fd49f3a3b112ef0c944095a05d393c08"),
		new("item", "renaissance_personal_japanese_ryukyuan_brush_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8387472a2dc0cea4e6a64838925a8f2be7c83e071ec86b698360c546ade24342"),
		new("item", "renaissance_personal_persianate_court_perfume_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d3e89c756d308be25aae7a00bdcd2debda14371a56237aa4997a724a528d7b2"),
		new("item", "renaissance_personal_persianate_court_shoulder_bag", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1b9ae94abc3fa0e5a31521a322b91f2510aac9ab520d91f829540256205cac37"),
		new("item", "renaissance_personal_sahel_redsea_indianocean_manuscript_satchel", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c3e1114eb4c1904dee43b513e0ffc1f34b61046a020bab6d293a0ac3abe98af"),
		new("item", "renaissance_personal_south_asian_merchant_betel_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c17cfed212a1016d617a07bdb1632d632a3d97af38bd350aeb22ecf40effea2c"),
		new("item", "renaissance_personal_steppe_caravan_felt_shoulder_bag", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "54a11b7c8557484486dabc56fe685cafe11ee36112a9867ab2c96fa700f40581"),
		new("item", "renaissance_personal_tavern_inn_servers_tote", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "762997027ad9f7255377dcd3ea2259f06e9aaf8b26b932c4b72ff4531d842bfd"),
		new("item", "renaissance_personal_urban_kitchen_market_purse", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d55d37616f044065260b6a58ea8730b0c9230a2cd20d7114d6bcbdfb94f8797"),
		new("item", "renaissance_personal_western_counting_house_cashiers_purse", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1324b2ecc204ac89938bd1c26ea9a729bc79dad3d7cc4bfb911a286e1e6a4c7"),
		new("item", "renaissance_ryukyu_pleated_wrapskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65197294eb2933406287faa6d22d3279d1bfc2bf98870d732cc9b9768ac10adf"),
		new("item", "renaissance_sahel_narrow_longtunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9cca6bf90713e58cd71f915c5be02906e6a0f0b134bd5386c8518a32cba69e8b"),
		new("item", "renaissance_sahel_soft_ridingboots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8e258f39fab193923b089502bfc4e62e14860aef543a42ab18eb1d55d1a0f30"),
		new("item", "renaissance_seasia_full_ceremonialskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0cff1c657331f77dc19cb9a1dbc1ab26c06faf5c58f8a3607c27945336f1e9a"),
		new("item", "renaissance_seasia_short_collarless_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee9e747db99f55d6140d26dc9b7d4274078d1bbc60df4a262ff886ca6cd0d74a"),
		new("item", "renaissance_shared_clothing_soleless_stockings", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "252e472373f69d73c0dd0e371f0244070f226df20a8a0bce581ec2f0b70edb83"),
		new("item", "renaissance_southasian_longsleeve_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3243bd93f7d91118d2686022e04cd78429c9916da515d90c6559547e5332755a"),
		new("item", "renaissance_southasian_panelled_longskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02c1c1a25e8bcb3c04ef356371b3b91caf259239aa0d51f0286de5b8e0f40e0c"),
		new("item", "renaissance_southasian_toepost_woodensandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d5c1affe8280296db06c60c1639d56c9e20dc31b961dd4067b679838fad27e6"),
		new("item", "renaissance_trade_east_asian_literati_porcelain_sample_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7391c1531dc712fe49b19e17d13446de20b86f0b029f3d55c5c57d0542fb2bcd"),
		new("item", "renaissance_trade_steppe_caravan_felt_lined_food_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "733ba19757245dbf0711a4a98344dab463cf567b6ae0fdb0aca8f9e1c47a68e1"),
		new("item", "renaissance_trade_warehouse_dock_tally_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36e8c4d014b0e2837474a470e2e4b178e08b57b8b21a538b15613efb94a2e3a1"),
		new("item", "renaissance_trade_west_african_court_brassweight_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c64def2671129078845c983a7ec05fb6791b6cd551cf026ccd1632c70ce2c71"),
		new("item", "renaissance_western_dress_apron", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9377389ad2b5a6e849bea32061d75a8231087252cd09cacc8f6439328cd9b7b"),
		new("item", "renaissance_western_furlined_lowshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbc42b2a81fa44824fbf31a81562927b43b678ece94ed70603322ac7398c62d4"),
		new("item", "renaissance_western_linen_coif", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72c1bd4a2896d9be375e657211e766a5a84c152b9c6bfb2651d00dd82d1f8105"),
		new("item", "renaissance_western_pointed_attifet_hood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bfe518fa8306ca94aa127fdcc33dce3e77e88b11a6094874009ac100d1a0e38"),
		new("item", "renaissance_western_sleeveless_overbodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82b662a50b65eda639ea57bb05b2c67a2a275a24d7453ba273084e1d07c53cfb"),
		new("item", "renaissance_western_stiffened_pair_of_bodies", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c85eca74fcd5ad03d498698ed273a4d7a573b0d4b43217282cc28497b687475"),
		new("knowledge", "Ancient Common Clothing Crafting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "22f10b9a7f64c4872c1f0a59f0e81cfc4441e93e0b6c1cba2536aea065c3c285"),
	];
}
