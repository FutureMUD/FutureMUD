// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch069() =>
	[
		new("prog", "CrApKMedFooPro73TMil-3810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b999c70628c10dabd6bcf0a03a457874db05d8c735560d1e1a09ebbb1a85706d"),
		new("prog", "CrApKMedFurandConCra72TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb39238099e508f7ca26e88fe63ecdf311e947dd8fb57ce44dfdf2234c2b6c1f"),
		new("prog", "CrApKMedIndFou67TBla-415", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2415bdb2dccbffdab9b3df44c639e2cc0c862b4cfe91b139d7ad86a2d544e37"),
		new("prog", "CrApKMedIndFou67TPot-215", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c33ab99896215923311eae65b8cf62783e1235f20747db64ad21fb4c3145c49"),
		new("prog", "CrApKPerTex11TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa46d68ff02cf86622feeed592d4e7feb9e75cf47514321a7423cd53500a88ae"),
		new("prog", "CrApKPerTex11TTai-540", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9227c808d265a1cb5af406b00ebb194ff70395685ceee7054095e0399151bf1f"),
		new("prog", "CrApKPreIndFooCatPro74TBre-3920", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd83e7929df4bd67262db8fba022f85bd5913b9aab3d64b114fa25dfd4c2c373"),
		new("prog", "CrApKPreIndFooCatPro74TBut-4015", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7e57e07035e6ca579119ecc98de6b175d25f2efc4a31661b33f2e4106854635c"),
		new("prog", "CrApKPunTex10TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d748b0af17fcb2d93d571bc40bd4ecf4bb24c4dbb78fae5eeddb80001f646eb4"),
		new("prog", "CrUseKAnaTex13TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "514fc27888934efd31233bb8eeae78b4c1b3e79b196809b17f1fe4c589435c90"),
		new("prog", "CrUseKAnaTex13TTai-545", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1002b728851f8ce536f2c0627e1194d095d712f8d0c393869e30367f6a1468f"),
		new("prog", "CrUseKAncAgrPro54TDye-1710", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9627b6a626c4e324c0902a18a928a9c1873d4949fda9e4f61c703b25fe0e858"),
		new("prog", "CrUseKAncApi53TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5626a8358abc601799dce0a33976704b96bc80f979c80fb578e7ea6876781dda"),
		new("prog", "CrUseKAncArm20TArm-1930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf4add3dd76d0e80a274ddb06b67af9955c763cd0c5bea10f66e7d98da30ae13"),
		new("prog", "CrUseKAncComCloCra22TBas-2310", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6adaed015a31e25a4d42d5d6de87f4ad377204e581ccee66181b0e80b318d10c"),
		new("prog", "CrUseKAncComCloCra22TTai-525", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0db0592dacd46f8575a7c912eff39e15ec5b4eae44b9aba08d6cf2d03a16a61"),
		new("prog", "CrUseKAncHerRem31TMed-2620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c4b86d6aea2e0892207f009790c3ba8faf77de80b0fcb363c32125cc1a026ed"),
		new("prog", "CrUseKAncHidandLea52TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "967f6111c5ac383d96cacc6b2241c97155bfd54143a55469d91b31cd64e45917"),
		new("prog", "CrUseKAncHouCra38TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "761df07dd4c723c487811af03f2a7028271d0d368c83e3583c151028b59d02b5"),
		new("prog", "CrUseKAncHouCra38TMas-1015", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a0fd91e7553d5235346769f43c41375715b775144ef842c744cf2a4b1f6f003"),
		new("prog", "CrUseKAncJewCra23TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "702878d940b90a3111a8a42cd314c08f99db25c958134dc857df5154841411d0"),
		new("prog", "CrUseKAncLigandHea15TCan-2815", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "1580d21db83e1ae6ad6e739988040d44a6a20ec7f7922f9f617be9358f994c24"),
		new("prog", "CrUseKAncMedTreSup30TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d2d46aa3ce46fcb119d261a1dfa490c3e4d88ae04c4b8b1eb084e3e773b87949"),
		new("prog", "CrUseKAncScrImp28TBon-2215", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d0b0377e183e232b1ee8c065aec23830793f07ff98f3dd9ec94c1a866feab17"),
		new("prog", "CrUseKAncScrImp28TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "57932202fc4db134156f768ba631f1e7be61659f53911e5ad924cb0cac87d456"),
		new("prog", "CrUseKAncTexPro3TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa033a525b04961edfc5582c35f90f563e5eeabe2eb34788e2ba0670481ed39f"),
		new("prog", "CrUseKAncWea19TFle-2115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d07c004c3a79d855d4eb77e115e7da1e821de384fe234fda9b05d4102341f913"),
		new("prog", "CrUseKCelHouCra48TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8017ad32594550bcb801bf5e53349252c2919cf9b5365cc0c2fa213730bfec25"),
		new("prog", "CrUseKEgyHouCra42TGla-1135", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c96c65b2b01c0ea891700d21c50b0352979571cd47303ff97cf757cfb657968e"),
		new("prog", "CrUseKHelFoo56", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a3d83d6a6b7a20ead2feec60e6744427098e9ce4b0239e66f1f60a62e16095b"),
		new("prog", "CrUseKMedFooPro73TCar-325", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70088bdc3ea39701a292c3674ecdcbf8bbb97bf765dd34706732fa6d8a9b29e2"),
		new("prog", "CrUseKMedJewandDevCra71TGol-3510", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "df31ada37fc422d297a9e3dff50ea77225b19ab4be215426dacab1808ea7bf21"),
		new("prog", "CrUseKPerTex11TTai-520", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "86d039be94e8fb95c2ae008beebb861c8637878970bc0cac1c0578b9812c0c9b"),
		new("prog", "CrUseKPriProHisComWor2TMas-1020", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c7278dfa039cee88698f7c34c6380338581cb9167ad2d5c4956953da6107e44"),
		new("prog", "CrUseKPunHouCra46TBla-425", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3af6d2d7524449b2949be7429cd2735d96d295e92162d2aa705fc5f9ba8aaa9"),
		new("prog", "CrUseKRomTex6TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "df051728d7362b60ba3d7e08ac9984b6317fc4073fb825fe60eb064d2e2662dc"),
		new("prog", "CrUseKScySarHouCra47TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5794fd71fa347511361ecae87f89c47b482b0307ecb9b3cc8c631c0cf5a1b74f"),
		new("prog", "CrWhyKAnaFoo65", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d18697fdd8bc648bc5231c82399d402912c9a5701e4e59528a748169d4159af4"),
		new("prog", "CrWhyKAnaHouCra43TPot-220", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2c58c5105c6ce560f26470d87bbc3eea764d6c89ceb7d538da415161c9ff6bb9"),
		new("prog", "CrWhyKAncAgrPro54TDye-1720", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "89b06ebfc1bc2ca0209cc8e70248660d45c4fceba3d8bb8c4a336c9031d540ac"),
	];
}
