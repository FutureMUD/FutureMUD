using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class LayerChangeMove : CombatMoveBase
{
	public enum DesiredLayerChange
	{
		ClimbUp,
		ClimbDown,
		Fly,
		Land,
		FlyUp,
		FlyDown,
		SwimDown,
		SwimUp
	}

	public DesiredLayerChange DesiredLayer { get; }

	public LayerChangeMove(ICharacter assailant, DesiredLayerChange desiredLayer)
	{
		DesiredLayer = desiredLayer;
		Assailant = assailant;
	}

	public override string Description => $"Attempting to {DesiredLayer.DescribeEnum()}";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		switch (DesiredLayer)
		{
			case DesiredLayerChange.ClimbUp:
				Assailant.ClimbUp();
				break;
			case DesiredLayerChange.ClimbDown:
				Assailant.ClimbDown();
				break;
			case DesiredLayerChange.Fly:
				Assailant.Fly();
				break;
			case DesiredLayerChange.Land:
				Assailant.Land();
				break;
			case DesiredLayerChange.FlyUp:
				((IFly)Assailant).Ascend();
				break;
			case DesiredLayerChange.FlyDown:
				((IFly)Assailant).Dive();
				break;
			case DesiredLayerChange.SwimDown:
				((ISwim)Assailant).Dive();
				break;
			case DesiredLayerChange.SwimUp:
				((ISwim)Assailant).Ascend();
				break;
		}

		return CombatMoveResult.Irrelevant;
	}
}