#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class MotionSensorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3value <number>#0 - the signal value emitted while the sensor is active
	#3duration <seconds>#0 - how long the sensor remains active after motion is detected
	#3size <size>#0 - the minimum target size category that can trigger the sensor
	#3mode <any|begin|enter|stop>#0 - which witnessed movement events trigger the sensor

#6Notes:#0

	This sensor only emits a signal while it is switched on and receiving power.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected MotionSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Motion Sensor")
	{
		UseMountHostPowerSource = true;
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

	protected override string ComponentDescriptionOLCByline => "This item is a powered motion sensor";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Signal Value: {SignalValue.ToString("N2", actor).ColourValue()}\nActive Duration: {SignalDuration.Describe(actor).ColourValue()}\nMinimum Size: {MinimumSize.Describe().ColourValue()}\nDetection Mode: {DetectionMode.Describe().ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SignalValue = double.Parse(root.Element("SignalValue")?.Value ?? "1.0");
		SignalDuration = TimeSpan.FromSeconds(double.Parse(root.Element("SignalDurationSeconds")?.Value ?? "10.0"));
		MinimumSize = Enum.TryParse<SizeCategory>(root.Element("MinimumSize")?.Value, out var minimumSize)
			? minimumSize
			: SizeCategory.Normal;
		DetectionMode = Enum.TryParse<MotionSensorDetectionMode>(root.Element("DetectionMode")?.Value, out var mode)
			? mode
			: MotionSensorDetectionMode.AnyMovement;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("SignalValue", SignalValue));
		root.Add(new XElement("SignalDurationSeconds", SignalDuration.TotalSeconds));
		root.Add(new XElement("MinimumSize", MinimumSize));
		root.Add(new XElement("DetectionMode", DetectionMode));
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
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
				return base.BuildingCommand(actor, command.GetUndo());
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
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits a timed signal when it witnesses configured movement",
			CombinedBuildingHelpText);
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
