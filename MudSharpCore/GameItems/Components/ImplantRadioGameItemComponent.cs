using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantRadioGameItemComponent : ImplantBaseGameItemComponent, IImplantReportStatus,
	IImplantRespondToCommands, ITransmit, IReceive
{
	protected ImplantRadioGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantRadioGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantRadioGameItemComponent(ImplantRadioGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ImplantRadioGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantRadioGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantRadioGameItemComponent(ImplantRadioGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
		_aliasForCommands = root.Element("AliasForCommands").Value;
		Encryption = long.Parse(root.Element("Encryption").Value);
		foreach (var freq in root.Element("Listening").Elements())
		{
			_listeningFrequencies.Add(double.Parse(freq.Value));
		}

		foreach (var freq in root.Element("Broadcasting").Elements())
		{
			_broadcastingFrequencies.Add(double.Parse(freq.Value));
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantRadioGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var definition = SaveToXmlNoTextConversion();
		definition.Add(new XElement("OverridenBodypart", OverridenBodypart?.Id ?? 0),
			new XElement("SwitchedOn", SwitchedOn),
			new XElement("AliasForCommands", new XCData(AliasForCommands ?? string.Empty)),
			new XElement("Encryption", Encryption),
			new XElement("Listening", from freq in _listeningFrequencies select new XElement("Frequency", freq)),
			new XElement("Broadcasting", from freq in _broadcastingFrequencies select new XElement("Frequency", freq)));
		return definition.ToString();
	}

	#endregion

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

	public override double PowerConsumptionInWatts
		=>
			SwitchedOn
				? Math.Max(TransmittedThisTick ? _prototype.WattageTransmit : _prototype.WattageIdle,
					ReceivedThisTick ? _prototype.WattageReceive : _prototype.WattageIdle)
				: 0.0;

	public long Encryption
	{
		get => _encryption;
		set
		{
			_encryption = value;
			Changed = true;
		}
	}

	private readonly List<double> _listeningFrequencies = new();
	private readonly List<double> _broadcastingFrequencies = new();

	public string ReportStatus()
	{
		if (!_powered)
		{
			return "\t* Implant is unpowered and non-functional.";
		}

		return
			$"* It is an internal radio, and it is {(SwitchedOn ? "on" : "off").ColourValue()}.\n* It is listening to the {(_listeningFrequencies.Count == 1 ? "frequency" : "frequencies")} {_listeningFrequencies.Select(x => _prototype.ChannelNames[_prototype.Channels.IndexOf(x)].ColourValue()).ListToString()}\n* It is broadcasting on the {(_broadcastingFrequencies.Count == 1 ? "frequency" : "frequencies")} {_broadcastingFrequencies.Select(x => _prototype.ChannelNames[_prototype.Channels.IndexOf(x)].ColourValue()).ListToString()}\n* It {(Encryption == 0 ? "is not using any encryption" : $"is using an encryption key of {Encryption.ToString("N0").ColourValue()}")}.";
	}

	public string AliasForCommands
	{
		get => _aliasForCommands;
		set
		{
			_aliasForCommands = value;
			Changed = true;
		}
	}

	public IEnumerable<string> Commands => new[] { "transmit", "listen", "broadcast", "on", "off", "encryption" };

	public string CommandHelp =>
		$"You can use the following options:\n\ton - switches the radio on\n\toff - switches the radio off\n\tlisten <channel> - toggles listening on a particular channel\n\tbroadcast <channel> - toggles broadcasting on a particular channel\n\tencryption <number> - set the radio to be transmitting and receiving with particular encryption. Use 0 for no encryption. Others should set their encryption to the same value.\n\n{Telnet.BoldWhite.Colour}Note: You can listen and broadcast on multiple channels simultaneously.{Telnet.RESETALL}";

	public void IssueCommand(string command, StringStack arguments)
	{
		var whichCommand = Commands.FirstOrDefault(x => x.EqualTo(command)) ??
		                   Commands.FirstOrDefault(x =>
			                   x.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
		switch (whichCommand)
		{
			case "on":
				CommandOn(arguments);
				break;
			case "off":
				CommandOff(arguments);
				break;
			case "listen":
				CommandListen(arguments);
				break;
			case "transmit":
				CommandTransmit(arguments);
				break;
			case "broadcast":
				CommandBroadcast(arguments);
				break;
			case "encryption":
				CommandEncryption(arguments);
				break;
		}
	}

	#region Subcommands

	private void CommandOn(StringStack arguments)
	{
		if (!_powered)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is already switched on.");
			return;
		}

		SwitchedOn = true;
		InstalledBody.Actor.OutputHandler.Send(
			$"You issue the 'on' command to {Parent.HowSeen(InstalledBody.Actor)} and it switches on.");
	}

	private void CommandOff(StringStack arguments)
	{
		if (!_powered)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is already switched off.");
			return;
		}

		SwitchedOn = false;
		InstalledBody.Actor.OutputHandler.Send(
			$"You issue the 'off' command to {Parent.HowSeen(InstalledBody.Actor)} and it switches off.");
	}

	private void CommandListen(StringStack arguments)
	{
		if (!_powered)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is switched off, so the only command you can issue is 'on'.");
			return;
		}

		if (arguments.IsFinished)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"Which channel do you want to toggle listening to via {Parent.HowSeen(InstalledBody.Actor)}?\nThe options are {_prototype.ChannelNames.Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var addedChannels = new List<string>();
		var removedChannels = new List<string>();

		string argument;
		do
		{
			argument = arguments.PopSpeech();
			var index = _prototype.ChannelNames.FindIndex(x => x.EqualTo(argument));
			if (index == -1)
			{
				index = _prototype.ChannelNames.FindIndex(x =>
					x.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
			}

			if (index == -1)
			{
				InstalledBody.Actor.OutputHandler.Send(
					$"'{argument}' is not a valid channel choice for {Parent.HowSeen(InstalledBody.Actor)}.\nThe options are {_prototype.ChannelNames.Select(x => x.ColourValue()).ListToString()}");
				return;
			}

			var name = _prototype.ChannelNames[index];
			var freq = _prototype.Channels[index];
			if (_listeningFrequencies.Contains(freq))
			{
				removedChannels.Add(name);
			}
			else
			{
				addedChannels.Add(name);
			}
		} while (!arguments.IsFinished);

		if (addedChannels.Any() && removedChannels.Any())
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You start listening on {addedChannels.Select(x => x.ColourValue()).ListToString()} and stop listening on {removedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}
		else if (addedChannels.Any())
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You start listening on {addedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}
		else
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You stop listening on {removedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}

		foreach (var channel in addedChannels)
		{
			_listeningFrequencies.Add(_prototype.Channels[_prototype.ChannelNames.IndexOf(channel)]);
		}

		foreach (var channel in removedChannels)
		{
			_listeningFrequencies.Remove(_prototype.Channels[_prototype.ChannelNames.IndexOf(channel)]);
		}

		Changed = true;
	}

	private void CommandTransmit(StringStack arguments)
	{
		var actor = InstalledBody.Actor;
		if (!_powered)
		{
			actor.OutputHandler.Send($"{Parent.HowSeen(actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			actor.OutputHandler.Send(
				$"{Parent.HowSeen(actor, true)} is switched off, so the only command you can issue is 'on'.");
			return;
		}

		if (!_broadcastingFrequencies.Any())
		{
			actor.OutputHandler.Send("You are not broadcasting on any frequencies.");
			return;
		}

		if (actor.CurrentLanguage == null)
		{
			actor.OutputHandler.Send("You must first set a speaking language before you can transmit anything.");
			return;
		}

		if (arguments.IsFinished)
		{
			actor.OutputHandler.Send("What is it that you want to transmit?");
			return;
		}

		if (arguments.RemainingArgument.Length > 350)
		{
			actor.Send("That is far too much to say at any one time. Keep it under 350 characters.");
			return;
		}

		var langInfo = new SpokenLanguageInfo(actor.CurrentLanguage, actor.CurrentAccent, Form.Audio.AudioVolume.Decent,
			arguments.RemainingArgument,
			Gameworld.GetCheck(RPG.Checks.CheckType.SpokenLanguageSpeakCheck).Check(actor, RPG.Checks.Difficulty.Easy,
				actor.CurrentLanguage.LinkedTrait), actor, null);
		actor.OutputHandler.Send(new LanguageOutput(
			new Emote("You issue a command to $0 through your neural interface and broadcast", actor, Parent), langInfo,
			null));
		Transmit(langInfo);
	}

	private void CommandBroadcast(StringStack arguments)
	{
		if (!_powered)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is switched off, so the only command you can issue is 'on'.");
			return;
		}

		if (arguments.IsFinished)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"Which channel do you want to toggle broadcasting on via {Parent.HowSeen(InstalledBody.Actor)}?\nThe options are {_prototype.ChannelNames.Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var addedChannels = new List<string>();
		var removedChannels = new List<string>();

		string argument;
		do
		{
			argument = arguments.PopSpeech();
			var index = _prototype.ChannelNames.FindIndex(x => x.EqualTo(argument));
			if (index == -1)
			{
				index = _prototype.ChannelNames.FindIndex(x =>
					x.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
			}

			if (index == -1)
			{
				InstalledBody.Actor.OutputHandler.Send(
					$"'{argument}' is not a valid channel choice for {Parent.HowSeen(InstalledBody.Actor)}.\nThe options are {_prototype.ChannelNames.Select(x => x.ColourValue()).ListToString()}");
				return;
			}

			var name = _prototype.ChannelNames[index];
			var freq = _prototype.Channels[index];
			if (_broadcastingFrequencies.Contains(freq))
			{
				removedChannels.Add(name);
			}
			else
			{
				addedChannels.Add(name);
			}
		} while (!arguments.IsFinished);

		if (addedChannels.Any() && removedChannels.Any())
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You start broadcasting on {addedChannels.Select(x => x.ColourValue()).ListToString()} and stop broadcasting on {removedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}
		else if (addedChannels.Any())
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You start broadcasting on {addedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}
		else
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You stop broadcasting on {removedChannels.Select(x => x.ColourValue()).ListToString()}.");
		}

		foreach (var channel in addedChannels)
		{
			_broadcastingFrequencies.Add(_prototype.Channels[_prototype.ChannelNames.IndexOf(channel)]);
		}

		foreach (var channel in removedChannels)
		{
			_broadcastingFrequencies.Remove(_prototype.Channels[_prototype.ChannelNames.IndexOf(channel)]);
		}

		Changed = true;
	}

	private void CommandEncryption(StringStack arguments)
	{
		if (!_powered)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} is switched off, so the only command you can issue is 'on'.");
			return;
		}

		if (arguments.IsFinished)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"You can either specify 0 or none to disable encryption, or specify a number as an encryption key.");
			return;
		}

		if (!long.TryParse(arguments.PopSpeech(), out var value))
		{
			if (!arguments.Last.EqualTo("none"))
			{
				InstalledBody.Actor.OutputHandler.Send(
					"You can either specify 0 or none to disable encryption, or specify a number as an encryption key.");
				return;
			}

			value = 0;
		}

		Encryption = value;
		if (value == 0)
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} will broadcast and receive without any encryption.");
		}
		else
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"{Parent.HowSeen(InstalledBody.Actor, true)} will broadcast and receive with an encryption key of {value.ToString(InstalledBody.Actor).ColourValue()}.");
		}
	}

	#endregion

	public bool ManualTransmit => true;
	private string _aliasForCommands;
	private long _encryption;

	public string TransmitPremote => string.Empty;

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!SwitchedOn || !_powered)
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
			Gameworld.Items.Where(x => x.TrueLocations.Any(y => y != null && zones.Contains(y.Room.Zone)))
			         .SelectNotNull(x => x.GetItemType<IReceive>()))
		foreach (var freq in _broadcastingFrequencies)
		{
			item.ReceiveTransmission(freq, spokenLanguage, Encryption, this);
		}

		_transmittedThisTick = true;
	}

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

		if (InstalledBody == null)
		{
			return;
		}

		if (!SwitchedOn || !_powered)
		{
			return;
		}

		_receivedThisTick = true;

		if (InstalledBody.Implants.OfType<IImplantNeuralLink>()
		                 .All(x => !x.IsLinkedTo(this) || !x.PermitsAudio))
		{
			return;
		}

		if (_listeningFrequencies.All(x => Math.Abs(x - frequency) > 0.05))
		{
			return;
		}


		if (Encryption != encryption)
		{
			InstalledBody.Actor.OutputHandler.Send(new EmoteOutput(
				new Emote(
					$"You receive an internal transmission on the {_prototype.ChannelNames[_prototype.Channels.IndexOf(frequency)].ColourValue()} channel, but it was not using the same encryption key as you.",
					Parent, Parent), flags: OutputFlags.ElectronicOnly));
			return;
		}

		InstalledBody.Actor.OutputHandler.Send(new LanguageOutput(
			new Emote(
				$"You receive an internal transmission on the {_prototype.ChannelNames[_prototype.Channels.IndexOf(frequency)].ColourValue()} channel",
				Parent, Parent), spokenLanguage, null, flags: OutputFlags.ElectronicOnly));
	}
}