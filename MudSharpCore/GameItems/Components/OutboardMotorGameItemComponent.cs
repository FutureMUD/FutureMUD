#nullable enable

using MudSharp.GameItems.Prototypes;
using MudSharp.Vehicles;

namespace MudSharp.GameItems.Components;

public class OutboardMotorGameItemComponent : GameItemComponent, IOutboardMotor
{
	private OutboardMotorGameItemComponentProto _prototype;

	public OutboardMotorGameItemComponent(OutboardMotorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public OutboardMotorGameItemComponent(MudSharp.Models.GameItemComponent component,
		OutboardMotorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
	}

	private OutboardMotorGameItemComponent(OutboardMotorGameItemComponent rhs, IGameItem newParent, bool temporary)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public OutboardMotorEnergySource EnergySource => _prototype.EnergySource;
	public double OutputMultiplier => _prototype.OutputMultiplier;
	public long? FuelLiquidId => _prototype.FuelLiquid?.Id;
	public double FuelVolumePerMove => _prototype.FuelVolumePerMove;
	public double RequiredPowerSpikeInWatts => _prototype.RequiredPowerSpikeInWatts;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new OutboardMotorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return $"{description}\n\nIt is a {EnergySource.DescribeEnum().ColourName(colour)} outboard motor with {OutputMultiplier.ToString("N2", voyeur).ColourValue()} output.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (OutboardMotorGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
