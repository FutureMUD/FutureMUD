#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallRoomCommands()
	{
		InstallGateCommands();
		InstallStuckElevator();
		if (_source.Rooms.Any(x => x.Programs.Contains("C jump dw_jump"))) InstallChasmJumps();
		if (_rooms.ContainsKey(1003) && _rooms.ContainsKey(50617) &&
			_source.Rooms.Any(x => x.Vnum == 1003 && x.Programs.Any(p => p.EndsWith(" hall_of_kings", StringComparison.Ordinal))))
		{
			var prog = RoomCommandProg("ArmageddonWorldfileHallDeparture", """
if (not(@actor.canmove) or @actor.incombat)
	send(@actor, "You cannot depart right now.")
	return false
end if
var count = @args.Count(word, true)
if (not((@count == 1 and lowercase(getindex(@args, 0)) == "allanak") or (@count == 2 and lowercase(getindex(@args, 0)) == "to" and lowercase(getindex(@args, 1)) == "allanak")))
	send(@actor, "Point to Allanak on the map to depart.")
	return false
end if
sendloc(@actor.location, "$0 point|points to Allanak on the map, then disappear|disappears.", @actor)
""" + $"\nreturn teleportnoecho(@actor, tolocation({_rooms[50617].Id}))");
			InstallRoomCommandHook(1003, "point", prog,
				"Approved departure-only reconstruction. Source attaches hall_of_kings to pulse although its body accepts only point, so departure was inert in this source build. Accept point Allanak / point to Allanak; preserve destination 50617. Do not rebuild stats/skills, refill resources, change language/start city, or grant waterskin.");
		}
	}

	private void InstallStuckElevator()
	{
		if (!_rooms.ContainsKey(66055) || !_rooms.TryGetValue(66005, out var stop) || !_items.ContainsKey(66000)) return;
		if (!_source.Rooms.Any(x => x.Vnum == 66055 && x.Programs.Any(p => p.EndsWith(" elevator", StringComparison.Ordinal)))) return;
		var family = ItemVariantIdExpression(66000).Replace("@x.proto", "@box.proto", StringComparison.Ordinal);
		var prog = RoomCommandProg("ArmageddonWorldfileStuckElevator", $"""
if (not(tolocation({stop.Id}).items.Any(box, ({family}))))
	return false
end if
sendloc(@actor.location, "$0 pull|pulls on the rope with all $0's strength, but it won't budge.", @actor)
return false
""");
		foreach (var command in new[] { "up", "u", "down", "d" })
			InstallRoomCommandHook(66055, command, prog,
				"Approved source stuck-elevator behaviour. Loaded box Vnum66000 starts at 66005; all four other coded stops are absent. Preserve refusal and normal command fall-through; invent no shaft rooms or moving vehicle.", conditional: true);
	}

	private void InstallGateCommands()
	{
		foreach (var (special, clans, keywords) in new[]
		{
			("arena_gate", new[] { 12, 23 }, new[] { "gate" }),
			("tuluk_gate", new[] { 8 }, new[] { "gate" }),
			("allanak_gate", new[] { 12 }, new[] { "gates" }),
			("luirs_gate", new[] { 4, 66 }, new[] { "gate", "gates" })
		})
		{
			var rooms = _source.Rooms.Where(x => x.Programs.Any(p => p.EndsWith(" " + special, StringComparison.Ordinal))).ToArray();
			if (rooms.Length == 0) continue;
			var member = string.Join(" or ", clans.Where(_clans.ContainsKey).Select(c => $"isclanmember(@actor, toclan({_clans[c].Id}))"));
			var permitted = string.Join(" or ", clans.Where(c => _clans.ContainsKey(c) && _ranks.ContainsKey((c, 4)))
				.Select(c => $"isclanmember(@actor, toclan({_clans[c].Id}), torank({_ranks[(c, 4)].Id}))"));
			var target = string.Join(" or ", keywords.Select(k => $"lowercase(getindex(@args, 0)) == \"{k}\""));
			var tower = special == "arena_gate" ? "guard towers" : "watch towers";
			var body = new System.Text.StringBuilder("var operation = lowercase(commandname(@actor, @command))\n" +
				"if (@operation != \"open\" and @operation != \"close\")\nreturn false\nend if\n" +
				$"var member = {(member.Length == 0 ? "false" : member)}\n");
			// Tuluk's source member branch calls out even when operating an unrelated object.
			if (special == "tuluk_gate") body.AppendLine("if (not(@member))");
			body.AppendLine($"if (@args.Count(word, true) == 0)\nreturn false\nend if\nif (not({target}))\nreturn false\nend if");
			if (special == "tuluk_gate") body.AppendLine("end if");
			body.AppendLine($"if (not(@member))\nsend(@actor, \"The mechanism to open such mighty gates would be in the {tower}.\")\nreturn true\nend if");
			var call = special == "arena_gate" ? "calls out for the gates of the arena to be" :
				special == "luirs_gate" ? "sends up a call for the gates to be" : "sends up a call to the tower for the gates to be";
			// Use explicit conjugation in the native emote rather than source act()'s separate self echo.
			call = call.Replace("calls", "call|calls").Replace("sends", "send|sends");
			body.AppendLine($"if (@operation == \"open\")\nsendloc(@actor.location, \"$0 {call} opened.\", @actor)\nelse\nsendloc(@actor.location, \"$0 {call} closed.\", @actor)\nend if");
			body.AppendLine(special == "luirs_gate" ? $"return not({(permitted.Length == 0 ? "false" : permitted)})" : "return false");
			var prog = RoomCommandProg("ArmageddonWorldfileGate" + special, body.ToString());
			foreach (var room in rooms) InstallRoomCommandHook(room.Vnum, "*", prog,
				"Source gate-keyword operation permissions and callouts. Resolve native open/close abbreviations. Authorised attempts continue through ordinary door commands; passage is not independently restricted. Luir requires clan rank >=4; absent clan 66 is not invented. Tuluk's member callout on unrelated operations is retained.", conditional: true);
		}
	}

	private void InstallChasmJumps()
	{
		var attributes = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		var agility = new[] { "Agility", "Dexterity" }.Select(name => attributes.FirstOrDefault(x =>
			x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault(x => x is not null)
			?? throw new InvalidOperationException("Chasm jumping requires an agility/dexterity attribute.");
		var inches = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Metre]).BaseMultiplier * 0.0254;
		var giants = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0]
			.Replace("-", "").Replace(" ", "").Equals("halfgiant", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		var giant = giants.Length == 0 ? "false" : string.Join(" or ", giants.Select(id => $"@actor.race.id == {id}"));
		foreach (var (origin, destination, direction) in new[] { (25167, 25165, "west"), (25165, 25167, "east") })
		{
			if (!_rooms.ContainsKey(origin) || !_rooms.ContainsKey(destination) || !_rooms.ContainsKey(25604))
				throw new InvalidOperationException("The source chasm jump has an unavailable endpoint or fall room.");
			var text = $"""
if (@args.Count(word, true) != 1 or lowercase(getindex(@args, 0)) != "{direction}")
	send(@actor, "Which direction do you want to jump? Try jump {direction}.")
	return false
end if
if (not(isnull(@actor.mount)))
	send(@actor, "You are unable to goad your mount into jumping across that.")
	return false
end if
if (not(@actor.canmove) or @actor.incombat)
	send(@actor, "You cannot make that jump right now.")
	return false
end if
var score = floor(@actor.height / {inches.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} / 10) + floor(gettrait(@actor, totrait({agility.Id})) / 2) + random(-5, 5)
if (textcontains(@actor.currentspeed, "run") and not({giant}))
	score = @score + 5
end if
if (@actor.issneaking)
	score = @score - 5
end if
sendloc(@actor.location, "$0 leap|leaps off the ledge and sail|sails through the air.", @actor)
if (@score < 20)
	send(@actor, "You miss the far ledge and plummet down the ridge.")
	return teleportnoecho(@actor, tolocation({_rooms[25604].Id}))
end if
send(@actor, "You leap across the ledge and land upon the other side.")
return teleportnoecho(@actor, tolocation({_rooms[destination].Id}))
""";
			var prog = RoomCommandProg("ArmageddonWorldfileChasmJump" + origin, text);
			InstallRoomCommandHook(origin, "jump", prog,
				"Approved checked prog/teleport. Source height in inches/10, agility/2, random -5..5, +5 running except Half-Giants, -5 sneaking; success >=20. Native selected gait containing run supplies running and native Sneak effects supply sneaking. Fall destination 25604. Source debugging score output is omitted. Native movement/state/combat/mount admission applies.");
		}
	}

	private FutureProg RoomCommandProg(string name, string text) => Prog(name, text, ProgVariableTypes.Boolean,
		("actor", ProgVariableTypes.Character), ("room", ProgVariableTypes.Perceivable),
		("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));

	private void InstallRoomCommandHook(int roomVnum, string command, FutureProg prog, string disposition, bool conditional = false)
	{
		var hook = new Hooks { Id = Next<Hooks>(), Name = $"Worldfile Vnum{roomVnum} {command}", Type = "HookOnInput",
			Category = "Worldfile", TargetEventType = (int)EventType.CommandInput,
			Definition = new XElement("Definition", new XElement("TargetCommand", command), new XElement("UseProgResultForHandling", conditional), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook); _db.HooksPerceivables.Add(new HooksPerceivable { HookId = hook.Id, RoomId = _rooms[roomVnum].Id });
		Record($"room-command.{roomVnum}.{command}", nameof(Hooks), hook.Id, null,
			new { SourceRoom = roomVnum, Command = command, Prog = prog.Id, Disposition = disposition });
		_counts["room command hooks"] = _counts.GetValueOrDefault("room command hooks") + 1;
	}
}
