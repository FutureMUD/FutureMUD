#nullable enable

using System.IO;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Work.Projects.ConcreteTypes;

internal static class LocalProjectSpatialRules
{
	internal static SpatialLocation ValidateLoadedSite(
		ICell cell,
		RoomLayer layer,
		double? routePositionMetres,
		long projectId)
	{
		ArgumentNullException.ThrowIfNull(cell);
		if (!Enum.IsDefined(layer))
		{
			throw new InvalidDataException(
				$"Active local project #{projectId} has invalid room layer {(int)layer} in cell #{cell.Id}.");
		}

		var site = new SpatialLocation(cell, layer, routePositionMetres);
		if (RouteSpatialService.Instance.TryValidateLocation(site, out var error))
		{
			return site;
		}

		throw new InvalidDataException(
			$"Active local project #{projectId} has invalid spatial data in cell #{cell.Id}: {error}");
	}

	internal static bool IsAtSite(SpatialLocation site, ICharacter character)
	{
		ArgumentNullException.ThrowIfNull(character);
		if (!ReferenceEquals(site.Cell, character.Location))
		{
			return false;
		}

		if (site.Cell.RouteDefinition is null)
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
		if (site.Cell.RouteDefinition is null)
		{
			return site.Cell.Characters.ToArray();
		}

		var maximumDistance = RouteSpatialConfiguration.FromGameworld(site.Cell.Gameworld)
			.ImmediateDistanceMetres;
		return RouteSpatialService.Instance
			.GetPerceivablesWithin(site, maximumDistance)
			.OfType<ICharacter>()
			.ToArray();
	}

	internal static IReadOnlyCollection<IGameItem> GameItemsAtSite(SpatialLocation site)
	{
		if (site.Cell.RouteDefinition is null)
		{
			return site.Cell.GameItems.ToArray();
		}

		var maximumDistance = RouteSpatialConfiguration.FromGameworld(site.Cell.Gameworld)
			.ImmediateDistanceMetres;
		return RouteSpatialService.Instance
			.GetPerceivablesWithin(site, maximumDistance)
			.OfType<IGameItem>()
			.ToArray();
	}

	internal static void HandleAtSite(SpatialLocation site, string text)
	{
		if (site.Cell.RouteDefinition is null)
		{
			site.Cell.Handle(text);
			return;
		}

		foreach (var character in CharactersAtSite(site))
		{
			character.OutputHandler.Send(text);
		}
	}
}
