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

public class HandheldRadioGameItemComponent : GameItemComponent, ITransmit, IReceive, IConsumePower, ISwitchable,
	IOnOff
{
	protected HandheldRadioGameItemComponentProto _prototype;

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
		if (origin == this)
		{
			return;
		}

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
			foreach (var wear in Parent.InInventoryOf?.WornItemsFullInfo.Where(x => x.Item1 == Parent) ?? [])
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

	public override IGameItemComponentProto Prototype => _prototype;

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
					$"It is currently switched on, set to the {Volume.Describe().Colour(Telnet.Green)} volume, and on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}
			else if (SwitchedOn)
			{
				sb.AppendLine(
					$"It is currently switched on but not powered, set to the {Volume.Describe().Colour(Telnet.Green)} volume, and on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}
			else
			{
				sb.AppendLine(
					$"It is currently switched off, set to the {Volume.Describe().Colour(Telnet.Green)} volume, and on channel {_prototype.ChannelNames[_prototype.Channels.IndexOf(Frequency)]} ({Frequency:N3}MHz).");
			}

			return sb.ToString();
		}

		return description;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (HandheldRadioGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Frequency", Frequency),
			new XElement("Volume", (int)Volume),
			new XElement("SwitchedOn", SwitchedOn)
		).ToString();
	}

	#endregion

	#region IConsumePower Implementation

	private bool _powered;

	protected bool TransmittedThisTick
	{
		get
		{
			var value = _transmittedThisTick;
			_transmittedThisTick = false;
			return value;
		}
	}

	private bool _transmittedThisTick;

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
				? Math.Max(TransmittedThisTick ? _prototype.WattageTransmit : _prototype.WattageIdle,
					ReceivedThisTick ? _prototype.WattageReceive : _prototype.WattageIdle)
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

	private AudioVolume Volume { get; set; } = AudioVolume.Decent;

	public bool CanSwitch(ICharacter actor, string setting)
	{
		switch (setting.ToLowerInvariant())
		{
			case "on":
				return !SwitchedOn;
			case "off":
				return SwitchedOn;
			case "silent":
				return Volume != AudioVolume.Silent;
			case "quiet":
				return Volume != AudioVolume.Quiet;
			case "normal":
				return Volume != AudioVolume.Decent;
			case "loud":
				return Volume != AudioVolume.Loud;
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
			case "silent":
				return "You cannot switch that to silent because it is already on silent.";
			case "quiet":
				return "You cannot switch that to quiet because it is already on quiet.";
			case "normal":
				return "You cannot switch that to normal volume because it is already on normal volume.";
			case "loud":
				return "You cannot switch that to loud because it is already on loud.";
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
			case "silent":
				return SwitchVolume(actor, AudioVolume.Silent);
			case "quiet":
				return SwitchVolume(actor, AudioVolume.Quiet);
			case "normal":
				return SwitchVolume(actor, AudioVolume.Decent);
			case "loud":
				return SwitchVolume(actor, AudioVolume.Loud);
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

	private bool SwitchVolume(ICharacter actor, AudioVolume volume)
	{
		Volume = volume;
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
		=> new[] { "on", "off", "silent", "quiet", "normal", "loud", "channelup", "channeldown" };

	#endregion

	#region ITransmit Implementation

	public double Frequency { get; set; }
	public bool ManualTransmit => true;
	public string TransmitPremote => _prototype.TransmitPremote;

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!SwitchedOn || !_powered || !Parent.GetItemType<IProducePower>().ProducingPower)
		{
			return;
		}

		var location = Parent.TrueLocations.FirstOrDefault();
		if (location == null)
		{
#if DEBUG
			Console.WriteLine("Transmit had no location.");
#endif
			return;
		}

		var zones =
			Gameworld.Zones.Where(x => x.Shard == location.Shard)
			         .Where(x => x.Geography.DistanceTo(location.Zone.Geography) < _prototype.BroadcastRange)
			         .ToList();

		foreach (
			var item in
			Gameworld.Items
			         .SelectNotNull(x => x.GetItemType<IReceive>())
			         .Where(x => x.Parent.TrueLocations.Any(y => y != null && zones.Contains(y.Room.Zone)))
		)
		{
			item.ReceiveTransmission(Frequency, spokenLanguage, 0, this);
		}

		_transmittedThisTick = true;
	}

	#endregion

	#region Constructors

	public HandheldRadioGameItemComponent(HandheldRadioGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		Frequency = _prototype.Channels.First();
		Volume = AudioVolume.Decent;
	}

	public HandheldRadioGameItemComponent(MudSharp.Models.GameItemComponent component,
		HandheldRadioGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public HandheldRadioGameItemComponent(HandheldRadioGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		Frequency = double.Parse(root.Element("Frequency").Value);
		Volume = (AudioVolume)int.Parse(root.Element("Volume").Value);
		_switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new HandheldRadioGameItemComponent(this, newParent, temporary);
	}

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