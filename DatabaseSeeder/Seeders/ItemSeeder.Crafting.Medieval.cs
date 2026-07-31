#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string MedievalIndustryKnowledge = "Medieval Industry Foundations";
	private const string HistoricFoundationSource = "historic_foundation";
	private const string PrimaryProductionSource = "primary_production";
	private const string MedievalCraftedSource = "medieval_crafted";
	private const string UpstreamSourceExempt = "upstream_source_exempt";

	internal sealed record MedievalProductionDependencyTestData(
		string Contract,
		string? StableReference,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	internal sealed record MedievalProductionCraftSpecTestData(
		int Phase,
		string Name,
		string Category,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		IReadOnlyCollection<string> Inputs,
		IReadOnlyCollection<string> Tools,
		IReadOnlyCollection<string> Products,
		IReadOnlyCollection<MedievalProductionDependencyTestData> Dependencies,
		IReadOnlyCollection<string> SourceOwnership);

	private sealed record MedievalProductionInput(
		string Import,
		string? StableReference,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	private sealed record MedievalProductionTool(
		string Tag,
		string Location,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	private sealed record MedievalProductionCraftSpec(
		int Phase,
		string Name,
		string Category,
		string Blurb,
		string Action,
		string ActiveDescription,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		IReadOnlyList<MedievalProductionInput> Inputs,
		IReadOnlyList<MedievalProductionTool> Tools,
		string OutputStableReference,
		string? ReturnedInputStableReference = null);

	internal static IReadOnlyCollection<MedievalProductionCraftSpecTestData> MedievalProductionCraftSpecsForTesting =>
		MedievalProductionCraftSpecs()
			.Select(x =>
			{
				var dependencies = x.Inputs
					.Select(input => new MedievalProductionDependencyTestData(
						input.Import,
						input.StableReference,
						input.SourceStatus,
						input.SourceOwner,
						input.SourcePhase))
					.Concat(x.Tools.Select(tool => new MedievalProductionDependencyTestData(
						$"TagTool - {tool.Location} - an item with the {tool.Tag} tag",
						null,
						tool.SourceStatus,
						tool.SourceOwner,
						tool.SourcePhase)))
					.ToArray();
				return new MedievalProductionCraftSpecTestData(
					x.Phase,
					x.Name,
					x.Category,
					x.Trait,
					x.MinimumTraitValue,
					x.Difficulty,
					x.KnowledgeSubtype,
					x.Inputs.Select(InputContractForTesting).ToArray(),
					x.Tools.Select(ToolImport).ToArray(),
					[x.OutputStableReference],
					dependencies,
					dependencies
						.Select(y => y.SourceStatus)
						.Append(MedievalCraftedSource)
						.Distinct(StringComparer.Ordinal)
						.ToArray());
			})
			.ToArray();

	internal static bool ShouldSeedMedievalCraftsForTesting(string? eras)
	{
		return !string.IsNullOrWhiteSpace(eras) &&
		       eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase);
	}

	internal void SeedMedievalProductionChainCraftsForTesting(FuturemudDatabaseContext context)
	{
		InitialiseCraftAuthoringForTesting(context);
		_questionAnswers = new Dictionary<string, string>
		{
			["eras"] = "medieval"
		};
		SeedMedievalProductionChainCrafts();
	}

	private bool ShouldSeedMedievalCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       ShouldSeedMedievalCraftsForTesting(eras);
	}

	private void SeedMedievalProductionChainCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		foreach (var spec in MedievalProductionCraftSpecs().OrderBy(x => x.Phase))
		{
			AddMedievalProductionCraft(spec);
		}
	}

	private Craft? AddMedievalProductionCraft(MedievalProductionCraftSpec spec)
	{
		var inputs = spec.Inputs
			.Select(x => x.StableReference is null ? x.Import : StableSimpleItemInput(x.StableReference))
			.ToArray();
		var failProducts = spec.ReturnedInputStableReference is null
			? []
			: new[] { StableUnusedInputProduct(spec.ReturnedInputStableReference, 1) };

		return AddCraft(
			spec.Name,
			spec.Category,
			spec.Blurb,
			spec.Action,
			spec.ActiveDescription,
			MedievalIndustryKnowledge,
			spec.Trait,
			spec.MinimumTraitValue,
			spec.Difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			spec.Phase == 4 ? MedievalIndustryLightingPhases() : MedievalIndustryCraftingPhases(),
			inputs,
			spec.Tools.Select(ToolImport),
			[StableSimpleProduct(spec.OutputStableReference)],
			failProducts,
			knowledgeSubtype: spec.KnowledgeSubtype,
			knowledgeDescription: "Foundational medieval stock preparation, toolmaking, and workshop construction.",
			knowledgeLongDescription: "Practical medieval industry knowledge for turning shared historic and primary-production resources into first-tier stock, portable tools, workshop apparatus, and active high-heat work sites.");
	}

	private static IReadOnlyList<MedievalProductionCraftSpec> MedievalProductionCraftSpecs()
	{
		return
		[
			Stock(
				"prepare worked plank bundle",
				"Timber Stock",
				"Carpentry",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_plank_bundle",
				[Commodity(12000.0, "oak", "Furniture Timber Stock", PrimaryProductionSource, "Primary Production forestry")],
				[HistoricTool("Hammer"), HistoricTool("Awl Punch")]),
			Stock(
				"shape handle blanks",
				"Timber Stock",
				"Carpentry",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_handle_blanks",
				[Commodity(900.0, "ash", "Tool Blank Stock", PrimaryProductionSource, "Primary Production forestry")],
				[HistoricTool("Hammer"), HistoricTool("Awl Punch")]),
			Stock(
				"sort wrought iron bar stock",
				"Metal Stock",
				"Blacksmithing",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_iron_bar",
				[Commodity(2400.0, "wrought iron", "Metal Bar Stock Commodity", PrimaryProductionSource, "Primary Production ironworking")],
				[PrimaryTool("Smelting Tool"), HistoricTool("Forge Tongs")]),
			Stock(
				"cast bronze bar stock",
				"Metal Stock",
				"Smelting",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_bronze_bar",
				[Commodity(1600.0, "bronze", "Metal Billet Commodity", PrimaryProductionSource, "Primary Production alloying")],
				[PrimaryRoomTool("Smelting Tool"), HistoricTool("Forge Tongs")]),
			Stock(
				"draw bronze wire coil",
				"Metal Stock",
				"Blacksmithing",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_wire_coil",
				[Commodity(500.0, "bronze", "Metal Billet Commodity", PrimaryProductionSource, "Primary Production alloying")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			Stock(
				"forge packet of iron rivets",
				"Metal Stock",
				"Blacksmithing",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_rivet_packet",
				[Commodity(600.0, "wrought iron", "Metal Bar Stock Commodity", PrimaryProductionSource, "Primary Production ironworking")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			Stock(
				"knead prepared clay body",
				"Clay and Building Stock",
				"Pottery",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_clay_body_lump",
				[Commodity(4200.0, "prepared clay", "Prepared Clay Commodity", PrimaryProductionSource, "Primary Production clay preparation")],
				[PrimaryTool("Kiln Tool")]),
			Stock(
				"stack fired bricks",
				"Clay and Building Stock",
				"Pottery",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_fired_brick_stack",
				[Commodity(28000.0, "fired brick", "Fired Brick Commodity", PrimaryProductionSource, "Primary Production brick firing")],
				[PrimaryTool("Hauling Tool")]),
			Stock(
				"prepare leather panel",
				"Leather and Parchment Stock",
				"Leathermaking",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_leather_panel",
				[Commodity(1200.0, "leather", "Prepared Leather Panel", UpstreamSourceExempt, "Butchery and leather preparation")],
				[HistoricTool("Awl Punch"), HistoricTool("Shears")]),
			Stock(
				"prepare parchment sheet",
				"Leather and Parchment Stock",
				"Parchmentmaking",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_parchment_sheet",
				[Commodity(30.0, "parchment", "Parchment Sheet Stock", UpstreamSourceExempt, "Butchery and parchment preparation")],
				[HistoricRoomTool("Tanning Rack"), HistoricTool("Awl Punch")]),
			Stock(
				"spin wool yarn skein",
				"Textile Stock",
				"Spinning",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_yarn_skein",
				[Commodity(280.0, "wool", "Raw Textile Fibre", UpstreamSourceExempt, "Agriculture and pastoral production")],
				[HistoricTool("Drop Spindle")]),
			Stock(
				"spin linen sewing thread",
				"Textile Stock",
				"Spinning",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_sewing_thread",
				[Commodity(70.0, "linen", "Prepared Textile Fibre", UpstreamSourceExempt, "Agriculture and textile preparation")],
				[HistoricTool("Drop Spindle")]),
			Stock(
				"weave plain wool cloth bolt",
				"Textile Stock",
				"Weaving",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_plain_cloth_bolt",
				[Commodity(4500.0, "wool", "Spun Yarn", UpstreamSourceExempt, "Agriculture and pastoral production")],
				[HistoricRoomTool("Loom")]),
			Stock(
				"render hide glue cake",
				"Workshop Supplies",
				"Leathermaking",
				15,
				Difficulty.Normal,
				"medieval_industry_stock_glue_cake",
				[Commodity(500.0, "bone", null, UpstreamSourceExempt, "Butchery")],
				[HistoricRoomTool("Hot Fire")]),
			Stock(
				"form sealing wax stick",
				"Workshop Supplies",
				"Candlemaking",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_seal_wax_stick",
				[Commodity(45.0, "beeswax", null, UpstreamSourceExempt, "Apiary production")],
				[HistoricRoomTool("Hot Fire")]),
			Stock(
				"cut clean bandage roll",
				"Medical Stock",
				"Tailoring",
				10,
				Difficulty.Easy,
				"medieval_industry_stock_bandage_roll",
				[Commodity(200.0, "linen", "Woven Cloth", UpstreamSourceExempt, "Agriculture and textile preparation")],
				[HistoricTool("Shears")]),

			ToolCraft(
				2,
				"forge iron felling axe",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_felling_axe",
				[MedievalInput("medieval_industry_stock_iron_bar"), MedievalInput("medieval_industry_stock_handle_blanks")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"forge iron hand saw",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_hand_saw",
				[
					MedievalInput("medieval_industry_stock_iron_bar"),
					MedievalInput("medieval_industry_stock_handle_blanks"),
					MedievalInput("medieval_industry_stock_rivet_packet")
				],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"forge iron wood chisel",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_wood_chisel",
				[MedievalInput("medieval_industry_stock_iron_bar"), MedievalInput("medieval_industry_stock_handle_blanks")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"forge iron wood auger",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_wood_auger",
				[MedievalInput("medieval_industry_stock_iron_bar"), MedievalInput("medieval_industry_stock_handle_blanks")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"forge iron drawknife",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_drawknife",
				[MedievalInput("medieval_industry_stock_iron_bar"), MedievalInput("medieval_industry_stock_handle_blanks")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"forge iron hide scraper",
				"Basic Tools",
				"Blacksmithing",
				20,
				Difficulty.Normal,
				"medieval_tool_hide_scraper",
				[MedievalInput("medieval_industry_stock_iron_bar"), MedievalInput("medieval_industry_stock_handle_blanks")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				2,
				"build sloped tanning beam",
				"Basic Tools",
				"Carpentry",
				20,
				Difficulty.Normal,
				"medieval_tool_tanning_beam",
				[MedievalInput("medieval_industry_stock_plank_bundle")],
				[HistoricTool("Hammer"), HistoricTool("Awl Punch")]),
			ToolCraft(
				2,
				"build smithing forge",
				"Workshop Apparatus",
				"Masonry",
				25,
				Difficulty.Hard,
				"medieval_workshop_forge",
				[
					MedievalInput("medieval_industry_stock_clay_body_lump"),
					MedievalInput("medieval_industry_stock_fired_brick_stack"),
					MedievalInput("medieval_industry_stock_iron_bar")
				],
				[PrimaryTool("Masonry Tool"), PrimaryTool("Kiln Tool")]),
			ToolCraft(
				2,
				"build bloomery smelting furnace",
				"Workshop Apparatus",
				"Masonry",
				25,
				Difficulty.Hard,
				"medieval_workshop_smelting_furnace",
				[
					MedievalInput("medieval_industry_stock_clay_body_lump"),
					MedievalInput("medieval_industry_stock_fired_brick_stack"),
					MedievalInput("medieval_industry_stock_iron_bar")
				],
				[PrimaryTool("Masonry Tool"), PrimaryTool("Kiln Tool")]),
			ToolCraft(
				2,
				"fire ceramic crucible",
				"Workshop Apparatus",
				"Pottery",
				25,
				Difficulty.Hard,
				"medieval_tool_crucible",
				[MedievalInput("medieval_industry_stock_clay_body_lump")],
				[PrimaryRoomTool("Kiln Tool")]),
			ToolCraft(
				2,
				"build treadled grindstone",
				"Workshop Apparatus",
				"Masonry",
				25,
				Difficulty.Hard,
				"medieval_tool_grindstone",
				[
					MedievalInput("medieval_industry_stock_plank_bundle"),
					MedievalInput("medieval_industry_stock_iron_bar"),
					TaggedCommodity(28000.0, "Dressed Stone Block Commodity", PrimaryProductionSource, "Primary Production quarrying")
				],
				[PrimaryTool("Masonry Tool"), HistoricTool("Hammer")]),

			ToolCraft(
				3,
				"build parchment stretching frame",
				"Specialist Tools",
				"Parchmentmaking",
				25,
				Difficulty.Hard,
				"medieval_tool_parchment_stretching_frame",
				[
					MedievalInput("medieval_industry_stock_plank_bundle"),
					MedievalInput("medieval_industry_stock_sewing_thread")
				],
				[MedievalTool("Wood Chisel", 2), MedievalTool("Wood Auger", 2)]),
			ToolCraft(
				3,
				"build papermaking mould and deckle",
				"Specialist Tools",
				"Papermaking",
				25,
				Difficulty.Hard,
				"medieval_tool_mould_and_deckle",
				[
					MedievalInput("medieval_industry_stock_handle_blanks"),
					MedievalInput("medieval_industry_stock_wire_coil")
				],
				[MedievalTool("Wood Chisel", 2), MedievalTool("Wood Auger", 2)]),
			ToolCraft(
				3,
				"build wooden book press",
				"Specialist Tools",
				"Bookbinding",
				25,
				Difficulty.Hard,
				"medieval_workshop_book_press",
				[
					MedievalInput("medieval_industry_stock_plank_bundle"),
					MedievalInput("medieval_industry_stock_iron_bar"),
					MedievalInput("medieval_industry_stock_glue_cake")
				],
				[MedievalTool("Wood Chisel", 2), MedievalTool("Wood Auger", 2), HistoricTool("Hammer")]),
			ToolCraft(
				3,
				"carve stone mortar and pestle",
				"Specialist Tools",
				"Masonry",
				25,
				Difficulty.Hard,
				"medieval_tool_mortar_and_pestle",
				[TaggedCommodity(4000.0, "Dressed Stone Block Commodity", PrimaryProductionSource, "Primary Production quarrying")],
				[PrimaryTool("Masonry Tool")]),
			ToolCraft(
				3,
				"forge curved suture needle",
				"Medical Tools",
				"Blacksmithing",
				25,
				Difficulty.Hard,
				"medieval_tool_suture_needle",
				[MedievalInput("medieval_industry_stock_iron_bar")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),
			ToolCraft(
				3,
				"forge bronze surgical probe",
				"Medical Tools",
				"Goldsmithing",
				25,
				Difficulty.Hard,
				"medieval_tool_surgical_probe",
				[MedievalInput("medieval_industry_stock_bronze_bar")],
				[HistoricRoomTool("Anvil"), HistoricTool("Hammer"), HistoricTool("Forge Tongs")]),

			Lighting(
				"light smithing forge",
				"Blacksmithing",
				"medieval_workshop_forge",
				"medieval_workshop_lit_forge",
				[PrimaryTool("Charcoal Burning Tool")]),
			Lighting(
				"light bloomery smelting furnace",
				"Smelting",
				"medieval_workshop_smelting_furnace",
				"medieval_workshop_lit_smelting_furnace",
				[PrimaryTool("Kiln Tool"), HistoricTool("Forge Tongs")])
		];
	}

	private static MedievalProductionCraftSpec Stock(
		string action,
		string knowledgeSubtype,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string outputStableReference,
		IReadOnlyList<MedievalProductionInput> inputs,
		IReadOnlyList<MedievalProductionTool> tools)
	{
		return Define(
			1,
			action,
			"Medieval Industry / Stock",
			trait,
			minimumTraitValue,
			difficulty,
			knowledgeSubtype,
			outputStableReference,
			inputs,
			tools);
	}

	private static MedievalProductionCraftSpec ToolCraft(
		int phase,
		string action,
		string knowledgeSubtype,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string outputStableReference,
		IReadOnlyList<MedievalProductionInput> inputs,
		IReadOnlyList<MedievalProductionTool> tools)
	{
		var category = knowledgeSubtype.Contains("Apparatus", StringComparison.Ordinal)
			? "Medieval Industry / Workshop Apparatus"
			: "Medieval Industry / Toolmaking";
		return Define(
			phase,
			action,
			category,
			trait,
			minimumTraitValue,
			difficulty,
			knowledgeSubtype,
			outputStableReference,
			inputs,
			tools);
	}

	private static MedievalProductionCraftSpec Lighting(
		string action,
		string trait,
		string inputStableReference,
		string outputStableReference,
		IReadOnlyList<MedievalProductionTool> tools)
	{
		return Define(
			4,
			action,
			"Medieval Industry / Lighting",
			trait,
			10,
			Difficulty.Easy,
			"Lighting",
			outputStableReference,
			[
				new MedievalProductionInput(
					string.Empty,
					inputStableReference,
					MedievalCraftedSource,
					"Medieval Industry Foundations",
					2),
				Commodity(2500.0, "charcoal", "Charcoal Fuel Commodity", PrimaryProductionSource, "Primary Production charcoal burning")
			],
			tools,
			inputStableReference);
	}

	private static MedievalProductionCraftSpec Define(
		int phase,
		string action,
		string category,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string knowledgeSubtype,
		string outputStableReference,
		IReadOnlyList<MedievalProductionInput> inputs,
		IReadOnlyList<MedievalProductionTool> tools,
		string? returnedInputStableReference = null)
	{
		var name = $"medieval industry - {action}";
		return new MedievalProductionCraftSpec(
			phase,
			name,
			category,
			action,
			action,
			$"{action} in progress",
			trait,
			minimumTraitValue,
			difficulty,
			knowledgeSubtype,
			inputs,
			tools,
			outputStableReference,
			returnedInputStableReference);
	}

	private static MedievalProductionInput Commodity(
		double grams,
		string material,
		string? pileTag,
		string sourceStatus,
		string sourceOwner)
	{
		return new MedievalProductionInput(
			CommodityInput(grams, material, pileTag),
			null,
			sourceStatus,
			sourceOwner,
			0);
	}

	private static MedievalProductionInput MedievalInput(string stableReference)
	{
		return new MedievalProductionInput(
			string.Empty,
			stableReference,
			MedievalCraftedSource,
			"Medieval Industry Foundations",
			1);
	}

	private static MedievalProductionInput TaggedCommodity(
		double grams,
		string pileTag,
		string sourceStatus,
		string sourceOwner)
	{
		return new MedievalProductionInput(
			CommodityPileInput(grams, pileTag),
			null,
			sourceStatus,
			sourceOwner,
			0);
	}

	private static MedievalProductionTool HistoricTool(string tag)
	{
		return new MedievalProductionTool(tag, "Held", HistoricFoundationSource, "Historic Workshop Foundations", 0);
	}

	private static MedievalProductionTool HistoricRoomTool(string tag)
	{
		return new MedievalProductionTool(tag, "InRoom", HistoricFoundationSource, "Historic Workshop Foundations", 0);
	}

	private static MedievalProductionTool PrimaryTool(string tag)
	{
		return new MedievalProductionTool(tag, "Held", PrimaryProductionSource, "Primary Production", 0);
	}

	private static MedievalProductionTool PrimaryRoomTool(string tag)
	{
		return new MedievalProductionTool(tag, "InRoom", PrimaryProductionSource, "Primary Production", 0);
	}

	private static MedievalProductionTool MedievalTool(string tag, int phase)
	{
		return new MedievalProductionTool(tag, "Held", MedievalCraftedSource, "Medieval Industry Foundations", phase);
	}

	private static string ToolImport(MedievalProductionTool tool)
	{
		return $"TagTool - {tool.Location} - an item with the {tool.Tag} tag";
	}

	private static string InputContractForTesting(MedievalProductionInput input)
	{
		return input.StableReference ?? input.Import;
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalIndustryCraftingPhases()
	{
		return
		[
			(30, "$0 inspect|inspects the stock and lay|lays out the work.", "$0 inspect|inspects the stock, but overlook|overlooks flaws that will weaken the work."),
			(45, "$0 shape|shapes and fit|fits the prepared pieces with $t1.", "$0 shape|shapes the stock unevenly and struggle|struggles to fit the pieces."),
			(45, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.")
		];
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalIndustryLightingPhases()
	{
		return
		[
			(20, "$0 prepare|prepares $i1 with a measured charcoal charge.", "$0 prepare|prepares $i1 poorly and scatter|scatters part of the charge."),
			(30, "$0 coax|coaxes the fuel into a steady high heat.", "$0 fail|fails to establish an even working heat."),
			(20, "$0 set|sets $p1 into its working state.", "$0 leave|leaves only $f1 after the fire fails to take.")
		];
	}

	private void SeedMedievalClothingCrafts()
	{
	}

	private void SeedMedievalEquipmentCrafts()
	{
	}

	private void SeedMedievalWritingAdministrationCrafts()
	{
	}

	private void SeedMedievalMedicalApothecaryCrafts()
	{
	}

	private void SeedMedievalJewelleryDevotionalCrafts()
	{
	}

	private void SeedMedievalFurnitureAndContainerCrafts()
	{
	}

	private void SeedMedievalRepairKitCrafts()
	{
	}

	private void SeedMedievalComponentGapCrafts()
	{
	}
}
