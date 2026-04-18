#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class NetworkAdapterGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3address <text>#0 - sets the preferred local network address for this adapter";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{SpecificBuildingHelpText}";

	public NetworkAdapterGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Network Adapter")
	{
		PreferredNetworkAddress = string.Empty;
	}

	protected NetworkAdapterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string PreferredNetworkAddress { get; protected set; } = string.Empty;
	public override string TypeDescription => "Network Adapter";
	protected override string ComponentDescriptionOLCByline => "This item is a powered network adapter for a computer host";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Preferred Address: {(string.IsNullOrWhiteSpace(PreferredNetworkAddress) ? "automatic".ColourError() : PreferredNetworkAddress.ColourName())}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PreferredNetworkAddress = root.Element("PreferredNetworkAddress")?.Value ?? string.Empty;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("PreferredNetworkAddress", new XCData(PreferredNetworkAddress ?? string.Empty)));
		return root;
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "address":
				return BuildingCommandAddress(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandAddress(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What preferred local network address should this adapter use? Use NONE for automatic addressing.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			PreferredNetworkAddress = string.Empty;
			Changed = true;
			actor.Send("This adapter will now use automatic local addressing.");
			return true;
		}

		PreferredNetworkAddress = command.SafeRemainingArgument.Trim();
		Changed = true;
		actor.Send($"This adapter will now prefer the local network address {PreferredNetworkAddress.ColourName()}.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("networkadapter", true,
			(gameworld, account) => new NetworkAdapterGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("network adapter", false,
			(gameworld, account) => new NetworkAdapterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Network Adapter",
			(proto, gameworld) => new NetworkAdapterGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Network Adapter",
			$"Makes an item a {"[network adapter]".Colour(Telnet.BoldGreen)} {"[powered]".Colour(Telnet.BoldGreen)} telecom-backed network endpoint for a computer host",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new NetworkAdapterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new NetworkAdapterGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new NetworkAdapterGameItemComponentProto(proto, gameworld));
	}
}
