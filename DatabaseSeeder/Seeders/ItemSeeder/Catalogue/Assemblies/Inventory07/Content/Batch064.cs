// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory07Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch064() =>
	[
		new("item", "renaissance_shared_clothing_soft_slippers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0086cd4d749ddc08098d2efa2fc7ec7d61222c18f4caa0ec11ec63eb5b68367e"),
		new("item", "renaissance_southasian_flat_wound_turban", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e6198f8d48c4dbe3c2ad65a669ba1200f7d14b16201e98354c25df6cb579a9a"),
		new("item", "renaissance_trade_andean_storehouse_chicha_vessel_cradle", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "30517fe27f7a361fa11d9f0df59230fe894a08325beb31db5a17845b7359f6eb"),
		new("item", "renaissance_trade_ottoman_bazaar_oil_merchants_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8ab54c42972622189ce683566154ecb324c966659c9642e89c26813f8b82c98"),
		new("item", "renaissance_trade_persianate_court_manuscript_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2f96a35d50c991ec0704f7a4cfba33d1df5b2f3e803f019d124e108406bf09f1"),
		new("item", "renaissance_trade_sahel_redsea_indianocean_caravan_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "950d9a36553bfdae52a9a59d2ede2f13a8b20de7d2fc25294daf66d07f9bc7c8"),
		new("item", "renaissance_trade_urban_kitchen_grocers_till_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9d9881291df5f1c6fc40897188da84a5d6c959a32b61cf6ef13672dee01f202"),
		new("item", "renaissance_western_broad_plumed_ridinghat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9a0a6483849fc78ec691f09e131e2b00fc2f0db1fe0e7733181c10d47c06fe3f"),
		new("item", "renaissance_western_french_hood", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8784897762466f03e573cf8bc9552379f17f9df8cf57e27561c524f9385603b6"),
		new("item", "renaissance_western_full_petticoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "04f341374b8f1cd6c35de945f4a14c201f867e91f979a4e6ad571805de293072"),
		new("item", "renaissance_western_long_skirted_doublet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c54056b5372ed256aded526e07054cc8406af1883af8ff19c64683cad93a0d9c"),
		new("item", "renaissance_western_square_neck_gown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "dfcc5860b52b3d94fe58058c5070f324166e1b1d592c06831bf569e38290a026"),
		new("item", "renaissance_western_square_toed_shoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "362d280fef97c5fdd31bc419b13f5833a6e32071e24fa83995a184952099b812"),
		new("knowledge", "Anatolian Textilecraft", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ee618a13a758ef71c1873f8de9a287fe0a6610443b2063bae8d01df878cf0ae"),
		new("knowledge", "Ancient Herbal Remedies", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c1ea4f276cd7d50834a8f2bd94a527d984d1083f4183db94906f66be5e8aafe"),
		new("knowledge", "Ancient Surgical Instruments", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "5d09e96b9683bc6bb3fed444c418ad6cd9596ec69cf3bcb55294de76c9c19d42"),
		new("knowledge", "Etruscan Textilecraft", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cf9c3aa46242edc4525a7530599eca70482c6deb5827717a279a42c18d13b2f2"),
		new("knowledge", "Germanic Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2af082a860f035de757061181eb4bfbaafeb203df5e0cbe17132cf5f4c8e0db"),
		new("knowledge", "Scythian-Sarmatian Foodways", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "8859db9791960f28f5d962ff6a101ef334081a61cad6fb5fd7c7472a94af247c"),
		new("liquid", "Caribbean cacao-shell plantation infusion", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "53aad98ded6e213bf68740d83bcfc1a9750fab973a66b14f6b794a7af9fd60ab"),
		new("liquid", "Ottoman sour-cherry sherbet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "66a76da8c657c871bb974535bd4e0ae982a0084f5fee3b5b68800c66eee05f98"),
		new("liquid", "Safavid mint sekanjabin concentrate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01876ea8fba826ae967ae376e69a30e585e302227522e815fa39178837d94b32"),
		new("liquid", "bitter-orange agrodolce reduction", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be60ff9409374fb0cd3f27a31f58ec8eeb55086f0aa5b14416523cee05692682"),
		new("liquid", "breaded garlic jance", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bf701ec53850cb7ad955c70d2f844b807c27b6df8385649c3be53a5651f194f"),
		new("liquid", "chile-maize cacao froth", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "18aa4ad4ca1c2269db77bcf3b198a620fd211c16c246dc5917cf5dd3ac514102"),
		new("liquid", "creamy cacao drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3c2f347386834460e9b49a2ee3e4c9188b995ee5d74e58ff70a58bfe41beca80"),
		new("liquid", "date reduction syrup", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4fd0930f64a0e3f4972f5099f5cf9a45af43160ac4726d4b7cc78770a237bede"),
		new("liquid", "date vinegar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "969222802664f17662612a7e5b2d6d6ad409d084ffd50bb19beb2952c25d3f47"),
		new("liquid", "fermented pear drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b6e39d40b2a0f17868294d3da15482d2494c8811b3d98f6782f8e90fc313e81c"),
		new("liquid", "fowl cooking stock", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b57d2968b29e7fbfbcb42f9c4a4abbe53826b17b2ecd49fc1a449b19777c05d3"),
		new("liquid", "poppy-seed fast emulsion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "528451d0a641d582370c600a4f7d3d2cc086514572abbd83095fb350f5245790"),
		new("liquid", "vanilla-flower cacao liquor", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01a396c1042407d524dc8f866f66b812096f888db99187e89207f3a5dfe16ad5"),
		new("liquid", "warmed peppered wine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f5ef9d0d718b3c71a4422e3c91d0f87991e9bc17927cb876654d6efc4e84b1a9"),
		new("material", "hematite", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ffebf6d8e960ec338ed40d8a7e2cbda93322219beca048263aed6e9b5d660ff2"),
		new("material", "kaolinite clay", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c22e0445aec84f55e4bf3a3c94cfff1b9fe3075c95f68412c2636d2cc6bdd426"),
		new("material", "limonite ore", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2b537da62d2232635647ece16c9c74d682f5a5b9ade81cdc31867e7b0496230d"),
		new("material", "oak", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "aa1f6c4d4764147b173f7db2107e97efc251c5c0b1831f0ff4a57da4344b7bf9"),
		new("outfit", "antiquity_outfit_0024", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_anaxyrides_wool_trousers", "item:antiquity_linen_breastband", "item:antiquity_sarapis_wool_tunic", "item:antiquity_soft_leather_riding_boots", "item:antiquity_tall_kyrbasia", "item:antiquity_wide_cloth_belt", "item:antiquity_wool_kandys"], ItemSeederOwnershipPolicy.StockAggregate, "492c4bed22e86ae567f660babf4b9728083655a05924eaf576e9c585b1a2444c"),
		new("outfit", "earlymodern_outfit_0008", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_french_clothing_lace_trimmed_day_cap", "item:earlymodern_french_clothing_short_casaquin_jacket", "item:earlymodern_western_clothing_buckled_leather_shoes", "item:earlymodern_western_clothing_canvas_stays", "item:earlymodern_western_clothing_fine_linen_apron", "item:earlymodern_western_clothing_full_wool_petticoat", "item:earlymodern_western_clothing_linen_shoulder_kerchief", "item:earlymodern_western_clothing_long_linen_shift"], ItemSeederOwnershipPolicy.StockAggregate, "8f8b7a98bbf776b66afbcc7f9c62452f1329c75984051cd97686cfdb382a465b"),
		new("outfit", "earlymodern_outfit_0022", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_western_clothing_broadcloth_work_coat", "item:earlymodern_western_clothing_knee_breeches", "item:earlymodern_western_clothing_plain_linen_shirt", "item:earlymodern_western_clothing_plain_wool_waistcoat", "item:preindustrial_clothing_iron_buckled_leather_belt", "item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_shared_clothing_felt_brimmed_hat", "item:renaissance_shared_clothing_footed_stockings", "item:renaissance_shared_clothing_low_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, "15b5e9da88b7dc060324c0fd233f35c46c7db9bfccf8a0ea1a52d62e9eb94df4"),
	];
}
