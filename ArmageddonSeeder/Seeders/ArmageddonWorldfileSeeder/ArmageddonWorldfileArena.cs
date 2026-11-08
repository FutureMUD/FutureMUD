#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallArenaRelease()
	{
		if (!_source.Rooms.Any(x => x.Vnum == 50683 && x.Programs.Contains("C release arena_beast"))) return;
		if (!_rooms.TryGetValue(50664, out var arena)) throw new InvalidOperationException("Arena release destination 50664 is missing.");
		var templates = new List<(string Keyword, string Label, int Vnum, long Template)>();
		var entries = new (string Keyword, string Label, int Vnum, (int Item, int Slot)[] Gear)[]
		{
			("anakore", "anakore", 50110, []), ("gaj", "gaj", 50111, []),
			("gith", "gith", 50101, [(50101, 13), (1147, 16)]),
			("mul", "mul gladiator", 50112, [(50101, 13), (1110, 19)]),
			("elven", "elven thief", 50113, [(50101, 13), (50153, 16)]),
			("human", "human cut-throat", 50114, [(50101, 13), (50152, 16)]),
			("gwoshi", "gwoshi", 50116, []), ("wezer", "wezer", 50117, [])
		};
		for (var i = 0; i < entries.Length; i++)
		{
			var entry = entries[i]; var line = 2_000_001_000 + i * 16;
			if (!_npcs.ContainsKey(entry.Vnum)) throw new InvalidOperationException($"Approved arena creature {entry.Vnum} is missing.");
			var generated = new List<ArmWorldReset> { new(new("generated/arena_release", line), 50, "M", [0, entry.Vnum, 1, 50664], [], []) };
			foreach (var (itemVnum, slot) in entry.Gear)
			{
				var item = _source.Items.Single(x => x.Vnum == itemVnum);
				generated.Add(new(new("generated/arena_release", line + generated.Count), 50, "E",
					new[] { 0, itemVnum, 0, slot }.Concat(item.Values).Append(0).ToArray(), [], []));
			}
			PrepareObjectResetVariants(generated.ToArray());
			var template = BuildNpcOutfitVariants(generated.ToArray())[(50, line)];
			AddNpcFamilyVariant(entry.Vnum, template.Id);
			templates.Add((entry.Keyword, entry.Label, entry.Vnum, template.Id));
		}
		var permission = new[] { (Clan: 12, Rank: 6), (Clan: 23, Rank: 3) }
			.Where(x => _clans.ContainsKey(x.Clan) && _ranks.ContainsKey((x.Clan, x.Rank)))
			.Select(x => $"isclanmember(@actor, toclan({_clans[x.Clan].Id}), torank({_ranks[(x.Clan, x.Rank)].Id}))").ToArray();
		if (permission.Length == 0) throw new InvalidOperationException("Arena release has no source-backed authorising ranks.");
		var listing = "The following things may be released into the arena. Type 'release <name> arena': anakore, gaj, gith, mul gladiator, elven thief, human cut-throat, gwoshi, wezer.";
		var text = new StringBuilder($"if (not(@actor.isawake) or not({string.Join(" or ", permission)}))\nreturn false\nend if\n" +
			$"if (@args.Count(word, true) < 2)\nsendfixed(@actor, {ProgText(listing)})\nreturn true\nend if\n" +
			"if (lowercase(getindex(@args, 1)) != \"arena\")\nreturn true\nend if\nvar keyword = lowercase(getindex(@args, 0))\n");
		foreach (var entry in templates)
		{
			text.AppendLine($"""
if (startswith("{entry.Keyword}", @keyword))
	var beast = loadnpc({entry.Template}, tolocation({arena.Id}))
	if (isnull(@beast))
		send(@actor, "The beast could not be released.")
		return true
	end if
	sendloc(@actor.location, "$0 release|releases a beast into the arena.", @actor)
	sendloc(tolocation({arena.Id}), "A trapdoor opens in the ground, and $0 is shoved into the arena.", @beast)
	return true
end if
""");
		}
		text.AppendLine($"sendfixed(@actor, {ProgText(listing)})\nreturn true");
		var prog = RoomCommandProg("ArmageddonWorldfileArenaRelease", text.ToString());
		InstallRoomCommandHook(50683, "release", prog,
			"Approved reconstruction of source-inert NPC-dependent room binding. Native room prog preserves clan 12 rank >=6 / clan 23 rank >=3, source prefix selection, eight available beasts, equipment and 50664 destination. Missing Braxat 50118 is unavailable. No source-unattached list/remove commands or broad cleanup are activated.", conditional: true);
		Record("room-special.arena-beast", nameof(FutureProg), prog.Id, null, new { SourceRoom = 50683, Destination = 50664, Creatures = templates,
			Disposition = "Eight-creature release reconstruction approved. Native outfit variants retain source equipment, HP, inventory and AI. No missing creature definition or artificial keeper NPC is invented." });
		Flush();
	}

	private void AddNpcFamilyVariant(int vnum, long template)
	{
		_npcFamilyIds[vnum] = _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]).Append(template).Distinct().ToArray();
		foreach (var spawner in _db.NpcSpawners.Local)
		{
			var definition = XElement.Parse(spawner.Definition);
			if (!definition.Descendants("Template").Any(x => (long?)x.Attribute("id") == _npcs[vnum].Id)) continue;
			definition.Element("Templates")!.Add(new XElement("Template", new XAttribute("id", template), new XAttribute("weight", 0)));
			spawner.Definition = definition.ToString();
		}
		foreach (var prog in _progs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileZone", StringComparison.Ordinal)))
			prog.FunctionText = Regex.Replace(prog.FunctionText, $@"@x\.npctemplateid == {_npcs[vnum].Id}(?![0-9])",
				_ => $"(@x.npctemplateid == {_npcs[vnum].Id} or @x.npctemplateid == {template})");
	}
}
