using System;

namespace MudSharp.Models;

public class AIStorytellerSituation
{
	public long Id { get; set; }
	public long AIStorytellerId { get; set; }
	public long? ScopeCharacterId { get; set; }
	public long? ScopeRoomId { get; set; }

	public virtual AIStoryteller AIStoryteller { get; set; }
	public virtual Character ScopeCharacter { get; set; }
	public virtual Cell ScopeRoom { get; set; }
	public string Name { get; set; }
	public string SituationText { get; set; }
	public DateTime CreatedOn { get; set; }
	public bool IsResolved { get; set; }
}
