#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalWritingAdministrationAndDocuments()
	{
		#region Early Anglo-Saxon/Insular

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_early_anglo_saxon_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: wax tablets and charter strips. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Early Anglo-Saxon/Insular."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_early_anglo_saxon_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Early Anglo-Saxon/Insular slice a reusable short-record surface inspired by wax tablets and charter strips.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_early_anglo_saxon_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by wax tablets and charter strips.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_early_anglo_saxon_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of wax tablets and charter strips.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Seal-tag and office-label support."
		);

		#endregion

		#region Late Anglo-Saxon/Anglo-Danish

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_anglo_danish_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: sealed writs and reeve tallies. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Late Anglo-Saxon/Anglo-Danish."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_anglo_danish_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Late Anglo-Saxon/Anglo-Danish slice a reusable short-record surface inspired by sealed writs and reeve tallies.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_anglo_danish_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by sealed writs and reeve tallies.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_anglo_danish_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of sealed writs and reeve tallies.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Seal-tag and office-label support."
		);

		#endregion

		#region Norse

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_norse_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: runic tallies and trade tags. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Norse."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_norse_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Norse slice a reusable short-record surface inspired by runic tallies and trade tags.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_norse_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by runic tallies and trade tags.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_norse_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of runic tallies and trade tags.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Seal-tag and office-label support."
		);

		#endregion

		#region Norman/Angevin

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_norman_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: sealed charters and exchequer rolls. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Norman/Angevin."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_norman_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Norman/Angevin slice a reusable short-record surface inspired by sealed charters and exchequer rolls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_norman_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by sealed charters and exchequer rolls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_norman_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of sealed charters and exchequer rolls.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Seal-tag and office-label support."
		);

		#endregion

		#region High Medieval Britain/Marcher

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_high_british_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: seal tags and manor accounts. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: High Medieval Britain/Marcher."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_high_british_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the High Medieval Britain/Marcher slice a reusable short-record surface inspired by seal tags and manor accounts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_high_british_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by seal tags and manor accounts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_high_british_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of seal tags and manor accounts.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Seal-tag and office-label support."
		);

		#endregion

		#region Gaelic/Welsh/Highland

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_gaelic_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: memoranda strips and boundary tallies. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Gaelic/Welsh/Highland."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_gaelic_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Gaelic/Welsh/Highland slice a reusable short-record surface inspired by memoranda strips and boundary tallies.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_gaelic_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by memoranda strips and boundary tallies.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_gaelic_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of memoranda strips and boundary tallies.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Seal-tag and office-label support."
		);

		#endregion

		#region Carolingian/Frankish

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_carolingian_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: capitularies and estate lists. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Carolingian/Frankish."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_carolingian_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Carolingian/Frankish slice a reusable short-record surface inspired by capitularies and estate lists.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_carolingian_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by capitularies and estate lists.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_carolingian_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of capitularies and estate lists.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Seal-tag and office-label support."
		);

		#endregion

		#region Capetian/Low Countries

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_capetian_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: notarial notes and sealed letters. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Capetian/Low Countries."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_capetian_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Capetian/Low Countries slice a reusable short-record surface inspired by notarial notes and sealed letters.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_capetian_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by notarial notes and sealed letters.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_capetian_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of notarial notes and sealed letters.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Seal-tag and office-label support."
		);

		#endregion

		#region German/HRE/Alpine-North Italian

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_german_hre_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: guild marks and court seals. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: German/HRE/Alpine-North Italian."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_german_hre_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the German/HRE/Alpine-North Italian slice a reusable short-record surface inspired by guild marks and court seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_german_hre_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by guild marks and court seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_german_hre_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of guild marks and court seals.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Seal-tag and office-label support."
		);

		#endregion

		#region Iberian Christian

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_iberian_christian_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: frontier charters and seal cords. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Iberian Christian."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_iberian_christian_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Iberian Christian slice a reusable short-record surface inspired by frontier charters and seal cords.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_iberian_christian_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by frontier charters and seal cords.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_iberian_christian_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of frontier charters and seal cords.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Seal-tag and office-label support."
		);

		#endregion

		#region al-Andalus/Maghreb

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_andalusi_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: paper contracts and office seals. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: al-Andalus/Maghreb."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_andalusi_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the al-Andalus/Maghreb slice a reusable short-record surface inspired by paper contracts and office seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_andalusi_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by paper contracts and office seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_andalusi_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of paper contracts and office seals.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Seal-tag and office-label support."
		);

		#endregion

		#region Byzantine

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_byzantine_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: chrysobull copies and sealed packets. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Byzantine."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_byzantine_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Byzantine slice a reusable short-record surface inspired by chrysobull copies and sealed packets.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_byzantine_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by chrysobull copies and sealed packets.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_byzantine_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of chrysobull copies and sealed packets.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Seal-tag and office-label support."
		);

		#endregion

		#region Abbasid/Persianate

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_abbasid_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: paper decrees and chancery seals. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Abbasid/Persianate."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_abbasid_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Abbasid/Persianate slice a reusable short-record surface inspired by paper decrees and chancery seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_abbasid_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by paper decrees and chancery seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_abbasid_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of paper decrees and chancery seals.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Seal-tag and office-label support."
		);

		#endregion

		#region Fatimid Egypt/Ifriqiya

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_fatimid_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: tax rolls and sealed paper orders. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Fatimid Egypt/Ifriqiya."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_fatimid_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Fatimid Egypt/Ifriqiya slice a reusable short-record surface inspired by tax rolls and sealed paper orders.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_fatimid_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by tax rolls and sealed paper orders.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_fatimid_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of tax rolls and sealed paper orders.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Seal-tag and office-label support."
		);

		#endregion

		#region Seljuk/Ayyubid/early Mamluk

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_seljuk_ayyubid_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: iqta records and sealed orders. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Seljuk/Ayyubid/early Mamluk."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_seljuk_ayyubid_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Seljuk/Ayyubid/early Mamluk slice a reusable short-record surface inspired by iqta records and sealed orders.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_seljuk_ayyubid_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by iqta records and sealed orders.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_seljuk_ayyubid_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of iqta records and sealed orders.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Seal-tag and office-label support."
		);

		#endregion

		#region Kyivan Rus/Novgorod

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_rus_novgorod_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: birchbark notes and princely seals. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Kyivan Rus/Novgorod."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_rus_novgorod_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Kyivan Rus/Novgorod slice a reusable short-record surface inspired by birchbark notes and princely seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_rus_novgorod_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by birchbark notes and princely seals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_rus_novgorod_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of birchbark notes and princely seals.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Seal-tag and office-label support."
		);

		#endregion

		#region Steppe Turkic/Cuman/Mongol-adjacent

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_steppe_turkic_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: paiza tags and sealed pouches. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_steppe_turkic_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Steppe Turkic/Cuman/Mongol-adjacent slice a reusable short-record surface inspired by paiza tags and sealed pouches.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_steppe_turkic_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by paiza tags and sealed pouches.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_steppe_turkic_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of paiza tags and sealed pouches.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Seal-tag and office-label support."
		);

		#endregion

		#region Song China

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_song_china_office_bundle",
			"bundle",
			"a sealed administrative document bundle",
			null,
			"This tied bundle represents regional administrative practice for builders: paper registers and official chops. It has prepared sealing points and a writable surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			20.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder. Medieval culture slice: Song China."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_song_china_record_tablet",
			"tablet",
			"a regional record tablet",
			null,
			"This tablet gives the Song China slice a reusable short-record surface inspired by paper registers and official chops.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Regional short-record surface."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_song_china_tally_bundle",
			"bundle",
			"a regional tally bundle",
			null,
			"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by paper registers and official chops.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Tally and account prop."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_song_china_seal_tag_packet",
			"packet",
			"a regional seal-tag packet",
			null,
			"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of paper registers and official chops.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Seal-tag and office-label support."
		);

		#endregion

		#region Common Writing and Administration

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_parchment_charter",
			"charter",
			"a sealable parchment charter",
			null,
			"This blank parchment charter has a folded foot, room for witness marks, and a prepared tag for wax sealing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			60.0,
			16.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_sealable_envelope",
			"envelope",
			"a sealable parchment envelope",
			null,
			"This folded parchment envelope has a closable flap, a small address face, and a prepared seal spot.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			24.0,
			6.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Envelope",
				"PaperSheet_Envelope",
				"Sealable_Envelope",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_office_signet_ring",
			"ring",
			"an office signet ring",
			null,
			"This signet ring has a raised face cut for official impressions and enough wear to suggest regular chancery or household use.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			28.0,
			60.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Wear_Ring",
				"SealStamp_Antiquity_BronzeSignet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_office_seal_matrix",
			"matrix",
			"a bronze office seal matrix",
			null,
			"This bronze seal matrix has a flat engraved face, a small loop, and a plain grip for pressing wax seals on charters and packets.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			120.0,
			80.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"SealStamp_Antiquity_BronzeSignet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("beeswax", MaterialBehaviourType.Wax,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Ink",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_wax_seal_cake",
			"cake",
			"a cake of red sealing wax",
			null,
			"This small cake of sealing wax is wrapped in cloth and ready to melt for charters, envelopes, bales, or chests.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			70.0,
			5.0m,
			false,
			false,
			"beeswax",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Ink",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("bone", MaterialBehaviourType.Bone,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			]
		);

		CreateItem(
			"medieval_writing_quill_pen",
			"pen",
			"a trimmed quill pen",
			null,
			"This trimmed quill pen is cut for ink writing and suited to chancery, monastic, scholastic, mercantile, or household records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			14.0,
			5.0m,
			false,
			false,
			"bone",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			]
		);

		CreateItem(
			"medieval_writing_reed_pen",
			"pen",
			"a cut reed pen",
			null,
			"This cut reed pen has a shaped nib and hollow body for paper, parchment, wax-tablet notes, or practice writing.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			3.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("bone", MaterialBehaviourType.Bone,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			]
		);

		CreateItem(
			"medieval_writing_ink_horn",
			"horn",
			"a small ink horn",
			null,
			"This small ink horn is stoppered and corded for carrying black ink, coloured ink, or scribe-mixed writing fluid.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			10.0m,
			false,
			false,
			"bone",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_wax_tablet",
			"tablet",
			"a wooden wax tablet",
			null,
			"This wooden tablet has a shallow waxed face for temporary notes, school exercises, accounts, passwords, or message drafts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			9.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Paper",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_paper_sheet",
			"sheet",
			"a loose paper sheet",
			null,
			"This loose paper sheet is sized for letters, contracts, copies, notes, accounts, or chancery drafts.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			4.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Paper",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_parchment_quire",
			"quire",
			"a folded parchment quire",
			null,
			"This folded parchment quire is pricked near the spine and ready for binding, record copying, scholarship, liturgy, or legal archives.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			22.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Books",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_bound_codex",
			"codex",
			"a small bound codex",
			null,
			"This small codex has stiff boards, sewn gatherings, and room for devotional, scholastic, legal, merchant, or household text.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			80.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Books",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Book_Small_40_Page",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_account_roll",
			"roll",
			"a sealed account roll",
			null,
			"This account roll is tied with cord, labelled on the outside, and prepared to take a wax seal for audit or archive work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			18.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"PaperSheet_Scroll",
				"Sealable_Document_Wax",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			]
		);

		CreateItem(
			"medieval_writing_tally_sticks",
			"tallies",
			"a split tally stick set",
			null,
			"This split tally set has matching notched pieces for debt, rent, tax, custody, delivery, or market-account scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			6.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("parchment", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_charter_tag_set",
			"tags",
			"a charter tag set",
			null,
			"This tag set bundles parchment tongues, cords, and prepared loops for attaching seals to charters and legal packets.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			7.0m,
			false,
			false,
			"parchment",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Parchment",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_seal_cord_bundle",
			"cord",
			"a bundle of seal cord",
			null,
			"This cord bundle is cut and waxed for tying scrolls, charters, packets, trade bales, and labelled account bundles.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			60.0,
			5.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Market / Writing Materials",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval writing, sealing, and administrative item."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_document_satchel",
			"satchel",
			"a sealable document satchel",
			null,
			"This leather satchel has a shoulder strap, internal sleeve, and a closing flap prepared for tamper-evident sealing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			38.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Tote",
				"Wear_Shoulder",
				"Sealable_Container_Wax",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_ledger_chest",
			"chest",
			"a sealable ledger chest",
			null,
			"This narrow chest is sized for ledgers, rolls, tallies, seal matrices, wax cakes, and locked office papers.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			68.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Writing Materials / Document Containers",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Trunk",
				"Sealable_Container_Wax",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("leather", MaterialBehaviourType.Leather,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_notary_kit",
			"kit",
			"a notary's sealing kit",
			null,
			"This kit holds a small seal, wax, cord, folded sheets, tally slips, and a pouch of witness tags for legal or mercantile work.",
			SizeCategory.Small,
			ItemQuality.Good,
			1600.0,
			95.0m,
			false,
			false,
			"leather",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"SealStamp_Antiquity_BronzeSignet",
				"Sealable_Container_Wax",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_writing_guild_stamp",
			"stamp",
			"a guild seal stamp",
			null,
			"This small stamp has a plain handle and engraved face for guild, workshop, office, or household authority marks.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			140.0,
			64.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"SealStamp_Antiquity_BronzeSignet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_trade_balance_scale",
			"scale",
			"a merchant balance scale",
			null,
			"This portable balance scale has paired pans, a folding beam, and cords suitable for table, market, or customs use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			42.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"MeasuringInstrument_Antiquity_BalanceScale",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_trade_standard_weight_set",
			"weights",
			"a standard weight set",
			null,
			"This set of stamped weights nests into a pouch, marked for checking fair trade at market, mill, mint, or customs gate.",
			SizeCategory.Small,
			ItemQuality.Good,
			3000.0,
			65.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"MeasuringInstrument_Antiquity_StandardWeights",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_trade_false_weight_set",
			"weights",
			"a false weight set",
			null,
			"This false weight set is outwardly respectable, but its pieces are subtly biased for dishonest measuring.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2900.0,
			45.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"MeasuringInstrument_Antiquity_FalseWeights",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_trade_grain_measure",
			"measure",
			"a wooden grain measure",
			null,
			"This wooden grain measure has a level rim, a reinforced base, and calibration marks for granary, mill, or tithe use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			22.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"MeasuringInstrument_Antiquity_GrainMeasure",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_trade_tax_customs_kit",
			"kit",
			"a tax and customs measuring kit",
			null,
			"This kit bundles a folding scale, nested weights, cord tags, tally slips, and sealing wax for inspecting taxable goods.",
			SizeCategory.Small,
			ItemQuality.Good,
			5200.0,
			120.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"MeasuringInstrument_Antiquity_TaxAssessorKit",
				"Sealable_Container_Wax",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			]
		);

		CreateItem(
			"medieval_trade_sealable_bale",
			"bale",
			"a sealable cloth trade bale",
			null,
			"This trade bale is wrapped in stout cloth, tied with cord, and prepared to take a wax or clay seal as customs evidence.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			35.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Market / Household Goods / Standard Wares",
				"Functions / Security Tools"
			],
			[
				"Holdable",
				"Container_Trunk",
				"Sealable_Container_Wax",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("hemp", MaterialBehaviourType.Plant,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_surveyor_measuring_rope",
			"rope",
			"a knotted measuring rope",
			null,
			"This knotted rope is marked for field and building measures, but remains a prop until length and surveying measurement modes exist.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			12.0m,
			false,
			false,
			"hemp",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Seeded as a prop because richer runtime support is deferred to a future engine component."
		);

		#endregion
	}
}
