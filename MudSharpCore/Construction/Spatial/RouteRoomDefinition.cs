#nullable enable

using System.IO;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// An immutable runtime snapshot of a persisted linear route-cell definition.
/// </summary>
public sealed class RouteRoomDefinition : IRouteRoomDefinition
{
	private readonly IReadOnlyList<IRouteRoomLandmark> _landmarks;
	private readonly IReadOnlyCollection<IRouteExitAnchor> _exitAnchors;

	public RouteRoomDefinition(IRoom room, Models.RouteRoom model)
	{
		ArgumentNullException.ThrowIfNull(room);
		ArgumentNullException.ThrowIfNull(model);

		if (model.RoomId != room.Id)
		{
			throw new InvalidDataException(
				$"RouteCell #{model.RoomId:N0} cannot be attached to Cell #{room.Id:N0}.");
		}

		Room = room;
		LengthMetres = (double)model.LengthMetres;
		DefaultPositionMetres = (double)model.DefaultPositionMetres;
		PositiveDirectionName = model.PositiveDirectionName;
		NegativeDirectionName = model.NegativeDirectionName;
		MetresPerRoomEquivalent = (double)model.MetresPerRoomEquivalent;
		TopologyVersion = model.TopologyVersion;

		ValidateDefinition();

		_landmarks = model.Landmarks
			.Select(x => (IRouteRoomLandmark)new RouteRoomLandmark(this, x))
			.OrderBy(x => x.DisplayOrder)
			.ThenBy(x => x.PositionMetres)
			.ThenBy(x => x.Id)
			.ToArray();
		_exitAnchors = model.ExitAnchors
			.Select(x => (IRouteExitAnchor)new RouteRoomExitAnchor(this, x))
			.ToArray();
	}

	internal RouteRoomDefinition(IRoom room, IRouteRoomDefinition template)
	{
		ArgumentNullException.ThrowIfNull(room);
		ArgumentNullException.ThrowIfNull(template);
		Room = room;
		LengthMetres = template.LengthMetres;
		DefaultPositionMetres = template.DefaultPositionMetres;
		PositiveDirectionName = template.PositiveDirectionName;
		NegativeDirectionName = template.NegativeDirectionName;
		MetresPerRoomEquivalent = template.MetresPerRoomEquivalent;
		TopologyVersion = template.TopologyVersion;
		ValidateDefinition();
		_landmarks = [];
		_exitAnchors = [];
	}

	public IRoom Room { get; }
	public double LengthMetres { get; }
	public double DefaultPositionMetres { get; }
	public string PositiveDirectionName { get; }
	public string NegativeDirectionName { get; }
	public double MetresPerRoomEquivalent { get; }
	public long TopologyVersion { get; }
	public IReadOnlyList<IRouteRoomLandmark> Landmarks => _landmarks;
	public IReadOnlyCollection<IRouteExitAnchor> ExitAnchors => _exitAnchors;

	private void ValidateDefinition()
	{
		if (!double.IsFinite(LengthMetres) || LengthMetres <= 0.0)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Room.Id:N0} has invalid length {LengthMetres} metres.");
		}

		if (!double.IsFinite(DefaultPositionMetres) ||
			DefaultPositionMetres < 0.0 ||
			DefaultPositionMetres > LengthMetres)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Room.Id:N0} has invalid default position {DefaultPositionMetres} metres.");
		}

		if (!double.IsFinite(MetresPerRoomEquivalent) || MetresPerRoomEquivalent <= 0.0)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Room.Id:N0} has invalid room-equivalent length {MetresPerRoomEquivalent} metres.");
		}

		if (string.IsNullOrWhiteSpace(PositiveDirectionName) ||
			string.IsNullOrWhiteSpace(NegativeDirectionName))
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Room.Id:N0} must have names for both directions.");
		}
	}
}

public sealed class RouteRoomLandmark : FrameworkItem, IRouteRoomLandmark
{
	private readonly IReadOnlyList<string> _keywords;

	internal RouteRoomLandmark(RouteRoomDefinition routeRoom, Models.RouteRoomLandmark model)
	{
		ArgumentNullException.ThrowIfNull(routeRoom);
		ArgumentNullException.ThrowIfNull(model);

		RouteRoom = routeRoom;
		_id = model.Id;
		_name = model.Name;
		Description = model.Description;
		PositionMetres = (double)model.PositionMetres;
		DisplayOrder = model.DisplayOrder;
		_keywords = model.Keywords
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Append(model.Name)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray();

		if (string.IsNullOrWhiteSpace(Name) ||
			!double.IsFinite(PositionMetres) ||
			PositionMetres < 0.0 ||
			PositionMetres > routeRoom.LengthMetres)
		{
			throw new InvalidDataException(
				$"RouteCell landmark #{Id:N0} has invalid name or position {PositionMetres} metres.");
		}
	}

	public override string FrameworkItemType => "RouteRoomLandmark";
	public IRouteRoomDefinition RouteRoom { get; }
	public double PositionMetres { get; }
	public string Description { get; }
	public int DisplayOrder { get; }
	public IEnumerable<string> Keywords => _keywords;

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return _keywords;
	}
}

public sealed class RouteRoomExitAnchor : IRouteExitAnchor
{
	internal RouteRoomExitAnchor(RouteRoomDefinition routeRoom, Models.RouteExitAnchor model)
	{
		ArgumentNullException.ThrowIfNull(routeRoom);
		ArgumentNullException.ThrowIfNull(model);

		RouteRoom = routeRoom;
		ExitId = model.ExitId;
		MinimumPositionMetres = (double)model.MinimumPositionMetres;
		MaximumPositionMetres = (double)model.MaximumPositionMetres;
		ArrivalPositionMetres = (double)model.ArrivalPositionMetres;

		if (!double.IsFinite(MinimumPositionMetres) ||
			!double.IsFinite(MaximumPositionMetres) ||
			!double.IsFinite(ArrivalPositionMetres) ||
			MinimumPositionMetres < 0.0 ||
			MaximumPositionMetres < MinimumPositionMetres ||
			MaximumPositionMetres > routeRoom.LengthMetres ||
			ArrivalPositionMetres < MinimumPositionMetres ||
			ArrivalPositionMetres > MaximumPositionMetres)
		{
			throw new InvalidDataException(
				$"Exit #{ExitId:N0} has an invalid anchor in RouteCell #{routeRoom.Room.Id:N0}.");
		}
	}

	public RouteRoomDefinition RouteRoom { get; }
	public long ExitId { get; }

	public IRoomExit Exit
	{
		get
		{
			var exit = Room.Gameworld.ExitManager.GetExitByID(ExitId);
			return exit?.RoomExitFor(Room) ?? throw new InvalidOperationException(
				$"Exit #{ExitId:N0} for RouteCell #{Room.Id:N0} has not been loaded.");
		}
	}

	public IRoom Room => RouteRoom.Room;
	public double MinimumPositionMetres { get; }
	public double MaximumPositionMetres { get; }
	public double ArrivalPositionMetres { get; }
}
