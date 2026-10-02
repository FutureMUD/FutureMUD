using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunSupportProgressionAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var first = FixtureSeed.Create(database, "arm_support", false);
		var second = FixtureSeed.Create(database, "arm_support_other", false);
		var native = NativeRuntime.Load(first, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var world = native.World; var actor = native.Actor;
		// Select real native improver/skill bindings explicitly. These deterministic fixture settings
		// prove attainability through the ordinary skill-use hook; they are not stock balance defaults.
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var improver = new MudSharp.Models.Improver { Name = "ARM support native use", Type = "classic",
				Definition = "<Definition Chance='1' Expression='10' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0'/>" };
			var cap = new MudSharp.Models.TraitExpression { Name = "ARM support native cap", Expression = "100" };
			db.Improvers.Add(improver); db.TraitExpressions.Add(cap); db.SaveChanges();
			var models = new All<IImprovementModel>(); models.Add(new ClassicImprovement(world, improver));
			native.WorldMock.SetupGet(x => x.ImprovementModels).Returns(models);
			native.WorldMock.SetupGet(x => x.LogManager).Returns(Mock.Of<ILogManager>());
			((All<ITraitExpression>)world.TraitExpressions).Add(new TraitExpression(cap, world));
			var traits = new All<ITraitDefinition>();
			foreach (var model in db.TraitDefinitions.Where(x => x.TraitGroup == "ARM02"))
			{
				if (model.Type == (int)TraitType.Skill) { model.ImproverId = improver.Id; model.ExpressionId = cap.Id; }
				TraitDefinition definition = model.Type == (int)TraitType.Skill ? new SkillDefinition(model, world) : new AttributeDefinition(model, world);
				definition.Initialise(model); traits.Add(definition);
			}
			db.SaveChanges(); native.WorldMock.SetupGet(x => x.Traits).Returns(traits);
		}
		var parentTrait = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var supportTrait = CastingRequired(world.Traits.GetByName("ARM02 Secondary Proficiency"));
		var identifyTrait = CastingRequired(world.Traits.GetByName("ARM02 Sorcerer Proficiency"));
		var capPolicy = (SkillLevelBasedMagicCapability)native.Capability;
		var parent = NewSupportFixtureSpell(native, "ARM Shadow Passage", parentTrait);
		var identify = NewSupportFixtureSpell(native, "ARM Identify", identifyTrait);
		foreach (var command in new[] { $"casting trait {parentTrait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} gather",
			$"casting entry add {parent.Id}", $"casting entry starting {parent.Id} on", $"casting entry skill {parent.Id} 30 90 relative",
			$"casting entry add {identify.Id}", $"casting entry trait {identify.Id} {identifyTrait.Id}", $"casting entry skill {identify.Id} 30 90 relative",
			$"casting support add {supportTrait.Id} 30 90 off", $"casting support prerequisite spell {supportTrait.Id} {parent.Id} 1 80",
			$"casting prerequisite trait {identify.Id} {supportTrait.Id} 80", "casting enable on" })
			Require(capPolicy.BuildingCommand(actor, new StringStack(command)), "Support builder refused: " + command);
		var other = (SkillLevelBasedMagicCapability)capPolicy.Clone("ARM Support Alternate");
		((All<IMagicCapability>)world.MagicCapabilities).Add(other);
		Require(other.BuildingCommand(actor, new StringStack($"casting resources {native.Resource.Id} {second.ResourceId} gather")), "Alternate reserve refused.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(capPolicy), NativeRuntime.NewCapabilityMerit(other)]);
		world.SaveManager.Flush();
		var service = new MagicCastingService(world, flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var enrol = new FutureProg(world, "armAuthoredEnrolment", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap")],
			"return enrolchannelcasting(@actor, @cap, \"selected NPC creation fixture\")");
		Require(enrol.Compile(), enrol.CompileError);
		Require(enrol.ExecuteBool(actor, capPolicy), "Authored native enrolment refused.");
		Require(!actor.HasTrait(supportTrait), "Support opened before parent reached 80.");
		var store = new MagicCastingStateStore();
		var before = store.Acquisition(actor.Id, parent.Id)!;
		var primary = (Skill)actor.GetTrait(parentTrait);
		for (var i = 0; i < 5; i++) Require(primary.TraitUsed(actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []),
			$"Parent native use failed: raw {primary.RawValue}, native maximum {actor.TraitMaxValue(parentTrait)}, casting cap {service.RawSkillImprovementCap(actor, parentTrait.Id)}.");
		Require(actor.TraitRawValue(parentTrait) == 80 && actor.GetTrait(supportTrait) is Skill && actor.TraitRawValue(supportTrait) == 30,
			"Parent native use did not authorise/open Component Crafting at 30.");
		var secondary = CreateSupportFocusedBody(database, native, second, capPolicy, other);
		Require(ReferenceEquals(actor.GetTrait(supportTrait), secondary.Actor.GetTrait(supportTrait)), "Second body copied support skill state.");
		var support = (Skill)actor.GetTrait(supportTrait);
		for (var i = 0; i < 4; i++) Require(support.TraitUsed(secondary.Actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []), "Support native use failed.");
		Require(actor.TraitRawValue(supportTrait) == 70 && service.Acquisition(actor, identify.Id) is null, "Identify acquired below 80.");
		Require(support.TraitUsed(secondary.Actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []), "Support threshold native use failed.");
		Require(actor.TraitRawValue(supportTrait) == 80 && service.Acquisition(actor, identify.Id)?.ControlledGrade == 1 && actor.TraitRawValue(identifyTrait) == 30,
			"Native Component Crafting 80 did not acquire Identify at grade 1.");
		Require(store.Acquisition(actor.Id, parent.Id) == before && store.Opportunity(actor.Id, supportTrait.Id) is null,
			"Ordinary support use changed spell mastery or a paid magic opportunity.");
		Require(support.TraitUsed(secondary.Actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []), "Support gain to cap failed.");
		Require(actor.TraitRawValue(supportTrait) == 90 && !support.TraitUsed(secondary.Actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []), "Support cap90 failed.");
		Require(enrol.ExecuteBool(actor, other), "Second enrolled route refused.");
		actor.AddResource(native.Resource, 17); FlushCasting(native);
		var writes = store.SupportGrant(actor.Id, capPolicy.CastingPolicy!.Identity, capPolicy.CastingPolicy.Supports.Single().Key)!;
		Require(writes.OpeningSkill == 30 && writes.RawSkillCap == 90 && store.Unresolved(actor.Id).Count == 0, "Support durable grant/terminal exclusion failed.");
		Require(enrol.ExecuteBool(actor, capPolicy) && actor.TraitRawValue(supportTrait) == 90 && actor.MagicResourceAmounts[native.Resource] == 17,
			"Repeat authored enrolment reset a skill or refilled a reserve.");
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			var count = read.MagicCastingOperations.Count(x => x.CharacterId == actor.Id);
			Require(enrol.ExecuteBool(secondary.Actor, capPolicy), "Second-body authored enrolment refused.");
			Require(read.MagicCastingOperations.Count(x => x.CharacterId == actor.Id) == count && read.CharacterCastingEnrolments.Count(x => x.CharacterId == actor.Id) == 2,
				"Second body duplicated support grants, roots or enrolment.");
		}
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Require(!service.ReconcileOperation(staff.Object, actor, writes.OperationId, "cannot erase terminal grant").Changed &&
			store.Operation(writes.OperationId)?.Stage == MagicCastingStateStore.CappedSupportGranted, "Terminal support grant was mutable.");
		actor.SetMerits([]); secondary.Actor.SetMerits([]); Require(!enrol.ExecuteBool(actor, capPolicy), "Temporary/absent merit enrolled through authored hook.");
		Require(service.RawSkillImprovementCap(actor, supportTrait.Id) == 0 && !support.TraitUsed(actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []),
			"Lost permanent route allowed a support gain.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(capPolicy), NativeRuntime.NewCapabilityMerit(other)]);
		secondary.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(capPolicy), NativeRuntime.NewCapabilityMerit(other)]); FlushCasting(native);
		RunCastingReaderProcess(new(database.Name, first, second, parent.Id, capPolicy.Id, other.Id, parentTrait.Id, identifyTrait.Id,
			null, 1, 17, 0, RawSkill: 80, SupportTrait: supportTrait.Id, IdentifySpell: identify.Id));
		Console.WriteLine("ARM-SUPPORT-native=passed authored-enrolment parent-native-use:30-to-80 second-focused-body-support-native-use:30-to-80-to-90 Identify-grade:1 cap:90 no-dummy-spell no-refill terminal-immutable separate-process-reload craft-command:NOT_RUN");
		return 0;
	}

	private static NativeRuntime CreateSupportFocusedBody(TestDatabase database, NativeRuntime native, FixtureIds second,
		IMagicCapability capability, IMagicCapability other)
	{
		var actor = native.Actor; long instanceId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var instance = new MudSharp.Models.CharacterInstance { Id = actor.InstanceId + 100000, CharacterId = actor.Id, BodyId = second.BodyId,
				EmbodiedBodyId = second.BodyId, InstanceName = "ARM Support Focused Body", InstanceKind = (int)CharacterInstanceKind.PhysicalClone,
				ControlPolicy = (int)CharacterInstanceControlPolicy.PlayerFocusable, PersistencePolicy = (int)CharacterInstancePersistencePolicy.Persistent,
				IsEmbodied = true, IsControllable = true, LocationId = second.CellId, State = (int)CharacterState.Awake,
				PositionId = (int)MudSharp.Body.Position.PositionStates.PositionStanding.Instance.Id, PositionTargetType = "", PositionEmote = "",
				CreatedBySourceKey = "ARM support isolated acceptance", CreatedDateTime = DateTime.UtcNow, EffectData = "<Effects/>" };
			db.CharacterInstances.Add(instance); db.SaveChanges(); instanceId = instance.Id;
		}
		var secondary = NativeRuntime.Load(second with { CharacterId = actor.Id, ResourceId = native.Resource.Id, CapabilityId = capability.Id }, database.ConnectionString, true);
		ConfigureCastingWorld(secondary, database.ConnectionString, false);
		SetPrivateMember(secondary.Actor, "Gameworld", native.World); SetPrivateMember(secondary.Body, "Gameworld", native.World);
		secondary.Actor.HarnessIdentity = actor; SetPrivateField(secondary.Actor, "_instanceId", instanceId);
		SetPrivateField(secondary.Actor, "_characterTraits", new List<ITrait>());
		SetPrivateField(actor, "_secondaryInstances", new List<ICharacterInstance> { secondary.Actor }); SetPrivateField(actor, "_focusedInstance", secondary.Actor);
		secondary.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(capability), NativeRuntime.NewCapabilityMerit(other)]);
		return secondary;
	}

	private static MagicSpell NewSupportFixtureSpell(NativeRuntime native, string name, ITraitDefinition trait)
	{
		var spell = new MagicSpell(name, native.Capability.School); ((All<IMagicSpell>)native.World.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new self", $"trait {trait.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration",
			$"cost {native.Resource.Id} ARM02 Cost", "prog rejuvenation_known", "castemote A shell forms.", "failcastemote The shell crumbles.",
			"effect add spellarmour", "effect 1 absorb grade*10+variable", "grades fixture" })
			Require(spell.BuildingCommand(native.Actor, new StringStack(command)), "Support fixture spell refused: " + command);
		return spell;
	}

	private static void VerifySupportProgressionReload(TestDatabase database, CastingReader input)
	{
		var native = NativeRuntime.Load(input.Earth, database.ConnectionString, true); ConfigureCastingWorld(native, database.ConnectionString, false);
		var world = native.World; var actor = native.Actor; var cap = (IMagicCastingCapability)CastingRequired(world.MagicCapabilities.Get(input.EarthCapability));
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		var service = new MagicCastingService(world, flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var store = new MagicCastingStateStore(); var grant = cap.CastingPolicy!.Supports.Single();
		Require(store.SupportGrant(actor.Id, cap.CastingPolicy.Identity, grant.Key)?.RawSkillCap == 90, "Restart lost scoped native support authorisation.");
		Require(actor.GetTrait(world.Traits.Get(input.SupportTrait!.Value)) is Skill && actor.TraitRawValue(world.Traits.Get(input.SupportTrait.Value)) == 90,
			"Restart lost native support value.");
		Require(service.RawSkillImprovementCap(actor, input.SupportTrait.Value) == 90 && service.Acquisition(actor, input.IdentifySpell!.Value)?.ControlledGrade == 1,
			"Restart lost support cap or Identify grade.");
		Require(store.Unresolved(actor.Id).Count == 0 && actor.MagicResourceAmounts[native.Resource] == 17, "Restart mutated support/resource state.");
		Console.WriteLine("ARM-SUPPORT-reload=passed native-Skill raw:90 cap:90 Identify-grade:1 balance:17 scoped-grant terminal-history");
	}
}
