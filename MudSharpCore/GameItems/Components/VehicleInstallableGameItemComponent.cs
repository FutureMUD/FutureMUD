#nullable enable

using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Vehicles;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class VehicleInstallableGameItemComponent : GameItemComponent, IVehicleInstallable
{
	private VehicleInstallableGameItemComponentProto _prototype;
	private long? _vehicleId;
	private long? _installationId;

	public VehicleInstallableGameItemComponent(VehicleInstallableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VehicleInstallableGameItemComponent(MudSharp.Models.GameItemComponent component,
		VehicleInstallableGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public VehicleInstallableGameItemComponent(VehicleInstallableGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string MountType => _prototype.MountType;
	public string Role => _prototype.Role;
	public bool IsInstalled => Installation is not null;

	public IVehicleInstallation? Installation => _vehicleId is null || _installationId is null
		? null
		: Gameworld.Vehicles.Get(_vehicleId.Value)?.Installations.FirstOrDefault(x => x.Id == _installationId.Value);

	public void LinkInstallation(IVehicleInstallation installation)
	{
		_vehicleId = installation?.Vehicle.Id;
		_installationId = installation?.Id;
		Changed = true;
	}

	public void ClearInstallation()
	{
		_vehicleId = null;
		_installationId = null;
		Changed = true;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VehicleInstallableGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Evaluate or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Evaluate)
		{
			return $"{description}\n\nIt is a {MountType.ColourCommand()} vehicle module{(string.IsNullOrWhiteSpace(Role) ? "" : $" for {Role.ColourCommand()}")}.";
		}

		if (type == DescriptionType.Full && Installation is not null)
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine($"It is installed in {Installation.Vehicle.Name.ColourName(colour)}.");
			return sb.ToString();
		}

		return description;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (VehicleInstallableGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_vehicleId = long.TryParse(root.Element("VehicleId")?.Value, out var vehicleId) ? vehicleId : null;
		_installationId = long.TryParse(root.Element("InstallationId")?.Value, out var installationId) ? installationId : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehicleId", _vehicleId),
			new XElement("InstallationId", _installationId)
		).ToString();
	}
}
