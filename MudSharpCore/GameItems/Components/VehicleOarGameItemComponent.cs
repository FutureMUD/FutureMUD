#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class VehicleOarGameItemComponent : GameItemComponent, IVehicleOar
{
	private VehicleOarGameItemComponentProto _prototype;

	public VehicleOarGameItemComponent(VehicleOarGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VehicleOarGameItemComponent(MudSharp.Models.GameItemComponent component,
		VehicleOarGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
	}

	private VehicleOarGameItemComponent(VehicleOarGameItemComponent rhs, IGameItem newParent, bool temporary)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public double EfficiencyMultiplier => _prototype.EfficiencyMultiplier;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VehicleOarGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return $"{description}\n\nIt can be used as a vehicle oar with {EfficiencyMultiplier.ToString("N2", voyeur).ColourValue()} efficiency.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (VehicleOarGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
