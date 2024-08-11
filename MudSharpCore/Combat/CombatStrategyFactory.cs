using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat;

public class CombatStrategyFactory
{
	public static ICombatStrategy GetStrategy(CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.Clinch:
				return Strategies.ClinchStrategy.Instance;
			case CombatStrategyMode.CoverAndAdvance:
				return Strategies.CoverAndAdvanceStrategy.Instance;
			case CombatStrategyMode.CoveringFire:
				return Strategies.CoveringFireStrategy.Instance;
			case CombatStrategyMode.FireAndAdvance:
				return Strategies.FireAndAdvanceStrategy.Instance;
			case CombatStrategyMode.FireNoCover:
				return Strategies.FireNoCoverStrategy.Instance;
			case CombatStrategyMode.Flee:
				return Strategies.FleeStrategy.Instance;
			case CombatStrategyMode.FullAdvance:
				return Strategies.FullAdvanceStrategy.Instance;
			case CombatStrategyMode.FullCover:
				return Strategies.FullCoverStrategy.Instance;
			case CombatStrategyMode.FullDefense:
				return Strategies.FullDefenseStrategy.Instance;
			case CombatStrategyMode.FullSkirmish:
				return Strategies.FullSkirmishStrategy.Instance;
			case CombatStrategyMode.GrappleForControl:
				return Strategies.GrappleForControlStrategy.Instance;
			case CombatStrategyMode.GrappleForIncapacitation:
				return Strategies.GrappleForIncapacitationStrategy.Instance;
			case CombatStrategyMode.GrappleForKill:
				return Strategies.GrappleForKillStrategy.Instance;
			case CombatStrategyMode.Skirmish:
				return Strategies.SkirmishStrategy.Instance;
			case CombatStrategyMode.StandardMelee:
				return Strategies.StandardMeleeStrategy.Instance;
			case CombatStrategyMode.StandardRange:
				return Strategies.StandardRangeStrategy.Instance;
			case CombatStrategyMode.Ward:
				return Strategies.WardStrategy.Instance;
			case CombatStrategyMode.MeleeShooter:
				return Strategies.MeleeShooter.Instance;
			case CombatStrategyMode.MeleeMagic:
				return Strategies.MeleeMagicStrategy.Instance;
			default:
				throw new NotImplementedException("Unknown CombatStrategyMode in CombatStrategyFactory.GetStrategy: " +
				                                  mode.Describe());
		}
	}
}