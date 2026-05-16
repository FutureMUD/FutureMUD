using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class VehicleExteriorGameItemComponent : GameItemComponent, IVehicleExterior, IProvideItemTargetProjections,
	IOverrideItemWoundBehaviour
{
	private VehicleExteriorGameItemComponentProto _prototype;
	private long? _vehicleId;
	private string _repairNote = string.Empty;

	public VehicleExteriorGameItemComponent(VehicleExteriorGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VehicleExteriorGameItemComponent(MudSharp.Models.GameItemComponent component,
		VehicleExteriorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public VehicleExteriorGameItemComponent(VehicleExteriorGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_repairNote = "Copied vehicle exterior component; no canonical vehicle is linked.";
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long? VehicleId => _vehicleId;
	public IVehicle Vehicle => _vehicleId is null ? null : Gameworld.Vehicles.Get(_vehicleId.Value);
	public IEnumerable<IGameItem> TargetProjections => Vehicle?.ProjectedTargetItems ?? Enumerable.Empty<IGameItem>();
	public IHealthStrategy HealthStrategy => Parent.HealthStrategy;
	public IEnumerable<IWound> Wounds => Vehicle?.DamageZones.SelectMany(x => x.Wounds) ?? Enumerable.Empty<IWound>();

	public void LinkVehicle(IVehicle vehicle)
	{
		_vehicleId = vehicle?.Id;
		_repairNote = string.Empty;
		Changed = true;
	}

	public void ClearVehicleLink(string reason)
	{
		_vehicleId = null;
		_repairNote = reason ?? string.Empty;
		Changed = true;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VehicleExteriorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Evaluate or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Evaluate)
		{
			var vehicle = Vehicle;
			if (vehicle is null)
			{
				return $"{description}\n\nThis is an unlinked vehicle exterior.".Colour(Telnet.Yellow);
			}

			return $"{description}\n\nIt is the exterior of {vehicle.Name.ColourName(colour)}, a {vehicle.Prototype.Scale.DescribeEnum().ColourValue()} vehicle.";
		}

		if (type == DescriptionType.Full && voyeur is ICharacter actor && actor.IsAdministrator())
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("Vehicle Link:");
			sb.AppendLine($"\tVehicle: {(Vehicle is null ? "None".ColourError() : $"{Vehicle.Name.ColourName()} (#{Vehicle.Id.ToString("N0", actor)})")}");
			sb.AppendLine($"\tPrototype: {(_prototype.VehiclePrototype is null ? "None".ColourError() : $"{_prototype.VehiclePrototype.Name.ColourName()} (#{_prototype.VehiclePrototype.Id.ToString("N0", actor)})")}");
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
		_prototype = (VehicleExteriorGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_vehicleId = long.TryParse(root.Element("VehicleId")?.Value, out var id) ? id : null;
		_repairNote = root.Element("RepairNote")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehicleId", VehicleId),
			new XElement("RepairNote", _repairNote)
		).ToString();
	}

	public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
	{
		return examinationType switch
		{
			WoundExaminationType.Glance => Wounds.Where(x => x.Severity > WoundSeverity.Minor),
			WoundExaminationType.Look => Wounds.Where(x => x.Severity > WoundSeverity.Superficial),
			_ => Wounds
		};
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		return Vehicle?.SufferDamage(damage) ?? Enumerable.Empty<IWound>();
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
	{
		return Vehicle?.PassiveSufferDamage(damage) ?? Enumerable.Empty<IWound>();
	}

	public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
	{
		return Vehicle?.PassiveSufferDamage(damage, proximity, facing) ?? Enumerable.Empty<IWound>();
	}

	public void ProcessPassiveWound(IWound wound)
	{
		if (Vehicle is Vehicle vehicle)
		{
			vehicle.AddVehicleWound(wound);
		}
	}

	public WoundSeverity GetSeverityFor(IWound wound)
	{
		return HealthStrategy.GetSeverityFor(wound, Parent);
	}

	public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
	{
		return HealthStrategy.GetSeverityFloor(severity, usePercentageModel);
	}

	public void EvaluateWounds()
	{
		foreach (var wound in Wounds.Where(x => x.Severity == WoundSeverity.None).ToList())
		{
			wound.Delete();
		}
	}

	public void CureAllWounds()
	{
		if (Vehicle is Vehicle vehicle)
		{
			vehicle.CureAllVehicleWounds();
		}
	}

	public void StartHealthTick(bool initial = false)
	{
		// Vehicle wounds currently do not have autonomous healing ticks.
	}

	public void EndHealthTick()
	{
		// Vehicle wounds currently do not have autonomous healing ticks.
	}

	public void AddWound(IWound wound)
	{
		if (Vehicle is Vehicle vehicle)
		{
			vehicle.AddVehicleWound(wound);
		}
	}

	public void AddWounds(IEnumerable<IWound> wounds)
	{
		foreach (var wound in wounds)
		{
			AddWound(wound);
		}
	}

	public bool TryTransferWoundTo(IWound wound, IHaveWounds newOwner, IBodypart newBodypart,
		IBodypart newSeveredBodypart = null)
	{
		return false;
	}
}
