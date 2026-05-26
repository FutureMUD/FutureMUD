using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Construction;

public partial class Cell
{
	private readonly Dictionary<RoomLayer, SurfaceLiquidState> _surfaceLiquidStates = new();
	private readonly Dictionary<RoomLayer, System.DateTime> _lastWeatherExposureByLayer = new();
	private bool _surfaceLiquidChanged;

	public IEnumerable<(RoomLayer Layer, ISurfaceLiquidState State)> SurfaceLiquidStates =>
		_surfaceLiquidStates.Select(x => (x.Key, (ISurfaceLiquidState)x.Value));

	public void AddLiquidToSurface(LiquidMixture mixture, RoomLayer layer, IPerceivable? referenceItem)
	{
		if (mixture.IsEmpty || IsSwimmingLayer(layer))
		{
			return;
		}

		if (!Gameworld.GetStaticBool("PuddlesEnabled"))
		{
			mixture.SetLiquidVolume(0.0);
			return;
		}

		var state = GetOrCreateSurfaceState(layer);
		state.AddLiquid(mixture);
		CapSurfaceLiquid(state);
		mixture.SetLiquidVolume(0.0);
	}

	public string DescribeLiquidSurface(RoomLayer layer, IPerceiver voyeur, bool colour)
	{
		ResolveSurfaceDrying(layer);
		if (!Gameworld.GetStaticBool("PuddlesEnabled") || !_surfaceLiquidStates.TryGetValue(layer, out var state) || state.IsEmpty)
		{
			return string.Empty;
		}

		var sb = new StringBuilder();
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
			sb.AppendLine(colour && residue.Material.ResidueColour is not null ? text.Colour(residue.Material.ResidueColour) : text);
		}

		return sb.ToString().TrimEnd();
	}

	public void ResolveRoomWeatherExposure(IPerceiver? voyeur)
	{
		var layer = voyeur?.RoomLayer ?? RoomLayer.GroundLevel;
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
			var state = GetOrCreateSurfaceState(layer);
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
			_surfaceLiquidStates[layer] = new SurfaceLiquidState(Gameworld, element.Element("Surface"), SurfaceLiquidChanged);
		}

		_surfaceLiquidChanged = false;
	}

	private string? SaveSurfaceLiquidState()
	{
		foreach (var layer in _surfaceLiquidStates.Keys.ToList())
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
					new XAttribute("id", (int)x.Key),
					x.Value.SaveToXml()
				))
		).ToString();
	}

	private SurfaceLiquidState GetOrCreateSurfaceState(RoomLayer layer)
	{
		if (_surfaceLiquidStates.TryGetValue(layer, out var state))
		{
			return state;
		}

		state = new SurfaceLiquidState(Gameworld, SurfaceLiquidChanged);
		_surfaceLiquidStates[layer] = state;
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
		if (!_surfaceLiquidStates.TryGetValue(layer, out var state) || state.ContaminatingLiquid.IsEmpty)
		{
			return;
		}

		var duration = TimeSpan.FromSeconds(Math.Max(1.0,
			Gameworld.GetStaticDouble("LiquidContaminationEffectDuration") *
			Math.Max(state.ContaminatingLiquid.RelativeEnthalpy, double.Epsilon)));
		state.ResolveDrying(duration, 0.02 / Gameworld.UnitManager.BaseFluidToLitres, 0.1, roomSurface: true);
	}

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
