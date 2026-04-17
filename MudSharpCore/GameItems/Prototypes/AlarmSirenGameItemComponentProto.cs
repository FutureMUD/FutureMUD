#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class AlarmSirenGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3source <component>#0 - the signal source component prototype name or id whose default signal endpoint drives this alarm
	#3threshold <number>#0 - the numeric threshold used to determine when the alarm is active
	#3invert#0 - toggles whether the alarm activates above or below the threshold
	#3volume <volume>#0 - sets the alarm volume
	#3emote <emote>#0 - the audible emote shown whenever the alarm sounds. Use $0 for the item";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected AlarmSirenGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Alarm Siren")
	{
		SourceComponentId = 0L;
		SourceComponentName = string.Empty;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		ActivationThreshold = 0.5;
		SoundWhenAboveThreshold = true;
		AlarmVolume = AudioVolume.VeryLoud;
		AlarmEmote = "@ blare|blares with a piercing alarm tone";
	}

	protected AlarmSirenGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long SourceComponentId { get; protected set; }
	public string SourceComponentName { get; protected set; } = string.Empty;
	public string SourceEndpointKey { get; protected set; } = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public double ActivationThreshold { get; protected set; }
	public bool SoundWhenAboveThreshold { get; protected set; }
	public AudioVolume AlarmVolume { get; protected set; }
	public string AlarmEmote { get; protected set; } = string.Empty;
	public override string TypeDescription => "Alarm Siren";

	protected override string ComponentDescriptionOLCByline => "This item is a powered signal-driven alarm siren";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Source Endpoint: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()} (#{SourceComponentId.ToString("N0", actor)})\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(SoundWhenAboveThreshold ? "Sounds at or above threshold".ColourValue() : "Sounds below threshold".ColourValue())}\nVolume: {AlarmVolume.Describe().ColourValue()}\nAlarm Emote: {AlarmEmote.ColourCommand()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SourceComponentId = long.TryParse(root.Element("SourceComponentId")?.Value, out var sourceId) ? sourceId : 0L;
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("SourceEndpointKey")?.Value);
		ActivationThreshold = double.Parse(root.Element("ActivationThreshold")?.Value ?? "0.5");
		SoundWhenAboveThreshold = bool.Parse(root.Element("SoundWhenAboveThreshold")?.Value ?? "true");
		AlarmVolume = int.TryParse(root.Element("AlarmVolume")?.Value, out var volume) &&
		              Enum.IsDefined(typeof(AudioVolume), volume)
			? (AudioVolume)volume
			: AudioVolume.VeryLoud;
		AlarmEmote = root.Element("AlarmEmote")?.Value ?? "@ blare|blares with a piercing alarm tone";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("SourceComponentId", SourceComponentId));
		root.Add(new XElement("SourceComponentName", new XCData(SourceComponentName)));
		root.Add(new XElement("SourceEndpointKey", new XCData(SourceEndpointKey)));
		root.Add(new XElement("ActivationThreshold", ActivationThreshold));
		root.Add(new XElement("SoundWhenAboveThreshold", SoundWhenAboveThreshold));
		root.Add(new XElement("AlarmVolume", (int)AlarmVolume));
		root.Add(new XElement("AlarmEmote", new XCData(AlarmEmote)));
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "source":
				return BuildingCommandSource(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "invert":
				return BuildingCommandInvert(actor);
			case "volume":
			case "loudness":
				return BuildingCommandVolume(actor, command);
			case "emote":
			case "alarm":
				return BuildingCommandEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which signal source component prototype should drive this alarm siren?");
			return false;
		}

		if (!SignalComponentUtilities.TryResolveSignalComponentPrototype(Gameworld, command.SafeRemainingArgument.Trim(),
			    out var sourcePrototype) || sourcePrototype is null)
		{
			actor.Send("There is no such item component prototype.");
			return false;
		}

		SourceComponentId = sourcePrototype.Id;
		SourceComponentName = sourcePrototype.Name;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		Changed = true;
		actor.Send(
			$"This alarm siren is now driven by the {SourceEndpointKey.ColourCommand()} endpoint on signal source component prototype {SourceComponentName.ColourName()} (#{SourceComponentId.ToString("N0", actor)}).");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric threshold should determine when this alarm siren is active?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		ActivationThreshold = value;
		Changed = true;
		actor.Send($"This alarm siren now uses a threshold of {ActivationThreshold.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInvert(ICharacter actor)
	{
		SoundWhenAboveThreshold = !SoundWhenAboveThreshold;
		Changed = true;
		actor.Send(
			$"This alarm siren is now {(SoundWhenAboveThreshold ? "active above or equal to the threshold".ColourValue() : "active below the threshold".ColourValue())}.");
		return true;
	}

	private bool BuildingCommandVolume(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What audio volume should this alarm siren use?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<AudioVolume>(out var value))
		{
			actor.Send("That is not a valid audio volume.");
			return false;
		}

		AlarmVolume = value;
		Changed = true;
		actor.Send($"This alarm siren will now sound at {AlarmVolume.Describe().ColourValue()} volume.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when this alarm siren sounds? Use @ for the item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		AlarmEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This alarm siren now uses the emote {AlarmEmote.ColourCommand()} when it sounds.");
		return true;
	}

	public override bool CanSubmit()
	{
		return (SourceComponentId > 0 || !string.IsNullOrWhiteSpace(SourceComponentName)) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SourceComponentId <= 0 && string.IsNullOrWhiteSpace(SourceComponentName))
		{
			return "You must specify a signal source component prototype for this alarm siren.";
		}

		return base.WhyCannotSubmit();
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("alarmsiren", true,
			(gameworld, account) => new AlarmSirenGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("alarm siren", false,
			(gameworld, account) => new AlarmSirenGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Alarm Siren",
			(proto, gameworld) => new AlarmSirenGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"AlarmSiren",
			$"A {"[powered]".Colour(Telnet.Magenta)} audible alarm driven by a sibling signal source component",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new AlarmSirenGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AlarmSirenGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new AlarmSirenGameItemComponentProto(proto, gameworld));
	}
}
