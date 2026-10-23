// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_shinto_priest_hakama", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b2a70ec6037bf3358afd50b9c713ea5f52bf52defe646e7f94b894e7932cce6"),
		new("item", "medieval_shinto_shrine_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a3e225d1d198a73c3b072ab560626804345ae51da95d1bd32757872ad5ba718"),
		new("item", "medieval_song_china_cypress_brush_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd6e8aaeb9530f2d4e625d5e3d92e60ef668e55e6507074c5b87e04ffb4d5d40"),
		new("item", "medieval_steppe_ritual_felt_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a17aaebe10e11f258f0de7d642e1fb32cd896a298e4dc2718edef6b2cdebdaf2"),
		new("item", "medieval_steppe_turkic_mongol_felt_lined_travel_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f5404d34fcd1dac688cc985ce5579f01ea662b3f1fe99dae3cfcea93e7ac795"),
		new("item", "medieval_tableware_ale_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d5ae76a3e56e700b456c0378bb9d849739c2724f3ef078bcf7dd4e9ff819a08"),
		new("item", "medieval_tableware_brass_serving_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc42bb8bacb3e61284f68f11778a95ca74b3a2320351e69b7aa66234dadecd9e"),
		new("item", "medieval_tableware_earthenware_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "11a384a74c566dc9463f7b0fe08ae1dc6a0f87023b91b6ea9d52554136ead400"),
		new("item", "medieval_tableware_fruit_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "39ff188d9ceecff93c6442b23e4da9dc0528dc4f06013c633c8065a4515a1671"),
		new("item", "medieval_tableware_salt_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd841912a4de6a01f86203767eb22cfc536255b63448ce708ab40ea6e4415b3c"),
		new("item", "medieval_tableware_shallow_beech_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fbd5b58d1a9309abd326fb80ad958a8832099e3fbea7bd4bd47a24821d45c96"),
		new("item", "medieval_tableware_shared_islamicate_urban_wide_couscous_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ac573e852e6bb9b3c0095bad4265cb1589f194fc9fa29608aa1102904326f04"),
		new("item", "medieval_tableware_small_bread_plate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "861959d0e3f079051ae1052da1207238936ab9f177ebd67ca0313eb58e739d4d"),
		new("item", "medieval_tableware_small_wine_flagon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "84d0bba5f824f4a075d94ae32a8738e0dbb5050fb531a2fbf48026042ed57e0a"),
		new("item", "medieval_tableware_table_olive_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f51ec0aab43dadc4aaa60572d910b833af07d356af90edd32237531d7c1d148"),
		new("item", "medieval_tableware_wooden_porringer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bd3f079986617feaa11effbf12adc1ea956f5432aff584f30b445c6cb5ed0a8"),
		new("item", "medieval_tool_ball_stake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d64e2066a3e9de84f8f85777efec03296abb202c4363c56d3d7948aea00046ed"),
		new("item", "medieval_tool_beater_batten", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5989b62c9d3b735a8ee9c17c6f01b0f8568b952610c14a8d0d6e3b1376f048b9"),
		new("item", "medieval_tool_bone_saw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90fc982f656d7ec6c984e58b4e44c3b2918218eb6a20a9185e840b68d7d31b54"),
		new("item", "medieval_tool_charging_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ab793fbfc0426084bcd1957ca9575ef43e3abe8bef5a651c0e83bfac36790e3"),
		new("item", "medieval_tool_forceps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba113ee715ab79a9a1afe00c97fe25d46d9d9f2c61e458d6f9c402fe4231c8e2"),
		new("item", "medieval_tool_mashing_paddle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b021a9306f94d22aad1cef59c5fb2e6a94bddd636ffbb1a1731bcfbcd378f13c"),
		new("item", "medieval_tool_masons_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "90a912b529c368f70b06f1dcde3c5399018146f9449896f4082f0d9ee6831c2a"),
		new("item", "medieval_tool_masons_line", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "338b67b0cec55cccc2ca95c5fcb5bd610f0cf7ad410a791e600ca54df5b22d50"),
		new("item", "medieval_tool_planishing_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bec1eab5f53a54191f7c940f8de4ecc1c7152e9c910ee67ddff683be62783327"),
		new("item", "medieval_tool_plate_snips", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "241d16b0d122e69ecb6babde97f1744eed92e341e84cafee9c97732d8d30157c"),
		new("item", "medieval_tool_rope_hook", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d07878d888ebec5593d824138a8b3e78c1ce1d3691ee9511a03fc8e7969ece52"),
		new("item", "medieval_trade_charcoal_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "55c0cba0c2311e1de820d13cb58eed1618810f4c6465597b1ae67bcd85e2deb1"),
		new("item", "medieval_trade_earthenware_seed_jar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "20a7eb28f2275fe26deb1f4924f9d566942933c782b78f33cf7b39760c14c3bc"),
		new("item", "medieval_trade_glassmaker_cullet_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8565c897fa2a01ad2daa30f3c4586a760586be44496ae84eda3ddab0c608f198"),
		new("item", "medieval_trade_lime_powder_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "250632351f4de1bc6be3f54fb1fe9805b58a4afb1a701ac5fd68e365fc7fce33"),
		new("item", "medieval_trade_scroll_shipping_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c939de3ad1f40c2a85b99c385f66e940963a099fc4354330c73facab29a01de"),
		new("item", "medieval_trade_spice_display_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e913e9343ec22f9a45daf1305e50db39b2a4199bfe16e9c92e6318b7dfa4ccb4"),
		new("item", "medieval_trade_wax_block_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa58dfad037af774a54637db303e453c034e4e243b0e3d54d551bb2242980aab"),
		new("item", "medieval_travel_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1db3c7bfe52d5f7475dffe79d55b6f04c0399228524810c0325723755701c2f9"),
		new("item", "medieval_water_east_asian_bamboo_spring_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c10121a2d8b2cdce82dfe06589c530fa5ee15bffb6eaa7f994ea7f809ed41875"),
		new("item", "medieval_water_islamicate_ablution_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0672c8273a7ea116244799f0693e3f4beaea1d493c85eb0fcccdf710fbc46ba"),
		new("item", "medieval_water_islamicate_sabil_cistern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6113e1afaa4b3cad5566a73d66311fd6b0ae631ef51068453871c963209199c"),
		new("item", "medieval_water_oak_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "14de89786483984d62f46851f7661f6e598a51e39183e45f7339180993615d81"),
		new("item", "medieval_water_pine_plank_bath_tub", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "77bde81fd3c4ba3eddddd51e2dac2fa75ceef235451d657d35660ea0c94891ac"),
	];
}
