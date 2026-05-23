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
	IReadOnlyCollection<AgricultureFieldUse> AllowedUses { get; }
	IProject Project { get; }
	IFutureProg CompletionProg { get; }
	IReadOnlyDictionary<AgricultureScoreType, int> ScoreDeltas { get; }
	double WoodlandYieldMultiplier { get; }
	int WoodlandYieldCost { get; }
	int ApiaryInstallHiveCount { get; }
	int ApiaryPollinationRadius { get; }
	int ApiaryTendHealthDelta { get; }
	int ApiaryTendStoresDelta { get; }
	int ApiaryTendYieldDelta { get; }
	double ApiaryYieldMultiplier { get; }
	int ApiaryYieldCost { get; }
	IReadOnlyCollection<AgricultureCommodityYield> ApiaryYieldOutputs { get; }
	bool CanApply(IAgricultureField field, IFrameworkItem target);
	string WhyCannotApply(IAgricultureField field, IFrameworkItem target);
}
