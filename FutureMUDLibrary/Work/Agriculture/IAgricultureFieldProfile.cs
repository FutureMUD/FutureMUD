using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureFieldProfile : IFrameworkItem, IHaveFuturemud
{
	string Description { get; }
	IReadOnlyDictionary<AgricultureScoreType, int> DefaultScores { get; }
	bool AllowsUse(AgricultureFieldUse use);
}
