using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunCompletionProgressionAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var first = FixtureSeed.Create(database, "arm_completion", false);
		var second = FixtureSeed.Create(database, "arm_completion_other", false);
		var native = NativeRuntime.Load(first, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var actor = native.Actor; var world = native.World;
		var skill = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var otherSkill = CastingRequired(world.Traits.GetByName("ARM02 Sorcerer Proficiency"));
		var source = native.Resource;
		var earth = (SkillLevelBasedMagicCapability)native.Capability;
		var spell = new MagicSpell("ARM Completion Native", earth.School);
		((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new self", $"trait {skill.Id}", "difficulty easy", "threshold minorpass",
			"duration ARM02 Duration", $"cost {source.Id} ARM02 Cost", "prog rejuvenation_known", "castemote A shell forms.",
			"failcastemote The shell crumbles.", "effect add spellarmour", "effect 1 absorb grade*10+variable",
			"grades fixture", "grades efficiency source 7 1", "grades overreach 1 1" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), $"Completion spell builder refused {command}");
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {source.Id} {source.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on",
			$"casting entry skill {spell.Id} 30 60 relative", "casting enable on" })
			Require(earth.BuildingCommand(actor, new StringStack(command)), $"Completion capability builder refused {command}");
		var sorcerer = (SkillLevelBasedMagicCapability)earth.Clone("ARM Completion Other Route");
		((All<IMagicCapability>)world.MagicCapabilities).Add(sorcerer);
		Require(sorcerer.BuildingCommand(actor, new StringStack($"casting trait {otherSkill.Id}")), "Alternate trait refused.");
		Require(sorcerer.BuildingCommand(actor, new StringStack($"casting resources {source.Id} {second.ResourceId} gather")), "Alternate reserve refused.");
		Require(sorcerer.BuildingCommand(actor, new StringStack($"casting entry skill {spell.Id} 60 90 relative")), "Alternate opening/cap refused.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var now = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);
		var samples = 0;
		var service = new MagicCastingService(world, clock: () => now, random: () => { samples++; return 0.1; }, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		Require(service.Enrol(staff.Object, actor, earth.Id, "completion native").Allowed, "Native enrolment refused.");
		Require(service.Enrol(staff.Object, actor, sorcerer.Id, "completion alternate").Allowed, "Alternate enrolment refused.");
		Require(actor.GetTrait(skill) is Skill && actor.TraitRawValue(skill) == 30 && actor.TraitRawValue(otherSkill) == 60,
			"Route-specific source openings did not reach real native skills.");
		actor.GetTrait(skill).Value += 100;
		Require(actor.TraitRawValue(skill) == 60, "Native positive write exceeded the route cap.");
		Require(service.Grant(staff.Object, actor, earth.Id, spell.Id, "idempotence").Allowed && actor.TraitRawValue(skill) == 60,
			"Repeated grant lowered or re-opened proficiency.");
		actor.SetMerits([]); actor.GetTrait(skill).Value += 20;
		Require(actor.TraitRawValue(skill) == 60, "Lost route erased history or permitted another gain.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		FlushCasting(native);
		Console.WriteLine("ARM-COMP-opening-cap=passed native-Skill opening:30/60 cap:60 route-loss-history-preserved");
		var store = new MagicCastingStateStore();
		foreach (var (mastery, cost) in new[] { (1, 50.0), (2, 25.0) })
		{
			now = now.AddHours(1); actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
			actor.AddResource(source, 100);
			store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = mastery });
			var quote = service.Quote(new(actor, earth.Id, spell.Id, 1, false, "self"));
			Require(quote.Allowed && quote.Invocation!.Costs.Single().Amount == cost, "Native efficiency quote mismatch.");
			MagicModule.MagicGeneric(actor, $"{earth.School.SchoolVerb} cast \"{spell.Name}\" grade 1 on self via {earth.Id}");
			Require(actor.MagicResourceAmounts[source] == 100 - cost && samples == 0, "Native command debit or sample mismatch.");
			Console.WriteLine($"ARM-COMP-efficiency=passed mastery:{mastery} requested:1 cost:{cost} non-admin-command");
		}
		now = now.AddHours(1); actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		actor.AddResource(source, 100);
		store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 6, NextMasteryUtc = DateTime.UnixEpoch });
		MagicModule.MagicGeneric(actor, $"{earth.School.SchoolVerb} cast \"{spell.Name}\" grade 7 overreach on self via {earth.Id}");
		Require(service.Acquisition(actor, spell.Id)!.ControlledGrade == 7 && samples == 1 && actor.MagicResourceAmounts[source] == 25,
			$"Cap-60 maximum grade mismatch: grade {service.Acquisition(actor, spell.Id)!.ControlledGrade}, samples {samples}, balance {actor.MagicResourceAmounts[source]} (expected 7/1/25 after source cost 75).");
		Require(!service.Quote(new(actor, earth.Id, spell.Id, 8, true, "self")).Allowed, "Grade eight escaped bounds.");
		now = now.AddHours(1); actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		Require(service.Quote(new(actor, earth.Id, spell.Id, 1, false, "self")).Invocation!.Costs.Single().Amount == 7, "Maximum mastery floor quote mismatch.");
		MagicModule.MagicGeneric(actor, $"{earth.School.SchoolVerb} cast \"{spell.Name}\" grade 1 on self via {earth.Id}");
		Require(actor.MagicResourceAmounts[source] == 18 && samples == 1, "Maximum mastery floor payment mismatch.");
		FlushCasting(native);
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			Require(read.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == actor.Id && x.TraitDefinitionId == skill.Id).Value == 60,
				"Native cap result did not persist.");
			Require(read.MagicCastingOperations.Count(x => x.CharacterId == actor.Id && x.Stage == "Completed") == 4, "Native invocations did not produce four completed receipts.");
		}
		Require(earth.BuildingCommand(actor, new StringStack($"casting entry remove {spell.Id}")), "Admission removal refused.");
		Require(service.RawSkillImprovementCap(actor, skill.Id) == 0, "Admission removal released the historic capped skill.");
		FlushCasting(native);
		RunCastingReaderProcess(new(database.Name, first, second, spell.Id, earth.Id, sorcerer.Id, skill.Id, otherSkill.Id,
			null, 7, 18, 0, RawSkill: 60, VerifyCapLoss: true));
		Console.WriteLine("ARM-COMP-native=passed cap:60 max-grade:7 efficiency:50/25/7 overreach-cost:75 balance:18 receipts:4 separate-process-reload practice:NOT_RUN");
		return 0;
	}

	private static void VerifyCompletionCapLossReload(TestDatabase database, CastingReader input)
	{
		var native = NativeRuntime.Load(input.Earth, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, false);
		var world = native.World; var actor = native.Actor;
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(CastingRequired(world.MagicCapabilities.Get(input.EarthCapability))),
			NativeRuntime.NewCapabilityMerit(CastingRequired(world.MagicCapabilities.Get(input.SorcererCapability)))]);
		var service = new MagicCastingService(world, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var skill = CastingRequired(world.Traits.Get(input.EarthSkill));
		Require(service.RawSkillImprovementCap(actor, skill.Id) == 0, "Restart lost cap opt-in after admission removal.");
		actor.GetTrait(skill).Value += 5;
		Require(actor.TraitRawValue(skill) == input.RawSkill, "Restart allowed a gain or erased historic raw proficiency.");
		Console.WriteLine("ARM-COMP-cap-loss-reload=passed removed-admission cap:0 history:60 independent-process");
	}
}
