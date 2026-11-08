// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch047() =>
	[
		new("item", "jewellery_shell_bead_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "66afc2819138d77c19665e9b7e2f9122dd589e7dbafdf4657833f9263084c735"),
		new("item", "jewellery_silver_bell_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9436f69050585e5e372518810151db018a11a279c6b7ca0490eeb8a3f47b1303"),
		new("item", "medieval_buddhist_travelling_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "255f8ffc5545dae7c0c52759902c625f1d523a40aeb117d40894eca333864a6c"),
		new("item", "medieval_decor_checked_wool_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3113098438104660841e1f76a14d8f237cc48b273179fe3e1ed979e8a6529177"),
		new("item", "medieval_decor_east_asian_porcelain_screen_tile", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dd24198f4df7cb8b5045b2b35c944552fc8b0ec7199f7c0d022c1aca7c5b866"),
		new("item", "medieval_decor_hemp_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "353071dfa2febbcb9acba14161906552aafc865fb308c84e8c2b80af8fce38a1"),
		new("item", "medieval_decor_linen_floor_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9a15ddd4b466afd367430aa6329092a398b12254cdcf818e6768a03f436f9b4"),
		new("item", "medieval_decor_room_divider_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ed9731ced5f250e82a637a5002864230f135af39f2a25cc35f6cfe3e2fc26c2"),
		new("item", "medieval_decor_south_asian_painted_wall_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6bba705b222a38f643cef2f6ea12648dcd5138c596d6300b3e87616cc9ad54c7"),
		new("item", "medieval_domestic_alms_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "fbf676a656dce075a6cd00d5a3a47073ceef2e747672f83c147c7666e5e64665"),
		new("item", "medieval_domestic_candle_end_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "378769a5ad0eb8718435e53c6e66bb18f63906d53cc634cf79a42c1b09391bef"),
		new("item", "medieval_domestic_cedar_bookcase", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ca48855b61f668eb952ccc23e2cd914ae0d3ba27a7abc1eb060f1792ebfd135a"),
		new("item", "medieval_domestic_cedar_cloak_wardrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "00fc5b772be76d6948e7877647c51af84e756006e8556ccd262db8fb3518b000"),
		new("item", "medieval_domestic_childrens_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "52cb91734a0e550c2cc9803ed3a4901e334df2e30bf4b197824f858f85784c30"),
		new("item", "medieval_domestic_clasped_tablet_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f00e987af57d85c787db2ba7578c6db6ea0315565eb0898d4b352ce1f016d39"),
		new("item", "medieval_domestic_cloth_scrap_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c395dc3bbc2fa95fad147b58d402e9ea3070f245cde5f70931cefc530e5f6537"),
		new("item", "medieval_domestic_cupboard_hutch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3ce171bfab1fa64b284231dda5cc768f510d2a08bdd3ae3f01f575d8d869c46"),
		new("item", "medieval_domestic_double_strap_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "deb8d2bc43c4432b9db179213013081b25d4cc41b9204c01a957c209e1026208"),
		new("item", "medieval_domestic_fleece_lined_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "918d90ae9db37c43606ed153347edfe2e9b358e59f63feda540651fd746cf04b"),
		new("item", "medieval_domestic_fruit_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "42d6ffff57f8703d53206357b454a057c0feb5248ee37e7804a86ad907a7c030"),
		new("item", "medieval_domestic_horn_powder_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d06237cd65d37d8c8119b791e52cd2aa52182dd2a8729cfbb51b7c0b06eea20d"),
		new("item", "medieval_domestic_key_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f68a5f1569feef5ecf40e20e716f6310bf1915e6ed43a54b5b0c8da4e3c6e3e2"),
		new("item", "medieval_domestic_kitchen_errand_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ad47d36918f8b2206a31f01a14d77228d6caa6ae86cbe656dc6a53fe6ddea8af"),
		new("item", "medieval_domestic_linen_press_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b9643a66cb5696cb551e707d42c4f2a6db04768a09bcfdaadd606f11e8cd4af"),
		new("item", "medieval_domestic_linen_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "961eda334f4d1bed61b91bd739a902bc667e76e1fe5c2f6ed24428cbefce0d2b"),
		new("item", "medieval_domestic_low_book_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5505cb905826ba64c0cac3abdb0a1b7729a9a27b89d00c3af49d71f20dcafa7"),
		new("item", "medieval_domestic_narrow_spice_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "e97b6580ff58eb357c417539127e3261211b924da8a1280e3ce8715d722191dc"),
		new("item", "medieval_domestic_needlework_stand_drawer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a810342d4495b8f6fefa79e91f45adb116005a50068f2ac4ed493d3e42d1346"),
		new("item", "medieval_domestic_pantry_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "b385439f1461b686562addbf55b4689d5e99d8c612ecc5b2816bc82f7e4e70da"),
		new("item", "medieval_domestic_parchment_roll_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "607d155abba7f3b7185e1074189d2cbde32e0b84a46bdd2b90eed49b1a9198b1"),
		new("item", "medieval_domestic_pine_clothes_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c15cfb3e8d9940d6e3cb1625e03b92f35047a8fe013639a10d71f5df7fbe7ff3"),
		new("item", "medieval_domestic_plain_desk_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1135d77412c23046fc095eea47993295f177b608a7d192f28a891710238af915"),
		new("item", "medieval_domestic_plain_pine_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b6ba87acf2902794223073c47d526390cfb010bb2d485cc81b64d7a8f76c7dc"),
		new("item", "medieval_domestic_plain_pine_sideboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d10714eab2d867a96211dc3600f98dbb905daa27ea6eafab1dcc642ec3469ab3"),
		new("item", "medieval_domestic_spice_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "789ba76664b6ff56e023382ea31d730e95f82af5b6cacab96a351102d16bf001"),
		new("item", "medieval_domestic_spindle_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "efbdb707cdf8d66d08fc260320b6698a71a8ab8897054b443e85c5c0e697f5c7"),
		new("item", "medieval_domestic_tablet_banded_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec0328cac8f638962adedf827c103657a620fc417e0aa443b0b7f3bdefef4f38"),
		new("item", "medieval_domestic_wooden_spool_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "d33b728f2935d02057cfd7d799cccf04b71929c75fb275d020acb46a84c52241"),
		new("item", "medieval_door_east_asian_iron_grille", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a462b344a55ba9f50d863d3413454c062faa78729a9bead476e9a2044bc7238"),
		new("item", "medieval_door_east_asian_lockable_storehouse_door", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa1cad736df0f4f85ccf51395c534085ac7ddf4b6b79ba3a472abba08b91ff17"),
	];
}
