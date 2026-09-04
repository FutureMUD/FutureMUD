using MudSharp.Body;
using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class RefrigeratorGameItemComponent : ContainerGameItemComponent, IConsumePower, IItemTimeRateModifier
{
	private RefrigeratorGameItemComponentProto RefrigeratorPrototype =>
		(RefrigeratorGameItemComponentProto)_prototype;
	private bool _powered;

	public RefrigeratorGameItemComponent(RefrigeratorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
	}

	public RefrigeratorGameItemComponent(MudSharp.Models.GameItemComponent component,
		RefrigeratorGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
	}

	public RefrigeratorGameItemComponent(RefrigeratorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_powered = false;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RefrigeratorGameItemComponent(this, newParent, temporary);
	}

	public double PowerConsumptionInWatts => RefrigeratorPrototype.PowerUsageInWatts;

	public void OnPowerCutIn()
	{
		RebaseContents();
		_powered = true;
		RebaseContents();
	}

	public void OnPowerCutOut()
	{
		RebaseContents();
		_powered = false;
		RebaseContents();
	}

	public override void Login()
	{
		base.Login();
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}

	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		_powered = false;
		base.Quit();
	}

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		_powered = false;
		base.Delete();
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
		if (type is not (ItemTimeRateType.PreparedFoodFreshness or ItemTimeRateType.BiologicalDecay or
		    ItemTimeRateType.Morph or ItemTimeRateType.LiquidFreshness))
		{
			return null;
		}

		return ItemTimeRateMath.RefrigerationRate(_powered, IsOpen,
			RefrigeratorPrototype.PoweredClosedRate, RefrigeratorPrototype.PoweredOpenRate,
			RefrigeratorPrototype.UnpoweredClosedRate, RefrigeratorPrototype.UnpoweredOpenRate);
	}

	protected void RebaseContents()
	{
		foreach (var item in Parent.DeepItems.Where(x => x != Parent).ToList())
		{
			item.RebaseItemTimeRates();
		}
	}
}
