#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private string SourceNpcPredicate(string character, Func<ArmWorldNpc, bool> predicate)
	{
		var ids = _source.Npcs.Where(predicate).SelectMany(x =>
			_npcFamilyIds.TryGetValue(x.Vnum, out var family) ? family : _npcs.TryGetValue(x.Vnum, out var npc) ? [npc.Id] : Array.Empty<long>());
		var terms = ids.Select(x => $"@{character}.npctemplateid == {x}").ToArray();
		string Combine(int start, int count) => count switch
		{
			0 => "false",
			1 => terms[start],
			_ => $"({Combine(start, count / 2)} or {Combine(start + count / 2, count - count / 2)})"
		};
		return Combine(0, terms.Length);
	}

	private string SourceRacePredicate(string character, int index)
	{
		var npc = SourceNpcPredicate(character, x => x.Identity[6] == index);
		var name = _source.Races.SingleOrDefault(x => x.Index == index)?.Name;
		var native = _db.Races.Local.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		// An anatomical fallback parent is not a source racial identity.
		return native is null ? npc : $"({npc}) or (not(@{character}.npc) and samerace(@{character}.race, torace({native.Id})))";
	}

	private string ClanPredicate(string character, int sourceClan) => _clans.TryGetValue(sourceClan, out var clan)
		? $"isclanmember(@{character}, toclan({clan.Id}))" : "false";

	private void InstallGenericNpcAi()
	{
		EnsureNpcMemoryPolicy();
		EnsureMountRegisters();
		var merchants = _source.Shops.Select(x => x.Keeper).ToHashSet();
		var merchantProg = Prog("ArmageddonWorldfileIsMerchant", "return " + SourceNpcPredicate("subject", x => merchants.Contains(x.Vnum)),
			ProgVariableTypes.Boolean, ("subject", ProgVariableTypes.Character));
		var mountProg = Prog("ArmageddonWorldfileIsMount",
			$"return getregister(@subject, \"{MountTrainedRegister}\") == 1 or (" + SourceNpcPredicate("subject", x => (x.Flags[0] & (1 << 26)) != 0) + ")",
			ProgVariableTypes.Boolean, ("subject", ProgVariableTypes.Character));
		var standing = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionStanding.Instance.Id;
		var peek = _source.Skills.Select((name, index) => (name, index)).FirstOrDefault(x => x.name.Equals("peek", StringComparison.OrdinalIgnoreCase)).index;
		foreach (var source in _source.Npcs.Where(x => (x.Flags[0] & (1 << 5)) != 0 || (x.Flags[0] & (1 << 29)) == 0))
		{
			var usesMemory = (source.Flags[0] & (1 << 29)) == 0;
			var hate = usesMemory ? "remembers(@actor, @target, \"Hate\")" : "false";
			var body = new StringBuilder($$"""
			if (@actor == @target or isadmin(@target) or not(@actor.isawake) or @actor.incombat or positionid(@actor) != {{standing}})
				return false
			end if
			if (@{{merchantProg.FunctionName}}(@target) or isincorporeal(@actor) != isincorporeal(@target))
				return false
			end if
			""");
			body.AppendLine();
			if ((source.Flags[0] & (1 << 5)) == 0) body.AppendLine($"if (not({hate}))\nreturn false\nend if");
			var localOnly = (source.Flags[0] & 2) != 0 || merchants.Contains(source.Vnum) || source.Identity[6] is 14 or 30;
			if (localOnly) body.AppendLine("if (@actor.location != @target.location)\nreturn false\nend if");
			if (source.Identity[6] == 25)
			{
				var guild = _source.Guilds.SingleOrDefault(x => x.Index == source.Identity[3]);
				var sameGuild = $"({SourceNpcPredicate("target", x => x.Identity[3] == source.Identity[3])})" +
					(guild is null ? "" : $" or @target.class == \"{guild.Name.Replace("\"", "")}\"");
				body.AppendLine($"if (({sameGuild}) and not({hate}))\nreturn false\nend if");
			}
			if (source.Identity[6] == 20) body.AppendLine($"if ({SourceRacePredicate("target", 20)})\nreturn false\nend if");
			// Jakhal's explicit mount preference precedes the ordinary same-clan/race filters.
			if (source.Identity[6] == 30) body.AppendLine($"if (@{mountProg.FunctionName}(@target))\n" + (usesMemory ? "remember(@actor, @target, \"Hate\")\n" : "") + "return true\nend if");
			if (source.Identity[6] == 12) body.AppendLine($"if ({SourceRacePredicate("target", 12)})\nreturn false\nend if");
			if (source.Identity[6] == 37) body.AppendLine("if (not(@target.isawake) or not(@target.canmove))\nreturn false\nend if");
			body.AppendLine("if (@ArmageddonWorldfileMemorySameClan(@actor, @target))\nreturn false\nend if");
			body.AppendLine("if (not(isnull(@target.following)))\nif (@ArmageddonWorldfileMemorySameClan(@actor, @target.following))\nreturn false\nend if\nend if");
			if (_db.LegalAuthorities.Local.FirstOrDefault(x => x.Name == "Allanak Militia") is { } allanak)
				body.AppendLine($"if ({ClanPredicate("actor", 12)} and not(isnull(enforceablecrime(@target, tolegalauthority({allanak.Id})))) and @target.location.characters.Any(captor, issubduedby(@target, @captor)))\nreturn false\nend if");
			body.AppendLine($"if ({ClanPredicate("actor", 7)} and {ClanPredicate("target", 24999)} and not({hate}))\nreturn false\nend if");
			body.AppendLine($"if ({ClanPredicate("actor", 34)})\nreturn {ClanPredicate("target", 12)}\nend if");
			if ((source.Flags[0] & 4) != 0 && source.Skills.Any(x => x[0] == peek && x[1] > 0) && source.Identity[6] is 1 or 2 or 3 or 23 or 32 or 33 or 98)
				body.AppendLine($"return @target.isawake and ({hate} or @target.wornitems.Any(garment, @garment.basecost > 60 and cansee(@actor, @garment)) or @target.helditems.Any(held, @held.basecost > 60 and cansee(@actor, @held)) or @target.wieldeditems.Any(weapon, @weapon.basecost > 60 and cansee(@actor, @weapon)))");
			else body.AppendLine($"return {hate} or not(@{mountProg.FunctionName}(@target))");
			var targetProg = Prog("ArmageddonWorldfileAggressive" + source.Vnum, body.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("target", ProgVariableTypes.Character));
			var ai = CreateAi("Selective Aggression Vnum" + source.Vnum, "Aggressor", new XElement("Definition",
				new XElement("WillAttackProg", targetProg.Id), new XElement("AcquisitionRange", localOnly ? 0 : 1),
				new XElement("RespectDispositionMemory", usesMemory), new XElement("AggressiveClassification", (source.Flags[0] & (1 << 5)) != 0),
				new XElement("EngageDelayDiceExpression", "3000+1d18000"), new XElement("EngageEmote", "")));
			AttachGenericAggressor(source.Vnum, ai, targetProg);
			if (usesMemory && !localOnly) InstallNpcMemoryPursuit(source, targetProg);
		}

		var disabledZones = _source.Zones.Where(x => x.Fields[2] == 3).Where(x => _zones.ContainsKey(x.Index)).Select(x => _zones[x.Index].Id).ToArray();
		foreach (var stayZone in new[] { false, true })
		{
			var disabled = string.Join(" or ", disabledZones.Select(x => $"@origin.zone.id == {x}").Prepend("false"));
			var body = $$"""
			if (@actor.possessednpc or not(isnull(@actor.following)) or @actor.riders.Count(rider, true) > 0 or not(@actor.canmove) or positionid(@actor) != {{standing}} or not(cansee(@actor, @origin)))
				return false
			end if
			if ({{disabled}})
				return false
			end if
			if ({{(stayZone ? "@destination.zone != @origin.zone" : "false")}})
				return false
			end if
			return @origin.exits.Any(passage, @passage.destination == @destination and @passage.direction != "Up")
			""";
			var prog = Prog("ArmageddonWorldfileWander" + (stayZone ? "WithinZone" : "Unrestricted"), body, ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("destination", ProgVariableTypes.Location), ("origin", ProgVariableTypes.Location));
			var ai = CreateAi(stayZone ? "Wander Within Zone" : "Wander Through Accessible Rooms", "Wanderer", new XElement("Definition",
				new XElement("FutureProg", prog.Id), new XElement("WanderTimeDiceExpression", "1d121+59"),
				new XElement("TargetBody", 0), new XElement("TargetSpeed", 0), new XElement("OpenDoors", false), new XElement("EmoteText", "")));
			foreach (var npc in _source.Npcs.Where(x => (x.Flags[0] & 2) == 0 && ((x.Flags[0] & 64) != 0) == stayZone)) AttachFamilyAi(npc.Vnum, ai);
		}
		Adapt("generic NPC AI", 0, "Native Aggressor/Wanderer AI consumes source aggressive/sentinel/stay-zone flags. Target filters preserve clan, racial, mount and shopkeeper exclusions. Session-only native memories record future source-backed interactions; no historic entries are invented. Non-aggressive memory consumers retain non-aggressive classification. Hunt-trained non-sentinels use bounded native pursuit. Native detection, pacifism, combat settings and movement remain authoritative. Wander delays use 60–180 seconds; native valid-exit selection and ten-second aggression evaluation replace source scheduler details.");
		Flush();
	}

	private void AttachGenericAggressor(int vnum, ArtificialIntelligence ai, FutureProg targetProg)
	{
		var ids = _npcFamilyIds.TryGetValue(vnum, out var family) ? family : [_npcs[vnum].Id];
		foreach (var id in ids)
		{
			var template = _db.NpcTemplates.Local.Single(x => x.Id == id);
			var existing = template.NpctemplatesArtificalIntelligences.Where(x =>
				_db.ArtificialIntelligences.Local.Single(y => y.Id == x.AiId).Type == "Aggressor").ToArray();
			if (existing.Length == 0)
			{
				template.NpctemplatesArtificalIntelligences.Add(new() { AiId = ai.Id, NpcTemplateId = id, NpcTemplateRevisionNumber = template.RevisionNumber });
				continue;
			}
			// Preserve an explicit special's motives, but use a single native aggression controller.
			foreach (var old in existing)
			{
				var previous = _db.ArtificialIntelligences.Local.Single(x => x.Id == old.AiId);
				var xml = XElement.Parse(previous.Definition);
				var special = _db.FutureProgs.Local.Single(x => x.Id == (long)xml.Element("WillAttackProg")!);
				var arguments = special.FutureProgsParameters.Count switch { 0 => "", 1 => "@actor", _ => "@actor, @target" };
				var combined = Prog("ArmageddonWorldfileCombinedAggression" + id + "With" + old.AiId,
					$"return @{special.FunctionName}({arguments}) or @{targetProg.FunctionName}(@actor, @target)", ProgVariableTypes.Boolean,
					("actor", ProgVariableTypes.Character), ("target", ProgVariableTypes.Character));
				xml.SetElementValue("WillAttackProg", combined.Id);
				xml.SetElementValue("AcquisitionRange", XElement.Parse(ai.Definition).Element("AcquisitionRange")!.Value);
				xml.SetElementValue("RespectDispositionMemory", XElement.Parse(ai.Definition).Element("RespectDispositionMemory")!.Value);
				var merged = CreateAi("Combined Selective Aggression " + id, "Aggressor", xml);
				template.NpctemplatesArtificalIntelligences.Remove(old);
				template.NpctemplatesArtificalIntelligences.Add(new() { AiId = merged.Id, NpcTemplateId = id, NpcTemplateRevisionNumber = template.RevisionNumber });
			}
		}
	}
}
