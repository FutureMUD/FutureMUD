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

		FMDB.Context.ProjectLabourQueues.RemoveRange(character.ProjectLabourQueues);
		foreach (var entry in ProjectQueueOwner._projectLabourQueue.OrderBy(x => x.QueueOrder))
		{
			if (entry.Project == null || entry.Labour == null)
			{
				continue;
			}

			character.ProjectLabourQueues.Add(new MudSharp.Models.ProjectLabourQueue
			{
				Character = character,
				ActiveProjectId = entry.ProjectId,
				ProjectLabourRequirementId = entry.LabourId,
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
	public IEnumerable<IProjectLabourQueueEntry> ProjectLabourQueue =>
		ProjectQueueOwner._projectLabourQueue.OrderBy(x => x.QueueOrder);

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
		var owner = ProjectQueueOwner;
		var existing = owner._projectLabourQueue.FirstOrDefault(x => x.Project == project && x.Labour == labour);
		if (existing != null)
		{
			return existing;
		}

		var entry = new ProjectLabourQueueEntry(owner, project, labour, owner._projectLabourQueue.Count + 1);
		owner._projectLabourQueue.Add(entry);
		owner.Changed = true;
		return entry;
	}

	public bool RemoveProjectQueueEntry(int position)
	{
		var owner = ProjectQueueOwner;
		var entry = owner._projectLabourQueue
			.OrderBy(x => x.QueueOrder)
			.ElementAtOrDefault(position - 1);
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

	public bool TryJoinQueuedProjectLabour()
	{
		if (CurrentProject.Project != null)
		{
			return false;
		}

		var owner = ProjectQueueOwner;
		while (owner._projectLabourQueue.Any())
		{
			var next = owner._projectLabourQueue.OrderBy(x => x.QueueOrder).First();
			switch (next.StatusFor(this))
			{
				case ProjectLabourQueueStatus.Stale:
					OutputHandler.Send(
						$"Your queued project labour entry for {(next.Project?.Name ?? "an unknown project").ColourName()} / {(next.Labour?.Name ?? "an unknown labour requirement").ColourValue()} has become stale and has been removed.");
					owner._projectLabourQueue.Remove(next);
					owner.RenumberProjectQueue();
					owner.Changed = true;
					continue;
				case ProjectLabourQueueStatus.Ready:
					owner._projectLabourQueue.Remove(next);
					owner.RenumberProjectQueue();
					owner.Changed = true;
					next.Project.TryJoinLabour(this, next.Labour, true, out _);
					return true;
				default:
					return false;
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
}
