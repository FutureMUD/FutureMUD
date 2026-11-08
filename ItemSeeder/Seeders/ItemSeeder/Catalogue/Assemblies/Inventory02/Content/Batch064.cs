// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch064() =>
	[
		new("item", "renaissance_repair_fine_clockwork_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac861a65a6a48186a5fb59c2eccafd2d6280ab6fcc014bcb68e87644a22331be"),
		new("item", "renaissance_repair_optical_instrument_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a784d62f2a5866dd03275060c391f76eea6da429204799540e900152f95c1c26"),
		new("item", "renaissance_sahel_embroidered_necktunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b8679375b3c3c75ea143f172d8f7013b9047fa0aaa88124e66509298967d2a9"),
		new("item", "renaissance_seasia_fitted_dancebodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "01ecb9b6e9b3a4aa1dd7b7f747be76a3a950e67b64b22c5e8061c96daab7a11e"),
		new("item", "renaissance_shared_clothing_breast_wrap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7ad78edb72d327be0b21bf4298595967328107293ba4132de3b36d66f5160151"),
		new("item", "renaissance_shared_clothing_face_veil", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e889eefa4981987c36c95c6f9b9498e82f5b662df7f55edacf6b06bf50de007"),
		new("item", "renaissance_shared_clothing_gathered_skirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a85cb98d9b4edca4c72d6dce57041cd9eed24f3f9a6a314e72c86ec32e4bbc5f"),
		new("item", "renaissance_shared_clothing_high_riding_boots", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c7416ab8e894199826d80e8d933cc05449a1d014cf7c324d8d45d12ed5063f48"),
		new("item", "renaissance_shared_clothing_low_ridingshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8d598d3b982d2be9d1e06bd8e4b13d5379c73098780aace942798359ed02b6b"),
		new("item", "renaissance_shared_clothing_mittens", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "70f78c289c6efdc6d642e5f9a8809349a0f926bf9093be732a4b72e7d23396f6"),
		new("item", "renaissance_shared_clothing_side_slit_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b322cded17a98af040eaefb60a2655a6a1b5849ba588f738d7b6caf12d759e1a"),
		new("item", "renaissance_shared_clothing_straight_tunic", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b173ff4a3d5ff141f4838e55af7a09272f3d9af339bdc233e32a5b86ce6764c2"),
		new("item", "renaissance_southasian_stitched_loindrawers", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "98ff2362d95b0f064c41af7eac36a2a06e11b9bdadb0185c445e86346fddd40a"),
		new("item", "renaissance_trade_atlantic_contact_mission_supply_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5c797575f6a148bd868de534e9b19dfb40247d60d2996bbb587fec00f9935596"),
		new("item", "renaissance_trade_japanese_ryukyuan_shell_faced_export_casket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1915a0ab3b4f14e2bd23da0c6f3178c0b4cdba56a316d1307e0a63ea8ba8a043"),
		new("item", "renaissance_trade_ottoman_bazaar_spice_sample_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1a1f4f7b3407a3607bd4fffb61cd025b931a6ad3547c70880a48160e98e75565"),
		new("item", "renaissance_trade_urban_kitchen_dairy_pail", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "27420613418cb5af7c8aed148297a1a32a22b0528b676c3e5fb43f33df8568fc"),
		new("item", "renaissance_trade_western_counting_house_coin_weight_casket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c374c6133a6cad2029d1e90a6f1ddfae370ce3e7c327e656cd8d89cf4c131049"),
		new("item", "renaissance_trade_western_counting_house_merchant_till_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "26b3324fcc093c4f9b3c010aeedc763c7b1cc28e9aceb52fbe2d8698baec4e98"),
		new("item", "renaissance_western_back_laced_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "affb3fa79ede337140326de756b6188cc618b3743cdc26ff4f03931acf3f69fd"),
		new("item", "renaissance_western_side_laced_bodice", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "af5e5cf7f1923b1930563657069f5f265de5ebb0796583294e95b73a41424e9c"),
		new("item", "renaissance_writing_archive_chest", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3df5331dfbe86bd3b66ad06be4846898efb91f4a1633cf548133f59a090a1c4"),
		new("item", "renaissance_writing_notarial_instrument", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be5e1287b340c051c93b1a65349cf506b6b534f6f9fd94810723c60b59f313cb"),
		new("item", "renaissance_writing_printed_prayer_book", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "6335fd3e8efdc82bbb0cbf97e662254ccbbe02f28721a28eeb9fdb73c104980f"),
		new("knowledge", "Ancient Household Crafting", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "b46d9ac7fd5d3f668f6efb43f33d70223ffefd16abc8a2fdeb14971ae78d8e87"),
		new("knowledge", "Ancient Parchment Making", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "6674171bce16e833a2c312e079348c1b7cfb644f48303e3c7360787422bc9f82"),
		new("liquid", "Ottoman rose-petal sherbet", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "50f1bd7965c848331caf7fdf789a0ca02588f8bf3193ac16ef1236944eba24a0"),
		new("liquid", "Tokugawa citrus-soy ponzu", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de3b851ee47a7a6ee7c187363858bf94d9918f5a66aea0e43ce3a17b848694f6"),
		new("liquid", "almond-saffron caudle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "db58d07a5a2098aab2a00346a3b8cd9e76826e99997eeac4fe4755bcb790cda6"),
		new("liquid", "date-steeped fruit drink", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b57c0a226ec776f1c99806c8ac0bc7913c2dc8b5e3d24304e24f617b846ba5f2"),
		new("liquid", "honeyed cultured dairy", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "0068ba42e22a67272d46a444bba51f7dab042a5a91a06982224bf8c7f56a27aa"),
		new("liquid", "light fowl sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a3058fc88a167f6ec08ce78634b621b7b2677dd1a1f9ccbdd2e777c5cc8b6ad3"),
		new("liquid", "rice beer", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "fafebdb2955c641ffdc7fe511119a7da8e99621a64f5c72ce7381fe31a005af0"),
		new("liquid", "rice vinegar", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4c76aa138e58569f3ac8726a7d281b344c6fef467b448fbc2bed2e9923aa0666"),
		new("liquid", "shellfish reduction", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7db7f9a834ced57935a3a8dacd92a21b77ab01439938052f07a9ff6f9e75490d"),
		new("liquid", "sorrel verjuice sauce", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, "f2e983c355a205d425a4c20bdc950f758b999863be61c875228bea2931f10c53"),
		new("liquid", "spiced distilled spirit", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b0a90108aab2297d5a74313b06e339fdf91ee2a22aacacdf94463bf03639f7a2"),
		new("liquid", "tart fruit meat sauce", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "991e7879b4d1c62016338d66554bbbc246aef9b4c5539f241cd991884abece86"),
		new("liquid", "wheat beer", "shared-preindustrial", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7d18ea379760500a99a5f3643bdcf415a8cabe3d85004da0fbe6dd9b3679aae"),
		new("material", "bronze", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc5fcc7aef93bc8aa4e12aa812697805de823b3a6be314089f1b33c36b662f52"),
	];
}
