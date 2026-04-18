#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ComputerHostGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3capacity <bytes>#0 - sets the internal storage capacity for host files
	#3storageports <count>#0 - sets how many storage devices can connect
	#3terminalports <count>#0 - sets how many terminals can connect
	#3networkports <count>#0 - sets how many network adapters can connect";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{SpecificBuildingHelpText}";

	public ComputerHostGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Computer Host")
	{
		StorageCapacityInBytes = 131072;
		StoragePorts = 2;
		TerminalPorts = 1;
		NetworkPorts = 1;
	}

	protected ComputerHostGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long StorageCapacityInBytes { get; protected set; }
	public int StoragePorts { get; protected set; }
	public int TerminalPorts { get; protected set; }
	public int NetworkPorts { get; protected set; }
	public override string TypeDescription => "Computer Host";
	protected override string ComponentDescriptionOLCByline => "This item is a powered in-world computer host";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Internal Storage: {StorageCapacityInBytes.ToString("N0", actor).ColourValue()} bytes\nStorage Ports: {StoragePorts.ToString("N0", actor).ColourValue()}\nTerminal Ports: {TerminalPorts.ToString("N0", actor).ColourValue()}\nNetwork Ports: {NetworkPorts.ToString("N0", actor).ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		StorageCapacityInBytes = long.TryParse(root.Element("StorageCapacityInBytes")?.Value, out var capacity)
			? capacity
			: 131072L;
		StoragePorts = int.TryParse(root.Element("StoragePorts")?.Value, out var storagePorts)
			? storagePorts
			: 2;
		TerminalPorts = int.TryParse(root.Element("TerminalPorts")?.Value, out var terminalPorts)
			? terminalPorts
			: 1;
		NetworkPorts = int.TryParse(root.Element("NetworkPorts")?.Value, out var networkPorts)
			? networkPorts
			: 1;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("StorageCapacityInBytes", StorageCapacityInBytes));
		root.Add(new XElement("StoragePorts", StoragePorts));
		root.Add(new XElement("TerminalPorts", TerminalPorts));
		root.Add(new XElement("NetworkPorts", NetworkPorts));
		return root;
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capacity":
			case "storage":
				return BuildingCommandCapacity(actor, command);
			case "storageports":
			case "drives":
				return BuildingCommandPorts(actor, command, "storage");
			case "terminalports":
			case "terminals":
				return BuildingCommandPorts(actor, command, "terminal");
			case "networkports":
			case "network":
			case "adapters":
				return BuildingCommandPorts(actor, command, "network");
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !long.TryParse(command.PopSpeech(), out var capacity) || capacity < 0)
		{
			actor.Send("How many bytes of internal storage should this host provide?");
			return false;
		}

		StorageCapacityInBytes = capacity;
		Changed = true;
		actor.Send(
			$"This computer host now has {StorageCapacityInBytes.ToString("N0", actor).ColourValue()} bytes of internal storage.");
		return true;
	}

	private bool BuildingCommandPorts(ICharacter actor, StringStack command, string portType)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var ports) || ports < 0)
		{
			actor.Send($"How many {portType} ports should this host have?");
			return false;
		}

		switch (portType)
		{
			case "storage":
				StoragePorts = ports;
				break;
			case "terminal":
				TerminalPorts = ports;
				break;
			default:
				NetworkPorts = ports;
				break;
		}

		Changed = true;
		actor.Send(
			$"This computer host now has {ports.ToString("N0", actor).ColourValue()} {portType.ColourCommand()} {"port".Pluralise(ports != 1)}.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("computerhost", true,
			(gameworld, account) => new ComputerHostGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("computer host", false,
			(gameworld, account) => new ComputerHostGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Computer Host",
			(proto, gameworld) => new ComputerHostGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Computer Host",
			$"Makes an item a {"[computer host]".Colour(Telnet.BoldGreen)} {"[powered]".Colour(Telnet.BoldGreen)} runtime owner for files, executables, and processes",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ComputerHostGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ComputerHostGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ComputerHostGameItemComponentProto(proto, gameworld));
	}
}
