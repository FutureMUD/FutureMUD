#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallJailNeeds()
	{
		const int sourceProvisioningMarker = 37980;
		var markerRooms = _source.Resets
			.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode == "O" && x.Arguments.Length >= 4 &&
			            x.Arguments[1] == sourceProvisioningMarker && _rooms.ContainsKey(x.Arguments[3]) &&
			            !_interiorTemplates.Contains(x.Arguments[3]))
			.Select(x => x.Arguments[3])
			.Distinct()
			.Order()
			.ToArray();
		var custodyRoomIds = _db.Rooms.Local
			.Where(x => x.UniqueName.StartsWith("justice.", StringComparison.OrdinalIgnoreCase) &&
			            x.UniqueName.EndsWith(".holding", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.Id)
			.ToArray();
		var targetRoomIds = markerRooms
			.Select(vnum => _rooms[vnum].Id)
			.Concat(custodyRoomIds)
			.Distinct()
			.Order()
			.ToArray();
		if (targetRoomIds.Length == 0)
		{
			return;
		}

		var generated = custodyRoomIds.Length == 0 ? "false" : string.Join(" or ", custodyRoomIds.Select(x => $"@location.id == {x}"));
		var marker = _items.ContainsKey(sourceProvisioningMarker) ? ItemVariantIdExpression(sourceProvisioningMarker) : "false";
		var body = $$"""
			if (isadmin(@character) or @character.location != @location)
				return false
			end if
			if (not({{generated}}) and not(@location.items.Any(x, {{marker}})))
				return false
			end if
			var hungry = lowercase(hungerstatus(@character)) == "hungry" or lowercase(hungerstatus(@character)) == "starving"
			var thirsty = lowercase(thirststatus(@character)) == "thirsty" or lowercase(thirststatus(@character)) == "parched"
			if (not(@hungry) and not(@thirsty))
				return false
			end if
			if (not(satisfyneeds(@character, @hungry, @thirsty)))
				return false
			end if
			if (@hungry and @thirsty)
				send(@character, "A guard hands you food and water, which you quickly consume.")
			else
				if (@hungry)
					send(@character, "A guard hands you some stale bread, which you quickly consume.")
				else
					send(@character, "A guard hands you some stagnated water, which you quickly drink.")
				end if
			end if
			return true
			""";
		var stateNeeds = Prog("ArmageddonWorldfileJailNeeds", body, ProgVariableTypes.Boolean,
			("character", ProgVariableTypes.Character), ("location", ProgVariableTypes.Location),
			("witness", ProgVariableTypes.Perceivable));
		var entryNeeds = Prog("ArmageddonWorldfileJailNeedsOnEntry", body, ProgVariableTypes.Boolean,
			("character", ProgVariableTypes.Character), ("location", ProgVariableTypes.Location),
			("exit", ProgVariableTypes.Exit), ("witness", ProgVariableTypes.Perceivable));

		InstallRoomEventHook("Jail Needs", EventType.CharacterInRoomHungry, stateNeeds, targetRoomIds);
		InstallRoomEventHook("Jail Needs", EventType.CharacterInRoomStarving, stateNeeds, targetRoomIds);
		InstallRoomEventHook("Jail Needs", EventType.CharacterInRoomThirsty, stateNeeds, targetRoomIds);
		InstallRoomEventHook("Jail Needs", EventType.CharacterInRoomParched, stateNeeds, targetRoomIds);
		InstallRoomEventHook("Jail Needs on Entry", EventType.CharacterEnterRoomFinishWitness, entryNeeds, targetRoomIds);
		Record("jail.needs", nameof(FutureProg), stateNeeds.Id, null, new
		{
			EntryProg = entryNeeds.Id,
			SourceMarker = sourceProvisioningMarker,
			SourceRooms = markerRooms,
			NativeCustodyRoomIds = custodyRoomIds,
			Disposition = "Source marker reset rooms and generated native custody holding cells call satisfyneeds on entry and on Hungry/Starving/Thirsty/Parched room events. Native fulfillment adds only deficits to Full/NotThirsty and half of native body water capacity; already-satisfied reserves are unchanged. Admin characters are exempt. No source condition unit is converted to native hours."
		});
	}

	private void InstallRoomEventHook(string label, EventType eventType, FutureProg prog, long[] roomIds)
	{
		var hook = new Hooks
		{
			Id = Next<Hooks>(),
			Name = $"Worldfile {label} {eventType}",
			Type = "FutureProgHook",
			Category = "Worldfile",
			TargetEventType = (int)eventType,
			Definition = new XElement("Definition", new XElement("FutureProg", prog.Id)).ToString()
		};
		_db.Hooks.Add(hook);
		foreach (var roomId in roomIds)
		{
			_db.HooksPerceivables.Add(new HooksPerceivable { HookId = hook.Id, RoomId = roomId });
		}
	}
}
