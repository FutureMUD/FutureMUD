// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_food_fish_broth_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4bf1c9baf2d09ceb645429e985af87ece8318056e1cff727eea77b15f28f3da"),
		new("item", "preindustrial_food_fish_cheek_medallions", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "35feb0d76c06cd1ef7cafd1e0f09ba31a0fbc52e31b830c25b5a8ca5da8c4f1f"),
		new("item", "preindustrial_food_fish_sausage", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "326a733bece32b3d27739da4554c0b93020e4154575eaaf770ac5c701a2003a9"),
		new("item", "preindustrial_food_fresh_milk_curds", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4697150e56600ceda3137e8b1824b5ee81c710a8e643995b42a786b37297bf0"),
		new("item", "preindustrial_food_fruit_and_grain_pudding", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36d05ad6724e33ac07662163f6cbb04388bc0e094781c783c5a00422234209a3"),
		new("item", "preindustrial_food_fruit_nut_pastry", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cfece8c885915483a089c23fd14dda3954837d9cef4eaec46a0a3c86d1c9264"),
		new("item", "preindustrial_food_gourd_and_lentil_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f2fa3ee65ab824dc8286512f393736a6db52ba5dc087e1b100bab8aa4e186f1"),
		new("item", "preindustrial_food_greens_omelette", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c8f9d285420db422e8df895ec79cd849fbbadc52799159a74a0ca9b7a74ebbc"),
		new("item", "preindustrial_food_greens_stuffed_meat", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "491e72dfb811870282b98c451af1309e9cc26b837f7d2fcfad87a6fe6dc34bf6"),
		new("item", "preindustrial_food_grilled_meat_chop", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a29b0f875888adcc799db96d5c562cfc4ee392e2c27d696c6fdad19babdbc4ff"),
		new("item", "preindustrial_food_harvest_grain_dish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5790efb82a5d3b2a3543a42adbd96c405d00159ffd5d77e6f0e2611f0629df92"),
		new("item", "preindustrial_food_herb_barley_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d192a1771d4e7f50f3c6dc3302f84cc9983a3d93758bb42f1bdbee89049a85a"),
		new("item", "preindustrial_food_herbed_rice", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e35b260c33b4b2abad4778fe761821fe97e72327b88365aa534aa4bb2e467dd4"),
		new("item", "preindustrial_food_honey_cheese_sweet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b667553c7b3f8c7f9978fed8afac36b03bd376c9b64a553c508a722569108f8"),
		new("item", "preindustrial_food_honey_fruit_compote", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac161fc7b9969c841fa6738dd1c5ab5441e314193637e1c02ed49f308ec2ca7a"),
		new("item", "preindustrial_food_layered_fruit_meat_feast_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48684a8b6da723f50be93dfe2e953cb75fddd4e3e30049916655e4048a5a59be"),
		new("item", "preindustrial_food_layered_pastry_dough_block", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11d4fd778a97982637d14bf83c6153d6210ac9727b7dd2b6c83c04beba33aee4"),
		new("item", "preindustrial_food_liver_and_grain_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ae4bd93898d94165883e51d79c3a1a45fbfc5a0a95e0c39149c58c7c3433f02"),
		new("item", "preindustrial_food_liver_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b6b9d69f94ab1f3a11a93df783b85ee5dd536966fe2052b04a21cfc424b75ff"),
		new("item", "preindustrial_food_lumpy_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b7c437444590d44ff5f948ea336e2772772696dafe7dd613628c430f2767270"),
		new("item", "preindustrial_food_lung_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1da8115bf326c1cafdce0e8fd9849594efbd2710fcc783697d6b9adb3716b75"),
		new("item", "preindustrial_food_mashed_root_medley", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "904bd61a4452ec8cf6df99925f5ea34ad61f85892eed13a7f788ceb66722a8be"),
		new("item", "preindustrial_food_meat_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "38477bffd76effa1e9e94768665c4e170c6b4986269f70a9834594a18ae54261"),
		new("item", "preindustrial_food_meat_egg_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40c857f185e019565b65793139143d4a800b1a055667f11c6d1b8999723846f7"),
		new("item", "preindustrial_food_meat_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90481afe71406ae827fbffe1a29e818721855b318ac8eac9a5a2a50e35d5351d"),
		new("item", "preindustrial_food_mixed_travel_ration_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "12dfea42347db85d78ad0724bd7401f8172f8305252fb604e8a925cbf2a4a7ca"),
		new("item", "preindustrial_food_mushroom_filling_portion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8ca4cfcdf7b3936572b6fda4bcce21d3e982fd98918333dc57f79599181ea43"),
		new("item", "preindustrial_food_mushroom_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6abb220229d8bf6bc4b0fbadaeb29df129e47bd8bd7047a36275eaee8a18b10"),
		new("item", "preindustrial_food_nut_sugar_tart", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dc584bcdddc75b5f19e20212a7921288d17aed31c6a52882bb74894ed7c166e"),
		new("item", "preindustrial_food_oily_fish_onion_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "578d4590fc0721b6a2adedbf91fa7c6baa983e681d64df98fa9e745c91356136"),
		new("item", "preindustrial_food_onion_and_garlic_mash", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "63f97f7da50df3b7bba12a99d005cc59c2aeb66ba8de2c8f2311773363d703d2"),
		new("item", "preindustrial_food_onion_and_lentil_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e2a18f1d0983fdf7c42bcee7c2a1c49cb429e037414121a8d051f94734e45ee"),
		new("item", "preindustrial_food_onion_and_turnip_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18ca4b6c9a9913fc16ae0e9f27920482ce4c512ae28ee9086cec6e58923be03d"),
		new("item", "preindustrial_food_onion_grain_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "78973b4f3b83c1fbda9ca4c1108d734b67301a147706d10ce32bc83bc4333bf1"),
		new("item", "preindustrial_food_onion_mushroom_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e7566c64c698e9265741b48f4d77c1c08a59efb9fe4e336a3afbd5e3751384ac"),
		new("item", "preindustrial_food_oven_wall_bread", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93bfde5e01f22a5d359d48b789d6a90329d9f22a0d3e902ff09d3d766a6185dd"),
		new("item", "preindustrial_food_pan_fried_bread_scraps", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "45fced5ae7af7d55d08f5a82bd14418ce7091e6cfa75a22804a41e21a6640d5f"),
		new("item", "preindustrial_food_parsnip_and_pea_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a816be0b61d309fe8138f0f78193d3b5dbe1104106087989a4c280d81c86fd0"),
		new("item", "preindustrial_food_pea_herb_broth", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3afd49cc39801cceb88135d00812b5153673c9b2c5a2194c1759b022013ffcf"),
		new("item", "preindustrial_food_pickled_radishes", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9db4c2dc5aa79ecafc1a2521bd4eadca1ecc56b55406e7be5ff4bece2dcd940"),
	];
}
