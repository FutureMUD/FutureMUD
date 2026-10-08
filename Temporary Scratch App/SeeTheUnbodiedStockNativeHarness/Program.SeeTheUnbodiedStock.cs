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
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
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
	internal static int SeeTheUnbodiedStockMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--see-unbodied-stock-run" or "--see-unbodied-stock-reader")) return EtherealComponentAdapterMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--see-unbodied-stock-reader" ? ReadSeeTheUnbodiedStock(args[1]) : RunSeeTheUnbodiedStock();
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private const string SeeTheUnbodiedStockGroup = "armageddon.detect_ethereal";
	private sealed record SeeTheUnbodiedStockReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		DateTime Expiry, Guid ParentIdentity, Guid Operation, double Balance, long Component, int Quantity, bool Expire);

	private static void VerifySeeStockChannels(NativeRuntime native, bool granted)
	{
		var expected = PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal;
		Require((native.Actor.GetPerception(PerceptionTypes.None) & expected) == (granted ? expected : PerceptionTypes.None),
			"Native character ethereal perception channels disagree with retained grant");
	}

	private sealed class SeeStockEnvironment(ITerrain outside, ITerrain silt, ITerrain shadow)
	{
		public ITerrain Outside { get; } = outside;
		public ITerrain Silt { get; } = silt;
		public ITerrain Shadow { get; } = shadow;
		public ITerrain Current { get; set; } = outside;
	}

	private static SeeStockEnvironment ConfigureSeeStockWorld(NativeRuntime native, TestDatabase database)
	{
		FinaliseStormTags(database, native.World);
		ITerrain Terrain(long id, string name) => Mock.Of<ITerrain>(x => x.Id == id && x.Name == name &&
			x.TerrainBehaviourString == "land" && x.TerrainLayers == new[] { RoomLayer.GroundLevel } && x.Tags == Array.Empty<ITag>());
		var environment = new SeeStockEnvironment(Terrain(100, "Outside fixture"), Terrain(101, "Silt fixture"), Terrain(102, "Shadow fixture"));
		var terrains = new All<ITerrain>(); foreach (var terrain in new[] { environment.Outside, environment.Silt, environment.Shadow }) terrains.Add(terrain);
		native.WorldMock.SetupGet(x => x.Terrains).Returns(terrains);
		Mock.Get(native.Actor.Location.CurrentOverlay).SetupGet(x => x.Terrain).Returns(() => environment.Current);
		return environment;
	}

	private static void SeedSeeStockComponents(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var stack = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B stack");
		long id = db.GameItemProtos.Max(x => x.Id); MudSharp.Models.Tag? parent = null;
		for (var rank = 0; rank <= 5; ++rank)
		{
			var tag = new MudSharp.Models.Tag { Name = $"Ethereal Divination {rank}", ParentId = parent?.Id };
			db.Tags.Add(tag); db.SaveChanges(); parent = tag;
			if (rank is not (0 or 5)) continue;
			var prototype = new MudSharp.Models.GameItemProto { Id = ++id, Name = $"ARM03B2B Ethereal token {rank}", Keywords = "divination token",
				ShortDescription = "a Divination token", FullDescription = "Declared native rank fixture; source consumption adapts to one quantity unit.",
				MaterialId = stack.MaterialId, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
			foreach (var component in stack.GameItemProtosGameItemComponentProtos) prototype.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.GameItemComponentProtoId });
			prototype.GameItemProtosTags.Add(new() { TagId = tag.Id }); db.GameItemProtos.Add(prototype); db.SaveChanges();
		}
	}

	private static int RunSeeTheUnbodiedStock()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString); Console.WriteLine("SEE-STOCK-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "see_unbodied_stock_lane", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		SeedSeeStockComponents(database);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var environment = ConfigureSeeStockWorld(native, database);
		var ranks = Enumerable.Range(0, 5).Select(x => world.Tags.GetByName($"Ethereal Divination {x}")!).ToArray();
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var expressions = (All<ITraitExpression>)world.TraitExpressions; var spells = (All<IMagicSpell>)world.MagicSpells;
		var progs = (All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => spells.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => expressions.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		PierceRealResource(native, database.ConnectionString);
		var attribute = world.Traits.GetByName("ARM02 Agility")!;
		Require(actor.AddTrait(attribute, 19), "Ethereal native capacity attribute");
		MudSharp.Models.TraitExpression capacity;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			capacity = new() { Name = "Ethereal adapter capacity", Expression = "80+2*variable" };
			db.TraitExpressions.Add(capacity); db.SaveChanges();
		}
		expressions.Add(new TraitExpression(capacity, world));
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Actual editable resource capacity");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		actor.PositionState = PositionStanding.Instance;
		var stockCommand = $"stock see-the-unbodied {cap.School.Id} {skill.Id} {native.Resource.Id} 101 102 {string.Join(' ', ranks.Select(x => x.Id))}";
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(stockCommand));
		var spell = (MagicSpell)spells.Single(x => x.Name == ArmageddonSeeTheUnbodiedStock.Name);
		Require(spell.ReadyForGame && spell.StockIdentity == ArmageddonSeeTheUnbodiedStock.Key &&
			spell.GradeProfile!.OpeningSkill == 30 && spell.GradeProfile.Efficiency!.MinimumCost == 7 &&
			spell.EffectDurationExpression.OriginalFormulaText == "1800*grade" && !spell.ScrollInscriptionAllowed,
			"Normal editable See stock construction");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var rows = db.MagicSpells.Count(); var supports = db.FutureProgs.Count(); var formulas = db.TraitExpressions.Count();
			foreach (var invalid in new[] { stockCommand, stockCommand + " unexpected", $"stock see-the-unbodied {cap.School.Id} {skill.Id} {native.Resource.Id} 101 102" })
				EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(invalid));
			using var after = NewIndependentContext(database.ConnectionString);
			Require(after.MagicSpells.Count() == rows && after.FutureProgs.Count() == supports && after.TraitExpressions.Count() == formulas,
				"Duplicate or malformed stock builder leaked rows");
		}
		foreach (var edit in new[] { "difficulty easy", "grades practice difficulty easy", "effect 1 source component 1 see.stock.divination",
			"effect 1 source 101 102 see.stock.divination" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary See stock builder: " + edit);
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()),
				"Editable ethereal template database reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		var parentSkill = world.Traits.GetByName("ARM02 Sorcerer Proficiency")!;
		var pierce = ArmageddonPierceConcealmentStock.Create(world, cap.School, parentSkill, native.Resource);
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {pierce.Id}", $"casting entry trait {pierce.Id} {parentSkill.Id}", $"casting entry skill {pierce.Id} 30 90 relative",
			$"casting entry starting {pierce.Id} on", $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative",
			$"casting prerequisite add {spell.Id} {pierce.Id} 1 80", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Source See capability/prerequisite: " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		Action<string>? callback = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			checkpoint: stage => callback?.Invoke(stage), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "See source stock native fixture").Allowed, "Stock enrolment");
		Require(casting.Acquisition(actor, pierce.Id) is not null && casting.Acquisition(actor, spell.Id) is null, "See granted before acquired prerequisite");
		actor.SetTraitValue(parentSkill, 79); casting.NotifyProgress(actor, parentSkill.Id);
		Require(casting.Acquisition(actor, spell.Id) is null, "Pierce79 incorrectly unlocked See");
		actor.SetTraitValue(parentSkill, 80); casting.NotifyProgress(actor, parentSkill.Id);
		Require(casting.Acquisition(actor, spell.Id) is { ControlledGrade: 1 } && actor.TraitRawValue(skill) == 30 &&
			casting.RawSkillImprovementCap(actor, skill.Id) == 90, "Pierce80 opens See30/cap90");
		actor.SetTraitValue(skill, 90);
		Console.WriteLine("SEE-STOCK-acquisition=passed real-Pierce-content distinct-skill raw79-refusal raw80-opening30 cap90 relative fixture-capability-policy");
		var state = new MagicCastingStateStore(); state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		GameItem NewComponent(int rank, int quantity)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == $"ARM03B2B Ethereal token {rank}").CreateNew(actor);
			item.GetItemType<IStackable>()!.Quantity = quantity; world.Add(item); actor.Location.Insert(item, true); item.Login();
			native.Body.Get(item, silent: true); Require(ReferenceEquals(item.InInventoryOf, native.Body), "Native component was not carried");
			world.SaveManager.Flush(); return item;
		}
		actor.AddResource(native.Resource, 118); FlushCasting(native);
		var low = NewComponent(0, 3);
		var rejectedBalance = actor.MagicResourceAmounts[native.Resource];
		var rejected = casting.Cast(new(actor, cap.Id, spell.Id, 7, false, "me"));
		Require(rejected.Status == MagicCastingStatus.Refused && rejected.OperationId is null && low.GetItemType<IStackable>()!.Quantity == 3 &&
			actor.MagicResourceAmounts[native.Resource] == rejectedBalance, "Low-rank prepayment conservation");
		native.Body.Drop(low, silent: true);
		var component = NewComponent(5, 40); var stack = component.GetItemType<IStackable>()!;
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == SeeTheUnbodiedStockGroup);
		void Refuse(string target, string reason)
		{
			Ready(); var before = actor.MagicResourceAmounts[native.Resource]; var quantity = stack.Quantity;
			using var db = NewIndependentContext(database.ConnectionString); var operations = db.MagicCastingOperations.Count();
			var result = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, target));
			using var after = NewIndependentContext(database.ConnectionString);
			Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null && actor.MagicResourceAmounts[native.Resource] == before &&
				stack.Quantity == quantity && after.MagicCastingOperations.Count() == operations, "Stock refusal conservation: " + reason);
			Console.WriteLine("SEE-STOCK-refusal=passed " + reason);
		}
		Refuse("missing_see_stock_target", "missing target");
		VerifyLiveStockTargetPolicy(native, database.ConnectionString, spell, "me", (_, target, reason) => Refuse(target, reason));
		actor.PositionState = PositionSitting.Instance; Refuse("me", "minimum Standing"); actor.PositionState = PositionStanding.Instance;
		native.Body.Drop(component, silent: true); Refuse("me", "missing carried component"); native.Body.Get(component, silent: true);
		foreach (var position in new MudSharp.Body.Position.IPositionState[] { PositionStandingAttention.Instance, PositionStandingEasy.Instance,
			PositionSwimming.Instance, PositionFlying.Instance, PositionRiding.Instance })
		{
			actor.PositionState = position;
			Require(casting.Quote(new(actor, cap.Id, spell.Id, 1, false, "me")).Allowed, "Admitted mapped active/standing posture");
		}
		actor.PositionState = PositionStanding.Instance;
		MagicCastingResult Cast(int grade, bool overreach = false, bool applied = true)
		{
			Ready(); var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			var quantity = stack.Quantity; var result = casting.Cast(intent);
			Require(stack.Quantity == quantity - (ReferenceEquals(environment.Current, environment.Shadow) ? 0 : 1), "Observed native quantity conservation");
			Require(result.Status == MagicCastingStatus.Succeeded, "Actual paid ethereal adapter cast: " + result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
				(overreach || before - actor.MagicResourceAmounts[native.Resource] == Math.Max(7, 50 / (8 - grade))), "Quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied,
				"Ethereal native application receipt");
			VerifySeeStockChannels(native, true);
			Console.WriteLine($"SEE-STOCK-paid=passed grade:{grade} cost:{before - actor.MagicResourceAmounts[native.Resource]} applied:{applied}");
			return result;
		}
		Cast(1); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(1800), "Grade1 three-hour increment");
		var old = Parent(); Cast(7);
		Require(Parent().LifetimeState!.Grade == 7 && scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(14400) &&
			!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old), "Native high-grade accumulation/cleanup");
		Cast(7); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(21600), "Native cap36");
		for (var grade = 1; grade <= 7; ++grade) Cast(grade, applied: false);
		environment.Current = environment.Shadow; var shadowQuantity = stack.Quantity; Cast(1, applied: false);
		Require(stack.Quantity == shadowQuantity, "Shadow consumed the dedicated component");
		environment.Current = environment.Silt; Ready(); var siltBalance = actor.MagicResourceAmounts[native.Resource];
		var silt = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me"));
		Require(silt.Status == MagicCastingStatus.Refused && silt.OperationId is null && actor.MagicResourceAmounts[native.Resource] == siltBalance &&
			stack.Quantity == shadowQuantity, "Silt prepayment refusal"); environment.Current = environment.Outside;

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
		var paidBalance = actor.MagicResourceAmounts[native.Resource];
		var retry = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me", OriginId: uncertain.OperationId));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == paidBalance &&
			Parent() == old && XElement.Parse(state.Operation(uncertain.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"Paid uncertainty replayed, refunded, replaced the cohort or sampled mastery");
		// Staff reconciles this inspected disposable fixture explicitly so later independent
		// acceptance can continue. The engine has not automatically resolved uncertainty.
		Require(casting.ReconcileOperation(staff, actor, uncertain.OperationId!.Value,
			"Owned disposable fixture: retained old ethereal cohort and paid reserve inspected").Allowed,
			"Explicit disposable-fixture reconciliation");
		Require(actor.MagicResourceAmounts[native.Resource] == paidBalance, "Fixture reconciliation refunded payment");
		foreach (var startShadow in new[] { false, true })
		{
			environment.Current = startShadow ? environment.Shadow : environment.Outside;
			Ready(); old = Parent(); var quantity = stack.Quantity; balance = actor.MagicResourceAmounts[native.Resource];
			callback = stage => { if (stage == "PaymentMutated") environment.Current = startShadow ? environment.Outside : environment.Shadow; };
			var drift = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
			Require(drift.Status == MagicCastingStatus.NeedsReview && actor.MagicResourceAmounts[native.Resource] == balance - 7 &&
				stack.Quantity == quantity && Parent() == old && XElement.Parse(state.Operation(drift.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
				"Post-debit environment drift guessed consumption or perception");
			FlushCasting(native); world.SaveManager.Flush();
			RunItemReaderProcess(new SeeTheUnbodiedStockReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
				scheduler.ScheduledExpiry(old)!.Value, old.Identity, drift.OperationId.Value, actor.MagicResourceAmounts[native.Resource], component.Id, stack.Quantity, false),
				"--see-unbodied-stock-reader");
			Require(casting.ReconcileOperation(staff, actor, drift.OperationId.Value, "Owned fixture retained old cohort, exact quantity and paid reserve inspected").Allowed,
				"Explicit environment drift fixture reconciliation");
		}
		environment.Current = environment.Outside;
		Ready(); native.Body.Drop(component, silent: true); var whole = NewComponent(5, 1); var wholeId = whole.Id;
		var wholeResult = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me"));
		Require(wholeResult.Status == MagicCastingStatus.Succeeded && whole.Deleted && world.Items.Get(wholeId) is null &&
			!native.Body.HeldOrWieldedItems.Any(x => x.Id == wholeId), "Native whole-item disappearance did not prove consumption: " + wholeResult.Message);
		world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.GameItems.Any(x => x.Id == wholeId), "Consumed item database row survived");
		Console.WriteLine("SEE-STOCK-whole-item=passed native-quantity-one Delete catalogue-body-database-disappearance");
		native.Body.Get(component, silent: true);
		var improver = ConfigurePracticeImprovement(native, database.ConnectionString, true);
		Require(improver.BuildingCommand(actor, new StringStack("interval 20")), "Native See practice interval");
		using (var db = NewIndependentContext(database.ConnectionString))
			SetPrivateField(native.Body, "_traits", db.Traits.AsNoTracking().Where(x => x.BodyId == native.Body.Id).ToArray()
				.Select(x => CastingRequired(world.Traits.Get(x.TraitDefinitionId)).LoadTrait(x, native.Body)).ToList());
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Rebound practice capacity");
		skill = world.Traits.GetByName("ARM02 Earth Proficiency")!; actor.SetTraitValue(skill, 30);
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		for (var grade = 1; grade <= 7; grade++)
		{
			clock.Advance(TimeSpan.FromMinutes(11)); Ready();
			var before = actor.MagicResourceAmounts[native.Resource]; var quantity = stack.Quantity; var parents = actor.EffectsOfType<MagicSpellParent>().Count();
			var quote = casting.Quote(new(actor, cap.Id, spell.Id, grade, grade > 1, "", MagicCastingMode.Practice));
			Require(quote.Allowed, "See practice quote: " + quote.Reason);
			MudSharp.Commands.Modules.MagicModule.MagicGeneric(actor,
				$"{cap.School.SchoolVerb} practice \"{spell.Name}\" grade {grade}{(grade > 1 ? " overreach" : "")} via {cap.Id}");
			var action = actor.EffectsOfType<MagicPracticeAction>().Single();
			Require(actor.MagicResourceAmounts[native.Resource] == before - quote.Invocation!.Costs.Single().Amount, "Native paid practice start");
			clock.Advance(TimeSpan.FromSeconds(30)); action.ExpireEffect(); action.ExpireEffect();
			Require(state.Operation(action.OperationId)!.Stage == "Completed" && actor.TraitRawValue(skill) == Math.Min(90, 30 + grade * 10) &&
				casting.Acquisition(actor, spell.Id)!.ControlledGrade == grade && actor.EffectsOfType<MagicSpellParent>().Count() == parents && stack.Quantity == quantity,
				"See cap90 paid effect-free practice failed at grade " + grade);
			Console.WriteLine($"SEE-STOCK-practice=passed grade:{grade} raw:{actor.TraitRawValue(skill)} cap:90 paid-once effect-free no-component-consumption accelerated-clock");
		}
		var final = Cast(7); var saved = Parent(); FlushCasting(native);
		RunItemReaderProcess(new SeeTheUnbodiedStockReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
			scheduler.ScheduledExpiry(saved)!.Value, saved.Identity, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource], component.Id, stack.Quantity, true),
			"--see-unbodied-stock-reader");
		Console.WriteLine("SEE-STOCK-acceptance=passed normal-stock-builder editable-reload Pierce80-opening30-cap90 grade7-paid-practice mapped-Standing self-filter low-high-paid cap36 strongest-grade nochange-no-mastery source-component-custody uncertainty fresh-reader expiry; full-world-and-hidden-target-visibility-unqualified");
		return 0;
	}

	private static int ReadSeeTheUnbodiedStock(string encoded)
	{
		var input = JsonSerializer.Deserialize<SeeTheUnbodiedStockReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString); var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock); var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor;
		ConfigureSeeStockWorld(native, database);
		if (input.Expire) ConfigurePracticeImprovement(native, database.ConnectionString, false);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var model in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking()
				.Where(x => x.Subcategory == ArmageddonSeeTheUnbodiedStock.Name || x.Subcategory == ArmageddonPierceConcealmentStock.Name))
			{
				var prog = new MudSharp.FutureProg.FutureProg(model, native.World); Require(prog.Compile(), "Fresh editable stock filter: " + prog.CompileError);
				if (!native.World.FutureProgs.Has(prog.Id)) ((All<IFutureProg>)native.World.FutureProgs).Add(prog);
			}
			var spells = (All<IMagicSpell>)native.World.MagicSpells;
			foreach (var existing in spells.ToArray()) spells.Remove(existing);
			foreach (var model in db.MagicSpells.AsNoTracking()) spells.Add(new MagicSpell(model, native.World));
		}
		var component = native.World.TryGetItem(input.Component, true)!;
		Require(component.GetItemType<IStackable>()!.Quantity == input.Quantity, "Fresh process component quantity changed");
		var source = (MagicSpell)native.World.MagicSpells.Get(input.Spell)!;
		Require(source.ReadyForGame && source.StockIdentity == ArmageddonSeeTheUnbodiedStock.Key && source.GradeProfile!.OpeningSkill == 30 && source.GradeProfile.Efficiency!.MinimumCost == 7, "Fresh editable stock readiness/profile");
		Require(source.SpellEffects.Single().SaveToXml().Element("SourceScope") is { } scope && (long)scope.Attribute("silt")! == 101 &&
			(long)scope.Attribute("shadow")! == 102 && ((IMagicSpellEffectLifetimePolicy)source.SpellEffects.Single()).LifetimePolicyError is null,
			"Fresh process source scope configuration did not reload");
		var scheduler = new EffectScheduler(native.World, clock);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler); PierceRealResource(native, database.ConnectionString);
		SpellDetectEtherealEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString)) actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.Identity == input.ParentIdentity && parent.LifetimeState == new MagicSpellLifetimeState(new(SeeTheUnbodiedStockGroup, 600, 36), 7) &&
			scheduler.ScheduledExpiry(parent) == input.Expiry && parent.SpellEffects.Single() is SpellDetectEtherealEffect &&
			actor.Effects.Contains(parent.SpellEffects.Single()), "Fresh ethereal parent/child/policy/grade/identity/deadline");
		VerifySeeStockChannels(native, true);
		var store = new MagicCastingStateStore(); var operation = store.Operation(input.Operation)!;
		if (input.Expire) Require(store.Acquisition(actor.Id, input.Spell) is { ControlledGrade: 7 } && actor.TraitRawValue(source.CastingTrait) == 90, "Fresh practiced grade7/raw90");
		Require(input.Expire ? operation.Stage == "Completed" && XElement.Parse(operation.Definition).Attribute("applied") is not null :
			operation.Stage == "NeedsReview" && XElement.Parse(operation.Definition).Attribute("masterySample") is null, "Persisted completion/quarantine report");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow,
			random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var operationReceipt = XElement.Parse(operation.Definition);
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, (int)operationReceipt.Attribute("grade")!, false,
			operationReceipt.Element("Targets")!.Value, OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed or refunded");
		Require(component.GetItemType<IStackable>()!.Quantity == input.Quantity, "Fresh retry consumed material again");
		if (!input.Expire)
		{
			Console.WriteLine("SEE-STOCK-reader-quarantine=passed fresh-process exact-quantity paid-reserve old-cohort no-replay no-consume no-refund no-mastery"); return 0;
		}
		clock.Advance(input.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) && !actor.EffectsOfType<SpellDetectEtherealEffect>().Any() &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance, "Expiry orphaned ethereal parent/child or refunded");
		VerifySeeStockChannels(native, false);
		Console.WriteLine("SEE-STOCK-reader=passed fresh-process native-typed-child identity grade7 policy exact-deadline no-replay no-refund expiry-removes-parent-child-perception");
		return 0;
	}
}
