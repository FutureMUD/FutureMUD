using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class RadioDetonatorTransmitterGameItemComponent : GameItemComponent, IConsumePower, IOnOff, ISelectable,
	ISwitchable
{
	protected RadioDetonatorTransmitterGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RadioDetonatorTransmitterGameItemComponentProto)newProto;
	}

	#region Constructors

	public RadioDetonatorTransmitterGameItemComponent(RadioDetonatorTransmitterGameItemComponentProto proto,
		IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		_detonationSequence = string.Empty;
	}

	public RadioDetonatorTransmitterGameItemComponent(MudSharp.Models.GameItemComponent component,
		RadioDetonatorTransmitterGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public RadioDetonatorTransmitterGameItemComponent(RadioDetonatorTransmitterGameItemComponent rhs,
		IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_detonationSequence = rhs._detonationSequence;
		_switchedOn = rhs._switchedOn;
	}

	protected void LoadFromXml(XElement root)
	{
		_detonationSequence = root.Element("DetonationSequence").Value;
		_switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RadioDetonatorTransmitterGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("DetonationSequence", new XCData(DetonationSequence ?? string.Empty)),
			new XElement("SwitchedOn", SwitchedOn)).ToString();
	}

	#endregion

	private string _detonationSequence;

	public string DetonationSequence
	{
		get => _detonationSequence;
		set
		{
			_detonationSequence = value;
			Changed = true;
		}
	}

	#region IConsumePower Implementation

	private bool _broadcastedThisTick;

	public bool BroadcastedThisTick
	{
		get
		{
			var value = _broadcastedThisTick;
			_broadcastedThisTick = false;
			return value;
		}
	}

	public double PowerConsumptionInWatts => _powered && SwitchedOn
		? BroadcastedThisTick ? _prototype.PowerConsumptionOnBroadcast : _prototype.PowerConsumptionOnIdle
		: 0.0;

	private bool _powered;

	public void OnPowerCutIn()
	{
		_powered = true;
		if (SwitchedOn)
		{
			TurnOn();
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (SwitchedOn)
		{
			TurnOff();
		}
	}

	#endregion

	#region IOnOff Implementation

	private bool _switchedOn;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			if (value)
			{
				Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
			}
			else
			{
				Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
			}

			Changed = true;
		}
	}

	private void TurnOff()
	{
		Parent.Handle(new EmoteOutput(new Emote(_prototype.PowerOffEmote, Parent, Parent)), OutputRange.Local);
	}

	private void TurnOn()
	{
		Parent.Handle(new EmoteOutput(new Emote(_prototype.PowerOnEmote, Parent, Parent)), OutputRange.Local);
	}

	#endregion

	#region ISelectable Implementation

	public bool CanSelect(ICharacter character, string argument)
	{
		var ss = new StringStack(argument);
		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("detonate"))
		{
			return true;
		}

		if (!cmd.EqualTo("code"))
		{
			return false;
		}

		if (ss.IsFinished)
		{
			return false;
		}

		return true;
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		var ss = new StringStack(argument);
		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("detonate"))
		{
			DoDetonate(character, playerEmote, silent);
			return true;
		}

		if (!cmd.EqualTo("code"))
		{
			character.OutputHandler.Send(
				$"The only valid options to use with select for {Parent.HowSeen(character)} are {"code".ColourCommand()} and {"detonate".ColourCommand()}.");
			return false;
		}

		if (ss.IsFinished)
		{
			character.OutputHandler.Send("You must specify a text code to set for this radio detonator.");
			return false;
		}

		DetonationSequence = ss.SafeRemainingArgument;
		character.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
			$"@ select|selects the 'code' option on $1 and types in {DetonationSequence.ColourCommand()} as the new code",
			character, character, Parent)));
		return true;
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
		return true;
	}

	private bool SwitchOff(ICharacter actor)
	{
		Changed = true;
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

	private void DoDetonate(ICharacter character, IEmote playerEmote, bool silent = false)
	{
		if (!silent)
		{
			character.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote(_prototype.DetonateCommandEmote, character, character, Parent)).Append(
					playerEmote));
		}

		var vicinity = character.Location.CellsInVicinity((uint)_prototype.DetonationRange, x => true, x => true)
		                        .ToList();
		var items = vicinity
		            .SelectMany(x => x.GameItems)
		            .SelectMany(x => x.DeepItems)
		            .Concat(vicinity.SelectMany(x => x.Characters.SelectMany(y => y.DeepContextualItems)))
		            .SelectNotNull(x => x.GetItemType<IReceive>())
		            .Distinct()
		            .ToList();
		foreach (var item in items)
		{
			item.ReceiveTransmission(0.0, DetonationSequence, 0L, null);
		}

		_broadcastedThisTick = true;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Short;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Full:
				return
					$"{description}\n\nThis item is a transmitter for radio detonators.\nIt is currently {(SwitchedOn && _powered ? "switched on".Colour(Telnet.Green) : "switched off".Colour(Telnet.Yellow))}.\nYou can {"switch".ColourCommand()} to switch it on or off (see {"help switch".FluentTagMXP("send", "href='help switch' hint='show the helpfile for the switch command'")}).\nYou can use {"select".ColourCommand()} to set the detonation code (see {"help select".FluentTagMXP("send", "href='help select' hint='show the helpfile for the select command'")}).\nYou can use {"select <this> detonate".ColourCommand()} to trigger the broadcast of the detonation code.";
			case DescriptionType.Short:
				if (_switchedOn && _powered)
				{
					return $"{description} {"(on)".Colour(Telnet.BoldWhite)}";
				}

				break;
		}

		return description;
	}


	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Quit();
	}

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Delete();
	}

	public override void Login()
	{
		base.Login();
		if (SwitchedOn)
		{
			Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		}
	}
}