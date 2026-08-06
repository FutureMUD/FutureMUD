#nullable enable
using MudSharp.Character;
using System;

namespace MudSharp.Work.Projects;

public interface IProjectLabourQueueEntry
{
	long Id { get; }
	ICharacter Character { get; }
	ProjectLabourQueueEntryType EntryType { get; }
	long? ProjectDefinitionId { get; }
	IActiveProject? Project { get; }
	IProjectLabourRequirement? Labour { get; }
	string? LabourPreference { get; }
	ProjectLabourQueueCompletionMode CompletionMode { get; }
	double TargetHours { get; }
	double ElapsedHours { get; }
	long? WatchedPhaseId { get; }
	long? ClaimingCharacterInstanceId { get; }
	int QueueOrder { get; }
	DateTime QueuedAt { get; }
	ProjectLabourQueueStatus Status { get; }
}
