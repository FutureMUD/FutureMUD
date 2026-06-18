#nullable enable

using DatabaseSeeder;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public sealed class PrimaryProductionSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Stock Primary Production";

	private enum ProjectMaterialSeedType
	{
		Commodity,
		CommodityTag
	}

	private enum ProjectActionSeedType
	{
		CommodityOutput,
		ResourceDiscovery
	}

	private sealed record ProjectMaterialSeed(
		string Name,
		string Description,
		ProjectMaterialSeedType Type,
		string MaterialOrTag,
		double Amount,
		string? CommodityTag);

	private sealed record ProjectActionSeed(
		string Name,
		string Description,
		ProjectActionSeedType Type,
		string? Material,
		double Weight,
		string? CommodityTag,
		string? RequiredLocationTag,
		string? OutputItemStableReference,
		string? DuplicatePreventionTag,
		string Echo,
		string? AlreadyPresentEcho = null,
		string? FailureEcho = null);

	private sealed record ProjectSeed(
		string Name,
		string Description,
		string LabourTrait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		double Hours,
		int MaximumWorkers,
		bool IncludeSupervision,
		IReadOnlyList<ProjectMaterialSeed> Materials,
		IReadOnlyList<ProjectActionSeed> Actions);

	internal sealed record PrimaryProductionProjectSpecTestData(
		string Name,
		string LabourTrait,
		IReadOnlyCollection<string> MaterialTypes,
		IReadOnlyCollection<string> ActionTypes,
		IReadOnlyCollection<string> OutputCommodityTags,
		IReadOnlyCollection<string> RequiredLocationTags,
		IReadOnlyCollection<string> OutputStableReferences);

	internal static IReadOnlyCollection<PrimaryProductionProjectSpecTestData> PrimaryProductionProjectSpecsForTesting =>
		Projects()
			.Select(x => new PrimaryProductionProjectSpecTestData(
				StockProjectName(x.Name),
				x.LabourTrait,
				x.Materials.Select(y => y.Type.ToString()).ToArray(),
				x.Actions.Select(y => ActionTypeText(y.Type)).ToArray(),
				x.Actions.Select(y => y.CommodityTag).Where(y => y is not null).Select(y => y!).ToArray(),
				x.Actions.Select(y => y.RequiredLocationTag).Where(y => y is not null).Select(y => y!).ToArray(),
				x.Actions.Select(y => y.OutputItemStableReference).Where(y => y is not null).Select(y => y!).ToArray()))
			.ToArray();

	internal static IReadOnlyCollection<string> PrimaryProductionProjectNamesForTesting =>
		Projects()
			.Select(x => StockProjectName(x.Name))
			.ToArray();

	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];
	public int SortOrder => 420;
	public string Name => "Primary Production";
	public string Tagline => "Stock prospecting, extraction, quarrying, kiln, smelting, salt, tar, peat, and pigment project templates.";
	public string FullDescription => "Installs setting-neutral local project templates for primary production chains. These templates use resource-discovery actions, commodity output actions, and bulk commodity requirements as builder-editable starting points.";
	public bool SafeToRunMoreThanOnce => true;

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!PrerequisitesMet(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var allProjectsPresent = Projects().All(project =>
			context.Projects.Any(x => x.Name == StockProjectName(project.Name) && x.RevisionNumber == 0));
		if (allProjectsPresent)
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return context.Projects.Any(x => x.Name.StartsWith($"{StockPrefix}: "))
			? ShouldSeedResult.ExtraPackagesAvailable
			: ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!PrerequisitesMet(context))
		{
			return "Primary production prerequisites are not installed.";
		}

		var created = 0;
		foreach (var project in Projects())
		{
			if (EnsureProject(context, project))
			{
				created++;
			}
		}

		return created == 0
			? "Primary production project templates were already installed and have been refreshed."
			: $"Installed or refreshed {Projects().Count} primary production project templates, including {created} newly created templates.";
	}

	private static IReadOnlyList<ProjectSeed> Projects()
	{
		return
		[
			ProspectingProject(
				"Survey Mineral Signs",
				"Survey the ground for broad primary-production resource signs.",
				"Primary Production Resource",
				"primary_production_hematite_deposit",
				"Visible Resource Deposit",
				"Labouring"),
			ProspectingProject(
				"Prospect for Iron Deposits",
				"Search for a workable hematite iron deposit and recover a small sample.",
				"Hematite Resource",
				"primary_production_hematite_deposit",
				"Hematite Resource",
				"Labouring",
				[Output("Sample Iron Ore", "Create a small hematite sample from the prospecting work.", "hematite", 1500.0, "Sample Ore Commodity", "A small heap of hematite sample ore is set aside.")]),
			ProspectingProject(
				"Prospect for Tin Deposits",
				"Search stream gravels and hard rock for cassiterite signs.",
				"Cassiterite Resource",
				"primary_production_cassiterite_deposit",
				"Cassiterite Resource",
				"Labouring",
				[Output("Sample Tin Ore", "Create a small cassiterite sample from the prospecting work.", "cassiterite", 1200.0, "Sample Ore Commodity", "A small heap of cassiterite sample ore is set aside.")]),
			ProspectingProject(
				"Prospect for Copper Deposits",
				"Search for malachite and related copper-bearing surface signs.",
				"Malachite Resource",
				"primary_production_malachite_deposit",
				"Malachite Resource",
				"Labouring",
				[Output("Sample Copper Ore", "Create a small malachite sample from the prospecting work.", "malachite", 1500.0, "Sample Ore Commodity", "A small heap of green malachite sample ore is set aside.")]),
			ProspectingProject(
				"Prospect for Lead And Silver Deposits",
				"Search for galena and lead-silver mineral signs.",
				"Galena Resource",
				"primary_production_galena_deposit",
				"Galena Resource",
				"Labouring",
				[Output("Sample Lead Ore", "Create a small galena sample from the prospecting work.", "galena", 1500.0, "Sample Ore Commodity", "A small heap of galena sample ore is set aside.")]),
			ProspectingProject(
				"Prospect for Quarry Stone",
				"Search for a quarryable limestone outcrop.",
				"Limestone Resource",
				"primary_production_limestone_outcrop",
				"Limestone Resource",
				"Masonry"),
			ProspectingProject(
				"Prospect for Clay",
				"Search for a workable clay bank.",
				"Clay Pit Resource",
				"primary_production_clay_bank",
				"Clay Pit Resource",
				"Pottery"),
			ProspectingProject(
				"Prospect for Salt Or Brine",
				"Search for a brine spring or similar saltworking site.",
				"Brine Spring Resource",
				"primary_production_brine_spring",
				"Brine Spring Resource",
				"Surviving"),
			ProspectingProject(
				"Prospect for Fuel Deposits",
				"Search for peat and other local fuel resources.",
				"Peat Bog Resource",
				"primary_production_peat_bog",
				"Peat Bog Resource",
				"Surviving"),
			ProspectingProject(
				"Prospect for Alkali Deposits",
				"Search for natron flats or other alkali resource signs.",
				"Natron Resource",
				"primary_production_natron_flat",
				"Natron Resource",
				"Surviving"),
			ProspectingProject(
				"Prospect for Pigment Earth",
				"Search for ochre earth and related pigment deposits.",
				"Ochre Resource",
				"primary_production_ochre_bank",
				"Ochre Resource",
				"Painting"),
			ProspectingProject(
				"Prospect for Sulfur Deposits",
				"Search for visible sulfur or brimstone deposits.",
				"Sulfur Deposit Resource",
				"primary_production_sulfur_deposit",
				"Sulfur Deposit Resource",
				"Labouring"),
			ProspectingProject(
				"Prospect for Bitumen Seeps",
				"Search for natural bitumen or mineral pitch seeps.",
				"Bitumen Seep Resource",
				"primary_production_bitumen_seep",
				"Bitumen Seep Resource",
				"Surviving"),

			ProductionProject(
				"Burn a Charcoal Clamp",
				"Stack and burn timber under a covered clamp to produce industrial charcoal.",
				"Labouring",
				32.0,
				[CommodityTag("Fuel Wood", "Wood or timber stock consumed by the charcoal clamp.", "Primary Production Fuel", null, 250000.0)],
				[Output("Charcoal Output", "Create industrial charcoal fuel.", "charcoal", 90000.0, "Charcoal Fuel Commodity", "The charcoal clamp is opened and usable charcoal is raked out.")]),
			ProductionProject(
				"Extract Iron Ore",
				"Extract raw hematite ore from a marked iron working.",
				"Labouring",
				36.0,
				[],
				[
					Output("Raw Iron Ore", "Create raw hematite ore.", "hematite", 60000.0, "Raw Ore Commodity", "Raw hematite ore is hauled from the working."),
					Output("Mine Spoil", "Create mine spoil from waste rock.", "stone rubble", 18000.0, "Mine Spoil Commodity", "Waste stone and spoil are heaped nearby.")
				]),
			ProductionProject(
				"Extract Copper Ore",
				"Extract raw malachite ore from a marked copper working.",
				"Labouring",
				36.0,
				[],
				[
					Output("Raw Copper Ore", "Create raw malachite ore.", "malachite", 50000.0, "Raw Ore Commodity", "Raw malachite ore is hauled from the working."),
					Output("Mine Spoil", "Create mine spoil from waste rock.", "stone rubble", 16000.0, "Mine Spoil Commodity", "Waste stone and spoil are heaped nearby.")
				]),
			ProductionProject(
				"Extract Tin Ore",
				"Extract raw cassiterite ore from a marked tin working.",
				"Labouring",
				32.0,
				[],
				[
					Output("Raw Tin Ore", "Create raw cassiterite ore.", "cassiterite", 35000.0, "Raw Ore Commodity", "Raw cassiterite ore is hauled from the working."),
					Output("Mine Spoil", "Create mine spoil from waste rock.", "stone rubble", 14000.0, "Mine Spoil Commodity", "Waste stone and spoil are heaped nearby.")
				]),
			ProductionProject(
				"Extract Lead Ore",
				"Extract raw galena ore from a marked lead and silver working.",
				"Labouring",
				36.0,
				[],
				[
					Output("Raw Lead Ore", "Create raw galena ore.", "galena", 45000.0, "Raw Ore Commodity", "Raw galena ore is hauled from the working."),
					Output("Mine Spoil", "Create mine spoil from waste rock.", "stone rubble", 16000.0, "Mine Spoil Commodity", "Waste stone and spoil are heaped nearby.")
				]),
			ProductionProject(
				"Quarry Limestone Blocks",
				"Cut rough limestone blocks and recover rubble from the quarry face.",
				"Masonry",
				40.0,
				[],
				[
					Output("Rough Limestone Blocks", "Create rough limestone blocks.", "limestone", 90000.0, "Rough Stone Block Commodity", "Rough limestone blocks are levered free of the quarry face."),
					Output("Stone Rubble", "Create limestone rubble.", "stone rubble", 30000.0, "Stone Rubble Commodity", "Stone rubble and spalls are left in a useful heap.")
				]),
			ProductionProject(
				"Quarry Sandstone Blocks",
				"Cut rough sandstone blocks and recover rubble from the quarry face.",
				"Masonry",
				40.0,
				[],
				[
					Output("Rough Sandstone Blocks", "Create rough sandstone blocks.", "sandstone", 90000.0, "Rough Stone Block Commodity", "Rough sandstone blocks are levered free of the quarry face."),
					Output("Stone Rubble", "Create sandstone rubble.", "stone rubble", 30000.0, "Stone Rubble Commodity", "Stone rubble and spalls are left in a useful heap.")
				]),
			ProductionProject(
				"Burn a Lime Kiln",
				"Burn limestone and fuel in a lime kiln to make quicklime.",
				"Masonry",
				36.0,
				[
					Commodity("Limestone Charge", "Limestone or chalk charged into the kiln.", "limestone", "Rough Stone Block Commodity", 80000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the kiln.", "charcoal", "Charcoal Fuel Commodity", 25000.0)
				],
				[Output("Quicklime Output", "Create quicklime.", "calcium oxide", 42000.0, "Quicklime Commodity", "The lime kiln cools enough to draw off caustic quicklime.")]),
			ProductionProject(
				"Fire a Brick Clamp",
				"Fire green brick stock in a clamp or kiln.",
				"Pottery",
				40.0,
				[
					Commodity("Green Brick Stock", "Unfired green bricks ready for firing.", "green brick", "Green Brick Commodity", 90000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the firing.", "charcoal", "Charcoal Fuel Commodity", 20000.0)
				],
				[
					Output("Fired Brick Output", "Create fired brick stock.", "fired brick", 70000.0, "Fired Brick Commodity", "Fired bricks are drawn from the clamp."),
					Output("Brick Wasters", "Create fired-brick waste.", "stone rubble", 8000.0, "Primary Production Waste", "Wasters and fragments are sorted out from the firing.")
				]),
			ProductionProject(
				"Fire Roof Tiles",
				"Fire moulded roof tile stock in a clamp or kiln.",
				"Pottery",
				36.0,
				[
					Commodity("Roof Tile Stock", "Moulded roof tile stock ready for firing.", "roof tile", "Roof Tile Commodity", 70000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the firing.", "charcoal", "Charcoal Fuel Commodity", 18000.0)
				],
				[Output("Roof Tile Output", "Create fired roof tile stock.", "roof tile", 56000.0, "Roof Tile Commodity", "Roof tiles are drawn from the firing.")]),
			ProductionProject(
				"Smelt an Iron Bloom",
				"Smelt roasted iron ore with charcoal and flux in a bloomery furnace.",
				"Smelting",
				48.0,
				[
					Commodity("Roasted Iron Ore", "Roasted iron ore concentrate.", "hematite", "Roasted Ore Commodity", 50000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the bloomery.", "charcoal", "Charcoal Fuel Commodity", 30000.0),
					Commodity("Flux", "Quicklime or limestone flux for the smelt.", "calcium oxide", "Quicklime Commodity", 6000.0)
				],
				[
					Output("Iron Bloom", "Create a bloom of sponge iron.", "iron bloom", 18000.0, "Bloom Commodity", "An iron bloom is drawn from the furnace."),
					Output("Slag", "Create metallurgical slag.", "slag", 16000.0, "Slag Commodity", "Slag is broken away from the smelting hearth.")
				]),
			ProductionProject(
				"Smelt Copper Ore",
				"Smelt roasted copper ore into copper metal stock.",
				"Smelting",
				44.0,
				[
					Commodity("Roasted Copper Ore", "Roasted malachite concentrate.", "malachite", "Roasted Ore Commodity", 40000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the smelt.", "charcoal", "Charcoal Fuel Commodity", 25000.0),
					Commodity("Flux", "Quicklime or limestone flux for the smelt.", "calcium oxide", "Quicklime Commodity", 5000.0)
				],
				[
					Output("Copper Ingot Stock", "Create copper metal ingot stock.", "copper", 18000.0, "Metal Ingot Commodity", "Copper ingot stock is cast from the smelt."),
					Output("Slag", "Create metallurgical slag.", "slag", 14000.0, "Slag Commodity", "Slag is broken away from the smelting hearth.")
				]),
			ProductionProject(
				"Smelt Tin Ore",
				"Smelt washed cassiterite ore into tin metal stock.",
				"Smelting",
				36.0,
				[
					Commodity("Washed Tin Ore", "Washed cassiterite concentrate.", "cassiterite", "Washed Ore Commodity", 28000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the smelt.", "charcoal", "Charcoal Fuel Commodity", 14000.0)
				],
				[
					Output("Tin Ingot Stock", "Create tin metal ingot stock.", "tin", 9000.0, "Metal Ingot Commodity", "Tin ingot stock is cast from the smelt."),
					Output("Slag", "Create metallurgical slag.", "slag", 6000.0, "Slag Commodity", "Slag is broken away from the smelting hearth.")
				]),
			ProductionProject(
				"Boil Brine For Salt",
				"Boil brine in salt pans to create useful salt stock.",
				"Labouring",
				30.0,
				[Commodity("Charcoal Fuel", "Fuel for the salt pans.", "charcoal", "Charcoal Fuel Commodity", 10000.0)],
				[Output("Salt Output", "Create salt stock from brine.", "salt", 18000.0, "Salt Commodity", "Salt crystals are raked from the pans.")]),
			ProductionProject(
				"Collect Natron",
				"Collect and grade natron from an alkali flat.",
				"Surviving",
				20.0,
				[],
				[Output("Natron Output", "Create natron alkali stock.", "natron", 18000.0, "Potash Commodity", "Natron is scraped, dried, and bagged.")]),
			ProductionProject(
				"Fire a Glass Furnace Batch",
				"Fire prepared glass batch into glass blank stock.",
				"Glassworking",
				44.0,
				[
					Commodity("Glass Batch", "Prepared glass batch.", "glass batch", "Glass Batch Commodity", 45000.0),
					Commodity("Charcoal Fuel", "Industrial charcoal fuel for the glass furnace.", "charcoal", "Charcoal Fuel Commodity", 30000.0)
				],
				[Output("Glass Blank Output", "Create glass blank stock.", "glass blank", 24000.0, "Glass Blank Commodity", "Usable glass blank stock is drawn from the furnace.")]),
			ProductionProject(
				"Burn a Tar Kiln",
				"Slow-burn resinous timber in a tar kiln.",
				"Surviving",
				30.0,
				[Commodity("Resinous Wood", "Resinous wood or tarwood feedstock.", "pine", null, 90000.0)],
				[Output("Tar Output", "Create pine tar stock.", "tar", 18000.0, "Tar Commodity", "Dark tar is drawn from the kiln.")]),
			ProductionProject(
				"Cut Peat Turves",
				"Cut peat into turves and stack them to dry.",
				"Labouring",
				24.0,
				[],
				[Output("Peat Fuel Output", "Create dried peat fuel stock.", "dried peat", 40000.0, "Peat Fuel Commodity", "Peat turves are cut and stacked into usable fuel blocks.")]),
			ProductionProject(
				"Collect Ochre Earth",
				"Dig and grade ochre earth for pigment stock.",
				"Painting",
				18.0,
				[],
				[Output("Ochre Pigment Output", "Create ochre pigment stock.", "ochre pigment", 12000.0, "Pigment Commodity", "Ochre earth is graded into pigment stock.")]),
			ProductionProject(
				"Collect Bitumen",
				"Collect natural bitumen from a seep.",
				"Labouring",
				18.0,
				[],
				[Output("Bitumen Output", "Create bitumen stock.", "bitumen", 16000.0, "Tar Commodity", "Sticky bitumen is collected from the seep.")]),
			ProductionProject(
				"Mine Coal",
				"Mine coal for later medieval or industrial fuel chains.",
				"Labouring",
				36.0,
				[],
				[
					Output("Coal Fuel Output", "Create coal fuel stock.", "coal", 45000.0, "Coal Fuel Commodity", "Coal is hauled from the working."),
					Output("Mine Spoil", "Create mine spoil from waste rock.", "stone rubble", 14000.0, "Mine Spoil Commodity", "Waste stone and spoil are heaped nearby.")
				])
		];
	}

	private static ProjectSeed ProspectingProject(string name, string description, string requiredLocationTag,
		string outputStableReference, string duplicatePreventionTag, string labourTrait,
		IReadOnlyList<ProjectActionSeed>? extraActions = null)
	{
		var actions = new List<ProjectActionSeed>
		{
			Reveal(
				"Reveal Resource Marker",
				"Reveal the discovered visible resource marker.",
				requiredLocationTag,
				outputStableReference,
				duplicatePreventionTag,
				"The prospecting work reveals a more obvious resource marker.",
				"The resource marker is already obvious here.",
				"The prospecting work finds no configured resource at this site.")
		};
		if (extraActions is not null)
		{
			actions.AddRange(extraActions);
		}

		return new ProjectSeed(
			name,
			description,
			labourTrait,
			10,
			Difficulty.Easy,
			16.0,
			4,
			false,
			[],
			actions);
	}

	private static ProjectSeed ProductionProject(string name, string description, string labourTrait, double hours,
		IReadOnlyList<ProjectMaterialSeed> materials, IReadOnlyList<ProjectActionSeed> actions)
	{
		return new ProjectSeed(
			name,
			description,
			labourTrait,
			15,
			Difficulty.Normal,
			hours,
			6,
			true,
			materials,
			actions);
	}

	private static ProjectMaterialSeed Commodity(string name, string description, string material, string? tag,
		double amount)
	{
		return new ProjectMaterialSeed(name, description, ProjectMaterialSeedType.Commodity, material, amount, tag);
	}

	private static ProjectMaterialSeed CommodityTag(string name, string description, string materialTag, string? tag,
		double amount)
	{
		return new ProjectMaterialSeed(name, description, ProjectMaterialSeedType.CommodityTag, materialTag, amount,
			tag);
	}

	private static ProjectActionSeed Output(string name, string description, string material, double weight,
		string? tag, string echo)
	{
		return new ProjectActionSeed(name, description, ProjectActionSeedType.CommodityOutput, material, weight, tag,
			null, null, null, echo);
	}

	private static ProjectActionSeed Reveal(string name, string description, string requiredLocationTag,
		string outputStableReference, string duplicatePreventionTag, string echo, string alreadyPresentEcho,
		string failureEcho)
	{
		return new ProjectActionSeed(name, description, ProjectActionSeedType.ResourceDiscovery, null, 0.0, null,
			requiredLocationTag, outputStableReference, duplicatePreventionTag, echo, alreadyPresentEcho, failureEcho);
	}

	private static bool EnsureProject(FuturemudDatabaseContext context, ProjectSeed seed)
	{
		var projectName = StockProjectName(seed.Name);
		var created = false;
		var project = context.Projects.FirstOrDefault(x => x.Name == projectName && x.RevisionNumber == 0);
		if (project is null)
		{
			var accountId = context.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
			var editable = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = accountId,
				BuilderComment = "Seeded stock primary-production project template.",
				BuilderDate = DateTime.UtcNow,
				ReviewerAccountId = accountId,
				ReviewerComment = "Seeded stock primary-production project template.",
				ReviewerDate = DateTime.UtcNow
			};
			project = new Project
			{
				Id = context.Projects.Any() ? context.Projects.Max(x => x.Id) + 1 : 1,
				RevisionNumber = 0,
				EditableItem = editable,
				Type = "local",
				Name = projectName,
				AppearInJobsList = false
			};
			context.Projects.Add(project);
			created = true;
		}

		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		project.Type = "local";
		project.AppearInJobsList = false;
		project.Definition = new XElement("Project",
			new XElement("Tagline", new XCData(seed.Description)),
			new XElement("AppearInProjectListProg", alwaysTrue.Id),
			new XElement("CanInitiateProg", alwaysTrue.Id),
			new XElement("WhyCannotInitiateProg", 0),
			new XElement("OnStartProg", 0),
			new XElement("OnFinishProg", 0),
			new XElement("OnCancelProg", 0),
			new XElement("CanCancelProg", 0),
			new XElement("WhyCannotCancelProg", 0)).ToString();
		context.SaveChanges();

		var phase = EnsurePhase(context, project, seed);
		EnsurePrimaryLabour(context, phase, seed);
		EnsureSupervisionLabour(context, phase, seed);
		EnsureMaterials(context, phase, seed);
		EnsureActions(context, phase, seed);
		context.SaveChanges();
		return created;
	}

	private static ProjectPhase EnsurePhase(FuturemudDatabaseContext context, Project project, ProjectSeed seed)
	{
		var phase = context.ProjectPhases.FirstOrDefault(x =>
			x.ProjectId == project.Id &&
			x.ProjectRevisionNumber == project.RevisionNumber &&
			x.PhaseNumber == 1);
		if (phase is null)
		{
			phase = new ProjectPhase
			{
				ProjectId = project.Id,
				ProjectRevisionNumber = project.RevisionNumber,
				PhaseNumber = 1
			};
			context.ProjectPhases.Add(phase);
			context.SaveChanges();
		}

		phase.Description = seed.Description;
		return phase;
	}

	private static void EnsurePrimaryLabour(FuturemudDatabaseContext context, ProjectPhase phase, ProjectSeed seed)
	{
		var trait = ResolveTrait(context, seed.LabourTrait);
		var labour = context.ProjectLabourRequirements.FirstOrDefault(x =>
			x.ProjectPhaseId == phase.Id &&
			x.Name == "Primary Production Labour");
		if (labour is null)
		{
			labour = new ProjectLabourRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Primary Production Labour"
			};
			context.ProjectLabourRequirements.Add(labour);
		}

		labour.Type = "simple";
		labour.Description = $"Primary production work using {seed.LabourTrait} methods. Durable tools are expected by builders but are not consumed by this project requirement.";
		labour.TotalProgressRequired = seed.Hours;
		labour.MaximumSimultaneousWorkers = seed.MaximumWorkers;
		labour.Definition = LabourDefinition(true, trait.Id, seed.MinimumTraitValue, seed.Difficulty);
	}

	private static void EnsureSupervisionLabour(FuturemudDatabaseContext context, ProjectPhase phase, ProjectSeed seed)
	{
		var existing = context.ProjectLabourRequirements.FirstOrDefault(x =>
			x.ProjectPhaseId == phase.Id &&
			x.Name == "Primary Production Supervision");
		if (!seed.IncludeSupervision)
		{
			if (existing is not null)
			{
				context.ProjectLabourRequirements.Remove(existing);
			}

			return;
		}

		var trait = ResolveTrait(context, seed.LabourTrait);
		if (existing is null)
		{
			existing = new ProjectLabourRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Primary Production Supervision"
			};
			context.ProjectLabourRequirements.Add(existing);
		}

		existing.Type = "supervision";
		existing.Description = $"Skilled oversight for {seed.Name}.";
		existing.TotalProgressRequired = Math.Max(8.0, seed.Hours * 0.5);
		existing.MaximumSimultaneousWorkers = 1;
		existing.Definition = new XElement("Labour",
			new XElement("Mandatory", false),
			new XElement("IsQualifiedProg", 0),
			new XElement("RequiredTrait", trait.Id),
			new XElement("MinimumTraitValue", Math.Max(20, seed.MinimumTraitValue + 10)),
			new XElement("TraitCheckDifficulty", (int)Difficulty.Normal),
			new XElement("MultiplierForOtherLabours", 1.25),
			new XElement("TraitScaledMultiplier", true)).ToString();
	}

	private static string LabourDefinition(bool mandatory, long traitId, double minimumTraitValue,
		Difficulty difficulty)
	{
		return new XElement("Labour",
			new XElement("Mandatory", mandatory),
			new XElement("IsQualifiedProg", 0),
			new XElement("RequiredTrait", traitId),
			new XElement("MinimumTraitValue", minimumTraitValue),
			new XElement("TraitCheckDifficulty", (int)difficulty)).ToString();
	}

	private static void EnsureMaterials(FuturemudDatabaseContext context, ProjectPhase phase, ProjectSeed seed)
	{
		foreach (var material in seed.Materials)
		{
			var existing = context.ProjectMaterialRequirements.FirstOrDefault(x =>
				x.ProjectPhaseId == phase.Id &&
				x.Name == material.Name);
			if (existing is null)
			{
				existing = new ProjectMaterialRequirement
				{
					ProjectPhaseId = phase.Id,
					Name = material.Name
				};
				context.ProjectMaterialRequirements.Add(existing);
			}

			existing.Type = material.Type == ProjectMaterialSeedType.Commodity ? "commodity" : "commoditytag";
			existing.Description = material.Description;
			existing.IsMandatoryForProjectCompletion = true;
			existing.Definition = MaterialDefinition(context, material);
		}
	}

	private static string MaterialDefinition(FuturemudDatabaseContext context, ProjectMaterialSeed material)
	{
		var pileTagId = string.IsNullOrWhiteSpace(material.CommodityTag)
			? 0L
			: ResolveTag(context, material.CommodityTag).Id;
		return material.Type switch
		{
			ProjectMaterialSeedType.Commodity => new XElement("Material",
				new XElement("Material", ResolveMaterial(context, material.MaterialOrTag).Id),
				new XElement("Tag", pileTagId),
				new XElement("Amount", material.Amount),
				new XElement("Quality", (int)ItemQuality.Terrible),
				new XElement("Characteristics")).ToString(),
			ProjectMaterialSeedType.CommodityTag => new XElement("Material",
				new XElement("MaterialTag", ResolveTag(context, material.MaterialOrTag).Id),
				new XElement("Tag", pileTagId),
				new XElement("Amount", material.Amount),
				new XElement("Quality", (int)ItemQuality.Terrible),
				new XElement("Characteristics")).ToString(),
			_ => throw new ApplicationException($"Unsupported primary-production material seed type {material.Type}.")
		};
	}

	private static void EnsureActions(FuturemudDatabaseContext context, ProjectPhase phase, ProjectSeed seed)
	{
		var sortOrder = 0;
		foreach (var actionSeed in seed.Actions)
		{
			var existing = context.ProjectActions.FirstOrDefault(x =>
				x.ProjectPhaseId == phase.Id &&
				x.Name == actionSeed.Name);
			if (existing is null)
			{
				existing = new ProjectAction
				{
					ProjectPhaseId = phase.Id,
					Name = actionSeed.Name
				};
				context.ProjectActions.Add(existing);
			}

			existing.Type = ActionTypeText(actionSeed.Type);
			existing.Description = actionSeed.Description;
			existing.SortOrder = sortOrder++;
			existing.Definition = ActionDefinition(context, actionSeed);
		}
	}

	private static string ActionDefinition(FuturemudDatabaseContext context, ProjectActionSeed action)
	{
		return action.Type switch
		{
			ProjectActionSeedType.CommodityOutput => new XElement("Action",
				new XElement("MaterialId", ResolveMaterial(context, action.Material!).Id),
				new XElement("Weight", action.Weight.ToString("R", System.Globalization.CultureInfo.InvariantCulture)),
				new XElement("TagId", string.IsNullOrWhiteSpace(action.CommodityTag) ? 0 : ResolveTag(context, action.CommodityTag).Id),
				new XElement("UseIndirectDescription", false),
				new XElement("Echo", action.Echo),
				new XElement("Characteristics")).ToString(),
			ProjectActionSeedType.ResourceDiscovery => ResourceDiscoveryDefinition(context, action),
			_ => throw new ApplicationException($"Unsupported primary-production action seed type {action.Type}.")
		};
	}

	private static string ResourceDiscoveryDefinition(FuturemudDatabaseContext context, ProjectActionSeed action)
	{
		var proto = ResolveItemPrototype(context, action.OutputItemStableReference!);
		return new XElement("Action",
			new XElement("RequiredLocationTagId", ResolveTag(context, action.RequiredLocationTag!).Id),
			new XElement("OutputItemProtoId", proto.Id),
			new XElement("OutputItemProtoRevision", proto.RevisionNumber),
			new XElement("DuplicatePreventionTagId", ResolveTag(context, action.DuplicatePreventionTag!).Id),
			new XElement("Echo", action.Echo),
			new XElement("AlreadyPresentEcho", action.AlreadyPresentEcho ?? string.Empty),
			new XElement("FailureEcho", action.FailureEcho ?? string.Empty)).ToString();
	}

	private static string ActionTypeText(ProjectActionSeedType type)
	{
		return type switch
		{
			ProjectActionSeedType.CommodityOutput => "commodityoutput",
			ProjectActionSeedType.ResourceDiscovery => "resourcediscovery",
			_ => throw new ApplicationException($"Unsupported primary-production action seed type {type}.")
		};
	}

	private static bool PrerequisitesMet(FuturemudDatabaseContext context)
	{
		return context.Accounts.Any() &&
		       context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue") &&
		       RequiredTags().All(tag => context.Tags.Any(x => x.Name == tag)) &&
		       RequiredMaterials().All(material => context.Materials.Any(x => x.Name == material)) &&
		       RequiredItemStableReferences().All(stable => context.GameItemProtos.Any(x => x.UniqueName == stable)) &&
		       Projects().Select(x => x.LabourTrait).Distinct(StringComparer.OrdinalIgnoreCase)
		                 .All(trait => TryResolveTrait(context, trait) is not null);
	}

	private static IReadOnlyCollection<string> RequiredTags()
	{
		return Projects()
			.SelectMany(x => x.Materials.Select(y => y.CommodityTag)
			                  .Concat(x.Materials.Where(y => y.Type == ProjectMaterialSeedType.CommodityTag)
			                           .Select(y => y.MaterialOrTag))
			                  .Concat(x.Actions.Select(y => y.CommodityTag))
			                  .Concat(x.Actions.Select(y => y.RequiredLocationTag))
			                  .Concat(x.Actions.Select(y => y.DuplicatePreventionTag)))
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static IReadOnlyCollection<string> RequiredMaterials()
	{
		return Projects()
			.SelectMany(x => x.Materials.Where(y => y.Type == ProjectMaterialSeedType.Commodity)
			                  .Select(y => y.MaterialOrTag)
			                  .Concat(x.Actions.Select(y => y.Material)))
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static IReadOnlyCollection<string> RequiredItemStableReferences()
	{
		return Projects()
			.SelectMany(x => x.Actions.Select(y => y.OutputItemStableReference))
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static Material ResolveMaterial(FuturemudDatabaseContext context, string name)
	{
		return context.Materials.First(x => x.Name == name);
	}

	private static Tag ResolveTag(FuturemudDatabaseContext context, string name)
	{
		return context.Tags.First(x => x.Name == name);
	}

	private static GameItemProto ResolveItemPrototype(FuturemudDatabaseContext context, string stableReference)
	{
		return context.GameItemProtos
		              .OrderByDescending(x => x.RevisionNumber)
		              .First(x => x.UniqueName == stableReference);
	}

	private static TraitDefinition ResolveTrait(FuturemudDatabaseContext context, string name)
	{
		return TryResolveTrait(context, name) ??
		       throw new ApplicationException($"Primary production project trait {name} is not installed.");
	}

	private static TraitDefinition? TryResolveTrait(FuturemudDatabaseContext context, string name)
	{
		var names = TraitAliases(name);
		return context.TraitDefinitions
		              .AsEnumerable()
		              .FirstOrDefault(x => names.Any(y => NormaliseName(x.Name) == NormaliseName(y)));
	}

	private static IReadOnlyCollection<string> TraitAliases(string name)
	{
		return name switch
		{
			"Labouring" => ["Labouring", "Labourer", "Laboring", "Laborer"],
			"Surviving" => ["Surviving", "Survival"],
			"Masonry" => ["Masonry", "Stonecraft", "Stoneworking", "Mason"],
			"Pottery" => ["Pottery", "Potter"],
			"Smelting" => ["Smelting", "Smelter"],
			"Glassworking" => ["Glassworking", "Glasswork", "Glassworker"],
			"Painting" => ["Painting", "Paint"],
			_ => [name]
		};
	}

	private static string NormaliseName(string text)
	{
		return new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
	}

	private static string StockProjectName(string name)
	{
		return $"{StockPrefix}: {name}";
	}
}
