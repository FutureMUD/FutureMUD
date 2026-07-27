#nullable enable

using MudSharp.Accounts;
using MudSharp.Form.Audio;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class ElectricEngineGameItemComponentProto : PoweredMachineBaseGameItemComponentProto,
	IVehicleEnginePrototype
{
	public ElectricEngineGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Electric Engine")
	{
		Description = "Makes an installable item an electrically powered terrestrial vehicle engine";
		FormFactor = "automotive";
		MaximumPowerInWatts = 100000.0;
		NoiseLevel = AudioVolume.Decent;
		Wattage = 50000.0;
		WattageDiscountPerQuality = 0.0;
		PowerOnEmote = "@ whine|whines to life.";
		PowerOffEmote = "@ wind|winds down.";
	}

	protected ElectricEngineGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public string FormFactor { get; private set; } = "automotive";
	public double MaximumPowerInWatts { get; private set; }
	public AudioVolume NoiseLevel { get; private set; }
	public override string TypeDescription => "Electric Engine";
	public override string ShowBuildingHelp => BuildingHelpText;
	protected override string ComponentDescriptionOLCByline =>
		"This is an electric terrestrial vehicle engine";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("electric engine", true,
			(gameworld, account) => new ElectricEngineGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electricengine", false,
			(gameworld, account) => new ElectricEngineGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Electric Engine",
			(proto, gameworld) => new ElectricEngineGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ElectricEngine",
			$"Makes an item an electrically powered terrestrial {"[vehicle engine]".Colour(Telnet.BoldGreen)}.",
			BuildingHelpText);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		FormFactor = root.Element("FormFactor")?.Value ?? "automotive";
		MaximumPowerInWatts = double.TryParse(root.Element("MaximumPowerInWatts")?.Value, out var power) &&
		                     double.IsFinite(power) && power > 0.0
			? power
			: 100000.0;
		NoiseLevel = Enum.TryParse<AudioVolume>(root.Element("NoiseLevel")?.Value, true, out var noise)
			? noise
			: AudioVolume.Decent;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(
			new XElement("FormFactor", new XCData(FormFactor)),
			new XElement("MaximumPowerInWatts", MaximumPowerInWatts),
			new XElement("NoiseLevel", NoiseLevel));
		return root;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ElectricEngineGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ElectricEngineGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ElectricEngineGameItemComponentProto(proto, gameworld));
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrWhiteSpace(FormFactor) &&
		       double.IsFinite(MaximumPowerInWatts) && MaximumPowerInWatts > 0.0 &&
		       double.IsFinite(Wattage) && Wattage > 0.0 &&
		       Enum.IsDefined(NoiseLevel) &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (string.IsNullOrWhiteSpace(FormFactor))
		{
			return "The electric engine must have a form factor.";
		}

		if (!double.IsFinite(MaximumPowerInWatts) || MaximumPowerInWatts <= 0.0)
		{
			return "The electric engine must have positive maximum mechanical power.";
		}

		if (!double.IsFinite(Wattage) || Wattage <= 0.0)
		{
			return "The electric engine must have positive continuous electrical draw.";
		}

		return base.WhyCannotSubmit();
	}

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $@"Form Factor: {FormFactor.ColourCommand()}
Maximum Mechanical Power: {MaximumPowerInWatts.ToString("N2", actor).ColourValue()}W
Noise: {NoiseLevel.Describe().ColourName()}
The parent item must also have a VehicleInstallable component and a compatible power producer or connection.";
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
				return BuildingCommandMaximumPower(actor, command);
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

	private new const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3formfactor <text>#0 - sets the compatible vehicle installation mount type
	#3output <watts>#0 - sets maximum mechanical power
	#3wattage <watts>#0 - sets continuous electrical draw
	#3discount <watts>#0 - sets the wattage discount per quality
	#3mountpower#0 - toggles drawing power from a mounted host
	#3switchable#0 - toggles whether players can switch the engine
	#3onemote <emote>#0 - sets the power-on emote
	#3offemote <emote>#0 - sets the power-off emote
	#3noise <volume>#0 - sets the engine's audio volume";
}
