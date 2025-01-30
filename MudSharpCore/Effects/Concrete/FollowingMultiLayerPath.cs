using System.Collections.Generic;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Concrete;

public class FollowingMultiLayerPath : FollowingPath
{
	public RoomLayer TargetMovingLayer { get; }
	public RoomLayer TargetFinalLayer { get; }
	public bool InitialLayerChangeCompleted { get; private set; }

	#region Overrides of FollowingPath

	/// <inheritdoc />
	protected override bool RemoveWhenExitsEmpty => false;

	#endregion

	/// <inheritdoc />
	public FollowingMultiLayerPath(ICharacter owner, IEnumerable<ICellExit> exits, RoomLayer targetMovingLayer, RoomLayer targetFinalLayer) : base(owner, exits)
	{
		TargetMovingLayer = targetMovingLayer;
		TargetFinalLayer = targetFinalLayer;
		InitialLayerChangeCompleted = false;
	}

	#region Overrides of FollowingPath

	/// <inheritdoc />
	protected override string SpecificEffectType => "FollowingMultiLayerPath";

	/// <inheritdoc />
	public override void FollowPathAction()
	{
		var ch = (ICharacter)Owner;
		if (!InitialLayerChangeCompleted)
		{
			if (ch.RoomLayer == TargetMovingLayer)
			{
				InitialLayerChangeCompleted = true;
				FollowPathAction();
				return;
			}

			if (!PathTowardsLayer(TargetMovingLayer))
			{
				ch.RemoveEffect(this);
			}

			return;
		}

		if (Exits.Count == 0)
		{
			if (ch.RoomLayer == TargetFinalLayer)
			{
				if (ch.PositionState == PositionFlying.Instance)
				{
					ch.Land();
				}
				ch.RemoveEffect(this);
				return;
			}

			if (!PathTowardsLayer(TargetFinalLayer))
			{
				ch.RemoveEffect(this);
			}

			return;
		}

		base.FollowPathAction();
	}

	#endregion
}