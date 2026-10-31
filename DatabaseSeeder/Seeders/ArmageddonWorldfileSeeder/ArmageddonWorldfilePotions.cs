#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPotions()
	{
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var keys = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.Where(x => x.TryGetProperty("historical_source", out var source) && source.ValueKind == JsonValueKind.Object &&
				source.TryGetProperty("skill_id", out var skill) && skill.ValueKind == JsonValueKind.Number &&
				x.GetProperty("key").GetString()!.StartsWith("arm.spell.", StringComparison.Ordinal))
			.ToDictionary(x => x.GetProperty("historical_source").GetProperty("skill_id").GetInt32(), x => x.GetProperty("key").GetString()!);
		MagicSpell? rewind = null;
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 10).ToArray())
		{
			var slots = effective.Values.Skip(1).Take(3).ToArray();
			var payloads = slots.Where(x => x > 0 && x is not (28 or 33 or 36 or 42)).ToArray();
			var sourceGrade = effective.Values[0];
			var grade = Math.Min(sourceGrade, 7);
			if (sourceGrade > 7)
				Adapt("potion", effective.Source.Vnum, $"Saved grade {sourceGrade} maps to maximum native grade 7 by the approved import policy.");
			if (payloads.Length > 0 && grade < 1)
			{
				Adapt("potion", effective.Source.Vnum, "Saved grade is below native grade 1; the complete item remains inactive.");
				continue;
			}
			// Resolve every slot before creating anything. Missing dependencies must never produce a partial potion.
			var resolved = new List<(int Source, MagicSpell Spell)>();
			foreach (var source in payloads)
			{
				if (!keys.TryGetValue(source, out var key) || OwnedBehaviourId(key, nameof(MagicSpell)) is not { } spellId) break;
				var spell = _db.MagicSpells.Local.FirstOrDefault(x => x.Id == spellId) ?? _db.MagicSpells.SingleOrDefault(x => x.Id == spellId);
				if (spell is null) break;
				var definition = XElement.Parse(spell.Definition);
				if (definition.Element("ControlledPower")?.Elements("Grade").Any(x => (int?)x.Attribute("number") == grade) != true) break;
				var filterId = (long?)definition.Element("Trigger")?.Element("TargetFilterProg") ?? 0;
				if (filterId != 0 && !_db.FutureProgs.Local.Any(x => x.Id == filterId) && !_db.FutureProgs.Any(x => x.Id == filterId)) break;
				resolved.Add((source, spell));
			}
			if (resolved.Count != payloads.Length)
			{
				Adapt("potion", effective.Source.Vnum, "At least one complete native payload, grade or eligibility dependency is unavailable; all potion payloads remain inactive.");
				continue;
			}
			var entries = resolved.Select(x => new SubstanceEffectEntry
			{
				SpellId = x.Source == 65 ? (rewind ??= CreatePotionRewind(x.Spell)).Id : x.Spell.Id,
				NativeSpellGrade = grade, Lifecycle = SubstanceLifecycle.Activation, Scaling = SubstanceScaling.Magnitude,
				MinimumDose = 1, MaximumDose = 1
			}).ToArray();
			MagicalSubstance? substance = null;
			if (entries.Length > 0)
			{
				substance = new MagicalSubstance
				{
					Id = Next<MagicalSubstance>(), Name = "Vnum" + effective.Source.Vnum + " Potion " + id,
					Definition = new XElement("Substance", new XAttribute("vectors", (int)DrugVector.Ingested),
						new XAttribute("power", (int)SpellPower.ExtremelyWeak + grade - 1), new XAttribute("reference", 1), new XAttribute("clearance", 1),
						entries.Select(PotionEntry)).ToString()
				};
				_db.MagicalSubstances.Add(substance);
			}
			var consume = Prog("ArmageddonWorldfilePotionConsume" + id,
				substance is null ? "return" : $"applymagicalsubstance(@actor, {substance.Id}, 1, \"ingested\")",
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
			Record("potion." + id, nameof(GameItemProto), proto.Id, proto.RevisionNumber, new
			{
				SourceVnum = effective.Source.Vnum, SourceLevel = sourceGrade, NativeGrade = grade, Slots = slots,
				Substance = substance?.Id, Spells = entries.Select(x => x.SpellId).ToArray(),
				Disposition = "Complete ordered native spell payloads at saved grade capped at native grade 7, including duplicates; no Mana, skill roll or extra materials. Source slots 28/33/36/42 are mechanical no-ops. Empty potions are consumable. Rewind gives a delayed native route hint."
			});
		}
	}

	private static XElement PotionEntry(SubstanceEffectEntry entry) => new("Entry", new XAttribute("key", entry.Key),
		new XAttribute("spell", entry.SpellId), new XAttribute("nativegrade", entry.NativeSpellGrade),
		new XAttribute("lifecycle", (int)entry.Lifecycle), new XAttribute("pulse", (int)entry.PulseMode),
		new XAttribute("stack", (int)entry.Stacking), new XAttribute("scale", (int)entry.Scaling),
		new XAttribute("minimum", 1), new XAttribute("maximum", 1), new XAttribute("duration", 1),
		new XAttribute("durationcap", 1), new XAttribute("interval", 10));

	private MagicSpell CreatePotionRewind(MagicSpell original)
	{
		var definition = XElement.Parse(original.Definition);
		definition.Element("StockIdentity")?.Remove();
		definition.Element("Effects")!.ReplaceNodes(new XElement("Effect", new XAttribute("type", "delayedlocationhint"),
			new XElement("BaseDelay", 1.875), new XElement("DelayPerGrade", 1.875),
			new XElement("BaseRange", 10), new XElement("RangePerGrade", 10)));
		var spell = new MagicSpell
		{
			Id = Next<MagicSpell>(), Name = "Potion Rewind", Blurb = "Remember a location through a delayed magical hint.",
			Description = "A delayed native route hint to the drinker's position when this payload activates. It never moves the recipient.",
			MagicSchoolId = original.MagicSchoolId, SpellKnownProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
			CastingTraitDefinitionId = original.CastingTraitDefinitionId, CastingDifficulty = original.CastingDifficulty,
			EffectDurationExpressionId = original.EffectDurationExpressionId, MinimumSuccessThreshold = original.MinimumSuccessThreshold,
			CastingEmote = "", FailCastingEmote = "", TargetEmote = "", TargetResistedEmote = "", TargetNullEmote = "",
			AppliedEffectsAreExclusive = false, Definition = definition.ToString()
		};
		_db.MagicSpells.Add(spell);
		Record("potion.rewind", nameof(MagicSpell), spell.Id, null, new { Disposition = "Source delayed direction hint; native bounded path permissions; no teleportation." });
		return spell;
	}
}
