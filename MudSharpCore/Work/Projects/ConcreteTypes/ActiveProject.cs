using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Work.Projects.Impacts;

namespace MudSharp.Work.Projects.ConcreteTypes;

public abstract class ActiveProject : LateInitialisingItem, IActiveProject, ILazyLoadDuringIdleTime
{
	public sealed override string FrameworkItemType => "ActiveProject";

	protected ActiveProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Gameworld.SaveManager.AddLazyLoad(this);
		_id = project.Id;
		IdInitialised = true;
		ProjectDefinition = gameworld.Projects.Get(project.ProjectId, project.ProjectRevisionNumber);
		CurrentPhase = ProjectDefinition.Phases.First(x => x.Id == project.CurrentPhaseId);
		_name = ProjectDefinition.Name;
		_cachedActiveLabour = new List<(long CharacterId, IProjectLabourRequirement Labour)>(
			project.Characters.Select(x =>
				(x.Id, CurrentPhase.LabourRequirements.First(y => y.Id == x.CurrentProjectLabourId))));
		foreach (var labour in project.ActiveProjectLabours)
		{
			_labourProgress[CurrentPhase.LabourRequirements.First(x => x.Id == labour.ProjectLabourRequirementsId)] =
				labour.Progress;
		}

		foreach (var material in project.ActiveProjectMaterials)
		{
			_materialProgress[
					CurrentPhase.MaterialRequirements.First(x => x.Id == material.ProjectMaterialRequirementsId)] =
				material.Progress;
		}
	}

	protected ActiveProject(IProject project)
	{
		Gameworld = project.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		ProjectDefinition = project;
		CurrentPhase = ProjectDefinition.Phases.First();
		_name = ProjectDefinition.Name;
		_cachedActiveLabour = new List<(long CharacterId, IProjectLabourRequirement Labour)>();
	}

	protected void Delete()
	{
		_labourProgress.Clear();
		_materialProgress.Clear();
		_activeLabour.Clear();
		_cachedActiveLabour.Clear();
		CurrentPhase = null;
		Gameworld.Destroy(this);
		Changed = false;
		_noSave = true;
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Scheduler.Destroy(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ActiveProjects.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ActiveProjects.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	#region Overrides of FrameworkItem

	private static readonly Regex _projectNameRegex = new("@job", RegexOptions.IgnoreCase);

	/// <inheritdoc />
	public override string Name
	{
		get
		{
			return _projectNameRegex.Replace(_name, m =>
			{
				var job = Gameworld.ActiveJobs.FirstOrDefault(x => x.ActiveProject == this);
				if (job is not null)
				{
					return job.Name;
				}

				return "a job";
			});
		}
	}

	#endregion

	public IProject ProjectDefinition { get; protected set; }
	public IProjectPhase CurrentPhase { get; protected set; }

	protected readonly DoubleCounter<IProjectLabourRequirement> _labourProgress = new();
	public IReadOnlyDictionary<IProjectLabourRequirement, double> LabourProgress => _labourProgress;

	protected readonly DoubleCounter<IProjectMaterialRequirement> _materialProgress = new();
	public IReadOnlyDictionary<IProjectMaterialRequirement, double> MaterialProgress => _materialProgress;

	private readonly List<(long CharacterId, IProjectLabourRequirement Labour)> _cachedActiveLabour;

	protected void CheckCachedLabour()
	{
		if (_cachedActiveLabour.Count > 0)
		{
			foreach (var labour in _cachedActiveLabour)
			{
				_activeLabour.Add((Gameworld.TryGetCharacter(labour.CharacterId, true), labour.Labour));
			}

			_cachedActiveLabour.Clear();
		}
	}


	protected readonly List<(ICharacter Character, IProjectLabourRequirement Labour)> _activeLabour = new();

	public IEnumerable<(ICharacter Character, IProjectLabourRequirement Labour)> ActiveLabour
	{
		get
		{
			CheckCachedLabour();
			return _activeLabour;
		}
	}

	public abstract void Cancel(ICharacter actor);

	public abstract bool FulfilLabour(IProjectLabourRequirement labour, double progress);

	public abstract void FulfilMaterial(IProjectMaterialRequirement material, double progress);

	public abstract void Join(ICharacter actor, IProjectLabourRequirement labour);

	public abstract void Leave(ICharacter actor);

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.ActiveProjects.Find(Id);
		dbitem.CurrentPhaseId = CurrentPhase.Id;
		FMDB.Context.ActiveProjectLabours.RemoveRange(dbitem.ActiveProjectLabours);
		foreach (var labour in _labourProgress)
		{
			var dbprogress = new Models.ActiveProjectLabour();
			dbitem.ActiveProjectLabours.Add(dbprogress);
			dbprogress.ProjectLabourRequirementsId = labour.Key.Id;
			dbprogress.Progress = labour.Value;
		}

		FMDB.Context.ActiveProjectMaterials.RemoveRange(dbitem.ActiveProjectMaterials);
		foreach (var material in _materialProgress)
		{
			var dbprogress = new Models.ActiveProjectMaterial();
			dbitem.ActiveProjectMaterials.Add(dbprogress);
			dbprogress.ProjectMaterialRequirementsId = material.Key.Id;
			dbprogress.Progress = material.Value;
		}

		Changed = false;
	}

	public sealed override object DatabaseInsert()
	{
		var dbitem = new Models.ActiveProject();
		FMDB.Context.ActiveProjects.Add(dbitem);
		dbitem.CurrentPhaseId = CurrentPhase.Id;
		dbitem.ProjectId = ProjectDefinition.Id;
		dbitem.ProjectRevisionNumber = ProjectDefinition.RevisionNumber;
		foreach (var labour in _labourProgress)
		{
			var dbprogress = new Models.ActiveProjectLabour();
			dbitem.ActiveProjectLabours.Add(dbprogress);
			dbprogress.ProjectLabourRequirementsId = labour.Key.Id;
			dbprogress.Progress = labour.Value;
		}

		foreach (var material in _materialProgress)
		{
			var dbprogress = new Models.ActiveProjectMaterial();
			dbitem.ActiveProjectMaterials.Add(dbprogress);
			dbprogress.ProjectMaterialRequirementsId = material.Key.Id;
			dbprogress.Progress = material.Value;
		}

		DatabaseInsert(dbitem);
		return dbitem;
	}

	protected abstract void DatabaseInsert(MudSharp.Models.ActiveProject project);

	public sealed override void SetIDFromDatabase(object item)
	{
		var dbitem = (MudSharp.Models.ActiveProject)item;
		_id = dbitem.Id;
	}

	protected long _characterOwnerId;
	protected ICharacter _characterOwner;

	public ICharacter CharacterOwner
	{
		get
		{
			if (_characterOwner == null)
			{
				InitialiseCharacterOwner(false);
			}

			return _characterOwner;
		}
	}

	public virtual ICell Location { get; protected set; }

	public void DoLoad()
	{
		InitialiseCharacterOwner(true);
	}

	private void InitialiseCharacterOwner(bool viaSaveManager)
	{
		_characterOwner = Gameworld.TryGetCharacter(_characterOwnerId, true);
		CheckCachedLabour();
		if (!viaSaveManager)
		{
			Gameworld.SaveManager.AbortLazyLoad(this);
		}
	}

	public abstract string ProjectsCommandOutput(ICharacter actor);

	public virtual void DoProjectsTick()
	{
		var multiplier = Gameworld.GetStaticDouble("ProjectProgressMultiplier");
		foreach (var labour in ActiveLabour)
		{
			var supervisorMultiplier = ActiveLabour.Aggregate(1.0,
				(sum, y) => sum * y.Labour.ProgressMultiplierForOtherLabourPerPercentageComplete(labour.Labour, this));
			if (FulfilLabour(labour.Labour,
				    labour.Labour.HourlyProgress(labour.Character) * multiplier * supervisorMultiplier))
			{
				break;
			}
		}

		foreach (var labour in ActiveLabour)
		{
			labour.Character.CurrentProjectHours += 1.0 * multiplier;
			foreach (var impact in labour.Labour.LabourImpacts.OfType<ILabourImpactActionAtTick>())
			{
				impact.DoAction(labour.Character, this, labour.Labour);
			}
		}
	}

	#region Futureprogs

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Project;
	public object GetObject => this;

	public virtual IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "location":
				return Location;
			case "owner":
				return CharacterOwner;
			case "workers":
				return new CollectionVariable(ActiveLabour.Select(x => x.Character).ToList(),
					FutureProgVariableTypes.Character);
		}

		throw new ApplicationException("There was an invalid property requested in ActiveProject.GetProperty: " +
		                               property);
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "location", FutureProgVariableTypes.Location },
			{ "owner", FutureProgVariableTypes.Character },
			{ "workers", FutureProgVariableTypes.Character | FutureProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "location", "" },
			{ "owner", "" },
			{ "workers", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Project, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}