#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static string ProgText(string text) => "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"")
		.Replace("\r", "").Replace("\n", "\\n") + "\"";

	private void InstallNarrativeSpecials()
	{
		InstallCaveWriting();
		var historians = _source.Npcs.Where(x => HasSpecial(x, "bw_historian")).Select(x => x.Vnum).ToArray();
		if (historians.Length == 0) return;
		if (!_clans.TryGetValue(25, out var blackwing)) throw new InvalidOperationException("Historian dialogue requires the imported Blackwing clan.");
		var language = _db.Languages.Single(x => x.Name == "Allundean");
		var accent = _db.Accents.Where(x => x.LanguageId == language.Id).OrderBy(x => x.Id).First();
		using var text = ArmageddonWorldfilePackage.LoadSpecialText();
		if (text.RootElement.GetProperty("Version").GetInt32() != 1) throw new InvalidOperationException("Unsupported source-special prose version.");
		Record("special-text-source", "ArmageddonSpecialTextSource", null, null, text.RootElement.GetProperty("Source"));
		var body = new StringBuilder($"if (not(isclanmember(@actor, toclan({blackwing.Id}))))\nreturn false\nend if\n" +
			"if (@args.Count(word, true) < 2)\nreturn false\nend if\n" +
			"if (targetcharacter(@actor, getindex(@args, 0)) != tocharacter(@npc))\nreturn false\nend if\n" +
			"var topic = lowercase(getindex(@args, 1))\nvar replied = false\n");
		foreach (var topic in text.RootElement.GetProperty("Historian").EnumerateObject())
		{
			body.AppendLine($"if (not(@replied))\nif (startswith({ProgText(topic.Name)}, @topic))");
			var emote = topic.Value.GetProperty("Emote").GetString();
			if (!string.IsNullOrEmpty(emote)) body.AppendLine($"force tocharacter(@npc) {ProgText(emote)}");
			var message = "$0 say|says, \"" + topic.Value.GetProperty("Text").GetString() + "\"";
			body.AppendLine($"sendloclang(@actor.location, {ProgText(message)}, tolanguage({language.Id}), toaccent({accent.Id}), @npc)\nreplied = true\nend if\nend if");
		}
		body.AppendLine($"if (not(@replied))\nsendloclang(@actor.location, {ProgText(text.RootElement.GetProperty("HistorianFallback").GetString()!)}, tolanguage({language.Id}), toaccent({accent.Id}), @npc)\nend if\nreturn true");
		var prog = Prog("ArmageddonWorldfileBlackwingHistorian", body.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("input", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		InstallNpcInputHook("Blackwing Historian", "ask", prog, historians);
		Record("npc-special.blackwing-historian", nameof(FutureProg), prog.Id, null, new
		{ Sources = historians, Clan = blackwing.Id, Language = language.Id,
			Disposition = "Approved Blackwing membership replaces absent hometown. Visible native target matching, ordered source topic prefixes and all six complete source narratives, including riding smirk. Public native Allundean speech retains comprehension/perception rules rather than printing universally readable fake-language text." });
		Flush();
	}

	private void InstallNpcInputHook(string name, string command, FutureProg prog, int[] sources)
	{
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile " + name, Category = "Worldfile", Type = "HookOnInput",
			TargetEventType = (int)EventType.CommandInput,
			Definition = new XElement("Definition", new XElement("TargetCommand", command), new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		foreach (var vnum in sources)
		foreach (var id in _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]))
		{
			var template = _db.NpcTemplates.Local.Single(x => x.Id == id); var definition = XElement.Parse(template.Definition);
			definition.Element("TemplateLoadAdditions")!.Element("Hooks")!.Add(new XElement("Hook", new XAttribute("id", hook.Id)));
			template.Definition = definition.ToString();
		}
		_counts["NPC command hooks"] = _counts.GetValueOrDefault("NPC command hooks") + 1;
	}

	private void InstallCaveWriting()
	{
		var rooms = _source.Rooms.Where(x => x.Programs.Any(p => p.EndsWith(" cave_writing", StringComparison.Ordinal))).ToArray();
		if (rooms.Length == 0) return;
		var body = new StringBuilder("if (@args.Count(word, true) == 0)\nreturn false\nend if\n" +
			"var word = lowercase(getindex(@args, 0))\nif (@word != \"writing\" and @word != \"writings\")\nreturn false\nend if\n" +
			"sendfixed(@actor, \"You examine three inscriptions in different languages.\")\n");
		using var text = ArmageddonWorldfilePackage.LoadSpecialText();
		var readings = text.RootElement.GetProperty("CaveReadings").EnumerateArray();
		foreach (var reading in readings)
		{
			var name = reading.GetProperty("Language").GetString()!;
			var ordinal = reading.GetProperty("Ordinal").GetString()!;
			var full = reading.GetProperty("Full").GetString()!;
			var partial = reading.GetProperty("Partial").GetString()!;
			var recognised = reading.GetProperty("Recognised").GetString()!;
			var language = _db.Languages.Single(x => x.Name == name);
			body.AppendLine($"sendfixed(@actor, {ProgText("\nYou look at the " + ordinal + " set of writings:")})\nvar proficiency{name} = gettrait(@actor, totrait({language.LinkedTraitId}))\n" +
				$"if (@proficiency{name} > 80)\nsendfixed(@actor, {ProgText(full)})\nelseif (@proficiency{name} > 50)\nsendfixed(@actor, {ProgText(partial)})\nelseif (@proficiency{name} > 0)\nsendfixed(@actor, {ProgText(recognised)})\nelse\nsendfixed(@actor, \"You are unable to read the writings.\")\nend if");
		}
		body.AppendLine("return true");
		var prog = RoomCommandProg("ArmageddonWorldfileCaveWriting", body.ToString());
		foreach (var room in rooms)
		foreach (var command in new[] { "look", "read", "examine" })
			InstallRoomCommandHook(room.Vnum, command, prog,
				"Three independent native language proficiency readings at strict >80/>50/>0 thresholds; original garbled text retained. Unrelated inspection passes through. Explicit inscription handling consumes the command to avoid a generic not-found echo after the source callback's false return.", conditional: true);
	}
}
