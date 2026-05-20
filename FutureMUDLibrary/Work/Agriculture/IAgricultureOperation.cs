using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Work.Projects;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureOperation : IFrameworkItem, IHaveFuturemud
{
	string Description { get; }
	AgricultureOperationType OperationType { get; }
	AgricultureTargetType TargetType { get; }
	AgricultureFieldUse RequiredUse { get; }
	AgricultureFieldUse ResultUse { get; }
	IProject Project { get; }
	IFutureProg CompletionProg { get; }
	IReadOnlyDictionary<AgricultureScoreType, int> ScoreDeltas { get; }
	bool CanApply(IAgricultureField field, IFrameworkItem target);
	string WhyCannotApply(IAgricultureField field, IFrameworkItem target);
}
