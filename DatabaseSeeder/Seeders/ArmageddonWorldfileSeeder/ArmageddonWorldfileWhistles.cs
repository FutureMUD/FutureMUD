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
	private void InstallHuntingWhistles()
	{
		var sources = _source.Items.Where(x => x.Programs.Contains("C whistle halfling_whistle") && _items.ContainsKey(x.Vnum) &&
			_source.Resets.Any(r => r.ReferenceIssues.Length == 0 && r.Opcode is "O" or "P" or "G" or "E" &&
				r.Arguments.Length > 1 && r.Arguments[1] == x.Vnum)).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var races = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0]
			.Equals("Halfling", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		if (races.Length == 0) throw new InvalidOperationException("The hunting whistle requires the installed Halfling race.");
		var actorRace = string.Join(" or ", races.Select(x => $"@actor.race.id == {x}"));
		var listenerRace = string.Join(" or ", races.Select(x => $"@listener.race.id == {x}"));
		var reverse = string.Join("\n", new[] { ("north", "the south"), ("east", "the west"), ("south", "the north"), ("west", "the east"), ("up", "below"), ("down", "above") }
			.Select(x => $"if (@direction == \"{x.Item1}\")\nfrom = \"{x.Item2}\"\nend if"));
		var prog = Prog("ArmageddonWorldfileHuntingWhistle", $$"""
			if (not(contains(@actor.helditems, @item)) and not(contains(@actor.wieldeditems, @item)))
				return false
			end if
			if (not({{actorRace}}))
				sendloc(@actor.location, "$0 blow|blows on $1, producing a warbling noise that could be a bird.", @actor, @item)
				return true
			end if
			sendloc(@actor.location, "$0 blow|blows on $1, producing a whistling noise like a bird.", @actor, @item)
			foreach (passage in @actor.location.exits)
				var direction = lowercase(@passage.direction)
				var from = ""
				{{reverse}}
				if (@from != "")
					foreach (listener in @passage.destination.characters)
						if (@listener.isawake)
							if (canhearnoise(@listener, @actor, 4, 5))
								if ({{listenerRace}})
									send(@listener, "You hear the familiar sound of a hunting whistle from " + @from + ".")
								else
									send(@listener, "You hear the long, slow whistle of an unfamiliar bird from nearby.")
								end if
							end if
						end if
					end foreach
				end if
			end foreach
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile Halfling Hunting Whistle", Category = "Worldfile", Type = "HookOnInput",
			TargetEventType = (int)EventType.CommandInput, Definition = new XElement("Definition", new XElement("TargetCommand", "whistle"),
				new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		foreach (var vnum in sources)
		foreach (var proto in new[] { _items[vnum] }.Concat(_resetItemVariants.Where(x => x.Key.StartsWith(vnum + ":", StringComparison.Ordinal)).Select(x => x.Value)).DistinctBy(x => x.Id))
			AddItemOnLoadProg(proto, "HuntingWhistle", $"addhook(@item, {hook.Id})\nreturn true");
		Record("item-special.hunting-whistle", nameof(Hooks), hook.Id, null, new { Sources = sources, Prog = prog.Id,
			Disposition = "Held/wielded whistle only. Source race-specific local and adjacent bird recognition; six cardinal/vertical source directions only, including one-way exits. Native Loud/Unapproximable hearing checks replace the deafness-only gate; awake gate retained. No new audio engine, race ability or generic whistle activation." });
	}
}
