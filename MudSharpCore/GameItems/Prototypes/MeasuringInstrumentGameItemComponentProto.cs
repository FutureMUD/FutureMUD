#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class MeasuringInstrumentGameItemComponentProto : GameItemComponentProto, IMeasuringInstrumentPrototype, IConditionDegradingComponentPrototype
{
	public override string TypeDescription => "MeasuringInstrument";

	public MeasuringInstrumentMode Mode { get; protected set; } = MeasuringInstrumentMode.Weight;
	public UnitType UnitType => Mode == MeasuringInstrumentMode.Weight ? UnitType.Mass : UnitType.FluidVolume;
	public double Precision { get; protected set; } = 0.01;
	public double Capacity { get; protected set; } = 100.0;
	public double BaseDriftPerUse { get; protected set; } = 0.0005;
	public double MaximumDrift { get; protected set; } = 0.05;
	public double MaximumWrongCalibration { get; protected set; } = 0.5;
	public Difficulty CalibrationInspectionDifficulty { get; protected set; } = Difficulty.Normal;
	public ConditionMaintenanceProfile ConditionMaintenance { get; } = new(ConditionMaintenanceProfile.DefaultMeasurementUseExpression);

	protected MeasuringInstrumentGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "MeasuringInstrument")
	{
	}

	protected MeasuringInstrumentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Mode = root.Element("Mode")?.Value.TryParseEnum<MeasuringInstrumentMode>(out var mode) == true
			? mode
			: MeasuringInstrumentMode.Weight;
		Precision = double.Parse(root.Element("Precision")?.Value ?? "0.01");
		Capacity = double.Parse(root.Element("Capacity")?.Value ?? "100.0");
		BaseDriftPerUse = double.Parse(root.Element("BaseDriftPerUse")?.Value ?? "0.0005");
		MaximumDrift = double.Parse(root.Element("MaximumDrift")?.Value ?? "0.05");
		MaximumWrongCalibration = double.Parse(root.Element("MaximumWrongCalibration")?.Value ?? "0.5");
		CalibrationInspectionDifficulty = (Difficulty)int.Parse(root.Element("CalibrationInspectionDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		ConditionMaintenance.LoadFromXml(root);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Mode", Mode.ToString()),
			new XElement("Precision", Precision),
			new XElement("Capacity", Capacity),
			new XElement("BaseDriftPerUse", BaseDriftPerUse),
			new XElement("MaximumDrift", MaximumDrift),
			new XElement("MaximumWrongCalibration", MaximumWrongCalibration),
			new XElement("CalibrationInspectionDifficulty", (int)CalibrationInspectionDifficulty),
			ConditionMaintenance.SaveToXml()).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MeasuringInstrumentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MeasuringInstrumentGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("measuringinstrument", true,
			(gameworld, account) => new MeasuringInstrumentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("MeasuringInstrument",
			(proto, gameworld) => new MeasuringInstrumentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"MeasuringInstrument",
			$"Lets an item {"[measure weight or liquid volume]".Colour(Telnet.Yellow)} with calibration drift",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MeasuringInstrumentGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3mode weight|fluidvolume#0 - sets what this instrument measures
	#3precision <amount>#0 - sets the rounding precision
	#3capacity <amount>#0 - sets the maximum measurable amount
	#3drift <percent>#0 - sets the drift accumulated per use at standard quality
	#3maxdrift <percent>#0 - sets the maximum honest drift
	#3maxwrong <percent>#0 - sets the maximum deliberate wrong-calibration bias
	#3difficulty <difficulty>#0 - sets the appraise difficulty to inspect calibration
	#3condition <option>#0 - configures optional condition degradation";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "mode":
			case "type":
				return BuildingCommandMode(actor, command);
			case "precision":
			case "rounding":
				return BuildingCommandPrecision(actor, command);
			case "capacity":
			case "cap":
				return BuildingCommandCapacity(actor, command);
			case "drift":
			case "base":
			case "basedrift":
				return BuildingCommandDrift(actor, command);
			case "maxdrift":
			case "maximumdrift":
				return BuildingCommandMaximumDrift(actor, command);
			case "maxwrong":
			case "maximumwrong":
			case "wrong":
				return BuildingCommandMaximumWrong(actor, command);
			case "difficulty":
			case "inspect":
			case "inspection":
				return BuildingCommandDifficulty(actor, command);
			case "condition":
				return ConditionMaintenance.BuildingCommand(actor, command, () => Changed = true);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<MeasuringInstrumentMode>(out var mode))
		{
			actor.OutputHandler.Send($"Which mode should this instrument use? Valid values are {Enum.GetValues<MeasuringInstrumentMode>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		Mode = mode;
		Changed = true;
		actor.OutputHandler.Send($"This instrument will now measure {Mode.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandPrecision(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType, actor, out var value) ||
		    value <= 0.0)
		{
			actor.OutputHandler.Send($"What positive {UnitType.DescribeEnum().ColourName()} precision should this instrument round to?");
			return false;
		}

		Precision = value;
		Changed = true;
		actor.OutputHandler.Send($"This instrument will round measurements to {Gameworld.UnitManager.DescribeExact(Precision, UnitType, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType, actor, out var value) ||
		    value <= 0.0)
		{
			actor.OutputHandler.Send($"What positive {UnitType.DescribeEnum().ColourName()} capacity should this instrument have?");
			return false;
		}

		Capacity = value;
		Changed = true;
		actor.OutputHandler.Send($"This instrument can now measure up to {Gameworld.UnitManager.DescribeExact(Capacity, UnitType, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDrift(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) ||
		    value < 0.0)
		{
			actor.OutputHandler.Send("What non-negative percentage drift should this instrument gain per use at standard quality?");
			return false;
		}

		BaseDriftPerUse = value;
		Changed = true;
		actor.OutputHandler.Send($"This instrument will drift by {BaseDriftPerUse.ToString("P3", actor).ColourValue()} per use at standard quality.");
		return true;
	}

	private bool BuildingCommandMaximumDrift(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) ||
		    value < 0.0)
		{
			actor.OutputHandler.Send("What non-negative maximum honest drift percentage should this instrument have?");
			return false;
		}

		MaximumDrift = value;
		Changed = true;
		actor.OutputHandler.Send($"This instrument's honest calibration drift is capped at {MaximumDrift.ToString("P3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMaximumWrong(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) ||
		    value < 0.0)
		{
			actor.OutputHandler.Send("What non-negative maximum deliberate wrong-calibration percentage should this instrument allow?");
			return false;
		}

		MaximumWrongCalibration = value;
		Changed = true;
		actor.OutputHandler.Send($"This instrument's deliberate wrong calibration is capped at {MaximumWrongCalibration.ToString("P3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send($"What difficulty should it be to inspect calibration? Valid values are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		CalibrationInspectionDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This component's calibration inspection difficulty is now {CalibrationInspectionDifficulty.DescribeColoured()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"MeasuringInstrument Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Mode: {Mode.DescribeEnum().ColourName()}
Capacity: {Gameworld.UnitManager.DescribeExact(Capacity, UnitType, actor).ColourValue()}
Precision: {Gameworld.UnitManager.DescribeExact(Precision, UnitType, actor).ColourValue()}
Base Drift Per Use: {BaseDriftPerUse.ToString("P3", actor).ColourValue()}
Maximum Drift: {MaximumDrift.ToString("P3", actor).ColourValue()}
Maximum Wrong Calibration: {MaximumWrongCalibration.ToString("P3", actor).ColourValue()}
Calibration Inspection Difficulty: {CalibrationInspectionDifficulty.DescribeColoured()}
{ConditionMaintenance.Describe(actor)}";
	}
}
