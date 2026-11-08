// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory10Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch007() =>
	[
		new("craft", "Carpentry and joinery/build rosette cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_plh_courtyard_gate_141"], ItemSeederOwnershipPolicy.StockAggregate, "03fc4b4b09bced6e567e88e6e10a3ee76a45327648e68ba36dcbbfef6b4b7fce"),
		new("craft", "Carpentry and joinery/build rosette cedar warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_wer_warehouse_gate_09"], ItemSeederOwnershipPolicy.StockAggregate, "9051d980059f2e7a27a2d1a8df42331cb85b939c0c203b8b553e4ea14859168b"),
		new("craft", "Carpentry and joinery/build rosette pine lockable door", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_sahel_lockable_door_478"], ItemSeederOwnershipPolicy.StockAggregate, "6eb0196ccc12abb983fe6b4719f691274c9b0a2319ca8d3222044c9aafa831a2"),
		new("craft", "Carpentry and joinery/build rosette teak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:preindustrial_door_garden_gate_24"], ItemSeederOwnershipPolicy.StockAggregate, "17968952d24c2d76e11f05824184dc9dd9813828b6cb6c1d602c12fe8bc2ea5d"),
		new("craft", "Carpentry and joinery/build rosette teak shuttered opening pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_mes_shutter_537"], ItemSeederOwnershipPolicy.StockAggregate, "67287930dd813d4b19192640d226efacb1fd3d09a73a65f47dc53908d981a2e6"),
		new("craft", "Carpentry and joinery/build shell cedar courtyard gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_it_courtyard_gate_103"], ItemSeederOwnershipPolicy.StockAggregate, "44be7e73a997cea04cc1c44a1d957335a58a70254c92968adae2523946ff25cb"),
		new("craft", "Carpentry and joinery/build shell cedar warehouse gate", "crafts", ["earlymodern", "renaissance"], ["item:preindustrial_door_warehouse_gate_23"], ItemSeederOwnershipPolicy.StockAggregate, "4e8b28519921246446f130b51cd8948357f477becec165fd00b3fe4df16d34bb"),
		new("craft", "Carpentry and joinery/build shell teak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_jpn_garden_gate_290"], ItemSeederOwnershipPolicy.StockAggregate, "ffcb1c25e49d7d863742edbfe5cfc7c96cbce76357738b83cf773fd89ab02e7d"),
		new("craft", "Carpentry and joinery/build slender carved cedar lock-fitted wardrobe", "crafts", ["renaissance"], ["item:renaissance_furniture_rse_lock_fitted_wardrobe_01"], ItemSeederOwnershipPolicy.StockAggregate, "8c95b94d58b807398a90436449ec19c3464f82b4b14cb660b0ce675ec9f62208"),
		new("craft", "Carpentry and joinery/build star oak door leaf", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_col_panel_door_659"], ItemSeederOwnershipPolicy.StockAggregate, "bc6a9991b417b3260bd46c8337355119151b6d678786aa7d9a6ec547ecb3045b"),
		new("craft", "Carpentry and joinery/build star teak garden gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_mrd_garden_gate_257"], ItemSeederOwnershipPolicy.StockAggregate, "05f28c36032a4f04b367e0f6c0afa8fb26c0d13e621000de840efb84d2e55527"),
		new("craft", "Carpentry and joinery/build sunburst cedar wicket gate", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_wicket_112"], ItemSeederOwnershipPolicy.StockAggregate, "02308442761060b8938a6d0a7455bca87dae4b80e80defa29ac167d1b2ac1047"),
		new("craft", "Carpentry and joinery/build sunburst oak door leaf pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_eon_panel_door_141"], ItemSeederOwnershipPolicy.StockAggregate, "29e21c31cd95c8febc2a2e3d4bc3674c11cfe560db0ba9fc2a1bc71a22ef317a"),
		new("craft", "Carpentry and joinery/build sunburst pine lockable door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_eon_lockable_door_157"], ItemSeederOwnershipPolicy.StockAggregate, "16ee40a1fee47c0966cacb7775f9b91c564f57c43d6fea81da463785da7f7ab5"),
		new("craft", "Carpentry and joinery/build sunburst teak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_ena_garden_gate_580"], ItemSeederOwnershipPolicy.StockAggregate, "402e287a65cd4e94dc500c2bb50519c9cbf19488cc82f27cf84007683779631f"),
		new("craft", "Carpentry and joinery/build tall mirror-inlaid cedar campaign armour tree", "crafts", ["renaissance"], ["item:renaissance_furniture_pip_campaign_armour_tree_03"], ItemSeederOwnershipPolicy.StockAggregate, "85ff04bc7d1dee80b5a8bb7226e6f7a3596e920430acb78e53abee6ab3ea4bab"),
		new("craft", "Carpentry and joinery/build vine oak garden gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_edo_garden_gate_352"], ItemSeederOwnershipPolicy.StockAggregate, "d9c9e3ff2aec0e618857d9159d17c27a97046cd32f42eab81e84edc4e8c8b384"),
		new("craft", "Carpentry and joinery/build vine pine service hatch", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_common_hatch_27"], ItemSeederOwnershipPolicy.StockAggregate, "a9f80e4a3c57fb246294ed60721e51bebc344748d5c7cea2e20dbdc89ced4fdc"),
		new("craft", "Carpentry and joinery/build vine teak door leaf", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_scb_panel_door_115"], ItemSeederOwnershipPolicy.StockAggregate, "fa7a312a33d5f8acc6398ebaacfc43bb568b4764d3f7950ee16daf6957edeab1"),
		new("craft", "Carpentry and joinery/build vine wrought iron lattice gate", "crafts", ["earlymodern", "renaissance"], ["item:earlymodern_door_mug_lattice_gate_235"], ItemSeederOwnershipPolicy.StockAggregate, "53bafa3983d1c6b6513059c18b4b4ec27b382e2ecd92b2ae5556fa2f9c9490fb"),
		new("craft", "Carpentry and joinery/build water-wave bronze warehouse gate pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_ott_warehouse_gate_177"], ItemSeederOwnershipPolicy.StockAggregate, "287084b1b1eca6c147af02b1ceb5abda06aa68469622522df7e99c323b0dd313"),
		new("craft", "Carpentry and joinery/build water-wave pine door leaf pattern 2", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_ott_panel_door_169"], ItemSeederOwnershipPolicy.StockAggregate, "1bf533f30fb8cec27ef01bc61699cbdc4552799fbfff338aa4865511762297f4"),
		new("craft", "Carpentry and joinery/build water-wave wood secure door", "crafts", ["earlymodern", "renaissance"], ["item:renaissance_door_ind_secure_door_508"], ItemSeederOwnershipPolicy.StockAggregate, "0617f3d1c75d1bd951f77712de320c0c7870a1f24f55930f558017d5a2ec0479"),
		new("craft", "Carpentry and joinery/build water-wave wrought iron metal grille", "crafts", ["earlymodern", "renaissance"], ["item:preindustrial_door_grille_11"], ItemSeederOwnershipPolicy.StockAggregate, "6d0ab71a13486d4dc90190360c24ed7e34f570e33fc7124abcf99ab22bd6523c"),
		new("craft", "Carpentry and joinery/build wax-sealable manuscript chest", "crafts", ["renaissance"], ["item:renaissance_trade_persianate_court_manuscript_chest"], ItemSeederOwnershipPolicy.StockAggregate, "4d30e61a284f462889e711de5d30f2584d18f4924233e5d3a370daaed7d24d8f"),
		new("craft", "Carpentry/carve birch household vessel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b71f38e4cd0b064796d975ceaf6057ace0287e90aaf6ea6a1623ae26470fc6ff"),
		new("craft", "Carpentry/make fine woven grain sieve", "crafts", ["antiquity"], ["item:antiquity_food_grain_sieve"], ItemSeederOwnershipPolicy.StockAggregate, "8069e59c7985270b7595417f9b3addd511fa1a7c6778358d51b35f4bf2f8b745"),
		new("craft", "Carpentry/make linen medicine strainer", "crafts", ["antiquity"], ["item:antiquity_medicine_strainer"], ItemSeederOwnershipPolicy.StockAggregate, "b6dda8a310899a88ca39ec9a82c7eea5e9935fb1fee002fb15de3c82262a45d4"),
		new("craft", "Carpentry/make oak fuller's mallet", "crafts", ["antiquity"], ["item:antiquity_fullers_mallet"], ItemSeederOwnershipPolicy.StockAggregate, "6a761cc84683b13add05b41f52b0e9c213b918bcab769f00b0cc4bab817b3ff0"),
		new("craft", "Carpentry/make oak weapon rack", "crafts", ["antiquity"], ["item:antiquity_oak_weapon_rack"], ItemSeederOwnershipPolicy.StockAggregate, "e1582d3d0e903b6f4fc87eea3d67e3e6373a8655478ef70f5bfe3ac679791184"),
		new("craft", "Carpentry/make small household cupboard", "crafts", ["antiquity"], ["item:antiquity_small_household_cupboard"], ItemSeederOwnershipPolicy.StockAggregate, "574bdd34f657fc141ced50df999f107bab1fb22c8cb9c1de86cf60e2f14e9da1"),
		new("craft", "Carpentry/make wooden bead necklace", "crafts", ["antiquity"], ["item:jewellery_wooden_bead_necklace"], ItemSeederOwnershipPolicy.StockAggregate, "95d0a447fb1d19aa5e259262b6a0fe9d490b75337c8e0f399a3328b186ee4651"),
		new("craft", "Carpentry/make wooden drop spindle with a clay whorl", "crafts", ["antiquity"], ["item:antiquity_drop_spindle"], ItemSeederOwnershipPolicy.StockAggregate, "972b9b8b82afbc97bb9ad12e0735303bbf29740c3fae4785fbe65dc71e45023e"),
		new("craft", "Carpentry/make wooden honey press", "crafts", ["antiquity"], ["item:antiquity_honey_press"], ItemSeederOwnershipPolicy.StockAggregate, "e81641deae4634f4d9156eb33825162c948fd7669a7443daed4d5590955a1ccd"),
		new("craft", "Carpentry/make wooden pin door lock", "crafts", ["antiquity"], ["item:antiquity_wooden_pin_lock"], ItemSeederOwnershipPolicy.StockAggregate, "87b05ad58a20a9b353453d234974e6b5d275c9fae2c1bb82f0d80b8f982baa34"),
		new("craft", "Carpentry/saw cedar furniture panel stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4b5256c85d888c8c6ddae5d31529be0cbd8485f05234d332934444c63d4d1e8"),
		new("craft", "Carpentry/shape cedar weapon shaft stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e384ad7f12ca67dbf102f22f1c71b66fa065c9dfcd9f04740b560e8146f9d2d"),
		new("craft", "Carpentry/shape wood tool blank stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e3f02e8c6b1d35ee123f8c9522146dd7fdaa46cb518622d405d19e720d220036"),
		new("craft", "Carpentry/shape yew weapon shaft stock", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8896085da576756bf7595e78762102a299ef38fd59b9e6d777440a9addb9539d"),
		new("craft", "Ceramic and glass work/form dark green glass syrup bottle", "crafts", ["renaissance"], ["item:renaissance_domestic_apothecary_perfumer_syrup_bottle"], ItemSeederOwnershipPolicy.StockAggregate, "b36eefd3eac308088f79c13ba2cf26046274b421d378ad7af2885ec144654b8a"),
	];
}
