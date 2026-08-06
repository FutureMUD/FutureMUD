using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;

namespace MudSharp.Character;

public partial class Character
{
	private void SaveProjects(MudSharp.Models.Character character)
	{
		character.CurrentProjectId = _currentProject.Project?.Id;
		character.CurrentProjectLabourId = _currentProject.Labour?.Id;
		character.CurrentProjectHours = CurrentProjectHours;
		character.CurrentProjectProjectHours = CurrentProjectProjectHours;
		character.ProjectLabourQueueLooping = ProjectQueueOwner._projectLabourQueueLooping;

		FMDB.Context.ProjectLabourQueues.RemoveRange(character.ProjectLabourQueues);
		foreach (var entry in ProjectQueueOwner._projectLabourQueue.OrderBy(x => x.QueueOrder))
		{
			character.ProjectLabourQueues.Add(new MudSharp.Models.ProjectLabourQueue
			{
				Character = character,
				ActiveProjectId = entry.ProjectId,
				ProjectId = entry.ProjectDefinitionId,
				ProjectLabourRequirementId = entry.LabourId,
				EntryType = (int)entry.EntryType,
				LabourPreference = entry.LabourPreference,
				CompletionMode = (int)entry.CompletionMode,
				TargetHours = entry.TargetHours,
				ElapsedHours = entry.ElapsedHours,
				WatchedPhaseId = entry.WatchedPhaseId,
				ClaimingCharacterInstanceId = entry.ClaimingCharacterInstanceId,
				QueueOrder = entry.QueueOrder,
				QueuedAt = entry.QueuedAt
			});
		}
	}

	private void LoadProjects(MudSharp.Models.Character character)
	{
		_personalProjects.AddRange(character.ActiveProjects.Select(x => Gameworld.ActiveProjects.Get(x.Id)).Where(x => x != null));
		var primaryInstance = character.CharacterInstances
		                               .Where(x => x.IsPrimary)
		                               .OrderBy(x => x.Id)
		                               .FirstOrDefault();
		var currentProjectId = primaryInstance?.CurrentProjectId ?? character.CurrentProjectId;
		var currentProjectLabourId = primaryInstance?.CurrentProjectLabourId ?? character.CurrentProjectLabourId;
		var currentProjectHours = primaryInstance?.CurrentProjectHours ?? character.CurrentProjectHours;
		var currentProjectProjectHours = primaryInstance?.CurrentProjectProjectHours ??
		                                 character.CurrentProjectProjectHours;
		var project = Gameworld.ActiveProjects.Get(currentProjectId ?? 0);
		_currentProject = (project,
			project?.CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == currentProjectLabourId));
		_currentProjectHours = currentProjectHours;
		_currentProjectProjectHours = currentProjectId.HasValue &&
		                              currentProjectProjectHours <= 0.0
			? currentProjectHours
			: currentProjectProjectHours;
		_projectLabourQueue.Clear();
		_projectLabourQueueLooping = character.ProjectLabourQueueLooping;
		foreach (var queue in character.ProjectLabourQueues.OrderBy(x => x.QueueOrder))
		{
			_projectLabourQueue.Add(new ProjectLabourQueueEntry(queue, Gameworld, this));
		}
	}

	private void LoadInstanceProject(MudSharp.Models.CharacterInstance instance)
	{
		var project = Gameworld.ActiveProjects.Get(instance.CurrentProjectId ?? 0);
		_currentProject = (project,
			project?.CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == instance.CurrentProjectLabourId));
		_currentProjectHours = instance.CurrentProjectHours;
		_currentProjectProjectHours = instance.CurrentProjectId.HasValue &&
		                              instance.CurrentProjectProjectHours <= 0.0
			? instance.CurrentProjectHours
			: instance.CurrentProjectProjectHours;
	}

	private void SaveInstanceProject(MudSharp.Models.CharacterInstance instance)
	{
		instance.CurrentProjectId = _currentProject.Project?.Id;
		instance.CurrentProjectLabourId = _currentProject.Labour?.Id;
		instance.CurrentProjectHours = _currentProjectHours;
		instance.CurrentProjectProjectHours = _currentProjectProjectHours;
	}

	private readonly List<IActiveJob> _activeJobs = new();
	public IEnumerable<IActiveJob> ActiveJobs => _activeJobs;

	public void AddJob(IActiveJob job)
	{
		_activeJobs.Add(job);
		Changed = true;
	}

	public void RemoveJob(IActiveJob job)
	{
		_activeJobs.Remove(job);
		Changed = true;
	}

	private readonly List<IActiveProject> _personalProjects = new();
	public IEnumerable<IPersonalProject> PersonalProjects => ProjectIdentityOwner._personalProjects.OfType<IPersonalProject>();
	private readonly List<ProjectLabourQueueEntry> _projectLabourQueue = new();
	private bool _projectLabourQueueLooping;
	public IEnumerable<IProjectLabourQueueEntry> ProjectLabourQueue =>
		ProjectQueueOwner._projectLabourQueue.OrderBy(x => x.QueueOrder);
	public bool ProjectLabourQueueLooping => ProjectQueueOwner._projectLabourQueueLooping;

	private (IActiveProject Project, IProjectLabourRequirement Labour) _currentProject;

	private Character ProjectIdentityOwner =>
		!IsPrimaryInstance && Identity is Character identity ? identity : this;

	private Character ProjectQueueOwner => ProjectIdentityOwner;

	public (IActiveProject Project, IProjectLabourRequirement Labour) CurrentProject
	{
		get => _currentProject;
		set
		{
			var oldProject = _currentProject.Project;
			var oldLabour = _currentProject.Labour;
			_currentProject = value;
			if (oldProject != value.Project)
			{
				_currentProjectHours = 0.0;
				_currentProjectProjectHours = 0.0;
			}
			else if (oldLabour != value.Labour)
			{
				_currentProjectHours = 0.0;
			}

			Changed = true;
		}
	}

	private double _currentProjectHours;

	public double CurrentProjectHours
	{
		get => _currentProjectHours;
		set
		{
			_currentProjectHours = value;
			Changed = true;
		}
	}

	private double _currentProjectProjectHours;

	public double CurrentProjectProjectHours
	{
		get => _currentProjectProjectHours;
		set
		{
			_currentProjectProjectHours = value;
			Changed = true;
		}
	}

	public void AddPersonalProject(IActiveProject project)
	{
		var owner = ProjectIdentityOwner;
		owner._personalProjects.Add(project);
		owner.Changed = true;
	}

	public void RemovePersonalProject(IActiveProject project)
	{
		var owner = ProjectIdentityOwner;
		owner._personalProjects.Remove(project);
		owner.Changed = true;
		if (CurrentProject.Project == project)
		{
			CurrentProject = (null, null);
			CurrentProjectHours = 0.0;
			CurrentProjectProjectHours = 0.0;
		}
	}

	public IProjectLabourQueueEntry QueueProjectLabour(IActiveProject project, IProjectLabourRequirement labour)
	{
		return QueueProjectLabour(project, labour.Name, ProjectLabourQueueCompletionMode.JoinOnce, 0.0);
	}

	public IProjectLabourQueueEntry QueueProjectLabour(IActiveProject project, string labourPreference,
		ProjectLabourQueueCompletionMode completionMode, double targetHours)
	{
		var owner = ProjectQueueOwner;
		var entry = new ProjectLabourQueueEntry(owner, project, null, owner._projectLabourQueue.Count + 1,
			labourPreference, completionMode, targetHours);
		owner._projectLabourQueue.Add(entry);
		owner.Changed = true;
		return entry;
	}

	public IProjectLabourQueueEntry QueueProjectStart(IProject project, string labourPreference,
		ProjectLabourQueueCompletionMode completionMode, double targetHours)
	{
		var owner = ProjectQueueOwner;
		var entry = new ProjectLabourQueueEntry(owner, project, labourPreference, completionMode, targetHours,
			owner._projectLabourQueue.Count + 1);
		owner._projectLabourQueue.Add(entry);
		owner.Changed = true;
		return entry;
	}

	public bool SetProjectLabourQueueMode(int position, ProjectLabourQueueCompletionMode completionMode,
		double targetHours)
	{
		var entry = ProjectQueueOwner.QueueEntryAt(position);
		if (entry is null || (completionMode == ProjectLabourQueueCompletionMode.Duration && targetHours <= 0.0))
		{
			return false;
		}

		entry.SetCompletionMode(completionMode, targetHours);
		ProjectQueueOwner.Changed = true;
		return true;
	}

	public bool SetProjectLabourQueueLabour(int position, string labourPreference)
	{
		var entry = ProjectQueueOwner.QueueEntryAt(position);
		if (entry is null)
		{
			return false;
		}

		entry.SetLabourPreference(labourPreference);
		ProjectQueueOwner.Changed = true;
		return true;
	}

	public bool MoveProjectQueueEntry(int position, int newPosition)
	{
		var owner = ProjectQueueOwner;
		var entries = owner._projectLabourQueue.OrderBy(x => x.QueueOrder).ToList();
		if (position < 1 || position > entries.Count || newPosition < 1 || newPosition > entries.Count)
		{
			return false;
		}

		var entry = entries[position - 1];
		entries.RemoveAt(position - 1);
		entries.Insert(newPosition - 1, entry);
		for (var i = 0; i < entries.Count; i++)
		{
			entries[i].QueueOrder = i + 1;
		}

		owner.Changed = true;
		return true;
	}

	public bool SetProjectLabourQueueLooping(bool looping)
	{
		var owner = ProjectQueueOwner;
		if (looping && owner._projectLabourQueue.Any(x => x.CompletionMode == ProjectLabourQueueCompletionMode.JoinOnce))
		{
			return false;
		}

		owner._projectLabourQueueLooping = looping;
		owner.Changed = true;
		return true;
	}

	public bool RemoveProjectQueueEntry(int position)
	{
		var owner = ProjectQueueOwner;
		var entry = owner.QueueEntryAt(position);
		if (entry == null)
		{
			return false;
		}

		owner._projectLabourQueue.Remove(entry);
		owner.RenumberProjectQueue();
		owner.Changed = true;
		return true;
	}

	public void ClearProjectQueue()
	{
		var owner = ProjectQueueOwner;
		if (!owner._projectLabourQueue.Any())
		{
			return;
		}

		owner._projectLabourQueue.Clear();
		owner.Changed = true;
	}

	public void HandleProjectQueuePhaseChange(IActiveProject project)
	{
		var owner = ProjectQueueOwner;
		foreach (var entry in owner._projectLabourQueue
			.Where(x => x.IsLinkedTo(project) && x.ClaimingCharacterInstanceId.HasValue)
			.ToList())
		{
			if (entry.DurationReached || entry.CompletionMode == ProjectLabourQueueCompletionMode.PhaseCompletion)
			{
				owner.CompleteQueueCycle(entry);
			}
		}
	}

	public void HandleProjectQueueProjectEnd(IActiveProject project)
	{
		var owner = ProjectQueueOwner;
		foreach (var entry in owner._projectLabourQueue.Where(x => x.IsLinkedTo(project)).ToList())
		{
			entry.ClearActiveProjectLink();
			owner.CompleteQueueCycle(entry);
		}
	}

	public void RecordQueuedProjectLabour(IActiveProject project, double fundedHours)
	{
		var entry = ProjectQueueOwner._projectLabourQueue
			.FirstOrDefault(x => x.IsLinkedTo(project) && x.IsClaimedBy(this));
		entry?.AddFundedHours(fundedHours);
		if (entry is not null)
		{
			ProjectQueueOwner.Changed = true;
		}
	}

	public bool CompleteQueuedDurationIfReached(IActiveProject project)
	{
		var owner = ProjectQueueOwner;
		var entry = owner._projectLabourQueue
			.FirstOrDefault(x => x.IsLinkedTo(project) && x.IsClaimedBy(this) && x.DurationReached);
		if (entry is null)
		{
			return false;
		}

		owner.CompleteQueueCycle(entry);
		return true;
	}

	public void CompleteQueuedProjectLabour(IActiveProject project)
	{
		var owner = ProjectQueueOwner;
		var entry = owner._projectLabourQueue
			.FirstOrDefault(x => x.IsLinkedTo(project) && x.IsClaimedBy(this));
		if (entry is not null)
		{
			owner.CompleteQueueCycle(entry);
		}
	}

	public void ReleaseQueuedProjectLabourClaim(IActiveProject project)
	{
		var owner = ProjectQueueOwner;
		var entry = owner._projectLabourQueue
			.FirstOrDefault(x => x.IsLinkedTo(project) && x.IsClaimedBy(this));
		if (entry is null)
		{
			return;
		}

		entry.ReleaseClaim();
		owner.Changed = true;
	}

	public bool TryJoinQueuedProjectLabour()
	{
		if (CurrentProject.Project != null)
		{
			return false;
		}

		var owner = ProjectQueueOwner;
		var claimed = owner._projectLabourQueue.FirstOrDefault(x => x.IsClaimedBy(this));
		if (claimed is not null && owner.TryActivateQueueEntry(this, claimed))
		{
			return true;
		}
		if (claimed is not null)
		{
			claimed.ReleaseClaim();
			owner.Changed = true;
		}

		foreach (var next in owner._projectLabourQueue.OrderBy(x => x.QueueOrder).ToList())
		{
			if (next.ClaimingCharacterInstanceId.HasValue)
			{
				continue;
			}

			if (next.StatusFor(this) == ProjectLabourQueueStatus.Stale)
			{
				OutputHandler.Send($"Your queued project labour entry for {(next.Project?.Name ?? "an unknown project").ColourName()} has become stale and has been removed.");
				owner._projectLabourQueue.Remove(next);
				owner.RenumberProjectQueue();
				owner.Changed = true;
				continue;
			}

			if (owner.TryActivateQueueEntry(this, next))
			{
				return true;
			}
		}

		return false;
	}

	private void RenumberProjectQueue()
	{
		var i = 0;
		foreach (var entry in _projectLabourQueue.OrderBy(x => x.QueueOrder))
		{
			entry.QueueOrder = ++i;
		}
	}

	private ProjectLabourQueueEntry QueueEntryAt(int position)
	{
		return _projectLabourQueue.OrderBy(x => x.QueueOrder).ElementAtOrDefault(position - 1);
	}

	private bool TryActivateQueueEntry(ICharacter actor, ProjectLabourQueueEntry entry)
	{
		if (!entry.TryActivate(actor, out _, out var labour, out _))
		{
			return false;
		}

		if (entry.CompletionMode == ProjectLabourQueueCompletionMode.JoinOnce)
		{
			CompleteQueueCycle(entry);
		}
		else
		{
			entry.Claim(actor, labour!);
			Changed = true;
		}

		return true;
	}

	private void CompleteQueueCycle(ProjectLabourQueueEntry entry)
	{
		if (_projectLabourQueueLooping && entry.CompletionMode != ProjectLabourQueueCompletionMode.JoinOnce)
		{
			_projectLabourQueue.Remove(entry);
			entry.ResetForNextCycle();
			entry.QueueOrder = _projectLabourQueue.Count;
			_projectLabourQueue.Add(entry);
			RenumberProjectQueue();
			Changed = true;
			return;
		}

		_projectLabourQueue.Remove(entry);
		RenumberProjectQueue();
		Changed = true;
	}
}
