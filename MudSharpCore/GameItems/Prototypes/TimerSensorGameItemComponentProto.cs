#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TimerSensorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3activevalue <number>#0 - the signal value emitted during the active phase
	#3inactivevalue <number>#0 - the signal value emitted during the inactive phase
	#3activeduration <seconds>#0 - how long each active phase lasts
	#3inactiveduration <seconds>#0 - how long each inactive phase lasts
	#3initial <active|inactive>#0 - whether the timer starts in its active or inactive phase

#6Notes:#0

	This sensor only emits its timer-cycle signal while it is switched on and receiving power.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected TimerSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Timer Sensor")
	{
		UseMountHostPowerSource = true;
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

	protected override string ComponentDescriptionOLCByline => "This item is a powered timer sensor";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Active Value: {ActiveValue.ToString("N2", actor).ColourValue()}\nInactive Value: {InactiveValue.ToString("N2", actor).ColourValue()}\nActive Duration: {ActiveDuration.Describe(actor).ColourValue()}\nInactive Duration: {InactiveDuration.Describe(actor).ColourValue()}\nInitial Phase: {(StartActive ? "Active".ColourValue() : "Inactive".ColourName())}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		ActiveValue = double.Parse(root.Element("ActiveValue")?.Value ?? "1.0");
		InactiveValue = double.Parse(root.Element("InactiveValue")?.Value ?? "0.0");
		ActiveDuration = TimeSpan.FromSeconds(double.Parse(root.Element("ActiveDurationSeconds")?.Value ?? "5.0"));
		InactiveDuration =
			TimeSpan.FromSeconds(double.Parse(root.Element("InactiveDurationSeconds")?.Value ?? "55.0"));
		StartActive = bool.Parse(root.Element("StartActive")?.Value ?? "false");
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("ActiveValue", ActiveValue));
		root.Add(new XElement("InactiveValue", InactiveValue));
		root.Add(new XElement("ActiveDurationSeconds", ActiveDuration.TotalSeconds));
		root.Add(new XElement("InactiveDurationSeconds", InactiveDuration.TotalSeconds));
		root.Add(new XElement("StartActive", StartActive));
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
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
				return base.BuildingCommand(actor, command.GetUndo());
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
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits a repeating active/inactive signal cycle",
			CombinedBuildingHelpText);
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
