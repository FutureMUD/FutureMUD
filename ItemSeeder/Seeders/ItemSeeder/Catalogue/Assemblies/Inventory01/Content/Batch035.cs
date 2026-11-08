// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "earlymodern_military_armour_zischagge_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "45884ed1284eea5727612cafa472efbf26278b7f1f7e197f462da55dc6224781"),
		new("item", "earlymodern_military_armour_zischagge_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ac30e210b49adeb09f9451e35d8136f18aa9cf4a91f666b4589e76f685aba1ca"),
		new("item", "earlymodern_military_artillery_breech_loading_peterero_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "63a03d6bc22497fbb436ebe6e2e71a57cb88b7084db21193051807ccd005c02e"),
		new("item", "earlymodern_military_artillery_breech_loading_peterero_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d5a7f82d63b24eff8a249dbb5088880c897b2ff0154a821147039e0d99fdf0d3"),
		new("item", "earlymodern_military_artillery_coehorn_mortar_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "cc282a1771d23accedc6ab8eeab6b3e8270e56bc34bd8e92adc49380809a496f"),
		new("item", "earlymodern_military_artillery_falcon_gun_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "00ce977c8218d06ab89a070fe4555b5f513fc436ebde2fc85557514f991f624c"),
		new("item", "earlymodern_military_artillery_falconet_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f3ae3e03ed2456158c62560dd6f6c4bb9408b57bac14648d4ce5119a6f02bc8d"),
		new("item", "earlymodern_military_artillery_light_field_mortar_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1417d50d27d5f7ef684f7b8d296e7140d44242e0e0da777f346d6dcf9957feca"),
		new("item", "earlymodern_military_artillery_light_field_mortar_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a134ec27756c51ad2dc3b47d7be00dc51a0fd946c0fadc323c4e6157b5873e67"),
		new("item", "earlymodern_military_artillery_nine_pounder_field_gun_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a5ef75744715c71e2fcaef20002b164ba10db7bd81dca4166480b2985f2383a"),
		new("item", "earlymodern_military_firearm_doglock_pistol_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b10c4b10a65ad8180df264002d2511d7298bea0f56b92dd5da7912daafb467ab"),
		new("item", "earlymodern_military_firearm_doglock_pistol_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0b760f3ef53f506700951887fd5fac3b638a5cab27ea7baeee147a7999d16aa6"),
		new("item", "earlymodern_military_firearm_flintlock_musket_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d91366479e586b2b5859ca65a250800054a54cdb483019416b5a570cbf49981"),
		new("item", "earlymodern_military_firearm_flintlock_wall_gun_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b19e655199c434da604ccd6a3a30a876a316854266c4c446e81113aa53ba96f2"),
		new("item", "earlymodern_military_firearm_jezail_matchlock_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a4ea5e8c94feeabcd31a4a4b47c8cb74b51670c08b16b7dfa8750cd777236616"),
		new("item", "earlymodern_military_firearm_miquelet_blunderbuss_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "51bfe0516f7a96ad76852e7a3007c0a0474c40f1a143526118d278298ae46780"),
		new("item", "earlymodern_military_firearm_snaphaunce_musket_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b2a84c89e8213b6fec346121c5a57fad1b3875b333734ddae9da723c74db60bc"),
		new("item", "earlymodern_military_firearms_wheellock_spanner", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a0e63ac535029501229d4297464a66e270e9c1df4a5596020fc8569e034bdbb8"),
		new("item", "earlymodern_military_melee_basket_hilt_backsword_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "983bc75b999fdc547bf560b5d7e61436fe8b5ee7ba6a137594759991a77d7574"),
		new("item", "earlymodern_military_melee_basket_hilt_backsword_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "83da599e8dd4df9f740b32a33c44822fe5a3d35828472cd801201788cc8118ff"),
		new("item", "earlymodern_military_melee_boar_spear_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "887c0e3a1ff944ac63085b83242a3513f456aff71d4661edc63b0e3458042b41"),
		new("item", "earlymodern_military_melee_boarding_pike_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a66bf88170a4fd8eeb718b85194205a3b2e90306207b6e08f022673c5e3d4f9a"),
		new("item", "earlymodern_military_melee_chain_flail_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c8e65da2cd4f4b6e5cfa4234d36f55ead08d8c156ca683a73b37672c80ff6773"),
		new("item", "earlymodern_military_melee_contoured_tournament_lance_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9b51132afb0c268bf741a9dfded13f6b7e0b8e0a510851d6790b8787111a4035"),
		new("item", "earlymodern_military_melee_contoured_tournament_lance_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b37cfb9053327aa720a2d85afb630784f0bac7e1f0ac8624057805435da64f7f"),
		new("item", "earlymodern_military_melee_court_sabre", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f992ba1e19ddb34c3930e87fb0fbf4c6a931e3d84b51a16bb3d913909502eaf8"),
		new("item", "earlymodern_military_melee_goose_quill_dao_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8439e7fa4d1d578d31eba4e009e3cefe31a3410a39a5341c30ab1c80afe24127"),
		new("item", "earlymodern_military_melee_halberd_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a23ea6904904c9663010ac76334ed20f96d381c57559c57663caed3f27a38607"),
		new("item", "earlymodern_military_melee_kampilan_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "96cd9ba06b9d522dfa9982a205bf313825dc08125eeb4f92642117ea422a51a2"),
		new("item", "earlymodern_military_melee_kampilan_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "508cda04838f571992ee3165f7ccebbc5e4a5ebfe85b532e0d3f0d5ca7704070"),
		new("item", "earlymodern_military_melee_khanda_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b8638e54122ea38a5f96829e27f3e0626370a8416ecec826e3c8d0e80ea5339a"),
		new("item", "earlymodern_military_melee_khanda_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d6d849475f8cc50fa38c4fbadc751df6e814fb1a57df94c3c15c1eb154d9edf6"),
		new("item", "earlymodern_military_melee_kris_sword_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "76b3f68309f3fda241527090c7c2764b4633472babf70c00251361dcdbbcefee"),
		new("item", "earlymodern_military_melee_kris_sword_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "91a4857a2f70e506d118b2d59e6c26938df583021e9b6691ae9cfe8256b40627"),
		new("item", "earlymodern_military_melee_naginata_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "060a3bdfd66a05cd74c410f3cac5ad752cd1f29831a153ea6205476a527a4c4a"),
		new("item", "earlymodern_military_melee_naginata_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e520409f6f538ebddc4daacbedfb0dbd1dfc1a90758cdc486e65af60757caf6e"),
		new("item", "earlymodern_military_melee_naval_boarding_axe_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2245919ccab799761d5c3dd304dc487bf805ca22f9e69ccd7e7cceda7fa5974f"),
		new("item", "earlymodern_military_melee_swept_hilt_rapier_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "073374890505be4c9f18ff89c9a9202be1099e38ed45ff4d051b1e08940a9d58"),
		new("item", "earlymodern_military_melee_swept_hilt_rapier_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7dbf82d71822491cbd61a0d6e1fbeb8622c311d86bb22b8cf8fb1af9b7b054f0"),
		new("item", "earlymodern_military_melee_walloon_sword_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "21f028fa4a126aa3cc5b68b372f9521764eef8d8314e49c60f85dcbc5664e8d9"),
	];
}
