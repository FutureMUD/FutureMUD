// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch055() =>
	[
		new("item", "preindustrial_food_pickled_trotters", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1330ef3909297a7806268066a48f570ad7a62964ae2a0da13ef26902bf30de9"),
		new("item", "preindustrial_food_pickled_turnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea86eef8b7599c9fc472f3cc01975e848de3ec151d589c95268df3fa62ea603"),
		new("item", "preindustrial_food_pilgrims_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "698b3212bc30d65438879db3ef6387aa70284bd216d30191bf3051a2c4317cc9"),
		new("item", "preindustrial_food_plain_boiled_chicken", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3cbf2509e4b7194197fd1ea9cd1fe02b03bedc8079c8242d69cceb7014ab098"),
		new("item", "preindustrial_food_plain_boiled_duck", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc9ea2a2cd25862a4c030f798e7fa4e9f67022656f0c3e7920a35d1ea9b2e822"),
		new("item", "preindustrial_food_plain_boiled_hen", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c07e4bc0343d0cb71d413d57bfcea5750c8ead2ec5e6e7584f261216ee336657"),
		new("item", "preindustrial_food_plain_boiled_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b8db997d5fa5a6bf2a7c139b5b72dd949039e0acb132c23623d92959a34458d"),
		new("item", "preindustrial_food_plain_herb_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50a4ebc8e38b60eac82842ec28ad8f5bd05695319c83b2be1a397cc4fbf2dc64"),
		new("item", "preindustrial_food_plain_salted_mutton", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4704bbbf9802359c37ca4d8275ef51246c4a58c0e38fa1722179467e87580018"),
		new("item", "preindustrial_food_poached_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfdc2673670b501205d840c6eeaef7c4e93f5b7f54fa2a5ce6a572cb9ec7a222"),
		new("item", "preindustrial_food_pork_and_cabbage_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdd513ee2b4a2ed15826400a323f3ecddd06b187ae9d7384b38380d90a2f2b93"),
		new("item", "preindustrial_food_prepared_sweet_cheese_filling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "447f4fbaab3a48ac5a728cfbed41facfd4ba05ae07bff8785d5577ab7279232e"),
		new("item", "preindustrial_food_pulse_grain_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "de0ce6ec0f91cb406fd50e116d3316ec53c0545ac714e0ee524a6883deffc2a3"),
		new("item", "preindustrial_food_pumpkin_honey_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b812d24a90b9bb11e3d0eb889be1c6f4f7c97e8304757bf6df0bd5e5cf7d5df7"),
		new("item", "preindustrial_food_quinoa_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f2f8575db01403ce517c6c6e830d57eddc517f173c84d096c29ebbe528e5bb0"),
		new("item", "preindustrial_food_raisin_rye_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b0365ca0ee798fc4203fcb261ad99b09eea0d36112daaf0808a0721b1ce6f51"),
		new("item", "preindustrial_food_raw_egg_custard_mix", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "47b3a63c7d783603d24d7dd10ba4eb41d3d81a225fc9166b78401ed2ed34c8c7"),
		new("item", "preindustrial_food_red_bean_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "542a4700eab4fcea3e85b60a2500f6f293e15b8e89a71d3d3d13a06b3b627717"),
		new("item", "preindustrial_food_red_rice_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39937c55061543ce9554003586a510b637308ae2c587c9e4dfd9eb81873af6e8"),
		new("item", "preindustrial_food_rice_flour_oven_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ea2c77e771b84fd9541fc5eb14fa47f100f35259fe7da2f05822257c9b43316"),
		new("item", "preindustrial_food_rice_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5ab171bb1b38961f2c8fe7675c834e832f3580632ac7ca4f208721f09be369c"),
		new("item", "preindustrial_food_rice_noodles", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1539caf07617f6a6c586b39ef9d4b11ec3ca6c73534ea53f392f0d2ef54e7737"),
		new("item", "preindustrial_food_roast_mutton_joint", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d981d257231a8e4e5b6b7625c5c0de3c37e9532a0301c3ff5160237abe450b37"),
		new("item", "preindustrial_food_roast_rabbit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2761f8a23bd3f4b90dd35492ecd1c5bd47c24988edcadc1c76aafe3ce2e2814e"),
		new("item", "preindustrial_food_roasted_chestnut_bowl", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac24b8987d11ef65d39a6c931615a0cd6a6c7313a0791fe7e8b7c1505fef27a6"),
		new("item", "preindustrial_food_roasted_fish_head", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8afca995325b45591190a5bde484c8d3aba29bfd3648988ea5fd75166c857145"),
		new("item", "preindustrial_food_rolled_stuffed_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ded3afa96480600281530e8b117f2c7ada0c2c3730bd03f39114bf260cfd8542"),
		new("item", "preindustrial_food_sesame_steamed_bun", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03c3b9d2db9528307f3cd4fdc6bab45917ff272c97a3884d452817b73e71cb29"),
		new("item", "preindustrial_food_shellfish_barley_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3a290aeb1a4fbec30f7cebac4a7096bdbf844b63034e1251855431ba88547b4"),
		new("item", "preindustrial_food_shellfish_vegetable_soup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a569a7338cfa692e6c4c80dc13600e5e2f2ecf6d392334c78a455627f950eb82"),
		new("item", "preindustrial_food_simmered_fish_head", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63f5ddad28e85181e86e61c39dc33fe6e9b318d955f12d683da8090559577bc7"),
		new("item", "preindustrial_food_soft_millet_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11945e0a2e1b36a92e825149a6defa0da060bc6ec94b88675ceac769792fa493"),
		new("item", "preindustrial_food_soft_ripened_cheese", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ff7e5d1d97b4837bba643eb440d5f31f70f02bc194ad927a0f4c58bd6fb94e7"),
		new("item", "preindustrial_food_sorrel_leaf_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd0ebe2eba98df9f170c3209a5b873d709c20a00fcc0137718d56a2387759e51"),
		new("item", "preindustrial_food_spongy_grain_flatbread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "282e306f9ad135332a864895bfc8ea0f8bec796e5aacc8989e4c210a74aa2cbc"),
		new("item", "preindustrial_food_squash_blossom_parcels", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c7e5faabe4eba63f7ea8f3496dbdf91963b392a65a91c7761824385bdd996a6"),
		new("item", "preindustrial_food_steamed_bread_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cdc4973364f78fd46f69daba4007101f50bbda5212091fd5c937436f67aec87"),
		new("item", "preindustrial_food_sugar_crusted_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb9309e8f096e713ca7bf08552adb212f7039869593d0c1ca103b866e449eabd"),
		new("item", "preindustrial_food_thick_mutton_pottage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d72435408f2df574c75700336930a8a829b193c70fdea8fd65937dab06bf1ca5"),
		new("item", "preindustrial_food_thin_ration_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d202fb92a0aaf4776013503e1ed1970d7076900a019335118640afd728e6e715"),
	];
}
