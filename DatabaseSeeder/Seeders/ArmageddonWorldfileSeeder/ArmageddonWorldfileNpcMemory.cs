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
	private FutureProg? _npcMemoryRawHate;
	private FutureProg EnsureNpcMemoryPolicy()
	{
		if (_npcMemoryRawHate is not null) return _npcMemoryRawHate;
		EnsureMountRegister(ProgVariableTypes.Character, NpcTribeRegister, ProgVariableTypes.Number, "<var>0</var>");
		Prog("ArmageddonWorldfileMemorySameClan", $$"""
		return @first.clans.Any(clan, isclanmember(@second, @clan)) or (getregister(@first, "{{NpcTribeRegister}}") > 0 and getregister(@first, "{{NpcTribeRegister}}") == getregister(@second, "{{NpcTribeRegister}}"))
		""", ProgVariableTypes.Boolean, ("first", ProgVariableTypes.Character), ("second", ProgVariableTypes.Character));
		return _npcMemoryRawHate = Prog("ArmageddonWorldfileRawHate", $$"""
		if (isnull(@subject) or isnull(@actor) or @subject == @actor or not(@subject.npc) or ({{SourceNpcPredicate("subject", x => (x.Flags[0] & (1 << 29)) != 0)}}))
			return false
		end if
		return remember(@subject, @actor, "Hate")
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("subject", ProgVariableTypes.Character));
	}

	private void InstallNpcMemories()
	{
		var raw = EnsureNpcMemoryPolicy();
		var eligible = _source.Npcs.Where(x => (x.Flags[0] & (1 << 29)) == 0).Select(x => x.Vnum).ToArray();
		var ordinary = Prog("ArmageddonWorldfileOrdinaryHate", $$"""
		if (isnull(@aggressor) or @observer == @aggressor or not(@observer.npc))
			return false
		end if
		if ({{ClanPredicate("observer", 12)}} or {{ClanPredicate("observer", 8)}} or getregister(@observer, "{{NpcTribeRegister}}") == 12 or getregister(@observer, "{{NpcTribeRegister}}") == 8)
			return false
		end if
		if (@ArmageddonWorldfileMemorySameClan(@observer, @victim))
			@{{raw.FunctionName}}(@aggressor, @observer)
		end if
		return false
		""", ProgVariableTypes.Boolean, ("victim", ProgVariableTypes.Character), ("weapon", ProgVariableTypes.Item),
			("aggressor", ProgVariableTypes.Character), ("observer", ProgVariableTypes.Character));
		InstallNpcEventHook("Remember Tribal Violence", EventType.CharacterDamagedWitness, ordinary, eligible);
		var own = Prog("ArmageddonWorldfileRememberOwnTribalDamage",
			$"return @{ordinary.FunctionName}(@victim, @weapon, @aggressor, @victim)", ProgVariableTypes.Boolean,
			("victim", ProgVariableTypes.Character), ("weapon", ProgVariableTypes.Item), ("aggressor", ProgVariableTypes.Character));
		InstallNpcEventHook("Remember Own Tribal Violence", EventType.CharacterDamaged, own, eligible);
		var escape = Prog("ArmageddonWorldfileRememberEscape", """
		foreach (subject in @leftbehind)
			forget(@actor, @subject, "Hate")
			remember(@actor, @subject, "Fear")
		end foreach
		return false
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("origin", ProgVariableTypes.Location),
			("leftbehind", ProgVariableTypes.Character | ProgVariableTypes.Collection));
		InstallNpcEventHook("Remember Successful Escape", EventType.CharacterEscapedCombat, escape, eligible);
		InstallNpcSpellMemories(raw);
		Record("npc.memory", "NpcMemory", ordinary.Id, null, new
		{
			Lifecycle = "Session-only live instances; no expiry, cleared on either participant logout, death or removal.",
			Policy = "Source ordinary/raw hate distinction, tribal harm, Jakhal mount targeting, independent liking and fear; escape remembers visible characters left behind including bystanders. Native movement and combat admission remain authoritative. Hunt pursuit is bounded and uses native pathing."
		});
	}

	private void InstallNpcMemoryPursuit(ArmWorldNpc source, FutureProg admission)
	{
		var savedHunt = source.Skills.Where(x => x.Length >= 2 && x[0] >= 0 && x[0] < _source.Skills.Length &&
			_source.Skills[x[0]].Equals("hunt", StringComparison.OrdinalIgnoreCase)).ToArray();
		if (savedHunt.Length == 0) return;
		var hunt = savedHunt.Last()[1]; // Source tests for the saved skill entry, including zero proficiency.
		var target = Prog("ArmageddonWorldfileHatedPursuitTarget" + source.Vnum,
			$"return remembers(@actor, @target, \"Hate\") and not(remembers(@actor, @target, \"Fear\")) and not(remembers(@actor, @target, \"Like\")) and @{admission.FunctionName}(@actor, @target)",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("target", ProgVariableTypes.Character));
		var enabled = Prog("ArmageddonWorldfileHatedPursuitEnabled" + source.Vnum, $$"""
		if (@actor.possessednpc or haspausedaieffect(@actor) or not(@actor.isawake) or @actor.incombat)
			return false
		end if
		if (rememberedcharacters(@actor, "Fear").Any(subject, @subject.location == @actor.location and cansee(@actor, @subject)))
			return false
		end if
		return rememberedcharacters(@actor, "Hate").Any(subject, @{{target.FunctionName}}(@actor, @subject))
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var ai = CreateAi("Pursue Remembered Enemies Vnum" + source.Vnum, "TrackingAggressor", new XElement("Definition",
			new XElement("WillAttackProg", target.Id), new XElement("PathingEnabledProg", enabled.Id),
			new XElement("PursuitOnly", true), new XElement("MaximumRange", Math.Max(5, hunt / 10)),
			new XElement("EngageDelayDiceExpression", "0"), new XElement("EngageEmote", ""),
			new XElement("OpenDoors", false), new XElement("UseKeys", false), new XElement("UseDoorguards", false),
			new XElement("SmashLockedDoors", false)));
		AttachFamilyAi(source.Vnum, ai);
	}

	private void InstallNpcSpellMemories(FutureProg raw)
	{
		MagicSpell? Spell(string key) => OwnedBehaviourId(key, nameof(MagicSpell)) is { } id ? _db.MagicSpells.Single(x => x.Id == id) : null;
		var flame = Spell("arm.spell.flame_mantle");
		var shadow = OwnedBehaviourId("arm.support.plane.shadow", nameof(Plane));
		var rain = Prog("ArmageddonWorldfileRainOfFireHate", $$"""
		if (isadmin(@subject) or isincorporeal(@subject){{(shadow is { } plane ? $" or currentplaneid(@subject) == {plane}" : "")}}{{(flame is not null ? $" or contains(activespells(@subject), tomagicspell({flame.Id}))" : "")}})
			return false
		end if
		return @{{raw.FunctionName}}(@actor, @subject)
		""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("subject", ProgVariableTypes.Character));
		foreach (var key in new[] { "arm.spell.leaping_lightning", "arm.spell.immolate", "arm.spell.cinder_rain" })
		{
			var spell = Spell(key);
			if (spell is null) throw new InvalidOperationException("NPC memory requires installed spell " + key);
			var definition = XElement.Parse(spell.Definition);
			definition.Element("Effects")!.AddFirst(new XElement("Effect", new XAttribute("type", "executeprog"),
				new XElement("Prog", key.EndsWith("cinder_rain", StringComparison.Ordinal) ? rain.Id : raw.Id)));
			spell.Definition = definition.ToString();
		}
		if (Spell("arm.spell.ground_tremor") is { } tremor)
		{
			var definition = XElement.Parse(tremor.Definition);
			definition.Descendants("Effect").Single(x => (string?)x.Attribute("type") == "groundtremor").SetElementValue("RecipientProg", raw.Id);
			tremor.Definition = definition.ToString();
		}
		foreach (var key in new[] { "arm.spell.clay_sentinel", "arm.spell.air_guardian" })
		{
			var spell = Spell(key);
			if (spell is null) continue;
			var definition = XElement.Parse(spell.Definition);
			foreach (var effect in definition.Descendants("Effect").Where(x => (string?)x.Attribute("type") == "createnpc"))
			{
				var previous = _db.FutureProgs.SingleOrDefault(x => x.Id == ((long?)effect.Element("OnLoadProg") ?? 0));
				var args = previous?.FutureProgsParameters.Count switch { 1 => "@npc", 2 => "@npc, @caster", _ => "@npc, @caster, @spell" };
				var callback = Prog("ArmageddonWorldfileSummonLikesCaster" + spell.Id,
					(previous is null ? "" : $"@{previous.FunctionName}({args})\n") + "remember(@npc, @caster, \"Like\")",
					ProgVariableTypes.Void, ("npc", ProgVariableTypes.Character), ("caster", ProgVariableTypes.Character), ("spell", ProgVariableTypes.MagicSpell));
				effect.SetElementValue("OnLoadProg", callback.Id);
			}
			spell.Definition = definition.ToString();
		}
	}
}
