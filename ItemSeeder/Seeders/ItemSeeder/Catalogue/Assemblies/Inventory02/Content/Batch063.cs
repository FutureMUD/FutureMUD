// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch063() =>
	[
		new("item", "renaissance_medical_dental_pelican", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e138045b3c07ccbfd6df3f3dce5c68d1afa81cdb0867156d68f8444f893a1a6d"),
		new("item", "renaissance_medical_drag_harness", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "231d1f820997e9eabad4a83c9fdab34ac84f30cd6098e6fdaadeb2418ff0f70f"),
		new("item", "renaissance_medical_horse_sling", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2d0eef0299f5b0ff482b63c50a8f0398128dcf76847361c596a7473965edd2e7"),
		new("item", "renaissance_medical_sassafras_bark_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea9ac3c4b752d9b5dc08a9b5f13f17c489bbf6f72f0ddac32abadc6894b490d4"),
		new("item", "renaissance_medical_senna_leaf_stock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "65a0df61d4a4bde476d14abd816bf540dd9debd73845b5e6651be951d2424b32"),
		new("item", "renaissance_medical_truss_belt", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25c9d91ac1b6ef137a3f43ca0aeb18d4cbb9ef209f328c7b5be1461739485dbf"),
		new("item", "renaissance_medical_washable_sickbed_blanket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0238eef1223baa373c0e2d7e3f9e01e009cfe42a3a176cbe11a1abb34dc8d25a"),
		new("item", "renaissance_medical_waxed_medicine_vial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7ec4c2c3bfbce38b89e0effc320ea191e49041a8d8affaf8cf682cd10409bc1"),
		new("item", "renaissance_mesoamerican_woven_fibresandals", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a66fe80829cd116c0b500d9aedab869c0b00c060726338f7fce732a4ec8f5b5"),
		new("item", "renaissance_military_artillery_grapeshot_light", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "003d8a91305788a1dc940d55cb7a55665c52aaa8bd8925efd3be43f31ab52a47"),
		new("item", "renaissance_military_barding_chanfron", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4017e8217e0431db96b80a9dd2bf6cb795ac61cdc47e8612c6829021a494b01"),
		new("item", "renaissance_military_buff_leather_jerkin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a8a0eacc7f6b6c46afab6d17d330bc40c910fc9f4abc0ca1c039b2db4288b687"),
		new("item", "renaissance_military_char_aina_mughal", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fd2ccd4bc1eca22aedb91f3e14bcd4feae9ee89bfe33e3d80c4c40429d94e228"),
		new("item", "renaissance_military_char_aina_safavid", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9de3de030e32c981d23b831ba7f7bc36f7f843dfb30316e71dfd74750182a5e7"),
		new("item", "renaissance_military_composite_war_bow", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "28b68b6b0341932c5accd985d03a96172c61e4244da4cec7bae227cea4af78a2"),
		new("item", "renaissance_military_cuirass_burmese_rattan", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5beff530346b65a1981ffa024b79616a539a2e1e12cd0144130ff8e64dc14a33"),
		new("item", "renaissance_military_cuirass_fluted_parade", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8ad929e3a3ef7e19ed9837da55fc3d2ffdbdb4ff9dd325e4b782ef2023d777a"),
		new("item", "renaissance_military_cuirass_uzbek_lamellar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cfd2aeaac4c6830096d617c3eb7578932ce6725a73dca54ec5704dcd7e90f0a"),
		new("item", "renaissance_military_hide_oval_shield", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eeb249fe863827fe1c25ddbf71e292d2cfc837f63389e5c9f07819c547683206"),
		new("item", "renaissance_military_hussar_karwasze", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9432ad30fa5fdab1695e74324374463c79ee1f9795349a6301f43c2ab4d6c764"),
		new("item", "renaissance_military_khula_khud_mughal", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "80d5f747b2cbc8b338524779605fdd1ecfd4d0b13ad35e2546ef7d524f7e3d6d"),
		new("item", "renaissance_military_mail_shirt_mughal", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "29a0855d595b4a521ca0f233261d51fbd4b9b56efdf06ce6a3ed905ef3c7026f"),
		new("item", "renaissance_military_mail_shirt_safavid", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "39b0fd6c265fd9d5cb35c3a0e875269d61015a42e8f41892ecfc5b950060308f"),
		new("item", "renaissance_military_morion_combed", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d904d0d278505123ba1a0a7939a786e816cdc85f10ddd8cdac7c747e01c35d4c"),
		new("item", "renaissance_military_saker_bronze", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ab036093d8f96e2ce86806e79ab6388748df4ebc350d8406cd40e036fd3cae8"),
		new("item", "renaissance_military_shishak_muscovite", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "59a343d1c705161ca7d11d8b43bc5b807c839b919d564fd719da029e7f6b8f3e"),
		new("item", "renaissance_military_shishak_polish", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec56bac91417357e2f7faa991d0b8fe7f7414c52e767fd148b769fd15615fe3f"),
		new("item", "renaissance_military_sode_japanese", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07e19bd11c5955c9a58633b3b9cca9d6bd72f75fa7eba08647e5b2f7975cd428"),
		new("item", "renaissance_military_yew_war_bow", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88c8dbe30bcdf97ec73fae7bb7e328cc8c6cbecf367157a61f8fe952c9bc36d1"),
		new("item", "renaissance_ming_soft_scholarcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a01471de371b341d7d67ee895735f7dd3086c34774ea75e02dad98abf872c1bd"),
		new("item", "renaissance_ottoman_quilted_undercoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "25cdd83961b45e305996383465508a4ef1a2233a72e4ecba6513e177bf0be26c"),
		new("item", "renaissance_ottoman_short_fitted_vest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e99e5828702a7d131a2f1ff046baf4c24964b947bdc3c33e6687cca55362cf17"),
		new("item", "renaissance_persianate_long_inner_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3f85b14f1a12fdab14035e06343b7feaa64854aa16a5e7460b067d7db87192f"),
		new("item", "renaissance_personal_andean_storehouse_coca_pouch", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68b1b53474eabb298024c30fe49101394aaf45d211afaba0cd05008566d607de"),
		new("item", "renaissance_personal_andean_storehouse_gourd_bottle_sling", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "67e2d63b79c1e6159c2c0b85ef7effb46d4d23284108d293bd589d54801693f9"),
		new("item", "renaissance_personal_atlantic_contact_birchbark_document_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "68f0f2bc260ff32d51c2984556da343ff441b7562aad17fa2afc68867a371cf2"),
		new("item", "renaissance_personal_sahel_redsea_indianocean_rolled_document_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e1d0c0c1824c772edde3eb1a83cf53154667b677b1a5d5187a4868fdcdfb71f2"),
		new("item", "renaissance_personal_urban_kitchen_recipe_wallet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9207c9af76370062752ce02ad9989f0b3ca25ec87778f992c292f176d8d5d58d"),
		new("item", "renaissance_personal_urban_kitchen_waterskin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e03e54258ccc8d7decbb4e2a67cb100d168ad04a6c0fef1b8b948880766e7c4d"),
		new("item", "renaissance_personal_west_african_court_bead_merchants_bag", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cc6211e9c94fbacd21a8df065f3cb340bac545bb2a8a6ac7f6f5a4c652e6794"),
	];
}
