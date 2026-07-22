#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands.Modules;

internal partial class MovementModule
{
	private const string RouteTravelHelp = @"The travel command moves continuously along a linear RouteCell.

	travel forward|backward - travel to that endpoint
	travel forward|backward <distance> - travel that far in the named direction
	travel to <distance|landmark|visible exit|target> - travel to an exact route destination
	stop - materialise your current position and stop travelling

Passing an exit does not use it. Travelling to an exit stops at the nearest point in its authored access band.";

	[PlayerCommand("Travel", "travel")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("travel", RouteTravelHelp, AutoHelp.HelpArg)]
	protected static void Travel(ICharacter actor, string input)
	{
		if (actor.Movement is not null)
		{
			actor.OutputHandler.Send("You are already moving. Use stop first.");
			return;
		}

		var route = actor.Location.RouteDefinition;
		var origin = RouteSpatialService.Instance.GetEffectiveLocation(actor);
		if (route is null || !origin.RoutePositionMetres.HasValue)
		{
			actor.OutputHandler.Send("You can only use travel while you are in a linear RouteCell.");
			return;
		}

		var command = new StringStack(input.RemoveFirstWord());
		var action = command.PopSpeech().ToLowerInvariant();
		if (action.Length == 0)
		{
			actor.OutputHandler.Send(RouteTravelHelp.SubstituteANSIColour());
			return;
		}

		double destination;
		double? targetMinimum = null;
		double? targetMaximum = null;
		long? selectedExitId = null;
		switch (action)
		{
			case "forward":
			case "forwards":
			case "positive":
				destination = route.LengthMetres;
				if (!command.IsFinished)
				{
					if (!TryParseTravelDistance(actor, command.SafeRemainingArgument, out var distance) || distance <= 0.0)
					{
						actor.OutputHandler.Send("How far forward do you want to travel?");
						return;
					}

					destination = Math.Min(route.LengthMetres, origin.RoutePositionMetres.Value + distance);
				}
				break;
			case "backward":
			case "backwards":
			case "negative":
				destination = 0.0;
				if (!command.IsFinished)
				{
					if (!TryParseTravelDistance(actor, command.SafeRemainingArgument, out var distance) || distance <= 0.0)
					{
						actor.OutputHandler.Send("How far backward do you want to travel?");
						return;
					}

					destination = Math.Max(0.0, origin.RoutePositionMetres.Value - distance);
				}
				break;
			case "to":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What route position, landmark, exit, or target do you want to travel to?");
					return;
				}

				if (!TryResolveTravelDestination(
						actor,
						route,
						origin,
						command.SafeRemainingArgument,
						out destination,
						out targetMinimum,
						out targetMaximum,
						out selectedExitId))
				{
					actor.OutputHandler.Send("There is no route position, landmark, visible exit, or target like that.");
					return;
				}
				break;
			default:
				actor.OutputHandler.Send(RouteTravelHelp.SubstituteANSIColour());
				return;
		}

		if (!LinearRouteMovement.TryCreate(
				actor,
				destination,
				out var movement,
				out var error,
				targetMinimumMetres: targetMinimum,
				targetMaximumMetres: targetMaximum,
				selectedExitId: selectedExitId))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		movement!.InitialAction();
	}

	private static bool TryResolveTravelDestination(
		ICharacter actor,
		IRouteCellDefinition route,
		SpatialLocation origin,
		string text,
		out double destination,
		out double? targetMinimum,
		out double? targetMaximum,
		out long? selectedExitId)
	{
		targetMinimum = null;
		targetMaximum = null;
		selectedExitId = null;
		if (TryParseTravelDistance(actor, text, out destination) &&
			destination >= 0.0 && destination <= route.LengthMetres)
		{
			return true;
		}

		var landmark = route.Landmarks.FirstOrDefault(x =>
			x.Name.EqualTo(text) || x.HasKeyword(text, actor, abbreviated: true));
		if (landmark is not null)
		{
			destination = landmark.PositionMetres;
			return true;
		}

		var maximumVisibleDistance = RouteSpatialConfiguration
			.FromGameworld(actor.Gameworld)
			.VeryDistantDistanceMetres;
		var anchor = route.ExitAnchors
			.Where(x => RouteSpatialService.Instance.IsExitVisible(
				actor,
				x.Exit,
				maximumVisibleDistance))
			.FirstOrDefault(x => x.Exit.HasKeyword(text, actor, abbreviated: true) ||
			                     x.Exit.OutboundDirection.Describe().EqualTo(text));
		if (anchor is not null)
		{
			destination = Math.Clamp(
				origin.RoutePositionMetres!.Value,
				anchor.MinimumPositionMetres,
				anchor.MaximumPositionMetres);
			targetMinimum = anchor.MinimumPositionMetres;
			targetMaximum = anchor.MaximumPositionMetres;
			selectedExitId = anchor.Exit.Exit.Id;
			return true;
		}

		var target = RouteSpatialService.Instance
			.GetPerceivablesWithin(
				origin,
				maximumVisibleDistance)
			.Where(x => !ReferenceEquals(x, actor))
			.Where(x => actor.CanSee(x))
			.GetFromItemListByKeyword(text, actor);
		if (target is not null)
		{
			var targetLocation = RouteSpatialService.Instance.GetEffectiveLocation(target);
			if (targetLocation.RoutePositionMetres.HasValue)
			{
				destination = targetLocation.RoutePositionMetres.Value;
				return true;
			}
		}

		destination = 0.0;
		return false;
	}

	private static bool TryParseTravelDistance(ICharacter actor, string text, out double metres)
	{
		metres = 0.0;
		if (!actor.Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.Length, actor, out var baseUnits))
		{
			return false;
		}

		metres = baseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres;
		return double.IsFinite(metres);
	}
}
