#nullable enable

using MudSharp.Form.Audio;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ElectricEngineGameItemComponent : PoweredMachineBaseGameItemComponent, IVehicleEngine
{
	private ElectricEngineGameItemComponentProto _enginePrototype;

	public ElectricEngineGameItemComponent(ElectricEngineGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_enginePrototype = proto;
	}

	public ElectricEngineGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectricEngineGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_enginePrototype = proto;
	}

	private ElectricEngineGameItemComponent(ElectricEngineGameItemComponent rhs, IGameItem newParent,
		bool temporary) : base(rhs, newParent, temporary)
	{
		_enginePrototype = rhs._enginePrototype;
	}

	public override IGameItemComponentProto Prototype => _enginePrototype;
	public string FormFactor => _enginePrototype.FormFactor;
	public double MaximumPowerInWatts => _enginePrototype.MaximumPowerInWatts;
	public AudioVolume NoiseLevel => _enginePrototype.NoiseLevel;
	public bool IsRunning => SwitchedOn && IsPowered;
	public string WhyNotRunning => !SwitchedOn
		? "the engine is switched off"
		: !IsPowered
			? "the engine is not receiving enough electrical power"
			: string.Empty;
	protected override AudioVolume? PowerAudioVolume => NoiseLevel;

	public void EmitOperatingNoise()
	{
		if (IsRunning && NoiseLevel != AudioVolume.Silent)
		{
			Parent.Handle(new AudioOutput("@ whine|whines as it drives the vehicle.", NoiseLevel, Parent),
				OutputRange.Local);
		}
	}

	protected override void OnPowerCutInAction()
	{
	}

	protected override void OnPowerCutOutAction()
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectricEngineGameItemComponent(this, newParent, temporary);
	}

	protected override XElement SaveToXml(XElement root)
	{
		return root;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Short => $"{description}{(IsRunning ? " (running)".FluentColour(Telnet.BoldWhite, colour) : "")}",
			DescriptionType.Evaluate =>
				$"{description}\n\nIt is a {FormFactor.ColourCommand()} electric vehicle engine producing up to {MaximumPowerInWatts.ToString("N2", voyeur).ColourValue()}W. It is {(IsRunning ? "running".ColourValue() : WhyNotRunning.ColourError())}.",
			_ => description
		};
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_enginePrototype = (ElectricEngineGameItemComponentProto)newProto;
	}
}
