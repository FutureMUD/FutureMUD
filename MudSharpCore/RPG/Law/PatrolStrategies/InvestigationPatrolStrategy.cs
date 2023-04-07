using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class InvestigationPatrolStrategy : PatrolStrategyBase
{
	public InvestigationPatrolStrategy(IFuturemud gameworld) : base(gameworld)
	{
	}

	public override string Name => "Investigation Patrol";

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		throw new NotImplementedException();
	}
}