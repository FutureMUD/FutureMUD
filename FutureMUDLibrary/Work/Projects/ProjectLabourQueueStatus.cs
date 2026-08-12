#nullable enable

namespace MudSharp.Work.Projects;

public enum ProjectLabourQueueStatus
{
	Ready = 0,
	WaitingForSlot = 1,
	WaitingForQualification = 2,
	WaitingForLocation = 3,
	Stale = 4,
	WaitingForLabourSelection = 5,
	WaitingForInitiation = 6,
	WaitingForRevision = 7,
	WaitingForFunding = 8,
	Claimed = 9
}
