#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Events;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string GuardRelationshipCleanup = """
		if (not(isnull(@guard.following)))
			if (@guard.following.location != @guard.location)
				force @guard "follow self"
			end if
		end if
		if (not(isnull(@guard.bodyguarding)))
			if (@guard.bodyguarding.location != @guard.location)
				force @guard "bodyguard off"
			end if
		else
			if (@guard.bodyguardingid != 0)
				force @guard "bodyguard off"
			end if
		end if
		""";

	private void InstallGuardSpecials()
	{
		InstallGuardCommands();
		InstallGuardPulses();
		InstallNobleGate();
	}

	private void InstallGuardCommands()
	{
		foreach (var special in new[] { "kohmar_body_guards", "gyp_gate_guard", "gyp_oasis_guard" })
		foreach (var group in _source.Npcs.Where(x => x.Programs.Contains("C ask " + special)).GroupBy(x => x.Identity[4]))
		{
			if (!_clans.TryGetValue(group.Key, out var clan))
				throw new InvalidOperationException($"Guard special {special} requires source clan {group.Key}.");
			var eligibility = special == "kohmar_body_guards" ? "@ArmageddonWorldfileClan28Leader(@actor)"
				: $"isclanmember(@actor, toclan({clan.Id}))";
			if (special == "gyp_oasis_guard") eligibility += " or lowercase(@actor.name) == \"marit\"";
			var prog = NpcServiceProg($"ArmageddonWorldfileGuardAsk{special}{group.Key}", $$"""
				var guard = tocharacter(@npc)
				if (@args.Count(word, true) < 2 or not({{eligibility}}))
					return false
				end if
				if (targetcharacter(@actor, getindex(@args, 0)) != @guard)
					return false
				end if
				var action = lowercase(getindex(@args, 1))
				if (@action == "help")
					sendfixed(@actor, "The guard can do the following for you:\nask guard follow target\nask guard guard target\nFor example: ask guard follow me. Use follow self to stop following, or guard self to stop guarding.")
					return true
				end if
				if (@action != "follow" and @action != "guard")
					force @guard "emote shake|shakes $0's head."
					return true
				end if
				if (@args.Count(word, true) < 3)
					if (@action == "follow")
						force @guard "say Follow who?"
					else
						force @guard "say Guard who?"
					end if
					return true
				end if
				var word = lowercase(getindex(@args, 2))
				if (@word == "self")
					if (@action == "follow")
						force @guard "follow self"
					else
						force @guard "bodyguard off"
					end if
					return true
				end if
				var target = @actor
				if (@word != "me")
					target = targetcharacter(@guard, @word)
				end if
				if (isnull(@target) or not(cansee(@guard, @target)))
					force @guard "say I don't see them here."
					return true
				end if
				if (@target == @guard)
					if (@action == "follow")
						force @guard "follow self"
					else
						force @guard "bodyguard off"
					end if
				else
					if (@action == "follow")
						force @guard ("follow " + bestkeyword(@guard, @target))
					else
						force @guard ("bodyguard " + bestkeyword(@guard, @target))
					end if
				end if
				return true
				""");
			InstallNpcInputHook($"Guard Ask {special} {group.Key}", "ask", prog, group.Select(x => x.Vnum).ToArray());
			Record($"npc-guard-ask.{special}.{group.Key}", nameof(FutureProg), prog.Id, null,
				new { Sources = group.Select(x => x.Vnum), SourceClan = group.Key, Disposition = "Only source ASK attachments. Conditional addressed commands preserve eligibility, me/self targets and native follow/bodyguard actions. Missing source command arguments are safely refused instead of copying uninitialised C strings." });
		}
	}

	private void InstallGuardPulses()
	{
		foreach (var special in new[] { "kohmar_body_guards", "gyp_gate_guard", "gyp_oasis_guard" })
		{
			var sources = _source.Npcs.Where(x => x.Programs.Contains("C pulse " + special)).Select(x => x.Vnum).ToArray();
			if (sources.Length == 0) continue;
			var kohmar = special == "kohmar_body_guards";
			var home = kohmar ? 20040 : special == "gyp_gate_guard" ? 6592 : 13999;
			var body = new StringBuilder("var guard = @npc\n" + GuardRelationshipCleanup + "\nif (not(@guard.canmove) or @guard.incombat)\nreturn false\nend if\n");
			if (kohmar)
				body.AppendLine($"if (@guard.location != tolocation({_rooms[home].Id}) and isnull(@guard.following) and isnull(@guard.bodyguarding) and not(@guard.isguarding))\nvar path = getpath(@guard, tolocation({_rooms[home].Id}))\nif (@path.Count(step, true) > 0)\nforce @guard getindex(@path, 0)\nend if\nreturn true\nend if");
			else
			{
				body.AppendLine($"if (@guard.location != tolocation({_rooms[home].Id}))\nreturn false\nend if");
				body.AppendLine(special == "gyp_gate_guard" ? "if (random(1, 6) == 1)\nforce @guard \"close gate\"\nend if" : "force @guard \"close flap\"");
			}
			body.AppendLine("if (random(1, 30) == 1)\nforce @guard \"scan\"\nend if\nreturn true");
			var pulse = Prog("ArmageddonWorldfileGuardPulse" + special, body.ToString(), ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
			InstallNpcTickHook("Guard Pulse " + special, pulse, sources);
			Record("npc-guard-pulse." + special, nameof(FutureProg), pulse.Id, null,
				new { Sources = sources, Home = home, Disposition = "Native ten-second pulse preserves conditional closing/scanning and clears separated follow/bodyguard targets. Kohmar return uses one native movement step per available pulse, allowing movement completion. No undefined wanted-name targets or disabled oasis homing are invented." });
		}
		foreach (var special in new[] { "gyp_gate_guard", "gyp_oasis_guard" })
		{
			var sources = _source.Npcs.Where(x => x.Programs.Contains("C gather " + special)).Select(x => x.Vnum).ToArray();
			if (sources.Length == 0) continue;
			var home = special == "gyp_gate_guard" ? 6592 : 13999;
			var gather = Prog("ArmageddonWorldfileGuardGather" + special, $"""
				var guard = tocharacter(@witness)
				if (@guard.location == tolocation({_rooms[home].Id}) and @actor.location == @location)
					force @guard "shout Defiler!"
					force @guard "shout Defiler!"
					force @guard ("kill " + bestkeyword(@guard, @actor))
				end if
				return false
				""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("witness", ProgVariableTypes.Perceivable),
				("location", ProgVariableTypes.Location), ("capability", ProgVariableTypes.MagicCapability),
				("method", ProgVariableTypes.Text), ("amount", ProgVariableTypes.Number), ("operation", ProgVariableTypes.Text));
			InstallNpcEventHook("Guard Land Gathering " + special, EventType.CharacterBeginsLandMagicGatheringWitness, gather, sources);
			Record("npc-guard-land-gathering." + special, nameof(FutureProg), gather.Id, null,
				new { Sources = sources, Home = home, Disposition = "Source gather land retaliation uses the native accepted-Land-gathering witness event. Two shouts then ordinary native engagement; results do not consume the gathering command. Foraging, previews, invalid starts and non-Land methods never trigger it." });
		}
	}

	private void AppendNpcOnLoadCode(int vnum, string code)
	{
		var definition = XElement.Parse(_npcs[vnum].Definition);
		var prog = _progs.Single(x => x.Id == long.Parse(definition.Element("OnLoadProg")!.Value));
		var end = prog.FunctionText.LastIndexOf("return true", StringComparison.OrdinalIgnoreCase);
		if (end < 0) throw new InvalidOperationException($"NPC {vnum} has no terminal on-load return.");
		prog.FunctionText = prog.FunctionText.Insert(end, code + "\n");
	}

	private void InstallNobleGate()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C south noble_gate")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		string Member(int clan) => _clans.TryGetValue(clan, out var imported)
			? $"isclanmember(@subject, toclan({imported.Id}))"
			: "false";
		foreach (var missing in new[] { 58, 39, 12, 17, 22, 23, 36 }.Where(x => !_clans.ContainsKey(x)))
			Adapt("noble gate clan", missing, "The executable gate references a clan absent from the source clan file. This membership branch is false; no clan or members are invented.");
		var unconditional = string.Join(" or ", new[] { 58, 39, 12, 17, 22, 23, 36 }.Select(Member));
		var allowed = Prog("ArmageddonWorldfileNobleGatePermission", SourceTraveller + $"""
			if (not(@guard.isawake) or @guard.incombat or not(cansee(@guard, @subject)))
				return false
			end if
			if (@exit.direction != "South" or @exit.origin != tolocation({_rooms[50881].Id}))
				return false
			end if
			return {unconditional} or (@subject.npctemplateid == 0 and (@ArmageddonWorldfileClan41Leader(@subject) or isclanmember(@subject, toclan({_clans[26].Id}), torank({_ranks[(26, 2)].Id}))))
			""", ProgVariableTypes.Boolean, ("guard", ProgVariableTypes.Character), ("traveller", ProgVariableTypes.Character), ("exit", ProgVariableTypes.Exit));
		var opening = new StringBuilder(SourceTraveller + $"if (not(@{allowed.FunctionName}(@guard, @traveller, @exit)))\nreturn\nend if\n");
		var templarRole = OwnedBehaviourId("arm.class.southern_templar", nameof(ChargenRole));
		if (templarRole is { } role)
			opening.AppendLine($"if ({Member(12)} and contains(@subject.roles, torole({role})))\nforce @guard \"emote bow|bows deeply.\"\nforce @guard \"say Hail the Servants of the Allmighty Dragon!\"\nforce @guard \"say Hail Lord Templar!\"\nend if");
		opening.AppendLine($"if ({Member(58)} or {Member(39)})\nforce @guard \"emote nod|nods curtly.\"\nelse");
		opening.AppendLine("var title = \"Lord\"\nif (@subject.gender == togender(\"female\"))\ntitle = \"Lady\"\nend if");
		foreach (var (clan, name) in new[] { (17, "Tor"), (22, "Fale"), (23, "Borsail"), (36, "Oash") })
			opening.AppendLine($"if ({Member(clan)})\nforce @guard \"emote bow|bows deeply.\"\nif (@subject.npctemplateid == 0)\nforce @guard (\"say Hail \" + @title + \" \" + @subject.name + \", of House {name}!\")\nelse\nforce @guard \"say Hail the Lords of House {name}!\"\nend if\nend if");
		opening.AppendLine($"if ({Member(41)})\nforce @guard \"say You may pass, Servant of House Oash.\"\nend if\nif ({Member(26)})\nforce @guard \"say You may pass, Servant of House Fale.\"\nend if\nend if\nforce @guard (\"open \" + @exit.keyword)");
		var parameters = new[] { ("guard", ProgVariableTypes.Character), ("traveller", ProgVariableTypes.Character), ("exit", ProgVariableTypes.Exit) };
		var open = Prog("ArmageddonWorldfileNobleGateOpen", opening.ToString(), ProgVariableTypes.Void, parameters);
		var close = Prog("ArmageddonWorldfileNobleGateClose", "force @guard (\"close \" + @exit.keyword)", ProgVariableTypes.Void, parameters);
		var refuse = Prog("ArmageddonWorldfileNobleGateRefuse", $"if (@exit.origin == tolocation({_rooms[50881].Id}) and @exit.direction == \"South\" and cansee(@guard, @traveller))\nif (not(@{allowed.FunctionName}(@guard, @traveller, @exit)))\nforce @guard \"emote level|levels $0's weapons and growl|growls.\"\nforce @guard \"say You are not allowed to pass.\"\nend if\nend if", ProgVariableTypes.Void, parameters);
		var delay = Prog("ArmageddonWorldfileNobleGateDelay", "return 100", ProgVariableTypes.Number, parameters);
		var closeDelay = Prog("ArmageddonWorldfileNobleGateCloseDelay", "return 10000", ProgVariableTypes.Number, parameters);
		var ai = CreateAi("Worldfile Noble Gate", "Doorguard", new XElement("Definition",
			new XElement("WillOpenDoorForProg", allowed.Id), new XElement("OpenDoorActionProg", open.Id),
			new XElement("CloseDoorActionProg", close.Id), new XElement("WontOpenDoorForActionProg", refuse.Id),
			new XElement("OnWitnessDoorStopProg", refuse.Id), new XElement("BaseDelayProg", delay.Id),
			new XElement("OpenCloseDelayProg", closeDelay.Id), new XElement("RespectGameRulesForOpeningDoors", true),
			new XElement("OwnSideOnly", true)));
		foreach (var vnum in sources)
		{
			AttachFamilyAi(vnum, ai);
			AppendNpcOnLoadCode(vnum, "force @npc \"doorguard\"");
		}
		var guardIds = sources.SelectMany(vnum => _npcFamilyIds.TryGetValue(vnum, out var family) ? family : [_npcs[vnum].Id]).Distinct();
		var matchingGuard = string.Join(" or ", guardIds.Select(id => $"@guard.npctemplateid == {id}"));
		var admission = Prog("ArmageddonWorldfileNobleGateAdmission", SourceTraveller + $"""
			var guarded = @origin.characters.Any(guard, ({matchingGuard}) and cansee(@guard, @subject))
			if (not(@guarded))
				return true
			end if
			return {unconditional.Replace("@traveller", "@subject", StringComparison.Ordinal)} or (@subject.npctemplateid == 0 and (@ArmageddonWorldfileClan41Leader(@subject) or isclanmember(@subject, toclan({_clans[26].Id}), torank({_ranks[(26, 2)].Id}))))
			""", ProgVariableTypes.Boolean, ("traveller", ProgVariableTypes.Character), ("origin", ProgVariableTypes.Location), ("destination", ProgVariableTypes.Location));
		var passage = ExitPolicyXml(MudSharp.Construction.CardinalDirection.South, "The soldier levels their weapons and refuses you passage.", false);
		passage.SetElementValue("AccessProg", admission.Id);
		AddRoomEffect(_rooms[50881], "RoomExitPolicy", passage);
		Record("npc-special.noble-gate", nameof(ArtificialIntelligence), ai.Id, null,
			new { Sources = sources, Disposition = "Native DoorguardAI replaces synchronous forced movement. Only the authored south gate is served. Source effective unrestricted clan 39 branch, noble/militia clans, player-only Oash Leader appointment and Fale rank 2 admission retained. Native door rules and transit-aware closing apply." });
	}
}
