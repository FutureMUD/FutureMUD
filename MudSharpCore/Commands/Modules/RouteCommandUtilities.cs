#nullable enable

using MudSharp.Construction;
using MudSharp.Framework.Units;

namespace MudSharp.Commands.Modules;

internal static class RouteCommandUtilities
{
	public static bool TrySplitAtClause(string text, out string subject, out string? routePosition)
	{
		text = text?.Trim() ?? string.Empty;
		var index = text.LastIndexOf(" at ", StringComparison.InvariantCultureIgnoreCase);
		if (index <= 0 || index + 4 >= text.Length)
		{
			subject = text;
			routePosition = null;
			return false;
		}

		subject = text[..index].Trim();
		routePosition = text[(index + 4)..].Trim();
		return subject.Length > 0 && routePosition.Length > 0;
	}

	public static bool TryResolveRoutePosition(
		ICharacter actor,
		ICell cell,
		string text,
		out double positionMetres,
		out string error)
	{
		positionMetres = 0.0;
		if (cell.RouteDefinition is not { } route)
		{
			error = "That destination is an ordinary cell and does not accept a route coordinate.";
			return false;
		}

		var landmark = route.Landmarks.FirstOrDefault(x =>
			x.Name.EqualTo(text) || x.HasKeyword(text, actor, abbreviated: true));
		if (landmark is not null)
		{
			positionMetres = landmark.PositionMetres;
			error = string.Empty;
			return true;
		}

		if (!actor.Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.Length, actor, out var baseUnits))
		{
			error = "Specify a valid distance or RouteCell landmark after AT.";
			return false;
		}

		positionMetres = baseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres;
		if (!double.IsFinite(positionMetres) || positionMetres < 0.0 || positionMetres > route.LengthMetres)
		{
			error = $"The route coordinate must be between 0 and {DescribeMetres(actor, route.LengthMetres)}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public static string DescribeMetres(ICharacter actor, double metres)
	{
		return actor.Gameworld.UnitManager.DescribeMostSignificantExact(
			metres / actor.Gameworld.UnitManager.BaseHeightToMetres,
			UnitType.Length,
			actor);
	}
}
