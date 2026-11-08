#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Law;
using EnforcementAuthority = MudSharp.Models.EnforcementAuthority;
using Law = MudSharp.Models.Law;
using LegalAuthority = MudSharp.Models.LegalAuthority;
using LegalClass = MudSharp.Models.LegalClass;
using Room = MudSharp.Models.Room;
using PatrolRoute = MudSharp.Models.PatrolRoute;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallJustice()
	{
		var identity = Prog("ArmageddonWorldfileEnforcerIdentity", "return true", ProgVariableTypes.Boolean,
			("enforcer", ProgVariableTypes.Character), ("criminal", ProgVariableTypes.Character));
		FutureProg Echo(string suffix, string text) => Prog("ArmageddonWorldfileEnforcer" + suffix,
			$"return \"tell \" + bestkeyword(@enforcer, @criminal) + \" {text}\"", ProgVariableTypes.Text,
			("enforcer", ProgVariableTypes.Character), ("criminal", ProgVariableTypes.Character), ("crime", ProgVariableTypes.Crime));
		var enforcer = CreateAi("Worldfile Settlement Enforcer", "Enforcer", new XElement("Definition",
			new XElement("IdentityProg", identity.Id), new XElement("WarnEchoProg", Echo("Warn", "You are under arrest. Surrender and do not resist.").Id),
			new XElement("WarnStartMoveEchoProg", Echo("Stop", "Stop and surrender.").Id),
			new XElement("FailToComplyEchoProg", Echo("Resist", "Stop resisting arrest.").Id),
			new XElement("ThrowInPrisonEchoProg", Echo("Hold", "You are being held pending a hearing.").Id)));
		InstallAllanakJustice(enforcer);
		foreach (var (special, zone, name) in new[]
		{
			("luir_soldier", 64, "Luir's Guard"),
			("mantis_soldier", 24, "Cai-Shyzn Pack"), ("bw_soldier", 21, "Blackwing Guard"),
			("mal_krian_soldier", 37, "Mal-Krian Guard"), ("cenyr_soldier", 49, "Cenyr Guard"),
			("red_storm_soldier", 48, "Red Storm Guard")
		})
		{
			var sourceNpcs = _source.Npcs.Where(x => HasSpecial(x, special)).Select(x => x.Vnum).ToArray();
			var posts = _source.Resets.Where(x => x.Zone == zone && x.Opcode == "M" && x.ReferenceIssues.Length == 0 &&
				 sourceNpcs.Contains(x.Arguments[1]) && !_interiorTemplates.Contains(x.Arguments[3]))
				.Select(x => x.Arguments[3]).Distinct().Order().ToArray();
			if (posts.Length == 0) continue;
			InstallSettlementJustice(special, zone, name, sourceNpcs, posts, enforcer);
		}
		Flush();
	}

	private LegalAuthority InstallSettlementJustice(string key, int zone, string name, int[] sourceNpcs, int[] posts,
		ArtificialIntelligence ai, int[]? jurisdictionZones = null, int? anchorVnum = null)
	{
		jurisdictionZones ??= [zone];
		var clockRate = SourceLawClockRate();
		Room Facility(string suffix, string label)
		{
			var room = CreateAuxiliaryRoom(zone, $"justice.{key}.{suffix}", $"The {label} of {name}", $"The {label.ToLowerInvariant()} of {name}.");
			room.RoomOverlays.Single().OutdoorsType = (int)RoomOutdoorsType.Indoors;
			room.RoomOverlays.Single().AddedLight = 25;
			return room;
		}
		var reception = Facility("guardhouse", "Guardhouse");
		var holding = Facility("holding", "Holding Cells");
		var belongings = Facility("belongings", "Belongings Storeroom");
		var equipment = Facility("equipment", "Equipment Storeroom");
		var court = Facility("court", "Court");
		var anchor = _rooms[anchorVnum ?? posts[0]];
		LinkJusticeRooms(anchor, reception, "guardhouse", "street");
		LinkJusticeRooms(reception, equipment, "equipment", "guardhouse");
		LinkJusticeRooms(reception, court, "court", "guardhouse");
		Flush();
		foreach (var room in new[] { reception, holding, belongings, equipment, court }) room.CurrentOverlayId = room.RoomOverlays.Single().Id;
		var authority = new LegalAuthority
		{
			Id = Next<LegalAuthority>(), Name = name, CurrencyId = _currency.Id,
			AutomaticallyConvict = true, AutomaticConvictionTime = 300, PlayersKnowTheirCrimes = true,
			PreparingLocationId = reception.Id, MarshallingLocationId = reception.Id, EnforcerStowingLocationId = equipment.Id,
			PrisonLocationId = reception.Id, PrisonReleaseLocationId = anchor.Id, PrisonBelongingsLocationId = belongings.Id,
			JailLocationId = holding.Id, CourtLocationId = court.Id
		};
		foreach (var jurisdiction in jurisdictionZones)
			authority.LegalAuthoritiesZones.Add(new LegalAuthoritiesZones { LegalAuthorityId = authority.Id, ZoneId = _zones[jurisdiction].Id });
		authority.LegalAuthorityRooms.Add(new LegalAuthorityRooms { LegalAuthorityId = authority.Id, RoomId = holding.Id });
		authority.LegalAuthorityJailRooms.Add(new LegalAuthorityJailRoom { LegalAuthorityId = authority.Id, RoomId = holding.Id });
		_db.LegalAuthorities.Add(authority);
		var member = Prog("ArmageddonWorldfileLegalClass" + key, "return true", ProgVariableTypes.Boolean, ("character", ProgVariableTypes.Character));
		var legalClass = new LegalClass { Id = Next<LegalClass>(), LegalAuthorityId = authority.Id, Name = "People",
			MembershipProgId = member.Id, LegalClassPriority = 0, CanBeDetainedUntilFinesPaid = false };
		_db.LegalClasses.Add(legalClass);
		var familyIds = sourceNpcs.SelectMany(v => _npcFamilyIds.TryGetValue(v, out var family) ? family : [_npcs[v].Id]).Distinct().ToArray();
		var eligible = Prog("ArmageddonWorldfileEnforcerRole" + key,
			"return (" + (familyIds.Length == 0 ? "false" : string.Join(" or ", familyIds.Select(id => $"@character.npctemplateid == {id}"))) +
			") and (" + string.Join(" or ", jurisdictionZones.Select(z => $"@character.location.zone.id == {_zones[z].Id}")) + ")",
			ProgVariableTypes.Boolean, ("character", ProgVariableTypes.Character));
		var role = new EnforcementAuthority { Id = Next<EnforcementAuthority>(), LegalAuthorityId = authority.Id, Name = "Guard",
			Priority = 1, CanAccuse = true, CanConvict = false, CanForgive = false, FilterProgId = eligible.Id };
		role.EnforcementAuthoritiesArrestableLegalClasses.Add(new EnforcementAuthoritiesArrestableLegalClasses
			{ EnforcementAuthorityId = role.Id, LegalClassId = legalClass.Id });
		role.EnforcementAuthoritiesAccusableClasses.Add(new EnforcementAuthoritiesAccusableClasses
			{ EnforcementAuthorityId = role.Id, LegalClassId = legalClass.Id });
		_db.EnforcementAuthorities.Add(role);
		var applicability = Prog("ArmageddonWorldfileLawArea" + key,
			"return " + string.Join(" or ", posts.Select(v => $"@criminal.location.id == {_rooms[v].Id}")), ProgVariableTypes.Boolean,
			("criminal", ProgVariableTypes.Character), ("victim", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("lawId", ProgVariableTypes.Number), ("crimeName", ProgVariableTypes.Text));
		foreach (var (type, fine) in new[] { (CrimeTypes.Theft, 10), (CrimeTypes.Assault, 25), (CrimeTypes.AssaultWithADeadlyWeapon, 50) })
		{
			var law = new Law { Id = Next<Law>(), LegalAuthorityId = authority.Id, Name = type.DescribeEnum(), CrimeType = (int)type,
				ActivePeriod = SourceCrimeMaximumRealSeconds(type) * clockRate, EnforcementStrategy = EnforcementStrategy.ArrestAndDetain.ToString(),
				EnforcementPriority = fine, CanBeAppliedAutomatically = true, DoNotAutomaticallyApplyRepeats = true,
				CanBeArrested = true, CanBeOfferedBail = false, LawAppliesProgId = applicability.Id,
				PunishmentStrategy = $"<Strategy type=\"fine\"><DefaultFineAmount>{fine}</DefaultFineAmount><MaximumFineAmount>{fine}</MaximumFineAmount></Strategy>" };
			law.LawsOffenderClasses.Add(new LawsOffenderClasses { LawId = law.Id, LegalClassId = legalClass.Id });
			law.LawsVictimClasses.Add(new LawsVictimClasses { LawId = law.Id, LegalClassId = legalClass.Id });
			_db.Laws.Add(law);
		}
		var route = new PatrolRoute { Id = Next<PatrolRoute>(), LegalAuthorityId = authority.Id, Name = name + " Guard Post",
			PatrolStrategy = "ArmedPatrol", StrategyData = "<Strategy/>", IsReady = true, Priority = 1,
			LingerTimeMajorNode = 60, LingerTimeMinorNode = 5 };
		route.PatrolRouteNodes.Add(new PatrolRouteNode { PatrolRouteId = route.Id, RoomId = anchor.Id, Order = 0 });
		route.PatrolRouteNumbers.Add(new PatrolRouteNumbers { PatrolRouteId = route.Id, EnforcementAuthorityId = role.Id, NumberRequired = 1 });
		foreach (var tod in Enum.GetValues<TimeOfDay>()) route.TimesOfDay.Add(new PatrolRouteTimeOfDay { PatrolRouteId = route.Id, TimeOfDay = (int)tod });
		_db.PatrolRoutes.Add(route);
		foreach (var vnum in sourceNpcs)
		{
			AttachFamilyAi(vnum, ai);
			var onLoad = _progs.Single(x => x.FunctionName == "ArmageddonWorldfileNpcCash" + vnum);
			onLoad.FunctionText = onLoad.FunctionText.Replace("return true",
				$"if (@{eligible.FunctionName}(@npc))\n\tforce @npc \"enforcer {authority.Id}\"\nend if\nreturn true", StringComparison.Ordinal);
		}
		_counts["legal authorities"] = _counts.GetValueOrDefault("legal authorities") + 1;
		Record("justice." + key, nameof(LegalAuthority), authority.Id, null, new
		{
			SourceSpecial = key, SourceZone = zone, JurisdictionZones = jurisdictionZones, SourceGuardPosts = posts, NativeGuardhouse = reception.Id,
			NativeHolding = holding.Id, NativeCourt = court.Id, NativeRelease = anchor.Id,
			CriminalFlagDurationPolicy = "Source maximum", SourceDurationUnitRealSeconds = 600,
			NativeClockRate = clockRate, SourceAssaultAndTheftMaximumUnits = 10,
			Disposition = "Authored conservative native baseline, not recovered source law: theft 10, assault 25, deadly assault 50 bits; warning/arrest, automatic conviction after 300 native seconds, no capital/mutilation or fine-based continued detention. Crime creation is limited to listed guard posts; native pursuit/custody persists beyond them. Missing source jail rooms are replaced by plainly named facilities."
		});
		return authority;
	}

	private double SourceLawClockRate()
	{
		var calendar = _db.Calendars.Single(x => x.Id == _db.Cultures.Single(c => c.Id ==
			_settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId);
		var clock = _db.Clocks.Single(x => x.Id == calendar.FeedClockId);
		var rate = (double?)XElement.Parse(clock.Definition).Element("InGameSecondsPerRealSecond")
			?? throw new InvalidOperationException("Imported laws require the setting clock's time rate.");
		return double.IsFinite(rate) && rate > 0 ? rate :
			throw new InvalidOperationException("Imported laws require a finite positive clock time rate.");
	}

	internal static double SourceCrimeMaximumRealSeconds(CrimeTypes type) => type switch
	{
		// handler.c affect_to_char uses RT_ZAL_HOUR (600 real seconds), not
		// SECS_PER_MUD_HOUR. Preserve the approved maximum of each random range.
		// Source assault has no separate deadly-weapon duration category.
		CrimeTypes.Theft or CrimeTypes.Assault or CrimeTypes.AssaultWithADeadlyWeapon => 10 * 600.0,
		CrimeTypes.TrafficingContraband => 5 * 600.0,
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, "No source criminal-flag duration is mapped.")
	};

	private void LinkJusticeRooms(Room from, Room to, string outgoing, string incoming)
	{
		var exit = new Exit
		{
			Id = Next<Exit>(), RoomId1 = from.Id, RoomId2 = to.Id,
			Direction1 = (int)CardinalDirection.Unknown, Direction2 = (int)CardinalDirection.Unknown,
			Keywords1 = outgoing, PrimaryKeyword1 = outgoing, Keywords2 = incoming, PrimaryKeyword2 = incoming,
			Verb1 = "enter", Verb2 = "enter", OutboundTarget1 = outgoing, InboundTarget1 = incoming,
			OutboundTarget2 = incoming, InboundTarget2 = outgoing, InboundDescription1 = "", InboundDescription2 = "",
			OutboundDescription1 = "", OutboundDescription2 = "", TimeMultiplier = 1.0, BlockedLayers = "",
			DoorSize = (int)SizeCategory.Titanic, MaximumSizeToEnter = (int)SizeCategory.Titanic,
			MaximumSizeToEnterUpright = (int)SizeCategory.Titanic
		};
		_db.Exits.Add(exit);
		_db.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlayId = from.RoomOverlays.Single().Id, ExitId = exit.Id });
		_db.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlayId = to.RoomOverlays.Single().Id, ExitId = exit.Id });
	}
}
