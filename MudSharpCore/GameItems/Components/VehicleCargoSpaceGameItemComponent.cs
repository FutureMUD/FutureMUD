#nullable enable

using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class VehicleCargoSpaceGameItemComponent : GameItemComponent, IVehicleCargoSpaceItem, IContainer
{
	private VehicleCargoSpaceGameItemComponentProto _prototype;
	private long? _vehicleId;
	private long? _cargoSpaceId;
	private string _repairNote = string.Empty;

	public VehicleCargoSpaceGameItemComponent(VehicleCargoSpaceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VehicleCargoSpaceGameItemComponent(MudSharp.Models.GameItemComponent component,
		VehicleCargoSpaceGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public VehicleCargoSpaceGameItemComponent(VehicleCargoSpaceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_repairNote = "Copied vehicle cargo projection; no canonical cargo space is linked.";
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long? VehicleId => _vehicleId;
	public long? CargoSpaceId => _cargoSpaceId;

	public IVehicleCargoSpace? CargoSpace => _vehicleId is null || _cargoSpaceId is null
		? null
		: Gameworld.Vehicles.Get(_vehicleId.Value)?.CargoSpaces.FirstOrDefault(x => x.Id == _cargoSpaceId.Value);

	private IContainer? BackingContainer => Parent.Components
	                                             .OfType<IContainer>()
	                                             .FirstOrDefault(x => !ReferenceEquals(x, this));

	public IEnumerable<IGameItem> Contents => BackingContainer?.Contents ?? Enumerable.Empty<IGameItem>();
	public string ContentsPreposition => BackingContainer?.ContentsPreposition ?? "in";
	public bool Transparent => BackingContainer?.Transparent ?? false;

	public void LinkCargoSpace(IVehicleCargoSpace cargoSpace)
	{
		_vehicleId = cargoSpace?.Vehicle.Id;
		_cargoSpaceId = cargoSpace?.Id;
		_repairNote = string.Empty;
		Changed = true;
	}

	public void ClearCargoSpace(string reason)
	{
		_vehicleId = null;
		_cargoSpaceId = null;
		_repairNote = reason ?? string.Empty;
		Changed = true;
	}

	public bool CanPut(IGameItem item)
	{
		return CargoSpace?.CanAccess(null, out _) == true &&
		       BackingContainer?.CanPut(item) == true;
	}

	public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
	{
		BackingContainer?.Put(putter, item, allowMerge);
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (CargoSpace?.CanAccess(null, out _) != true)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

		return BackingContainer?.WhyCannotPut(item) ?? WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return CargoSpace?.CanAccess(taker, out _) == true &&
		       BackingContainer?.CanTake(taker, item, quantity) == true;
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		return BackingContainer!.Take(taker, item, quantity);
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (CargoSpace?.CanAccess(taker, out _) != true)
		{
			return WhyCannotGetContainerReason.ContainerClosed;
		}

		return BackingContainer?.WhyCannotTake(taker, item) ?? WhyCannotGetContainerReason.NotContainer;
	}

	public int CanPutAmount(IGameItem item)
	{
		return CargoSpace?.CanAccess(null, out _) == true
			? BackingContainer?.CanPutAmount(item) ?? 0
			: 0;
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
	{
		if (CargoSpace?.CanAccess(emptier, out _) != true)
		{
			emptier.OutputHandler.Send("You cannot access that cargo space right now.");
			return;
		}

		BackingContainer?.Empty(emptier, intoContainer, playerEmote);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VehicleCargoSpaceGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Evaluate or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var cargo = CargoSpace;
		if (type == DescriptionType.Evaluate)
		{
			return cargo is null
				? $"{description}\n\nThis is an unlinked vehicle cargo projection.".Colour(Telnet.Yellow)
				: $"{description}\n\nIt is a cargo space on {cargo.Vehicle.Name.ColourName(colour)}{(cargo.IsDisabled ? ", but it is disabled".Colour(Telnet.Red) : "")}.";
		}

		if (type == DescriptionType.Full && voyeur is ICharacter actor && actor.IsAdministrator())
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("Vehicle Cargo Projection:");
			sb.AppendLine($"\tVehicle: {(cargo?.Vehicle is null ? "None".ColourError() : $"{cargo.Vehicle.Name.ColourName()} (#{cargo.Vehicle.Id.ToString("N0", actor)})")}");
			sb.AppendLine($"\tCargo Space: {(cargo is null ? "None".ColourError() : $"{cargo.Name.ColourName()} (#{cargo.Id.ToString("N0", actor)})")}");
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
		_prototype = (VehicleCargoSpaceGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_vehicleId = long.TryParse(root.Element("VehicleId")?.Value, out var vehicleId) ? vehicleId : null;
		_cargoSpaceId = long.TryParse(root.Element("CargoSpaceId")?.Value, out var cargoId) ? cargoId : null;
		_repairNote = root.Element("RepairNote")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehicleId", _vehicleId),
			new XElement("CargoSpaceId", _cargoSpaceId),
			new XElement("RepairNote", _repairNote)
		).ToString();
	}
}
