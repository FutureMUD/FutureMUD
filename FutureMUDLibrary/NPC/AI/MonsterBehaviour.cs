#nullable enable

namespace MudSharp.NPC.AI;

/// <summary>Ordered by defensive priority. SelfDefence is implicit, never a proactive hunting motive.</summary>
public enum MonsterMotive
{
	None,
	SelfDefence,
	Provocation,
	Territory,
	Condition,
	Scheduled,
	Hunger
}

public enum MonsterFeedingMode
{
	Off,
	Needs,
	AfterKill
}
