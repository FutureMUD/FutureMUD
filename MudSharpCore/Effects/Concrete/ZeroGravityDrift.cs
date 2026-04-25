using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;

namespace MudSharp.Effects.Concrete;

public class ZeroGravityDrift : Effect, IEffectSubtype
{
	public ZeroGravityDrift(IPerceivable owner, CardinalDirection direction, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Direction = direction;
	}

	public CardinalDirection Direction { get; }

	protected override string SpecificEffectType => "ZeroGravityDrift";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drifting {Direction.DescribeBrief().ColourName()} through zero gravity.";
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this, false);
		if (Owner is not ICharacter character)
		{
			return;
		}

		if (character.Movement is not null ||
		    !ZeroGravityMovementHelper.IsZeroGravity(character.Location, character.RoomLayer, character))
		{
			return;
		}

		var exit = character.Location.GetExit(Direction, character);
		if (exit is null)
		{
			ZeroGravityMovementHelper.EchoStop(character, "@ bump|bumps gently against the end of open space and drift|drifts to a stop.");
			return;
		}

		var response = ZeroGravityMovementHelper.CanMoveInZeroGravity(character, exit);
		if (!response.Result)
		{
			character.OutputHandler.Send(response.ErrorMessage);
			return;
		}

		if (!character.Move(exit, null, true))
		{
			ZeroGravityMovementHelper.EchoStop(character, "@ drift|drifts to a stop.");
			return;
		}
	}
}
