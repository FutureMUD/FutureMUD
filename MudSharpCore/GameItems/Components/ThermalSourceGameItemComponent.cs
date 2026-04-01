#nullable enable
using System.Globalization;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public abstract class ThermalSourceGameItemComponent : GameItemComponent, IProduceHeat
{
	protected ThermalSourceGameItemComponent(IGameItem parent, IGameItemComponentProto proto, bool temporary = false)
		: base(parent, proto, temporary)
	{
	}

	protected ThermalSourceGameItemComponent(Models.GameItemComponent component, IGameItem parent) : base(component, parent)
	{
	}

	protected ThermalSourceGameItemComponent(ThermalSourceGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
	}

	protected abstract ThermalSourceGameItemComponentProto ThermalPrototype { get; }
	protected abstract bool IsProducingHeat { get; }

	public double CurrentAmbientHeat => IsProducingHeat ? ThermalPrototype.AmbientHeat : 0.0;
	public double CurrentHeat(Proximity proximity) => IsProducingHeat ? ThermalPrototype.HeatFor(proximity) : 0.0;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var stateText = IsProducingHeat ? ThermalPrototype.ActiveDescriptionAddendum : ThermalPrototype.InactiveDescriptionAddendum;
		return type switch
		{
			DescriptionType.Short => $"{description}{(IsProducingHeat ? " (active)".FluentColour(Telnet.Red, colour) : string.Empty)}",
			DescriptionType.Evaluate => $"{description}\n\nThermal Profile: {ThermalPrototype.ThermalProfileDisplay(voyeur)}",
			DescriptionType.Full => $"{description}\n\n{stateText}",
			_ => description
		};
	}
}
