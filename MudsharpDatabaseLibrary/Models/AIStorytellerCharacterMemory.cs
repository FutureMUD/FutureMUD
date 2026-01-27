using System;

namespace MudSharp.Models;

public class AIStorytellerCharacterMemory
{
	public long Id { get; set; }
	public long AIStorytellerId { get; set; }
	public virtual AIStoryteller AIStoryteller { get; set; }
	public long CharacterId { get; set; }
	public virtual Character Character { get; set; }
	public string CharacterName { get; set; }
	public string MemoryTitle { get; set; }
	public string MemoryText { get; set; }
	public DateTime CreatedOn { get; set; }

}
