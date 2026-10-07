#nullable enable

using System.IO;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Work.Projects.ConcreteTypes;

internal static class LocalProjectSpatialRules
{
	internal static SpatialLocation ValidateLoadedSite(
		IRoom room,
		RoomLayer layer,
		double? routePositionMetres,
		long projectId)
	{
		ArgumentNullException.ThrowIfNull(room);
		if (!Enum.IsDefined(layer))
		{
			throw new InvalidDataException(
				$"Active local project #{projectId} has invalid room layer {(int)layer} in room #{room.Id}.");
		}

		var site = new SpatialLocation(room, layer, routePositionMetres);
		if (RouteSpatialService.Instance.TryValidateLocation(site, out var error))
		{
			return site;
		}

		throw new InvalidDataException(
			$"Active local project #{projectId} has invalid spatial data in room #{room.Id}: {error}");
	}

	internal static bool IsAtSite(SpatialLocation site, ICharacter character)
	{
		ArgumentNullException.ThrowIfNull(character);
		if (!ReferenceEquals(site.Room, character.Location))
		{
			return false;
		}

		if (site.Room.RouteDefinition is null)
		{
			return true;
		}

		var maximumDistance = RouteSpatialConfiguration.FromGameworld(character.Gameworld)
			.ImmediateDistanceMetres;
		return RouteSpatialService.Instance.GetExactSeparation(
			site,
			RouteSpatialService.Instance.GetEffectiveLocation(character)) is { } separation &&
		       separation <= maximumDistance;
	}

	internal static IReadOnlyCollection<ICharacter> CharactersAtSite(SpatialLocation site)
	{
		if (site.Room.RouteDefinition is null)
		{
			return site.Room.Characters.ToArray();
		}

		var maximumDistance = RouteSpatialConfiguration.FromGameworld(site.Room.Gameworld)
			.ImmediateDistanceMetres;
		return RouteSpatialService.Instance
			.GetPerceivablesWithin(site, maximumDistance)
			.OfType<ICharacter>()
			.ToArray();
	}

	internal static IReadOnlyCollection<IGameItem> GameItemsAtSite(SpatialLocation site)
	{
		if (site.Room.RouteDefinition is null)
		{
			return site.Room.GameItems.ToArray();
		}

		var maximumDistance = RouteSpatialConfiguration.FromGameworld(site.Room.Gameworld)
			.ImmediateDistanceMetres;
		return RouteSpatialService.Instance
			.GetPerceivablesWithin(site, maximumDistance)
			.OfType<IGameItem>()
			.ToArray();
	}

	internal static void HandleAtSite(SpatialLocation site, string text)
	{
		if (site.Room.RouteDefinition is null)
		{
			site.Room.Handle(text);
			return;
		}

		foreach (var character in CharactersAtSite(site))
		{
			character.OutputHandler.Send(text);
		}
	}
}
