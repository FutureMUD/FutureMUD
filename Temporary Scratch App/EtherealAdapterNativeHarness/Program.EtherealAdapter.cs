#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	internal static int EtherealAdapterMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--ethereal-adapter-run" or "--ethereal-adapter-reader")) return WaterBreathingStockMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--ethereal-adapter-reader" ? ReadEtherealAdapter(args[1]) : RunEtherealAdapter();
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private const string EtherealAdapterGroup = "armageddon.detect_ethereal";
	private sealed record EtherealAdapterReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		DateTime Expiry, Guid ParentIdentity, Guid Operation, double Balance);

	private static void VerifyNativeEtherealChannels(NativeRuntime native, bool granted)
	{
		var expected = PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal;
		Require((native.Actor.GetPerception(PerceptionTypes.None) & expected) == (granted ? expected : PerceptionTypes.None),
			"Native character ethereal perception channels disagree with retained grant");
	}

	private static int RunEtherealAdapter()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString); Console.WriteLine("ETHEREAL-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "ethereal_adapter_lane", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var expressions = (All<ITraitExpression>)world.TraitExpressions; var spells = (All<IMagicSpell>)world.MagicSpells;
		var progs = (All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => spells.Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		PierceRealResource(native, database.ConnectionString);
		var attribute = world.Traits.GetByName("ARM02 Agility")!;
		Require(actor.AddTrait(attribute, 19), "Ethereal native capacity attribute");
		MudSharp.Models.TraitExpression capacity; MudSharp.Models.TraitExpression duration; MudSharp.Models.TraitExpression cost;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			capacity = new() { Name = "Ethereal adapter capacity", Expression = "80+2*variable" };
			duration = new() { Name = "Ethereal adapter duration", Expression = "1800*grade" };
			cost = new() { Name = "Ethereal adapter minimum", Expression = "7" };
			db.TraitExpressions.AddRange(capacity, duration, cost); db.SaveChanges();
		}
		foreach (var model in new[] { capacity, duration, cost }) expressions.Add(new TraitExpression(model, world));
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Actual editable resource capacity");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack($"\"Ethereal Adapter Fixture\" {cap.School.Id}"));
		var spell = (MagicSpell)spells.Single(x => x.Name == "Ethereal Adapter Fixture");
		foreach (var edit in new[] { "trigger new self", "effect add detectethereal", $"effect 1 lifetime accumulate {EtherealAdapterGroup} 600 36",
			$"trait {skill.Id}", "difficulty easy", $"duration {duration.Id}", $"cost {native.Resource.Id} {cost.Id}",
			"castemote $0 invokes an ethereal perception enchantment.", "failcastemote $0 falters.", "targetemote $0 surrounds $1 with an enchantment.",
			"grades fixture", "grades efficiency source 7 1" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary ethereal adapter builder: " + edit);
		if (!spell.AppliedEffectsAreExclusive)
			Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Enable exclusive replacement when absent");
		Require(spell.ReadyForGame && spell.StockIdentity is null, "Editable adapter readiness; this is not source stock acceptance");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()),
				"Editable ethereal template database reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Ethereal adapter capability: " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); actor.SetTraitValue(skill, 90);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		Action<string>? callback = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			checkpoint: stage => callback?.Invoke(stage), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "Ethereal adapter native fixture").Allowed, "Adapter enrolment");
		var state = new MagicCastingStateStore(); state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == EtherealAdapterGroup);
		MagicCastingResult Cast(int grade, bool overreach = false, bool applied = true)
		{
			Ready(); var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			var result = casting.Cast(intent);
			Require(result.Status == MagicCastingStatus.Succeeded, "Actual paid ethereal adapter cast: " + result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
				(overreach || before - actor.MagicResourceAmounts[native.Resource] == (grade == 1 ? 7 : 17.5)), "Quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied,
				"Ethereal native application receipt");
			VerifyNativeEtherealChannels(native, true); return result;
		}
		Cast(1); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(1800), "Grade1 three-hour increment");
		var old = Parent(); Cast(7);
		Require(Parent().LifetimeState!.Grade == 7 && scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(14400) &&
			!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old), "Native high-grade accumulation/cleanup");
		Cast(7); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(21600), "Native cap36");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		var unchanged = Cast(2, true, false);
		Require(casting.Acquisition(actor, spell.Id)!.ControlledGrade == 1 &&
			XElement.Parse(state.Operation(unchanged.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"No-change paid overreach sampled mastery");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		Ready(); old = Parent(); var balance = actor.MagicResourceAmounts[native.Resource];
		callback = stage => { if (stage == "BeforePayment") scheduler.Reschedule(old, TimeSpan.FromSeconds(6000)); };
		var refused = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null &&
			actor.MagicResourceAmounts[native.Resource] == balance && actor.Effects.Contains(old), "Prepayment cohort drift charged or replaced");
		Ready(); balance = actor.MagicResourceAmounts[native.Resource];
		callback = stage => { if (stage == "Committed") scheduler.Reschedule(old, TimeSpan.FromSeconds(6600)); };
		var uncertain = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
		Require(uncertain.Status == MagicCastingStatus.NeedsReview && balance - actor.MagicResourceAmounts[native.Resource] == 7 &&
			actor.Effects.Contains(old) && Parent() == old, "Paid cohort uncertainty guessed replacement or refunded");
		var final = Cast(7); var saved = Parent(); FlushCasting(native);
		RunItemReaderProcess(new EtherealAdapterReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
			scheduler.ScheduledExpiry(saved)!.Value, saved.Identity, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource]),
			"--ethereal-adapter-reader");
		Console.WriteLine("ETHEREAL-adapter=passed ordinary-builder reload paid-grade1-grade7 min7 cumulative-cap36 strongest-grade nochange-no-mastery cohort-drift fresh-reader expiry; source-stock-components-and-environment-unqualified");
		return 0;
	}

	private static int ReadEtherealAdapter(string encoded)
	{
		var input = JsonSerializer.Deserialize<EtherealAdapterReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString); var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock); var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor; var scheduler = new EffectScheduler(native.World, clock);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler); PierceRealResource(native, database.ConnectionString);
		SpellDetectEtherealEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString)) actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.Identity == input.ParentIdentity && parent.LifetimeState == new MagicSpellLifetimeState(new(EtherealAdapterGroup, 600, 36), 7) &&
			scheduler.ScheduledExpiry(parent) == input.Expiry && parent.SpellEffects.Single() is SpellDetectEtherealEffect &&
			actor.Effects.Contains(parent.SpellEffects.Single()), "Fresh ethereal parent/child/policy/grade/identity/deadline");
		VerifyNativeEtherealChannels(native, true);
		var operation = new MagicCastingStateStore().Operation(input.Operation)!;
		Require(operation.Stage == "Completed" && (bool)XElement.Parse(operation.Definition).Attribute("applied")!, "Persisted application report");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow,
			random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, 7, false, "me", OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed or refunded");
		clock.Advance(input.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) && !actor.EffectsOfType<SpellDetectEtherealEffect>().Any() &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance, "Expiry orphaned ethereal parent/child or refunded");
		VerifyNativeEtherealChannels(native, false);
		Console.WriteLine("ETHEREAL-reader=passed fresh-process native-typed-child identity grade7 policy exact-deadline no-replay no-refund expiry-removes-parent-child-perception");
		return 0;
	}
}
