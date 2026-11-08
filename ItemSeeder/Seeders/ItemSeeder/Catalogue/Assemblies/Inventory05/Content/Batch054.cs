// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch054() =>
	[
		new("item", "preindustrial_food_fruit_studded_bread_bake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "02be727a4673ef35dc86e18914ae48b916a47a9dbd57b1990a624ee4e5949f41"),
		new("item", "preindustrial_food_garlic_butter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aaf8ab0c7bda6ffda3bfa94956c0382bba82119f75fe7c0aaad019f8a9619c3a"),
		new("item", "preindustrial_food_garlic_chickpea_paste", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "562f69e9c5450bdc0819163b9d929ac874d11e975a8fa0c97424202deb4441ad"),
		new("item", "preindustrial_food_griddled_porridge_round", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49eb6ad66cf60dee4120fe453d27ccbdd3ba3da981a37ced5141dd39e222307b"),
		new("item", "preindustrial_food_hempseed_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1316a5a621a0fe82b7d895ea39e99d97e1a2515a6886bea7a094dd14c271d05"),
		new("item", "preindustrial_food_hen_and_grain_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b693495b3e40ca582169bf5dc88e4a1ad50ea7bbdd105576c13282bfaf605fb4"),
		new("item", "preindustrial_food_herb_batter_fritter", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dab0ca23d30fbad11c3f886ddc48f612fc8c5fd1347e6d72fe079eb61ebfb2d"),
		new("item", "preindustrial_food_herb_fried_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ca3ab9d1db01448aa84f1ca96566c3b3fcc6aebb5fbe292aab7b8d1ebd3cc59"),
		new("item", "preindustrial_food_herb_grain_cracker", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c4357cfcd160dca4d4230d46199c2361a4887a5e30b60fcb26532b9fb3258a5"),
		new("item", "preindustrial_food_honey_nut_pastry", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a99b16767c2119f4431ee1a08497fbc339ff580add9de9796fa720ab5b054ea4"),
		new("item", "preindustrial_food_honey_preserved_fruit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a6544971e1fe5f95f7a2166c3868d7a4739a85a4410b9f3724a1e167c2f872c"),
		new("item", "preindustrial_food_honey_sealed_grain_delicacy", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "384febf0f9153df2d51f22c9858c29162b18a0251c94a7b4fa95fff9a27b9d63"),
		new("item", "preindustrial_food_honeyed_roast_parsnips", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bbd495e1c3d3c8d17898ceae20fb5cb917a5f0b261363ad84bab480be04314c"),
		new("item", "preindustrial_food_honeyed_yoghurt", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e69cc0d13146ed0b367b845715fba9fc10437738677f31d90721316b0eb9eaa"),
		new("item", "preindustrial_food_husk_meal_pancake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "94cef2648175de139f47d0ad2a44fe89ba247534f4a8de8f37cb24fbba486a31"),
		new("item", "preindustrial_food_lacy_rice_crepe", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "00122aacee0a17b6d015ea727638a73178b2e1326e4451ddd7a8d038eaf48f22"),
		new("item", "preindustrial_food_leek_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2793bd4ccf1c0bd5d34291989406be131bad7f3e2b60f80c0bbf8c76a2ece7cc"),
		new("item", "preindustrial_food_leek_oat_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fe7a894589cc01c9753b04b8909aaec7cabc8f65fa213e8dacab4d569cde0db"),
		new("item", "preindustrial_food_leek_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fab129abe8065630d37c4c1d90b73cee5177891c39da2abee2c223720cf87b3"),
		new("item", "preindustrial_food_lentil_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d73ad3b8ceeb10545d0317301596509caa5188e39545e1ad98b19da85d9c194"),
		new("item", "preindustrial_food_meat_and_nut_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb230b87facdd54d3bd2d5b587fceb92ce6122692460f9c8826a6453c05ee329"),
		new("item", "preindustrial_food_milk_gruel", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ed4608d5d8f44de4545d90f2338e9c4ef5e60f48a6910c9923a953f15b43695"),
		new("item", "preindustrial_food_mushroom_stuffed_leeks", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c27eeda7ca6c53f95f5a27f263f17c0b73264be5310a530887bbbb4573cfc552"),
		new("item", "preindustrial_food_nut_herb_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d233978f75b51f29bb3ee6f96aca158c0d32544f46c87432dd1d826d9d029c8"),
		new("item", "preindustrial_food_nut_stuffed_dates", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfcc8d2dcbc73c396a48b7c5f84a936b336ad85b2e7ccac759303521e5bedbe1"),
		new("item", "preindustrial_food_oat_porridge", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ed973eb92be6e80369db788cddd127107a0d0c4abfdc501df899f99061d6b6a"),
		new("item", "preindustrial_food_offal_meat_loaf", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "83807992a6334fd6ff8dbec53940bc2d9c9d98ef0218ea39e34d35f7b5731f7c"),
		new("item", "preindustrial_food_okra_with_leafy_greens", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "21c1f5d8331469428cf6ce4747c8dcf50687fdeb798e4f47a9b2b1fea5587191"),
		new("item", "preindustrial_food_onion_smothered_fish_fillet", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a80304cd6f293c0434af6f6f370d498ac500e574ef003f697b91faea810e41c9"),
		new("item", "preindustrial_food_pan_browned_steamed_bun", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bfb8a775aa1c8a519f5c1306c3a649f7c8894a3cd5d900bf7e8d28962aba29e"),
		new("item", "preindustrial_food_pear_filled_dumplings", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8d44cdd1c65d967e4a2cc1e7291037615f71b8802044953e2377b392f9f31bc"),
		new("item", "preindustrial_food_peppered_fish_steak", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b1c1aaede69fac564262d20e019a1ae24379f86197c656f2d18fb031696410f"),
		new("item", "preindustrial_food_pickled_fruit_relish", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "adea3cad187d2b3fb4f5776ef6d6f95945519fe690dd796c84dc28a4e78d3d5f"),
		new("item", "preindustrial_food_pickled_offal", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecd5ec5dc55ba26dd4b60e1893252feab9d81c932eba6e9d9cd47144b15d0aba"),
		new("item", "preindustrial_food_pigeon_and_pea_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "81d1d38d9865c7187ca0b4b81b227cbaaef33c2cc480853116769f1aea2111b6"),
		new("item", "preindustrial_food_plain_boiled_broad_beans", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4cd0a7c858ea6bfb00b0180f29f5e8607027ed2aaf54e5e9807ce4588d2527f"),
		new("item", "preindustrial_food_plain_honeycomb", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df7c2fbcaa9cb5ff529caf2b96bce6deec8fc9d7623863221047e1b921d4a236"),
		new("item", "preindustrial_food_plain_nut_cake", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19b6f17673fc49dd3584e901687187db5e54cb0c3e1b3e0eda697d95aab01f90"),
		new("item", "preindustrial_food_plain_scrambled_eggs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fb8f23df60b7260c596aefca5a172d9752325aa6b05698c2f10900740023d23"),
		new("item", "preindustrial_food_plain_smoked_pork", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fe40536d82ab989030a4e08d7d8693db6dc8a83ed2ee1521caa80a8e33bde72"),
	];
}
