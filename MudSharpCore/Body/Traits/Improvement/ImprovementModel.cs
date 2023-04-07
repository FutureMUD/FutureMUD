using System;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public abstract class ImprovementModel : FrameworkItem, IImprovementModel
{
	public sealed override string FrameworkItemType => "ImprovementModel";

	public abstract double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype);

	public abstract bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers);

	public static ImprovementModel LoadModel(Improver improver, IFuturemud gameworld)
	{
		switch (improver.Type)
		{
			case "classic":
				return new ClassicImprovement(improver);
			case "non-improving":
				return new NonImproving(improver.Id);
			case "theoretical":
				return new TheoreticalImprovementModel(improver);
			case "branching":
				return new BranchingImprover(improver, gameworld);
			default:
				throw new NotSupportedException();
		}
	}
}