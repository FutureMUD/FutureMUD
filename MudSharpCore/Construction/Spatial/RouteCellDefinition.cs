#nullable enable

using System.IO;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// An immutable runtime snapshot of a persisted linear route-cell definition.
/// </summary>
public sealed class RouteCellDefinition : IRouteCellDefinition
{
	private readonly IReadOnlyList<IRouteCellLandmark> _landmarks;
	private readonly IReadOnlyCollection<IRouteExitAnchor> _exitAnchors;

	public RouteCellDefinition(ICell cell, Models.RouteCell model)
	{
		ArgumentNullException.ThrowIfNull(cell);
		ArgumentNullException.ThrowIfNull(model);

		if (model.CellId != cell.Id)
		{
			throw new InvalidDataException(
				$"RouteCell #{model.CellId:N0} cannot be attached to Cell #{cell.Id:N0}.");
		}

		Cell = cell;
		LengthMetres = (double)model.LengthMetres;
		DefaultPositionMetres = (double)model.DefaultPositionMetres;
		PositiveDirectionName = model.PositiveDirectionName;
		NegativeDirectionName = model.NegativeDirectionName;
		MetresPerRoomEquivalent = (double)model.MetresPerRoomEquivalent;
		TopologyVersion = model.TopologyVersion;

		ValidateDefinition();

		_landmarks = model.Landmarks
			.Select(x => (IRouteCellLandmark)new RouteCellLandmark(this, x))
			.OrderBy(x => x.DisplayOrder)
			.ThenBy(x => x.PositionMetres)
			.ThenBy(x => x.Id)
			.ToArray();
		_exitAnchors = model.ExitAnchors
			.Select(x => (IRouteExitAnchor)new RouteCellExitAnchor(this, x))
			.ToArray();
	}

	public ICell Cell { get; }
	public double LengthMetres { get; }
	public double DefaultPositionMetres { get; }
	public string PositiveDirectionName { get; }
	public string NegativeDirectionName { get; }
	public double MetresPerRoomEquivalent { get; }
	public long TopologyVersion { get; }
	public IReadOnlyList<IRouteCellLandmark> Landmarks => _landmarks;
	public IReadOnlyCollection<IRouteExitAnchor> ExitAnchors => _exitAnchors;

	private void ValidateDefinition()
	{
		if (!double.IsFinite(LengthMetres) || LengthMetres <= 0.0)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Cell.Id:N0} has invalid length {LengthMetres} metres.");
		}

		if (!double.IsFinite(DefaultPositionMetres) ||
			DefaultPositionMetres < 0.0 ||
			DefaultPositionMetres > LengthMetres)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Cell.Id:N0} has invalid default position {DefaultPositionMetres} metres.");
		}

		if (!double.IsFinite(MetresPerRoomEquivalent) || MetresPerRoomEquivalent <= 0.0)
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Cell.Id:N0} has invalid room-equivalent length {MetresPerRoomEquivalent} metres.");
		}

		if (string.IsNullOrWhiteSpace(PositiveDirectionName) ||
			string.IsNullOrWhiteSpace(NegativeDirectionName))
		{
			throw new InvalidDataException(
				$"RouteCell for Cell #{Cell.Id:N0} must have names for both directions.");
		}
	}
}

public sealed class RouteCellLandmark : FrameworkItem, IRouteCellLandmark
{
	private readonly IReadOnlyList<string> _keywords;

	internal RouteCellLandmark(RouteCellDefinition routeCell, Models.RouteCellLandmark model)
	{
		ArgumentNullException.ThrowIfNull(routeCell);
		ArgumentNullException.ThrowIfNull(model);

		RouteCell = routeCell;
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
			PositionMetres > routeCell.LengthMetres)
		{
			throw new InvalidDataException(
				$"RouteCell landmark #{Id:N0} has invalid name or position {PositionMetres} metres.");
		}
	}

	public override string FrameworkItemType => "RouteCellLandmark";
	public IRouteCellDefinition RouteCell { get; }
	public double PositionMetres { get; }
	public string Description { get; }
	public int DisplayOrder { get; }
	public IEnumerable<string> Keywords => _keywords;

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return _keywords;
	}
}

public sealed class RouteCellExitAnchor : IRouteExitAnchor
{
	internal RouteCellExitAnchor(RouteCellDefinition routeCell, Models.RouteExitAnchor model)
	{
		ArgumentNullException.ThrowIfNull(routeCell);
		ArgumentNullException.ThrowIfNull(model);

		RouteCell = routeCell;
		ExitId = model.ExitId;
		MinimumPositionMetres = (double)model.MinimumPositionMetres;
		MaximumPositionMetres = (double)model.MaximumPositionMetres;
		ArrivalPositionMetres = (double)model.ArrivalPositionMetres;

		if (!double.IsFinite(MinimumPositionMetres) ||
			!double.IsFinite(MaximumPositionMetres) ||
			!double.IsFinite(ArrivalPositionMetres) ||
			MinimumPositionMetres < 0.0 ||
			MaximumPositionMetres < MinimumPositionMetres ||
			MaximumPositionMetres > routeCell.LengthMetres ||
			ArrivalPositionMetres < MinimumPositionMetres ||
			ArrivalPositionMetres > MaximumPositionMetres)
		{
			throw new InvalidDataException(
				$"Exit #{ExitId:N0} has an invalid anchor in RouteCell #{routeCell.Cell.Id:N0}.");
		}
	}

	public RouteCellDefinition RouteCell { get; }
	public long ExitId { get; }

	public ICellExit Exit
	{
		get
		{
			var exit = Cell.Gameworld.ExitManager.GetExitByID(ExitId);
			return exit?.CellExitFor(Cell) ?? throw new InvalidOperationException(
				$"Exit #{ExitId:N0} for RouteCell #{Cell.Id:N0} has not been loaded.");
		}
	}

	public ICell Cell => RouteCell.Cell;
	public double MinimumPositionMetres { get; }
	public double MaximumPositionMetres { get; }
	public double ArrivalPositionMetres { get; }
}
