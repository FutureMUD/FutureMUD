// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch052() =>
	[
		new("item", "medieval_work_wool_cotte", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "600c669f5e2eb1d79e51852e1381653275c61200cdb07aaef97cabd6f850e41e"),
		new("item", "medieval_workshop_papermakers_vat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7557bac9d82d3027885daad97c4b4d79daff9467c699eae7a0ab824edf735295"),
		new("item", "medieval_workshop_pole_lathe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "74d0c4ea69a02043bf87ced497aee210e75d01c7fb3fbca962023169def81371"),
		new("item", "medieval_writing_birch_bark_account_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2b51191d88bb97b34bc338c35248ac61e6ed96d9cdae604de957284a2cce2d1"),
		new("item", "medieval_writing_bronze_travel_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "da5b8a969466edddeac06ef94ce4cddc74f21a81dd9e1cf212f1d2d1a460f310"),
		new("item", "medieval_writing_courier_document_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb5e13628d478dcfd2f96467e771fb43a4bce96fda2aec37e5fc7639aac36531"),
		new("item", "medieval_writing_deed_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2694b7fcc17f88f91d7340b70bed319ad593b1d760ec71bb181ad33ac7cd4a8a"),
		new("item", "medieval_writing_east_asian_account_book", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "894e4d95c9c6b96ddf495cbd978dbecce4465835fb4113a982710688e2c8bbda"),
		new("item", "medieval_writing_fine_east_asian_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8e6c8983d8cda69a7d493d0b1d05d254881f2043d3f298896914ac3beefea3e"),
		new("item", "medieval_writing_fine_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "89172e93200413dc3b453b98f0ab548707a013fbd84c0db4c3f83f0f60607e78"),
		new("item", "medieval_writing_gilding_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c3c8247f8ca26e553b120e52840c43b7b82fa188054e73d1d84aa80972257f06"),
		new("item", "medieval_writing_gold_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea350e40bfd87219c6abc19a9e5dd4a758c940594cd23e321c22263bc192378"),
		new("item", "medieval_writing_inked_palm_leaf_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "97ce3b896d0755248ee3c223fc693cb4a6b40b2a311795664289557ad0b2a8fe"),
		new("item", "medieval_writing_ivory_handled_seal_stamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "059720d13511609fc1a7da216bb18f6865c5208f7c84da546de56f3634ec4ab5"),
		new("item", "medieval_writing_ivory_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0e72bf409bda8a6c3de2e273229d29e2ee38e380f14e61ec2442906b04cce20"),
		new("item", "medieval_writing_lay_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ee93df621bceede0dcd7c22cada19474482c808ac8c665326f948c5624e51ad4"),
		new("item", "medieval_writing_linen_document_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "52b275195c9774b311fad6f69fc5ddecbec73bd6bdf17e374cd92ad39c4fd7e2"),
		new("item", "medieval_writing_paper_devotional_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a74b06c27962c2b9bf7b9a105b5c3c13f850460b96b6ce71c3de284707d6b76"),
		new("item", "medieval_writing_parchment_lacing_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dbad299195f4caf8cef5b36c5e4b78b330eb5a8de8c85e7e920ce4c906b7bb8"),
		new("item", "medieval_writing_parchment_pumice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f73dc6cfca3e3f5809f5af672748322bc31dc7fdb86fac443983335f27c2d67"),
		new("item", "medieval_writing_parchment_scraping_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ed0b50c26e56b066ab30a778967673b9cb79e2d13ecf7dbe86c80c0c798fd8a"),
		new("item", "medieval_writing_print_drying_line", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ba14b4da50180729e970d285dd3a9ecc35c51ef3e9eabc8ed66d4b77c1529ac"),
		new("item", "medieval_writing_registration_pins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ca2635b4496c8db7aed9702c8aa743c1725f96e912cd90220df6f4ed7f921cc"),
		new("item", "medieval_writing_scholarly_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5158717663a421746cdc7cbfb47a6b7a00e249e805cceadfffe6dc9c3d58cca"),
		new("item", "medieval_writing_wooden_writing_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7f1a238afe027efcae09ce1f237a04ef6ec331d59a1fa78bb7af3009a8a613bf"),
		new("item", "preindustrial_door_door_latch_81", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8549c1ec70e2c4894e0d55ec09f03324c34250435b5abb0e82440cbf967e4a51"),
		new("item", "preindustrial_door_escutcheon_90", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e265e59b883d6aee1bf91ad0ef6916b5af495c16994f2719c67313a160a01af5"),
		new("item", "preindustrial_door_hinge_pair_88", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fc1204acb9c05ad9bfac9258092c49b74cf7c0e9845cd71f6e5ae85163a02138"),
		new("item", "preindustrial_door_hook_latch_86", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0cdf84c2f4100503af4f23d3ddccdbf05db8dd64fb609b0e7ed34079bfbfd8a6"),
		new("item", "preindustrial_door_keyring_74", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad935efc770654e10fe71f689f30a8d80c1e6be30c953d57d378d3119204cfd4"),
		new("item", "preindustrial_door_lockable_shop_door", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9edd5344bf3c61edb2613224f04688324af3fee6d5167240bba7676f56008ddb"),
		new("item", "preindustrial_door_panel_door_43", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6e744fa859d527f2872bfea54453bf20a8ff7f308cfc30bac49ed23763d4c4b"),
		new("item", "preindustrial_door_secure_door_46", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d23be1779de867180f127b8bd5d4e079357622b3b1ce0e2bf996a8e52185f3e5"),
		new("item", "preindustrial_door_wicket_14", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39b05c66f0649d645a3ec54c98e858d35504c0fdc8c0cd1d5a391adf1fbcd5fe"),
		new("item", "preindustrial_firearms_bullet_mould", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68314f62e4aef544668caf4c02fc1f9298e2f9528ce54cafb377affaa549d0f8"),
		new("item", "preindustrial_food_asparagus_and_leek_pot", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "289cf2941d9f1720513a11654cf7863a92eb390dbb53b774e8f0a0ebc3084e67"),
		new("item", "preindustrial_food_beetroot_and_mushroom_stew", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b89834838054bde93200a864a1a1d7b53bb6f6bce040daae090ac65dfa16986"),
		new("item", "preindustrial_food_boiled_barley", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "72c422a04fa1fe1db453d2d3906cc9d3d6be32df98d84c2477e93dba542a9369"),
		new("item", "preindustrial_food_boiled_fish_dumpling", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b31efb4fc090dac0c675007d00b2bcc123d4434909fded6daa4bf09cbc538083"),
		new("item", "preindustrial_food_boiled_lungs", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "deca0d55a90cf55113d7325b3e2672e62dcb64fe25c592dcca3521ccacd14a5b"),
	];
}
