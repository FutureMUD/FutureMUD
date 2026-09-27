using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Form.Material;

public enum EnvironmentalExposureMode { Legacy, Disabled, Enabled }

[Flags]
public enum ExposureRoute
{
	None = 0,
	LiquidContact = 1,
	GasContact = 2,
	Inhalation = 4,
	AmbientHeat = 8,
	Ingestion = 16,
	Injection = 32
}

public enum ExposureSourceKind { Splash, Retained, Immersion, Puddle, ContainerInterior, Atmosphere, Cloud, Breath, Ambient, Ingested, Injected }
public enum ReactionConsumption { None, PerExposure }
public enum RetainedHazardPersistence { OrdinaryDrying, UntilRemoved }

/// <summary>A sample of fluid that actually entered this body's respiratory route, independent of oxygen compatibility.</summary>
public sealed record RespiratoryExposureSample(IBody Body, IFluid Fluid, string SourceIdentity,
	MudSharp.Construction.ICell Location, MudSharp.Construction.RoomLayer Layer, double Strength,
	double Seconds, bool Airflow, bool Supplied, bool WithdrawalSucceeded);

/// <summary>Rates are per second of full reference surface exposure. Volumes use UnitManager base units.</summary>
public interface IEnvironmentalReaction
{
	Guid Id { get; }
	string Name { get; }
	int Version { get; }
	ExposureRoute Routes { get; }
	string Channel { get; }
	string Category { get; }
	int Priority { get; }
	bool NoReaction { get; }
	ISolid? TargetMaterial { get; }
	IEnumerable<ITag> TargetTags { get; }
	DamageType DamageType { get; }
	double DamageRate { get; }
	double PainRate { get; }
	double StunRate { get; }
	double? MinimumTemperature { get; }
	double? MaximumTemperature { get; }
	ReactionConsumption Consumption { get; }
	double ConsumptionRate { get; }
	ILiquid? SpentLiquid { get; }
	IFutureProg? ApplicabilityProg { get; }
	IFutureProg? IntensityProg { get; }
	IFutureProg? NotificationProg { get; }
	string? Message { get; }
	IEnumerable<string> ValidationErrors(bool gas);
}

/// <summary>Only these states own liquid. Aggregate views must not create new lots or reservoirs.</summary>
public interface ILocalisedSurfaceLiquidState : ISurfaceLiquidState
{
	IEnumerable<(long PartId, ISurfaceLiquidState State)> Parts { get; }
	ISurfaceLiquidState ForPart(IExternalBodypart part);
	void ReconcileParts();
}

/// <summary>Damage has already traversed physical equipment. Natural and spell resistance still apply.</summary>
public sealed record ExposureDamageContext(ExposureRoute Route, ExposureSourceKind SourceKind,
	string SourceIdentity, string Category, string Channel, Guid ReactionId, double ReferenceSeconds = 1.0, long TargetBodyId = 0);

public interface IExposureResistance : IEffect
{
	double ExposureDamageMultiplier(ExposureDamageContext context, IBodypart? part);
}
