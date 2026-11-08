// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory03Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch062() =>
	[
		new("item", "renaissance_jewellery_sas_pendant_243", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "854e26ddf8b07ef8f5d175541f409ec3e12037ea876949b054c91b9b40d1b53a"),
		new("item", "renaissance_jewellery_sem_hairpin_372", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "5f22b3091c487837ce930b45169d60ba34f84dcd61ba32f937a14c7fb7653f28"),
		new("item", "renaissance_jewellery_sem_waist_chain_375", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "74340acf50c6ff9714c4ccaa86e67e4d72d2fdccb12d55640b5e37419dc169ac"),
		new("item", "renaissance_jewellery_stp_anklet_428", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "2334d69a09e81ec573c4415f1a56c4ae1f3d83f1d33db6fc487dbaf13f3c6f14"),
		new("item", "renaissance_jewellery_stp_chain_necklace_425", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1ef40dbba9a624abc3b0b06e85230f9cd3fa320a82edbbf908a572c2b08a6d1c"),
		new("item", "renaissance_jewellery_wer_processional_ornament_30", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ec3c4350f141413a4d5fb0a851a34c1db064caa75e84b4fe2c2f5c69d3959768"),
		new("item", "renaissance_jewellery_wer_votive_token_25", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "06965c5d388e133e4c55ebd5c4430fa6ecbbd7ee4b5fd5d81e5fe297fc944890"),
		new("item", "renaissance_joseon_broadsleeve_scholaroverrobe", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "a82e284b332d0b87ffd7120a0d0e869d3a1af30ec1b40f1b6cee933e98503913"),
		new("item", "renaissance_medical_aromatic_vinegar_cloth", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "775076d5a3c1cf7bc3b037a324d2c94bb5da34e857e80cfe652a4a78fdcd05cc"),
		new("item", "renaissance_medical_bone_saw", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "c959616853afdb387f1121bddddf9d7025f44d299959c9c1b109cde179ead31b"),
		new("item", "renaissance_medical_distillation_alembic", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "4cb74fd80262b0a9132c9a6b25047c23b7109255dc6409c93f9ae7cbd569664a"),
		new("item", "renaissance_medical_enema_syringe", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "7429bb636f3d6927fe22faabc7c71a86fc09a2149d7f1292944f69076c864caa"),
		new("item", "renaissance_medical_powder_paper", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1dd3266ee7ccc13951889f346a2b19deda497aa8bbaadfebd5f15d6aa7cc7647"),
		new("item", "renaissance_medical_sickroom_chamber_pot", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "268e0852ef1f493bc570713af0fdad7c8c9ee16650a3a1597954131502bd0602"),
		new("item", "renaissance_mesoamerican_tied_cotton_headcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8617071d4ef8fc8f7ef02d38b8fc5b7f8081ca6345b8962b592989237627cdf4"),
		new("item", "renaissance_military_armet_italian", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "cfe1296b5188212dfe6b20db2067c8e2bd1e026f4650561ee32d149e1f6c5265"),
		new("item", "renaissance_military_arming_cap_padded", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "e177113a5d4c5fe79b5213503a98fb70e5a3702a7038886e44b8b0732e6bd912"),
		new("item", "renaissance_military_artillery_stone_shot_light", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "36eed474202e83be7e82ad8d4fff82e37d37297a58209e57b3dc462f739e6433"),
		new("item", "renaissance_military_carbine_sling", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c057ca608ea8ec29c8f0563ede8ba4745ded68d19260fa4276666cf4478c068"),
		new("item", "renaissance_military_demi_culverin", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3253d0ed148a2d50ad4bcd77cca2433df6577415b55df27409426150b01786f9"),
		new("item", "renaissance_military_helmet_burmese_lacquered", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "48d657a8805d66e6d27398b182751e1dad46f3012aad9db81730a07a24be8dba"),
		new("item", "renaissance_military_helmet_uzbek_lamellar", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "1c22ffd5ba4d92e17da997def00c0469955651a38d8fe61ffae1515592344a10"),
		new("item", "renaissance_military_mail_plate_shirt_ottoman", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "11c5e10f131acfa7c8ac4368c55c00afe6d91d91c22e367ef993426ce84173b3"),
		new("item", "renaissance_military_matchlock_heavy_musket", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "07bb8b104e154f7a75fc147dff7afbf6dc9f5b99c6e3f075c1246e3c0ae87cd5"),
		new("item", "renaissance_military_padded_jack", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8d3b40df1cee81a0e0c27b5a01bef87d12ca0942dc3c05e4adc5252fefb911ef"),
		new("item", "renaissance_military_tassets_scrolled", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "ae9990f6390623d467c9200c39692bd2681f1a69319f6e7d6125e4af0fa954d7"),
		new("item", "renaissance_ming_horseface_panelskirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "82e2b811416a70fdd5d08813774e426ec41f20e0f86315d5624dd803172c4d48"),
		new("item", "renaissance_ottoman_dolman_riding_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "669f6c160da5c7b69df138ebc59368b19c1bd3332ad10fc8877211d48f489d90"),
		new("item", "renaissance_ottoman_sleeveless_kaftan", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "828c44acf67b9d77f656b9a8dbd3bdb0caccdfbe4cdd55734b7a3c9afa5c2643"),
		new("item", "renaissance_persianate_fur_edged_longcoat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bd9b0e56a7293f0f65d9d073209edc57df99427864d596ad1f523a59742898d"),
		new("item", "renaissance_personal_apothecary_perfumer_herb_pouch", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "be1d2bf0c7e3da1b5be1992c3e62577b63bac909be94bb057df341ab387e51ff"),
		new("item", "renaissance_personal_warehouse_dock_warehouse_key_pouch", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "bf624875999333d6707033a052bc9008e5a508ea689b423a86c3bf97a7c80e5a"),
		new("item", "renaissance_redsea_leather_highlandcloak", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "9dbe8d6b1cb7279dcb6dc93186e89ab1ad89f2a1673f067de2d03cbc7cab41b0"),
		new("item", "renaissance_repair_rough_medical_instrument_repair_kit", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8ddcb68d5335454536c3e6d6894e260832acd93c34289c917c32ed2d8f7ff66"),
		new("item", "renaissance_science_horizontal_sundial", "renaissance", ["renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "253442753b5382253cdf5b862fefaa8b5197d91ca4ee272acc3dd6c48e1b7602"),
		new("item", "renaissance_seasia_split_riding_waistcloth", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "3bc3de6fa5aacc18714fa84bd590734fee5377c00ed170bf95cc196f489cd4d5"),
		new("item", "renaissance_shared_clothing_neck_kerchief", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "92a2b8a605642a9f1a118e9cd515ec7bbb8a581c3634eb35350b847302bfb2df"),
		new("item", "renaissance_shared_clothing_quilted_nightcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "b34e0a06e5599e93cba8fb36a20e8fbbf3c1c8c7b32e46e1518e4f416265a643"),
		new("item", "renaissance_shared_clothing_sidelaced_townshoes", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "d43ba21ba169398d06e857234924ff135b18b8a4bfc0fed40b523494017cef03"),
		new("item", "renaissance_shared_clothing_tarred_canvas_deckcap", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "8a6f504260214bc0808b21e20ee8bae23044094655f5247bc069d96c602d4fc3"),
	];
}
