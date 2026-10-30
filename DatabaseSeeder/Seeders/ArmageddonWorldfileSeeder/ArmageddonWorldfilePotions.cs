#nullable enable

using System;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallHealingPotions()
	{
		var candidates = _effectiveItems.Where(x => x.Value.Source.ItemType == 10).ToArray();
		if (candidates.Length == 0) return;
		var baseId = OwnedBehaviourId(ArmageddonReviewedUtilityContent.MendFleshKey, nameof(MagicSpell));
		var original = baseId is null ? null : _db.MagicSpells.Local.FirstOrDefault(x => x.Id == baseId) ?? _db.MagicSpells.Single(x => x.Id == baseId);
		foreach (var (id, effective) in candidates)
		{
			var spells = effective.Values.Skip(1).Take(3).Where(x => x > 0).ToArray();
			if (original is null || spells.Length != 1 || spells[0] != 12 || effective.Values[0] is < 1 or > 7)
			{
				Adapt("potion", effective.Source.Vnum, "Only one explicitly mapped source heal payload at levels 1-7 is installed; mixed, unknown or unavailable spell payloads remain deferred as whole items.");
				continue;
			}
			var definition = XElement.Parse(original.Definition);
			var filterId = (long?)definition.Element("Trigger")?.Element("TargetFilterProg") ?? 0;
			var filter = _db.FutureProgs.Local.FirstOrDefault(x => x.Id == filterId) ?? _db.FutureProgs.SingleOrDefault(x => x.Id == filterId);
			var effects = definition.Element("Effects")!.Elements().ToArray();
			if (filter is null || effects.Length != 1 || (string?)effects[0].Attribute("type") != "heal")
				throw new InvalidOperationException("Healing fruit requires the installed Mend Flesh healing effect and target eligibility prog.");
			var effect = new XElement(effects[0]);
			effect.Element("HealingAmount")!.Value = Regex.Replace(effect.Element("HealingAmount")!.Value,
				@"\bgrade\b", effective.Values[0].ToString(CultureInfo.InvariantCulture), RegexOptions.IgnoreCase);
			var spell = new MagicSpell
			{
				Id = Next<MagicSpell>(), Name = "Vnum" + effective.Source.Vnum + " Healing", Blurb = "Prepared healing fruit",
				Description = "Source-level healing delivered by consuming the prepared item. Uses the installed native Mend Flesh effect and eligibility policy without Mana costs.",
				MagicSchoolId = original.MagicSchoolId, SpellKnownProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				MinimumSuccessThreshold = 1, CastingEmote = "", FailCastingEmote = "", TargetEmote = original.TargetEmote,
				TargetResistedEmote = "", TargetNullEmote = "", TargetEmoteFlags = original.TargetEmoteFlags,
				Definition = new XElement("Spell", new XElement("Trigger", new XAttribute("type", "substancecharacter")),
					new XElement("Costs"), new XElement("Effects", effect), new XElement("CasterEffects"), new XElement("Plan")).ToString()
			};
			_db.MagicSpells.Add(spell);
			var substance = new MagicalSubstance { Id = Next<MagicalSubstance>(), Name = "Vnum" + effective.Source.Vnum + " Healing Dose",
				Definition = new XElement("Substance", new XAttribute("vectors", (int)DrugVector.Ingested),
					new XAttribute("power", (int)SpellPower.ExtremelyWeak + effective.Values[0] - 1), new XAttribute("reference", 1), new XAttribute("clearance", 1),
					new XElement("Entry", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell.Id),
						new XAttribute("lifecycle", (int)SubstanceLifecycle.Activation), new XAttribute("pulse", (int)SubstancePulseMode.Presence),
						new XAttribute("stack", (int)SubstanceStacking.Aggregate), new XAttribute("scale", (int)SubstanceScaling.Magnitude),
						new XAttribute("minimum", 1), new XAttribute("maximum", 1), new XAttribute("duration", 1), new XAttribute("durationcap", 1), new XAttribute("interval", 10))).ToString() };
			_db.MagicalSubstances.Add(substance);
			var consume = Prog("ArmageddonWorldfilePotionConsume" + id,
				$"if (@{filter.FunctionName}(@actor, @actor))\napplymagicalsubstance(@actor, {substance.Id}, 1, \"ingested\")\nend if",
				ProgVariableTypes.Void, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item));
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			var pill = Component("potion." + id, "Pill", new XElement("Definition", new XElement("Drug", 0), new XElement("GramsPerPill", 0),
				new XElement("OnSwallowProg", consume.Id), new XElement("ConsumptionEmote", "@ quaff|quaffs $0, which dissolves away.")));
			AttachDeliveryComponent(proto, pill, new XElement("Definition"));
			var input = Prog("ArmageddonWorldfilePotionInput" + id,
				"if (@args.Count(word, true) != 1)\nreturn false\nend if\nif (targetitem(@actor, @actor.helditems, getindex(@args, 0)) != @item)\nreturn false\nend if\nswallowitem(@actor, @item)\nreturn true",
				ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
				("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			AttachSpiceHook(proto, "PotionInputHook", SpiceHook("quaff", input));
			Record("potion.healing." + id, nameof(MagicalSubstance), substance.Id, null,
				new { SourceVnum = effective.Source.Vnum, SourceLevel = effective.Values[0], Spell = spell.Id, Eligibility = filter.Id,
					Disposition = "One-shot native swallow and substance healing; consumes even if ineligible. Installed Mend Flesh mean wound budget, no terrain level shifts, no drinker Mana or casting skill. No direct carrier binding: on-swallow prog checks native target eligibility first." });
		}
	}
}
