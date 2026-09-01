#nullable enable

using MudSharp.Accounts;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class CombustionEngineGameItemComponentProto : GameItemComponentProto, IVehicleEnginePrototype,
	ISwitchablePrototype, IOnOffPrototype
{
	public CombustionEngineGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Combustion Engine")
	{
		Description = "Makes an installable item a liquid-fuelled terrestrial vehicle engine";
		FormFactor = "automotive";
		MaximumPowerInWatts = 100000.0;
		NoiseLevel = AudioVolume.Loud;
	}

	protected CombustionEngineGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public string FormFactor { get; private set; } = "automotive";
	public double MaximumPowerInWatts { get; private set; }
	public AudioVolume NoiseLevel { get; private set; }
	public ILiquid? FuelLiquid { get; private set; }
	public double FuelPerSecond { get; private set; }
	public override string TypeDescription => "Combustion Engine";
	public override string ShowBuildingHelp => BuildingHelpText;

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("combustion engine", true,
			(gameworld, account) => new CombustionEngineGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("combustionengine", false,
			(gameworld, account) => new CombustionEngineGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Combustion Engine",
			(proto, gameworld) => new CombustionEngineGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("CombustionEngine",
			$"Makes an item a liquid-fuelled terrestrial {"[vehicle engine]".Colour(Telnet.BoldGreen)}.",
			BuildingHelpText);
	}

	protected override void LoadFromXml(XElement root)
	{
		FormFactor = root.Element("FormFactor")?.Value ?? "automotive";
		MaximumPowerInWatts = double.TryParse(root.Element("MaximumPowerInWatts")?.Value, out var power) &&
		                     double.IsFinite(power) && power > 0.0
			? power
			: 100000.0;
		NoiseLevel = Enum.TryParse<AudioVolume>(root.Element("NoiseLevel")?.Value, true, out var noise)
			? noise
			: AudioVolume.Loud;
		FuelLiquid = long.TryParse(root.Element("FuelLiquidId")?.Value, out var liquidId)
			? Gameworld.Liquids.Get(liquidId)
			: null;
		FuelPerSecond = double.TryParse(root.Element("FuelPerSecond")?.Value, out var fuel) &&
		                double.IsFinite(fuel) && fuel >= 0.0
			? fuel
			: 0.0;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("FormFactor", new XCData(FormFactor)),
			new XElement("MaximumPowerInWatts", MaximumPowerInWatts),
			new XElement("NoiseLevel", NoiseLevel),
			new XElement("FuelLiquidId", FuelLiquid?.Id ?? 0),
			new XElement("FuelPerSecond", FuelPerSecond)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new CombustionEngineGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CombustionEngineGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new CombustionEngineGameItemComponentProto(proto, gameworld));
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrWhiteSpace(FormFactor) &&
		       FuelLiquid is not null &&
		       double.IsFinite(FuelPerSecond) && FuelPerSecond > 0.0 &&
		       double.IsFinite(MaximumPowerInWatts) && MaximumPowerInWatts > 0.0 &&
		       Enum.IsDefined(NoiseLevel) &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (string.IsNullOrWhiteSpace(FormFactor))
		{
			return "The combustion engine must have a form factor.";
		}

		if (FuelLiquid is null || !double.IsFinite(FuelPerSecond) || FuelPerSecond <= 0.0)
		{
			return "The combustion engine must have a fuel liquid and positive hourly consumption.";
		}

		if (!double.IsFinite(MaximumPowerInWatts) || MaximumPowerInWatts <= 0.0)
		{
			return "The combustion engine must have positive maximum power.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var hourlyFuel = Gameworld.UnitManager.DescribeExact(FuelPerSecond * 3600.0, UnitType.FluidVolume, actor);
		return $@"{"Combustion Engine Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Form Factor: {FormFactor.ColourCommand()}
Maximum Mechanical Power: {MaximumPowerInWatts.ToString("N2", actor).ColourValue()}W
Fuel: {FuelLiquid?.Name.ColourName() ?? "not set".ColourError()}
Consumption: {hourlyFuel.ColourValue()} per hour
Noise: {NoiseLevel.Describe().ColourName()}

The parent item must also have a VehicleInstallable component and a same-item liquid container.";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech();
		switch (cmd.ToLowerInvariant())
		{
			case "form":
			case "formfactor":
			case "factor":
				return BuildingCommandFormFactor(actor, command);
			case "output":
			case "maximum":
			case "maxpower":
			case "power":
				return BuildingCommandMaximumPower(actor, command);
			case "fuel":
				return BuildingCommandFuel(actor, command);
			case "consumption":
			case "draw":
			case "rate":
				return BuildingCommandConsumption(actor, command);
			case "noise":
			case "volume":
				return BuildingCommandNoise(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"{cmd} {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandFormFactor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What non-blank form factor should this engine use?");
			return false;
		}

		FormFactor = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This engine now has the {FormFactor.ColourCommand()} form factor.");
		return true;
	}

	private bool BuildingCommandMaximumPower(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var watts) ||
		    !double.IsFinite(watts) || watts <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive maximum mechanical power in watts.");
			return false;
		}

		MaximumPowerInWatts = watts;
		Changed = true;
		actor.OutputHandler.Send(
			$"This engine now produces up to {watts.ToString("N2", actor).ColourValue()} watts of mechanical power.");
		return true;
	}

	private bool BuildingCommandFuel(ICharacter actor, StringStack command)
	{
		var liquidText = command.PopSpeech();
		var liquid = Gameworld.Liquids.GetByIdOrName(liquidText);
		if (liquid is null)
		{
			actor.OutputHandler.Send("You must specify a valid fuel liquid.");
			return false;
		}

		FuelLiquid = liquid;
		if (!command.IsFinished)
		{
			if (!TryParseHourlyConsumption(command.SafeRemainingArgument, out var fuelPerSecond))
			{
				actor.OutputHandler.Send("The optional consumption must be a positive fluid volume per hour.");
				return false;
			}

			FuelPerSecond = fuelPerSecond;
		}

		Changed = true;
		actor.OutputHandler.Send($"This engine now burns {liquid.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandConsumption(ICharacter actor, StringStack command)
	{
		if (!TryParseHourlyConsumption(command.SafeRemainingArgument, out var fuelPerSecond))
		{
			actor.OutputHandler.Send("You must specify a positive fluid volume consumed per hour.");
			return false;
		}

		FuelPerSecond = fuelPerSecond;
		Changed = true;
		actor.OutputHandler.Send(
			$"This engine now consumes {Gameworld.UnitManager.DescribeExact(FuelPerSecond * 3600.0, UnitType.FluidVolume, actor).ColourValue()} per hour.");
		return true;
	}

	private bool TryParseHourlyConsumption(string text, out double fuelPerSecond)
	{
		var hourly = Gameworld.UnitManager.GetBaseUnits(text, UnitType.FluidVolume, out var success);
		fuelPerSecond = success && double.IsFinite(hourly) && hourly > 0.0 ? hourly / 3600.0 : 0.0;
		return fuelPerSecond > 0.0;
	}

	private bool BuildingCommandNoise(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out AudioVolume noise))
		{
			actor.OutputHandler.Send(
				$"You must specify a valid audio volume: {Enum.GetValues<AudioVolume>().Select(x => x.Describe()).ListToString()}.");
			return false;
		}

		NoiseLevel = noise;
		Changed = true;
		actor.OutputHandler.Send($"This engine's noise is now {noise.Describe().ColourName()}.");
		return true;
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3formfactor <text>#0 - sets the compatible vehicle installation mount type
	#3output <watts>#0 - sets maximum mechanical power
	#3fuel <liquid> [volume per hour]#0 - sets the liquid fuel and optional consumption
	#3consumption <volume per hour>#0 - sets hourly fuel consumption
	#3noise <volume>#0 - sets the engine's audio volume";
}
