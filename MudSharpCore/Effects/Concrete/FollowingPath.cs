using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.NPC.AI.Strategies;

namespace MudSharp.Effects.Concrete;

public class FollowingPath : Effect, IEffectSubtype, IRemoveOnCombatStart
{
	public Queue<ICellExit> Exits { get; set; }

	public IPerceiver UltimateTarget { get; set; }

	public FollowingPath(ICharacter owner, IEnumerable<ICellExit> exits) : base(owner, null)
	{
		Exits = new Queue<ICellExit>(exits);
	}

	protected override string SpecificEffectType => "FollowingPath";

	public override string Describe(IPerceiver voyeur)
	{
		var exitStrings = Exits.Select(x =>
		{
			if (x.OutboundDirection != CardinalDirection.Unknown)
			{
				return x.OutboundDirection.DescribeBrief();
			}

			return x is NonCardinalCellExit nc ? $"'{nc.Verb} {nc.PrimaryKeyword}'".ToLowerInvariant() : "??";
		}).Humanize();
		return $"Following a path: {exitStrings}";
	}

	public bool OpenDoors { get; set; }
	public bool UseKeys { get; set; }
	public bool SmashLockedDoors { get; set; }
	public bool UseDoorguards { get; set; }

	public void FollowPathAction()
	{
		var ch = (ICharacter)Owner;

		if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
		{
			ch.RemoveEffect(this);
			return;
		}

		if (Exits.Count == 0)
		{
			ch.RemoveEffect(this);
			return;
		}

		var exit = Exits.Peek();
		if (exit.Origin != ch.Location)
		{
			ch.RemoveEffect(this);
			return;
		}

		if (!exit.WhichLayersExitAppears().Contains(ch.RoomLayer))
		{
			var targetLayer = exit.WhichLayersExitAppears()
			                      .OrderBy(x => Math.Abs(ch.RoomLayer.LayerHeight() - x.LayerHeight()))
			                      .FirstOrDefault();
			if (!PathTowardsLayer(targetLayer))
			{
				ch.RemoveEffect(this);
				return;
			}
		}

		if (!ch.PositionState.Upright &&
		    !ch.PositionState.MoveRestrictions.In(MovementAbility.Flying, MovementAbility.Swimming))
		{
			var mobile = ch.MostUprightMobilePosition();
			if (mobile == null || !ch.CanMovePosition(mobile))
			{
				ch.RemoveEffect(this);
				return;
			}

			ch.MovePosition(mobile, null, null);
		}

		var strategy = MovementStrategyFactory.GetStrategy(OpenDoors, UseKeys, SmashLockedDoors, UseDoorguards);
		if (strategy.TryToMove(ch, exit))
		{
			Exits.Dequeue();
			if (Exits.Count == 0)
			{
				ch.RemoveEffect(this);
			}
		}
		else
		{
			ch.RemoveEffect(this);
		}
	}

	public bool PathTowardsLayer(RoomLayer targetLayer)
	{
		var ch = (ICharacter)Owner;
		if (ch.RoomLayer.IsLowerThan(targetLayer))
		{
			if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsUnderwaterLayer(ch.RoomLayer))
			{
				ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
				return true;
			}

			if (ch.Location.IsUnderwaterLayer(ch.RoomLayer))
			{
				((ISwim)ch).Ascend();
				return true;
			}

			if (ch.PositionState != PositionFlying.Instance && ch.CanFly().Truth)
			{
				ch.Fly();
				return true;
			}

			if (ch.PositionState == PositionFlying.Instance)
			{
				((IFly)ch).Ascend();
				return true;
			}

			if (ch.CanClimbUp().Truth)
			{
				ch.ClimbUp();
				return true;
			}

			return false;
		}

		if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsSwimmingLayer(ch.RoomLayer))
		{
			ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
			return true;
		}

		if (ch.Location.IsSwimmingLayer(ch.RoomLayer))
		{
			((ISwim)ch).Dive();
			return true;
		}

		if (ch.PositionState == PositionFlying.Instance)
		{
			if (ch.RoomLayer == RoomLayer.GroundLevel)
			{
				ch.Land();
				return true;
			}

			((IFly)ch).Dive();
			return true;
		}

		if (ch.CanClimbDown().Truth)
		{
			ch.ClimbDown();
			return true;
		}

		return false;
	}
}