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
		AgricultureFieldUse resultUse, IProject project, IReadOnlyDictionary<AgricultureScoreType, int> deltas)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		OperationType = operationType;
		TargetType = targetType;
		RequiredUse = requiredUse;
		ResultUse = resultUse;
		_project = project;
		_projectId = project?.Id ?? 0;
		_projectRevisionNumber = project?.RevisionNumber ?? 0;
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
	public IReadOnlyDictionary<AgricultureScoreType, int> ScoreDeltas => _scoreDeltas;

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

		if (RequiredUse != field.CurrentUse && OperationType != AgricultureOperationType.Improve &&
		    OperationType != AgricultureOperationType.Clear)
		{
			return $"This operation can only be used on {RequiredUse.DescribeEnum().ToLowerInvariant()} fields.";
		}

		if (OperationType == AgricultureOperationType.Harvest &&
		    (field.CurrentCrop == null ||
		     field.CropStage is not (AgricultureCropStage.Harvestable or AgricultureCropStage.Overripe)))
		{
			return "There is no harvest-ready crop in this field.";
		}

		if (TargetType == AgricultureTargetType.None)
		{
			return string.Empty;
		}

		if (target == null)
		{
			return $"This operation needs a {TargetType.DescribeEnum().ToLowerInvariant()} target.";
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
		foreach (var element in root.Elements("Score"))
		{
			if (!Enum.TryParse<AgricultureScoreType>((string)element.Attribute("type"), true, out var type))
			{
				continue;
			}

			_scoreDeltas[type] = (int?)element.Attribute("value") ?? 0;
		}
	}

	private XElement SaveDefinition()
	{
		return new XElement("Operation",
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
