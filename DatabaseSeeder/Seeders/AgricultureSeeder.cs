using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.Work.Agriculture;

namespace DatabaseSeeder.Seeders;

public sealed class AgricultureSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Stock Agriculture";

	private static readonly (string Name, string Description, AgricultureFieldUse[] Uses, (AgricultureScoreType Score, int Value)[] Scores)[] Profiles =
	[
		("Arable Field", "A general-purpose field suited to broadacre cropping and periodic improvement.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 65), (AgricultureScoreType.Tilth, 50), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 60)]),
		("Garden Bed", "A small intensively-worked plot suitable for vegetables, herbs, and specialist crops.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 70), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 75), (AgricultureScoreType.Tilth, 70), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 25), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 45), (AgricultureScoreType.Pasture, 15), (AgricultureScoreType.Condition, 70)]),
		("Orchard Grove", "A cultivated grove suitable for long-lived fruit or nut trees.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 60), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 55), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 65)]),
		("Pasture", "A grazed field with enough grass, fencing, and soil cover to support abstract herds.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 50), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 40), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 70), (AgricultureScoreType.Pasture, 75), (AgricultureScoreType.Condition, 65)]),
		("Wet Field", "Low, damp land that can be productive after drainage and careful crop choice.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 80), (AgricultureScoreType.Drainage, 25), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 45)]),
		("Rocky Field", "A stony plot that needs clearance before it can reliably crop well.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 35), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 75), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 35)]),
		("Saline Coastal Field", "A salt-affected field where salinity management matters more than perfect drainage.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 40), (AgricultureScoreType.Salinity, 65), (AgricultureScoreType.Topsoil, 45), (AgricultureScoreType.Tilth, 40), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 35)]),
		("Managed Woodland", "A cultivated woodland, coppice, or timber stand managed as a field.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 25), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 65)])
	];

	private static readonly (string Name, string Description, string Category, int Growth, int Window, int MinMoisture, int MaxMoisture, int MinTemp, int MaxTemp)[] Crops =
	[
		("Cereal Grain", "A generic wheat, barley, millet, rice, or similar staple grain crop.", "grain", 90, 14, 35, 80, 5, 38),
		("Vegetable Crop", "A generic vegetable garden crop for leaves, stems, pods, or fruits.", "vegetable", 55, 10, 40, 85, 8, 36),
		("Legume Crop", "A generic bean, pea, pulse, or nitrogen-building fodder crop.", "legume", 70, 12, 35, 80, 8, 34),
		("Root Crop", "A generic tuber, bulb, or root vegetable crop.", "root", 75, 18, 35, 75, 4, 32),
		("Orchard Fruit", "A generic fruit or nut orchard crop that represents seasonal care and harvest.", "orchard", 180, 25, 35, 85, -5, 40),
		("Fibre Crop", "A generic flax, hemp, cotton, or other fibre crop.", "fibre", 100, 14, 30, 75, 10, 40)
	];

	private static readonly (string Name, string Description, double AnimalUnits, double DailyGraze, int MaximumCondition)[] Herds =
	[
		("Cattle Herd", "A generic cattle, aurochs, buffalo, or large grazing herd.", 1.0, 1.0, 100),
		("Sheep or Goat Flock", "A generic flock of sheep, goats, or other small browsers.", 0.2, 0.3, 100),
		("Pig Herd", "A generic pig, boar, or omnivorous rooting herd.", 0.4, 0.45, 100),
		("Poultry Flock", "A generic flock of chickens, ducks, geese, or similar fowl.", 0.03, 0.05, 100)
	];

	private static readonly (string Name, string Description, string Type, int Establishment, int Cycle)[] Woodlands =
	[
		("Coppice Woodland", "A managed stand cut on a short cycle for poles, stakes, firewood, and withies.", "coppice", 180, 365),
		("Timber Stand", "A managed stand grown on a longer cycle for larger timber.", "timber", 365, 1825),
		("Pollarded Grove", "A managed grove of trees repeatedly cut above browsing height.", "pollard", 240, 730)
	];

	private static readonly (string Name, string Description, AgricultureOperationType Type, AgricultureTargetType Target, AgricultureFieldUse Required, AgricultureFieldUse Result, double Hours, (AgricultureScoreType Score, int Delta)[] Deltas)[] Operations =
	[
		("Plough Field", "Break and turn the soil to improve tilth and suppress weeds.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 8.0, [(AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Weeds, -10), (AgricultureScoreType.Moisture, -3), (AgricultureScoreType.Condition, 3)]),
		("Sow Crop", "Sow a crop into prepared ground.", AgricultureOperationType.Sow, AgricultureTargetType.Crop, AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, 6.0, [(AgricultureScoreType.Tilth, -5), (AgricultureScoreType.Weeds, 5)]),
		("Weed Field", "Remove competing weeds and tidy a growing field.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 5.0, [(AgricultureScoreType.Weeds, -20), (AgricultureScoreType.Condition, 3)]),
		("Irrigate Field", "Move water onto the field or garden.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 4.0, [(AgricultureScoreType.Moisture, 15), (AgricultureScoreType.Salinity, 2)]),
		("Drain Field", "Cut ditches, clear channels, or improve drainage.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 12.0, [(AgricultureScoreType.Drainage, 15), (AgricultureScoreType.Moisture, -8), (AgricultureScoreType.Condition, 4)]),
		("Fertilise Field", "Add manure, compost, ash, or other soil amendments.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 5.0, [(AgricultureScoreType.Nutrients, 20), (AgricultureScoreType.Weeds, 3), (AgricultureScoreType.Condition, 3)]),
		("Clear Stones", "Dig out rocks and boulders from the usable topsoil.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 10.0, [(AgricultureScoreType.Rockiness, -20), (AgricultureScoreType.Tilth, 5), (AgricultureScoreType.Condition, 2)]),
		("Build or Repair Fences", "Build or repair fencing, hedges, walls, gates, or other stock barriers.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 8.0, [(AgricultureScoreType.Fence, 25), (AgricultureScoreType.Condition, 2)]),
		("Harvest Crop", "Harvest a crop that is ready to be gathered.", AgricultureOperationType.Harvest, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Fallow, 8.0, [(AgricultureScoreType.Nutrients, -5), (AgricultureScoreType.Weeds, 5)]),
		("Rotate Grazing", "Move or settle an abstract herd onto pasture.", AgricultureOperationType.Herd, AgricultureTargetType.Herd, AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, 4.0, [(AgricultureScoreType.Fence, -2), (AgricultureScoreType.Pasture, -5)]),
		("Plant Woodland", "Plant or establish a managed woodland, coppice, pollard, or timber stand.", AgricultureOperationType.Woodland, AgricultureTargetType.Woodland, AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, 16.0, [(AgricultureScoreType.Topsoil, -3), (AgricultureScoreType.Weeds, -5), (AgricultureScoreType.Condition, 5)]),
		("Coppice Woodland", "Cut a coppice stand back on cycle to produce poles or firewood.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 12.0, [(AgricultureScoreType.Nutrients, -3), (AgricultureScoreType.Condition, 2)]),
		("Thin Woodland", "Thin a woodland stand to improve remaining growth.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 10.0, [(AgricultureScoreType.Pests, -5), (AgricultureScoreType.Condition, 4)]),
		("Fell Woodland", "Fell a managed woodland stand and return the field to fallow use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 20.0, [(AgricultureScoreType.Topsoil, -5), (AgricultureScoreType.Weeds, 5), (AgricultureScoreType.Condition, -5)]),
		("Clear Land", "Clear brush, stumps, scrub, or unmanaged growth for future field use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 18.0, [(AgricultureScoreType.Weeds, -10), (AgricultureScoreType.Topsoil, -3), (AgricultureScoreType.Condition, -2)])
	];

	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];
	public int SortOrder => 220;
	public string Name => "Agriculture";
	public string Tagline => "Adds stock field profiles, crop, herd, woodland, and agriculture project-operation definitions.";
	public string FullDescription => "Installs the generic agriculture definitions used by the farming system. These are setting-neutral examples and builder-editable starting points.";
	public bool SafeToRunMoreThanOnce => true;

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue"))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var allOperationsPresent = Operations.All(x => context.AgricultureOperations.Any(y => y.Name == x.Name));
		if (allOperationsPresent)
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return context.AgricultureFieldProfiles.Any() ||
		       context.AgricultureCropDefinitions.Any() ||
		       context.AgricultureHerdDefinitions.Any() ||
		       context.AgricultureWoodlandDefinitions.Any()
			? ShouldSeedResult.ExtraPackagesAvailable
			: ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		foreach (var profile in Profiles)
		{
			EnsureProfile(context, profile);
		}

		foreach (var crop in Crops)
		{
			EnsureCrop(context, crop);
		}

		foreach (var herd in Herds)
		{
			EnsureHerd(context, herd);
		}

		foreach (var woodland in Woodlands)
		{
			EnsureWoodland(context, woodland);
		}

		foreach (var operation in Operations)
		{
			var project = EnsureProject(context, operation.Name, operation.Description, operation.Hours);
			EnsureOperation(context, operation, project);
		}

		context.SaveChanges();
		return "Installed or refreshed stock agriculture definitions and project-backed operations.";
	}

	private static void EnsureProfile(FuturemudDatabaseContext context, (string Name, string Description, AgricultureFieldUse[] Uses, (AgricultureScoreType Score, int Value)[] Scores) definition)
	{
		var profile = context.AgricultureFieldProfiles.FirstOrDefault(x => x.Name == definition.Name);
		if (profile == null)
		{
			profile = new AgricultureFieldProfile { Name = definition.Name };
			context.AgricultureFieldProfiles.Add(profile);
		}

		profile.Description = definition.Description;
		profile.Definition = new XElement("Profile",
			new XAttribute("uses", string.Join(",", definition.Uses.Select(x => x.ToString()))),
			definition.Scores.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Value)))).ToString();
	}

	private static void EnsureCrop(FuturemudDatabaseContext context, (string Name, string Description, string Category, int Growth, int Window, int MinMoisture, int MaxMoisture, int MinTemp, int MaxTemp) definition)
	{
		var crop = context.AgricultureCropDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (crop == null)
		{
			crop = new AgricultureCropDefinition { Name = definition.Name };
			context.AgricultureCropDefinitions.Add(crop);
		}

		crop.Description = definition.Description;
		crop.Category = definition.Category;
		crop.Definition = new XElement("Crop",
			new XAttribute("growthDays", definition.Growth),
			new XAttribute("harvestWindowDays", definition.Window),
			new XAttribute("minMoisture", definition.MinMoisture),
			new XAttribute("maxMoisture", definition.MaxMoisture),
			new XAttribute("minTemperature", definition.MinTemp),
			new XAttribute("maxTemperature", definition.MaxTemp)).ToString();
	}

	private static void EnsureHerd(FuturemudDatabaseContext context, (string Name, string Description, double AnimalUnits, double DailyGraze, int MaximumCondition) definition)
	{
		var herd = context.AgricultureHerdDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (herd == null)
		{
			herd = new AgricultureHerdDefinition { Name = definition.Name };
			context.AgricultureHerdDefinitions.Add(herd);
		}

		herd.Description = definition.Description;
		herd.NpcTemplateId = null;
		herd.NpcTemplateRevisionNumber = null;
		herd.Definition = new XElement("Herd",
			new XAttribute("animalUnits", definition.AnimalUnits),
			new XAttribute("dailyGraze", definition.DailyGraze),
			new XAttribute("maximumCondition", definition.MaximumCondition)).ToString();
	}

	private static void EnsureWoodland(FuturemudDatabaseContext context, (string Name, string Description, string Type, int Establishment, int Cycle) definition)
	{
		var woodland = context.AgricultureWoodlandDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (woodland == null)
		{
			woodland = new AgricultureWoodlandDefinition { Name = definition.Name };
			context.AgricultureWoodlandDefinitions.Add(woodland);
		}

		woodland.Description = definition.Description;
		woodland.WoodlandType = definition.Type;
		woodland.Definition = new XElement("Woodland",
			new XAttribute("establishmentDays", definition.Establishment),
			new XAttribute("harvestCycleDays", definition.Cycle)).ToString();
	}

	private static Project EnsureProject(FuturemudDatabaseContext context, string operationName, string description, double hours)
	{
		var projectName = $"{StockPrefix}: {operationName}";
		var project = context.Projects.FirstOrDefault(x => x.Name == projectName && x.RevisionNumber == 0);
		if (project == null)
		{
			var accountId = context.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
			var editable = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = accountId,
				BuilderComment = "Seeded stock agriculture project template.",
				BuilderDate = DateTime.UtcNow
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
		}

		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		project.Definition = new XElement("Project",
			new XElement("Tagline", new XCData(description)),
			new XElement("AppearInProjectListProg", alwaysTrue.Id),
			new XElement("CanInitiateProg", alwaysTrue.Id),
			new XElement("WhyCannotInitiateProg", 0),
			new XElement("OnStartProg", 0),
			new XElement("OnFinishProg", 0),
			new XElement("OnCancelProg", 0),
			new XElement("CanCancelProg", 0),
			new XElement("WhyCannotCancelProg", 0)).ToString();
		context.SaveChanges();

		var phase = context.ProjectPhases.FirstOrDefault(x => x.ProjectId == project.Id && x.ProjectRevisionNumber == project.RevisionNumber && x.PhaseNumber == 1);
		if (phase == null)
		{
			phase = new ProjectPhase
			{
				ProjectId = project.Id,
				ProjectRevisionNumber = project.RevisionNumber,
				PhaseNumber = 1,
				Description = $"Complete the field work for {operationName}."
			};
			context.ProjectPhases.Add(phase);
			context.SaveChanges();
		}

		phase.Description = $"Complete the field work for {operationName}.";

		var labour = context.ProjectLabourRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == "Field Labour");
		if (labour == null)
		{
			labour = new ProjectLabourRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Field Labour",
				Type = "simple"
			};
			context.ProjectLabourRequirements.Add(labour);
		}

		labour.Description = "General agricultural labour.";
		labour.TotalProgressRequired = hours;
		labour.MaximumSimultaneousWorkers = 6;
		labour.Definition = new XElement("Labour",
			new XElement("Mandatory", true),
			new XElement("IsQualifiedProg", 0),
			new XElement("RequiredTrait", 0),
			new XElement("MinimumTraitValue", 0.0),
			new XElement("TraitCheckDifficulty", 0)).ToString();

		var action = context.ProjectActions.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Type == "agriculture");
		if (action == null)
		{
			action = new ProjectAction
			{
				ProjectPhaseId = phase.Id,
				Name = "Agriculture Operation",
				Type = "agriculture",
				SortOrder = 0
			};
			context.ProjectActions.Add(action);
		}

		action.Description = "Apply the agriculture operation when the project completes.";
		action.Definition = "<Action />";
		context.SaveChanges();
		return project;
	}

	private static void EnsureOperation(FuturemudDatabaseContext context, (string Name, string Description, AgricultureOperationType Type, AgricultureTargetType Target, AgricultureFieldUse Required, AgricultureFieldUse Result, double Hours, (AgricultureScoreType Score, int Delta)[] Deltas) definition, Project project)
	{
		var operation = context.AgricultureOperations.FirstOrDefault(x => x.Name == definition.Name);
		if (operation == null)
		{
			operation = new AgricultureOperation { Name = definition.Name };
			context.AgricultureOperations.Add(operation);
		}

		operation.Description = definition.Description;
		operation.OperationType = (int)definition.Type;
		operation.TargetType = (int)definition.Target;
		operation.RequiredUse = (int)definition.Required;
		operation.ResultUse = (int)definition.Result;
		operation.ProjectId = project.Id;
		operation.ProjectRevisionNumber = project.RevisionNumber;
		operation.CompletionProgId = null;
		operation.Definition = new XElement("Operation",
			definition.Deltas.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Delta)))).ToString();
	}
}
