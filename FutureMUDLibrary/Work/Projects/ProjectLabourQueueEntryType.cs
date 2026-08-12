#nullable enable

namespace MudSharp.Work.Projects;

/// <summary>
/// The source that a queued project-labour assignment should use when it is activated.
/// </summary>
public enum ProjectLabourQueueEntryType
{
	JoinActiveProject = 0,
	StartProject = 1
}
