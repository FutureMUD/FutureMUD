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
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	internal static int PierceStockMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--pierce-stock-run" or "--pierce-stock-reader" or "--pierce-shared-runtime-run")) return ProvisionStockMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--pierce-stock-reader" ? ReadPierceStock(args[1]) : RunPierceStock(args[0] == "--pierce-shared-runtime-run");
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private sealed record PierceReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		DateTime Expiry, Guid Operation, double Balance, long SelfCasterSpell = 0, DateTime? SelfCasterExpiry = null);

	private static void PierceRealResource(NativeRuntime native, string connection)
	{
		using var db = NewIndependentContext(connection);
		var old = native.Resource;
		var resource = new SimpleMagicResource(db.MagicResources.AsNoTracking().Single(x => x.Id == old.Id), native.World);
		var amounts = (DoubleCounter<IMagicResource>)native.Actor.MagicResourceAmounts;
		var balance = amounts[old]; amounts.Remove(old); amounts[resource] = balance;
		var catalogue = (All<IMagicResource>)native.World.MagicResources;
		catalogue.Remove(old); catalogue.Add(resource); SetPrivateMember(native, "Resource", resource);
	}

	private static int RunPierceStock(bool sharedRuntimeControls = false)
	{
		using var globals = new ConsumableGlobals();
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "pierce_stock_lane", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id);
		SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var world = native.World; var actor = native.Actor;
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var expressions = (All<ITraitExpression>)world.TraitExpressions;
		var spells = (All<IMagicSpell>)world.MagicSpells;
		var progs = (All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => expressions.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => spells.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		PierceRealResource(native, database.ConnectionString);
		var attribute = world.Traits.GetByName("ARM02 Agility")!;
		Require(actor.AddTrait(attribute, 19), "Pierce capacity attribute");
		MudSharp.Models.TraitExpression capacity;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			capacity = new() { Name = "Pierce native capacity", Expression = "80+2*variable" };
			db.TraitExpressions.Add(capacity); db.SaveChanges();
		}
		expressions.Add(new TraitExpression(capacity, world));
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Actual SimpleMagicResource authored capacity");
		var cap = (SkillLevelBasedMagicCapability)native.Capability;
		var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var command = $"stock pierce-concealment {cap.School.Id} {skill.Id} {native.Resource.Id}";
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(command));
		var spell = (MagicSpell)spells.Single(x => x.Name == ArmageddonPierceConcealmentStock.Name);
		Require(spell.ReadyForGame && spell.StockIdentity == ArmageddonPierceConcealmentStock.Key, "Normal Pierce stock builder");
		var count = spells.Count;
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(command));
		Require(spells.Count == count, "Duplicate stock construction added rows");
		Require(spell.BuildingCommand(actor, new StringStack("grades practice difficulty easy")), "Pierce ordinary builder edit");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && reload.GradeProfile!.Practice!.Difficulty == Difficulty.Easy &&
				reload.EffectDurationExpression.OriginalFormulaText == "3000*grade", "Editable stock reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Pierce capability edit " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true);
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); actor.SetTraitValue(skill, 90);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "Pierce native stock").Allowed, "Pierce enrolment");
		var state = new MagicCastingStateStore();
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		void Refuse(MagicSpell value, string target, string reason)
		{
			Ready(); var balance = actor.MagicResourceAmounts[native.Resource];
			using var before = NewIndependentContext(database.ConnectionString); var operations = before.MagicCastingOperations.Count();
			var result = casting.Cast(new(actor, cap.Id, value.Id, 1, false, target));
			using var after = NewIndependentContext(database.ConnectionString);
			Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null &&
				actor.MagicResourceAmounts[native.Resource] == balance && after.MagicCastingOperations.Count() == operations,
				"Pierce refusal paid or recorded an operation: " + reason + " " + result.Message);
			Console.WriteLine("PIERCE-refusal=passed " + reason);
		}
		var terrain = Mock.Get(actor.Location.CurrentOverlay.Terrain);
		terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object);
		Mock.Get(actor.Location).SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		Mock.Get(actor.Location).SetupGet(x => x.GetObject).Returns(actor.Location);
		Mock.Get(actor.Location).Setup(x => x.GetProperty("terrain")).Returns(terrain.Object);
		void Terrain(string name) { terrain.SetupGet(x => x.Name).Returns(name); terrain.Setup(x => x.GetProperty("name")).Returns(new TextVariable(name)); }
		Refuse(spell, "missing_pierce_target", "missing target");
		Terrain("Silt"); Refuse(spell, "me", "source Silt before payment"); Terrain("Desert");
		VerifyLiveStockTargetPolicy(native, database.ConnectionString, spell, "me", Refuse);
		MagicCastingResult Cast(int grade, bool overreach = false, bool applied = true)
		{
			Ready(); var before = actor.MagicResourceAmounts[native.Resource];
			var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason);
			var result = casting.Cast(intent);
			Require(result.Status is MagicCastingStatus.Succeeded or MagicCastingStatus.Failed, result.Message);
			var paid = before - actor.MagicResourceAmounts[native.Resource];
			Require(paid == quote.Invocation!.Costs.Single().Amount, "Native quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied, "Native application reporting");
			Require(overreach || paid == (grade == 1 ? 7 : 50), "Source efficiency low/high cost");
			Console.WriteLine($"PIERCE-paid=passed grade:{grade} cost:{paid} applied:{applied} actual-SimpleMagicResource");
			return result;
		}
		var probe = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B C2 vessel").CreateNew(actor);
		world.Add(probe); actor.Location.Insert(probe, true); probe.Login(); world.SaveManager.Flush();
		Require(actor.CanSee(probe), "Native visible item fixture");
		bool CharacterBlindness(string phase)
		{
			var blindness = new SpellBlindnessEffect(actor, new MagicSpellParent(actor, spell, actor), null!);
			actor.AddEffect(blindness);
			try
			{
				var enforced = !actor.CanSee(probe);
				Require(actor.Effects.Contains(blindness), "Detection removed the character-owned blindness effect");
				Require(actor.CanSee(actor) && actor.CanSee(probe, PerceiveIgnoreFlags.IgnoreCanSee), "Blindness changed existing self/ignore exceptions");
				Console.WriteLine($"PIERCE-character-blindness= {(enforced ? "passed" : "FAILED")} phase:{phase} character-owned-native-status body-CanSee-contract");
				return enforced;
			}
			finally { actor.RemoveEffect(blindness, true); }
		}
		var blindnessBeforeDetection = CharacterBlindness("before-detection-visible-item");
		var invisibleParent = new MagicSpellParent(probe, spell, actor);
		var invisible = new SpellInvisibilityEffect(probe, invisibleParent, null!);
		invisibleParent.AddSpellEffect(invisible); probe.AddEffect(invisible); probe.AddEffect(invisibleParent, TimeSpan.FromDays(1));
		Require(!actor.CanSee(probe), "Native invisibility did not conceal item before detection");
		Cast(1);
		var low = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
		Require(actor.CanSee(probe) && low.SpellEffects.Count() == 1 && scheduler.OriginalDuration(low) == TimeSpan.FromSeconds(3000), "Paid low native detection/duration");
		Require((actor.GetPerception(PerceptionTypes.None) & (PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal)) == PerceptionTypes.None,
			"Invisibility detection invented ethereal perception");
		var blindnessAfterDetection = CharacterBlindness("after-detection-invisible-item");
		Require(blindnessBeforeDetection == blindnessAfterDetection, "Detection changed native character-owned blindness enforcement");
		var bodyBlindness = new SpellBlindnessEffect(native.Body, new MagicSpellParent(native.Body, spell, actor), null!);
		native.Body.AddEffect(bodyBlindness);
		Require(!actor.CanSee(probe), "Detection bypassed body-owned native blindness");
		native.Body.RemoveEffect(bodyBlindness, true);
		clock.Advance(TimeSpan.FromSeconds(3001)); scheduler.CheckSchedules();
		Require(!actor.EffectsOfType<SpellDetectInvisibleEffect>().Any() && !actor.CanSee(probe), "Normal low expiry did not remove perception");
		Cast(7); var firstHigh = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
		var configuredCallbacks = 0;
		if (sharedRuntimeControls)
		{
			var observer = new PierceRemovalProbe(actor, firstHigh, () =>
			{
				configuredCallbacks++;
				var newChild = actor.EffectsOfType<SpellDetectInvisibleEffect>().Single(x => !ReferenceEquals(x.ParentEffect, firstHigh));
				Require(newChild.ParentEffect.SpellEffects.Contains(newChild), "Configured cleanup callback removed new ownership");
			});
			firstHigh.AddSpellEffect(observer); actor.AddEffect(observer);
			actor.RemoveEffect(firstHigh.SpellEffects.Single(x => !ReferenceEquals(x, observer)), true);
		}
		clock.Advance(TimeSpan.FromSeconds(10)); Cast(7);
		var refreshed = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
		var refreshQualified = !ReferenceEquals(firstHigh, refreshed) && actor.EffectsOfType<SpellDetectInvisibleEffect>().Count() == 1 &&
			refreshed.SpellEffects.Count() == 1 && scheduler.ScheduledExpiry(refreshed) == RuntimeClock.UtcNow.AddSeconds(28800) &&
			refreshed.LifetimeState?.Grade == 7 && refreshed.LifetimeGroup == ArmageddonPierceConcealmentStock.LifetimeGroup &&
			((IEffectExpiryObserver)scheduler).ScheduledExpiry(firstHigh) is null;
		Console.WriteLine("PIERCE-exclusive-accumulation=" + (refreshQualified ? "passed cap48 strongest-source-grade7" : "FAILED"));
		if (sharedRuntimeControls) Require(refreshQualified && configuredCallbacks == 1 && !firstHigh.SpellEffects.Any(), "Shared runtime repair did not clean configured exclusive refresh/callback");
		// Historical diagnostic continuation remains available in the stock mode.
		if (!refreshQualified) firstHigh.RemovalEffect();
		VerifyPierceLifetimeNative(native, database.ConnectionString, spell, clock, scheduler, Cast, Refuse);
		refreshed = actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == ArmageddonPierceConcealmentStock.LifetimeGroup);
		var selfCasterSpell = sharedRuntimeControls ? VerifyPierceSharedRuntime(native, database.ConnectionString, spell, clock, scheduler, probe) : 0L;
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		var ward = new SpellPersonalWardEffect(actor, new MagicSpellParent(actor, spell, actor), cap.School,
			MagicInterdictionMode.Fail, MagicInterdictionCoverage.Incoming, false, null);
		actor.AddEffect(ward); var warded = Cast(2, true, false); actor.RemoveEffect(ward, true);
		Require(casting.Acquisition(actor, spell.Id)!.ControlledGrade == 1 && actor.EffectsOfType<MagicSpellParent>().Contains(refreshed), "Paid warded cast refreshed or advanced");
		Require(XElement.Parse(state.Operation(warded.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"Warded no-application sampled mastery");
		clock.Advance(spell.GradeProfile!.MasteryInterval + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		Cast(2, true); Require(casting.Acquisition(actor, spell.Id)!.ControlledGrade == 2, "Ordinary paid intended-operation mastery");
		Console.WriteLine("PIERCE-mastery=passed paid-ward-no-application-no-advancement paid-retained-refresh-next-grade");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		if (sharedRuntimeControls)
		{
			actor.RemoveAllEffects<MagicSpellParent>(x => x.Spell.Id == spell.Id, true);
			Require(spell.BuildingCommand(actor, new StringStack("effect 1 lifetime off")), "Normal builder policy off for ordinary nonexclusive control");
			Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Configured nonexclusive toggle");
			Cast(7); var first = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
			clock.Advance(TimeSpan.FromSeconds(1)); Cast(7);
			var second = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id && !ReferenceEquals(x, first));
			Require(actor.EffectsOfType<SpellDetectInvisibleEffect>().Count() == 2 && scheduler.IsScheduled(first) && scheduler.IsScheduled(second), "Configured nonexclusive stacking replaced old child/deadline");
			clock.Advance(TimeSpan.FromSeconds(20999.5)); scheduler.CheckSchedules();
			Require(!actor.Effects.Contains(first) && actor.Effects.Contains(second) && actor.EffectsOfType<SpellDetectInvisibleEffect>().Count() == 1, "Configured first expiry removed live sibling or retained orphan");
			clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
			Require(!actor.EffectsOfType<SpellDetectInvisibleEffect>().Any() && !actor.CanSee(probe), "Configured nonexclusive final expiry retained perception");
			Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Configured exclusive restore");
			Require(spell.BuildingCommand(actor, new StringStack($"effect 1 lifetime accumulate {ArmageddonPierceConcealmentStock.LifetimeGroup} 600 48")), "Normal builder accumulation restore");
			Console.WriteLine("PIERCE-configured-nonexclusive=passed paid-two-parent-child-deadlines first-expiry-preserves-second final-expiry-no-grant");
		}
		actor.RemoveAllEffects<MagicSpellParent>(x => x.LifetimeGroup == ArmageddonPierceConcealmentStock.LifetimeGroup, true);
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		var final = Cast(7); var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
		DateTime? selfCasterExpiry = null;
		if (selfCasterSpell > 0)
		{
			CastPierceSelfCaster(native, selfCasterSpell);
			var pair = actor.EffectsOfType<MagicSpellParent>().Where(x => x.Spell.Id == selfCasterSpell).ToArray();
			Require(pair.Length == 2, "Final paired saved fixture missing phase");
			selfCasterExpiry = ((IEffectExpiryObserver)scheduler).ScheduledExpiry(pair[0]);
			Require(selfCasterExpiry == ((IEffectExpiryObserver)scheduler).ScheduledExpiry(pair[1]), "Final paired fixture deadline mismatch");
		}
		probe.Delete(); FlushCasting(native);
		var descriptor = new PierceReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
			((IEffectExpiryObserver)scheduler).ScheduledExpiry(parent)!.Value, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource], selfCasterSpell, selfCasterExpiry);
		RunItemReaderProcess(descriptor, "--pierce-stock-reader");
		var blindnessQualified = blindnessBeforeDetection && blindnessAfterDetection;
		Console.WriteLine("PIERCE-independent-checks=passed builder-edit-reload source-low-high perception body-blindness ward mastery saved-parent fresh-reader expiry");
		Console.WriteLine("PIERCE-runtime-repairs=" + (blindnessQualified && refreshQualified ? "passed" : "FAILED native blindness or exclusive cleanup"));
		Console.WriteLine("PIERCE-content-clearance=passed historical accumulation cap48 strongest-source-grade normal-editable-policy native-clock-offline-adaptations-recorded");
		if (sharedRuntimeControls) Console.WriteLine("PIERCE-shared-runtime-acceptance=" + (blindnessQualified && refreshQualified ? "passed" : "FAILED") + " configured prepared callback nonexclusive expiry reload applicable-blindness exceptions");
		// Entry acceptance requires both runtime repairs plus all accumulated-lifetime assertions above.
		return blindnessQualified && refreshQualified ? 0 : 1;
	}

	private static int ReadPierceStock(string encoded)
	{
		var input = JsonSerializer.Deserialize<PierceReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals();
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var world = native.World; var actor = native.Actor;
		PierceRealResource(native, database.ConnectionString);
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		SpellDetectInvisibleEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var spell = (MagicSpell)world.MagicSpells.Get(input.Spell)!;
			var filter = (long)spell.Trigger.SaveToXml().Element("TargetFilterProg")!;
			var model = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Single(x => x.Id == filter);
			var prog = new FutureProg(model, world); Require(prog.Compile(), prog.CompileError); ((All<IFutureProg>)world.FutureProgs).Add(prog);
			actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
			Require(spell.ReadyForGame && spell.GradeProfile!.Practice!.Difficulty == Difficulty.Easy, "Fresh stock lost builder edits");
		}
		typeof(MudSharp.Framework.PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.LifetimeState == new MagicSpellLifetimeState(new(ArmageddonPierceConcealmentStock.LifetimeGroup, 600, 48), 7), "Fresh retained lifetime policy/source grade lost");
		Require(actor.EffectsOfType<SpellDetectInvisibleEffect>().Count(x => x.Spell.Id == input.Spell) == 1 && parent.SpellEffects.Count() == 1 &&
			((IEffectExpiryObserver)scheduler).ScheduledExpiry(parent) == input.Expiry, "Fresh process lost exact parent/child/deadline");
		var state = new MagicCastingStateStore(); var operation = state.Operation(input.Operation)!;
		Require(operation.Stage == "Completed" && (bool)XElement.Parse(operation.Definition).Attribute("applied")!, "Fresh operation report");
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, 7, false, "me", OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			((IEffectExpiryObserver)scheduler).ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed, refunded or refreshed");
		if (input.SelfCasterSpell > 0)
		{
			var pair = actor.EffectsOfType<MagicSpellParent>().Where(x => x.Spell.Id == input.SelfCasterSpell).ToArray();
			Require(pair.Length == 2 && pair.All(x => x.SpellEffects.Count() == 1 && ((IEffectExpiryObserver)scheduler).ScheduledExpiry(x) == input.SelfCasterExpiry) &&
				actor.EffectsOfType<SpellDetectInvisibleEffect>().Count(x => x.Spell.Id == input.SelfCasterSpell) == 2, "Fresh paired primary/caster parents, children or deadlines lost");
			clock.Advance(input.SelfCasterExpiry!.Value - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
			Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.SelfCasterSpell) && !actor.EffectsOfType<SpellDetectInvisibleEffect>().Any(x => x.Spell.Id == input.SelfCasterSpell) &&
				actor.Effects.Contains(parent) && ((IEffectExpiryObserver)scheduler).ScheduledExpiry(parent) == input.Expiry && actor.MagicResourceAmounts[native.Resource] == input.Balance,
				"Fresh paired expiry leaked phase, removed independent stock parent or refunded");
			Console.WriteLine("PIERCE-self-caster-reader=passed both-native-phase-parents-children-exact-deadlines restored expiry-removes-both independent-stock-parent-and-reserve-preserved");
		}
		var restarted = VerifyPierceLifetimeRestart(native, parent, operation.CapabilityId, clock, scheduler);
		clock.Advance(restarted.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<SpellDetectInvisibleEffect>().Any() && !actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) &&
			(actor.GetPerception(PerceptionTypes.None) & PerceptionTypes.VisualMagical) == PerceptionTypes.None && actor.MagicResourceAmounts[native.Resource] == restarted.Balance,
			"Restart expiry left detection, parent, grant or changed the paid reserve");
		Console.WriteLine("PIERCE-reader=passed fresh-process policy-source-grade-power reload paid-accumulation quantised-deadline no-replay no-refund expiry-removes-parent-child-perception");
		return 0;
	}
}
