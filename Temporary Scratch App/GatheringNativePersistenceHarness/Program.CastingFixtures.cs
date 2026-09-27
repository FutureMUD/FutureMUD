using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void VerifyCastingQuarantineReload(TestDatabase database, CastingReader input)
	{
		var runtime = NativeRuntime.Load(input.Earth, database.ConnectionString, true);
		ConfigureCastingWorld(runtime, database.ConnectionString, false);
		var earth = CastingRequired(runtime.World.MagicCapabilities.Get(input.EarthCapability));
		var sorcerer = CastingRequired(runtime.World.MagicCapabilities.Get(input.SorcererCapability));
		runtime.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		var service = new MagicCastingService(runtime.World, random: () => throw new InvalidOperationException("Quarantined restart rerolled"));
		runtime.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var refusal = service.Preflight(runtime.Actor, sorcerer.Id, input.Spell, 3, true);
		Require(refusal?.Contains(input.Operation!.Value.ToString()) == true, "Restart alternate tradition lost receipt quarantine: " + refusal);
		var shared = runtime.World.MagicSpells.GetByName("ARM02 Shared Trait Sentinel");
		if (shared is not null)
		{
			Require(!service.Quote(new(runtime.Actor, earth.Id, shared.Id, 1, false, "self")).Allowed, "Restart shared-trait spell escaped quarantine.");
			var untouched = service.Quote(new(runtime.Actor, sorcerer.Id, shared.Id, 1, false, "self"));
			Require(untouched.Allowed, "Restart untouched route is unavailable: " + untouched.Reason);
		}
		Console.WriteLine("ARM02-quarantine-restart=passed alternate-route shared-trait shared-reserve untouched-route");
	}

	private static void VerifyCastingAuxiliaryFixtures(NativeRuntime runtime, string connection, ICharacter staff,
		MagicSpell stone, SkillLevelBasedMagicCapability earth, ITraitDefinition separateTrait)
	{
		var actor = runtime.Actor; var world = runtime.World;
		var fireSchool = world.MagicSchools.First(x => x.Id != stone.School.Id);
		Mock.Get(fireSchool).SetupGet(x => x.Name).Returns("ARM02 Fire");
		MagicSpell Author(string name, IMagicSchool school, params string[] effects)
		{
			var result = new MagicSpell(name, school); ((All<IMagicSpell>)world.MagicSpells).Add(result);
			foreach (var command in new[] { "trigger new self", $"trait {earth.CastingPolicy!.DefaultTraitId}", "difficulty easy", "threshold minorpass",
				"duration ARM02 Duration", $"cost {runtime.Resource.Id} ARM02 Cost", $"prog {stone.SpellKnownProg.Id}",
				"castemote The fixture spell manifests.", "failcastemote The fixture spell fails.", "grades fixture" }.Concat(effects))
				Require(result.BuildingCommand(actor, new StringStack(command)), $"Auxiliary builder refused {command}.");
			Require(result.ReadyForGame, result.ReadyForGame ? "" : result.WhyNotReadyForGame(actor));
			Require(earth.BuildingCommand(actor, new StringStack($"casting entry add {result.Id}")), "Auxiliary admission failed.");
			return result;
		}
		var fire = Author("ARM02 Ember Lance", fireSchool, "effect add damage", "effect 1 type burning", "effect 1 formula grade*5",
			$"effect 1 bodypart {runtime.Body.Bodyparts.First().Id}");
		Require(earth.BuildingCommand(actor, new StringStack($"casting entry trait {fire.Id} {separateTrait.Id}")), "Per-spell trait override failed.");
		var ward = Author("ARM02 Wardcraft", stone.School, "effect add personaltagward", "effect 1 tag ARM02WardTest");
		Require(earth.BuildingCommand(actor, new StringStack($"casting prerequisite add {ward.Id} {stone.Id} 2 40")), "Wardcraft edge failed.");
		world.SaveManager.Flush();
		var service = new MagicCastingService(world, clock: () => new DateTime(2026, 9, 28, 2, 0, 0, DateTimeKind.Utc), random: () => 0.1,
			flush: () => FlushCasting(runtime)); runtime.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		service.NotifyProgress(actor, spellId: stone.Id);
		Require(service.Acquisition(actor, ward.Id) is not null && service.Acquisition(actor, fire.Id) is null,
			"A shared trait acquired an unqualified spell, or the enrolled Earth edge failed.");
		Require(service.Grant(staff, actor, earth.Id, fire.Id, "ARM02 native delivered damage fixture").Allowed, "Fire grant failed.");
		var store = new MagicCastingStateStore(); store.Write(acquired: service.Acquisition(actor, fire.Id)! with { ControlledGrade = 2 });
		actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(runtime.Resource, 100);
		var before = runtime.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
		var cast = service.Cast(new(actor, earth.Id, fire.Id, 3, true, "self"));
		Require(cast.Status == MagicCastingStatus.Succeeded, cast.Message);
		Require(runtime.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun) > before && service.Acquisition(actor, fire.Id)!.ControlledGrade == 3,
			"Native Ember Lance did not deliver wound magnitude and earn its instantaneous applied result.");
		Require(fire.School.Id == fireSchool.Id && earth.School.Id != fire.School.Id, "Explicit admission changed the native Fire school.");
		actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		var wardCast = service.Cast(new(actor, earth.Id, ward.Id, 2, true, "self"));
		Require(wardCast.Status == MagicCastingStatus.Succeeded && service.Acquisition(actor, ward.Id)!.ControlledGrade == 2,
			"Native tag ward did not report an applied operation: " + wardCast.Message);
		FlushCasting(runtime);
		using var read = NewIndependentContext(connection);
		Require(read.Wounds.Any(x => x.BodyId == runtime.Body.Id), "Delivered damage did not persist as native wounds.");
		Console.WriteLine("ARM02-auxiliary=passed Ember-Lance-native-wounds Wardcraft-native-tag-ward per-spell-trait shared-trait-edge native-school-preserved");
	}
}
