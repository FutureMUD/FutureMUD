#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class NetworkAdapterGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IConnectablePrototype, ICanConnectToTelecommunicationsGridPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3address <text>#0 - sets the preferred local network address for this adapter
	#3public#0 - toggles whether this adapter is visible on the public telecom-backed network
	#3subnet <name|none>#0 - sets an exchange-private subnet for this adapter
	#3vpn add <name>#0 - grants this adapter access to a named VPN
	#3vpn remove <name>#0 - removes a named VPN from this adapter";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{SpecificBuildingHelpText}";

	public NetworkAdapterGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Network Adapter")
	{
		PreferredNetworkAddress = string.Empty;
		PublicNetworkEnabled = true;
		ExchangeSubnetId = string.Empty;
	}

	protected NetworkAdapterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string PreferredNetworkAddress { get; protected set; } = string.Empty;
	public bool PublicNetworkEnabled { get; protected set; }
	public string ExchangeSubnetId { get; protected set; } = string.Empty;
	public List<string> VpnNetworkIds { get; } = [];
	public override string TypeDescription => "Network Adapter";
	protected override string ComponentDescriptionOLCByline => "This item is a powered network adapter for a computer host";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Preferred Address: {(string.IsNullOrWhiteSpace(PreferredNetworkAddress) ? "automatic".ColourError() : PreferredNetworkAddress.ColourName())}\nPublic Network: {PublicNetworkEnabled.ToColouredString()}\nExchange Subnet: {(string.IsNullOrWhiteSpace(ExchangeSubnetId) ? "none".ColourError() : ExchangeSubnetId.ColourName())}\nVPN Memberships: {(VpnNetworkIds.Any() ? VpnNetworkIds.Select(x => x.ColourName()).ListToString() : "none".ColourError())}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PreferredNetworkAddress = root.Element("PreferredNetworkAddress")?.Value ?? string.Empty;
		PublicNetworkEnabled = bool.Parse(root.Element("PublicNetworkEnabled")?.Value ?? "true");
		ExchangeSubnetId = root.Element("ExchangeSubnetId")?.Value ?? string.Empty;
		VpnNetworkIds.Clear();
		VpnNetworkIds.AddRange(root.Element("VpnNetworkIds")?.Elements("Vpn")
			.Select(x => x.Value)
			.Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>());
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("PreferredNetworkAddress", new XCData(PreferredNetworkAddress ?? string.Empty)));
		root.Add(new XElement("PublicNetworkEnabled", PublicNetworkEnabled));
		root.Add(new XElement("ExchangeSubnetId", new XCData(ExchangeSubnetId ?? string.Empty)));
		root.Add(new XElement("VpnNetworkIds",
			from vpn in VpnNetworkIds.OrderBy(x => x)
			select new XElement("Vpn", new XCData(vpn))));
		return root;
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "address":
				return BuildingCommandAddress(actor, command);
			case "public":
				return BuildingCommandPublic(actor);
			case "subnet":
			case "private":
			case "privatesubnet":
				return BuildingCommandSubnet(actor, command);
			case "vpn":
				return BuildingCommandVpn(actor, command);
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

	private bool BuildingCommandPublic(ICharacter actor)
	{
		PublicNetworkEnabled = !PublicNetworkEnabled;
		Changed = true;
		actor.Send($"This adapter will {(PublicNetworkEnabled ? "now".ColourValue() : "no longer".ColourError())} be visible on the public telecom-backed network.");
		return true;
	}

	private bool BuildingCommandSubnet(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What exchange-private subnet should this adapter join? Use NONE to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			ExchangeSubnetId = string.Empty;
			Changed = true;
			actor.Send("This adapter will no longer participate in any exchange-private subnet.");
			return true;
		}

		ExchangeSubnetId = command.SafeRemainingArgument.Trim();
		Changed = true;
		actor.Send($"This adapter will now join the exchange-private subnet {ExchangeSubnetId.ColourName()}.");
		return true;
	}

	private bool BuildingCommandVpn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a VPN membership?");
			return false;
		}

		var action = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.Send("Which VPN should that affect?");
			return false;
		}

		var vpn = command.SafeRemainingArgument.Trim();
		switch (action)
		{
			case "add":
				if (VpnNetworkIds.Any(x => x.EqualTo(vpn)))
				{
					actor.Send($"This adapter is already a member of VPN {vpn.ColourName()}.");
					return false;
				}

				VpnNetworkIds.Add(vpn);
				Changed = true;
				actor.Send($"This adapter now has access to VPN {vpn.ColourName()}.");
				return true;
			case "remove":
			case "delete":
			case "rem":
				if (VpnNetworkIds.RemoveAll(x => x.EqualTo(vpn)) == 0)
				{
					actor.Send($"This adapter is not currently a member of VPN {vpn.ColourName()}.");
					return false;
				}

				Changed = true;
				actor.Send($"This adapter no longer has access to VPN {vpn.ColourName()}.");
				return true;
			default:
				actor.Send("Do you want to add or remove a VPN membership?");
				return false;
		}
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

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
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
