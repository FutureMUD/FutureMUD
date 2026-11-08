// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_globalmaritime_officer_shipboard_waistcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07b11d4702bf71795863ce3b4902067f4b7f392ec59848f2c7b90de0425a1bdf"),
		new("item", "earlymodern_headwear_joseon_padded_winterhood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd09a0fc441519b06c2de761bca81c4be0af018921334eac5ad189036ed0a1b9"),
		new("item", "earlymodern_headwear_long_tasselled_nightcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "19a748e87586e7167c400f4c34f4fd9849928fb090b0ad0fbffd38169fa40114"),
		new("item", "earlymodern_headwear_qing_broad_conical_workhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70616c022e5ae1ef20f0d81891d6d95102937c16adb5ba41155a897ffc6ab114"),
		new("item", "earlymodern_headwear_sahel_embroidered_scholarcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "96fc46214092714b0b49aa567d9cbafb81d7130da3fa738f56b0d9b9d1a57412"),
		new("item", "earlymodern_headwear_seasia_lacquered_court_crown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be29479e19a327093c4d8d31ef59c7c44c6c2fbe7c15b33d71e68c6f38b41de9"),
		new("item", "earlymodern_household_coffeehouse_bench", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ad74f24d2e25ac4a504e704f46bfb4770f8a64c06acaaaf09e123ca20df92b2"),
		new("item", "earlymodern_iberian_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bcfbd3fa87951333c1dc530132729335b1f3b2de463a79f3d2be0b3d351c0a0"),
		new("item", "earlymodern_italian_clothing_light_short_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "354333e2df83cf865fb8721ae9f0cdb4960026b55156462d1998879f03012597"),
		new("item", "earlymodern_italian_noble_male_formal_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "60b41f46aecb8215ceae0acdf03255db9ad8f1dc3442dc8e35c3713d5e376226"),
		new("item", "earlymodern_jewellery_and_bracelet_666", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf569286d046e8be1c0e62b112d62bbefe1ff54747fb92d1da9f127120e7937c"),
		new("item", "earlymodern_jewellery_bra_signet_ring_582", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0969e132bfdbe020dc1b8ebe9ad8365314ce92f1e4cc83eb1dbab3c5cc1acb5a"),
		new("item", "earlymodern_jewellery_car_bead_necklace_684", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb74c252d7667058cc0d265d7051590c3bd0c41f9703083aa4f04e0275c8ae8c"),
		new("item", "earlymodern_jewellery_car_chain_necklace_685", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "621f3ac4b20b3c3cd6bdd652b37f6f35090a72a055ec65d9279834f6d93f129d"),
		new("item", "earlymodern_jewellery_edo_earrings_369", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "96246a49fed1da628b393cf7f3614c24155820d06c3665c007096d424a9c3f3b"),
		new("item", "earlymodern_jewellery_edo_waist_chain_375", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bc1b8f8732343393021d12d2ad9e30b1d97dde65d023ca87f4443b6c32d7a65"),
		new("item", "earlymodern_jewellery_ena_pin_611", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d86a990704add649e3e6bd44fd9c2adc4dba9d2442a71d7eef96989715cc9c31"),
		new("item", "earlymodern_jewellery_ena_signet_ring_602", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "91c916447265bd30e1297d4c48b145f660ac6335d13621c860025d5f05415b25"),
		new("item", "earlymodern_jewellery_ena_temple_ring_618", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "466cb75804442610f2cf5fbc0201bcaaad42cecfe6aa6f79814a7c119d8171cd"),
		new("item", "earlymodern_jewellery_ena_wreath_620", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd4df4e9ebb847549f7afd56e3d25c1b4df72980ea9c9306ed1d62cc60ffcaf"),
		new("item", "earlymodern_jewellery_eth_anklet_528", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a20864d14e9ea54b78635ff0c93f537c6d5fe7060720313cc0999a0b381daba9"),
		new("item", "earlymodern_jewellery_eth_badge_537", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d93d0b816f79bc2a81200f38807ee3b80e0c21db760e97e1d76ab95c162cbfe1"),
		new("item", "earlymodern_jewellery_eth_ring_521", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "399367f328eae9f00cd18cf7b91fda6a9e8051b57528e1a044b1152fc9f14600"),
		new("item", "earlymodern_jewellery_fr_badge_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "910b1e7bd1db369164183fc13b470af96d3a1316024da7cb0a8a95e479791c45"),
		new("item", "earlymodern_jewellery_fr_belt_ornament_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a2315a5e64afdcfbaed9d6f2de03b93dd4ebb7d46a84f22aee38fc5df7a022ad"),
		new("item", "earlymodern_jewellery_fr_chain_necklace_05", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "091e8b95539dc2c4a17950a4d0060d3a9284742a14b89dc890f6f5a1a4e9bdf1"),
		new("item", "earlymodern_jewellery_fr_diadem_14", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0dac8cee7e24bdba690e4fdaad719487e5203f23fc81e2fa649b8c4426ae975f"),
		new("item", "earlymodern_jewellery_hre_bracelet_86", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5e95fa9b77f3892a5dfedce2d1d5efe781ac6d6df357f72571d2a8036f616855"),
		new("item", "earlymodern_jewellery_hre_brooch_90", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d686e604eb7dc74ae318ff6b7781c32e0a7dff42524abc3dfec6f057602eb6d"),
		new("item", "earlymodern_jewellery_ias_brooch_450", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d1a7d59e320cfddbb5d8a49bcb5d27d7493d1bad69b56d1247e1724d73055ef"),
		new("item", "earlymodern_jewellery_ina_pendant_623", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae8f68720bebc4e1704f8d85dc9832211f6a62ce306ab98780b00fff8c52c06b"),
		new("item", "earlymodern_jewellery_it_temple_ring_118", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f982eee5d1ca9096a413f4abfe102bf16ff4825d982331258560abc8f1aea8bb"),
		new("item", "earlymodern_jewellery_mag_armlet_207", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d1c3dac08bf2f6f7e2dc946cb040add60aca1384a96ba8a1637b63ea386eba8"),
		new("item", "earlymodern_jewellery_mag_bracelet_206", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b2f39c40ec48f78d25ee6b01003c2a876eeada6b5e92d80fcfe54310a4d43cb"),
		new("item", "earlymodern_jewellery_mag_circlet_213", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d03b0421d908df61c3e266cf9ec261c13472ec359d69dae27c85f5f3d6531c5"),
		new("item", "earlymodern_jewellery_ming_signet_ring_322", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2858de55ca9e122f7153e7845bcdcb3451f657858a21a0f1375765fdebe3843f"),
		new("item", "earlymodern_jewellery_ming_temple_ring_338", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d53363bec8570cb81ccf5e87a41e6c4cbfdcb4cbea6617a104332f85fe68b8b7"),
		new("item", "earlymodern_jewellery_mrd_hairpin_272", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ed4c570e22b88c0c462a14cb6484596791c6b44f050ef30d91c1e2bbc174bb8"),
		new("item", "earlymodern_jewellery_mrd_waist_chain_275", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "258e68f8049bfc3cf8441ceeacf1a32fec7402fa381dbb8987b545f2387c4b54"),
		new("item", "earlymodern_jewellery_mug_bracelet_246", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0467e2df148f737b6dce1f3e9e8320bd5e4a677d1ea9624acd083641f38edce4"),
	];
}
