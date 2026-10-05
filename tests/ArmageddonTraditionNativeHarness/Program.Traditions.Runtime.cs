#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Gathering;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void LoadTraditionNative(NativeRuntime native, TestDatabase database, ArmageddonMagicInstallPlan utilityPlan,
		IReadOnlyDictionary<string, long> utilityIds, IReadOnlyDictionary<string, long> ids, int admissions = 3, IReadOnlyDictionary<string, long>? provisionIds = null)
	{
		var world = native.World; using var db = NewIndependentContext(database.ConnectionString);
		var progs = (All<IFutureProg>)world.FutureProgs;
		var progIds = utilityIds.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value)
			.Append(utilityPlan.AlwaysFalseProg).Append(utilityPlan.MendEligibilityProg).Append(db.FutureProgs.Single(x => x.FunctionName == "traditionAlwaysTrue").Id)
			.Concat(provisionIds?.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value) ?? [])
			.Concat(db.FutureProgs.Where(x => x.Subcategory == "Installed Provisions").Select(x => x.Id)).ToArray();
		foreach (var model in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => progIds.Contains(x.Id)))
			if (progs.Get(model.Id) is null) { var prog = new FutureProg(model, world); Require(prog.Compile(), prog.CompileError); progs.Add(prog); }
		var decorators = new All<ITraitValueDecorator>();
		foreach (var model in db.TraitDecorators.AsNoTracking()) decorators.Add(DecoratorBase.GetDecorator(model));
		native.WorldMock.SetupGet(x => x.TraitDecorators).Returns(decorators);
		var models = new All<IImprovementModel>();
		foreach (var model in db.Improvers.AsNoTracking()) models.Add(ImprovementModel.LoadModel(model, world));
		native.WorldMock.SetupGet(x => x.ImprovementModels).Returns(models); native.WorldMock.SetupGet(x => x.LogManager).Returns(Mock.Of<ILogManager>());
		var expressions = new All<ITraitExpression>();
		foreach (var model in db.TraitExpressions.Include(x => x.TraitExpressionParameters).AsNoTracking()) expressions.Add(new TraitExpression(model, world));
		native.WorldMock.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var traits = new All<ITraitDefinition>();
		foreach (var model in db.TraitDefinitions.AsNoTracking().Where(x => x.TraitGroup == "ARM02" || x.TraitGroup == "Armageddon Spell"))
		{
			TraitDefinition definition = model.Type == (int)TraitType.Skill ? new SkillDefinition(model, world) : new AttributeDefinition(model, world);
			definition.Initialise(model); traits.Add(definition);
		}
		native.WorldMock.SetupGet(x => x.Traits).Returns(traits);
		SetPrivateField(native.Actor, "_characterTraits", db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == native.Actor.Id).ToArray()
			.Select(x => CastingRequired(traits.Get(x.TraitDefinitionId)).LoadTrait(new MudSharp.Models.Trait { Value = x.Value, AdditionalValue = x.AdditionalValue }, native.Actor)).ToList());
		var spells = (All<IMagicSpell>)world.MagicSpells;
		foreach (var id in utilityPlan.SpellSkills.Keys.Select(x => utilityIds[x]).Concat(provisionIds?.Where(x => x.Key is ArmageddonReviewedProvisionContent.SustainMealKey or ArmageddonReviewedProvisionContent.DrawWineKey).Select(x => x.Value) ?? []))
		{
			if (spells.Get(id) is { } previous) spells.Remove(previous);
			var spell = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == id), world);
			spells.Add(spell); if (!spell.ReadyForGame) throw new InvalidOperationException(spell.WhyNotReadyForGame(native.Actor));
		}
		var capabilities = (All<IMagicCapability>)world.MagicCapabilities;
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		{
			var id = ids["arm.capability." + variant]; if (capabilities.Get(id) is { } previous) capabilities.Remove(previous);
			var cap = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(db.MagicCapabilities.AsNoTracking().Single(x => x.Id == id), world);
			capabilities.Add(cap); Require(cap.CastingConfigurationErrors().Count == 0, string.Join("\n", cap.CastingConfigurationErrors()));
			Require(cap.GatheringConfigurationErrors().Count == 0, string.Join("\n", cap.GatheringConfigurationErrors()));
			Require(!cap.Regenerators.Any() && cap.CastingPolicy!.Admissions.Count == admissions && !cap.CastingPolicy.PassiveEntitlement, "Loaded policy gained passive regen or unimplemented spells.");
		}
		MagicCapabilityMerit.RegisterMeritInitialiser(); var merits = new All<IMerit>();
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
			merits.Add(MeritFactory.LoadMerit(db.Merits.AsNoTracking().Single(x => x.Id == ids["arm.merit." + variant]), world));
		native.WorldMock.SetupGet(x => x.Merits).Returns(merits); native.Actor.SetMerits(merits);
		Console.WriteLine("ARMTRAD-load=passed actual-installed-native-skills cap-expressions classic-improver three-capabilities three-real-capability-merits no-passive-regenerators");
	}

	private static void VerifyInstalledTraditionProgression(NativeRuntime native, TestDatabase database,
		ArmageddonTraditionInstallPlan plan, IReadOnlyDictionary<string, long> ids, HarnessClock clock)
	{
		var world = native.World; var actor = native.Actor;
		var service = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var hook = new FutureProg(world, "traditionAuthoredEnrol", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap")],
			"return enrolchannelcasting(@actor, @cap, \"selected disposable NPC creation workflow\")");
		Require(hook.Compile(), hook.CompileError);
		var caps = ArmageddonTraditionInstaller.Variants.Select(x => (SkillLevelBasedMagicCapability)CastingRequired(world.MagicCapabilities.Get(ids["arm.capability." + x]))).ToArray();
		Require(service.Acquisition(actor, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.SenseEnchantmentKey]) is null && actor.MagicResourceAmounts[native.Resource] == 0,
			"Definition loading implicitly enrolled, acquired or refilled.");
		foreach (var cap in caps) Require(hook.ExecuteBool(actor, cap), "Authored permanent-merit enrolment refused.");
		var sense = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey + ".skill"]));
		var unravel = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.UnravelEnchantmentKey + ".skill"]));
		var water = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.DrawWaterKey + ".skill"]));
		Require(actor.GetTrait(sense) is Skill && actor.GetTrait(unravel) is Skill && actor.TraitRawValue(sense) == 60 && actor.TraitRawValue(unravel) == 60 &&
			actor.TraitRawValue(world.Traits.Get(plan.SupportSkills["arm.support.gather"])) == 10 && !actor.HasTrait(water) && actor.MagicResourceAmounts[native.Resource] == 0,
			"Source openings, support scope, absent child or initial zero failed.");
		Require(service.Acquisition(actor, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.SenseEnchantmentKey])!.ControlledGrade == 1, "Raw60 became grade6.");
		actor.SetTraitValue(unravel, 79.5); service.NotifyProgress(actor, unravel.Id);
		Require(service.Acquisition(actor, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.DrawWaterKey]) is null, "Child acquired below parent80.");
		Require(((Skill)actor.GetTrait(unravel)).TraitUsed(actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []), "Installed classic native use refused.");
		Require(actor.TraitRawValue(unravel) == 80 && actor.GetTrait(water) is Skill && actor.TraitRawValue(water) == 30 &&
			service.Acquisition(actor, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.DrawWaterKey])!.ControlledGrade == 1, "Native use did not branch at source80/open30/grade1.");
		actor.GetTrait(unravel).Value += 100; Require(actor.TraitRawValue(unravel) == 90, "Source raw90 cap failed.");
		Require(!actor.HasTrait(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.MendFleshKey + ".skill"])) &&
			!actor.HasTrait(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.HoveringLightKey + ".skill"])), "Incomplete parent path was granted.");
		Console.WriteLine("ARMTRAD-progression=passed authored-enrolment actual-merits available-roots-60 grade1 native-use-parent80 child30 cap90 zero-reserve unimplemented-parent-refusal");
		var gathering = new MagicGatheringService(world, clock: clock); native.WorldMock.SetupGet(x => x.MagicGathering).Returns(gathering);
		var begin = gathering.Begin(actor, caps[0], "draw", 2); Require(begin.Success && begin.OperationId.HasValue, begin.Message);
		clock.Advance(TimeSpan.FromSeconds(2)); var completed = gathering.Complete(actor, begin.OperationId!.Value); Require(completed.Success, completed.Message);
		Require(actor.MagicResourceAmounts[native.Resource] == 2 && !actor.MagicResourceGenerators.Any(), "Real gathering did not credit exactly2 or gained passive refill.");
		// Controlled native fixture funding permits one ordinary spell check. Installation and enrolment never supply this balance.
		actor.AddResource(native.Resource, 100); FlushCasting(native); var before = actor.MagicResourceAmounts[native.Resource];
		var quote = service.Quote(new(actor, caps[0].Id, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.SenseEnchantmentKey], 1, false, "me"));
		Require(quote.Allowed, quote.Reason); var cost = quote.Invocation!.Costs.Single().Amount;
		var cast = service.Cast(new(actor, caps[0].Id, plan.ImplementedSpells[ArmageddonReviewedUtilityContent.SenseEnchantmentKey], 1, false, "me"));
		Require(cast.Status == MagicCastingStatus.Succeeded && actor.MagicResourceAmounts[native.Resource] == before - cost, "Installed ordinary direct cast/debit failed: " + cast.Message);
		FlushCasting(native); var players = TraditionPlayers(database); var result = InstallTraditions(database, plan); RequireTraditions(result);
		Require(result.Identities.All(x => ids[x.Key] == x.Value) && players == TraditionPlayers(database), "Active rerun reset acquisition, native skills, balance or merit attachment.");
		var raw = actor.TraitRawValue(unravel); var balance = actor.MagicResourceAmounts[native.Resource];
		foreach (var cap in caps) Require(hook.ExecuteBool(actor, cap), "Repeat enrolment refused.");
		Require(actor.TraitRawValue(unravel) == raw && actor.MagicResourceAmounts[native.Resource] == balance, "Repeat enrolment reopened/refilled.");
		Console.WriteLine($"ARMTRAD-native=passed Self-gathering:2 native-direct-Sense-debit:{cost} no-passive-refill active-rerun-no-player-mutation repeat-enrolment-conserved");
	}
}
