#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class NetworkSwitchGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IConnectablePrototype, ICanConnectToTelecommunicationsGridPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3ports <##>#0 - sets how many downstream network switch ports this switch exposes";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{SpecificBuildingHelpText}";

	public NetworkSwitchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Network Switch")
	{
		PortCount = 8;
		Wattage = 25.0;
	}

	protected NetworkSwitchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public int PortCount { get; protected set; }

	public IEnumerable<ConnectorType> Connections =>
		Enumerable.Repeat(ComputerConnectionTypes.NetworkUplinkSocket, PortCount)
			.Concat([ComputerConnectionTypes.NetworkUplinkPlug])
			.ToList();

	public override string TypeDescription => "Network Switch";

	protected override string ComponentDescriptionOLCByline =>
		"This item is a powered daisy-chainable telecommunications network switch";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Downstream Ports: {PortCount.ToString("N0", actor).ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PortCount = int.Parse(root.Element("PortCount")?.Value ?? "8");
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("PortCount", PortCount));
		return root;
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ports":
			case "port":
				return BuildingCommandPorts(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandPorts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many downstream network ports should this switch expose?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.Send("You must enter a whole number of ports greater than zero.");
			return false;
		}

		PortCount = value;
		Changed = true;
		actor.Send($"This network switch will now expose {PortCount.ToString("N0", actor).ColourValue()} downstream ports.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("networkswitch", true,
			(gameworld, account) => new NetworkSwitchGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("network switch", false,
			(gameworld, account) => new NetworkSwitchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Network Switch",
			(proto, gameworld) => new NetworkSwitchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Network Switch",
			$"Makes an item a {"[powered]".Colour(Telnet.BoldGreen)} daisy-chainable telecommunications switch",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new NetworkSwitchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new NetworkSwitchGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new NetworkSwitchGameItemComponentProto(proto, gameworld));
	}
}
