using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureFieldProfile : IFrameworkItem, IHaveFuturemud, IProgVariable
{
	string Description { get; }
	IReadOnlyDictionary<AgricultureScoreType, int> DefaultScores { get; }
	bool AllowsUse(AgricultureFieldUse use);
}
