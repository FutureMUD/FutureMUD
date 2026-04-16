#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class TimerSensorGameItemComponentProto : GameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	activevalue <number> - the signal value emitted during the active phase
	inactivevalue <number> - the signal value emitted during the inactive phase
	activeduration <seconds> - how long each active phase lasts
	inactiveduration <seconds> - how long each inactive phase lasts
	initial <active|inactive> - whether the timer starts in its active or inactive phase";

	protected TimerSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Timer Sensor")
	{
		ActiveValue = 1.0;
		InactiveValue = 0.0;
		ActiveDuration = TimeSpan.FromSeconds(5);
		InactiveDuration = TimeSpan.FromSeconds(55);
		StartActive = false;
	}

	protected TimerSensorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double ActiveValue { get; protected set; }
	public double InactiveValue { get; protected set; }
	public TimeSpan ActiveDuration { get; protected set; }
	public TimeSpan InactiveDuration { get; protected set; }
	public bool StartActive { get; protected set; }
	public override string TypeDescription => "Timer Sensor";

	protected override void LoadFromXml(XElement root)
	{
		ActiveValue = double.Parse(root.Element("ActiveValue")?.Value ?? "1.0");
		InactiveValue = double.Parse(root.Element("InactiveValue")?.Value ?? "0.0");
		ActiveDuration = TimeSpan.FromSeconds(double.Parse(root.Element("ActiveDurationSeconds")?.Value ?? "5.0"));
		InactiveDuration =
			TimeSpan.FromSeconds(double.Parse(root.Element("InactiveDurationSeconds")?.Value ?? "55.0"));
		StartActive = bool.Parse(root.Element("StartActive")?.Value ?? "false");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ActiveValue", ActiveValue),
			new XElement("InactiveValue", InactiveValue),
			new XElement("ActiveDurationSeconds", ActiveDuration.TotalSeconds),
			new XElement("InactiveDurationSeconds", InactiveDuration.TotalSeconds),
			new XElement("StartActive", StartActive)
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "activevalue":
			case "onvalue":
				return BuildingCommandActiveValue(actor, command);
			case "inactivevalue":
			case "offvalue":
				return BuildingCommandInactiveValue(actor, command);
			case "activeduration":
			case "onduration":
				return BuildingCommandActiveDuration(actor, command);
			case "inactiveduration":
			case "offduration":
				return BuildingCommandInactiveDuration(actor, command);
			case "initial":
			case "start":
				return BuildingCommandInitialPhase(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandActiveValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric signal value should this timer emit during its active phase?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number for the active signal value.");
			return false;
		}

		ActiveValue = value;
		Changed = true;
		actor.Send(
			$"This timer sensor now emits {ActiveValue.ToString("N2", actor).ColourValue()} during its active phase.");
		return true;
	}

	private bool BuildingCommandInactiveValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric signal value should this timer emit during its inactive phase?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number for the inactive signal value.");
			return false;
		}

		InactiveValue = value;
		Changed = true;
		actor.Send(
			$"This timer sensor now emits {InactiveValue.ToString("N2", actor).ColourValue()} during its inactive phase.");
		return true;
	}

	private bool BuildingCommandActiveDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds should this timer sensor remain in its active phase?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds.");
			return false;
		}

		ActiveDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.Send(
			$"This timer sensor will now stay active for {ActiveDuration.Describe(actor).ColourValue()} each cycle.");
		return true;
	}

	private bool BuildingCommandInactiveDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds should this timer sensor remain in its inactive phase?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds.");
			return false;
		}

		InactiveDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.Send(
			$"This timer sensor will now stay inactive for {InactiveDuration.Describe(actor).ColourValue()} each cycle.");
		return true;
	}

	private bool BuildingCommandInitialPhase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Should this timer sensor begin active or inactive?");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "active":
			case "on":
				StartActive = true;
				break;
			case "inactive":
			case "off":
				StartActive = false;
				break;
			default:
				actor.Send("You must specify either active or inactive.");
				return false;
		}

		Changed = true;
		actor.Send(
			$"This timer sensor will now begin in its {(StartActive ? "active".ColourValue() : "inactive".ColourName())} phase.");
		return true;
	}

	public override bool CanSubmit()
	{
		return ActiveDuration > TimeSpan.Zero && InactiveDuration > TimeSpan.Zero && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (ActiveDuration <= TimeSpan.Zero)
		{
			return "You must set a positive active duration for this timer sensor.";
		}

		if (InactiveDuration <= TimeSpan.Zero)
		{
			return "You must set a positive inactive duration for this timer sensor.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Timer Sensor Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component alternates between emitting {ActiveValue.ToString("N2", actor).ColourValue()} for {ActiveDuration.Describe(actor).ColourValue()} and {InactiveValue.ToString("N2", actor).ColourValue()} for {InactiveDuration.Describe(actor).ColourValue()}. It begins in its {(StartActive ? "active".ColourValue() : "inactive".ColourName())} phase.";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("timersensor", true,
			(gameworld, account) => new TimerSensorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("timer sensor", false,
			(gameworld, account) => new TimerSensorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Timer Sensor",
			(proto, gameworld) => new TimerSensorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"TimerSensor",
			$"A {"[signal source]".Colour(Telnet.Yellow)} that emits a repeating active/inactive signal cycle",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TimerSensorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TimerSensorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TimerSensorGameItemComponentProto(proto, gameworld));
	}
}
