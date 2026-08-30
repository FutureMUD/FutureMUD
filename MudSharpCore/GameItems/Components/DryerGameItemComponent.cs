using MudSharp.Body;
using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class DryerGameItemComponent : ContainerGameItemComponent, IConsumePower, ISwitchable, IOnOff,
	IItemTimeRateModifier
{
	private DryerGameItemComponentProto DryerPrototype => (DryerGameItemComponentProto)_prototype;
	private bool _powered;
	private bool _switchedOn;

	public DryerGameItemComponent(DryerGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	public DryerGameItemComponent(MudSharp.Models.GameItemComponent component, DryerGameItemComponentProto proto,
		IGameItem parent) : base(component, proto, parent)
	{
		var root = XElement.Parse(component.Definition);
		_switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "false");
	}

	public DryerGameItemComponent(DryerGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_switchedOn = rhs._switchedOn;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new DryerGameItemComponent(this, newParent, temporary);

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("SwitchedOn", SwitchedOn));
		return root.ToString();
	}

	public double PowerConsumptionInWatts => SwitchedOn ? DryerPrototype.PowerUsageInWatts : 0.0;

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

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			if (_switchedOn == value)
			{
				return;
			}

			if (value && IsOpen)
			{
				return;
			}

			RebaseContents();
			_switchedOn = value;
			Changed = true;
			var source = Parent.GetItemType<IProducePower>();
			if (value)
			{
				source?.BeginDrawdown(this);
			}
			else
			{
				source?.EndDrawdown(this);
				_powered = false;
			}
			RebaseContents();
		}
	}

	public IEnumerable<string> SwitchSettings => ["on", "off"];
	public bool CanSwitch(ICharacter actor, string setting) => setting.EqualTo("off") ||
		(setting.EqualTo("on") && !IsOpen);
	public string WhyCannotSwitch(ICharacter actor, string setting) => IsOpen
		? $"You must close {Parent.HowSeen(actor)} before switching it on."
		: "That is not a valid switch setting.";

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			actor.Send(WhyCannotSwitch(actor, setting));
			return false;
		}

		SwitchedOn = setting.EqualTo("on");
		actor.Send($"You switch {Parent.HowSeen(actor)} {setting.ToLowerInvariant()}.");
		return true;
	}

	public override void Open()
	{
		if (SwitchedOn)
		{
			SwitchedOn = false;
		}
		base.Open();
	}

	public override void Login()
	{
		base.Login();
		if (SwitchedOn && !IsOpen)
		{
			Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		}
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

	public double? RateMultiplierFor(ItemTimeRateType type) => type == ItemTimeRateType.SurfaceLiquidDrying
		? _powered && SwitchedOn && !IsOpen ? DryerPrototype.DryingMultiplier : 1.0
		: null;

	private void RebaseContents()
	{
		foreach (var item in Parent.DeepItems.Where(x => x != Parent).ToList())
		{
			item.RebaseItemTimeRates();
		}
	}
}
