using System;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStorytellerSituation : IFrameworkItem, ISaveable
{
	IAIStoryteller AIStoryteller { get; }
	string SituationText { get; }
	DateTime CreatedOn { get; }
	bool IsResolved { get; }
	long? ScopeCharacterId { get; }
	long? ScopeRoomId { get; }
	void Resolve();
	void UpdateSituation(string newTitle, string newSituationText);
	void SetScope(long? scopeCharacterId, long? scopeRoomId);
}
