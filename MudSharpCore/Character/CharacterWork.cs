using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;
using System.Collections.Generic;
using System.Linq;

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
		foreach (var entry in _projectLabourQueue.OrderBy(x => x.QueueOrder))
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
		var project = Gameworld.ActiveProjects.Get(character.CurrentProjectId ?? 0);
		_currentProject = (project,
			project?.CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == character.CurrentProjectLabourId));
		_currentProjectHours = character.CurrentProjectHours;
		_currentProjectProjectHours = character.CurrentProjectId.HasValue &&
		                              character.CurrentProjectProjectHours <= 0.0
			? character.CurrentProjectHours
			: character.CurrentProjectProjectHours;
		_projectLabourQueue.Clear();
		foreach (var queue in character.ProjectLabourQueues.OrderBy(x => x.QueueOrder))
		{
			_projectLabourQueue.Add(new ProjectLabourQueueEntry(queue, Gameworld, this));
		}
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
	public IEnumerable<IPersonalProject> PersonalProjects => _personalProjects.OfType<IPersonalProject>();
	private readonly List<ProjectLabourQueueEntry> _projectLabourQueue = new();
	public IEnumerable<IProjectLabourQueueEntry> ProjectLabourQueue => _projectLabourQueue.OrderBy(x => x.QueueOrder);

	private (IActiveProject Project, IProjectLabourRequirement Labour) _currentProject;

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
		_personalProjects.Add(project);
	}

	public void RemovePersonalProject(IActiveProject project)
	{
		_personalProjects.Remove(project);
		if (CurrentProject.Project == project)
		{
			CurrentProject = (null, null);
			CurrentProjectHours = 0.0;
			CurrentProjectProjectHours = 0.0;
		}
	}

	public IProjectLabourQueueEntry QueueProjectLabour(IActiveProject project, IProjectLabourRequirement labour)
	{
		var existing = _projectLabourQueue.FirstOrDefault(x => x.Project == project && x.Labour == labour);
		if (existing != null)
		{
			return existing;
		}

		var entry = new ProjectLabourQueueEntry(this, project, labour, _projectLabourQueue.Count + 1);
		_projectLabourQueue.Add(entry);
		Changed = true;
		return entry;
	}

	public bool RemoveProjectQueueEntry(int position)
	{
		var entry = _projectLabourQueue
			.OrderBy(x => x.QueueOrder)
			.ElementAtOrDefault(position - 1);
		if (entry == null)
		{
			return false;
		}

		_projectLabourQueue.Remove(entry);
		RenumberProjectQueue();
		Changed = true;
		return true;
	}

	public void ClearProjectQueue()
	{
		if (!_projectLabourQueue.Any())
		{
			return;
		}

		_projectLabourQueue.Clear();
		Changed = true;
	}

	public bool TryJoinQueuedProjectLabour()
	{
		if (CurrentProject.Project != null)
		{
			return false;
		}

		while (_projectLabourQueue.Any())
		{
			var next = _projectLabourQueue.OrderBy(x => x.QueueOrder).First();
			switch (next.Status)
			{
				case ProjectLabourQueueStatus.Stale:
					OutputHandler.Send(
						$"Your queued project labour entry for {(next.Project?.Name ?? "an unknown project").ColourName()} / {(next.Labour?.Name ?? "an unknown labour requirement").ColourValue()} has become stale and has been removed.");
					_projectLabourQueue.Remove(next);
					RenumberProjectQueue();
					Changed = true;
					continue;
				case ProjectLabourQueueStatus.Ready:
					_projectLabourQueue.Remove(next);
					RenumberProjectQueue();
					Changed = true;
					next.Project.Join(this, next.Labour);
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
