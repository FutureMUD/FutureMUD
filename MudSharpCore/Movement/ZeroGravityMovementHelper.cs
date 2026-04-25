using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Linq;

#nullable enable annotations

namespace MudSharp.Movement;

public static class ZeroGravityMovementHelper
{
	public static GravityModel GravityFor(ICell cell, IPerceiver? voyeur = null)
	{
		var effect = cell.EffectsOfType<IGravityOverrideEffect>()
		                 .Where(x => x.Applies())
		                 .OrderByDescending(x => x.Priority)
		                 .FirstOrDefault();
		return effect?.GravityModel ?? cell.Terrain(voyeur).GravityModel;
	}

	public static bool IsZeroGravity(ICell cell, RoomLayer layer, IPerceiver? voyeur = null)
	{
		return GravityFor(cell, voyeur) == GravityModel.ZeroGravity &&
		       !cell.IsSwimmingLayer(layer);
	}

	public static bool HasIndependentPropulsion(ICharacter character)
	{
		return character.EffectsOfType<IImmwalkEffect>().Any() ||
		       character.PositionState == PositionFlying.Instance ||
		       character.CanFly().Truth ||
		       character.Body.ExternalItems.Any(x => x.IsItemType<IZeroGravityPropulsion>() &&
		                                             x.GetItemType<IZeroGravityPropulsion>().CanPropel(character));
	}

	public static bool HasPushOffPoint(ICharacter character)
	{
		if (!IsZeroGravity(character.Location, character.RoomLayer, character))
		{
			return true;
		}

		if (character.RoomLayer.In(RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.HighInTrees, RoomLayer.OnRooftops))
		{
			return true;
		}

		if (character.Location.OutdoorsType(character) != CellOutdoorsType.Outdoors)
		{
			return true;
		}

		if (character.Location.LayerGameItems(character.RoomLayer)
		             .Any(item => item.EffectsOfType<IZeroGravityAnchorEffect>().Any(x => x.Applies() && x.AllowsZeroGravityPushOff) ||
		                          item.IsItemType<IZeroGravityAnchorItem>()))
		{
			return true;
		}

		return character.EffectsOfType<IZeroGravityTetherEffect>().Any(x => x.Applies()) ||
		       character.Body.EffectsOfType<IZeroGravityTetherEffect>().Any(x => x.Applies());
	}

	public static bool CanManeuver(ICharacter character)
	{
		return HasIndependentPropulsion(character) || HasPushOffPoint(character);
	}

	public static CanMoveResponse CanMoveInZeroGravity(ICharacter character, ICellExit exit)
	{
		if (!IsZeroGravity(character.Location, character.RoomLayer, character))
		{
			return CanMoveResponse.True;
		}

		if (character.EffectsOfType<IZeroGravityTetherEffect>().Any(x => x.Applies() && x.BlocksMovementTo(exit.Destination)) ||
		    character.Body.EffectsOfType<IZeroGravityTetherEffect>().Any(x => x.Applies() && x.BlocksMovementTo(exit.Destination)))
		{
			return new CanMoveResponse
			{
				Result = false,
				ErrorMessage = "Your tether is taut and prevents you from drifting any farther in that direction."
			};
		}

		if (CanManeuver(character))
		{
			return CanMoveResponse.True;
		}

		return new CanMoveResponse
		{
			Result = false,
			ErrorMessage = "You have nothing to push off from and no independent propulsion in zero gravity."
		};
	}

	public static IPositionState ZeroGravityMovementPosition(ICharacter character, IPositionState current)
	{
		if (!IsZeroGravity(character.Location, character.RoomLayer, character))
		{
			return current;
		}

		if (current.In(PositionSwimming.Instance, PositionClimbing.Instance, PositionFlying.Instance))
		{
			return current;
		}

		return PositionFloatingInZeroGravity.Instance;
	}

	public static void EnsureFloating(ICharacter character)
	{
		if (!IsZeroGravity(character.Location, character.RoomLayer, character))
		{
			return;
		}

		if (character.PositionState.In(PositionSwimming.Instance, PositionFlying.Instance, PositionFloatingInZeroGravity.Instance))
		{
			return;
		}

		character.MovePosition(PositionFloatingInZeroGravity.Instance, PositionModifier.None, null, null, null);
	}

	public static void EnsureFloating(IGameItem item)
	{
		if (item.Location is not ICell cell || !IsZeroGravity(cell, item.RoomLayer))
		{
			return;
		}

		if (item.PositionState == PositionFloatingInZeroGravity.Instance ||
		    item.EffectsOfType<IZeroGravityAnchorEffect>().Any(x => x.Applies()) ||
		    item.IsItemType<IZeroGravityAnchorItem>())
		{
			return;
		}

		item.SetPosition(PositionFloatingInZeroGravity.Instance, PositionModifier.None, null, null);
	}

	public static void StartDriftAfterMovement(ICharacter character, ICellExit exit)
	{
		if (character.Location is null ||
		    exit is null ||
		    !IsZeroGravity(character.Location, character.RoomLayer, character) ||
		    exit.OutboundDirection == CardinalDirection.Unknown ||
		    character.Movement is not null ||
		    character.EffectsOfType<ZeroGravityDrift>().Any())
		{
			return;
		}

		EnsureFloating(character);
		character.AddEffect(new ZeroGravityDrift(character, exit.OutboundDirection), TimeSpan.FromSeconds(5));
	}

	public static bool ConsumePropellantFor(ICharacter character)
	{
		var propulsor = character.Body.ExternalItems
		                         .Select(x => x.GetItemType<IZeroGravityPropulsion>())
		                         .FirstOrDefault(x => x?.CanPropel(character) == true);
		return propulsor?.ConsumePropellant(character) == true;
	}

	public static void EchoStop(ICharacter character, string emote)
	{
		character.OutputHandler.Handle(new EmoteOutput(new Emote(emote, character, character),
			flags: OutputFlags.SuppressObscured));
	}
}
