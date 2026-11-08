// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch034() =>
	[
		new("item", "earlymodern_globalmaritime_officer_tropical_company_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5a8023d872b93223e386e129ed84d987a0731bb8d2c23434e40e942fec7e0d6"),
		new("item", "earlymodern_headwear_physicians_round_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9ffb7d13412e1531528ad720e2c52fa7ab88a1fe7de330555d86ec062d854e4"),
		new("item", "earlymodern_household_account_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ba105690d0febbeb2acca093d026476e32adb4e228635a01bcf0914c32300d0"),
		new("item", "earlymodern_household_coffee_cup", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d48a6d29a67a4387a4b8e9dc93fc4cbed0d44a873048e7b0d51931d6b2e54d3a"),
		new("item", "earlymodern_jewellery_and_armlet_667", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "056a0919932480a128cb655ce7b5ef711fa76ee1518ac78fd629fa2a0c828335"),
		new("item", "earlymodern_jewellery_and_hairpin_672", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2096d4ab280311ea731756fa4d80a272d1fbbc5839faace28b4d7dd51444f2f3"),
		new("item", "earlymodern_jewellery_bra_bead_necklace_584", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a4e3b5c0e5507d2b90cde434ed666b6de864e831a1c6ab85b2ee8fc9812da48"),
		new("item", "earlymodern_jewellery_car_ring_681", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fce9e80da694935ffbb32d3f728ce07f986b9a5cd0b071830879cee877db0c41"),
		new("item", "earlymodern_jewellery_car_temple_ring_698", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b6f1111ce9128c012e012f9132f3db08ffa335e5437782a7eb939bd2980eb1e"),
		new("item", "earlymodern_jewellery_edo_hairpin_372", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "191f3fe41cd4d4323ac26d33803cf2199010a92730af3e22cac757c057462158"),
		new("item", "earlymodern_jewellery_ena_diadem_614", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "48f025302902bbd53801a2af8dbc07af656ebdbd6ec7a54d8964d43f2a0a0da8"),
		new("item", "earlymodern_jewellery_eth_diadem_534", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a654f1b9501e38dc098a484e23501a1465125986b71d216d7e214c0ee28b1bb8"),
		new("item", "earlymodern_jewellery_fr_ring_01", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d944fc6665d3eda73e51587fbe42a21a1b705d3dbeb424d31931f84de29fa35"),
		new("item", "earlymodern_jewellery_fr_temple_ring_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e60573d0ce7dd5c49742a4f2d10a231445062bdaab3ca83936211e95a652fec9"),
		new("item", "earlymodern_jewellery_gb_torc_59", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a5e27494fb4489ea22395ef0569c9484957524f7df02e5feb1da3e8098c1722"),
		new("item", "earlymodern_jewellery_gmt_hairpin_712", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "80f4d1d4a3aba2d6b63fa6e866458c46789d77b12ad5041f6c8d01a7fbce80a4"),
		new("item", "earlymodern_jewellery_gmt_torc_719", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c58d1c9c1db553496aa49a689ddb41a05d0ddd3cc13016b3d43d1971204e6ca9"),
		new("item", "earlymodern_jewellery_gmt_waist_chain_715", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4526de12e8038b371cff5eb3e79ff326cb4efb7a15283ad780efff964babf3d3"),
		new("item", "earlymodern_jewellery_hre_circlet_93", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f83764ad44f0065759b9c5559cd3f4f04e00ec257e052d315086600e17551c80"),
		new("item", "earlymodern_jewellery_ias_earrings_449", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f721dd8873629c1e7705ffc62f5215e5868e944a6567c9a241ddc5a4b77b1ea4"),
		new("item", "earlymodern_jewellery_ias_torc_459", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "43633e19824e85f7a4a5b424d777dd534d4d6c051f47fac3a4e973eba81d75a6"),
		new("item", "earlymodern_jewellery_ina_badge_637", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7951561584129edfbb3c7e1cafad6c9a19341988ed47985a03e4a92761ecf1ea"),
		new("item", "earlymodern_jewellery_jos_hairpin_352", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bfc0aab581b871504da1da74266198ea72b5cac4da927e35d561da4e27683f0"),
		new("item", "earlymodern_jewellery_kongo_pendant_483", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b45eaabaa995a87382199cceb06f6ee5f3f09f73caef980590a17b7f06cb1f8b"),
		new("item", "earlymodern_jewellery_mca_armlet_647", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "60de023a0b335dbedfda9b84a4ef052bbcd7e2ea41eac23ebd00b8458029095a"),
		new("item", "earlymodern_jewellery_mrd_bracelet_266", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac85c2b511cf376da4d8a7728613c5763100dafa5dbce4def89779301563de5b"),
		new("item", "earlymodern_jewellery_ott_badge_197", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b576b5dc6f74eb04d7fc376068e7754b48f7b8bad37bc6114fe89393c551ffd2"),
		new("item", "earlymodern_jewellery_ott_diadem_194", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "68aee81c8f91212f1b626e68c3facc3ddd98c9b6713d41e42285d628281922bb"),
		new("item", "earlymodern_jewellery_qing_anklet_308", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff870a50a971e5efb9b0e89199d5fad233d8c1f3e9f5cd0758e212a44a9717e0"),
		new("item", "earlymodern_jewellery_qing_badge_317", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "084db292c89f90ce24f1b13645b16d60061e5f30e5c04c28edef85ff24af92bf"),
		new("item", "earlymodern_jewellery_qing_wreath_320", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c18e1bdff24106f1b2982745208741fa7922be3c68c8b668a02b34bd6e3deba"),
		new("item", "earlymodern_jewellery_sahel_wreath_520", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd61a089e9ea3e329374f6e2d1574e7da12a017e13d6194337f577cde73aed28"),
		new("item", "earlymodern_jewellery_sea_brooch_410", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2db22afad46fc87a1f09c29bc38ada410a9d94e0f68b8b92ab35c9f450c35908"),
		new("item", "earlymodern_jewellery_sic_brooch_290", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c22c3125a007711e564ebb574106a0941c4a40df41c1b0a1993f95b407c4b3d"),
		new("item", "earlymodern_jewellery_sic_wreath_300", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f915a5b00159d7ab324bbd0546283a7062d2d5797684602da8ed37417ec759e7"),
		new("item", "earlymodern_jewellery_spa_armlet_567", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f29b42a969b9b583e5f799855f7da97b050605fee86eff58c184ee09f1e5182"),
		new("item", "earlymodern_jewellery_swa_badge_557", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ebab6f7ee15fcb8a19264fc8547a7405d91166222b9e5f589ec3d6d1a336e101"),
		new("item", "earlymodern_jewellery_swa_diadem_554", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f7075d021b0ecc8d96ec55c04935ae0f46355960b25a2c77f289c0dc9f1ade0"),
		new("item", "earlymodern_jewellery_wa_bead_necklace_464", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3e8e38295a55af4c1a9c17e26b59b602c566fcf717a6e76d0f826f8978e1fee"),
		new("item", "earlymodern_jewellery_wa_temple_ring_478", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "43411886c583be160a8ad812df99df8847f5b2c974f782552f162c0baa44585a"),
	];
}
