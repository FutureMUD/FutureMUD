#nullable enable

using MudSharp.Accounts;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Vehicles;

namespace MudSharp.GameItems.Prototypes;

public class OutboardMotorGameItemComponentProto : GameItemComponentProto, IOutboardMotorPrototype
{
	public OutboardMotorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Outboard Motor")
	{
		Description = "Makes an installable item an outboard motor for a surface-water vehicle";
	}

	protected OutboardMotorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public OutboardMotorEnergySource EnergySource { get; private set; } = OutboardMotorEnergySource.Fuelled;
	public double OutputMultiplier { get; private set; } = 1.0;
	public ILiquid? FuelLiquid { get; private set; }
	public double FuelVolumePerMove { get; private set; }
	public double RequiredPowerSpikeInWatts { get; private set; }
	public override string TypeDescription => "Outboard Motor";
	public override string ShowBuildingHelp => BuildingHelpText;

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("outboard motor", true,
			(gameworld, account) => new OutboardMotorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("outboardmotor", false,
			(gameworld, account) => new OutboardMotorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Outboard Motor",
			(proto, gameworld) => new OutboardMotorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("OutboardMotor",
			$"Makes an item an {"[outboard motor]".Colour(Telnet.BoldGreen)} for surface-water vehicles.",
			BuildingHelpText);
	}

	protected override void LoadFromXml(XElement root)
	{
		EnergySource = Enum.TryParse<OutboardMotorEnergySource>(root.Element("EnergySource")?.Value, true,
			out var source)
			? source
			: OutboardMotorEnergySource.Fuelled;
		OutputMultiplier = double.TryParse(root.Element("OutputMultiplier")?.Value, out var output) &&
		                   double.IsFinite(output) && output > 0.0
			? output
			: 1.0;
		FuelLiquid = long.TryParse(root.Element("FuelLiquidId")?.Value, out var liquidId)
			? Gameworld.Liquids.Get(liquidId)
			: null;
		FuelVolumePerMove = double.TryParse(root.Element("FuelVolumePerMove")?.Value, out var fuel) &&
		                    double.IsFinite(fuel) && fuel >= 0.0
			? fuel
			: 0.0;
		RequiredPowerSpikeInWatts = double.TryParse(root.Element("RequiredPowerSpikeInWatts")?.Value, out var power) &&
		                                double.IsFinite(power) && power >= 0.0
			? power
			: 0.0;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("EnergySource", EnergySource),
			new XElement("OutputMultiplier", OutputMultiplier),
			new XElement("FuelLiquidId", FuelLiquid?.Id ?? 0),
			new XElement("FuelVolumePerMove", FuelVolumePerMove),
			new XElement("RequiredPowerSpikeInWatts", RequiredPowerSpikeInWatts)
		).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var resource = EnergySource switch
		{
			OutboardMotorEnergySource.Fuelled =>
				$"{FuelVolumePerMove.ToString("N2", actor).ColourValue()} of {FuelLiquid?.Name.ColourName() ?? "no fuel".ColourError()} per move",
			OutboardMotorEnergySource.Electric =>
				$"{RequiredPowerSpikeInWatts.ToString("N2", actor).ColourValue()}W spike per move",
			_ => "invalid".ColourError()
		};
		return $@"{"Outboard Motor Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Energy Source: {EnergySource.DescribeEnum().ColourName()}
Output Multiplier: {OutputMultiplier.ToString("N2", actor).ColourValue()}
Resource: {resource}

The parent item must also be a functional VehicleInstallable module. Fuelled motors use a same-item liquid container; electric motors use a same-item power producer.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new OutboardMotorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new OutboardMotorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new OutboardMotorGameItemComponentProto(proto, gameworld));
	}

	public override bool CanSubmit()
	{
		if (!double.IsFinite(OutputMultiplier) || OutputMultiplier <= 0.0)
		{
			return false;
		}

		return EnergySource switch
		{
			OutboardMotorEnergySource.Fuelled => FuelLiquid is not null &&
			                                           double.IsFinite(FuelVolumePerMove) &&
			                                           FuelVolumePerMove > 0.0 &&
			                                           base.CanSubmit(),
			OutboardMotorEnergySource.Electric => double.IsFinite(RequiredPowerSpikeInWatts) &&
			                                          RequiredPowerSpikeInWatts > 0.0 &&
			                                          base.CanSubmit(),
			_ => false
		};
	}

	public override string WhyCannotSubmit()
	{
		if (!double.IsFinite(OutputMultiplier) || OutputMultiplier <= 0.0)
		{
			return "The outboard motor output multiplier must be positive and finite.";
		}

		if (EnergySource == OutboardMotorEnergySource.Fuelled &&
		    (FuelLiquid is null || !double.IsFinite(FuelVolumePerMove) || FuelVolumePerMove <= 0.0))
		{
			return "Fuelled outboard motors must specify a fuel liquid and positive volume per move.";
		}

		if (EnergySource == OutboardMotorEnergySource.Electric &&
		    (!double.IsFinite(RequiredPowerSpikeInWatts) || RequiredPowerSpikeInWatts <= 0.0))
		{
			return "Electric outboard motors must specify a positive power spike.";
		}

		return base.WhyCannotSubmit();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "energy":
			case "source":
				return BuildingCommandEnergy(actor, command);
			case "output":
			case "multiplier":
				return BuildingCommandOutput(actor, command);
			case "fuel":
				return BuildingCommandFuel(actor, command);
			case "power":
			case "watts":
				return BuildingCommandPower(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandEnergy(ICharacter actor, StringStack command)
	{
		var text = command.PopSpeech();
		if (!text.TryParseEnum(out OutboardMotorEnergySource source))
		{
			actor.OutputHandler.Send("You must specify either fuelled or electric.");
			return false;
		}

		EnergySource = source;
		Changed = true;
		actor.OutputHandler.Send($"This motor is now {source.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandOutput(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || !double.IsFinite(value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive, finite output multiplier.");
			return false;
		}

		OutputMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This motor now has an output multiplier of {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid and volume per move should this motor consume?");
			return false;
		}

		var liquidText = command.PopSpeech();
		var liquid = Gameworld.Liquids.GetByIdOrName(liquidText);
		if (liquid is null || !double.TryParse(command.SafeRemainingArgument, out var volume) ||
		    !double.IsFinite(volume) || volume <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a valid liquid and a positive volume per move.");
			return false;
		}

		EnergySource = OutboardMotorEnergySource.Fuelled;
		FuelLiquid = liquid;
		FuelVolumePerMove = volume;
		Changed = true;
		actor.OutputHandler.Send(
			$"This fuelled motor now consumes {volume.ToString("N2", actor).ColourValue()} of {liquid.Name.ColourName()} per move.");
		return true;
	}

	private bool BuildingCommandPower(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var watts) || !double.IsFinite(watts) || watts <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive power spike in watts.");
			return false;
		}

		EnergySource = OutboardMotorEnergySource.Electric;
		RequiredPowerSpikeInWatts = watts;
		Changed = true;
		actor.OutputHandler.Send(
			$"This electric motor now requires a {watts.ToString("N2", actor).ColourValue()}W power spike per move.");
		return true;
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3energy <fuelled|electric>#0 - sets the motor's energy variant
	#3output <multiplier>#0 - sets the positive propulsion-output multiplier
	#3fuel <liquid> <volume>#0 - makes the motor fuelled and sets its per-move fuel use
	#3power <watts>#0 - makes the motor electric and sets its per-move power spike";
}
