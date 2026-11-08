using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

#nullable enable
namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPoisonCleansing()
	{
		var baseId = OwnedBehaviourId("arm.spell.purge_toxin", nameof(MagicSpell));
		if (baseId is null) return;
		var original = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == baseId) ?? _db.MagicSpells.Single(x => x.Id == baseId);
		var baseDrug = (long)XElement.Parse(original.Definition).Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "removepoison").Element("Drug")!;
		InstallCurativeLeaves(baseDrug);
		var sourceGroups = new List<long[]> { new[] { baseDrug }, new[] { _poisonDrugIds[1] }, new[] { _poisonDrugIds[2] },
			new[] { _poisonDrugIds[3], _poisonDrugIds[9], _poisonDrugIds[10] } };
		sourceGroups.AddRange(new[] { 4, 5, 6, 7, 8 }.Select(kind => new[] { _poisonDrugIds[kind] }));
		ConfigurePoisonCure(original, sourceGroups);
		var kelwinRecord = _db.SeederManagedRecords.Local.SingleOrDefault(x => x.StableKey == "npc-service.kelwin.cure" && !x.Retired)?.LogicalId
			?? OwnedBehaviourId("npc-service.kelwin.cure", nameof(MagicSpell));
		if (kelwinRecord is { } kelwinId)
		{
			var kelwin = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == kelwinId) ?? _db.MagicSpells.Single(x => x.Id == kelwinId);
			var groups = sourceGroups.Where(group => !group.Contains(_poisonDrugIds[4]) && !group.Contains(_poisonDrugIds[8])).ToList();
			var venoms = XElement.Parse(kelwin.Definition).Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "removepoison")
				.Element("AdditionalDrugs")?.Elements("Drug").Select(x => (long)x).ToArray() ?? [];
			groups.AddRange(venoms.Select(id => new[] { id }));
			ConfigurePoisonCure(kelwin, groups);
		}
		var clean = new StringBuilder($"var kind = getregister(@item, \"{PoisonTargetRegister}\")\nif (@kind != 5 and @kind != 17 and @kind != 19 and @kind != 24 and not(hasweaponpoison(@item)))\nreturn\nend if\nremoveweaponpoison(@item)\nsetregister @item \"{PoisonKindRegister}\" 0\nvar from as number collection\nvar to as number collection\n");
		foreach (var (poisoned, basis) in _poisonLiquidBases)
			clean.AppendLine($"additem from {poisoned}\nadditem to {basis}");
		clean.AppendLine("remapliquids(@item, @from, @to, false, false)");
		var prog = Prog("ArmageddonWorldfileCleansePoisonedItem", clean.ToString(), ProgVariableTypes.Void,
			("caster", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item));
		var filter = Prog("ArmageddonWorldfileCleansePoisonedItemFilter", $"var kind = getregister(@item, \"{PoisonTargetRegister}\")\nreturn @kind == 5 or @kind == 17 or @kind == 19 or @kind == 24 or hasweaponpoison(@item)",
			ProgVariableTypes.Boolean, ("item", ProgVariableTypes.Item), ("caster", ProgVariableTypes.Character));
		var spell = (MagicSpell)_db.Entry(original).CurrentValues.ToObject();
		spell.Id = Next<MagicSpell>(); spell.Name = "Cleanse Poisoned Object";
		spell.Blurb = "Remove poison from a weapon, dart, meal or drink.";
		spell.Description = "A native item-targeted form of Purge Toxin. Removes coatings or imported food/drink poison without consuming contents or healing damage.";
		spell.CastingEmote = "$0 place|places a hand over $1, extracting a dark aura from it.";
		spell.TargetEmote = spell.TargetNullEmote = spell.TargetResistedEmote = string.Empty;
		var xml = XElement.Parse(spell.Definition);
		var trigger = xml.Element("Trigger")!;
		trigger.SetAttributeValue("type", "item"); trigger.SetElementValue("TargetFilterProg", filter.Id);
		xml.Element("Effects")!.ReplaceNodes(new XElement("Effect", new XAttribute("type", "executeprog"), new XElement("Prog", prog.Id)));
		spell.Definition = xml.ToString(); _db.MagicSpells.Add(spell);
		Record("poison.item-cleansing", nameof(MagicSpell), spell.Id, null, new { NativeBasis = original.Id,
			Disposition = "Native item trigger/prog, existing school/knowledge/costs. Coatings and source food/drink payloads are cleared; closed vessels are supported and unmapped constituents preserved. Source spice is not a cure-poison object target." });
	}

	private static void ConfigurePoisonCure(MagicSpell spell, IEnumerable<long[]> groups)
	{
		var families = groups.ToArray();
		var xml = XElement.Parse(spell.Definition);
		var removal = xml.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "removepoison");
		removal.Element("AdditionalDrugs")?.Remove(); removal.Element("DrugGroups")?.Remove();
		removal.Add(new XElement("AdditionalDrugs", families.SelectMany(x => x).Distinct().Select(id => new XElement("Drug", id))),
			new XElement("DrugGroups", families.Select(family => new XElement("Group", family.Select(id => new XElement("Drug", id))))));
		removal.SetElementValue("FirstMatchingGroup", true); removal.SetElementValue("RemoveAftereffects", true);
		spell.Definition = xml.ToString();
	}

	private void InstallCurativeLeaves(long magicalPoison)
	{
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.Programs.Contains("C apply poison_curative")))
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			var prog = Prog("ArmageddonWorldfileCurativeLeaf" + id, $$"""
				if (@args.Count(word, true) == 0)
					return false
				end if
				if (targetitem(@actor, @actor.inventory, getindex(@args, 0)) != @item)
					return false
				end if
				if (@args.Count(word, true) != 2)
					send(@actor, "Apply to whom? Use apply <leaf> <person>.")
					return true
				end if
				var patient = targetcharacter(@actor, getindex(@args, 1))
				if (isnull(@patient))
					send(@actor, "You do not see that person here.")
					return true
				end if
				if (not(@actor.isawake) or isincorporeal(@actor) or isincorporeal(@patient))
					send(@actor, "You cannot apply the leaf right now.")
					return true
				end if
				sendloc(@actor.location, "$0 rub|rubs $1 on $2's skin.", @actor, @item, @patient)
				deleteitem(@item)
				if (purgedrugdosages(@patient, todrug({{magicalPoison}}), "arm.effect.poison"))
					send(@patient, "The burning in your veins ceases.")
				end if
				return true
				""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
				("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			AttachSpiceHook(proto, "CurativeLeafHook", SpiceHook("apply", prog));
			Record("item-special.curative-leaf." + id, nameof(FutureProg), prog.Id, null, new
			{
				SourceVnum = effective.Source.Vnum, Drug = magicalPoison, Family = "arm.effect.poison",
				Disposition = "Source attached topical cure consumes the leaf even on an unaffected patient. Only magical poison is removed; mundane toxins, injuries and permanent losses remain. No new reset or supply is invented for this definition-only item."
			});
		}
	}
}
