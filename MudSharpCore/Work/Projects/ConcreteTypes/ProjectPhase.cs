using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ProjectPhase : SaveableItem, IProjectPhase
{
	public ProjectPhase(MudSharp.Models.ProjectPhase phase, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = phase.Id;
		_name = "Phase";
		Description = phase.Description;
		PhaseNumber = phase.PhaseNumber;
		foreach (var labour in phase.ProjectLabourRequirements)
		{
			_labourRequirements.Add(ProjectFactory.LoadLabour(labour, gameworld));
		}

		foreach (var material in phase.ProjectMaterialRequirements)
		{
			_materialRequirements.Add(ProjectFactory.LoadMaterial(material, gameworld));
		}

		foreach (var action in phase.ProjectActions)
		{
			_completionActions.Add(ProjectFactory.LoadAction(action, gameworld));
		}
	}

	public ProjectPhase(IFuturemud gameworld, IProject project)
	{
		Gameworld = gameworld;
		_name = "Phase";
		Description = "Performing all of the work.";
		PhaseNumber = project.Phases.Count() + 1;
		using (new FMDB())
		{
			var dbitem = new Models.ProjectPhase();
			FMDB.Context.ProjectPhases.Add(dbitem);
			dbitem.ProjectId = project.Id;
			dbitem.ProjectRevisionNumber = project.RevisionNumber;
			dbitem.Description = Description;
			dbitem.PhaseNumber = PhaseNumber;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public ProjectPhase(IProject project, IProjectPhase rhs)
	{
		Gameworld = project.Gameworld;
		_name = "Phase";
		Description = rhs.Description;
		PhaseNumber = project.Phases.Count() + 1;
		using (new FMDB())
		{
			var dbitem = new Models.ProjectPhase();
			FMDB.Context.ProjectPhases.Add(dbitem);
			dbitem.ProjectId = project.Id;
			dbitem.ProjectRevisionNumber = project.RevisionNumber;
			dbitem.Description = Description;
			dbitem.PhaseNumber = PhaseNumber;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		foreach (var labour in rhs.LabourRequirements)
		{
			_labourRequirements.Add(labour.Duplicate(this));
		}

		foreach (var material in rhs.MaterialRequirements)
		{
			_materialRequirements.Add(material.Duplicate(this));
		}

		foreach (var action in rhs.CompletionActions)
		{
			_completionActions.Add(action.Duplicate(this));
		}
	}

	public IProjectPhase Duplicate(IProject newProject)
	{
		return new ProjectPhase(newProject, this);
	}

	public int PhaseNumber
	{
		get => _phaseNumber;
		set
		{
			_phaseNumber = value;
			Changed = true;
		}
	}

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Changed = true;
		}
	}

	private readonly List<IProjectLabourRequirement> _labourRequirements = new();
	public IEnumerable<IProjectLabourRequirement> LabourRequirements => _labourRequirements;

	private readonly List<IProjectMaterialRequirement> _materialRequirements = new();
	private int _phaseNumber;
	private string _description;

	public IEnumerable<IProjectMaterialRequirement> MaterialRequirements => _materialRequirements;


	public override void Save()
	{
		var dbitem = FMDB.Context.ProjectPhases.Find(Id);
		dbitem.Description = Description;
		dbitem.PhaseNumber = PhaseNumber;
		Changed = false;
	}

	public override string FrameworkItemType => "ProjectPhase";

	public void Delete()
	{
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProjectPhases.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProjectPhases.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void AddLabour(IProjectLabourRequirement labour)
	{
		_labourRequirements.Add(labour);
	}

	public void RemoveLabour(IProjectLabourRequirement labour)
	{
		_labourRequirements.Remove(labour);
	}

	public void AddMaterial(IProjectMaterialRequirement material)
	{
		_materialRequirements.Add(material);
	}

	public void RemoveMaterial(IProjectMaterialRequirement material)
	{
		_materialRequirements.Remove(material);
	}

	public (bool Truth, string Error) CanSubmit()
	{
		if (!_labourRequirements.Any() && !_materialRequirements.Any())
		{
			return (false, "You must set at least one material requirement or labour requirement per phase.");
		}

		var failedLabour = _labourRequirements.Select(x => (Labour: x, Result: x.CanSubmit()))
		                                      .FirstOrDefault(x => !x.Result.Truth);
		if (failedLabour.Labour != null)
		{
			return (false, $"[LabourReq {failedLabour.Labour.Name}] {failedLabour.Result.Error}");
		}

		var failedMaterial = _materialRequirements.Select(x => (Material: x, Result: x.CanSubmit()))
		                                          .FirstOrDefault(x => !x.Result.Truth);
		if (failedMaterial.Material != null)
		{
			return (false, $"[MaterialReq {failedMaterial.Material.Name}] {failedMaterial.Result.Error}");
		}

		var failedAction = _completionActions.Select(x => (Action: x, Result: x.CanSubmit()))
		                                     .FirstOrDefault(x => !x.Result.Truth);
		if (failedAction.Action != null)
		{
			return (false, $"[Action {failedAction.Action.Name}] {failedAction.Result.Error}");
		}

		return (true, string.Empty);
	}

	private readonly List<IProjectAction> _completionActions = new();
	public IEnumerable<IProjectAction> CompletionActions => _completionActions;

	public void AddAction(IProjectAction action)
	{
		_completionActions.Add(action);
	}

	public void RemoveAction(IProjectAction action)
	{
		_completionActions.Remove(action);
	}
}