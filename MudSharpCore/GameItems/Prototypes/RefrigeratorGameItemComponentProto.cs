using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class RefrigeratorGameItemComponentProto : ContainerGameItemComponentProto, IConsumePowerPrototype,
	IItemTimeRateModifierPrototype
{
	public override string TypeDescription => "Refrigerator";
	public double PowerUsageInWatts { get; protected set; } = 150.0;
	public double PoweredClosedRate { get; protected set; } = 0.10;
	public double PoweredOpenRate { get; protected set; } = 0.50;
	public double UnpoweredClosedRate { get; protected set; } = 0.75;
	public double UnpoweredOpenRate { get; protected set; } = 1.0;

	protected RefrigeratorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Refrigerator")
	{
		WeightLimit = 100.0 / gameworld.UnitManager.BaseWeightToKilograms;
		MaximumContentsSize = SizeCategory.Normal;
		ContentsPreposition = "in";
		Closable = true;
		Transparent = false;
	}

	protected RefrigeratorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PowerUsageInWatts = double.Parse(root.Element("PowerUsageInWatts")?.Value ?? "150");
		PoweredClosedRate = double.Parse(root.Element("PoweredClosedRate")?.Value ?? "0.1");
		PoweredOpenRate = double.Parse(root.Element("PoweredOpenRate")?.Value ?? "0.5");
		UnpoweredClosedRate = double.Parse(root.Element("UnpoweredClosedRate")?.Value ?? "0.75");
		UnpoweredOpenRate = double.Parse(root.Element("UnpoweredOpenRate")?.Value ?? "1.0");
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(
			new XElement("PowerUsageInWatts", PowerUsageInWatts),
			new XElement("PoweredClosedRate", PoweredClosedRate),
			new XElement("PoweredOpenRate", PoweredOpenRate),
			new XElement("UnpoweredClosedRate", UnpoweredClosedRate),
			new XElement("UnpoweredOpenRate", UnpoweredOpenRate));
		return root.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new RefrigeratorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RefrigeratorGameItemComponent(component, this, parent);
	}

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("refrigerator", true,
			(gameworld, account) => new RefrigeratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("fridge", false,
			(gameworld, account) => new RefrigeratorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Refrigerator",
			(proto, gameworld) => new RefrigeratorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Refrigerator",
			$"A powered {"[container]".Colour(Telnet.BoldGreen)} that slows food freshness, biological decay and opted-in morph timers",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RefrigeratorGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use all normal container options plus:
	wattage <watts> - sets the power draw while connected
	poweredclosed <percentage> - rate while powered and closed
	poweredopen <percentage> - rate while powered and open
	unpoweredclosed <percentage> - rate while unpowered and closed
	unpoweredopen <percentage> - rate while unpowered and open";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n\n{BuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		switch (option)
		{
			case "watt":
			case "watts":
			case "wattage":
				return BuildingCommandWattage(actor, command);
			case "poweredclosed":
			case "powered closed":
				return BuildingCommandRate(actor, command, "powered and closed", x => PoweredClosedRate = x);
			case "poweredopen":
			case "powered open":
				return BuildingCommandRate(actor, command, "powered and open", x => PoweredOpenRate = x);
			case "unpoweredclosed":
			case "unpowered closed":
				return BuildingCommandRate(actor, command, "unpowered and closed", x => UnpoweredClosedRate = x);
			case "unpoweredopen":
			case "unpowered open":
				return BuildingCommandRate(actor, command, "unpowered and open", x => UnpoweredOpenRate = x);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{option}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, actor, out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative number of watts.");
			return false;
		}

		PowerUsageInWatts = value;
		Changed = true;
		actor.Send($"This refrigerator will draw {value.ToString("N2", actor).ColourValue()} watts.");
		return true;
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command, string description, Action<double> setter)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative percentage.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.Send($"The {description} time rate is now {value.ToString("P2", actor).ColourValue()} of normal.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $"{base.ComponentDescriptionOLC(actor)}\nIt draws {PowerUsageInWatts.ToString("N2", actor).ColourValue()} watts. Rates: powered/closed {PoweredClosedRate.ToString("P2", actor).ColourValue()}, powered/open {PoweredOpenRate.ToString("P2", actor).ColourValue()}, unpowered/closed {UnpoweredClosedRate.ToString("P2", actor).ColourValue()}, unpowered/open {UnpoweredOpenRate.ToString("P2", actor).ColourValue()}.";
	}
}
