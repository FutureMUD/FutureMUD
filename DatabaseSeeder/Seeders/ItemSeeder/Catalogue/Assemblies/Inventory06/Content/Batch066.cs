// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch066() =>
	[
		new("prog", "CrUseKAncGla18TGla-1135", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ec7deb0ff6ccc63d32334c8796bfd30fe42c2c4a7d89dd65cb59da38ddf0a10"),
		new("prog", "CrUseKAncHouCra38TDye-1715", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "500ad890f067bb7940f3d2510bebecb7abaf02a1c234d3f4c693859e54f64477"),
		new("prog", "CrUseKAncHouCra38TSil-2415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "24879aa186abc390e8ef07feadac4f8bde9bd6987167ae28db28112455f927c3"),
		new("prog", "CrUseKAncJewCra23TGem-2515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8acbb9f18447f604b54755df143d29cc462d2bd0e05c504500017792b0f6c769"),
		new("prog", "CrUseKAncScrImp28TBla-420", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "83a6ee394c743b010aa8d46a38c328f463280578fd88416bcdeee09de2da63b6"),
		new("prog", "CrUseKAncSurIns33TBla-420", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "be78910ca4ea7c3e3b0913fedb6e768c01cb778af2948f1490f17265cc9e216c"),
		new("prog", "CrUseKAncTabMak26TCar-320", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "49691a9e5b200da0217a523e59a3161c6c5db79c7df1b7e9ec93bc3c206b3748"),
		new("prog", "CrUseKAncTexPro3TWea-151", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "11142a384c8c9748a7e242433cea7087b43ecef33b927a402592307358f39814"),
		new("prog", "CrUseKAncToo17TLea-614", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "104947eb29fe7f1c42e3e3ad9be8400d6af608ab72d40635336ffd8a781c6d78"),
		new("prog", "CrUseKAncToo17TSal-3112", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "07d77de0093b1b29bc11db94003724248824127ae1f007af229f620297815b29"),
		new("prog", "CrUseKAncWea19TWea-1815", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ba7e78073b92322d6f090e593efdef30d0435b6f482f3d163f3fee94ed2e285"),
		new("prog", "CrUseKAncWooandJoi34TCar-325", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9af730a2edf9c59566548ef82114cfe9f159c93c033a94e71b939ca80e7402d2"),
		new("prog", "CrUseKCelTex7TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1cbde57737f7f74897fd11517864c71664a3cf3d966d78c3377ab85fa117210"),
		new("prog", "CrUseKEtrTex12TTai-530", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "07b871c57d63c3a904d4254447fdc15079239110e0e5d6205c20f0a63b3d7efe"),
		new("prog", "CrUseKGerHouCra49TScr-2930", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1b4a6eae80223d5292d733b0dd40e1119ac19d2828d3582ed6ba4e0b18574ee"),
		new("prog", "CrUseKHelTex4TTai-535", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "30c0b77503d2d3771d34281fc4c5a626b98da8b7e71874d7eb79c83ebb37a46e"),
		new("prog", "CrUseKKusFoo61", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3aacf01d1e2197d57ceb1a547525729a3d7b8c3031d7a8bb4b608a65289a259d"),
		new("prog", "CrUseKMedFooPro73TCoo-715", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b974aace6bf38e42bf8145f55ab7f365fc87dfd4ecaed221d44e2efa4e217290"),
		new("prog", "CrUseKMedFooPro73TMil-3810", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0c93ec382883011ca8fce5f851f55ee66b77f07f0124856f8012c90d47acbbcc"),
		new("prog", "CrUseKMedFooPro73TPot-225", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a1ceb3d69487709f3366fd49fd17f7714afe38cb3a544f139d4e61e94456b8be"),
		new("prog", "CrUseKMedFurandConCra72TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d0e229ce2b16a2e4c05237f565e64432f987e9b549036de4c63b48acf9d9c21"),
		new("prog", "CrUseKMedIndFou67TMas-1025", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be4acd966945de9698e80a850843036b11669bfa042ad5aa9d8eb32c07868c61"),
		new("prog", "CrUseKMedIndFou67TPar-3225", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c265ed2230263d31f29c1e48bc94ffe7f743c213c0f1e0e2cc14c164b0d26ac1"),
		new("prog", "CrUseKMedIndFou67TPot-215", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75c52b3019911603ddbd39dc964a608025f5abd0f9367fde2acc3fb3b64d3bf0"),
		new("prog", "CrUseKMedResStoCra76TCar-310", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d62d66cff9ae5ec013b98e7faa6be4efc373303f9e0dfc23d4f09197cf3dc91"),
		new("prog", "CrUseKPreIndFooCatPro74TBak-4115", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65c16ee0cfa47b003af2bb9ef1e78202c10001a799defb9cc07f6c5b4f4f28f6"),
		new("prog", "CrUseKPreIndFooCatPro74TBut-4015", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0423a9cfd9e8b5821a336de10d3e407ee916a27a344ae0a8935b69232af160d4"),
		new("prog", "CrUseKPriProHisComWor2TPai-1320", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "73e286d05dda6861382cadc3f06c4d127d4d61b8cca0a9b520e3e1c64399dc77"),
		new("prog", "CrUseKRenFinIteCra81TLea-615", "crafts", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f15ec199439911df16ead4931fa6b703a5285143a3939cf49e8fe75691488766"),
		new("prog", "CrWhyKAnaTex13TTai-515", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "298a3aceeaf22310036585ebd7cb485ec4d173ccab780527db1f6662853c0df4"),
		new("prog", "CrWhyKAncApi53TFar-1415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9185a386e536ec408f2491bf8097e2d140208e001cd30cda562fa770d5b84fc2"),
		new("prog", "CrWhyKAncApi53TTai-510", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d040e3fab27ac0c21d9bcb7548304f457edbef90d01f362a089d4cbf9a47f8d"),
		new("prog", "CrWhyKAncHidandLea52TLea-630", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddee03964c113f370afe5a97a77ee49c6e8451215bed7d250ba0527c45d76af5"),
		new("prog", "CrWhyKAncHouCra38TScr-2915", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "826f96cc544fbdcaec26fae4eb1ebf8b1a5d68dae22bfb7e7ea8e70e4db6837a"),
		new("prog", "CrWhyKAncJewCra23TBla-415", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b4c316c160066527536d5f90595b7de425b1f3ebd47e66207b9314f0d6b9a8d1"),
		new("prog", "CrWhyKAncLeaCon37TLea-620", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b7baa615152dd5bc10d9608587dbebcfbbb1cad4b9b465dd10c8720983c7e6d"),
		new("prog", "CrWhyKAncMetVes39TSil-2440", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dc323a93db9a84134e718c269727b7ece071c0203da337855480580b2f04df5f"),
		new("prog", "CrWhyKAncToo17TGla-1115", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0de038c3ac4fa27b269355ac9d9ba7a48e09082886e13b6e42402e7df1f3d946"),
		new("prog", "CrWhyKAncToo17TScr-2916", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4ee8773ed07daa86af8b1a06da201b4ca90cfd79c31dd4b4211f460293586bf7"),
		new("prog", "CrWhyKAncWooandJoi34TCar-315", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "ff8e45a38d02ea59476d4856a12907740eb8bc12f3af9a051974cb63b0a0d85b"),
	];
}
