using System.Collections.Generic;
using System.Text;
using Castle.Components.DictionaryAdapter;

namespace MudSharp.Models;

public class AIStoryteller
{
	public AIStoryteller()
	{
		Situations = new HashSet<AIStorytellerSituation>();
		CharacterMemories = new HashSet<AIStorytellerCharacterMemory>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Model { get; set; }
	public string SystemPrompt { get; set; }
	public string AttentionAgentPrompt { get; set; }
	public string SurveillanceStrategyDefinition { get; set; }
	public string ReasoningEffort { get; set; }
	public bool SubscribeToRoomEvents { get; set; }
	public bool SubscribeTo5mHeartbeat { get; set; }
	public bool SubscribeToHourHeartbeat { get; set; }
	public bool IsPaused { get; set; }

	public virtual ICollection<AIStorytellerSituation> Situations { get; set; }
	public virtual ICollection<AIStorytellerCharacterMemory> CharacterMemories { get; set; }
}
