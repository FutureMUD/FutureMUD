// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.
#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

public static partial class Inventory00Stock
{
	public static IReadOnlyList<ItemSeederManifestEntry> Batch035() =>
	[
		new("item", "industrial_food_drink_packaged_0026_countertop_preserve", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e70ac65f7166858580e5110f2b52278547d1f3d5c6c67a0877254571a55ef2d"),
		new("item", "industrial_food_drink_packaged_0036_wall_mounted_biscuit", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a95f07b25ae6aaf656fc4147e0230cf2357857a092826d7dfaab281b5984f787"),
		new("item", "industrial_food_drink_packaged_0044_wall_mounted_sweet", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "634a6fbbbbe9d036e5ff39fff2ab0fc6312353335be3ebca44c7bb995c9e909a"),
		new("item", "industrial_food_drink_packaged_0045_wall_mounted_cracker", "industrial", ["industrial"], ["item:industrial_food_drink_packaged_0001_stackable_ration"], ItemSeederOwnershipPolicy.RequiredRelationship, "ade2cdacf60a0a5fdb6419f263eb8108a63f875d4fa099afac00b71b5350dd40"),
		new("item", "industrial_food_drink_packaged_0047_travel_crumbs", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "901cfdf7d2b743de2f85f5d8e1badde5e05f303ec07455d26ac4a0db8b8fc0ff"),
		new("item", "industrial_household_furniture_storage_0004_lined_rack", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "0866d2295e2a836fed4bc0da3b426f57d31c8f1bd4c36685b51aa673b632c18b"),
		new("item", "industrial_household_furniture_storage_0018_ventilated_bench", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8329d5b8220aaea433deafa4d2347ac46bfef2e86d20aaa72512b4670f218c2f"),
		new("item", "industrial_logistics_retail_hospitality_0032_portable_crate", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "7838fd14dff1e3e1c381e7079955a73802777a02e0213ce1c31085df81824152"),
		new("item", "industrial_logistics_retail_hospitality_0035_portable_counter", "industrial", ["industrial"], ["item:industrial_logistics_retail_hospitality_0001_compact_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "be2562f189b55b4caf62088505958aa022d27aa6293eb0707ddc67f286f4ac9a"),
		new("item", "industrial_medical_mobility_0013_precision_splint", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4e9adceff7832bea05937e0f6dc661135429822a0a867707d42a066550d9b950"),
		new("item", "industrial_medical_mobility_0016_precision_syringe", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c2615b704e33841a57c73f02e1024b44a26452da719ae0eedf774befbae849e7"),
		new("item", "industrial_office_school_mail_payment_0001_travel_folder", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8534f38e5e8e4b66f4f7c44f38b9cbd0a7f1294015145bea9dc553fef51ea77b"),
		new("item", "industrial_office_school_mail_payment_0002_travel_stamp", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "3e9ff861054535ebe26dc8f012223fdc2b40df9cf2cafbdf47348cd8d04b4a10"),
		new("item", "industrial_office_school_mail_payment_0020_utility_register", "industrial", ["industrial"], ["item:industrial_office_school_mail_payment_0001_travel_folder"], ItemSeederOwnershipPolicy.RequiredRelationship, "1ca2e520efa47d879f3e54978bfd896686fce455da7d5cadb1a33ff57a45061c"),
		new("item", "industrial_office_school_mail_payment_0021_plain_wastepaper", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "dbc8480816be900b124edc22523c687351855d0206fc769807e17cdb204b3cb7"),
		new("item", "industrial_office_school_mail_payment_0033_reinforced_wastepaper", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "eb9151a1a74498e53bdd8b8b85814b2f8527e3e259468e1d74c802fa6503e4f1"),
		new("item", "industrial_ppe_ballistic_0015_countertop_helmet", "industrial", ["industrial"], ["item:industrial_ppe_ballistic_0001_field_liner"], ItemSeederOwnershipPolicy.RequiredRelationship, "73936dadf8a14a790539afe2cf4d519ee25ad6a393ecab336cb3cdd3ab9540e0"),
		new("item", "industrial_raw_materials_chemicals_waste_0008_stackable_offcut", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "646cbab8a168a4418e198be42e93bc58de0a0f04cd24198d62f7f51272ed6f7b"),
		new("item", "industrial_raw_materials_chemicals_waste_0025_countertop_rod", "industrial", ["industrial"], ["item:industrial_raw_materials_chemicals_waste_0001_stackable_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "276d937d808ee71897167b52a1d3db818b0e6ee8cc60cdb45a344266e7c36f46"),
		new("item", "industrial_science_education_0002_stackable_specimen", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "c4b14b846132d927ca25306be71d493f1367a0db9d4ea50efd70ae9982eb109a"),
		new("item", "industrial_science_education_0015_field_beaker", "industrial", ["industrial"], ["item:industrial_science_education_0001_stackable_waste"], ItemSeederOwnershipPolicy.RequiredRelationship, "9f9a8718e4c682e4fe1d11d47a67dc5838ed39261cdb456f9d0dd9caa70dcb49"),
		new("item", "industrial_telecom_media_photography_0003_compact_receiver", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "d328174c116227dfa7b14c9b81a1c7ff9f973f5967533062b588c693801a7522"),
		new("item", "industrial_telecom_media_photography_0011_heavy_duty_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "27d26f93d6e1f6d7b1fdfa96a4d16179d32f7bcfee8da8d03bb9d3379c182fdd"),
		new("item", "industrial_telecom_media_photography_0013_heavy_duty_receiver", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "ba10d515633f135ca59884713ca70631b7b39f4d664b1ea7e0e2a1ff4f52b494"),
		new("item", "industrial_tools_repair_machinery_0005_lined_anvil", "industrial", ["industrial"], ["item:industrial_tools_repair_machinery_0001_lined_plane"], ItemSeederOwnershipPolicy.RequiredRelationship, "b44fa4ed3d60014594364fdce3c56ffc615cd5a0f0403bf37c6675aea17b8acf"),
		new("item", "industrial_transport_support_spares_0001_reinforced_scrap", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "a18f0db973eb50f0d45e964d24d81540e12ea9df6395b56066cf8f383b26d757"),
		new("item", "industrial_transport_support_spares_0009_reinforced_chain", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "5a69da4111a3c2e0a07e493c1d04acfec7d40ab7e7256bc536d242785edf914b"),
		new("item", "industrial_transport_support_spares_0017_compact_pump", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "074f8dfcea31e733780255d575772691fed06d7fcb4f2c9205d0d9f5f17804a4"),
		new("item", "industrial_transport_support_spares_0022_heavy_duty_wheel", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "4deb53ac53774609d5ea88483595551720c6a5b4921836b0b359a20cb02b62bf"),
		new("item", "industrial_transport_support_spares_0030_heavy_duty_tyre", "industrial", ["industrial"], ["item:industrial_transport_support_spares_0001_reinforced_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "caf12d71454094df3bae8cf69ed084a5b7d4b40038fefbe329ae2a109b1d5f52"),
		new("item", "industrial_weapons_police_military_0009_institutional_pouch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8c1145dd767a617e19bbdd35d49fa33a04c854d073ae4666f730fee077d6d429"),
		new("item", "industrial_weapons_police_military_0019_weatherproof_pouch", "industrial", ["industrial"], [], ItemSeederOwnershipPolicy.StockAggregate, "8e5457420b9a051c389da2a3b89782645cbd4b60ef5ba4aea17070bc34718e8a"),
		new("item", "industrial_weapons_police_military_0025_service_cartridge", "industrial", ["industrial"], ["item:industrial_weapons_police_military_0001_institutional_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "c7ded1ea344d045c76cb3dffbdd6e5a5366966022b516ef890b3b58f86758440"),
		new("item", "industrialised_agriculture_forestry_fishing_0008_plain_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "6c375ae9d07e65907cca2b6b86c4ecf664ee7f88d63b14e4590cadb7326b4412"),
		new("item", "industrialised_agriculture_forestry_fishing_0023_compact_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "5866207fa9fb42fe8bc8ec1e8f731e595feb7673eb50a01c5309d383e63ed092"),
		new("item", "industrialised_agriculture_forestry_fishing_0031_heavy_duty_scrap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "cdea83a42b5f6fc3b453ffd4ead146940764c596f4af40a6bb778bdfeefbbefa"),
		new("item", "industrialised_agriculture_forestry_fishing_0033_heavy_duty_rake", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "8940eb1dc7e1337097c1dcc58bf23c7aa77c0186476e776b40fd290db381dc67"),
		new("item", "industrialised_agriculture_forestry_fishing_0058_portable_trap", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], [], ItemSeederOwnershipPolicy.StockAggregate, "20d452cdf686cc79c207315c870cd2f10b3364837223dc45e1c9f5b735fd9c91"),
		new("item", "industrialised_agriculture_forestry_fishing_0090_weatherproof_yoke", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "f7ed6e68163e71b6a68c156b7e47b7314d32408e0cb27efe6c274c29e3a66856"),
		new("item", "industrialised_agriculture_forestry_fishing_0105_precision_spade", "shared-industrialised", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_agriculture_forestry_fishing_0001_plain_scrap"], ItemSeederOwnershipPolicy.RequiredRelationship, "fd958d8c56a37d56159689e6d374e50fbc4925df87d9327a6150e0e6c6e9c2f3"),
	];
}
