#nullable enable
using MudSharp.Character;
using System;

namespace MudSharp.Work.Projects;

public interface IProjectLabourQueueEntry
{
	long Id { get; }
	ICharacter Character { get; }
	IActiveProject Project { get; }
	IProjectLabourRequirement Labour { get; }
	int QueueOrder { get; }
	DateTime QueuedAt { get; }
	ProjectLabourQueueStatus Status { get; }
}
