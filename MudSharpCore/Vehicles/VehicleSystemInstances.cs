#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using System;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class VehicleAccessPoint : FrameworkItem, IVehicleAccessPoint
{
	private readonly List<ILock> _locks = new();
	private long? _projectionItemId;
	private IGameItem? _projectionItem;
	private bool _isOpen;
	private bool _isDisabled;

	public VehicleAccessPoint(IVehicle vehicle, DB.VehicleAccessPoint dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_projectionItemId = dbitem.ProjectionItemId;
		_isOpen = dbitem.IsOpen;
		_isDisabled = dbitem.IsDisabled;
		Prototype = vehicle.Prototype.AccessPoints.FirstOrDefault(x => x.Id == dbitem.VehicleAccessPointProtoId)!;

		foreach (var lockItem in dbitem.Locks
		                               .Select(x => vehicle.Gameworld.TryGetItem(x.LockItemId, true))
		                               .Select(x => x?.GetItemType<ILock>())
		                               .Where(x => x is not null)
		                               .Cast<ILock>())
		{
			_locks.Add(lockItem);
		}
	}

	public override string FrameworkItemType => "VehicleAccessPoint";
	public IVehicle Vehicle { get; }
	public IVehicleAccessPointPrototype Prototype { get; }
	public long? ProjectionItemId => _projectionItemId;

	public IGameItem? ProjectionItem
	{
		get
		{
			if (_projectionItem is not null)
			{
				return _projectionItem;
			}

			_projectionItem = _projectionItemId is null ? null : Vehicle.Gameworld.TryGetItem(_projectionItemId.Value, true);
			return _projectionItem;
		}
	}

	public bool IsOpen => _isOpen;
	public bool IsManuallyDisabled => _isDisabled;
	public bool IsDisabled => _isDisabled ||
	                          Vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.AccessPoint, Prototype.Id);
	public bool IsLocked => Locks.Any(x => x.IsLocked);
	public IEnumerable<ILock> Locks => _locks;

	public bool CanUse(ICharacter actor, out string reason)
	{
		if (IsDisabled)
		{
			reason = Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.AccessPoint, Prototype.Id);
			reason = string.IsNullOrWhiteSpace(reason) ? "That access point is disabled." : $"That access point is disabled because {reason}.";
			return false;
		}

		if (IsLocked)
		{
			reason = "That access point is locked.";
			return false;
		}

		if (!IsOpen)
		{
			reason = "That access point is closed.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public void SetOpen(bool open)
	{
		if (_isOpen == open)
		{
			return;
		}

		_isOpen = open;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsOpen = open;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public bool InstallLock(ILock theLock, ICharacter? actor = null)
	{
		if (IsDisabled || theLock is null || _locks.Contains(theLock))
		{
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehicleAccessPointLocks.Add(new DB.VehicleAccessPointLock
			{
				VehicleAccessPointId = Id,
				LockItemId = theLock.Parent.Id
			});
			FMDB.Context.SaveChanges();
		}

		_locks.Add(theLock);
		theLock.Parent.ContainedIn = ProjectionItem;
		theLock.InstallLock(ProjectionItem?.GetItemType<IVehicleAccessPointItem>() as ILockable, null,
			actor?.Location ?? Vehicle.Location);
		return true;
	}

	public bool RemoveLock(ILock theLock)
	{
		if (IsDisabled)
		{
			return false;
		}

		if (!_locks.Remove(theLock))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPointLocks.FirstOrDefault(x =>
				x.VehicleAccessPointId == Id && x.LockItemId == theLock.Parent.Id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleAccessPointLocks.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		theLock.Parent.ContainedIn = null;
		theLock.InstallLock(null, null, null);
		return true;
	}

	public void LinkProjectionItem(IGameItem item)
	{
		_projectionItem = item;
		_projectionItemId = item?.Id;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.ProjectionItemId = item?.Id;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void SetDisabled(bool disabled)
	{
		if (_isDisabled == disabled)
		{
			return;
		}

		_isDisabled = disabled;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessPoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsDisabled = disabled;
				FMDB.Context.SaveChanges();
			}
		}
	}
}

public class VehicleCargoSpace : FrameworkItem, IVehicleCargoSpace
{
	private long? _projectionItemId;
	private IGameItem? _projectionItem;
	private bool _isDisabled;

	public VehicleCargoSpace(IVehicle vehicle, DB.VehicleCargoSpace dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_projectionItemId = dbitem.ProjectionItemId;
		_isDisabled = dbitem.IsDisabled;
		Prototype = vehicle.Prototype.CargoSpaces.FirstOrDefault(x => x.Id == dbitem.VehicleCargoSpaceProtoId)!;
	}

	public override string FrameworkItemType => "VehicleCargoSpace";
	public IVehicle Vehicle { get; }
	public IVehicleCargoSpacePrototype Prototype { get; }
	public long? ProjectionItemId => _projectionItemId;

	public IGameItem? ProjectionItem
	{
		get
		{
			if (_projectionItem is not null)
			{
				return _projectionItem;
			}

			_projectionItem = _projectionItemId is null ? null : Vehicle.Gameworld.TryGetItem(_projectionItemId.Value, true);
			return _projectionItem;
		}
	}

	public bool IsManuallyDisabled => _isDisabled;
	public bool IsDisabled => _isDisabled ||
	                          Vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.CargoSpace, Prototype.Id);

	public bool CanAccess(ICharacter actor, out string reason)
	{
		if (IsDisabled)
		{
			reason = Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.CargoSpace, Prototype.Id);
			reason = string.IsNullOrWhiteSpace(reason) ? "That cargo space is disabled." : $"That cargo space is disabled because {reason}.";
			return false;
		}

		if (Prototype.RequiredAccessPoint is not null)
		{
			var access = Vehicle.AccessPoints.FirstOrDefault(x => x.Prototype.Id == Prototype.RequiredAccessPoint.Id);
			if (access is not null && !access.CanUse(actor, out reason))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public void LinkProjectionItem(IGameItem item)
	{
		_projectionItem = item;
		_projectionItemId = item?.Id;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleCargoSpaces.Find(Id);
			if (dbitem is not null)
			{
				dbitem.ProjectionItemId = item?.Id;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void SetDisabled(bool disabled)
	{
		if (_isDisabled == disabled)
		{
			return;
		}

		_isDisabled = disabled;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleCargoSpaces.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsDisabled = disabled;
				FMDB.Context.SaveChanges();
			}
		}
	}
}

public class VehicleInstallation : FrameworkItem, IVehicleInstallation
{
	private long? _installedItemId;
	private IGameItem? _installedItem;
	private bool _isDisabled;

	public VehicleInstallation(IVehicle vehicle, DB.VehicleInstallation dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = $"Vehicle Installation #{dbitem.Id:N0}";
		_installedItemId = dbitem.InstalledItemId;
		_isDisabled = dbitem.IsDisabled;
		Prototype = vehicle.Prototype.InstallationPoints.FirstOrDefault(x => x.Id == dbitem.VehicleInstallationPointProtoId)!;
	}

	public override string FrameworkItemType => "VehicleInstallation";
	public IVehicle Vehicle { get; }
	public IVehicleInstallationPointPrototype Prototype { get; }
	public long? InstalledItemId => _installedItemId;

	public IGameItem? InstalledItem
	{
		get
		{
			if (_installedItem is not null)
			{
				return _installedItem;
			}

			_installedItem = _installedItemId is null ? null : Vehicle.Gameworld.TryGetItem(_installedItemId.Value, true);
			return _installedItem;
		}
	}

	public bool IsManuallyDisabled => _isDisabled;
	public bool IsDisabled => _isDisabled ||
	                          Vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.InstallationPoint, Prototype.Id);

	public bool CanInstall(ICharacter actor, IGameItem item, out string reason)
	{
		if (IsDisabled)
		{
			reason = Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.InstallationPoint, Prototype.Id);
			reason = string.IsNullOrWhiteSpace(reason) ? "That installation point is disabled." : $"That installation point is disabled because {reason}.";
			return false;
		}

		if (InstalledItem is not null)
		{
			reason = "That installation point is already occupied.";
			return false;
		}

		var installable = item?.GetItemType<IVehicleInstallable>();
		if (installable is null)
		{
			reason = "That item is not a vehicle-installable module.";
			return false;
		}

		if (!installable.MountType.EqualTo(Prototype.MountType))
		{
			reason = $"That module requires a {installable.MountType.ColourCommand()} mount, not a {Prototype.MountType.ColourCommand()} mount.";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(Prototype.RequiredRole) &&
		    !installable.Role.EqualTo(Prototype.RequiredRole))
		{
			reason = $"That module does not fulfil the {Prototype.RequiredRole.ColourCommand()} vehicle role.";
			return false;
		}

		if (Prototype.RequiredAccessPoint is not null)
		{
			var access = Vehicle.AccessPoints.FirstOrDefault(x => x.Prototype.Id == Prototype.RequiredAccessPoint.Id);
			if (access is not null && !access.CanUse(actor, out reason))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public bool Install(ICharacter actor, IGameItem item)
	{
		if (!CanInstall(actor, item, out _))
		{
			return false;
		}

		item.InInventoryOf?.Take(item);
		item.ContainedIn?.Take(item);
		item.Location?.Extract(item);
		item.Get(null);

		_installedItem = item;
		_installedItemId = item.Id;
		item.GetItemType<IVehicleInstallable>()?.LinkInstallation(this);
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleInstallations.Find(Id);
			if (dbitem is not null)
			{
				dbitem.InstalledItemId = item.Id;
				FMDB.Context.SaveChanges();
			}
		}

		return true;
	}

	public bool CanRemove(ICharacter actor, out string reason)
	{
		if (InstalledItem is null)
		{
			reason = "There is no module installed there.";
			return false;
		}

		if (IsDisabled)
		{
			reason = Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.InstallationPoint, Prototype.Id);
			reason = string.IsNullOrWhiteSpace(reason) ? "That installation point is disabled." : $"That installation point is disabled because {reason}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public IGameItem? Remove(ICharacter actor)
	{
		if (!CanRemove(actor, out _))
		{
			return null;
		}

		var item = InstalledItem;
		_installedItem = null;
		_installedItemId = null;
		item?.GetItemType<IVehicleInstallable>()?.ClearInstallation();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleInstallations.Find(Id);
			if (dbitem is not null)
			{
				dbitem.InstalledItemId = null;
				FMDB.Context.SaveChanges();
			}
		}

		return item;
	}

	public void SetDisabled(bool disabled)
	{
		if (_isDisabled == disabled)
		{
			return;
		}

		_isDisabled = disabled;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleInstallations.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsDisabled = disabled;
				FMDB.Context.SaveChanges();
			}
		}
	}
}

public class VehicleTowLink : FrameworkItem, IVehicleTowLink
{
	private readonly long _sourceVehicleId;
	private readonly long _targetVehicleId;
	private readonly long _sourceTowPointProtoId;
	private readonly long _targetTowPointProtoId;
	private readonly long? _hitchItemId;
	private IGameItem? _hitchItem;
	private readonly bool _isDisabled;

	public VehicleTowLink(IFuturemud gameworld, DB.VehicleTowLink dbitem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = $"Vehicle Tow Link #{dbitem.Id:N0}";
		_sourceVehicleId = dbitem.SourceVehicleId;
		_targetVehicleId = dbitem.TargetVehicleId;
		_sourceTowPointProtoId = dbitem.SourceTowPointProtoId;
		_targetTowPointProtoId = dbitem.TargetTowPointProtoId;
		_hitchItemId = dbitem.HitchItemId;
		_isDisabled = dbitem.IsDisabled;
	}

	public IFuturemud Gameworld { get; }
	public override string FrameworkItemType => "VehicleTowLink";
	public long SourceVehicleId => _sourceVehicleId;
	public long TargetVehicleId => _targetVehicleId;
	public long SourceTowPointPrototypeId => _sourceTowPointProtoId;
	public long TargetTowPointPrototypeId => _targetTowPointProtoId;
	public IVehicle? SourceVehicle => Gameworld.TryGetVehicle(_sourceVehicleId);
	public IVehicle? TargetVehicle => Gameworld.TryGetVehicle(_targetVehicleId);
	public IVehicleTowPointPrototype? SourceTowPoint => SourceVehicle?.Prototype.TowPoints.FirstOrDefault(x => x.Id == _sourceTowPointProtoId);
	public IVehicleTowPointPrototype? TargetTowPoint => TargetVehicle?.Prototype.TowPoints.FirstOrDefault(x => x.Id == _targetTowPointProtoId);
	public long? HitchItemId => _hitchItemId;

	public IGameItem? HitchItem
	{
		get
		{
			if (_hitchItem is not null)
			{
				return _hitchItem;
			}

			_hitchItem = _hitchItemId is null ? null : Gameworld.TryGetItem(_hitchItemId.Value, true);
			return _hitchItem;
		}
	}

	public bool IsManuallyDisabled => _isDisabled;
	public bool IsBroken => !string.IsNullOrWhiteSpace(WhyInvalid);
	public bool IsDisabled => IsBroken;

	public string WhyInvalid
	{
		get
		{
			if (_isDisabled)
			{
				return "the link is manually disabled";
			}

			var sourceVehicle = SourceVehicle;
			if (sourceVehicle is null)
			{
				return "the source vehicle is missing";
			}

			var targetVehicle = TargetVehicle;
			if (targetVehicle is null)
			{
				return "the target vehicle is missing";
			}

			var sourceTowPoint = SourceTowPoint;
			if (sourceTowPoint is null)
			{
				return "the source tow point is missing";
			}

			var targetTowPoint = TargetTowPoint;
			if (targetTowPoint is null)
			{
				return "the target tow point is missing";
			}

			if (!sourceTowPoint.CanTow)
			{
				return "the source tow point cannot tow";
			}

			if (!targetTowPoint.CanBeTowed)
			{
				return "the target tow point cannot be towed";
			}

			if (!sourceTowPoint.TowType.Equals(targetTowPoint.TowType, StringComparison.InvariantCultureIgnoreCase))
			{
				return "the tow point types are incompatible";
			}

			if (sourceVehicle.Location != targetVehicle.Location || sourceVehicle.RoomLayer != targetVehicle.RoomLayer)
			{
				return "the linked vehicles are not in the same location and layer";
			}

			if (sourceVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id))
			{
				return $"{sourceTowPoint.Name} is disabled because {sourceVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id)}";
			}

			if (targetVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id))
			{
				return $"{targetTowPoint.Name} is disabled because {targetVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id)}";
			}

			if (HitchGearRules.TowPointsRequireHitchItem(sourceTowPoint, targetTowPoint) && _hitchItemId is null)
			{
				return "the tow point requires a hitch item";
			}

			if (_hitchItemId is not null)
			{
				var item = HitchItem;
				if (item is null)
				{
					return "the hitch item is missing";
				}

				if (item.Deleted || item.Destroyed)
				{
					return "the hitch item is destroyed";
				}

				if (item.ContainedIn is not null || item.InInventoryOf is not null ||
				    item.Location != sourceVehicle.Location || item.RoomLayer != sourceVehicle.RoomLayer)
				{
					return "the hitch item is not with the tow train";
				}

				if (!HitchGearRules.GearCompatible(item, targetVehicle.ExteriorItem?.Weight ?? 0.0, out var reason,
					    false, sourceTowPoint, targetTowPoint))
				{
					return reason;
				}
			}

			return string.Empty;
		}
	}
}

public class VehicleHitchLink : FrameworkItem, IVehicleHitchLink
{
	private readonly VehicleHitchEndpointType _sourceType;
	private readonly long? _sourceVehicleId;
	private readonly long? _sourceCharacterId;
	private readonly long? _sourceTowPointProtoId;
	private readonly VehicleHitchEndpointType _targetType;
	private readonly long? _targetVehicleId;
	private readonly long? _targetCharacterId;
	private readonly long? _targetTowPointProtoId;
	private readonly long? _hitchItemId;
	private readonly bool _isDisabled;
	private readonly DateTime _createdDateTime;
	private IGameItem? _hitchItem;

	public VehicleHitchLink(IFuturemud gameworld, DB.VehicleHitchLink dbitem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = $"Vehicle Hitch Link #{dbitem.Id:N0}";
		_sourceType = (VehicleHitchEndpointType)dbitem.SourceType;
		_sourceVehicleId = dbitem.SourceVehicleId;
		_sourceCharacterId = dbitem.SourceCharacterId;
		_sourceTowPointProtoId = dbitem.SourceTowPointProtoId;
		_targetType = (VehicleHitchEndpointType)dbitem.TargetType;
		_targetVehicleId = dbitem.TargetVehicleId;
		_targetCharacterId = dbitem.TargetCharacterId;
		_targetTowPointProtoId = dbitem.TargetTowPointProtoId;
		_hitchItemId = dbitem.HitchItemId;
		_isDisabled = dbitem.IsDisabled;
		_createdDateTime = dbitem.CreatedDateTime;
	}

	public IFuturemud Gameworld { get; }
	public override string FrameworkItemType => "VehicleHitchLink";
	public VehicleHitchEndpointType SourceType => _sourceType;
	public long? SourceVehicleId => _sourceVehicleId;
	public long? SourceCharacterId => _sourceCharacterId;
	public long? SourceTowPointPrototypeId => _sourceTowPointProtoId;
	public VehicleHitchEndpointType TargetType => _targetType;
	public long? TargetVehicleId => _targetVehicleId;
	public long? TargetCharacterId => _targetCharacterId;
	public long? TargetTowPointPrototypeId => _targetTowPointProtoId;
	public long? HitchItemId => _hitchItemId;
	public bool IsManuallyDisabled => _isDisabled;
	public bool IsDisabled => IsBroken;
	public DateTime CreatedDateTime => _createdDateTime;
	public IVehicle? SourceVehicle => _sourceVehicleId is null ? null : Gameworld.TryGetVehicle(_sourceVehicleId.Value);
	public ICharacter? SourceCharacter => _sourceCharacterId is null ? null : Gameworld.Characters.Get(_sourceCharacterId.Value);
	public IVehicleTowPointPrototype? SourceTowPoint => SourceVehicle?.Prototype.TowPoints.FirstOrDefault(x => x.Id == _sourceTowPointProtoId);
	public IVehicle? TargetVehicle => _targetVehicleId is null ? null : Gameworld.TryGetVehicle(_targetVehicleId.Value);
	public ICharacter? TargetCharacter => _targetCharacterId is null ? null : Gameworld.Characters.Get(_targetCharacterId.Value);
	public IVehicleTowPointPrototype? TargetTowPoint => TargetVehicle?.Prototype.TowPoints.FirstOrDefault(x => x.Id == _targetTowPointProtoId);

	public IGameItem? HitchItem
	{
		get
		{
			if (_hitchItem is not null)
			{
				return _hitchItem;
			}

			_hitchItem = _hitchItemId is null ? null : Gameworld.TryGetItem(_hitchItemId.Value, true);
			return _hitchItem;
		}
	}

	public bool IsBroken => !string.IsNullOrWhiteSpace(WhyInvalid);

	public string WhyInvalid
	{
		get
		{
			if (_isDisabled)
			{
				return "the link is manually disabled";
			}

			var sourceReason = EndpointInvalidReason(true);
			if (!string.IsNullOrWhiteSpace(sourceReason))
			{
				return sourceReason;
			}

			var targetReason = EndpointInvalidReason(false);
			if (!string.IsNullOrWhiteSpace(targetReason))
			{
				return targetReason;
			}

			if (SourceTowPoint is not null && TargetTowPoint is not null &&
			    !SourceTowPoint.TowType.Equals(TargetTowPoint.TowType, StringComparison.InvariantCultureIgnoreCase))
			{
				return "the tow point types are incompatible";
			}

			var sourceLocation = SourceLocation();
			var targetLocation = TargetLocation();
			if (sourceLocation is null || targetLocation is null)
			{
				return "one of the hitch endpoints is not in the world";
			}

			if (sourceLocation != targetLocation || SourceRoomLayer() != TargetRoomLayer())
			{
				return "the hitch endpoints are not in the same location and layer";
			}

			if (TargetType == VehicleHitchEndpointType.Vehicle &&
			    TargetTowPoint is not null &&
			    HitchGearRules.TowPointRequiresHitchItem(TargetTowPoint) &&
			    _hitchItemId is null)
			{
				return "the vehicle tow point requires a hitch item";
			}

			if (_hitchItemId is not null)
			{
				var item = HitchItem;
				if (item is null)
				{
					return "the hitch item is missing";
				}

				if (item.Deleted || item.Destroyed)
				{
					return "the hitch item is destroyed";
				}

				if (!HitchItemIsWithChain(item, sourceLocation, SourceRoomLayer()))
				{
					return "the hitch item is not with the hitch chain";
				}

				if (TargetType == VehicleHitchEndpointType.Vehicle &&
				    TargetTowPoint is not null &&
				    !HitchGearRules.GearCompatible(item, TargetVehicle?.ExteriorItem?.Weight ?? 0.0,
					    out var reason, false, TargetTowPoint))
				{
					return reason;
				}
			}

			return string.Empty;
		}
	}

	private string EndpointInvalidReason(bool source)
	{
		var type = source ? SourceType : TargetType;
		var vehicle = source ? SourceVehicle : TargetVehicle;
		var character = source ? SourceCharacter : TargetCharacter;
		var towPoint = source ? SourceTowPoint : TargetTowPoint;
		var vehicleId = source ? _sourceVehicleId : _targetVehicleId;
		var characterId = source ? _sourceCharacterId : _targetCharacterId;
		var towPointId = source ? _sourceTowPointProtoId : _targetTowPointProtoId;
		var label = source ? "source" : "target";

		switch (type)
		{
			case VehicleHitchEndpointType.Vehicle:
				if (vehicleId is null)
				{
					return $"the {label} vehicle id is missing";
				}

				if (vehicle is null)
				{
					return $"the {label} vehicle is missing";
				}

				if (towPointId is null)
				{
					return $"the {label} tow point id is missing";
				}

				if (towPoint is null)
				{
					return $"the {label} tow point is missing";
				}

				if (source && !towPoint.CanTow)
				{
					return "the source tow point cannot tow";
				}

				if (!source && !towPoint.CanBeTowed)
				{
					return "the target tow point cannot be towed";
				}

				if (vehicle.Destroyed)
				{
					return $"the {label} vehicle is destroyed";
				}

				if (vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, towPoint.Id))
				{
					return $"{towPoint.Name} is disabled because {vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, towPoint.Id)}";
				}

				return string.Empty;
			case VehicleHitchEndpointType.Character:
				if (characterId is null)
				{
					return $"the {label} character id is missing";
				}

				if (character is null)
				{
					return $"the {label} character is missing";
				}

				return string.Empty;
			default:
				return $"the {label} endpoint type is invalid";
		}
	}

	private ICell? SourceLocation()
	{
		return SourceType switch
		{
			VehicleHitchEndpointType.Vehicle => SourceVehicle?.Location,
			VehicleHitchEndpointType.Character => SourceCharacter?.Location,
			_ => null
		};
	}

	private ICell? TargetLocation()
	{
		return TargetType switch
		{
			VehicleHitchEndpointType.Vehicle => TargetVehicle?.Location,
			VehicleHitchEndpointType.Character => TargetCharacter?.Location,
			_ => null
		};
	}

	private RoomLayer SourceRoomLayer()
	{
		return SourceType switch
		{
			VehicleHitchEndpointType.Vehicle => SourceVehicle?.RoomLayer ?? RoomLayer.GroundLevel,
			VehicleHitchEndpointType.Character => SourceCharacter?.RoomLayer ?? RoomLayer.GroundLevel,
			_ => RoomLayer.GroundLevel
		};
	}

	private RoomLayer TargetRoomLayer()
	{
		return TargetType switch
		{
			VehicleHitchEndpointType.Vehicle => TargetVehicle?.RoomLayer ?? RoomLayer.GroundLevel,
			VehicleHitchEndpointType.Character => TargetCharacter?.RoomLayer ?? RoomLayer.GroundLevel,
			_ => RoomLayer.GroundLevel
		};
	}

	private bool HitchItemIsWithChain(IGameItem item, ICell sourceLocation, RoomLayer sourceLayer)
	{
		if (item.Location == sourceLocation && item.RoomLayer == sourceLayer && item.ContainedIn is null &&
		    item.InInventoryOf is null)
		{
			return true;
		}

		if (SourceCharacter?.Body.ExternalItems.Any(x => x.Id == item.Id) == true ||
		    TargetCharacter?.Body.ExternalItems.Any(x => x.Id == item.Id) == true)
		{
			return true;
		}

		return false;
	}
}

public class VehicleDamageZone : FrameworkItem, IVehicleDamageZone
{
	private readonly List<IWound> _wounds = new();
	private double _currentDamage;
	private VehicleSystemStatus _status;

	public VehicleDamageZone(IVehicle vehicle, DB.VehicleDamageZone dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_currentDamage = dbitem.CurrentDamage;
		_status = (VehicleSystemStatus)dbitem.Status;
		Prototype = vehicle.Prototype.DamageZones.FirstOrDefault(x => x.Id == dbitem.VehicleDamageZoneProtoId)!;

		if (vehicle.ExteriorItem is not null)
		{
			foreach (var wound in dbitem.Wounds)
			{
				_wounds.Add(new SimpleWound(vehicle.ExteriorItem, wound, vehicle.Gameworld));
			}
		}
	}

	public override string FrameworkItemType => "VehicleDamageZone";
	public IVehicle Vehicle { get; }
	public IVehicleDamageZonePrototype Prototype { get; }
	public double CurrentDamage => _currentDamage;
	public VehicleSystemStatus Status => _status;
	public IEnumerable<IWound> Wounds => _wounds;

	public void AddWound(IWound wound)
	{
		if (!_wounds.Contains(wound))
		{
			_wounds.Add(wound);
		}
	}

	public void AddDamage(double amount)
	{
		_currentDamage = Math.Max(0.0, _currentDamage + amount);
		if (_currentDamage >= Prototype.DestroyedThreshold)
		{
			_status = VehicleSystemStatus.Destroyed;
		}
		else if (_currentDamage >= Prototype.DisabledThreshold)
		{
			_status = VehicleSystemStatus.Disabled;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleDamageZones.Find(Id);
			if (dbitem is not null)
			{
				dbitem.CurrentDamage = _currentDamage;
				dbitem.Status = (int)_status;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void SetStatus(VehicleSystemStatus status)
	{
		if (_status == status)
		{
			return;
		}

		_status = status;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleDamageZones.Find(Id);
			if (dbitem is not null)
			{
				dbitem.Status = (int)_status;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void ClearWoundsAndDamage()
	{
		foreach (var wound in _wounds.ToList())
		{
			wound.Delete();
		}

		_wounds.Clear();
		_currentDamage = 0.0;
		_status = VehicleSystemStatus.Functional;
		using (new FMDB())
		{
			var persistedWounds = FMDB.Context.Wounds.Where(x => x.VehicleDamageZoneId == Id).ToList();
			if (persistedWounds.Any())
			{
				FMDB.Context.Wounds.RemoveRange(persistedWounds);
			}

			var dbitem = FMDB.Context.VehicleDamageZones.Find(Id);
			if (dbitem is not null)
			{
				dbitem.CurrentDamage = 0.0;
				dbitem.Status = (int)_status;
			}

			FMDB.Context.SaveChanges();
		}
	}
}
