#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallSourceSpellMaterialPlans()
	{
		var exemptions = new List<string> { "isadmin(@actor)" };
		foreach (var key in new[] { "jihaen_templar", "lirathan_templar", "southern_templar" })
			if (OwnedBehaviourId("arm.class." + key, nameof(ChargenRole)) is { } role)
				exemptions.Add($"contains(@actor.roles, torole({role}))");
		foreach (var source in _source.Races.Where(x => x.Name.Equals("elemental", StringComparison.OrdinalIgnoreCase) || x.Name.Equals("fiend", StringComparison.OrdinalIgnoreCase)))
			if (_races.TryGetValue(source.Index, out var race)) exemptions.Add($"@actor.race.id == {race.Id}");
		var ordinary = Policy("Ordinary", "if (@actor.npc)\nreturn true\nend if\nreturn not(" + string.Join(" or ", exemptions) + ")");
		var immortalOnly = Policy("Immortal", "return not(isadmin(@actor))");
		var classic = OwnedBehaviourId("arm.class.southern_templar", nameof(ChargenRole));
		var exchange = Policy("Exchange", "if (@actor.npc)\nreturn true\nend if\nreturn not(isadmin(@actor)" +
			(classic is { } id ? $" or contains(@actor.roles, torole({id}))" : "") + ")");
		var stone = OwnedBehaviourId("arm.class.earth", nameof(ChargenRole));
		var mount = stone is { } stoneRole ? Policy("Mount", $"if (@actor.npc)\nreturn true\nend if\nif (contains(@actor.roles, torole({stoneRole})))\nreturn false\nend if\nreturn not(" + string.Join(" or ", exemptions) + ")") : ordinary;

		foreach (var requirement in new (string Key, string Sphere, int Offset)[]
		{
			("compelling_regard", "alteration", -1), ("consuming_flame", "necromancy", -1),
			("shadow_passage", "alteration", -1), ("haunting_shape", "conjuration", -1),
			("storm_spear", "creation", -3), ("storm_locus", "nature", -1),
			("wind_courier", "conjuration", -1), ("earth_mount", "creation", -1),
			("open_threshold", "teleportation", -1), ("borrow_the_dead", "necromancy", -1),
			("join_traveller", "teleportation", -1), ("draw_traveller", "conjuration", -1),
			("walking_shadow", "illusion", -1), ("seeking_wind", "conjuration", -1),
			("anchor_rune", "creation", -1)
		})
			Edit(requirement.Key, xml => ReplacePlan(xml, [Ranked(requirement.Sphere, requirement.Offset,
				requirement.Key == "earth_mount" ? mount.Id : ordinary.Id)]));

		Edit("air_guardian", xml => ReplacePlan(xml, Enumerable.Range(2, 6).Select(grade => Ranked("conjuration", -1, ordinary.Id, grade))));
		foreach (var (key, sphere) in new[] { ("flame_knife", "conjuration"), ("spark_lash", "creation"), ("shadow_blade", "creation"),
			("sand_knife", "creation"), ("mist_shield", "conjuration"), ("wind_shield", "conjuration") })
			Edit(key, xml => ReplacePlan(xml, [Fixed(sphere, 6, ordinary.Id, 7)]));
		Edit("kindle_enchantment", xml => ReplacePlan(xml, [Fixed("enchantment", 6, immortalOnly.Id, 7), Fixed("alteration", 6, immortalOnly.Id, 7)]));
		Edit("call_outsider", xml =>
		{
			var action = Ranked("conjuration", -1, ordinary.Id);
			action.SetAttributeValue("originalreference", "summon.material");
			ReplacePlan(xml, [action]);
		});
		Edit("see_the_unbodied", xml =>
		{
			// Its native effect owns Shadow-plane waiving and Silt refusal, including frozen selection.
			xml.Element("Plan")!.Descendants("Action").Single().SetAttributeValue("castrequiredprog", ordinary.Id);
		});
		Edit("sheltering_veil", xml =>
		{
			var action = Ranked("clerical", -1, ordinary.Id, hierarchy: true);
			action.SetAttributeValue("originalreference", "armageddon.sheltering.clerical");
			ReplacePlan(xml, [action]);
		});
		Edit("exchange_places", xml =>
		{
			ReplacePlan(xml, [Ranked("alteration", -1, exchange.Id, hierarchy: true)]);
			var effect = xml.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "exchangeplaces");
			effect.SetElementValue("ComponentRequiredProg", exchange.Id);
			effect.SetElementValue("RetainedRankOffset", -1);
			effect.SetElementValue("DirectlyCarriedOnly", true);
			effect.Element("RetainedRanks")!.ReplaceNodes(Enumerable.Range(0, 7).Select(rank => new XElement("Rank", new XAttribute("number", rank), new XAttribute("tag", Rank("teleportation", rank)))));
		});
		Edit("return_tether", xml =>
		{
			var action = Ranked("teleportation", -1, ordinary.Id);
			action.SetAttributeValue("castoptional", true);
			action.SetAttributeValue("originalreference", "return.duration");
			ReplacePlan(xml, [action]);
			xml.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "returntether").SetElementValue("OptionalDurationComponent", "return.duration");
		});
		return;

		FutureProg Policy(string suffix, string text) => Prog("ArmageddonWorldfileMaterial" + suffix, text, ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		long Rank(string sphere, int rank) => ComponentRank(sphere, rank)?.Id ?? throw new InvalidOperationException($"Missing or modified native {sphere} rank {rank} tags; spell requirements cannot be installed safely.");
		XElement Fixed(string sphere, int rank, long prog, int? grade = null) => Action(Rank(sphere, rank), prog, grade);
		XElement Ranked(string sphere, int offset, long prog, int? grade = null, bool hierarchy = false)
		{
			var action = Action(hierarchy ? Rank(sphere, 0) : 0, prog, grade);
			action.Add(new XElement("GradeRank", new XAttribute("offset", offset), hierarchy ? null : new XAttribute("selection", "explicit"),
				Enumerable.Range(0, 7).Select(rank => new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", Rank(sphere, rank))))));
			return action;
		}
		static XElement Action(long tag, long prog, int? grade) => new("Action", new XAttribute("state", "consumed"), new XAttribute("tag", tag),
			new XAttribute("secondtag", 0), new XAttribute("quantity", 1), new XAttribute("carriedonly", true),
			new XAttribute("castrequiredprog", prog), grade is { } g ? new XAttribute("grade", g) : null);
		static void ReplacePlan(XElement xml, IEnumerable<XElement> actions) => xml.Element("Plan")!.ReplaceNodes(new XElement("Phase", actions));
		void Edit(string suffix, Action<XElement> edit)
		{
			if (OwnedBehaviourId("arm.spell." + suffix, nameof(MagicSpell)) is not { } id) throw new InvalidOperationException($"Missing native spell {suffix}.");
			var spell = _db.MagicSpells.Find(id) ?? throw new InvalidOperationException($"Missing native spell {id}.");
			var xml = XElement.Parse(spell.Definition);
			edit(xml);
			spell.Definition = xml.ToString();
			spell.Description = (spell.Description ?? "")
				.Replace("max(grade-2,0)", "max(grade-1,0)", StringComparison.Ordinal)
				.Replace("minimum ranks 0/0/0/1/2/3/4 by grade", "minimum ranks 0/1/2/3/4/5/6 by grade", StringComparison.Ordinal)
				.Replace("Retain a directly carried Teleportation component or one accessible immediately inside an open carried container", "Retain a directly carried Teleportation component", StringComparison.Ordinal)
				.Replace("Every grade expires; no permanent output or component is required.", "Every grade expires; grade seven requires one carried Creation rank-six component.", StringComparison.Ordinal);
			if (suffix == "return_tether") spell.Description += " An available directly carried Teleportation component of rank grade minus one increases the delay by max(2, grade); it is consumed unless material payment is exempt. Without one the ordinary delay applies.";
			Record("spellmaterial." + suffix, nameof(MagicSpell), id, null, new { Policy = "source requirements; native prepayment validation", Definition = xml.ToString() });
		}
	}
}
