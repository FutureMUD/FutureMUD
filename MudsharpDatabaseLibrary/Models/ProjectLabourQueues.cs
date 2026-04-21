using System;

namespace MudSharp.Models;

public partial class ProjectLabourQueue
{
	public long Id { get; set; }
	public long CharacterId { get; set; }
	public long ActiveProjectId { get; set; }
	public long ProjectLabourRequirementId { get; set; }
	public int QueueOrder { get; set; }
	public DateTime QueuedAt { get; set; }

	public virtual ActiveProject ActiveProject { get; set; }
	public virtual Character Character { get; set; }
	public virtual ProjectLabourRequirement ProjectLabourRequirement { get; set; }
}
