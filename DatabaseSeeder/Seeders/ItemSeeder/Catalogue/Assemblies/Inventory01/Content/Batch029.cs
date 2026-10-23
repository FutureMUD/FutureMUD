// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory01Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch029() =>
	[
		new("item", "antiquity_wide_household_shelves", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "9ee0c43133fb2d17634fc62c3d0b40eacbf88d52d6bab44b6c2f05ff4dd493fd"),
		new("item", "antiquity_wood_rasp", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "0d8c1311487fe484bd1b59f20b25aec20618f730aa3c6242e941c6a741260bac"),
		new("item", "antiquity_wooden_corpse_bier", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "3a6e7ae2a99bcff2cf1a16c3367f27124fdfdb6747d087fa74e39a3b27e4387b"),
		new("item", "antiquity_wooden_smoothing_plane", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "7b6010c319047b4ee9f188c615168ec797c3d14a423ef5ea296bee773fc1e525"),
		new("item", "antiquity_wooden_splint_pair", "antiquity", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, "aeb133087804c5160e97d008d446d7b79064dce0c067ebc608c32984c723c69b"),
		new("item", "earlymodern_brazil_noble_female_travel_outerwear", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51077131b0993e1235fe6c3507b77416578d2913bd62a2e677afa747f6ce6467"),
		new("item", "earlymodern_colonial_clothing_full_cotton_skirt", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "51f42aa0ae2bfc469c2c3a8666a051fbf8dfdbd18e4f9c06202bcaa824042176"),
		new("item", "earlymodern_colonial_clothing_short_wool_jacket", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "db14d40afd8fd8a9b0354075a10f3db150adbc2eec7e80a1a326fd63ae33d19e"),
		new("item", "earlymodern_colonialnorthamerica_noble_male_court_garment", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "f72113a5e2001a67a9746dd64213836d74806953e14efe40bb339418fcbfd0a3"),
		new("item", "earlymodern_container_and_ewer_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8369294176eb148820e1c55197b594468f45e056e933d48e56a632cae59df3e"),
		new("item", "earlymodern_container_and_linen_hamper_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "4bab56940b53904a4721011337b11aac0871dce4fc85a84635b000cbb815f218"),
		new("item", "earlymodern_container_bra_lockbox_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "405f9f1dd46366a96b1757652c86940880eaab997f1d5f703239661b45eb0438"),
		new("item", "earlymodern_container_bra_storage_jar_22", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8b2b64d45fec355f6f4adcf9097d34c631249194cb0b00617f0303a4de41cea8"),
		new("item", "earlymodern_container_car_lockbox_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "0a4693c5ef1429eb85699e8d8ff27437d875b3cdc57c4f3695cceaff1b8c629c"),
		new("item", "earlymodern_container_dut_purse_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e0ad2edf55b3d4fe87972682d304bd234f9d17fe85a754a1dfa8a91c21bcc12"),
		new("item", "earlymodern_container_frb_lockbox_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c67ddb7637cf39260e3dfc0ee6c15eb2dbfa1dc9774ac81e072c54afd7a5a221"),
		new("item", "earlymodern_container_ger_linen_hamper_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "c60bc3fe6453ada8b848bed631ee73700c5c874ca4b3552704109548185bd9a4"),
		new("item", "earlymodern_container_ger_lockbox_33", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f1b55582ae34afaf0608acdac151de253a839e946d5a975d35d9034467b35d2d"),
		new("item", "earlymodern_container_ger_storage_jar_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a6b425307096bf8d043ed24884d16b289fe367f69f10b142f89b1d0b6a9b7b41"),
		new("item", "earlymodern_container_gmt_sample_case_25", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a94bb3749460c3ea892a1f9777afdf56e993d6fa542a242a4bd6a54d15e2e7c5"),
		new("item", "earlymodern_container_gmt_tankard_30", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f9866fb15a4aaf0da16820dc94ff526ccca0aa1699e5e1fe3ceab3aed9626e0b"),
		new("item", "earlymodern_container_gmt_tea_pot_28", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "de12c12b646dda85383790fb8b645629e8dc559481ba262915ea8f3de74e6b4c"),
		new("item", "earlymodern_container_ibe_tea_pot_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "508d852606246b6611cd86ce2c926cd3642625441ce3cb30db19b3eff14c7369"),
		new("item", "earlymodern_container_ita_sample_case_21", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1557f1c52aa87dfd4ee5b04b7842b141f2d8da0a7b9918c0dc1cd9701669b293"),
		new("item", "earlymodern_container_ita_storage_jar_29", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "def70a5b06bccaaf95f3ba6f4b0a7a82c38ffa48b172087385b610456bbb31bc"),
		new("item", "earlymodern_container_ita_tea_pot_24", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "9c67cefad80c77b6d44cb90724dda83884372189213f79fa46e156eb1d773c0f"),
		new("item", "earlymodern_container_kon_linen_hamper_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "6d8e2a1ecff6dd5321552b78cb67f9a32fb269f0f9f29c8131a23d784144cf26"),
		new("item", "earlymodern_container_kon_spice_basket_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f6cf1b09e80ee4254558633ebaee03664a78de6ebc6eba90eaeda9319fa9bb11"),
		new("item", "earlymodern_container_kon_tea_caddy_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "728aac689adb87b3ef8764fb7d6b9947d57baa13a5428f15a196553e9146ab63"),
		new("item", "earlymodern_container_lmg_storage_jar_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "d8ceb8ff59b6832462b584c8b5847cf5ff9c189f2a330c27e2ad7aaea4e9b1a8"),
		new("item", "earlymodern_container_mug_tankard_19", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "f90885e96038f63d7e4b1ddb64e6573831fa73132b2af2de27dd75040595e10f"),
		new("item", "earlymodern_container_qin_ewer_18", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "37f08e0a1ae0eb362ff63eb2b59116206b8258cf04aa0d145de1106ae026284b"),
		new("item", "earlymodern_container_qin_linen_hamper_26", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "377ff4976a3a56fe79db35feceb893496d4e1b821027d76393aa9db3c060ee93"),
		new("item", "earlymodern_container_qin_tankard_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "a7bf0f8c05573d321dca31ae6911a6e2cafbd014f7d360d70bd91087f1275809"),
		new("item", "earlymodern_container_sea_account_box_15", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "e8bdece08e4118b31c50ad88b67a395d7360a5c4104985ed4d877b23445fa7e5"),
		new("item", "earlymodern_container_sea_document_case_16", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "483e26eafd0933014ef00825a74ad86f84a7dd5c4ea7df23ede7c2377fd22382"),
		new("item", "earlymodern_container_sic_lockbox_23", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "1551365d7c6b94da8589ed78e305479c650fc4e50c047c5c4e7462323df251c5"),
		new("item", "earlymodern_container_sic_storage_jar_20", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "fb5d754d52baed3092129b6d711f08c911550c1c4d402344e824dffe767a2aa7"),
		new("item", "earlymodern_container_sic_tankard_17", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, "26ae32b10e6f6155cd968121ba555883ba549955adfc89429edc9e34bb59cd89"),
		new("item", "earlymodern_deccan_naval_officer_coastal_coat", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, "85d5eae5732041ec570b37faed29f55d1eeb5d3bc38d9318ac6390ae7bd2106a"),
	];
}
