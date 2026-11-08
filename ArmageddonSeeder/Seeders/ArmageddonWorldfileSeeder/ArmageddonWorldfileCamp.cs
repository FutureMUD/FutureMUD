#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string GanChoice = "armw_gan_tribe";
	private const string GanTent = "armw_gan_tent_id";

	private void InstallGanZienCamp()
	{
		var source = _source.Rooms.SingleOrDefault(x => x.Programs.Contains("C pulse gan_zien_tribes"));
		if (source is null) return;
		foreach (var key in new[] { GanChoice, GanTent })
		{
			var owner = ProgVariableTypes.Location.ToStorageString();
			if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == key))
				throw new InvalidOperationException($"Register {key} already exists; worldfile camp cannot replace it.");
			_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = key,
				ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = key, DefaultValue = "<var>0</var>" });
		}
		var body = new StringBuilder($"var camp = tolocation({_rooms[source.Vnum].Id})\nvar tribe = getregister(@camp, \"{GanChoice}\")\n" +
			$"if (@tribe < 1 or @tribe > 3)\ntribe = random(1, 3)\nsetregister @camp \"{GanChoice}\" @tribe\nend if\n");
		foreach (var choice in new[] { 1, 2, 3 })
		{
			var vnum = 50164 + choice; var tent = _items[50622 + choice];
			var line = 2_000_000_000 + choice * 16;
			var generated = new List<ArmWorldReset> { new(new("generated/gan_zien", line), source.Zone, "M", [0, vnum, 1, source.Vnum], [], []) };
			var gear = new[] { (50619 + choice, 10), (66215, 5), (50021, 7), (20012, 8), (21031, 0), (1103, 11), (1120, 18) };
			foreach (var (itemVnum, slot) in gear)
			{
				var item = _source.Items.Single(x => x.Vnum == itemVnum);
				generated.Add(new(new("generated/gan_zien", line + generated.Count), source.Zone, "E",
					new[] { 0, itemVnum, 0, slot }.Concat(item.Values).Append(0).ToArray(), [], []));
			}
			PrepareObjectResetVariants(generated.ToArray());
			var template = BuildNpcOutfitVariants(generated.ToArray())[(source.Zone, line)];
			var family = _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]);
			_npcFamilyIds[vnum] = family.Append(template.Id).Distinct().ToArray();
			// Existing population caps must count this additional member of the same source family.
			foreach (var spawner in _db.NpcSpawners.Local)
			{
				var definition = XElement.Parse(spawner.Definition);
				if (!definition.Descendants("Template").Any(x => (long?)x.Attribute("id") == _npcs[vnum].Id)) continue;
				definition.Element("Templates")!.Add(new XElement("Template", new XAttribute("id", template.Id), new XAttribute("weight", 0)));
				spawner.Definition = definition.ToString();
			}
			foreach (var prog in _progs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileZone", StringComparison.Ordinal)))
				prog.FunctionText = Regex.Replace(prog.FunctionText, $@"@x\.npctemplateid == {_npcs[vnum].Id}(?![0-9])",
					_ => $"(@x.npctemplateid == {_npcs[vnum].Id} or @x.npctemplateid == {template.Id})");
			body.AppendLine($"if (@tribe == {choice})\nif (loadedcharacters().Count(x, @x.npctemplateid == {template.Id}) == 0)\nloadnpc({template.Id}, @camp)\nend if");
			body.AppendLine($"var survivingtent{choice} = getexistingitem(getregister(@camp, \"{GanTent}\"))\nif (isnull(@survivingtent{choice}))\nvar tent{choice} = loaditem({tent.Id})\nif (put(@tent{choice}, @camp))\nsetregister @camp \"{GanTent}\" @tent{choice}.id\nelse\ndeleteitem(@tent{choice})\nend if\nend if\nend if");
		}
		body.AppendLine("return true");
		var maintenance = Prog("ArmageddonWorldfileGanZienCamp", body.ToString(), ProgVariableTypes.Boolean);
		foreach (var prog in _progs.Where(x => x.FunctionName == $"ArmageddonWorldfileZone{source.Zone}Boot" ||
			x.FunctionName == $"ArmageddonWorldfileZone{source.Zone}Reset"))
		{
			var insertion = prog.FunctionText.LastIndexOf("return true", StringComparison.Ordinal);
			if (prog.FunctionName.EndsWith("Reset", StringComparison.Ordinal) && _source.Zones.Single(x => x.Index == source.Zone).Fields[2] == 1)
				insertion = prog.FunctionText.LastIndexOf("end if", insertion, StringComparison.Ordinal);
			if (insertion < 0) throw new InvalidOperationException("Gan Zien maintenance has no valid population insertion point.");
			prog.FunctionText = prog.FunctionText.Insert(insertion, $"@{maintenance.FunctionName}()\n");
		}
		Record("room-special.gan-zien", nameof(Room), _rooms[source.Vnum].Id, null, new
		{ SourceRoom = source.Vnum, ChoiceRegister = GanChoice, TentIdRegister = GanTent,
			Disposition = "Approved persistent first-boot random tribe. Native boot/reset progs replenish the chosen hawker with source gear and recreate only a missing tent. Existing tent is resolved by persistent instance ID, including when unloaded or moved; surviving NPCs/items are never cleared on restart. Ordinary family caps count camp variants." });
		_counts["persistent camps"] = 1;
		Flush();
	}
}
