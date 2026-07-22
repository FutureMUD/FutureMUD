using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using ExpressionEngine;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Framework.Units;
using MudSharp.TimeAndDate;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class VehiclePrototype : EditableItem, IVehiclePrototype
{
	private readonly List<IVehicleCompartmentPrototype> _compartments = new();
	private readonly List<IVehicleCompartmentLinkPrototype> _compartmentLinks = new();
	private readonly List<IVehicleOccupantSlotPrototype> _occupantSlots = new();
	private readonly List<IVehicleControlStationPrototype> _controlStations = new();
	private readonly List<IVehicleMovementProfilePrototype> _movementProfiles = new();
	private readonly List<IVehicleAccessPointPrototype> _accessPoints = new();
	private readonly List<IVehicleCargoSpacePrototype> _cargoSpaces = new();
	private readonly List<IVehicleInstallationPointPrototype> _installationPoints = new();
	private readonly List<IVehicleTowPointPrototype> _towPoints = new();
	private readonly List<IVehicleDamageZonePrototype> _damageZones = new();
	private string _description;
	private VehicleScale _scale;
	private long? _exteriorItemPrototypeId;
	private int? _exteriorItemPrototypeRevision;

	public VehiclePrototype(DB.VehicleProto dbitem, IFuturemud gameworld) : base(dbitem.EditableItem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_description = dbitem.Description;
		_scale = (VehicleScale)dbitem.VehicleScale;
		_exteriorItemPrototypeId = dbitem.ExteriorItemProtoId;
		_exteriorItemPrototypeRevision = dbitem.ExteriorItemProtoRevision;

		foreach (var item in dbitem.Compartments.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_compartments.Add(new VehicleCompartmentPrototype(item, gameworld));
		}

		foreach (var item in dbitem.CompartmentLinks.OrderBy(x => x.Id))
		{
			_compartmentLinks.Add(new VehicleCompartmentLinkPrototype(item, _compartments));
		}

		foreach (var item in dbitem.OccupantSlots.OrderBy(x => x.Id))
		{
			_occupantSlots.Add(new VehicleOccupantSlotPrototype(item, _compartments, Gameworld));
		}

		foreach (var item in dbitem.ControlStations.OrderBy(x => x.Id))
		{
			_controlStations.Add(new VehicleControlStationPrototype(item, _occupantSlots));
		}

		foreach (var item in dbitem.MovementProfiles.OrderBy(x => x.Id))
		{
			_movementProfiles.Add(new VehicleMovementProfilePrototype(item, gameworld));
		}

		foreach (var item in dbitem.AccessPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_accessPoints.Add(new VehicleAccessPointPrototype(item, _compartments, gameworld));
		}

		foreach (var item in dbitem.CargoSpaces.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_cargoSpaces.Add(new VehicleCargoSpacePrototype(item, _compartments, _accessPoints, gameworld));
		}

		foreach (var item in dbitem.InstallationPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_installationPoints.Add(new VehicleInstallationPointPrototype(item, _accessPoints));
		}

		foreach (var item in dbitem.TowPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_towPoints.Add(new VehicleTowPointPrototype(item, _accessPoints));
		}

		foreach (var item in dbitem.DamageZones.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id))
		{
			_damageZones.Add(new VehicleDamageZonePrototype(item));
		}
	}

	public VehiclePrototype(IFuturemud gameworld, IAccount originator, string name) : base(originator)
	{
		Gameworld = gameworld;
		_id = gameworld.VehiclePrototypes.NextID();
		_name = name.TitleCase();
		_description = "An undescribed vehicle prototype.";
		_scale = VehicleScale.ItemScale;

		using (new FMDB())
		{
			var dbitem = new DB.VehicleProto
			{
				Id = Id,
				RevisionNumber = RevisionNumber,
				Name = Name,
				Description = _description,
				VehicleScale = (int)_scale,
				EditableItem = new DB.EditableItem
				{
					BuilderAccountId = BuilderAccountID,
					BuilderDate = BuilderDate,
					RevisionStatus = (int)Status
				}
			};
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			FMDB.Context.VehicleProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	public override string FrameworkItemType => "VehiclePrototype";
	public VehicleScale Scale => _scale;
	public long? ExteriorItemPrototypeId => _exteriorItemPrototypeId;
	public IGameItemProto ExteriorItemPrototype => _exteriorItemPrototypeId is null
		? null
		: _exteriorItemPrototypeRevision is null
			? Gameworld.ItemProtos.Get(_exteriorItemPrototypeId.Value)
			: Gameworld.ItemProtos.Get(_exteriorItemPrototypeId.Value, _exteriorItemPrototypeRevision.Value);
	public IEnumerable<IVehicleCompartmentPrototype> Compartments => _compartments;
	public IEnumerable<IVehicleCompartmentLinkPrototype> CompartmentLinks => _compartmentLinks;
	public IEnumerable<IVehicleOccupantSlotPrototype> OccupantSlots => _occupantSlots;
	public IEnumerable<IVehicleControlStationPrototype> ControlStations => _controlStations;
	public IEnumerable<IVehicleMovementProfilePrototype> MovementProfiles => _movementProfiles;
	public IEnumerable<IVehicleAccessPointPrototype> AccessPoints => _accessPoints;
	public IEnumerable<IVehicleCargoSpacePrototype> CargoSpaces => _cargoSpaces;
	public IEnumerable<IVehicleInstallationPointPrototype> InstallationPoints => _installationPoints;
	public IEnumerable<IVehicleTowPointPrototype> TowPoints => _towPoints;
	public IEnumerable<IVehicleDamageZonePrototype> DamageZones => _damageZones;

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.VehiclePrototypes.GetAll(Id);
	}

	public override string EditHeader()
	{
		return $"Vehicle Prototype [{Name.Proper().ColourValue()}] (#{Id:N0}r{RevisionNumber:N0})";
	}

	public bool CanCreateVehicle(out string reason)
	{
		if (ExteriorItemPrototype is null)
		{
			reason = "This vehicle prototype does not have an exterior item prototype.";
			return false;
		}

		if (!ExteriorItemPrototype.Components.OfType<VehicleExteriorGameItemComponentProto>()
		                          .Any(x => x.VehiclePrototypeId == Id))
		{
			reason = "The exterior item prototype does not have a vehicle exterior component linked to this vehicle prototype.";
			return false;
		}

		if (Scale == VehicleScale.RoomScale &&
		    !ValidateRoomScaleTopology(_compartments, _compartmentLinks, _accessPoints, out reason))
		{
			return false;
		}

		if (!_occupantSlots.Any())
		{
			reason = "This vehicle prototype does not define any occupant slots.";
			return false;
		}

		if (!_occupantSlots.Any(x => x.SlotType == VehicleOccupantSlotType.Driver))
		{
			reason = "This vehicle prototype does not define a driver slot.";
			return false;
		}

		if (!_controlStations.Any(x => x.OccupantSlot.SlotType == VehicleOccupantSlotType.Driver))
		{
			reason = "This vehicle prototype does not define a control station on a driver slot.";
			return false;
		}

		if (_controlStations.Count(x => x.IsPrimary) != 1)
		{
			reason = "This vehicle prototype must define exactly one primary control station.";
			return false;
		}

		if (!_movementProfiles.Any(x => x.MovementType is VehicleMovementProfileType.CellExit or
			    VehicleMovementProfileType.Route))
		{
			reason = "This vehicle prototype does not define a cell-exit or RouteCell movement profile.";
			return false;
		}

		var invalidMovementProfile = _movementProfiles.FirstOrDefault(x =>
			!double.IsFinite(x.RequiredPowerSpikeInWatts) || x.RequiredPowerSpikeInWatts < 0.0 ||
			!double.IsFinite(x.FuelVolumePerMove) || x.FuelVolumePerMove < 0.0 ||
			!Enum.IsDefined(x.MovementEnvironment) ||
			x.ExposesOccupantsToWater && x.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater);
		if (invalidMovementProfile is not null)
		{
			reason = $"The {invalidMovementProfile.Name} movement profile has invalid resource or environment settings.";
			return false;
		}

		foreach (var movementProfile in _movementProfiles)
		{
			if (movementProfile.MovementType == VehicleMovementProfileType.Route)
			{
				if (!double.IsFinite(movementProfile.RouteSpeedMetresPerSecond) ||
				    movementProfile.RouteSpeedMetresPerSecond <= 0.0 ||
				    !Enum.IsDefined(movementProfile.RoutePropulsionMode) ||
				    !double.IsFinite(movementProfile.RouteFuelVolumePerMetre) ||
				    movementProfile.RouteFuelVolumePerMetre < 0.0 ||
				    !double.IsFinite(movementProfile.RoutePowerDrawWatts) ||
				    movementProfile.RoutePowerDrawWatts < 0.0)
				{
					reason = $"The {movementProfile.Name} RouteCell movement profile has invalid speed, propulsion, fuel, or power settings.";
					return false;
				}

				if (movementProfile.AutomaticOperationCapable &&
				    movementProfile.RoutePropulsionMode != RouteVehiclePropulsionMode.Powered)
				{
					reason = $"The {movementProfile.Name} RouteCell movement profile can only be automatic-capable when powered.";
					return false;
				}
			}

			var propulsionProfiles = movementProfile.PropulsionProfiles.ToList();
			if (movementProfile.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater &&
			    movementProfile.MovementType != VehicleMovementProfileType.CellExit)
			{
				reason = $"The {movementProfile.Name} movement profile uses surface water but is not a cell-exit profile.";
				return false;
			}

			if (propulsionProfiles.Any() &&
			    (movementProfile.MovementType != VehicleMovementProfileType.CellExit ||
			     movementProfile.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater))
			{
				reason = $"The {movementProfile.Name} movement profile has propulsion modes but is not a surface-water cell-exit profile.";
				return false;
			}

			if (movementProfile.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater &&
			    Status == RevisionStatus.UnderDesign && !propulsionProfiles.Any())
			{
				reason = $"The {movementProfile.Name} surface-water movement profile must define an explicit propulsion mode.";
				return false;
			}

			if (propulsionProfiles.Any() && propulsionProfiles.Count(x => x.IsDefault) != 1)
			{
				reason = $"The {movementProfile.Name} movement profile must have exactly one default propulsion mode.";
				return false;
			}

			if (propulsionProfiles.Any(x => x.PropulsionType == VehiclePropulsionType.None) &&
			    propulsionProfiles.Count != 1)
			{
				reason = $"The {movementProfile.Name} movement profile cannot combine none with another propulsion mode.";
				return false;
			}

			foreach (var propulsionProfile in propulsionProfiles)
			{
				if (!VehiclePropulsionProfilePrototype.Validate(propulsionProfile, out reason))
				{
					reason = $"The {movementProfile.Name} movement profile's {propulsionProfile.Name} mode is invalid: {reason}";
					return false;
				}
			}
		}

		foreach (var access in _accessPoints)
		{
			if (access.ProjectionItemPrototype is null)
			{
				reason = $"The {access.Name} access point does not have a projection item prototype.";
				return false;
			}

			if (!access.ProjectionItemPrototype.Components
			           .OfType<VehicleAccessPointGameItemComponentProto>()
			           .Any(x => x.VehiclePrototypeId == Id))
			{
				reason = $"The {access.Name} access point projection item does not have a linked vehicle access component.";
				return false;
			}
		}

		foreach (var cargo in _cargoSpaces)
		{
			if (cargo.ProjectionItemPrototype is null)
			{
				reason = $"The {cargo.Name} cargo space does not have a projection item prototype.";
				return false;
			}

			if (!cargo.ProjectionItemPrototype.Components.OfType<IContainerPrototype>().Any())
			{
				reason = $"The {cargo.Name} cargo projection item is not a container.";
				return false;
			}

			if (!cargo.ProjectionItemPrototype.Components
			          .OfType<VehicleCargoSpaceGameItemComponentProto>()
			          .Any(x => x.VehiclePrototypeId == Id))
			{
				reason = $"The {cargo.Name} cargo projection item does not have a linked vehicle cargo component.";
				return false;
			}
		}

		foreach (var point in _installationPoints)
		{
			if (string.IsNullOrWhiteSpace(point.MountType))
			{
				reason = $"The {point.Name} installation point does not define a mount type.";
				return false;
			}
		}

		var invalidTowPoint = _towPoints.FirstOrDefault(x =>
			!double.IsFinite(x.MaximumTowedWeight) || x.MaximumTowedWeight < 0.0 ||
			!double.IsFinite(x.CharacterPullMultiplier) || x.CharacterPullMultiplier <= 0.0 ||
			x.TowStressWarningRatio is { } warning && (!double.IsFinite(warning) || warning < 0.0 || warning > 1.0) ||
			x.TowStressFailureStartRatio is { } failure && (!double.IsFinite(failure) || failure < 0.0 || failure > 1.0) ||
			x.TowStressMaximumFailureChance is { } chance && (!double.IsFinite(chance) || chance < 0.0 || chance > 1.0) ||
			x.TowStressDamageMultiplier is { } damage && (!double.IsFinite(damage) || damage < 0.0));
		if (invalidTowPoint is not null)
		{
			reason = $"The {invalidTowPoint.Name} tow point has invalid capacity, pull, or stress values.";
			return false;
		}

		foreach (var zone in _damageZones)
		{
			if (!double.IsFinite(zone.MaximumDamage) || !double.IsFinite(zone.HitWeight) ||
			    !double.IsFinite(zone.DisabledThreshold) || !double.IsFinite(zone.DestroyedThreshold) ||
			    zone.MaximumDamage <= 0.0 || zone.HitWeight <= 0.0 ||
			    zone.DisabledThreshold <= 0.0 || zone.DestroyedThreshold < zone.DisabledThreshold)
			{
				reason = $"The {zone.Name} damage zone has invalid thresholds.";
				return false;
			}

			foreach (var effect in zone.Effects)
			{
				if (!ValidateDamageZoneEffect(effect.TargetType, effect.TargetPrototypeId, effect.MinimumStatus,
					    out reason))
				{
					reason = $"The {zone.Name} damage zone has an invalid effect: {reason}";
					return false;
				}
			}
		}

		reason = string.Empty;
		return true;
	}

	public static bool ValidateRoomScaleTopology(
		IEnumerable<IVehicleCompartmentPrototype> compartments,
		IEnumerable<IVehicleCompartmentLinkPrototype> links,
		IEnumerable<IVehicleAccessPointPrototype> accessPoints,
		out string reason)
	{
		var compartmentList = compartments.ToList();
		var linkList = links.ToList();
		if (!compartmentList.Any())
		{
			reason = "A room-scale vehicle must define at least one hosted compartment.";
			return false;
		}

		var missingInterior = compartmentList.FirstOrDefault(x => x.InteriorTerrain is null);
		if (missingInterior is not null)
		{
			reason = $"The {missingInterior.Name} compartment does not define an interior terrain and exposure.";
			return false;
		}

		if (!accessPoints.Any())
		{
			reason = "A room-scale vehicle must define at least one external access point.";
			return false;
		}

		var unboundAccess = accessPoints.FirstOrDefault(x => x.Compartment is null);
		if (unboundAccess is not null)
		{
			reason = $"The {unboundAccess.Name} access point must lead to a compartment on a room-scale vehicle.";
			return false;
		}

		var invalidLink = linkList.FirstOrDefault(x =>
			x.SourceCompartment is null || x.DestinationCompartment is null ||
			x.SourceCompartment.Id == x.DestinationCompartment.Id ||
			string.IsNullOrWhiteSpace(x.OutboundDirection) ||
			string.IsNullOrWhiteSpace(x.InboundDirection) ||
			string.IsNullOrWhiteSpace(x.OutboundDescription) ||
			string.IsNullOrWhiteSpace(x.InboundDescription));
		if (invalidLink is not null)
		{
			reason = $"Compartment link #{invalidLink.Id:N0} is incomplete or links a compartment to itself.";
			return false;
		}

		var duplicatePair = linkList
			.GroupBy(x => string.Join(":", new[] { x.SourceCompartment.Id, x.DestinationCompartment.Id }.Order()))
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicatePair is not null)
		{
			reason = "A pair of hosted compartments has more than one internal link.";
			return false;
		}

		var duplicateDirection = linkList
			.SelectMany(x => new[]
			{
				(x.SourceCompartment.Id, Direction: x.OutboundDirection),
				(x.DestinationCompartment.Id, Direction: x.InboundDirection)
			})
			.GroupBy(x => (x.Id, x.Direction.ToLowerInvariant()))
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicateDirection is not null)
		{
			reason = $"A hosted compartment has more than one internal exit named {duplicateDirection.Key.Item2}.";
			return false;
		}

		if (compartmentList.Count > 1)
		{
			var reachable = new HashSet<long> { compartmentList[0].Id };
			var queue = new Queue<long>(reachable);
			while (queue.TryDequeue(out var current))
			{
				foreach (var next in linkList
					         .Where(x => x.SourceCompartment.Id == current || x.DestinationCompartment.Id == current)
					         .Select(x => x.SourceCompartment.Id == current
						         ? x.DestinationCompartment.Id
						         : x.SourceCompartment.Id))
				{
					if (reachable.Add(next))
					{
						queue.Enqueue(next);
					}
				}
			}

			if (reachable.Count != compartmentList.Count)
			{
				reason = "All room-scale vehicle compartments must be connected by internal links.";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidateDamageZoneEffect(VehicleDamageEffectTargetType targetType, long? targetPrototypeId,
		VehicleSystemStatus minimumStatus, out string reason)
	{
		if (minimumStatus is not VehicleSystemStatus.Disabled and not VehicleSystemStatus.Destroyed)
		{
			reason = "damage effects must activate at disabled or destroyed status.";
			return false;
		}

		if (targetType == VehicleDamageEffectTargetType.WholeVehicleMovement)
		{
			if (targetPrototypeId is not null)
			{
				reason = "whole-vehicle movement effects must use all as their target.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (targetPrototypeId is null)
		{
			reason = string.Empty;
			return true;
		}

		var valid = targetType switch
		{
			VehicleDamageEffectTargetType.MovementProfile => _movementProfiles.Any(x => x.Id == targetPrototypeId.Value),
			VehicleDamageEffectTargetType.AccessPoint => _accessPoints.Any(x => x.Id == targetPrototypeId.Value),
			VehicleDamageEffectTargetType.CargoSpace => _cargoSpaces.Any(x => x.Id == targetPrototypeId.Value),
			VehicleDamageEffectTargetType.InstallationPoint => _installationPoints.Any(x => x.Id == targetPrototypeId.Value),
			VehicleDamageEffectTargetType.TowPoint => _towPoints.Any(x => x.Id == targetPrototypeId.Value),
			_ => false
		};

		reason = valid ? string.Empty : $"target #{targetPrototypeId.Value:N0} is not a valid {targetType.DescribeEnum()} target on this prototype.";
		return valid;
	}

	public override bool CanSubmit()
	{
		return CanCreateVehicle(out _);
	}

	public override string WhyCannotSubmit()
	{
		return CanCreateVehicle(out var reason) ? string.Empty : reason;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle Prototype #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Status: {Status.DescribeColour()}");
		sb.AppendLine($"Scale: {Scale.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Exterior Item: {(ExteriorItemPrototype is null ? "None".ColourError() : $"{ExteriorItemPrototype.ShortDescription.ColourObject()} (#{ExteriorItemPrototype.Id.ToString("N0", actor)}r{ExteriorItemPrototype.RevisionNumber.ToString("N0", actor)})")}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine(_description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Compartments:");
		sb.AppendLine(_compartments.Any()
			? _compartments.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} - {x.Description}{(Scale == VehicleScale.RoomScale ? $" [terrain {(x.InteriorTerrain?.Name.ColourName() ?? "unset".ColourError())}, {x.InteriorOutdoorsType.Describe().ColourValue()}]" : string.Empty)}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Compartment Links:");
		sb.AppendLine(_compartmentLinks.Any()
			? _compartmentLinks.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.SourceCompartment.Name.ColourName()} {x.OutboundDirection.ColourCommand()} -> {x.DestinationCompartment.Name.ColourName()} {x.InboundDirection.ColourCommand()} [{x.OutboundDescription} / {x.InboundDescription}]").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Occupant Slots:");
		sb.AppendLine(_occupantSlots.Any()
			? _occupantSlots.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} [{x.SlotType.DescribeEnum().ColourValue()}] x{x.Capacity.ToString("N0", actor).ColourValue()} in {x.Compartment.Name.ColourName()}{(x.RequiredForMovement ? " required".Colour(Telnet.Yellow) : "")}{(x.ContributesToPropulsion ? " propulsion".Colour(Telnet.Cyan) : "")} stability {x.BoatStabilityDifficulty.Describe().ColourValue()} cover [same {DescribeSlotCover(x.SameLevelRangedCover)}, above {DescribeSlotCover(x.AboveRangedCover)}, below {DescribeSlotCover(x.BelowRangedCover)}]").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Control Stations:");
		sb.AppendLine(_controlStations.Any()
			? _controlStations.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} on {x.OccupantSlot.Name.ColourName()}{(x.IsPrimary ? " primary".Colour(Telnet.Green) : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Movement Profiles:");
		sb.AppendLine(_movementProfiles.Any()
			? _movementProfiles.Select(x => DescribeMovementProfileForShow(x, actor)).ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Access Points:");
		sb.AppendLine(_accessPoints.Any()
			? _accessPoints.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} [{x.AccessPointType.DescribeEnum().ColourValue()}] via {(x.ProjectionItemPrototype is null ? "no projection".ColourError() : x.ProjectionItemPrototype.ShortDescription.ColourObject())}{(x.StartsOpen ? " starts open".Colour(Telnet.Green) : "")}{(x.MustBeClosedForMovement ? " must close to move".Colour(Telnet.Yellow) : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Cargo Spaces:");
		sb.AppendLine(_cargoSpaces.Any()
			? _cargoSpaces.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} via {(x.ProjectionItemPrototype is null ? "no projection".ColourError() : x.ProjectionItemPrototype.ShortDescription.ColourObject())}{(x.RequiredAccessPoint is null ? "" : $" requires {x.RequiredAccessPoint.Name.ColourName()}")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Installation Points:");
		sb.AppendLine(_installationPoints.Any()
			? _installationPoints.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} mount {x.MountType.ColourCommand()}{(string.IsNullOrWhiteSpace(x.RequiredRole) ? "" : $" role {x.RequiredRole.ColourCommand()}")}{(x.RequiredForMovement ? " required".Colour(Telnet.Yellow) : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Tow Points:");
		sb.AppendLine(_towPoints.Any()
			? _towPoints.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} [{x.TowType.ColourCommand()}] {(x.CanTow ? "tow".Colour(Telnet.Green) : "")}{(x.CanBeTowed ? " towed".Colour(Telnet.Green) : "")} max {x.MaximumTowedWeight.ToString("N2", actor).ColourValue()} pull x{x.CharacterPullMultiplier.ToString("N2", actor).ColourValue()}{TowStressSummary(actor, x)}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Damage Zones:");
		sb.AppendLine(_damageZones.Any()
			? _damageZones.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} max {x.MaximumDamage.ToString("N2", actor).ColourValue()} disable {x.DisabledThreshold.ToString("N2", actor).ColourValue()} destroy {x.DestroyedThreshold.ToString("N2", actor).ColourValue()}{(x.DisablesMovement ? " disables movement".Colour(Telnet.Yellow) : "")}{(x.Effects.Any() ? $" effects {x.Effects.Select(effect => DescribeDamageZoneEffect(actor, effect)).ListToString()}" : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		return sb.ToString();
	}

	private static string DescribeSlotCover(IRangedCover cover)
	{
		return cover is null ? "none".ColourError() : cover.Name.ColourName();
	}

	private static string TowStressSummary(ICharacter actor, IVehicleTowPointPrototype towPoint)
	{
		var parts = new List<string>();
		if (towPoint.TowStressWarningRatio is not null)
		{
			parts.Add($"warn {towPoint.TowStressWarningRatio.Value.ToStringP2Colour(actor)}");
		}
		if (towPoint.TowStressFailureStartRatio is not null)
		{
			parts.Add($"fail {towPoint.TowStressFailureStartRatio.Value.ToStringP2Colour(actor)}");
		}
		if (towPoint.TowStressMaximumFailureChance is not null)
		{
			parts.Add($"chance {towPoint.TowStressMaximumFailureChance.Value.ToStringP2Colour(actor)}");
		}
		if (towPoint.TowStressDamageMultiplier is not null)
		{
			parts.Add($"damage x{towPoint.TowStressDamageMultiplier.Value.ToString("N2", actor).ColourValue()}");
		}
		return parts.Any() ? $" stress [{parts.ListToString()}]" : string.Empty;
	}
	private string DescribeDamageZoneEffect(ICharacter actor, IVehicleDamageZoneEffectPrototype effect)
	{
		var target = effect.TargetPrototypeId is null
			? "all".ColourValue()
			: effect.TargetType switch
			{
				VehicleDamageEffectTargetType.MovementProfile => _movementProfiles.FirstOrDefault(x => x.Id == effect.TargetPrototypeId.Value)?.Name.ColourName() ?? $"missing #{effect.TargetPrototypeId.Value.ToString("N0", actor)}".ColourError(),
				VehicleDamageEffectTargetType.AccessPoint => _accessPoints.FirstOrDefault(x => x.Id == effect.TargetPrototypeId.Value)?.Name.ColourName() ?? $"missing #{effect.TargetPrototypeId.Value.ToString("N0", actor)}".ColourError(),
				VehicleDamageEffectTargetType.CargoSpace => _cargoSpaces.FirstOrDefault(x => x.Id == effect.TargetPrototypeId.Value)?.Name.ColourName() ?? $"missing #{effect.TargetPrototypeId.Value.ToString("N0", actor)}".ColourError(),
				VehicleDamageEffectTargetType.InstallationPoint => _installationPoints.FirstOrDefault(x => x.Id == effect.TargetPrototypeId.Value)?.Name.ColourName() ?? $"missing #{effect.TargetPrototypeId.Value.ToString("N0", actor)}".ColourError(),
				VehicleDamageEffectTargetType.TowPoint => _towPoints.FirstOrDefault(x => x.Id == effect.TargetPrototypeId.Value)?.Name.ColourName() ?? $"missing #{effect.TargetPrototypeId.Value.ToString("N0", actor)}".ColourError(),
				_ => "unknown".ColourError()
			};
		return $"#{effect.Id.ToString("N0", actor)} {effect.TargetType.DescribeEnum().ColourValue()} {target} at {effect.MinimumStatus.DescribeEnum().ColourValue()}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
			switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "scale":
				return BuildingCommandScale(actor, command);
			case "exterior":
			case "item":
				return BuildingCommandExterior(actor, command);
			case "compartment":
			case "comp":
				return BuildingCommandCompartment(actor, command);
			case "slot":
				return BuildingCommandSlot(actor, command);
			case "station":
			case "control":
				return BuildingCommandStation(actor, command);
			case "movement":
			case "move":
				return BuildingCommandMovement(actor, command);
			case "access":
			case "accesspoint":
				return BuildingCommandAccess(actor, command);
			case "cargo":
			case "cargospace":
				return BuildingCommandCargo(actor, command);
			case "install":
			case "installpoint":
			case "installation":
				return BuildingCommandInstallPoint(actor, command);
			case "tow":
			case "towpoint":
				return BuildingCommandTowPoint(actor, command);
			case "damage":
			case "zone":
			case "damagezone":
				return BuildingCommandDamageZone(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private const string BuildingHelp = @"You can use the following options with this vehicle prototype:

	#3name <name>#0 - sets the name
	#3desc <description>#0 - sets the description
	#3scale <itemscale|roomcontainer|roomscale>#0 - sets the vehicle scale
	#3exterior <item proto>#0 - links the exterior item prototype
	#3compartment add <name>#0 - adds a compartment
	#3compartment remove <id>#0 - removes a compartment
	#3compartment interior <id> <terrain id|name> <indoors|windows|outdoors|dark|climateexposed>#0 - sets hosted-cell terrain and exposure
	#3compartment link add <source id> <destination id> <out direction> <in direction> ""<out target>"" ""<in target>""#0 - links two hosted compartments
	#3compartment link remove <id>#0 - removes a hosted compartment link
	#3slot add <compartment id> <driver|passenger|crew> <capacity> <name>#0 - adds an occupant slot
	#3slot propulsion <id>#0 - toggles whether occupants in a slot contribute to rowing
	#3slot cover <id> <same|above|below|all> <cover id|name|none>#0 - sets directional ranged cover
	#3slot stability <id> <difficulty>#0 - sets the base boat stability difficulty
	#3slot remove <id>#0 - removes an occupant slot
	#3station add <slot id> <name>#0 - adds a control station
	#3station primary <id>#0 - toggles a primary station
	#3station remove <id>#0 - removes a control station
	#3movement cell#0 - ensures a cell-exit movement profile
	#3movement route#0 - ensures a longitudinal RouteCell movement profile
	#3movement route speed <distance>/<time>#0 - sets longitudinal route speed
	#3movement route propulsion <powered|externallypulled>#0 - sets route propulsion
	#3movement route fuel <liquid id|none> <volume>/<distance>#0 - sets route fuel use
	#3movement route power <watts>#0 - sets continuous route power draw
	#3movement route automatic#0 - toggles automatic-operation capability
	#3movement fuel <id> <liquid id|none> <volume>#0 - configures movement fuel use
	#3movement power <id> <watts>#0 - configures movement power spike use
	#3movement role <id> <role|none>#0 - configures required installed module role
	#3movement environment <id> <unrestricted|surfacewater>#0 - configures the movement environment
	#3movement waterexposure <id> <protected|exposed>#0 - configures occupant water exposure
	#3movement propulsion add <movement id> <selfpowered|rowed|sail|outboard|none>#0 - adds a propulsion mode
	#3movement propulsion remove <propulsion id>#0 - removes a propulsion mode
	#3movement propulsion default <propulsion id>#0 - selects the default mode
	#3movement propulsion time <propulsion id> <seconds>#0 - sets base traversal time
	#3movement propulsion trait <propulsion id> <trait id|name>#0 - sets the propulsion trait
	#3movement propulsion difficulty <propulsion id> <difficulty>#0 - sets the propulsion check difficulty
	#3movement propulsion speed <propulsion id> <expression>#0 - sets the speed multiplier expression
	#3movement propulsion stamina <propulsion id> <expression>#0 - sets the stamina-cost expression
	#3movement access <id>#0 - toggles requiring access points closed
	#3movement tow <id>#0 - toggles requiring valid tow links
	#3movement remove <id>#0 - removes a movement profile
	#3access add <compartment id|none> <door|hatch|ramp|canopy|servicepanel> <item proto> <name>#0 - adds an access point
	#3access open <id>#0 - toggles whether the access point starts open
	#3access closedmove <id>#0 - toggles whether it must be closed to move
	#3access remove <id>#0 - removes an access point
	#3cargo add <compartment id|none> <access id|none> <item proto> <name>#0 - adds a cargo space
	#3cargo remove <id>#0 - removes a cargo space
	#3installpoint add <access id|none> <mount type> <required role|none> <required true|false> <name>#0 - adds an install point
	#3installpoint remove <id>#0 - removes an install point
	#3tow add <access id|none> <tow type> <tow|towed|both> <max weight> [pull <multiplier>] <name>#0 - adds a tow point
	#3tow <id> stress <warning|failstart|maxchance|damage> <value>#0 - sets tow-stress tuning
	#3tow <id> stress reset#0 - clears tow-stress tuning overrides
	#3tow remove <id>#0 - removes a tow point
	#3damage add <max damage> <hit weight> <disable threshold> <destroy threshold> <disables movement true|false> <name>#0 - adds a damage zone
	#3damage <zone id> effect add <wholemovement|movement|access|cargo|install|tow> <id|all> [disabled|destroyed]#0 - adds a damage effect
	#3damage <zone id> effect remove <effect id>#0 - removes a damage effect
	#3damage remove <id>#0 - removes a damage zone";

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this vehicle prototype?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		actor.OutputHandler.Send($"This vehicle prototype is now named {Name.ColourName()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give this vehicle prototype?");
			return false;
		}

		_description = command.SafeRemainingArgument.Proper();
		actor.OutputHandler.Send("You update the description of this vehicle prototype.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandScale(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out VehicleScale scale))
		{
			actor.OutputHandler.Send($"That is not a valid scale. Valid scales are {Enum.GetValues<VehicleScale>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		_scale = scale;
		actor.OutputHandler.Send($"This vehicle prototype is now {scale.DescribeEnum().ColourValue()} scale.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandExterior(ICharacter actor, StringStack command)
	{
		var proto = actor.Gameworld.ItemProtos.GetByIdOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		var vehicleComponents = proto.Components
		                             .OfType<VehicleExteriorGameItemComponentProto>()
		                             .ToList();
		var existingComponent = vehicleComponents.FirstOrDefault(x => x.VehiclePrototypeId == Id);
		if (existingComponent is null)
		{
			if (vehicleComponents.Any())
			{
				actor.OutputHandler.Send("That item prototype is already linked as the exterior of another vehicle prototype.");
				return false;
			}

			if (proto is not GameItemProto concreteProto)
			{
				actor.OutputHandler.Send("That item prototype cannot be updated with the required vehicle exterior component.");
				return false;
			}

			existingComponent = new VehicleExteriorGameItemComponentProto(actor.Gameworld, actor.Account);
			actor.Gameworld.Add(existingComponent);
			existingComponent.ConfigureForVehiclePrototype(this);
			existingComponent.ChangeStatus(RevisionStatus.Current, "Automatically generated for vehicle exterior item.", actor.Account);
			concreteProto.AddComponent(existingComponent);
			actor.OutputHandler.Send($"A vehicle exterior component was automatically created and attached to {proto.ShortDescription.ColourObject()}.");
		}

		_exteriorItemPrototypeId = proto.Id;
		_exteriorItemPrototypeRevision = proto.RevisionNumber;
		actor.OutputHandler.Send($"This vehicle prototype will now use {proto.ShortDescription.ColourObject()} (#{proto.Id.ToString("N0", actor)}r{proto.RevisionNumber.ToString("N0", actor)}) as its exterior item prototype.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandCompartment(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What name should this compartment have?");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = new DB.VehicleCompartmentProto
					{
						VehicleProtoId = Id,
						VehicleProtoRevision = RevisionNumber,
						Name = command.SafeRemainingArgument.TitleCase(),
						Description = "An undescribed compartment.",
						DisplayOrder = _compartments.Count + 1
					};
					FMDB.Context.VehicleCompartmentProtos.Add(dbitem);
					FMDB.Context.SaveChanges();
					_compartments.Add(new VehicleCompartmentPrototype(dbitem, Gameworld));
				}

				actor.OutputHandler.Send("You add a new vehicle compartment.");
				return true;
			case "interior":
				return BuildingCommandCompartmentInterior(actor, command);
			case "link":
				return BuildingCommandCompartmentLink(actor, command);
			case "remove":
			case "delete":
				if (!long.TryParse(command.PopSpeech(), out var id))
				{
					actor.OutputHandler.Send("Which compartment ID do you want to remove?");
					return false;
				}

				var comp = _compartments.FirstOrDefault(x => x.Id == id);
				if (comp is null)
				{
					actor.OutputHandler.Send("There is no such compartment.");
					return false;
				}

				if (_occupantSlots.Any(x => x.Compartment.Id == id))
				{
					actor.OutputHandler.Send("You must remove occupant slots from that compartment first.");
					return false;
				}

				if (_compartmentLinks.Any(x => x.SourceCompartment.Id == id || x.DestinationCompartment.Id == id))
				{
					actor.OutputHandler.Send("You must remove internal links from that compartment first.");
					return false;
				}

				if (_accessPoints.Any(x => x.Compartment?.Id == id) || _cargoSpaces.Any(x => x.Compartment?.Id == id))
				{
					actor.OutputHandler.Send("You must remove access points and cargo spaces from that compartment first.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleCompartmentProtos.Find(id);
					if (dbitem is not null)
					{
						FMDB.Context.VehicleCompartmentProtos.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}

				_compartments.Remove(comp);
				actor.OutputHandler.Send("You remove that compartment.");
				return true;
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandCompartmentInterior(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which compartment ID do you want to configure?");
			return false;
		}

		var index = _compartments.FindIndex(x => x.Id == id);
		if (index < 0)
		{
			actor.OutputHandler.Send("There is no such compartment.");
			return false;
		}

		var terrain = actor.Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
		if (terrain is null)
		{
			actor.OutputHandler.Send("There is no such terrain.");
			return false;
		}

		if (!TryParseInteriorOutdoorsType(command.PopSpeech(), out var outdoorsType))
		{
			actor.OutputHandler.Send("Exposure must be indoors, windows, outdoors, dark, or climateexposed.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleCompartmentProtos.Find(id);
			if (dbitem is null)
			{
				actor.OutputHandler.Send("That compartment no longer exists in the database.");
				return false;
			}

			dbitem.InteriorTerrainId = terrain.Id;
			dbitem.InteriorOutdoorsType = (int)outdoorsType;
			FMDB.Context.SaveChanges();
			_compartments[index] = new VehicleCompartmentPrototype(dbitem, Gameworld);
		}

		Changed = true;
		actor.OutputHandler.Send($"The {_compartments[index].Name.ColourName()} hosted interior now uses {terrain.Name.ColourName()} and {outdoorsType.Describe().ColourValue()} exposure.");
		return true;
	}

	private bool BuildingCommandCompartmentLink(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				if (!long.TryParse(command.PopSpeech(), out var sourceId) ||
				    !long.TryParse(command.PopSpeech(), out var destinationId))
				{
					actor.OutputHandler.Send("You must specify source and destination compartment IDs.");
					return false;
				}

				var source = _compartments.FirstOrDefault(x => x.Id == sourceId);
				var destination = _compartments.FirstOrDefault(x => x.Id == destinationId);
				if (source is null || destination is null)
				{
					actor.OutputHandler.Send("One or both compartment IDs do not exist on this prototype.");
					return false;
				}

				if (source.Id == destination.Id)
				{
					actor.OutputHandler.Send("A compartment cannot link to itself.");
					return false;
				}

				if (_compartmentLinks.Any(x =>
					    x.SourceCompartment.Id == source.Id && x.DestinationCompartment.Id == destination.Id ||
					    x.SourceCompartment.Id == destination.Id && x.DestinationCompartment.Id == source.Id))
				{
					actor.OutputHandler.Send("Those two compartments are already linked.");
					return false;
				}

				var outboundDirection = command.PopSpeech().ToLowerInvariant();
				var inboundDirection = command.PopSpeech().ToLowerInvariant();
				var outboundDescription = command.PopSpeech();
				var inboundDescription = command.PopSpeech();
				if (new[] { outboundDirection, inboundDirection, outboundDescription, inboundDescription }
					.Any(string.IsNullOrWhiteSpace))
				{
					actor.OutputHandler.Send("You must specify both directions and both quoted destination descriptions.");
					return false;
				}

				if (_compartmentLinks.Any(x =>
					    x.SourceCompartment.Id == source.Id && x.OutboundDirection.EqualTo(outboundDirection) ||
					    x.DestinationCompartment.Id == source.Id && x.InboundDirection.EqualTo(outboundDirection) ||
					    x.SourceCompartment.Id == destination.Id && x.OutboundDirection.EqualTo(inboundDirection) ||
					    x.DestinationCompartment.Id == destination.Id && x.InboundDirection.EqualTo(inboundDirection)))
				{
					actor.OutputHandler.Send("One of those compartment directions is already in use.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = new DB.VehicleCompartmentLinkProto
					{
						VehicleProtoId = Id,
						VehicleProtoRevision = RevisionNumber,
						SourceVehicleCompartmentProtoId = source.Id,
						DestinationVehicleCompartmentProtoId = destination.Id,
						OutboundDirection = outboundDirection,
						InboundDirection = inboundDirection,
						OutboundDescription = outboundDescription,
						InboundDescription = inboundDescription
					};
					FMDB.Context.VehicleCompartmentLinkProtos.Add(dbitem);
					FMDB.Context.SaveChanges();
					_compartmentLinks.Add(new VehicleCompartmentLinkPrototype(dbitem, _compartments));
				}

				Changed = true;
				actor.OutputHandler.Send($"You link {source.Name.ColourName()} to {destination.Name.ColourName()}.");
				return true;
			case "remove":
			case "delete":
				if (!long.TryParse(command.PopSpeech(), out var linkId))
				{
					actor.OutputHandler.Send("Which compartment link ID do you want to remove?");
					return false;
				}

				var link = _compartmentLinks.FirstOrDefault(x => x.Id == linkId);
				if (link is null)
				{
					actor.OutputHandler.Send("There is no such compartment link.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleCompartmentLinkProtos.Find(linkId);
					if (dbitem is not null)
					{
						FMDB.Context.VehicleCompartmentLinkProtos.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}

				_compartmentLinks.Remove(link);
				Changed = true;
				actor.OutputHandler.Send("You remove that compartment link.");
				return true;
			default:
				actor.OutputHandler.Send("Use #3compartment link add ...#0 or #3compartment link remove <id>#0.".SubstituteANSIColour());
				return false;
		}
	}

	public static bool TryParseInteriorOutdoorsType(string text, out CellOutdoorsType outdoorsType)
	{
		outdoorsType = text.ToLowerInvariant() switch
		{
			"indoors" or "indoor" => CellOutdoorsType.Indoors,
			"windows" or "window" or "indoorswithwindows" => CellOutdoorsType.IndoorsWithWindows,
			"outdoors" or "outdoor" => CellOutdoorsType.Outdoors,
			"dark" or "nolight" or "indoorsnolight" => CellOutdoorsType.IndoorsNoLight,
			"climateexposed" or "exposed" or "indoorsclimateexposed" => CellOutdoorsType.IndoorsClimateExposed,
			_ => (CellOutdoorsType)(-1)
		};
		return Enum.IsDefined(outdoorsType);
	}

	private bool BuildingCommandSlot(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				if (!long.TryParse(command.PopSpeech(), out var compartmentId))
				{
					actor.OutputHandler.Send("Which compartment ID should contain this slot?");
					return false;
				}

				var compartment = _compartments.FirstOrDefault(x => x.Id == compartmentId);
				if (compartment is null)
				{
					actor.OutputHandler.Send("There is no such compartment.");
					return false;
				}

				if (!command.PopSpeech().TryParseEnum(out VehicleOccupantSlotType slotType))
				{
					actor.OutputHandler.Send("You must specify a slot type: driver, passenger or crew.");
					return false;
				}

				if (!int.TryParse(command.PopSpeech(), out var capacity) || capacity < 1)
				{
					actor.OutputHandler.Send("You must specify a positive capacity.");
					return false;
				}

				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What should this slot be called?");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = new DB.VehicleOccupantSlotProto
					{
						VehicleProtoId = Id,
						VehicleProtoRevision = RevisionNumber,
						VehicleCompartmentProtoId = compartment.Id,
						Name = command.SafeRemainingArgument.TitleCase(),
						SlotType = (int)slotType,
						Capacity = capacity,
						RequiredForMovement = slotType == VehicleOccupantSlotType.Driver,
						BoatStabilityDifficulty = (int)Difficulty.Normal
					};
					FMDB.Context.VehicleOccupantSlotProtos.Add(dbitem);
					FMDB.Context.SaveChanges();
					_occupantSlots.Add(new VehicleOccupantSlotPrototype(dbitem, _compartments, Gameworld));
				}

				actor.OutputHandler.Send("You add a new vehicle occupant slot.");
				return true;
			case "propulsion":
			case "rower":
				if (!long.TryParse(command.PopSpeech(), out var propulsionSlotId))
				{
					actor.OutputHandler.Send("Which occupant slot ID do you want to toggle for propulsion?");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleOccupantSlotProtos.Find(propulsionSlotId);
					if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
					{
						actor.OutputHandler.Send("There is no such occupant slot.");
						return false;
					}

					dbitem.ContributesToPropulsion = !dbitem.ContributesToPropulsion;
					FMDB.Context.SaveChanges();
					actor.OutputHandler.Send($"Occupants in that slot will {(dbitem.ContributesToPropulsion ? "now" : "no longer")} contribute to propulsion.");
				}

				ReloadChildDefinitions();
				return true;
			case "cover":
				return BuildingCommandSlotCover(actor, command);
			case "stability":
				return BuildingCommandSlotStability(actor, command);
			case "remove":
			case "delete":
				if (!long.TryParse(command.PopSpeech(), out var id))
				{
					actor.OutputHandler.Send("Which slot ID do you want to remove?");
					return false;
				}

				var slot = _occupantSlots.FirstOrDefault(x => x.Id == id);
				if (slot is null)
				{
					actor.OutputHandler.Send("There is no such occupant slot.");
					return false;
				}

				if (_controlStations.Any(x => x.OccupantSlot.Id == id))
				{
					actor.OutputHandler.Send("You must remove control stations from that slot first.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleOccupantSlotProtos.Find(id);
					if (dbitem is not null)
					{
						FMDB.Context.VehicleOccupantSlotProtos.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}

				_occupantSlots.Remove(slot);
				actor.OutputHandler.Send("You remove that occupant slot.");
				return true;
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandStation(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				if (!long.TryParse(command.PopSpeech(), out var slotId))
				{
					actor.OutputHandler.Send("Which slot ID should host this control station?");
					return false;
				}

				var slot = _occupantSlots.FirstOrDefault(x => x.Id == slotId);
				if (slot is null)
				{
					actor.OutputHandler.Send("There is no such occupant slot.");
					return false;
				}

				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What should this control station be called?");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = new DB.VehicleControlStationProto
					{
						VehicleProtoId = Id,
						VehicleProtoRevision = RevisionNumber,
						VehicleOccupantSlotProtoId = slot.Id,
						Name = command.SafeRemainingArgument.TitleCase(),
						IsPrimary = !_controlStations.Any()
					};
					FMDB.Context.VehicleControlStationProtos.Add(dbitem);
					FMDB.Context.SaveChanges();
					_controlStations.Add(new VehicleControlStationPrototype(dbitem, _occupantSlots));
				}

				actor.OutputHandler.Send("You add a new vehicle control station.");
				return true;
			case "primary":
				if (!long.TryParse(command.PopSpeech(), out var id))
				{
					actor.OutputHandler.Send("Which control station ID should be primary?");
					return false;
				}

				if (_controlStations.All(x => x.Id != id))
				{
					actor.OutputHandler.Send("There is no such control station.");
					return false;
				}

				using (new FMDB())
				{
					foreach (var dbitem in FMDB.Context.VehicleControlStationProtos.Where(x => x.VehicleProtoId == Id && x.VehicleProtoRevision == RevisionNumber))
					{
						dbitem.IsPrimary = dbitem.Id == id;
					}

					FMDB.Context.SaveChanges();
				}

				ReloadChildDefinitions();
				actor.OutputHandler.Send("You update the primary control station.");
				return true;
			case "remove":
			case "delete":
				if (!long.TryParse(command.PopSpeech(), out id))
				{
					actor.OutputHandler.Send("Which control station ID do you want to remove?");
					return false;
				}

				var station = _controlStations.FirstOrDefault(x => x.Id == id);
				if (station is null)
				{
					actor.OutputHandler.Send("There is no such control station.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleControlStationProtos.Find(id);
					if (dbitem is not null)
					{
						FMDB.Context.VehicleControlStationProtos.Remove(dbitem);
						if (station.IsPrimary)
						{
							var replacement = FMDB.Context.VehicleControlStationProtos
								.FirstOrDefault(x => x.VehicleProtoId == Id &&
								                     x.VehicleProtoRevision == RevisionNumber && x.Id != id);
							if (replacement is not null)
							{
								replacement.IsPrimary = true;
							}
						}
						FMDB.Context.SaveChanges();
					}
				}

				ReloadChildDefinitions();
				actor.OutputHandler.Send("You remove that control station.");
				return true;
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandMovement(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "cell":
			case "cellexit":
				if (_movementProfiles.Any(x => x.MovementType == VehicleMovementProfileType.CellExit))
				{
					actor.OutputHandler.Send("This vehicle prototype already has a cell-exit movement profile.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = new DB.VehicleMovementProfileProto
					{
						VehicleProtoId = Id,
						VehicleProtoRevision = RevisionNumber,
						Name = "Cell Exit Movement",
						MovementType = (int)VehicleMovementProfileType.CellExit,
						IsDefault = !_movementProfiles.Any(),
						RequiredInstalledRole = string.Empty
					};
					FMDB.Context.VehicleMovementProfileProtos.Add(dbitem);
					FMDB.Context.SaveChanges();
					_movementProfiles.Add(new VehicleMovementProfilePrototype(dbitem, Gameworld));
				}

				actor.OutputHandler.Send("You add a cell-exit movement profile.");
				return true;
			case "route":
				return BuildingCommandMovementRoute(actor, command);
			case "remove":
			case "delete":
				if (!long.TryParse(command.PopSpeech(), out var id))
				{
					actor.OutputHandler.Send("Which movement profile ID do you want to remove?");
					return false;
				}

				var profile = _movementProfiles.FirstOrDefault(x => x.Id == id);
				if (profile is null)
				{
					actor.OutputHandler.Send("There is no such movement profile.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
					if (dbitem is not null)
					{
						FMDB.Context.VehicleMovementProfileProtos.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}

				_movementProfiles.Remove(profile);
				actor.OutputHandler.Send("You remove that movement profile.");
				return true;
			case "fuel":
				return BuildingCommandMovementFuel(actor, command);
			case "power":
				return BuildingCommandMovementPower(actor, command);
			case "role":
				return BuildingCommandMovementRole(actor, command);
			case "environment":
			case "environmenttype":
				return BuildingCommandMovementEnvironment(actor, command);
			case "waterexposure":
			case "water":
				return BuildingCommandMovementWaterExposure(actor, command);
			case "propulsion":
			case "propel":
				return BuildingCommandMovementPropulsion(actor, command);
			case "access":
				return BuildingCommandMovementAccess(actor, command);
			case "tow":
				return BuildingCommandMovementTow(actor, command);
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandMovementRoute(ICharacter actor, StringStack command)
	{
		var profile = _movementProfiles.FirstOrDefault(x => x.MovementType == VehicleMovementProfileType.Route);
		if (command.IsFinished)
		{
			if (profile is not null)
			{
				actor.OutputHandler.Send("This vehicle prototype already has a RouteCell movement profile.");
				return false;
			}

			using (new FMDB())
			{
				var dbitem = new DB.VehicleMovementProfileProto
				{
					VehicleProtoId = Id,
					VehicleProtoRevision = RevisionNumber,
					Name = "RouteCell Movement",
					MovementType = (int)VehicleMovementProfileType.Route,
					MovementEnvironment = (int)VehicleMovementEnvironment.Unrestricted,
					IsDefault = !_movementProfiles.Any(),
					RequiredInstalledRole = string.Empty,
					RouteSpeedMetresPerSecond = 1.0,
					RoutePropulsionMode = (int)RouteVehiclePropulsionMode.Powered
				};
				FMDB.Context.VehicleMovementProfileProtos.Add(dbitem);
				FMDB.Context.SaveChanges();
				_movementProfiles.Add(new VehicleMovementProfilePrototype(dbitem, Gameworld));
			}

			actor.OutputHandler.Send("You add a RouteCell movement profile with a default speed of one metre per second.");
			return true;
		}

		if (profile is null)
		{
			actor.OutputHandler.Send("Add the RouteCell movement profile with #3movement route#0 first."
				.SubstituteANSIColour());
			return false;
		}

		var option = command.PopSpeech().ToLowerInvariant();
		switch (option)
		{
			case "speed":
				if (!TryParseRouteSpeed(actor, command.SafeRemainingArgument, out var speed))
				{
					actor.OutputHandler.Send("Specify a positive longitudinal speed as #3<distance>/<time>#0, for example #310m/2s#0."
						.SubstituteANSIColour());
					return false;
				}

				using (new FMDB())
				{
					FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!.RouteSpeedMetresPerSecond = speed;
					FMDB.Context.SaveChanges();
				}
				ReloadChildDefinitions();
				actor.OutputHandler.Send($"That vehicle now travels along RouteCells at {speed.ToString("N3", actor).ColourValue()} metres per second.");
				return true;
			case "propulsion":
			case "propel":
				var propulsion = command.PopSpeech().ToLowerInvariant() switch
				{
					"powered" or "power" => RouteVehiclePropulsionMode.Powered,
					"externallypulled" or "externally-pulled" or "pulled" or "tow" => RouteVehiclePropulsionMode.ExternallyPulled,
					_ => (RouteVehiclePropulsionMode?)null
				};
				if (propulsion is null)
				{
					actor.OutputHandler.Send("Specify either #3powered#0 or #3externallypulled#0."
						.SubstituteANSIColour());
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!;
					dbitem.RoutePropulsionMode = (int)propulsion.Value;
					if (propulsion == RouteVehiclePropulsionMode.ExternallyPulled)
					{
						dbitem.AutomaticOperationCapable = false;
					}
					FMDB.Context.SaveChanges();
				}
				ReloadChildDefinitions();
				actor.OutputHandler.Send($"That RouteCell movement profile is now {propulsion.Value.DescribeEnum().ColourName()}.");
				return true;
			case "fuel":
				return BuildingCommandMovementRouteFuel(actor, command, profile);
			case "power":
				if (!double.TryParse(command.SafeRemainingArgument, actor, out var watts) ||
				    !double.IsFinite(watts) || watts < 0.0)
				{
					actor.OutputHandler.Send("Specify a non-negative continuous power draw in watts.");
					return false;
				}

				using (new FMDB())
				{
					FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!.RoutePowerDrawWatts = watts;
					FMDB.Context.SaveChanges();
				}
				ReloadChildDefinitions();
				actor.OutputHandler.Send($"That RouteCell movement profile now draws {watts.ToString("N2", actor).ColourValue()} watts while moving.");
				return true;
			case "automatic":
			case "auto":
				if (profile.RoutePropulsionMode != RouteVehiclePropulsionMode.Powered)
				{
					actor.OutputHandler.Send("Only powered RouteCell movement profiles can be automatic-capable.");
					return false;
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!;
					dbitem.AutomaticOperationCapable = !dbitem.AutomaticOperationCapable;
					FMDB.Context.SaveChanges();
				}
				ReloadChildDefinitions();
				actor.OutputHandler.Send($"That RouteCell movement profile is {(profile.AutomaticOperationCapable ? "no longer" : "now")} automatic-capable.");
				return true;
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandMovementRouteFuel(ICharacter actor, StringStack command,
		IVehicleMovementProfilePrototype profile)
	{
		var liquidText = command.PopSpeech();
		if (string.IsNullOrWhiteSpace(liquidText))
		{
			actor.OutputHandler.Send("Specify a fuel liquid, or #3none#0 to clear route fuel use."
				.SubstituteANSIColour());
			return false;
		}

		if (liquidText.EqualTo("none"))
		{
			using (new FMDB())
			{
				var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!;
				dbitem.FuelLiquidId = null;
				dbitem.RouteFuelVolumePerMetre = 0.0;
				FMDB.Context.SaveChanges();
			}
			ReloadChildDefinitions();
			actor.OutputHandler.Send("That RouteCell movement profile no longer consumes liquid fuel.");
			return true;
		}

		var liquid = actor.Gameworld.Liquids.GetByIdOrName(liquidText);
		if (liquid is null || !TryParseFuelPerDistance(actor, command.SafeRemainingArgument, out var volumePerMetre))
		{
			actor.OutputHandler.Send("Specify a valid liquid and a positive #3<volume>/<distance>#0 rate, for example #31L/100km#0."
				.SubstituteANSIColour());
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(profile.Id)!;
			dbitem.FuelLiquidId = liquid.Id;
			dbitem.RouteFuelVolumePerMetre = volumePerMetre;
			FMDB.Context.SaveChanges();
		}
		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That RouteCell movement profile now consumes {volumePerMetre.ToString("N8", actor).ColourValue()} base-volume units per metre of {liquid.Name.ColourName()}.");
		return true;
	}

	internal static bool TryParseRouteSpeed(ICharacter actor, string text, out double metresPerSecond)
	{
		metresPerSecond = 0.0;
		if (!TrySplitRate(text, out var distanceText, out var durationText) ||
		    !actor.Gameworld.UnitManager.TryGetBaseUnits(distanceText, UnitType.Length, actor,
			    out var baseDistance) ||
		    !TryParseRouteSpeedDuration(actor, durationText, out var duration))
		{
			return false;
		}

		metresPerSecond = baseDistance * actor.Gameworld.UnitManager.BaseHeightToMetres /
		                  duration.TotalSeconds;
		return double.IsFinite(metresPerSecond) && metresPerSecond > 0.0;
	}

	private static bool TryParseRouteSpeedDuration(ICharacter actor, string text, out TimeSpan duration)
	{
		duration = TimeSpan.Zero;
		var durationText = text.Trim();
		if (!MudTimeSpan.TryParse(durationText, actor, out var mudDuration))
		{
			if (!durationText.ToLowerInvariant().EqualToAny(
				    "ms", "millisecond", "milliseconds",
				    "s", "sec", "secs", "second", "seconds",
				    "m", "min", "mins", "minute", "minutes",
				    "h", "hr", "hrs", "hour", "hours",
				    "d", "day", "days",
				    "w", "week", "weeks",
				    "mo", "mon", "mons", "month", "months",
				    "y", "year", "years") ||
			    !MudTimeSpan.TryParse($"1{durationText}", actor, out mudDuration))
			{
				return false;
			}
		}

		duration = mudDuration;
		return duration > TimeSpan.Zero;
	}

	private static bool TryParseFuelPerDistance(ICharacter actor, string text, out double volumePerMetre)
	{
		volumePerMetre = 0.0;
		if (!TrySplitRate(text, out var volumeText, out var distanceText) ||
		    !actor.Gameworld.UnitManager.TryGetBaseUnits(volumeText, UnitType.FluidVolume, actor,
			    out var volume) ||
		    !actor.Gameworld.UnitManager.TryGetBaseUnits(distanceText, UnitType.Length, actor,
			    out var baseDistance))
		{
			return false;
		}

		var metres = baseDistance * actor.Gameworld.UnitManager.BaseHeightToMetres;
		volumePerMetre = volume / metres;
		return volume > 0.0 && metres > 0.0 && double.IsFinite(volumePerMetre);
	}

	private static bool TrySplitRate(string text, out string numerator, out string denominator)
	{
		var index = text?.LastIndexOf('/') ?? -1;
		if (index <= 0 || index >= text!.Length - 1)
		{
			numerator = string.Empty;
			denominator = string.Empty;
			return false;
		}

		numerator = text[..index].Trim();
		denominator = text[(index + 1)..].Trim();
		return numerator.Length > 0 && denominator.Length > 0;
	}

	private bool BuildingCommandSlotCover(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var slotId))
		{
			actor.OutputHandler.Send("Which occupant slot ID do you want to configure?");
			return false;
		}

		var direction = command.PopSpeech().ToLowerInvariant();
		if (!direction.EqualToAny("same", "samelevel", "above", "below", "all"))
		{
			actor.OutputHandler.Send("You must specify same, above, below or all.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ranged cover should apply, or none to clear it?");
			return false;
		}

		IRangedCover cover = null;
		if (!command.SafeRemainingArgument.EqualTo("none"))
		{
			cover = actor.Gameworld.RangedCovers.GetByIdOrName(command.SafeRemainingArgument);
			if (cover is null)
			{
				actor.OutputHandler.Send("There is no such ranged cover definition.");
				return false;
			}
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleOccupantSlotProtos.Find(slotId);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such occupant slot.");
				return false;
			}

			if (direction.EqualToAny("same", "samelevel", "all"))
			{
				dbitem.SameLevelRangedCoverId = cover?.Id;
			}
			if (direction.EqualToAny("above", "all"))
			{
				dbitem.AboveRangedCoverId = cover?.Id;
			}
			if (direction.EqualToAny("below", "all"))
			{
				dbitem.BelowRangedCoverId = cover?.Id;
			}

			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That slot now uses {(cover is null ? "no cover".ColourError() : cover.Name.ColourName())} against attacks from {direction.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSlotStability(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var slotId))
		{
			actor.OutputHandler.Send("Which occupant slot ID do you want to configure?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("You must specify a valid difficulty. See SHOW DIFFICULTIES.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleOccupantSlotProtos.Find(slotId);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such occupant slot.");
				return false;
			}

			dbitem.BoatStabilityDifficulty = (int)difficulty;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That slot now uses {difficulty.Describe().ColourValue()} boat stability checks.");
		return true;
	}

	private bool BuildingCommandMovementPropulsion(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandMovementPropulsionAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandMovementPropulsionRemove(actor, command);
			case "default":
				return BuildingCommandMovementPropulsionDefault(actor, command);
			case "time":
				return BuildingCommandMovementPropulsionTime(actor, command);
			case "trait":
			case "skill":
				return BuildingCommandMovementPropulsionTrait(actor, command);
			case "difficulty":
			case "diff":
				return BuildingCommandMovementPropulsionDifficulty(actor, command);
			case "speed":
				return BuildingCommandMovementPropulsionExpression(actor, command, true);
			case "stamina":
			case "cost":
				return BuildingCommandMovementPropulsionExpression(actor, command, false);
		}

		actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandMovementPropulsionAdd(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var movementId))
		{
			actor.OutputHandler.Send("Which movement profile ID should receive this propulsion mode?");
			return false;
		}

		var type = ParseVehiclePropulsionType(command.PopSpeech());
		if (type is null)
		{
			actor.OutputHandler.Send("You must specify selfpowered, rowed, sail, outboard or none.");
			return false;
		}

		using (new FMDB())
		{
			var movement = FMDB.Context.VehicleMovementProfileProtos
				.Include(x => x.PropulsionProfiles)
				.FirstOrDefault(x => x.Id == movementId && x.VehicleProtoId == Id &&
				                     x.VehicleProtoRevision == RevisionNumber);
			if (movement is null)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			if ((VehicleMovementProfileType)movement.MovementType != VehicleMovementProfileType.CellExit ||
			    (VehicleMovementEnvironment)movement.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater)
			{
				actor.OutputHandler.Send("Propulsion modes can only be added to a surface-water cell-exit movement profile.");
				return false;
			}

			if (movement.PropulsionProfiles.Any(x => x.PropulsionType == (int)type.Value))
			{
				actor.OutputHandler.Send("That movement profile already has that propulsion mode.");
				return false;
			}

			if ((type == VehiclePropulsionType.None && movement.PropulsionProfiles.Any()) ||
			    (type != VehiclePropulsionType.None && movement.PropulsionProfiles.Any(x =>
				    x.PropulsionType == (int)VehiclePropulsionType.None)))
			{
				actor.OutputHandler.Send("The none propulsion mode is exclusive and cannot be combined with another mode.");
				return false;
			}

			var dbitem = new DB.VehiclePropulsionProfileProto
			{
				VehicleMovementProfileProtoId = movement.Id,
				PropulsionType = (int)type.Value,
				IsDefault = !movement.PropulsionProfiles.Any(),
				BaseMoveTimeMilliseconds = 10000.0,
				CheckDifficulty = (int)Difficulty.Normal,
				SpeedMultiplierExpression = DefaultPropulsionSpeedExpression(type.Value),
				StaminaCostExpression = DefaultPropulsionStaminaExpression(type.Value)
			};
			FMDB.Context.VehiclePropulsionProfileProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"You add the {type.Value.DescribeEnum().ColourName()} propulsion mode.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionRemove(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which propulsion profile ID do you want to remove?");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehiclePropulsionProfileProtos
				.Include(x => x.VehicleMovementProfileProto)
				.FirstOrDefault(x => x.Id == id && x.VehicleMovementProfileProto.VehicleProtoId == Id &&
				                     x.VehicleMovementProfileProto.VehicleProtoRevision == RevisionNumber);
			if (dbitem is null)
			{
				actor.OutputHandler.Send("There is no such propulsion profile.");
				return false;
			}

			var movementId = dbitem.VehicleMovementProfileProtoId;
			var wasDefault = dbitem.IsDefault;
			FMDB.Context.VehiclePropulsionProfileProtos.Remove(dbitem);
			FMDB.Context.SaveChanges();
			if (wasDefault)
			{
				var replacement = FMDB.Context.VehiclePropulsionProfileProtos
					.OrderBy(x => x.Id)
					.FirstOrDefault(x => x.VehicleMovementProfileProtoId == movementId);
				if (replacement is not null)
				{
					replacement.IsDefault = true;
					FMDB.Context.SaveChanges();
				}
			}
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You remove that propulsion mode.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionDefault(ICharacter actor, StringStack command)
	{
		if (!TryGetOwnedPropulsionProfile(actor, command, out var id))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehiclePropulsionProfileProtos
				.Include(x => x.VehicleMovementProfileProto)
				.First(x => x.Id == id);
			foreach (var item in FMDB.Context.VehiclePropulsionProfileProtos
			         .Where(x => x.VehicleMovementProfileProtoId == dbitem.VehicleMovementProfileProtoId))
			{
				item.IsDefault = item.Id == id;
			}
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You select that propulsion mode as the default for new vehicles.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionTime(ICharacter actor, StringStack command)
	{
		if (!TryGetOwnedPropulsionProfile(actor, command, out var id))
		{
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var seconds) || !double.IsFinite(seconds) || seconds <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive number of seconds.");
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehiclePropulsionProfileProtos.Find(id)!.BaseMoveTimeMilliseconds = seconds * 1000.0;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That propulsion mode now has a base traversal time of {TimeSpan.FromSeconds(seconds).Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionTrait(ICharacter actor, StringStack command)
	{
		if (!TryGetOwnedPropulsionProfile(actor, command, out var id, out var profile))
		{
			return false;
		}

		if (profile.PropulsionType is not (VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed))
		{
			actor.OutputHandler.Send("Only self-powered and rowed propulsion modes use a propulsion trait.");
			return false;
		}

		var trait = actor.Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehiclePropulsionProfileProtos.Find(id)!.PropulsionTraitDefinitionId = trait.Id;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That propulsion mode now uses the {trait.Name.ColourName()} trait.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionDifficulty(ICharacter actor, StringStack command)
	{
		if (!TryGetOwnedPropulsionProfile(actor, command, out var id, out var profile))
		{
			return false;
		}

		if (profile.PropulsionType is not (VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed))
		{
			actor.OutputHandler.Send("Only self-powered and rowed propulsion modes make propulsion checks.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum(out Difficulty difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehiclePropulsionProfileProtos.Find(id)!.CheckDifficulty = (int)difficulty;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That propulsion mode now makes a {difficulty.DescribeEnum().ColourName()} check.");
		return true;
	}

	private bool BuildingCommandMovementPropulsionExpression(ICharacter actor, StringStack command, bool speed)
	{
		if (!TryGetOwnedPropulsionProfile(actor, command, out var id, out var profile))
		{
			return false;
		}

		if (profile.PropulsionType == VehiclePropulsionType.None ||
		    !speed && profile.PropulsionType is not (VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed))
		{
			actor.OutputHandler.Send(speed
				? "The none propulsion mode does not use a speed expression."
				: "Only self-powered and rowed propulsion modes use a stamina-cost expression.");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (!ValidatePropulsionExpression(profile.PropulsionType, text, speed, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehiclePropulsionProfileProtos.Find(id)!;
			if (speed)
			{
				dbitem.SpeedMultiplierExpression = text;
			}
			else
			{
				dbitem.StaminaCostExpression = text;
			}
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"You update that propulsion mode's {(speed ? "speed multiplier" : "stamina cost")} expression.");
		return true;
	}

	private bool TryGetOwnedPropulsionProfile(ICharacter actor, StringStack command, out long id)
	{
		return TryGetOwnedPropulsionProfile(actor, command, out id, out _);
	}

	private bool TryGetOwnedPropulsionProfile(ICharacter actor, StringStack command, out long id,
		out IVehiclePropulsionProfilePrototype profile)
	{
		if (!long.TryParse(command.PopSpeech(), out id))
		{
			actor.OutputHandler.Send("Which propulsion profile ID do you want to configure?");
			profile = null!;
			return false;
		}

		var propulsionProfileId = id;
		profile = _movementProfiles
			.SelectMany(x => x.PropulsionProfiles)
			.FirstOrDefault(x => x.Id == propulsionProfileId)!;
		if (profile is not null)
		{
			return true;
		}

		actor.OutputHandler.Send("There is no such propulsion profile.");
		return false;
	}

	internal static VehiclePropulsionType? ParseVehiclePropulsionType(string text)
	{
		return text?.ToLowerInvariant() switch
		{
			"self" or "selfpowered" or "self-powered" or "paddle" or "paddled" => VehiclePropulsionType.SelfPowered,
			"row" or "rowed" or "rowing" => VehiclePropulsionType.Rowed,
			"sail" or "sailed" or "sailing" => VehiclePropulsionType.Sail,
			"outboard" or "motor" or "outboardmotor" or "outboard-motor" => VehiclePropulsionType.OutboardMotor,
			"none" => VehiclePropulsionType.None,
			_ => null
		};
	}

	private static string DefaultPropulsionSpeedExpression(VehiclePropulsionType type)
	{
		return type switch
		{
			VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed => "max(0.25, 1.0 + (0.15 * outcome))",
			VehiclePropulsionType.Sail => "1.0 + (0.15 * (wind - 1.0))",
			VehiclePropulsionType.OutboardMotor => "output",
			_ => "0"
		};
	}

	private static string DefaultPropulsionStaminaExpression(VehiclePropulsionType type)
	{
		return type is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed
			? "swimcost * max(0.5, 1.0 - (0.10 * outcome))"
			: "0";
	}

	internal static bool ValidatePropulsionExpression(VehiclePropulsionType type, string text, bool speed,
		out string reason)
	{
		var expression = new Expression(text);
		if (expression.HasErrors())
		{
			reason = $"That expression is invalid: {expression.Error}";
			return false;
		}

		var permittedParameters = (type, speed) switch
		{
			(VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed, true) => ["outcome"],
			(VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed, false) => ["outcome", "swimcost"],
			(VehiclePropulsionType.Sail, true) => ["wind"],
			(VehiclePropulsionType.OutboardMotor, true) => ["output"],
			_ => Array.Empty<string>()
		};
		var unsupportedParameters = expression.ParameterNames
			.Where(x => !permittedParameters.Contains(x, StringComparer.OrdinalIgnoreCase))
			.ToList();
		if (unsupportedParameters.Any())
		{
			reason = $"That expression uses unsupported parameter{(unsupportedParameters.Count == 1 ? "" : "s")}: {unsupportedParameters.ListToString()}.";
			return false;
		}

		var samples = type switch
		{
			VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed => Enumerable.Range(-3, 7)
				.Select(x => (Outcome: (double)x, Wind: 1.0, Output: 1.0, SwimCost: 1.0)),
			VehiclePropulsionType.Sail => Enumerable.Range(1, 7)
				.Select(x => (Outcome: 0.0, Wind: (double)x, Output: 1.0, SwimCost: 1.0)),
			VehiclePropulsionType.OutboardMotor => new[] { 0.01, 1.0, 100.0 }
				.Select(x => (Outcome: 0.0, Wind: 1.0, Output: x, SwimCost: 1.0)),
			_ => [(Outcome: 0.0, Wind: 1.0, Output: 1.0, SwimCost: 1.0)]
		};

		foreach (var sample in samples)
		{
			var value = expression.EvaluateDoubleWith(
				("outcome", sample.Outcome),
				("wind", sample.Wind),
				("output", sample.Output),
				("swimcost", sample.SwimCost));
			if (!double.IsFinite(value) || speed && value <= 0.0 || !speed && value < 0.0)
			{
				reason = speed
					? "The speed expression must return a finite positive value for every supported input."
					: "The stamina expression must return a finite non-negative value for every supported input.";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	private bool BuildingCommandMovementFuel(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which movement profile ID do you want to configure?");
			return false;
		}

		var liquidText = command.PopSpeech();
		long? liquidId = null;
		if (!liquidText.EqualTo("none"))
		{
			if (!long.TryParse(liquidText, out var parsedLiquidId) || actor.Gameworld.Liquids.Get(parsedLiquidId) is null)
			{
				actor.OutputHandler.Send("You must specify a valid liquid ID, or #3none#0.".SubstituteANSIColour());
				return false;
			}

			liquidId = parsedLiquidId;
		}

		if (!double.TryParse(command.PopSpeech(), out var volume) || volume < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative fuel volume per move.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			dbitem.FuelLiquidId = liquidId;
			dbitem.FuelVolumePerMove = liquidId is null ? 0.0 : volume;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You update that movement profile's fuel requirements.");
		return true;
	}

	private bool BuildingCommandMovementPower(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id) ||
		    !double.TryParse(command.PopSpeech(), out var watts) ||
		    watts < 0.0)
		{
			actor.OutputHandler.Send("You must specify a movement profile ID and a non-negative watt requirement.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			dbitem.RequiredPowerSpikeInWatts = watts;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You update that movement profile's power requirements.");
		return true;
	}

	private bool BuildingCommandMovementRole(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which movement profile ID do you want to configure?");
			return false;
		}

		var role = command.SafeRemainingArgument.EqualTo("none") ? string.Empty : command.SafeRemainingArgument.ToLowerInvariant();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			dbitem.RequiredInstalledRole = role;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You update that movement profile's required installed role.");
		return true;
	}

	private bool BuildingCommandMovementAccess(ICharacter actor, StringStack command)
	{
		return ToggleMovementFlag(actor, command, x => x.RequiresAccessPointsClosed, (x, value) => x.RequiresAccessPointsClosed = value,
			"access-point closure");
	}

	private bool BuildingCommandMovementEnvironment(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which movement profile ID do you want to configure?");
			return false;
		}

		var environment = ParseMovementEnvironment(command.PopSpeech());
		if (environment is null)
		{
			actor.OutputHandler.Send("You must specify either unrestricted or surfacewater.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			if (environment == VehicleMovementEnvironment.SurfaceWater &&
			    (VehicleMovementProfileType)dbitem.MovementType != VehicleMovementProfileType.CellExit)
			{
				actor.OutputHandler.Send("Only a cell-exit movement profile can use the surface-water environment.");
				return false;
			}

			if (environment == VehicleMovementEnvironment.Unrestricted &&
			    FMDB.Context.VehiclePropulsionProfileProtos.Any(x => x.VehicleMovementProfileProtoId == dbitem.Id))
			{
				actor.OutputHandler.Send("Remove this movement profile's propulsion modes before returning it to unrestricted movement.");
				return false;
			}

			dbitem.MovementEnvironment = (int)environment.Value;
			if (environment == VehicleMovementEnvironment.Unrestricted)
			{
				dbitem.ExposesOccupantsToWater = false;
			}

			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That movement profile now uses the {environment.Value.DescribeEnum(true).ColourName()} environment.");
		return true;
	}

	private bool BuildingCommandMovementWaterExposure(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which movement profile ID do you want to configure?");
			return false;
		}

		var exposed = ParseMovementWaterExposure(command.PopSpeech());
		if (exposed is null)
		{
			actor.OutputHandler.Send("You must specify either protected or exposed.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			if ((VehicleMovementEnvironment)dbitem.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater)
			{
				actor.OutputHandler.Send("Only a surface-water movement profile can configure occupant water exposure.");
				return false;
			}

			dbitem.ExposesOccupantsToWater = exposed.Value;
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That movement profile now leaves its occupants {(exposed.Value ? "exposed to" : "protected from")} surface water.");
		return true;
	}

	internal static VehicleMovementEnvironment? ParseMovementEnvironment(string text)
	{
		return text?.ToLowerInvariant() switch
		{
			"unrestricted" => VehicleMovementEnvironment.Unrestricted,
			"surfacewater" => VehicleMovementEnvironment.SurfaceWater,
			"surface-water" => VehicleMovementEnvironment.SurfaceWater,
			"water" => VehicleMovementEnvironment.SurfaceWater,
			_ => null
		};
	}

	internal static bool? ParseMovementWaterExposure(string text)
	{
		return text?.ToLowerInvariant() switch
		{
			"protected" => false,
			"exposed" => true,
			_ => null
		};
	}

	internal static string DescribeMovementProfileForShow(IVehicleMovementProfilePrototype profile, ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append($"\t#{profile.Id.ToString("N0", actor)} {profile.Name.ColourName()} [{profile.MovementType.DescribeEnum().ColourValue()}] environment {profile.MovementEnvironment.DescribeEnum(true).ColourName()}{(profile.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater ? profile.ExposesOccupantsToWater ? " occupants exposed".Colour(Telnet.Yellow) : " occupants protected".Colour(Telnet.Green) : "")}{(profile.IsDefault ? " default".Colour(Telnet.Green) : "")}{(string.IsNullOrWhiteSpace(profile.RequiredInstalledRole) ? "" : $" role {profile.RequiredInstalledRole.ColourCommand()}")}{(profile.FuelLiquidId is null ? "" : $" fuel {profile.FuelVolumePerMove.ToString("N2", actor).ColourValue()}")}{(profile.RequiredPowerSpikeInWatts > 0.0 ? $" power {profile.RequiredPowerSpikeInWatts.ToString("N2", actor).ColourValue()}W" : "")}");
		if (profile.MovementType == VehicleMovementProfileType.Route)
		{
			sb.Append($" speed {profile.RouteSpeedMetresPerSecond.ToString("N3", actor).ColourValue()}m/s propulsion {profile.RoutePropulsionMode.DescribeEnum().ColourName()}{(profile.FuelLiquidId is null ? "" : $" route-fuel {profile.RouteFuelVolumePerMetre.ToString("N8", actor).ColourValue()}/m")}{(profile.RoutePowerDrawWatts > 0.0 ? $" route-power {profile.RoutePowerDrawWatts.ToString("N2", actor).ColourValue()}W" : "")}{(profile.AutomaticOperationCapable ? " automatic-capable".Colour(Telnet.Green) : "")}");
			return sb.ToString();
		}
		if (profile.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater)
		{
			return sb.ToString();
		}

		var propulsionProfiles = profile.PropulsionProfiles?.ToList() ?? [];
		if (!propulsionProfiles.Any())
		{
			sb.Append($"\n\t\t{("Warning: legacy surface-water profile has no explicit propulsion mode.".Colour(Telnet.Yellow))}");
			return sb.ToString();
		}

		foreach (var propulsion in propulsionProfiles)
		{
			sb.Append($"\n\t\t#{propulsion.Id.ToString("N0", actor)} {propulsion.PropulsionType.DescribeEnum().ColourName()}{(propulsion.IsDefault ? " default".Colour(Telnet.Green) : "")} time {TimeSpan.FromMilliseconds(propulsion.BaseMoveTimeMilliseconds).Describe(actor).ColourValue()}");
			if (propulsion.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed)
			{
				sb.Append($" trait {(propulsion.PropulsionTrait?.Name.ColourName() ?? "none".ColourError())} difficulty {propulsion.CheckDifficulty.DescribeEnum().ColourName()}");
			}

			if (propulsion.PropulsionType != VehiclePropulsionType.None)
			{
				sb.Append($" speed {propulsion.SpeedMultiplierExpression.ColourCommand()}");
			}

			if (propulsion.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed)
			{
				sb.Append($" stamina {propulsion.StaminaCostExpression.ColourCommand()}");
			}
		}

		return sb.ToString();
	}

	private bool BuildingCommandMovementTow(ICharacter actor, StringStack command)
	{
		return ToggleMovementFlag(actor, command, x => x.RequiresTowLinksClosed, (x, value) => x.RequiresTowLinksClosed = value,
			"tow-link validation");
	}

	private bool ToggleMovementFlag(ICharacter actor, StringStack command, Func<DB.VehicleMovementProfileProto, bool> getter,
		Action<DB.VehicleMovementProfileProto, bool> setter, string description)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which movement profile ID do you want to configure?");
			return false;
		}

		bool newValue;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleMovementProfileProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such movement profile.");
				return false;
			}

			newValue = !getter(dbitem);
			setter(dbitem, newValue);
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That movement profile now {(newValue ? "requires" : "does not require")} {description}.");
		return true;
	}

	private bool BuildingCommandAccess(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandAccessAdd(actor, command);
			case "open":
				return ToggleAccessFlag(actor, command, x => x.StartsOpen, (x, value) => x.StartsOpen = value,
					"starts open");
			case "closedmove":
			case "mustclose":
				return ToggleAccessFlag(actor, command, x => x.MustBeClosedForMovement,
					(x, value) => x.MustBeClosedForMovement = value, "must be closed for movement");
			case "remove":
			case "delete":
				return BuildingCommandAccessRemove(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandAccessAdd(ICharacter actor, StringStack command)
	{
		var compartmentText = command.PopSpeech();
		long? compartmentId = null;
		if (!compartmentText.EqualTo("none"))
		{
			if (!long.TryParse(compartmentText, out var parsedCompartmentId) ||
			    _compartments.All(x => x.Id != parsedCompartmentId))
			{
				actor.OutputHandler.Send("You must specify a valid compartment ID, or #3none#0.".SubstituteANSIColour());
				return false;
			}

			compartmentId = parsedCompartmentId;
		}

		if (!command.PopSpeech().TryParseEnum(out VehicleAccessPointType type))
		{
			actor.OutputHandler.Send($"You must specify one of {Enum.GetValues<VehicleAccessPointType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		var proto = actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such projection item prototype.");
			return false;
		}

		if (proto is not GameItemProto concreteProto)
		{
			actor.OutputHandler.Send("That item prototype cannot be updated with the required vehicle access component.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this access point be called?");
			return false;
		}

		DB.VehicleAccessPointProto dbitem;
		using (new FMDB())
		{
			dbitem = new DB.VehicleAccessPointProto
			{
				VehicleProtoId = Id,
				VehicleProtoRevision = RevisionNumber,
				VehicleCompartmentProtoId = compartmentId,
				Name = command.SafeRemainingArgument.TitleCase(),
				Description = "An undescribed vehicle access point.",
				AccessPointType = (int)type,
				ProjectionItemProtoId = proto.Id,
				ProjectionItemProtoRevision = proto.RevisionNumber,
				StartsOpen = false,
				MustBeClosedForMovement = true,
				DisplayOrder = _accessPoints.Count + 1
			};
			FMDB.Context.VehicleAccessPointProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var access = new VehicleAccessPointPrototype(dbitem, _compartments, actor.Gameworld);
		_accessPoints.Add(access);
		var component = proto.Components.OfType<VehicleAccessPointGameItemComponentProto>()
		                     .FirstOrDefault(x => x.VehiclePrototypeId == Id);
		if (component is null)
		{
			component = new VehicleAccessPointGameItemComponentProto(actor.Gameworld, actor.Account);
			actor.Gameworld.Add(component);
			component.ConfigureForAccessPoint(this, access);
			component.ChangeStatus(RevisionStatus.Current, "Automatically generated for vehicle access projection.", actor.Account);
			concreteProto.AddComponent(component);
		}

		actor.OutputHandler.Send($"You add the {access.Name.ColourName()} access point.");
		return true;
	}

	private bool ToggleAccessFlag(ICharacter actor, StringStack command, Func<DB.VehicleAccessPointProto, bool> getter,
		Action<DB.VehicleAccessPointProto, bool> setter, string description)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which access point ID do you want to configure?");
			return false;
		}

		bool value;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPointProtos.Find(id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such access point.");
				return false;
			}

			value = !getter(dbitem);
			setter(dbitem, value);
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"That access point now {(value ? "" : "does not ")}{description}.");
		return true;
	}

	private bool BuildingCommandAccessRemove(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which access point ID do you want to remove?");
			return false;
		}

		if (_cargoSpaces.Any(x => x.RequiredAccessPoint?.Id == id) ||
		    _installationPoints.Any(x => x.RequiredAccessPoint?.Id == id) ||
		    _towPoints.Any(x => x.RequiredAccessPoint?.Id == id))
		{
			actor.OutputHandler.Send("You must remove cargo, install, and tow definitions that depend on that access point first.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPointProtos.Find(id);
			if (dbitem is not null && dbitem.VehicleProtoId == Id && dbitem.VehicleProtoRevision == RevisionNumber)
			{
				FMDB.Context.VehicleAccessPointProtos.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_accessPoints.RemoveAll(x => x.Id == id);
		actor.OutputHandler.Send("You remove that access point.");
		return true;
	}

	private bool BuildingCommandCargo(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandCargoAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandCargoRemove(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCargoAdd(ICharacter actor, StringStack command)
	{
		if (!TryParseOptionalCompartment(actor, command.PopSpeech(), out var compartmentId) ||
		    !TryParseOptionalAccess(actor, command.PopSpeech(), out var accessId))
		{
			return false;
		}

		var proto = actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such projection item prototype.");
			return false;
		}

		if (!proto.Components.OfType<IContainerPrototype>().Any())
		{
			actor.OutputHandler.Send("Cargo projection item prototypes must include a normal container component.");
			return false;
		}

		if (proto is not GameItemProto concreteProto)
		{
			actor.OutputHandler.Send("That item prototype cannot be updated with the required vehicle cargo component.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this cargo space be called?");
			return false;
		}

		DB.VehicleCargoSpaceProto dbitem;
		using (new FMDB())
		{
			dbitem = new DB.VehicleCargoSpaceProto
			{
				VehicleProtoId = Id,
				VehicleProtoRevision = RevisionNumber,
				VehicleCompartmentProtoId = compartmentId,
				RequiredAccessPointProtoId = accessId,
				Name = command.SafeRemainingArgument.TitleCase(),
				Description = "An undescribed cargo space.",
				ProjectionItemProtoId = proto.Id,
				ProjectionItemProtoRevision = proto.RevisionNumber,
				DisplayOrder = _cargoSpaces.Count + 1
			};
			FMDB.Context.VehicleCargoSpaceProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var cargo = new VehicleCargoSpacePrototype(dbitem, _compartments, _accessPoints, actor.Gameworld);
		_cargoSpaces.Add(cargo);
		var component = proto.Components.OfType<VehicleCargoSpaceGameItemComponentProto>()
		                     .FirstOrDefault(x => x.VehiclePrototypeId == Id);
		if (component is null)
		{
			component = new VehicleCargoSpaceGameItemComponentProto(actor.Gameworld, actor.Account);
			actor.Gameworld.Add(component);
			component.ConfigureForCargoSpace(this, cargo);
			component.ChangeStatus(RevisionStatus.Current, "Automatically generated for vehicle cargo projection.", actor.Account);
			concreteProto.AddComponent(component);
		}

		actor.OutputHandler.Send($"You add the {cargo.Name.ColourName()} cargo space.");
		return true;
	}

	private bool BuildingCommandCargoRemove(ICharacter actor, StringStack command)
	{
				return RemoveChildDefinition(actor, command, ctx => ctx.VehicleCargoSpaceProtos, _cargoSpaces, "cargo space");
	}

	private bool BuildingCommandInstallPoint(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandInstallPointAdd(actor, command);
			case "remove":
			case "delete":
				return RemoveChildDefinition(actor, command, ctx => ctx.VehicleInstallationPointProtos, _installationPoints,
					"installation point");
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandInstallPointAdd(ICharacter actor, StringStack command)
	{
		if (!TryParseOptionalAccess(actor, command.PopSpeech(), out var accessId))
		{
			return false;
		}

		var mount = command.PopSpeech().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(mount))
		{
			actor.OutputHandler.Send("What mount type should this installation point accept?");
			return false;
		}

		var role = command.PopSpeech();
		role = role.EqualTo("none") ? string.Empty : role.ToLowerInvariant();
		if (!bool.TryParse(command.PopSpeech(), out var required))
		{
			actor.OutputHandler.Send("You must specify whether this installation point is required for movement.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this installation point be called?");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = new DB.VehicleInstallationPointProto
			{
				VehicleProtoId = Id,
				VehicleProtoRevision = RevisionNumber,
				RequiredAccessPointProtoId = accessId,
				Name = command.SafeRemainingArgument.TitleCase(),
				Description = "An undescribed vehicle installation point.",
				MountType = mount,
				RequiredRole = role,
				RequiredForMovement = required,
				DisplayOrder = _installationPoints.Count + 1
			};
			FMDB.Context.VehicleInstallationPointProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
			_installationPoints.Add(new VehicleInstallationPointPrototype(dbitem, _accessPoints));
		}

		actor.OutputHandler.Send("You add a vehicle installation point.");
		return true;
	}

	private bool BuildingCommandTowPoint(ICharacter actor, StringStack command)
	{
		var commandText = command.PopSpeech();
		switch (commandText.ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandTowPointAdd(actor, command);
			case "remove":
			case "delete":
				return RemoveChildDefinition(actor, command, ctx => ctx.VehicleTowPointProtos, _towPoints, "tow point");
			default:
				if (long.TryParse(commandText, out var id))
				{
					var towPoint = _towPoints.FirstOrDefault(x => x.Id == id);
					if (towPoint is null)
					{
						actor.OutputHandler.Send("There is no such tow point.");
						return false;
					}

					return BuildingCommandTowPointChild(actor, command, towPoint);
				}

				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandTowPointChild(ICharacter actor, StringStack command,
		IVehicleTowPointPrototype towPoint)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "stress":
				return BuildingCommandTowPointStress(actor, command, towPoint);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandTowPointStress(ICharacter actor, StringStack command,
		IVehicleTowPointPrototype towPoint)
	{
		var setting = command.PopSpeech().ToLowerInvariant();
		if (setting.EqualTo("reset"))
		{
			using (new FMDB())
			{
				var dbitem = FMDB.Context.VehicleTowPointProtos.Find(towPoint.Id);
				if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
				{
					actor.OutputHandler.Send("There is no such tow point.");
					return false;
				}

				dbitem.TowStressWarningRatio = null;
				dbitem.TowStressFailureStartRatio = null;
				dbitem.TowStressMaximumFailureChance = null;
				dbitem.TowStressDamageMultiplier = null;
				FMDB.Context.SaveChanges();
			}

			ReloadChildDefinitions();
			actor.OutputHandler.Send($"You clear tow-stress overrides for {towPoint.Name.ColourName()}.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What value do you want to set for that tow-stress setting?");
			return false;
		}

		double value;
		var text = command.SafeRemainingArgument;
		if (setting.EqualToAny("warning", "warn", "failstart", "failure", "maxchance", "chance"))
		{
			if (!text.TryParsePercentage(actor.Account.Culture, out value) || value < 0.0 || value > 1.0)
			{
				actor.OutputHandler.Send($"Enter a percentage between {0.ToStringP2Colour(actor)} and {1.ToStringP2Colour(actor)}.");
				return false;
			}
		}
		else if (setting.EqualTo("damage"))
		{
			if (!text.TryParsePercentage(actor.Account.Culture, out value) || value < 0.0)
			{
				actor.OutputHandler.Send("Enter a non-negative damage multiplier, e.g. #32%#0 or #30.02#0.".SubstituteANSIColour());
				return false;
			}
		}
		else
		{
			actor.OutputHandler.Send("Choose warning, failstart, maxchance, damage, or reset.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleTowPointProtos.Find(towPoint.Id);
			if (dbitem is null || dbitem.VehicleProtoId != Id || dbitem.VehicleProtoRevision != RevisionNumber)
			{
				actor.OutputHandler.Send("There is no such tow point.");
				return false;
			}

			switch (setting)
			{
				case "warning":
				case "warn":
					dbitem.TowStressWarningRatio = value;
					break;
				case "failstart":
				case "failure":
					dbitem.TowStressFailureStartRatio = value;
					break;
				case "maxchance":
				case "chance":
					dbitem.TowStressMaximumFailureChance = value;
					break;
				case "damage":
					dbitem.TowStressDamageMultiplier = value;
					break;
				default:
					actor.OutputHandler.Send("Choose warning, failstart, maxchance, damage, or reset.");
					return false;
			}

			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send($"You update tow-stress tuning for {towPoint.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandTowPointAdd(ICharacter actor, StringStack command)
	{
		if (!TryParseOptionalAccess(actor, command.PopSpeech(), out var accessId))
		{
			return false;
		}

		var towType = command.PopSpeech().ToLowerInvariant();
		var direction = command.PopSpeech().ToLowerInvariant();
		var canTow = direction is "tow" or "both";
		var canBeTowed = direction is "towed" or "both";
		if (!canTow && !canBeTowed)
		{
			actor.OutputHandler.Send("The tow direction must be #3tow#0, #3towed#0, or #3both#0.".SubstituteANSIColour());
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var maxWeight) || maxWeight < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative maximum towed weight.");
			return false;
		}

		var pullMultiplier = 1.0;
		if (command.PeekSpeech().EqualTo("pull"))
		{
			command.PopSpeech();
			if (!double.TryParse(command.PopSpeech(), out pullMultiplier) || pullMultiplier <= 0.0)
			{
				actor.OutputHandler.Send("You must specify a positive pull multiplier.");
				return false;
			}
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this tow point be called?");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = new DB.VehicleTowPointProto
			{
				VehicleProtoId = Id,
				VehicleProtoRevision = RevisionNumber,
				RequiredAccessPointProtoId = accessId,
				Name = command.SafeRemainingArgument.TitleCase(),
				Description = "An undescribed tow point.",
				TowType = towType,
				CanTow = canTow,
				CanBeTowed = canBeTowed,
				MaximumTowedWeight = maxWeight,
				CharacterPullMultiplier = pullMultiplier,
				DisplayOrder = _towPoints.Count + 1
			};
			FMDB.Context.VehicleTowPointProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
			_towPoints.Add(new VehicleTowPointPrototype(dbitem, _accessPoints));
		}

		actor.OutputHandler.Send("You add a vehicle tow point.");
		return true;
	}

	private bool BuildingCommandDamageZone(ICharacter actor, StringStack command)
	{
		var commandText = command.PopSpeech();
		switch (commandText.ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandDamageZoneAdd(actor, command);
			case "remove":
			case "delete":
				return RemoveChildDefinition(actor, command, ctx => ctx.VehicleDamageZoneProtos, _damageZones, "damage zone");
			default:
				if (long.TryParse(commandText, out var id))
				{
					var zone = _damageZones.FirstOrDefault(x => x.Id == id);
					if (zone is null)
					{
						actor.OutputHandler.Send("There is no such damage zone.");
						return false;
					}

					return BuildingCommandDamageZoneChild(actor, command, zone);
				}

				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDamageZoneChild(ICharacter actor, StringStack command,
		IVehicleDamageZonePrototype zone)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "effect":
			case "effects":
				return BuildingCommandDamageZoneEffect(actor, command, zone);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDamageZoneEffect(ICharacter actor, StringStack command,
		IVehicleDamageZonePrototype zone)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandDamageZoneEffectAdd(actor, command, zone);
			case "remove":
			case "delete":
				return BuildingCommandDamageZoneEffectRemove(actor, command, zone);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDamageZoneEffectAdd(ICharacter actor, StringStack command,
		IVehicleDamageZonePrototype zone)
	{
		if (!TryParseDamageEffectTargetType(command.PopSpeech(), out var targetType))
		{
			actor.OutputHandler.Send($"You must specify one of {Enum.GetValues<VehicleDamageEffectTargetType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		var targetText = command.PopSpeech();
		if (!TryParseDamageEffectTarget(actor, targetType, targetText, out var targetId))
		{
			return false;
		}

		var minimumStatus = VehicleSystemStatus.Disabled;
		if (!command.IsFinished &&
		    !command.PopSpeech().TryParseEnum(out minimumStatus))
		{
			actor.OutputHandler.Send("The minimum status must be #3disabled#0 or #3destroyed#0.".SubstituteANSIColour());
			return false;
		}

		if (!ValidateDamageZoneEffect(targetType, targetId, minimumStatus, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehicleDamageZoneEffectProtos.Add(new DB.VehicleDamageZoneEffectProto
			{
				VehicleDamageZoneProtoId = zone.Id,
				TargetType = (int)targetType,
				TargetProtoId = targetId,
				MinimumStatus = (int)minimumStatus
			});
			FMDB.Context.SaveChanges();
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You add a damage-zone effect.");
		return true;
	}

	private bool BuildingCommandDamageZoneEffectRemove(ICharacter actor, StringStack command,
		IVehicleDamageZonePrototype zone)
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which damage-zone effect do you want to remove?");
			return false;
		}

		if (zone.Effects.All(x => x.Id != id))
		{
			actor.OutputHandler.Send("That damage-zone effect does not belong to this damage zone.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleDamageZoneEffectProtos.Find(id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleDamageZoneEffectProtos.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		ReloadChildDefinitions();
		actor.OutputHandler.Send("You remove the damage-zone effect.");
		return true;
	}

	private bool TryParseDamageEffectTargetType(string text, out VehicleDamageEffectTargetType targetType)
	{
		switch (text.ToLowerInvariant())
		{
			case "whole":
			case "wholemovement":
			case "wholevehiclemovement":
			case "vehicle":
			case "movementall":
				targetType = VehicleDamageEffectTargetType.WholeVehicleMovement;
				return true;
			case "movement":
			case "profile":
			case "movementprofile":
				targetType = VehicleDamageEffectTargetType.MovementProfile;
				return true;
			case "access":
			case "accesspoint":
				targetType = VehicleDamageEffectTargetType.AccessPoint;
				return true;
			case "cargo":
			case "cargospace":
				targetType = VehicleDamageEffectTargetType.CargoSpace;
				return true;
			case "install":
			case "installation":
			case "installpoint":
			case "installationpoint":
				targetType = VehicleDamageEffectTargetType.InstallationPoint;
				return true;
			case "tow":
			case "towpoint":
				targetType = VehicleDamageEffectTargetType.TowPoint;
				return true;
			default:
				return text.TryParseEnum(out targetType);
		}
	}

	private bool TryParseDamageEffectTarget(ICharacter actor, VehicleDamageEffectTargetType targetType, string text,
		out long? targetId)
	{
		targetId = null;
		if (text.EqualTo("all") || targetType == VehicleDamageEffectTargetType.WholeVehicleMovement && text.EqualTo("none"))
		{
			return true;
		}

		if (!long.TryParse(text, out var id))
		{
			actor.OutputHandler.Send("You must specify a target id or #3all#0.".SubstituteANSIColour());
			return false;
		}

		targetId = id;
		if (ValidateDamageZoneEffect(targetType, targetId, VehicleSystemStatus.Disabled, out _))
		{
			return true;
		}

		actor.OutputHandler.Send("That is not a valid target id for that effect type.");
		return false;
	}

	private bool BuildingCommandDamageZoneAdd(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.PopSpeech(), out var maxDamage) ||
		    !double.TryParse(command.PopSpeech(), out var hitWeight) ||
		    !double.TryParse(command.PopSpeech(), out var disabledThreshold) ||
		    !double.TryParse(command.PopSpeech(), out var destroyedThreshold) ||
		    !bool.TryParse(command.PopSpeech(), out var disablesMovement))
		{
			actor.OutputHandler.Send("You must specify max damage, hit weight, disabled threshold, destroyed threshold, and whether it disables movement.");
			return false;
		}

		if (maxDamage <= 0.0 || hitWeight <= 0.0 || disabledThreshold <= 0.0 || destroyedThreshold < disabledThreshold)
		{
			actor.OutputHandler.Send("Damage zones require positive values, and the destroyed threshold must be at least the disabled threshold.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this damage zone be called?");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = new DB.VehicleDamageZoneProto
			{
				VehicleProtoId = Id,
				VehicleProtoRevision = RevisionNumber,
				Name = command.SafeRemainingArgument.TitleCase(),
				Description = "An undescribed damage zone.",
				MaximumDamage = maxDamage,
				HitWeight = hitWeight,
				DisabledThreshold = disabledThreshold,
				DestroyedThreshold = destroyedThreshold,
				DisablesMovement = disablesMovement,
				DisplayOrder = _damageZones.Count + 1
			};
			FMDB.Context.VehicleDamageZoneProtos.Add(dbitem);
			FMDB.Context.SaveChanges();
			_damageZones.Add(new VehicleDamageZonePrototype(dbitem));
		}

		actor.OutputHandler.Send("You add a vehicle damage zone.");
		return true;
	}

	private bool TryParseOptionalCompartment(ICharacter actor, string text, out long? id)
	{
		id = null;
		if (text.EqualTo("none"))
		{
			return true;
		}

		if (!long.TryParse(text, out var parsed) || _compartments.All(x => x.Id != parsed))
		{
			actor.OutputHandler.Send("You must specify a valid compartment ID, or #3none#0.".SubstituteANSIColour());
			return false;
		}

		id = parsed;
		return true;
	}

	private bool TryParseOptionalAccess(ICharacter actor, string text, out long? id)
	{
		id = null;
		if (text.EqualTo("none"))
		{
			return true;
		}

		if (!long.TryParse(text, out var parsed) || _accessPoints.All(x => x.Id != parsed))
		{
			actor.OutputHandler.Send("You must specify a valid access point ID, or #3none#0.".SubstituteANSIColour());
			return false;
		}

		id = parsed;
		return true;
	}

	private bool RemoveChildDefinition<TDb, TRuntime>(ICharacter actor, StringStack command,
		Func<FuturemudDatabaseContext, DbSet<TDb>> setFactory, List<TRuntime> list, string name)
		where TDb : class
		where TRuntime : IFrameworkItem
	{
		if (!long.TryParse(command.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send($"Which {name} ID do you want to remove?");
			return false;
		}

		var runtime = list.FirstOrDefault(x => x.Id == id);
		if (runtime is null)
		{
			actor.OutputHandler.Send($"There is no such {name}.");
			return false;
		}

		using (new FMDB())
		{
			var set = setFactory(FMDB.Context);
			var dbitem = set.Find(id);
			if (dbitem is not null)
			{
				set.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		list.Remove(runtime);
		actor.OutputHandler.Send($"You remove that {name}.");
		return true;
	}

	private void ReloadChildDefinitions()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleProtos
			                 .Include(x => x.Compartments)
			                 .Include(x => x.CompartmentLinks)
			                 .Include(x => x.OccupantSlots)
			                 .Include(x => x.ControlStations)
				                 .Include(x => x.MovementProfiles).ThenInclude(x => x.PropulsionProfiles)
			                 .Include(x => x.AccessPoints)
			                 .Include(x => x.CargoSpaces)
			                 .Include(x => x.InstallationPoints)
			                 .Include(x => x.TowPoints)
			                 .Include(x => x.DamageZones).ThenInclude(x => x.Effects)
			                 .First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
			_compartments.Clear();
			_compartmentLinks.Clear();
			_occupantSlots.Clear();
			_controlStations.Clear();
			_movementProfiles.Clear();
			_accessPoints.Clear();
			_cargoSpaces.Clear();
			_installationPoints.Clear();
			_towPoints.Clear();
			_damageZones.Clear();
			_compartments.AddRange(dbitem.Compartments.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleCompartmentPrototype(x, Gameworld)));
			_compartmentLinks.AddRange(dbitem.CompartmentLinks.OrderBy(x => x.Id).Select(x => new VehicleCompartmentLinkPrototype(x, _compartments)));
			_occupantSlots.AddRange(dbitem.OccupantSlots.OrderBy(x => x.Id).Select(x => new VehicleOccupantSlotPrototype(x, _compartments, Gameworld)));
			_controlStations.AddRange(dbitem.ControlStations.OrderBy(x => x.Id).Select(x => new VehicleControlStationPrototype(x, _occupantSlots)));
			_movementProfiles.AddRange(dbitem.MovementProfiles.OrderBy(x => x.Id).Select(x => new VehicleMovementProfilePrototype(x, Gameworld)));
			_accessPoints.AddRange(dbitem.AccessPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleAccessPointPrototype(x, _compartments, Gameworld)));
			_cargoSpaces.AddRange(dbitem.CargoSpaces.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleCargoSpacePrototype(x, _compartments, _accessPoints, Gameworld)));
			_installationPoints.AddRange(dbitem.InstallationPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleInstallationPointPrototype(x, _accessPoints)));
			_towPoints.AddRange(dbitem.TowPoints.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleTowPointPrototype(x, _accessPoints)));
			_damageZones.AddRange(dbitem.DamageZones.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new VehicleDamageZonePrototype(x)));
		}
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var newRevisionNumber = FMDB.Context.VehicleProtos.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
			                                  .AsEnumerable()
			                                  .DefaultIfEmpty(0)
			                                  .Max() + 1;
			var dbnew = new DB.VehicleProto
			{
				Id = Id,
				RevisionNumber = newRevisionNumber,
				EditableItem = new DB.EditableItem
				{
					BuilderAccountId = initiator.Account.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionNumber = newRevisionNumber,
					RevisionStatus = (int)RevisionStatus.UnderDesign
				},
				Name = Name,
				Description = _description,
				VehicleScale = (int)_scale,
				ExteriorItemProtoId = _exteriorItemPrototypeId,
				ExteriorItemProtoRevision = _exteriorItemPrototypeRevision
			};
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			FMDB.Context.VehicleProtos.Add(dbnew);
			FMDB.Context.SaveChanges();

			var movementProfileMap = new Dictionary<long, DB.VehicleMovementProfileProto>();
			var accessPointMap = new Dictionary<long, DB.VehicleAccessPointProto>();
			var cargoSpaceMap = new Dictionary<long, DB.VehicleCargoSpaceProto>();
			var installationPointMap = new Dictionary<long, DB.VehicleInstallationPointProto>();
			var towPointMap = new Dictionary<long, DB.VehicleTowPointProto>();
			var damageZoneMap = new Dictionary<long, DB.VehicleDamageZoneProto>();

			foreach (var compartment in _compartments)
			{
				FMDB.Context.VehicleCompartmentProtos.Add(new DB.VehicleCompartmentProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					Name = compartment.Name,
					Description = compartment.Description,
					DisplayOrder = compartment.DisplayOrder,
					InteriorTerrainId = compartment.InteriorTerrainId,
					InteriorOutdoorsType = (int)compartment.InteriorOutdoorsType
				});
			}

			FMDB.Context.SaveChanges();
			var newCompartments = FMDB.Context.VehicleCompartmentProtos.Where(x => x.VehicleProtoId == dbnew.Id && x.VehicleProtoRevision == dbnew.RevisionNumber).ToList();
			foreach (var link in _compartmentLinks)
			{
				FMDB.Context.VehicleCompartmentLinkProtos.Add(new DB.VehicleCompartmentLinkProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					SourceVehicleCompartmentProtoId = newCompartments.First(x => x.Name == link.SourceCompartment.Name).Id,
					DestinationVehicleCompartmentProtoId = newCompartments.First(x => x.Name == link.DestinationCompartment.Name).Id,
					OutboundDirection = link.OutboundDirection,
					InboundDirection = link.InboundDirection,
					OutboundDescription = link.OutboundDescription,
					InboundDescription = link.InboundDescription
				});
			}

			FMDB.Context.SaveChanges();
			foreach (var slot in _occupantSlots)
			{
				var newCompartment = newCompartments.First(x => x.Name == slot.Compartment.Name);
				FMDB.Context.VehicleOccupantSlotProtos.Add(new DB.VehicleOccupantSlotProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					VehicleCompartmentProtoId = newCompartment.Id,
					Name = slot.Name,
					SlotType = (int)slot.SlotType,
					Capacity = slot.Capacity,
					RequiredForMovement = slot.RequiredForMovement,
					ContributesToPropulsion = slot.ContributesToPropulsion,
					SameLevelRangedCoverId = slot.SameLevelRangedCover?.Id,
					AboveRangedCoverId = slot.AboveRangedCover?.Id,
					BelowRangedCoverId = slot.BelowRangedCover?.Id,
					BoatStabilityDifficulty = (int)slot.BoatStabilityDifficulty
				});
			}

			FMDB.Context.SaveChanges();
			var newSlots = FMDB.Context.VehicleOccupantSlotProtos.Where(x => x.VehicleProtoId == dbnew.Id && x.VehicleProtoRevision == dbnew.RevisionNumber).ToList();
			foreach (var station in _controlStations)
			{
				var newSlot = newSlots.First(x => x.Name == station.OccupantSlot.Name);
				FMDB.Context.VehicleControlStationProtos.Add(new DB.VehicleControlStationProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					VehicleOccupantSlotProtoId = newSlot.Id,
					Name = station.Name,
					IsPrimary = station.IsPrimary
				});
			}

			foreach (var profile in _movementProfiles)
			{
				var dbprofile = new DB.VehicleMovementProfileProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					Name = profile.Name,
					MovementType = (int)profile.MovementType,
					MovementEnvironment = (int)profile.MovementEnvironment,
					ExposesOccupantsToWater = profile.ExposesOccupantsToWater,
					IsDefault = profile.IsDefault,
					RequiredPowerSpikeInWatts = profile.RequiredPowerSpikeInWatts,
					FuelLiquidId = profile.FuelLiquidId,
					FuelVolumePerMove = profile.FuelVolumePerMove,
					RequiredInstalledRole = profile.RequiredInstalledRole,
					RequiresTowLinksClosed = profile.RequiresTowLinksClosed,
					RequiresAccessPointsClosed = profile.RequiresAccessPointsClosed,
					RouteSpeedMetresPerSecond = profile.RouteSpeedMetresPerSecond,
					RoutePropulsionMode = (int)profile.RoutePropulsionMode,
					RouteFuelVolumePerMetre = profile.RouteFuelVolumePerMetre,
					RoutePowerDrawWatts = profile.RoutePowerDrawWatts,
					AutomaticOperationCapable = profile.AutomaticOperationCapable
				};
				FMDB.Context.VehicleMovementProfileProtos.Add(dbprofile);
				movementProfileMap[profile.Id] = dbprofile;
			}

			FMDB.Context.SaveChanges();
			foreach (var profile in _movementProfiles)
			{
				var newMovementProfile = movementProfileMap[profile.Id];
				foreach (var propulsion in profile.PropulsionProfiles)
				{
					FMDB.Context.VehiclePropulsionProfileProtos.Add(new DB.VehiclePropulsionProfileProto
					{
						VehicleMovementProfileProtoId = newMovementProfile.Id,
						PropulsionType = (int)propulsion.PropulsionType,
						IsDefault = propulsion.IsDefault,
						BaseMoveTimeMilliseconds = propulsion.BaseMoveTimeMilliseconds,
						PropulsionTraitDefinitionId = propulsion.PropulsionTrait?.Id,
						CheckDifficulty = (int)propulsion.CheckDifficulty,
						SpeedMultiplierExpression = propulsion.SpeedMultiplierExpression,
						StaminaCostExpression = propulsion.StaminaCostExpression
					});
				}
			}

			foreach (var access in _accessPoints)
			{
				var newCompartment = access.Compartment is null
					? null
					: newCompartments.FirstOrDefault(x => x.Name == access.Compartment.Name);
				var dbaccess = new DB.VehicleAccessPointProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					VehicleCompartmentProtoId = newCompartment?.Id,
					Name = access.Name,
					Description = access.Description,
					AccessPointType = (int)access.AccessPointType,
					ProjectionItemProtoId = access.ProjectionItemPrototypeId,
					ProjectionItemProtoRevision = access.ProjectionItemPrototype?.RevisionNumber,
					StartsOpen = access.StartsOpen,
					MustBeClosedForMovement = access.MustBeClosedForMovement,
					DisplayOrder = access.DisplayOrder
				};
				FMDB.Context.VehicleAccessPointProtos.Add(dbaccess);
				accessPointMap[access.Id] = dbaccess;
			}

			FMDB.Context.SaveChanges();
			foreach (var cargo in _cargoSpaces)
			{
				var newCompartment = cargo.Compartment is null
					? null
					: newCompartments.FirstOrDefault(x => x.Name == cargo.Compartment.Name);
				var newAccess = cargo.RequiredAccessPoint is null
					? null
					: accessPointMap[cargo.RequiredAccessPoint.Id];
				var dbcargo = new DB.VehicleCargoSpaceProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					VehicleCompartmentProtoId = newCompartment?.Id,
					RequiredAccessPointProtoId = newAccess?.Id,
					Name = cargo.Name,
					Description = cargo.Description,
					ProjectionItemProtoId = cargo.ProjectionItemPrototypeId,
					ProjectionItemProtoRevision = cargo.ProjectionItemPrototype?.RevisionNumber,
					DisplayOrder = cargo.DisplayOrder
				};
				FMDB.Context.VehicleCargoSpaceProtos.Add(dbcargo);
				cargoSpaceMap[cargo.Id] = dbcargo;
			}

			foreach (var install in _installationPoints)
			{
				var newAccess = install.RequiredAccessPoint is null
					? null
					: accessPointMap[install.RequiredAccessPoint.Id];
				var dbinstall = new DB.VehicleInstallationPointProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					RequiredAccessPointProtoId = newAccess?.Id,
					Name = install.Name,
					Description = install.Description,
					MountType = install.MountType,
					RequiredRole = install.RequiredRole,
					RequiredForMovement = install.RequiredForMovement,
					DisplayOrder = install.DisplayOrder
				};
				FMDB.Context.VehicleInstallationPointProtos.Add(dbinstall);
				installationPointMap[install.Id] = dbinstall;
			}

			foreach (var tow in _towPoints)
			{
				var newAccess = tow.RequiredAccessPoint is null
					? null
					: accessPointMap[tow.RequiredAccessPoint.Id];
				var dbtow = new DB.VehicleTowPointProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					RequiredAccessPointProtoId = newAccess?.Id,
					Name = tow.Name,
					Description = tow.Description,
					TowType = tow.TowType,
					CanTow = tow.CanTow,
					CanBeTowed = tow.CanBeTowed,
					MaximumTowedWeight = tow.MaximumTowedWeight,
					CharacterPullMultiplier = tow.CharacterPullMultiplier,
					TowStressWarningRatio = tow.TowStressWarningRatio,
					TowStressFailureStartRatio = tow.TowStressFailureStartRatio,
					TowStressMaximumFailureChance = tow.TowStressMaximumFailureChance,
					TowStressDamageMultiplier = tow.TowStressDamageMultiplier,
					DisplayOrder = tow.DisplayOrder
				};
				FMDB.Context.VehicleTowPointProtos.Add(dbtow);
				towPointMap[tow.Id] = dbtow;
			}

			foreach (var zone in _damageZones)
			{
				var dbzone = new DB.VehicleDamageZoneProto
				{
					VehicleProtoId = dbnew.Id,
					VehicleProtoRevision = dbnew.RevisionNumber,
					Name = zone.Name,
					Description = zone.Description,
					MaximumDamage = zone.MaximumDamage,
					HitWeight = zone.HitWeight,
					DisabledThreshold = zone.DisabledThreshold,
					DestroyedThreshold = zone.DestroyedThreshold,
					DisablesMovement = zone.DisablesMovement,
					DisplayOrder = zone.DisplayOrder
				};
				FMDB.Context.VehicleDamageZoneProtos.Add(dbzone);
				damageZoneMap[zone.Id] = dbzone;
			}

			FMDB.Context.SaveChanges();

			long? RemapEffectTarget(IVehicleDamageZoneEffectPrototype effect)
			{
				if (effect.TargetPrototypeId is null)
				{
					return null;
				}

				return effect.TargetType switch
				{
					VehicleDamageEffectTargetType.MovementProfile => movementProfileMap[effect.TargetPrototypeId.Value].Id,
					VehicleDamageEffectTargetType.AccessPoint => accessPointMap[effect.TargetPrototypeId.Value].Id,
					VehicleDamageEffectTargetType.CargoSpace => cargoSpaceMap[effect.TargetPrototypeId.Value].Id,
					VehicleDamageEffectTargetType.InstallationPoint => installationPointMap[effect.TargetPrototypeId.Value].Id,
					VehicleDamageEffectTargetType.TowPoint => towPointMap[effect.TargetPrototypeId.Value].Id,
					_ => null
				};
			}

			foreach (var zone in _damageZones.Where(x => x.Effects.Any()))
			{
				var newZone = damageZoneMap[zone.Id];
				foreach (var effect in zone.Effects)
				{
					FMDB.Context.VehicleDamageZoneEffectProtos.Add(new DB.VehicleDamageZoneEffectProto
					{
						VehicleDamageZoneProtoId = newZone.Id,
						TargetType = (int)effect.TargetType,
						TargetProtoId = RemapEffectTarget(effect),
						MinimumStatus = (int)effect.MinimumStatus
					});
				}
			}

			FMDB.Context.SaveChanges();
			return new VehiclePrototype(
				FMDB.Context.VehicleProtos
				    .Include(x => x.EditableItem)
				    .Include(x => x.Compartments)
				    .Include(x => x.CompartmentLinks)
				    .Include(x => x.OccupantSlots)
				    .Include(x => x.ControlStations)
				    .Include(x => x.MovementProfiles).ThenInclude(x => x.PropulsionProfiles)
				    .Include(x => x.AccessPoints)
				    .Include(x => x.CargoSpaces)
				    .Include(x => x.InstallationPoints)
				    .Include(x => x.TowPoints)
				    .Include(x => x.DamageZones).ThenInclude(x => x.Effects)
				    .AsNoTracking()
				    .First(x => x.Id == dbnew.Id && x.RevisionNumber == dbnew.RevisionNumber),
				Gameworld);
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleProtos.Include(x => x.EditableItem).First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
			Save(dbitem.EditableItem);
			dbitem.Name = Name;
			dbitem.Description = _description;
			dbitem.VehicleScale = (int)_scale;
			dbitem.ExteriorItemProtoId = _exteriorItemPrototypeId;
			dbitem.ExteriorItemProtoRevision = _exteriorItemPrototypeRevision;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}
}

public class VehicleCompartmentPrototype : FrameworkItem, IVehicleCompartmentPrototype
{
	public VehicleCompartmentPrototype(DB.VehicleCompartmentProto dbitem, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		Description = dbitem.Description;
		DisplayOrder = dbitem.DisplayOrder;
		InteriorTerrainId = dbitem.InteriorTerrainId;
		InteriorTerrain = dbitem.InteriorTerrainId is null
			? null
			: gameworld.Terrains.Get(dbitem.InteriorTerrainId.Value);
		InteriorOutdoorsType = (CellOutdoorsType)dbitem.InteriorOutdoorsType;
	}

	public override string FrameworkItemType => "VehicleCompartmentPrototype";
	public string Description { get; }
	public int DisplayOrder { get; }
	public long? InteriorTerrainId { get; }
	public ITerrain InteriorTerrain { get; }
	public CellOutdoorsType InteriorOutdoorsType { get; }
}

public class VehicleCompartmentLinkPrototype : FrameworkItem, IVehicleCompartmentLinkPrototype
{
	public VehicleCompartmentLinkPrototype(DB.VehicleCompartmentLinkProto dbitem,
		IEnumerable<IVehicleCompartmentPrototype> compartments)
	{
		_id = dbitem.Id;
		SourceCompartment = compartments.FirstOrDefault(x => x.Id == dbitem.SourceVehicleCompartmentProtoId);
		DestinationCompartment = compartments.FirstOrDefault(x => x.Id == dbitem.DestinationVehicleCompartmentProtoId);
		_name = $"{SourceCompartment?.Name ?? "missing"} to {DestinationCompartment?.Name ?? "missing"}";
		OutboundDirection = dbitem.OutboundDirection;
		InboundDirection = dbitem.InboundDirection;
		OutboundDescription = dbitem.OutboundDescription;
		InboundDescription = dbitem.InboundDescription;
	}

	public override string FrameworkItemType => "VehicleCompartmentLinkPrototype";
	public IVehicleCompartmentPrototype SourceCompartment { get; }
	public IVehicleCompartmentPrototype DestinationCompartment { get; }
	public string OutboundDirection { get; }
	public string InboundDirection { get; }
	public string OutboundDescription { get; }
	public string InboundDescription { get; }
}

public class VehicleOccupantSlotPrototype : FrameworkItem, IVehicleOccupantSlotPrototype
{
	public VehicleOccupantSlotPrototype(DB.VehicleOccupantSlotProto dbitem,
		IEnumerable<IVehicleCompartmentPrototype> compartments, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		Compartment = compartments.First(x => x.Id == dbitem.VehicleCompartmentProtoId);
		SlotType = (VehicleOccupantSlotType)dbitem.SlotType;
		Capacity = dbitem.Capacity;
		RequiredForMovement = dbitem.RequiredForMovement;
		ContributesToPropulsion = dbitem.ContributesToPropulsion;
		SameLevelRangedCover = gameworld?.RangedCovers.Get(dbitem.SameLevelRangedCoverId ?? 0);
		AboveRangedCover = gameworld?.RangedCovers.Get(dbitem.AboveRangedCoverId ?? 0);
		BelowRangedCover = gameworld?.RangedCovers.Get(dbitem.BelowRangedCoverId ?? 0);
		BoatStabilityDifficulty = (Difficulty)dbitem.BoatStabilityDifficulty;
	}

	public override string FrameworkItemType => "VehicleOccupantSlotPrototype";
	public IVehicleCompartmentPrototype Compartment { get; }
	public VehicleOccupantSlotType SlotType { get; }
	public int Capacity { get; }
	public bool RequiredForMovement { get; }
	public bool ContributesToPropulsion { get; }
	public IRangedCover SameLevelRangedCover { get; }
	public IRangedCover AboveRangedCover { get; }
	public IRangedCover BelowRangedCover { get; }
	public Difficulty BoatStabilityDifficulty { get; }
}

public class VehicleControlStationPrototype : FrameworkItem, IVehicleControlStationPrototype
{
	public VehicleControlStationPrototype(DB.VehicleControlStationProto dbitem, IEnumerable<IVehicleOccupantSlotPrototype> slots)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		OccupantSlot = slots.First(x => x.Id == dbitem.VehicleOccupantSlotProtoId);
		IsPrimary = dbitem.IsPrimary;
	}

	public override string FrameworkItemType => "VehicleControlStationPrototype";
	public IVehicleOccupantSlotPrototype OccupantSlot { get; }
	public bool IsPrimary { get; }
}

public class VehicleMovementProfilePrototype : FrameworkItem, IVehicleMovementProfilePrototype
{
	private readonly List<IVehiclePropulsionProfilePrototype> _propulsionProfiles = new();

	public VehicleMovementProfilePrototype(DB.VehicleMovementProfileProto dbitem) : this(dbitem, null)
	{
	}

	public VehicleMovementProfilePrototype(DB.VehicleMovementProfileProto dbitem, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		MovementType = (VehicleMovementProfileType)dbitem.MovementType;
		MovementEnvironment = (VehicleMovementEnvironment)dbitem.MovementEnvironment;
		ExposesOccupantsToWater = dbitem.ExposesOccupantsToWater;
		IsDefault = dbitem.IsDefault;
		RequiredPowerSpikeInWatts = dbitem.RequiredPowerSpikeInWatts;
		FuelLiquidId = dbitem.FuelLiquidId;
		FuelVolumePerMove = dbitem.FuelVolumePerMove;
		RequiredInstalledRole = dbitem.RequiredInstalledRole ?? string.Empty;
		RequiresTowLinksClosed = dbitem.RequiresTowLinksClosed;
		RequiresAccessPointsClosed = dbitem.RequiresAccessPointsClosed;
		RouteSpeedMetresPerSecond = dbitem.RouteSpeedMetresPerSecond;
		RoutePropulsionMode = (RouteVehiclePropulsionMode)dbitem.RoutePropulsionMode;
		RouteFuelVolumePerMetre = dbitem.RouteFuelVolumePerMetre;
		RoutePowerDrawWatts = dbitem.RoutePowerDrawWatts;
		AutomaticOperationCapable = dbitem.AutomaticOperationCapable;
		_propulsionProfiles.AddRange(dbitem.PropulsionProfiles
			.OrderBy(x => x.Id)
			.Select(x => new VehiclePropulsionProfilePrototype(x, gameworld)));
	}

	public override string FrameworkItemType => "VehicleMovementProfilePrototype";
	public VehicleMovementProfileType MovementType { get; }
	public VehicleMovementEnvironment MovementEnvironment { get; }
	public bool ExposesOccupantsToWater { get; }
	public bool IsDefault { get; }
	public double RequiredPowerSpikeInWatts { get; }
	public long? FuelLiquidId { get; }
	public double FuelVolumePerMove { get; }
	public string RequiredInstalledRole { get; }
	public bool RequiresTowLinksClosed { get; }
	public bool RequiresAccessPointsClosed { get; }
	public double RouteSpeedMetresPerSecond { get; }
	public RouteVehiclePropulsionMode RoutePropulsionMode { get; }
	public double RouteFuelVolumePerMetre { get; }
	public double RoutePowerDrawWatts { get; }
	public bool AutomaticOperationCapable { get; }
	public IEnumerable<IVehiclePropulsionProfilePrototype> PropulsionProfiles => _propulsionProfiles;
}

public class VehiclePropulsionProfilePrototype : FrameworkItem, IVehiclePropulsionProfilePrototype
{
	public VehiclePropulsionProfilePrototype(DB.VehiclePropulsionProfileProto dbitem, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		PropulsionType = (VehiclePropulsionType)dbitem.PropulsionType;
		_name = PropulsionType.DescribeEnum();
		IsDefault = dbitem.IsDefault;
		BaseMoveTimeMilliseconds = dbitem.BaseMoveTimeMilliseconds;
		PropulsionTrait = dbitem.PropulsionTraitDefinitionId is null || gameworld is null
			? null!
			: gameworld.Traits.Get(dbitem.PropulsionTraitDefinitionId.Value)!;
		CheckDifficulty = (Difficulty)dbitem.CheckDifficulty;
		SpeedMultiplierExpression = dbitem.SpeedMultiplierExpression;
		StaminaCostExpression = dbitem.StaminaCostExpression;
	}

	public override string FrameworkItemType => "VehiclePropulsionProfilePrototype";
	public VehiclePropulsionType PropulsionType { get; }
	public bool IsDefault { get; }
	public double BaseMoveTimeMilliseconds { get; }
	public ITraitDefinition PropulsionTrait { get; }
	public Difficulty CheckDifficulty { get; }
	public string SpeedMultiplierExpression { get; }
	public string StaminaCostExpression { get; }

	public static bool Validate(IVehiclePropulsionProfilePrototype profile, out string reason)
	{
		if (!Enum.IsDefined(profile.PropulsionType))
		{
			reason = "the propulsion type is invalid";
			return false;
		}

		if (!double.IsFinite(profile.BaseMoveTimeMilliseconds) || profile.BaseMoveTimeMilliseconds <= 0.0)
		{
			reason = "the base traversal time must be positive";
			return false;
		}

		if (profile.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed &&
		    profile.PropulsionTrait is null)
		{
			reason = "self-powered and rowed modes require a propulsion trait";
			return false;
		}

		if (profile.PropulsionType != VehiclePropulsionType.None &&
		    !VehiclePrototype.ValidatePropulsionExpression(profile.PropulsionType,
			    profile.SpeedMultiplierExpression, true, out reason))
		{
			return false;
		}

		if (profile.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed &&
		    !VehiclePrototype.ValidatePropulsionExpression(profile.PropulsionType,
			    profile.StaminaCostExpression, false, out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}
}

public class VehicleAccessPointPrototype : FrameworkItem, IVehicleAccessPointPrototype
{
	private readonly IFuturemud _gameworld;
	private readonly long? _projectionItemPrototypeId;
	private readonly int? _projectionItemPrototypeRevision;

	public VehicleAccessPointPrototype(DB.VehicleAccessPointProto dbitem, IEnumerable<IVehicleCompartmentPrototype> compartments,
		IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Compartment = dbitem.VehicleCompartmentProtoId is null
			? null
			: compartments.FirstOrDefault(x => x.Id == dbitem.VehicleCompartmentProtoId.Value);
		Description = dbitem.Description;
		AccessPointType = (VehicleAccessPointType)dbitem.AccessPointType;
		_projectionItemPrototypeId = dbitem.ProjectionItemProtoId;
		_projectionItemPrototypeRevision = dbitem.ProjectionItemProtoRevision;
		StartsOpen = dbitem.StartsOpen;
		MustBeClosedForMovement = dbitem.MustBeClosedForMovement;
		DisplayOrder = dbitem.DisplayOrder;
	}

	public override string FrameworkItemType => "VehicleAccessPointPrototype";
	public IVehicleCompartmentPrototype Compartment { get; }
	public VehicleAccessPointType AccessPointType { get; }
	public string Description { get; }
	public long? ProjectionItemPrototypeId => _projectionItemPrototypeId;
	public IGameItemProto ProjectionItemPrototype => _projectionItemPrototypeId is null
		? null
		: _projectionItemPrototypeRevision is null
			? _gameworld.ItemProtos.Get(_projectionItemPrototypeId.Value)
			: _gameworld.ItemProtos.Get(_projectionItemPrototypeId.Value, _projectionItemPrototypeRevision.Value);
	public bool StartsOpen { get; }
	public bool MustBeClosedForMovement { get; }
	public int DisplayOrder { get; }
}

public class VehicleCargoSpacePrototype : FrameworkItem, IVehicleCargoSpacePrototype
{
	private readonly IFuturemud _gameworld;
	private readonly long? _projectionItemPrototypeId;
	private readonly int? _projectionItemPrototypeRevision;

	public VehicleCargoSpacePrototype(DB.VehicleCargoSpaceProto dbitem, IEnumerable<IVehicleCompartmentPrototype> compartments,
		IEnumerable<IVehicleAccessPointPrototype> accessPoints, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Compartment = dbitem.VehicleCompartmentProtoId is null
			? null
			: compartments.FirstOrDefault(x => x.Id == dbitem.VehicleCompartmentProtoId.Value);
		RequiredAccessPoint = dbitem.RequiredAccessPointProtoId is null
			? null
			: accessPoints.FirstOrDefault(x => x.Id == dbitem.RequiredAccessPointProtoId.Value);
		Description = dbitem.Description;
		_projectionItemPrototypeId = dbitem.ProjectionItemProtoId;
		_projectionItemPrototypeRevision = dbitem.ProjectionItemProtoRevision;
		DisplayOrder = dbitem.DisplayOrder;
	}

	public override string FrameworkItemType => "VehicleCargoSpacePrototype";
	public IVehicleCompartmentPrototype Compartment { get; }
	public IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	public string Description { get; }
	public long? ProjectionItemPrototypeId => _projectionItemPrototypeId;
	public IGameItemProto ProjectionItemPrototype => _projectionItemPrototypeId is null
		? null
		: _projectionItemPrototypeRevision is null
			? _gameworld.ItemProtos.Get(_projectionItemPrototypeId.Value)
			: _gameworld.ItemProtos.Get(_projectionItemPrototypeId.Value, _projectionItemPrototypeRevision.Value);
	public int DisplayOrder { get; }
}

public class VehicleInstallationPointPrototype : FrameworkItem, IVehicleInstallationPointPrototype
{
	public VehicleInstallationPointPrototype(DB.VehicleInstallationPointProto dbitem,
		IEnumerable<IVehicleAccessPointPrototype> accessPoints)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		RequiredAccessPoint = dbitem.RequiredAccessPointProtoId is null
			? null
			: accessPoints.FirstOrDefault(x => x.Id == dbitem.RequiredAccessPointProtoId.Value);
		Description = dbitem.Description;
		MountType = dbitem.MountType ?? string.Empty;
		RequiredRole = dbitem.RequiredRole ?? string.Empty;
		RequiredForMovement = dbitem.RequiredForMovement;
		DisplayOrder = dbitem.DisplayOrder;
	}

	public override string FrameworkItemType => "VehicleInstallationPointPrototype";
	public IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	public string Description { get; }
	public string MountType { get; }
	public string RequiredRole { get; }
	public bool RequiredForMovement { get; }
	public int DisplayOrder { get; }
}

public class VehicleTowPointPrototype : FrameworkItem, IVehicleTowPointPrototype
{
	public VehicleTowPointPrototype(DB.VehicleTowPointProto dbitem, IEnumerable<IVehicleAccessPointPrototype> accessPoints)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		RequiredAccessPoint = dbitem.RequiredAccessPointProtoId is null
			? null
			: accessPoints.FirstOrDefault(x => x.Id == dbitem.RequiredAccessPointProtoId.Value);
		Description = dbitem.Description;
		TowType = dbitem.TowType ?? string.Empty;
		CanTow = dbitem.CanTow;
		CanBeTowed = dbitem.CanBeTowed;
		MaximumTowedWeight = dbitem.MaximumTowedWeight;
		CharacterPullMultiplier = dbitem.CharacterPullMultiplier <= 0.0 ? 1.0 : dbitem.CharacterPullMultiplier;
		TowStressWarningRatio = dbitem.TowStressWarningRatio;
		TowStressFailureStartRatio = dbitem.TowStressFailureStartRatio;
		TowStressMaximumFailureChance = dbitem.TowStressMaximumFailureChance;
		TowStressDamageMultiplier = dbitem.TowStressDamageMultiplier;
		DisplayOrder = dbitem.DisplayOrder;
	}

	public override string FrameworkItemType => "VehicleTowPointPrototype";
	public IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	public string Description { get; }
	public string TowType { get; }
	public bool CanTow { get; }
	public bool CanBeTowed { get; }
	public double MaximumTowedWeight { get; }
	public double CharacterPullMultiplier { get; }
	public double? TowStressWarningRatio { get; }
	public double? TowStressFailureStartRatio { get; }
	public double? TowStressMaximumFailureChance { get; }
	public double? TowStressDamageMultiplier { get; }
	public int DisplayOrder { get; }
}

public class VehicleDamageZonePrototype : FrameworkItem, IVehicleDamageZonePrototype
{
	private readonly List<IVehicleDamageZoneEffectPrototype> _effects = new();

	public VehicleDamageZonePrototype(DB.VehicleDamageZoneProto dbitem)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		Description = dbitem.Description;
		MaximumDamage = dbitem.MaximumDamage;
		HitWeight = dbitem.HitWeight;
		DisabledThreshold = dbitem.DisabledThreshold;
		DestroyedThreshold = dbitem.DestroyedThreshold;
		DisablesMovement = dbitem.DisablesMovement;
		DisplayOrder = dbitem.DisplayOrder;
		_effects.AddRange(dbitem.Effects.OrderBy(x => x.Id).Select(x => new VehicleDamageZoneEffectPrototype(x)));
	}

	public override string FrameworkItemType => "VehicleDamageZonePrototype";
	public string Description { get; }
	public double MaximumDamage { get; }
	public double HitWeight { get; }
	public double DisabledThreshold { get; }
	public double DestroyedThreshold { get; }
	public bool DisablesMovement { get; }
	public IEnumerable<IVehicleDamageZoneEffectPrototype> Effects => _effects;
	public int DisplayOrder { get; }
}

public class VehicleDamageZoneEffectPrototype : FrameworkItem, IVehicleDamageZoneEffectPrototype
{
	public VehicleDamageZoneEffectPrototype(DB.VehicleDamageZoneEffectProto dbitem)
	{
		_id = dbitem.Id;
		TargetType = (VehicleDamageEffectTargetType)dbitem.TargetType;
		TargetPrototypeId = dbitem.TargetProtoId;
		MinimumStatus = (VehicleSystemStatus)dbitem.MinimumStatus;
		_name = $"{TargetType.DescribeEnum()} {(TargetPrototypeId?.ToString("N0") ?? "all")}";
	}

	public override string FrameworkItemType => "VehicleDamageZoneEffectPrototype";
	public VehicleDamageEffectTargetType TargetType { get; }
	public long? TargetPrototypeId { get; }
	public VehicleSystemStatus MinimumStatus { get; }
}
