#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class VehicleAccessPointGameItemComponent : GameItemComponent, IVehicleAccessPointItem, IOpenable, ILockable
{
	private VehicleAccessPointGameItemComponentProto _prototype;
	private long? _vehicleId;
	private long? _accessPointId;
	private string _repairNote = string.Empty;

	public VehicleAccessPointGameItemComponent(VehicleAccessPointGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VehicleAccessPointGameItemComponent(MudSharp.Models.GameItemComponent component,
		VehicleAccessPointGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public VehicleAccessPointGameItemComponent(VehicleAccessPointGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_repairNote = "Copied vehicle access projection; no canonical access point is linked.";
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long? VehicleId => _vehicleId;
	public long? AccessPointId => _accessPointId;

	public IVehicleAccessPoint? AccessPoint => _vehicleId is null || _accessPointId is null
		? null
		: Gameworld.Vehicles.Get(_vehicleId.Value)?.AccessPoints.FirstOrDefault(x => x.Id == _accessPointId.Value);

	public IEnumerable<ILock> Locks => AccessPoint?.IsDisabled == true
		? Enumerable.Empty<ILock>()
		: AccessPoint?.Locks ?? Enumerable.Empty<ILock>();
	public bool IsOpen => AccessPoint?.IsOpen ?? false;

	public event OpenableEvent? OnOpen;
	public event OpenableEvent? OnClose;

	public void LinkAccessPoint(IVehicleAccessPoint accessPoint)
	{
		_vehicleId = accessPoint?.Vehicle.Id;
		_accessPointId = accessPoint?.Id;
		_repairNote = string.Empty;
		Changed = true;
	}

	public void ClearAccessPoint(string reason)
	{
		_vehicleId = null;
		_accessPointId = null;
		_repairNote = reason ?? string.Empty;
		Changed = true;
	}

	public bool CanOpen(IBody opener)
	{
		var access = AccessPoint;
		return access is not null && !access.IsDisabled && !access.IsOpen && !access.IsLocked;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		var access = AccessPoint;
		if (access is null || access.IsDisabled)
		{
			return WhyCannotOpenReason.NotOpenable;
		}

		if (access.IsOpen)
		{
			return WhyCannotOpenReason.AlreadyOpen;
		}

		return access.IsLocked ? WhyCannotOpenReason.Locked : WhyCannotOpenReason.Unknown;
	}

	public void Open()
	{
		AccessPoint?.SetOpen(true);
		OnOpen?.Invoke(this);
	}

	public bool CanClose(IBody closer)
	{
		var access = AccessPoint;
		return access is not null && !access.IsDisabled && access.IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		var access = AccessPoint;
		if (access is null || access.IsDisabled)
		{
			return WhyCannotCloseReason.NotOpenable;
		}

		return access.IsOpen ? WhyCannotCloseReason.Unknown : WhyCannotCloseReason.AlreadyClosed;
	}

	public void Close()
	{
		AccessPoint?.SetOpen(false);
		OnClose?.Invoke(this);
	}

	public bool InstallLock(ILock theLock, ICharacter? actor = null)
	{
		var access = AccessPoint;
		return access is not null && !access.IsDisabled && access.InstallLock(theLock, actor);
	}

	public bool RemoveLock(ILock theLock)
	{
		var access = AccessPoint;
		return access is not null && !access.IsDisabled && access.RemoveLock(theLock);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VehicleAccessPointGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Evaluate or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var access = AccessPoint;
		if (type == DescriptionType.Evaluate)
		{
			return access is null
				? $"{description}\n\nThis is an unlinked vehicle access projection.".Colour(Telnet.Yellow)
				: $"{description}\n\nIt is {(access.IsOpen ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Yellow))}{(access.IsLocked ? " and locked".Colour(Telnet.Red) : "")}{(access.IsDisabled ? " and disabled".Colour(Telnet.Red) : "")}.";
		}

		if (type == DescriptionType.Full && voyeur is ICharacter actor && actor.IsAdministrator())
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("Vehicle Access Projection:");
			sb.AppendLine($"\tVehicle: {(access?.Vehicle is null ? "None".ColourError() : $"{access.Vehicle.Name.ColourName()} (#{access.Vehicle.Id.ToString("N0", actor)})")}");
			sb.AppendLine($"\tAccess Point: {(access is null ? "None".ColourError() : $"{access.Name.ColourName()} (#{access.Id.ToString("N0", actor)})")}");
			if (!string.IsNullOrWhiteSpace(_repairNote))
			{
				sb.AppendLine($"\tRepair Note: {_repairNote.Colour(Telnet.Yellow)}");
			}

			return sb.ToString();
		}

		return description;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (VehicleAccessPointGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_vehicleId = long.TryParse(root.Element("VehicleId")?.Value, out var vehicleId) ? vehicleId : null;
		_accessPointId = long.TryParse(root.Element("AccessPointId")?.Value, out var accessId) ? accessId : null;
		_repairNote = root.Element("RepairNote")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehicleId", _vehicleId),
			new XElement("AccessPointId", _accessPointId),
			new XElement("RepairNote", _repairNote)
		).ToString();
	}
}
