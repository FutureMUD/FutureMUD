#nullable enable
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ElectricHeaterCoolerGameItemComponent : SwitchableThermalSourceGameItemComponent, IConsumePower
{
	public ElectricHeaterCoolerGameItemComponent(ElectricHeaterCoolerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ElectricHeaterCoolerGameItemComponent(Models.GameItemComponent component,
		ElectricHeaterCoolerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
	}

	public ElectricHeaterCoolerGameItemComponent(ElectricHeaterCoolerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_powered = rhs._powered;
	}

	private ElectricHeaterCoolerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	protected override bool CanCurrentlyProduceHeat => _powered;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectricHeaterCoolerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ElectricHeaterCoolerGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return SaveSwitchableStateToXml(new XElement("Definition"));
	}

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Delete();
	}

	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Quit();
	}

	public override void Login()
	{
		base.Login();
		if (SwitchedOn)
		{
			Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		}
	}

	private bool _powered;

	public double PowerConsumptionInWatts => SwitchedOn ? _prototype.Wattage : 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
		Changed = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		Changed = true;
	}

	protected override void HandleSwitchStateChanged(bool switchedOn)
	{
		base.HandleSwitchStateChanged(switchedOn);
		var producer = Parent.GetItemType<IProducePower>();
		if (switchedOn)
		{
			producer?.BeginDrawdown(this);
		}
		else
		{
			producer?.EndDrawdown(this);
		}
	}
}
