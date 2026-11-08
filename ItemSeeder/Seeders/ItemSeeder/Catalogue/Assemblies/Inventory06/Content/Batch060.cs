// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch060() =>
	[
		new("item", "renaissance_jewellery_cen_anklet_68", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "54b76c3f49ccfdce8491d866e5595c14bf286975f1a0c4237305acb16e5f39f0"),
		new("item", "renaissance_jewellery_cen_pin_71", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "014120949bbd55a9f449aedbc7c08795f2ee8abb6c54b8d849a15928cc8bada6"),
		new("item", "renaissance_jewellery_common_anklet_38", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "77c712fe0abffed83676471653cd65ef2778fd7e87a72a1597ebf62c9c2bcf4d"),
		new("item", "renaissance_jewellery_common_armlet_07", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f47dbdce8dfb1b615c280eed7f10d4407c6274b92930c15f40189d164ca5d44"),
		new("item", "renaissance_jewellery_common_armlet_157", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1d96a8b154cd95dd2a90b34e9efaecf2994c8796fdf6d2123daab0c98f5f8169"),
		new("item", "renaissance_jewellery_common_badge_137", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1d52a2ad61e26c07c64c1d4372e3d2b0c4553facee2073e0efcd133c78e6d3d"),
		new("item", "renaissance_jewellery_common_bead_necklace_94", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "37c996c16348272336c9b233630852d8aee7e0ea81ecd9642118d66f084051f8"),
		new("item", "renaissance_jewellery_common_devotional_badge_113", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3deee10021d120ef110d00b6380e00dc79d07bcb1c548d76d3e371de02c5a7ca"),
		new("item", "renaissance_jewellery_common_diadem_44", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f55168df18e41690f5fb725f3e62df794c1d58cd5fda8745284ee8ca137d4b71"),
		new("item", "renaissance_jewellery_common_earrings_69", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb56858a31ce62bed43b5c2e845fae121890666aa6458b9f31eb85ef8611b84a"),
		new("item", "renaissance_jewellery_common_incense_holder_28", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ca0c16704489763bff6affa54580e2733e3875580386eb0222b6284ad0c7ebc"),
		new("item", "renaissance_jewellery_common_pin_191", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d353106f354011a63a6b209d7d9142b8df27b17ca5fc59dff3edd17865c0cdf0"),
		new("item", "renaissance_jewellery_common_prayer_cord_114", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "358e8fa727c2e390282a9477e0d34c9bbc248996347d2280018d0d2d829f0562"),
		new("item", "renaissance_jewellery_common_prayer_cord_24", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a948d7f84e5d49544ce4007ba69d028c339a499b2cef095aa6f7520680ae8114"),
		new("item", "renaissance_jewellery_common_signet_ring_182", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1333e1a089f886fa79ed46352d80d4a036f1ca307fb434ad78d9133e302cea1b"),
		new("item", "renaissance_jewellery_common_signet_ring_212", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "939c7cd86232764227d785d250c8771208398637d7fd8eb4b1812fbdd435cb72"),
		new("item", "renaissance_jewellery_common_signet_ring_92", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "40bfde460f59273f6b0c60f1c9c91db0173ad3a1ca50a0bf56fae6747087ae28"),
		new("item", "renaissance_jewellery_common_waist_chain_105", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae3b4b64e391e8262ea59697c61a0741a1d030db95c5fbf30ce5c293f2d68b8c"),
		new("item", "renaissance_jewellery_eal_armlet_277", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6cd8e0f6d919912fdc485b2cee74f3909a220aa49ec1ddb030f80b4b3341b90e"),
		new("item", "renaissance_jewellery_eal_hairpin_282", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7135e25680e6b6f3d6654dc72c95cae8fb7b5d751f3f8e1f0262b388b78a92df"),
		new("item", "renaissance_jewellery_eal_prayer_cord_294", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "718df4b820ca4be948ee56b9127c9f265ff982ec05cef707adbf4ed4953bc1ed"),
		new("item", "renaissance_jewellery_eal_waist_chain_285", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e70d50ba76b17531f452bc936c1c2287d506bda44039c00c478b9816fbb786c"),
		new("item", "renaissance_jewellery_eon_anklet_158", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d6a134661abff18248901d61aa230f8d35fd77597e7ef882701eb0f70a9fbb9"),
		new("item", "renaissance_jewellery_iba_prayer_beads_51", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "16f589f4b9612a681ae52e62290a8f3f31587cfd1781b734f9f54f62fb00312d"),
		new("item", "renaissance_jewellery_iba_processional_ornament_60", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3526f88d5faf4b27385903b862b6a5e5af9780fcb8b334d0331d52272a84dc44"),
		new("item", "renaissance_jewellery_iba_torc_49", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "17920c12fe318a798ea57a9d790f1ee8131c3d4c79810d34a023182b6562df2a"),
		new("item", "renaissance_jewellery_iba_waist_chain_45", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d5c78a6e14d4030ff09d8f0ee1cb53b978ed940a28a70bd94dbe265faa4e6b6"),
		new("item", "renaissance_jewellery_ind_badge_557", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4014956eb833636b83f05347e80196a035cf2ed1b060c70d9b1eaef4c708fa4"),
		new("item", "renaissance_jewellery_jpn_circlet_313", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "159436bc34a35ad6b46560f791b5bdd13223fd7e272540c9ab1d264296a241e3"),
		new("item", "renaissance_jewellery_jpn_offering_bowl_327", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1fa6cbe03e93889289108360aa1e8f2bf2953f6d7485da39420de9209a0ebb51"),
		new("item", "renaissance_jewellery_jpn_torc_319", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "336e0f5bbac7fd5132c4429625ebe33626aa9f099176a617af6300809a86928d"),
		new("item", "renaissance_jewellery_mea_circlet_343", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "574e82d973cc4ddc0f413e1d0a06e250b392dc0a6a7d7769ca18df4188a6b115"),
		new("item", "renaissance_jewellery_mea_devotional_badge_353", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c03df478cb6ed0ea7965a5556dca04a1685962f2d5444786d310054c848128f7"),
		new("item", "renaissance_jewellery_mes_prayer_beads_591", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3978acfafe8773a9787b3abcaff12fe773f97b969b8b80424f93a89d0b650bcb"),
		new("item", "renaissance_jewellery_msea_anklet_398", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6782ce445aa661ccd6cf43c3eecbf26b217a940131813a0b7d72a9bfc211aedd"),
		new("item", "renaissance_jewellery_msea_bead_necklace_394", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b99465d6d4387ad737753ad5aba41231336843c628ce965cb9688f05f7269f15"),
		new("item", "renaissance_jewellery_msea_brooch_400", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b5c476054b3428ff61a212f69ad2789bd89f28b06ac490cf4de051cdef99552"),
		new("item", "renaissance_jewellery_msea_ring_391", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "016a11bdf3901aac289ccfccbcbfa0e21dfa73c5e13b06d4623515a96b4ed4da"),
		new("item", "renaissance_jewellery_msea_torc_409", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b258a0599617360785eafe49fcc0aaf9566c6a05a17d264cf2e296ecb6d142f"),
		new("item", "renaissance_jewellery_nac_badge_677", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1745b0bceba05f74a37f7694e07da8d111a0b3d07a9b754db174a1d2c02bd0bf"),
	];
}
