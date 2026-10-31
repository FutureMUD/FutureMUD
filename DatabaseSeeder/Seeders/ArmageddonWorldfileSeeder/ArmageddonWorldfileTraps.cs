#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallExplosiveTraps()
	{
		var powders = _effectiveItems.Where(x => x.Value.Source.ItemType == 7 && x.Value.Values[0] == 2).ToArray();
		if (powders.Length == 0) return;
		var skill = OwnedBehaviourId("arm.skill.trap", nameof(TraitDefinition))
			?? throw new InvalidOperationException("Explosive traps require the installed Trap skill.");
		var tag = new Tag { Id = Next<Tag>(), Name = "Armageddon Explosive Trap Material" };
		_db.Tags.Add(tag);
		var quality = new StringBuilder();
		foreach (var (id, entry) in powders)
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			proto.GameItemProtosTags.Add(new GameItemProtosTags { TagId = tag.Id });
			quality.AppendLine($"if (@item.proto == {id})\nreturn {entry.Values[1]}\nend if");
		}
		quality.AppendLine("return 0");
		var bonus = Prog("ArmageddonWorldfileTrapMaterialBonus", quality.ToString(), ProgVariableTypes.Number, ("item", ProgVariableTypes.Item));
		var knows = Prog("ArmageddonWorldfileKnowsPowderTrap", $"return gettrait(@actor, totrait({skill})) > 0",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var setup = Prog("ArmageddonWorldfileSetPowderTrap", $$"""
		var material = getindex(@materials, 0)
		if (isnull(@material) or not(@actor.helditems.Any(held, @held == @material)))
			send(@actor, "You must hold the explosive powder to set this trap.")
			return false
		end if
		if (check(@actor, totrait({{skill}}), 5, @{{bonus.FunctionName}}(@material)) > 0)
			return true
		end if
		var damage = random(10, 20) + random(10, 20)
		foreach (explosive in @material.contents)
			if (istagged(@explosive, {{tag.Id}}))
				damage = @damage + random(10, 20)
			end if
		end foreach
		deleteitem(@material)
		sendloc(@actor.location, "The trap backfires and explodes in $0's face!", @actor)
		adjusthitpoints(@actor, 0 - @damage, random(10, 25) * random(1, 4))
		return false
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("anchor", ProgVariableTypes.Perceivable),
			("materials", ProgVariableTypes.Item | ProgVariableTypes.Collection));
		var payload = Prog("ArmageddonWorldfilePowderTrapBlast", $$"""
		var item = toitem(@anchor.id)
		if (isnull(@item) or isnull(@actor) or @actor.isdead)
			return false
		end if
		var damage = random(10, 25) + random(10, 25)
		if (@item.isdoor)
			sendloc(@actor.location, "An explosion throws $0 backward!", @actor)
			adjusthitpoints(@actor, 0 - @damage)
			return true
		end if
		foreach (explosive in @item.contents)
			if (istagged(@explosive, {{tag.Id}}))
				damage = @damage + random(10, 25)
			end if
		end foreach
		sendloc(@actor.location, "$1 explodes in $0's hands!", @actor, @item)
		adjusthitpoints(@actor, 0 - @damage, random(10, 25) * random(1, 4))
		return true
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("anchor", ProgVariableTypes.Perceivable));

		TrapTemplate Template(string name, bool preassembled)
		{
			XElement Trigger(string type) => new("Trigger", new XAttribute("type", type),
				new XElement("Parameter", new XAttribute("name", "spotdifficulty"), "Impossible"),
				new XElement("Parameter", new XAttribute("name", "avoiddifficulty"), "Impossible"));
			var template = new TrapTemplate
			{
				Id = Next<TrapTemplate>(), RevisionNumber = 0, EditableItem = Approval(), Name = name,
				Definition = new XElement("TrapTemplate", new XAttribute("source", "Mechanical"),
					new XAttribute("preassembled", preassembled), new XAttribute("closeditem", !preassembled),
					new XAttribute("charges", 1), new XAttribute("disarm", "Impossible"), new XAttribute("lifecycle", "Indefinite"),
					new XAttribute("setuptime", "00:00:00"), new XAttribute("recoverytime", "00:00:05"),
					new XAttribute("knowledgeprog", knows.Id), preassembled ? null : new XAttribute("setupprog", setup.Id),
					new XElement("Triggers", Trigger("Openable"), Trigger("Junked")),
					new XElement("Payloads", new XElement("Payload", new XAttribute("type", "ExecuteProg"),
						new XAttribute("delay", "00:00:00"), new XAttribute("target", "Triggerer"),
						new XElement("Parameter", new XAttribute("name", "prog"), payload.Id))),
					new XElement("Components", preassembled ? null : new XElement("Component", new XAttribute("tag", tag.Id),
						new XAttribute("role", "TriggerAndPayload"), new XAttribute("recovery", 0), new XAttribute("qualityweight", 0)))).ToString()
			};
			_db.TrapTemplates.Add(template);
			return template;
		}
		var playerTrap = Template("Powder Trap", false);
		var builtInTrap = Template("Armageddon Authored Powder Trap", true);
		var trapped = _effectiveItems.Where(x => x.Value.Source.ItemType == 15 && (x.Value.Values[1] & 16) != 0).ToArray();
		foreach (var (id, entry) in trapped)
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			AddItemOnLoadProg(proto, "AuthoredTrap", $"if (isnull(trapat(@item)))\nvar trap = createtrap(@item, {builtInTrap.Id}, null(\"character\"))\nend if\nreturn true");
			foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id)) AddAuthoredTrap(item, builtInTrap.Id);
		}
		var trappedDoors = 0;
		foreach (var (exit, room, side, reverse) in _doors)
		{
			if (((side.Flags | (reverse?.Flags ?? 0)) & 512) == 0 || exit.DoorId is not { } doorId) continue;
			var item = _db.GameItems.Local.Single(x => x.Id == doorId);
			AddAuthoredTrap(item, builtInTrap.Id);
			trappedDoors++;
		}
		Record("tool.explosive-traps", nameof(TrapTemplate), playerTrap.Id, 0, new
		{
			MaterialVnums = powders.Select(x => x.Value.Source.Vnum).Distinct().ToArray(),
			PretrappedVnums = trapped.Select(x => x.Value.Source.Vnum).Distinct().ToArray(), TrappedDoors = trappedDoors,
			SetupProg = setup.Id, PayloadProg = payload.Id, PreassembledTemplate = builtInTrap.Id,
			Disposition = "Native trap lay Powder Trap on <container|direction> using <held powder>. One shared door/item charge, consumed before opening or confirmed junk payload. Native skill check plus source quality. Failed setting consumes powder and inflicts 2d11+18 HP and source stun. Opening inflicts 2d16+18 HP, plus 1d16+9 per contained tagged explosive on containers, with source stun. Native perception/admin rules apply. No fuse, snare or disarming content."
		});
		_counts["explosive trap material definitions"] = powders.Length;
		_counts["pretrapped container definitions"] = trapped.Length;
		_counts["pretrapped doors"] = trappedDoors;
		Flush();
	}

	private static void AddAuthoredTrap(GameItem item, long template)
	{
		var xml = XElement.Parse(item.EffectData ?? "<Effects/>");
		if (xml.Elements("Effect").Any(x => (string?)x.Element("Type") == "Trap")) return;
		xml.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "Trap"),
			new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect",
				new XElement("InstanceId", Guid.NewGuid()), new XElement("TemplateId", template), new XElement("TemplateRevision", 0),
				new XElement("CreatorId", 0), new XElement("Power", "Standard"), new XElement("ChargesRemaining", 1),
				new XElement("State", "Armed"), new XElement("Components"))));
		item.EffectData = xml.ToString();
	}
}
