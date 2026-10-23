// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory02Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch036() =>
	[
		new("item", "earlymodern_military_armour_mail_sleeves_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f7bcc5f48f3dea506685eeb4978ddde46bdf75dbecf7bbf5e628d7e9eb0db93b"),
		new("item", "earlymodern_military_armour_metal_scale_spaulders_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7506e61357f14cdeb22e84e346d6dff8b8ac5866fdd31b92feadbdcc2f966f8c"),
		new("item", "earlymodern_military_armour_padded_infantry_jack_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "bb49ec279fad326a3e54885697d150402923176da6a8610775e60f0f5d6a8048"),
		new("item", "earlymodern_military_armour_parade_armour_harness_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "03282fbebf125457901d9715d61fdf1b91fff12c2a4a591974bd222b894531e6"),
		new("item", "earlymodern_military_armour_pikemans_cuirass_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a9be32365e97f1db55ec5f18eb89e9a362423f8d9acd09baed81b21a8fa7ff22"),
		new("item", "earlymodern_military_armour_plate_couters_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb6aa7af49df55cab9d54f2e30c3da89ba565551e78ea2bf206b82958c650ba1"),
		new("item", "earlymodern_military_armour_plate_gorget_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "47b667635033cc859dda478cd3ded11772ff38f3c569dd593e7b8f1165fd619e"),
		new("item", "earlymodern_military_armour_plate_greaves_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1e94ef6614667f8826daa62f6dde9623f7d9c34df63e8048f3e016069e122f90"),
		new("item", "earlymodern_military_armour_plate_peytral_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ddc72a13a524f80da08a5bfe44d4782a5f20eb130247208f7687f706d2377719"),
		new("item", "earlymodern_military_armour_plate_spaulders_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8535ba3dd2076beff5084e9b81464241f75a9dddcfa53cc18d4d96a6440d0405"),
		new("item", "earlymodern_military_armour_proofed_cuirass_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e454a91c37db19da2588d1d36d860405fd5c68662fc7d12dd597801dccfb4c3"),
		new("item", "earlymodern_military_armour_quilted_skullcap_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e484d1ef98bba586453c2e51a2790ed581dbe6b86ccc9d5452524213f2eaf6c6"),
		new("item", "earlymodern_military_armour_silk_faced_quilted_coat_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d953a22efad5d769fe08daa482363e4ea49f70898f06351a32a36a43cb32453a"),
		new("item", "earlymodern_military_armour_splinted_cuisses_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c126f20f8dc41fce5f8c851d1f33ddbdb0bbadd37d88717f794a22b8701a831"),
		new("item", "earlymodern_military_armour_turban_helmet_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8734e85af270d22aa942ad66c712d2db7fc2631993862db050a335c3f9b9dc00"),
		new("item", "earlymodern_military_artillery_ammunition_bar_shot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de7c16f04dd74b3ca835b89e24ab411bb44818d9f8c023f594acc17a3c3c0333"),
		new("item", "earlymodern_military_artillery_ammunition_twelve_pound_case_shot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8667a0eb4830695e82350b4e1b465ad102fc81e82118250c4163290cbfd06c4c"),
		new("item", "earlymodern_military_artillery_swivel_gun_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fcce890af176d28f5d6abd34ecd5b7f6edb07cdcb182b5ec6305ff7a024aa087"),
		new("item", "earlymodern_military_firearm_flintlock_fowling_piece_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "345e2699d55e2ed68ab4fd2e4222d5855bea33b9ed88d6f63eb6e999d4fd22a0"),
		new("item", "earlymodern_military_firearm_flintlock_fusil_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b35f9a35f688e461d9c835ae901641df08ddf56f64821e89d9f4fb5e33fe377e"),
		new("item", "earlymodern_military_firearm_matchlock_arquebus_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "327bb903c7cb91b2b1488dd549cad886187edffb5aaee636c32b343c127284ee"),
		new("item", "earlymodern_military_firearm_matchlock_blunderbuss_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba135aff45d32792c35e5d58fa0b45ac2b889f4d1a3f8a8fb56d980f0792a6ee"),
		new("item", "earlymodern_military_firearm_snaphaunce_fowling_piece_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e2ff638bd2d7bd1ea9a3901a9cfb8318645d0cff4964cabbce13befa044d54f5"),
		new("item", "earlymodern_military_firearm_wheellock_carbine_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6a845fc30bff195e0a5fd4c0f534593366feea38bc05969cfdf47fb885d2da5c"),
		new("item", "earlymodern_military_firearm_wheellock_holster_pistol_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1995ffc0dc1e1c033e67d31f86ebb8c0b2f7a8e0fa5eba4e22f20731c89cec35"),
		new("item", "earlymodern_military_melee_ballock_dagger_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b7339e41d7c484ec1c4845af6ba12d73f280303bd920c11617b892fad8e4ea67"),
		new("item", "earlymodern_military_melee_curved_hunting_sword_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "03e67d70c0e0fbfd1f7cc2b78e2dbd586657259b1f44ba0ad86d588b6ed85d7a"),
		new("item", "earlymodern_military_melee_infantry_half_pike_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8941f1330bfae0fb13bc1db216e971e963725f06f7002cb3e9ef6cbe2fe546bf"),
		new("item", "earlymodern_military_melee_katana_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d49a27d9d4fdcbe9c1a64daceda218229127f193a19e0aa85ceb0b07303a5ff"),
		new("item", "earlymodern_military_melee_knobbed_cudgel_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9e5ad83238c92f6ccfd52f89c39fd4ccf8ee4e1adba26d3b6c618e069925ef93"),
		new("item", "earlymodern_military_melee_long_company_pike_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e9e055c47758cd920be2588043a8ffc0032911b4e00f0c5ddbca58e62d74e6ab"),
		new("item", "earlymodern_military_melee_long_shafted_war_hammer_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d815070d3bef838c0353c057cbba6b93120a3ed4205e0bade03ef9adaa586d22"),
		new("item", "earlymodern_military_melee_mortuary_hilt_sword_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c97f0a11be8c5dd189f234c22333c3cd2d478640da981e0ee7497f6c8c8dc66d"),
		new("item", "earlymodern_military_melee_naval_cutlass_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e0d586f632ce971ab2dc63d26f2d6b1d89e4f0006ddcf800167cbf8e650e3c78"),
		new("item", "earlymodern_military_melee_ono_war_axe_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8589ec947f90243e3e843714d47fe00286efdf2df5963bb40f84ee94de36903e"),
		new("item", "earlymodern_military_melee_ranseur_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "2e46c4f8410cb3448226a37a7a259954fbbc1515240c03202cca0dc8420e232f"),
		new("item", "earlymodern_military_melee_saif_fine", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "acd9a756fdfb18daebd5af106a9993c4ff6399b4d3dd8b74e431ce64b7ef1c29"),
		new("item", "earlymodern_military_melee_schiavona_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "797077edb76f1e40a615f37d2ca7035b07da0ca7738c9e2443f5fe3e58a1cb1d"),
		new("item", "earlymodern_military_melee_sidesword_service", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "7c60ee99f239e713b1136b334a073e545c7fb5655fa6f67a474417e3a81aea1f"),
		new("item", "earlymodern_military_melee_socket_bayonet_ornate", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "b704133b6ed3a3aedc262379089ec3ae86c64c93aacf443cd76af3fc8ac1a128"),
	];
}
