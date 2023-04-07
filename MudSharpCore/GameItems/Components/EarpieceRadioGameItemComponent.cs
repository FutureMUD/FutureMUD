using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class EarpieceRadioGameItemComponent : GameItemComponent, IReceive, IConsumePower, ISwitchable
{
	protected EarpieceRadioGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public double Frequency { get; set; }

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (EarpieceRadioGameItemComponentProto)newProto;
	}

	#region Constructors

	public EarpieceRadioGameItemComponent(EarpieceRadioGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		Frequency = _prototype.Channels.First();
		Volume = AudioVolume.Quiet;
	}

	public EarpieceRadioGameItemComponent(MudSharp.Models.GameItemComponent component,
		EarpieceRadioGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		Volume = AudioVolume.Quiet;
		_noSave = false;
	}

	public EarpieceRadioGameItemComponent(EarpieceRadioGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		Frequency = double.Parse(root.Element("Frequency").Value);
		_switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new EarpieceRadioGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Frequency", Frequency),
			new XElement("SwitchedOn", SwitchedOn)
		).ToString();
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
			Changed = true;
		}
	}

	#endregion

	#region IReceive Implementation

	public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
	{
		// Do nothing
	}

	public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption,
		ITransmit origin)
	{
		if (!SwitchedOn || !Parent.GetItemType<IProducePower>().ProducingPower)
		{
			return;
		}

		if (Math.Abs(Frequency - frequency) > 0.05)
		{
			return;
		}

		if (Volume == AudioVolume.Silent)
		{
			return;
		}

		// Radios don't echo to the room if they're being worn on a location with an ear organ and are set to quiet volume
		var inEars = false;
		var canHear = true;
		if (Volume == AudioVolume.Quiet)
		{
			var wornEars = new List<EarProto>();
			foreach (var wear in Parent.InInventoryOf?.WornItemsFullInfo.Where(x => x.Item1 == Parent) ??
			                     Enumerable.Empty<(IGameItem, IWear, IWearlocProfile)>())
			foreach (var organ in wear.Item2.Organs.OfType<EarProto>())
			{
				wornEars.Add(organ);
			}

			inEars = wornEars.Any();
			canHear = wornEars.Any(x => Parent.InInventoryOf.OrganFunction(x) > 0);
		}


		if (encryption != 0)
		{
			if (!inEars)
			{
				Parent.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ make|makes a horrible series of squeals, pops, whistles and static.",
						Parent, Parent), flags: OutputFlags.PurelyAudible));
			}
			else if (canHear)
			{
				Parent.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ make|makes a horrible series of squeals, pops, whistles and static.",
						Parent, Parent), flags: OutputFlags.PurelyAudible), OutputRange.Personal);
			}

			return;
		}

		var spoken = new SpokenLanguageInfo(spokenLanguage, Volume, Parent);
		if (!inEars)
		{
			Parent.OutputHandler.Handle(new LanguageOutput(new Emote("@ broadcast|broadcasts", Parent, Parent), spoken,
				null, flags: OutputFlags.PurelyAudible));
		}
		else if (canHear)
		{
			Parent.OutputHandler.Handle(new LanguageOutput(new Emote("@ broadcast|broadcasts", Parent, Parent), spoken,
				null, flags: OutputFlags.PurelyAudible), OutputRange.Personal);
		}

		_receivedThisTick = true;
	}

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			var sb = new StringBuilder();
			sb.Append(description);
			sb.AppendLine();
			if (SwitchedOn && _powered)
			{
				sb.AppendLine(
					$"It is currently switched on and on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}
			else if (SwitchedOn)
			{
				sb.AppendLine(
					$"It is currently switched on but not powered, on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}
			else
			{
				sb.AppendLine(
					$"It is currently switched off and on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}

			return sb.ToString();
		}

		return description;
	}

	#region IConsumePower Implementation

	private bool _powered;

	protected bool ReceivedThisTick
	{
		get
		{
			var value = _receivedThisTick;
			_receivedThisTick = false;
			return value;
		}
	}

	private bool _receivedThisTick;

	public double PowerConsumptionInWatts
		=>
			SwitchedOn
				? ReceivedThisTick ? _prototype.WattageReceive : _prototype.WattageIdle
				: 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
		if (SwitchedOn)
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.OnPowerOnEmote, Parent, Parent)), OutputRange.Local);
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (SwitchedOn)
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.OnPowerOffEmote, Parent, Parent)), OutputRange.Local);
		}
	}

	#endregion

	#region ISwitchable Implementation

	private AudioVolume Volume { get; set; } = AudioVolume.Quiet;

	public bool CanSwitch(ICharacter actor, string setting)
	{
		switch (setting.ToLowerInvariant())
		{
			case "on":
				return !SwitchedOn;
			case "off":
				return SwitchedOn;
			case "channelup":
			case "channeldown":
				return true;
			default:
				return false;
		}
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		switch (setting.ToLowerInvariant())
		{
			case "on":
				return "You cannot switch that on because it is already on.";
			case "off":
				return "You cannot switch that off because it is already off.";
			default:
				return "That is not a valid setting to switch.";
		}
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			actor.Send(WhyCannotSwitch(actor, setting));
			return false;
		}

		switch (setting.ToLowerInvariant())
		{
			case "on":
				return SwitchOn(actor);
			case "off":
				return SwitchOff(actor);
			case "channelup":
				return SwitchChannelUp(actor);
			case "channeldown":
				return SwitchChannelDown(actor);
			default:
				return false;
		}
	}

	private bool SwitchChannelDown(ICharacter actor)
	{
		Frequency = _prototype.Channels.First() == Frequency
			? _prototype.Channels.Last()
			: _prototype.Channels.ElementAt(_prototype.Channels.IndexOf(Frequency) - 1);

		actor.Send(
			$"You switch {Parent.HowSeen(actor)} to channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
		Changed = true;
		return true;
	}

	private bool SwitchChannelUp(ICharacter actor)
	{
		Frequency = _prototype.Channels.Last() == Frequency
			? _prototype.Channels.First()
			: _prototype.Channels.ElementAt(_prototype.Channels.IndexOf(Frequency) + 1);

		actor.Send(
			$"You switch {Parent.HowSeen(actor)} to channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
		Changed = true;
		return true;
	}

	private bool SwitchOff(ICharacter actor)
	{
		SwitchedOn = false;
		return true;
	}

	private bool SwitchOn(ICharacter actor)
	{
		SwitchedOn = true;
		return true;
	}

	public IEnumerable<string> SwitchSettings
		=> new[] { "on", "off", "channelup", "channeldown" };

	#endregion

	/// <summary>
	///     Handles any finalisation that this component needs to perform before being deleted.
	/// </summary>
	public override void Delete()
	{
		base.Delete();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
	}

	public override void Login()
	{
		base.Login();
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}

	/// <summary>
	///     Handles any finalisation that this component needs to perform before being removed from the game (e.g. in inventory
	///     when player quits)
	/// </summary>
	public override void Quit()
	{
		base.Quit();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
	}
}