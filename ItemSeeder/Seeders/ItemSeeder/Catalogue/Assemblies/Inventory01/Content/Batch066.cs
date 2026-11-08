// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch066() =>
	[
		new("prog", "CrUseKAncHouCra38TLac-3035", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e56fc9a624838907ca21f14d4a8b3df2b7e49f121ad80476faed4653716a064c"),
		new("prog", "CrUseKAncMetVes39TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0f41d38d2eefb695cb38e9febf5eba8bccbcca024559aece053f028b3c790ee"),
		new("prog", "CrUseKCelTex7TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "06dd1943dc8e63dbc4609cfd45fad204e333b5af67e6e5c2038959019c73b2f7"),
		new("prog", "CrUseKEgyTex5TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "74f181b78f7a0f2f8959dc70f77bf7144d7f23c0cb97eff9df85069f7db2a0c4"),
		new("prog", "CrUseKEtrFoo64", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca2c77974b5c2221c9be949d910ea0bfc7c4cad56606fde64ee4a367d9a45771"),
		new("prog", "CrUseKGerTex8TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "e64ea0ca7a5ea9ad477732f7f05e00a75287c390f4cf5a24f8838ef253b9ef78"),
		new("prog", "CrUseKHelTex4TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fa2aaa5f22808d882f6e1ed2243400794e36a4408e5b93e99c3d0cddc3ae793"),
		new("prog", "CrUseKKusTex9TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57243815e885f854a0e5ee785295616ffa2b8406a58611173b4163fb0fbdff54"),
		new("prog", "CrUseKMedFooPro73TBre-3915", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2dae667067b67244778361b9ec54c95ff5870ba4b00569ead17759b5e85ab0f2"),
		new("prog", "CrUseKMedFooPro73TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7df965e7f49600498701184a4edabeb14665ba12ded4b33c7dfd5f7634eb9c53"),
		new("prog", "CrUseKMedIndFou67TBla-410", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e30ba30014c3c93f0b1923c0bdf4ae6e551eddb9446a2a01395f6d4cc7026d9"),
		new("prog", "CrUseKMedIndFou67TBla-425", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a062a0ac299f147129b13ffea8690ec801e5732f9a671ac858ce7af43292cef"),
		new("prog", "CrUseKMedIndFou67TBoo-3425", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ea59715d391e3facaac8dd4c8c081335a7d7ad4e79635a6b73a3c4907219b7f"),
		new("prog", "CrUseKMedWriandAdm69TBoo-3410", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44985460987efd8db3243e0742a16929c2495463760add8e73c7f2c44affc27c"),
		new("prog", "CrUseKPriProHisComWor2TPot-215", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22a8134ffc00cba6926958fc7dba854b1ddfabef914ab61d89046dccf9adeb3a"),
		new("prog", "CrUseKPriProHisComWor2TPot-220", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c817500d7c9fcdc2c3246bab31b40e8467d33eae535eaeec99280a35a896c818"),
		new("prog", "CrUseKPriProHisComWor2TSme-930", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6c81169796f8af7641e8105f861038a64584be99bab58bee8052c663b7cc5f3"),
		new("prog", "CrUseKPunFoo62", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5cddba3c08d8bf7da1d9f670b5f5332748c16965aaacc74dd8f3fc3fbeab7e0a"),
		new("prog", "CrUseKPunTex10TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "be15d58552f04425239a989b1fe04ff1903b37926dfba68e38c059b06eefd222"),
		new("prog", "CrUseKRenFinIteCra81TCar-315", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fa4479984a63d7ea13d65be36211287a9b000f029815c481b4c8e28a680b146"),
		new("prog", "CrUseKScySarTex14TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca508435c52d0e6f3a0ba7b331294d5df7d887e322ce358be416818230323cc5"),
		new("prog", "CrWhyKAncApi53TPot-215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "34302aa08ff53d095c5ef0ddcf6b1d665034134d07e00417277447399d3b3608"),
		new("prog", "CrWhyKAncApi53TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "caf315718e2f8f9200ec5fb352d3a0396a084be19d7ddf40fa5d8a79e1baa8c0"),
		new("prog", "CrWhyKAncJewCra23TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "fa4376f18fdc3e41408bf8257c7b0def2bb5939027b98a4345945b28caada603"),
		new("prog", "CrWhyKAncParMak25TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57d6338a41224f1c34135713c9fd9a9928c79e87464ab895b2799dbc790d8287"),
		new("prog", "CrWhyKEgyHouCra42TMas-1015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "da803c59580219899e09b3c91dff9959c2f66590857e07e989f146589800b17c"),
		new("prog", "CrWhyKEgyTex5TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "99ada739f4d5d8c98bcb8e915a787eb407bae6d2f901457cb453ba77f0dc129b"),
		new("prog", "CrWhyKHisWorFou1TLea-615", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec43ee8af010f801b3d8ae25f2d77aefc9ae8140a1ac81c69b6592492871436f"),
		new("prog", "CrWhyKKusTex9TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1ad9659bc84b9793ac3dc88e35a39e02982e956ab126f48dcb1aba2419ee1f8"),
		new("prog", "CrWhyKMedFooPro73TBut-4010", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d54390dc6684e259291c39b97cdfefa5c0837748780c458f7515426bfd69ec58"),
		new("prog", "CrWhyKMedFooPro73TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5b27cdfa0e6b921b4e1f9a6475986fff6cd988178ac73dcc7f11552cb7c9a181"),
		new("prog", "CrWhyKMedFooPro73TCar-325", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c85f92a2c4b47c4a2c398da07570f19135744a0a732cba20ce0ad68cde80201d"),
		new("prog", "CrWhyKMedIndFou67TCan-2810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5df7e1bb729fcc8da0cb6dae383baa7837002880db21beecf81ef257e9411c3"),
		new("prog", "CrWhyKMedIndFou67TWea-1515", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dd18b26d6297749caae364ab2d3a0c1703cae59a5de3538584a020559512d4e9"),
		new("prog", "CrWhyKPerTex11TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7d848ce5ed249fb933afec764caa4c5615d7de38f0c15fef5e15a68116e18f4d"),
		new("prog", "CrWhyKPreIndFooCatPro74TBak-4115", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd005e2bb18778dc4b542b53850cbd7d6bfa26eecfb42eff45494df5ee9d8493"),
		new("prog", "CrWhyKPreIndFooCatPro74TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48edd07d39eb8c9400f28c12cf7ab6ee186e97bb7904307a2b35a32d49ea6c67"),
		new("prog", "CrWhyKPreIndFooCatPro74TBut-4015", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1babf586f50efca2b44cf0fb15f4c85e19f8cd335d634eabca76e12c44b71528"),
		new("prog", "CrWhyKPriProHisComWor2TMas-1020", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0395364aa80fa7f4d6b4c9060c6515abc99e9ba1b38c26f5711ca7d25f317a37"),
		new("prog", "CrWhyKPriProHisComWor2TPot-225", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "407366816822413ccf92f71a9477cc03af50ef93fe5bda74ec5e481ce2b2bb6c"),
	];
}
