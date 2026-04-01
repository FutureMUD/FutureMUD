#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public abstract class SwitchableThermalSourceGameItemComponent : ThermalSourceGameItemComponent, ISwitchable, IOnOff
{
	protected SwitchableThermalSourceGameItemComponent(SwitchableThermalSourceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		SwitchableThermalPrototype = proto;
	}

	protected SwitchableThermalSourceGameItemComponent(Models.GameItemComponent component,
		SwitchableThermalSourceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		SwitchableThermalPrototype = proto;
		_noSave = true;
		LoadSwitchableStateFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	protected SwitchableThermalSourceGameItemComponent(SwitchableThermalSourceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		SwitchableThermalPrototype = rhs.SwitchableThermalPrototype;
		_switchedOn = rhs._switchedOn;
	}

	protected sealed override ThermalSourceGameItemComponentProto ThermalPrototype => SwitchableThermalPrototype;
	protected SwitchableThermalSourceGameItemComponentProto SwitchableThermalPrototype { get; private set; }
	protected override bool IsProducingHeat => SwitchedOn && CanCurrentlyProduceHeat;
	protected abstract bool CanCurrentlyProduceHeat { get; }

	private bool _switchedOn;

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
			HandleSwitchStateChanged(value);
		}
	}

	protected void LoadSwitchableStateFromXml(XElement root)
	{
		_switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "false");
		LoadSwitchableStateFromXmlAdditional(root);
	}

	protected virtual void LoadSwitchableStateFromXmlAdditional(XElement root)
	{
	}

	protected string SaveSwitchableStateToXml(XElement root)
	{
		root.Add(new XElement("SwitchedOn", SwitchedOn));
		SaveSwitchableStateToXmlAdditional(root);
		return root.ToString();
	}

	protected virtual void SaveSwitchableStateToXmlAdditional(XElement root)
	{
	}

	protected virtual void HandleSwitchStateChanged(bool switchedOn)
	{
		if (switchedOn)
		{
			Parent.Handle(new EmoteOutput(new Emote(SwitchableThermalPrototype.SwitchOnEmote, Parent, Parent),
				flags: OutputFlags.SuppressObscured), OutputRange.Local);
		}
		else
		{
			Parent.Handle(new EmoteOutput(new Emote(SwitchableThermalPrototype.SwitchOffEmote, Parent, Parent),
				flags: OutputFlags.SuppressObscured), OutputRange.Local);
		}
	}

	public IEnumerable<string> SwitchSettings => ["on", "off"];

	public virtual bool CanSwitch(ICharacter actor, string setting)
	{
		return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) ? !SwitchedOn : SwitchedOn;
	}

	public virtual string WhyCannotSwitch(ICharacter actor, string setting)
	{
		return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)
			? $"{Parent.HowSeen(actor)} is already on."
			: $"{Parent.HowSeen(actor)} is already off.";
	}

	public virtual bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		SwitchedOn = setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase);
		return true;
	}
}
