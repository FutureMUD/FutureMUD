// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("item", "antiquity_charcoal_writing_stick", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b72bbc7c934c5ac0e455a4058afe10f11caf56df47c659eaf0b611eaf5aab83"),
		new("item", "antiquity_checked_wool_belt_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fa87f92b5b19e0904d11a544d41df616201a3550a159490a33f74bdd52a6d8f"),
		new("item", "antiquity_checked_wool_market_bag", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "32687e6ae59141b221a42e612796863ad606c995d5eddebe7a45776a4d2d8c45"),
		new("item", "antiquity_etruscan_bronze_studded_sandals", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bbc4c70fb3d47df04bac9686142093a13d0811a55e44b206444c68ddb45ddb89"),
		new("item", "antiquity_etruscan_pair_bronze_greaves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5c4358ba39d3aa6145e0922b798d1e461ed03f6f1e5cacc3b2a64bcf7db2f1b"),
		new("item", "antiquity_fibre_hackle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f547dbc59e0dcfca37d1140491f00ffb19bcbcadc76e4657b300807d7e6d678"),
		new("item", "antiquity_fine_conical_felt_cap", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1df2b33190e0644299354f0be5000ecb06064a2454ae11a9ec8b464582a557f8"),
		new("item", "antiquity_fine_full_linen_gown", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "64b4e7b49ea899b1098b29e9deedcc90d0234ae11ad743dc1528957e157490c3"),
		new("item", "antiquity_fine_long_linen_tunica", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cbbd569e5f85b0d645f730ddf5da8291a1beb0d073b168624bd6226b7b3d6076"),
		new("item", "antiquity_fine_sarapis_linen_tunic", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a075bccfb565466bad8746d7d0bd327be69edef7e66a17064e67cc23ffc087b7"),
		new("item", "antiquity_fine_toga_praetexta", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "74dfcaeef7a0279dd497752ca221512fa06acd6d3fc1aab671b36162f6f6c499"),
		new("item", "antiquity_food_anatolian_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "436385dffe497c031ad1dc7c0d6d35d495917bbe7afe59092258eea4f5446b2d"),
		new("item", "antiquity_food_egyptian_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f60ccc21290a0264366ccf2be504b20eedaa349a8cc0af6a96619a871ec3db9a"),
		new("item", "antiquity_food_egyptian_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "09a136997c75f891ba8657ac70e68999deba90eb200cc99866215f1c890628e7"),
		new("item", "antiquity_food_etruscan_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ad038bbedfe7c97f1aea09d2aa6a49dfe1713801f56eebd87a507c0a0e22715"),
		new("item", "antiquity_food_etruscan_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "63c11e6582964074173aa65648fa8a89f1e6b9fee23e2b06c552f3f2992ac40f"),
		new("item", "antiquity_food_fermenting_red_wine_amphora", "antiquity", ["antiquity"], ["item:antiquity_food_finished_red_wine_amphora"], ItemSeederOwnershipPolicy.RequiredRelationship, "cb767c4f09aff9d45795f3770d162b9e5144a336c42360af3302def1e235b3f8"),
		new("item", "antiquity_food_finished_spiced_kumis_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1121877c4534b3f50154048d4bf842af17b3ed72577866e05cdf742ec92aa7b8"),
		new("item", "antiquity_food_hellenic_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "128a5f98a1cafba745d93d6a62340839d3b335a9a515b026b9eff1bad64dcacd"),
		new("item", "antiquity_food_hellenic_pulse_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5593ceb642554c4ddd9f56e5aac5df8ec2fe0e40ce8f689b3abf18ab6228d1e"),
		new("item", "antiquity_food_kushite_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cde662f890e87b7ce6666bb8664a56d98f44ee298992f4fd16a97952d2a7455"),
		new("item", "antiquity_food_kushite_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1da04f01e42b04a5ba62da00989179b56e4009a8cbb248b1b78ee804e697115d"),
		new("item", "antiquity_food_prepared_fruit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d05a8d5b00ec9a444606411080bc3a0a8813da8843d7d54f8d10972afb54fe4"),
		new("item", "antiquity_food_punic_sweet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e820d0f76ffa58efd2781cecba85eef3f982a882382c651c23600e22cf14466"),
		new("item", "antiquity_food_roman_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "41e346d7060611c020b7b8d44f1f6a7cc1335c88d61ed8ae35d8a12a2893533c"),
		new("item", "antiquity_food_roman_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "502664c90fcf9a5a9c39e5fa511ee3d6e433bb005d4e2a8ce7e01fdc9c652eea"),
		new("item", "antiquity_food_salting_trough", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "67cd9e483e1219b15ba50ba5b359b081e0e16c87b4863f96d885b1736fe852d6"),
		new("item", "antiquity_fullers_mallet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1b7edefea42eefd466064411a44cbc4a2f98309ca55744dfa740e474b4d4891"),
		new("item", "antiquity_fur_provision_pouch", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dcff8d14362e63fcf0276d92346cd7d3cbeea4a1a4059b629b7f14036b6a663"),
		new("item", "antiquity_germanic_fur_cuffed_high_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee5a18834dbf10c73bf97d84655d10fe05e94795bda8d3f1de565cd651089867"),
		new("item", "antiquity_germanic_hide_war_bracers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "009526b439d187ca73dd3fb09e86cf02b1ce73a0df660109bd34aa9e60ea0a0e"),
		new("item", "antiquity_germanic_iron_mail_coat", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "76528e992c2ffeba581cab6252f5aa4147f8faa81c6b3651b96eaa24c68867af"),
		new("item", "antiquity_hellenic_bronze_greaves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "69d77f22b26556a4586dc5d7756b5c39d7c7dbc647f65c3b646cf7072cb04ed1"),
		new("item", "antiquity_honey_knife", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2109b9c46a1a7bf9903331a3a4237a1cc56be2663d025585b1f2b9125d552142"),
		new("item", "antiquity_hoop_driver", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8de57e62343eeac0e4917bd977498420b25a0821c3afc1d6b4312a4b5a6454ec"),
		new("item", "antiquity_ivory_six_sided_die", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "066ae6e5e717d4f9444a0e2998f8279222b637bbd6354d620b418dfcf588438d"),
		new("item", "antiquity_large_gate_key", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e076bf07b29d532a607940af03a6ca8d2f357e92b61fd9903298ab2b5bd3db0c"),
		new("item", "antiquity_left_shoulder_overdrape", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc69cb68834c956eaa8c147dd3c721cc39c993a7e2c053d7010be97de5de1c78"),
		new("item", "antiquity_liquid_black_ink_pot", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c86d18a1bdefd57f8cb09aa236a96e2f56e9e0b33641cfc993bdc6e44cf5e51"),
		new("item", "antiquity_liquid_cedar_stoppered_wine_jar", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fe7b532def059f95d6908094adca3ad0212698ff552a907134519c211cf8063"),
	];
}
