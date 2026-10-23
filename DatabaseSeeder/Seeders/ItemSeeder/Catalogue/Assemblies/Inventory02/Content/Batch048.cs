// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch048() =>
	[
		new("item", "industrialised_weapons_police_military_0134_ventilated_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f94665a0b4b71f0c2164d1dcd41c31de4f7515611217004f4d1b5963f2deb46"),
		new("item", "industrialised_weapons_police_military_0150_stackable_signal", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_weapons_police_military_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "261b0ecc0c1379db50e9792cdc23174cd1bade1d439f394cdd18dc22e06f9cd2"),
		new("item", "industrialised_weapons_police_military_0167_countertop_shield", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d1ac138a0a62cb10885eb83d15745adcc9b3d801f5fd5d50507c6faf816874ab"),
		new("item", "industrialised_weapons_police_military_0183_travel_baton", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "e77d3b82b4d500099272bc9afbe4e612275dbf7202eade20b0491678b2802cb1"),
		new("item", "industrialised_weapons_police_military_0196_utility_magazine", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "f599a825657c2b376d3a499bba318258711d77bff4533d3e6d33192364acab83"),
		new("item", "industrialised_weapons_police_military_0209_plain_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5f56a50e9d44d40fd06bb87b1c18644caa582f1cd624f261dce39e367810695"),
		new("item", "industrialised_weapons_police_military_0214_reinforced_scabbard", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f2cd3cd21b112866f2f6e84ba398f76d6433b3028655275f0c66064babac3eb"),
		new("item", "industrialised_weapons_police_military_0219_reinforced_pouch", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "03dec2e7504fbd2cc90ed905cac817fe3a7f799d46805e62e36c77b6a08e9b3e"),
		new("item", "jewellery_bone_tooth_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b5204eb2747eb1e5e75db60a170fa25a55f04667b1bed53f6152367b277770b"),
		new("item", "jewellery_bronze_animal_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8444d13f15aab8917d7c0b53745a473428e5c9deed7433a30bec5c8e7eebbffd"),
		new("item", "jewellery_bronze_hollow_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2531dece860a4c167ba11a6115713cedd70a40fd821f1142df2c783f89fe80b"),
		new("item", "jewellery_bronze_rounded_armlet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6524baae2cdfe162d134ea30cee00dff5533ce77b7be4f5efc11707df01d3ade"),
		new("item", "jewellery_gold_amber_earrings", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2381632b3ebcc665d397065133a12b3f3fb6d93ab43032d89f989868da905126"),
		new("item", "jewellery_gold_animal_plaque_headband", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "dada753a983391dc8ac7a9ce7870fd3133578a80c6f58d258cf84c8d2a4ae7cb"),
		new("item", "jewellery_gold_emerald_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0ebaed2a914d29b4170447ee5de63e4681effa455632175038a842fe3f4ee4b2"),
		new("item", "jewellery_gold_falcon_bracelets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "27c7194b87a6beb1c1e23e6abdbd10fa958a7bf4f1240491c956cd32f268b684"),
		new("item", "jewellery_gold_pearl_anklets", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5441250e018bd29ec6497d0035ffdc73197ead30eb5e27bacc12209dc585520b"),
		new("item", "jewellery_gold_ribbon_bracelet", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5da74954c021571ac96f0725eea140023a3c7b4c65221129f157f4064fafe742"),
		new("item", "jewellery_gold_scarab_ring", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "18b877e1d6c9d7d2111eb18827680a99b2f35848c50edbaebfbda457c452f29d"),
		new("item", "jewellery_wooden_bead_necklace", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f926e08f59c744112b64686d7549b546b78e915a3b64ec8011c8b7f09d918d29"),
		new("item", "medieval_brooched_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0e7104e35c3c15cd14e04961f89cb8450053d2c4c17090ca897b0173b3c6cae7"),
		new("item", "medieval_buddhist_formal_kesa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "6eb96ecb89c58ae68eb6342012b90277bc410cc2150ce817d1298d09b4b35eca"),
		new("item", "medieval_church_incense_boat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "feee29cf4918304e48077e903ac6dbc9d8b364b2299fbdc4617f9badc92f00ba"),
		new("item", "medieval_clean_town_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "552ddb2e57e8dfdd669a1f6de8401b77db1d472b3e7018e48e834e053bc52709"),
		new("item", "medieval_cotton_work_waistcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c12e449e703afdd54c3fbe3e4eff245e434637d7f128b7b0571a5ddc01f0f32d"),
		new("item", "medieval_daoist_tablet_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0875ffb8c16e10136700c339d2f9070c2d357ad03d9ad7fbf431f265b01ababe"),
		new("item", "medieval_decor_bordered_valance", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2784ea772bdcea5bcbf06178117e68960a6e99dcb713968f0b473cff1b82578e"),
		new("item", "medieval_decor_brass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "21bc83d52fb898b473afe01c7575ad46566ac084b7dbf5835f9ff410a7d2a019"),
		new("item", "medieval_decor_ivory_inlay_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "36b4fbf2cc657f2876e90040246a9045c9ccd4ce5f5476f44a5b7d36473594c1"),
		new("item", "medieval_decor_polished_bronze_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea3cc0112236984b8a37f9c5cff2d97d0479bd193973909eb146a57aa509192d"),
		new("item", "medieval_domestic_basket_backpack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "956ceb732324146e4c71ac814766cb79831c90d8e943bbabef68caaa1aa18737"),
		new("item", "medieval_domestic_bedroom_drawer_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c40548c3b2f0bc8413acac1008403efff0cb268231baf939bc2eff714c4e0d65"),
		new("item", "medieval_domestic_dish_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "8fc0763656c5a609f6d6c47cb2d8cf6690478452ba49a5f9473105ea22afe75c"),
		new("item", "medieval_domestic_drawstring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b1100bee136a8235fa9af5d98ec31a09f92ae1d0934076fe6cbc9dc72d80fd"),
		new("item", "medieval_domestic_dried_flower_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1a62dafc4ec7eb14eeeae88bcba829d75270633ba515c03463937a436e9827c"),
		new("item", "medieval_domestic_felt_saddle_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "7416f0f4971be34b21fc9afa348dcf6072c61b24f2ec34985d937314a9b4e37b"),
		new("item", "medieval_domestic_game_piece_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7801e900af309109cfb577cdbb5e6a167f00a54d72266978d232373c9194f1d"),
		new("item", "medieval_domestic_hall_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cc47969eba4287d7eee1be9deaf60f8051a1bf32f7d6e2441b46c8904afa880"),
		new("item", "medieval_domestic_hearth_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "1196fe0ee00a10f99080110152d271ad0047697f130cdbca970fa78c0bdfb1b5"),
		new("item", "medieval_domestic_hearth_tool_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b54bb532c34fd33c4c45bf08ceadb9ee5d4a972d93e932e16837bd44e740cdb"),
	];
}
