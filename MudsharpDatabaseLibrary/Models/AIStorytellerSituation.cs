using System;

namespace MudSharp.Models;

public class  AIStorytellerSituation
{
	public long Id { get; set; }
	public long AIStorytellerId { get; set; }

	public virtual AIStoryteller AIStoryteller { get; set; }
	public string Name { get; set; }
	public string SituationText { get; set; }
	public DateTime CreatedOn { get; set; }
	public bool IsResolved { get; set; }
}
