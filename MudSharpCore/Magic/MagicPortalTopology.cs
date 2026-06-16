#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DB = MudSharp.Models;

namespace MudSharp.Magic;

public class MagicPortalNetwork : SaveableItem, IMagicPortalNetwork
{
	private readonly List<MagicPortalEndpoint> _endpoints = new();
	private readonly List<MagicPortalLink> _links = new();
	private long? _schoolId;
	private IMagicSchool? _school;

	public MagicPortalNetwork(DB.MagicPortalNetwork dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_schoolId = dbitem.MagicSchoolId;
		IsActive = dbitem.IsActive;
		AllowCrossZone = dbitem.AllowCrossZone;
		Verb = dbitem.Verb;
		OutboundKeyword = dbitem.OutboundKeyword;
		InboundKeyword = dbitem.InboundKeyword;
		OutboundTarget = dbitem.OutboundTarget;
		InboundTarget = dbitem.InboundTarget;
		OutboundDescription = dbitem.OutboundDescription;
		InboundDescription = dbitem.InboundDescription;
		TimeMultiplier = dbitem.TimeMultiplier <= 0.0 ? 1.0 : dbitem.TimeMultiplier;

		foreach (var endpoint in dbitem.MagicPortalEndpoints.OrderBy(x => x.Id))
		{
			_endpoints.Add(new MagicPortalEndpoint(this, endpoint));
		}

		foreach (var link in dbitem.MagicPortalLinks.OrderBy(x => x.Id))
		{
			var source = _endpoints.FirstOrDefault(x => x.Id == link.SourceEndpointId);
			var destination = _endpoints.FirstOrDefault(x => x.Id == link.DestinationEndpointId);
			if (source is null || destination is null)
			{
				continue;
			}

			_links.Add(new MagicPortalLink(this, link, source, destination));
		}
	}

	public MagicPortalNetwork(IFuturemud gameworld, string name, IMagicSchool? school, ICharacter? creator = null)
	{
		Gameworld = gameworld;
		_name = name;
		_school = school;
		_schoolId = school?.Id;
		IsActive = true;
		AllowCrossZone = false;
		Verb = "enter";
		OutboundKeyword = "portal";
		InboundKeyword = "portal";
		OutboundTarget = "a standing portal";
		InboundTarget = "a standing portal";
		OutboundDescription = "through";
		InboundDescription = "through";
		TimeMultiplier = 1.0;

		using (new FMDB())
		{
			var dbitem = new DB.MagicPortalNetwork
			{
				Name = Name,
				MagicSchoolId = _schoolId,
				IsActive = IsActive,
				AllowCrossZone = AllowCrossZone,
				Verb = Verb,
				OutboundKeyword = OutboundKeyword,
				InboundKeyword = InboundKeyword,
				OutboundTarget = OutboundTarget,
				InboundTarget = InboundTarget,
				OutboundDescription = OutboundDescription,
				InboundDescription = InboundDescription,
				TimeMultiplier = TimeMultiplier,
				CreatedByCharacterId = CharacterInstanceIdentityComparer.IdentityId(creator),
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.MagicPortalNetworks.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "MagicPortalNetwork";
	public IMagicSchool? School => _school ??= _schoolId.HasValue ? Gameworld.MagicSchools.Get(_schoolId.Value) : null;
	public bool IsActive { get; private set; }
	public bool AllowCrossZone { get; private set; }
	public string Verb { get; private set; }
	public string OutboundKeyword { get; private set; }
	public string InboundKeyword { get; private set; }
	public string OutboundTarget { get; private set; }
	public string InboundTarget { get; private set; }
	public string OutboundDescription { get; private set; }
	public string InboundDescription { get; private set; }
	public double TimeMultiplier { get; private set; }
	public IEnumerable<IMagicPortalEndpoint> Endpoints => _endpoints;
	public IEnumerable<IMagicPortalLink> Links => _links;

	internal MagicPortalEndpoint? EndpointByKeyOrId(string text)
	{
		var byKey = _endpoints.FirstOrDefault(x => x.Key.EqualTo(text) || x.Name.EqualTo(text));
		if (byKey is not null)
		{
			return byKey;
		}

		return long.TryParse(text, out var id) ? _endpoints.FirstOrDefault(x => x.Id == id) : null;
	}

	internal void AddEndpoint(MagicPortalEndpoint endpoint)
	{
		_endpoints.Add(endpoint);
	}

	internal void AddLink(MagicPortalLink link)
	{
		_links.Add(link);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.MagicPortalNetworks.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Name = Name;
		dbitem.MagicSchoolId = _schoolId;
		dbitem.IsActive = IsActive;
		dbitem.AllowCrossZone = AllowCrossZone;
		dbitem.Verb = Verb;
		dbitem.OutboundKeyword = OutboundKeyword;
		dbitem.InboundKeyword = InboundKeyword;
		dbitem.OutboundTarget = OutboundTarget;
		dbitem.InboundTarget = InboundTarget;
		dbitem.OutboundDescription = OutboundDescription;
		dbitem.InboundDescription = InboundDescription;
		dbitem.TimeMultiplier = TimeMultiplier;
		Changed = false;
	}

	public const string HelpText = @"You can use the following options with this portal network:

	#3name <name>#0 - renames this network
	#3school <which|none>#0 - sets or clears the associated magic school
	#3active#0 - toggles whether this network materialises exits
	#3crosszone#0 - toggles whether links can cross zones
	#3verb <verb>#0 - sets the movement verb
	#3outkey <keyword>#0 - sets the outbound command keyword
	#3inkey <keyword>#0 - sets the inbound command keyword
	#3outtarget <text>#0 - sets the outbound portal target text
	#3intarget <text>#0 - sets the inbound portal target text
	#3outdesc <text>#0 - sets the outbound movement preposition
	#3indesc <text>#0 - sets the inbound movement preposition
	#3speed <multiplier>#0 - sets the movement time multiplier
	#3endpoint add room <key> <cell|here> [name]#0 - adds or replaces a room endpoint
	#3endpoint add item <key> <item id> [name]#0 - adds or replaces a directly placed item endpoint
	#3endpoint remove <key|id>#0 - removes an endpoint and its links
	#3endpoint active <key|id>#0 - toggles an endpoint
	#3endpoint key <key|id> <new key>#0 - changes an endpoint key
	#3link add <from> <to>#0 - adds an explicit bidirectional link
	#3link remove <id|from to>#0 - removes a link
	#3link active <id>#0 - toggles a link
	#3refresh#0 - rebuilds the runtime transient exits";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "school":
				return BuildingCommandSchool(actor, command);
			case "active":
				IsActive = !IsActive;
				Changed = true;
				new MagicPortalTopologyService().RebuildNetwork(this);
				actor.OutputHandler.Send($"This portal network is now {(IsActive ? "active" : "inactive").ColourValue()}.");
				return true;
			case "crosszone":
				AllowCrossZone = !AllowCrossZone;
				Changed = true;
				new MagicPortalTopologyService().RebuildNetwork(this);
				actor.OutputHandler.Send($"This portal network will {(AllowCrossZone ? "now" : "no longer").ColourValue()} allow cross-zone links.");
				return true;
			case "verb":
				return BuildingCommandText(actor, command, "movement verb", x => Verb = x, true);
			case "outkey":
				return BuildingCommandText(actor, command, "outbound keyword", x => OutboundKeyword = x.CollapseString(), true);
			case "inkey":
				return BuildingCommandText(actor, command, "inbound keyword", x => InboundKeyword = x.CollapseString(), true);
			case "outtarget":
				return BuildingCommandText(actor, command, "outbound target text", x => OutboundTarget = x, true);
			case "intarget":
				return BuildingCommandText(actor, command, "inbound target text", x => InboundTarget = x, true);
			case "outdesc":
				return BuildingCommandText(actor, command, "outbound movement preposition", x => OutboundDescription = x, true);
			case "indesc":
				return BuildingCommandText(actor, command, "inbound movement preposition", x => InboundDescription = x, true);
			case "speed":
				return BuildingCommandSpeed(actor, command);
			case "endpoint":
				return BuildingCommandEndpoint(actor, command);
			case "link":
				return BuildingCommandLink(actor, command);
			case "refresh":
			case "repair":
				new MagicPortalTopologyService().RebuildNetwork(this);
				actor.OutputHandler.Send("This portal network has been refreshed.");
				return true;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this portal network to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicPortalNetworks.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a portal network called {name.ColourName()}.");
			return false;
		}

		actor.OutputHandler.Send($"You rename portal network {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic school should this portal network be associated with, or #3none#0?".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_school = null;
			_schoolId = null;
			Changed = true;
			actor.OutputHandler.Send("This portal network is no longer associated with any magic school.");
			return true;
		}

		var school = Gameworld.MagicSchools.GetByIdOrName(command.SafeRemainingArgument);
		if (school is null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		_school = school;
		_schoolId = school.Id;
		Changed = true;
		actor.OutputHandler.Send($"This portal network is now associated with {school.Name.Colour(school.PowerListColour)}.");
		return true;
	}

	private bool BuildingCommandText(ICharacter actor, StringStack command, string label, Action<string> setter,
		bool rebuild)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the {label} be?");
			return false;
		}

		setter(command.SafeRemainingArgument);
		Changed = true;
		if (rebuild)
		{
			new MagicPortalTopologyService().RebuildNetwork(this);
		}

		actor.OutputHandler.Send($"The portal network {label} has been updated.");
		return true;
	}

	private bool BuildingCommandSpeed(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var speed) || speed <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive movement time multiplier.");
			return false;
		}

		TimeMultiplier = speed;
		Changed = true;
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"This portal network now has a movement time multiplier of {speed.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEndpoint(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				return BuildingCommandEndpointAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandEndpointRemove(actor, command);
			case "active":
				return BuildingCommandEndpointActive(actor, command);
			case "key":
				return BuildingCommandEndpointKey(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandEndpointAdd(ICharacter actor, StringStack command)
	{
		var typeText = command.PopForSwitch();
		if (!typeText.EqualToAny("room", "cell", "item"))
		{
			actor.OutputHandler.Send("You must specify whether you are adding a #3room#0 or #3item#0 endpoint.".SubstituteANSIColour());
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What key should this endpoint use?");
			return false;
		}

		var key = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which room/cell or item should this endpoint use?");
			return false;
		}

		var targetText = command.PopSpeech();
		var name = command.IsFinished ? key.TitleCase() : command.SafeRemainingArgument.TitleCase();
		ICell? cell = null;
		IGameItem? item = null;
		var endpointType = typeText.EqualToAny("room", "cell") ? MagicPortalEndpointType.Cell : MagicPortalEndpointType.Item;
		if (endpointType == MagicPortalEndpointType.Cell)
		{
			cell = targetText.EqualTo("here") ? actor.Location : long.TryParse(targetText, out var id) ? Gameworld.Cells.Get(id) : null;
			if (cell is null)
			{
				actor.OutputHandler.Send("There is no such cell.");
				return false;
			}
		}
		else
		{
			if (!long.TryParse(targetText, out var id))
			{
				actor.OutputHandler.Send("You must specify an item ID for item portal endpoints.");
				return false;
			}

			item = Gameworld.TryGetItem(id, true);
			if (item is null)
			{
				actor.OutputHandler.Send("There is no such item.");
				return false;
			}
		}

		var endpoint = new MagicPortalTopologyService().CreateOrUpdateEndpoint(actor, this, key, name, endpointType,
			cell, item, true, null, out var reason);
		if (endpoint is null)
		{
			actor.OutputHandler.Send(reason.ColourError());
			return false;
		}

		actor.OutputHandler.Send($"Endpoint {endpoint.Key.ColourName()} is now bound to {DescribeEndpointTarget(endpoint, actor)}.");
		return true;
	}

	private bool BuildingCommandEndpointRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which endpoint do you want to remove?");
			return false;
		}

		var endpoint = EndpointByKeyOrId(command.SafeRemainingArgument);
		if (endpoint is null)
		{
			actor.OutputHandler.Send("There is no such endpoint.");
			return false;
		}

		DeleteEndpoint(endpoint);
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"Endpoint {endpoint.Key.ColourName()} has been removed.");
		return true;
	}

	private bool BuildingCommandEndpointActive(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which endpoint do you want to toggle?");
			return false;
		}

		var endpoint = EndpointByKeyOrId(command.SafeRemainingArgument);
		if (endpoint is null)
		{
			actor.OutputHandler.Send("There is no such endpoint.");
			return false;
		}

		endpoint.SetActive(!endpoint.IsActive);
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"Endpoint {endpoint.Key.ColourName()} is now {(endpoint.IsActive ? "active" : "inactive").ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEndpointKey(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.OutputHandler.Send("You must specify the endpoint and its new key.");
			return false;
		}

		var endpoint = EndpointByKeyOrId(command.PopSpeech());
		if (endpoint is null)
		{
			actor.OutputHandler.Send("There is no such endpoint.");
			return false;
		}

		var newKey = command.SafeRemainingArgument.ToLowerInvariant();
		if (_endpoints.Any(x => x.Id != endpoint.Id && x.Key.EqualTo(newKey)))
		{
			actor.OutputHandler.Send($"There is already an endpoint with the key {newKey.ColourCommand()}.");
			return false;
		}

		endpoint.SetKey(newKey);
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"Endpoint {endpoint.Name.ColourName()} now uses the key {newKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandLink(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				return BuildingCommandLinkAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandLinkRemove(actor, command);
			case "active":
				return BuildingCommandLinkActive(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandLinkAdd(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.OutputHandler.Send("You must specify the two endpoints to link.");
			return false;
		}

		var source = EndpointByKeyOrId(command.PopSpeech());
		var destination = EndpointByKeyOrId(command.SafeRemainingArgument);
		if (source is null || destination is null)
		{
			actor.OutputHandler.Send("One or both of those endpoints do not exist.");
			return false;
		}

		var link = new MagicPortalTopologyService().CreateLink(actor, this, source, destination, null, out var reason);
		if (link is null)
		{
			actor.OutputHandler.Send(reason.ColourError());
			return false;
		}

		actor.OutputHandler.Send($"Endpoint {source.Key.ColourName()} is now linked to {destination.Key.ColourName()}.");
		return true;
	}

	private bool BuildingCommandLinkRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which link do you want to remove?");
			return false;
		}

		var first = command.PopSpeech();
		MagicPortalLink? link;
		if (long.TryParse(first, out var id))
		{
			link = _links.FirstOrDefault(x => x.Id == id);
		}
		else
		{
			var source = EndpointByKeyOrId(first);
			var destination = EndpointByKeyOrId(command.SafeRemainingArgument);
			link = source is null || destination is null ? null : LinkBetween(source, destination);
		}

		if (link is null)
		{
			actor.OutputHandler.Send("There is no such link.");
			return false;
		}

		DeleteLink(link);
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"Link #{link.Id.ToString("N0", actor)} has been removed.");
		return true;
	}

	private bool BuildingCommandLinkActive(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send("Which link ID do you want to toggle?");
			return false;
		}

		var link = _links.FirstOrDefault(x => x.Id == id);
		if (link is null)
		{
			actor.OutputHandler.Send("There is no such link.");
			return false;
		}

		link.SetActive(!link.IsActive);
		new MagicPortalTopologyService().RebuildNetwork(this);
		actor.OutputHandler.Send($"Link #{link.Id.ToString("N0", actor)} is now {(link.IsActive ? "active" : "inactive").ColourValue()}.");
		return true;
	}

	internal MagicPortalLink? LinkBetween(MagicPortalEndpoint source, MagicPortalEndpoint destination)
	{
		return _links.FirstOrDefault(x =>
			x.SourceEndpoint.Id == source.Id && x.DestinationEndpoint.Id == destination.Id ||
			x.SourceEndpoint.Id == destination.Id && x.DestinationEndpoint.Id == source.Id);
	}

	internal void DeleteEndpoint(MagicPortalEndpoint endpoint)
	{
		foreach (var link in _links.Where(x => x.SourceEndpoint.Id == endpoint.Id || x.DestinationEndpoint.Id == endpoint.Id).ToList())
		{
			DeleteLink(link);
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalEndpoints.Find(endpoint.Id);
			if (dbitem is not null)
			{
				FMDB.Context.MagicPortalEndpoints.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_endpoints.Remove(endpoint);
	}

	internal void DeleteLink(MagicPortalLink link)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalLinks.Find(link.Id);
			if (dbitem is not null)
			{
				FMDB.Context.MagicPortalLinks.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_links.Remove(link);
	}

	private static string DescribeEndpointTarget(IMagicPortalEndpoint endpoint, IPerceiver voyeur)
	{
		return endpoint.EndpointType switch
		{
			MagicPortalEndpointType.Cell => endpoint.CurrentCell is null
				? $"missing cell #{endpoint.CellId?.ToString("N0", voyeur) ?? "0"}".ColourError()
				: $"room #{endpoint.CurrentCell.Id.ToString("N0", voyeur)} {endpoint.CurrentCell.Name.ColourName()}",
			MagicPortalEndpointType.Item => endpoint.CurrentCell is null
				? $"item #{endpoint.GameItemId?.ToString("N0", voyeur) ?? "0"} not directly placed".ColourError()
				: $"item #{endpoint.GameItemId?.ToString("N0", voyeur) ?? "0"} in room #{endpoint.CurrentCell.Id.ToString("N0", voyeur)} {endpoint.CurrentCell.Name.ColourName()}",
			_ => "unknown".ColourError()
		};
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Portal Network #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"School: {School?.Name.Colour(School.PowerListColour) ?? "None".ColourError()}");
		sb.AppendLine($"Active: {IsActive.ToColouredString()}");
		sb.AppendLine($"Cross Zone: {AllowCrossZone.ToColouredString()}");
		sb.AppendLine($"Command: {$"{Verb} {OutboundKeyword}".ColourCommand()} / {$"{Verb} {InboundKeyword}".ColourCommand()}");
		sb.AppendLine($"Targets: {OutboundTarget.ColourName()} / {InboundTarget.ColourName()}");
		sb.AppendLine($"Descriptions: {OutboundDescription.ColourCommand()} / {InboundDescription.ColourCommand()}");
		sb.AppendLine($"Speed: {TimeMultiplier.ToString("N2", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Endpoints".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine(_endpoints.Any()
			? StringUtilities.GetTextTable(
				from endpoint in _endpoints.OrderBy(x => x.Key)
				select new List<string>
				{
					endpoint.Id.ToString("N0", actor),
					endpoint.Key,
					endpoint.Name,
					endpoint.EndpointType.DescribeEnum(),
					endpoint.IsActive.ToColouredString(),
					DescribeEndpointTarget(endpoint, actor),
					string.IsNullOrWhiteSpace(endpoint.WhyInvalid) ? "valid".Colour(Telnet.Green) : endpoint.WhyInvalid.ColourError()
				},
				new List<string> { "Id", "Key", "Name", "Type", "Active", "Target", "Status" },
				actor,
				Telnet.Cyan)
			: "No endpoints defined.");
		sb.AppendLine();
		sb.AppendLine("Links".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine(_links.Any()
			? StringUtilities.GetTextTable(
				from link in _links.OrderBy(x => x.SourceEndpoint.Key).ThenBy(x => x.DestinationEndpoint.Key)
				select new List<string>
				{
					link.Id.ToString("N0", actor),
					link.SourceEndpoint.Key,
					link.DestinationEndpoint.Key,
					link.IsActive.ToColouredString(),
					string.IsNullOrWhiteSpace(link.WhyInvalid) ? "valid".Colour(Telnet.Green) : link.WhyInvalid.ColourError()
				},
				new List<string> { "Id", "From", "To", "Active", "Status" },
				actor,
				Telnet.Cyan)
			: "No links defined.");
		return sb.ToString();
	}
}

public class MagicPortalEndpoint : FrameworkItem, IMagicPortalEndpoint
{
	private readonly MagicPortalNetwork _network;

	public MagicPortalEndpoint(MagicPortalNetwork network, DB.MagicPortalEndpoint dbitem)
	{
		_network = network;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Key = dbitem.Key;
		EndpointType = (MagicPortalEndpointType)dbitem.AnchorType;
		CellId = dbitem.CellId;
		GameItemId = dbitem.GameItemId;
		IsActive = dbitem.IsActive;
	}

	public MagicPortalEndpoint(MagicPortalNetwork network, DB.MagicPortalEndpoint dbitem, string key, string name,
		MagicPortalEndpointType endpointType, long? cellId, long? gameItemId)
	{
		_network = network;
		_id = dbitem.Id;
		_name = name;
		Key = key;
		EndpointType = endpointType;
		CellId = cellId;
		GameItemId = gameItemId;
		IsActive = true;
	}

	public override string FrameworkItemType => "MagicPortalEndpoint";
	public IFuturemud Gameworld => _network.Gameworld;
	public IMagicPortalNetwork Network => _network;
	public string Key { get; private set; }
	public MagicPortalEndpointType EndpointType { get; private set; }
	public long? CellId { get; private set; }
	public long? GameItemId { get; private set; }
	public bool IsActive { get; private set; }

	public ICell? CurrentCell
	{
		get
		{
			if (!IsActive)
			{
				return null;
			}

			return EndpointType switch
			{
				MagicPortalEndpointType.Cell => CellId.HasValue ? Gameworld.Cells.Get(CellId.Value) : null,
				MagicPortalEndpointType.Item => GameItemId.HasValue ? DirectCellForItem(Gameworld.TryGetItem(GameItemId.Value, true)) : null,
				_ => null
			};
		}
	}

	public string WhyInvalid
	{
		get
		{
			if (!IsActive)
			{
				return "endpoint is inactive";
			}

			if (EndpointType == MagicPortalEndpointType.Item)
			{
				if (!GameItemId.HasValue)
				{
					return "item endpoint has no item id";
				}

				var item = Gameworld.TryGetItem(GameItemId.Value, true);
				if (item is null)
				{
					return "the target item is missing";
				}

				return DirectCellForItem(item) is null ? "the target item is not directly located in a room" : string.Empty;
			}

			return EndpointType switch
			{
				MagicPortalEndpointType.Cell when !CellId.HasValue => "cell endpoint has no cell id",
				MagicPortalEndpointType.Cell when Gameworld.Cells.Get(CellId!.Value) is null => "the target cell is missing",
				_ => string.Empty
			};
		}
	}

	private static ICell? DirectCellForItem(IGameItem? item)
	{
		if (item is null || item.ContainedIn is not null || item.InInventoryOf is not null)
		{
			return null;
		}

		var cell = item.Location;
		if (cell is null)
		{
			return null;
		}

		return cell.GameItems.Any(x => ReferenceEquals(x, item) || x.Id == item.Id) ? cell : null;
	}

	public void Update(string key, string name, MagicPortalEndpointType endpointType, long? cellId, long? gameItemId)
	{
		Key = key;
		_name = name;
		EndpointType = endpointType;
		CellId = cellId;
		GameItemId = gameItemId;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalEndpoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.Key = Key;
				dbitem.Name = Name;
				dbitem.AnchorType = (int)EndpointType;
				dbitem.CellId = CellId;
				dbitem.GameItemId = GameItemId;
				dbitem.IsActive = IsActive;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void SetActive(bool value)
	{
		IsActive = value;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalEndpoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsActive = IsActive;
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void SetKey(string key)
	{
		Key = key;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalEndpoints.Find(Id);
			if (dbitem is not null)
			{
				dbitem.Key = Key;
				FMDB.Context.SaveChanges();
			}
		}
	}
}

public class MagicPortalLink : FrameworkItem, IMagicPortalLink
{
	private readonly MagicPortalNetwork _network;

	public MagicPortalLink(MagicPortalNetwork network, DB.MagicPortalLink dbitem, MagicPortalEndpoint source,
		MagicPortalEndpoint destination)
	{
		_network = network;
		_id = dbitem.Id;
		_name = $"{source.Key} <-> {destination.Key}";
		SourceEndpoint = source;
		DestinationEndpoint = destination;
		IsActive = dbitem.IsActive;
	}

	public override string FrameworkItemType => "MagicPortalLink";
	public IFuturemud Gameworld => _network.Gameworld;
	public IMagicPortalNetwork Network => _network;
	public MagicPortalEndpoint SourceEndpoint { get; }
	public MagicPortalEndpoint DestinationEndpoint { get; }
	IMagicPortalEndpoint IMagicPortalLink.SourceEndpoint => SourceEndpoint;
	IMagicPortalEndpoint IMagicPortalLink.DestinationEndpoint => DestinationEndpoint;
	public bool IsActive { get; private set; }

	public string WhyInvalid
	{
		get
		{
			if (!_network.IsActive)
			{
				return "network is inactive";
			}

			if (!IsActive)
			{
				return "link is inactive";
			}

			if (!string.IsNullOrWhiteSpace(SourceEndpoint.WhyInvalid))
			{
				return $"source endpoint {SourceEndpoint.Key}: {SourceEndpoint.WhyInvalid}";
			}

			if (!string.IsNullOrWhiteSpace(DestinationEndpoint.WhyInvalid))
			{
				return $"destination endpoint {DestinationEndpoint.Key}: {DestinationEndpoint.WhyInvalid}";
			}

			var source = SourceEndpoint.CurrentCell;
			var destination = DestinationEndpoint.CurrentCell;
			if (source is null || destination is null)
			{
				return "one or both endpoint rooms are unavailable";
			}

			if (source.Id == destination.Id)
			{
				return "source and destination resolve to the same room";
			}

			if (!_network.AllowCrossZone && !ReferenceEquals(source.Zone, destination.Zone))
			{
				return "cross-zone links are disabled";
			}

			if (!source.CanInteractPlanar(destination, PlanarInteractionKind.Magic))
			{
				return "the endpoint rooms cannot interact across planes";
			}

			return string.Empty;
		}
	}

	public void SetActive(bool value)
	{
		IsActive = value;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.MagicPortalLinks.Find(Id);
			if (dbitem is not null)
			{
				dbitem.IsActive = IsActive;
				FMDB.Context.SaveChanges();
			}
		}
	}
}

public class MagicPortalTopologyExit : TransientExit, IMagicPortalTopologyExit
{
	public MagicPortalTopologyExit(IMagicPortalNetwork network, IMagicPortalLink link, ICell source, ICell destination)
		: base(network.Gameworld, source, destination, network.Verb, network.OutboundKeyword, network.InboundKeyword,
			network.OutboundTarget, network.InboundTarget, network.OutboundDescription, network.InboundDescription,
			network.TimeMultiplier)
	{
		Network = network;
		Link = link;
		SourceEndpoint = link.SourceEndpoint;
		DestinationEndpoint = link.DestinationEndpoint;
	}

	public IMagicPortalNetwork Network { get; }
	public IMagicPortalLink Link { get; }
	public IMagicPortalEndpoint SourceEndpoint { get; }
	public IMagicPortalEndpoint DestinationEndpoint { get; }
}

public class MagicPortalTopologyService : IMagicPortalTopologyService
{
	public IEnumerable<IMagicPortalTopologyExit> MaterializedExits(IFuturemud gameworld)
	{
		return gameworld.ExitManager.TransientExits.OfType<IMagicPortalTopologyExit>();
	}

	public void RebuildAll(IFuturemud gameworld)
	{
		foreach (var network in gameworld.MagicPortalNetworks.ToList())
		{
			RebuildNetwork(network);
		}
	}

	public void RebuildNetwork(IMagicPortalNetwork network)
	{
		RemoveNetworkExits(network);
		if (!network.IsActive)
		{
			return;
		}

		foreach (var link in network.Links)
		{
			if (!string.IsNullOrWhiteSpace(link.WhyInvalid))
			{
				continue;
			}

			var source = link.SourceEndpoint.CurrentCell;
			var destination = link.DestinationEndpoint.CurrentCell;
			if (source is null || destination is null)
			{
				continue;
			}

			network.Gameworld.ExitManager.RegisterTransientExit(
				new MagicPortalTopologyExit(network, link, source, destination));
		}
	}

	public void RebuildNetworksForItem(IFuturemud gameworld, IGameItem item)
	{
		foreach (var network in gameworld.MagicPortalNetworks
			         .Where(x => x.Endpoints.Any(y =>
				         y.EndpointType == MagicPortalEndpointType.Item &&
				         y.GameItemId == item.Id))
			         .ToList())
		{
			RebuildNetwork(network);
		}
	}

	public void RemoveNetworkExits(IMagicPortalNetwork network)
	{
		foreach (var exit in MaterializedExits(network.Gameworld).Where(x => x.Network.Id == network.Id).ToList())
		{
			network.Gameworld.ExitManager.UnregisterTransientExit(exit.Exit);
		}
	}

	public IMagicPortalEndpoint? CreateOrUpdateEndpoint(ICharacter actor, IMagicPortalNetwork network, string key,
		string name, MagicPortalEndpointType endpointType, ICell? cell, IGameItem? item, bool replace, long? spellId,
		out string reason)
	{
		reason = string.Empty;
		if (network is not MagicPortalNetwork concreteNetwork)
		{
			reason = "That portal network cannot be edited by this runtime.";
			return null;
		}

		key = key.Trim().ToLowerInvariant();
		name = string.IsNullOrWhiteSpace(name) ? key.TitleCase() : name.Trim();
		if (string.IsNullOrWhiteSpace(key))
		{
			reason = "Portal endpoints must have a key.";
			return null;
		}

		if (endpointType == MagicPortalEndpointType.Cell && cell is null)
		{
			reason = "Cell endpoints must reference a room.";
			return null;
		}

		if (endpointType == MagicPortalEndpointType.Item && item is null)
		{
			reason = "Item endpoints must reference an item.";
			return null;
		}

		var existing = concreteNetwork.EndpointByKeyOrId(key);
		if (existing is not null)
		{
			if (!replace)
			{
				reason = $"There is already an endpoint with the key {key}.";
				return null;
			}

			existing.Update(key, name, endpointType, cell?.Id, item?.Id);
			RebuildNetwork(network);
			return existing;
		}

		DB.MagicPortalEndpoint dbitem;
		using (new FMDB())
		{
			dbitem = new DB.MagicPortalEndpoint
			{
				MagicPortalNetworkId = network.Id,
				Key = key,
				Name = name,
				AnchorType = (int)endpointType,
				CellId = cell?.Id,
				GameItemId = item?.Id,
				IsActive = true,
				CreatedByCharacterId = CharacterInstanceIdentityComparer.IdentityId(actor),
				CreatedBySpellId = spellId,
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.MagicPortalEndpoints.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var endpoint = new MagicPortalEndpoint(concreteNetwork, dbitem, key, name, endpointType, cell?.Id, item?.Id);
		concreteNetwork.AddEndpoint(endpoint);
		RebuildNetwork(network);
		return endpoint;
	}

	public IMagicPortalLink? CreateLink(ICharacter actor, IMagicPortalNetwork network, IMagicPortalEndpoint source,
		IMagicPortalEndpoint destination, long? spellId, out string reason)
	{
		reason = string.Empty;
		if (network is not MagicPortalNetwork concreteNetwork ||
			source is not MagicPortalEndpoint concreteSource ||
			destination is not MagicPortalEndpoint concreteDestination)
		{
			reason = "That portal network cannot be edited by this runtime.";
			return null;
		}

		if (source.Id == destination.Id)
		{
			reason = "A portal link cannot target the same endpoint twice.";
			return null;
		}

		if (source.Network.Id != network.Id || destination.Network.Id != network.Id)
		{
			reason = "Both endpoints must belong to the same portal network.";
			return null;
		}

		if (concreteNetwork.LinkBetween(concreteSource, concreteDestination) is not null)
		{
			reason = "Those endpoints are already linked.";
			return null;
		}

		DB.MagicPortalLink dbitem;
		using (new FMDB())
		{
			dbitem = new DB.MagicPortalLink
			{
				MagicPortalNetworkId = network.Id,
				SourceEndpointId = source.Id,
				DestinationEndpointId = destination.Id,
				IsActive = true,
				CreatedByCharacterId = CharacterInstanceIdentityComparer.IdentityId(actor),
				CreatedBySpellId = spellId,
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.MagicPortalLinks.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var link = new MagicPortalLink(concreteNetwork, dbitem, concreteSource, concreteDestination);
		concreteNetwork.AddLink(link);
		RebuildNetwork(network);
		return link;
	}

	public void DeleteSpellCreatedTopology(IFuturemud gameworld, IEnumerable<long> endpointIds, IEnumerable<long> linkIds)
	{
		var endpointSet = endpointIds.ToHashSet();
		var linkSet = linkIds.ToHashSet();
		var affectedNetworks = gameworld.MagicPortalNetworks
			.OfType<MagicPortalNetwork>()
			.Where(x => x.Endpoints.Any(y => endpointSet.Contains(y.Id)) || x.Links.Any(y => linkSet.Contains(y.Id)))
			.ToList();

		foreach (var network in affectedNetworks)
		{
			foreach (var link in network.Links.OfType<MagicPortalLink>().Where(x => linkSet.Contains(x.Id)).ToList())
			{
				network.DeleteLink(link);
			}

			foreach (var endpoint in network.Endpoints.OfType<MagicPortalEndpoint>().Where(x => endpointSet.Contains(x.Id)).ToList())
			{
				network.DeleteEndpoint(endpoint);
			}

			RebuildNetwork(network);
		}
	}
}
