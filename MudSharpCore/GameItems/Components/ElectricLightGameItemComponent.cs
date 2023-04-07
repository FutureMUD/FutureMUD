using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ElectricLightGameItemComponent : GameItemComponent, IProduceLight, IConsumePower, ISwitchable, IOnOff
{
	protected bool _lit;
	protected ElectricLightGameItemComponentProto _prototype;

	public void OnPowerCutIn()
	{
		if (SwitchedOn && !_lit)
		{
			_lit = true;
			Parent.Handle(new EmoteOutput(new Emote(_prototype.LightOnEmote, Parent, Parent),
				flags: OutputFlags.SuppressObscured), OutputRange.Local);
			_prototype.OnLightProg?.Execute(Parent);
		}
	}

	public void OnPowerCutOut()
	{
		if (SwitchedOn && _lit)
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.LightOffEmote, Parent, Parent),
				flags: OutputFlags.SuppressObscured), OutputRange.Local);
			_prototype.OnOffProg?.Execute(Parent);
			_lit = false;
		}
	}

	#region IOnOff Implementation

	private bool _switchedOn = true;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
			var power = Parent.GetItemType<IProducePower>();
			if (value)
			{
				power.BeginDrawdown(this);
			}
			else
			{
				power.EndDrawdown(this);
			}
		}
	}

	#endregion

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

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectricLightGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return $"{description}{(_lit ? " (lit)".FluentColour(Telnet.Red, colour) : "")}";
			case DescriptionType.Full:
				return
					$"{description}\n\nIt is{(_lit ? "" : " not")} currently lit.";
		}

		throw new NotSupportedException("Invalid Decorate type in ElectricLightGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return true;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ElectricLightGameItemComponentProto)newProto;
	}

	#region IProduceLight Members

	public double CurrentIllumination => _lit ? _prototype.IlluminationProvided : 0.0;

	public double PowerConsumptionInWatts => _prototype.Wattage;

	#endregion

	#region Constructors

	public ElectricLightGameItemComponent(ElectricLightGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public ElectricLightGameItemComponent(ElectricLightGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ElectricLightGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectricLightGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	#region Overrides of GameItemComponent

	public override void FinaliseLoad()
	{
	}

	#endregion

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("SwitchedOn", SwitchedOn)).ToString();
	}

	protected void LoadFromXml(XElement root)
	{
		_switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
	}

	#endregion

	#region ISwitchable Implementation

	public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

	public bool CanSwitch(ICharacter actor, string setting)
	{
		// TODO - more reasons why something couldn't be switched on or off
		return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
		       (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
			;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already on.";
		}

		if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already off.";
		}

		return $"{Parent.HowSeen(actor)} cannot be switched to {setting} at this time.";
	}

	private bool SwitchOn(ICharacter actor)
	{
		SwitchedOn = true;
		Changed = true;
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		return true;
	}

	private bool SwitchOff(ICharacter actor)
	{
		Changed = true;
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		SwitchedOn = false;
		return true;
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)
			? SwitchOn(actor)
			: SwitchOff(actor);
	}

	#endregion
}