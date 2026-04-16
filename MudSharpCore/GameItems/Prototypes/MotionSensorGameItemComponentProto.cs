#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class MotionSensorGameItemComponentProto : GameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	value <number> - the signal value emitted while the sensor is active
	duration <seconds> - how long the sensor remains active after motion is detected
	size <size> - the minimum target size category that can trigger the sensor
	mode <any|begin|enter|stop> - which witnessed movement events trigger the sensor";

	protected MotionSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Motion Sensor")
	{
		SignalValue = 1.0;
		SignalDuration = TimeSpan.FromSeconds(10);
		MinimumSize = SizeCategory.Normal;
		DetectionMode = MotionSensorDetectionMode.AnyMovement;
	}

	protected MotionSensorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double SignalValue { get; protected set; }
	public TimeSpan SignalDuration { get; protected set; }
	public SizeCategory MinimumSize { get; protected set; }
	public MotionSensorDetectionMode DetectionMode { get; protected set; }
	public override string TypeDescription => "Motion Sensor";

	protected override void LoadFromXml(XElement root)
	{
		SignalValue = double.Parse(root.Element("SignalValue")?.Value ?? "1.0");
		SignalDuration = TimeSpan.FromSeconds(double.Parse(root.Element("SignalDurationSeconds")?.Value ?? "10.0"));
		MinimumSize = Enum.TryParse<SizeCategory>(root.Element("MinimumSize")?.Value, out var minimumSize)
			? minimumSize
			: SizeCategory.Normal;
		DetectionMode = Enum.TryParse<MotionSensorDetectionMode>(root.Element("DetectionMode")?.Value, out var mode)
			? mode
			: MotionSensorDetectionMode.AnyMovement;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SignalValue", SignalValue),
			new XElement("SignalDurationSeconds", SignalDuration.TotalSeconds),
			new XElement("MinimumSize", MinimumSize),
			new XElement("DetectionMode", DetectionMode)
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "value":
			case "signal":
				return BuildingCommandSignalValue(actor, command);
			case "duration":
			case "time":
				return BuildingCommandDuration(actor, command);
			case "size":
			case "minimumsize":
			case "minsize":
				return BuildingCommandMinimumSize(actor, command);
			case "mode":
				return BuildingCommandDetectionMode(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSignalValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric signal value should this motion sensor emit when triggered?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number for the signal value.");
			return false;
		}

		SignalValue = value;
		Changed = true;
		actor.Send(
			$"This motion sensor now emits a signal value of {SignalValue.ToString("N2", actor).ColourValue()} when triggered.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds should this motion sensor remain active after it detects movement?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds.");
			return false;
		}

		SignalDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.Send(
			$"This motion sensor will now remain active for {SignalDuration.Describe(actor).ColourValue()} after detecting movement.");
		return true;
	}

	private bool BuildingCommandMinimumSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What is the minimum size category that should trigger this motion sensor?");
			return false;
		}

		if (!MudSharp.GameItems.GameItemEnumExtensions.TryParseSize(command.SafeRemainingArgument, out var value))
		{
			actor.Send("That is not a valid size category.");
			return false;
		}

		MinimumSize = value;
		Changed = true;
		actor.Send(
			$"This motion sensor will now only react to {MinimumSize.Describe().ColourValue()} or larger targets.");
		return true;
	}

	private bool BuildingCommandDetectionMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which movement mode should trigger this sensor: any, begin, enter, or stop?");
			return false;
		}

		if (!MotionSensorDetectionModeExtensions.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("That is not a valid motion detection mode. Valid options are any, begin, enter, or stop.");
			return false;
		}

		DetectionMode = value;
		Changed = true;
		actor.Send($"This motion sensor now triggers on {DetectionMode.Describe().ColourValue()} events.");
		return true;
	}

	public override bool CanSubmit()
	{
		return SignalDuration > TimeSpan.Zero && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SignalDuration <= TimeSpan.Zero)
		{
			return "You must set a positive active duration for this motion sensor.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Motion Sensor Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component emits a signal value of {SignalValue.ToString("N2", actor).ColourValue()} for {SignalDuration.Describe(actor).ColourValue()} whenever it witnesses {DetectionMode.Describe().ColourValue()} from {MinimumSize.Describe().ColourValue()} or larger targets.";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("motionsensor", true,
			(gameworld, account) => new MotionSensorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("motion sensor", false,
			(gameworld, account) => new MotionSensorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Motion Sensor",
			(proto, gameworld) => new MotionSensorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"MotionSensor",
			$"A {"[signal source]".Colour(Telnet.Yellow)} that emits a timed signal when it witnesses configured movement",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MotionSensorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MotionSensorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MotionSensorGameItemComponentProto(proto, gameworld));
	}
}
