#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Position.PositionStates;
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
	internal static int WaterBreathingStockMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--water-breathing-stock-run" or "--water-breathing-stock-reader")) return WaterBreathingAdapterMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--water-breathing-stock-reader" ? ReadWaterBreathingStock(args[1]) : RunWaterBreathingStock();
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private const string WaterStockGroup = "armageddon.water_breathing";
	private sealed record WaterStockReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		long Water, long Unmapped, DateTime Expiry, Guid ParentIdentity, Guid Operation, double Balance);
	private sealed class WaterStockRandom : Random
	{
		public int Draws; public int Value;
		public override int Next(int minValue, int maxValue) { Draws++; return Math.Clamp(Value, minValue, maxValue - 1); }
	}

	private static void VerifyWaterStockBreathing(NativeRuntime native, ILiquid water, ILiquid unmapped, bool granted)
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
		Console.WriteLine($"WATER-STOCK-native-breathing=passed actual-body-character-combined-effects partless mapped-grant:{granted} unmapped-refused racial-compatibility-preserved");
	}

	private static int RunWaterBreathingStock()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString); Console.WriteLine("WATER-STOCK-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "water_breathing_stock_lane", true);
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
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => expressions.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => spells.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
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
		actor.PositionState = PositionStanding.Instance;
		var stockCommand = $"stock water-breathing {cap.School.Id} {skill.Id} {native.Resource.Id} {water.Id}";
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(stockCommand));
		var spell = (MagicSpell)spells.Single(x => x.Name == ArmageddonWaterBreathingStock.Name);
		Require(spell.ReadyForGame && spell.StockIdentity == ArmageddonWaterBreathingStock.Key &&
			!spell.ScrollInscriptionAllowed && spell.EffectDurationExpression.OriginalFormulaText == "0" &&
			spell.GradeProfile!.OpeningSkill == 30 && spell.GradeProfile.Efficiency!.MinimumCost == 20,
			"Normal builder-created source Water Breathing stock");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var stockRows = db.MagicSpells.Count(); var supportRows = db.FutureProgs.Count(); var expressionRows = db.TraitExpressions.Count();
			foreach (var invalid in new[] { stockCommand, $"stock water-breathing {cap.School.Id} {skill.Id} {native.Resource.Id}",
				stockCommand + " " + water.Id, stockCommand + " missing_water_stock_liquid" })
				EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(invalid));
			using var after = NewIndependentContext(database.ConnectionString);
			Require(after.MagicSpells.Count() == stockRows && after.FutureProgs.Count() == supportRows &&
				after.TraitExpressions.Count() == expressionRows, "Duplicate/malformed native stock construction leaked rows");
		}
		foreach (var edit in new[] { "difficulty easy", "grades practice difficulty easy", $"effect 1 water remove {water.Id}",
			$"effect 1 water add {water.Id}", "effect 1 lifetime armageddon.water_breathing 600 36" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary stock builder: " + edit);
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && reload.StockIdentity == ArmageddonWaterBreathingStock.Key &&
				reload.GradeProfile!.Practice!.Difficulty == Difficulty.Easy &&
				XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()), "Stock builder/database reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		var wineSkill = world.Traits.GetByName("ARM02 Sorcerer Proficiency")!;
		var wine = ArmageddonDrawWineStock.Create(world, cap.School, wineSkill, native.Resource, world.Liquids.GetByName("ARM03C2 wine")!);
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {wine.Id}", $"casting entry trait {wine.Id} {wineSkill.Id}", $"casting entry skill {wine.Id} 30 90 relative",
			$"casting entry starting {wine.Id} on", $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative",
			$"casting prerequisite add {spell.Id} {wine.Id} 0 80", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Fixture-authored capability/dependency: " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		Action<string>? callback = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			checkpoint: stage => callback?.Invoke(stage), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "Water stock native fixture").Allowed, "Stock enrolment");
		Require(casting.Acquisition(actor, wine.Id) is not null && casting.Acquisition(actor, spell.Id) is null, "Stock acquired before source prerequisite");
		actor.SetTraitValue(wineSkill, 79); casting.NotifyProgress(actor, wineSkill.Id);
		Require(casting.Acquisition(actor, spell.Id) is null, "Draw Wine raw79 wrongly unlocked Water Breathing");
		actor.SetTraitValue(wineSkill, 80); casting.NotifyProgress(actor, wineSkill.Id);
		Require(casting.Acquisition(actor, spell.Id) is { ControlledGrade: 1 } && actor.TraitRawValue(skill) == 30 &&
			casting.RawSkillImprovementCap(actor, skill.Id) == 90, "Normal raw80 acquisition/opening30/cap90");
		actor.SetTraitValue(skill, 90);
		Console.WriteLine("WATER-STOCK-acquisition=passed real-Draw-Wine-content distinct-native-skill raw79-refused raw80-grants opening30 cap90 relative no-invented-grade-gate fixture-policy-only");
		var state = new MagicCastingStateStore(); state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		var random = new WaterStockRandom(); using var selectionRandom = Constants.PushRandom(random);
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == WaterStockGroup);
		MagicCastingResult Cast(int grade, int draw, bool overreach = false, bool applied = true)
		{
			Ready(); random.Value = draw; var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			random.Draws = 0; var result = casting.Cast(intent);
			Require(result.Status == MagicCastingStatus.Succeeded && random.Draws == 1, "Actual paid source selection/cast: " + result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
				(overreach || before - actor.MagicResourceAmounts[native.Resource] == (grade == 1 ? 20 : 50)), "Native source-minimum quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied, "Native application report");
			Console.WriteLine($"WATER-STOCK-paid=passed grade:{grade} duration-draw:{draw} draws:1 cost:{before - actor.MagicResourceAmounts[native.Resource]} applied:{applied}"); return result;
		}
		void Refuse(MagicSpell value, string target, string reason)
		{
			Ready(); var balance = actor.MagicResourceAmounts[native.Resource]; random.Draws = 0;
			using var before = NewIndependentContext(database.ConnectionString); var operations = before.MagicCastingOperations.Count();
			var result = casting.Cast(new(actor, cap.Id, value.Id, 1, false, target));
			using var after = NewIndependentContext(database.ConnectionString);
			Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null &&
				actor.MagicResourceAmounts[native.Resource] == balance && after.MagicCastingOperations.Count() == operations && random.Draws == 0,
				"Stock refusal paid, drew duration or recorded an operation: " + reason + " " + result.Message);
			Console.WriteLine("WATER-STOCK-refusal=passed " + reason + " no-payment no-operation no-source-draw");
		}
		Refuse(spell, "missing_water_stock_target", "missing character");
		actor.PositionState = PositionSitting.Instance; Refuse(spell, "me", "source Standing caster policy");
		actor.PositionState = PositionStanding.Instance;
		Require(spell.BuildingCommand(actor, new StringStack($"effect 1 water remove {water.Id}")), "Editable scope removal");
		Refuse(spell, "me", "unconfigured native water scope");
		Require(spell.BuildingCommand(actor, new StringStack($"effect 1 water add {water.Id}")), "Editable scope restoration");
		VerifyLiveStockTargetPolicy(native, database.ConnectionString, spell, "me", Refuse);
		var terrain = Mock.Get(actor.Location.CurrentOverlay.Terrain);
		foreach (var name in new[] { "Nilaz", "Silt", "Fire Plane", "Desert" })
		{
			terrain.SetupGet(x => x.Name).Returns(name);
			var quote = casting.Quote(new(actor, cap.Id, spell.Id, 1, false, "me"));
			Require(quote.Allowed, "Stock invented terrain-name prohibition: " + name + " " + quote.Reason);
		}
		Console.WriteLine("WATER-STOCK-terrain=passed no-authored-Nilaz-Silt-Fire-underwater-only-filter native-reach-policy-preserved");
		VerifyWaterStockBreathing(native, water, unmapped, false);
		Cast(1, 0); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(600), "Grade1 zero draw clamp");
		VerifyWaterStockBreathing(native, water, unmapped, true);
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
			Console.WriteLine("WATER-STOCK-callback=passed " + boundary + " exact-cohort no-replacement no-redraw no-refund");
		}
		var balanceBeforeRemoval = actor.MagicResourceAmounts[native.Resource];
		var removal = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='removewaterbreathing'/>"), spell);
		removal.GetOrApplyEffect(actor, actor, OpposedOutcomeDegree.None, SpellPower.Standard, new MagicSpellParent(actor, spell, actor), []);
		Require(!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old) &&
			!actor.EffectsOfType<SpellScopedWaterBreathingEffect>().Any() && actor.MagicResourceAmounts[native.Resource] == balanceBeforeRemoval,
			"Registered generic water-breathing removal leaked scoped child/parent/schedule or refunded");
		Console.WriteLine("WATER-STOCK-generic-removal=passed existing-removal-adapter scoped-subtype parent-child-schedule-clean no-refund");
		// Exercise this content's cap-relative progression through the existing paid practice action.
		var improver = ConfigurePracticeImprovement(native, database.ConnectionString, true);
		Require(improver.BuildingCommand(actor, new StringStack("interval 20")), "Native practice improvement interval");
		using (var db = NewIndependentContext(database.ConnectionString))
			SetPrivateField(native.Body, "_traits", db.Traits.AsNoTracking().Where(x => x.BodyId == native.Body.Id).ToArray()
				.Select(x => CastingRequired(world.Traits.Get(x.TraitDefinitionId)).LoadTrait(x, native.Body)).ToList());
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {capacityAttribute.Id} {capacityRow.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Rebound native practice resource capacity");
		skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		actor.SetTraitValue(skill, 30);
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		for (var grade = 1; grade <= 7; grade++)
		{
			clock.Advance(TimeSpan.FromMinutes(11)); Ready();
			var balance = actor.MagicResourceAmounts[native.Resource]; var parents = actor.EffectsOfType<MagicSpellParent>().Count();
			var quote = casting.Quote(new(actor, cap.Id, spell.Id, grade, grade > 1, "", MagicCastingMode.Practice));
			Require(quote.Allowed, "Water stock practice quote: " + quote.Reason);
			MudSharp.Commands.Modules.MagicModule.MagicGeneric(actor,
				$"{cap.School.SchoolVerb} practice \"{spell.Name}\" grade {grade}{(grade > 1 ? " overreach" : "")} via {cap.Id}");
			var action = actor.EffectsOfType<MagicPracticeAction>().Single();
			Require(actor.MagicResourceAmounts[native.Resource] == balance - quote.Invocation!.Costs.Single().Amount, "Native paid practice start");
			clock.Advance(TimeSpan.FromSeconds(30)); action.ExpireEffect(); action.ExpireEffect();
			Require(state.Operation(action.OperationId)!.Stage == "Completed" && actor.TraitRawValue(skill) == Math.Min(90, 30 + grade * 10) &&
				casting.Acquisition(actor, spell.Id)!.ControlledGrade == grade && actor.EffectsOfType<MagicSpellParent>().Count() == parents,
				$"Water cap90 effect-free practice at grade {grade}: raw {actor.TraitRawValue(skill)}, controlled {casting.Acquisition(actor, spell.Id)!.ControlledGrade}");
			Console.WriteLine($"WATER-STOCK-practice=passed grade:{grade} raw:{actor.TraitRawValue(skill)} cap:90 native-skill paid-once effect-free accelerated-clock");
		}
		var final = Cast(7, 21); var saved = Parent(); FlushCasting(native);
		RunItemReaderProcess(new WaterStockReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id, water.Id, unmapped.Id,
			scheduler.ScheduledExpiry(saved)!.Value, saved.Identity, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource]), "--water-breathing-stock-reader");
		Console.WriteLine("WATER-STOCK-acceptance=passed ordinary-stock-builder database-reload raw80-acquisition opening30 cap90 grade7-practice Standing-filter low-high-paid one-source-draw cap36 strongest-grade no-change-no-mastery callback-drift native-breathing fresh-reader expiry");
		return 0;
	}

	private static int ReadWaterBreathingStock(string encoded)
	{
		var input = JsonSerializer.Deserialize<WaterStockReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
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
			Require(spell.ReadyForGame && spell.StockIdentity == ArmageddonWaterBreathingStock.Key &&
				spell.GradeProfile!.OpeningSkill == 30 && spell.GradeProfile.Efficiency!.MinimumCost == 20 &&
				spell.GradeProfile.Practice!.Difficulty == Difficulty.Easy && spell.SpellEffects.Single() is SourceWaterBreathingEffect,
				"Fresh editable stock after native liquid catalogue load");
			actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		}
		typeof(MudSharp.Framework.PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.Identity == input.ParentIdentity && parent.LifetimeState == new MagicSpellLifetimeState(new(WaterStockGroup, 600, 36), 7) &&
			scheduler.ScheduledExpiry(parent) == input.Expiry && parent.SpellEffects.Count() == 1, "Fresh process policy/grade/identity/child/deadline reload");
		var water = native.World.Liquids.Get(input.Water)!; var unmapped = native.World.Liquids.Get(input.Unmapped)!;
		VerifyWaterStockBreathing(native, water, unmapped, true);
		var state = new MagicCastingStateStore(); var operation = state.Operation(input.Operation)!;
		Require(operation.Stage == "Completed" && (bool)XElement.Parse(operation.Definition).Attribute("applied")! &&
			state.Acquisition(actor.Id, input.Spell) is { ControlledGrade: 7 } &&
			actor.TraitRawValue(((MagicSpell)parent.Spell).CastingTrait) == 90, "Persisted application/acquisition/raw-cap receipt");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow, random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, 7, false, "me", OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed or refunded");
		clock.Advance(input.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) && !actor.EffectsOfType<SpellScopedWaterBreathingEffect>().Any() &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance, "Expiry orphaned grant or refunded energy");
		VerifyWaterStockBreathing(native, water, unmapped, false);
		Console.WriteLine("WATER-STOCK-reader=passed fresh-process native-liquid-identities policy-grade7 exact-deadline no-replay no-refund expiry-removes-parent-child-grant"); return 0;
	}
}
