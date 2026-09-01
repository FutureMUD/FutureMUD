using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class PowerBankGameItemComponentProto : ConnectableGameItemComponentProto, IProducePowerPrototype,
	IConsumePowerPrototype
{
	public override string TypeDescription => "PowerBank";
	public double CapacityInWattHours { get; protected set; } = 40.0;
	public double MaximumInputInWatts { get; protected set; } = 18.0;
	public double MaximumOutputInWatts { get; protected set; } = 18.0;
	public double ChargingEfficiency { get; protected set; } = 0.90;
	public List<ConnectorType> InputConnections { get; } = [];
	public List<ConnectorType> OutputConnections { get; } = [];

	protected PowerBankGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "PowerBank")
	{
	}

	protected PowerBankGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		CapacityInWattHours = double.Parse(root.Element("CapacityInWattHours")?.Value ?? "40");
		MaximumInputInWatts = double.Parse(root.Element("MaximumInputInWatts")?.Value ?? "18");
		MaximumOutputInWatts = double.Parse(root.Element("MaximumOutputInWatts")?.Value ?? "18");
		ChargingEfficiency = double.Parse(root.Element("ChargingEfficiency")?.Value ?? "0.9");
		InputConnections.Clear();
		OutputConnections.Clear();
		LoadConnections(root.Element("InputConnectors"), InputConnections);
		LoadConnections(root.Element("OutputConnectors"), OutputConnections);
		Connections.Clear();
		Connections.AddRange(InputConnections);
		Connections.AddRange(OutputConnections);
	}

	private static void LoadConnections(XElement? root, ICollection<ConnectorType> target)
	{
		if (root is null)
		{
			return;
		}

		foreach (var element in root.Elements("Connection"))
		{
			target.Add(new ConnectorType((Gender)Convert.ToSByte(element.Attribute("gender")!.Value),
				element.Attribute("type")!.Value, true));
		}
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("CapacityInWattHours", CapacityInWattHours),
			new XElement("MaximumInputInWatts", MaximumInputInWatts),
			new XElement("MaximumOutputInWatts", MaximumOutputInWatts),
			new XElement("ChargingEfficiency", ChargingEfficiency),
			SaveConnections("InputConnectors", InputConnections),
			SaveConnections("OutputConnectors", OutputConnections));
		return root.ToString();
	}

	private static XElement SaveConnections(string name, IEnumerable<ConnectorType> connections) =>
		new(name, connections.Select(x => new XElement("Connection",
			new XAttribute("gender", (short)x.Gender), new XAttribute("type", x.ConnectionType))));

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new PowerBankGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new PowerBankGameItemComponent(component, this, parent);

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("powerbank", true,
			(gameworld, account) => new PowerBankGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("power bank", false,
			(gameworld, account) => new PowerBankGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PowerBank", (proto, gameworld) => new PowerBankGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("PowerBank", "An integrated rechargeable power source for connected items", BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new PowerBankGameItemComponentProto(proto, gameworld));

	private const string BuildingHelpText = @"You can use the following options:
	capacity <watt-hours>
	inputwatts <watts>
	outputwatts <watts>
	efficiency <percentage>
	input add|remove <male|female> <type>
	output add|remove <male|female> <type>";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		switch (option)
		{
			case "capacity":
				return SetPositive(actor, command, "capacity in watt-hours", x => CapacityInWattHours = x);
			case "inputwatts":
			case "input watts":
			case "charge":
				return SetPositive(actor, command, "maximum input wattage", x => MaximumInputInWatts = x);
			case "outputwatts":
			case "output watts":
			case "outputpower":
				return SetPositive(actor, command, "maximum output wattage", x => MaximumOutputInWatts = x);
			case "efficiency":
				return SetEfficiency(actor, command);
			case "input":
				return SetConnection(actor, command, InputConnections, "input");
			case "output":
				return SetConnection(actor, command, OutputConnections, "output");
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{option}\" {command.RemainingArgument}"));
		}
	}

	private bool SetPositive(ICharacter actor, StringStack command, string description, Action<double> setter)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send($"You must enter a positive {description}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.Send($"This power bank's {description} is now {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool SetEfficiency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) || value is <= 0.0 or > 1.0)
		{
			actor.Send("You must enter an efficiency greater than 0% and no more than 100%.");
			return false;
		}

		ChargingEfficiency = value;
		Changed = true;
		actor.Send($"This power bank's charging efficiency is now {value.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool SetConnection(ICharacter actor, StringStack command, List<ConnectorType> target, string direction)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var gendering = Gendering.Get(command.PopSpeech());
		if (gendering.Enum is not (Form.Shape.Gender.Male or Form.Shape.Gender.Female) || command.IsFinished)
		{
			actor.Send($"Syntax: {direction} add|remove <male|female> <type>".ColourCommand());
			return false;
		}

		var type = command.SafeRemainingArgument;
		var existing = target.FirstOrDefault(x => x.Gender == gendering.Enum && x.ConnectionType.EqualTo(type));
		if (action.EqualTo("remove") || action.EqualTo("delete"))
		{
			if (existing is null)
			{
				actor.Send("There is no such connector to remove.");
				return false;
			}
			target.Remove(existing);
			Connections.Remove(existing);
		}
		else if (action.EqualTo("add"))
		{
			if (existing is not null)
			{
				actor.Send("That connector already exists.");
				return false;
			}
			var connector = new ConnectorType(gendering.Enum, type, true);
			target.Add(connector);
			Connections.Add(connector);
		}
		else
		{
			actor.Send($"Syntax: {direction} add|remove <male|female> <type>".ColourCommand());
			return false;
		}

		Changed = true;
		actor.Send($"The {direction} connector list has been updated.");
		return true;
	}

	public override bool CanSubmit() => InputConnections.Any() && OutputConnections.Any() && base.CanSubmit();
	public override string WhyCannotSubmit() => !InputConnections.Any() ? "You must add an input connector."
		: !OutputConnections.Any() ? "You must add an output connector." : base.WhyCannotSubmit();

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{base.ComponentDescriptionOLC(actor)}\nCapacity {CapacityInWattHours.ToString("N2", actor).ColourValue()} Wh; input {MaximumInputInWatts.ToString("N2", actor).ColourValue()} W; output {MaximumOutputInWatts.ToString("N2", actor).ColourValue()} W; efficiency {ChargingEfficiency.ToString("P2", actor).ColourValue()}. Inputs: {InputConnections.Select(x => x.ToString().ColourCommand()).ListToString()}. Outputs: {OutputConnections.Select(x => x.ToString().ColourCommand()).ListToString()}.";
}
