// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory12Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch038() =>
	[
		new("item", "earlymodern_religious_eastern_prayer_rope", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e6722b2441084ab44ee59fd42a750fb6e98b61158455cac1879870743990bf8c"),
		new("item", "earlymodern_religious_jewish_arm_phylactery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9948bd81721d7a411ef99653b36ad1fea88f0730e650e0523a5116a9f2b6584f"),
		new("item", "earlymodern_religious_local_beadwork_collar", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e50a19d82474e71b9941c0df17ee00344b8dc6a2e1acab54b4c87582182effac"),
		new("item", "earlymodern_religious_theravada_underrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "93e07da0d62d42429491f78484b69848183763298721501c9c2a3f2e313d69bf"),
		new("item", "earlymodern_religious_western_mozzetta", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0f7aeb05dc18e4827270669e1613ef0965488ce870abe0bb9f1eea28da1b9ab1"),
		new("item", "earlymodern_ryukyu_naval_officer_overcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "42509ba2dca259879355cb268d384346a906a54d791d553454a6a6e0a7eda59e"),
		new("item", "earlymodern_ryukyu_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18a185de375e0e22e770797559f6542b20be0fcb93cd1f3ca0e4cd0e5162fe7a"),
		new("item", "earlymodern_ryukyu_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "377bc1e4ad6741bb3f30b767cdfec0a54979e2dfbe107a5b32ed04a9eb736012"),
		new("item", "earlymodern_sahel_military_leather_riding_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3f083613bbe5ece22e3b4ac0f23559c4f47e26c0506515a23a8a47d51eab2a85"),
		new("item", "earlymodern_sahel_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7cfee59fa468b074a9f916a5aa9b586f59d108b8f90e484a8e9898c263617f6e"),
		new("item", "earlymodern_sahel_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec5f5117df9f0a57928656eaf037c20a548ebba7a55f7d7727fb8cf78b306944"),
		new("item", "earlymodern_science_instrument_stand", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2cbce5a667c6788b7036417727ad8482d5692030dead35165af600e3948007ba"),
		new("item", "earlymodern_science_protractor", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "59f4dc654b638ef796147a756aa62a49c753051797a608ecf72f05c336ab14df"),
		new("item", "earlymodern_southindian_naval_officer_command_robe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ceaedd67cd194669d0b819d89bdfbe47da2902c012b57a48205349d999391009"),
		new("item", "earlymodern_southindian_noble_jewellery", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6b70c5407327cf3da48420c901e2c61f2b453e3f0b7825b587da89d187f80a60"),
		new("item", "earlymodern_southindian_noble_regalia", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "71ece55993e5748c1fb3060c41729d06275947050c5380c8bafdd5470aa53ebb"),
		new("item", "earlymodern_steppe_military_frontier_musket_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ef3ef5d39ec3e559b593c1ebc121765030b75e08e5febdb51265c98b5ece870c"),
		new("item", "earlymodern_westafrican_noble_female_court_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "58802bcf67c92732a9cf871a2399f63f39cadefac449861c331b099dccebe12e"),
		new("item", "earlymodern_westafrican_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9510977e9e54d4bd9774661e398566de9fdf3fe6992948e2c2fe7b01e2cb4853"),
		new("item", "earlymodern_western_clothing_fine_broadcloth_waistcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b672ee83119a54ce291d8b7ebd391fe818590ee7aa164aa1e2eb06e0fce7a246"),
		new("item", "earlymodern_writing_penknife", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e6f1ed21ad56fa1bf3dacdf70f117d5efeeb34909f52e05cf4aac8ac98094ad"),
		new("item", "historic_dye_vat", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7af45062232c67bbb5fcb328d886c2e33b616b157cbe391400171e2b976fe942"),
		new("item", "historic_workshop_hearth", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ac4b7cca6494c8f1c869e610f1910e7fe781343c52ad042cb790da784e68197"),
		new("item", "industrial_agriculture_forestry_fishing_0004_field_fork", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "13f82dfcd3297653e2d0829f3e0f5ed6e3455388ecf49424ad0b56cadcdd6816"),
		new("item", "industrial_cleaning_personal_care_0002_wall_mounted_brush", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "b023c2681702f72fb3d23a0e3729eeb27a4d9a6de3b55e0ef0d79e7101e006e7"),
		new("item", "industrial_cleaning_personal_care_0006_wall_mounted_cloth", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "88624ec576ca3e9cd87a92f3cffeb3e324d6cff48cd4980c2a3f776e97ec0932"),
		new("item", "industrial_clothing_footwear_uniforms_0017_reinforced_apron", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "cd7776a460c5b9576ecde46c108f71dc8555c48a511e8cdf3ee88edaaca7705f"),
		new("item", "industrial_clothing_footwear_uniforms_0027_reinforced_socks", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c151136ddcedb6c747a1ac86a106e447ec124ada48d25180e3ccb9db6896ca17"),
		new("item", "industrial_clothing_footwear_uniforms_0033_compact_shirt", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "78b9a930258ee7421902f8fd36c19142b2999f5754a55e56d37da78cd5d4d38c"),
		new("item", "industrial_clothing_footwear_uniforms_0036_compact_dress", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4b2551173bb37fdde6db7ca51221d87445f7c3f748614acec95726e955b59884"),
		new("item", "industrial_clothing_footwear_uniforms_0039_compact_gloves", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a1a552ec7f328e38d0604999002bcacb31f560f7322939e96a67cecf85002a4"),
		new("item", "industrial_clothing_footwear_uniforms_0059_heavy_duty_coat", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "e64d73a2cbaa1b29f535ed49d1fb7a22f57bc57e5b362a74c3d5c381890b701f"),
		new("item", "industrial_electrical_power_lighting_0001_heavy_duty_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "43d0859bcdf64b3d629daeb2b04ba203b50fadc49dec318c4065601572a2e108"),
		new("item", "industrial_electrical_power_lighting_0009_heavy_duty_meter", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7f4267afc5bfe2017315471de43df5cd982fe8661db3bb42caef2b684c33ad3"),
		new("item", "industrial_electrical_power_lighting_0013_folding_socket", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7fd3fe5e79e49959ccbdc6a6457cf696ab5fcf6489370bfa3da0aba831f79988"),
		new("item", "industrial_food_drink_packaged_0040_wall_mounted_cake", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "31a71bb7a8d6d3ba561c12778360c2cf3492efd7123bf47151d11f4d111375e6"),
		new("item", "industrial_food_drink_packaged_0048_travel_biscuit", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "58dd1b7a693511797a08b6958c90ff1309fbcb98deb889d4140a2197136c1f7f"),
		new("item", "industrial_household_furniture_storage_0003_lined_screen", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "da27ffade8a07cfef9992d7b55acd5d731150359207b9d3984fbeba09f587aa7"),
		new("item", "industrial_household_furniture_storage_0023_stackable_cabinet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "522198ba48305b0dc8a74a251df1da1e9ed5c09f5ddafdaf91db3d15bb4bb4e9"),
		new("item", "industrial_kitchen_appliances_0002_institutional_pan", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "9623109f8391bbd6c822744dd08bb78e9db56920624e6b7242b2a6776217cabb"),
	];
}
