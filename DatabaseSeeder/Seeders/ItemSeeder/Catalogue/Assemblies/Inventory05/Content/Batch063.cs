// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory05Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch063() =>
	[
		new("item", "renaissance_trade_elite_household_portrait_casket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "491b6605cf4352421b738b5db6c1e2f4127806c297c2a52a37c1da9791f1cff1"),
		new("item", "renaissance_trade_mesoamerican_market_shell_ornament_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e581eaa5003ff6bbba42d366537dd3e3425ff31f93b236576791f7df532b8826"),
		new("item", "renaissance_trade_sahel_redsea_indianocean_ivory_parcel_lockbox", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c67cee65d258f9caaea8e70d50ba9d9ee15d2fad12cfd1f4c399776895ee2750"),
		new("item", "renaissance_trade_shipboard_purser_pursers_till_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f15e556fbc552b984899ce9564b1aa88164ad9768ec40da5dafb7f134a806e63"),
		new("item", "renaissance_trade_south_asian_merchant_cash_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac3ad97f4bf7ce542fe92f8584a71c5f06dca83424069b79cce7f06953974119"),
		new("item", "renaissance_trade_steppe_caravan_coin_weight_casket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "33acb03fe92fac60073832aaecf79680bc3ff6042fbdb79ba2bc5f496ecb5979"),
		new("item", "renaissance_trade_tavern_inn_cask_sample_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "de17dff08bb5a07270b4a77985c411d29697f805a9a3c74fcdbb84f5a471da00"),
		new("item", "renaissance_western_full_circle_cloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7a55bfc2047d132bff151658914ebcd1698892405ba3a03e9c6b984bc3d0f528"),
		new("item", "renaissance_western_open_neck_work_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "76b99b114113750e65da2c46b7248910f87c6eac0bc5b2f08180b0f7cf9ec084"),
		new("item", "renaissance_western_paned_trunk_hose", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b27f237005604e7667f4eb149564d3b832eaeb8d0b7dcf09150c384b16bedb58"),
		new("item", "renaissance_western_sleeveless_doublet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bd03aede917755a43081774a4767b0e1daada8be70c5d78afe21a17725eadcbe"),
		new("item", "renaissance_western_sugarloaf_felthat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0ed014a6c18a8cd3224600cb4b02d0b467101b6dc3d23cd01d09795cab07683"),
		new("item", "renaissance_western_work_smock", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef22596ef9743249a9b3ecbdf8b049a86eb73f17b022be7b23ea44261a5a9c84"),
		new("item", "renaissance_writing_seal_matrix", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "62305840591ca9d8da9c77dfef59d74a846dff9c5d72ab8c177c7d43bc7d16cc"),
		new("knowledge", "Ancient Weaponcrafting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "63cc017dbef760d78e7054be81437b9d9ba29065296066167e2f439986f82703"),
		new("knowledge", "Antiquity Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "a033e0b46fef80fcba7e4e5ac28e42db6bab6e3a289d46e885e12158c52a5e1d"),
		new("knowledge", "Celtic Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b31a54aaca3b93905ffdc056543a166c5997f6bf2145c6b312b73faeda26877a"),
		new("knowledge", "Hellenic Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3729ad785d3781e49d11f0e691450a8d27ab5f00b24d17b09c51c4948a39bd61"),
		new("knowledge", "Medieval Repair Crafting", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6cb7f1b300143098e9652bc293ff7b01e5946ec6fd5d453c19ab7bd4e79de81"),
		new("liquid", "Ming fermented-black-bean cooking liquor", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52ba38afe77efd267f78180dd3d0074645950559c817ac952b9608bd07239a4d"),
		new("liquid", "achiote-red cacao drink", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a33bd2900ab35bb93feb021813198b12d2c9d4cbce5f4fb08f34f73e9bb8941"),
		new("liquid", "ale-onion pottage liquor", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "53a04749a2cf92c06dbfadda10683b34a1175bc7a368611df29b4cfea68750b7"),
		new("liquid", "apple reduction syrup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1046f1701002ae17a182229255f2b52b835203b6323aede7a7b2219703a1ddb8"),
		new("liquid", "apple vinegar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9ea3fc4dc0966dba8d1ef0419868b4bd7908802fd46a78c15783171e4606648"),
		new("liquid", "barley water drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9faa0004f2eb29eb18525b051ea328bf64ab8f272d46302fe1a0a1b300d47d4d"),
		new("liquid", "berry reduction syrup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb70f030d7c786a9c18db81f0ae8b1f2833b4967dd8f6216f70c801a5f365ab2"),
		new("liquid", "berry vinegar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5fecd084754772712f67f5131543f08a973d318f8614d0527ca680564aba41a5"),
		new("liquid", "brown meat sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e687c47758523544bb8b95b42cbf09982c8ae4896bcae7b99b296d94b887b60a"),
		new("liquid", "cream-bound mushroom sauce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "595922fbcdc367b2cd78d6167cdfedc2dc016d6acefb97d275e0f0578751367a"),
		new("liquid", "cultured cow dairy drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "55b669d27cbf204bdf69443dc91bb2bc26777ec042c72129f0762e119774fd1f"),
		new("liquid", "fermented apple drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f8904665221970f7636b5586645b870eab5eb67e6e7bf078be8725ac7cbb88da"),
		new("liquid", "flower infusion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7459ebc2906b7af1782409e8c2a10b1323e998fae84121552a0487be4996e84"),
		new("liquid", "game cooking stock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "21605b000e7090c6abe33aa86a53ab6a8f1620470661121f7bca44e4dafb37e6"),
		new("liquid", "grapeseed cooking oil", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b6fc99fd53a7a73b4b8ec4209e19c223c376ed86173ca44557504646fd91836"),
		new("liquid", "green vegetable stock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c40a73b2440d9df7da460d0033c48ea1c3155eaea9e25f51a2296559d84499cd"),
		new("liquid", "herbed yoghurt drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9435f85c0787841d3ffbb4920edf36572b7f73cbc57d87469a0df38b4ce745be"),
		new("liquid", "honey spirit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "338ed0d2fd8f413ba664492ea8adcc21cfffdc8b72d8361d4936ee04fb538476"),
		new("liquid", "mushroom cooking stock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "49eb21e94ffe2ab1af46db7b15de46280ec6615259612e9679b0d07127cf1a54"),
		new("liquid", "pork cooking stock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69f34dd492d89dab539c946c583b91fdc0864a3b3126540f3b554f2ea51df13f"),
		new("liquid", "pressed pineapple drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7169946f870850ed3446d334e6b48ab7aa8fb81dda59aaf4d9aba90b432b80f"),
	];
}
