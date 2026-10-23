// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory11Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_light_workshop_lantern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3a77f9f1fa949cf7d4184d6e556c86effa8cd45e5d785f4af5ff2a77764292"),
		new("item", "medieval_linen_wimple", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "152eef59c91c8ec434141bbc069bdf3746b0d5b290e8df9086ddb816b589c1a8"),
		new("item", "medieval_linen_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9b27776390c9a4d4da10b0e8cd1857fda5c284097f0732c31cd1167661c64e9"),
		new("item", "medieval_linen_work_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "829047f66882fc1467a0e4accdbffc64a4355d588d98cd5701377046e01dc862"),
		new("item", "medieval_locking_domestic_east_asian_cypress_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "684ea28d41f598b27f214474c50dcc5d149e5a2aa538c04689c1e4e6bd2dc14f"),
		new("item", "medieval_locking_domestic_islamicate_cedar_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5063e1bf6c3ca60bfcebab11d74c032c3e56903328bd105cb056e4ab542578ff"),
		new("item", "medieval_locking_personal_thread_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "923aa2312f48669a98717353b58cd9cdf7d7af1dc07330fdc17261eb18b3d7c8"),
		new("item", "medieval_loros_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "baf7e6d3c1c9c224396d0c032cac0bc0f389244eba86ab04d5c6719a4599105c"),
		new("item", "medieval_market_cotton_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2271555301417baf9d5618042a1eb02a70930a1d899bbae6f76b65012a47695a"),
		new("item", "medieval_medical_bronze_suture_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5460d76370872a627b64cee2f43e8f71eae23bac8765808ddd42ec30cdeca3a5"),
		new("item", "medieval_medical_clean_linen_dressing_stock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "12209ad5f7db7d1ba3fe7d8d9f99265d08afbff914f40147786bd6ee8d0908a4"),
		new("item", "medieval_medical_honeyed_linen_dressing", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5ffa32f0279ce5f69dc81402f1315fafe6458306f9d8305838a04d840380440"),
		new("item", "medieval_medical_left_plain_hand_prosthesis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00e4652962e1a74333786287dc89d5a1a8623ad0240d69f7880a1a1058402cea"),
		new("item", "medieval_medical_rough_linen_bandage", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2742875906279a7de1f5943cb301b4a42a735f44b728d7227369c58fae2aa1f1"),
		new("item", "medieval_medical_small_bonesaw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8ea5fe6df3e2e5ef8a788b636d3283710e94a5ce0fc5db7f490e21c22f9361d"),
		new("item", "medieval_medical_winter_casualty_sled", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "958219cfbf253954fcf9959ce093655274fdf0512070601648f10a88bf936b09"),
		new("item", "medieval_medical_wooden_litter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d63ab30d64b51074c258ce2275b5f0b3952e386a935f3883c71dfa878a7be7d"),
		new("item", "medieval_medical_yarrow_styptic_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5dde4597ae0f5f1b2e129a852157adc4aebda4e200aac109f1e4da6959b3d16"),
		new("item", "medieval_military_ash_quarterstaff", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b842e7862c00c625f2d73e8da2d313e2be80d397a2723758c1c525f3dc070d0"),
		new("item", "medieval_military_balanced_throwing_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "22ef12161cf69a1631c3e81f22a97cf49d0937c904ca9395944988c4cd66d052"),
		new("item", "medieval_military_barbed_throwing_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5cd6989640d56238d1f59517553469a891c9f6dba7cebb26febf3c6901d1446"),
		new("item", "medieval_military_bearded_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "863ff4fc0e7b2da84f287405eb89f3881ba3b3584446196b09299474c5bcf32b"),
		new("item", "medieval_military_broad_bearded_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "43bd79615858967edac1c8c57754c11e1a2d9a0a77c632cbc3a6fb9632664865"),
		new("item", "medieval_military_broad_poleblade", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "91461f61939aae0f7fe782b5c8c20652d435face385d3eefb02a3e9fca8bb64c"),
		new("item", "medieval_military_broad_weapon_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2472f6264c1831385a6d016a087eaaa2f40abfca87aef843304e0536416c933"),
		new("item", "medieval_military_cane_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b911ab5e0cb242210b8cd93d24b66f0a4f5ded5b30f137b7796060495355802"),
		new("item", "medieval_military_compact_kite_shield", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f60c07e313c8f2c7de9b376488ec088a30675319ccb80be55625d3561382e35e"),
		new("item", "medieval_military_crucible_steel_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "72f254269b175ea6ec65288d5ea6979133e6e337f6611ed8191a0c80595cefd4"),
		new("item", "medieval_military_field_mattock_weapon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c22858ae0bcf37d61e14c5ad5648bba92937a5c37c0193ff06669c95878bc590"),
		new("item", "medieval_military_field_point_arrow", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c9fa86bc72cfffee15e2235bc0fd4c10d5f6231754898cd1375413001ee86e4"),
		new("item", "medieval_military_fine_war_spear", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e45309cf650045fa0531cbed8e0d582a49793eef558468648219badacf0113"),
		new("item", "medieval_military_hand_me_down_sword", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "bea20f8052e0cae16f4b63fa089ee6b4c410cfbd2f0f62f21fc22defb32fcbe9"),
		new("item", "medieval_military_hardened_leather_jerkin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "58e0fab0b78670e4076120ce81a85b381408212d3280a1586ded441a41928be9"),
		new("item", "medieval_military_heavy_fighting_staff", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0486a67c7c21283a6f08340625ea541f7f9cd6fb94c1540f61b6c2ce0ee23f7"),
		new("item", "medieval_military_heavy_throwing_axe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1920bf2a93d0349093c7fedfb001467d57031300e65f8b369b7ee1926432881"),
		new("item", "medieval_military_iron_bossed_targe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "af7e55ceb632239e3b47dfb59ec33380c6afdc055308f6db62fc0ee055fdda15"),
		new("item", "medieval_military_iron_buckler", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "28a97d2dbe93983b3f6b902e17dd059b8b663ebf0af06f22be4f7007b9f2da8f"),
		new("item", "medieval_military_iron_curb_bit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "75a135d9d32e275846f4e4acdc41e731484d76f897901f23fee97cc8c5bd5f7b"),
		new("item", "medieval_military_leather_criniere", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "57bf1eaa864855db2a5a9fd4ab1b701887ecf122302f3b62eb87b9a5ff513d6a"),
		new("item", "medieval_military_leather_cuisses", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1806b0f83826feb4a2a4877c509545d3bc7010d67a8342dddbea28a844de7bb"),
	];
}
