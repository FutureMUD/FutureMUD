#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private Dictionary<string, VehicleInstallationPointProto> UpsertVehicleInstallationPoints(
		VehicleProto vehicle,
		IReadOnlyList<VehicleInstallationPointSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accesses)
	{
		var existing = _context!.VehicleInstallationPointProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleInstallationPointProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "installation point");
			if (row is null)
			{
				row = new VehicleInstallationPointProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleInstallationPointProtos.Add(row);
				existing.Add(row);
			}

			row.RequiredAccessPointProtoId = spec.RequiredAccessPointKey is null ? null : accesses[spec.RequiredAccessPointKey].Id;
			row.Name = spec.Name;
			row.Description = spec.Description;
			row.MountType = spec.MountType;
			row.RequiredRole = spec.RequiredRole;
			row.RequiredForMovement = spec.RequiredForMovement;
			row.DisplayOrder = spec.DisplayOrder;
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private Dictionary<string, VehicleTowPointProto> UpsertVehicleTowPoints(
		VehicleProto vehicle,
		IReadOnlyList<VehicleTowPointSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accesses)
	{
		var existing = _context!.VehicleTowPointProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleTowPointProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "tow point");
			if (row is null)
			{
				row = new VehicleTowPointProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleTowPointProtos.Add(row);
				existing.Add(row);
			}

			row.RequiredAccessPointProtoId = spec.RequiredAccessPointKey is null ? null : accesses[spec.RequiredAccessPointKey].Id;
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
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private void UpsertVehicleDamageZones(
		VehicleProto vehicle,
		IReadOnlyList<VehicleDamageZoneSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleMovementProfileProto> movements,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accesses,
		IReadOnlyDictionary<string, VehicleCargoSpaceProto> cargos,
		IReadOnlyDictionary<string, VehicleInstallationPointProto> installations,
		IReadOnlyDictionary<string, VehicleTowPointProto> towPoints)
	{
		var existing = _context!.VehicleDamageZoneProtos
			.Include(x => x.Effects)
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "damage zone");
			if (row is null)
			{
				row = new VehicleDamageZoneProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber,
					Effects = new HashSet<VehicleDamageZoneEffectProto>()
				};
				_context.VehicleDamageZoneProtos.Add(row);
				existing.Add(row);
			}

			row.Name = spec.Name;
			row.Description = spec.Description;
			row.MaximumDamage = spec.MaximumDamage;
			row.HitWeight = spec.HitWeight;
			row.DisabledThreshold = spec.DisabledThreshold;
			row.DestroyedThreshold = spec.DestroyedThreshold;
			row.DisablesMovement = spec.DisablesMovement;
			row.DisplayOrder = spec.DisplayOrder;
			_context.SaveChanges();

			UpsertVehicleDamageEffects(row, spec.Effects, movements, accesses, cargos, installations, towPoints);
		}

		_context.SaveChanges();
	}

	private void UpsertVehicleDamageEffects(
		VehicleDamageZoneProto zone,
		IReadOnlyList<VehicleDamageEffectSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleMovementProfileProto> movements,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accesses,
		IReadOnlyDictionary<string, VehicleCargoSpaceProto> cargos,
		IReadOnlyDictionary<string, VehicleInstallationPointProto> installations,
		IReadOnlyDictionary<string, VehicleTowPointProto> towPoints)
	{
		var existing = _context!.VehicleDamageZoneEffectProtos
			.Where(x => x.VehicleDamageZoneProtoId == zone.Id)
			.AsEnumerable()
			.ToList();
		foreach (var spec in specs)
		{
			var targetId = ResolveVehicleDamageTargetId(spec, movements, accesses, cargos, installations, towPoints);
			var matches = existing.Where(x => x.TargetType == (int)spec.TargetType && x.TargetProtoId == targetId).ToArray();
			if (matches.Length > 1)
			{
				throw new InvalidOperationException($"Damage zone {zone.Name} contains duplicate effects for {spec.TargetType}/{spec.TargetKey}.");
			}

			var row = matches.SingleOrDefault();
			if (row is null)
			{
				row = new VehicleDamageZoneEffectProto
				{
					VehicleDamageZoneProtoId = zone.Id,
					TargetType = (int)spec.TargetType,
					TargetProtoId = targetId
				};
				_context.VehicleDamageZoneEffectProtos.Add(row);
				existing.Add(row);
			}

			row.MinimumStatus = (int)spec.MinimumStatus;
		}

		_context.SaveChanges();
	}

}
