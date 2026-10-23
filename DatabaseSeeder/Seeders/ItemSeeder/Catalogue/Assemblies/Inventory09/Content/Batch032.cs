// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory09Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch032() =>
	[
		new("item", "earlymodern_household_dice_pair", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5fa926019732809075802aa9bc2de91431d22ab090640fb41acae376cc2735d"),
		new("item", "earlymodern_iberian_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d47c12796c84b324c0f7bf50e20208714ef2cc3b17d8716430c20b9bbe1a39e"),
		new("item", "earlymodern_italian_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10807ad5418685ee3c2e1847ca3c786f185b7fc4df1aded7c18cb092699d5550"),
		new("item", "earlymodern_jewellery_and_bead_necklace_664", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9d11a32f169fc502f1ab1076ad8eeea2868a57f31bac6469b1d1ba6828aae2b8"),
		new("item", "earlymodern_jewellery_and_pin_671", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f959787e17d100eccff0cde1f552f58074111b67527e6115fd9f356eacdf41f"),
		new("item", "earlymodern_jewellery_bra_armlet_587", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d57e4e01493f9bce1b29b2dbf061351bd1e4d99790d6730b8e2ba7b8e9b2daa"),
		new("item", "earlymodern_jewellery_bra_bracelet_586", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0c5be2d021473bbbc16d12e4d74aeca0207fb939339f1990353f7ab828a06eb"),
		new("item", "earlymodern_jewellery_bra_waist_chain_595", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "768ba145d1c249ba8be10e2a9bd85522d19224387518f35b226a227f428b4416"),
		new("item", "earlymodern_jewellery_bra_wreath_600", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c570586d269229e9e2217a0e38554f9d5ff8616999e2f823a53334e585bdee71"),
		new("item", "earlymodern_jewellery_car_brooch_690", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1214148255de512500eccf4d99e1057fb02707b8803f7c7d9aadd74f9f611c2"),
		new("item", "earlymodern_jewellery_car_hairpin_692", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "73d93201bd573c198e9c479f2e6c5c4595861e1fcf33f47dc9e162424bff095f"),
		new("item", "earlymodern_jewellery_car_waist_chain_695", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "39d56bda3387c6e336300a44ffcdf248405ea7763edc1d74f0fff8069469dfd3"),
		new("item", "earlymodern_jewellery_edo_chain_necklace_365", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9bb6e64dc5178007b3bcd7888186ac474af8e93e5ef28828ea7aa96f4fc31f82"),
		new("item", "earlymodern_jewellery_edo_pin_371", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ba7d29137533f0174166ede7da77cc1012e5d6c3456b0bcbd919c0bd3c5ad65"),
		new("item", "earlymodern_jewellery_edo_wreath_380", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "84c2afffaf9c3d51f0467625fbd8924fae3a20bd30e7da7d10aa4b40d3023bc9"),
		new("item", "earlymodern_jewellery_ena_circlet_613", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f433092c973f5bdaa7cd27ececdf5102b03febf3e0258639821b23d1242e5cd0"),
		new("item", "earlymodern_jewellery_fr_armlet_07", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "719cd2e3f62388f2f63e7e97ff8f43c8b2e898ce9ec2e3e5b4592b816caf8ad3"),
		new("item", "earlymodern_jewellery_gb_pendant_43", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b9f26e629b347c2d87ed38a655feaf470854a03ba856af91f924870068ce2ea4"),
		new("item", "earlymodern_jewellery_gmt_bead_necklace_704", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d67c943d081ae15c3f4d3e5ead4de3d2aacf70341dd792c348777d4881aa75a7"),
		new("item", "earlymodern_jewellery_gmt_belt_ornament_716", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cceca990956eea30aa6f44277cdc84771a1e0036e077933deea47e3f7c2d69fc"),
		new("item", "earlymodern_jewellery_gmt_diadem_714", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b38d10329bbd45908ce0e1fb362ce3fc1dec32956bbe2af484114ccea63e394f"),
		new("item", "earlymodern_jewellery_gmt_temple_ring_718", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "38b9968410b688d04d257b24e468fc07ef1c7c36858cb0d607a4a079eaf7dbc4"),
		new("item", "earlymodern_jewellery_hre_anklet_88", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7fdb533760f30a62fb20f4988453f8968e031ae7a8a1168d51e26bfa145f892"),
		new("item", "earlymodern_jewellery_ias_wreath_460", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b691c139fa6bdc4cd4f65bf77319f44c553a02abc5b4a4bf8ae994cfcde7617b"),
		new("item", "earlymodern_jewellery_ib_chain_necklace_65", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3b12bb62ac10a3e204b088392380b4fcd3255f3ba61270fb45bc76aebff752e5"),
		new("item", "earlymodern_jewellery_ib_diadem_74", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f02dc94c6ac591f72aadf3595cb28d13e73da03fafcb06a085d9876039f9aab8"),
		new("item", "earlymodern_jewellery_ib_signet_ring_62", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e29f1b3e394f5d794981f68900a3769b4c7e5b6f5a141da7ef71f5c82bdb9f2e"),
		new("item", "earlymodern_jewellery_jos_bead_necklace_344", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b1ee045722a1e8bb71c12efae31f3c1aca9375b1e26bcb049f082920adc4e367"),
		new("item", "earlymodern_jewellery_jos_chain_necklace_345", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "01f8e37275ad828035485dd6d6497eb38d602c2f847c5c02b0b16d234c98c0d3"),
		new("item", "earlymodern_jewellery_jos_diadem_354", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d16cbce929a667721e95c5ae3c3d40f418fa341691096a5d813fd6ec138685ce"),
		new("item", "earlymodern_jewellery_jos_temple_ring_358", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "05e0e4ef8991068ddadfd0e1df3ab80b78d4f19ce57b48905c213225073eefa8"),
		new("item", "earlymodern_jewellery_kongo_bracelet_486", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "378513e63c110fbbbd704b7e5f81720783fb8c740d580f9c7e2ce313cbd65690"),
		new("item", "earlymodern_jewellery_mca_bead_necklace_644", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb9fa76955455fe7ecb0f68df1d45fa8e2d00269bb5008262d42c7088fb4c03e"),
		new("item", "earlymodern_jewellery_mca_belt_ornament_656", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a08e72d930977a8aa33bc057224c94df9d79c4f29fcbf2b4faf25e32fcc1682"),
		new("item", "earlymodern_jewellery_ming_armlet_327", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "36bf1a0593610123011548bca6ca184d4b643a20674ee3982b2311289703bdea"),
		new("item", "earlymodern_jewellery_mrd_signet_ring_262", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "72455725771ffd72a8b8213dae2ed61b7a66b36deaf1ed33d959f8c40389d9ac"),
		new("item", "earlymodern_jewellery_ott_brooch_190", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9033df1c41812c942797c68bcae8c90700f93d9635b67485f2b49a51427cc403"),
		new("item", "earlymodern_jewellery_ott_earrings_189", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad8665f2f323d8899ec10547ac9db06db6109e9b52895d4926e4a335c97198dc"),
		new("item", "earlymodern_jewellery_plh_pendant_143", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "056a7c044339aafdb711526e9031e82f1fb6293bb0bc09fc99da60d46ad6c272"),
		new("item", "earlymodern_jewellery_ryu_bead_necklace_384", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c9eeda19a3565a5ecf5c0f97015872b4d643aa40d47781179a450633ba64f46"),
	];
}
