// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch058() =>
	[
		new("item", "preindustrial_food_plain_baked_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f9169ae42e9a6e1f116810602f495d0d69e6dece0c8decfc86dce2cf0e5c66c"),
		new("item", "preindustrial_food_plain_smoked_mutton", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40250993cf99a78394ab99dc915c6857e2c70d4a6694af9a0f744802eb858893"),
		new("item", "preindustrial_food_plain_sweet_custard", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5f0f76db1cc8ef4c7fe7517f1321c5eeea277614e1bc2e4e5c9001304b9cf82"),
		new("item", "preindustrial_food_plain_wheat_biscuit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a36d84f5d6c045f61557c491323f0ee9bd05e88395f1c894faef120e613850f"),
		new("item", "preindustrial_food_plaited_egg_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f750a7385ff2ec52963735196cb1f9830251614bdc251862f5f6591865e2c79f"),
		new("item", "preindustrial_food_pumpkin_maize_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51a9c196b5de07dbcc41683100f2bbe114e6dda70caf8eacac961ac8019b2dec"),
		new("item", "preindustrial_food_raisin_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b3b57dd76ea0635540e5670ca52148603f1ca8f5c88c3ada9663c9576524b61"),
		new("item", "preindustrial_food_red_lentil_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db673b45285728c95717a3a46dcd6fcc3b91ce6cd804d0aa2d874425a8a223f2"),
		new("item", "preindustrial_food_roast_boar_haunch", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5409630700be58f155bd527fcee1c5b1ffa70445387659adfec75a4393ac92d7"),
		new("item", "preindustrial_food_roast_pigeon", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3141587385c5d43b582a53f0ffd9ca5f9a1a8bd77c6e1fe9446b2272ac4c372"),
		new("item", "preindustrial_food_root_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f26fc1c1b78073eac3873d8e7d5d4c4855616bcea84bfc353c4c6511e8da8f86"),
		new("item", "preindustrial_food_rose_honey_sweet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3fb731e6be3bf3be8a232c1365a7ba9fa4788bca37a5b88c365cb61d9d4edebb"),
		new("item", "preindustrial_food_salt_crusted_feast_fish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d7e77091e7cf2bb0609b188f72ba5a3f5bc676d73a42bca0cf01350106315e2e"),
		new("item", "preindustrial_food_salted_beef_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bea134ad5ad555b047218dd33d1ebc358ba26ce237a2fea07239d0db346fed63"),
		new("item", "preindustrial_food_salted_spice_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8335ce465647615ab2df2964b4bfe74bad6c92c734506b2c4a1c3a71be43238f"),
		new("item", "preindustrial_food_scallion_griddle_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83f0c971ed75f68052548ab629da45e3528737cb18ad1441cb9b86975e9d76c9"),
		new("item", "preindustrial_food_scored_loaf_blank", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5db1777d327590557bf17eb5633859a4f085b75fd0e7a342470caac0d23a5f9"),
		new("item", "preindustrial_food_shellfish_bread_sop", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "612ec51f19c6bd4483f23400264387660aca4110d909bd44dcaf6f8b971d5f1f"),
		new("item", "preindustrial_food_shellfish_onion_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "20a68df914abf94fb4d7339d0dc155e7037c037cfd69ac67a391cbb68e1430c1"),
		new("item", "preindustrial_food_simmered_corn_dough_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b6c33beace02cc253461ba802efb5d3d5baa5f77fdf228e29c9b19c27d9d432"),
		new("item", "preindustrial_food_smoked_meat_crumb_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "654b813ba766a4466c63738d629567329e2a57dcb2052ed26f375dd9847e3e99"),
		new("item", "preindustrial_food_smoked_roe_sac", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d208f1579928caf78b236a54eb58fb58bcca7e0587b94d2475e1b8aa9dd23b6"),
		new("item", "preindustrial_food_smoked_tongue", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78171146a49a971a73680a67c212a28c40f5f5063588447073a973048a56e5ed"),
		new("item", "preindustrial_food_sour_chickpea_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95bfdb38844534f8ec402fae7a46ec8d44191f1bdaa01b63d9246fc5fc068216"),
		new("item", "preindustrial_food_soured_milk_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fa50ea2b93129ba07bc1d8479ad675c151b42f54574d81fe688c8f5d6b8ff1c"),
		new("item", "preindustrial_food_spiced_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "722c59d64c0346c1eb7e5d1de86cf53074edb7d9bf79a51ffdb5eadcbb6f3be7"),
		new("item", "preindustrial_food_steamed_fish_in_leaf_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae7ffa3e3a3539abd909e585b0c9de410c55c25f86674daf89b8d52ce4a13d5b"),
		new("item", "preindustrial_food_steamed_rice_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d311d7dddb3ef842de65b145521f9b7042055a84a214bae8655299b576c5ace"),
		new("item", "preindustrial_food_stuffed_dumpling_blanks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8b34a1a107df5ce865b3453bb4dce32759d6c9125a3444c461b7d00b2f87c76"),
		new("item", "preindustrial_food_tea_stained_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a9469cfb9e84cbbca62483e399738cba09eb9a9fee35465f617349f150b88dc"),
		new("item", "preindustrial_food_toasted_rye_kernel_packet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6103781ea2fbc23eb22f2e72edb6bfe31d3120b333f8c2386ea92179d027e12b"),
		new("item", "preindustrial_food_vinegar_dressed_broad_beans", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3508aef860468791aacb6c78d80c608a8acd8788d5b9565cf5d41fe121c31096"),
		new("item", "preindustrial_food_vinegar_dressed_parsnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a05fd816bb91e30d0a6d81450dc8b0c31f36882d453968986f5a98069dbd40b"),
		new("item", "preindustrial_food_wheat_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae52dc9ffee4c6f7377c69dabaadcf3e46a04a63a165863ff0d5121d9cbc1d24"),
		new("item", "preindustrial_jewellery_bracelet_46", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "147f3aaa48e272dec9c2c9757d1c19b82d1b3aa97908a68b74ea457498f587ac"),
		new("item", "preindustrial_jewellery_earrings_49", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa2831d278b4b5ea540483431d9e0a963b51fa3854e7ecf18a35efa6635bcd54"),
		new("item", "preindustrial_jewellery_hairpin_32", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e2707939d4deb48e877f30a66e52870a6cfc32dccf07089c999b55136ffcea4"),
		new("item", "preindustrial_jewellery_waist_chain_15", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "862f0f3e1e59fdb19fb201af12fbeb89c90d17d5a2c47c36048aa6329fe3a78f"),
		new("item", "preindustrial_leisure_backgammon_board", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8511dec25c0ed88b221029675c4e321c0ecf54f3c077c5a425c6bbbd5930fa56"),
		new("item", "preindustrial_military_support_plain_sword_scabbard", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6854c11e05085cd2145751a11725555d137d2865766908135ab18a89f3532669"),
	];
}
