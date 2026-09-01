using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ImplantRefrigeratorGameItemComponentProto : ImplantContainerGameItemComponentProto,
	IItemTimeRateModifierPrototype
{
	public override string TypeDescription => "ImplantRefrigerator";
	public double PoweredClosedRate { get; protected set; } = 0.10;
	public double PoweredOpenRate { get; protected set; } = 0.50;
	public double UnpoweredClosedRate { get; protected set; } = 0.75;
	public double UnpoweredOpenRate { get; protected set; } = 1.0;

	protected ImplantRefrigeratorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ImplantRefrigerator")
	{
	}

	protected ImplantRefrigeratorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PoweredClosedRate = double.Parse(root.Element("PoweredClosedRate")?.Value ?? "0.1");
		PoweredOpenRate = double.Parse(root.Element("PoweredOpenRate")?.Value ?? "0.5");
		UnpoweredClosedRate = double.Parse(root.Element("UnpoweredClosedRate")?.Value ?? "0.75");
		UnpoweredOpenRate = double.Parse(root.Element("UnpoweredOpenRate")?.Value ?? "1.0");
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("PoweredClosedRate", PoweredClosedRate),
			new XElement("PoweredOpenRate", PoweredOpenRate),
			new XElement("UnpoweredClosedRate", UnpoweredClosedRate),
			new XElement("UnpoweredOpenRate", UnpoweredOpenRate));
		return root.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new ImplantRefrigeratorGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new ImplantRefrigeratorGameItemComponent(component, this, parent);

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("implantrefrigerator", true,
			(gameworld, account) => new ImplantRefrigeratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("implantfridge", false,
			(gameworld, account) => new ImplantRefrigeratorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantRefrigerator",
			(proto, gameworld) => new ImplantRefrigeratorGameItemComponentProto(proto, gameworld));
		manager.AddFuturisticTypeHelpInfo("ImplantRefrigerator",
			$"An implantable {"[container]".Colour(Telnet.BoldGreen)} with refrigeration mechanics", BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator,
		(proto, gameworld) => new ImplantRefrigeratorGameItemComponentProto(proto, gameworld));

	private const string BuildingHelpText = @"Use all implant-container options plus:
	poweredclosed <percentage>
	poweredopen <percentage>
	unpoweredclosed <percentage>
	unpoweredopen <percentage>";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n\n{BuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		return option switch
		{
			"poweredclosed" or "powered closed" => SetRate(actor, command, "powered and closed", x => PoweredClosedRate = x),
			"poweredopen" or "powered open" => SetRate(actor, command, "powered and open", x => PoweredOpenRate = x),
			"unpoweredclosed" or "unpowered closed" => SetRate(actor, command, "unpowered and closed", x => UnpoweredClosedRate = x),
			"unpoweredopen" or "unpowered open" => SetRate(actor, command, "unpowered and open", x => UnpoweredOpenRate = x),
			_ => base.BuildingCommand(actor, new StringStack($"\"{option}\" {command.RemainingArgument}"))
		};
	}

	private bool SetRate(ICharacter actor, StringStack command, string description, Action<double> setter)
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

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{base.ComponentDescriptionOLC(actor)} Rates: powered/closed {PoweredClosedRate.ToString("P2", actor).ColourValue()}, powered/open {PoweredOpenRate.ToString("P2", actor).ColourValue()}, unpowered/closed {UnpoweredClosedRate.ToString("P2", actor).ColourValue()}, unpowered/open {UnpoweredOpenRate.ToString("P2", actor).ColourValue()}.";
}
