#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	// Imported mounts have one rider. Native movement is initiated on the mount, while source
	// command specials checked its rider's clan and inflicted their refusal damage on that rider.
	private const string SourceTraveller = "var subject = @traveller\nif (@traveller.riders.Count(rider, true) == 1)\nsubject = getindex(@traveller.riders, 0)\nend if\n";
	private ArtificialIntelligence CreateAi(string name, string type, XElement definition)
	{
		var ai = new ArtificialIntelligence { Id = Next<ArtificialIntelligence>(), Name = name, Type = type, Definition = definition.ToString() };
		_db.ArtificialIntelligences.Add(ai);
		Record("ai." + name, nameof(ArtificialIntelligence), ai.Id, null, new { Type = type, Definition = definition.ToString() });
		_counts["NPC AI definitions"] = _counts.GetValueOrDefault("NPC AI definitions") + 1;
		return ai;
	}

	private void AttachFamilyAi(int vnum, ArtificialIntelligence ai)
	{
		var templates = _npcFamilyIds.TryGetValue(vnum, out var family) ? family : [_npcs[vnum].Id];
		foreach (var id in templates)
		{
			var template = _db.NpcTemplates.Local.Single(x => x.Id == id);
			if (template.NpctemplatesArtificalIntelligences.Any(x => x.AiId == ai.Id)) continue;
			template.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
			{ AiId = ai.Id, NpcTemplateId = template.Id, NpcTemplateRevisionNumber = template.RevisionNumber });
		}
	}

	private void InstallNpcBehaviours()
	{
		EnsureMountRegisters();
		var permit = Prog("ArmageddonWorldfilePermitMount", MountPermitProg, ProgVariableTypes.Boolean,
			("rider", ProgVariableTypes.Character), ("mount", ProgVariableTypes.Character));
		var mountAi = CreateAi("Worldfile Mount", "Mount", new XElement("Definition",
			new XElement("CompanionRequiresColocation", true), new XElement("CompanionFollowsHandler", true), new XElement("PermitRiderProg", permit.Id), new XElement("PermitControlProg", permit.Id),
			new XElement("WhyCannotPermitRiderProg", 0), new XElement("MaximumNumberOfRiders", 1),
			new XElement("MountNonConsensualMountDifficulty", (int)Difficulty.Impossible),
			new XElement("MountControlDifficulty", (int)Difficulty.Trivial), new XElement("MountResistBuckDifficulty", (int)Difficulty.Normal),
			new XElement("RawMountEmote", "$1 $1|mount|mounts $0."), new XElement("RawDismountEmote", "$1 $1|dismount|dismounts $0."),
			new XElement("RawBuckEmote", "$0 try|tries to buck $1!"), new XElement("RawControlDeniedEmote", "$0 refuse|refuses to obey $1's order.")));
		foreach (var source in _source.Npcs.Where(x => (unchecked((uint)x.Flags[0]) & (1u << 26)) != 0))
			AttachFamilyAi(source.Vnum, mountAi);
		InstallArmageddonMountPolicies(mountAi, mountAi.Id);
		Adapt("mounts", 0, "Native MountAI applies source-backed ownership and approved-rider access; character identity registers preserve owner and riders. Source-eligible wild races can be tamed by standing Rangers with Riding over 60 on a Normal native Riding check. Native size, riding and equipment safety remain authoritative. Nearby companions reuse the bodyguard logout lifecycle; recall whistle 64129 has no placement in this package.");

		var mantisRaces = _races.Values.Where(x => x.Name.StartsWith("mantis", StringComparison.OrdinalIgnoreCase))
			.SelectMany(x => new[] { x.Id, x.ParentRaceId ?? x.Id }).Distinct().ToArray();
		var predicates = mantisRaces.Select(id => $"@target.race == torace({id})").ToList();
		if (_clans.TryGetValue(12, out var militia)) predicates.Add($"isclanmember(@target, toclan({militia.Id}))");
		var targetProg = Prog("ArmageddonWorldfileOutriderTargets", "return " + (predicates.Count == 0 ? "false" : string.Join(" or ", predicates)),
			ProgVariableTypes.Boolean, ("outrider", ProgVariableTypes.Character), ("target", ProgVariableTypes.Character));
		var outrider = CreateAi("Worldfile Mercenary Outrider", "Aggressor", new XElement("Definition",
			new XElement("WillAttackProg", targetProg.Id), new XElement("EngageDelayDiceExpression", "1000"), new XElement("EngageEmote", "")));
		foreach (var source in _source.Npcs.Where(x => HasSpecial(x, "outrider"))) AttachFamilyAi(source.Vnum, outrider);
		Adapt("outrider", 60100, "Native AggressorAI preserves visible Allanak militia/Mantis target selection; ordinary native engagement replaces the source backstab opener by explicit design decision.");
		if (_clans.TryGetValue(7, out var sentryClan))
		{
			var sentry = CreateAi("Worldfile Mantis Sentry", "ClanSentry", new XElement("Definition",
				new XElement("Clan", sentryClan.Id), new XElement("HideAfterAlert", true),
				new XElement("AlertFormat", "{0} just passed, heading {1}."),
				new XElement("Directions", new XElement("Direction", (int)CardinalDirection.East), new XElement("Direction", (int)CardinalDirection.West))));
			foreach (var source in _source.Npcs.Where(x => HasSpecial(x, "cs_sentry"))) AttachFamilyAi(source.Vnum, sentry);
		}
		InstallGuardedPassage("gyp_gate_guard", 6592, [24, 14]);
		InstallGuardedPassage("gyp_oasis_guard", 13999, [24]);
		InstallRestrictedRoomPassages();
		InstallTalZenHazard();
		Flush();
	}

	private void InstallTalZenHazard()
	{
		var sources = _source.Rooms.Where(x => x.Programs.Contains("C south tal_zen_tunnel")).ToArray();
		if (sources.Length == 0) return;
		var attributes = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		var wisdom = new[] { "Wisdom", "Willpower", "Mind", "Intelligence" }.Select(name =>
			attributes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault(x => x is not null)
			?? throw new InvalidOperationException("Tal-Zen requires a native mental attribute for source Wisdom damage.");
		var allowed = _clans.TryGetValue(12, out var militia) ? $"isclanmember(@subject, toclan({militia.Id})) or " : "";
		var access = Prog("ArmageddonWorldfileTalZenAccess", SourceTraveller + $"return {allowed}isincorporeal(@subject)", ProgVariableTypes.Boolean,
			("traveller", ProgVariableTypes.Character), ("origin", ProgVariableTypes.Location), ("destination", ProgVariableTypes.Location));
		var hazard = Prog("ArmageddonWorldfileTalZenRefusal",
			SourceTraveller + $"send(@subject, \"As you enter the tunnel of Tal-Zen, you feel something penetrate your soul.\")\n" +
			"sendloc(@origin, \"A voice booms: Begone Fool!\")\n" +
			$"wound(@subject, \"eldritch\", random(3, 8) * floor(gettrait(@subject, totrait({wisdom.Id})) / 2))\nreturn true",
			ProgVariableTypes.Boolean, ("traveller", ProgVariableTypes.Character),
			("origin", ProgVariableTypes.Location), ("destination", ProgVariableTypes.Location));
		foreach (var source in sources)
		{
			var definition = ExitPolicyXml(CardinalDirection.South, "You are psionically refused entrance.", false);
			definition.SetElementValue("AccessProg", access.Id); definition.SetElementValue("DeniedAttemptProg", hazard.Id);
			AddRoomEffect(_rooms[source.Vnum], "RoomExitPolicy", definition);
			Record($"room-special.{source.Vnum}.tal_zen_tunnel", nameof(Room), _rooms[source.Vnum].Id, null,
				new { Program = "C south tal_zen_tunnel", MentalAttribute = wisdom.Id,
					Disposition = "Approved attempted-passage hazard. Militia and native incorporeal travellers bypass. Source random integer 3..8 times integer Wisdom/2 becomes native eldritch wounds with damage/pain/stun; source maximum HP remains native health configuration. Feasibility and pathfinding never inflict damage." });
			_counts["active room special associations"] = _counts.GetValueOrDefault("active room special associations") + 1;
		}
	}

	private void InstallRestrictedRoomPassages()
	{
		var halflings = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0]
			.Equals("Halfling", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		foreach (var source in _source.Rooms)
		foreach (var program in source.Programs)
		{
			string predicate; string denial; CardinalDirection direction;
			if (program == "C up lo_pyramid")
			{
				predicate = halflings.Length == 0 ? "false" : string.Join(" or ", halflings.Select(id => $"@subject.race.id == {id}"));
				denial = "Your weight prevents you from climbing up."; direction = CardinalDirection.Up;
			}
			else if (program == "C down kurac_tunnel")
			{
				predicate = _clans.TryGetValue(4, out var kurac) ? $"isclanmember(@subject, toclan({kurac.Id}))" : "false";
				denial = "You are not permitted to enter the tunnel."; direction = CardinalDirection.Down;
			}
			else continue;
			var special = program.Split(' ').Last();
			var prog = Prog("ArmageddonWorldfileRoomAccess" + source.Vnum + special, SourceTraveller + "return " + predicate,
				ProgVariableTypes.Boolean, ("traveller", ProgVariableTypes.Character),
				("origin", ProgVariableTypes.Location), ("destination", ProgVariableTypes.Location));
			var definition = ExitPolicyXml(direction, denial, false); definition.SetElementValue("AccessProg", prog.Id);
			AddRoomEffect(_rooms[source.Vnum], "RoomExitPolicy", definition);
			_counts["active room special associations"] = _counts.GetValueOrDefault("active room special associations") + 1;
			Record($"room-special.{source.Vnum}.{special}", nameof(Room), _rooms[source.Vnum].Id, null,
				new { Program = program, Disposition = "Native directional race/clan admission prog; source destination remains unchanged." });
		}
	}

	private void InstallGuardedPassage(string special, int roomVnum, int[] permittedClans)
	{
		if (!_rooms.TryGetValue(roomVnum, out var room)) return;
		var sources = _source.Npcs.Where(x => HasSpecial(x, special)).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var ids = sources.SelectMany(v => _npcFamilyIds.TryGetValue(v, out var family) ? family : [_npcs[v].Id]).Distinct();
		var membership = permittedClans.Where(_clans.ContainsKey).Select(id => $"isclanmember(@subject, toclan({_clans[id].Id}))");
		var allowed = string.Join(" or ", membership);
		var guards = string.Join(" or ", ids.Select(id => $"@guard.npctemplateid == {id}"));
		var text = SourceTraveller + (allowed.Length > 0 ? $"if ({allowed})\n\treturn true\nend if\n" : "") +
			$"return not(@origin.characters.Any(guard, ({guards}) and cansee(@guard, @subject)))";
		var prog = Prog("ArmageddonWorldfileGuardAccess" + special, text, ProgVariableTypes.Boolean,
			("traveller", ProgVariableTypes.Character), ("origin", ProgVariableTypes.Location), ("destination", ProgVariableTypes.Location));
		var definition = ExitPolicyXml(CardinalDirection.North, "The guards block your access to the gate.", false);
		definition.SetElementValue("AccessProg", prog.Id);
		definition.SetElementValue("ControlsOpening", true);
		AddRoomEffect(room, "RoomExitPolicy", definition);
		Record("guard-access." + special, nameof(Room), room.Id, null,
			new { Special = special, SourceRoom = roomVnum, PermittedClans = permittedClans,
				Disposition = "Native passage/opening policy applies only while a matching guard can see the traveller. Clan exemptions preserved. The separate guard progs implement attached ask/follow/bodyguard, home door/scan pulses and accepted Land gathering retaliation. Disabled/inconsistent off-home branches retain provenance; the broken personal-name vendetta is omitted by approval." });
	}
}
