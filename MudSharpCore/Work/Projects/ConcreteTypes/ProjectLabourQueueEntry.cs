#nullable enable
using MudSharp.Framework.Revision;

namespace MudSharp.Work.Projects.ConcreteTypes;

/// <summary>
/// A durable scheduler entry owned by a character identity. The active project and labour
/// are deliberately optional: start entries resolve them only when the scheduler claims them.
/// </summary>
public class ProjectLabourQueueEntry : IProjectLabourQueueEntry
{
	private readonly ICharacter _character;
	private readonly IFuturemud _gameworld;
	private long? _activeProjectId;
	private long? _projectDefinitionId;
	private long? _labourId;
	private IActiveProject? _project;
	private IProjectLabourRequirement? _labour;

	public ProjectLabourQueueEntry(ICharacter character, IActiveProject project, IProjectLabourRequirement? labour,
		int queueOrder, string? labourPreference = null,
		ProjectLabourQueueCompletionMode completionMode = ProjectLabourQueueCompletionMode.JoinOnce,
		double targetHours = 0.0)
	{
		_character = character;
		_gameworld = character.Gameworld;
		EntryType = ProjectLabourQueueEntryType.JoinActiveProject;
		_project = project;
		_activeProjectId = project.Id;
		_projectDefinitionId = project.ProjectDefinition?.Id;
		_labour = labour;
		_labourId = labour?.Id;
		LabourPreference = labourPreference ?? labour?.Name;
		CompletionMode = completionMode;
		TargetHours = completionMode == ProjectLabourQueueCompletionMode.Duration ? targetHours : 0.0;
		QueueOrder = queueOrder;
		QueuedAt = DateTime.UtcNow;
	}

	public ProjectLabourQueueEntry(ICharacter character, IProject project, string? labourPreference,
		ProjectLabourQueueCompletionMode completionMode, double targetHours, int queueOrder)
	{
		_character = character;
		_gameworld = character.Gameworld;
		EntryType = ProjectLabourQueueEntryType.StartProject;
		_projectDefinitionId = project.Id;
		LabourPreference = labourPreference;
		CompletionMode = completionMode;
		TargetHours = completionMode == ProjectLabourQueueCompletionMode.Duration ? targetHours : 0.0;
		QueueOrder = queueOrder;
		QueuedAt = DateTime.UtcNow;
	}

	public ProjectLabourQueueEntry(MudSharp.Models.ProjectLabourQueue queue, IFuturemud gameworld, ICharacter character)
	{
		Id = queue.Id;
		_character = character;
		_gameworld = gameworld;
		EntryType = (ProjectLabourQueueEntryType)queue.EntryType;
		_activeProjectId = queue.ActiveProjectId;
		_projectDefinitionId = queue.ProjectId;
		_labourId = queue.ProjectLabourRequirementId;
		LabourPreference = queue.LabourPreference;
		CompletionMode = (ProjectLabourQueueCompletionMode)queue.CompletionMode;
		TargetHours = queue.TargetHours;
		ElapsedHours = queue.ElapsedHours;
		WatchedPhaseId = queue.WatchedPhaseId;
		ClaimingCharacterInstanceId = queue.ClaimingCharacterInstanceId;
		QueueOrder = queue.QueueOrder;
		QueuedAt = queue.QueuedAt;
		_project = _activeProjectId.HasValue ? gameworld.ActiveProjects.Get(_activeProjectId.Value) : null;
		_labour = ResolveLabour(_project);
	}

	public long Id { get; }
	public ICharacter Character => _character;
	public ProjectLabourQueueEntryType EntryType { get; private set; }
	public long? ProjectDefinitionId => _projectDefinitionId;
	public IActiveProject? Project => ResolveProject();
	public IProjectLabourRequirement? Labour => ResolveLabour(Project);
	public string? LabourPreference { get; private set; }
	public ProjectLabourQueueCompletionMode CompletionMode { get; private set; }
	public double TargetHours { get; private set; }
	public double ElapsedHours { get; private set; }
	public long? WatchedPhaseId { get; private set; }
	public long? ClaimingCharacterInstanceId { get; private set; }
	public int QueueOrder { get; set; }
	public DateTime QueuedAt { get; }

	// Compatibility conveniences for consumers that present the queue.
	public long? ProjectId => _activeProjectId;
	public long? LabourId => _labourId;

	public ProjectLabourQueueStatus Status => StatusFor(_character);

	public bool IsClaimedBy(ICharacter character)
	{
		return ClaimingCharacterInstanceId == character.InstanceId;
	}

	public void Claim(ICharacter character, IProjectLabourRequirement labour)
	{
		ClaimingCharacterInstanceId = character.InstanceId;
		_labour = labour;
		_labourId = labour.Id;
		if (CompletionMode == ProjectLabourQueueCompletionMode.PhaseCompletion)
		{
			WatchedPhaseId = Project?.CurrentPhase?.Id;
		}
	}

	public void ReleaseClaim()
	{
		ClaimingCharacterInstanceId = null;
	}

	public void SetLabourPreference(string? labourPreference)
	{
		LabourPreference = labourPreference;
		_labour = null;
		_labourId = null;
	}

	public void SetCompletionMode(ProjectLabourQueueCompletionMode completionMode, double targetHours)
	{
		CompletionMode = completionMode;
		TargetHours = completionMode == ProjectLabourQueueCompletionMode.Duration ? targetHours : 0.0;
		ElapsedHours = 0.0;
		WatchedPhaseId = completionMode == ProjectLabourQueueCompletionMode.PhaseCompletion &&
			ClaimingCharacterInstanceId.HasValue
			? Project?.CurrentPhase?.Id
			: null;
	}

	public void AddFundedHours(double hours)
	{
		if (CompletionMode == ProjectLabourQueueCompletionMode.Duration)
		{
			ElapsedHours += hours;
		}
	}

	public bool DurationReached => CompletionMode == ProjectLabourQueueCompletionMode.Duration &&
		ElapsedHours >= TargetHours;

	public void ResetForNextCycle()
	{
		ElapsedHours = 0.0;
		WatchedPhaseId = null;
		ReleaseClaim();
		_labour = null;
		_labourId = null;
	}

	public void ClearActiveProjectLink()
	{
		_activeProjectId = null;
		_project = null;
		_labour = null;
		_labourId = null;
	}

	public bool IsLinkedTo(IActiveProject project)
	{
		return _activeProjectId == project.Id;
	}

	public ProjectLabourQueueStatus StatusFor(ICharacter character)
	{
		if (ClaimingCharacterInstanceId.HasValue && !IsClaimedBy(character))
		{
			return ProjectLabourQueueStatus.Claimed;
		}

		var project = ResolveProject();
		if (project is null)
		{
			if (EntryType == ProjectLabourQueueEntryType.JoinActiveProject)
			{
				return ProjectLabourQueueStatus.Stale;
			}

			var definition = ResolveCurrentProjectDefinition();
			if (definition is null)
			{
				return ProjectLabourQueueStatus.WaitingForRevision;
			}

			if (!definition.AppearInProjectList(character) || !definition.CanInitiateProject(character))
			{
				return ProjectLabourQueueStatus.WaitingForInitiation;
			}

			return StatusForLabour(character, definition.Phases.FirstOrDefault()?.LabourRequirements,
				false, null);
		}

		if (project.CurrentPhase is null)
		{
			return EntryType == ProjectLabourQueueEntryType.JoinActiveProject
				? ProjectLabourQueueStatus.Stale
				: ProjectLabourQueueStatus.WaitingForRevision;
		}

		if (project is ILocalProject localProject && !localProject.IsAtProjectSite(character))
		{
			return ProjectLabourQueueStatus.WaitingForLocation;
		}

		return StatusForLabour(character, project.CurrentPhase.LabourRequirements, true, project);
	}

	public bool TryActivate(ICharacter character, out IActiveProject? project,
		out IProjectLabourRequirement? labour, out ProjectLabourQueueStatus status)
	{
		project = ResolveProject();
		if (project is null && EntryType == ProjectLabourQueueEntryType.StartProject)
		{
			var definition = ResolveCurrentProjectDefinition();
			if (definition is null)
			{
				labour = null;
				status = ProjectLabourQueueStatus.WaitingForRevision;
				return false;
			}

			status = StatusFor(character);
			if (status != ProjectLabourQueueStatus.Ready)
			{
				labour = null;
				return false;
			}

			project = definition.InitiateProject(character);
			_project = project;
			_activeProjectId = project.Id;
			_character.Changed = true;
		}

		if (project is null)
		{
			labour = null;
			status = ProjectLabourQueueStatus.Stale;
			return false;
		}

		status = StatusFor(character);
		if (status != ProjectLabourQueueStatus.Ready)
		{
			labour = null;
			return false;
		}

		labour = SelectLabour(character, project.CurrentPhase!.LabourRequirements, true, project);
		if (labour is null || !project.TryJoinLabour(character, labour, true, out _))
		{
			status = ProjectLabourQueueStatus.WaitingForSlot;
			return false;
		}

		status = ProjectLabourQueueStatus.Ready;
		return true;
	}

	private IActiveProject? ResolveProject()
	{
		if (_project is not null || !_activeProjectId.HasValue)
		{
			return _project;
		}

		_project = _gameworld.ActiveProjects.Get(_activeProjectId.Value);
		return _project;
	}

	private IProject? ResolveCurrentProjectDefinition()
	{
		if (!_projectDefinitionId.HasValue)
		{
			return null;
		}

		return _gameworld.Projects.GetAll(_projectDefinitionId.Value)
			.FirstOrDefault(x => x.Status == RevisionStatus.Current);
	}

	private IProjectLabourRequirement? ResolveLabour(IActiveProject? project)
	{
		if (project?.CurrentPhase is null)
		{
			return null;
		}

		if (_labour is not null && project.CurrentPhase.LabourRequirements.Contains(_labour))
		{
			return _labour;
		}

		_labour = _labourId.HasValue
			? project.CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == _labourId.Value)
			: null;
		return _labour;
	}

	private ProjectLabourQueueStatus StatusForLabour(ICharacter character,
		IEnumerable<IProjectLabourRequirement>? requirements, bool requireJoinable, IActiveProject? project)
	{
		var roles = requirements?.ToList() ?? [];
		var labour = SelectLabour(character, roles, requireJoinable, project);
		if (labour is null)
		{
			if (string.IsNullOrWhiteSpace(LabourPreference) && roles.Count(x => x.CharacterIsQualified(character)) > 1)
			{
				return ProjectLabourQueueStatus.WaitingForLabourSelection;
			}

			return roles.Any(x => x.CharacterIsQualified(character))
				? ProjectLabourQueueStatus.WaitingForSlot
				: ProjectLabourQueueStatus.WaitingForQualification;
		}

		if (requireJoinable && project is not null && project.LabourPaymentRateFor(labour) > 0.0M &&
			!project.CanPayLabourContribution(labour, _gameworld.GetStaticDouble("ProjectProgressMultiplier"), out _))
		{
			return ProjectLabourQueueStatus.WaitingForFunding;
		}

		return ProjectLabourQueueStatus.Ready;
	}

	private IProjectLabourRequirement? SelectLabour(ICharacter character,
		IEnumerable<IProjectLabourRequirement> requirements, bool requireJoinable, IActiveProject? project)
	{
		var roles = requirements.Where(x => x.CharacterIsQualified(character)).ToList();
		if (_labourId.HasValue)
		{
			var stored = roles.FirstOrDefault(x => x.Id == _labourId.Value);
			if (stored is not null && (!requireJoinable || project!.CanJoinLabour(character, stored)))
			{
				return stored;
			}
		}

		if (!string.IsNullOrWhiteSpace(LabourPreference))
		{
			var preferred = roles.FirstOrDefault(x => x.Name.EqualTo(LabourPreference));
			if (preferred is not null && (!requireJoinable || project!.CanJoinLabour(character, preferred)))
			{
				return preferred;
			}
		}

		var selectable = requireJoinable ? roles.Where(x => project!.CanJoinLabour(character, x)).ToList() : roles;
		return selectable.Count == 1 ? selectable[0] : null;
	}
}
