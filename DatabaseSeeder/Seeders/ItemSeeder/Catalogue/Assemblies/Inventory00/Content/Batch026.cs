// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch026() =>
	[
		new("item", "antiquity_bronze_workshop_hammer", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7620e724e1b49c361385cad76fffe0a1dc969eea96ec2b64a3186e0d8cfe1e8f"),
		new("item", "antiquity_casting_crucible", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "30ae8b115dfece92fe644b5ae366d60f973870cedeadf53e3cee8466b101597a"),
		new("item", "antiquity_ceramic_repair_kit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ffaf5cf8c24f928dfe88f84bccf0e41a39ab443ce4782d314b5d35ccfc80950"),
		new("item", "antiquity_clay_tube_hive", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "65fc06fca859a88850ac6db80843edf41cca4abb476a69132a41fa9f6de26320"),
		new("item", "antiquity_double_strap_travel_pack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "008439ccdfaab4663875dec63735c4af90ce94618b6505d046d28140caad8714"),
		new("item", "antiquity_etruscan_bronze_left_greave", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef013cd3070fe6b225a3aabf5bdef6a20b3d00e8bc812022ee440855d09a760e"),
		new("item", "antiquity_fine_checked_wool_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "727595e19c0ec5439a70df460b4b40610e93f452beb0fd64a04334fb93e3f95f"),
		new("item", "antiquity_fine_sheer_linen_cape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30ed01f75bb4a50a403eebb163524b7f55dc905b12e635129a1b26555a5cd669"),
		new("item", "antiquity_fine_wide_cloth_belt", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f17b82ebc50598036f00540e3080826824019a7a3ef04e5dfa82f826356eccd"),
		new("item", "antiquity_fired_clay_tablet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e05e36879d189fbfdc06c4d2ece79cd3b3ce72a97fd370a42f91fc86022cd31d"),
		new("item", "antiquity_flax_break", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6e1767bae334b2b1dbce9c97141275aa135eb6b6f097caaafc57647aebc43de"),
		new("item", "antiquity_food_celtic_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5070ebcb97f3144116f33dce23545926df7ee6b4e9e660a8bb17d363d1bb9820"),
		new("item", "antiquity_food_egyptian_honeyed_pastry", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3b2ec83d4fc9cbaac105e6e942e34f13862974771ec9c36a07448469d0aa580"),
		new("item", "antiquity_food_egyptian_oilseed_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "630d0649116f7ecb994351f0a1e08d31e4992b7004a581f106cdc70e4106d42c"),
		new("item", "antiquity_food_etruscan_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ab301b5d604e32902e74682b1daf3ec31b57ff339b751ab4e28f008e6471c257"),
		new("item", "antiquity_food_etruscan_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d63334ca6a73b8a02c5ffe98ef3e8179e2588747290f4fa7a602ba7c6499eaaf"),
		new("item", "antiquity_food_finished_beer_amphora", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f4d03163d233cec2d2fdade00d8d90f5e72f546ae0559bb681ca182e0305bb4"),
		new("item", "antiquity_food_kushite_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "11819d929d57d205f1a3708bef0d03dfe82ce2c4206ec60381ce506e47b8156d"),
		new("item", "antiquity_food_kushite_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5527035ef89231ceae28e0597bd43aeecfd0a8e76b1a458c7c34952ccc4aa6e"),
		new("item", "antiquity_food_oil_press", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a167a36e7b2c1ff4a09327750744d2c771c3f8e986004bbadf859b35ed18248"),
		new("item", "antiquity_food_persian_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "eadcc3cf53d6518b53b27b09ac520413454de53c25f69436ba7752a872273fdb"),
		new("item", "antiquity_food_roman_fish_sauce_relish", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5a5c6b3ca7ddbc339deabc615e3363653f7effea9390a694075a1860b0791bf"),
		new("item", "antiquity_food_roman_spiced_meat_stew", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "44b1b95b25b3d9dadd9fd72c4cd1d1636fe69d4994816a03fb2a2af8a10f3587"),
		new("item", "antiquity_food_scythian_sarmatian_stuffed_flatbread", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "01b3c84a2aa0955643ab004c19a68a011eff7d4d125ac489eec128a4d0820740"),
		new("item", "antiquity_full_head_and_neck_veil", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d16d73ac26cd3916333a59e492c336830a338308961d97a318b3e5fc7ca8edd"),
		new("item", "antiquity_full_wool_himation", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf268e958822753f0071449e3d0338be35cf265421106dee56c4d29fb7641a86"),
		new("item", "antiquity_fur_lined_wool_cloak", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee6a4f89a3cfd6d36bd325a97f920800028be461cc80d1e3e1960b31eca032cc"),
		new("item", "antiquity_henbane_fumigation_cone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c50539f763ec0eca74e9d6d70d2225da0487c575acae5930b9d6353bea2b8719"),
		new("item", "antiquity_honey_antiseptic_kit", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1eb9bd43af86645d52e940d4b2e3a7848fb7e2e3dc186d4ae319e3b2db2edbd8"),
		new("item", "antiquity_liquid_bronze_situla_pail", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8b99d7b2d63d5f9531d1d4ad8f39ff99efc0b09efbbd46f55773880f3f02cce"),
		new("item", "antiquity_liquid_sailor_water_skin", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d113178a5152e8810ef2e58ed562cb062fea669dbb017bd73a41f4ff7ff7a4a1"),
		new("item", "antiquity_liquid_small_coopered_rundlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7541b884723c31350a2730ca4335c94b21259830ecb14f121fbd1e383c938d1"),
		new("item", "antiquity_liquid_steppe_bronze_kettle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cff7d6001cbd5b00e5cd1a21e3cd64814ecea611c5972f76a7ba092348d62fd"),
		new("item", "antiquity_lockable_acacia_courtyard_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "025af25cace3aae1efbb5dce5d37339ef184e27339c3d2cdebec9546fdee59fa"),
		new("item", "antiquity_lockable_city_sally_port_grate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e2a46ea909bd1515e35a17c8d563b60298117d9085619e6cb64bab9dfe0a28b"),
		new("item", "antiquity_locksmith_probe_roll", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5ccbd4042fd1695cd58882586635675ce3e9b015ec382c9aa43967362a5eafd"),
		new("item", "antiquity_long_hall_bench", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3fff287dd0c07974f16493af2f418dc044859ded0ad92b3974704597fb380a4"),
		new("item", "antiquity_narrow_wool_trousers", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c595e9e5c28ef166bccba9f2c096653163ab5d522c7276b4c796e75fde967e9"),
		new("item", "antiquity_oak_house_door", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "278d2e76b388f25d26a657d6b45e240cb83f272d015088ec9d39d890503569b8"),
		new("item", "antiquity_oak_tanning_beam", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57047722a94bddfd1fbc00790b13a8914055ab647084b095555a0b7939dcd579"),
	];
}
