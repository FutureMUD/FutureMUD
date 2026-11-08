// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory06Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_military_joseon_helmet_brimmed", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7df9b6cf07acf12b190d198b824db32734b7a6628f323d5e723486f4774cca1"),
		new("item", "renaissance_military_matchlock_caliver", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cb4841382220b17a8d4d28ec5a9ccd7c45d196028bd99740557de4dca665c658"),
		new("item", "renaissance_military_ming_brigandine_panelled", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c28d452680ca9a7e5ecf8f86c8d90c4402e6dc66f86b4b05d4dafa2d6edb37b"),
		new("item", "renaissance_military_ming_leg_guards", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "69c4086ab31bd607a9cd44d3b891dfd38a124a74e3bca1af1f6d07a6ab54c5e9"),
		new("item", "renaissance_military_paper_cartridge_055", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8ecc3a6a9c9c3f6286961eb0dd8500dce4617a848d743cca8059d8b24061bb02"),
		new("item", "renaissance_ming_gauze_officialhat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c5ea25909dc9f0e75f4ba3ce8311c45ded110791d1efd6c8b085cef0d19b055"),
		new("item", "renaissance_ming_short_sidefastened_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6573b7a9ae83dfebf9e6f46ec3e5ce55a98329dd7b7ed756dffde30f485d4d82"),
		new("item", "renaissance_ming_wide_drawstring_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d28319d971b8d0a40da39ad2b1de894651baee480c94597ebc4aedc8d80843e"),
		new("item", "renaissance_ottoman_loose_kaftan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b247ad20727e023d768ec6b2e11cf04bd0c3b6d431ed8ade4fd1723a3d6d9e2e"),
		new("item", "renaissance_persianate_furedge_domedcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d0f4e8400a936ab22e62d5def06f9bf3830a2a7eabbaf2a5b40793f8b2ae9aa"),
		new("item", "renaissance_persianate_sleeveless_longvest", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1107d6111cecb8ec8a1998ac36d459b8ba7b0f889b7dc4bee1b93cfa6bfeac6c"),
		new("item", "renaissance_personal_japanese_ryukyuan_tiered_medicine_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "eaf31cd5c184ccd200772865b916f2381e1004703c8a85a45da50c7fbc166a4b"),
		new("item", "renaissance_personal_ottoman_bazaar_urban_waterskin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88934ed821f89991679ace93bbef8b681ca23e30cd53238a2b7ffabb0126cb4d"),
		new("item", "renaissance_personal_southeast_asian_port_betel_pouch", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ffacf4b5955443bc1608ea7ce4d1fadf312f8cf45ea8478be51b43d42e9f535"),
		new("item", "renaissance_repair_fine_scientific_instrument_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d3c3ba1ba33de2635955fdb62d987683295cd804fb421f940e709597d453c06c"),
		new("item", "renaissance_ryukyu_formal_courtcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "637b93472f4a6ea774156a74cc2c930040f90ab75d03c2d79b581332a7ff4986"),
		new("item", "renaissance_seasia_embroidered_silk_courtshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "13a0817c6edbe80e9dcaea96d78afdf9eaf9f49a16c74000acd168785703f723"),
		new("item", "renaissance_shared_clothing_half_mask", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6fb434a6738e602f44339ec98d0a5b25274c43d0b5d22d67e06747add6d0f104"),
		new("item", "renaissance_shared_clothing_rectangular_mantle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3c0d006b0516fe1b4c160fd6631cca8c7f2a35bfb9b5c29462f966eefae5b7"),
		new("item", "renaissance_shared_clothing_travelling_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4d20b07fca12512f27efcab3d97237f3012b7f43426deddeba20844735b68b5d"),
		new("item", "renaissance_trade_atlantic_contact_shell_bead_trade_coffer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c1ad8b40d84b9483ca1cbed383ce3e77435e13bba5ad73ac87f0f4cbe5f4ba75"),
		new("item", "renaissance_trade_civic_guild_seal_register_coffer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6ab182a48330a50a51301d3c9732ef1bb658660536040ca5c51eaea2cf3cbe9d"),
		new("item", "renaissance_trade_east_asian_literati_seal_chop_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "03bfb43808468fa0a8b96d625e7245960eda0135f0072085b7d0bd751d7a235c"),
		new("item", "renaissance_trade_west_african_court_coastal_document_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2fed77e288382de60fbf4c860f29da4c32b7b74ae3339abc5fb061b7e3356b47"),
		new("item", "renaissance_western_furred_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d7fa2308b7f8336fee4baf44d027e8f814abec372b45e48aba9bf520691ed5e"),
		new("item", "renaissance_western_gathered_linen_shirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a5cd9b6209735acd37a701b4c911c4c0904c67991678173e559c4fb881d1ae8b"),
		new("item", "renaissance_western_padded_doublet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6e315c8688ff97032f12fcb35f533e56570aba4e0567e2ab70e478ae720478b"),
		new("item", "renaissance_western_partlet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d0c8fc1e8c8c66fffab438bbdb8eafa31ee1ad9305a51b1256aea6205199d4ef"),
		new("item", "renaissance_western_spanish_farthingale", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dffad8f4680d69c147be3b1a46f70c8c981d47e1dd17bba82e6103bbefad1b7e"),
		new("item", "renaissance_western_trunk_hose", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5dec031edc2f0a200cae0b7e059bc4bb98d25e67b30105291c27f6e638400e4c"),
		new("item", "renaissance_writing_port_customs_register", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bed1414ec7548bf9da2ff467dc97f6e37014763ced659d6530f145da30dd7c92"),
		new("knowledge", "Ancient Hide and Leatherworking", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3ce16dff9fc8405f7b9d1d1727cc9e5364549abb158ff2d3350269320dccd8cf"),
		new("knowledge", "Ancient Prosthetics and Mobility Aids", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f89abcc5a5774fd9f75e1f1bb7d84d5e71b5a29e3684bffb5339844612861b3e"),
		new("knowledge", "Egyptian Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf60e7cfb8976ecc7d5533eae443a1b9f003cae3422aed60c1b28395405b3ea3"),
		new("knowledge", "Gunpowder Manufacture - Corning Black Powder", "crafts", ["earlymodern", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f0476b6371ffab0953ded544fd266339dd86c3d350247d2e41bceec752dbe704"),
		new("knowledge", "Medieval Furniture and Container Crafting", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e67da93a4fb44e5660b0d9b32239c8b07fb22e041a02a82037c8436a773952a2"),
		new("knowledge", "Persian Household Crafting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "92ca92fddee6953d3d96a718a1bb6b935f9351f6c05a0e2ed8541881f595259e"),
		new("liquid", "ale-mustard trencher sauce", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2a91090bc020fd9739d364c6aea52fe3153d0f39ac0a12320472118b8d22be0"),
		new("liquid", "butter-mounted caper sauce", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "abae4c60110c2cd9c41572a38462111f4987cb74b5acad3d5e6b69ebea20223d"),
		new("liquid", "buttermilk table drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9f7db15dc188f4300bb974609369ddf2bbf2fa60156e93074fce02fe24ecc54f"),
	];
}
