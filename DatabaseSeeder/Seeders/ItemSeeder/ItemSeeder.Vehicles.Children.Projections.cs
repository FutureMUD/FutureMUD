#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private VehicleAccessPointProto UpsertAccessPoint(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleAccessPointSeedSpec spec,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var compartment = string.IsNullOrWhiteSpace(spec.CompartmentKey)
			? null
			: RequireKey(compartments, spec.CompartmentKey, vehicle.Name, "compartment");
		var row = FindAccessPointByProjection(vehicle, vehicleReference, spec.Key) ??
		          _context!.VehicleAccessPointProtos.Local.FirstOrDefault(x =>
			          x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleAccessPointProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleAccessPointProto
			{
				Id = NextVehicleChildId(_context!.VehicleAccessPointProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleAccessPointProtos.Add(row);
		}

		row.VehicleCompartmentProto = compartment;
		row.VehicleCompartmentProtoId = compartment?.Id;
		row.Name = spec.Name;
		row.Description = spec.Description;
		row.AccessPointType = (int)spec.AccessPointType;
		row.StartsOpen = spec.StartsOpen;
		row.MustBeClosedForMovement = spec.MustBeClosedForMovement;
		row.DisplayOrder = spec.DisplayOrder;
		return row;
	}

	private VehicleAccessPointProto? FindAccessPointByProjection(VehicleProto vehicle, string vehicleReference, string key)
	{
		var stableReference = $"{vehicleReference}_access_{NormaliseKey(key)}";
		if (!_items.TryGetValue(stableReference, out var projection))
		{
			return null;
		}

		return _context!.VehicleAccessPointProtos.Local.FirstOrDefault(x =>
			       x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			       x.ProjectionItemProtoId == projection.Id && x.ProjectionItemProtoRevision == projection.RevisionNumber) ??
		       _context.VehicleAccessPointProtos.FirstOrDefault(x =>
			       x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			       x.ProjectionItemProtoId == projection.Id && x.ProjectionItemProtoRevision == projection.RevisionNumber);
	}

	private VehicleCargoSpaceProto UpsertCargoSpace(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleCargoSpaceSeedSpec spec,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accessPoints)
	{
		var compartment = string.IsNullOrWhiteSpace(spec.CompartmentKey)
			? null
			: RequireKey(compartments, spec.CompartmentKey, vehicle.Name, "compartment");
		var requiredAccess = string.IsNullOrWhiteSpace(spec.RequiredAccessPointKey)
			? null
			: RequireKey(accessPoints, spec.RequiredAccessPointKey, vehicle.Name, "access point");
		var row = FindCargoSpaceByProjection(vehicle, vehicleReference, spec.Key) ??
		          _context!.VehicleCargoSpaceProtos.Local.FirstOrDefault(x =>
			          x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleCargoSpaceProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleCargoSpaceProto
			{
				Id = NextVehicleChildId(_context!.VehicleCargoSpaceProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleCargoSpaceProtos.Add(row);
		}

		row.VehicleCompartmentProto = compartment;
		row.VehicleCompartmentProtoId = compartment?.Id;
		row.RequiredAccessPointProto = requiredAccess;
		row.RequiredAccessPointProtoId = requiredAccess?.Id;
		row.Name = spec.Name;
		row.Description = spec.Description;
		row.DisplayOrder = spec.DisplayOrder;
		return row;
	}

	private VehicleCargoSpaceProto? FindCargoSpaceByProjection(VehicleProto vehicle, string vehicleReference, string key)
	{
		var stableReference = $"{vehicleReference}_cargo_{NormaliseKey(key)}";
		if (!_items.TryGetValue(stableReference, out var projection))
		{
			return null;
		}

		return _context!.VehicleCargoSpaceProtos.Local.FirstOrDefault(x =>
			       x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			       x.ProjectionItemProtoId == projection.Id && x.ProjectionItemProtoRevision == projection.RevisionNumber) ??
		       _context.VehicleCargoSpaceProtos.FirstOrDefault(x =>
			       x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			       x.ProjectionItemProtoId == projection.Id && x.ProjectionItemProtoRevision == projection.RevisionNumber);
	}

	private VehicleInstallationPointProto UpsertInstallationPoint(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleInstallationPointSeedSpec spec,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accessPoints)
	{
		EnsureVehicleInstallationSupport(spec.MountType);
		var requiredAccess = string.IsNullOrWhiteSpace(spec.RequiredAccessPointKey)
			? null
			: RequireKey(accessPoints, spec.RequiredAccessPointKey, vehicle.Name, "access point");
		var row = _context!.VehicleInstallationPointProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleInstallationPointProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleInstallationPointProto
			{
				Id = NextVehicleChildId(_context.VehicleInstallationPointProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleInstallationPointProtos.Add(row);
		}

		row.RequiredAccessPointProto = requiredAccess;
		row.RequiredAccessPointProtoId = requiredAccess?.Id;
		row.Name = spec.Name;
		row.Description = spec.Description;
		row.MountType = spec.MountType;
		row.RequiredRole = spec.RequiredRole;
		row.RequiredForMovement = spec.RequiredForMovement;
		row.DisplayOrder = spec.DisplayOrder;
		return row;
	}

	private VehicleTowPointProto UpsertTowPoint(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleTowPointSeedSpec spec,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accessPoints)
	{
		EnsureVehicleTowSupport(spec);
		var requiredAccess = string.IsNullOrWhiteSpace(spec.RequiredAccessPointKey)
			? null
			: RequireKey(accessPoints, spec.RequiredAccessPointKey, vehicle.Name, "access point");
		var row = _context!.VehicleTowPointProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleTowPointProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleTowPointProto
			{
				Id = NextVehicleChildId(_context.VehicleTowPointProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleTowPointProtos.Add(row);
		}

		row.RequiredAccessPointProto = requiredAccess;
		row.RequiredAccessPointProtoId = requiredAccess?.Id;
		row.Name = spec.Name;
		row.Description = spec.Description;
		row.TowType = spec.TowType;
		row.CanTow = spec.CanTow;
		row.CanBeTowed = spec.CanBeTowed;
		row.MaximumTowedWeight = spec.MaximumTowedWeight;
		row.CharacterPullMultiplier = spec.CharacterPullMultiplier;
		row.TowStressWarningRatio = spec.TowStressWarningRatio;
		row.TowStressFailureStartRatio = spec.TowStressFailureStartRatio;
		row.TowStressMaximumFailureChance = spec.TowStressMaximumFailureChance;
		row.TowStressDamageMultiplier = spec.TowStressDamageMultiplier;
		row.DisplayOrder = spec.DisplayOrder;
		return row;
	}

	private VehicleDamageZoneProto UpsertDamageZone(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleDamageZoneSeedSpec spec)
	{
		var row = _context!.VehicleDamageZoneProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleDamageZoneProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleDamageZoneProto
			{
				Id = NextVehicleChildId(_context.VehicleDamageZoneProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleDamageZoneProtos.Add(row);
		}

		row.Name = spec.Name;
		row.Description = spec.Description;
		row.MaximumDamage = spec.MaximumDamage;
		row.HitWeight = spec.HitWeight;
		row.DisabledThreshold = spec.DisabledThreshold;
		row.DestroyedThreshold = spec.DestroyedThreshold;
		row.DisablesMovement = spec.DisablesMovement;
		row.DisplayOrder = spec.DisplayOrder;
		return row;
	}

	private void UpsertDamageEffect(
		VehicleDamageZoneProto damageZone,
		VehicleDamageEffectSeedSpec spec,
		IReadOnlyDictionary<string, VehicleMovementProfileProto> movements,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accessPoints,
		IReadOnlyDictionary<string, VehicleCargoSpaceProto> cargoSpaces,
		IReadOnlyDictionary<string, VehicleInstallationPointProto> installationPoints,
		IReadOnlyDictionary<string, VehicleTowPointProto> towPoints)
	{
		var targetId = ResolveDamageEffectTargetId(spec, movements, accessPoints, cargoSpaces, installationPoints, towPoints);
		var row = _context!.VehicleDamageZoneEffectProtos.Local.FirstOrDefault(x =>
				x.VehicleDamageZoneProtoId == damageZone.Id && x.TargetType == (int)spec.TargetType &&
				x.TargetProtoId == targetId) ??
		          _context.VehicleDamageZoneEffectProtos.FirstOrDefault(x =>
			          x.VehicleDamageZoneProtoId == damageZone.Id && x.TargetType == (int)spec.TargetType &&
			          x.TargetProtoId == targetId);
		if (row is null)
		{
			row = new VehicleDamageZoneEffectProto
			{
				Id = NextVehicleChildId(_context.VehicleDamageZoneEffectProtos, x => x.Id),
				VehicleDamageZoneProto = damageZone,
				VehicleDamageZoneProtoId = damageZone.Id
			};
			_context.VehicleDamageZoneEffectProtos.Add(row);
		}

		row.TargetType = (int)spec.TargetType;
		row.TargetProtoId = targetId;
		row.MinimumStatus = (int)spec.MinimumStatus;
	}

	private static long? ResolveDamageEffectTargetId(
		VehicleDamageEffectSeedSpec spec,
		IReadOnlyDictionary<string, VehicleMovementProfileProto> movements,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accessPoints,
		IReadOnlyDictionary<string, VehicleCargoSpaceProto> cargoSpaces,
		IReadOnlyDictionary<string, VehicleInstallationPointProto> installationPoints,
		IReadOnlyDictionary<string, VehicleTowPointProto> towPoints)
	{
		if (spec.TargetType == VehicleDamageEffectTargetType.WholeVehicleMovement)
		{
			return null;
		}
		if (string.IsNullOrWhiteSpace(spec.TargetKey))
		{
			throw new InvalidOperationException($"Damage effect target {spec.TargetType} requires a target key.");
		}

		return spec.TargetType switch
		{
			VehicleDamageEffectTargetType.MovementProfile => RequireKey(movements, spec.TargetKey, "damage effect", "movement profile").Id,
			VehicleDamageEffectTargetType.AccessPoint => RequireKey(accessPoints, spec.TargetKey, "damage effect", "access point").Id,
			VehicleDamageEffectTargetType.CargoSpace => RequireKey(cargoSpaces, spec.TargetKey, "damage effect", "cargo space").Id,
			VehicleDamageEffectTargetType.InstallationPoint => RequireKey(installationPoints, spec.TargetKey, "damage effect", "installation point").Id,
			VehicleDamageEffectTargetType.TowPoint => RequireKey(towPoints, spec.TargetKey, "damage effect", "tow point").Id,
			_ => throw new ArgumentOutOfRangeException(nameof(spec.TargetType), spec.TargetType, "Unsupported damage effect target type.")
		};
	}
}
