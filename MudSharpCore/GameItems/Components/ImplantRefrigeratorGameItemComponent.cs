using MudSharp.GameItems.Prototypes;
using MudSharp.Health;

#nullable enable

namespace MudSharp.GameItems.Components;

public class ImplantRefrigeratorGameItemComponent : ImplantContainerGameItemComponent, IItemTimeRateModifier
{
	private ImplantRefrigeratorGameItemComponentProto RefrigeratorPrototype =>
		(ImplantRefrigeratorGameItemComponentProto)_prototype;
	private double _resolvedFunctionFactor;

	public ImplantRefrigeratorGameItemComponent(ImplantRefrigeratorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		SubscribeToFunctionChanges();
	}

	public ImplantRefrigeratorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantRefrigeratorGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		SubscribeToFunctionChanges();
	}

	public ImplantRefrigeratorGameItemComponent(ImplantRefrigeratorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_resolvedFunctionFactor = rhs._resolvedFunctionFactor;
		SubscribeToFunctionChanges();
	}

	private void SubscribeToFunctionChanges()
	{
		Parent.OnWounded += ParentFunctionChanged;
		Parent.OnHeal += ParentFunctionChanged;
		Parent.OnRemoveWound += ParentFunctionChanged;
		_resolvedFunctionFactor = Math.Clamp(FunctionFactor, 0.0, 1.0);
	}

	private void ParentFunctionChanged(IMortalPerceiver wounded, IWound wound)
	{
		RebaseContents();
		_resolvedFunctionFactor = Math.Clamp(FunctionFactor, 0.0, 1.0);
		RebaseContents();
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new ImplantRefrigeratorGameItemComponent(this, newParent, temporary);

	public override void OnPowerCutIn()
	{
		RebaseContents();
		base.OnPowerCutIn();
		_resolvedFunctionFactor = Math.Clamp(FunctionFactor, 0.0, 1.0);
		RebaseContents();
	}

	public override void OnPowerCutOut()
	{
		RebaseContents();
		base.OnPowerCutOut();
		_resolvedFunctionFactor = 0.0;
		RebaseContents();
	}

	public override void Open()
	{
		RebaseContents();
		base.Open();
		RebaseContents();
	}

	public override void Close()
	{
		RebaseContents();
		base.Close();
		RebaseContents();
	}

	public double? RateMultiplierFor(ItemTimeRateType type)
	{
		if (type is not (ItemTimeRateType.PreparedFoodFreshness or ItemTimeRateType.BiologicalDecay or ItemTimeRateType.Morph))
		{
			return null;
		}

		var poweredRate = ItemTimeRateMath.RefrigerationRate(true, IsOpen,
			RefrigeratorPrototype.PoweredClosedRate, RefrigeratorPrototype.PoweredOpenRate,
			RefrigeratorPrototype.UnpoweredClosedRate, RefrigeratorPrototype.UnpoweredOpenRate);
		var unpoweredRate = ItemTimeRateMath.RefrigerationRate(false, IsOpen,
			RefrigeratorPrototype.PoweredClosedRate, RefrigeratorPrototype.PoweredOpenRate,
			RefrigeratorPrototype.UnpoweredClosedRate, RefrigeratorPrototype.UnpoweredOpenRate);
		return unpoweredRate + (poweredRate - unpoweredRate) * _resolvedFunctionFactor;
	}

	public override void Delete()
	{
		Parent.OnWounded -= ParentFunctionChanged;
		Parent.OnHeal -= ParentFunctionChanged;
		Parent.OnRemoveWound -= ParentFunctionChanged;
		base.Delete();
	}

	private void RebaseContents()
	{
		foreach (var item in Parent.DeepItems.Where(x => x != Parent).ToList())
		{
			item.RebaseItemTimeRates();
		}
	}
}
