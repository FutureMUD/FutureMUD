using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Gathering;
using MudSharp.Magic.SpellEffects;
using Db = MudSharp.Models;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void VerifyCastingCharacterAndLifecycle(TestDatabase database, NativeRuntime earth,
		FixtureIds sorcererFixture, CastingReader descriptor, DateTime now)
	{
		var sorcerer = NativeRuntime.Load(sorcererFixture, database.ConnectionString, true);
		ConfigureCastingWorld(sorcerer, database.ConnectionString, false);
		var capability = CastingRequired(sorcerer.World.MagicCapabilities.Get(descriptor.SorcererCapability));
		var spell = (MagicSpell)CastingRequired(sorcerer.World.MagicSpells.Get(descriptor.Spell));
		var trait = CastingRequired(sorcerer.World.Traits.Get(descriptor.SorcererSkill));
		var reserve = CastingRequired(sorcerer.World.MagicResources.Get(sorcererFixture.ResourceId));
		sorcerer.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(capability)]);
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		var service = new MagicCastingService(sorcerer.World, clock: () => now, random: () => 0.1, flush: () => FlushCasting(sorcerer));
		sorcerer.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		Require(!sorcerer.Actor.IsAdministrator(), "Sorcerer fixture is administrative.");
		Require(service.Enrol(staff.Object, sorcerer.Actor, capability.Id, "ARM02 second non-admin").Allowed, "Sorcerer enrolment failed.");
		sorcerer.Actor.SetTraitValue(trait, 62);
		sorcerer.Actor.AddTrait(sorcerer.World.Traits.GetByName("ARM02 Agility"), 10);
		sorcerer.Actor.AddResource(reserve, 100);
		var store = new MagicCastingStateStore();
		store.Write(acquired: service.Acquisition(sorcerer.Actor, spell.Id)! with { ControlledGrade = 2 });
		var result = service.Cast(new(sorcerer.Actor, capability.Id, spell.Id, 3, true, "self"));
		Require(result.Status == MagicCastingStatus.Succeeded, result.Message);
		Require(sorcerer.Actor.MagicResourceAmounts[reserve] == 77.5 && sorcerer.Actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -3,
			"Sorcerer did not get the grade-three agility/cost binding.");
		Require(sorcerer.Actor.EffectsOfType<SpellArmourProtectionEffect>().Single().ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(sorcerer.Actor) == 92,
			"Sorcerer armour did not bind its own trait.");
		Mock.Get(sorcerer.World.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Require(sorcerer.Actor.MagicResourceAmounts[reserve] == 77.5 && !sorcerer.Actor.MagicResourceGenerators.Any(), "Gathering-only Sorcerer gained passive energy.");
		FlushCasting(sorcerer);
		Console.WriteLine("ARM02-sorcerer=passed second-non-admin distinct-reserve:77.5 agility:-3 armour:92 gathering-only");

		RunCastingReaderProcess(descriptor with { VerifyEffects = true, SkillDeadline = now.AddSeconds(60), MasteryDeadline = now.AddSeconds(600) });
		var secondBody = FixtureSeed.Create(database, "arm02_second_body", false);
		long instanceId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var instance = new Db.CharacterInstance
			{
				Id = earth.Actor.InstanceId + 100000,
				CharacterId = earth.Actor.Id, BodyId = secondBody.BodyId, EmbodiedBodyId = secondBody.BodyId,
				InstanceName = "ARM02 Second Body", InstanceKind = (int)CharacterInstanceKind.PhysicalClone,
				ControlPolicy = (int)CharacterInstanceControlPolicy.PlayerFocusable, PersistencePolicy = (int)CharacterInstancePersistencePolicy.Persistent,
				IsEmbodied = true, IsControllable = true, LocationId = secondBody.RoomId, State = (int)CharacterState.Awake,
				PositionId = (int)MudSharp.Body.Position.PositionStates.PositionStanding.Instance.Id,
				PositionTargetType = "", PositionEmote = "", CreatedBySourceKey = "ARM02 isolated acceptance",
				CreatedDateTime = now, EffectData = "<Effects/>"
			};
			db.CharacterInstances.Add(instance); db.SaveChanges(); instanceId = instance.Id;
		}
		RunCastingReaderProcess(descriptor with { SecondBody = secondBody, SecondInstance = instanceId,
			SkillDeadline = now.AddSeconds(60), MasteryDeadline = now.AddSeconds(600) });
		// A further fresh process verifies the second body's persisted effects and shared clocks without another cast.
		RunCastingReaderProcess(descriptor with { Balance = 72.5, VerifyEffects = true, SecondBody = secondBody, SecondInstance = instanceId,
			SkillDeadline = now.AddSeconds(60), MasteryDeadline = now.AddSeconds(600) });
	}

	private static void VerifyCastingRuntimeReload(TestDatabase database, CastingReader input)
	{
		var runtime = NativeRuntime.Load(input.Earth, database.ConnectionString, true);
		ConfigureCastingWorld(runtime, database.ConnectionString, false);
		var actor = runtime.Actor;
		var earth = CastingRequired(runtime.World.MagicCapabilities.Get(input.EarthCapability));
		var sorcerer = CastingRequired(runtime.World.MagicCapabilities.Get(input.SorcererCapability));
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		using var db = NewIndependentContext(database.ConnectionString);
		actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		var service = new MagicCastingService(runtime.World, clock: () => new DateTime(2026, 9, 27, 2, 0, 10, DateTimeKind.Utc),
			random: () => throw new InvalidOperationException("Restart must not reroll mastery"), flush: () => FlushCasting(runtime));
		runtime.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		service.Reconcile(actor);
		Require(actor.MagicResourceGenerators.Count() == 1, "Restart did not reconcile exactly one Earth generator.");
		Require(actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -3, "Retained agility modifier did not reload.");
		Require(actor.EffectsOfType<SpellArmourProtectionEffect>().Single().ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(actor) == 72,
			"Retained Earth armour magnitude changed on reload.");
		Require(((TraitBoostEffect)CastingRequired(runtime.World.MagicSpells.Get(input.Spell)).SpellEffects.Last()).Bonus == 0, "Catalogue scalar was mutated.");
		if (input.SecondBody is not { } fixture) return;
		var secondary = NativeRuntime.Load(fixture with { CharacterId = actor.Id, ResourceId = input.Earth.ResourceId, CapabilityId = input.EarthCapability }, database.ConnectionString, true);
		ConfigureCastingWorld(secondary, database.ConnectionString, false);
		SetPrivateMember(secondary.Actor, "Gameworld", runtime.World);
		SetPrivateMember(secondary.Body, "Gameworld", runtime.World);
		Mock.Get(secondary.Actor.Location).SetupGet(x => x.Gameworld).Returns(runtime.World);
		((All<MudSharp.Construction.IRoom>)runtime.World.Rooms).Add(secondary.Actor.Location);
		secondary.Actor.HarnessIdentity = actor;
		SetPrivateField(secondary.Actor, "_instanceId", input.SecondInstance!.Value);
		Require(secondary.Actor.InstanceId != actor.InstanceId && secondary.Body.Id != runtime.Body.Id,
			"The secondary fixture must have distinct instance and physical body identities.");
		SetPrivateField(secondary.Actor, "_characterTraits", new List<ITrait>());
		SetPrivateField(actor, "_secondaryInstances", new List<ICharacterInstance> { secondary.Actor });
		secondary.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		secondary.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		SetPrivateField(actor, "_focusedInstance", secondary.Actor);
		var skill = CastingRequired(runtime.World.Traits.Get(input.EarthSkill));
		Require(ReferenceEquals(actor.GetTrait(skill), secondary.Actor.GetTrait(skill)), "Second body copied rather than shared its native skill.");
		Require(service.Preflight(actor, earth.Id, input.Spell, 1, false) is not null, "Unfocused primary was allowed to cast.");
		var row = db.CharacterInstances.AsNoTracking().Single(x => x.Id == input.SecondInstance);
		secondary.Actor.RestoreCastingEffects(row.EffectData);
		if (input.VerifyEffects)
		{
			Require(secondary.Actor.TraitRawValue(runtime.World.Traits.GetByName("ARM02 Secondary Proficiency")) == 10 &&
				service.Acquisition(secondary.Actor, CastingRequired(runtime.World.MagicSpells.GetByName("ARM02 Secondary Acquisition")).Id) is not null,
				"Second-body acquisition or its newly opened canonical native skill did not persist.");
			Require(secondary.Actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -1, "Second-body grade-one effect did not persist.");
			Require(secondary.Actor.MagicResourceAmounts[runtime.Resource] == 72.5, "Second-body canonical reserve did not persist.");
			Console.WriteLine("ARM02-second-body-restart=passed persisted-instance-effects shared-skill shared-deadlines shared-reserve");
			return;
		}
		var secondSpell = CastingRequired(runtime.World.MagicSpells.GetByName("ARM02 Secondary Acquisition"));
		var secondTrait = CastingRequired(runtime.World.Traits.GetByName("ARM02 Secondary Proficiency"));
		Require(!actor.HasTrait(secondTrait), "Second-body proficiency unexpectedly opened before acquisition.");
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		Require(service.Grant(staff.Object, secondary.Actor, earth.Id, secondSpell.Id, "ARM02 second-body explicit acquisition").Allowed,
			"Acquisition through the second body failed.");
		Require(ReferenceEquals(actor.GetTrait(secondTrait), secondary.Actor.GetTrait(secondTrait)) && actor.TraitRawValue(secondTrait) == 10,
			"Second-body acquisition did not open one shared native skill.");
		var before = actor.MagicResourceAmounts[runtime.Resource];
		secondary.Actor.SetMerits([]); service.Reconcile(secondary.Actor);
		Require(!actor.MagicResourceGenerators.Any(), "Capability detachment left a generator attached.");
		Mock.Get(runtime.World.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Require(actor.MagicResourceAmounts[runtime.Resource] == before, "Detached capability regenerated energy.");
		secondary.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		service.Reconcile(secondary.Actor); service.Reconcile(secondary.Actor);
		Require(actor.MagicResourceGenerators.Count() == 1 && actor.MagicResourceAmounts[runtime.Resource] == before, "Restoration duplicated a generator or balance.");
		// Repeated focus changes preserve a single callback; the configured generator itself controls conscious/rest rules.
		SetPrivateField(actor, "_focusedInstance", actor); service.Reconcile(actor);
		SetPrivateField(actor, "_focusedInstance", secondary.Actor); service.Reconcile(secondary.Actor);
		Mock.Get(runtime.World.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Require(actor.MagicResourceAmounts[runtime.Resource] == before + 2, "Focused passive tick was missing or duplicated.");
		Require(actor.UseResource(runtime.Resource, 2), "Could not isolate generated amount for cast assertion.");
		var store = new MagicCastingStateStore(); var skillDeadline = store.Opportunity(actor.Id, skill.Id)!.NextUtc;
		var result = service.Cast(new(secondary.Actor, earth.Id, input.Spell, 1, false, "self"));
		Require(result.Status == MagicCastingStatus.Succeeded, result.Message);
		Require(actor.MagicResourceAmounts[runtime.Resource] == before - 5 && store.Opportunity(actor.Id, skill.Id)!.NextUtc == skillDeadline,
			"Second-body cast duplicated an opportunity or paid from the wrong holder.");
		Require(secondary.Actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -1, "Second-body grade-one agility binding failed.");
		var gatheringClock = new HarnessClock();
		var gathering = new MagicGatheringService(runtime.World, clock: gatheringClock);
		var gather = gathering.Begin(secondary.Actor, (IMagicGatheringCapability)earth, "draw", 2);
		Require(gather.Success && gather.OperationId.HasValue, "Second-body gathering did not begin: " + gather.Message);
		gatheringClock.Advance(TimeSpan.FromSeconds(1));
		var gathered = gathering.Complete(secondary.Actor, gather.OperationId!.Value);
		Require(gathered.Success, "Second-body gathering failed: " + gathered.Message);
		Require(actor.MagicResourceAmounts[runtime.Resource] == before - 3 && secondary.Body.Wounds.Any() && !runtime.Body.Wounds.Any(),
			"Gathering did not credit the canonical holder while charging the acting body.");
		using (var observed = NewIndependentContext(database.ConnectionString))
			Require(observed.CharactersMagicResources.Single(x => x.CharacterId == actor.Id && x.MagicResourceId == runtime.Resource.Id).Amount == before - 3 &&
				observed.Wounds.Any(x => x.BodyId == secondary.Body.Id), "Second-body gathering accounting did not persist together.");
		Require(actor.UseResource(runtime.Resource, 2), "Could not isolate the gathering credit for subsequent restart assertions.");
		FlushCasting(runtime);
		using (new FMDB()) { secondary.Actor.Save(); secondary.Body.Save(); FMDB.Context.SaveChanges(); }
		Require(db.CharacterCastingEnrolments.Count(x => x.CharacterId == actor.Id) == 2 && db.CharacterAcquiredSpells.Count(x => x.CharacterId == actor.Id) == 2,
			"Capability restoration duplicated starting roots or enrolment markers.");
		Console.WriteLine("ARM02-second-body=passed focus detach restore single-passive-tick grade-one-cost:5 shared-opportunity native-gathering-canonical-credit-physical-costs");
	}
}
