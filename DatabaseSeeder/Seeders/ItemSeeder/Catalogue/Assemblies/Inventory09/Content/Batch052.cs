// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_skaramangion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6f21c88430e6651bd5c461116d68b8bd3e38c3dbbb9ea4b2798b3ee85b475cd"),
		new("item", "medieval_soft_leather_slippers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7acba4874fd5c5259ed75670e17235fff557c1ae29eb84fdd0efdea068e564b1"),
		new("item", "medieval_song_cloth_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9023d30963e66fad34876dc3f202c061b74a0c31bc5eb7bbb726087476dc361f"),
		new("item", "medieval_song_round_collar_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "97306ff7cdd2c4f25bed798aece18d0076cdd8abf23fb8901f1dbc6c9d88f18b"),
		new("item", "medieval_south_indian_work_veshti", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfba6084754ff427d4a150589fd955681c06067035b688c1c3056f9f9366abee"),
		new("item", "medieval_steppe_felt_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecd275b3d7a7f59732e8ac9adf2a1389ca6acd7685e61e505d62b7e9142f8c69"),
		new("item", "medieval_tableware_brass_goblet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a6f5f932d0ff686e6dcb4cae7442d52ff579460f20ad5fede6d647f282902aa"),
		new("item", "medieval_tableware_ceramic_decanter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "05eb419032fb921a34026fa8cdfe2bb7602b12070fe7099b06db9cd64dc5dc66"),
		new("item", "medieval_tableware_corded_wooden_platter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "999081c891097b78e55d8917501c5cb2c7450c484ae16c58d0f8df7300ad8645"),
		new("item", "medieval_tableware_fruit_dish", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb8a1875cbd452b3ca1063e49504f40d31f10d4666ec12cd56b73b2ee1690cb5"),
		new("item", "medieval_tableware_honey_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4086d7870aa23f34bf70465c1ec4f53a4df2bac128edb7eb52c8a5c945961137"),
		new("item", "medieval_tableware_pewter_ewer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b740ecbc38631d23c05c35552b6732d5b78e1bb964fc89ef41658ac06069828d"),
		new("item", "medieval_tableware_shared_east_asian_bamboo_serving_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce4367eef4923e1eb728fd71a049b88381bb94f49b70832ae213048562f63eba"),
		new("item", "medieval_tableware_shared_east_asian_low_lacquered_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d99a3c05be1f32e95e7344c15e5151c850d20e7b98788b711008f7debb80fb7"),
		new("item", "medieval_tableware_shared_western_european_monastic_refectory_trencher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d5b4ebaa08bddb494b9cf1fafdf2d278dbf85cfd213893909eb03628a1940a8"),
		new("item", "medieval_tableware_shared_western_european_pewter_salt_cellar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9eb3593800bc1b7bf0bc141ccfb4c40bd0a4e3651d0e87f0a66b9fe349e46c0a"),
		new("item", "medieval_tableware_stoneware_beaker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f2f418431694647b85aac30c371b801130ab2af5a1836dfb260ccab587635b7"),
		new("item", "medieval_tableware_wooden_canteen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b77bf06eb45121fd6a3932e223a85905c48c87bdca8a95db0d9067ccf0645df7"),
		new("item", "medieval_tool_block_carving_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7729834d6331c29461826177cd5b5988a783ad54557a0103610bb23c14174796"),
		new("item", "medieval_tool_felling_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "736e7ef61670c43ecfa15b5c2863482279140c4bd5419b368c946c278ea26087"),
		new("item", "medieval_tool_fine_saw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1215aadbc9e6994a54b32148d8556105454372c7d26ee9d3538870df1aa7e78"),
		new("item", "medieval_tool_locksmithing_installation_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "40719e84ca256e329989a0ba4e04131ca0198a4d32099db245e34f839cd67766"),
		new("item", "medieval_tool_raising_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d12337199d84db2482943a3b7126597034f0bcdeda1e12fd26df750a4d6aa28"),
		new("item", "medieval_tool_ruling_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "df0084cb15cb3deb750f0c038e2b17443e3889fc44dc59864ef6d13cb0bdff32"),
		new("item", "medieval_tool_suture_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "72e8dcbafd730041d3965d982dcdcdd83cfd67371772f493d047f07e99fff65f"),
		new("item", "medieval_tool_wood_auger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "59a23e6019e581f32999c1afe56e9860a65c8c398dd46bcb2eaf44a4d7d86ba9"),
		new("item", "medieval_trade_blacksmith_scrap_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dd3878192ef7cfe2229d8ee15ee0be44bf3b4abc5a64650907999c78ef5a475"),
		new("item", "medieval_trade_bolt_cloth_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b8cf526a99940dc1f76d43ba9f2d94c8d7981744f13978a8fa82561afc03b1e"),
		new("item", "medieval_trade_dyers_pigment_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "89d441e2be29ac76e9a7f7db6a85fb6cc53289fe1f91f88c8360806216db2078"),
		new("item", "medieval_trade_grain_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54ff1a2f1be1f87aac101cb62e4fb1b081af1392ec52e49773d8901ae9b9fac6"),
		new("item", "medieval_trade_lime_cask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da39da9d6b3101cd05e510207b4225e2de1a0ea7f8927d5bf117259ba0852ccb"),
		new("item", "medieval_trade_merchant_seal_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63a2e6dd31f53453718699fbceb4ec2f04e76e75bc6f28ed9423317f9c7364f7"),
		new("item", "medieval_trade_willow_market_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dac1ac20ca1b0dd3a288e79d7798ed1f42df6325560c559e6ec533f262feb5f"),
		new("item", "medieval_water_laundry_rinse_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b6e07d47e3307171c9e98e08a11ed1283ff358577203da844d115617182503a"),
		new("item", "medieval_water_steppe_camp_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f653f40a5ed821522e0094fbadb5c2d2904568a5b59cde8b46cb74323d2219c2"),
		new("item", "medieval_wide_leather_work_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "810e2d1aed4c14c2eabee716c38ee9da2a2c8336fed2f0e78d544a286f5d2bec"),
		new("item", "medieval_wool_chausses", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "082343000b6d1250cae258cd0cf650261e9da79f8dd0bee82c240ad82cd40e2c"),
		new("item", "medieval_workshop_fermenting_gyle_tun", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b62f95222473701a2cab7f13c3a79b4c4ff158dd3c47f2aad99c4cb2f3f9e66a"),
		new("item", "medieval_workshop_forge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc64589af5ad05018889c55cf89d7536dc1cd699824a529c5e9c6c6a2b2f3096"),
		new("item", "medieval_writing_agate_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c168c533bf4475c16288d8a802e06474378c13ff77727c0e0e1010c0616c9a3b"),
	];
}
