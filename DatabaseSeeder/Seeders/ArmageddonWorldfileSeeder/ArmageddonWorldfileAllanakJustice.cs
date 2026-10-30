#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Law;
using Law = MudSharp.Models.Law;
using LegalAuthority = MudSharp.Models.LegalAuthority;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static string SourceDirectionInput(string direction)
	{
		var cardinal = Enum.Parse<CardinalDirection>(direction, true);
		var aliases = Constants.CardinalDirectionStringToDirection.Where(x => x.Value == cardinal)
			.Select(x => $"lowercase(firstword(@input)) == \"{x.Key}\"");
		return "lowercase(commandname(@actor, @input)) == \"move\" and (" + string.Join(" or ", aliases) + ")";
	}

	private void InstallAllanakJustice(ArtificialIntelligence enforcer)
	{
		var guards = _source.Npcs.Where(x => HasSpecial(x, "allanak_soldier") ||
			x.Programs.Contains("C order allanak_soldier_stop")).Select(x => x.Vnum).ToArray();
		var jailers = _source.Npcs.Where(x => x.Programs.Contains("C release allanak_jailer")).Select(x => x.Vnum).ToArray();
		var participants = guards.Concat(jailers).ToHashSet();
		var roomZones = _source.Rooms.ToDictionary(x => x.Vnum, x => x.Zone);
		var placements = _source.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 &&
			participants.Contains(x.Arguments[1]) && _rooms.ContainsKey(x.Arguments[3]) && !_interiorTemplates.Contains(x.Arguments[3]))
			.Select(x => (Room: x.Arguments[3], Zone: roomZones.GetValueOrDefault(x.Arguments[3], x.Zone)))
			.Where(x => x.Zone is 50 or 66).Distinct().ToArray();
		if (placements.Length == 0) return;
		var zones = placements.Select(x => x.Zone).Distinct().Order().ToArray();
		var posts = placements.Select(x => x.Room).Distinct().Order().ToArray();
		var primaryZone = zones.Contains(50) ? 50 : 66;
		var anchor = jailers.Length > 0 && posts.Contains(50535) ? 50535 : posts[0];
		var authority = InstallSettlementJustice("allanak", primaryZone, "Allanak Militia", guards, posts, enforcer, zones, anchor);
		var legalClass = _db.LegalClasses.Local.Single(x => x.LegalAuthorityId == authority.Id);
		if (jailers.Length > 0)
		{
			var ids = jailers.SelectMany(v => _npcFamilyIds.GetValueOrDefault(v, [_npcs[v].Id])).Distinct().ToArray();
			var eligible = Prog("ArmageddonWorldfileAllanakJailerRole",
				"return (" + string.Join(" or ", ids.Select(id => $"@character.npctemplateid == {id}")) + ") and (" +
				string.Join(" or ", zones.Select(z => $"@character.location.zone.id == {_zones[z].Id}")) + ")",
				ProgVariableTypes.Boolean, ("character", ProgVariableTypes.Character));
			var role = new EnforcementAuthority { Id = Next<EnforcementAuthority>(), LegalAuthorityId = authority.Id,
				Name = "Jailer", Priority = 0, FilterProgId = eligible.Id, CanAccuse = false, CanConvict = false, CanForgive = false };
			role.EnforcementAuthoritiesArrestableLegalClasses.Add(new EnforcementAuthoritiesArrestableLegalClasses
				{ EnforcementAuthorityId = role.Id, LegalClassId = legalClass.Id });
			_db.EnforcementAuthorities.Add(role);
			InstallJailerHandover(authority, jailers);
		}
		InstallSpiceCheckpoint(authority, legalClass);
		Record("justice.allanak.coverage", nameof(LegalAuthority), authority.Id, null, new
		{
			Zones = zones, CrimeCreationRooms = posts, CentralCustodyAnchor = anchor, Guards = guards, Jailers = jailers,
			Disposition = "One Allanaki authority for city zone 50 and its explicitly Allanaki mine outpost in 66. New automatic offences apply only at retained source guard/jailer reset posts. Player houses, sewers, above-city and other zones are not inferred into jurisdiction. Existing native pursuit and custody apply; the central facility uses the source jail as its public anchor when present. Jailer reception grants no patrol AI or autonomous arrest duty."
		});
	}

	private void InstallSpiceCheckpoint(LegalAuthority authority, LegalClass legalClass)
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C north spice_guard") ||
			x.Programs.Contains("C south spice_guard") || x.Programs.Contains("C east spice_guard")).ToArray();
		if (sources.Length == 0) return;
		var locations = _source.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 &&
			sources.Any(n => n.Vnum == x.Arguments[1]) && _rooms.ContainsKey(x.Arguments[3]))
			.Select(x => x.Arguments[3]).Distinct().Order().ToArray();
		var area = Prog("ArmageddonWorldfileSpiceLawArea", "return " + (locations.Length == 0 ? "false" :
			string.Join(" or ", locations.Select(v => $"@criminal.location.id == {_rooms[v].Id}"))), ProgVariableTypes.Boolean,
			("criminal", ProgVariableTypes.Character), ("victim", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("lawId", ProgVariableTypes.Number), ("crimeName", ProgVariableTypes.Text));
		var law = new Law { Id = Next<Law>(), LegalAuthorityId = authority.Id, Name = "Spice Smuggling",
			CrimeType = (int)CrimeTypes.TrafficingContraband, CanBeAppliedAutomatically = false, DoNotAutomaticallyApplyRepeats = true,
			ActivePeriod = SourceCrimeMaximumRealSeconds(CrimeTypes.TrafficingContraband) * SourceLawClockRate(),
			CanBeArrested = true, CanBeOfferedBail = false, EnforcementStrategy = EnforcementStrategy.ArrestAndDetain.ToString(),
			EnforcementPriority = 10, LawAppliesProgId = area.Id,
			PunishmentStrategy = "<Strategy type=\"fine\"><DefaultFineAmount>10</DefaultFineAmount><MaximumFineAmount>10</MaximumFineAmount></Strategy>" };
		law.LawsOffenderClasses.Add(new LawsOffenderClasses { LawId = law.Id, LegalClassId = legalClass.Id });
		_db.Laws.Add(law);

		var powers = _effectiveItems.Where(x => x.Value.Source.ItemType == 25 && x.Value.Values[2] != 1 && x.Value.Values[0] != 0)
			.GroupBy(x => x.Value.Values[0]).OrderBy(x => x.Key).ToArray();
		var powerCode = new StringBuilder("if (isnull(@item))\nreturn 0\nend if\n");
		foreach (var group in powers)
		{
			powerCode.AppendLine("if (" + string.Join(" or ", group.Select(x => $"@item.proto == {x.Key}")) + ")");
			if (_db.VariableDefinitions.Local.Any(x => x.Property == SpiceInitialisedRegister))
				powerCode.AppendLine($"if (getregister(@item, \"{SpiceInitialisedRegister}\") == 1)\nreturn max(0, getregister(@item, \"{SpiceUnitsRegister}\"))\nend if");
			powerCode.AppendLine($"return {group.Key}\nend if");
		}
		powerCode.AppendLine("return 0");
		var power = Prog("ArmageddonWorldfileSpicePower", powerCode.ToString(), ProgVariableTypes.Number, ("item", ProgVariableTypes.Item));
		var search = Prog("ArmageddonWorldfileSpiceSearch", """
			var total = 0
			foreach (item in @items)
				if (random(1, 100) <= @chance and cansee(@guard, @item))
					total = @total + @ArmageddonWorldfileSpicePower(@item)
				end if
				if (@item.iscontainer and not(@item.locked))
					total = @total + @ArmageddonWorldfileSpiceSearch(@guard, @item.contents, floor(@chance * 2 / 3))
				end if
			end foreach
			return @total
			""", ProgVariableTypes.Number, ("guard", ProgVariableTypes.Character),
			("items", ProgVariableTypes.Collection | ProgVariableTypes.Item), ("chance", ProgVariableTypes.Number));
		var exemptions = string.Join(" or ", new[] { 12, 17, 22, 23, 36 }.Where(_clans.ContainsKey)
			.Select(c => $"isclanmember(@actor, toclan({_clans[c].Id}))"));
		var jurisdiction = string.Join(" or ", authority.LegalAuthoritiesZones.Select(x => $"@guard.location.zone.id == {x.ZoneId}"));
		foreach (var direction in new[] { "north", "south", "east" })
		{
			var attached = sources.Where(x => x.Programs.Contains($"C {direction} spice_guard")).Select(x => x.Vnum).ToArray();
			if (attached.Length == 0) continue;
			var checkpoint = NpcServiceProg("ArmageddonWorldfileSpiceCheckpoint" + direction, $$"""
				if (not({{SourceDirectionInput(direction)}}))
					return false
				end if
				var guard = tocharacter(@npc)
				if (isnull(@guard.location) or not({{jurisdiction}}))
					return false
				end if
				if (isadmin(@actor) or isincorporeal(@actor){{(exemptions.Length == 0 ? "" : " or " + exemptions)}} or not(cansee(@guard, @actor)))
					return false
				end if
				if (not(isnull(enforceablecrime(@actor, tolegalauthority({{authority.Id}})))))
					force @guard "say Be off, criminal!"
					return true
				end if
				var total = @ArmageddonWorldfileSpiceSearch(@guard, @actor.wornitems, 50)
				var wielded = @ArmageddonWorldfileSpiceSearch(@guard, @actor.wieldeditems, 50)
				var carried = @ArmageddonWorldfileSpiceSearch(@guard, @actor.helditems, 75)
				total = @total + @wielded + @carried
				var mount = @actor.mount
				if (not(isnull(@mount)))
					var mountworn = @ArmageddonWorldfileSpiceSearch(@guard, @mount.wornitems, 50)
					var mountwielded = @ArmageddonWorldfileSpiceSearch(@guard, @mount.wieldeditems, 50)
					var mountcarried = @ArmageddonWorldfileSpiceSearch(@guard, @mount.helditems, 75)
					total = @total + @mountworn + @mountwielded + @mountcarried
				end if
				if (@total <= 12)
					sendloc(@actor.location, "$0 briefly inspect|inspects $1's belongings before allowing $1 to pass.", @guard, @actor)
					return false
				end if
				sendloc(@actor.location, "$0 briefly inspect|inspects $1's belongings.", @guard, @actor)
				var crime = reportcrime(tolaw({{law.Id}}), @actor, @guard, null("character"), null("item"), "Detected spice power " + @total)
				if (not(isnull(@crime)))
					force @guard "shout You dare to smuggle spice into Allanak!"
				end if
				return false
				""");
			// Only an actual input attempt runs this callback: no search/roll/crime in
			// pathfinding predicates, and first detection still falls through to movement.
			InstallNpcInputHook("Spice Checkpoint " + direction, "*", checkpoint, attached);
		}
		Record("npc-special.spice-checkpoint", nameof(Law), law.Id, null, new
		{
			Sources = sources.Select(x => x.Vnum), Locations = locations, Law = law.Id, PowerProg = power.Id, SearchProg = search.Id,
			EffectivePowerBindings = powers.SelectMany(g => g.Select(x => new { Prototype = x.Key, Source = x.Value.Source.Vnum, Power = g.Key })),
			Disposition = "Actual attached north/south/east input attempts only. Existing active native crimes refuse passage; first new detected sum >12 is reported but passage continues. Worn/wielded 50%, carried 75%, unlocked nested containers floor(2*chance/3), including closed unlocked containers and ridden mounts. Native visibility applies. Source smoking marker value[2]==1 and zero powers contribute nothing. Base/reset/shop variant effective values are used. No automatic border/possession law, searches on feasibility, confiscation, west/open/close hook, defective pulse or new drug ingestion is invented. Approved arrest/detain and fixed 10-bit fine, no unpaid-fine detention; maximum 50-real-minute active period."
		});
	}

	private void InstallJailerHandover(LegalAuthority authority, int[] sources)
	{
		var handover = NpcServiceProg("ArmageddonWorldfileAllanakJailerHandover", $$"""
			if (@args.Count(word, true) < 2)
				return false
			end if
			var jailer = tocharacter(@npc)
			if (targetcharacter(@actor, getindex(@args, 1)) != @jailer)
				return false
			end if
			var prisoner = targetcharacter(@actor, getindex(@args, 0))
			if (isnull(@prisoner) or not(issubduedby(@prisoner, @actor)))
				return false
			end if
			if (isnull(enforceablecrime(@prisoner, tolegalauthority({{authority.Id}}))))
				force @jailer "say That person is not a criminal."
				return true
			end if
			if (not(handovercriminal(@actor, @prisoner, @jailer, tolegalauthority({{authority.Id}}))))
				send(@actor, "The jailer cannot accept that prisoner right now.")
			end if
			return true
			""");
		InstallNpcInputHook("Allanak Jailer Handover", "release", handover, sources);
		Record("npc-special.allanak-jailer", nameof(FutureProg), handover.Id, null, new
		{
			Sources = sources, Authority = authority.Id,
			Disposition = "Only attached release <visible prisoner> <visible jailer>. The caller must actually subdue the target through a controlled native grapple; no caller clan/rank restriction is added. The same Allanaki authority supplies arrestability and central native remand/belongings. Ineligible targets and wrong addressees fall through; noncriminal controlled targets get the source refusal. Native cleanup releases grapple/drag/restraint and combat links before intake. No unattached theft/stun/pulse/pardon branch is activated."
		});
	}
}
