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
	public string CustomToolCallsDefinition { get; set; }
	public bool SubscribeTo5mHeartbeat { get; set; }
	public bool SubscribeTo10mHeartbeat { get; set; }
	public bool SubscribeTo30mHeartbeat { get; set; }
	public bool SubscribeToHourHeartbeat { get; set; }
	public long? HeartbeatStatus5mProgId { get; set; }
	public long? HeartbeatStatus10mProgId { get; set; }
	public long? HeartbeatStatus30mProgId { get; set; }
	public long? HeartbeatStatus1hProgId { get; set; }
	public virtual FutureProg HeartbeatStatus5mProg { get; set; }
	public virtual FutureProg HeartbeatStatus10mProg { get; set; }
	public virtual FutureProg HeartbeatStatus30mProg { get; set; }
	public virtual FutureProg HeartbeatStatus1hProg { get; set; }
	public bool IsPaused { get; set; }

	public virtual ICollection<AIStorytellerSituation> Situations { get; set; }
	public virtual ICollection<AIStorytellerCharacterMemory> CharacterMemories { get; set; }
	public bool SubscribeToRoomEvents { get; set; }
}
