#nullable enable

using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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
	private void UpsertVehiclePrototype(VehicleSeedSpec spec)
	{
		var vehicleManifestEntry = RegisterManifestAggregate(
			"vehicle",
			spec.StableReference,
			spec,
			module: "vehicles",
			eraAdmissions: spec.SupportedEraKeys);
		if (_manifestCaptureOnly)
		{
			return;
		}

		var priorExterior = FindItemByStableReference($"{spec.StableReference}_exterior");
		var priorVehicle = priorExterior is null ? null : FindVehiclePrototypeByExterior(priorExterior);
		string? priorFingerprint = null;
		if (priorVehicle is not null && !_manifestCaptureOnly)
		{
			priorFingerprint = ItemSeederManifestCatalogue.Fingerprint(
				BuildLiveVehicleGraphManifestDefinition(priorVehicle));
			var managedRecord = FindManagedRecord(vehicleManifestEntry.EntityType, vehicleManifestEntry.StableKey);
			if (managedRecord is null)
			{
				IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"Unmanaged vehicle conflict for '{spec.StableReference}'. The existing legacy graph cannot be proven stock-identical and will not be claimed or overwritten.");
			}
			if (managedRecord.LogicalId is not null && managedRecord.LogicalId != priorVehicle.Id)
			{
				IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"ItemSeeder ownership conflict for vehicle:{spec.StableReference}: provenance names ID {managedRecord.LogicalId:N0}, but the stable exterior resolves to vehicle {priorVehicle.Id:N0}.");
			}
			if (!priorFingerprint.Equals(managedRecord.AppliedFingerprint, StringComparison.OrdinalIgnoreCase))
			{
				MarkManifestAggregateCustomized(vehicleManifestEntry.EntityType, vehicleManifestEntry.StableKey);
				IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Customized = x.Customized + 1 });
				return;
			}
		}

		var eraTags = spec.SupportedEraKeys
			.Select(x => VehicleEraTags[x])
			.ToArray();
		var domainFunctionTag = spec.Domain.Equals(VehicleDomainAquatic, StringComparison.OrdinalIgnoreCase)
			? "Functions / Vehicles / Aquatic Vehicles"
			: "Functions / Vehicles / Terrestrial Vehicles";
		var domainMarketTag = spec.Domain.Equals(VehicleDomainAquatic, StringComparison.OrdinalIgnoreCase)
			? "Market / Transportation / Vehicles / Aquatic Vehicles"
			: "Market / Transportation / Vehicles / Terrestrial Vehicles";
		var tags = new List<string>
		{
			"Functions / Vehicles",
			domainFunctionTag,
			"Market / Transportation / Vehicles",
			domainMarketTag
		};
		tags.AddRange(eraTags);
		if (spec.ProvidesCargoService)
		{
			tags.Add("Market / Transportation / Cargo Transportation");
		}
		if (spec.ProvidesPassengerService)
		{
			tags.Add("Market / Transportation / Passenger Transportation");
		}

		var exteriorComponents = new List<string> { spec.ExteriorItem.DestroyableComponent };
		if (spec.ExteriorItem.Portable)
		{
			exteriorComponents.Insert(0, "Holdable");
		}
		var exterior = UpsertVehicleItem(
			$"{spec.StableReference}_exterior",
			spec.ExteriorItem,
			tags,
			exteriorComponents,
			$"Vehicle prototype stable reference: {spec.StableReference}. {spec.BuilderNotes}".Trim());

		var vehicle = FindVehiclePrototypeByExterior(exterior);
		if (vehicle is null)
		{
			vehicle = new VehicleProto
			{
				Id = NextVehiclePrototypeId(),
				RevisionNumber = 0,
				EditableItem = NewReworkEditableItem()
			};
		}
		if (_context!.Entry(vehicle).State == EntityState.Detached)
		{
			_context.VehicleProtos.Add(vehicle);
		}
		vehicle.Name = spec.Name;
		vehicle.Description = spec.Description.Trim();
		vehicle.VehicleScale = (int)spec.Scale;
		vehicle.ExteriorItemProto = exterior;
		vehicle.ExteriorItemProtoId = exterior.Id;
		vehicle.ExteriorItemProtoRevision = exterior.RevisionNumber;
		var exteriorComponent = EnsureVehicleComponent(
			ComponentName("Exterior", spec.StableReference),
			"Vehicle Exterior",
			$"Internal projection component linking {spec.StableReference} to its exterior item.",
			new XElement("Definition", new XElement("VehiclePrototypeId", vehicle.Id)).ToString());
		EnsureItemHasComponent(exterior, exteriorComponent);

		var compartments = spec.Compartments.ToDictionary(
			x => x.Key,
			x => UpsertCompartment(vehicle, spec.StableReference, x),
			StringComparer.OrdinalIgnoreCase);
		foreach (var link in spec.CompartmentLinks)
		{
			UpsertCompartmentLink(vehicle, link, compartments);
		}

		var slots = spec.OccupantSlots.ToDictionary(
			x => x.Key,
			x => UpsertOccupantSlot(vehicle, x, compartments),
			StringComparer.OrdinalIgnoreCase);
		var movementProfiles = spec.MovementProfiles.ToDictionary(
			x => x.Key,
			x => UpsertMovementProfile(vehicle, x),
			StringComparer.OrdinalIgnoreCase);
		var accessPoints = spec.AccessPoints.ToDictionary(
			x => x.Key,
			x => UpsertAccessPoint(vehicle, spec.StableReference, x, compartments),
			StringComparer.OrdinalIgnoreCase);
		var damageZones = spec.DamageZones.ToDictionary(
			x => x.Key,
			x => UpsertDamageZone(vehicle, spec.StableReference, x),
			StringComparer.OrdinalIgnoreCase);
		foreach (var station in spec.ControlStations)
		{
			UpsertControlStation(vehicle, station, slots);
		}

		foreach (var movementSpec in spec.MovementProfiles)
		{
			foreach (var propulsion in movementSpec.PropulsionProfiles)
			{
				UpsertPropulsionProfile(movementProfiles[movementSpec.Key], propulsion);
			}
		}

		var cargoSpaces = spec.CargoSpaces.ToDictionary(
			x => x.Key,
			x => UpsertCargoSpace(vehicle, spec.StableReference, x, compartments, accessPoints),
			StringComparer.OrdinalIgnoreCase);
		var installationPoints = spec.InstallationPoints.ToDictionary(
			x => x.Key,
			x => UpsertInstallationPoint(vehicle, spec.StableReference, x, accessPoints),
			StringComparer.OrdinalIgnoreCase);
		var towPoints = spec.TowPoints.ToDictionary(
			x => x.Key,
			x => UpsertTowPoint(vehicle, spec.StableReference, x, accessPoints),
			StringComparer.OrdinalIgnoreCase);
		foreach (var zoneSpec in spec.DamageZones)
		{
			foreach (var effectSpec in zoneSpec.Effects)
			{
				UpsertDamageEffect(
					damageZones[zoneSpec.Key],
					effectSpec,
					movementProfiles,
					accessPoints,
					cargoSpaces,
					installationPoints,
					towPoints);
			}
		}

		foreach (var accessSpec in spec.AccessPoints)
		{
			var access = accessPoints[accessSpec.Key];
			var projection = UpsertVehicleProjectionItem(
				$"{spec.StableReference}_access_{NormaliseKey(accessSpec.Key)}",
				accessSpec.ProjectionItem,
				eraTags,
				[]);
			var component = EnsureVehicleComponent(
				ComponentName("Access", spec.StableReference, accessSpec.Key),
				"Vehicle Access Point",
				$"Internal projection component for {spec.StableReference} access point {accessSpec.Key}.",
				new XElement("Definition",
					new XElement("VehiclePrototypeId", vehicle.Id),
					new XElement("AccessPointPrototypeId", access.Id)).ToString());
			EnsureItemHasComponent(projection, component);
			access.ProjectionItemProto = projection;
			access.ProjectionItemProtoId = projection.Id;
			access.ProjectionItemProtoRevision = projection.RevisionNumber;
		}

		foreach (var cargoSpec in spec.CargoSpaces)
		{
			var cargo = cargoSpaces[cargoSpec.Key];
			var projection = UpsertVehicleProjectionItem(
				$"{spec.StableReference}_cargo_{NormaliseKey(cargoSpec.Key)}",
				cargoSpec.ProjectionItem,
				eraTags,
				[cargoSpec.ContainerComponent]);
			var component = EnsureVehicleComponent(
				ComponentName("Cargo", spec.StableReference, cargoSpec.Key),
				"Vehicle Cargo Space",
				$"Internal projection component for {spec.StableReference} cargo space {cargoSpec.Key}.",
				new XElement("Definition",
					new XElement("VehiclePrototypeId", vehicle.Id),
					new XElement("CargoSpacePrototypeId", cargo.Id)).ToString());
			EnsureItemHasComponent(projection, component);
			cargo.ProjectionItemProto = projection;
			cargo.ProjectionItemProtoId = projection.Id;
			cargo.ProjectionItemProtoRevision = projection.RevisionNumber;
		}

		_context.SaveChanges();
		var appliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(
			BuildLiveVehicleGraphManifestDefinition(vehicle));
		RecordAppliedManifestEntry(
			vehicleManifestEntry,
			vehicle.Id,
			vehicle.RevisionNumber,
			appliedFingerprint);
		if (priorVehicle is null)
		{
			IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Inserted = x.Inserted + 1 });
		}
		else if (priorFingerprint!.Equals(appliedFingerprint, StringComparison.OrdinalIgnoreCase))
		{
			IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Unchanged = x.Unchanged + 1 });
		}
		else
		{
			IncrementManifestResult(vehicleManifestEntry.Module, x => x with { Updated = x.Updated + 1 });
		}
	}

	private GameItemProto UpsertVehicleProjectionItem(
		string stableReference,
		VehicleItemSeedSpec spec,
		IEnumerable<string> eraTags,
		IEnumerable<string> extraComponents)
	{
		var components = new List<string> { spec.DestroyableComponent };
		components.AddRange(extraComponents);
		return UpsertVehicleItem(
			stableReference,
			spec with { Portable = false, Skinnable = false, HiddenFromPlayers = true },
			eraTags.Append("Functions / Vehicles / Projection Items"),
			components,
			"Internal vehicle projection item. Do not place or clone directly.");
	}

	private GameItemProto UpsertVehicleItem(
		string stableReference,
		VehicleItemSeedSpec spec,
		IEnumerable<string> tags,
		IEnumerable<string> ordinaryComponents,
		string builderNotes)
	{
		if (!_materials.ContainsKey(spec.Material))
		{
			throw new InvalidOperationException($"Vehicle item {stableReference} uses unknown material {spec.Material}.");
		}
		foreach (var tag in tags)
		{
			EnsureAntiquityTagPath(tag);
		}

		var item = CreateItem(
			stableReference,
			spec.Noun,
			spec.ShortDescription,
			spec.LongDescription,
			spec.FullDescription,
			spec.Size,
			spec.Quality,
			spec.WeightInGrams,
			spec.Cost,
			spec.Skinnable,
			spec.HiddenFromPlayers,
			spec.Material,
			tags,
			ordinaryComponents,
			null,
			null,
			null,
			null,
			builderNotes,
			false) ?? throw new InvalidOperationException($"Unable to create vehicle item {stableReference}.");
		if (_manifestCaptureOnly)
		{
			return item;
		}

		if (IsManifestAggregateCustomized("item", stableReference))
		{
			return item;
		}

		item.Name = spec.Noun.ToLowerInvariant();
		item.Keywords = new ExplodedString(spec.ShortDescription.Strip_A_An()).Words.Distinct().ListToCommaSeparatedValues(" ");
		item.MaterialId = _materials[spec.Material].Id;
		item.Size = (int)spec.Size;
		item.Weight = spec.WeightInGrams;
		item.LongDescription = spec.LongDescription;
		item.BaseItemQuality = (int)spec.Quality;
		item.ShortDescription = spec.ShortDescription;
		item.FullDescription = spec.FullDescription;
		item.PermitPlayerSkins = spec.Skinnable;
		item.CostInBaseCurrency = spec.Cost;
		item.IsHiddenFromPlayers = spec.HiddenFromPlayers;
		item.ReadOnly = false;
		ApplyReworkItemTags(item, tags);
		foreach (var componentName in ordinaryComponents)
		{
			if (!_components.TryGetValue(componentName, out var component))
			{
				throw new InvalidOperationException($"Vehicle item {stableReference} uses unknown component {componentName}.");
			}
			EnsureItemHasComponent(item, component);
		}
		return item;
	}

	private GameItemComponentProto EnsureVehicleComponent(string name, string type, string description, string definition)
	{
		var manifestDefinition = new ComponentManifestDefinition(name, description, type, 0, definition);
		var manifestEntry = RegisterManifestAggregate("component", name, manifestDefinition);
		var component = _components.TryGetValue(name, out var cached)
			? cached
			: _context!.GameItemComponentProtos.Local.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
			  _context.GameItemComponentProtos.AsEnumerable()
				  .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (component is null)
		{
			component = new GameItemComponentProto
			{
				Id = NextComponentId(),
				Name = name,
				Description = description,
				Type = type,
				RevisionNumber = 0,
				Definition = definition,
				EditableItem = NewReworkEditableItem()
			};
			_context!.GameItemComponentProtos.Add(component);
			CompleteManifestAggregate(manifestEntry, component.Id, manifestDefinition, ManifestAggregateDisposition.Insert);
		}
		else
		{
			if (!component.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(
					$"Vehicle component {name} already exists as type {component.Type}, not {type}.");
			}
			var liveDefinition = new ComponentManifestDefinition(
				component.Name, component.Description, component.Type, component.RevisionNumber, component.Definition);
			var disposition = InspectManifestAggregate(manifestEntry, component.Id, liveDefinition);
			if (disposition == ManifestAggregateDisposition.Customized)
			{
				_components[name] = component;
				return component;
			}
			component.Description = description;
			component.Definition = definition;
			if (disposition == ManifestAggregateDisposition.Update)
			{
				CompleteManifestAggregate(manifestEntry, component.Id, manifestDefinition, disposition);
			}
		}
		_components[name] = component;
		return component;
	}

	private void EnsureItemHasComponent(GameItemProto item, GameItemComponentProto component)
	{
		var exists = item.GameItemProtosGameItemComponentProtos.Any(x =>
				x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber) ||
			_context!.GameItemProtosGameItemComponentProtos.Local.Any(x =>
				x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber &&
				x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber) ||
			_context.GameItemProtosGameItemComponentProtos.Any(x =>
				x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber &&
				x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber);
		if (exists)
		{
			return;
		}

		_context!.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemProtoId = item.Id,
			GameItemProtoRevision = item.RevisionNumber,
			GameItemComponentProtoId = component.Id,
			GameItemComponentRevision = component.RevisionNumber,
			GameItemProto = item,
			GameItemComponent = component
		});
	}

}
