// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory04Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch064() =>
	[
		new("item", "renaissance_jewellery_nac_anklet_668", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "06a85002d12c7b1d7c872f2b70aa6cb8fa4938bbe826af242e7ee5d73a77b082"),
		new("item", "renaissance_jewellery_nac_chain_necklace_665", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "575751b7d453729c7dbdc545c7c58ff99dc3d97cf17baa040712a41124e3d41c"),
		new("item", "renaissance_jewellery_nac_signet_ring_662", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "99d905c062d7fa59655f371d9b3b383c594b9dbe258f4604fea1366a9d210d3f"),
		new("item", "renaissance_jewellery_ott_belt_ornament_196", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5c794f4f1319465e61c5a704482c9d85dff59b7d7991b26ae3639b4d57e5a56"),
		new("item", "renaissance_jewellery_ott_pin_191", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f900b8d6786e6876ea9d48e4012e32574e131866e4ef3fc457f17145e53605c"),
		new("item", "renaissance_jewellery_ott_temple_ring_198", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3923565660590985c11d59b10715b0dad60df2b36e82a535fc7859aec7b91a3e"),
		new("item", "renaissance_jewellery_pip_badge_227", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aabf181a6ac347eb0ae249ea30a468afc24ba36480b8694ec815d614f26be708"),
		new("item", "renaissance_jewellery_pip_devotional_medal_232", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e3f7d89716e0028780d5bb2095bdc83c3568860874edca442820972ad81d0f5"),
		new("item", "renaissance_jewellery_pip_portable_panel_239", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "10ff9174f2ac01cf695bea91a87d02a5fd5637e1edc840ec43a5473df620c7c2"),
		new("item", "renaissance_jewellery_rse_belt_ornament_526", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5934254a5101bd711be16950608be040438d180dc4eff8945e41f902c1276c9b"),
		new("item", "renaissance_jewellery_sas_armlet_247", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7b7ccbf21b9c8f8394406344d3b20a09a12cb55840646871c00d76d65910956"),
		new("item", "renaissance_jewellery_sas_circlet_253", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1be9df172a01b51deab16eac82790c63166ebfec8ea97433190bbed055e1b4e"),
		new("item", "renaissance_jewellery_sas_offering_bowl_267", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67ec934e43f24598b698b42325c5b6d4dc4127689fc7293cb59916ea0784e6e2"),
		new("item", "renaissance_jewellery_sem_chain_necklace_365", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "613bf22c1bb411c9a81c4bd550de4ae09fcb3c56871a7326c137ed75f2c2b915"),
		new("item", "renaissance_jewellery_sem_pendant_363", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e9dc64591be819e1862108d2c87d342f336fd98c34bb9c10b93aff2b0f25a7f"),
		new("item", "renaissance_jewellery_stp_armlet_427", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2a8e317815d0a7c03d3b4a1fb5698c6c9eeef9040adf9e285ccca6ec46eff08d"),
		new("item", "renaissance_jewellery_wer_pendant_03", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "75bffa18a04a9a84d30c5735775d8fc0854f199efe0161204122ffdf4cabdafa"),
		new("item", "renaissance_jewellery_wer_portable_panel_29", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f05356fabc0b881cb264738452119a1bed773064da1fc865780cfe71aebc844"),
		new("item", "renaissance_jewellery_wer_wreath_20", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5bdd88cd67d885978e790c1449fc500dac4a74504b15f18dcfdb4e1813e67ecd"),
		new("item", "renaissance_joseon_tall_horsehairhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5673e69e302cf43e3536596f9586442658aa98e49174591662e615f3f4928903"),
		new("item", "renaissance_medical_dissection_sheet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44782beea707c6df26726c5a7d33af64ae130f2324e9b6c12569e78bb60c62ee"),
		new("item", "renaissance_medical_drug_sieve", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c2c4a887935bfc8ff6ef86138340762e98a11d02476c5460cdf77199b45519b"),
		new("item", "renaissance_medical_fever_compress", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13e3964c12d10ba431fd5454ff52d316468dfe0b38359e21b162dee29065f4a0"),
		new("item", "renaissance_medical_fumigation_brazier", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca1bb3e3cb260c67860d7457b3266f8e843088af2d35e3e6aa7cbd5f62ad29c9"),
		new("item", "renaissance_medical_horse_fleam", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1730986b41802ae592d0ff0cfaf55ed703662788c25939f480f8a80ebed78d56"),
		new("item", "renaissance_medical_mummy_powder_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9969dd919df5322901af751f8060d1aa7d77bcb61db35a053461a4abccdc770d"),
		new("item", "renaissance_medical_neck_support", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "44979eaa971d6a4706c847856e5ffeb557f8c56292a0ca9afb693850196f9346"),
		new("item", "renaissance_medical_sarsaparilla_root_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b33c0f56c978d5085eca2964c96f1062cd971624dd04d1e2834f790358e2518"),
		new("item", "renaissance_medical_stable_eye_wash", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "653b55ff8316134892eea97bd5585e506c1b410899b2e9fb85226b2d1b7dd179"),
		new("item", "renaissance_mesoamerican_featheredge_crown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "97a8f027ecd64fe13b766e347f13c08b7897057534d5c8a1360f0c495d5bc00a"),
		new("item", "renaissance_mesoamerican_featherwork_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4254d450d0cfd077c0d721f097fba01f00848956c9166b5319470b346bbbfcd5"),
		new("item", "renaissance_mesoamerican_long_wrapskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b3c422eb712b021aafdb9d54ad3ddb6a2302167311136113a3ad0258f02fcda"),
		new("item", "renaissance_mesoamerican_panelled_loincloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ea993273039efccfbd3878db10d19ba90bfbe024bfa9ef62fa4b0627487264b"),
		new("item", "renaissance_mesoamerican_woven_headcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5720c9d5b67d50db752948628b00865b8084c1a82b872466e4c1d6b2f2f4a5ab"),
		new("item", "renaissance_military_bazubands_safavid", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7be091f3d59310416b9287590a0ccf86b0bd9204dabefed894e8986a780bd843"),
		new("item", "renaissance_military_jousting_breastplate_reinforce", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e08a3d93947c8c022386565fe9d1199e0b2da9f2ccd0b885e93976889465e5f9"),
		new("item", "renaissance_military_menpo_japanese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "95efacd0ec66ed7b17a59ded2ea6c6371f7693f723f6058e25fac783cf701070"),
		new("item", "renaissance_military_rajput_coat_quilted", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf4134f090c1e8127e8d13ddfa0d8e4a9edfd02d75c6ad2ece9cf78e8e8b0920"),
		new("item", "renaissance_military_rapier_swept_hilt", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d9e737ab024ef7b5c9e96c274c8f953d3de18f3c151015642baefc1dc4980b0"),
		new("item", "renaissance_military_rattan_vest_siamese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a25e7dcc0e2f8a75ad9daf061997c611c97e9a956967738ac951cd013778155d"),
	];
}
