// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch050() =>
	[
		new("item", "medieval_goryeo_korea_silk_lined_jewel_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4a9a9a37fda7a22a5c4c28fb6ae323618bc3a374a975bbb72c8910f2d4dc2c92"),
		new("item", "medieval_hindu_carved_shrine_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "be932692d8149e27ad4c8dc25bdc684ba74a678bf07a91e9296bb68ccad902e3"),
		new("item", "medieval_hindu_temple_supply_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3396062b6649947b033dfc0b047d541c47709cf6dc87644d2b14852a5e7ee92c"),
		new("item", "medieval_household_work_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "817b9e5fca8b6fe504547592c85a8abf83cd36791b1d7267bfe534bda3b9ba79"),
		new("item", "medieval_industry_stock_blade_blank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fea41b1b16a15a6208fa87bb8949f1e62da5fd2372b1aa802e851d81c767a7aa"),
		new("item", "medieval_industry_stock_jewellery_setting_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ca50be0de05faf0a953200d0a2e3793bfc1157aa727e26134b6d3163be952af"),
		new("item", "medieval_industry_stock_plain_cloth_bolt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "54bee8c19a4e9a8a745e209286c3e1abe44ab69babab7fe94630723d375152b0"),
		new("item", "medieval_industry_stock_rag_paper_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "56f8c77fba6d9c4781f7e6fe908a49f100d68b8742a903f5752114e832c23d82"),
		new("item", "medieval_industry_stock_suture_thread", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5819714da97f8f02ce8699ad69d6740730ba89179e4a5b9bcd14301bc0a0af6e"),
		new("item", "medieval_islamic_madrasa_text_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "09a70a7ab6ce75b66b91d69fb8bae46f30d31b8be276d07d3315afdd485e846e"),
		new("item", "medieval_islamic_scholars_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e05c578077745b3aaa25ddfa290dd38977c197a062ae8302ece6052d570f27c"),
		new("item", "medieval_japanese_suikan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "368b2c5212f58092aad2423632d0e253f39ef6c6637a753b2cd51073bbdc74d9"),
		new("item", "medieval_jewellery_elite_court_girdle_jewel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb4c020b8533847b3a69996c5a08e834617580265153692017e80258903969b6"),
		new("item", "medieval_jewellery_garland_fresh_ivy_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_ivy_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, "6e067beb24981ba441aab27f98b8ee4425621e432ec0a036880e56852e3459ab"),
		new("item", "medieval_jewellery_garland_wilted_blossom_anklet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_blossom_anklet"], ItemSeederOwnershipPolicy.RequiredRelationship, "70ab4aa41dbf40110eacbc73ef423c8604c3ffefc0f3e6fe3b0d4aa1951a8a5c"),
		new("item", "medieval_jewellery_garland_wilted_chamomile_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_chamomile_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, "36edbd4a9d753e78b4c4ab61bb33005eb8ff39fc451cbca9af7b8e23c2689f03"),
		new("item", "medieval_jewellery_head_fresh_flower_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0599c4074a1e99458a53a98e324d26a65f2fd73537c74073aa35f3332026c78e"),
		new("item", "medieval_jewellery_head_jade_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ce000c139d18dc6486578b2afc57c2f99092dc44e6692e3584dd884f6de274e8"),
		new("item", "medieval_jewellery_limb_bone_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f99e7d6e67ad2db772de11a0dad7b9af02df2bebcce0f62d0c667327d3d47048"),
		new("item", "medieval_jewellery_limb_plain_silver_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fc9366c3cbcea6a21d14f3e183365a5dd8d9612443a7fc720b8fe1c333a362c"),
		new("item", "medieval_jewellery_limb_silver_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e5665852ce8ab1c79c1dfda7bfc33c5d6898d4f1bdfdf4437df249bf818825d1"),
		new("item", "medieval_jewellery_neck_blue_glass_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "444cd6149c088e8857f3236b83080a4ed97df604023b4e7a8fe7dcb414e9877a"),
		new("item", "medieval_jewellery_neck_gold_link_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "804151a173caab7163b75b9c58847faa983e6e517fa6b6980e8d356e44059d71"),
		new("item", "medieval_jewellery_neck_rock_crystal_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7596f11245e9fa5f530ef10f809f2e1db6ce4eedeb7a2fddeedcc259ce56b3a1"),
		new("item", "medieval_jewellery_neck_twisted_silver_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6208666b808190076551b0ebfb246761aa76de8f21ec98aeeb2ede19f7cbe1b"),
		new("item", "medieval_jewellery_pin_copper_fastening_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ecf6661a4fd0205be1cb896b4a5f89275a13ca677a10aa8c66c012344676a9fc"),
		new("item", "medieval_jewellery_pin_motif_silver_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aea9d388c1e271d650d1bee45762619751e3476ae95d40513c9f3964410c53a8"),
		new("item", "medieval_jewellery_ring_amber_set_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3d16ccdf80fd24354fdb6b15fd186c0d9771f412dbdd6559d12ed85eab795a7b"),
		new("item", "medieval_jewellery_ring_brass_trade_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2aa60851576e9935373e73cde1bfb18f6e0a3a86e3981d8dbf9831757c42f9ca"),
		new("item", "medieval_jewellery_ring_heavy_gold_thumb_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d070ea9db9f06e9b60614fc560e54115051f2818f605749553d2ea08d59540a8"),
		new("item", "medieval_jewellery_ring_moonstone_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "149195b0108abc48cd200178d89c97770325b116a635b4d4b7e9434a40680329"),
		new("item", "medieval_jewellery_waist_motif_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "51425de9e9623d9db9795d22359d2465bdb69b9fab9ded432b2e009f7bde504d"),
		new("item", "medieval_jewellery_waist_silver_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1182d93a44741750fb57a54c0ace05e5d46000845ed771e5bce31b4d00e0068c"),
		new("item", "medieval_jewish_memorial_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "63803aadc2156fa1d73a9275cd87891703217777c38ee1f31fa3b9e4cad079f5"),
		new("item", "medieval_latin_black_over_scapular", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2802320351e9dc44c04e74fbc508c3bc49a7b0ae9236d4babbf00e9cc4bf8c92"),
		new("item", "medieval_latin_brown_friar_habit", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b5b13f18df3657d7a68c3a430b2966656a3d0687d1021a6b20b7cb2de81874d4"),
		new("item", "medieval_latin_linen_cincture", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "5448272233a2073be26d8ee2315e109602f7b058a114a5b712f221d88075f8c1"),
		new("item", "medieval_latin_maniple", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7043911c0879fd1b69c56ec942412bfb4ca5d591d60480e7f3e56b33faebb042"),
		new("item", "medieval_light_brass_wall_sconce", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c09224c6e0198c069f478bc7ca30478d3d125cdb748b8ec2ffda92d128847be"),
		new("item", "medieval_light_cedar_resin_torch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d51b6251d09d9832d480126ca1bad4812a857908c3c822f01a105cd741b6157c"),
	];
}
