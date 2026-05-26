using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Form.Material;

public interface ISurfaceResidue
{
	ISolid Material { get; }
	ILiquid? OriginalLiquid { get; }
	double Weight { get; }
}

public interface ISurfaceLiquidState
{
	LiquidMixture ContaminatingLiquid { get; }
	IEnumerable<ISurfaceResidue> Residues { get; }
	double LiquidVolume { get; }
	double ResidueWeight { get; }
	double AddedWeight { get; }
	bool IsEmpty { get; }
	bool IsWet { get; }
	DateTime LastResolvedUtc { get; set; }
	ILiquid? LiquidRequired { get; }
	double LiquidAmountConsumed { get; }
	void AddLiquid(LiquidMixture liquid);
	LiquidMixture? RemoveLiquidVolume(double volume);
	bool CleanWithLiquid(LiquidMixture? liquid, double amount);
	void Dry(double amount, bool roomSurface = false);
	bool ResolveDrying(TimeSpan interval, double minimumDryVolume, double dryFraction, bool roomSurface = false,
		int maxTicks = 24);
	ItemSaturationLevel SaturationLevel(double coating, double absorb);
	ItemSaturationLevel SaturationLevelForLiquid(double total, double coating, double absorb);
	string GetAddendumText(double coating, double absorb, bool colour);
	string GetAdditionalText(double coating, double absorb, IPerceiver voyeur, bool colour);
	XElement SaveToXml();
}

public interface ISurfaceContaminable : IPerceivable
{
	ISurfaceLiquidState SurfaceLiquidState { get; }
	void SurfaceLiquidChanged();
}

public interface IRoomLiquidSurface
{
	IEnumerable<(RoomLayer Layer, ISurfaceLiquidState State)> SurfaceLiquidStates { get; }
	void AddLiquidToSurface(LiquidMixture mixture, RoomLayer layer, IPerceivable? referenceItem);
	string DescribeLiquidSurface(RoomLayer layer, IPerceiver voyeur, bool colour);
	void ResolveRoomWeatherExposure(IPerceiver? voyeur);
}

public interface ILiquidExposureStrategy
{
	void Expose(IPerceivable owner, LiquidMixture mixture, LiquidExposureDirection direction, IEnumerable<IExternalBodypart>? bodyparts = null);
	void Dry(IPerceivable owner, LiquidMixture driedLiquid, IEnumerable<IExternalBodypart>? bodyparts = null);
}
