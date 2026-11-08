// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch051() =>
	[
		new("item", "medieval_leather_stable_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a64d512c85913a2be3356de6bef9c922ac7ba7ac03aacb9bd85d2a583934a35"),
		new("item", "medieval_light_bundle_of_brands", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5d83d56503048fde5912907fcf4c234f486374ae026954a4a3f7c2fc12340c8"),
		new("item", "medieval_light_long_beeswax_taper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e72f11f2f7d4a5c07f09262cc957038729b0d2cdf0e4ddb79026b8a7259c076"),
		new("item", "medieval_light_south_asian_terracotta_diya_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b27c335dc04d2874a2140e309435c1ad4a4923f6bdd6868dca7cb3b92e9226d0"),
		new("item", "medieval_linen_kamision", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d40ed197b051177d38a459629cde3ed71e7e6c4c2dac490e237ec299f4ade9c0"),
		new("item", "medieval_locking_church_reliquary_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "94161519cfc3e6a0cfdcc92083b07a983fed12e876f3d391ab8cc557b4473e58"),
		new("item", "medieval_locking_domestic_bedding_lock_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "65bd0498fd28b57ecc88ad2b6ba0d3dedc95058d24ecc061d1beb6b712a466eb"),
		new("item", "medieval_locking_islamic_quran_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5da839445ed16729403e1f2979b10922ba299f5c76751271f2ae52c87c236cf7"),
		new("item", "medieval_locking_trade_caravan_pay_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a98ecfbab358746214644ae0c0d2bb0af9fb57b13b5bd58b60407f0b3f9662c"),
		new("item", "medieval_locking_trade_merchant_strongbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9fa3fca3de44af81a25d7eb7347da9fd57cc84d4d40fdf2164fe2fc420a991ef"),
		new("item", "medieval_locking_trade_tax_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d25025ef68575372d503cd4cbf6c65c623221970a235880ab48f26726b6b8f54"),
		new("item", "medieval_market_izar_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "17e20cd97731aed5572f0c4eba5009f39b4cc88a1828beeb157b4481ecb72130"),
		new("item", "medieval_medical_bronze_ointment_spatula", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "610ba05ebc714e550f01334cc86fe425afe1ba9ae0345ececa940e0df9a1f790"),
		new("item", "medieval_medical_field_bandage_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ed2f2a512cff2c2b6e3fd1d313e048419ebec4941513fa184f2099bf96a0ab2"),
		new("item", "medieval_medical_suturing_kit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51e61500e93d6b44d3db72e894caab49c32c2fc5937ba1ce9932cc09720e85ee"),
		new("item", "medieval_medical_wrought_iron_surgical_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6985eb346564b24a6b5626c639dfb0c823402b301d2c212053d8f35be2fb6f8f"),
		new("item", "medieval_military_balanced_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a98c52c038947096ee6eae8c5e1e084d5d732befb5f8bae663d795ba3bde653"),
		new("item", "medieval_military_belt_bolt_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "caf95006db480565d31d9943f3cdc6819cc2e9b46a8f0aa5a6eccbf5921e09bb"),
		new("item", "medieval_military_boar_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "66d291ae57e32209bc814a4c3bfd39eb1aefb81ae123fc4d80cd07c9165b7950"),
		new("item", "medieval_military_boiled_leather_cuirass", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aec7b41292095f6bfc1da30b1d8ed0e8d016b36b746a3812c4998f2aaf9dd77c"),
		new("item", "medieval_military_buckler_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00900bb40b0eb2cca706593ab4c36f7e82b831370efa5442bc5e94ff4c641b19"),
		new("item", "medieval_military_early_poleyns", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "956b1112a4d591f7cb031697b93725bd2a76243df0278312259f38b521b1fbd6"),
		new("item", "medieval_military_fine_steel_fitted_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "33b21bccb746272b6067b52096132be987ea3c257e46ac47cbbcd2d4c7ee4cb7"),
		new("item", "medieval_military_heavy_oak_staff_sling", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "52c41f1961d1b919af4d7590adcfb1fce2392624c03e5307902a4f22e9518306"),
		new("item", "medieval_military_horn_lath_crossbow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5c8c22883ad97eea6587d0a927d71750551467924c787229fe2feae181b730d"),
		new("item", "medieval_military_iron_collar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae7dc0fa0ad28699c265a007ae279d653863265b53f928c67a73d6b5deb87bd6"),
		new("item", "medieval_military_lamellar_spaulders", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f494038499485880452a758202431311784aa9e21bb329ec40b9be9c9cd7f44c"),
		new("item", "medieval_military_leather_flanchards", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0d200da2bb5eac44eaa858ec5e8b9480e28c78bc23c9e393421a585adfb29d9"),
		new("item", "medieval_military_metal_scale_corselet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "93b763cf012667f15fcd591d4eee75993a90840bb7a06f585f762f4c1bfccf06"),
		new("item", "medieval_military_padded_arming_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f10af17b603928d7289941b867bd13a6ee6f4f24af85b989cc8e791783dee2e1"),
		new("item", "medieval_military_painted_round_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b44004241d23f07f6168d371c9ddda45c1d40f0a0aab49a56d9c3fb8f582251"),
		new("item", "medieval_military_plain_infantry_tower_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8a6bc3c4d168bab0d0f8ba5da5467daf7edc7af17f9265abe94d1cb91f29c0b"),
		new("item", "medieval_military_plain_iron_mace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa761b70934edc84a3c833e1b6cebd33a8f06ce156e02ffa6affdef9896b25db"),
		new("item", "medieval_military_plain_wooden_buckler", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4864f4ba36035eafda533f52337e13e63a3db1ddf98fc467c7be3e0c4c7172c1"),
		new("item", "medieval_military_reinforced_siege_pavise", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "48854f69447d1f40023f625a3b289973ae266a492f1972df24a0faf359495c35"),
		new("item", "medieval_military_rough_throwing_hatchet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c5c4258c3b0d43a41ba71f95695e14ccf6ae5a9e1b1d139fd09fc5a0e70b4979"),
		new("item", "medieval_military_scale_chanfron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "252dc73f8ccc4b18ddce00bb255bf66d88361f120ec690334c292c5425de2cb5"),
		new("item", "medieval_military_scale_greaves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c81ca9e1e4c4d47b3eb4b93d104667192baf6f1ffb5f5a5e603854763f2d50e9"),
		new("item", "medieval_military_spearhead_rawhide_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc5856a51b34ffad9307785989446b267ac440924b6a31f313356ab036bae4fd"),
		new("item", "medieval_military_splinted_leather_vambraces", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "559d4c24b0776635d92019f9ca9cc7a2657657d3586fb0fa841c91cabc25faa8"),
	];
}
