#nullable enable
using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Linq;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ProjectLabourQueueEntry : IProjectLabourQueueEntry
{
	private readonly ICharacter _character;
	private readonly long _projectId;
	private readonly long _labourId;
	private IActiveProject? _project;
	private IProjectLabourRequirement? _labour;

	public ProjectLabourQueueEntry(ICharacter character, IActiveProject project, IProjectLabourRequirement labour,
		int queueOrder)
	{
		_character = character;
		_project = project;
		_projectId = project.Id;
		_labour = labour;
		_labourId = labour.Id;
		QueueOrder = queueOrder;
		QueuedAt = DateTime.UtcNow;
	}

	public ProjectLabourQueueEntry(MudSharp.Models.ProjectLabourQueue queue, IFuturemud gameworld, ICharacter character)
	{
		Id = queue.Id;
		_character = character;
		_projectId = queue.ActiveProjectId;
		_project = gameworld.ActiveProjects.Get(queue.ActiveProjectId);
		_labourId = queue.ProjectLabourRequirementId;
		_labour = _project?.ProjectDefinition.Phases
			.SelectMany(x => x.LabourRequirements)
			.FirstOrDefault(x => x.Id == queue.ProjectLabourRequirementId);
		QueueOrder = queue.QueueOrder;
		QueuedAt = queue.QueuedAt;
	}

	public long Id { get; }
	public ICharacter Character => _character;
	public IActiveProject Project => _project!;
	public IProjectLabourRequirement Labour => _labour!;
	public int QueueOrder { get; set; }
	public DateTime QueuedAt { get; }

	public long ProjectId => _projectId;
	public long LabourId => _labourId;

	public ProjectLabourQueueStatus Status
	{
		get
		{
			if (_project == null || _labour == null || _project.CurrentPhase == null)
			{
				return ProjectLabourQueueStatus.Stale;
			}

			if (!_project.CurrentPhase.LabourRequirements.Contains(_labour))
			{
				return ProjectLabourQueueStatus.Stale;
			}

			if (_project is ActiveProject activeProject &&
				_project is ILocalProject &&
				activeProject.Location != _character.Location)
			{
				return ProjectLabourQueueStatus.WaitingForLocation;
			}

			if (!_labour.CharacterIsQualified(_character))
			{
				return ProjectLabourQueueStatus.WaitingForQualification;
			}

			if (_project.ActiveLabour.Count(x => x.Labour == _labour) >= _labour.MaximumSimultaneousWorkers)
			{
				return ProjectLabourQueueStatus.WaitingForSlot;
			}

			return ProjectLabourQueueStatus.Ready;
		}
	}
}
