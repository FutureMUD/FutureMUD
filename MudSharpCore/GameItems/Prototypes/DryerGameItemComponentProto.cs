using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class DryerGameItemComponentProto : ContainerGameItemComponentProto, IConsumePowerPrototype, ISwitchablePrototype,
	IOnOffPrototype, IItemTimeRateModifierPrototype
{
	public override string TypeDescription => "Dryer";
	public double PowerUsageInWatts { get; protected set; } = 2000.0;
	public double DryingMultiplier { get; protected set; } = 10.0;

	protected DryerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Dryer")
	{
		WeightLimit = 10.0 / gameworld.UnitManager.BaseWeightToKilograms;
		MaximumContentsSize = SizeCategory.Normal;
		ContentsPreposition = "in";
		Closable = true;
		Transparent = true;
	}

	protected DryerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PowerUsageInWatts = double.Parse(root.Element("PowerUsageInWatts")?.Value ?? "2000");
		DryingMultiplier = double.Parse(root.Element("DryingMultiplier")?.Value ?? "10");
		Closable = true;
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.SetAttributeValue("Closable", true);
		root.Add(new XElement("PowerUsageInWatts", PowerUsageInWatts),
			new XElement("DryingMultiplier", DryingMultiplier));
		return root.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new DryerGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new DryerGameItemComponent(component, this, parent);

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("dryer", true, (gameworld, account) => new DryerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Dryer", (proto, gameworld) => new DryerGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("Dryer",
			$"A powered, switchable {"[container]".Colour(Telnet.BoldGreen)} that accelerates surface-liquid drying",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new DryerGameItemComponentProto(proto, gameworld));

	private const string BuildingHelpText = @"You can use normal container options except closability, plus:
	wattage <watts> - sets the power draw while running
	drying <multiplier> - sets the surface-liquid drying speed, e.g. 10 for ten times normal";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n\n{BuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		switch (option)
		{
			case "close":
			case "closable":
			case "openable":
				actor.Send("Dryers must have a closable door.");
				return false;
			case "watt":
			case "watts":
			case "wattage":
				return SetNumber(actor, command, "wattage", x => PowerUsageInWatts = x);
			case "dry":
			case "drying":
			case "multiplier":
				return SetNumber(actor, command, "drying multiplier", x => DryingMultiplier = x);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{option}\" {command.RemainingArgument}"));
		}
	}

	private bool SetNumber(ICharacter actor, StringStack command, string description, Action<double> setter)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send($"You must enter a non-negative {description}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.Send($"This dryer's {description} is now {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{base.ComponentDescriptionOLC(actor)}\nIt draws {PowerUsageInWatts.ToString("N2", actor).ColourValue()} watts and dries at {DryingMultiplier.ToString("N2", actor).ColourValue()} times normal speed.";
}
