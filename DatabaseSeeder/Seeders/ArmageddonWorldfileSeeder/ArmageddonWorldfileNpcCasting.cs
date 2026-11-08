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
	private readonly Dictionary<int, TraitDefinition> _npcSpellTraits = new();
	private FutureProg? _npcCombatCasterPredicate;
	private FutureProg? _npcClassicCombatCasterPredicate;
	private readonly Dictionary<int, (long Capability, long Reserve, long Spell, int Proficiency)> _npcHealingRoutes = new();
	private Dictionary<long, NpcTemplate> _npcCombatTemplates = new();
	private Dictionary<string, FutureProg> _npcCombatOnloads = new(StringComparer.Ordinal);

	private void InstallNpcCombatAbilities()
	{
		// Cache tracked entities once: repeated DbSet.Local access runs change detection over the whole dump.
		var importedIds = _npcs.Values.Select(x => x.Id).Concat(_npcFamilyIds.Values.SelectMany(x => x)).ToHashSet();
		_npcCombatTemplates = _db.NpcTemplates.Local.Where(x => importedIds.Contains(x.Id)).ToDictionary(x => x.Id);
		_npcCombatOnloads = _progs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileNpcCash", StringComparison.Ordinal))
			.ToDictionary(x => x.FunctionName, StringComparer.Ordinal);
		InstallNpcAuxiliaryAvailability();
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var keys = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.Where(x => x.GetProperty("key").GetString()!.StartsWith("arm.spell.", StringComparison.Ordinal) &&
				x.TryGetProperty("historical_source", out var history) && history.ValueKind == JsonValueKind.Object &&
				history.TryGetProperty("skill_id", out var id) && id.ValueKind == JsonValueKind.Number)
			.ToDictionary(x => x.GetProperty("historical_source").GetProperty("skill_id").GetInt32(), x => x.GetProperty("key").GetString()!);
		var nativeCaps = _db.MagicCapabilities.ToArray();
		var casterCaps = new[] { "fire", "water", "earth", "wind", "lightning", "void", "defiler", "southern_templar", "lirathan_templar", "jihaen_templar" }
			.Select(x => OwnedBehaviourId("arm.capability." + x, nameof(MagicCapability))).Where(x => x.HasValue).Select(x => x!.Value).ToArray();
		_npcCombatCasterPredicate = Prog("ArmageddonWorldfileIsCombatCaster", "return (" +
			SourceNpcPredicate("subject", x => x.Identity[3] is 1 or 5 or 9 or 10 or 11 or 12 or 15 or 16 or 17 or 18) + ")" +
			string.Concat(casterCaps.Select(x => $" or contains(@subject.magiccapabilities, tomagiccapability({x}))")),
			ProgVariableTypes.Boolean, ("subject", ProgVariableTypes.Character));
		var classicCaps = new[] { "fire", "water", "earth", "wind", "defiler", "southern_templar", "lirathan_templar", "jihaen_templar" }
			.Select(x => OwnedBehaviourId("arm.capability." + x, nameof(MagicCapability))).Where(x => x.HasValue).Select(x => x!.Value);
		_npcClassicCombatCasterPredicate = Prog("ArmageddonWorldfileIsClassicCombatCaster", "return (" +
			SourceNpcPredicate("subject", x => x.Identity[3] is 1 or 5 or 9 or 10 or 11 or 12 or 17 or 18) + ")" +
			string.Concat(classicCaps.Select(x => $" or contains(@subject.magiccapabilities, tomagiccapability({x}))")),
			ProgVariableTypes.Boolean, ("subject", ProgVariableTypes.Character));
		var admissions = nativeCaps.SelectMany(cap => XElement.Parse(cap.Definition).Element("Casting")?.Elements("Admission")
			.Select(x => (Cap: cap, Admission: x)) ?? []).ToArray();
		var spellIds = keys.ToDictionary(x => x.Key, x => OwnedBehaviourId(x.Value, nameof(MagicSpell)));
		var boundSpells = spellIds.Where(x => x.Value.HasValue).ToDictionary(x => x.Key, x => _db.MagicSpells.Single(s => s.Id == x.Value!.Value));
		MagicSpell? travellerTeleport = null;
		foreach (var source in _source.Npcs)
		{
			var manoeuvres = NpcSavedCombatManoeuvres(source, _source.Skills);
			var saved = NpcSavedCombatSpells(source, _source.Skills);
			if (manoeuvres.Length == 0 && saved.Count == 0 && source.Identity[3] != 11) continue;
			var code = new StringBuilder("if (not(@npc.isawake) or not(@npc.incombat) or @npc.possessednpc or haspausedaieffect(@npc))\nreturn false\nend if\nvar target = @npc.combattargetchar\nif (isnull(@target) or @target.location != @npc.location)\nreturn false\nend if\n");
			if (source.Identity[3] == 11 && boundSpells.TryGetValue(30, out var calm))
				code.AppendLine($"if (healthremaining(@npc, \"health\") <= healthcapacity(@npc, \"health\") * 0.5 and contains(activespells(@target), tomagicspell({calm.Id})))\nforce @npc \"flee\"\nreturn true\nend if");
			if (manoeuvres.Length > 0)
			{
				code.AppendLine("if (not(cansee(@npc, @target)))\nreturn false\nend if");
				foreach (var verb in manoeuvres)
				{
					var selection = verb == "disarm"
						? (OwnedBehaviourId("arm.auxiliary.beat_the_weapon", nameof(CombatAction))
							?? throw new InvalidOperationException("NPC disarming requires the installed native Beat the Weapon action.")).ToString(System.Globalization.CultureInfo.InvariantCulture)
						: $"\"{verb}\"";
					code.AppendLine($"if (random(1, {(verb == "disarm" ? 5 : 4)}) == 1)\nreturn performcombataction(@npc, @target, {selection})\nend if");
				}
			}
			if (saved.Count > 0)
			{
				var baseKey = source.Identity[3] switch { 1 => "defiler", 5 => "southern_templar", 9 => "fire", 10 => "water", 11 => "earth", 12 => "wind", 14 => "shadow", 15 => "lightning", 16 => "void", _ => throw new InvalidOperationException("No source caster guild.") };
				var baseId = OwnedBehaviourId("arm.capability." + baseKey, nameof(MagicCapability))
					?? throw new InvalidOperationException($"NPC Vnum{source.Vnum} requires the installed {baseKey} casting foundation.");
				var baseline = nativeCaps.Single(x => x.Id == baseId);
				var capability = (MagicCapability)_db.Entry(baseline).CurrentValues.ToObject();
				capability.Id = Next<MagicCapability>(); capability.Name = "Vnum" + source.Vnum + " Spellcaster";
				var xml = XElement.Parse(capability.Definition);
				xml.Elements("Power").Remove(); xml.Element("Powers")?.Remove(); xml.Element("Gathering")?.Remove();
				var casting = xml.Element("Casting") ?? throw new InvalidOperationException("NPC casting requires a native casting policy.");
				casting.RemoveNodes(); casting.SetAttributeValue("identity", Guid.NewGuid()); casting.SetAttributeValue("enabled", true);
				casting.SetAttributeValue("startingVersion", 1); casting.SetAttributeValue("enrolOnClassSelection", false);
				var reserve = (long)casting.Attribute("reserve")!;
				var sourceResource = (long)casting.Attribute("source")!;
				code.AppendLine($"var mana = magicresourcelevel(@npc, {reserve})\nif (@mana < 10)\nreturn false\nend if\nvar grade = 1\nvar reduction = 4\nvar destination = \"\"\nvar result = \"\"\nif (@mana >= 50)\nreduction = 1\nelseif (@mana >= 33)\nreduction = 2\nelseif (@mana >= 20)\nreduction = 3\nend if");
				var installed = new List<object>();
				foreach (var entry in saved)
				{
					if (!boundSpells.TryGetValue(entry.SourceIndex, out var original))
						throw new InvalidOperationException($"Saved NPC combat spell {entry.Rule.SourceName} on Vnum{source.Vnum} lacks its installed native mapping.");
					var admissionSource = admissions.FirstOrDefault(x => (long)x.Admission.Attribute("spell")! == original.Id &&
						(long)XElement.Parse(x.Cap.Definition).Element("Casting")!.Attribute("source")! == sourceResource);
					if (admissionSource.Admission is null) throw new InvalidOperationException($"NPC spell {original.Name} has no compatible native proficiency admission.");
					var originalTrait = (long?)admissionSource.Admission.Attribute("trait") ??
						(long)XElement.Parse(admissionSource.Cap.Definition).Element("Casting")!.Attribute("trait")!;
					var trait = NpcSpellTrait(entry.SourceIndex, original.Name, originalTrait);
					var spell = entry.Rule.SourceName == "teleport" ? travellerTeleport ??= CreateNpcTravellerTeleport(original) : original;
					casting.Add(new XElement("Admission", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell.Id),
						new XAttribute("trait", trait.Id), new XAttribute("starting", true), new XAttribute("min", 1),
						new XAttribute("max", entry.MaximumGrade), new XAttribute("initialGrade", entry.MaximumGrade),
						new XAttribute("opening", entry.Proficiency), new XAttribute("rawCap", 100)));
					foreach (var id in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
					{
						var template = _npcCombatTemplates[id]; var definition = XElement.Parse(template.Definition);
						definition.Element("SelectedSkills")!.Add(new XElement("Skill", new XAttribute("Value", entry.Proficiency), trait.Id));
						template.Definition = definition.ToString();
					}
					var level = Math.Max(0, (entry.Proficiency - 30) / 10);
					installed.Add(new { entry.SourceIndex, entry.Proficiency, entry.MaximumGrade, Spell = spell.Id, Trait = trait.Id, entry.Rule });
					if (entry.Rule.SourceName == "heal") _npcHealingRoutes[source.Vnum] = (capability.Id, reserve, spell.Id, entry.Proficiency);
					if (entry.Rule.Condition == "ally-only") continue;
					code.AppendLine($"grade = min(7, max(1, {level} - @reduction + 1))");
					var condition = NpcSpellCondition(entry, spell, boundSpells, reserve);
					if (!entry.Rule.Self) code.AppendLine("if (cansee(@npc, @target))");
					code.AppendLine($"if ({condition})");
					code.AppendLine(entry.Rule.Self ? "destination = \"self\"" : "destination = bestkeyword(@npc, @target)");
					if (entry.Rule.SourceName == "repel")
						code.AppendLine("var paths = @npc.location.exits\nif (@paths.Count(path, true) == 0)\nreturn false\nend if\nvar path = getindex(@paths, random(0, @paths.Count(exit, true) - 1))\ndestination = @destination + \" \" + @path.direction");
					code.AppendLine($"result = channelspell(@npc, tomagiccapability({capability.Id}), tomagicspell({spell.Id}), @grade, false, @destination)\nif (not(startswith(@result, \"Refused:\")))\nreturn true\nend if\nend if");
					if (!entry.Rule.Self) code.AppendLine("end if");
				}
				capability.Definition = xml.ToString(); _db.MagicCapabilities.Add(capability);
				var merit = new Merit { Id = Next<Merit>(), Name = capability.Name, Type = "Magic Capability", MeritScope = 2, MeritType = 0,
					Definition = new XElement("Definition", new XElement("ChargenAvailableProg", _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
						new XElement("ApplicabilityProg", _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Authored NPC spell repertoire."),
						new XElement("DescriptionText", "@ practice|practices a restricted spell repertoire."), new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() };
				_db.Merits.Add(merit);
				foreach (var id in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
				{
					var template = _npcCombatTemplates[id]; var definition = XElement.Parse(template.Definition);
					if (definition.Element("Merits") is null) definition.Add(new XElement("Merits"));
					definition.Element("Merits")!.Add(new XElement("Merit", merit.Id)); template.Definition = definition.ToString();
				}
				var onload = _npcCombatOnloads["ArmageddonWorldfileNpcCash" + source.Vnum];
				onload.FunctionText = $"if (enrolchannelcasting(@npc, tomagiccapability({capability.Id}), \"Saved NPC combat repertoire Vnum{source.Vnum}\"))\nsetmagicresource(@npc, {reserve}, magicresourcecapacity(@npc, {reserve}))\nend if\n" + onload.FunctionText;
				Record("npc.combat.spells." + source.Vnum, nameof(MagicCapability), capability.Id, null,
					new { Source = source.Vnum, Guild = source.Identity[3], Merit = merit.Id, Reserve = reserve, Spells = installed,
						Policy = "Saved source-used spells only. Private native admissions and proficiency traits; no player class, support curriculum or extra powers. Source power tiers with native paid checks/materials. New NPCs start full; normal native regeneration entitlement is retained." });
			}
			code.AppendLine("return false");
			var prog = Prog("ArmageddonWorldfileCombatAbilities" + source.Vnum, code.ToString(), ProgVariableTypes.Boolean,
				("npc", ProgVariableTypes.Character), ("opponent", ProgVariableTypes.Character), ("successful", ProgVariableTypes.Boolean));
			InstallNpcEventHook("Combat Abilities " + Key(source.Vnum), EventType.CharacterCombatAttackResolved, prog, [source.Vnum]);
			Record("npc.combat.actions." + source.Vnum, "NpcCombatAbilityPolicy", prog.Id, null, new
			{ source.Vnum, Manoeuvres = manoeuvres, Timing = "One extra ability attempt after ordinary native melee/natural attack resolution, including misses; no separate polling clock." });
		}
		InstallNpcOpeningAbilities();
		InstallNpcAllySupport();
		_counts["NPC saved-spell repertoires"] = _source.Npcs.Count(x => NpcSavedCombatSpells(x, _source.Skills).Count > 0);
		_counts["NPC combat ability policies"] = _progs.Count(x => x.FunctionName.StartsWith("ArmageddonWorldfileCombatAbilities", StringComparison.Ordinal));
	}

	private void InstallNpcOpeningAbilities()
	{
		var allAis = _db.ArtificialIntelligences.Local.ToDictionary(x => x.Id);
		foreach (var source in _source.Npcs.Where(x => (x.Flags[0] & (1 << 5)) != 0 && !HasSpecial(x, "outrider")))
		{
			var saved = source.Skills.Where(x => x.Length >= 2 && x[0] >= 0 && x[0] < _source.Skills.Length)
				.Select(x => _source.Skills[x[0]]).ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (!saved.Contains("backstab") && !saved.Contains("sap")) continue;
			var family = _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]);
			var ais = family.SelectMany(id => _npcCombatTemplates[id].NpctemplatesArtificalIntelligences)
				.Select(x => allAis[x.AiId]).Where(x => x.Type == "Aggressor").Distinct().ToArray();
			if (ais.Length == 0) continue;
			var body = new StringBuilder("if (@npc.incombat or not(cansee(@npc, @target)) or @npc.location != @target.location)\nreturn false\nend if\n");
			foreach (var verb in new[] { "backstab", "sap" }.Where(saved.Contains))
				body.AppendLine($"force @npc (\"{verb} \" + bestkeyword(@npc, @target))\nif (@npc.incombat)\nreturn true\nend if");
			body.AppendLine("return false");
			var prog = Prog("ArmageddonWorldfileCombatOpening" + source.Vnum, body.ToString(), ProgVariableTypes.Boolean,
				("npc", ProgVariableTypes.Character), ("target", ProgVariableTypes.Character));
			foreach (var ai in ais)
			{
				var xml = XElement.Parse(ai.Definition); xml.SetElementValue("OpeningProg", prog.Id);
				var replacement = CreateAi("Authored Opening Vnum" + source.Vnum + " " + ai.Id, "Aggressor", xml);
				allAis.Add(replacement.Id, replacement);
				foreach (var id in family)
				{
					var template = _npcCombatTemplates[id];
					foreach (var link in template.NpctemplatesArtificalIntelligences.Where(x => x.AiId == ai.Id).ToArray())
					{
						template.NpctemplatesArtificalIntelligences.Remove(link); _db.Set<NpcTemplatesArtificalIntelligences>().Remove(link);
						template.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
						{ NpcTemplateId = template.Id, NpcTemplateRevisionNumber = template.RevisionNumber, AiId = replacement.Id });
					}
				}
			}
			Record("npc.combat.opening." + source.Vnum, "NpcCombatOpening", prog.Id, null,
				new { source.Vnum, Commands = saved.Where(x => x is "backstab" or "sap").ToArray(), Policy = "Native weapon/skill/stealth admission and attack resolution; normal native aggression on refusal. Outrider keeps its explicitly approved ordinary opener." });
		}
	}

	private TraitDefinition NpcSpellTrait(int source, string name, long basisId)
	{
		if (_npcSpellTraits.TryGetValue(source, out var existing)) return existing;
		var basis = _db.TraitDefinitions.Single(x => x.Id == basisId && x.Type == 0);
		var trait = (TraitDefinition)_db.Entry(basis).CurrentValues.ToObject();
		trait.Id = Next<TraitDefinition>(); trait.Name = "NPC " + name; trait.Alias = "npc_spell_" + source;
		trait.TraitGroup = "NPC Spell Proficiencies"; trait.Hidden = true; trait.BranchMultiplier = 0;
		trait.AvailabilityProgId = trait.LearnableProgId = trait.TeachableProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		var cap = new TraitExpression { Id = Next<TraitExpression>(), Name = trait.Name + " Cap", Expression = "100" };
		_db.TraitExpressions.Add(cap); trait.ExpressionId = cap.Id;
		trait.ImproverId = _db.Improvers.Local.FirstOrDefault(x => x.Type == "non-improving")?.Id ??
			_db.Improvers.First(x => x.Type == "non-improving").Id;
		_db.TraitDefinitions.Add(trait); _npcSpellTraits.Add(source, trait);
		Record("npc.spell.trait." + source, nameof(TraitDefinition), trait.Id, null, new { SourceSkill = source, Basis = basisId, NativeName = name });
		return trait;
	}

	private void InstallNpcAuxiliaryAvailability()
	{
		var needed = new HashSet<(long Race, long Action)>();
		foreach (var source in _source.Npcs)
		foreach (var verb in NpcSavedCombatManoeuvres(source, _source.Skills))
		{
			var key = verb == "disarm" ? "arm.auxiliary.beat_the_weapon" : "arm.action." + verb;
			var action = OwnedBehaviourId(key, nameof(CombatAction))
				?? throw new InvalidOperationException($"Saved NPC {verb} requires its installed native combat action.");
			foreach (var id in _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]))
				if ((long?)XElement.Parse(_npcCombatTemplates[id].Definition).Element("SelectedRace") is { } race)
					needed.Add((race, action));
		}
		var existing = _db.RacesCombatActions.Select(x => new { x.RaceId, x.CombatActionId }).AsEnumerable()
			.Select(x => (Race: x.RaceId, Action: x.CombatActionId)).ToHashSet();
		var parents = _db.Races.Select(x => new { x.Id, x.ParentRaceId }).ToDictionary(x => x.Id, x => x.ParentRaceId);
		foreach (var entry in needed)
		{
			var inherited = false; var seen = new HashSet<long>();
			long? current = entry.Race;
			while (current is { } race && seen.Add(race))
			{
				if (existing.Contains((race, entry.Action)) || race != entry.Race && needed.Contains((race, entry.Action)))
				{ inherited = true; break; }
				current = parents.GetValueOrDefault(race);
			}
			if (inherited) continue;
			_db.RacesCombatActions.Add(new RacesCombatActions { RaceId = entry.Race, CombatActionId = entry.Action });
			existing.Add(entry);
			Record($"npc.combat.race-action.{entry.Race}.{entry.Action}", "NpcRaceCombatAction", entry.Race, null,
				new { Race = entry.Race, Action = entry.Action, Policy = "Reuse the native action for a race with an authored NPC ability; do not duplicate inherited availability or grant proficiency." });
		}
	}

	private string NpcSpellCondition(NpcSavedSpell entry, MagicSpell spell, IReadOnlyDictionary<int, MagicSpell> spells, long reserve)
	{
		var subject = entry.Rule.Self ? "npc" : "target";
		string Active(int index, string who) => spells.TryGetValue(index, out var value) ? $"contains(activespells(@{who}), tomagicspell({value.Id}))" : "false";
		var caster = $"@{_npcCombatCasterPredicate!.FunctionName}(@target)";
		var classicCaster = $"@{_npcClassicCombatCasterPredicate!.FunctionName}(@target)";
		var absent = $"not(contains(activespells(@{subject}), tomagicspell({spell.Id})))";
		return entry.Rule.Condition switch
		{
			"injured" => "healthremaining(@npc, \"health\") < healthcapacity(@npc, \"health\") * 0.5",
			"losing" => "healthremaining(@npc, \"health\") <= healthcapacity(@npc, \"health\") * 0.5 and healthremaining(@npc, \"health\") < healthremaining(@target, \"health\")",
			"buffed" => string.Join(" or ", new[] { 17, 34, 19, 1 }.Select(x => Active(x, "target"))),
			"caster" => caster,
			"caster-unsilenced" => caster + " and not(hassilenceeffect(@target))",
			"classic-caster-unsilenced" => classicCaster + " and not(hassilenceeffect(@target))",
			"classic-caster-unbuffed" => classicCaster + " and " + absent,
			"classic-caster-aura" => classicCaster + $" and magicresourcelevel(@target, {reserve}) >= magicresourcecapacity(@target, {reserve}) * 0.25",
			"sighted" => "not(hasblindnesseffect(@target))",
			"mobile" => "not(hasparalysiseffect(@target))",
			"unpoisoned" => OwnedBehaviourId("arm.effect.poison", nameof(Drug)) is { } poison
				? $"not(hasdrugdosage(@target, todrug({poison}), \"\"))"
				: throw new InvalidOperationException("NPC poison casting requires the installed native magical toxin."),
			"above-second-tier" => "@grade >= 3",
			"outdoors" => "@npc.location.outdoors == 2",
			"always" => "true",
			_ => absent
		};
	}

	private MagicSpell CreateNpcTravellerTeleport(MagicSpell original)
	{
		var spell = (MagicSpell)_db.Entry(original).CurrentValues.ToObject();
		spell.Id = Next<MagicSpell>(); spell.Name = "Displace Traveller";
		spell.Blurb = "Displace a visible local traveller through the configured Waystep pool.";
		spell.Description = "Move the selected local traveller through the native same-plane Waystep pool, retaining wards, room policy and relocation safety. This authored route does not add Banish Traveller's saving throw or admit rune destinations.";
		spell.SpellKnownProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		var definition = XElement.Parse(spell.Definition);
		definition.Element("Trigger")!.SetAttributeValue("type", "banishtraveller");
		foreach (var effect in definition.Element("Effects")!.Elements().Where(x => (string?)x.Attribute("type") == "waystep")) effect.SetElementValue("TargetTraveller", true);
		spell.Definition = definition.ToString(); _db.MagicSpells.Add(spell);
		Record("npc.spell.teleport-traveller", nameof(MagicSpell), spell.Id, null, new { Basis = original.Id, NativePool = "Waystep", Resisted = false });
		return spell;
	}
}
