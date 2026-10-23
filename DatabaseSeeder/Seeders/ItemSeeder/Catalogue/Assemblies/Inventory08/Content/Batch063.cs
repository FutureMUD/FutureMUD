// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory08Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch063() =>
	[
		new("item", "renaissance_repair_printing_equipment_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "09e8e4bffbb3c21be2ee1efed7339e7a13c6f56854babc0e678bbeb70075d988"),
		new("item", "renaissance_repair_rough_firearm_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3cfaf56005ea006333014520f06717df218cfcc714c75c213863f98041130715"),
		new("item", "renaissance_sahel_veryfull_trousers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5675289e5afe6dcb0c29949b6d58dad5952cce609c47045281a4bf09bbb54d88"),
		new("item", "renaissance_shared_clothing_felt_overshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bdebcbf844d5f60a8259e939dbf9c778f6611454b0a206539ffd012512de42a7"),
		new("item", "renaissance_shared_clothing_leather_gloves", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7654beb8ea02370494fc65ec342e290f3332a19b4ef527e096cebb5b7480963"),
		new("item", "renaissance_shared_clothing_rain_cape", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1db6fd16cd8a0b476759ecbb47d3bd007621a359859e948120ab25d2038ccda2"),
		new("item", "renaissance_shared_clothing_wrapped_loincloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "52e8732c2539d5e2416e325a6f6dfccad302ba30ea328d4b5099e18d2eb53dba"),
		new("item", "renaissance_southasian_flaredskirt_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dfe95fbf9237db2d1218c90ed18ec3284093575b773687a61d3a12427e34daa"),
		new("item", "renaissance_trade_andean_storehouse_quipu_record_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ea779c5b1382acda5a640ad18bbbc391e6dc80294e6e568df5caa677d17072df"),
		new("item", "renaissance_trade_apothecary_perfumer_herb_sample_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8598b20c3fec8498c69900bb5f4556aa90daa3162cf410a2b27073cfea8ee73f"),
		new("item", "renaissance_trade_apothecary_perfumer_scent_oil_flask", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "88ab26e87d105412a50feae0414cdc5b72210d1e6fb53f375d2c39ae23aa329d"),
		new("item", "renaissance_trade_atlantic_contact_assay_strongbox", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5ff3198ca5853c6ccbc09ac38c7ce6af14dda13ff7f760652b1fb435d6d66796"),
		new("item", "renaissance_trade_east_asian_literati_invoice_scroll_case", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "87aa29f6f95f32d35d878ebe448fdbc5b2d7fc4a4ad34b705607ced5a63be75d"),
		new("item", "renaissance_trade_persianate_court_jewel_coffer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc9ee103f319938808a1c963608495421e3fdf6768323027fa4a2ae7627f77f7"),
		new("item", "renaissance_trade_sahel_redsea_indianocean_manuscript_trade_coffer", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a11db8154604a904c53b55bed4739861a5efc9b906da63791782fed9dcf67233"),
		new("item", "renaissance_trade_sahel_redsea_indianocean_spice_measure_box", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "deaed9d32a62acf505c0598ab70c0cf10a6afcf338ca54ae461755a9753ef2a0"),
		new("item", "renaissance_trade_southeast_asian_port_spice_pannier", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2bed27fef04d21bec9ff061b51ee6c4cf580cc8e725cb487bb2e3b1c34e4f6d6"),
		new("item", "renaissance_trade_tavern_inn_wine_account_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82b08f94f81660e091a559af77fc3dcfa0dbbc5f802f1a2ccec04b2b7cecae44"),
		new("item", "renaissance_trade_urban_kitchen_oil_transport_jar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c852f8376f2dca4a5db58c7b576adc8cc066b5fa6fcf3f79fbd120dd418b0064"),
		new("item", "renaissance_trade_warehouse_dock_marked_export_rundlet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a55e7640f506bdd36ab84486467788dd0fa1d2edbdb3b5ae672f3137af4c75a"),
		new("item", "renaissance_western_false_sleeve_overgown", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8319ff368a7ee9de02bab84c2d2e4b9a4b84c8f3743faa2ecee368d67b2ae05f"),
		new("item", "renaissance_western_fitted_kirtle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "22414ca7273c2e9d60a649fd0926169c973a0f9a2ad10b61e11ab6e4882ca579"),
		new("item", "renaissance_western_front_opening_kirtle", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e15f8a5742d81f832866cbe947ff4ee908c88f66fb35f8d1fde2fd720dbf7171"),
		new("item", "renaissance_western_netted_caul", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2ee88a2865de5f42dca42635fd2068f19e51e253861a562571b35e9a47156087"),
		new("item", "renaissance_western_slashed_doublet", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a38b1d24903e477352d59f05f55575c153b34c98ab6adb24c5e6326e4673e8cc"),
		new("item", "renaissance_western_slashed_puffed_cap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "faacbcbbfe96925f819a19b41a3554473db58bed1e5e3fee148953d365c83558"),
		new("item", "renaissance_western_stomachered_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b86ee8e2091f9dd8f552afc1d0555ed0502f915ead1096e838bf124c9f4493a"),
		new("item", "renaissance_writing_copper_engraving_plate", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4f17d6312aa3d25fc9deef398451f18b1932ae958610ac0adef9fde94c970717"),
		new("item", "renaissance_writing_engraving_burin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "90b39b10c68c2d4c9d40d88bd80efc3d96541f495d39e7f036232ddac88a7578"),
		new("item", "renaissance_writing_woodblock", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af6be3b753648a54d3faa9d46d118f880077114cd6ced2d092b10a9859a5b5ef"),
		new("knowledge", "Ancient Glassworking", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6f5a18e0a6e0ce18919d946a013123f4bff947d549867edbace67ef0642e8b6f"),
		new("knowledge", "Ancient Papyrus Making", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b59676743586724d42d443a4b1e8241def66e817452b5e28a93c5c1bc74ab6b2"),
		new("knowledge", "Ancient Tablet Making", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "2be1425ee93c34ed3bd1aef0584ced6bffd1bdaee07f90b20258f25ce4eebcaf"),
		new("knowledge", "Celtic Textilecraft", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cde06f365612a6c7031e9de182e6a9d75ae8a5f68cde9986c1986847b4524229"),
		new("knowledge", "Hellenic Textilecraft", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "f27cbacd43c07dbbdeb6a41f39ed9611f0a82633d952550a8aa37e68b3479b38"),
		new("knowledge", "Primary Production - Historic Commodity Work", "crafts", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e32ce6b1ed59fdb53db3c435782b9e05fa64b314c884d34c5d393b0fc2f4486f"),
		new("liquid", "Mughal sandalwood-rose sharbat", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2cd3041b1b4f4e0f6afd24df22b2794cb92106ba48d9f925f2f3432f3325019"),
		new("liquid", "dark grain vinegar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bc1f7e42cb304c69dee451f258e832a40a74562c606c0d48dfe06a55f3ea8930"),
		new("liquid", "dried leaf infusion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a8d0ad5705189f63f56602313f651e95b0e7fadfb900bcdaf4a6a4009a71c50"),
		new("liquid", "fresh leaf infusion", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "362a381f2b70a8e367789501906a4162c7a2a02360863b9a030448676aae3f74"),
	];
}
