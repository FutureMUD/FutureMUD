#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ToggleSwitchGameItemComponent : GameItemComponent, ISignalSourceComponent, ISwitchable, IOnOff
{
	private ToggleSwitchGameItemComponentProto _prototype;

	public ToggleSwitchGameItemComponent(ToggleSwitchGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		_switchedOn = proto.InitiallyOn;
	}

	public ToggleSwitchGameItemComponent(MudSharp.Models.GameItemComponent component,
		ToggleSwitchGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ToggleSwitchGameItemComponent(ToggleSwitchGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_switchedOn = rhs._switchedOn;
	}

	private bool _switchedOn;

	public override IGameItemComponentProto Prototype => _prototype;
	public ComputerSignal CurrentSignal => new(SwitchedOn ? _prototype.OnValue : _prototype.OffValue, null, null);
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => CurrentSignal.Value;
	public TimeSpan? Duration => null;
	public TimeSpan? PulseInterval => null;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ToggleSwitchGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Short;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Short => $"{description}{(SwitchedOn ? " (switched on)".FluentColour(Telnet.Green, colour) : "")}",
			DescriptionType.Full => $"{description}\n\nIts toggle switch is currently {(SwitchedOn ? "on".ColourValue() : "off".ColourName())}.",
			_ => description
		};
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ToggleSwitchGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SwitchedOn", SwitchedOn)
		).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("SwitchedOn");
		_switchedOn = element is not null
			? bool.Parse(element.Value)
			: _prototype.InitiallyOn;
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

			_switchedOn = value;
			Changed = true;
			SignalChanged?.Invoke(this, CurrentSignal);
		}
	}

	public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

	public bool CanSwitch(ICharacter actor, string setting)
	{
		return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
		       (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn);
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already switched on.";
		}

		if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already switched off.";
		}

		return $"{Parent.HowSeen(actor)} cannot be switched to {setting}.";
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		SwitchedOn = setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase);
		return true;
	}
}
