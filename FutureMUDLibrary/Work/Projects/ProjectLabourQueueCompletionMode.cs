#nullable enable

namespace MudSharp.Work.Projects;

/// <summary>
/// The event that completes one turn through a project labour queue entry.
/// </summary>
public enum ProjectLabourQueueCompletionMode
{
	JoinOnce = 0,
	Duration = 1,
	PhaseCompletion = 2,
	ProjectCompletion = 3
}
