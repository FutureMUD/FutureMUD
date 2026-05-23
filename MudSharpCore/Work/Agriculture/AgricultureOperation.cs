using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Work.Projects;

namespace MudSharp.Work.Agriculture;

public class AgricultureOperation : SaveableItem, IAgricultureOperation
{
	private readonly Dictionary<AgricultureScoreType, int> _scoreDeltas = new();
	private readonly HashSet<AgricultureFieldUse> _allowedUses = new();
	private readonly List<AgricultureCommodityYield> _apiaryYieldOutputs = new();
	private long _projectId;
	private int _projectRevisionNumber;
	private IProject _project;
	private long? _completionProgId;
	private IFutureProg _completionProg;

	public AgricultureOperation(Models.AgricultureOperation operation, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = operation.Id;
		_name = operation.Name;
		Description = operation.Description;
		OperationType = (AgricultureOperationType)operation.OperationType;
		TargetType = (AgricultureTargetType)operation.TargetType;
		RequiredUse = (AgricultureFieldUse)operation.RequiredUse;
		ResultUse = (AgricultureFieldUse)operation.ResultUse;
		_projectId = operation.ProjectId;
		_projectRevisionNumber = operation.ProjectRevisionNumber;
		_completionProgId = operation.CompletionProgId;
		LoadDefinition(operation.Definition);
	}

	public AgricultureOperation(IFuturemud gameworld, string name, string description,
		AgricultureOperationType operationType, AgricultureTargetType targetType, AgricultureFieldUse requiredUse,
		AgricultureFieldUse resultUse, IProject project, IReadOnlyDictionary<AgricultureScoreType, int> deltas,
		double woodlandYieldMultiplier = 0.0, int woodlandYieldCost = 0,
		IEnumerable<AgricultureFieldUse> allowedUses = null, int apiaryInstallHiveCount = 0,
		int apiaryPollinationRadius = 0, int apiaryTendHealthDelta = 0, int apiaryTendStoresDelta = 0,
		int apiaryTendYieldDelta = 0, double apiaryYieldMultiplier = 0.0, int apiaryYieldCost = 0,
		IEnumerable<AgricultureCommodityYield> apiaryYieldOutputs = null)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		OperationType = operationType;
		TargetType = targetType;
		RequiredUse = requiredUse;
		ResultUse = resultUse;
		foreach (var use in allowedUses ?? new[] { requiredUse })
		{
			_allowedUses.Add(use);
		}

		_project = project;
		_projectId = project?.Id ?? 0;
		_projectRevisionNumber = project?.RevisionNumber ?? 0;
		WoodlandYieldMultiplier = Math.Max(0.0, woodlandYieldMultiplier);
		WoodlandYieldCost = System.Math.Clamp(woodlandYieldCost, 0, 100);
		ApiaryInstallHiveCount = Math.Max(0, apiaryInstallHiveCount);
		ApiaryPollinationRadius = Math.Max(0, apiaryPollinationRadius);
		ApiaryTendHealthDelta = apiaryTendHealthDelta;
		ApiaryTendStoresDelta = apiaryTendStoresDelta;
		ApiaryTendYieldDelta = apiaryTendYieldDelta;
		ApiaryYieldMultiplier = Math.Max(0.0, apiaryYieldMultiplier);
		ApiaryYieldCost = System.Math.Clamp(apiaryYieldCost, 0, 100);
		_apiaryYieldOutputs.AddRange(apiaryYieldOutputs ?? Enumerable.Empty<AgricultureCommodityYield>());
		foreach (var delta in deltas)
		{
			_scoreDeltas[delta.Key] = delta.Value;
		}

		using (new FMDB())
		{
			var dbitem = new Models.AgricultureOperation
			{
				Name = Name,
				Description = Description,
				OperationType = (int)OperationType,
				TargetType = (int)TargetType,
				RequiredUse = (int)RequiredUse,
				ResultUse = (int)ResultUse,
				ProjectId = _projectId,
				ProjectRevisionNumber = _projectRevisionNumber,
				CompletionProgId = _completionProgId,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AgricultureOperations.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureOperation";
	public string Description { get; private set; }
	public AgricultureOperationType OperationType { get; private set; }
	public AgricultureTargetType TargetType { get; private set; }
	public AgricultureFieldUse RequiredUse { get; private set; }
	public AgricultureFieldUse ResultUse { get; private set; }
	public IReadOnlyCollection<AgricultureFieldUse> AllowedUses => _allowedUses;
	public IReadOnlyDictionary<AgricultureScoreType, int> ScoreDeltas => _scoreDeltas;
	public double WoodlandYieldMultiplier { get; private set; }
	public int WoodlandYieldCost { get; private set; }
	public int ApiaryInstallHiveCount { get; private set; }
	public int ApiaryPollinationRadius { get; private set; }
	public int ApiaryTendHealthDelta { get; private set; }
	public int ApiaryTendStoresDelta { get; private set; }
	public int ApiaryTendYieldDelta { get; private set; }
	public double ApiaryYieldMultiplier { get; private set; }
	public int ApiaryYieldCost { get; private set; }
	public IReadOnlyCollection<AgricultureCommodityYield> ApiaryYieldOutputs => _apiaryYieldOutputs;

	public void BuildingSetName(string name)
	{
		_name = name;
		Changed = true;
	}

	public void BuildingSetDescription(string description)
	{
		Description = description;
		Changed = true;
	}

	public void BuildingSetOperationType(AgricultureOperationType type)
	{
		OperationType = type;
		Changed = true;
	}

	public void BuildingSetTargetType(AgricultureTargetType type)
	{
		TargetType = type;
		Changed = true;
	}

	public void BuildingSetRequiredUse(AgricultureFieldUse use)
	{
		RequiredUse = use;
		_allowedUses.Clear();
		_allowedUses.Add(use);
		Changed = true;
	}

	public void BuildingSetResultUse(AgricultureFieldUse use)
	{
		ResultUse = use;
		Changed = true;
	}

	public void BuildingSetProject(IProject project)
	{
		_project = project;
		_projectId = project?.Id ?? 0;
		_projectRevisionNumber = project?.RevisionNumber ?? 0;
		Changed = true;
	}

	public void BuildingSetCompletionProg(IFutureProg prog)
	{
		_completionProg = prog;
		_completionProgId = prog?.Id;
		Changed = true;
	}

	public void BuildingSetScoreDelta(AgricultureScoreType score, int delta)
	{
		_scoreDeltas[score] = delta;
		Changed = true;
	}

	public void BuildingSetWoodlandYield(double multiplier, int cost)
	{
		WoodlandYieldMultiplier = Math.Max(0.0, multiplier);
		WoodlandYieldCost = System.Math.Clamp(cost, 0, 100);
		Changed = true;
	}

	public void BuildingSetAllowedUse(AgricultureFieldUse use, bool allowed)
	{
		if (allowed)
		{
			_allowedUses.Add(use);
		}
		else
		{
			_allowedUses.Remove(use);
		}

		if (!_allowedUses.Any())
		{
			_allowedUses.Add(RequiredUse);
		}

		Changed = true;
	}

	public void BuildingSetApiaryInstallation(int hiveCount, int pollinationRadius)
	{
		ApiaryInstallHiveCount = Math.Max(0, hiveCount);
		ApiaryPollinationRadius = Math.Max(0, pollinationRadius);
		Changed = true;
	}

	public void BuildingSetApiaryTending(int healthDelta, int storesDelta, int yieldDelta)
	{
		ApiaryTendHealthDelta = healthDelta;
		ApiaryTendStoresDelta = storesDelta;
		ApiaryTendYieldDelta = yieldDelta;
		Changed = true;
	}

	public void BuildingSetApiaryYield(double multiplier, int cost, IEnumerable<AgricultureCommodityYield> outputs)
	{
		ApiaryYieldMultiplier = Math.Max(0.0, multiplier);
		ApiaryYieldCost = System.Math.Clamp(cost, 0, 100);
		_apiaryYieldOutputs.Clear();
		_apiaryYieldOutputs.AddRange(outputs ?? Enumerable.Empty<AgricultureCommodityYield>());
		Changed = true;
	}

	public IProject Project
	{
		get
		{
			if (_project == null && _projectId != 0)
			{
				_project = _projectRevisionNumber == 0
					? Gameworld.Projects.Get(_projectId)
					: Gameworld.Projects.Get(_projectId, _projectRevisionNumber);
			}

			return _project;
		}
	}

	public IFutureProg CompletionProg
	{
		get
		{
			if (_completionProg == null && _completionProgId.HasValue)
			{
				_completionProg = Gameworld.FutureProgs.Get(_completionProgId.Value);
			}

			return _completionProg;
		}
	}

	public bool CanApply(IAgricultureField field, IFrameworkItem target)
	{
		return string.IsNullOrEmpty(WhyCannotApply(field, target));
	}

	public string WhyCannotApply(IAgricultureField field, IFrameworkItem target)
	{
		if (field == null)
		{
			return "There is no field to apply this operation to.";
		}

		if (!(_allowedUses.Any() ? _allowedUses : new HashSet<AgricultureFieldUse> { RequiredUse }).Contains(field.CurrentUse))
		{
			var uses = (_allowedUses.Any() ? _allowedUses : new HashSet<AgricultureFieldUse> { RequiredUse })
			           .OrderBy(x => (int)x)
			           .Select(x => x.DescribeEnum().ToLowerInvariant())
			           .ListToString();
			return $"This operation can only be used on {uses} fields.";
		}

		if (OperationType == AgricultureOperationType.Harvest &&
		    (field.CurrentCrop == null ||
		     field.CropStage is not (AgricultureCropStage.Harvestable or AgricultureCropStage.Overripe)))
		{
			return "There is no harvest-ready crop in this field.";
		}

		switch (OperationType)
		{
			case AgricultureOperationType.InstallApiary:
				if (field.HasActiveApiary)
				{
					return "This field already has an apiary installation.";
				}

				break;
			case AgricultureOperationType.TendApiary:
			case AgricultureOperationType.RemoveApiary:
				if (!field.HasActiveApiary)
				{
					return "This field does not have an apiary installation.";
				}

				break;
			case AgricultureOperationType.HarvestApiary:
				if (!field.HasActiveApiary)
				{
					return "This field does not have an apiary installation.";
				}

				if (field.Apiary.Stores <= 0 || field.Apiary.YieldPotential <= 0)
				{
					return "The apiary does not have harvestable stores.";
				}

				break;
		}

		if (TargetType == AgricultureTargetType.None)
		{
			return string.Empty;
		}

		if (target == null)
		{
			return $"This operation needs a {TargetType.DescribeEnum().ToLowerInvariant()} target.";
		}

		if (TargetType == AgricultureTargetType.Crop &&
		    target is IAgricultureCropDefinition crop)
		{
			if (OperationType == AgricultureOperationType.Sow && crop.IsPerennial)
			{
				return "Perennial orchard and vineyard crops must be planted with an orchard planting operation.";
			}

			if (OperationType == AgricultureOperationType.PlantOrchard && !crop.IsPerennial)
			{
				return "Annual field crops must be sown with a crop sowing operation.";
			}

			if (OperationType is AgricultureOperationType.Sow or AgricultureOperationType.PlantOrchard &&
			    !crop.CanPlantIn(field, out var seasonReason))
			{
				return seasonReason;
			}
		}

		return TargetType switch
		{
			AgricultureTargetType.Crop when target is not IAgricultureCropDefinition => "This operation needs a crop target.",
			AgricultureTargetType.Herd when target is not IAgricultureHerdDefinition => "This operation needs a herd target.",
			AgricultureTargetType.Woodland when target is not IAgricultureWoodlandDefinition => "This operation needs a woodland target.",
			_ => string.Empty
		};
	}

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Operation");
		WoodlandYieldMultiplier = Math.Max(0.0, (double?)root.Attribute("woodlandYieldMultiplier") ?? 0.0);
		WoodlandYieldCost = System.Math.Clamp((int?)root.Attribute("woodlandYieldCost") ?? 0, 0, 100);
		_allowedUses.Clear();
		var allowedRoot = root.Element("AllowedUses");
		foreach (var use in allowedRoot?.LoadUses(defaultAll: false) ?? Enumerable.Empty<AgricultureFieldUse>())
		{
			_allowedUses.Add(use);
		}

		if (!_allowedUses.Any())
		{
			_allowedUses.Add(RequiredUse);
		}

		var apiaryRoot = root.Element("Apiary");
		ApiaryInstallHiveCount = Math.Max(0, (int?)apiaryRoot?.Attribute("installHives") ?? 0);
		ApiaryPollinationRadius = Math.Max(0, (int?)apiaryRoot?.Attribute("pollinationRadius") ?? 0);
		ApiaryTendHealthDelta = (int?)apiaryRoot?.Attribute("tendHealthDelta") ?? 0;
		ApiaryTendStoresDelta = (int?)apiaryRoot?.Attribute("tendStoresDelta") ?? 0;
		ApiaryTendYieldDelta = (int?)apiaryRoot?.Attribute("tendYieldDelta") ?? 0;
		ApiaryYieldMultiplier = Math.Max(0.0, (double?)apiaryRoot?.Attribute("yieldMultiplier") ?? 0.0);
		ApiaryYieldCost = System.Math.Clamp((int?)apiaryRoot?.Attribute("yieldCost") ?? 0, 0, 100);
		_apiaryYieldOutputs.Clear();
		foreach (var element in apiaryRoot?.Element("Outputs")?.Elements("Commodity") ?? Enumerable.Empty<XElement>())
		{
			var material = (string)element.Attribute("material");
			var weight = (double?)element.Attribute("weight") ?? 0.0;
			if (string.IsNullOrWhiteSpace(material) || weight <= 0.0)
			{
				continue;
			}

			_apiaryYieldOutputs.Add(new AgricultureCommodityYield(material, weight, (string)element.Attribute("tag") ?? string.Empty));
		}

		foreach (var element in root.Elements("Score"))
		{
			if (!AgricultureScoreTypeExtensions.TryParseScoreType((string)element.Attribute("type"), Gameworld, out var type, true))
			{
				continue;
			}

			_scoreDeltas[type] = (int?)element.Attribute("value") ?? 0;
		}
	}

	private XElement SaveDefinition()
	{
		return new XElement("Operation",
			new XAttribute("woodlandYieldMultiplier", WoodlandYieldMultiplier),
			new XAttribute("woodlandYieldCost", WoodlandYieldCost),
			new XElement("AllowedUses",
				new XAttribute("uses", _allowedUses.OrderBy(x => (int)x).Select(x => x.ToString()).ListToCommaSeparatedValues())),
			new XElement("Apiary",
				new XAttribute("installHives", ApiaryInstallHiveCount),
				new XAttribute("pollinationRadius", ApiaryPollinationRadius),
				new XAttribute("tendHealthDelta", ApiaryTendHealthDelta),
				new XAttribute("tendStoresDelta", ApiaryTendStoresDelta),
				new XAttribute("tendYieldDelta", ApiaryTendYieldDelta),
				new XAttribute("yieldMultiplier", ApiaryYieldMultiplier),
				new XAttribute("yieldCost", ApiaryYieldCost),
				new XElement("Outputs",
					_apiaryYieldOutputs.Select(x => new XElement("Commodity",
						new XAttribute("material", x.MaterialName),
						new XAttribute("weight", x.BaseWeight),
						string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName))))),
			_scoreDeltas.Select(x => new XElement("Score",
				new XAttribute("type", x.Key.ToString()),
				new XAttribute("value", x.Value))));
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureOperations.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.OperationType = (int)OperationType;
		dbitem.TargetType = (int)TargetType;
		dbitem.RequiredUse = (int)RequiredUse;
		dbitem.ResultUse = (int)ResultUse;
		dbitem.ProjectId = _projectId;
		dbitem.ProjectRevisionNumber = _projectRevisionNumber;
		dbitem.CompletionProgId = _completionProgId;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}
}
