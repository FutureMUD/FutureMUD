// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch027() =>
	[
		new("item", "antiquity_oak_tanning_rack", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaf0b5e226f9e1152de87957c777f43a59abff7dd16541d0cf5e3696afe053bb"),
		new("item", "antiquity_papyrus_sheet_bundle", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa3b8d874b8c929ba11840c935cb2bfb3a5db150c8b2e64db2d0aea5a44d39f6"),
		new("item", "antiquity_persian_soft_riding_boots", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f814fb11143ed05861f13c3ce4d92a1c96e968d1698c6de93b3657f23c858f1d"),
		new("item", "antiquity_polishing_stone", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "afe76e78559eac8665faa1f343f59f9f5ffcf811f01bdaef38fadda99e370f85"),
		new("item", "antiquity_potters_rib", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e25419dde85f3c31065ee48c4b796f9eaae484d0a68e6d73ad5f348195f0afc3"),
		new("item", "antiquity_punic_gilded_scale_cuirass", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "60643a790bf17b6f2c86cacbcb20fd2123d35d2c07287c65405d95d49a1860dd"),
		new("item", "antiquity_quill_pen", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f994080283ad50a168ece8d7f26602b5e7c8d75e19a8dbed826dea17336a342"),
		new("item", "antiquity_resin_incense_pellets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f4810cf6086c6cecd45695283aad3c8dc8c675c207dccbd9df84bc89e1bf7946"),
		new("item", "antiquity_roman_plumed_iron_officer_helmet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "50da56136518b7b96f292eef3329045a4fe95e5bfe4bdd68b22d622f1588717d"),
		new("item", "antiquity_roman_wool_sagum", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c9756ce5067f827dc69c43fd25834156a5ae398f22c975983c745c08ee575b9b"),
		new("item", "antiquity_ship_signal_trumpet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4dd10e61b34bd38da87d8f83826ea155d0da943b1cbb284f296930247ace7132"),
		new("item", "antiquity_tableware_celtic_horn_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd908fb45344efe83891b4535b2271d8bf8c37e3dc09406b285e2a5cde5c0768"),
		new("item", "antiquity_tableware_etruscan_bronze_stemmed_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8464482360965da7edbc418a9e6612c3c034736246c6e2071fe0ed1d9193287"),
		new("item", "antiquity_tableware_etruscan_bucchero_chalice", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "285b24da9eb728fbfd60c0ab28611abe0aee93cf0ab49321968ae8ff9135f495"),
		new("item", "antiquity_tableware_germanic_wooden_food_bowl", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "541f4b48a68afce48c7699c02b27c72cb2701a7b15675c337dfae92c70b177ef"),
		new("item", "antiquity_tableware_italic_plain_earthenware_plate", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1613db23cdab406a34878e5bcc00d1b10e8bcfd87d31e18b8eb15d074fd86fc9"),
		new("item", "antiquity_tableware_punic_glass_cup", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2bb08471d4f8b8fa3fedde575924009fc1d39cdbbfd87affd5a96e0f76207b9"),
		new("item", "antiquity_wax_seal_cake", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff48afc60ae43908db8af91f470422c9732bd453708cdc529ac8321db103f201"),
		new("item", "antiquity_weapon_kushite_long_self_bow", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e652e04d8f67e5c781925e09799fa67999e5212b2890addd0eb714e84702757"),
		new("item", "antiquity_weapon_punic_curved_mercenary_sword", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d60d13efe46126d9b112f101a62b8aff42f8da1ef9388a7135908fb159d799f"),
		new("item", "antiquity_weapon_roman_weighted_plumbata", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "af7202059fdebbba438d3c3f9e7f136ff40413eefe4cf2a676dcf1ee4773d2d4"),
		new("item", "antiquity_wool_himation", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2564b3a54b23ae83091aab7f0fa0d414fa590a98d8e146601ff61cfac5e0ba7"),
		new("item", "earlymodern_colonialnorthamerica_clothing_soft_moccasins", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ff0f5d01401b0ff0d0ba32432fad72a17aa99dcaa848407ef93e2ddfb95113e"),
		new("item", "earlymodern_colonialnorthamerica_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b5a38c9ff911db1af7ad7032f0c97885f3df9718178d80f0efd4f3470997361"),
		new("item", "earlymodern_container_bra_account_box_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d02a02f5b6b9c34dc46a25d0fb4b3ff5c89ddf5e492fa68eabedb6220f0cb20"),
		new("item", "earlymodern_container_bra_punch_bowl_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c6a45e1bf144d2da3a8c5c5330e9c3ee35f31db9389f1d8e02ed3f92dd50f5f5"),
		new("item", "earlymodern_container_bra_travel_coffer_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "df4923428f425b93fdfcfe2eb3660aef9806dc9bf35799543eefcfb3e9f5f5ed"),
		new("item", "earlymodern_container_car_tea_pot_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fe5fe57a9b55d26f6b807bb3b0b468b7042655b3ac1a53bee8bdcd43b907b46"),
		new("item", "earlymodern_container_eth_punch_bowl_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8cd3807a4b8393e8552dc41b504f47fad52647b5fc5e1ff78c32ce33ccdcbb84"),
		new("item", "earlymodern_container_frb_tea_pot_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8a64aae6e998d7692b0133a9353c42f04b764b8893e2b024d57f0da6856698b"),
		new("item", "earlymodern_container_ger_account_box_31", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7aa6858def21bb50563f3f009dd7230aeeda609293f1eb9d4702c7f398234a38"),
		new("item", "earlymodern_container_ger_coffee_pot_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0216554088578d9f82c4df0fde6056c53db8cdbbad3e057b3f2eab0127c5cf5d"),
		new("item", "earlymodern_container_ger_document_case_32", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5ef3079cbde61e92edc280aead05c0dd03a670c3fd98d2619403d55f341e875"),
		new("item", "earlymodern_container_ias_spice_basket_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ed8db8be41597144012bf702b32d018d55eb35eb1c2396cdf5f6e731901b3dfc"),
		new("item", "earlymodern_container_ita_lockbox_32", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "95db6df256ff4a46b929f0af0519b28236591a524eee5fc60c3cde97e0863a59"),
		new("item", "earlymodern_container_jos_travel_coffer_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3603536af233a0c6e14b843f7e9b79abf7e8e804eb294a5af9b0803dc1da0200"),
		new("item", "earlymodern_container_kon_bottle_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c0c2cebece0de97465a5b3e4aa2e6855e8349a01f2de3ae36d4a03a1d684cec0"),
		new("item", "earlymodern_container_mes_tea_caddy_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fdd717e82a0ed42a9f3f3bca678e775a810ed0b571f30b7c990eb01930bd50af"),
		new("item", "earlymodern_container_mse_document_case_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "53b874e726f55ba06f18a0f60e9f9f93a9eda3652fb6605a68479aae76c7778f"),
		new("item", "earlymodern_container_plh_ewer_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "351b29a84c4d0b9d4ed7a278bd22ee110b79ee7d9bc1e2333aa438d23d586b46"),
	];
}
