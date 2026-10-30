extern alias EngineCompiler;

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private long? OwnedBehaviourId(string key, string type)
	{
		var records = _db.SeederManagedRecords.Where(x => x.StableKey == key && x.EntityType == type && !x.Retired && x.LogicalId != null).ToArray();
		if (records.Length > 1) throw new InvalidOperationException($"Ambiguous native identity for {key}.");
		return records.SingleOrDefault()?.LogicalId;
	}

	private void InstallNpcCommandSpecials()
	{
		var sources = _source.Npcs.Where(x => HasSpecial(x, "no_magick")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var standing = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionStanding.Instance.Id;
		var roles = new[] { "jihaen_templar", "lirathan_templar", "southern_templar" }
			.Select(key => OwnedBehaviourId("arm.class." + key, nameof(ChargenRole))).Where(x => x.HasValue).Select(x => x!.Value).ToArray();
		var clans = new[] { 8, 12 }.Where(_clans.ContainsKey).Select(id => $"isclanmember(@actor, toclan({_clans[id].Id}))").ToArray();
		var text = new StringBuilder("if (not(ismagiccastingcommand(@actor, @input)))\nreturn false\nend if\n" +
			$"if (positionid(@actor) != {standing})\nreturn false\nend if\n");
		if (roles.Length > 0 && clans.Length > 0)
		{
			// The native compiler accepts grouped binary expressions, but not redundant groups around a single call.
			var roleTerms = roles.Select(id => $"contains(@actor.roles, torole({id}))").ToArray();
			var rolePredicate = roleTerms.Length == 1 ? roleTerms[0] : "(" + string.Join(" or ", roleTerms) + ")";
			var clanPredicate = clans.Length == 1 ? clans[0] : "(" + string.Join(" or ", clans) + ")";
			text.AppendLine($"if ({rolePredicate} and {clanPredicate})\nreturn false\nend if");
		}
		text.AppendLine("var caster = false");
		foreach (var (key, self, room) in new[]
		{
			("defiler", "Currents of magick begin to gather around you.", "Magickal currents begin to swirl around $0."),
			("fire", "Flames erupt in response to your summons.", "Flames erupt around $0 as $0 begins an incantation."),
			("water", "You call upon Vivadu for your magick.", "A fine mist condenses near $0 as $0 begins a spell."),
			("earth", "The earth trembles in response to your call.", "The earth trembles as $0 begins a spell."),
			("wind", "You draw upon the power of Whira for a spell.", "Winds swirl around $0 as $0 summons magick."),
			("void", "The air grows still and quiet around you as you focus your magick.", "The air grows still and quiet around $0."),
			("lightning", "Sparks jump in the air around you as you focus your magick.", "Sparks jump in the air around $0."),
			("shadow", "Shadows gather around you as you focus your magick.", "Shadows gather around $0.")
		})
		{
			if (OwnedBehaviourId("arm.capability." + key, nameof(MagicCapability)) is not { } id) continue;
			text.AppendLine($"if (not(@caster) and contains(@actor.magiccapabilities, tomagiccapability(\"{id}\")))\nsend(@actor, \"{self}\")\nsendloc(@actor.location, \"{room}\", @actor)\ncaster = true\nend if");
		}
		text.AppendLine("if (not(@caster))\nsend(@actor, \"You have no knowledge of such magicks.\")\nreturn true\nend if");
		text.AppendLine("sendloc(@actor.location, \"$0 hurl|hurls an empty mug at $1!\", @npc, @actor)\nforce tocharacter(@npc) \"say Practice your foul magicks elsewhere, rogue!\"\nwound(@actor, \"crushing\", random(1, 3), \"head\")\nreturn true");
		var prog = Prog("ArmageddonWorldfileBartenderCasting", text.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("input", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hooks = new List<Hooks>();
		var commands = new[] { "*" };
		foreach (var command in commands)
		{
			var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile Bartender Casting Interdiction " + command, Type = "HookOnInput",
				Category = "Worldfile", TargetEventType = (int)EventType.CommandInput,
				Definition = new XElement("Definition", new XElement("TargetCommand", command), new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
			_db.Hooks.Add(hook); hooks.Add(hook);
		}
		foreach (var vnum in sources)
		foreach (var id in _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]))
		{
			var template = _db.NpcTemplates.Local.Single(x => x.Id == id); var definition = XElement.Parse(template.Definition);
			definition.Element("TemplateLoadAdditions")!.Element("Hooks")!.Add(hooks.Select(hook => new XElement("Hook", new XAttribute("id", hook.Id))));
			template.Definition = definition.ToString();
		}
		_counts["NPC command hooks"] = hooks.Count;
		Record("npc-special.no-magick", nameof(Hooks), hooks[0].Id, null, new
		{ Sources = sources, Prog = prog.Id, Commands = commands.ToArray(), Disposition = "Conditional wildcard input hooks preserve original command text and resolve native abbreviations; they intercept native school cast/quiet/formula/area/practice commands, legacy cast and approved spoken incantations recognised by the native pure formula parser. School help/status and ordinary dialogue pass through. Non-standing casters and Templars in source clan 8/12 remain exempt. Owned capabilities select source echoes; other guilds receive source refusal. Native 1..3 crushing damage represents the mug. This is not a room-wide NoMagic policy." });
		Flush();
	}
}
