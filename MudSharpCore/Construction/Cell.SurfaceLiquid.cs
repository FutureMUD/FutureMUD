using System.Globalization;
using System.IO;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.Construction;

public partial class Cell
{
	private readonly Dictionary<SurfaceLiquidLocation, SurfaceLiquidState> _surfaceLiquidStates = new();
	private readonly Dictionary<RoomLayer, System.DateTime> _lastWeatherExposureByLayer = new();
	private bool _surfaceLiquidChanged;

	public IEnumerable<(RoomLayer Layer, ISurfaceLiquidState State)> SurfaceLiquidStates =>
		_surfaceLiquidStates.Select(x => (x.Key.Layer, (ISurfaceLiquidState)x.Value));

	internal bool HasPointSurfaceLiquid => _surfaceLiquidStates.Any(x =>
		x.Key.RoutePositionMetres.HasValue && !x.Value.IsEmpty);

	internal bool HasPointSurfaceLiquidBeyond(double maximumPositionMetres)
	{
		return _surfaceLiquidStates.Any(x =>
			x.Key.RoutePositionMetres > maximumPositionMetres && !x.Value.IsEmpty);
	}

	public void AddLiquidToSurface(LiquidMixture mixture, RoomLayer layer, IPerceivable? referenceItem)
	{
		var route = SurfaceLiquidTransferService.SelectRoute(
			mixture.IsEmpty,
			() => Gameworld.GetStaticBool("PuddlesEnabled"),
			IsSwimmingLayer(layer));
		switch (route)
		{
			case SurfaceLiquidRoute.Ignore:
				return;
			case SurfaceLiquidRoute.Discard:
				mixture.SetLiquidVolume(0.0);
				return;
			case SurfaceLiquidRoute.Surface:
				SurfaceLiquidTransferService.TransferToSurface(
					GetOrCreateSurfaceState(layer, SurfaceCoordinateFor(referenceItem)),
					mixture,
					Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity"));
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public string DescribeLiquidSurface(RoomLayer layer, IPerceiver voyeur, bool colour)
	{
		ResolveSurfaceDrying(layer);
		if (!Gameworld.GetStaticBool("PuddlesEnabled"))
		{
			return string.Empty;
		}

		var sb = new StringBuilder();
		foreach (var state in VisibleSurfaceStates(layer, voyeur))
		{
			if (state.LiquidVolume >= Gameworld.GetStaticDouble("SplashLiquidQuantity"))
			{
				var text = $"{PuddleDescription(state.LiquidVolume).A_An()} of {state.ContaminatingLiquid.LiquidDescription} is here.";
				sb.AppendLine(colour ? text.Colour(state.ContaminatingLiquid.LiquidColour) : text);
			}

			foreach (var residue in state.Residues.Where(x => x.Weight > 0.0))
			{
				var text = string.IsNullOrWhiteSpace(residue.Material.ResidueDesc)
					? $"There are traces of {residue.Material.MaterialDescription.ToLowerInvariant()} here."
					: string.Format(residue.Material.ResidueDesc, "some");
				sb.AppendLine(colour && residue.Material.ResidueColour is not null
					? text.Colour(residue.Material.ResidueColour)
					: text);
			}
		}

		return sb.ToString().TrimEnd();
	}

	public void ResolveRoomWeatherExposure(IPerceiver? voyeur)
	{
		var layer = voyeur?.RoomLayer ?? RoomLayer.GroundLevel;
		ConsolidateLegacyPuddles(layer);
		if (!IsUnderwaterLayer(layer))
		{
			ResolveSurfaceDrying(layer);
		}

		if (IsUnderwaterLayer(layer) || OutdoorsType(voyeur) != CellOutdoorsType.Outdoors)
		{
			return;
		}

		if (CurrentWeather(voyeur) is not RainWeatherEvent { RainLiquid: { } rainLiquid } weather)
		{
			_lastWeatherExposureByLayer[layer] = System.DateTime.UtcNow;
			return;
		}

		var now = System.DateTime.UtcNow;
		if (!_lastWeatherExposureByLayer.TryGetValue(layer, out var lastResolved))
		{
			lastResolved = now - TimeSpan.FromSeconds(300.0);
		}

		var elapsed = Math.Min((now - lastResolved).TotalSeconds, 300.0);
		if (elapsed < 5.0)
		{
			return;
		}

		var amountPerTick = Gameworld.GetStaticDouble($"PrecipitationAmountPerItemSize{SizeCategory.Normal.DescribeEnum()}{weather.Precipitation.DescribeEnum()}");
		if (amountPerTick <= 0.0)
		{
			_lastWeatherExposureByLayer[layer] = now;
			return;
		}

		_lastWeatherExposureByLayer[layer] = now;
		if (Gameworld.GetStaticBool("PuddlesEnabled"))
		{
			// Precipitation is uniform RouteCell environment state rather than a point spill.
			var state = GetOrCreateSurfaceState(layer, null);
			state.AddLiquid(new LiquidMixture(rainLiquid, amountPerTick * elapsed / 5.0, Gameworld));
			CapSurfaceLiquid(state);
		}

		var boundedExposureTicks = (int)Math.Clamp(Math.Ceiling(elapsed / 5.0), 1.0, 10.0);
		foreach (var item in GameItems.ToArray())
		{
			if (item.PositionModifier == MudSharp.Body.Position.PositionModifier.Under &&
				item.PositionTarget is not null &&
				item.PositionTarget.Size > item.Size)
			{
				continue;
			}

			if (item.RoomLayer.IsUnderwater())
			{
				continue;
			}

			for (var i = 0; i < boundedExposureTicks; i++)
			{
				item.ExposeToPrecipitation(weather.Precipitation, rainLiquid);
			}
		}

		foreach (var ch in Characters.ToArray())
		{
			if (ch.PositionModifier == MudSharp.Body.Position.PositionModifier.Under &&
				ch.PositionTarget is not null &&
				ch.PositionTarget.Size > ch.CurrentContextualSize(SizeContext.RainfallExposure))
			{
				continue;
			}

			if (ch.RoomLayer.IsUnderwater())
			{
				continue;
			}

			for (var i = 0; i < boundedExposureTicks; i++)
			{
				ch.Body.ExposeToPrecipitation(weather.Precipitation, rainLiquid);
			}
		}
	}

	private void ConsolidateLegacyPuddles(RoomLayer layer)
	{
		if (!Gameworld.GetStaticBool("PuddlesEnabled") || IsSwimmingLayer(layer))
		{
			return;
		}

		var legacyPuddles = LayerGameItems(layer)
			.Select(x => (Item: x, Puddle: x.GetItemType<PuddleGameItemComponent>()))
			.Where(x => x.Puddle is not null)
			.ToList();
		if (legacyPuddles.Count == 0)
		{
			return;
		}

		foreach (var (item, puddle) in legacyPuddles)
		{
			if (puddle!.LiquidMixture?.IsEmpty == false)
			{
				var state = GetOrCreateSurfaceState(layer, SurfaceCoordinateFor(item));
				state.AddLiquid(puddle.LiquidMixture);
				CapSurfaceLiquid(state);
			}

			item.Delete();
		}
	}

	private void LoadSurfaceLiquidState(string? xml)
	{
		_surfaceLiquidStates.Clear();
		if (string.IsNullOrWhiteSpace(xml))
		{
			_surfaceLiquidChanged = false;
			return;
		}

		var root = XElement.Parse(xml);
		foreach (var element in root.Elements("Layer"))
		{
			var layer = (RoomLayer)int.Parse(element.Attribute("id")?.Value ?? "0", CultureInfo.InvariantCulture);
			var positionAttribute = element.Attribute("position");
			double? coordinate = null;
			if (positionAttribute is not null)
			{
				if (!double.TryParse(positionAttribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture,
						out var parsedCoordinate))
				{
					throw new InvalidDataException(
						$"Cell #{Id:N0} has an invalid persisted surface-liquid RouteCell coordinate '{positionAttribute.Value}'.");
				}

				coordinate = parsedCoordinate;
			}

			double? normalisedCoordinate;
			try
			{
				normalisedCoordinate = NormaliseSurfaceCoordinate(coordinate);
			}
			catch (ArgumentException exception)
			{
				throw new InvalidDataException(
					$"Cell #{Id:N0} has invalid persisted surface-liquid spatial data: {exception.Message}",
					exception);
			}

			var key = new SurfaceLiquidLocation(layer, normalisedCoordinate);
			_surfaceLiquidStates[key] = new SurfaceLiquidState(
				Gameworld,
				element.Element("Surface"),
				SurfaceLiquidChanged);
		}

		_surfaceLiquidChanged = false;
	}

	private string? SaveSurfaceLiquidState()
	{
		foreach (var layer in _surfaceLiquidStates.Keys.Select(x => x.Layer).Distinct().ToList())
		{
			ResolveSurfaceDrying(layer);
		}

		if (!_surfaceLiquidStates.Any(x => !x.Value.IsEmpty))
		{
			return null;
		}

		return new XElement("Surfaces",
			_surfaceLiquidStates
				.Where(x => !x.Value.IsEmpty)
				.Select(x => new XElement("Layer",
					new XAttribute("id", (int)x.Key.Layer),
					x.Key.RoutePositionMetres.HasValue
						? new XAttribute("position", x.Key.RoutePositionMetres.Value.ToString("R", CultureInfo.InvariantCulture))
						: null,
					x.Value.SaveToXml()
				))
		).ToString();
	}

	private SurfaceLiquidState GetOrCreateSurfaceState(RoomLayer layer, double? routePositionMetres)
	{
		var key = new SurfaceLiquidLocation(layer, NormaliseSurfaceCoordinate(routePositionMetres));
		if (_surfaceLiquidStates.TryGetValue(key, out var state))
		{
			return state;
		}

		state = new SurfaceLiquidState(Gameworld, SurfaceLiquidChanged);
		_surfaceLiquidStates[key] = state;
		return state;
	}

	private void SurfaceLiquidChanged()
	{
		if (_noSave)
		{
			return;
		}

		_surfaceLiquidChanged = true;
		Changed = true;
	}

	private void ResolveSurfaceDrying(RoomLayer layer)
	{
		foreach (var state in _surfaceLiquidStates
			.Where(x => x.Key.Layer == layer)
			.Select(x => x.Value)
			.Where(x => !x.ContaminatingLiquid.IsEmpty))
		{
			var duration = TimeSpan.FromSeconds(Math.Max(1.0,
				Gameworld.GetStaticDouble("LiquidContaminationEffectDuration") *
				Math.Max(state.ContaminatingLiquid.RelativeEnthalpy, double.Epsilon)));
			state.ResolveDrying(duration, 0.02 / Gameworld.UnitManager.BaseFluidToLitres, 0.1, roomSurface: true);
		}
	}

	private double? SurfaceCoordinateFor(IPerceivable? referenceItem)
	{
		if (RouteDefinition is null)
		{
			return null;
		}

		if (referenceItem is not null && ReferenceEquals(referenceItem.Location, this))
		{
			var sourceCoordinate = RouteSpatialService.Instance
				.GetEffectiveLocation(referenceItem)
				.RoutePositionMetres;
			return NormaliseSurfaceCoordinate(sourceCoordinate ?? RouteDefinition.DefaultPositionMetres);
		}

		return NormaliseSurfaceCoordinate(RouteDefinition.DefaultPositionMetres);
	}

	private IEnumerable<SurfaceLiquidState> VisibleSurfaceStates(RoomLayer layer, IPerceiver? voyeur)
	{
		var states = _surfaceLiquidStates
			.Where(x => x.Key.Layer == layer && !x.Value.IsEmpty);
		if (RouteDefinition is null)
		{
			return states.Select(x => x.Value).ToArray();
		}

		var coordinate = voyeur is not null && ReferenceEquals(voyeur.Location, this)
			? RouteSpatialService.Instance.GetEffectiveLocation(voyeur).RoutePositionMetres
			: null;
		if (!coordinate.HasValue)
		{
			return states.Where(x => !x.Key.RoutePositionMetres.HasValue).Select(x => x.Value).ToArray();
		}

		var immediate = RouteSpatialConfiguration.FromGameworld(Gameworld).ImmediateDistanceMetres;
		return states
			.Where(x => SurfaceLiquidSpatialRules.IsVisible(
				x.Key.RoutePositionMetres,
				coordinate,
				immediate))
			.Select(x => x.Value)
			.ToArray();
	}

	private double? NormaliseSurfaceCoordinate(double? coordinate)
	{
		return SurfaceLiquidSpatialRules.Normalise(RouteDefinition, coordinate);
	}

	private readonly record struct SurfaceLiquidLocation(RoomLayer Layer, double? RoutePositionMetres);

	private void CapSurfaceLiquid(SurfaceLiquidState state)
	{
		var cap = Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity");
		if (state.LiquidVolume > cap)
		{
			state.RemoveLiquidVolume(state.LiquidVolume - cap);
		}
	}

	private string PuddleDescription(double amount)
	{
		if (Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity") <= amount)
		{
			return "enormous pool";
		}

		if (Gameworld.GetStaticDouble("HugePoolLiquidQuantity") <= amount)
		{
			return "huge pool";
		}

		if (Gameworld.GetStaticDouble("LargePoolLiquidQuantity") <= amount)
		{
			return "large pool";
		}

		if (Gameworld.GetStaticDouble("PoolLiquidQuantity") <= amount)
		{
			return "pool";
		}

		if (Gameworld.GetStaticDouble("LargePuddleLiquidQuantity") <= amount)
		{
			return "large puddle";
		}

		if (Gameworld.GetStaticDouble("PuddleLiquidQuantity") <= amount)
		{
			return "puddle";
		}

		if (Gameworld.GetStaticDouble("SmallPuddleLiquidQuantity") <= amount)
		{
			return "small puddle";
		}

		if (Gameworld.GetStaticDouble("SplashLiquidQuantity") <= amount)
		{
			return "splash";
		}

		return "spot";
	}
}
