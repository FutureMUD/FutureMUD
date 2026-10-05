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
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Health.Breathing;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	internal static int WaterBreathingAdapterMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--water-breathing-adapter-run" or "--water-breathing-adapter-reader")) return PierceStockMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--water-breathing-adapter-reader" ? ReadWaterBreathingAdapter(args[1]) : RunWaterBreathingAdapter();
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private const string WaterAdapterGroup = "armageddon.water_breathing";
	private sealed record WaterAdapterReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		long Water, long Unmapped, DateTime Expiry, Guid ParentIdentity, Guid Operation, double Balance);
	private sealed class WaterAdapterRandom : Random
	{
		public int Draws; public int Value;
		public override int Next(int minValue, int maxValue) { Draws++; return Math.Clamp(Value, minValue, maxValue - 1); }
	}

	private static void VerifyWaterAdapterBreathing(NativeRuntime native, ILiquid water, ILiquid unmapped, bool granted)
	{
		var actor = native.Actor; var body = native.Body;
		var race = Mock.Get(body.Race); var cell = Mock.Get(actor.Location); var terrain = Mock.Get(actor.Location.CurrentOverlay.Terrain);
		cell.Setup(x => x.Terrain(actor)).Returns(terrain.Object);
		var strategy = new PartlessBreather(); race.SetupGet(x => x.BreathingStrategy).Returns(strategy);
		// The reused body fixture has already logged in with NonBreather. Select the
		// authored test strategy in its native cache without restarting unrelated health ticks.
		SetPrivateField(body, "_breathingStrategy", strategy);
		race.Setup(x => x.CanBreatheFluid(It.IsAny<IFluid>())).Returns((false, 0.0));
		cell.Setup(x => x.IsUnderwaterLayer(It.IsAny<RoomLayer>())).Returns(true);
		terrain.SetupGet(x => x.WaterFluid).Returns(water);
		Require(body.BreathingStrategy.CanBreathe(body) == granted, "Actual native body/character scoped water compatibility");
		terrain.SetupGet(x => x.WaterFluid).Returns(unmapped);
		Require(!body.BreathingStrategy.CanBreathe(body), "Unmapped native liquid became magically breathable");
		race.Setup(x => x.CanBreatheFluid(unmapped)).Returns((true, 1.0));
		Require(body.BreathingStrategy.CanBreathe(body), "Native racial compatibility was overwritten");
		race.Setup(x => x.CanBreatheFluid(unmapped)).Returns((false, 0.0));
		terrain.SetupGet(x => x.WaterFluid).Returns(water);
		Console.WriteLine($"WATER-native-breathing=passed actual-body-character-combined-effects partless mapped-grant:{granted} unmapped-refused racial-compatibility-preserved");
	}

	private static int RunWaterBreathingAdapter()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString); Console.WriteLine("WATER-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "water_breathing_adapter_lane", true);
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
		var capacityAttribute = world.Traits.GetByName("ARM02 Agility")!;
		Require(actor.AddTrait(capacityAttribute, 19), "Adapter native capacity attribute");
		MudSharp.Models.TraitExpression capacityRow;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			capacityRow = new() { Name = "Water adapter native capacity", Expression = "80+2*variable" };
			db.TraitExpressions.Add(capacityRow); db.SaveChanges();
		}
		expressions.Add(new TraitExpression(capacityRow, world));
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {capacityAttribute.Id} {capacityRow.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Normal editable native resource capacity");
		var water = world.Liquids.GetByName("ARM03C2 water")!; var unmapped = world.Liquids.GetByName("ARM03C2 oil")!;
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		MudSharp.Models.TraitExpression durationRow; MudSharp.Models.TraitExpression costRow;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			durationRow = new() { Name = "Water adapter duration placeholder", Expression = "0" };
			costRow = new() { Name = "Water adapter minimum energy", Expression = "20" };
			db.TraitExpressions.AddRange(durationRow, costRow); db.SaveChanges();
		}
		expressions.Add(new TraitExpression(durationRow, world)); expressions.Add(new TraitExpression(costRow, world));
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack($"\"Water Breathing Adapter Fixture\" {cap.School.Id}"));
		var spell = (MagicSpell)spells.Single(x => x.Name == "Water Breathing Adapter Fixture");
		foreach (var edit in new[] { "trigger new character", "effect add sourcewaterbreathing", $"effect 1 water add {water.Id}",
			$"trait {skill.Id}", "difficulty easy", $"duration {durationRow.Id}", $"cost {native.Resource.Id} {costRow.Id}",
			"castemote $0 invokes a water breathing enchantment.", "failcastemote $0 falters.", "targetemote $0 surrounds $1 with a water breathing enchantment.",
			"grades fixture", "grades efficiency source 20 1" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary adapter builder: " + edit);
		Require(spell.ReadyForGame && spell.StockIdentity is null, "Ordinary editable adapter readiness (no stock claim)");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()), "Adapter builder/database reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Adapter capability: " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); actor.SetTraitValue(skill, 90);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		Action<string>? callback = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			checkpoint: stage => callback?.Invoke(stage), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "Water adapter native fixture").Allowed, "Adapter enrolment");
		var state = new MagicCastingStateStore(); state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		var random = new WaterAdapterRandom(); using var selectionRandom = Constants.PushRandom(random);
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == WaterAdapterGroup);
		MagicCastingResult Cast(int grade, int draw, bool overreach = false, bool applied = true)
		{
			Ready(); random.Value = draw; var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			random.Draws = 0; var result = casting.Cast(intent);
			Require(result.Status == MagicCastingStatus.Succeeded && random.Draws == 1, "Actual paid source selection/cast: " + result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
				(overreach || before - actor.MagicResourceAmounts[native.Resource] == (grade == 1 ? 20 : 50)), "Native source-minimum quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied, "Native application report");
			Console.WriteLine($"WATER-paid=passed grade:{grade} duration-draw:{draw} draws:1 cost:{before - actor.MagicResourceAmounts[native.Resource]} applied:{applied}"); return result;
		}
		Ready(); var refusedBalance = actor.MagicResourceAmounts[native.Resource];
		var missing = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "missing_water_adapter_target"));
		Require(missing.Status == MagicCastingStatus.Refused && missing.OperationId is null && actor.MagicResourceAmounts[native.Resource] == refusedBalance, "Target refusal paid");
		VerifyWaterAdapterBreathing(native, water, unmapped, false);
		Cast(1, 0); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(600), "Grade1 zero draw clamp");
		VerifyWaterAdapterBreathing(native, water, unmapped, true);
		Cast(7, 21); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(13200) && Parent().LifetimeState!.Grade == 7, "High-grade accumulated source lifetime");
		Cast(7, 21); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(21600), "Source cap36");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		var noChange = Cast(2, 1, true, false);
		Require(casting.Acquisition(actor, spell.Id)!.ControlledGrade == 1 &&
			XElement.Parse(state.Operation(noChange.OperationId!.Value)!.Definition).Attribute("masterySample") is null, "At-cap no-change sampled mastery");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		var old = Parent(); var oldExpiry = scheduler.ScheduledExpiry(old)!.Value;
		foreach (var boundary in new[] { "BeforePayment", "Committed" })
		{
			Ready(); var balance = actor.MagicResourceAmounts[native.Resource]; random.Draws = 0;
			callback = stage => { if (stage == boundary) scheduler.Reschedule(old, TimeSpan.FromSeconds(21000)); };
			var drift = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
			Require(drift.Status == (boundary == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview) &&
				ReferenceEquals(old, Parent()) && random.Draws == 1 &&
				actor.MagicResourceAmounts[native.Resource] == balance - (boundary == "BeforePayment" ? 0 : 20), "Native callback admission/debit drift");
			if (boundary == "BeforePayment") Require(drift.OperationId is null, "Refusal recorded operation");
			else Require(casting.ReconcileOperation(staff, actor, drift.OperationId!.Value, "Owned disposable fixture: retained old cohort inspected").Allowed, "Paid uncertainty fixture reconciliation");
			scheduler.Reschedule(old, oldExpiry - RuntimeClock.UtcNow);
			Console.WriteLine("WATER-callback=passed " + boundary + " exact-cohort no-replacement no-redraw no-refund");
		}
		var balanceBeforeRemoval = actor.MagicResourceAmounts[native.Resource];
		var removal = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='removewaterbreathing'/>"), spell);
		removal.GetOrApplyEffect(actor, actor, OpposedOutcomeDegree.None, SpellPower.Standard, new MagicSpellParent(actor, spell, actor), []);
		Require(!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old) &&
			!actor.EffectsOfType<SpellScopedWaterBreathingEffect>().Any() && actor.MagicResourceAmounts[native.Resource] == balanceBeforeRemoval,
			"Registered generic water-breathing removal leaked scoped child/parent/schedule or refunded");
		Console.WriteLine("WATER-generic-removal=passed existing-removal-adapter scoped-subtype parent-child-schedule-clean no-refund");
		var final = Cast(7, 21); var saved = Parent(); FlushCasting(native);
		RunItemReaderProcess(new WaterAdapterReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id, water.Id, unmapped.Id,
			scheduler.ScheduledExpiry(saved)!.Value, saved.Identity, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource]), "--water-breathing-adapter-reader");
		Console.WriteLine("WATER-adapter-acceptance=passed ordinary-builder database-reload low-high-paid one-source-draw cap36 strongest-grade no-change-no-mastery callback-drift native-breathing fresh-reader expiry");
		return 0;
	}

	private static int ReadWaterBreathingAdapter(string encoded)
	{
		var input = JsonSerializer.Deserialize<WaterAdapterReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString); var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock); var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor; var scheduler = new EffectScheduler(native.World, clock);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler); PierceRealResource(native, database.ConnectionString);
		SpellScopedWaterBreathingEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// The small shared fixture loads spells before its optional liquid catalogue.
			// Reload this authored adapter after the real liquids exist, as normal world boot does.
			var spells = (All<IMagicSpell>)native.World.MagicSpells; spells.Remove(spells.Get(input.Spell));
			var spell = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == input.Spell), native.World); spells.Add(spell);
			Require(spell.ReadyForGame && spell.SpellEffects.Single() is SourceWaterBreathingEffect, "Fresh editable template after native liquid catalogue load");
			actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		}
		typeof(MudSharp.Framework.PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.Identity == input.ParentIdentity && parent.LifetimeState == new MagicSpellLifetimeState(new(WaterAdapterGroup, 600, 36), 7) &&
			scheduler.ScheduledExpiry(parent) == input.Expiry && parent.SpellEffects.Count() == 1, "Fresh process policy/grade/identity/child/deadline reload");
		var water = native.World.Liquids.Get(input.Water)!; var unmapped = native.World.Liquids.Get(input.Unmapped)!;
		VerifyWaterAdapterBreathing(native, water, unmapped, true);
		var state = new MagicCastingStateStore(); var operation = state.Operation(input.Operation)!;
		Require(operation.Stage == "Completed" && (bool)XElement.Parse(operation.Definition).Attribute("applied")!, "Persisted application receipt");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow, random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, 7, false, "me", OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed or refunded");
		clock.Advance(input.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) && !actor.EffectsOfType<SpellScopedWaterBreathingEffect>().Any() &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance, "Expiry orphaned grant or refunded energy");
		VerifyWaterAdapterBreathing(native, water, unmapped, false);
		Console.WriteLine("WATER-reader=passed fresh-process native-liquid-identities policy-grade7 exact-deadline no-replay no-refund expiry-removes-parent-child-grant"); return 0;
	}
}
