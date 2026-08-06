using System;

namespace MudSharp.Models;

public partial class ProjectLabourQueue
{
	public long Id { get; set; }
	public long CharacterId { get; set; }
	public long? ActiveProjectId { get; set; }
	public long? ProjectId { get; set; }
	public long? ProjectLabourRequirementId { get; set; }
	public int EntryType { get; set; }
	public string LabourPreference { get; set; }
	public int CompletionMode { get; set; }
	public double TargetHours { get; set; }
	public double ElapsedHours { get; set; }
	public long? WatchedPhaseId { get; set; }
	public long? ClaimingCharacterInstanceId { get; set; }
	public int QueueOrder { get; set; }
	public DateTime QueuedAt { get; set; }

	public virtual ActiveProject ActiveProject { get; set; }
	public virtual Character Character { get; set; }
	public virtual ProjectLabourRequirement ProjectLabourRequirement { get; set; }
	public virtual CharacterInstance ClaimingCharacterInstance { get; set; }
}
