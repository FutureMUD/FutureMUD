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
	void Resolve();
}
